using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.IO;
using RaaiVan.Modules.RaaiVanConfig;
using System.Collections;

namespace RaaiVan.Modules.GlobalUtilities
{
    public enum RVSettingsItem
    {
        UseLocalVariables,
        IsDevEnvironment,

        SAASBasedMultiTenancy,
        ReferenceTenantID,
        NodeTypeIdentityFormID,

        USBToken,

        OrganizationName,
        OrganizationLogo,

        FavIconName,
        LogoURL,
        LogoMiniURL,

        StoragePath,

        CephStorage,
        CephAccessKey,
        CephSecretKey,
        CephURL,
        CephBucket,

        RedisHosts,
        RedisPassword,

        ServiceUnavailable,
        IgnoreReturnURLOnLogin,
        DefaultPrivacy,
        DefaultLang,
        ShowSystemVersion,

        EnableThemes,
        DefaultTheme,
        BackgroundColor,
        ColorfulBubbles,

        LoginPageModel,
        LoginPageInfo,
        UserSignUp,
        SignUpViaInvitation,
        AllowNotAuthenticatedUsers,
        UserNamePattern,
        HideUserNames,
        LoginMessage,

        InformLastLogins,
        AuthCookieLifeTime,
        AuthCookieLifeTimeForAdmin,
        MaxAllowedInactiveTime,
        AllowedConsecutiveFailedLoginAttempts,
        LoginLockoutDuration,
        ReplayAttackQueueLength,
        UseSecureAuthCookie,
        PreventConcurrentSessions,
        FileEncryption,

        IgnoreActiveDirectoryUserCheck,
        RestrictPasswordsToActiveDirectory,
        EnableTwoStepAuthenticationViaEmail,
        EnableTwoStepAuthenticationViaSMS,
        TwoStepAuthenticationTimeoutInSeconds,
        ForceChangeFirstPassword,
        PasswordLifeTimeInDays,
        NotAvailablePreviousPasswordsCount,
        PasswordPolicyMinLength,
        PasswordPolicyNewCharacters,
        PasswordPolicyUpperLower,
        PasswordPolicyNonAlphabetic,
        PasswordPolicyNumber,
        PasswordPolicyNonAlphaNumeric,

        ReauthenticationForSettingsAdminPage,
        ReauthenticationForUsersAdminPage,

        CaptchaURL,
        CaptchaSiteKey,
        CaptchaSecretKey,
        CaptchaValidationURL,

        GATrackingID,
        GoogleSignInClientID,

        SSOEnabled,
        SSOAutoRedirect,
        SSOLoginTitle,
        SSOTicketVariableName,
        SSOLoginURL,
        SSOValidateURL,
        SSOXMLUserNameTag,
        SSOJSONUserNamePath,
        SSOInvalidTicketCode,

        DefaultAdditionalIDPattern,

        SystemName,
        SystemTitle,
        SystemURL,

        DailyDatabaseBackup,
        RemoveOldDatabaseBackups,

        SystemEmailAddress,
        SystemEmailDisplayName,
        SystemEmailPassword,
        SystemEmailSMTPAddress,
        SystemEmailSMTPPort,
        SystemEmailUseSSL,
        SystemEmailTimeout,
        SystemEmailSubject,

        DefaultCNExtensions,
        PersonalPagePriorities,

        AlertKnowledgeExpirationInDays,

        MinAcceptableExpertiseReferralsCount,
        MinAcceptableExpertiseConfirmsPercentage,

        NotificationsSeenTimeout,
        NotificationsUpdateInterval,

        RealTime,
        Explorer,

        GhostScriptDirectory,

        FileContents,
        FileContentExtractionInterval,
        FileContentExtractionBatchSize,
        FileContentExtractionStartTime,
        FileContentExtractionEndTime,

        Index,
        IndexUpdateInterval,
        IndexUpdateBatchSize,
        IndexUpdateStartTime,
        IndexUpdateEndTime,
        IndexUpdatePriorities,
        IndexUpdateUseRam,
        CheckPermissionsForSearchResults,

        Solr,
        SolrURL,
        SolrCollectionName,
        SolrUsername,
        SolrPassword,

        EmailQueue,
        EmailQueueInterval,
        EmailQueueBatchSize,
        EmailQueueStartTime,
        EmailQueueEndTime,

        SchedulerUsername,

        Recommender,
        RecommenderURL,
        RecommenderUsername,
        RecommenderPassword,

        UsePostgreSQL
    }

    public static class RaaiVanSettings
    {
        private static Dictionary<Guid, Dictionary<RVSettingsItem, string>> SettingsDic =
            new Dictionary<Guid, Dictionary<RVSettingsItem, string>>();

        private static string get_value(Guid? applicationId, RVSettingsItem name, string defaultValue = "")
        {
            if (!applicationId.HasValue) applicationId = Guid.Empty;

            if (SettingsDic.ContainsKey(applicationId.Value) && SettingsDic[applicationId.Value].ContainsKey(name))
                return string.IsNullOrEmpty(SettingsDic[applicationId.Value][name]) ? defaultValue : SettingsDic[applicationId.Value][name];
            else
            {
                Dictionary<RVSettingsItem, string> dic = get_value(applicationId.Value, new List<RVSettingsItem>() { name });
                string value = !dic.ContainsKey(name) ? string.Empty : dic[name];
                return string.IsNullOrEmpty(value) ? defaultValue : value;
            }
        }

