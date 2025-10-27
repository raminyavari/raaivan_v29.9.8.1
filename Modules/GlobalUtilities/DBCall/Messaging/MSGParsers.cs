using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Users;

namespace RaaiVan.Modules.Messaging
{
    public static class MSGParsers
    {
        public static List<ThreadInfo> threads(DBResultSet results)
        {
            List<ThreadInfo> retList = new List<ThreadInfo>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                ThreadInfo th = new ThreadInfo() {
                    ThreadID = table.GetGuid(i, "ThreadID"),
                    IsGroup = table.GetBool(i, "IsGroup"),
                    SentCount = table.GetInt(i, "SentCount"),
                    NotSeenCount = table.GetInt(i, "NotSeenCount"),
                    ID = table.GetLong(i, "RowNumber"),
                    MessagesCount = table.GetInt(i, "MessagesCount")
                };

                if (!th.IsGroup.HasValue || !th.IsGroup.Value)
                {
                    th.ThreadUsers.Add(new User()
                    {
                        UserID = table.GetGuid(i, "ThreadID"),
                        UserName = table.GetString(i, "UserName"),
                        FirstName = table.GetString(i, "FirstName"),
                        LastName = table.GetString(i, "LastName")
                    });
                }

                retList.Add(th);
            }

            return retList;
        }

        public static void thread_info(DBResultSet results, ref int messagesCount, ref int sentCount, ref int notSeenCount)
        {
            RVDataTable table = results.get_table();

            messagesCount = table.GetInt(0, "MessagesCount", defaultValue: 0).Value;
            sentCount = table.GetInt(0, "SentCount", defaultValue: 0).Value;
            notSeenCount = table.GetInt(0, "NotSeenCount", defaultValue: 0).Value;
        }

        public static List<Message> messages(DBResultSet results)
        {
            List<Message> retList = new List<Message>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new Message()
                {
                    ID = table.GetLong(i, "ID"),
                    ThreadID = table.GetGuid(i, "ThreadID"),
                    MessageID = table.GetGuid(i, "MessageID"),
                    ForwardedFrom = table.GetGuid(i, "ForwardedFrom"),
                    Title = table.GetString(i, "Title"),
                    MessageText = table.GetString(i, "MessageText"),
                    SendDate = table.GetDate(i, "SendDate"),
                    IsGroup = table.GetBool(i, "IsGroup"),
                    IsSender = table.GetBool(i, "IsSender"),
                    Seen = table.GetBool(i, "Seen"),
                    SenderUserID = table.GetGuid(i, "SenderUserID"),
                    SenderUserName = table.GetString(i, "UserName"),
                    SenderFirstName = table.GetString(i, "FirstName"),
                    SenderLastName = table.GetString(i, "LastName"),
                    HasAttachment = table.GetBool(i, "HasAttachment")
                });
            }

            return retList;
        }

        public static List<Message> forwarded_messages(DBResultSet results)
        {
            List<Message> retList = new List<Message>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new Message()
                {
                    MessageID = table.GetGuid(i, "MessageID"),
                    SenderUserID = table.GetGuid(i, "SenderUserID"),
                    SenderUserName = table.GetString(i, "SenderUserName"),
                    SenderFirstName = table.GetString(i, "SenderFirstName"),
                    SenderLastName = table.GetString(i, "SenderLastName"),
                    Title = table.GetString(i, "Title"),
                    MessageText = table.GetString(i, "MessageText"),
                    SendDate = table.GetDate(i, "SendDate"),
                    IsGroup = table.GetBool(i, "IsGroup"),
                    ForwardedFrom = table.GetGuid(i, "ForwardedFrom"),
                    Level = table.GetInt(i, "Level"),
                    HasAttachment = table.GetBool(i, "HasAttachment")
                });
            }

            return retList;
        }

        public static List<ThreadInfo> thread_users(DBResultSet results)
        {
            List<ThreadInfo> retList = new List<ThreadInfo>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                Guid? threadId = table.GetGuid(i, "ThreadID");

                if (!threadId.HasValue) continue;

                ThreadInfo th = retList.Where(u => u.ThreadID == threadId).FirstOrDefault();

                if (th == null)
                {
                    th = new ThreadInfo() { ThreadID = threadId };
                    retList.Add(th);
                }

                User usr = new User() {
                    UserID = table.GetGuid(i, "UserID"),
                    UserName = table.GetString(i, "UserName"),
                    FirstName = table.GetString(i, "FirstName"),
                    LastName = table.GetString(i, "LastName")
                };

                int usersCount = table.GetInt(i, "RevRowNumber", defaultValue: 0).Value;
                if (!th.UsersCount.HasValue || usersCount > th.UsersCount.Value) th.UsersCount = usersCount;

                th.ThreadUsers.Add(usr);
            }

            return retList;
        }

        public static List<Message> message_receivers(DBResultSet results)
        {
            List<Message> retList = new List<Message>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                Guid? messageId = table.GetGuid(i, "MessageID");

                if (!messageId.HasValue) continue;

                Message msg = retList.Where(u => u.MessageID == messageId).FirstOrDefault();

                if (msg == null)
                {
                    msg = new Message() { MessageID = messageId };
                    retList.Add(msg);
                }

                User usr = new User()
                {
                    UserID = table.GetGuid(i, "UserID"),
                    UserName = table.GetString(i, "UserName"),
                    FirstName = table.GetString(i, "FirstName"),
                    LastName = table.GetString(i, "LastName")
                };


                int receiversCount = table.GetInt(i, "RevRowNumber", defaultValue: 0).Value;
                if (!msg.ReceiversCount.HasValue || receiversCount > msg.ReceiversCount.Value) msg.ReceiversCount = receiversCount;

                msg.ReceiverUsers.Add(usr);
            }

            return retList;
        }
    }
}
