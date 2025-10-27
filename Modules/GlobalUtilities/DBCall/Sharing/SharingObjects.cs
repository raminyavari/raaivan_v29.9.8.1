using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Documents;
using RaaiVan.Modules.Users;

namespace RaaiVan.Modules.Sharing
{
    public enum PostTypes
    {
        None,
        Text,
        Knowledge,
        Node,
        Question,
        File,
        User
    }

    public enum PrivacyTypes
    {
        None,
        Public,
        Friends,
        OnlyMe,
        Custom
    }

    public enum PostOwnerType
    {
        None,
        User,
        Node,
        Knowledge,
        WFHistory
    }

    public static class SharingUtilities
    {
        public static PostTypes get_post_type(string postType)
        {
            try { return (PostTypes)Enum.Parse(typeof(PostTypes), postType); }
            catch { return PostTypes.None; }
        }

        public static PostTypes get_post_type(int postTypeId)
        {
            switch (postTypeId)
            {
                case 1:
                    return PostTypes.Text;
                case 2:
                    return PostTypes.Knowledge;
                case 3:
                    return PostTypes.Node;
                case 4:
                    return PostTypes.Question;
                case 5:
                    return PostTypes.File;
                case 6:
                    return PostTypes.User;
                default:
                    return 0;
            }
        }

        public static int get_post_type_id(PostTypes postType)
        {
            switch (postType)
            {
                case PostTypes.Text:
                    return 1;
                case PostTypes.Knowledge:
                    return 2;
                case PostTypes.Node:
                    return 3;
                case PostTypes.Question:
                    return 4;
                case PostTypes.File:
                    return 5;
                case PostTypes.User:
                    return 6;
                default:
                    return 0;
            }
        }

        public static int get_post_type_id(string postType)
        {
            return get_post_type_id(get_post_type(postType));
        }

        public static PrivacyTypes get_privacy_type(string privacyType)
        {
            try { return (PrivacyTypes)Enum.Parse(typeof(PrivacyTypes), privacyType); }
            catch { return PrivacyTypes.None; }
        }
    }

    public class Post
    {
        public Guid? PostID;
        public Guid? RefPostID;
        public int? PostTypeID;
        public string Description;
        public string OriginalDescription;
        public Guid? SharedObjectID;
        public User Sender;
        public User OriginalSender;
        public DateTime? SendDate;
        public DateTime? OriginalSendDate;
        public Guid? LastModifierUserID;
        public DateTime? LastModificationDate;
        public Guid? OwnerID;
        public string OwnerType;
        public string OwnerTitle;
        public string Privacy;
        public long? CommentsCount;
        public long? LikesCount;
        public long? DislikesCount;
        public bool? LikeStatus; //if true, current user likes the post, if false, current user dislikes the post and if null, it's null!
        public bool? HasPicture;
        public List<Comment> Comments;

        public Post()
        {
            Sender = new User();
            OriginalSender = new User();
            Comments = new List<Comment>();
        }

