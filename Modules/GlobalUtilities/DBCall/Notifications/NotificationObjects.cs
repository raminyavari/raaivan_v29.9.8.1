using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaaiVan.Modules.Users;
using RaaiVan.Modules.GlobalUtilities;

namespace RaaiVan.Modules.NotificationCenter
{
    public enum SubjectType
    {
        None,
        Node,
        Wiki,
        User,
        Post,
        Comment,
        Question,
        Answer
    }

    public enum ActionType
    {
        None,
        Like,
        Dislike,
        Post,
        Share,
        Comment,
        Remove,
        Question,
        Answer,
        Modify,
        Mention,
        FriendRequest,
        AcceptFriendRequest,
        Publish,

        //Knowledge WorkFlow Actions
        Accept,
        Reject,
        SendToAdmin,
        SendBackForRevision,
        SendToEvaluators,
        RemoveEvaluator,
        Evaluation,
        RefuseEvaluation,
        TerminateEvaluation
        //end of Knowledge WorkFlow Actions
    }

    public enum Media
    {
        None,
        SMS,
        Email
    }

    public enum UserStatus
    {
        None,
        Owner,
        Expert,
        Member,
        Contributor,
        Mentioned,
        Creator,
        Director,
        Fan
    }

    public enum AudienceType
    {
        NotSet,
        Creator,
        Contributor,
        SpecificNode,
        Experts,
        Members,
        RefOwner
    }

    public static class NotificationUtilities
    {
        public static List<DashboardType> get_dashboard_types()
        {
            List<DashboardType> types = new List<DashboardType>();

            Array arr = Enum.GetValues(typeof(DashboardType));
            for (int i = 0, lnt = arr.Length; i < lnt; ++i)
                if ((DashboardType)arr.GetValue(i) != DashboardType.NotSet) types.Add((DashboardType)arr.GetValue(i));

            return types;
        }
    }

    public class Notification
    {
        public Notification()
        {
            Sender = new User();
            SubjectIDs = new List<Guid>();
            RefItemIDs = new List<Guid>();
            Audience = new Dictionary<UserStatus, List<Guid>>();
            ReplacementDic = new Dictionary<string, string>();
            ReceiverUserIDs = new List<Guid>();
        }
        
        public long? NotificationID;
        public Guid? UserID;
        public Guid? SubjectID;
        public List<Guid> SubjectIDs;
        public Guid? RefItemID;
        public List<Guid> RefItemIDs;
        public string SubjectName;
        public SubjectType? SubjectType;
        public User Sender;
        public DateTime? SendDate;
        public ActionType? Action;
        public string Description;
        public string Info;
        public UserStatus? UserStatus;
        public bool? Seen;
        public DateTime? ViewDate;
        public bool? Archive;
        public Dictionary<UserStatus, List<Guid>> Audience;
        public Dictionary<string, string> ReplacementDic;
        public List<Guid> ReceiverUserIDs;

        public string toJson()
        {
            return "{\"NotificationID\":\"" + (!NotificationID.HasValue ? string.Empty : NotificationID.ToString()) + "\"" +
                ",\"UserID\":\"" + (!UserID.HasValue ? string.Empty : UserID.ToString()) + "\"" +
                ",\"SubjectID\":\"" + (SubjectID.HasValue ? SubjectID.ToString() : string.Empty) + "\"" +
                ",\"RefItemID\":\"" + (!RefItemID.HasValue ? string.Empty : RefItemID.Value.ToString()) + "\"" +
                ",\"SubjectName\":\"" + Base64.encode(SubjectName) + "\"" +
                ",\"SubjectType\":\"" + SubjectType + "\"" +
                ",\"Action\":\"" + Action + "\"" +
                ",\"SendDate\":\"" + (!SendDate.HasValue ? string.Empty :
                    GenericDate.get_local_date(SendDate.Value, true)) + "\"" +
                ",\"Description\":\"" + Base64.encode(Description) + "\"" +
                ",\"Info\":\"" + Base64.encode(Info) + "\"" +
                ",\"UserStatus\":\"" + (UserStatus.HasValue ? UserStatus.Value.ToString() : string.Empty) + "\"" +
                ",\"Seen\":" + (!Seen.HasValue ? false : Seen).ToString().ToLower() +
                ",\"ViewDate\":\"" + (!ViewDate.HasValue ? string.Empty :
                    GenericDate.get_local_date(SendDate.Value)) + "\"" +
                ",\"Sender\":" + (Sender == null || !Sender.UserID.HasValue ? "{}" : Sender.toJson()) +
                "}";
        }
    }

    public class MessageTemplate
    {
        public MessageTemplate()
        {
            AudienceType = NotificationCenter.AudienceType.NotSet;
        }

        public Guid? TemplateID;
        public Guid? OwnerID;
        public string BodyText;
        public AudienceType AudienceType;
        public Guid? AudienceRefOwnerID;
        public Guid? AudienceNodeID;
        public string AudienceNodeName;
        public string AudienceNodeType;
        public Guid? AudienceNodeTypeID;
        public bool? AudienceNodeAdmin;
        public Guid? CreatorUserID;
        public DateTime? CreationDate;
        public Guid? LastModifierUserID;
        public DateTime? LastModificationDate;
    }

