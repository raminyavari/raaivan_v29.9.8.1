using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using RaaiVan.Modules.Users;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.CoreNetwork;
using RaaiVan.Modules.FormGenerator;
using RaaiVan.Modules.Knowledge;
using RaaiVan.Modules.Documents;
using RaaiVan.Modules.Sharing;
using RaaiVan.Modules.Reports;
using RaaiVan.Modules.Privacy;
using RaaiVan.Modules.Wiki;
using System.Web.Security;

namespace RaaiVan.Web.API
{
    public enum RouteName
    {
        none,

        error,
        accessdenied,

        admin_configuration,
        admin_systemsettings,
        admin_users,
        admin_confidentiality,
        admin_usergroups,
        admin_map,
        admin_knowledge,
        admin_documents,
        admin_forms,
        admin_polls,
        admin_workflows,
        admin_qa,
        admin_dataimport,
        admin_externalnotifications,
        admin_remoteservers,
        admin_help,

        login,
        onboarding,
        teams,
        changepassword,
        newnode,
        node,
        advanced_search,
        search,
        usersearch,
        remotesearch,
        profile,
        form,
        posts,
        home,
        reports,
        dashboard,
        graph,
        explorer,
        questions,
        question,
        qatag,
        newquestion,
        network,
        messages,
        help
    }

    public class RouteActionParams
    {
        public ParamsContainer ParamsContainer;
        public User CurrentUser;
        public Dictionary<string, object> Data;

        public RouteActionParams(ParamsContainer paramsContainer, User currentUser, Dictionary<string, object> data)
        {
            ParamsContainer = paramsContainer;
            CurrentUser = currentUser;
            Data = data;
        }

        public void set_service_unavailable() { Data["ServiceUnavailable"] = true; }

        public void set_no_application_found() { Data["NoApplicationFound"] = true; }

        public void set_redirect_to_login() { Data["RedirectToLogin"] = true; }

        public void set_redirect_to_home() { Data["RedirectToHome"] = true; }

        public void set_redirect_to_profile(Guid? userId = null)
        {
            if (userId.HasValue) Data["RedirectToProfile"] = userId;
            else Data["RedirectToProfile"] = true;
        }

        public void set_redirect_to_teams() { Data["RedirectToTeams"] = true; }

        public void set_redirect_to_change_password() { Data["RedirectToChangePassword"] = true; }

        public void set_redirect_to_onboarding() { Data["RedirectToOnboarding"] = true; }

        public void set_redirect_to_url(string url)
        {
            if (!string.IsNullOrEmpty(url)) url = url.Trim();
            if (!string.IsNullOrEmpty(url)) Data["RedirectToURL"] = url;
        }

        public void set_access_denied() { Data["AccessDenied"] = true; }

        public void set_null_profile_exception()
        {
            Data["NullProfileException"] = true;
        }
    }

    public class RouteInfo
    {
        public RouteName RouteName;
        public AccessRoleName AccessRole;
        public Action<RouteActionParams> Action;

        public RouteInfo(RouteName name, AccessRoleName roleName = AccessRoleName.None, Action<RouteActionParams> action = null)
        {
            RouteName = name;
            AccessRole = roleName;
            Action = action;
        }

        private bool check_service_unavailable(RouteActionParams input)
        {
            if (RaaiVanSettings.ServiceUnavailable && RouteName != RouteName.none &&
                string.IsNullOrEmpty(input.ParamsContainer.request_param("iamadmin")))
            {
                input.set_service_unavailable();
                return false;
            }

            return true;
        }

        private bool check_module(RouteActionParams input)
        {
            List<RouteName> InactiveModule = new List<RouteName>() {
                !Modules.RaaiVanConfig.Modules.FormGenerator(input.ParamsContainer.ApplicationID) ? RouteName.admin_forms : RouteName.none,
                !Modules.RaaiVanConfig.Modules.FormGenerator(input.ParamsContainer.ApplicationID) ? RouteName.admin_polls : RouteName.none,
                !Modules.RaaiVanConfig.Modules.WorkFlow(input.ParamsContainer.ApplicationID) ? RouteName.admin_workflows : RouteName.none,
                !Modules.RaaiVanConfig.Modules.Documents(input.ParamsContainer.ApplicationID) ? RouteName.admin_documents : RouteName.none,
                !Modules.RaaiVanConfig.Modules.KnowledgeAdmin(input.ParamsContainer.ApplicationID) ? RouteName.admin_knowledge : RouteName.none,
                !Modules.RaaiVanConfig.Modules.SMSEMailNotifier(input.ParamsContainer.ApplicationID) ? RouteName.admin_externalnotifications : RouteName.none,
                !Modules.RaaiVanConfig.Modules.SocialNetwork(input.ParamsContainer.ApplicationID) ? RouteName.network : RouteName.none,
                !Modules.RaaiVanConfig.Modules.SocialNetwork(input.ParamsContainer.ApplicationID) ? RouteName.posts : RouteName.none,
                !Modules.RaaiVanConfig.Modules.Messaging(input.ParamsContainer.ApplicationID) ? RouteName.messages: RouteName.none,
                !Modules.RaaiVanConfig.Modules.QA(input.ParamsContainer.ApplicationID) ? RouteName.qatag : RouteName.none,
                !Modules.RaaiVanConfig.Modules.QA(input.ParamsContainer.ApplicationID) ? RouteName.question : RouteName.none,
                !Modules.RaaiVanConfig.Modules.QA(input.ParamsContainer.ApplicationID) ? RouteName.questions : RouteName.none,
                !Modules.RaaiVanConfig.Modules.QA(input.ParamsContainer.ApplicationID) ? RouteName.newquestion : RouteName.none,
                !Modules.RaaiVanConfig.Modules.QA(input.ParamsContainer.ApplicationID) ? RouteName.admin_qa : RouteName.none,
                !Modules.RaaiVanConfig.Modules.QAAdmin(input.ParamsContainer.ApplicationID) ? RouteName.admin_qa : RouteName.none,
                !Modules.RaaiVanConfig.Modules.Explorer(input.ParamsContainer.ApplicationID) ? RouteName.explorer : RouteName.none,
                !Modules.RaaiVanConfig.Modules.RestAPI(input.ParamsContainer.ApplicationID) || RaaiVanSettings.SAASBasedMultiTenancy ? 
                    RouteName.admin_remoteservers : RouteName.none,
                !Modules.RaaiVanConfig.Modules.RestAPI(input.ParamsContainer.ApplicationID) || RaaiVanSettings.SAASBasedMultiTenancy ? 
                    RouteName.remotesearch : RouteName.none,

                !RaaiVanSettings.SAASBasedMultiTenancy ? RouteName.onboarding : RouteName.none,
                !RaaiVanSettings.SAASBasedMultiTenancy ? RouteName.teams : RouteName.none
            }.Where(n => n != RouteName.none).Distinct().ToList();

            if (InactiveModule.Any(x => x == RouteName))
            {
                if (!input.ParamsContainer.IsAuthenticated)
                    input.set_redirect_to_login();
                else input.set_redirect_to_home();
                return false;
            }

            return true;
        }

