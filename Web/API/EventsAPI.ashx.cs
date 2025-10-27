using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;
using RaaiVan.Modules.Events;
using RaaiVan.Modules.CoreNetwork;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Log;
using RaaiVan.Modules.Messaging;

namespace RaaiVan.Web.API
{
    /// <summary>
    /// Summary description for EventsAPI
    /// </summary>
    public class EventsAPI : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        ParamsContainer paramsContainer = null;

        public void ProcessRequest(HttpContext context)
        {
            paramsContainer = new ParamsContainer(context, nullTenantResponse: true);
            if (!paramsContainer.ApplicationID.HasValue) return;

            string command = string.IsNullOrEmpty(context.Request.Params["Command"]) ?
                (string.IsNullOrEmpty(context.Request.Params["cmd"]) ? string.Empty : context.Request.Params["cmd"]) : context.Request.Params["Command"];
            string responseText = string.Empty;

            Guid mixedUserId = !string.IsNullOrEmpty(context.Request.Params["UserID"]) ? Guid.Parse(context.Request.Params["UserID"]) :
                (paramsContainer.CurrentUserID.HasValue ? paramsContainer.CurrentUserID.Value : Guid.Empty);
            Guid eventId = !string.IsNullOrEmpty(context.Request.Params["EventID"]) ? Guid.Parse(context.Request.Params["EventID"]) :
                (string.IsNullOrEmpty(context.Request.Params["CalenderID"]) ? Guid.Empty : Guid.Parse(context.Request.Params["CalenderID"]));

            switch (command)
            {
                case "NewEvent":
                    register_event(PublicMethods.parse_string(context.Request.Params["Type"]),
                        PublicMethods.parse_string(context.Request.Params["Title"]),
                        PublicMethods.parse_string(context.Request.Params["Description"]),
                        PublicMethods.parse_date(context.Request.Params["BeginDate"]),
                        PublicMethods.parse_date(context.Request.Params["FinishDate"]),
                        ListMaker.get_guid_items(context.Request.Params["UserIDs"], '|'),
                        ListMaker.get_guid_items(context.Request.Params["GroupIDs"], '|'),
                        ListMaker.get_guid_items(context.Request.Params["NodeIDs"], '|'),
                        ref responseText);
                    _return_response(responseText);
                    return;
                case "GetEvent":
                    get_event(eventId, ref responseText);
                    _return_response(responseText);
                    return;
                case "GetFinishedEventsCount":
                    get_finished_events_count(ref responseText);
                    _return_response(responseText);
                    return;
                case "GetFinishedEvents":
                    get_finished_events(ref responseText);
                    _return_response(responseText);
                    return;
                case "RemoveCalenderUser":
                    remove_calender_user(eventId, mixedUserId, ref responseText);
                    _return_response(responseText);
                    return;
                case "ChangeCalenderUserStatus":
                    string status = string.IsNullOrEmpty(context.Request.Params["Status"]) ? string.Empty : context.Request.Params["Status"];
                    change_calender_user_status(eventId, mixedUserId, status, ref responseText);
                    _return_response(responseText);
                    return;
            }

            paramsContainer.return_response(PublicConsts.BadRequestResponse);
        }

        protected void _return_response(string responseText)
        {
            paramsContainer.return_response(responseText);
        }

        protected void register_event(string type, string title, string description, DateTime? beginDate, DateTime? finishDate,
            List<Guid> userIds, List<Guid> groupIds, List<Guid> nodeIds, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (string.IsNullOrEmpty(type))
            {
                responseText = "{\"ErrorText\":\"" + Messages.EventTypeIsNotDetermined + "\"}";
                return;
            }

            if (string.IsNullOrEmpty(title))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TitleIsNotDetermined + "\"}";
                return;
            }

            if (!beginDate.HasValue && !finishDate.HasValue)
            {
                responseText = "{\"ErrorText\":\"" + Messages.DateIsNotValid + "\"}";
                return;
            }

            DateTime _now = DateTime.Now;
            _now = new DateTime(_now.Year, _now.Month, _now.Day);
            if (finishDate < _now)
            {
                responseText = "{\"ErrorText\":\"" + Messages.DateIsNotValid + "\"}";
                return;
            }

            if (beginDate.HasValue && finishDate.HasValue && finishDate.Value < beginDate.Value)
            {
                DateTime tdt = beginDate.Value;
                beginDate = finishDate;
                finishDate = tdt;
            }

            userIds.AddRange(CNController.get_members(paramsContainer.Tenant.Id, groupIds, pending: false, admin: null)
                .Where(u => u.Member.UserID != paramsContainer.CurrentUserID).Select(u => u.Member.UserID.Value).ToList());