        public static Dictionary<RVSettingsItem, string> get_value(Guid applicationId, List<RVSettingsItem> names)
        {
            Dictionary<RVSettingsItem, string> items = GlobalController.get_system_settings(applicationId, names);

            List<RVSettingsItem> theNames = names != null && names.Count > 0 ? names :
                Enum.GetValues(typeof(RVSettingsItem)).Cast<RVSettingsItem>().ToList();

            foreach (RVSettingsItem n in theNames)
            {
                if (!items.ContainsKey(n) || string.IsNullOrEmpty(items[n]))
                {
                    string env = n == RVSettingsItem.UseLocalVariables || RaaiVanSettings.UseLocalVariables ? string.Empty : 
                        PublicMethods.get_environment_variable("rv_" + n.ToString());
                    items[n] = string.IsNullOrEmpty(env) ? ConfigurationManager.AppSettings[n.ToString()] : env;
                }
            }

            foreach (RVSettingsItem n in items.Keys) save_value(applicationId, n, items[n]);
            return items;
        }

        private static void save_value(Guid applicationId, RVSettingsItem name, string value)
        {
            lock (SettingsDic)
            {
                if (!SettingsDic.ContainsKey(applicationId))
                    SettingsDic[applicationId] = new Dictionary<RVSettingsItem, string>();
                SettingsDic[applicationId][name] = string.IsNullOrEmpty(value) ? string.Empty : value;
            }
        }

        public static bool set_value(Guid applicationId, Dictionary<RVSettingsItem, string> items, Guid currentUserId)
        {
            if (applicationId != Guid.Empty)
            {
                bool result = GlobalController.set_system_settings(applicationId, items, currentUserId);
                if (!result) return false;
            }

            foreach (RVSettingsItem itm in items.Keys)
                save_value(applicationId, itm, items[itm]);

            return true;
        }

        public static bool set_value(Guid applicationId, RVSettingsItem name, string value, Guid currentUserId)
        {
            Dictionary<RVSettingsItem, string> items = new Dictionary<RVSettingsItem, string>();
            items[name] = value;
            return set_value(applicationId, items, currentUserId);
        }

        private static DateTime get_time(Guid applicationId, RVSettingsItem name, DateTime defaultValue)
        {
            string val = get_value(applicationId, name);
            DateTime tm = defaultValue;

            if (!string.IsNullOrEmpty(val))
            {
                string[] t = val.Split(':');
                int n = 0;
                if (t.Length >= 2 && int.TryParse(t[0], out n) && n >= 0 && n <= 23 && int.TryParse(t[1], out n) && n >= 0 && n <= 59)
                {
                    tm = new DateTime(defaultValue.Year, defaultValue.Month, defaultValue.Day,
                        int.Parse(t[0]), int.Parse(t[1]), 0);
                }
            }

            return tm;
        }

        private static int get_int(Guid? applicationId, RVSettingsItem name, int defaultValue, int? minValue)
        {
            int val = 0;
            if (!int.TryParse(get_value(applicationId, name), out val)) val = defaultValue;
            return minValue.HasValue ? Math.Max(val, minValue.Value) : val;
        }

        public static bool UseLocalVariables
        {
            get
            {
                return get_value(null, RVSettingsItem.UseLocalVariables).ToLower() == "true";
            }
        }

        public static bool IsDevEnvironment
        {
            get { return get_value(null, RVSettingsItem.IsDevEnvironment).ToLower() == "true"; }
        }

        public static bool SAASBasedMultiTenancy
        {
            get
            {
                return get_value(null, RVSettingsItem.SAASBasedMultiTenancy).ToLower() == "true";
            }
        }

        public static Guid? ReferenceTenantID {
            get
            {
                return !SAASBasedMultiTenancy ? null :
                    PublicMethods.parse_guid(get_value(null, RVSettingsItem.ReferenceTenantID));
            }
        }

        public static Guid? NodeTypeIdentityFormID
        {
            get
            {
                return !ReferenceTenantID.HasValue ? null : 
                    PublicMethods.parse_guid(get_value(null, RVSettingsItem.NodeTypeIdentityFormID));
            }
        }

        public static bool USBToken
        {
            get
            {
                return get_value(null, RVSettingsItem.USBToken).ToLower() == "true";
            }
        }

        public static class Organization
        {
            public static string Name(Guid applicationId)
            {
                return get_value(applicationId, RVSettingsItem.OrganizationName);
            }

            private static Dictionary<Guid, bool> _LogoInited = new Dictionary<Guid, bool>();
            private static Dictionary<Guid, System.Drawing.Image> _Logo = new Dictionary<Guid, System.Drawing.Image>();
            public static System.Drawing.Image Logo(Guid applicationId)
            {
                if (!_LogoInited.ContainsKey(applicationId) && !_Logo.ContainsKey(applicationId))
                {
                    _LogoInited[applicationId] = true;

                    try
                    {
                        string address = PublicMethods.map_path("~/" + get_value(applicationId, RVSettingsItem.OrganizationLogo));
                        if (File.Exists(address)) _Logo[applicationId] = System.Drawing.Image.FromFile(address);
                    }
                    catch { }
                }

                return _Logo.ContainsKey(applicationId) ? _Logo[applicationId] : null;
            }
        }

        public static string FavIconName
        {
            get { return get_value(null, RVSettingsItem.FavIconName, "RaaiVanFav"); }
        }

        public static string LogoURL
        {
            get { return get_value(null, RVSettingsItem.LogoURL, "../../Images/RaaiVanLogo.png"); }
        }

