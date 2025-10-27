using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Users;
using RaaiVan.Modules.RaaiVanConfig;

namespace RaaiVan.Modules.QA
{
    public enum AnswerBy {
        None,
        All,
        SelectedUsers
    }

    public enum PublishAfter
    {
        None,
        Registration,
        InitialCheck,
        FinalConfirmation,
        ChoosingTheBestAnswer
    }

    public enum NodeSelectType
    {
        None,
        Free,
        Limited
    }

    public enum QuestionStatus
    {
        None,
        Pending, //After Registration & Before Initial Check (if needed)
        Registered, //After Initial Check (if needed)
        Accepted //After Final Confirmation (if needed)
    }

    public class FAQCategory
    {
        public Guid? CategoryID;
        public string Name;
        public bool? HasChild;
    }

    public class QAWorkFlow
    {
        public Guid? WorkFlowID;
        public string Name;
        public string Description;
        public bool? InitialCheckNeeded;
        public bool? FinalConfirmationNeeded;
        public int? ActionDeadline;
        public AnswerBy AnswerBy;
        public PublishAfter PublishAfter;
        public bool? RemovableAfterConfirmation;
        public NodeSelectType NodeSelectType;
        public bool? DisableComments;
        public bool? DisableQuestionLikes;
        public bool? DisableAnswerLikes;
        public bool? DisableCommentLikes;
        public bool? DisableBestAnswer;

        public QAWorkFlow()
        {
            AnswerBy = AnswerBy.None;
            PublishAfter = PublishAfter.None;
            NodeSelectType = NodeSelectType.None;
        }
    }

    public class Question
    {
        public Guid? QuestionID;
        public Guid? WorkFlowID;
        public string Title;
        public string Description;
        public bool? HasBestAnswer;
        public Guid? BestAnswerID;
        public User Sender;
        public DateTime? SendDate;
        public QuestionStatus Status;
        public DateTime? PublicationDate;
        public int? RelatedNodesCount;
        public bool? IsGroup;
        public bool? IsExpertiseDomain;
        public bool? IsFavorite;
        public bool? IsProperty;
        public bool? FromFriend;
        public int? AnswersCount;
        public int? LikesCount;
        public int? DislikesCount;
        public bool? LikeStatus;
        public int? VisitsCount;
        public bool? FollowStatus;
        public List<Answer> Answers;
        public List<Comment> Comments;
        public List<RelatedNode> RelatedNodes;

        public Question()
        {
            Sender = new User();
            Status = QuestionStatus.None;
            Answers = new List<Answer>();
            Comments = new List<Comment>();
            RelatedNodes = new List<RelatedNode>();
        }

