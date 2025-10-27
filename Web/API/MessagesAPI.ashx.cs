using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RaaiVan.Modules.Messaging;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Documents;
using RaaiVan.Modules.Users;

namespace RaaiVan.Web.API
{
    /// <summary>
    /// Summary description for MessagesAPI
    /// </summary>
    public class MessagesAPI : IHttpHandler, System.Web.SessionState.IRequiresSessionState
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
                case "GetRecentThreads":
                    get_recent_threads(PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_int(context.Request.Params["LastID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetThreadUsers":
                    get_thread_users(PublicMethods.parse_guid(context.Request.Params["ThreadID"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_int(context.Request.Params["LastID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetMessages":
                    get_messages(PublicMethods.parse_guid(context.Request.Params["ThreadID"]),
                        PublicMethods.parse_long(context.Request.Params["MinID"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetForwardedMessages":
                    get_forwarded_messages(PublicMethods.parse_guid(context.Request.Params["MessageID"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_int(context.Request.Params["LastID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetMessageReceivers":
                    get_message_receivers(PublicMethods.parse_guid(context.Request.Params["MessageID"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_int(context.Request.Params["LastID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SendMessage":
                    send_message(PublicMethods.parse_guid(context.Request.Params["ForwardedFrom"]),
                        PublicMethods.parse_string(context.Request.Params["Title"]),
                        PublicMethods.parse_string(context.Request.Params["MessageText"]),
                        PublicMethods.parse_bool(context.Request.Params["IsGroup"]),
                        ListMaker.get_guid_items(context.Request.Params["ReceiverUserIDs"], '|'),
                        PublicMethods.parse_guid(context.Request.Params["ThreadID"]),
                        DocumentUtilities.get_files_info(paramsContainer.ApplicationID, context.Request.Params["AttachedFiles"]),
                        PublicMethods.parse_guid(context.Request.Params["GroupID"]),
                        PublicMethods.parse_string(context.Request.Params["Ref"], false),
                        ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RemoveThread":
                    remove_thread(PublicMethods.parse_guid(context.Request.Params["ThreadID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RemoveMessage":
                    remove_message(PublicMethods.parse_long(context.Request.Params["ID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetMessagesAsSeen":
                    set_messages_as_seen(PublicMethods.parse_guid(context.Request.Params["ThreadID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetNotSeenMessagesCount":
                    get_not_seen_messages_count(ref responseText);
                    _return_response(ref responseText);
                    return;
            }

            paramsContainer.return_response(PublicConsts.BadRequestResponse);
        }

        protected void _return_response(ref string responseText)
        {
            paramsContainer.return_response(ref responseText);
        }

        protected void get_recent_threads(int? count, int? lastId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            List<ThreadInfo> threads = MSGController.get_threads(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, count, lastId);

            List<ThreadInfo> threadUsers = MSGController.get_thread_users(paramsContainer.Tenant.Id,
                threads.Where(u => u.IsGroup == true).Select(
                    v => v.ThreadID.Value).ToList(), paramsContainer.CurrentUserID.Value);

            foreach (ThreadInfo tu in threadUsers)
            {
                ThreadInfo th = threads.Where(u => u.ThreadID == tu.ThreadID).FirstOrDefault();

                th.UsersCount = tu.UsersCount;
                th.ThreadUsers = tu.ThreadUsers;
            }

            responseText = "{\"LastID\":" + (threads.Count > 0 ? threads.Max(u => u.ID) : 0).ToString() + ",\"Threads\":[";
            bool isFirst = true;

            foreach (ThreadInfo th in threads)
            {
                responseText += (isFirst ? string.Empty : ",") + "{\"ThreadID\":\"" + th.ThreadID.Value.ToString() + "\"" +
                    ",\"IsGroup\":" + (th.IsGroup.HasValue && th.IsGroup.Value).ToString().ToLower() +
                    ",\"UsersCount\":" + (th.UsersCount.HasValue ? th.UsersCount.Value : 0).ToString() +
                    ",\"MessagesCount\":" + (th.MessagesCount.HasValue ? th.MessagesCount.Value : 0).ToString() +
                    ",\"SentCount\":" + (th.SentCount.HasValue ? th.SentCount.Value : 0).ToString() +
                    ",\"NotSeenCount\":" + (th.NotSeenCount.HasValue ? th.NotSeenCount.Value : 0).ToString() +
                    ",\"Users\":[" + ProviderUtil.list_to_string<string>(th.ThreadUsers.Select(
                        u => "{\"UserID\":\"" + u.UserID.ToString() + "\"" +
                            ",\"UserName\":\"" + Base64.encode(u.UserName) + "\"" +
                            ",\"FirstName\":\"" + Base64.encode(u.FirstName) + "\"" +
                            ",\"LastName\":\"" + Base64.encode(u.LastName) + "\"" +
                            ",\"ProfileImageURL\":\"" +
                            DocumentUtilities.get_personal_image_address(paramsContainer.Tenant.Id,
                                u.UserID.Value) + "\"}").ToList()) + "]}";
                isFirst = false;
            }

            responseText += "]}";
        }

        protected void get_thread_users(Guid? threadId, int? count, int? lastId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!lastId.HasValue) lastId = -1;

            if (!threadId.HasValue || !MSGController.has_message(paramsContainer.Tenant.Id,
                null, paramsContainer.CurrentUserID.Value, threadId, null))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            List<User> users = MSGController.get_thread_users(paramsContainer.Tenant.Id,
                threadId.Value, paramsContainer.CurrentUserID.Value, count, lastId);

            responseText = "{\"Users\":[" +
                ProviderUtil.list_to_string<string>(users.Select(
                    u => "{\"UserID\":\"" + u.UserID.ToString() + "\"" +
                        ",\"UserName\":\"" + Base64.encode(u.UserName) + "\"" +
                        ",\"FirstName\":\"" + Base64.encode(u.FirstName) + "\"" +
                        ",\"LastName\":\"" + Base64.encode(u.LastName) + "\"" +
                        ",\"ProfileImageURL\":\"" + DocumentUtilities.get_personal_image_address(paramsContainer.Tenant.Id,
                            u.UserID.Value) + "\"}").ToList()) +
                    "]}";
        }

        protected void get_messages(Guid? threadId, long? minId, int? count, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!count.HasValue) count = 0;

            List<Message> messages = !threadId.HasValue ? new List<Message>() :
                MSGController.get_messages(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, threadId, null, minId, count);

            responseText = "{\"MinID\":" + (messages.Count > 0 ? messages.Min(u => u.ID) : 0).ToString() +
                ",\"Messages\":[";

            List<DocFileInfo> attachments = DocumentsController.get_owner_files(paramsContainer.Tenant.Id,
                messages.Where(u => u.HasAttachment.HasValue && u.HasAttachment.Value).Select(v => v.MessageID.Value).ToList());

            bool isFirst = true;

            foreach (Message msg in messages)
            {
                responseText += (isFirst ? string.Empty : ",") +
                    "{\"ID\":\"" + msg.ID.ToString() + "\"" +
                    ",\"ThreadID\":\"" + msg.ThreadID.Value.ToString() + "\"" +
                    ",\"MessageID\":\"" + msg.MessageID.Value.ToString() + "\"" +
                    ",\"IsGroup\":" + (msg.IsGroup.HasValue && msg.IsGroup.Value).ToString().ToLower() +
                    ",\"IsSender\":" + (msg.IsSender.HasValue && msg.IsSender.Value).ToString().ToLower() +
                    ",\"Seen\":" + (msg.Seen.HasValue && msg.Seen.Value).ToString().ToLower() +
                    ",\"Title\":\"" + Base64.encode(msg.Title) + "\"" +
                    ",\"MessageText\":\"" + Base64.encode(msg.MessageText) + "\"" +
                    ",\"SendDate\":\"" + GenericDate.get_local_date(msg.SendDate.Value, true) + "\"" +
                    ",\"ForwardedFrom\":\"" + (msg.ForwardedFrom.HasValue ? msg.ForwardedFrom.Value.ToString() : "") + "\"" +
                    ",\"SenderUserID\":\"" + msg.SenderUserID.ToString() + "\"" +
                    ",\"SenderUserName\":\"" + Base64.encode(msg.SenderUserName) + "\"" +
                    ",\"SenderFirstName\":\"" + Base64.encode(msg.SenderFirstName) + "\"" +
                    ",\"SenderLastName\":\"" + Base64.encode(msg.SenderLastName) + "\"" +
                    ",\"ProfileImageURL\":\"" + DocumentUtilities.get_personal_image_address(paramsContainer.Tenant.Id,
                        msg.SenderUserID.Value) + "\"" +
                    ",\"AttachedFiles\":" + DocumentUtilities.get_files_json(attachments.Where(u => u.OwnerID == msg.MessageID).ToList(), true) +
                    "}";
                isFirst = false;
            }

            responseText += "]}";
        }

        protected void get_forwarded_messages(Guid? messageId, int? count, int? lastId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!messageId.HasValue || !MSGController.has_message(paramsContainer.Tenant.Id,
                null, paramsContainer.CurrentUserID.Value, null, messageId))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            List<Message> fwdMessages = MSGController.get_forwarded_messages(paramsContainer.Tenant.Id, messageId.Value);
            responseText = "{\"ForwardedMessages\": [";
            bool isFirst = true;

            List<DocFileInfo> attachments = DocumentsController.get_owner_files(paramsContainer.Tenant.Id,
                fwdMessages.Where(u => u.HasAttachment.HasValue && u.HasAttachment.Value).Select(v => v.MessageID.Value).ToList());

            List<Message> messageUsers = MSGController.get_message_receivers(paramsContainer.Tenant.Id,
                fwdMessages.Select(m => m.MessageID.Value).ToList(), count);

            foreach (Message msg in messageUsers)
            {
                Message ms = fwdMessages.Where(m => m.MessageID == msg.MessageID).FirstOrDefault();

                ms.ReceiverUsers = msg.ReceiverUsers;
                ms.ReceiversCount = msg.ReceiversCount;
            }

            foreach (Message msg in fwdMessages)
            {
                responseText += (isFirst ? string.Empty : ",") +
                     "{\"ID\":\"" + msg.ID.ToString() + "\"" +
                    ",\"MessageID\":\"" + msg.MessageID.Value.ToString() + "\"" +
                    ",\"IsGroup\":" + (msg.IsGroup.HasValue && msg.IsGroup.Value).ToString().ToLower() +
                    ",\"SenderUserID\":\"" + msg.SenderUserID.ToString() + "\"" +
                    ",\"SenderUserName\":\"" + Base64.encode(msg.SenderUserName) + "\"" +
                    ",\"SenderFirstName\":\"" + Base64.encode(msg.SenderFirstName) + "\"" +
                    ",\"SenderLastName\":\"" + Base64.encode(msg.SenderLastName) + "\"" +
                    ",\"SenderProfileImageURL\":\"" + DocumentUtilities.get_personal_image_address(
                        paramsContainer.Tenant.Id, msg.SenderUserID.Value) + "\"" +
                    ",\"Title\":\"" + Base64.encode(msg.Title) + "\"" +
                    ",\"MessageText\":\"" + Base64.encode(msg.MessageText) + "\"" +
                    ",\"SendDate\":\"" + GenericDate.get_local_date(msg.SendDate.Value, true) + "\"" +
                    ",\"ForwardedFrom\":\"" + (msg.ForwardedFrom.HasValue ? msg.ForwardedFrom.Value.ToString() : "") + "\"" +
                    ",\"AttachedFiles\":" + DocumentUtilities.get_files_json(attachments.Where(u => u.OwnerID == msg.MessageID).ToList(), true) +
                    ",\"Level\":" + msg.Level.ToString().ToLower() +
                    ",\"ReceiversCount\":" + (msg.ReceiversCount.HasValue ? msg.ReceiversCount.Value : 0).ToString() +
                    ",\"ReceiverUsers\":[" +
                        ProviderUtil.list_to_string<string>(msg.ReceiverUsers.Select(
                            u => "{\"UserID\":\"" + u.UserID.ToString() + "\"" +
                                ",\"UserName\":\"" + Base64.encode(u.UserName) + "\"" +
                                ",\"FirstName\":\"" + Base64.encode(u.FirstName) + "\"" +
                                ",\"LastName\":\"" + Base64.encode(u.LastName) + "\"" +
                                ",\"ProfileImageURL\":\"" + DocumentUtilities.get_personal_image_address(
                                    paramsContainer.Tenant.Id, u.UserID.Value) + "\"}").ToList()) +
                    "]}";
                isFirst = false;
            }

            responseText += "]}";
        }

        protected void send_message(Guid? forwardedFrom, string title, string messageText, bool? isGroup,
            List<Guid> receiverUserIds, Guid? threadId, List<DocFileInfo> attachedFiles,
            Guid? groupId, string _ref, ref string responseText)
        {
            //GroupID will not be stored in the database and is just used for chat

            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(title) && title.Length > 490)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(title))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            if (!isGroup.HasValue) isGroup = false;

            Guid messageId = Guid.NewGuid();
            isGroup = receiverUserIds.Count == 1 ? false : isGroup;

            bool selfChat = !isGroup.Value && (
                (receiverUserIds.Count == 1 && receiverUserIds.First() == paramsContainer.CurrentUserID.Value) ||
                (threadId == paramsContainer.CurrentUserID)
            );

            if (!threadId.HasValue && ((isGroup.Value && receiverUserIds.Count > 1) || (!isGroup.Value && receiverUserIds.Count == 1)))
                threadId = isGroup.Value ? Guid.NewGuid() : (receiverUserIds.Count == 1 ? receiverUserIds.First() : threadId);

            if (attachedFiles != null)
                attachedFiles.ForEach(f => f.move(FolderNames.TemporaryFiles, FolderNames.Attachments));

            long result = MSGController.send_message(paramsContainer.Tenant.Id, messageId, forwardedFrom,
                paramsContainer.CurrentUserID.Value, title, messageText, isGroup.Value, receiverUserIds, threadId, attachedFiles);

            if (result <= 0 && attachedFiles != null)
                attachedFiles.ForEach(f => f.move(FolderNames.Attachments, FolderNames.TemporaryFiles));

            List<User> receiverUsers;
            User senderUser = UsersController.get_user(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value);

            responseText = result <= 0 ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"";

            bool sendForMany = !isGroup.Value && receiverUserIds.Count > 1;

            if (result > 0 && !sendForMany)
            {
                int msgCount = 0, sentCount = 0, notSeenCount = 0;

                MSGController.get_thread_info(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value,
                    threadId.Value, ref msgCount, ref sentCount, ref notSeenCount);

                receiverUsers = UsersController.get_users(paramsContainer.Tenant.Id, receiverUserIds);

                responseText +=
                    ",\"Thread\":{" +
                        "\"ThreadID\":\"" + threadId.ToString() + "\"" +
                        ",\"IsGroup\":" + isGroup.ToString().ToLower() +
                        ",\"UsersCount\":" + receiverUserIds.Count.ToString().ToLower() +
                        ",\"MessagesCount\":" + msgCount.ToString() +
                        ",\"SentCount\":" + sentCount.ToString() +
                        ",\"NotSeenCount\":" + notSeenCount.ToString() +
                        ",\"Users\":[" + ProviderUtil.list_to_string<string>(receiverUsers.Select(
                            u => "{\"UserID\":\"" + u.UserID.ToString() + "\"" +
                                ",\"UserName\":\"" + Base64.encode(u.UserName) + "\"" +
                                ",\"FirstName\":\"" + Base64.encode(u.FirstName) + "\"" +
                                ",\"LastName\":\"" + Base64.encode(u.LastName) + "\"" +
                                ",\"ProfileImageURL\":\"" + DocumentUtilities.get_personal_image_address(
                                    paramsContainer.Tenant.Id, u.UserID.Value) + "\"" +
                            "}").ToList()) +
                        "]" +
                    "}";

                if (threadId == senderUser.UserID || (receiverUserIds != null && receiverUserIds.Contains(senderUser.UserID.Value)))
                    responseText += ",\"SenderIsReceiver\":" + true.ToString().ToLower();

                for (int i = 0; i < attachedFiles.Count; ++i)
                    attachedFiles[i].OwnerID = messageId;

                responseText +=
                    ",\"Message\":{" +
                        "\"ID\":" + result.ToString() +
                        ",\"ReceiverID\":" + (result + 1).ToString() +
                        ",\"MessageID\":\"" + messageId.ToString() + "\"" +
                        ",\"ThreadID\":\"" + threadId.Value.ToString() + "\"" +
                        ",\"ForwardedFrom\":\"" + (!forwardedFrom.HasValue ? "" : forwardedFrom.ToString()) + "\"" +
                        ",\"IsGroup\":" + isGroup.ToString().ToLower() +
                        ",\"GroupID\":\"" + (groupId.HasValue && groupId.HasValue ? groupId.Value : Guid.NewGuid()).ToString() + "\"" +
                        ",\"SelfChat\":" + selfChat.ToString().ToLower() +
                        ",\"IsSender\":" + true.ToString().ToLower() +
                        ",\"Seen\":" + true.ToString().ToLower() +
                        ",\"Title\":\"" + string.Empty + "\"" +
                        ",\"MessageText\":\"" + Base64.encode(messageText) + "\"" +
                        ",\"SendDate\":\"" + GenericDate.get_local_date(DateTime.Now, true) + "\"" +
                        ",\"SenderUserID\":\"" + senderUser.UserID.ToString() + "\"" +
                        ",\"SenderUserName\":\"" + Base64.encode(senderUser.UserName) + "\"" +
                        ",\"SenderFirstName\":\"" + Base64.encode(senderUser.FirstName) + "\"" +
                        ",\"SenderLastName\":\"" + Base64.encode(senderUser.LastName) + "\"" +
                        ",\"ProfileImageURL\":\"" + DocumentUtilities.get_personal_image_address(
                            paramsContainer.Tenant.Id, senderUser.UserID.Value) + "\"" +
                        ",\"AttachedFiles\":" + DocumentUtilities.get_files_json(attachedFiles, true) +
                        ",\"Ref\":\"" + (string.IsNullOrEmpty(_ref) ? string.Empty : _ref) + "\"" +
                    "}";
            }

            responseText += "}";

            //Send RealTime Data
            if (result > 0 && RaaiVanSettings.RealTime(paramsContainer.Tenant.Id) && !sendForMany)
            {
                List<Guid> userIds = new List<Guid>();

                if (!isGroup.Value)
                    userIds = new List<Guid>() { threadId.Value, senderUser.UserID.Value };
                else
                {
                    userIds = receiverUserIds.Count > 0 ? receiverUserIds :
                    MSGController.get_thread_users(paramsContainer.Tenant.Id, threadId.Value,
                    paramsContainer.CurrentUserID.Value, 1000, null).Select(u => u.UserID.Value).ToList();

                    if (receiverUserIds.Count > 0) userIds.Add(senderUser.UserID.Value);
                }

                if (userIds.Count > 0) RaaiVanHub.SendData(paramsContainer.Tenant.Id, userIds,
                    RaaiVanHub.RealTimeAction.NewMessage, responseText);
            }
            //end of Send RealTime Data
        }

        protected void remove_thread(Guid? threadId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            bool result = threadId.HasValue && MSGController.remove_thread(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, threadId.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected string _get_thread_info_json(int messagesCount, int sentCount, int notSeenCount)
        {
            return "{\"MessagesCount\":" + messagesCount.ToString() +
                ",\"SentCount\":" + sentCount.ToString() +
                ",\"NotSeenCount\":" + notSeenCount.ToString() + "}";
        }

        protected void remove_message(long? id, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!id.HasValue ||
                !MSGController.has_message(paramsContainer.Tenant.Id, id, paramsContainer.CurrentUserID.Value, null, null))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            bool result = id.HasValue && MSGController.remove_message(paramsContainer.Tenant.Id, id.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void set_messages_as_seen(Guid? threadId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!threadId.HasValue || !MSGController.has_message(paramsContainer.Tenant.Id,
                null, paramsContainer.CurrentUserID.Value, threadId, null))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            int msgCount = 0, sentCount = 0, notSeenCount = 0;

            bool result = MSGController.set_messages_as_seen(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, threadId.Value);

            if (result) MSGController.get_thread_info(paramsContainer.Tenant.Id,
                threadId.Value, paramsContainer.CurrentUserID.Value, ref msgCount, ref sentCount, ref notSeenCount);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"" +
                ",\"ThreadInfo\":" + _get_thread_info_json(msgCount, sentCount, notSeenCount) + "}";
        }

        protected void get_not_seen_messages_count(ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;
            responseText = MSGController.get_not_seen_messages_count(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value).ToString();
        }

        protected void get_message_receivers(Guid? messageId, int? count, int? lastId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!count.HasValue) count = 0;
            if (!lastId.HasValue) lastId = -1;

            if (!messageId.HasValue || !MSGController.has_message(paramsContainer.Tenant.Id,
                null, paramsContainer.CurrentUserID.Value, null, messageId))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            List<User> users = MSGController.get_message_receivers(paramsContainer.Tenant.Id, messageId.Value, count, lastId);

            responseText = "{\"Users\":[" +
                ProviderUtil.list_to_string<string>(users.Select(
                    u => "{\"UserID\":\"" + u.UserID.ToString() + "\"" +
                        ",\"UserName\":\"" + Base64.encode(u.UserName) + "\"" +
                        ",\"FirstName\":\"" + Base64.encode(u.FirstName) + "\"" +
                        ",\"LastName\":\"" + Base64.encode(u.LastName) + "\"" +
                        ",\"ProfileImageURL\":\"" + DocumentUtilities.get_personal_image_address(
                            paramsContainer.Tenant.Id, u.UserID.Value) + "\"}").ToList()) +
                    "]}";
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