        public static string LogoMiniURL
        {
            get { return get_value(null, RVSettingsItem.LogoMiniURL, "../../Images/RaaiVan-Mini.png"); }
        }

        private static string _StoragePath;

        public static string StoragePath
        {
            get
            {
                if (string.IsNullOrEmpty(_StoragePath))
                {
                    string path = get_value(null, RVSettingsItem.StoragePath);
                    if (string.IsNullOrEmpty(path)) path = System.Web.Hosting.HostingEnvironment.MapPath("~/");
                    if (path[path.Length - 1] == '\\') path = path.Substring(0, path.Length - 1);
                    _StoragePath = path;
                }

                return _StoragePath;
            }
        }

        public static class CephStorage
        {
            public static bool Enabled
            {
                get
                {
                    return get_value(null, RVSettingsItem.CephStorage).ToLower() != "false" &&
                        !string.IsNullOrEmpty(URL) && !string.IsNullOrEmpty(AccessKey) && !string.IsNullOrEmpty(SecretKey);
                }
            }

            public static string URL
            {
                get
                {
                    string val = get_value(null, RVSettingsItem.CephURL);
                    if (!string.IsNullOrEmpty(val) && val[val.Length - 1] == '/') val = val.Substring(0, val.Length - 1);
                    return val;
                }
            }

            public static string AccessKey
            {
                get { return get_value(null, RVSettingsItem.CephAccessKey); }
            }

            public static string SecretKey
            {
                get { return get_value(null, RVSettingsItem.CephSecretKey); }
            }

            public static string Bucket
            {
                get { return get_value(null, RVSettingsItem.CephBucket); }
            }
        }

        public class Redis {
            public static string Hosts
            {
                get { return get_value(null, RVSettingsItem.RedisHosts); }
            }

            public static string Password
            {
                get { return get_value(null, RVSettingsItem.RedisPassword); }
            }
        }

        public static string RaaiVanURL(Guid? applicationId)
        {
            ITenant tenant = RaaiVanSettings.SAASBasedMultiTenancy ? null : Tenants.FirstOrDefault(u => u.Id == applicationId);
            string val = get_value(applicationId, RVSettingsItem.SystemURL);

            if (!string.IsNullOrEmpty(val) && val[val.Length - 1] == '/') val = val.Substring(0, val.Length - 1);

            return tenant == null || string.IsNullOrEmpty(tenant.Domain) ?
                (string.IsNullOrEmpty(val) ? "http://127.0.0.1" : val) :
                tenant.Protocol + "://" + tenant.Domain;
        }

        public static bool IgnoreReturnURLOnLogin(Guid? applicationId)
        {
            return get_value(applicationId, RVSettingsItem.IgnoreReturnURLOnLogin).ToLower() == "true";
        }

        public static bool UserSignUp(Guid? applicationId)
        {
            return get_value(applicationId, RVSettingsItem.UserSignUp).ToLower() == "true";
        }

        public static bool SignUpViaInvitation(Guid? applicationId)
        {
            return get_value(applicationId, RVSettingsItem.SignUpViaInvitation).ToLower() == "true";
        }

        public static string LoginPageInfo(Guid? applicationId)
        {
            return get_value(applicationId, RVSettingsItem.LoginPageInfo);
        }

        public static string LoginPageModel(Guid? applicationId)
        {
            return get_value(applicationId, RVSettingsItem.LoginPageModel);
        }

        public static string DefaultPrivacy(Guid applicationId)
        {
            return get_value(applicationId, RVSettingsItem.DefaultPrivacy, "Public");
        }

        public static bool DailyDatabaseBackup
        {
            get
            {
                return get_value(Guid.Empty, RVSettingsItem.DailyDatabaseBackup, "true").ToLower() == "true" &&
                    RaaiVanSettings.SAASBasedMultiTenancy;
            }
        }

        public static bool RemoveOldDatabaseBackups
        {
            get
            {
                return get_value(Guid.Empty, RVSettingsItem.RemoveOldDatabaseBackups).ToLower() == "true";
            }
        }

        public static bool AllowNotAuthenticatedUsers(Guid? applicationId)
        {
            return get_value(applicationId, RVSettingsItem.AllowNotAuthenticatedUsers, "true").ToLower() == "true";
        }

        public static bool EnableThemes(Guid? applicationId)
        {
            return get_value(applicationId, RVSettingsItem.EnableThemes, "true").ToLower() == "true";
        }

        public static string DefaultTheme(Guid? applicationId)
        {
            string val = get_value(applicationId, RVSettingsItem.DefaultTheme, "Default");

            string fileName = (!ThemesDic.ContainsKey("Themes") ? new ArrayList() : (ArrayList)ThemesDic["Themes"]).ToArray()
                .Select(t => (string)((Dictionary<string, object>)t)["Name"]).ToList()
                .Where(name => name.ToLower().Contains(val.ToLower())).FirstOrDefault();

            return string.IsNullOrEmpty(fileName) ? "Default" : fileName;
        }

        private static Dictionary<string, object> _ThemesDic;

        private static Dictionary<string, object> ThemesDic
        {
            get
            {
                if (_ThemesDic == null)
                    _ThemesDic = PublicMethods.fromJSON("{\"Themes\":[" + string.Join(",", RaaiVanSettings.Themes) + "]}");

                return _ThemesDic;
            }
        }

