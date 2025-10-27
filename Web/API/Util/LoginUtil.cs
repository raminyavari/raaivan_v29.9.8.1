using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Log;
using RaaiVan.Modules.Users;

namespace RaaiVan.Web.API
{
    public enum LoginType
    {
        None,
        Normal,
        TwoStep,
        SignUp,
        SignInWithGoogle,
        SSO,
        ChangePassword
    }

    public class LoginState
    {
        private static ConcurrentDictionary<Guid, DateTime> _LoggedInUsers = new ConcurrentDictionary<Guid, DateTime>();

        private static string get_redis_key(Guid userId)
        {
            return userId.ToString() + "_logged_in";
        }

        public static bool is_logged_in(Guid userId)
        {
            if (RedisAPI.Enabled)
            {
                DateTime? dt = RedisAPI.get_value<DateTime?>(get_redis_key(userId));
                return dt.HasValue && dt > DateTime.Now;
            }
            else return _LoggedInUsers.ContainsKey(userId) && _LoggedInUsers[userId] > DateTime.Now;
        }

        public static void logged_out(Guid userId)
        {
            if (RedisAPI.Enabled)
                RedisAPI.remove_key(get_redis_key(userId));
            else
            {
                DateTime time = DateTime.Now;
                _LoggedInUsers.TryRemove(userId, out time);
            }
        }

        public static void still_logged_in(HttpContext context, Guid? applicationId, Guid? userId, bool hasActivity)
        {
            if (!userId.HasValue) return;

            if (RedisAPI.Enabled)
                RedisAPI.set_value<DateTime>(get_redis_key(userId.Value), DateTime.Now.AddMinutes(5));
            else
                _LoggedInUsers[userId.Value] = DateTime.Now.AddMinutes(5);

            UsersController.set_last_activity_date(applicationId, userId.Value);

            if (hasActivity) set_session_last_activity_time(context, applicationId);
        }

        public static void set_session_expiration_time(HttpContext context, int expiresAfterNSeconds)
        {
            try
            {
                string variableName = "AuthCookieExpirationTime";

                if (context != null && context.Session != null && context.Session[variableName] == null)
                    context.Session[variableName] = DateTime.Now.AddSeconds(expiresAfterNSeconds);
            }
            catch { }
        }

        private static void set_session_last_activity_time(HttpContext context, Guid? applicationId)
        {
            try
            {
                string variableName = "LastActivityBasedExpirationTime";

                if (context != null && context.Session != null)
                {
                    context.Session[variableName] =
                        DateTime.Now.AddSeconds(RaaiVanSettings.MaxAllowedInactiveTimeInSeconds(applicationId));
                }
            }
            catch { }
        }

        public static bool session_expired(HttpContext context)
        {
            try
            {
                string variableName = "AuthCookieExpirationTime";
                string activityVariableName = "LastActivityBasedExpirationTime";

                bool expired = context.Session != null && context.Session[variableName] != null &&
                    (DateTime)context.Session[variableName] <= DateTime.Now;

                bool maxAllowedInactiveTimeExceeded = expired || (context.Session != null &&
                    context.Session[activityVariableName] != null &&
                    (DateTime)context.Session[activityVariableName] <= DateTime.Now);

                return expired || maxAllowedInactiveTimeExceeded;
            }
            catch { return false; }
        }

    }

    public class LoginUtil
    {
        private ParamsContainer ParamsContainer;
        private HttpContext Context;
        private Guid? ApplicationID;
        private bool IsAlreadyAuthenticated = false;
        private Guid? InvitationID;

        private Guid? _InvitationApplicationID;

        private Guid? InvitationApplicationID
        {
            get
            {
                if (!_InvitationApplicationID.HasValue && InvitationID.HasValue)
                {
                    _InvitationApplicationID =
                        UsersController.get_invitation_application_id(InvitationID.Value, checkIfNotUsed: true);
                    if (_InvitationApplicationID.HasValue) InvitationID = null;
                }

                return _InvitationApplicationID;
            }
        }

        private LoginType LoginType = LoginType.None;
        private bool RememberMe = false;

        private Guid? UserID;
        private User _User;

        private User User
        {
            get
            {
                if (_User == null && UserID.HasValue)
                {
                    _User = UsersController.get_user(ApplicationID, UserID.Value);
                    if (_User == null) UserID = null;
                }

                return _User;
            }
        }