        private bool check_authenticated_in_login_page(RouteActionParams input)
        {
            if (RouteName == RouteName.login && input.ParamsContainer.IsAuthenticated)
            {
                Guid? invitationId = PublicMethods.parse_guid(input.ParamsContainer.request_param("inv"));
                User user = new User() { UserID = input.ParamsContainer.CurrentUserID.Value };

                RaaiVanUtil.init_user_application(invitationId, user, hasAlreadyLoggedIn: true);
                
                if (!input.ParamsContainer.ApplicationID.HasValue && RaaiVanSettings.SAASBasedMultiTenancy)
                    input.set_redirect_to_teams();
                else input.set_redirect_to_home();
                
                return false;
            }

            return true;
        }

        private bool check_authentication(RouteActionParams input)
        {
            bool allowNotAuth = RaaiVanSettings.AllowNotAuthenticatedUsers(input.ParamsContainer.ApplicationID);

            List<RouteName> AuthenticationException = new List<RouteName>() {
                RouteName.login,
                RouteName.accessdenied,
                RouteName.error,
                allowNotAuth ? RouteName.graph : RouteName.none,
                allowNotAuth ? RouteName.qatag : RouteName.none,
                allowNotAuth ? RouteName.question : RouteName.none,
                allowNotAuth ? RouteName.node : RouteName.none,
                allowNotAuth ? RouteName.advanced_search : RouteName.none
            }.Where(n => n != RouteName.none).ToList();

            if (input.ParamsContainer.IsAuthenticated)
                input.Data["IsAuthenticated"] = true;
            else if (!AuthenticationException.Any(x => x == RouteName))
            {
                input.set_redirect_to_login();
                return false;
            }

            return true;
        }

        private bool check_profile_completeness(RouteActionParams input)
        {
            List<RouteName> CompleteProfileException = new List<RouteName>() {
                RouteName.changepassword,
                RouteName.login,
                RouteName.onboarding,
                RouteName.profile,
                RouteName.accessdenied,
                RouteName.error,
                RouteName.teams,
                RouteName.help
            };

            bool isProfilePage = RouteName == RouteName.profile;

            User currentUser = null;

            if ((isProfilePage || !CompleteProfileException.Any(x => x == RouteName)) && input.ParamsContainer.IsAuthenticated)
            {
                input.CurrentUser = currentUser =
                    UsersController.get_user(input.ParamsContainer.ApplicationID, input.ParamsContainer.CurrentUserID.Value);
                if (currentUser == null || !currentUser.profileCompleted())
                {
                    input.Data["IncompleteProfile"] = true;

                    if (isProfilePage && currentUser == null)
                    {
                        input.set_null_profile_exception();
                        return false;
                    }
                    else if (!isProfilePage)
                    {
                        input.set_redirect_to_profile();
                        return false;
                    }
                }
            }

            return true;
        }

        private bool check_application(RouteActionParams input)
        {
            List<RouteName> CheckApplicationException = new List<RouteName>() {
                RouteName.onboarding,
                RouteName.teams,
                RouteName.help,
                RaaiVanSettings.SAASBasedMultiTenancy ? RouteName.login : RouteName.none
                //, RaaiVanSettings.SAASBasedMultiTenancy ? RouteName.profile : RouteName.none
            }.Where(n => n != RouteName.none).ToList();

            if (input.ParamsContainer.ApplicationID.HasValue)
                input.Data["ApplicationID"] = input.ParamsContainer.ApplicationID;
            else if (!CheckApplicationException.Any(x => x == RouteName))
            {
                if (RaaiVanSettings.SAASBasedMultiTenancy)
                {
                    if (input.ParamsContainer.IsAuthenticated)
                        input.set_redirect_to_teams();
                    else input.set_redirect_to_login();
                }
                else input.set_no_application_found();

                return false;
            }

            return true;
        }

        private bool check_access(RouteActionParams input)
        {
            List<RouteName> SystemAdminAccess = new List<RouteName>()
            {
                RouteName.admin_systemsettings,
                RouteName.admin_remoteservers,
                RouteName.admin_help
            };

            bool accessDenied = false;

            if (SystemAdminAccess.Any(x => x == RouteName))
            {
                accessDenied = !input.ParamsContainer.CurrentUserID.HasValue ||
                    !PublicMethods.is_system_admin(input.ParamsContainer.ApplicationID, input.ParamsContainer.CurrentUserID.Value);
            }
            else if (AccessRole != AccessRoleName.None)
            {
                accessDenied = !input.ParamsContainer.CurrentUserID.HasValue ||
                    !AuthorizationManager.has_right(AccessRole, input.ParamsContainer.CurrentUserID);
            }

            if (accessDenied) input.set_access_denied();

            return !accessDenied;
        }

