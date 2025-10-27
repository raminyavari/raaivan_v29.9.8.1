using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaaiVan.Modules.Users;
using RaaiVan.Modules.CoreNetwork;
using RaaiVan.Modules.GlobalUtilities;

namespace RaaiVan.Modules.Knowledge
{
    public enum KnowledgeStatus
    {
        NotSet,
        Personal,
        SentToAdmin,
        SentBackForRevision,
        SentToEvaluators,
        Rejected,
        Accepted
    }

    public enum KnowledgeEvaluationType
    {
        NotSet,
        N,          //Questions Related to Knowledge Type
        EN,         //Question Related to Knowledge Type Plus Questions Related to Expertise Domains that are Related to Knowledge
        MN          //Question Related to Knowledge Type Plus Questions Related to Membership Domains that are Related to Knowledge
    }

    public enum KnowledgeEvaluators
    {
        NotSet,
        KnowledgeAdmin,
        Experts,
        AdminMembers,
        Members,
        ExpertsAndMembers
    }

    public enum KnowledgeNodeSelectType
    {
        NotSet,
        Free,
        Fixed,
        Single,
        Limited
    }

    public enum KnowledgeAdminType
    {
        NotSet,
        AreaAdmin,
        ComplexAdmin,
        SpecificNode,
        Registerer
    }

    public enum SubmissionType
    {
        NotSet,
        Disabled,
        Enabled,
        WithConfirmation
    }

    public enum SearchableAfter
    {
        NotSet,
        Registration,
        Confirmation,
        Evaluation
    }

    public enum FeedBackTypes
    {
        None,
        Financial,
        Temporal
    }

    public enum NecessaryItem
    {
        Abstract,
        Keywords,
        Wiki,
        RelatedNodes,
        Attachments,
        DocumentTree,
        NecessaryFieldsOfForm
    }

    public class KnowledgeType
    {
        public Guid? KnowledgeTypeID;
        public string Name;
        public KnowledgeEvaluationType EvaluationType;
        public KnowledgeEvaluators Evaluators;
        public bool? PreEvaluateByOwner;
        public bool? ForceEvaluatorsDescribe;
        public int? MinEvaluationsCount;
        public KnowledgeNodeSelectType NodeSelectType;
        public SearchableAfter SearchableAfter;
        public double? ScoreScale;
        public double? MinAcceptableScore;
        public bool? ConvertEvaluatorsToExperts;
        public bool? EvaluationsEditable;
        public bool? EvaluationsEditableForAdmin;
        public bool? EvaluationsRemovable;
        public bool? UnhideEvaluators;
        public bool? UnhideEvaluations;
        public bool? UnhideNodeCreators;
        public string TextOptions;
        public User Creator;
        public User Modifier;
        public bool? Archive;
        public string AdditionalIDPattern;
        public List<NecessaryItem> NecessaryItems;
        public List<NodeType> CandidateNodeTypes;
        public List<Node> CandidateNodes;

        public KnowledgeType()
        {
            EvaluationType = KnowledgeEvaluationType.NotSet;
            Evaluators = KnowledgeEvaluators.NotSet;
            NodeSelectType = KnowledgeNodeSelectType.NotSet;
            SearchableAfter = SearchableAfter.NotSet;
            Creator = new User();
            Modifier = new User();
            NecessaryItems = new List<NecessaryItem>();
            CandidateNodeTypes = new List<NodeType>();
            CandidateNodes = new List<Node>();
        }