        private Dictionary<string, object> Response = new Dictionary<string, object>();

        private bool? _IsSystemAdmin;
        private bool IsSystemAdmin
        {
            get
            {
                if (!_IsSystemAdmin.HasValue && UserID.HasValue)
                    _IsSystemAdmin = PublicMethods.is_system_admin(ApplicationID, UserID.Value, ignoreAuthentication: true);
                return _IsSystemAdmin.HasValue && _IsSystemAdmin.Value;
            }
        }

        public LoginUtil(ParamsContainer paramsContainer, Guid? invitationId)
        {
            ParamsContainer = paramsContainer;
            Context = paramsContainer?.Context;
            ApplicationID = paramsContainer?.ApplicationID;
            IsAlreadyAuthenticated = paramsContainer != null && paramsContainer.IsAuthenticated;
            InvitationID = invitationId;
        }

        private Dictionary<string, object> logged_in()
        {
            if (!UserID.HasValue) return null;

            int authCookieLifeTime = IsSystemAdmin ? RaaiVanSettings.AuthCookieLifeTimeForAdminInSeconds(ApplicationID) :
                RaaiVanSettings.AuthCookieLifeTimeInSeconds(ApplicationID);

            //FormsAuthentication.SetAuthCookie(username, rememberMe.HasValue && rememberMe.Value);
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, UserID.ToString(), DateTime.Now,
                DateTime.Now.AddSeconds(authCookieLifeTime), RememberMe, string.Empty);
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
            cookie.Expires = ticket.Expiration;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            cookie.Secure = RaaiVanSettings.UseSecureAuthCookie(ApplicationID);
            Context.Response.Cookies.Add(cookie);

            Dictionary<string, object> retDic = new Dictionary<string, object>()
            {
                { "Name", Base64.encode(cookie.Name) },
                { "Value", Base64.encode(cookie.Value) },
                { "Expires", cookie.Expires.ToString("r") }, //(r) equals this format: Mon, 13 Apr 2020 10:18:06 GMT
                { "Path", Base64.encode(cookie.Path) },
                { "Secure", cookie.Secure }
            };

            int sessionTimeout = (RaaiVanSettings.MaxAllowedInactiveTimeInSeconds(ApplicationID) / 60);
            Context.Session.Timeout = sessionTimeout == 0 ? 1440 : sessionTimeout; //1440 minutes equals a day

            AccessTokenList.new_token(Context); //When someone logs in, at least one token must exist

            LoginState.still_logged_in(Context, ApplicationID, UserID, hasActivity: true);

            LoginState.set_session_expiration_time(Context, authCookieLifeTime);

            return retDic;
        }

        private bool pre_login_check(Guid applicationId, ref string errorMessage)
        {
            if (!RaaiVanUtil.check_tenants())
            {
                errorMessage = Messages.TenantsCountExceededMaxAllowed.ToString();
                return false;
            }
            else if (!RaaiVanUtil.check_system_expiration())
            {
                errorMessage = Messages.SystemHasBeenExpired.ToString();
                return false;
            }
            else if (!RaaiVanUtil.check_users_count_lisence(applicationId))
            {
                errorMessage = Messages.ActiveUsersCountExceededMaxAllowed.ToString();
                return false;
            }

            return true;
        }

