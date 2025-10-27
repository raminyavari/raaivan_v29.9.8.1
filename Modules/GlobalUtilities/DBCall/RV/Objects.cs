
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;

namespace RaaiVan.Modules.GlobalUtilities
{
    [Serializable]
    public class Application
    {
        public Guid? ApplicationID;
        public string Name;
        public string Title;
        public string Description;
        public string AvatarName;
        public Guid? CreatorUserID;
        public Guid? ExpertiseFieldID;
        public string ExpertiseFieldName;

        public Tenant toTenant()
        {
            return !ApplicationID.HasValue ? null : new Tenant(ApplicationID.Value, Name, Title, string.Empty, string.Empty, string.Empty);
        }

        public string toJson(Guid? currentUserId = null, bool icon = false, bool highQualityIcon = false)
        {
            string iconUrl = !icon || !ApplicationID.HasValue ? string.Empty :
                DocumentUtilities.get_application_icon_url(ApplicationID.Value);
            string highQualityIconUrl = !highQualityIcon || !ApplicationID.HasValue ? string.Empty :
                DocumentUtilities.get_application_icon_url(ApplicationID.Value, highQuality: true);
            
            string strDomainOfExpertise = string.Empty;
            if (ExpertiseFieldID.HasValue) strDomainOfExpertise = 
                    ",\"FieldOfExpertise\":{\"ID\":\"" + ExpertiseFieldID.ToString() + "\"" +
                    ",\"Name\":\"" + Base64.encode(ExpertiseFieldName) + "\"" + "}";

            return "{\"ApplicationID\":\"" + (!ApplicationID.HasValue ? string.Empty : ApplicationID.ToString()) + "\"" +
                ",\"Title\":\"" + Base64.encode(string.IsNullOrEmpty(Title) ? Name : Title) + "\"" +
                ",\"Description\":\"" + Base64.encode(Description) + "\"" +
                (string.IsNullOrEmpty(AvatarName) ? string.Empty : 
                    ",\"AvatarName\":\"" + AvatarName + "\"") +
                (!icon ? string.Empty : 
                    ",\"IconURL\":\"" + (string.IsNullOrEmpty(iconUrl) ? "" : iconUrl) + "\"") +
                (!highQualityIcon ? string.Empty : 
                    ",\"HighQualityIconURL\":\"" + highQualityIconUrl + "\"") +
                (!currentUserId.HasValue ? string.Empty :
                    ",\"Removable\":" + (CreatorUserID.HasValue && CreatorUserID == currentUserId).ToString().ToLower()) +
                (!currentUserId.HasValue ? string.Empty :
                    ",\"Editable\":" + (CreatorUserID.HasValue && CreatorUserID == currentUserId).ToString().ToLower()) +
                strDomainOfExpertise +
                "}";
        }
    }

    public class Hierarchy
    {
        public Guid? ID;
        public Guid? ParentID;
        public int? Level;
        public string Name;

        public string toJSON() {
            return "{\"NodeID\":\"" + (!ID.HasValue ? string.Empty : ID.Value.ToString()) + "\"" +
                ",\"Name\":\"" + Base64.encode(Name) + "\"" +
                ",\"ParentID\":\"" + (ParentID.HasValue ? ParentID.ToString() : string.Empty) + "\"" +
                ",\"Level\":" + (Level.HasValue ? Level.Value.ToString() : "null") + "}";
        }
    }

    public enum HTTPMethod
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public enum DashboardType
    {
        NotSet,
        Wiki,
        WorkFlow,
        Knowledge,
        MembershipRequest,
        Question
    }

    public enum DashboardSubType
    {
        NotSet,
        Admin,
        KnowledgeComment,
        EvaluationDone,
        EvaluationRefused,
        Evaluator,
        Knowledgable,
        Revision,
        ExpirationDate
    }

    public class DashboardCount
    {
        public DashboardType Type;
        public DashboardSubType SubType;
        public string SubTypeTitle;
        public Guid? NodeTypeID;
        public string NodeType;
        public DateTime? _DateOfEffect;
        private int? _ToBeDone;
        private int? _NotSeen;
        private int? _Done;
        private int? _DoneAndInWorkFlow;
        private int? _DoneAndNotInWorkFlow;
        public List<DashboardCount> Sub;

        public DashboardCount()
        {
            Type = new DashboardType();
            SubType = DashboardSubType.NotSet;
            Sub = new List<DashboardCount>();
        }

        public DateTime? DateOfEffect
        {
            set { _DateOfEffect = value; }

            get
            {
                try
                {
                    if (Sub != null && Sub.Where(x => x != null).Count() > 0) return Sub.Where(x => x != null).Max(u => u.DateOfEffect);
                    else return _DateOfEffect;
                }
                catch { return null; }
            }
        }