        public string toJson() {
            string[] names = Enum.GetNames(typeof(NecessaryItem));

            if(CandidateNodeTypes == null) CandidateNodeTypes = new List<NodeType>();
            if (CandidateNodes == null) CandidateNodes = new List<Node>();

            string strNecessaryItems = "{" + string.Join(",", names.Select(
                u => "\"" + u + "\":" + NecessaryItems.Any(x => x.ToString() == u).ToString().ToLower()).ToList()) + "}";

            return "{\"KnowledgeTypeID\":\"" + (!KnowledgeTypeID.HasValue ? string.Empty : KnowledgeTypeID.Value.ToString()) + "\"" +
                ",\"CandidateRelations\":{" + 
                    "\"NodeTypes\":[" + string.Join(",", CandidateNodeTypes.Select(u => u.toJson())) + "]" +
                    ",\"Nodes\":[" + string.Join(",", CandidateNodes.Select(u => u.toJson())) + "]" + 
                "}" +
                ",\"NodeSelectType\":\"" + (NodeSelectType == KnowledgeNodeSelectType.NotSet ? string.Empty :
                    NodeSelectType.ToString()) + "\"" +
                ",\"EvaluationType\":\"" + (EvaluationType == KnowledgeEvaluationType.NotSet ? string.Empty :
                    EvaluationType.ToString()) + "\"" +
                ",\"Evaluators\":\"" + (Evaluators == KnowledgeEvaluators.NotSet ? string.Empty :
                    Evaluators.ToString()) + "\"" +
                ",\"PreEvaluateByOwner\":" + (PreEvaluateByOwner.HasValue &&
                    PreEvaluateByOwner.Value).ToString().ToLower() +
                ",\"ForceEvaluatorsDescribe\":" + (ForceEvaluatorsDescribe.HasValue &&
                    ForceEvaluatorsDescribe.Value).ToString().ToLower() +
                ",\"MinEvaluationsCount\":" + (MinEvaluationsCount.HasValue ?
                    MinEvaluationsCount.ToString() : "null") +
                ",\"SearchableAfter\":\"" + (SearchableAfter == SearchableAfter.NotSet ? string.Empty :
                    SearchableAfter.ToString()) + "\"" +
                ",\"ScoreScale\":" + (ScoreScale.HasValue ? ScoreScale.ToString() : "null") +
                ",\"MinAcceptableScore\":" + (MinAcceptableScore.HasValue ?
                    MinAcceptableScore.ToString() : "null") +
                ",\"ConvertEvaluatorsToExperts\":" + (ConvertEvaluatorsToExperts.HasValue &&
                    ConvertEvaluatorsToExperts.Value).ToString().ToLower() +
                ",\"EvaluationsEditable\":" + (EvaluationsEditable.HasValue &&
                    EvaluationsEditable.Value).ToString().ToLower() +
                ",\"EvaluationsEditableForAdmin\":" + (EvaluationsEditableForAdmin.HasValue &&
                    EvaluationsEditableForAdmin.Value).ToString().ToLower() +
                ",\"EvaluationsRemovable\":" + (EvaluationsRemovable.HasValue &&
                    EvaluationsRemovable.Value).ToString().ToLower() +
                ",\"UnhideEvaluators\":" + (UnhideEvaluators.HasValue &&
                    UnhideEvaluators.Value).ToString().ToLower() +
                ",\"UnhideEvaluations\":" + (UnhideEvaluations.HasValue &&
                    UnhideEvaluations.Value).ToString().ToLower() +
                ",\"UnhideNodeCreators\":" + (UnhideNodeCreators.HasValue &&
                    UnhideNodeCreators.Value).ToString().ToLower() +
                ",\"TextOptions\":\"" + Base64.encode(TextOptions) + "\"" + 
                ",\"NecessaryItems\":" + strNecessaryItems +
                "}";
        }   
    }

    public class KnowledgeTypeQuestion
    {
        public Guid? ID;
        public Guid? KnowledgeTypeID;
        public Guid? QuestionID;
        public string QuestionBody;
        public double? Weight;
        public Node RelatedNode;
        public User Creator;
        public DateTime? CreationDate;
        public User LastModifier;
        public DateTime? LastModificationDate;
        public List<AnswerOption> Options;

        public KnowledgeTypeQuestion()
        {
            RelatedNode = new Node();
            Creator = new User();
            LastModifier = new User();
            Options = new List<AnswerOption>();
        }

        public string toJson()
        {
            return "{\"ID\":\"" + (ID.HasValue ? ID.Value.ToString() : string.Empty) + "\"" +
                ",\"QuestionID\":\"" + (QuestionID.HasValue ? QuestionID.ToString() : string.Empty) + "\"" +
                ",\"KnowledgeTypeID\":\"" + (KnowledgeTypeID.HasValue ? KnowledgeTypeID.ToString() : string.Empty) + "\"" +
                ",\"NodeID\":\"" + (RelatedNode.NodeID.HasValue ? RelatedNode.NodeID.ToString() : string.Empty) + "\"" +
                ",\"QuestionBody\":\"" + Base64.encode(QuestionBody) + "\"" +
                ",\"Weight\":" + (Weight.HasValue ? Weight.ToString() : "null") +
                ",\"Options\":[" + string.Join(",", Options.Select(u => u.toJson())) + "]" +
                "}";
        }
    }

    public class EvaluationAnswer {
        public Guid? QuestionID;
        public string Title;
        public string TextValue;
        public double? Score;
        public DateTime? EvaluationDate;
    }

    public class AnswerOption
    {
        public Guid? ID;
        public Guid? TypeQuestionID;
        public string Title;
        public double? Value;

        public string toJson()
        {
            return "{\"ID\":\"" + (ID.HasValue ? ID.ToString() : string.Empty) + "\"" +
                ",\"TypeQuestionID\":\"" + (TypeQuestionID.HasValue ? TypeQuestionID.ToString() : string.Empty) + "\"" +
                ",\"Title\":\"" + Base64.encode(Title) + "\"" +
                ",\"Value\":" + (Value.HasValue ? Value.ToString() : "null") +
                "}";
        }
    }