        public bool authenticate_user(string username, string password, string activeDirDomain,
            bool hasValidCaptcha, ref bool loggedInWithActiveDirectory, ref string failureText, ref int remainingLockoutTime)
        {
            if (!PublicMethods.check_sys_id()) return false;

            loggedInWithActiveDirectory = false;

            try
            {
                bool restrictToActiveDir = username.ToLower() != "admin" &&
                    ApplicationID.HasValue && RaaiVanSettings.Users.RestrictPasswordsToActiveDirectory(ApplicationID.Value);

                failureText = Messages.UserNameAndOrPasswordIsNotValid.ToString();

                if (username.ToLower() == "system") return false;

                User theUser = UsersController.locked(ApplicationID, username);

                if (theUser == null)
                    theUser = new User();
                else
                {
                    UserID = theUser.UserID;
                    _User = theUser;
                }

                if (theUser.UserID.HasValue)
                {
                    if (!theUser.IsApproved.HasValue || !theUser.IsApproved.Value)
                    {
                        failureText = "حساب کاربری شما غیر فعال شده است. برای فعال سازی آن به مدیریت سیستم مراجعه فرمایید";
                        return false;
                    }
                    else if (theUser.IsLockedOut.HasValue && theUser.IsLockedOut.Value && theUser.LastLockoutDate.HasValue)
                    {
                        TimeSpan offset = DateTime.Now - theUser.LastLockoutDate.Value;
                        int seconds = (int)offset.TotalSeconds;

                        if (hasValidCaptcha || seconds >= RaaiVanSettings.LoginLockoutDurationInSeconds(ApplicationID))
                            UsersController.unlock_user(ApplicationID, theUser.UserID.Value);
                        else
                        {
                            remainingLockoutTime = RaaiVanSettings.LoginLockoutDurationInSeconds(ApplicationID) - seconds;
                            failureText = Messages.YourAccountIsLockedOutForNextNSeconds.ToString();
                            return false;
                        }
                    }
                }

                bool userExistsInActiveDirectory = false;
                bool validated = false;

                bool prevalidated = !restrictToActiveDir && UserUtilities.validate_user(ApplicationID, username, password);

                if (!prevalidated && !string.IsNullOrEmpty(activeDirDomain))
                {
                    string errorMessage = string.Empty;

                    userExistsInActiveDirectory = UserUtilities.check_user_existence_in_active_directory(
                        ApplicationID, activeDirDomain, username, ref errorMessage);

                    if (!userExistsInActiveDirectory)
                    {
                        if (restrictToActiveDir)
                        {
                            failureText = errorMessage;
                            return false;
                        }
                        else errorMessage = string.Empty;
                    }

                    validated = loggedInWithActiveDirectory = userExistsInActiveDirectory &&
                        UserUtilities.is_active_directory_authenticated(ApplicationID, 
                        activeDirDomain, username, password, ref errorMessage);

                    /*
                    LogController.save_error_log(tenantId, null, "ActiveDirLogin",
                        "User: " + username + ", Pass: " + password + ", Result: " + validated.ToString().ToLower(), ModuleIdentifier.RV);
                    */

                    if (!validated && !string.IsNullOrEmpty(errorMessage))
                    {
                        failureText = errorMessage;
                        return false;
                    }

                    if (validated)
                    {
                        string newPass = password;
                        while (newPass.Length <= 5) newPass += password;

                        if (!theUser.UserID.HasValue)
                        {
                            Guid newUserId = Guid.NewGuid();

                            if (!RaaiVanUtil.new_user(ApplicationID, newUserId, username, newPass, passAutoGenerated: true))
                            {
                                failureText = "خطا در ایجاد کاربر جدید از سرویس دهنده شبکه، در اولین ورود کاربر";
                                return false;
                            }

                            theUser.UserID = newUserId;
                        }
                        else if (theUser.UserID.HasValue &&
                            !UsersController.set_password(ApplicationID, theUser.UserID.Value, newPass, true, false, ref errorMessage))
                        {
                            failureText = "خطا در بروز رسانی رمز ورود از طریق سرویس دهنده شبکه";
                            return false;
                        }
                    }
                }

                bool? membershipValidated = null;

                if (theUser.UserID.HasValue && !(restrictToActiveDir && !userExistsInActiveDirectory) && (
                    prevalidated || validated || (
                        (membershipValidated = UserUtilities.validate_user(ApplicationID, username, password)).HasValue &&
                        membershipValidated.Value
                    )
                ))
                    return true;
                else if (theUser.UserID.HasValue && membershipValidated.HasValue && !membershipValidated.Value)
                {
                    //Save Log
                    User usr = UsersController.locked(ApplicationID, username);
                    bool locked = usr != null && usr.IsLockedOut.HasValue && usr.IsLockedOut.Value;

                    if (locked)
                    {
                        LogController.save_log(ApplicationID, new Log()
                        {
                            UserID = theUser.UserID.Value,
                            Date = DateTime.Now,
                            HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                            HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                            Action = Modules.Log.Action.UserLockedOut,
                            SubjectID = theUser.UserID.Value,
                            ModuleIdentifier = ModuleIdentifier.USR
                        });
                    }
                    //end of Save Log
                }
            }
            catch (Exception ex) { failureText = ex.ToString(); }

            return false;
        }