        private static List<string> _Themes;
        public static List<string> Themes
        {
            get
            {
                if (_Themes == null)
                {
                    _Themes = new List<string>();

                    string[] files = Directory.GetFiles(PublicMethods.map_path("~/css/themes"));

                    foreach (string f in files)
                    {
                        string fileName = f.Substring(f.LastIndexOf('\\') + 1);
                        fileName = fileName.Substring(0, fileName.LastIndexOf('.'));

                        string txt = File.ReadAllText(f);
                        txt = txt.Substring(0, txt.IndexOf('}') + 1);
                        txt = txt.Substring(txt.IndexOf("{"))
                            .Replace("\n", "").Replace("\r", "").Replace("\t", "").Replace(" ", "");

                        _Themes.Add("{\"Name\":\"" + fileName + "\",\"Codes\":" + txt + "}");
                    }
                }

                return _Themes;
            }
        }

        public static string BackgroundColor(Guid? applicationId)
        {
            return get_value(applicationId, RVSettingsItem.BackgroundColor);
        }

        public static bool ColorfulBubbles(Guid? applicationId)
        {
            return get_value(applicationId, RVSettingsItem.ColorfulBubbles, "true").ToLower() != "false";
        }

        public static RVLang DefaultLang(Guid? applicationId)
        {
            string str = get_value(applicationId, RVSettingsItem.DefaultLang, "fa").ToLower();
            RVLang lng = RVLang.fa;
            return Enum.TryParse<RVLang>(str, out lng) && lng != RVLang.none ? lng : RVLang.fa;
        }

        public static string DefaultDirection(Guid? applicationId)
        {
            return PublicMethods.is_rtl_language(applicationId, DefaultLang(applicationId)) ? "RTL" : "LTR";
        }

        public static bool ShowSystemVersion(Guid? applicationId)
        {
            return get_value(applicationId, RVSettingsItem.ShowSystemVersion, "true").ToLower() == "true";
        }

        public static string SystemName(Guid? applicationId)
        {
            return get_value(applicationId, RVSettingsItem.SystemName).ToLower();
        }

        public static string SystemTitle(Guid? applicationId)
        {
            return get_value(applicationId, RVSettingsItem.SystemTitle).ToLower();
        }

        public static bool ServiceUnavailable
        {
            get
            {
                return get_value(Guid.Empty, RVSettingsItem.ServiceUnavailable).ToLower() == "true";
            }
        }

        private static List<ITenant> _Tenants;

        public static List<ITenant> Tenants

        {
            get
            {
                if (_Tenants == null)
                {
                    _Tenants = new List<ITenant>();

                    string[] Keys = ConfigurationManager.AppSettings.AllKeys;

                    foreach (string Key in Keys)
                    {
                        if (Key.ToLower().StartsWith("tenant"))
                        {
                            string[] vals = ConfigurationManager.AppSettings[Key].Split(',');

                            Guid tenantId = Guid.Empty;

                            if (vals.Length < 3 || !Guid.TryParse(vals[0], out tenantId)) continue;

                            if (GlobalSettings.TenantIDs != null && GlobalSettings.TenantIDs.Count > 0 &&
                                !GlobalSettings.TenantIDs.Any(u => u == tenantId)) continue;

                            string name = vals[1];
                            string domain = vals[2];

                            string protocol = domain.IndexOf("://") > 0 ?
                                domain.Substring(0, domain.IndexOf("://")) : string.Empty;

                            if (!string.IsNullOrEmpty(protocol)) domain = domain.Substring(domain.IndexOf("://") + 3);

                            if (string.IsNullOrEmpty(protocol)) protocol = "http";

                            _Tenants.Add(new Tenant(tenantId, name, string.Empty, domain, protocol, avatarName: string.Empty));
                        }
                    }

                    if (_Tenants.Count == 0)
                    {
                        List<Application> lst = GlobalController.get_applications();
                        if (lst.Count == 1) _Tenants.Add(new Tenant(lst[0].ApplicationID.Value, 
                            lst[0].Name, lst[0].Title, string.Empty, string.Empty, string.Empty));
                    }

                    if (_Tenants.Count > GlobalSettings.MaxTenantsCount) _Tenants = new List<ITenant>();

                    GlobalController.set_applications(_Tenants.Select(
                        u => new KeyValuePair<Guid, string>(u.Id, u.Name)).ToList());
                }
                return _Tenants;
            }
        }

        private static List<KeyValuePair<string, string>> _DefaultDomains;
        private static SortedList<Guid, List<KeyValuePair<string, string>>> _Domains;
        public static List<KeyValuePair<string, string>> Domains(Guid applicationId)
        {
            if (_Domains != null && _Domains.ContainsKey(applicationId))
                return _Domains[applicationId].Count > 0 || RaaiVanSettings.Tenants.Count > 1 ?
                    _Domains[applicationId] : _DefaultDomains;

            if (_DefaultDomains == null) _DefaultDomains = new List<KeyValuePair<string, string>>();
            if (_Domains == null) _Domains = new SortedList<Guid, List<KeyValuePair<string, string>>>();

            if (!_Domains.ContainsKey(applicationId))
                _Domains.Add(applicationId, new List<KeyValuePair<string, string>>());

            foreach (string Key in ConfigurationManager.AppSettings.AllKeys)
            {
                if (Key.ToLower().StartsWith("domainname"))
                {
                    string[] vals = ConfigurationManager.AppSettings[Key].Split('|');

                    string value = vals[0];
                    string text = vals.Length > 1 ? vals[1] : value;

                    if (Key.LastIndexOf("_") < 0)
                    {
                        if (!_DefaultDomains.Any(u => u.Value == value))
                            _DefaultDomains.Add(new KeyValuePair<string, string>(value, text));
                    }
                    else
                    {
                        string tenantName = Key.Substring(Key.LastIndexOf("_") + 1);

                        ITenant tnt = RaaiVanSettings.Tenants.Where(
                            u => u.Name.ToLower() == tenantName.ToLower()).FirstOrDefault();

                        if (tnt != null && tnt.Id != Guid.Empty)
                        {
                            if (!_Domains.ContainsKey(tnt.Id))
                                _Domains.Add(tnt.Id, new List<KeyValuePair<string, string>>());

                            if (!_Domains[tnt.Id].Any(u => u.Key == value))
                                _Domains[tnt.Id].Add(new KeyValuePair<string, string>(value, text));
                        }
                    }
                }
            } //end of 'foreach (string Key in ConfigurationManager.AppSettings.AllKeys)'

            return _Domains.ContainsKey(applicationId) &&
                (_Domains[applicationId].Count > 1 || RaaiVanSettings.Tenants.Count > 1) ?
                _Domains[applicationId] : _DefaultDomains;
        }

