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
    public class MessageTableType : ITableType
    {
        public MessageTableType() : base() { } //empty constructor is a must

        [JsonIgnore]
        public string MSSQLName { get { return "MessageTableType"; } }

        [PgName("message_id")]
        public Guid? MessageID;

        [PgName("sender_user_id")]
        public Guid? SenderUserID;

        [PgName("title")]
        public string Title;

        [PgName("message_text")]
        public string MessageText;

        public MessageTableType(Guid? messageId, Guid? senderUserId, string title, string messageText)
        {
            MessageID = messageId;
            SenderUserID = senderUserId;
            Title = title;
            MessageText = messageText;
        }

        public object[] to_array()
        {
            return new List<object>() {
                MessageID,
                SenderUserID,
                Title,
                MessageText
            }.ToArray();
        }

        public MessageTableType[] get_array(List<MessageTableType> list)
        {
            return list.ToArray();
        }
    }
}
