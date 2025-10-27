using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.NotificationCenter;
using NC = RaaiVan.Modules.NotificationCenter;
using RaaiVan.Modules.Users;
using RaaiVan.Modules.Log;
using RaaiVan.Modules.CoreNetwork;

namespace RaaiVan.Web.API
{
    /// <summary>
    /// Summary description for NotificationsAPI
    /// </summary>
    public class NotificationsAPI : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        private ParamsContainer paramsContainer = null;

        public void ProcessRequest(HttpContext context)
        {
            paramsContainer = new ParamsContainer(context, nullTenantResponse: true);
            if (!paramsContainer.ApplicationID.HasValue) return;

            string responseText = string.Empty;
            string command = PublicMethods.parse_string(context.Request.Params["Command"], false);

            switch (command)
            {
                case "GetNotificationsCount":
                    get_notifications_count(ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetNotifications":
                    get_notifications(PublicMethods.parse_bool(context.Request.Params["Seen"]),
                        PublicMethods.parse_int(context.Request.Params["Count"], 10),
                        PublicMethods.parse_long(context.Request.Params["LastNotSeenID"]),
                        PublicMethods.parse_long(context.Request.Params["LastSeenID"]),
                        PublicMethods.parse_date(context.Request.Params["LastViewDate"]),
                        PublicMethods.parse_date(context.Request.Params["LowerDateLimit"]),
                        PublicMethods.parse_date(context.Request.Params["UpperDateLimit"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetNotificationsAsSeen":
                    set_notifications_as_seen(ListMaker.get_long_items(context.Request.Params["NotificationIDs"], '|'),
                        ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetUserNotificationsAsSeen":
                    set_user_notifications_as_seen(ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RemoveNotification":
                    remove_notification(PublicMethods.parse_long(context.Request.Params["NotificationID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetDashboardTypes":
                    get_dashboard_types(ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetDashboardsAsSeen":
                    set_dashboards_as_seen(ListMaker.get_long_items(context.Request.Params["DashboardIDs"], '|'), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RemoveDashboards":
                    remove_dashboards(ListMaker.get_long_items(context.Request.Params["DashboardIDs"], '|'), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetDashboardsCount":
                    {
                        get_dashboards_count(PublicMethods.parse_guid(context.Request.Params["UserID"]),
                            PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                            PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                            PublicMethods.parse_string(context.Request.Params["NodeAdditionalID"], false),
                            PublicMethods.parse_enum(context.Request.Params["Type"], defaultValue: DashboardType.NotSet), 
                            ref responseText);
                        _return_response(ref responseText);
                    }
                    return;
                case "GetDashboards":
                    {
                        get_dashboards(PublicMethods.parse_guid(context.Request.Params["UserID"]),
                            PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                            PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                            PublicMethods.parse_string(context.Request.Params["NodeAdditionalID"]),
                            PublicMethods.parse_string(context.Request.Params["SearchText"]),
                            PublicMethods.parse_enum(context.Request.Params["Type"], defaultValue: DashboardType.NotSet),
                            PublicMethods.parse_enum(context.Request.Params["SubType"], defaultValue: DashboardSubType.NotSet),
                            PublicMethods.parse_string(context.Request.Params["SubTypeTitle"]),
                            PublicMethods.parse_bool(context.Request.Params["Done"]),
                            PublicMethods.parse_date(context.Request.Params["DateFrom"]),
                            PublicMethods.parse_date(context.Request.Params["DateTo"], 1),
                            PublicMethods.parse_bool(context.Request.Params["DistinctItems"]),
                            PublicMethods.parse_bool(context.Request.Params["InWorkFlow"]),
                            PublicMethods.parse_int(context.Request.Params["LowerBoundary"]),
                            PublicMethods.parse_int(context.Request.Params["Count"], 50), ref responseText);
                        _return_response(ref responseText);
                    }
                    return;
                case "GetCurrentDashboards":
                    {
                        get_node_dashboards(PublicMethods.parse_guid(context.Request.Params["NodeTypeID"]),
                            PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                            PublicMethods.parse_string(context.Request.Params["NodeAdditionalID"], false),
                            ref responseText);
                        _return_response(ref responseText);
                    }
                    return;
                case "SetMessageTemplate":
                    set_message_template(PublicMethods.parse_guid(context.Request.Params["TemplateID"]),
                        PublicMethods.parse_guid(context.Request.Params["OwnerID"]),
                        PublicMethods.parse_string(context.Request.Params["BodyText"]),
                        PublicMethods.parse_enum(context.Request.Params["AudienceType"], defaultValue: AudienceType.NotSet),
                        PublicMethods.parse_guid(context.Request.Params["AudienceRefOwnerID"]),
                        PublicMethods.parse_guid(context.Request.Params["AudienceNodeID"]),
                        PublicMethods.parse_bool(context.Request.Params["AudienceNodeAdmin"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RemoveMessageTemplate":
                    remove_message_template(PublicMethods.parse_guid(context.Request.Params["TemplateID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetOwnerMessageTemplates":
                    get_owner_message_templates(PublicMethods.parse_guid(context.Request.Params["OwnerID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                //Notification Messages
                case "SetAdminMessagingActivation":
                    {
                        set_admin_messaging_activation(PublicMethods.parse_guid(context.Request.Params["TemplateID"]),
                            PublicMethods.parse_enum(context.Request.Params["SubjectType"], defaultValue: SubjectType.None),
                            PublicMethods.parse_enum(context.Request.Params["Action"], defaultValue: ActionType.None),
                            PublicMethods.parse_enum(context.Request.Params["Media"], defaultValue: Media.None),
                            PublicMethods.parse_enum(context.Request.Params["UserStatus"], defaultValue: UserStatus.None),
                            PublicMethods.parse_string(context.Request.Params["Lang"], false, "fa"),
                            PublicMethods.parse_bool(context.Request.Params["Enable"]), ref responseText);
                        _return_response(ref responseText);
                    }
                    break;
                case "SetUserMessagingActivation":
                    {
                        set_user_messaging_activation(PublicMethods.parse_guid(context.Request.Params["OptionID"]),
                            PublicMethods.parse_guid(context.Request.Params["UserID"]),
                            PublicMethods.parse_enum(context.Request.Params["SubjectType"], defaultValue: SubjectType.None),
                            PublicMethods.parse_enum(context.Request.Params["UserStatus"], defaultValue: UserStatus.None),
                            PublicMethods.parse_enum(context.Request.Params["Action"], defaultValue: ActionType.None),
                            PublicMethods.parse_enum(context.Request.Params["Media"], defaultValue: Media.None),
                            PublicMethods.parse_string(context.Request.Params["Lang"], false, "fa"),
                            PublicMethods.parse_bool(context.Request.Params["Enable"]), ref responseText);
                        _return_response(ref responseText);
                    }
                    return;
                case "SetNotificationMessageTemplateText":
                    set_notification_message_template_text(PublicMethods.parse_guid(context.Request.Params["TemplateID"]),
                        PublicMethods.parse_string(context.Request.Params["Subject"]),
                        PublicMethods.parse_string(context.Request.Params["Text"]),
                        ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetUserMessagingActivation":
                    get_user_messaging_activation(PublicMethods.parse_guid(context.Request.Params["UserID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetNotificationMessageTemplatesInfo":
                    get_notification_message_templates_info(ref responseText);
                    _return_response(ref responseText);
                    return;
                    //end of Notification Messages
            }

            paramsContainer.return_response(PublicConsts.BadRequestResponse);
        }

        protected void _return_response(ref string responseText)
        {
            paramsContainer.return_response(ref responseText);
        }

        protected void get_notifications_count(ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!Modules.RaaiVanConfig.Modules.Notifications(paramsContainer.Tenant.Id))
            {
                responseText = "{\"Destroy\":true}";
                return;
            }

            int count = NotificationController.get_user_notifications_count(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value);
            responseText = "{\"Count\":\"" + count.ToString() + "\"}";
        }

        protected void get_notifications(bool? seen, int? count, long? lastNotSeenId, long? lastSeenId,
            DateTime? lastViewDate, DateTime? lowerDateLimit, DateTime? upperDateLimit, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!RaaiVan.Modules.RaaiVanConfig.Modules.Notifications(paramsContainer.Tenant.Id))
            {
                responseText = "{\"Destroy\":true}";
                return;
            }

            DateTime now = DateTime.Now;

            List<Notification> nots = NotificationController.get_user_notifications(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, seen, lastNotSeenId, lastSeenId, lastViewDate,
                lowerDateLimit, upperDateLimit, count);

            List<User> users = UsersController.get_users(paramsContainer.Tenant.Id,
                nots.Where(u => u.Sender.UserID.HasValue).Select(v => v.Sender.UserID.Value).ToList());

            nots.ForEach(nt => nt.Sender = users.Where(u => u.UserID == nt.Sender.UserID).FirstOrDefault());

            Notification lastSeen = nots.Where(u => u.Seen == true).LastOrDefault();
            Notification lastNotSeen = nots.Where(u => !u.Seen.HasValue || u.Seen == false).LastOrDefault();

            responseText = "{\"LastSeenID\":\"" + (lastSeen == null ? string.Empty : lastSeen.NotificationID.Value.ToString()) + "\"" +
                ",\"LastNotSeenID\":\"" + (lastNotSeen == null ? string.Empty : lastNotSeen.NotificationID.Value.ToString()) + "\"" +
                ",\"Now\":\"" + now.ToString() + "\"" +
                ",\"Notifications\":[" + string.Join(",", nots.Select(nt => nt.toJson())) + "]" +
                "}";
        }

        protected void set_notifications_as_seen(List<long> notificationIds, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            bool result = NotificationController.set_notifications_as_seen(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, notificationIds);
            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void set_user_notifications_as_seen(ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            bool result = NotificationController.set_user_notifications_as_seen(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void remove_notification(long? notificationId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            bool result = notificationId.HasValue && NotificationController.remove_notification(paramsContainer.Tenant.Id,
                notificationId.Value, paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void get_dashboard_types(ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            responseText = "{\"DashboardTypes\":[" + ProviderUtil.list_to_string<string>(
                NotificationUtilities.get_dashboard_types().Select(u => "\"" + u.ToString() + "\"").ToList()) + "]}";
        }

        protected void set_dashboards_as_seen(List<long> dashboardIds, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            responseText = NotificationController.set_dashboards_as_seen(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, dashboardIds) ?
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void remove_dashboards(List<long> dashboardIds, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            responseText = NotificationController.remove_dashboards(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, dashboardIds) ?
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void get_dashboards_count(Guid? userId, Guid? nodeTypeId, Guid? nodeId,
            string nodeAdditionalId, DashboardType type, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!userId.HasValue) userId = paramsContainer.CurrentUserID;

            if (userId != paramsContainer.CurrentUserID &&
                !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value))
                userId = paramsContainer.CurrentUserID.Value;

            List<DashboardCount> items = NotificationController.get_dashboards_count(paramsContainer.Tenant.Id,
                userId.Value, nodeTypeId, nodeId, nodeAdditionalId, type);

            responseText = "{\"ToBeDone\":" + items.Sum(u => u.ToBeDone).ToString() +
                ",\"NotSeen\":" + items.Sum(u => u.NotSeen).ToString() +
                ",\"Done\":" + items.Sum(u => u.Done).ToString() +
                ",\"DoneAndInWorkFlow\":" + items.Sum(u => u.DoneAndInWorkFlow).ToString() +
                ",\"DoneAndNotInWorkFlow\":" + items.Sum(u => u.DoneAndNotInWorkFlow).ToString() +
                ",\"Items\":[" + string.Join(",", items.Select(u => u.toJson())) + "]" +
                "}";
        }

        protected string _get_dashboard_json(Dashboard dash, User usr)
        {
            string strUser = "";
            if (usr != null)
            {
                strUser = "{\"UserID\":\"" + usr.UserID.Value.ToString() + "\"" +
                    ",\"FirstName\":\"" + Base64.encode(usr.FirstName) + "\"" +
                    ",\"LastName\":\"" + Base64.encode(usr.LastName) + "\"" +
                    ",\"UserName\":\"" + Base64.encode(usr.UserName) + "\"" +
                    "}";
            }

            return "{\"DashboardID\":\"" + dash.DashboardID.ToString() + "\"" +
                    ",\"UserID\":\"" + dash.UserID.ToString() + "\"" +
                    (usr == null ? string.Empty : ",\"User\":" + strUser) +
                    ",\"NodeID\":\"" + dash.NodeID.ToString() + "\"" +
                    ",\"NodeAdditionalID\":\"" + Base64.encode(dash.NodeAdditionalID) + "\"" +
                    ",\"NodeName\":\"" + Base64.encode(dash.NodeName) + "\"" +
                    ",\"NodeType\":\"" + Base64.encode(dash.NodeType) + "\"" +
                    ",\"Type\":\"" + (dash.Type == DashboardType.NotSet ? string.Empty : dash.Type.ToString()) + "\"" +
                    ",\"SubType\":\"" + (dash.SubType == DashboardSubType.NotSet ? string.Empty : dash.SubType.ToString()) + "\"" +
                    ",\"Info\":\"" + Base64.encode(dash.Info) + "\"" +
                    ",\"Removable\":" + ((!dash.Done.HasValue || !dash.Done.Value) &&
                        dash.Removable.HasValue && dash.Removable.Value).ToString().ToLower() +
                    ",\"SenderUserID\":\"" + (dash.SenderUserID.HasValue ? dash.SenderUserID.ToString() : string.Empty) + "\"" +
                    ",\"SendDate\":\"" + (dash.SendDate.HasValue ?
                        GenericDate.get_local_date(dash.SendDate.Value, true) : string.Empty) + "\"" +
                    ",\"ExpirationDate\":\"" + (dash.ExpirationDate.HasValue ?
                        GenericDate.get_local_date(dash.ExpirationDate.Value, true) : string.Empty) + "\"" +
                    ",\"Seen\":" + ((dash.Done.HasValue && dash.Done.Value) ||
                        (dash.Seen.HasValue && dash.Seen.Value)).ToString().ToLower() +
                    ",\"ViewDate\":\"" + (dash.ViewDate.HasValue ?
                        GenericDate.get_local_date(dash.ViewDate.Value, true) : string.Empty) + "\"" +
                    ",\"Done\":" + (dash.Done.HasValue ? dash.Done : false).ToString().ToLower() +
                    ",\"ActionDate\":\"" + (dash.ActionDate.HasValue ?
                        GenericDate.get_local_date(dash.ActionDate.Value, true) : string.Empty) + "\"" +
                    "}";
        }

        protected void get_dashboards(Guid? userId, Guid? nodeTypeId, Guid? nodeId, string nodeAdditionalId, string searchText,
            DashboardType dashboardType, DashboardSubType subType, string subTypeTitle, bool? done, DateTime? dateFrom, DateTime? dateTo,
            bool? distinctItems, bool? inWorkFlow, int? lowerBoundary, int? count, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!userId.HasValue) userId = paramsContainer.CurrentUserID.Value;

            bool isSystemAdmin = PublicMethods.is_system_admin(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value);
            bool isServiceAdmin = isSystemAdmin || (nodeId.HasValue && nodeId != Guid.Empty &&
                CNController.is_service_admin(paramsContainer.Tenant.Id, nodeId.Value, paramsContainer.CurrentUserID.Value));

            if (userId != paramsContainer.CurrentUserID && !isSystemAdmin) userId = paramsContainer.CurrentUserID.Value;

            if (subType != DashboardSubType.NotSet || string.IsNullOrEmpty(subTypeTitle)) subTypeTitle = null;

            if (nodeTypeId.HasValue && !nodeId.HasValue && !string.IsNullOrEmpty(searchText))
            {
                nodeId = CNController.get_node_id(paramsContainer.Tenant.Id, searchText, nodeTypeId.Value);
                if (nodeId.HasValue && nodeId != Guid.Empty) searchText = null;
            }

            long totalCount = 0;

            if (distinctItems.HasValue && distinctItems.Value)
            {
                List<Guid> nodeIds = NotificationController.get_dashboards(paramsContainer.Tenant.Id, userId, nodeTypeId, nodeId,
                    dashboardType, DashboardSubType.NotSet, null, searchText, inWorkFlow, lowerBoundary, count, ref totalCount);

                List<Node> nodes = CNController.get_nodes(applicationId: paramsContainer.Tenant.Id, nodeIds: nodeIds);

                responseText = "{\"TotalCount\":" + totalCount.ToString() +
                    ",\"Items\":[" + string.Join(",", nodes.Select(u => u.toJson())) + "]}";

                return;
            }

            List<Dashboard> dashboards = NotificationController.get_dashboards(paramsContainer.Tenant.Id, userId, nodeTypeId, nodeId,
                nodeAdditionalId, dashboardType, subType, subTypeTitle, done, dateFrom, dateTo, searchText, lowerBoundary, count, ref totalCount);

            responseText = "{\"TotalCount\":" + totalCount.ToString() +
                ",\"Items\":[" + string.Join(",", dashboards.Select(u => _get_dashboard_json(u, null))) + "]" +
                "}";
        }

        protected void get_node_dashboards(Guid? nodeTypeId, Guid? nodeId, string nodeAdditionalId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(nodeAdditionalId))
                nodeAdditionalId = PublicMethods.convert_numbers_from_local(nodeAdditionalId).Trim();

            bool isSystemAdmin = PublicMethods.is_system_admin(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value);
            bool isServiceAdmin = isSystemAdmin || (nodeId.HasValue && nodeId != Guid.Empty &&
                CNController.is_service_admin(paramsContainer.Tenant.Id, nodeId.Value, paramsContainer.CurrentUserID.Value));

            List<Dashboard> currentDashboards = new List<Dashboard>();
            List<User> currentOwners = new List<User>();
            if (((nodeId.HasValue && nodeId != Guid.Empty) || !string.IsNullOrEmpty(nodeAdditionalId)) &&
                (isSystemAdmin || isServiceAdmin || CNController.is_node_creator(paramsContainer.Tenant.Id,
                nodeId, nodeAdditionalId, paramsContainer.CurrentUserID.Value)))
            {
                currentDashboards = NotificationController.get_dashboards(paramsContainer.Tenant.Id, null, nodeTypeId, nodeId,
                    nodeAdditionalId, DashboardType.NotSet, DashboardSubType.NotSet, null, false, null, null, null, null, 100);

                if (currentDashboards.Count > 0) currentOwners = UsersController.get_users(paramsContainer.Tenant.Id,
                         currentDashboards.Select(u => u.UserID.Value).ToList());
            }

            //Remove invalid items
            List<DashboardSubType> invalidTypes = new List<DashboardSubType>() {
                DashboardSubType.EvaluationDone,
                DashboardSubType.KnowledgeComment
            };

            currentDashboards = currentDashboards.Where(u => !invalidTypes.Any(x => x == u.SubType &&
                !(u.Type == DashboardType.WorkFlow && u.Removable.HasValue && u.Removable.Value))).ToList();
            //end of Remove invalid items

            responseText = "{\"Items\":[" + string.Join(",", currentDashboards.Select(u =>
                _get_dashboard_json(u, currentOwners.Where(v => v.UserID == u.UserID).FirstOrDefault()))) + "]" +
                "}";
        }

        protected string _get_message_template_json(MessageTemplate template)
        {
            return "{\"TemplateID\":\"" + template.TemplateID.ToString() + "\"" +
                ",\"OwnerID\":\"" + template.OwnerID.ToString() + "\"" +
                ",\"BodyText\":\"" + Base64.encode(template.BodyText) + "\"" +
                ",\"AudienceType\":\"" + template.AudienceType.ToString() + "\"" +
                ",\"AudienceRefOwnerID\":\"" + (template.AudienceRefOwnerID.HasValue ?
                    template.AudienceRefOwnerID.ToString() : string.Empty) + "\"" +
                ",\"AudienceNodeID\":\"" + (template.AudienceNodeID.HasValue ?
                    template.AudienceNodeID.ToString() : string.Empty) + "\"" +
                ",\"AudienceNodeName\":\"" + Base64.encode(template.AudienceNodeName) + "\"" +
                ",\"AudienceNodeTypeID\":\"" + (template.AudienceNodeTypeID.HasValue ?
                    template.AudienceNodeTypeID.ToString() : string.Empty) + "\"" +
                ",\"AudienceNodeType\":\"" + Base64.encode(template.AudienceNodeType) + "\"" +
                ",\"AudienceNodeAdmin\":" + (template.AudienceNodeAdmin.HasValue ?
                    template.AudienceNodeAdmin : false).ToString().ToLower() +
                "}";
        }

        protected void set_message_template(Guid? templateId, Guid? ownerId, string bodyText, AudienceType audienceType,
            Guid? audienceRefOwnerId, Guid? audienceNodeId, bool? audienceNodeAdmin, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            MessageTemplate info = new MessageTemplate()
            {
                TemplateID = !templateId.HasValue ? Guid.NewGuid() : templateId,
                OwnerID = ownerId,
                BodyText = bodyText,
                AudienceType = audienceType,
                AudienceRefOwnerID = audienceRefOwnerId,
                AudienceNodeID = audienceNodeId,
                AudienceNodeAdmin = audienceNodeAdmin,
                CreatorUserID = paramsContainer.CurrentUserID.Value,
                CreationDate = DateTime.Now
            };

            bool result = NotificationController.set_message_template(paramsContainer.Tenant.Id, info);

            string _json = result ? _get_message_template_json(info) : string.Empty;

            responseText = result ? _json : "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID,
                    Date = info.CreationDate,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetMessageTemplate,
                    SubjectID = info.TemplateID,
                    Info = _json,
                    ModuleIdentifier = ModuleIdentifier.NTFN
                });
            }
            //end of Save Log
        }

        protected void remove_message_template(Guid? templateId, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            bool result = templateId.HasValue && NotificationController.remove_message_template(paramsContainer.Tenant.Id,
                templateId.Value, paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RemoveMessageTemplate,
                    SubjectID = templateId,
                    ModuleIdentifier = ModuleIdentifier.NTFN
                });
            }
            //end of Save Log
        }

        protected void get_owner_message_templates(Guid? ownerId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            responseText = !ownerId.HasValue ? "[]" : "[" + ProviderUtil.list_to_string<string>(
                NotificationController.get_owner_message_templates(paramsContainer.Tenant.Id, ownerId.Value).Select(
                    u => _get_message_template_json(u)).ToList()) + "]";
        }

        //Notification Messages

        private void set_admin_messaging_activation(Guid? templateId, SubjectType subjectType,
            NC.ActionType action, Media media, UserStatus userStatus, string language, bool? enable, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.SMSEMailNotifier, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            templateId = !templateId.HasValue ? Guid.NewGuid() : templateId;

            bool result = NotificationController.set_admin_messaging_activation(paramsContainer.Tenant.Id,
                templateId.Value, paramsContainer.CurrentUserID.Value, subjectType, action, media,
                userStatus, language, enable.HasValue && enable.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\", \"TemplateID\":\"" + templateId + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void set_user_messaging_activation(Guid? optionId, Guid? userId, SubjectType subjectType,
            UserStatus userStatus, NC.ActionType action, Media media, string lang, bool? enable, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!userId.HasValue) userId = paramsContainer.CurrentUserID;

            if (userId != paramsContainer.CurrentUserID &&
                !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value))
                userId = paramsContainer.CurrentUserID.Value;

            optionId = !optionId.HasValue ? Guid.NewGuid() : optionId;

            bool result = NotificationController.set_user_messaging_activation(paramsContainer.Tenant.Id,
                optionId.Value, userId.Value, paramsContainer.CurrentUserID.Value, subjectType, userStatus,
                action, media, lang, enable.HasValue && enable.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\", \"OptionID\":\"" + optionId + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void set_notification_message_template_text(Guid? templateId,
            string subject, string text, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(subject) && subject.Length > 500)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }

            if (!templateId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.SMSEMailNotifier, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            bool result = NotificationController.set_notification_message_template_text(paramsContainer.Tenant.Id,
                templateId.Value, paramsContainer.CurrentUserID.Value, subject, text);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void get_user_messaging_activation(Guid? userId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!userId.HasValue) userId = paramsContainer.CurrentUserID;

            if (userId != paramsContainer.CurrentUserID &&
                !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value))
                userId = paramsContainer.CurrentUserID.Value;

            List<MessagingActivationOption> lstMsgActOptions =
                NotificationController.get_user_messaging_activation(paramsContainer.Tenant.Id, userId.Value);

            List<SubjectTypeClass> lstSubjectTypes = new List<SubjectTypeClass>();
            Array MediaArr = Enum.GetValues(typeof(Media));
            Array SubjectTypeArr = Enum.GetValues(typeof(SubjectType));

            foreach (SubjectType st in SubjectTypeArr)
            {
                if (st != SubjectType.None)
                    lstSubjectTypes.Add(new SubjectTypeClass(st));
            }

            bool isFirstSubjectType = true;
            bool isFirstAction = true;
            bool isFirstUserStatus = true;
            bool isFirstMedia = true;
            responseText = "{\"Items\":[";

            foreach (SubjectTypeClass _subjectType in lstSubjectTypes)
            {
                responseText += (!isFirstSubjectType ? "," : "") +
                    "{\"SubjectTypeName\":\"" + _subjectType.SubjectTypeName + "\", \"Actions\":[";
                isFirstSubjectType = false;

                foreach (NC.ActionType _action in _subjectType.Actions)
                {
                    responseText += (!isFirstAction ? "," : "") +
                        "{\"ActionName\":\"" + _action + "\",\"Audience\":[";
                    isFirstAction = false;

                    foreach (UserStatus _userStatus in _subjectType.UserStatuses)
                    {
                        if (_userStatus != UserStatus.None)
                        {
                            responseText += (!isFirstUserStatus ? ", " : "") +
                                "{\"Name\":\"" + _userStatus + "\", \"Media\":[";
                            isFirstUserStatus = false;

                            foreach (Media _media in MediaArr)
                            {
                                if (_media != Media.None)
                                {
                                    Guid _optionId = Guid.Empty;
                                    string _lang = "";
                                    bool _enable = false;
                                    bool _adminEnable = false;

                                    if (lstMsgActOptions.Where(so => so.SubjectType == _subjectType.SubjectTypeName &&
                                            so.Action == _action && so.UserStatus == _userStatus && so.Media == _media).ToList<MessagingActivationOption>().Count == 1)
                                    {
                                        _optionId = lstMsgActOptions.Where(so => so.OptionID.HasValue && so.SubjectType == _subjectType.SubjectTypeName &&
                                            so.Action == _action && so.UserStatus == _userStatus && so.Media == _media)
                                            .Select(so => so.OptionID.Value).ToList<Guid>().FirstOrDefault();

                                        _lang = lstMsgActOptions.Where(so => so.SubjectType == _subjectType.SubjectTypeName &&
                                            so.Action == _action && so.UserStatus == _userStatus && so.Media == _media)
                                            .Select(so => so.Lang).ToList<string>().First();

                                        _enable = lstMsgActOptions.Where(so => so.Enable.HasValue &&
                                            so.SubjectType == _subjectType.SubjectTypeName && so.Action == _action &&
                                            so.UserStatus == _userStatus && so.Media == _media)
                                            .Select(so => so.Enable.Value).ToList<bool>().FirstOrDefault() == true;

                                        _adminEnable = lstMsgActOptions.Where(so => so.AdminEnable.HasValue &&
                                            so.SubjectType == _subjectType.SubjectTypeName && so.Action == _action &&
                                            so.UserStatus == _userStatus && so.Media == _media)
                                            .Select(so => so.AdminEnable.Value).ToList<bool>().FirstOrDefault() == true;
                                    }

                                    responseText += (!isFirstMedia ? "," : "") +
                                        "{\"Name\":\"" + _media + "\"," +
                                            "\"OptionID\":\"" + (_optionId == Guid.Empty ? "" : _optionId.ToString()) + "\"," +
                                            "\"Enabled\":" + _enable.ToString().ToLower() + "," +
                                            "\"EnabledByAdmin\": " + _adminEnable.ToString().ToLower() +
                                        "}";
                                    isFirstMedia = false;
                                }

                            }// end of Media foreach

                            responseText += "]}";
                            isFirstMedia = true;
                        }
                    }// end of UserStatus foreach

                    responseText += "]}";
                    isFirstUserStatus = true;
                }//end Of Actions foreach

                responseText += "]}";
                isFirstAction = true;
            }//end of SubjectTypeClass foreach

            responseText += "]}";
        }

        protected void get_notification_message_templates_info(ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.SMSEMailNotifier, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            List<NotificationMessageTemplate> lstMessageTemplates =
                NotificationController.get_notification_message_templates_info(paramsContainer.Tenant.Id);
            List<SubjectTypeClass> lstSubjectTypes = new List<SubjectTypeClass>();
            Array MediaArr = Enum.GetValues(typeof(Media));
            Array SubjectTypeArr = Enum.GetValues(typeof(SubjectType));

            foreach (SubjectType st in SubjectTypeArr)
            {
                if (st != SubjectType.None)
                    lstSubjectTypes.Add(new SubjectTypeClass(st));
            }

            bool isFirstSubjectType = true;
            bool isFirstAction = true;
            bool isFirstUserStatus = true;
            bool isFirstMedia = true;
            responseText = "{\"MessageTemplatesInfo\":[";

            foreach (SubjectTypeClass _subjectType in lstSubjectTypes)
            {
                responseText += (!isFirstSubjectType ? "," : "") +
                    "{\"SubjectType\":\"" + _subjectType.SubjectTypeName + "\", \"Actions\":[";
                isFirstSubjectType = false;

                foreach (NC.ActionType _action in _subjectType.Actions)
                {
                    responseText += (!isFirstAction ? "," : "") +
                        "{\"ActionName\":\"" + _action + "\", \"Audience\":[";
                    isFirstAction = false;

                    foreach (UserStatus _userStatus in _subjectType.UserStatuses)
                    {
                        if (_userStatus != UserStatus.None)
                        {
                            responseText += (!isFirstUserStatus ? ", " : "") +
                                "{\"Name\":\"" + _userStatus + "\", \"Media\":[";
                            isFirstUserStatus = false;

                            foreach (Media _media in MediaArr)
                            {
                                if (_media != Media.None)
                                {
                                    Guid _templateId = Guid.Empty;
                                    string _subject = "";
                                    string _text = "";
                                    string _lang = "";
                                    bool _enable = false;

                                    if (lstMessageTemplates.Where(so => so.SubjectType == _subjectType.SubjectTypeName &&
                                            so.Action == _action && so.UserStatus == _userStatus && so.Media == _media).ToList<NotificationMessageTemplate>().Count == 1)
                                    {
                                        _templateId = lstMessageTemplates.Where(so => so.SubjectType == _subjectType.SubjectTypeName &&
                                            so.Action == _action && so.UserStatus == _userStatus && so.Media == _media).Select(so => so.TemplateId.Value).ToList<Guid>().First();

                                        _subject = lstMessageTemplates.Where(so => so.SubjectType == _subjectType.SubjectTypeName &&
                                            so.Action == _action && so.UserStatus == _userStatus && so.Media == _media).Select(so => so.Subject).ToList<string>().First();

                                        _text = lstMessageTemplates.Where(so => so.SubjectType == _subjectType.SubjectTypeName &&
                                            so.Action == _action && so.UserStatus == _userStatus && so.Media == _media).Select(so => so.Text).ToList<string>().First();

                                        _lang = lstMessageTemplates.Where(so => so.SubjectType == _subjectType.SubjectTypeName &&
                                            so.Action == _action && so.UserStatus == _userStatus && so.Media == _media).Select(so => so.Lang).ToList<string>().First();

                                        _enable = lstMessageTemplates.Where(so => so.SubjectType == _subjectType.SubjectTypeName &&
                                            so.Action == _action && so.UserStatus == _userStatus && so.Media == _media).Select(so => so.Enable.Value).ToList<bool>().First();
                                    }

                                    responseText += (!isFirstMedia ? "," : "") +
                                        "{\"Name\":\"" + _media + "\"" +
                                            ",\"TemplateID\": \"" + (_templateId == Guid.Empty ? "" : _templateId.ToString()) + "\"" +
                                            ",\"MessageSubject\": \"" + Base64.encode(_subject) + "\"" +
                                            ",\"MessageText\": \"" + Base64.encode(_text) + "\"" +
                                            ",\"Language\": \"" + _lang + "\"" +
                                            ",\"Enabled\": " + _enable.ToString().ToLower() +
                                        "}";
                                    isFirstMedia = false;
                                }

                            }// end of Media foreach

                            responseText += "]}";
                            isFirstMedia = true;
                        }
                    }// end of UserStatus foreach

                    responseText += "]}";
                    isFirstUserStatus = true;
                }//end Of Actions foreach

                responseText += "]}";
                isFirstAction = true;
            }//end of SubjectTypeClass foreach

            responseText += "]}";
        }

        //end of Notification Messages

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}