        public int ToBeDone
        {
            set { _ToBeDone = value < 0 ? 0 : value; }

            get
            {
                if (Sub != null && Sub.Where(x => x != null).Count() > 0) return Sub.Where(x => x != null).Sum(u => u.ToBeDone);
                else return !_ToBeDone.HasValue || _ToBeDone.Value < 0 ? 0 : _ToBeDone.Value;
            }
        }

        public int NotSeen
        {
            set { _NotSeen = value < 0 ? 0 : value; }

            get
            {
                if (Sub != null && Sub.Where(x => x != null).Count() > 0) return Sub.Where(x => x != null).Sum(u => u.NotSeen);
                else return !_NotSeen.HasValue || _NotSeen.Value < 0 ? 0 : _NotSeen.Value;
            }
        }

        public int Done
        {
            set { _Done = value < 0 ? 0 : value; }

            get
            {
                if (Sub != null && Sub.Where(x => x != null).Count() > 0) return Sub.Where(x => x != null).Sum(u => u.Done);
                else return !_Done.HasValue || _Done.Value < 0 ? 0 : _Done.Value;
            }
        }

        public int DoneAndInWorkFlow
        {
            set { _DoneAndInWorkFlow = value < 0 ? 0 : value; }

            get
            {
                if (Sub != null && Sub.Where(x => x != null).Count() > 0)
                    return !NodeTypeID.HasValue ? Sub.Where(x => x != null).Sum(u => u.DoneAndInWorkFlow) :
                        Sub.Where(x => x != null).First().DoneAndInWorkFlow;
                else return !_DoneAndInWorkFlow.HasValue || _DoneAndInWorkFlow.Value < 0 ? 0 : _DoneAndInWorkFlow.Value;
            }
        }

        public int DoneAndNotInWorkFlow
        {
            set { _DoneAndNotInWorkFlow = value < 0 ? 0 : value; }

            get
            {
                if (Sub != null && Sub.Where(x => x != null).Count() > 0)
                    return !NodeTypeID.HasValue ? Sub.Where(x => x != null).Sum(u => u.DoneAndNotInWorkFlow) :
                        Sub.Where(x => x != null).First().DoneAndNotInWorkFlow;
                else return !_DoneAndNotInWorkFlow.HasValue || _DoneAndNotInWorkFlow.Value < 0 ? 0 : _DoneAndNotInWorkFlow.Value;
            }
        }

        public string toJson()
        {
            return "{\"Type\":\"" + (Type == DashboardType.NotSet ? string.Empty : Type.ToString()) + "\"" + 
                ",\"SubType\":\"" + (SubType == DashboardSubType.NotSet ? string.Empty : SubType.ToString()) + "\"" +
                ",\"SubTypeTitle\":\"" + Base64.encode(SubTypeTitle) + "\"" +
                ",\"NodeTypeID\":\"" + (!NodeTypeID.HasValue ? string.Empty : NodeTypeID.Value.ToString()) + "\"" +
                ",\"NodeType\":\"" + Base64.encode(NodeType) + "\"" +
                ",\"DateOfEffect\":\"" + (!DateOfEffect.HasValue ? string.Empty : DateOfEffect.Value.ToString()) + "\"" +
                ",\"DateOfEffect_Jalali\":\"" + (!DateOfEffect.HasValue ? string.Empty :
                    GenericDate.get_local_date(DateOfEffect.Value, detail: true, reverse: true)) + "\"" +
                ",\"ToBeDone\":" + ToBeDone.ToString() +
                ",\"NotSeen\":" + NotSeen.ToString() +
                ",\"Done\":" + Done.ToString() +
                ",\"DoneAndInWorkFlow\":" + DoneAndInWorkFlow.ToString() +
                ",\"DoneAndNotInWorkFlow\":" + DoneAndNotInWorkFlow.ToString() +
                ",\"Sub\":[" + (Sub == null ? string.Empty : string.Join(",", Sub.Where(a => a != null).Select(x => x.toJson()))) + "]" +
                "}";
        }
    }

    public class Dashboard
    {
        public Dashboard()
        {
            Type = new DashboardType();
        }

        public long? DashboardID;
        public Guid? UserID;
        public Guid? NodeID;
        public string NodeAdditionalID;
        public string NodeName;
        public string NodeType;
        public DashboardType Type;
        public DashboardSubType SubType;
        public string Info;
        public bool? Removable;
        public Guid? SenderUserID;
        public DateTime? SendDate;
        public DateTime? ExpirationDate;
        public bool? Seen;
        public DateTime? ViewDate;
        public bool? Done;
        public DateTime? ActionDate;
    }