        private bool check_password_change(RouteActionParams input)
        {
            List<RouteName> CheckPasswordException = new List<RouteName>() {
                RouteName.changepassword,
                RouteName.login,
                RouteName.accessdenied,
                RouteName.error
            };

            string reason = string.Empty;
            bool changePass = RaaiVanUtil.password_change_needed(input.ParamsContainer.Context, ref reason);

            if (changePass)
            {
                input.Data["PasswordChangeReason"] = reason;

                if (!CheckPasswordException.Any(x => x == RouteName))
                {
                    input.set_redirect_to_change_password();
                    return false;
                }
            }

            return true;
        }

        public Dictionary<string, object> get_data(ParamsContainer paramsContainer)
        {
            RouteActionParams actionParams = new RouteActionParams(paramsContainer,
                currentUser: null, data: new Dictionary<string, object>());

            List<Func<RouteActionParams, bool>> initialChecks = new List<Func<RouteActionParams, bool>>()
            {
                check_service_unavailable,
                check_module,
                check_application,
                check_authenticated_in_login_page,
                check_authentication,
                check_profile_completeness,
                check_access,
                check_password_change
            };

            foreach (Func<RouteActionParams, bool> check in initialChecks)
                if (!check(actionParams)) return actionParams.Data;


            //Check Reauthentication
            List<RouteName> RequiresReauthentication = new List<RouteName>() {
                RaaiVanSettings.ReautheticationForSensitivePages.SettingsAdmin(paramsContainer.ApplicationID) ?
                    RouteName.admin_systemsettings : RouteName.none,
                RaaiVanSettings.ReautheticationForSensitivePages.UsersAdmin(paramsContainer.ApplicationID) ?
                    RouteName.admin_users : RouteName.none
            }.Where(n => n != RouteName.none).ToList();

            if (RequiresReauthentication.Any(x => x == RouteName))
                actionParams.Data["Reauthenticate"] = true;
            //end of Check Reauthentication


            Application currApp = PublicMethods.get_current_application();
            if (currApp != null) actionParams.Data["Application"] = PublicMethods.fromJSON(currApp.toJson(icon: true));

            Action?.Invoke(actionParams);

            return actionParams.Data;
        }
    }

    public class RouteList
    {
        private static List<RouteInfo> Routes = new List<RouteInfo>() {
            new RouteInfo(name: RouteName.admin_configuration, roleName: AccessRoleName.ManagementSystem),
            new RouteInfo(name: RouteName.admin_users, roleName: AccessRoleName.UsersManagement),
            new RouteInfo(name: RouteName.admin_confidentiality, roleName: AccessRoleName.ManageConfidentialityLevels),
            new RouteInfo(name: RouteName.admin_usergroups, roleName: AccessRoleName.UserGroupsManagement),
            new RouteInfo(name: RouteName.admin_map, roleName: AccessRoleName.ManageOntology, action: admin_map),
            new RouteInfo(name: RouteName.admin_knowledge, roleName: AccessRoleName.KnowledgeAdmin),
            new RouteInfo(name: RouteName.admin_documents, roleName: AccessRoleName.ContentsManagement),
            new RouteInfo(name: RouteName.admin_forms, roleName: AccessRoleName.ManageForms),
            new RouteInfo(name: RouteName.admin_polls, roleName: AccessRoleName.ManagePolls),
            new RouteInfo(name: RouteName.admin_workflows, roleName: AccessRoleName.ManageWorkflow),
            new RouteInfo(name: RouteName.admin_qa, roleName: AccessRoleName.ManageQA),
            new RouteInfo(name: RouteName.admin_dataimport, roleName: AccessRoleName.DataImport),
            new RouteInfo(name: RouteName.admin_externalnotifications, roleName: AccessRoleName.SMSEMailNotifier),
            new RouteInfo(name: RouteName.admin_remoteservers),

            new RouteInfo(name: RouteName.reports, roleName: AccessRoleName.Reports, action: reports),

            new RouteInfo(name: RouteName.advanced_search, action: advanced_search),
            new RouteInfo(name: RouteName.search, action: search),
            new RouteInfo(name: RouteName.error, action: error),
            new RouteInfo(name: RouteName.form, action: form),
            new RouteInfo(name: RouteName.home, action: home),
            new RouteInfo(name: RouteName.profile, action: profile),
            new RouteInfo(name: RouteName.login, action: login),
            new RouteInfo(name: RouteName.newnode, action: new_node),
            new RouteInfo(name: RouteName.node, action: node),
            new RouteInfo(name: RouteName.posts, action: posts),
            new RouteInfo(name: RouteName.qatag, action: qa_tag),
            new RouteInfo(name: RouteName.question, action: question),
            new RouteInfo(name: RouteName.remotesearch, roleName: AccessRoleName.RemoteServers)
        };

        public static Dictionary<string, object> get_data(ParamsContainer paramsContainer, string name)
        {
            if (!string.IsNullOrEmpty(name)) name = name.ToLower().Trim();

            RouteName routeName = PublicMethods.parse_enum<RouteName>(name, RouteName.none);

            RouteInfo info = Routes.Where(r => r.RouteName == routeName).FirstOrDefault();

            if (info == null) info = new RouteInfo(routeName);

            Dictionary<string, object> data = info.get_data(paramsContainer);
            
            return data;
        }