        private void init_user_application()
        {
            if (!RaaiVanSettings.SAASBasedMultiTenancy || User == null || !User.UserID.HasValue) return;

            if (InvitationApplicationID.HasValue)
                GlobalController.add_user_to_application(InvitationApplicationID.Value, User.UserID.Value);

            List<Application> apps = GlobalController.get_user_applications(User.UserID.Value);

            if (apps != null && apps.Count == 1)
                PublicMethods.set_current_application(apps[0]);
            else if (apps != null && InvitationApplicationID.HasValue &&
                apps.Any(a => a.ApplicationID == InvitationApplicationID))
            {
                PublicMethods.set_current_application(
                    apps.Where(a => a.ApplicationID == InvitationApplicationID).FirstOrDefault());
            }

            if (RaaiVanSettings.SAASBasedMultiTenancy)
            {
                bool wizard = false;

                if (!IsAlreadyAuthenticated)
                {
                    if (apps == null ||
                        apps.Where(a => !InvitationApplicationID.HasValue || a.ApplicationID != InvitationApplicationID).Count() == 0)
                        wizard = true;
                    /*
                    if (apps == null || apps.Count == 0)
                        wizard = true;
                    else if (!apps.Any(ap => ap.CreatorUserID == User.UserID))
                        wizard = !PublicMethods.get_dic_value<bool>(User.Settings, "first_team_wizard", defaultValue: false);
                    //first_team_wizard: true => the user has passed the wizard at least one time
                    */
                }

                if (wizard) Response["RedirectToOnboarding"] = true;
            }
        }

        private void after_login_procedures(bool loggedInWithActiveDirectory)
        {
            if (!UserID.HasValue) return;
            
            if (!IsSystemAdmin && (RaaiVanSettings.Users.ForceChangeFirstPassword(ApplicationID) ||
                RaaiVanSettings.Users.PasswordLifeTimeInDays(ApplicationID) > 0))
            {
                bool passwordChangeNeeded = false;
                bool firstPassword = false, passwordExpired = false;

                if (!loggedInWithActiveDirectory)
                {
                    List<Password> lasts = UsersController.get_last_passwords(ApplicationID, UserID.Value, autoGenerated: null, count: 1);

                    if (lasts.Count == 1 && lasts[0].AutoGenerated.HasValue &&
                        lasts[0].AutoGenerated.Value && RaaiVanSettings.Users.ForceChangeFirstPassword(ApplicationID))
                        passwordChangeNeeded = firstPassword = true;
                    else if (RaaiVanSettings.Users.PasswordLifeTimeInDays(ApplicationID) > 0)
                    {
                        if (lasts.Count == 0) passwordChangeNeeded = passwordExpired = true;
                        else
                        {
                            DateTime? pd = UsersController.get_last_password_date(ApplicationID, UserID.Value);

                            if (!pd.HasValue ||
                                (DateTime.Now - pd.Value).Days > RaaiVanSettings.Users.PasswordLifeTimeInDays(ApplicationID))
                                passwordChangeNeeded = passwordExpired = true;
                        }
                    }

                    if (passwordChangeNeeded)
                        RaaiVanUtil.password_change_needed(HttpContext.Current, firstPassword, passwordExpired);
                }
            }

            init_user_application();
        }

