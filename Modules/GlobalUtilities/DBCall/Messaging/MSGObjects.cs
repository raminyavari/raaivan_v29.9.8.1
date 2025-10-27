using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaaiVan.Modules.Users;

namespace RaaiVan.Modules.Messaging
{
    public class ThreadInfo
    {
        private Guid? _ThreadID;
        private bool? _IsGroup;
        private int? _MessagesCount;
        private int? _SentCount;
        private int? _NotSeenCount;
        private long? _ID;
        private int? _UsersCount;

        public ThreadInfo()
        {
            ThreadUsers = new List<User>();
        }

        public Guid? ThreadID
        {
            get { return _ThreadID; }
            set { _ThreadID = value;}
        }

        public bool? IsGroup
        {
            get { return _IsGroup; }
            set { _IsGroup = value; }
        }

        public int? MessagesCount
        {
            get { return _MessagesCount; }
            set { _MessagesCount = value; }
        }

        public int? SentCount
        {
            get { return _SentCount; }
            set { _SentCount = value; }
        }

        public int? NotSeenCount
        {
            get { return _NotSeenCount; }
            set { _NotSeenCount = value; }
        }

        public long? ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public int? UsersCount
        {
            get { return _UsersCount; }
            set { _UsersCount = value; }
        }

        public List<User> ThreadUsers;
    }

    public class Message
    {
        public Message()
        {
            ReceiverUsers = new List<User>();
        }

        public long? ID;
        public Guid? ThreadID;
        public Guid? MessageID;
        public Guid? ForwardedFrom;
        public string Title;
        public string MessageText;
        public DateTime? SendDate;
        public bool? IsGroup;
		public bool? IsSender;
		public bool? Seen;
        public Guid? SenderUserID;
        public string SenderUserName;
        public string SenderFirstName;
        public string SenderLastName;
        public bool? HasAttachment;
        public int? Level;
        public int? ReceiversCount;
        public List<User> ReceiverUsers;
    }
}