        public static Dictionary<string, object> get_data_server_side(ParamsContainer paramsContainer, RouteName routeName)
        {
            Dictionary<string, object> data = get_data(paramsContainer, routeName.ToString());
            
            Guid? userId = null;
            string redirectToUrl = PublicMethods.get_dic_value(data, "RedirectToURL");
            
            if (PublicMethods.get_dic_value<bool>(data, "RedirectToLogin", false))
            {
                FormsAuthentication.RedirectToLoginPage();
                //FormsAuthentication.RedirectToLoginPage("ReturnUrl=" + HttpUtility.UrlEncode(paramsContainer.Context.Request.Url.AbsolutePath));
            }
            else if (PublicMethods.get_dic_value<bool>(data, "RedirectToHome", false))
                paramsContainer.Context.Response.Redirect(PublicConsts.HomePage);
            else if (PublicMethods.get_dic_value<bool>(data, "RedirectToProfile", false))
                paramsContainer.Context.Response.Redirect(PublicConsts.ProfilePage);
            else if ((userId = PublicMethods.parse_guid(PublicMethods.get_dic_value(data, "RedirectToProfile"))).HasValue)
                paramsContainer.Context.Response.Redirect(PublicConsts.ProfilePage + "/" + userId.Value.ToString());
            else if (PublicMethods.get_dic_value<bool>(data, "RedirectToTeams", false))
                paramsContainer.Context.Response.Redirect(PublicConsts.ApplicationsPage);
            else if (PublicMethods.get_dic_value<bool>(data, "RedirectToChangePassword", false))
                paramsContainer.Context.Response.Redirect(PublicConsts.ChangePasswordPage);
            else if (PublicMethods.get_dic_value<bool>(data, "AccessDenied", false) && routeName != RouteName.node)
                paramsContainer.Context.Response.Redirect(PublicConsts.NoAccessPage);
            else if (PublicMethods.get_dic_value<bool>(data, "NoApplicationFound", false))
                paramsContainer.return_response(PublicConsts.NullTenantResponse);
            else if (PublicMethods.get_dic_value<bool>(data, "ServiceUnavailable", false))
                paramsContainer.Context.Response.Redirect(PublicConsts.ServiceUnavailablePage);
            else if (!string.IsNullOrEmpty(redirectToUrl))
                paramsContainer.Context.Response.Redirect(redirectToUrl);
            else if (PublicMethods.get_dic_value<bool>(data, "NullProfileException", false))
            {
                paramsContainer.Context.Response.Write("پروفایل شما کامل نیست. لطفا به مدیر سیستم مراجعه فرمایید :)");
                paramsContainer.Context.Response.End();
            }

            return data;
        }

        private static void admin_map(RouteActionParams input)
        {
            Guid? formId = RaaiVanSettings.NodeTypeIdentityFormID;

            if (formId.HasValue && (!input.ParamsContainer.ApplicationID.HasValue ||
                input.ParamsContainer.ApplicationID != RaaiVanSettings.ReferenceTenantID)) formId = null;

            if (formId.HasValue) input.Data["IdentityFormID"] = formId;
        }

        private static void search(RouteActionParams input)
        {
            string encoding = input.ParamsContainer.request_param("Encoding");
            string searchText = input.ParamsContainer.request_param("SearchText");

            try
            {
                if (!string.IsNullOrEmpty(encoding) && encoding.ToLower().Trim() == "url" && !string.IsNullOrEmpty(searchText))
                    searchText = Base64.encode(HttpUtility.UrlDecode(searchText));
            }
            catch { }

            if (!string.IsNullOrEmpty(searchText))
                input.Data["SearchText"] = searchText.Replace('_', '/').Replace('~', '+');
        }

        private static void advanced_search(RouteActionParams input)
        {
            Guid? id = PublicMethods.parse_guid(input.ParamsContainer.request_param("ID"));
            List<Guid> ids = ListMaker.get_guid_items(input.ParamsContainer.request_param("IDs"), ',');

            if (id.HasValue && !ids.Any(u => u == id)) ids.Add(id.Value);

            List<NodeType> nodeTypes = ids.Count == 0 ? new List<NodeType>() :
                CNController.get_node_types(input.ParamsContainer.Tenant.Id, ids);

            Guid? relatedId = PublicMethods.parse_guid(input.ParamsContainer.request_param("RelatedID"));

            Dictionary<string, object> relatedItem = null;

            if (relatedId.HasValue)
            {
                Node relatedNode = CNController.get_node(input.ParamsContainer.Tenant.Id, relatedId.Value);

                User relatedUser = relatedNode != null && relatedNode.NodeID.HasValue ? null :
                    UsersController.get_user(input.ParamsContainer.Tenant.Id, relatedId.Value);

                if (relatedNode != null && relatedNode.NodeID.HasValue)
                    relatedItem = PublicMethods.fromJSON(relatedNode.toJson(simple: true));
                else if (relatedUser != null && relatedUser.UserID.HasValue)
                    relatedItem = PublicMethods.fromJSON(relatedUser.toJson());
            }

            input.Data["RelatedItem"] = relatedItem;

            input.Data["NodeTypes"] = new ArrayList(nodeTypes
                .Select(nt => PublicMethods.fromJSON(nt.toJson(input.ParamsContainer.ApplicationID, iconUrl: true))).ToList());

            bool? bookmarked = PublicMethods.parse_bool(input.ParamsContainer.request_param("Bookmarked"));
            if (bookmarked.HasValue && bookmarked.Value) input.Data["Bookmarked"] = true;

            if (ids.Count == 1 && input.ParamsContainer.ApplicationID.HasValue) {
                List<string> hierarchy = CNController.get_node_type_hierarchy(input.ParamsContainer.ApplicationID.Value, ids[0])
                    .Select(h => {
                        return new NodeType()
                        {
                            NodeTypeID = h.ID,
                            Name = h.Name
                        }.toJson(input.ParamsContainer.ApplicationID.Value, iconUrl: true);
                    }).ToList();

                hierarchy.Reverse();

                input.Data["Hierarchy"] = new ArrayList(hierarchy.Select(h => PublicMethods.fromJSON(h)).ToList());
            }
        }