        private bool login_normal(string username, string password, string activeDirDomain,
            bool hasValidCaptcha, ref string failureText, ref int remainingLockoutTime, ref bool stepTwoNeeded)
        {
            if (!RaaiVanSettings.SAASBasedMultiTenancy && !ApplicationID.HasValue) return false;

            /* Usernames like 'username@domain' or 'domain\username' or 'domain/username' are not allowed */
            if (!string.IsNullOrEmpty(username) && !RaaiVanSettings.SAASBasedMultiTenancy)
            {
                if (username.IndexOf("@") >= 0) username = username.Substring(0, username.IndexOf("@"));
                if (username.LastIndexOf("/") >= 0) username = username.Substring(username.LastIndexOf("/") + 1);
                if (username.LastIndexOf("\\") >= 0) username = username.Substring(username.LastIndexOf("\\") + 1);
            }

            UserID = UsersController.get_user_id(ApplicationID, username);

            if (!RaaiVanSettings.SAASBasedMultiTenancy && username.ToLower() == "admin") activeDirDomain = string.Empty;
            else if (ApplicationID.HasValue && !pre_login_check(ApplicationID.Value, ref failureText)) return false;

            bool loggedInWithActiveDirectory = false;

            if (!authenticate_user(username, password, activeDirDomain, hasValidCaptcha,
                ref loggedInWithActiveDirectory, ref failureText, ref remainingLockoutTime))
            {
                Response["RemainingLockoutTime"] = remainingLockoutTime;
                return false;
            }
            else if (!UserID.HasValue) UserID = UsersController.get_user_id(ApplicationID, username);

            if (!UserID.HasValue) return false;

            //Two step authentication
            if (RaaiVanSettings.Users.EnableTwoStepAuthenticationViaEmail(ApplicationID) ||
                RaaiVanSettings.Users.EnableTwoStepAuthenticationViaSMS(ApplicationID))
            {
                Guid? mainEmailId = !RaaiVanSettings.Users.EnableTwoStepAuthenticationViaEmail(ApplicationID) ? null :
                    UsersController.get_main_email(ApplicationID, UserID.Value);
                Guid? mainPhoneId = !RaaiVanSettings.Users.EnableTwoStepAuthenticationViaSMS(ApplicationID) ? null :
                    UsersController.get_main_phone(ApplicationID, UserID.Value);

                EmailAddress email = !mainEmailId.HasValue ? null :
                    UsersController.get_email_address(ApplicationID, mainEmailId.Value);
                PhoneNumber number = !mainPhoneId.HasValue ? null :
                    UsersController.get_phone_number(ApplicationID, mainPhoneId.Value);

                string emailAddress = email == null ? null : email.Address;
                string phoneNumber = number == null ? null : number.Number;

                if (!string.IsNullOrEmpty(emailAddress) || !string.IsNullOrEmpty(phoneNumber))
                {
                    TwoStepAuthenticationToken twoStepToken = new TwoStepAuthenticationToken(ApplicationID, UserID.Value,
                        !loggedInWithActiveDirectory, emailAddress, phoneNumber);

                    if (twoStepToken.send_code())
                    {
                        failureText = "{\"TwoStepAuthentication\":" + true.ToString().ToLower() +
                            ",\"Data\":" + twoStepToken.toJson() + "}";

                        stepTwoNeeded = true;
                    }
                    else failureText = Messages.SendingTwoStepAuthenticationCodeFailed.ToString();

                    return false;
                }
            }
            //end of Two step authentication

            if (RaaiVanSettings.PreventConcurrentSessions(ApplicationID) && LoginState.is_logged_in(UserID.Value))
            {
                failureText = Messages.YouAreAlreadyLoggedIn.ToString();
                return false;
            }

            after_login_procedures(loggedInWithActiveDirectory);

            return true;
        }

        public string login_normal(string username, string password, string activeDirDomain, bool? rememberMe, string captcha)
        {
            LoginType = LoginType.Normal;
            RememberMe = rememberMe.HasValue && rememberMe.Value;

            string failureText = string.Empty;
            int remainingLockoutTime = 0;
            bool stepTwoNeeded = false;

            bool hasValidCaptcha = !string.IsNullOrEmpty(captcha) && Captcha.check(HttpContext.Current, captcha);

            if (!string.IsNullOrEmpty(captcha) && !hasValidCaptcha)
                failureText = Messages.CaptchaIsNotValid.ToString();

            bool result = string.IsNullOrEmpty(failureText) && 
                login_normal(username, password, activeDirDomain, hasValidCaptcha, 
                ref failureText, ref remainingLockoutTime, ref stepTwoNeeded);

            if (!result && !stepTwoNeeded && UserID.HasValue && remainingLockoutTime <= 0)
            {
                LogController.save_log(ApplicationID, new Log()
                {
                    UserID = UserID,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.Login_Failed,
                    SubjectID = UserID,
                    Info = "{\"UserName\":\"" + username + "\"" +
                        ",\"Error\":\"" + Base64.encode(failureText) + "\"" +
                        //",\"RemainingLockoutTime\":" + remainingLockoutTime.ToString() +
                        "}",
                    ModuleIdentifier = ModuleIdentifier.RV
                });
            }
            else
            {
                /*
                Log lg = LogController.get_logs(ApplicationID, new List<Guid>() { UserID.Value },
                    new List<Modules.Log.Action>() { Modules.Log.Action.Login }, null, null, null, 1).FirstOrDefault();

                if (lg == null && UserID.HasValue)
                {
                    GlobalController.set_variable(ApplicationID, UserID.ToString() + "_LastVersionSeen",
                        "{\"Version\":\"" + PublicMethods.SystemVersion + "\",\"Tour\":\"Seen\"}", UserID.Value);
                }
                */
            }

            return create_response(loginResult: result, failureText: failureText);
        }

