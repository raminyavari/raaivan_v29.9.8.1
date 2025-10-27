using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using RaaiVan.Modules.Privacy;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Log;
using RaaiVan.Modules.Users;
using RaaiVan.Modules.CoreNetwork;
using RaaiVan.Modules.QA;
using RaaiVan.Modules.Reports;
using RaaiVan.Modules.FormGenerator;

namespace RaaiVan.Web.API
{
    /// <summary>
    /// Summary description for PrivacyAPI
    /// </summary>
    public class PrivacyAPI : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        ParamsContainer paramsContainer = null;

        public void ProcessRequest(HttpContext context)
        {
            paramsContainer = new ParamsContainer(context, nullTenantResponse: true);
            if (!paramsContainer.ApplicationID.HasValue) return;

            string responseText = string.Empty;
            string command = PublicMethods.parse_string(context.Request.Params["Command"], false);

            switch (command)
            {
                case "CheckAuthority":
                    check_authority(ListMaker.get_string_items(context.Request.Params["Permissions"], '|'), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "CheckAccess":
                    {
                        check_access(ListMaker.get_guid_items(context.Request.Params["ObjectIDs"], '|'),
                            PublicMethods.parse_enum(context.Request.Params["Type"], defaultValue: PrivacyObjectType.None),
                            ListMaker.get_enum_items<PermissionType>(context.Request.Params["Permissions"], '|')
                                .Where(p => p != PermissionType.None).ToList(),
                            ref responseText);
                        _return_response(ref responseText);
                    }
                    return;
                case "SetAudience":
                    {
                        Dictionary<string, object> data =
                            PublicMethods.fromJSON(PublicMethods.parse_string(context.Request.Params["Data"]));

                        List<Privacy> items = new List<Privacy>();
                        
                        foreach (string k in data.Keys)
                        {
                            Guid objId = Guid.Empty;

                            if (!Guid.TryParse(k, out objId) || data[k].GetType() != typeof(Dictionary<string, object>))
                                continue;

                            Privacy p = Privacy.fromJson((Dictionary<string, object>)data[k]);

                            if (p != null)
                            {
                                p.ObjectID = objId;
                                items.Add(p);
                            }
                        }

                        set_audience(items,
                            PublicMethods.parse_enum(context.Request.Params["ObjectType"], defaultValue: PrivacyObjectType.None), 
                            ref responseText);
                        _return_response(ref responseText);
                        return;
                    }
                case "GetAudience":
                    {
                        get_audience(ListMaker.get_guid_items(context.Request.Params["ObjectIDs"], '|'),
                            PublicMethods.parse_enum(context.Request.Params["ObjectType"], defaultValue: PrivacyObjectType.None), 
                            ref responseText);
                        _return_response(ref responseText);
                        return;
                    }
                case "AddConfidentialityLevel":
                    add_confidentiality_level(PublicMethods.parse_int(context.Request.Params["LevelID"]),
                        PublicMethods.parse_string(context.Request.Params["Title"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "ModifyConfidentialityLevel":
                    modify_confidentiality_level(PublicMethods.parse_guid(context.Request.Params["ConfidentialityID"]),
                        PublicMethods.parse_int(context.Request.Params["LevelID"]),
                        PublicMethods.parse_string(context.Request.Params["Title"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RemoveConfidentialityLevel":
                    remove_confidentiality_level(PublicMethods.parse_guid(context.Request.Params["ConfidentialityID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetConfidentialityLevels":
                    get_confidentiality_levels(ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetConfidentialityLevel":
                    set_confidentiality_level(PublicMethods.parse_guid(context.Request.Params["ObjectID"]),
                        PublicMethods.parse_guid(context.Request.Params["ConfidentialityID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "UnsetConfidentialityLevel":
                    unset_confidentiality_level(PublicMethods.parse_guid(context.Request.Params["ObjectID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetConfidentialityLevel":
                    get_confidentiality_level(PublicMethods.parse_guid(context.Request.Params["ObjectID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetConfidentialityLevelUsers":
                    get_confidentiality_level_users(PublicMethods.parse_guid(context.Request.Params["ConfidentialityID"]),
                        PublicMethods.parse_string(context.Request.Params["SearchText"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_long(context.Request.Params["LowerBoundary"]),
                        ref responseText);
                    _return_response(ref responseText);
                    return;
            }

            paramsContainer.return_response(PublicConsts.BadRequestResponse);
        }

        protected void _return_response(ref string responseText)
        {
            paramsContainer.return_response(ref responseText);
        }

        protected void _save_error_log(Modules.Log.Action action, List<Guid> subjectIds,
            Guid? secondSubjectId = null, string info = null)
        {
            try
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = action,
                    SubjectIDs = subjectIds,
                    SecondSubjectID = secondSubjectId,
                    Info = info,
                    ModuleIdentifier = ModuleIdentifier.PRVC
                });
            }
            catch { }
        }

        protected void _save_error_log(Modules.Log.Action action, Guid? subjectId,
            Guid? secondSubjectId = null, string info = null)
        {
            if (!subjectId.HasValue) return;
            _save_error_log(action, new List<Guid>() { subjectId.Value }, secondSubjectId, info);
        }

        protected void check_authority(List<string> permissions, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (permissions.Count == 0)
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }

            List<AccessRoleName> roles = new List<AccessRoleName>();

            foreach (string str in permissions)
            {
                AccessRoleName rl = AccessRoleName.None;
                if (Enum.TryParse<AccessRoleName>(str, out rl) && rl != AccessRoleName.None) roles.Add(rl);
            }

            List<AccessRoleName> accessRoles = AuthorizationManager.has_right(roles, paramsContainer.CurrentUserID);

            responseText = "{" + string.Join(",", roles.Select(
                u => "\"" + u.ToString() + "\":" + accessRoles.Any(x => x == u).ToString().ToLower())) + "}";
        }

        protected void check_access(List<Guid> objectIds, PrivacyObjectType objectType, 
            List<PermissionType> permissions, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;

            if (permissions.Count > 0) permissions = permissions.Where(p => p != PermissionType.None).ToList();

            Dictionary<Guid, List<PermissionType>> results = objectType == PrivacyObjectType.None ?
                new Dictionary<Guid, List<PermissionType>>() : (permissions.Count == 0 ?
                PrivacyController.check_access(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID, objectIds, objectType) :
                PrivacyController.check_access(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID, objectIds, objectType, permissions));

            responseText = "{" + string.Join(",", results.Keys.Select(
                k => "\"" + k + "\":[" + string.Join(",", results[k].Select(p => "\"" + p.ToString() + "\"")) + "]")) + "}";
        }

        protected bool check_object_type(List<Guid> objectIds, PrivacyObjectType objectType)
        {
            switch (objectType)
            {
                case PrivacyObjectType.None:
                case PrivacyObjectType.Node:
                case PrivacyObjectType.NodeType:
                    {
                        if (objectIds.Count != 1) return false;

                        bool isNodeType = objectType != PrivacyObjectType.Node &&
                            CNController.is_node_type(paramsContainer.Tenant.Id, objectIds).Count == objectIds.Count;
                        bool isNode = !isNodeType && objectType != PrivacyObjectType.NodeType &&
                            CNController.is_node(paramsContainer.Tenant.Id, objectIds).Count == objectIds.Count;

                        if (!isNodeType && !isNode) return false;

                        bool accessPermission =
                            AuthorizationManager.has_right(AccessRoleName.ManageOntology, paramsContainer.CurrentUserID);

                        if (!accessPermission) accessPermission = CNController.is_service_admin(paramsContainer.Tenant.Id,
                                objectIds[0], paramsContainer.CurrentUserID.Value);

                        if (!accessPermission && isNode)
                        {
                            accessPermission = CNController.is_node_admin(paramsContainer.Tenant.Id,
                                paramsContainer.CurrentUserID.Value, objectIds[0], null, null, null) ||
                                PrivacyController.check_access(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID,
                                    objectIds[0], PrivacyObjectType.Node, PermissionType.Modify);
                        }

                        if (!accessPermission && isNode && CNController.is_knowledge(paramsContainer.Tenant.Id, objectIds[0]))
                        {
                            Node nd = CNController.get_node(paramsContainer.Tenant.Id, objectIds[0]);

                            accessPermission = nd != null && nd.NodeID.HasValue &&
                                nd.isPersonal(paramsContainer.CurrentUserID.Value);
                        }

                        return accessPermission;
                    }
                case PrivacyObjectType.FAQCategory:
                    return QAController.is_faq_category(paramsContainer.Tenant.Id, objectIds).Count == objectIds.Count &&
                        (AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID) ||
                        QAController.is_workflow_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, null));
                case PrivacyObjectType.QAWorkFlow:
                    return QAController.is_workflow(paramsContainer.Tenant.Id, objectIds).Count == objectIds.Count &&
                        AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID);
                case PrivacyObjectType.Poll:
                    return FGController.is_poll(paramsContainer.Tenant.Id, objectIds).Count == objectIds.Count &&
                        AuthorizationManager.has_right(AccessRoleName.ManagePolls, paramsContainer.CurrentUserID);
                case PrivacyObjectType.Report:
                    return !objectIds.Any(u => !ReportUtilities.ReportsList.Any(x => x.ID == u)) &&
                        PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value);
                case PrivacyObjectType.FormElement:
                    return FGController.is_form_element(paramsContainer.Tenant.Id, objectIds).Count == objectIds.Count &&
                        AuthorizationManager.has_right(AccessRoleName.ManageForms, paramsContainer.CurrentUserID);
            }

            return false;
        }

        protected void set_audience(List<Privacy> items, PrivacyObjectType objectType, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (items.Count != 0 &&
                !check_object_type(items.Where(x => x.ObjectID.HasValue).Select(u => u.ObjectID.Value).ToList(), objectType))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (items.Count == 1 && items[0].ObjectID.HasValue)
                    _save_error_log(Modules.Log.Action.SetPrivacyAudience_PermissionFailed, items[0].ObjectID);
                return;
            }

            bool result = paramsContainer.CurrentUserID.HasValue &&
                PrivacyController.set_audience(paramsContainer.Tenant.Id, items, paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result && items.Count == 1)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetPrivacyAudience,
                    SubjectID = items[0].ObjectID,
                    Info = items[0].toJson(),
                    ModuleIdentifier = ModuleIdentifier.PRVC
                });
            }
            //end of Save Log
        }

        protected string _get_audience_json(ref List<Audience> audience)
        {
            string retStr = "[";

            bool isFirst = true;
            foreach (Audience au in audience)
            {
                string roleName = au.RoleName;

                Base64.encode(roleName, ref roleName);

                if (!au.Allow.HasValue) au.Allow = false;

                if (!isFirst) retStr += ",";
                isFirst = false;

                retStr += "{\"RoleID\":\"" + au.RoleID.Value.ToString() + "\"" +
                    ",\"RoleName\":\"" + roleName + "\"" +
                    ",\"Allow\":" + au.Allow.Value.ToString().ToLower() +
                    "}";
            }

            return retStr + "]";
        }

        protected void get_audience(List<Guid> objectIds, PrivacyObjectType objectType, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            bool accessDenied = objectIds.Count == 0 || !check_object_type(objectIds, objectType);
            
            Dictionary<Guid, List<Audience>> audience = accessDenied ? new Dictionary<Guid, List<Audience>>() :
                PrivacyController.get_audience(paramsContainer.Tenant.Id, objectIds);

            Dictionary<Guid, List<DefaultPermission>> defaultPermissions = accessDenied ? new Dictionary<Guid, List<DefaultPermission>>() :
                PrivacyController.get_default_permissions(paramsContainer.Tenant.Id, objectIds);

            List<Privacy> settings = accessDenied ? new List<Privacy>() :
                PrivacyController.get_settings(paramsContainer.Tenant.Id, objectIds);

            objectIds.ForEach(u => {
                if (!settings.Any(x => x.ObjectID == u)) settings.Add(new Privacy() { ObjectID = u });
            });

            settings.ForEach(s => {
                if (audience.ContainsKey(s.ObjectID.Value)) s.Audience = audience[s.ObjectID.Value];
                if (defaultPermissions.ContainsKey(s.ObjectID.Value))
                    s.DefaultPermissions = defaultPermissions[s.ObjectID.Value];
            });

            List<ConfidentialityLevel> levels = PrivacyController.get_confidentiality_levels(paramsContainer.Tenant.Id);

            responseText = "{\"ConfidentialityLevels\":[" + string.Join(",", levels.Select(x => x.toJson())) + "]" +
                ",\"Items\":{" + string.Join(",", settings.Where(x => x.ObjectID.HasValue)
                    .Select(u => "\"" + u.ObjectID.ToString() + "\":" + u.toJson())) + "}" +
                (!accessDenied ? string.Empty : ",\"ErrorText\":\"" + Messages.AccessDenied + "\"") +
                "}";
        }

        protected void add_confidentiality_level(int? levelId, string title, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.ManageConfidentialityLevels, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.AddConfidentialityLevel_PermissionFailed, Guid.Empty);
                return;
            }

            Guid id = Guid.NewGuid();
            string errorMessage = string.Empty;

            bool succeed = !levelId.HasValue ? false :
                PrivacyController.add_confidentiality_level(paramsContainer.Tenant.Id, id, levelId.Value, title,
                paramsContainer.CurrentUserID.Value, ref errorMessage);

            responseText = succeed ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"" +
                ",\"Level\":" + (new ConfidentialityLevel() { ID = id, LevelID = levelId, Title = title }).toJson() + "}" :
                "{\"ErrorText\":\"" + (string.IsNullOrEmpty(errorMessage) ? Messages.OperationFailed.ToString() : errorMessage) + "\"}";

            //Save Log
            if (succeed)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.AddConfidentialityLevel,
                    SubjectID = id,
                    Info = "{\"LevelID\":" + levelId.ToString() + ",\"Title\":\"" + Base64.encode(title) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.PRVC
                });
            }
            //end of Save Log
        }

        protected void modify_confidentiality_level(Guid? confidentialityId, int? levelId, string title, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.ManageConfidentialityLevels, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.ModifyConfidentialityLevel_PermissionFailed, confidentialityId);
                return;
            }

            string errorMessage = string.Empty;

            bool succeed = !confidentialityId.HasValue || !levelId.HasValue ? false :
                PrivacyController.modify_confidentiality_level(paramsContainer.Tenant.Id,
                confidentialityId.Value, levelId.Value, title, paramsContainer.CurrentUserID.Value, ref errorMessage);

            responseText = succeed ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + (string.IsNullOrEmpty(errorMessage) ? Messages.OperationFailed.ToString() : errorMessage) + "\"}";

            //Save Log
            if (succeed)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.ModifyConfidentialityLevel,
                    SubjectID = confidentialityId,
                    Info = "{\"LevelID\":" + levelId.ToString() + ",\"Title\":\"" + Base64.encode(title) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.PRVC
                });
            }
            //end of Save Log
        }