        private static void error(RouteActionParams input)
        {
            int code = 0;
            if (!int.TryParse(input.ParamsContainer.request_param("code"), out code) || code <= 0) code = 0;

            if (code > 0) input.Data["Code"] = code;

            //Response.TrySkipIisCustomErrors = true;
            if (code > 0) input.ParamsContainer.Context.Response.StatusCode = code;
            if (code == 404) input.ParamsContainer.Context.Response.StatusDescription = "Page not found";
        }

        private static void form(RouteActionParams input)
        {
            if (!input.ParamsContainer.ApplicationID.HasValue) return;

            Guid? id = null;
            if (!string.IsNullOrEmpty(input.ParamsContainer.request_param("ID")))
                id = Guid.Parse(input.ParamsContainer.request_param("ID"));
            if (!id.HasValue && !string.IsNullOrEmpty(input.ParamsContainer.request_param("InstanceID")))
                id = Guid.Parse(input.ParamsContainer.request_param("InstanceID"));

            /*
            Guid userId = input.ParamsContainer.CurrentUserID.HasValue ? input.ParamsContainer.CurrentUserID.Value : Guid.Empty;

            if (!FGController.is_director(instanceId.HasValue ? instanceId.Value : Guid.Empty, userId)) 
                Response.Redirect(publicCode.NoAccessPage);
            */

            Poll poll = !id.HasValue ? null : FGController.get_poll(input.ParamsContainer.Tenant.Id, id.Value);

            Guid? refInstanceId = null;
            if (!string.IsNullOrEmpty(input.ParamsContainer.request_param("RefInstanceID")))
                refInstanceId = Guid.Parse(input.ParamsContainer.request_param("RefInstanceID"));

            if (id.HasValue) input.Data["InstanceID"] = id;
            if (refInstanceId.HasValue) input.Data["RefInstanceID"] = refInstanceId;

            if (poll != null && poll.PollID.HasValue)
            {
                input.Data["IsPoll"] = true;
                input.Data["Poll"] = PublicMethods.fromJSON(poll.toJson());
            }
        }

        private static void home(RouteActionParams input)
        {
            if (!input.ParamsContainer.ApplicationID.HasValue) return;

            Dictionary<string, object> priors = new Dictionary<string, object>();
            priors["Left"] = new ArrayList(RaaiVanSettings.PersonalPagePriorities.Left(input.ParamsContainer.Tenant.Id));
            priors["Center"] = new ArrayList(RaaiVanSettings.PersonalPagePriorities.Center(input.ParamsContainer.Tenant.Id));
            priors["Right"] = new ArrayList(RaaiVanSettings.PersonalPagePriorities.Right(input.ParamsContainer.Tenant.Id));

            NodeMember nm = !input.ParamsContainer.IsAuthenticated ? null :
                CNController.get_user_department(input.ParamsContainer.Tenant.Id, input.ParamsContainer.CurrentUserID.Value);

            input.Data["PersonalPagePriorities"] = priors;

            if (input.CurrentUser != null)
            {
                Dictionary<string, object> userDic = PublicMethods.fromJSON(input.CurrentUser.toJson(input.ParamsContainer.ApplicationID,
                    profileImageUrl: true, coverPhotoUrl: true, highQualityCoverPhotoUrl: true));

                if (userDic != null && nm != null && nm.Node.NodeID.HasValue)
                {
                    userDic["GroupID"] = nm.Node.NodeID;
                    userDic["GroupName"] = Base64.encode(nm.Node.Name);
                }

                input.Data["User"] = userDic;
            }
        }

        private static void profile(RouteActionParams input)
        {
            Guid? userId = PublicMethods.parse_guid(input.ParamsContainer.request_param("UserID"));
            if (!userId.HasValue) userId = PublicMethods.parse_guid(input.ParamsContainer.request_param("uid"),
                alternatvieValue: input.ParamsContainer.CurrentUserID);

            User user = !userId.HasValue ? null : UsersController.get_user(input.ParamsContainer.ApplicationID, userId.Value);

            if (user == null) return;

            bool isOwnPage = input.ParamsContainer.CurrentUserID == userId;

            Guid? senderUserId = null;
            Friend fs = input.ParamsContainer.IsAuthenticated && input.ParamsContainer.ApplicationID.HasValue && !isOwnPage ?
                UsersController.get_friendship_status(input.ParamsContainer.ApplicationID.Value,
                userId.Value, input.ParamsContainer.CurrentUserID.Value) : null;
            if (fs != null && fs.AreFriends.HasValue)
                senderUserId = fs.IsSender.HasValue && fs.IsSender.Value ? input.ParamsContainer.CurrentUserID : userId;

            NodeMember nm = !input.ParamsContainer.IsAuthenticated || 
                !input.ParamsContainer.ApplicationID.HasValue || !userId.HasValue ? null :
                CNController.get_user_department(input.ParamsContainer.Tenant.Id, userId.Value);

            if (user != null)
            {
                Dictionary<string, object> userDic = PublicMethods.fromJSON(user.toJson(input.ParamsContainer.ApplicationID,
                    profileImageUrl: true, highQualityProfileImageUrl: true, coverPhotoUrl: true, highQualityCoverPhotoUrl: true));

                if (userDic != null && nm != null && nm.Node.NodeID.HasValue)
                {
                    userDic["GroupID"] = nm.Node.NodeID;
                    userDic["GroupName"] = Base64.encode(nm.Node.Name);
                }

                input.Data["User"] = userDic;
            }

            input.Data["ActiveTab"] = input.ParamsContainer.request_param("Tab");
            input.Data["IsOwnPage"] = isOwnPage;
            input.Data["AreFriends"] = !isOwnPage && fs != null && fs.AreFriends.HasValue && fs.AreFriends.Value;
            if (senderUserId.HasValue) input.Data["FriendRequestSenderUserID"] = senderUserId;
            input.Data["EmploymentTypes"] = new ArrayList(Enum.GetNames(typeof(EmploymentType))
                .Where(u => u != EmploymentType.NotSet.ToString()).Select(v => v).ToArray());
            input.Data["PhoneNumberTypes"] = new ArrayList(Enum.GetNames(typeof(PhoneType))
                .Where(u => u != PhoneType.NotSet.ToString()).Select(v => v).ToArray());

            if (!RaaiVanSettings.SAASBasedMultiTenancy && input.ParamsContainer.ApplicationID.HasValue)
            {
                input.Data["HasWikiTitle"] = WikiController.has_title(input.ParamsContainer.ApplicationID.Value,
                    userId.Value, input.ParamsContainer.CurrentUserID);
                input.Data["HasWikiParagraph"] = WikiController.has_paragraph(input.ParamsContainer.ApplicationID.Value,
                    userId.Value, input.ParamsContainer.CurrentUserID);
            }

            //check if user do not visit self page
            if (input.ParamsContainer.ApplicationID.HasValue &&
                !isOwnPage && input.ParamsContainer.Context.Session[userId.ToString()] == null)
            {
                ////register user page visitor
                input.ParamsContainer.Context.Session[userId.ToString()] = true;
                UsersController.register_item_visit(input.ParamsContainer.ApplicationID.Value,
                    userId.Value, input.ParamsContainer.CurrentUserID.Value, DateTime.Now, VisitItemTypes.User);
            }
        }