        public string login_two_step(string token, long? code, bool? rememberMe)
        {
            LoginType = LoginType.TwoStep;
            RememberMe = rememberMe.HasValue && rememberMe.Value;

            bool disposed = false;

            TwoStepAuthenticationToken twoStepToken = !code.HasValue ? null :
                (TwoStepAuthenticationToken)TwoStepAuthenticationToken.validate(token, code.Value, ref disposed);

            string failureText = string.Empty;
            bool result = false;

            if (twoStepToken == null)
            {
                Response["CodeDisposed"] = disposed;
                failureText = Messages.AuthenticationCodeDidNotMatch.ToString();
                result = false;
            }
            else
            {
                UserID = twoStepToken.UserID;
                after_login_procedures(loggedInWithActiveDirectory: !twoStepToken.WasNormalUserPassLogin);
                result = true;
            }

            return create_response(loginResult: result, failureText: failureText);
        }

        public string login_via_signup(string username, string password, 
            string email, string phone, string firstName, string lastName, bool login)
        {
            LoginType = LoginType.SignUp;

            string failureText = string.Empty;

            bool result = finalize_user_sign_up(
                username: username,
                email: email,
                phone: phone,
                firstName: firstName,
                lastName: lastName,
                password: password,
                login: login,
                failureText: ref failureText);

            return create_response(loginResult: login, failureText: failureText, actionResult: result);
        }

        private bool sso_auto_redirect(string loginUrl, bool skipRedirect)
        {
            if (RaaiVanSettings.SSO.AutoRedirect(ApplicationID))
            {
                if (!skipRedirect) ParamsContainer.redirect(loginUrl);
                return true;
            }
            else return false;
        }

        private bool login_sso(bool skipRedirect, ref bool shouldRedirect, ref string failureText)
        {
            if (ApplicationID.HasValue && !pre_login_check(ApplicationID.Value, ref failureText)) return false;

            string loginUrl = Modules.Jobs.SSO.get_login_url(ApplicationID);

            if (string.IsNullOrEmpty(loginUrl)) return false;

            string ticket = Modules.Jobs.SSO.get_ticket(ApplicationID, Context);

            string username = string.Empty;

            if (string.IsNullOrEmpty(ticket) ||
                !Modules.Jobs.SSO.validate_ticket(ApplicationID, HttpContext.Current, ticket, ref username))
                return (shouldRedirect = sso_auto_redirect(loginUrl, skipRedirect));

            if (string.IsNullOrEmpty(username))
            {
                failureText = Messages.LoginFailed.ToString();
                return false;
            }

            UserID = UsersController.get_user_id(ApplicationID, username);

            if (!UserID.HasValue)
            {
                Guid newUserId = Guid.NewGuid();

                if (RaaiVanUtil.new_user(ApplicationID, newUserId, username, username, passAutoGenerated: false))
                    UserID = newUserId;
            }

            if (!UserID.HasValue)
            {
                failureText = Messages.UserCreationFailed.ToString();
                return false;
            }
            else
            {
                after_login_procedures(loggedInWithActiveDirectory: false);
                return true;
            }
        }

        public bool login_sso(bool skipRedirect, ref bool shouldRedirect, ref Dictionary<string, object> res)
        {
            LoginType = LoginType.SSO;

            string failureText = string.Empty;
            bool result = login_sso(skipRedirect, ref shouldRedirect, ref failureText);
            res = PublicMethods.fromJSON(create_response(result, failureText));
            return result;
        }