        public string toJson(Guid applicationId, Guid? currentUserId, bool isAdmin)
        {
            bool isOriginal = (RefPostID.HasValue && PostID != RefPostID) ? false : true;

            Guid postId = PostID.Value;
            Guid? refPostId = null;
            if (!isOriginal) refPostId = RefPostID.Value;
            Guid ownerId = OwnerID.HasValue ? OwnerID.Value : Guid.Empty;

            string strLikeStatus = LikeStatus.HasValue ? (LikeStatus.Value ? "true" : "false") : "none";

            string originalSenderProfileImage = !OriginalSender.UserID.HasValue ? string.Empty :
                DocumentUtilities.get_personal_image_address(applicationId, OriginalSender.UserID.Value);
            string senderProfileImage = isOriginal || Sender.UserID == OriginalSender.UserID ||
                !Sender.UserID.HasValue ? originalSenderProfileImage :
                DocumentUtilities.get_personal_image_address(applicationId, Sender.UserID.Value);

            bool removable = isAdmin || ((currentUserId.HasValue && currentUserId != Guid.Empty &&
                ((isOriginal ? OriginalSender.UserID == currentUserId : Sender.UserID == currentUserId) ||
                ownerId == currentUserId)) ? true : false);
            bool editable = isAdmin || (currentUserId.HasValue && currentUserId != Guid.Empty && 
                (isOriginal ? OriginalSender.UserID == currentUserId : Sender.UserID == currentUserId));

            string ownerImageUrl = string.Empty;
            if (OwnerType == PostOwnerType.User.ToString() && Sender.UserID != ownerId)
                ownerImageUrl = DocumentUtilities.get_personal_image_address(applicationId, ownerId);

            Guid pictureId = refPostId.HasValue ? refPostId.Value : postId;

            return "{\"PostID\":\"" + postId.ToString() + "\"" +
                ",\"RefPostID\":\"" + (refPostId.HasValue ? refPostId.Value.ToString() : string.Empty) + "\"" +
                ",\"IsOriginal\":" + isOriginal.ToString().ToLower() +
                ",\"PostType\":\"" + (!PostTypeID.HasValue ? PostTypes.Text.ToString() :
                    Base64.encode(SharingUtilities.get_post_type((int)PostTypeID).ToString())) + "\"" +
                ",\"Description\":\"" + Base64.encode(Description) + "\"" +
                ",\"OriginalDescription\":\"" + Base64.encode(OriginalDescription) + "\"" +
                ",\"SharedObjectID\":\"" + (SharedObjectID.HasValue ? SharedObjectID.ToString() : string.Empty) + "\"" +
                ",\"Sender\":" + Sender.toJson(applicationId, false) +
                ",\"OriginalSender\":" + OriginalSender.toJson(applicationId, false) +
                ",\"SenderProfileImage\":\"" + senderProfileImage + "\"" +
                ",\"OriginalSenderProfileImage\":\"" + originalSenderProfileImage + "\"" +
                ",\"SendDate\":\"" + (!SendDate.HasValue ? string.Empty :
                    GenericDate.get_local_date(SendDate.Value, true)) + "\"" +
                ",\"GregorianSendDate\":\"" + (!SendDate.HasValue ? string.Empty : SendDate.ToString()) + "\"" +
                ",\"NDaysAgo\":" + (!SendDate.HasValue ? "null" : PublicMethods.n_days_ago(SendDate.Value).ToString()) +
                ",\"OriginalSendDate\":\"" + (!OriginalSendDate.HasValue ? string.Empty :
                    GenericDate.get_local_date(OriginalSendDate.Value, true)) + "\"" +
                ",\"GregorianOriginalSendDate\":\"" + (!OriginalSendDate.HasValue ? string.Empty :
                    OriginalSendDate.Value.ToString()) + "\"" +
                ",\"OriginalNDaysAgo\":" + (!OriginalSendDate.HasValue ? "null" : 
                    PublicMethods.n_days_ago(OriginalSendDate.Value).ToString()) +
                ",\"LastModificationDate\":\"" + (!LastModificationDate.HasValue ? string.Empty :
                    GenericDate.get_local_date(LastModificationDate.Value)) + "\"" +
                ",\"Privacy\":\"" + Base64.encode(Privacy) + "\"" +
                ",\"CommentsCount\":" + (CommentsCount.HasValue ? CommentsCount : 0).ToString() +
                ",\"LikesCount\":" + (LikesCount.HasValue ? LikesCount : 0).ToString() +
                ",\"DislikesCount\":" + (DislikesCount.HasValue ? DislikesCount : 0).ToString() +
                ",\"Removable\":" + removable.ToString().ToLower() +
                ",\"Editable\":" + editable.ToString().ToLower() +
                ",\"LikeStatus\":" + (LikeStatus.HasValue ? LikeStatus.Value.ToString().ToLower() : "null") +
                ",\"OwnerID\":\"" + ownerId.ToString() + "\"" +
                ",\"OwnerTitle\":\"" + Base64.encode(OwnerTitle) + "\"" +
                ",\"OwnerType\":\"" + Base64.encode(OwnerType) + "\"" +
                ",\"OwnerImageURL\":\"" + ownerImageUrl + "\"" +
                ",\"PictureID\":\"" + (HasPicture.HasValue && HasPicture.Value &&
                    DocumentUtilities.picture_exists(applicationId, pictureId) ?
                    pictureId.ToString() : string.Empty) + "\"" +
                ",\"Comments\":[" +
                    string.Join(",", Comments.Select(u => u.toJson(applicationId, currentUserId, isAdmin))) + "]" +
                "}";
        }
    }
    
    public class Comment
    {
        public Guid? CommentID;
        public Guid? PostID;
        public string Description;
        public User Sender;
        public DateTime? SendDate;
        public long? LikesCount;
        public long? DislikesCount;
        public Guid? LastModifierUserID;
        public DateTime? LastModificationDate;
        public bool? LikeStatus; //if true, current user likes the comment, if false, current user dislikes the comment and if null, it's null!

        public Comment() {
            Sender = new User();
        }

        public string toJson(Guid applicationId, Guid? currentUserId, bool isAdmin)
        {
            bool editable = currentUserId.HasValue && (isAdmin || Sender.UserID == currentUserId);

            return "{\"CommentID\":\"" + (!CommentID.HasValue ? string.Empty : CommentID.ToString()) + "\"" +
                ",\"PostID\":\"" + (!PostID.HasValue ? string.Empty : PostID.ToString()) + "\"" +
                ",\"Description\":\"" + Base64.encode(Description) + "\"" +
                ",\"Sender\":" + Sender.toJson(applicationId, true) +
                ",\"SendDate\":\"" + (!SendDate.HasValue ? string.Empty : GenericDate.get_local_date(SendDate, true)) + "\"" +
                ",\"GregorianSendDate\":\"" + (!SendDate.HasValue ? string.Empty : SendDate.ToString()) + "\"" +
                ",\"NDaysAgo\":" + (!SendDate.HasValue ? "null" : PublicMethods.n_days_ago(SendDate.Value).ToString()) +
                ",\"LikesCount\":" + (LikesCount.HasValue ? LikesCount.Value : 0).ToString() +
                ",\"DislikesCount\":" + (DislikesCount.HasValue ? DislikesCount.Value : 0).ToString() +
                ",\"LikeStatus\":" + (LikeStatus.HasValue ? LikeStatus.Value.ToString().ToLower() : "null") +
                ",\"Editable\":" + editable.ToString().ToLower() +
                ",\"Removable\":" + editable.ToString().ToLower() +
                "}";
        }
    }

    public class LikeDislike
    {
        private Guid? _ObjectID;
        private Guid? _UserID;
        private bool? _Like;
        private double? _Score;
        private DateTime? _Date;

        public Guid? ObjectID
        {
            get { return _ObjectID; }
            set { _ObjectID = value; }
        }

        public Guid? UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }

        public bool? Like
        {
            get { return _Like; }
            set { _Like = value; }
        }

        public double? Score
        {
            get { return _Score; }
            set { _Score = value; }
        }

        public DateTime? Date
        {
            get { return _Date; }
            set { _Date = value; }
        }
    }
}