        public static string LoginMessage(Guid? applicationId)
        {
            return get_value(applicationId, RVSettingsItem.LoginMessage).ToLower();
        }

        public static int InformLastLogins(Guid? applicationId)
        {
            return get_int(applicationId, RVSettingsItem.InformLastLogins, 0, 0);
        }

        private static int _GetDurationInSeconds(Guid? applicationId, RVSettingsItem name,
            int defaultDuration, int maxDuration)
        {
            int val = 0;

            int hours = 0, minutes = 0, seconds = 0;
            string[] timeElements = (get_value(applicationId, name)).Split(':');

            if (timeElements.Length == 3 && int.TryParse(timeElements[0], out hours) &&
                int.TryParse(timeElements[1], out minutes) && int.TryParse(timeElements[2], out seconds))
                val = (hours * 60 * 60) + (minutes * 60) + seconds;
            else
                val = defaultDuration;

            val = val <= 0 ? defaultDuration : val;

            return maxDuration > 0 && val > maxDuration ? maxDuration : val;
        }

        public static int AuthCookieLifeTimeInSeconds(Guid? applicationId)
        {
            int def = 24 * 60 * 60;
            return _GetDurationInSeconds(applicationId, RVSettingsItem.AuthCookieLifeTime, def, def);
        }

        public static int AuthCookieLifeTimeForAdminInSeconds(Guid? applicationId)
        {
            int def = 24 * 60 * 60;
            return _GetDurationInSeconds(applicationId, RVSettingsItem.AuthCookieLifeTimeForAdmin, def, def);
        }

        public static int MaxAllowedInactiveTimeInSeconds(Guid? applicationId)
        {
            int def = 24 * 60 * 60;
            return _GetDurationInSeconds(applicationId, RVSettingsItem.MaxAllowedInactiveTime, def, def);
        }

        public static int AllowedConsecutiveFailedLoginAttempts(Guid? applicationId)
        {
            return get_int(applicationId, RVSettingsItem.AllowedConsecutiveFailedLoginAttempts, 3, 0);
        }

        public static int LoginLockoutDurationInSeconds(Guid? applicationId)
        {
            return _GetDurationInSeconds(applicationId, RVSettingsItem.LoginLockoutDuration, 3 * 60, 0);
        }

        public static int ReplayAttackQueueLength
        {
            get
            {
                return get_int(Guid.Empty, RVSettingsItem.ReplayAttackQueueLength, 10000, 100);
            }
        }

        public static bool UseSecureAuthCookie(Guid? applicationId)
        {
            return get_value(applicationId, RVSettingsItem.UseSecureAuthCookie).ToLower() == "true";
        }

        public static bool PreventConcurrentSessions(Guid? applicationId)
        {
            return get_value(applicationId, RVSettingsItem.PreventConcurrentSessions).ToLower() == "true";
        }

        public static bool FileEncryption(Guid? applicationId)
        {
            return get_value(applicationId, RVSettingsItem.FileEncryption).ToLower() == "true";
        }

        public static bool RealTime(Guid? applicationId)
        {
            return RaaiVanConfig.Modules.RealTime(applicationId) && !RaaiVanSettings.SAASBasedMultiTenancy &&
                get_value(Guid.Empty, RVSettingsItem.RealTime).ToLower() != "false";
        }

        public static bool Explorer(Guid? applicationId)
        {
            return RaaiVanConfig.Modules.Explorer(applicationId) &&
                get_value(applicationId, RVSettingsItem.Explorer).ToLower() != "false";
        }

        public static string GhostScriptDirectory
        {
            get
            {
                return get_value(null, RVSettingsItem.GhostScriptDirectory);
            }
        }

        public static class SSO
        {
            public static bool Enabled(Guid? applicationId)
            {
                return get_value(applicationId, RVSettingsItem.SSOEnabled).ToLower() == "true";
            }

            public static bool AutoRedirect(Guid? applicationId)
            {
                return get_value(applicationId, RVSettingsItem.SSOAutoRedirect).ToLower() == "true";
            }

            public static string LoginTitle(Guid? applicationId)
            {
                return get_value(applicationId, RVSettingsItem.SSOLoginTitle);
            }

            public static string TicketVariableName(Guid? applicationId)
            {
                return get_value(applicationId, RVSettingsItem.SSOTicketVariableName);
            }