        public async void login_with_google(string token, string email, string firstName, 
            string lastName, string googleId, string imageUrl, Action<string> callback)
        {
            LoginType = LoginType.SignInWithGoogle;

            if (!RaaiVanSettings.UserSignUp(ApplicationID) && !RaaiVanSettings.SignUpViaInvitation(ApplicationID))
            {
                callback("{\"ErrorText\":\"" + Messages.AccessDenied + "\"}");
                return;
            }

            //this is temporary and we only need it because google has block server-side checks from Iran!
            bool ignoreServerSideCheck = !string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(email) &&
                !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(googleId);

            string failureText = string.Empty;

            if (!ignoreServerSideCheck)
            {
                Google.Apis.Auth.GoogleJsonWebSignature.Payload payload = null;

                try
                {
                    payload = await Google.Apis.Auth.GoogleJsonWebSignature.ValidateAsync(token);
                }
                catch (Exception ex)
                {
                    string strEx = ex.ToString();
                }

                if (payload == null)
                {
                    callback(create_response(loginResult: false, failureText: Messages.TokenValidationFailed.ToString()));
                    return;
                }

                email = payload.Email;
                firstName = string.IsNullOrEmpty(payload.Name) ? payload.GivenName : payload.Name;
                lastName = string.IsNullOrEmpty(payload.FamilyName) ? payload.GivenName : payload.FamilyName;

                if (string.IsNullOrEmpty(email))
                {
                    callback(create_response(loginResult: false, failureText: Messages.EmailIsNotValid.ToString()));
                    return;
                }
                else if (!payload.EmailVerified)
                {
                    callback(create_response(loginResult: false, failureText: Messages.EmailIsNotVerified.ToString()));
                    return;
                }
            }

            EmailAddress address = UsersController.get_email_owners(ApplicationID, new List<string>() { email })
               .Where(e => e.IsMain.HasValue && e.IsMain.Value).FirstOrDefault();

            bool result = false;

            string responseText = string.Empty;
            
            //if address is null, then this is a sign up
            if (address == null)
            {
                if (string.IsNullOrEmpty(firstName)) firstName = email.Substring(0, email.IndexOf('@'));
                if (string.IsNullOrEmpty(lastName)) lastName = email.Substring(0, email.IndexOf('@'));

                result = finalize_user_sign_up(username: PublicMethods.random_string(8).ToLower(),
                    email: email,
                    phone: null,
                    firstName: firstName,
                    lastName: lastName,
                    password: PublicMethods.random_string(10),
                    login: true,
                    failureText: ref failureText);
            }
            else
            {
                UserID = address.UserID;
                after_login_procedures(loggedInWithActiveDirectory: false);
                result = true;
            }

            callback(create_response(loginResult: result, failureText: failureText));
        }

        public string login_via_change_password(Guid? userId, string password, bool login)
        {
            LoginType = LoginType.ChangePassword;

            UserID = userId;

            string failureText = string.Empty;

            bool succeed = UserID.HasValue && UsersController.set_password(ApplicationID,
                UserID.Value, password, ignorePasswordPolicy: false, autoGenerated: false, ref failureText);

            if (succeed) RaaiVanUtil.password_change_not_needed(Context);

            if (succeed && login)
                after_login_procedures(loggedInWithActiveDirectory: false);

            return create_response(loginResult: login, failureText: failureText, actionResult: succeed);
        }

        private void user_sign_up_is_complete(bool succeed, bool login, ref string failureText)
        {
            if (succeed && login)
                after_login_procedures(loggedInWithActiveDirectory: false);
            else if (!succeed)
                failureText = Messages.UserCreationFailed.ToString();
        }

        private bool finalize_user_sign_up(string username, string email, string phone, string firstName,
            string lastName, string password, bool? login, ref string failureText)
        {
            if (!string.IsNullOrEmpty(email) &&
                UsersController.get_email_owners(ApplicationID, new List<string>() { email })
                    .Any(e => e.IsMain.HasValue && e.IsMain.Value))
            {
                failureText = Messages.EmailAlreadyExists.ToString();
                return false;
            }
            else if (!string.IsNullOrEmpty(phone) &&
                UsersController.get_phone_owners(ApplicationID, new List<string>() { phone })
                    .Any(e => e.IsMain.HasValue && e.IsMain.Value))
            {
                failureText = Messages.PhoneNumberAlreadyExists.ToString();
                return false;
            }

            User newUser = new User()
            {
                UserID = Guid.NewGuid(),
                UserName = username,
                FirstName = firstName,
                LastName = lastName,
                Password = password,
                Emails = string.IsNullOrEmpty(email) ? new List<EmailAddress>() :
                    new List<EmailAddress>() { new EmailAddress() { Address = email } },
                PhoneNumbers = string.IsNullOrEmpty(phone) ? new List<PhoneNumber>() :
                    new List<PhoneNumber>() { new PhoneNumber() { Number = phone } }
            };

            if (string.IsNullOrEmpty(newUser.UserName) || string.IsNullOrEmpty(newUser.FirstName) ||
                string.IsNullOrEmpty(newUser.LastName) || string.IsNullOrEmpty(newUser.Password))
            {
                failureText = Messages.InvalidInput.ToString();
                return false;
            }
            else if (UsersController.get_user(ApplicationID, newUser.UserName) != null)
            {
                failureText = Messages.UserNameAlreadyExists.ToString();
                return false;
            }

            Guid? appId = ApplicationID.HasValue ? ApplicationID : InvitationApplicationID;

            bool succeed = UsersController.create_user(appId, newUser, passAutoGenerated: false);

            if (succeed)
            {
                UserID = newUser.UserID;
                _User = newUser;
            }

            user_sign_up_is_complete(succeed: succeed, login: login.HasValue && login.Value, failureText: ref failureText);

            return succeed;
        }