    public class SubjectTypeClass
    {
        public SubjectType SubjectTypeName;
        public List<ActionType> Actions = new List<ActionType>();
        public List<UserStatus> UserStatuses = new List<UserStatus>();

        public SubjectTypeClass(SubjectType subjectType)
        {
            SubjectTypeName = subjectType;
            _setSubjectTypeActions(SubjectTypeName);
            _setSubjectTypeUserStatuses(SubjectTypeName);
        }

        private void _setSubjectTypeActions(SubjectType _subjectTypeName)
        {
            switch (_subjectTypeName)
            {
                case SubjectType.Answer:
                    Actions.Add(ActionType.Answer);
                    Actions.Add(ActionType.Comment);
                    break;
                case SubjectType.Comment:
                    Actions.Add(ActionType.Like);
                    Actions.Add(ActionType.Dislike);
                    Actions.Add(ActionType.Comment);
                    break;
                case SubjectType.Node:
                    Actions.Add(ActionType.Like);
                    Actions.Add(ActionType.Accept);
                    break;
                case SubjectType.Post:
                    Actions.Add(ActionType.Like);
                    Actions.Add(ActionType.Dislike);
                    Actions.Add(ActionType.Post);
                    Actions.Add(ActionType.Share);
                    break;
                case SubjectType.Question:
                    Actions.Add(ActionType.Like);
                    Actions.Add(ActionType.Comment);
                    Actions.Add(ActionType.Publish);
                    break;
                case SubjectType.Wiki:
                    Actions.Add(ActionType.Modify);
                    break;
                case SubjectType.User:
                    Actions.Add(ActionType.FriendRequest);
                    Actions.Add(ActionType.AcceptFriendRequest);
                    break;
            }
        }

        private void _setSubjectTypeUserStatuses(SubjectType _subjectTypeName)
        {
            switch (_subjectTypeName)
            {
                case SubjectType.Answer:
                    UserStatuses.Add(UserStatus.Owner);
                    UserStatuses.Add(UserStatus.Contributor);
                    UserStatuses.Add(UserStatus.Fan);
                    break;
                case SubjectType.Comment:
                    UserStatuses.Add(UserStatus.Owner);
                    UserStatuses.Add(UserStatus.Mentioned);
                    UserStatuses.Add(UserStatus.Contributor);
                    UserStatuses.Add(UserStatus.Fan);
                    break;
                case SubjectType.Node:
                    UserStatuses.Add(UserStatus.Owner);
                    UserStatuses.Add(UserStatus.Member);
                    UserStatuses.Add(UserStatus.Expert);
                    UserStatuses.Add(UserStatus.Fan);
                    break;
                case SubjectType.Post:
                    UserStatuses.Add(UserStatus.Owner);
                    UserStatuses.Add(UserStatus.Mentioned);
                    UserStatuses.Add(UserStatus.Member);
                    UserStatuses.Add(UserStatus.Fan);
                    break;
                case SubjectType.Question:
                    UserStatuses.Add(UserStatus.Owner);
                    break;
                case SubjectType.Wiki:
                    UserStatuses.Add(UserStatus.Owner);
                    UserStatuses.Add(UserStatus.Member);
                    UserStatuses.Add(UserStatus.Expert);
                    UserStatuses.Add(UserStatus.Fan);
                    break;
                case SubjectType.User:
                    UserStatuses.Add(UserStatus.Mentioned);
                    break;
            }
        }
    }

    public class MessagingActivationOption
    {
        public MessagingActivationOption()
        {
            SubjectType = new SubjectType();
            UserStatus = new UserStatus();
            Action = new ActionType();
            Media = new Media();
        }

        public Guid? OptionID;
        public SubjectType SubjectType;
        public UserStatus UserStatus;
        public ActionType Action;
        public Media Media;
        public string Lang;
        public bool? Enable;
        public bool? AdminEnable;
    }

    public class NotificationMessageTemplate
    {
        public NotificationMessageTemplate()
        {
            SubjectType = new SubjectType();
            Action = new ActionType();
            UserStatus = new UserStatus();
            Media = new Media();
        }

        public Guid? TemplateId;
        public SubjectType SubjectType;
        public ActionType Action;
        public UserStatus UserStatus;
        public Media Media;
        public string Subject;
        public string Text;
        public string Lang;
        public bool? Enable;
    }

    public class NotificationMessage
    {
        public Guid? ReceiverUserID;
        public EmailAddress EmailAddress;
        public PhoneNumber PhoneNumber;
        public string Subject;
        public string Text;
        public Media Media;
        public UserStatus UserStatus;
        public SubjectType SubjectType;
        public ActionType Action;
        public Guid? RefItemID;
        public string Lang;

        public NotificationMessage()
        {
            EmailAddress = new EmailAddress();
            PhoneNumber = new PhoneNumber();
            Media = new Media();
            UserStatus = new UserStatus();
            SubjectType = new SubjectType();
            Action = new ActionType();
        }

        public void send_email(object o)
        {
            Guid applicationId = (Guid)o;
            PublicMethods.send_email(applicationId, EmailAddress.Address, Subject, Text);
        }

        public void send_sms(object o)
        {
            SMSSender.send(PhoneNumber.Number, Text);
        }
    }
}