            public static string LoginURL(Guid? applicationId)
            {
                return get_value(applicationId, RVSettingsItem.SSOLoginURL).Replace("[return_url]",
                    PublicConsts.get_complete_url(applicationId, PublicConsts.LoginPage));
            }

            public static string ValidateURL(Guid? applicationId, string ticket)
            {
                return get_value(applicationId, RVSettingsItem.SSOValidateURL).Replace("[return_url]",
                    PublicConsts.get_complete_url(applicationId, PublicConsts.LoginPage)).Replace("[ticket]", ticket);
            }

            public static string XMLUserNameTag(Guid? applicationId)
            {
                return get_value(applicationId, RVSettingsItem.SSOXMLUserNameTag);
            }

            public static string JSONUserNamePath(Guid? applicationId)
            {
                return get_value(applicationId, RVSettingsItem.SSOJSONUserNamePath);
            }

            public static string InvalidTicketCode(Guid? applicationId)
            {
                return get_value(applicationId, RVSettingsItem.SSOInvalidTicketCode);
            }
        }

        public static class PersonalPagePriorities
        {
            private static List<string> _GetArea(Guid applicationId, string pos)
            {
                string _str = get_value(applicationId, RVSettingsItem.PersonalPagePriorities);
                List<string> areas = _str.Split('|').ToList();

                return (areas.Where(u => u.StartsWith(pos)).LastOrDefault() == null ? string.Empty :
                    areas.Where(u => u.StartsWith(pos)).LastOrDefault()).Split(':')
                    .Where(v => v != pos && !string.IsNullOrEmpty(v)).ToList();
            }

            public static List<string> Left(Guid applicationId)
            {
                return _GetArea(applicationId, "Left");
            }

            public static List<string> Center(Guid applicationId)
            {
                return _GetArea(applicationId, "Center");
            }

            public static List<string> Right(Guid applicationId)
            {
                return _GetArea(applicationId, "Right");
            }
        }

        public static class Users
        {
            public static bool IgnoreActiveDirectoryUserCheck(Guid? applicationId)
            {
                return get_value(applicationId, RVSettingsItem.IgnoreActiveDirectoryUserCheck).ToLower() == "true";
            }

            public static bool RestrictPasswordsToActiveDirectory(Guid applicationId)
            {
                return get_value(applicationId, RVSettingsItem.RestrictPasswordsToActiveDirectory).ToLower() == "true";
            }

            public static bool EnableTwoStepAuthenticationViaEmail(Guid? applicationId)
            {
                return get_value(applicationId, RVSettingsItem.EnableTwoStepAuthenticationViaEmail).ToLower() == "true";
            }

            public static bool EnableTwoStepAuthenticationViaSMS(Guid? applicationId)
            {
                return get_value(applicationId, RVSettingsItem.EnableTwoStepAuthenticationViaSMS).ToLower() == "true";
            }

            public static int TwoStepAuthenticationTimeout(Guid? applicationId)
            {
                return get_int(applicationId, RVSettingsItem.TwoStepAuthenticationTimeoutInSeconds, 60, 60);
            }

            public static bool ForceChangeFirstPassword(Guid? applicationId)
            {
                return get_value(applicationId, RVSettingsItem.ForceChangeFirstPassword).ToLower() == "true";
            }

            public static int PasswordLifeTimeInDays(Guid? applicationId)
            {
                return get_int(applicationId, RVSettingsItem.PasswordLifeTimeInDays, 0, 0);
            }

            public static int NotAvailablePreviousPasswordsCount(Guid? applicationId)
            {
                return get_int(applicationId, RVSettingsItem.NotAvailablePreviousPasswordsCount, 0, 0);
            }

            public static string UserNamePattern(Guid? applicationId)
            {
                return get_value(applicationId, RVSettingsItem.UserNamePattern).Trim();
            }

            public static bool HideUserNames(Guid? applicationId)
            {
                return get_value(applicationId, RVSettingsItem.HideUserNames).ToLower() == "true";
            }

            public static class PasswordPolicy
            {
                public static int MinLength(Guid? applicationId)
                {
                    return get_int(applicationId, RVSettingsItem.PasswordPolicyMinLength, 8, null);
                }

                public static int NewCharacters(Guid? applicationId)
                {
                    return get_int(applicationId, RVSettingsItem.PasswordPolicyNewCharacters, 1, 1);
                }

                public static bool UpperLower(Guid? applicationId)
                {
                    return get_value(applicationId, RVSettingsItem.PasswordPolicyUpperLower).ToLower() != "false";
                }

                public static bool NonAlphabetic(Guid? applicationId)
                {
                    return get_value(applicationId, RVSettingsItem.PasswordPolicyNonAlphabetic).ToLower() != "false";
                }

                public static bool Number(Guid? applicationId)
                {
                    return get_value(applicationId, RVSettingsItem.PasswordPolicyNumber).ToLower() != "false";
                }

                public static bool NonAlphaNumeric(Guid? applicationId)
                {
                    return get_value(applicationId, RVSettingsItem.PasswordPolicyNonAlphaNumeric).ToLower() != "false";
                }
            }
        }

        public static class Google {
            public static string GATrackingID //TrackingID for Google Analytics Account
            {
                get
                {
                    return get_value(null, RVSettingsItem.GATrackingID).ToLower();
                }
            }

            public static string SignInClientID
            {
                get
                {
                    return get_value(null, RVSettingsItem.GoogleSignInClientID);
                }
            }

