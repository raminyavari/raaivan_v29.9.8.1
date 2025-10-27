using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Users;

namespace RaaiVan.Modules.Sharing
{
    public static class SHParsers
    {
        public static List<Post> posts(DBResultSet results)
        {
            List<Post> retList = new List<Post>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new Post()
                {
                    PostID = table.GetGuid(i, "PostID"),
                    RefPostID = table.GetGuid(i, "RefPostID"),
                    PostTypeID = table.GetInt(i, "PostTypeID"),
                    Description = table.GetString(i, "Description"),
                    OriginalDescription = table.GetString(i, "OriginalDescription"),
                    SharedObjectID = table.GetGuid(i, "SharedObjectID"),
                    SendDate = table.GetDate(i, "SendDate"),
                    OriginalSendDate = table.GetDate(i, "OriginalSendDate"),
                    Sender = new User()
                    {
                        UserID = table.GetGuid(i, "SenderUserID"),
                        FirstName = table.GetString(i, "FirstName"),
                        LastName = table.GetString(i, "LastName"),
                        JobTitle = table.GetString(i, "JobTitle")
                    },
                    OriginalSender = new User()
                    {
                        UserID = table.GetGuid(i, "OriginalSenderUserID"),
                        FirstName = table.GetString(i, "OriginalFirstName"),
                        LastName = table.GetString(i, "OriginalLastName"),
                        JobTitle = table.GetString(i, "OriginalJobTitle")
                    },
                    LastModificationDate = table.GetDate(i, "LastModificationDate"),
                    OwnerID = table.GetGuid(i, "OwnerID"),
                    OwnerType = table.GetString(i, "OwnerType"),
                    Privacy = table.GetString(i, "Privacy"),
                    CommentsCount = table.GetLong(i, "CommentsCount"),
                    LikesCount = table.GetLong(i, "LikesCount"),
                    DislikesCount = table.GetLong(i, "DislikesCount"),
                    LikeStatus = table.GetBool(i, "LikeStatus"),
                    HasPicture = table.GetBool(i, "HasPicture")
                });
            }

            return retList;
        }

        public static List<Comment> comments(DBResultSet results)
        {
            List<Comment> retList = new List<Comment>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new Comment()
                {
                    CommentID = table.GetGuid(i, "CommentID"),
                    PostID = table.GetGuid(i, "PostID"),
                    Description = table.GetString(i, "Description"),
                    Sender = new User()
                    {
                        UserID = table.GetGuid(i, "SenderUserID"),
                        FirstName = table.GetString(i, "FirstName"),
                        LastName = table.GetString(i, "LastName")
                    },
                    SendDate = table.GetDate(i, "SendDate"),
                    LikesCount = table.GetLong(i, "LikesCount"),
                    DislikesCount = table.GetLong(i, "DislikesCount"),
                    LikeStatus = table.GetBool(i, "LikeStatus")
                });
            }

            return retList;
        }

        public static List<Guid> fan_user_ids(DBResultSet results, ref long totalCount)
        {
            List<Guid> retList = new List<Guid>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                totalCount = table.GetLong(i, "TotalCount", defaultValue: 0).Value;

                Guid? val = table.GetGuid(i, "UserID");
                if (val.HasValue) retList.Add(val.Value);
            }

            return retList;
        }

        public static List<Guid> fan_user_ids(DBResultSet results) {
            long totalCount = 0;
            return fan_user_ids(results, ref totalCount);
        }
    }
}
