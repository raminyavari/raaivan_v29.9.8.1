using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Users;

namespace RaaiVan.Modules.QA
{
    public static class QAParsers
    {
        public static List<QAWorkFlow> workflows(DBResultSet results)
        {
            List<QAWorkFlow> retList = new List<QAWorkFlow>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new QAWorkFlow()
                {
                    WorkFlowID = table.GetGuid(i, "WorkFlowID"),
                    Name = table.GetString(i, "Name"),
                    Description = table.GetString(i, "Description"),
                    InitialCheckNeeded = table.GetBool(i, "InitialCheckNeeded"),
                    FinalConfirmationNeeded = table.GetBool(i, "FinalConfirmationNeeded"),
                    ActionDeadline = table.GetInt(i, "ActionDeadline"),
                    RemovableAfterConfirmation = table.GetBool(i, "RemovableAfterConfirmation"),
                    DisableComments = table.GetBool(i, "DisableComments"),
                    DisableQuestionLikes = table.GetBool(i, "DisableQuestionLikes"),
                    DisableAnswerLikes = table.GetBool(i, "DisableAnswerLikes"),
                    DisableCommentLikes = table.GetBool(i, "DisableCommentLikes"),
                    DisableBestAnswer = table.GetBool(i, "DisableBestAnswer"),
                    AnswerBy = table.GetEnum<AnswerBy>(i, "AnswerBy", defaultValue: AnswerBy.None),
                    PublishAfter = table.GetEnum<PublishAfter>(i, "PublishAfter", defaultValue: PublishAfter.None),
                    NodeSelectType = table.GetEnum<NodeSelectType>(i, "NodeSelectType", defaultValue: NodeSelectType.None)
                });
            }

            return retList;
        }

        public static List<FAQCategory> faq_categories(DBResultSet results)
        {
            List<FAQCategory> retList = new List<FAQCategory>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new QA.FAQCategory()
                {
                    CategoryID = table.GetGuid(i, "CategoryID"),
                    Name = table.GetString(i, "Name"),
                    HasChild = table.GetBool(i, "HasChild")
                });
            }

            return retList;
        }

        public static List<Question> questions(DBResultSet results, bool full, ref long totalCount)
        {
            List<Question> retList = new List<Question>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                Question question = new Question()
                {
                    QuestionID = table.GetGuid(i, "QuestionID"),
                    Title = table.GetString(i, "Title"),
                    SendDate = table.GetDate(i, "SendDate"),
                    Sender = new User()
                    {
                        UserID = table.GetGuid(i, "SenderUserID"),
                        UserName = table.GetString(i, "SenderUserName"),
                        FirstName = table.GetString(i, "SenderFirstName"),
                        LastName = table.GetString(i, "SenderLastName")
                    },
                    AnswersCount = table.GetInt(i, "AnswersCount"),
                    LikesCount = table.GetInt(i, "LikesCount"),
                    DislikesCount = table.GetInt(i, "DislikesCount"),
                    Status = table.GetEnum<QuestionStatus>(i, "Status", defaultValue: QuestionStatus.None)
                };

                if (!full)
                {
                    totalCount = table.GetLong(i, "TotalCount", defaultValue: 0).Value;

                    question.HasBestAnswer = table.GetBool(i, "HasBestAnswer");
                    question.RelatedNodesCount = table.GetInt(i, "RelatedNodesCount");

                    question.IsGroup = table.GetBool(i, "IsGroup");
                    question.IsExpertiseDomain = table.GetBool(i, "IsExpertiseDomain");
                    question.IsFavorite = table.GetBool(i, "IsFavorite");
                    question.IsProperty = table.GetBool(i, "IsProperty");
                    question.FromFriend = table.GetBool(i, "FromFriend");
                }
                else
                {
                    question.WorkFlowID = table.GetGuid(i, "WorkFlowID");
                    question.Description = table.GetString(i, "Description");
                    question.BestAnswerID = table.GetGuid(i, "BestAnswerID");
                    question.LikeStatus = table.GetBool(i, "LikeStatus");
                    question.FollowStatus = table.GetBool(i, "FollowStatus");
                    question.PublicationDate = table.GetDate(i, "PublicationDate");
                }

                retList.Add(question);
            }

            return retList;
        }

        public static List<Question> questions(DBResultSet results, bool full) {
            long totalCount = 0;
            return questions(results, full, ref totalCount);
        }

        public static List<RelatedNode> related_nodes(DBResultSet results, ref long totalCount)
        {
            List<RelatedNode> retList = new List<RelatedNode>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                totalCount = table.GetLong(i, "TotalCount", defaultValue: 0).Value;

                retList.Add(new RelatedNode()
                {
                    NodeID = table.GetGuid(i, "NodeID"),
                    NodeName = table.GetString(i, "NodeName"),
                    NodeType = table.GetString(i, "NodeType"),
                    Count = table.GetInt(i, "Count"),
                    Deleted = table.GetBool(i, "Deleted")
                });
            }

            return retList;
        }

        public static List<RelatedNode> related_nodes(DBResultSet results) {
            long totalCount = 0;
            return related_nodes(results, ref totalCount);
        }

        public static List<Answer> answers(DBResultSet results)
        {
            List<Answer> retList = new List<Answer>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new Answer()
                {
                    AnswerID = table.GetGuid(i, "AnswerID"),
                    QuestionID = table.GetGuid(i, "QuestionID"),
                    AnswerBody = table.GetString(i, "AnswerBody"),
                    Sender = new User()
                    {
                        UserID = table.GetGuid(i, "SenderUserID"),
                        UserName = table.GetString(i, "SenderUserName"),
                        FirstName = table.GetString(i, "SenderFirstName"),
                        LastName = table.GetString(i, "SenderLastName")
                    },
                    SendDate = table.GetDate(i, "SendDate"),
                    LikesCount = table.GetInt(i, "LikesCount"),
                    DislikesCount = table.GetInt(i, "DislikesCount"),
                    LikeStatus = table.GetBool(i, "LikeStatus")
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
                    OwnerID = table.GetGuid(i, "OwnerID"),
                    ReplyToCommentID = table.GetGuid(i, "ReplyToCommentID"),
                    BodyText = table.GetString(i, "BodyText"),
                    Sender = new User()
                    {
                        UserID = table.GetGuid(i, "SenderUserID"),
                        UserName = table.GetString(i, "SenderUserName"),
                        FirstName = table.GetString(i, "SenderFirstName"),
                        LastName = table.GetString(i, "SenderLastName")
                    },
                    SendDate = table.GetDate(i, "SendDate"),
                    LikesCount = table.GetInt(i, "LikesCount"),
                    LikeStatus = table.GetBool(i, "LikeStatus")
                });
            }

            return retList;
        }
    }
}