        private static void login(RouteActionParams input)
        {
            //Check db connection with a fake stored procedure call named 'ping'
            DBResultSet pingRes = DBConnector.read(applicationId: null, "ping");
            if (!string.IsNullOrEmpty(pingRes.ConnectionErrorMessage))
                input.Data["DBConnectionError"] = pingRes.ConnectionErrorMessage;
            
            if (RaaiVanSettings.IgnoreReturnURLOnLogin(input.ParamsContainer.ApplicationID))
                input.Data["IgnoreReturnURL"] = true;

            bool disableLogin = false;
            bool loggedIn = false;
            bool shouldRedirect = false;

            bool? local = PublicMethods.parse_bool(input.ParamsContainer.request_param("Local"));

            if ((!local.HasValue || !local.Value) && RaaiVanSettings.SSO.Enabled(input.ParamsContainer.ApplicationID))
            {
                Dictionary<string, object> ssoLoginResult = new Dictionary<string, object>();

                loggedIn = new LoginUtil(paramsContainer: input.ParamsContainer, invitationId: null)
                    .login_sso(skipRedirect: true, shouldRedirect: ref shouldRedirect, ref ssoLoginResult);

                if (!loggedIn) disableLogin = true;

                ssoLoginResult.Keys.ToList().ForEach(key => input.Data[key] = ssoLoginResult[key]);
            }

            if (shouldRedirect)
            {
                input.set_redirect_to_url(Modules.Jobs.SSO.get_login_url(input.ParamsContainer.ApplicationID));
                return;
            }

            string returnUrl = input.ParamsContainer.request_param("ReturnUrl");
            if (!string.IsNullOrEmpty(returnUrl))
                input.Data["ReturnURL"] = Base64.encode(returnUrl);

            input.Data["LoginPageModel"] = RaaiVanSettings.LoginPageModel(input.ParamsContainer.ApplicationID);
            input.Data["LoggedIn"] = loggedIn;
            input.Data["DisableLogin"] = disableLogin;
            input.Data["NeedsSSORedirect"] = shouldRedirect;

            //login page info
            string[] info = null;
            if (!string.IsNullOrEmpty(RaaiVanSettings.LoginPageInfo(input.ParamsContainer.ApplicationID)))
                info = RaaiVanSettings.LoginPageInfo(input.ParamsContainer.ApplicationID).Split('|');

            if (info != null)
            {
                foreach (string str in info)
                {
                    Dictionary<string, object> dt = PublicMethods.fromJSON(
                        "{" + RaaiVanUtil.get_login_page_info(input.ParamsContainer, str) + "}");

                    if (dt != null)
                        foreach (string key in dt.Keys) input.Data[key] = dt[key];
                }
            }
            //end of login page info
        }