            public static class Captcha
            {
                public static string URL
                {
                    get
                    {
                        if (string.IsNullOrEmpty(SiteKey)) return string.Empty;
                        string ret = get_value(null, RVSettingsItem.CaptchaURL).Trim(); ;
                        return string.IsNullOrEmpty(ret) ? ret : ret.Replace("[SiteKey]", SiteKey);
                    }
                }

                public static string SiteKey
                {
                    get
                    {
                        return get_value(null, RVSettingsItem.CaptchaSiteKey).Trim();
                    }
                }

                public static string SecretKey
                {
                    get
                    {
                        return get_value(null, RVSettingsItem.CaptchaSecretKey).Trim();
                    }
                }

                public static string ValidationURL
                {
                    get
                    {
                        return get_value(null, RVSettingsItem.CaptchaValidationURL).Trim();
                    }
                }
            }
        }

        public static class ReautheticationForSensitivePages
        {
            public static bool SettingsAdmin(Guid? applicationId)
            {
                return !SSO.Enabled(applicationId) &&
                    get_value(applicationId, RVSettingsItem.ReauthenticationForSettingsAdminPage).ToLower() == "true";
            }

            public static bool UsersAdmin(Guid? applicationId)
            {
                return !SSO.Enabled(applicationId) &&
                    get_value(applicationId, RVSettingsItem.ReauthenticationForUsersAdminPage).ToLower() == "true";
            }
        }

        public static class CoreNetwork
        {
            public static int MinAcceptableExpertiseReferralsCount(Guid applicationId)
            {
                return get_int(applicationId, RVSettingsItem.MinAcceptableExpertiseReferralsCount, 1, null);
            }

            public static int MinAcceptableExpertiseConfirmsPercentage(Guid applicationId)
            {
                return get_int(applicationId, RVSettingsItem.MinAcceptableExpertiseConfirmsPercentage, 1, null);
            }

            public static List<string> DefaultCNExtensions(Guid applicationId)
            {
                string strExts = get_value(applicationId, RVSettingsItem.DefaultCNExtensions);
                return string.IsNullOrEmpty(strExts) ? new List<string>() : strExts.Trim().Split('|').ToList();
            }
        }

        public static class Knowledge
        {
            public static int AlertExpirationInDays(Guid applicationId)
            {
                return get_int(applicationId, RVSettingsItem.AlertKnowledgeExpirationInDays, 10, null);
            }
        }

        public static class Notifications
        {
            public static int SeenTimeout(Guid? applicationId)
            {
                return get_int(applicationId, RVSettingsItem.NotificationsSeenTimeout, 5000, 2000);
            }

            public static int UpdateInterval(Guid? applicationId)
            {
                return get_int(applicationId, RVSettingsItem.NotificationsUpdateInterval, 30000, 5000);
            }
        }

        public static class FileContentExtraction
        {
            public static bool FileContents(Guid? applicationId)
            {
                return get_value(applicationId, RVSettingsItem.FileContents).ToLower() != "false";
            }

            public static int ExtractionInterval(Guid applicationId)
            {
                return get_int(applicationId, RVSettingsItem.FileContentExtractionInterval, 3600000, null);
            }

            public static int BatchSize(Guid applicationId)
            {
                return get_int(applicationId, RVSettingsItem.FileContentExtractionBatchSize, 10, null);
            }

            public static DateTime StartTime(Guid applicationId)
            {
                return get_time(applicationId, RVSettingsItem.FileContentExtractionStartTime,
                    new DateTime(2000, 1, 1, 0, 0, 0));
            }

            public static DateTime EndTime(Guid applicationId)
            {
                return get_time(applicationId, RVSettingsItem.FileContentExtractionEndTime,
                    new DateTime(2000, 1, 1, 23, 23, 59));
            }
        }

        public static class IndexUpdate
        {
            public static bool Index(Guid applicationId)
            {
                return get_value(applicationId, RVSettingsItem.Index).ToLower() != "false";
            }

            public static bool Ram(Guid applicationId)
            {
                return get_value(applicationId, RVSettingsItem.IndexUpdateUseRam).ToLower() == "true";
            }

            public static int Interval(Guid applicationId)
            {
                return get_int(applicationId, RVSettingsItem.IndexUpdateInterval, 3600000, null);
            }

            public static int BatchSize(Guid applicationId)
            {
                return get_int(applicationId, RVSettingsItem.IndexUpdateBatchSize, 10, null);
            }

            public static DateTime StartTime(Guid applicationId)
            {
                return get_time(applicationId, RVSettingsItem.IndexUpdateStartTime, new DateTime(2000, 1, 1, 0, 0, 0));
            }

            public static DateTime EndTime(Guid applicationId)
            {
                return get_time(applicationId, RVSettingsItem.IndexUpdateEndTime, new DateTime(2000, 1, 1, 23, 59, 59));
            }

            public static string[] Priorities(Guid applicationId)
            {
                string val = get_value(applicationId, RVSettingsItem.IndexUpdatePriorities);

                string[] ps = string.IsNullOrEmpty(val) ? new string[0] : val.Split(',');

                List<string> p = new List<string>();
                for (int i = 0, lnt = ps.Length; i < lnt; ++i)
                {
                    switch (ps[i].ToLower())
                    {
                        case "node":
                            p.Add("Node");
                            break;
                        case "nodetype":
                            p.Add("NodeType");
                            break;
                        case "question":
                            p.Add("Question");
                            break;
                        case "file":
                            if (FileContentExtraction.FileContents(applicationId)) p.Add("File");
                            break;
                        case "user":
                            p.Add("User");
                            break;
                    }
                }

                if (!p.Any(u => u == "Node")) p.Add("Node");
                if (!p.Any(u => u == "NodeType")) p.Add("NodeType");
                if (!p.Any(u => u == "Question")) p.Add("Question");
                if (FileContentExtraction.FileContents(applicationId) && !p.Any(u => u == "File")) p.Add("File");
                if (!p.Any(u => u == "User")) p.Add("User");

                return p.ToArray();
            }

