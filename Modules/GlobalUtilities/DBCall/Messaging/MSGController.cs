using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.GlobalUtilities.DBCompositeTypes;
using RaaiVan.Modules.Users;

namespace RaaiVan.Modules.Messaging
{
    public static class MSGController
    {
        private static string GetFullyQualifiedName(string name)
        {
            return "[dbo]." + "[MSG_" + name + "]";
        }

        public static List<ThreadInfo> get_threads(Guid applicationId, Guid userId, int? count = null, int? lastId = null)
        {
            return MSGParsers.threads(DBConnector.read(applicationId, GetFullyQualifiedName("GetThreads"),
                applicationId, userId, count, lastId));
        }

        public static void get_thread_info(Guid applicationId, 
            Guid userId, Guid threadId, ref int messagesCount, ref int sentCount, ref int notSeenCount)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetThreadInfo"),
                applicationId, userId, threadId);

            MSGParsers.thread_info(results, ref messagesCount, ref sentCount, ref notSeenCount);
        }

        private static List<ThreadInfo> _get_thread_users(Guid applicationId, 
            List<Guid> threadIds, Guid userId, int? count = null, int? lastId = null)
        {
            return MSGParsers.thread_users(DBConnector.read(applicationId, GetFullyQualifiedName("GetThreadUsers"),
                applicationId, userId, ProviderUtil.list_to_string<Guid>(threadIds), ',', count, lastId));
        }

        public static List<ThreadInfo> get_thread_users(Guid applicationId, 
            List<Guid> threadIds, Guid userId, int? count = null)
        {
            return _get_thread_users(applicationId, threadIds, userId, count);
        }

        public static List<User> get_thread_users(Guid applicationId, 
            Guid threadId, Guid userId, int? count = null, int? lastId = null)
        {
            ThreadInfo ti = _get_thread_users(applicationId,
                new List<Guid>() { threadId }, userId, count, lastId).FirstOrDefault();

            return ti == null ? new List<User>() : ti.ThreadUsers;
        }

        public static List<Message> get_messages(Guid applicationId, 
            Guid userId, Guid? threadId, bool? sent, long? minId, int? count = null)
        {
            return MSGParsers.messages(DBConnector.read(applicationId, GetFullyQualifiedName("GetMessages"),
                applicationId, userId, threadId, sent, count, minId));
        }

        public static bool has_message(Guid applicationId, long? id, Guid userId, Guid? threadId, Guid? messageId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("HasMessage"),
                applicationId, id, userId, threadId, messageId);
        }

        public static List<Message> get_forwarded_messages(Guid applicationId, Guid messageId)
        {
            return MSGParsers.forwarded_messages(DBConnector.read(applicationId, GetFullyQualifiedName("GetForwardedMessages"),
                applicationId, messageId));
        }

        public static long send_message(Guid applicationId, Guid messageId, Guid? forwardedFrom, Guid userId, string title, 
            string messageText, bool isGroup, List<Guid> receiverUserIds, Guid? threadId, List<DocFileInfo> attachedFiles)
        {
            if (string.IsNullOrEmpty(title)) title = null;
            
            return DBConnector.get_long(applicationId, GetFullyQualifiedName("SendNewMessage"),
                applicationId, userId, threadId, messageId, forwardedFrom, title, messageText, isGroup, DateTime.Now, 
                GuidTableType.getCompositeType(receiverUserIds), DocFileInfoTableType.getCompositeType(attachedFiles));
        }

        public static bool send_message(Guid applicationId, 
            Guid senderUserId, Guid receiverUserId, string title, string messageText)
        {
            return bulk_send_message(applicationId, senderUserId, new List<Guid>() { receiverUserId }, title, messageText);
        }

        public static bool bulk_send_message(Guid applicationId, List<Message> messages)
        {
            if (messages == null) messages = new List<Message>();

            DBCompositeType<MessageTableType> messagesParam = new DBCompositeType<MessageTableType>();
            DBCompositeType<GuidPairTableType> receiversParam = new DBCompositeType<GuidPairTableType>();

            messages.Where(m => m.ReceiverUsers != null && m.ReceiverUsers.Count > 0).ToList().ForEach(m =>
            {
                messagesParam.add(new MessageTableType(
                    messageId: m.MessageID,
                    senderUserId: m.SenderUserID,
                    title: m.Title,
                    messageText: m.MessageText));

                receiversParam.add(m.ReceiverUsers.Select(u => new GuidPairTableType(m.MessageID, u.UserID)).ToList());
            });

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("BulkSendMessage"),
                applicationId, messagesParam, receiversParam, DateTime.Now);
        }

        public static bool bulk_send_message(Guid applicationId, Message message)
        {
            return bulk_send_message(applicationId, new List<Message>() { message });
        }

        public static bool bulk_send_message(Guid applicationId, 
            Guid senderUserId, List<Guid> receiverUserIds, string title, string messageText)
        {
            List<User> _receiverUsers = new List<User>();
            foreach (Guid uId in receiverUserIds) _receiverUsers.Add(new User() { UserID = uId });

            return bulk_send_message(applicationId, new Message()
            {
                MessageID = Guid.NewGuid(),
                SenderUserID = senderUserId,
                Title = title,
                MessageText = messageText,
                ReceiverUsers = _receiverUsers
            });
        }

        private static bool _remove_messages(Guid applicationId, Guid? userId, Guid? threadId, long? id)
        {
            if (id <= 0) id = null;
            if (!id.HasValue && (!userId.HasValue || !threadId.HasValue)) return false;

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RemoveMessages"), applicationId, userId, threadId, id);
        }

        public static bool remove_thread(Guid applicationId, Guid userId, Guid threadId)
        {
            return _remove_messages(applicationId, userId, threadId, id: null);
        }

        public static bool remove_message(Guid applicationId, long id)
        {
            return _remove_messages(applicationId, userId: null, threadId: null, id: id);
        }

        public static bool set_messages_as_seen(Guid applicationId, Guid userId, Guid threadId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetMessagesAsSeen"),
                applicationId, userId, threadId, DateTime.Now);
        }

        public static int get_not_seen_messages_count(Guid applicationId, Guid userId)
        {
            return DBConnector.get_int(applicationId, GetFullyQualifiedName("GetNotSeenMessagesCount"), applicationId, userId);
        }

        private static List<Message> _get_message_receivers(Guid applicationId, 
            List<Guid> messageIds, int? count = null, int? lastId = null)
        {
            return MSGParsers.message_receivers(DBConnector.read(applicationId, GetFullyQualifiedName("GetMessageReceivers"),
                applicationId, ProviderUtil.list_to_string<Guid>(messageIds), ',', count, lastId));
        }

        public static List<Message> get_message_receivers(Guid applicationId, List<Guid> messageIds, int? count = null)
        {
            return _get_message_receivers(applicationId, messageIds, count);
        }

        public static List<Users.User> get_message_receivers(Guid applicationId, 
            Guid messageId, int? count = null, int? lastId = null)
        {
            Message msg = _get_message_receivers(applicationId, new List<Guid>() { messageId }, count, lastId).FirstOrDefault();
            return msg == null ? new List<User>() : msg.ReceiverUsers;
        }
    }
}