        private static void new_node(RouteActionParams input)
        {
            if (!input.ParamsContainer.ApplicationID.HasValue || !input.ParamsContainer.CurrentUserID.HasValue) return;

            Guid? nodeTypeId = PublicMethods.parse_guid(input.ParamsContainer.request_param("ID"),
                alternatvieValue: PublicMethods.parse_guid(input.ParamsContainer.request_param("NodeTypeID")));

            Service service = !nodeTypeId.HasValue ? null :
                CNController.get_service(input.ParamsContainer.Tenant.Id, nodeTypeId.Value);

            if (!nodeTypeId.HasValue || service == null)
            {
                input.set_redirect_to_home();
                return;
            }

            Guid? parentId = PublicMethods.parse_guid(input.ParamsContainer.request_param("ParentID"));
            Guid? documentTreeNodeId = PublicMethods.parse_guid(input.ParamsContainer.request_param("DocumentTreeNodeID"));
            Guid? previousVersionId = PublicMethods.parse_guid(input.ParamsContainer.request_param("PreviousVersionID"));

            Node parentNode = !parentId.HasValue ? null :
                CNController.get_node(input.ParamsContainer.Tenant.Id, parentId.Value);
            Node previousVersion = !previousVersionId.HasValue ? null :
                CNController.get_node(input.ParamsContainer.Tenant.Id, previousVersionId.Value);
            if (previousVersion != null && previousVersion.NodeTypeID != nodeTypeId) previousVersion = null;

            Tree tree = !documentTreeNodeId.HasValue ? null :
                DocumentsController.get_tree(input.ParamsContainer.Tenant.Id, documentTreeNodeId.Value);
            List<Hierarchy> path = !documentTreeNodeId.HasValue ? new List<Hierarchy>() :
                DocumentsController.get_tree_node_hierarchy(input.ParamsContainer.Tenant.Id, documentTreeNodeId.Value);

            List<Extension> extensions = CNUtilities.extend_extensions(input.ParamsContainer.Tenant.Id,
                CNController.get_extensions(input.ParamsContainer.Tenant.Id, nodeTypeId.Value));

            Dictionary<string, object> extensionsDic = new Dictionary<string, object>();

            extensions.Where(x => !x.Disabled.HasValue || !x.Disabled.Value).ToList()
                .ForEach(ext => extensionsDic[ext.ExtensionType.ToString()] = PublicMethods.fromJSON(ext.toJson()));

            bool isServiceAdmin = CNController.is_service_admin(input.ParamsContainer.Tenant.Id,
                nodeTypeId.Value, input.ParamsContainer.CurrentUserID.Value);

            KnowledgeType kt = KnowledgeController.get_knowledge_type(input.ParamsContainer.Tenant.Id, nodeTypeId.Value);

            input.Data["Service"] = PublicMethods.fromJSON(service.toJson(input.ParamsContainer.Tenant.Id));
            input.Data["Extensions"] = extensionsDic;
            input.Data["IsServiceAdmin"] = isServiceAdmin;
            if (kt != null) input.Data["KnowledgeType"] = PublicMethods.fromJSON(kt.toJson());
            if (parentNode != null) input.Data["ParentNode"] = PublicMethods.fromJSON(parentNode.toJson());
            if (previousVersion != null) input.Data["PreviousVersion"] = PublicMethods.fromJSON(previousVersion.toJson());

            if (tree != null && path != null && path.Count > 0)
            {
                Dictionary<string, object> dtn = new Dictionary<string, object>();
                dtn["Tree"] = PublicMethods.fromJSON(tree.toJson());
                dtn["Path"] = new ArrayList(path.Select(u => PublicMethods.fromJSON(u.toJSON())).ToArray());

                input.Data["DocumentTreeNode"] = dtn;
            }
        }

        private static void node(RouteActionParams input)
        {
            if (!input.ParamsContainer.ApplicationID.HasValue) return;

            Guid? nodeId = PublicMethods.parse_guid(input.ParamsContainer.request_param("ID"),
                alternatvieValue: PublicMethods.parse_guid(input.ParamsContainer.request_param("NodeID")));

            if (!nodeId.HasValue)
            {
                if (input.ParamsContainer.IsAuthenticated) input.set_redirect_to_home();
                else input.set_redirect_to_login();

                return;
            }
            else if (UsersController.get_user(input.ParamsContainer.Tenant.Id, nodeId.Value) != null)
            {
                input.set_redirect_to_profile(nodeId);
                return;
            }

            bool isAuthenticated = input.ParamsContainer.IsAuthenticated;
            Guid? currentUserId = input.ParamsContainer.IsAuthenticated ? input.ParamsContainer.CurrentUserID : null;
            bool isSystemAdmin = input.ParamsContainer.IsAuthenticated && currentUserId.HasValue &&
                PublicMethods.is_system_admin(input.ParamsContainer.Tenant.Id, currentUserId.Value);

            //Check User access to Node
            bool isServiceAdmin = false, isAreaAdmin = false, isCreator = false, isContributor = false,
                isExpert = false, isMember = false, isAdminMember = false, editable = false;
            Service service = CNController.get_service(input.ParamsContainer.Tenant.Id, nodeId.Value);

            Node node = CNController.get_node(input.ParamsContainer.Tenant.Id, nodeId.Value, full: true);

            bool isKnowledge = service != null && service.IsKnowledge.HasValue && service.IsKnowledge.Value;

            bool result = true;
            if (isAuthenticated)
            {
                result = CNController.get_user2node_status(input.ParamsContainer.Tenant.Id, currentUserId.Value, nodeId.Value,
                    ref isCreator, ref isContributor, ref isExpert, ref isMember, ref isAdminMember,
                    ref isServiceAdmin, ref isAreaAdmin, ref editable, service);
            }

            bool hasKnowledgePermission = false, hasWorkFlowPermission = false,
                hasWFEditPermission = false, hideContributors = false;

            if (isAuthenticated)
            {
                (new CNAPI() { paramsContainer = input.ParamsContainer })
                    .check_node_workflow_permissions(node, isKnowledge, isSystemAdmin,
                    isServiceAdmin, isAreaAdmin, isCreator, ref hasKnowledgePermission,
                    ref hasWorkFlowPermission, ref hasWFEditPermission, ref hideContributors);

                hideContributors = hideContributors && !isSystemAdmin && !isServiceAdmin;
            }

            //isWorkflowDirector = 'some code to determine appropriate value

            List<PermissionType> lstPT = new List<PermissionType>() { PermissionType.View, PermissionType.ViewAbstract };

            bool hasAccess = result && (isSystemAdmin || isServiceAdmin || isAreaAdmin || isCreator || isContributor || isExpert ||
                isMember || hasWorkFlowPermission || hasKnowledgePermission ||
                (currentUserId.HasValue && PrivacyController.check_access(input.ParamsContainer.Tenant.Id,
                    currentUserId.Value, nodeId.Value, PrivacyObjectType.Node, lstPT).Count > 0));

            if (!hasAccess)
            {
                bool membershipButton = input.ParamsContainer.CurrentUserID.HasValue &&
                    CNController.has_extension(input.ParamsContainer.Tenant.Id, nodeId.Value, ExtensionType.Group) &&
                    !CNController.is_node_member(input.ParamsContainer.Tenant.Id, nodeId.Value,
                        input.ParamsContainer.CurrentUserID.Value, null, NodeMemberStatuses.Accepted) &&
                    CNController.has_admin(input.ParamsContainer.Tenant.Id, nodeId.Value);
                bool hasPendingRequest = membershipButton && input.ParamsContainer.CurrentUserID.HasValue &&
                    CNController.is_node_member(input.ParamsContainer.Tenant.Id, nodeId.Value,
                    input.ParamsContainer.CurrentUserID.Value, null, NodeMemberStatuses.Pending);

                input.Data["NodeID"] = nodeId;
                input.Data["NodeAccessDenied"] = true;

                if (!isAuthenticated)
                {
                    input.Data["Name"] = Base64.encode(node.Name);
                    input.Data["PublicDescription"] = Base64.encode(node.PublicDescription);
                    input.Data["NodeType"] = Base64.encode(node.NodeType);
                    input.Data["MembershipButton"] = membershipButton;
                    input.Data["HasPendingRequest"] = hasPendingRequest;
                }

                return;
            }
            //end of Check User access to Node

            input.Data["NodeID"] = nodeId;
            input.Data["ShowWorkFlow"] = hasWorkFlowPermission;
            input.Data["ShowKnowledgeOptions"] = hasKnowledgePermission;
            input.Data["HideContributors"] = hideContributors;

            //register node page visitor
            if ((input.ParamsContainer.Context.Session[nodeId.ToString()] == null) && currentUserId.HasValue)
            {
                input.ParamsContainer.Context.Session[nodeId.ToString()] = true;
                UsersController.register_item_visit(input.ParamsContainer.Tenant.Id,
                    nodeId.Value, currentUserId.Value, DateTime.Now, VisitItemTypes.Node);
            }
            //end of node page visitor registration
        }

