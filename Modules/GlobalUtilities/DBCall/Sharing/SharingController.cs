using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using RaaiVan.Modules.Users;
using RaaiVan.Modules.GlobalUtilities;

namespace RaaiVan.Modules.Sharing
{
    public static class SharingController
    {
        private static string GetFullyQualifiedName(string name)
        {
            return "[dbo]." + "[SH_" + name + "]"; //'[dbo].' is database owner and 'SH_' is module qualifier
        }

        public static bool add_post(Guid applicationId, Post info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AddPost"),
                applicationId, info.PostID, info.PostTypeID, info.OriginalDescription, info.SharedObjectID,
                info.OriginalSender.UserID, DateTime.Now, info.OwnerID, info.OwnerType, info.HasPicture, info.Privacy);
        }

        public static bool update_post(Guid applicationId, Post info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("UpdatePost"),
                applicationId, info.PostID, info.Description, info.LastModifierUserID, DateTime.Now);
        }

        public static bool remove_post(Guid applicationId, Guid? postId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeletePost"), applicationId, postId);
        }

        public static List<Post> get_posts(Guid applicationId, Guid? ownerId, Guid? userId, bool? news, 
            DateTime? maxDate, DateTime? minDate, int count = 10)
        {
            return SHParsers.posts(DBConnector.read(applicationId, GetFullyQualifiedName("GetPosts"),
                applicationId, ownerId, userId, news, maxDate, minDate, count));
        }

        public static List<Post> get_posts(Guid applicationId, List<Guid> postIds, Guid? userId)
        {
            return SHParsers.posts(DBConnector.read(applicationId, GetFullyQualifiedName("GetPostsByIDs"),
                applicationId, ProviderUtil.list_to_string<Guid>(postIds), ',', userId));
        }

        public static Post get_post(Guid applicationId, Guid postId, Guid? userId)
        {
            return get_posts(applicationId, new List<Guid> { postId }, userId).FirstOrDefault();
        }

        public static Guid? get_post_owner_id(Guid applicationId, Guid postIdOrCommentId)
        {
            return DBConnector.get_guid(applicationId, GetFullyQualifiedName("GetPostOwnerID"), applicationId, postIdOrCommentId);
        }

        public static Guid? get_post_sender_id(Guid applicationId, Guid postIdOrCommentId)
        {
            return DBConnector.get_guid(applicationId, GetFullyQualifiedName("GetPostSenderID"), applicationId, postIdOrCommentId);
        }

        public static bool share(Guid applicationId, Post info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("Share"),
                applicationId, info.PostID, info.RefPostID, info.OwnerID, info.Description,
                info.Sender.UserID, DateTime.Now, info.OwnerType, info.Privacy);
        }

        public static bool add_comment(Guid applicationId, Comment info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AddComment"),
                applicationId, info.CommentID, info.PostID, info.Description, info.Sender.UserID, DateTime.Now);
        }

        public static bool update_comment(Guid applicationId, Comment info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("UpdateComment"),
                applicationId, info.CommentID, info.Description, info.LastModifierUserID, DateTime.Now);
        }

        public static bool remove_comment(Guid applicationId, Guid? commentId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteComment"), applicationId, commentId);
        }

        public static List<Comment> get_post_comments(Guid applicationId, List<Guid> postIds, Guid? userId)
        {
            return SHParsers.comments(DBConnector.read(applicationId, GetFullyQualifiedName("GetComments"),
                applicationId, ProviderUtil.list_to_string<Guid>(postIds), ',', userId));
        }

        public static List<Comment> get_post_comments(Guid applicationId, Guid postId, Guid? userId)
        {
            return get_post_comments(applicationId, new List<Guid>() { postId }, userId);
        }

        public static List<Comment> get_comments(Guid applicationId, List<Guid> commentIds, Guid? userId)
        {
            return SHParsers.comments(DBConnector.read(applicationId, GetFullyQualifiedName("GetCommentsByIDs"),
                applicationId, ProviderUtil.list_to_string<Guid>(commentIds), ',', userId));
        }

        public static Comment get_comment(Guid applicationId, Guid commentId, Guid? userId)
        {
            return get_comments(applicationId, new List<Guid>() { commentId }, userId).FirstOrDefault();
        }

        public static Guid? get_comment_sender_id(Guid applicationId, Guid commentId)
        {
            return DBConnector.get_guid(applicationId, GetFullyQualifiedName("GetCommentSenderID"), applicationId, commentId);
        }

        public static List<Guid> get_comment_sender_ids(Guid applicationId, Guid postId)
        {
            return DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("GetCommentSenderIDs"), applicationId, postId);
        }

        public static bool like_dislike_post(Guid applicationId, LikeDislike info)
        {
            if (info.Score.HasValue) info.Score = 0;

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("LikeDislikePost"),
                applicationId, info.ObjectID, info.UserID, info.Like, info.Score, DateTime.Now);
        }

        public static bool unlike_post(Guid applicationId, LikeDislike info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("UnlikePost"), applicationId, info.ObjectID, info.UserID);
        }

        public static List<Guid> get_post_fan_ids(Guid applicationId, 
            Guid postId, bool? likeStatus, int? count, long? lowerBoundary, ref long totalCount)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetPostFanIDs"),
                applicationId, postId, likeStatus, count, lowerBoundary);

            return SHParsers.fan_user_ids(results, ref totalCount);
        }

        public static List<Guid> get_post_fan_ids(Guid applicationId, Guid postId)
        {
            long totalCount = 0;
            return get_post_fan_ids(applicationId, postId, true, null, null, ref totalCount);
        }

        public static List<User> get_post_fans(Guid applicationId, 
            Guid postId, bool? likeStatus, int? count, long? lowerBoundary, ref long totalCount)
        {
            return UsersController.get_users(applicationId, 
                get_post_fan_ids(applicationId, postId, likeStatus, count, lowerBoundary, ref totalCount));
        }

        public static List<Guid> get_comment_fan_ids(Guid applicationId, 
            Guid commentId, bool? likeStatus, int? count, long? lowerBoundary, ref long totalCount)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetCommentFanIDs"),
                applicationId, commentId, likeStatus, count, lowerBoundary);

            return SHParsers.fan_user_ids(results, ref totalCount);
        }

        public static List<Guid> get_comment_fan_ids(Guid applicationId, Guid commentId)
        {
            long totalCount = 0;
            return get_comment_fan_ids(applicationId, commentId, true, null, null, ref totalCount);
        }

        public static List<User> get_comment_fans(Guid applicationId, 
            Guid commentId, bool? likeStatus, int? count, long? lowerBoundary, ref long totalCount)
        {
            return UsersController.get_users(applicationId, 
                get_comment_fan_ids(applicationId, commentId, likeStatus, count, lowerBoundary, ref totalCount));
        }

        public static bool like_dislike_comment(Guid applicationId, LikeDislike info)
        {
            if (!info.Score.HasValue) info.Score = 0;

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("LikeDislikeComment"),
                applicationId, info.ObjectID, info.UserID, info.Like, info.Score, DateTime.Now);
        }

        public static bool unlike_comment(Guid applicationId, LikeDislike info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("UnlikeComment"),
                applicationId, info.ObjectID, info.UserID);
        }

        public static long get_posts_count(Guid applicationId, Guid? ownerId, Guid? senderUserId = null)
        {
            return DBConnector.get_long(applicationId, GetFullyQualifiedName("GetPostsCount"), applicationId, ownerId, senderUserId);
        }

        public static long get_shares_count(Guid applicationId, Guid? postId)
        {
            return DBConnector.get_long(applicationId, GetFullyQualifiedName("GetSharesCount"), applicationId, postId);
        }

        public static long get_comments_count(Guid applicationId, Guid? postId, Guid? senderUserId = null)
        {
            return DBConnector.get_long(applicationId, GetFullyQualifiedName("GetCommentsCount"), 
                applicationId, postId, senderUserId);
        }

        public static long get_user_posts_count(Guid applicationId, Guid? userId, int postTypeId = 0)
        {
            return DBConnector.get_long(applicationId, GetFullyQualifiedName("GetUserPostsCount"), 
                applicationId, userId, postTypeId);
        }

        public static long get_post_likes_dislikes_count(Guid applicationId, Guid? postId, bool like = true)
        {
            return DBConnector.get_long(applicationId, GetFullyQualifiedName("GetPostLikesDislikesCount"), 
                applicationId, postId, like);
        }

        public static long get_comment_likes_dislikes_count(Guid applicationId, Guid? commentId, bool like = true)
        {
            return DBConnector.get_long(applicationId, GetFullyQualifiedName("GetCommentLikesDislikesCount"),
                applicationId, commentId, like);
        }
    }
}