        public string toJson(Guid applicationId, Guid? currentUserId, bool isAdmin, QAWorkFlow wf = null)
        {
            string strWorkFlow = string.Empty;

            bool isOwner = Sender.UserID.HasValue && Sender.UserID == currentUserId;
            bool permissionOwnerLevel = isOwner || isAdmin;

            bool removable = permissionOwnerLevel;

            bool notConfirmed = false;

            if (wf != null)
            {
                if ((Status == QuestionStatus.Pending || Status == QuestionStatus.None) &&
                    wf.InitialCheckNeeded.HasValue && wf.InitialCheckNeeded.Value) notConfirmed = true;

                if (!wf.RemovableAfterConfirmation.HasValue || !wf.RemovableAfterConfirmation.Value) removable = isAdmin;

                strWorkFlow = ",\"WorkFlowOptions\":{\"InitialCheckNeeded\":" +
                        (wf.InitialCheckNeeded.HasValue && wf.InitialCheckNeeded.Value).ToString().ToLower() +
                    ",\"FinalConfirmationNeeded\":" + (wf.FinalConfirmationNeeded.HasValue &&
                        wf.FinalConfirmationNeeded.Value).ToString().ToLower() +
                    ",\"RemovableAfterConfirmation\":" + (wf.RemovableAfterConfirmation.HasValue &&
                        wf.RemovableAfterConfirmation.Value).ToString().ToLower() +
                    ",\"DisableComments\":" + (wf.DisableComments.HasValue &&
                        wf.DisableComments.Value).ToString().ToLower() +
                    ",\"DisableQuestionLikes\":" + (wf.DisableQuestionLikes.HasValue &&
                        wf.DisableQuestionLikes.Value).ToString().ToLower() +
                    ",\"DisableAnswerLikes\":" + (wf.DisableAnswerLikes.HasValue &&
                        wf.DisableAnswerLikes.Value).ToString().ToLower() +
                    ",\"DisableCommentLikes\":" + (wf.DisableCommentLikes.HasValue &&
                        wf.DisableCommentLikes.Value).ToString().ToLower() +
                    ",\"DisableBestAnswer\":" + (wf.DisableBestAnswer.HasValue &&
                        wf.DisableBestAnswer.Value).ToString().ToLower() +
                    ",\"ActionDeadline\":" + (!wf.ActionDeadline.HasValue || wf.ActionDeadline < 0 ? 0 :
                        wf.ActionDeadline.Value).ToString() +
                    ",\"AnswerBy\":\"" +
                        (wf.AnswerBy == AnswerBy.None ? string.Empty : wf.AnswerBy.ToString()) + "\"" +
                    ",\"PublishAfter\":\"" +
                        (wf.PublishAfter == PublishAfter.None ? string.Empty : wf.PublishAfter.ToString()) + "\"" +
                    ",\"NodeSelectType\":\"" + (wf.NodeSelectType == NodeSelectType.None ?
                        string.Empty : wf.NodeSelectType.ToString()) + "\"" + "}";
            }

            return "{\"QuestionID\":\"" + (!QuestionID.HasValue ? string.Empty : QuestionID.ToString()) + "\"" +
                ",\"Title\":\"" + Base64.encode(Title) + "\"" +
                ",\"Description\":\"" + Base64.encode(Description) + "\"" +
                ",\"Editable\":" + permissionOwnerLevel.ToString().ToLower() +
                ",\"Removable\":" + removable.ToString().ToLower() +
                ",\"IsOwner\":" + isOwner.ToString().ToLower() +
                ",\"IsWorkFlowAdmin\":" + isAdmin.ToString().ToLower() +
                ",\"FAQEnabled\":" + (Modules.RaaiVanConfig.Modules.QAAdmin(applicationId) &&
                    permissionOwnerLevel).ToString().ToLower() +
                ",\"FeedbacksEnabled\":" + (!notConfirmed && currentUserId.HasValue).ToString().ToLower() +
                ",\"BestAnswerEnabled\":" + (!notConfirmed && permissionOwnerLevel).ToString().ToLower() +
                ",\"NewAnswerEnabled\":" + (!notConfirmed && currentUserId.HasValue).ToString().ToLower() +
                (!HasBestAnswer.HasValue ? string.Empty :
                    ",\"HasBestAnswer\":" + HasBestAnswer.Value.ToString().ToLower()
                ) +
                (!BestAnswerID.HasValue ? string.Empty :
                    ",\"BestAnswerID\":\"" + BestAnswerID.Value.ToString().ToLower() + "\""
                ) +
                (!Sender.UserID.HasValue ? string.Empty : ",\"Sender\":" + Sender.toJson(applicationId, true)) +
                (!SendDate.HasValue ? string.Empty :
                    ",\"SendDate\":\"" + GenericDate.get_local_date(SendDate.Value, true) + "\"" +
                    ",\"SendDate_Gregorian\":\"" + SendDate.Value.ToString() + "\""
                ) +
                (Status == QuestionStatus.None ? string.Empty : ",\"Status\":\"" + Status + "\"") +
                (!PublicationDate.HasValue ? string.Empty :
                    ",\"PublicationDate\":\"" + GenericDate.get_local_date(PublicationDate.Value, true) + "\"" +
                    ",\"PublicationDate_Gregorian\":\"" + PublicationDate.Value.ToString() + "\""
                ) +
                (!RelatedNodesCount.HasValue ? string.Empty :
                    ",\"RelatedNodesCount\":" + RelatedNodesCount.ToString()
                ) +
                (!IsGroup.HasValue ? string.Empty :
                    ",\"IsGroup\":" + IsGroup.Value.ToString().ToLower()
                ) +
                (!IsExpertiseDomain.HasValue ? string.Empty :
                    ",\"IsExpertiseDomain\":" + IsExpertiseDomain.Value.ToString().ToLower()
                ) +
                (!IsFavorite.HasValue ? string.Empty :
                    ",\"IsFavorite\":" + IsFavorite.Value.ToString().ToLower()
                ) +
                (!IsProperty.HasValue ? string.Empty :
                    ",\"IsProperty\":" + IsProperty.Value.ToString().ToLower()
                ) +
                (!FromFriend.HasValue ? string.Empty :
                    ",\"FromFriend\":" + FromFriend.Value.ToString().ToLower()
                ) +
                (!AnswersCount.HasValue ? string.Empty : ",\"AnswersCount\":" + AnswersCount.ToString()) +
                (!LikesCount.HasValue ? string.Empty : ",\"LikesCount\":" + LikesCount.ToString()) +
                (!DislikesCount.HasValue ? string.Empty : ",\"DislikesCount\":" + DislikesCount.ToString()) +
                (!LikeStatus.HasValue ? string.Empty :
                    ",\"LikeStatus\":" + LikeStatus.Value.ToString().ToLower()
                ) +
                (!FollowStatus.HasValue ? string.Empty :
                    ",\"FollowStatus\":" + FollowStatus.Value.ToString().ToLower()
                ) +
                (!VisitsCount.HasValue ? string.Empty : ",\"VisitsCount\":" + VisitsCount.ToString()) +
                (Answers.Count == 0 ? string.Empty :
                    ",\"Answers\":[" + string.Join(",", Answers.Select(
                        u => u.toJson(applicationId, currentUserId, isAdmin)).ToList()) + "]"
                ) +
                (Comments.Count == 0 ? string.Empty :
                    ",\"Comments\":[" + string.Join(",", Comments.Select(
                        u => u.toJson(applicationId, currentUserId, isAdmin)).ToList()) + "]"
                ) +
                (RelatedNodes.Count == 0 ? string.Empty :
                    ",\"Tags\":[" + string.Join(",", RelatedNodes.Select(u => u.toJson())) + "]"
                ) +
                strWorkFlow +
                "}";
        }
    }