        private static void posts(RouteActionParams input)
        {
            Guid? ownerId = PublicMethods.parse_guid(input.ParamsContainer.request_param("OwnerID"));
            PostOwnerType ownerType = PublicMethods.parse_enum<PostOwnerType>(
                input.ParamsContainer.request_param("OwnerType"), PostOwnerType.None);
            Guid? postId = PublicMethods.parse_guid(input.ParamsContainer.request_param("PostID"));

            if (ownerId.HasValue) input.Data["OwnerID"] = ownerId;
            if (ownerType != PostOwnerType.None) input.Data["OwnerType"] = ownerType.ToString();
            if (postId.HasValue) input.Data["PostID"] = postId;
        }

        private static void qa_tag(RouteActionParams input)
        {
            if (!input.ParamsContainer.ApplicationID.HasValue) return;

            Guid? tagId = PublicMethods.parse_guid(input.ParamsContainer.request_param("id"));
            if (!tagId.HasValue) tagId = PublicMethods.parse_guid(input.ParamsContainer.request_param("TagID"));

            Node node = !tagId.HasValue ? null : CNController.get_node(input.ParamsContainer.Tenant.Id, tagId.Value);

            if (node != null)
            {
                input.Data["TagID"] = tagId;
                input.Data["Name"] = Base64.encode(node.Name);
            }
        }

        private static void question(RouteActionParams input)
        {
            if (!input.ParamsContainer.ApplicationID.HasValue) return;

            Guid? questionId = PublicMethods.parse_guid(input.ParamsContainer.request_param("id"));
            if (!questionId.HasValue) questionId = PublicMethods.parse_guid(input.ParamsContainer.request_param("QuestionID"));

            if (questionId.HasValue && input.ParamsContainer.CurrentUserID.HasValue)
            {
                input.Data["QuestionID"] = questionId;

                //register question page visitor
                if (input.ParamsContainer.Context.Session[questionId.ToString()] == null)
                {
                    input.ParamsContainer.Context.Session[questionId.ToString()] = true;
                    UsersController.register_item_visit(input.ParamsContainer.Tenant.Id,
                        questionId.Value, input.ParamsContainer.CurrentUserID.Value, DateTime.Now, VisitItemTypes.Question);
                }
                //end of question page visitor registration
            }
        }

        private static void reports(RouteActionParams input)
        {
            if (!input.ParamsContainer.ApplicationID.HasValue || !input.ParamsContainer.CurrentUserID.HasValue) return;

            List<Guid> lst = ReportUtilities.ReportsList.Select(u => u.ID).ToList();


            //Drop Applications Performance Report
            bool isRefTeam = RaaiVanSettings.SAASBasedMultiTenancy && RaaiVanSettings.ReferenceTenantID.HasValue &&
                RaaiVanSettings.ReferenceTenantID == input.ParamsContainer.ApplicationID;

            if (!isRefTeam)
            {
                Guid? reportId = ReportUtilities.get_report_id(ModuleIdentifier.RV, "ApplicationsPerformanceReport");

                if (reportId.HasValue)
                    lst = lst.Where(id => id != reportId.Value).ToList();
            }
            //end of Drop Applications Performance Report


            if (!PublicMethods.is_system_admin(input.ParamsContainer.Tenant.Id, input.ParamsContainer.CurrentUserID.Value))
            {
                lst = PrivacyController.check_access(input.ParamsContainer.Tenant.Id,
                    input.ParamsContainer.CurrentUserID, lst, PrivacyObjectType.Report, PermissionType.View);
            }

            List<Privacy> privacy = PrivacyController.get_settings(input.ParamsContainer.Tenant.Id,
                ReportUtilities.ReportsList.Select(u => u.ID).ToList());

            Dictionary<string, object> dic = new Dictionary<string, object>();

            lst.Select(reportId => ReportUtilities.ReportsList.Where(x => x.ID == reportId).FirstOrDefault())
                .Where(rpt => rpt != null).ToList().ForEach(rpt =>
                {
                    if (privacy.Any(x => x.ObjectID == rpt.ID))
                        rpt.Confidentiality = privacy.Where(x => x.ObjectID == rpt.ID).FirstOrDefault().Confidentiality;

                    dic[rpt.Name.ToLower()] = PublicMethods.fromJSON(rpt.toJson());
                });

            input.Data["Reports"] = dic;
        }
    }
}