    public class InlineTag
    {
        public string StringID;
        public Guid? ID;
        public string Type;
        public string Value;
        public string Info;

        public void set_id(string id)
        {
            StringID = id;
            Guid _id;
            if (Guid.TryParse(id, out _id)) ID = _id;
        }

        public string get_id()
        {
            return ID.HasValue && ID != Guid.Empty ? ID.Value.ToString() : StringID;
        }

        public string toString() {
            return !ID.HasValue || string.IsNullOrEmpty(Type) || string.IsNullOrEmpty(Value) ? string.Empty :
                "@[[" + ID.ToString() + ":" + Type + ":" + Base64.encode(Value) + 
                (string.IsNullOrEmpty(Info) ? string.Empty : ":" + Base64.encode(Info)) + "]]";
        }
    }

    public enum EmailAction
    {
        None,
        InviteUser
    }

    public class EmailQueueItem
    {
        public long? ID;
        public Guid? SenderUserID;
        public EmailAction Action;
        public string Email;
        public string Title;
        public string EmailBody;
    }

    public class RVJob
    {
        public Thread ThreadObject;
        public int? Interval;
        public DateTime? StartTime;
        public DateTime? EndTime;
        public DateTime? LastActivityDate;
        public long? LastActivityDuration;
        public string ErrorMessage;
        public Guid? TenantID;

        public RVJob(Guid tenantId)
        {
            TenantID = tenantId;
        }

        public bool check_time()
        {
            if (!StartTime.HasValue || !EndTime.HasValue || !TenantID.HasValue) return false;

            DateTime now = DateTime.Now;
            now = new DateTime(2000, 1, 1, now.Hour, now.Minute, now.Second, now.Millisecond);
            StartTime = new DateTime(2000, 1, 1, StartTime.Value.Hour, StartTime.Value.Minute, 0);
            EndTime = new DateTime(2000, 1, 1, EndTime.Value.Hour, EndTime.Value.Minute, 59);

            if (!LastActivityDate.HasValue && now < StartTime) Thread.Sleep((StartTime.Value - now).Milliseconds + 1000);

            return EndTime > StartTime ? (now >= StartTime && now <= EndTime) : (now >= StartTime || now <= EndTime);
        }
    }

    public class DeletedState
    {
        public long? ID;
        public Guid? ObjectID;
        public string ObjectType;
        public DateTime? Date;
        public bool? Deleted;
        public bool? Bidirectional;
        public bool? HasReverse;
        public Guid? RelSourceID;
        public Guid? RelDestinationID;
        public string RelSourceType;
        public string RelDestinationType;
        public Guid? RelCreatorID;
    }

    public enum TagContextType
    {
        None,
        Node,
        WikiChange,
        Post,
        Comment,
        FormElement
    }

    public enum TaggedType
    {
        None,
        Node,
        User,
        File,
        Node_Form,
        Node_Wiki,
        User_Form,
        User_Wiki
    }

    public class TaggedItem
    {
        public Guid? ContextID;
        public Guid? TaggedID;
        public TagContextType ContextType;
        public TaggedType TaggedType;

        public TaggedItem()
        {
            ContextType = TagContextType.None;
            TaggedType = TaggedType.None;
        }

        public TaggedItem(Guid contextId, Guid taggedId, TagContextType contextType, TaggedType taggedType)
        {
            ContextID = contextId;
            TaggedID = taggedId;
            ContextType = contextType;
            TaggedType = taggedType;
        }
    }

    public enum Liked
    {
        None,
        Node,
        Question
    }

    public class Variable
    {
        public long? ID;
        public Guid? OwnerID;
        public string Name;
        public string Value;
        public Guid? CreatorUserID;
        public DateTime? CreationDate;

        public string to_json(bool editable)
        {
            return "{\"ID\":" + (!ID.HasValue ? "null" : ID.Value.ToString()) +
                ",\"OwnerID\":\"" + (!OwnerID.HasValue ? string.Empty : OwnerID.Value.ToString()) + "\"" +
                ",\"Name\":\"" + (string.IsNullOrEmpty(Name) ? string.Empty : Name) + "\"" +
                ",\"Value\":\"" + Base64.encode(Value) + "\"" +
                ",\"Editable\":" + editable.ToString().ToLower() +
                "}";
        }
    }

    public class RVJsonContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Select(p => base.CreateProperty(p, memberSerialization))
                .Union(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Select(f => base.CreateProperty(f, memberSerialization)))
                .ToList();
            props.ForEach(p => { p.Writable = true; p.Readable = true; });
            return props;
        }
    }
}