    public class KnowledgeEvaluation
    {
        public User Evaluator;
        public double? Score;
        public DateTime? EvaluationDate;
        public bool? Removable;
        public int? WFVersionID;

        public KnowledgeEvaluation()
        {
            Evaluator = new User();
        }

        public string toJson(Guid applicationId) {
            return "{\"User\":" + Evaluator.toJson(applicationId, true) +
                (!Score.HasValue ? string.Empty : ",\"Score\":" + Score.ToString()) +
                (!EvaluationDate.HasValue ? string.Empty :
                    ",\"EvaluationDate\":\"" + GenericDate.get_local_date(EvaluationDate.Value, true) + "\"") +
                ",\"Removable\":" + (Removable.HasValue && Removable.Value).ToString().ToLower() +
                (!WFVersionID.HasValue ? string.Empty : ",\"WFVersionID\":" + WFVersionID.Value.ToString()) +
                "}";
        }
    }

    public class KWFHistory
    {
        public long? ID;
        public Guid? KnowledgeID;
        public string Action;
        public List<string> TextOptions;
        public string Description;
        public User Actor;
        public User Deputy;
        public DateTime? ActionDate;
        public long? ReplyToHistoryID;
        public List<KWFHistory> Sub;
        public KnowledgeEvaluation Evaluation;
        public int? WFVersionID;
        public bool? IsCreator;
        public bool? IsContributor;

        public KWFHistory()
        {
            Actor = new User();
            Deputy = new User();
            Sub = new List<KWFHistory>();
            Evaluation = new KnowledgeEvaluation();
            TextOptions = new List<string>();
        }

        public string toJson(Guid applicationId)
        {
            return "{\"ID\":\"" + (!ID.HasValue ? string.Empty : ID.Value.ToString()) + "\"" +
                ",\"Action\":\"" + Action.ToString() + "\"" +
                ",\"TextOptions\":[" + string.Join(",", TextOptions.Select(u => "\"" + Base64.encode(u) + "\"")) + "]" +
                ",\"Description\":\"" + Base64.encode(Description) + "\"" +
                ",\"ActionDate\":\"" + (!ActionDate.HasValue ? string.Empty :
                    GenericDate.get_local_date(ActionDate.Value, true)) + "\"" +
                ",\"Actor\":" + Actor.toJson(applicationId, true) +
                ",\"Deputy\":" + (!Deputy.UserID.HasValue ? "null" : Deputy.toJson(applicationId, true)) +
                (!ReplyToHistoryID.HasValue ? string.Empty :
                    ",\"ReplyToHistoryID\":\"" + ReplyToHistoryID.Value.ToString() + "\"") +
                (Sub == null || Sub.Count == 0 ? string.Empty :
                    ",\"Sub\":[" + string.Join(",", Sub.Select(u => u.toJson(applicationId))) + "]") +
                (Evaluation == null || !Evaluation.Evaluator.UserID.HasValue ? string.Empty :
                    ",\"Evaluation\":" + Evaluation.toJson(applicationId)) +
                (!WFVersionID.HasValue ? string.Empty : ",\"WFVersionID\":" + WFVersionID.Value.ToString()) +
                ",\"IsCreator\":" + (IsCreator.HasValue && IsCreator.Value).ToString().ToLower() +
                ",\"IsContributor\":" + (IsContributor.HasValue && IsContributor.Value).ToString().ToLower() +
                "}";
        }
    }

    public class FeedBack
    {
        private long? _FeedBackID;
        private Guid? _KnowledgeID;
        private User _User;
        private FeedBackTypes _FeedBackType;
        private DateTime? _SendDate;
        private double? _Value;
        private string _Description;


        public FeedBack()
        {
            _User = new User();
        }

        public long? FeedBackID
        {
            get { return _FeedBackID; }
            set { _FeedBackID = value; }

        }

        public Guid? KnowledgeID
        {
            get { return _KnowledgeID; }
            set { _KnowledgeID = value; }

        }

        public User User
        {
            get { return _User; }
            set { _User = value; }

        }

        public FeedBackTypes FeedBackType
        {
            get { return _FeedBackType; }
            set { _FeedBackType = value; }
        }

        public DateTime? SendDate
        {
            get { return _SendDate; }
            set { _SendDate = value; }

        }

        public double? Value
        {
            get { return _Value; }
            set { _Value = value; }

        }

        public string Description
        {
            get { return _Description; }
            set { _Description = value; }

        }
    }
}