        protected void remove_confidentiality_level(Guid? confidentialityId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.ManageConfidentialityLevels, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.RemoveConfidentialityLevel_PermissionFailed, confidentialityId);
                return;
            }

            string errorMessage = string.Empty;

            bool succeed = !confidentialityId.HasValue ? false :
                PrivacyController.remove_confidentiality_level(paramsContainer.Tenant.Id,
                confidentialityId.Value, paramsContainer.CurrentUserID.Value);

            responseText = succeed ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + (string.IsNullOrEmpty(errorMessage) ? Messages.OperationFailed.ToString() : errorMessage) + "\"}";

            //Save Log
            if (succeed)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RemoveConfidentialityLevel,
                    SubjectID = confidentialityId,
                    ModuleIdentifier = ModuleIdentifier.PRVC
                });
            }
            //end of Save Log
        }

        protected void get_confidentiality_levels(ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            List<ConfidentialityLevel> levels = PrivacyController.get_confidentiality_levels(paramsContainer.Tenant.Id)
                .OrderBy(u => u.LevelID).ToList();

            responseText = "{\"Levels\":[" + string.Join(",", levels.Select(u => u.toJson())) + "]}";
        }

        protected void set_confidentiality_level(Guid? objectId, Guid? confidentialityId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            bool isNode = objectId.HasValue && CNController.is_node(paramsContainer.Tenant.Id, objectId.Value);
            bool isUser = !isNode;

            bool accessDenied = false;

            if (isUser)
            {
                accessDenied = !objectId.HasValue ||
                    !AuthorizationManager.has_right(AccessRoleName.ManageConfidentialityLevels, paramsContainer.CurrentUserID);
            }
            else
            {
                accessDenied = !objectId.HasValue || (
                    !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) &&
                    !CNController.is_service_admin(paramsContainer.Tenant.Id,
                        objectId.Value, paramsContainer.CurrentUserID.Value) &&
                    !CNController.is_node_admin(paramsContainer.Tenant.Id,
                        paramsContainer.CurrentUserID.Value, objectId.Value, null, null, null));
            }

            if (accessDenied)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (objectId.HasValue)
                    _save_error_log(Modules.Log.Action.SetConfidentialityLevel_PermissionFailed, objectId, confidentialityId);
                return;
            }

            bool succeed = !objectId.HasValue || !confidentialityId.HasValue ? false :
                PrivacyController.set_confidentiality_level(paramsContainer.Tenant.Id,
                objectId.Value, confidentialityId.Value, paramsContainer.CurrentUserID.Value);

            responseText = succeed ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (succeed)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetConfidentialityLevel,
                    SubjectID = objectId,
                    SecondSubjectID = confidentialityId,
                    ModuleIdentifier = ModuleIdentifier.PRVC
                });
            }
            //end of Save Log
        }

        protected void unset_confidentiality_level(Guid? objectId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            bool isNode = objectId.HasValue && CNController.is_node(paramsContainer.Tenant.Id, objectId.Value);
            bool isUser = !isNode;

            bool accessDenied = false;

            if (isUser)
            {
                accessDenied = !objectId.HasValue ||
                    !AuthorizationManager.has_right(AccessRoleName.ManageConfidentialityLevels, paramsContainer.CurrentUserID);
            }
            else
            {
                accessDenied = !objectId.HasValue || (
                    !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) &&
                    !CNController.is_service_admin(paramsContainer.Tenant.Id,
                        objectId.Value, paramsContainer.CurrentUserID.Value) &&
                    !CNController.is_node_admin(paramsContainer.Tenant.Id,
                        paramsContainer.CurrentUserID.Value, objectId.Value, null, null, null));
            }

            if (accessDenied)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (objectId.HasValue) _save_error_log(Modules.Log.Action.UnsetConfidentialityLevel_PermissionFailed, objectId);
                return;
            }

            bool succeed = objectId.HasValue && PrivacyController.unset_confidentiality_level(paramsContainer.Tenant.Id,
                objectId.Value, paramsContainer.CurrentUserID.Value);

            responseText = succeed ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (succeed)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.UnsetConfidentialityLevel,
                    SubjectID = objectId,
                    ModuleIdentifier = ModuleIdentifier.PRVC
                });
            }
            //end of Save Log
        }

        protected void get_confidentiality_level(Guid? objectId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            Privacy p = !objectId.HasValue ? new Privacy() :
                PrivacyController.get_settings(paramsContainer.Tenant.Id, objectId.Value);

            responseText = p.Confidentiality.toJson();
        }

        protected void get_confidentiality_level_users(Guid? confidentialityId, 
            string searchText, int? count, long? lowerBoundary, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.ManageConfidentialityLevels, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            long totalCount = 0;

            List<User> users = !confidentialityId.HasValue ? new List<User>() :
                UsersController.get_users(paramsContainer.Tenant.Id,
                PrivacyController.get_confidentiality_level_user_ids(paramsContainer.Tenant.Id,
                confidentialityId.Value, searchText, count, lowerBoundary, ref totalCount));

            responseText = "{\"TotalCount\":" + totalCount.ToString() +
                ",\"Users\":[" + ProviderUtil.list_to_string<string>(users.Select(
                    u => "{\"UserID\":\"" + u.UserID.ToString() + "\"" +
                        ",\"UserName\":\"" + Base64.encode(u.UserName) + "\"" +
                        ",\"FirstName\":\"" + Base64.encode(u.FirstName) + "\"" +
                        ",\"LastName\":\"" + Base64.encode(u.LastName) + "\"" +
                        ",\"ProfileImageURL\":\"" + DocumentUtilities.get_personal_image_address(
                            paramsContainer.Tenant.Id, u.UserID.Value) + "\"" +
                        "}"
                    ).ToList()) + "]" +
                "}";
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}