            public static bool CheckPermissions(Guid applicationId)
            {
                return get_value(applicationId, RVSettingsItem.CheckPermissionsForSearchResults).ToLower() == "true";
            }
        }

        public static class Solr
        {
            public static bool Enabled
            {
                get
                {
                    return get_value(null, RVSettingsItem.Solr).ToLower() != "false" &&
                        !string.IsNullOrEmpty(URL) && !string.IsNullOrEmpty(CollectionName);
                }
            }

            public static string URL
            {
                get
                {
                    string val = get_value(null, RVSettingsItem.SolrURL);
                    if (!string.IsNullOrEmpty(val) && val[val.Length - 1] == '/') val = val.Substring(0, val.Length - 1);
                    return val;
                }
            }

            public static string CollectionName
            {
                get { return get_value(null, RVSettingsItem.SolrCollectionName); }
            }

            public static string Username
            {
                get { return get_value(null, RVSettingsItem.SolrUsername); }
            }

            public static string Password
            {
                get { return get_value(null, RVSettingsItem.SolrPassword); }
            }
        }

        public static class EmailQueue
        {
            public static bool EnableEmailQueue(Guid applicationId)
            {
                return RaaiVanConfig.Modules.SMSEMailNotifier(applicationId) &&
                    get_value(applicationId, RVSettingsItem.EmailQueue).ToLower() == "true";
            }

            public static int Interval(Guid applicationId)
            {
                return get_int(applicationId, RVSettingsItem.EmailQueueInterval, 3600000, null);
            }

            public static int BatchSize(Guid applicationId)
            {
                return get_int(applicationId, RVSettingsItem.EmailQueueBatchSize, 100, 1);
            }

            public static DateTime StartTime(Guid applicationId)
            {
                return get_time(applicationId, RVSettingsItem.EmailQueueStartTime, new DateTime(2000, 1, 1, 0, 0, 0));
            }

            public static DateTime EndTime(Guid applicationId)
            {
                return get_time(applicationId, RVSettingsItem.EmailQueueEndTime, new DateTime(2000, 1, 1, 23, 59, 59));
            }
        }

        public static class SystemEmail
        {
            public static string Address(Guid? applicationId)
            {
                return get_value(applicationId, RVSettingsItem.SystemEmailAddress);
            }

            public static string DisplayName(Guid? applicationId)
            {
                return get_value(applicationId, RVSettingsItem.SystemEmailDisplayName);
            }

            public static string Password(Guid? applicationId)
            {
                return get_value(applicationId, RVSettingsItem.SystemEmailPassword);
            }

            public static string SMTPAddress(Guid? applicationId)
            {
                return get_value(applicationId, RVSettingsItem.SystemEmailSMTPAddress);
            }

            public static int SMTPPort(Guid? applicationId)
            {
                return get_int(applicationId, RVSettingsItem.SystemEmailSMTPPort, 0, null);
            }

            public static bool UseSSL(Guid? applicationId)
            {
                return get_value(applicationId, RVSettingsItem.SystemEmailUseSSL).ToLower() == "true";
            }

            public static int Timeout(Guid? applicationId)
            {
                return get_int(applicationId, RVSettingsItem.SystemEmailTimeout, 0, null);
            }

            public static string EmailSubject(Guid? applicationId)
            {
                return get_value(applicationId, RVSettingsItem.SystemEmailSubject, "RaaiVan");
            }
        }

        public static string SchedulerUsername
        {
            get { return get_value(null, RVSettingsItem.SchedulerUsername); }
        }

        public static class Recommender
        {
            public static bool Enabled(Guid applicationId)
            {
                return RaaiVanConfig.Modules.Recommender(applicationId) && !string.IsNullOrEmpty(URL(applicationId)) &&
                    !string.IsNullOrEmpty(Username(applicationId)) && !string.IsNullOrEmpty(Password(applicationId)) &&
                    get_value(applicationId, RVSettingsItem.Recommender).ToLower() != "false";
            }

            public static string URL(Guid applicationId)
            {
                return get_value(applicationId, RVSettingsItem.RecommenderURL);
            }

            public static string Username(Guid applicationId)
            {
                return get_value(applicationId, RVSettingsItem.RecommenderUsername);
            }

            public static string Password(Guid applicationId)
            {
                return get_value(applicationId, RVSettingsItem.RecommenderPassword);
            }
        }

        public static class Jobs
        {
            public static List<string> JobsList
            {
                get
                {
                    List<string> lst = new List<string>();

                    string[] Keys = System.Configuration.ConfigurationManager.AppSettings.AllKeys;

                    foreach (string Key in Keys)
                    {
                        if (Key.ToLower().StartsWith("job"))
                            lst.Add(System.Configuration.ConfigurationManager.AppSettings[Key]);
                    }

                    return lst;
                }
            }
        }

        public static bool UsePostgreSQL
        {
            get
            {
                return get_value(null, RVSettingsItem.UsePostgreSQL).ToLower() == "true";
            }
        }
    }
}