        private string get_login_message()
        {
            User usr = null;

            if (string.IsNullOrEmpty(RaaiVanSettings.LoginMessage(ApplicationID)) || !UserID.HasValue || 
                (usr = UsersController.get_user(ApplicationID, UserID.Value)) == null) return string.Empty;

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["firstname"] = string.IsNullOrEmpty(usr.FirstName) ? string.Empty : usr.FirstName;
            dic["lastname"] = string.IsNullOrEmpty(usr.LastName) ? string.Empty : usr.LastName;
            dic["username"] = string.IsNullOrEmpty(usr.UserName) ? string.Empty : usr.UserName;
            dic["fullname"] = (dic["firstname"] + " " + dic["lastname"]).Trim();
            dic["pdate"] = GenericDate.get_local_date(DateTime.Now, true);

            return Expressions.replace(RaaiVanSettings.LoginMessage(ApplicationID), ref dic, Expressions.Patterns.AutoTag);
        }

        private ArrayList get_last_logins()
        {
            if (RaaiVanSettings.InformLastLogins(ApplicationID) <= 0 || !UserID.HasValue) return new ArrayList();

            List<Log> logs = LogController.get_logs(ApplicationID, new List<Guid>() { UserID.Value },
                new List<Modules.Log.Action>() { Modules.Log.Action.Login, Modules.Log.Action.Login_Failed },
                null, null, null, RaaiVanSettings.InformLastLogins(ApplicationID) + 1);

            if (logs.Count > 0) logs.RemoveAt(0);

            ArrayList retList = new ArrayList();

            logs.ForEach(l => retList.Add(PublicMethods.fromJSON(l.toJson())));

            return retList;
        }

        private string create_response(bool loginResult, string failureText, bool? actionResult = null)
        {
            if (!actionResult.HasValue) actionResult = loginResult;

            if (actionResult.HasValue && actionResult.Value)
            {
                Response["Succeed"] = Messages.OperationCompletedSuccessfully.ToString();

                if (loginResult)
                {
                    Response["AuthCookie"] = logged_in();

                    List<LoginType> lst = new List<LoginType>() {
                        LoginType.Normal,
                        LoginType.TwoStep,
                        LoginType.SSO
                    };

                    if (User != null && User.UserID.HasValue && lst.Any(t => t == LoginType))
                    {
                        Response["LoginMessage"] = Base64.encode(get_login_message());
                        Response["LastLogins"] = get_last_logins();
                    }

                    LogController.save_log(ApplicationID, new Log()
                    {
                        UserID = User.UserID,
                        Date = DateTime.Now,
                        HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                        HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                        Action = Modules.Log.Action.Login,
                        SubjectID = User.UserID,
                        ModuleIdentifier = ModuleIdentifier.USR
                    });
                }

                if (User != null && User.UserID.HasValue)
                    Response["User"] = User.toJson(ApplicationID, profileImageUrl: true);
            }
            else
            {
                Response["ErrorText"] = Response["LoginErrorMessage"] = 
                    string.IsNullOrEmpty(failureText) ? Messages.OperationFailed.ToString() :
                    failureText[0] == '{' ? (object)PublicMethods.fromJSON(failureText) : failureText;
            }

            return PublicMethods.toJSON(Response);
        }
    }
}