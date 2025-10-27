using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaaiVan.Modules.Users;

namespace RaaiVan.Modules.Events
{
    public enum UserStatus
    {
        Pending,
        Accept,
        Rejected,
        Deleted
    }

    public class Event
    {
        private Guid? _EventID;
        private string _EventType;
        private Guid? _OwnerID;
        private string _Title;
        private string _Description;
        private DateTime? _BeginDate;
        private DateTime? _FinishDate;
        private DateTime? _CreationDate;
        private Guid? _CreatorUserID;
        private string _CreatorUserName;
        private string _CreatorFirstName;
        private string _CreatorLastName;
        private DateTime? _LastModificationDate;
        private Guid? _LastModifierUserID;

        public Guid? EventID
        {
            get { return _EventID; }
            set { _EventID = value; }
        }

        public string EventType
        {
            get { return _EventType; }
            set { _EventType = value; }
        }

        public Guid? OwnerID
        {
            get { return _OwnerID; }
            set { _OwnerID = value; }
        }

        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        public DateTime? BeginDate
        {
            get { return _BeginDate; }
            set { _BeginDate = value; }
        }

        public DateTime? FinishDate
        {
            get { return _FinishDate; }
            set { _FinishDate = value; }
        }

        public DateTime? CreationDate
        {
            get { return _CreationDate; }
            set { _CreationDate = value; }
        }

        public Guid? CreatorUserID
        {
            get { return _CreatorUserID; }
            set { _CreatorUserID = value; }
        }

        public string CreatorUserName
        {
            get { return _CreatorUserName; }
            set { _CreatorUserName = value; }
        }

        public string CreatorFirstName
        {
            get { return _CreatorFirstName; }
            set { _CreatorFirstName = value; }
        }

        public string CreatorLastName
        {
            get { return _CreatorLastName; }
            set { _CreatorLastName = value; }
        }

        public DateTime? LastModificationDate
        {
            get { return _LastModificationDate; }
            set { _LastModificationDate = value; }
        }

        public Guid? LastModifierUserID
        {
            get { return _LastModifierUserID; }
            set { _LastModifierUserID = value; }
        }
    }

    public class RelatedUser
    {
        private User _UserInfo;
        private Event _EventInfo;
        private string _Status;
        private bool? _Done;
        private DateTime? _RealFinishDate;

        public RelatedUser()
        {
            _UserInfo = new User();
            _EventInfo = new Event();
        }

        public User UserInfo
        {
            get { return _UserInfo; }
            set { _UserInfo = value; }
        }

        public Event EventInfo
        {
            get { return _EventInfo; }
            set { _EventInfo = value; }
        }

        public string Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        public bool? Done
        {
            get { return _Done; }
            set { _Done = value; }
        }

        public DateTime? RealFinishDate
        {
            get { return _RealFinishDate; }
            set { _RealFinishDate = value; }
        }
    }
}
