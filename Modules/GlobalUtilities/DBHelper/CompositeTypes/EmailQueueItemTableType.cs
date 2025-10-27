using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpgsqlTypes;
using Newtonsoft.Json;

namespace RaaiVan.Modules.GlobalUtilities.DBCompositeTypes
{
    [Serializable]
    public class EmailQueueItemTableType : ITableType
    {
        public EmailQueueItemTableType() : base() { } //empty constructor is a must

        [JsonIgnore]
        public string MSSQLName { get { return "EmailQueueItemTableType"; } }

        [PgName("id")]
        public long? ID;

        [PgName("sender_user_id")]
        public Guid? SenderUserID;

        [PgName("action")]
        public string Action;

        [PgName("email")]
        public string Email;

        [PgName("title")]
        public string Title;

        [PgName("email_body")]
        public string EmailBody;

        public EmailQueueItemTableType(long? id, Guid? senderUserId, string action, string email, string title, string emailBody)
        {
            ID = id;
            SenderUserID = senderUserId;
            Action = action;
            Email = email;
            Title = title;
            EmailBody = emailBody;
        }

        public object[] to_array()
        {
            return new List<object>() {
                ID,
                SenderUserID,
                Action,
                Email,
                Title,
                EmailBody
            }.ToArray();
        }

        public EmailQueueItemTableType[] get_array(List<EmailQueueItemTableType> list)
        {
            return list.ToArray();
        }

        public static DBCompositeType<EmailQueueItemTableType> getCompositeType(List<EmailQueueItem> lst)
        {
            if (lst == null) lst = new List<EmailQueueItem>();

            return new DBCompositeType<EmailQueueItemTableType>()
                .add(lst.Select(itm => new EmailQueueItemTableType(
                    id: itm.ID,
                    senderUserId: itm.SenderUserID,
                    action: itm.Action.ToString(),
                    email: itm.Email,
                    title: itm.Title,
                    emailBody: itm.EmailBody)).ToList());
        }
    }
}