    public class Answer
    {
        public Guid? AnswerID;
        public Guid? QuestionID;
        public string AnswerBody;
        public User Sender;
        public DateTime? SendDate;
        public int? LikesCount;
        public int? DislikesCount;
        public bool? LikeStatus;
        public List<Comment> Comments;

        public Answer() {
            Sender = new User();
            Comments = new List<Comment>();
        }

        public string toJson(Guid applicationId, Guid? currentUserId, bool isAdmin)
        {
            return "{\"AnswerID\":\"" + AnswerID.ToString() + "\"" +
                ",\"BodyText\":\"" + Base64.encode(AnswerBody) + "\"" +
                (!Sender.UserID.HasValue ? string.Empty : ",\"Sender\":" + Sender.toJson(applicationId, true)) +
                (!SendDate.HasValue ? string.Empty :
                    ",\"SendDate\":\"" + GenericDate.get_local_date(SendDate.Value, true) + "\"" +
                    ",\"SendDate_Gregorian\":\"" + SendDate.Value.ToString() + "\""
                ) +
                (!LikesCount.HasValue ? string.Empty :
                    ",\"LikesCount\":" + LikesCount.ToString()
                ) +
                (!DislikesCount.HasValue ? string.Empty :
                    ",\"DislikesCount\":" + DislikesCount.ToString()
                ) +
                (!LikeStatus.HasValue ? string.Empty :
                    ",\"LikeStatus\":" + LikeStatus.Value.ToString().ToLower()
                ) +
                (Comments.Count == 0 ? string.Empty :
                    ",\"Comments\":[" + string.Join(",", Comments.Select(u => u.toJson(applicationId, currentUserId, isAdmin))) + "]"
                ) +
                ",\"Editable\":" + (isAdmin || (Sender.UserID == currentUserId)).ToString().ToLower() +
                ",\"Removable\":" + (isAdmin || (Sender.UserID == currentUserId)).ToString().ToLower() +
                "}";
        }
    }

    public class Comment
    {
        public Guid? CommentID;
        public Guid? OwnerID;
        public Guid? ReplyToCommentID;
        public string BodyText;
        public User Sender;
        public DateTime? SendDate;
        public int? LikesCount;
        public bool? LikeStatus;

        public Comment() {
            Sender = new User();
        }

        public string toJson(Guid applicationId, Guid? currentUserId, bool isAdmin)
        {
            return "{\"CommentID\":\"" + CommentID.ToString() + "\"" +
                ",\"BodyText\":\"" + Base64.encode(BodyText) + "\"" +
                (!Sender.UserID.HasValue ? string.Empty : ",\"Sender\":" + Sender.toJson(applicationId, true)) +
                (!LikesCount.HasValue ? string.Empty : ",\"LikesCount\":" + LikesCount.ToString()) +
                (!LikeStatus.HasValue ? string.Empty :
                    ",\"LikeStatus\":" + LikeStatus.Value.ToString().ToLower()
                ) +
                (!SendDate.HasValue ? string.Empty :
                    ",\"SendDate\":\"" + GenericDate.get_local_date(SendDate.Value, true) + "\"" +
                    ",\"SendDate_Gregorian\":\"" + SendDate.Value.ToString() + "\""
                ) +
                ",\"Editable\":" + (isAdmin || (Sender.UserID == currentUserId)).ToString().ToLower() +
                ",\"Removable\":" + (isAdmin || (Sender.UserID == currentUserId)).ToString().ToLower() +
                "}";
        }
    }

    public class RelatedNode
    {
        public Guid? NodeID;
        public string NodeName;
        public string NodeType;
        public int? Count;
        public bool? Deleted;

        public string toJson() {
            return "{\"NodeID\":\"" + (!NodeID.HasValue ? string.Empty : NodeID.ToString()) + "\"" +
                ",\"Name\":\"" + Base64.encode(NodeName) + "\"" +
                ",\"NodeType\":\"" + Base64.encode(NodeType) + "\"" +
                ",\"Count\":" + (!Count.HasValue ? 0 : Count.Value).ToString() +
                ",\"Deleted\":" + (Deleted.HasValue && Deleted.Value).ToString().ToLower() +
                "}";
        }
    }
}