            Event _event = new Event()
            {
                EventID = Guid.NewGuid(),
                EventType = type,
                Title = title,
                Description = description,
                BeginDate = beginDate,
                FinishDate = finishDate,
                CreationDate = DateTime.Now,
                CreatorUserID = paramsContainer.CurrentUserID
            };

            bool result = EventsController.create_event(paramsContainer.Tenant.Id, _event, userIds, groupIds, nodeIds);

            if (!result)
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }

            string message = string.Empty;
            try { SendMessage(ref _event, ref userIds, type, true, ref message); }
            catch { }

            responseText = "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            LogController.save_log(paramsContainer.Tenant.Id, new Log()
            {
                UserID = paramsContainer.CurrentUserID,
                Date = _event.CreationDate,
                HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                Action = Modules.Log.Action.RegisterNewEvent,
                SubjectID = _event.EventID,
                Info = "{\"Type\":\"" + Base64.encode(type) + "\"" +
                    ",\"Title\":\"" + Base64.encode(title) + "\"" +
                    ",\"Description\":\"" + Base64.encode(description) + "\"" +
                    (beginDate.HasValue ? ",\"BeginDate\":\"" + Base64.encode(GenericDate.get_local_date(beginDate, true)) + "\"" : string.Empty) +
                    (finishDate.HasValue ? ",\"FinishDate\":\"" + Base64.encode(GenericDate.get_local_date(finishDate, true)) + "\"" : string.Empty) +
                    "}",
                ModuleIdentifier = ModuleIdentifier.EVT
            });
            //end of Save Log
        }

        private void get_event(Guid eventId, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            Event evt = EventsController.get_event(paramsContainer.Tenant.Id, eventId, true);

            if (evt == null)
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }

            responseText = "{\"EventID\":\"" + eventId.ToString() + "\"" +
                ",\"Title\":\"" + Base64.encode(evt.Title) + "\"" +
                ",\"Type\":\"" + Base64.encode(evt.EventType) + "\"" +
                ",\"Description\":\"" + Base64.encode(evt.Description) + "\"" +
                ",\"BeginDate\":\"" + GenericDate.get_local_date(evt.BeginDate.Value) + "\"" +
                ",\"FinishDate\":\"" + GenericDate.get_local_date(evt.FinishDate.Value) + "\"" +
                "}";
        }

        public void SendMessage(ref Event Calendar, ref List<Guid> relatedUserIds, string type, bool isRegisterMode, ref string message)
        {
            try
            {
                if (relatedUserIds != null && relatedUserIds.Count > 1)
                {
                    string strRenderedKnowledgeDomains = "";

                    List<Modules.CoreNetwork.Node> CalenNodesKnowledgeDomains =
                        EventsController.get_related_nodes(paramsContainer.Tenant.Id, Calendar.EventID.Value);

                    if (CalenNodesKnowledgeDomains != null && CalenNodesKnowledgeDomains.Count() != 0)
                    {
                        int Counter = 0;
                        foreach (Modules.CoreNetwork.Node CalenNode in CalenNodesKnowledgeDomains)
                        {
                            Counter++;
                            if (Counter != 1)
                                strRenderedKnowledgeDomains += " ، ";

                            System.Web.UI.HtmlControls.HtmlAnchor a = new System.Web.UI.HtmlControls.HtmlAnchor();
                            a.InnerText = CalenNode.Name;
                            a.Target = "_blank";
                            a.HRef = PublicConsts.get_client_url(PublicConsts.NodePage) + "/" + CalenNode.NodeID.ToString();

                            StringBuilder SB = new StringBuilder();
                            HtmlTextWriter TW = new HtmlTextWriter(new StringWriter(SB));
                            a.RenderControl(TW);

                            strRenderedKnowledgeDomains += SB.ToString();
                        }
                    }

                    relatedUserIds.Remove(paramsContainer.CurrentUserID.Value);

                    string strTitle = "";
                    string PostFix = "";
                    if (isRegisterMode)
                    {
                        strTitle = "ثبت " + type;
                        PostFix = " ثبت شد";
                    }
                    else
                    {
                        strTitle = "ویرایش " + type;
                        PostFix = " ویرایش شد ، به تغییرات اعمال شده دقت فرمایید";
                    }

                    string strMessage = "رویدادی از نوع " + "\" " + type + " \" با عنوان " + "\" " + Calendar.Title + " \"" +
                        " برای تاریخ " + "\" " + GenericDate.get_local_date(Calendar.BeginDate.Value) + " \"" +
                        " شامل موضوعات : " + strRenderedKnowledgeDomains + PostFix;
                    if (string.IsNullOrEmpty(strRenderedKnowledgeDomains))
                        strMessage = "رویدادی از نوع " + "\" " + type + " \" با عنوان " + "\" " + Calendar.Title + " \"" +
                            " برای تاریخ " + "\" " + GenericDate.get_local_date(Calendar.BeginDate.Value) + " \"" + PostFix;
                    else
                        strMessage = "رویدادی از نوع " + "\" " + type + " \" با عنوان " + "\" " + Calendar.Title + " \"" +
                            " برای تاریخ " + "\" " + GenericDate.get_local_date(Calendar.BeginDate.Value) + " \"" +
                            " شامل موضوعات : " + strRenderedKnowledgeDomains + PostFix;


                    bool result = MSGController.bulk_send_message(paramsContainer.Tenant.Id,
                        paramsContainer.CurrentUserID.Value, relatedUserIds, strTitle, strMessage);
                    if (!result) message = "خطا در ارسال پیام به کاربران انتخاب شده";
                }
            }
            catch (Exception ex) { message = "خطا در ارسال پیام به کاربران انتخاب شده"; }
        }

        private void get_finished_events_count(ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            int finishedEvents = EventsController.get_user_finished_events_count(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, false);

            responseText = "{\"Count\":" + finishedEvents.ToString() + "}";
        }

        private void get_finished_events(ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            List<Event> finishedEvents = EventsController.get_user_finished_events(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, false);

            responseText = "{\"TotalCount\":" + finishedEvents.Count.ToString() + ",\"Events\":[";

            bool isFirst = true;
            foreach (Event _event in finishedEvents)
            {
                responseText += (isFirst ? string.Empty : ",") +
                    "{\"EventID\":\"" + _event.EventID.ToString() + "\"" +
                    ",\"Title\":\"" + Base64.encode(_event.Title) + "\"" +
                    "}";
                isFirst = false;
            }

            responseText += "]}";
        }

        private void remove_calender_user(Guid calenderId, Guid userId, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            bool calenderDeleted = false;
            bool result = EventsController.remove_related_user(paramsContainer.Tenant.Id,
                calenderId, userId, ref calenderDeleted);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            if (!result) return;

            List<Guid> relatedUserIds = EventsController.get_related_user_ids(paramsContainer.Tenant.Id, calenderId);
            Event _event = EventsController.get_event(paramsContainer.Tenant.Id, calenderId, true);

            if (userId == _event.CreatorUserID)
            {
                relatedUserIds.Remove(userId);

                DateTime NowDT = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                if (_event.BeginDate.Value > NowDT)
                {
                    string strType = _event.EventType;
                    string strMessage = strType + " " + "\" " + _event.Title + " \"" + " در تاریخ " + "\" " +
                        GenericDate.get_local_date(_event.BeginDate.Value) + " \"" + " لغو گردید";
                    string strTitle = "لغو " + strType;
                    MSGController.bulk_send_message(paramsContainer.Tenant.Id,
                        userId, relatedUserIds, strTitle, strMessage);
                }
            }
            else
            {
                if (!relatedUserIds.Any(u => u == _event.CreatorUserID.Value)) relatedUserIds.Add(_event.CreatorUserID.Value);

                string strMessage = "حذف رویداد از نوع " + _event.EventType + " و با عنوان: " + _event.Title;
                MSGController.bulk_send_message(paramsContainer.Tenant.Id,
                    userId, relatedUserIds, string.Empty, strMessage);
            }

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RemoveEventUser,
                    SubjectID = calenderId,
                    SecondSubjectID = userId,
                    ModuleIdentifier = ModuleIdentifier.EVT
                });
            }
            //end of Save Log
        }

        private int change_calender_user_status(Guid calenderId, Guid userId,
            string status, ref string responseText)
        {
            if (calenderId == Guid.Empty || userId == Guid.Empty) return 0;

            bool result = EventsController.change_user_status(paramsContainer.Tenant.Id, calenderId, userId, status);

            responseText = result ? "yes:عملیات با موفقیت انجام شد" : "no:خطا در اجرای عملیات";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.ChangeEventUserStatus,
                    SubjectID = calenderId,
                    SecondSubjectID = userId,
                    Info = "{\"Status\":\"" + status + "\"}",
                    ModuleIdentifier = ModuleIdentifier.EVT
                });
            }
            //end of Save Log

            return result ? 1 : -1;
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