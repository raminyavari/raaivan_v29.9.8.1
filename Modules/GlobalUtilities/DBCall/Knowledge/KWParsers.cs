using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaaiVan.Modules.CoreNetwork;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Users;

namespace RaaiVan.Modules.Knowledge
{
    public static class KWParsers
    {
        public static List<KnowledgeType> knowledge_types(DBResultSet results)
        {
            List<KnowledgeType> retList = new List<KnowledgeType>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new KnowledgeType()
                {
                    KnowledgeTypeID = table.GetGuid(i, "KnowledgeTypeID"),
                    Name = table.GetString(i, "KnowledgeType"),
                    ScoreScale = table.GetInt(i, "ScoreScale"),
                    MinAcceptableScore = table.GetDouble(i, "MinAcceptableScore"),
                    MinEvaluationsCount = table.GetInt(i, "MinEvaluationsCount"),
                    ConvertEvaluatorsToExperts = table.GetBool(i, "ConvertEvaluatorsToExperts"),
                    EvaluationsEditable = table.GetBool(i, "EvaluationsEditable"),
                    EvaluationsEditableForAdmin = table.GetBool(i, "EvaluationsEditableForAdmin"),
                    EvaluationsRemovable = table.GetBool(i, "EvaluationsRemovable"),
                    UnhideEvaluators = table.GetBool(i, "UnhideEvaluators"),
                    UnhideEvaluations = table.GetBool(i, "UnhideEvaluations"),
                    UnhideNodeCreators = table.GetBool(i, "UnhideNodeCreators"),
                    TextOptions = table.GetString(i, "TextOptions"),
                    PreEvaluateByOwner = table.GetBool(i, "PreEvaluateByOwner"),
                    ForceEvaluatorsDescribe = table.GetBool(i, "ForceEvaluatorsDescribe"),
                    AdditionalIDPattern = table.GetString(i, "AdditionalIDPattern", defaultValue: CNUtilities.DefaultAdditionalIDPattern),
                    NodeSelectType = table.GetEnum<KnowledgeNodeSelectType>(i, "NodeSelectType",
                        defaultValue: KnowledgeNodeSelectType.NotSet),
                    EvaluationType = table.GetEnum<KnowledgeEvaluationType>(i, "EvaluationType",
                        defaultValue: KnowledgeEvaluationType.NotSet),
                    Evaluators = table.GetEnum<KnowledgeEvaluators>(i, "Evaluators", defaultValue: KnowledgeEvaluators.NotSet),
                    SearchableAfter = table.GetEnum<SearchableAfter>(i, "SearchableAfter", defaultValue: SearchableAfter.NotSet)
                });
            }

            return retList;
        }

        public static List<KnowledgeTypeQuestion> questions(DBResultSet results)
        {
            List<KnowledgeTypeQuestion> retList = new List<KnowledgeTypeQuestion>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new KnowledgeTypeQuestion()
                {
                    ID = table.GetGuid(i, "ID"),
                    KnowledgeTypeID = table.GetGuid(i, "KnowledgeTypeID"),
                    QuestionID = table.GetGuid(i, "QuestionID"),
                    QuestionBody = table.GetString(i, "QuestionBody"),
                    Weight = table.GetDouble(i, "Weight"),
                    RelatedNode = new Node()
                    {
                        NodeID = table.GetGuid(i, "RelatedNodeID"),
                        Name = table.GetString(i, "RelatedNodeName")
                    }
                });
            }

            return retList;
        }

        public static List<EvaluationAnswer> answers(DBResultSet results)
        {
            List<EvaluationAnswer> retList = new List<EvaluationAnswer>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new EvaluationAnswer()
                {
                    QuestionID = table.GetGuid(i, "QuestionID"),
                    Title = table.GetString(i, "Title"),
                    TextValue = table.GetString(i, "TextValue"),
                    Score = table.GetDouble(i, "Score"),
                    EvaluationDate = table.GetDate(i, "EvaluationDate")
                });
            }

            return retList;
        }

        public static List<AnswerOption> answer_options(DBResultSet results)
        {
            List<AnswerOption> retList = new List<AnswerOption>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new AnswerOption()
                {
                    ID = table.GetGuid(i, "ID"),
                    TypeQuestionID = table.GetGuid(i, "TypeQuestionID"),
                    Title = table.GetString(i, "Title"),
                    Value = table.GetDouble(i, "Value")
                });
            }

            return retList;
        }

        public static void workflow_action_results(RVDataTable table, ref bool result,
            ref bool accepted, ref bool searchabilityActivated, ref string status)
        {
            result = table.GetInt(0, "Result", defaultValue: 0).Value > 0;
            accepted = table.GetBool(0, "Accepted", defaultValue: false).Value;
            searchabilityActivated = table.GetBool(0, "SearchabilityActivated", defaultValue: false).Value;
            status = table.GetString(0, "Status");
        }

        public static List<Dashboard> save_evaluation_form(DBResultSet results,
            ref bool result, ref bool accepted, ref bool searchabilityActivated, ref string status)
        {
            RVDataTable table = results.get_table();

            List<Dashboard> dashboards = new List<Dashboard>();

            if (DBConnector.get_dashboards(table, ref dashboards) <= 0) return dashboards;

            if (results.TablesCount > 1)
                workflow_action_results(results.get_table(1), ref result, ref accepted, ref searchabilityActivated, ref status);

            return dashboards;
        }

        public static List<KnowledgeEvaluation> evaluations(DBResultSet results)
        {
            List<KnowledgeEvaluation> retList = new List<KnowledgeEvaluation>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new KnowledgeEvaluation()
                {
                    Evaluator = new User()
                    {
                        UserID = table.GetGuid(i, "UserID"),
                        UserName = table.GetString(i, "UserName"),
                        FirstName = table.GetString(i, "FirstName"),
                        LastName = table.GetString(i, "LastName")
                    },
                    Score = table.GetDouble(i, "Score"),
                    EvaluationDate = table.GetDate(i, "EvaluationDate"),
                    WFVersionID = table.GetInt(i, "WFVersionID")
                });
            }

            return retList;
        }

        public static List<KWFHistory> history(DBResultSet results)
        {
            List<KWFHistory> retList = new List<KWFHistory>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new KWFHistory()
                {
                    ID = table.GetLong(i, "ID"),
                    KnowledgeID = table.GetGuid(i, "KnowledgeID"),
                    Action = table.GetString(i, "Action"),
                    TextOptions = table.GetString(i, "TextOptions", defaultValue: "")
                        .Split('~').Select(u => u.Trim()).Where(u => !string.IsNullOrEmpty(u)).ToList(),
                    Description = table.GetString(i, "Description"),
                    Actor = new User()
                    {
                        UserID = table.GetGuid(i, "ActorUserID"),
                        UserName = table.GetString(i, "ActorUserName"),
                        FirstName = table.GetString(i, "ActorFirstName"),
                        LastName = table.GetString(i, "ActorLastName")
                    },
                    Deputy = new User()
                    {
                        UserID = table.GetGuid(i, "DeputyUserID"),
                        UserName = table.GetString(i, "DeputyUserName"),
                        FirstName = table.GetString(i, "DeputyFirstName"),
                        LastName = table.GetString(i, "DeputyLastName")
                    },
                    ActionDate = table.GetDate(i, "ActionDate"),
                    ReplyToHistoryID = table.GetLong(i, "ReplyToHistoryID"),
                    WFVersionID = table.GetInt(i, "WFVersionID"),
                    IsCreator = table.GetBool(i, "IsCreator"),
                    IsContributor = table.GetBool(i, "IsContributor")
                });
            }

            return retList;
        }

        public static List<FeedBack> feedbacks(DBResultSet results)
        {
            List<FeedBack> retList = new List<FeedBack>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                retList.Add(new FeedBack()
                {
                    FeedBackID = table.GetLong(i, "FeedBackID"),
                    KnowledgeID = table.GetGuid(i, "KnowledgeID"),
                    FeedBackType = table.GetInt(i, "FeedBackTypeID", defaultValue: 1).Value == 1 ?
                        FeedBackTypes.Financial : FeedBackTypes.Temporal,
                    SendDate = table.GetDate(i, "SendDate"),
                    Value = table.GetDouble(i, "Value"),
                    Description = table.GetString(i, "Description"),
                    User = new User()
                    {
                        UserID = table.GetGuid(i, "UserID"),
                        UserName = table.GetString(i, "UserName"),
                        FirstName = table.GetString(i, "FirstName"),
                        LastName = table.GetString(i, "LastName")
                    }
                });
            }

            return retList;
        }

        public static void feedback_status(DBResultSet results, ref double totalFinancialFeedbacks,
            ref double totalTemporalFeedbacks, ref double financialFeedbackStatus, ref double temporalFeedbackStatus)
        {
            RVDataTable table = results.get_table();

            totalFinancialFeedbacks = table.GetDouble(0, "TotalFinancialFeedBacks", defaultValue: 0).Value;
            totalTemporalFeedbacks = table.GetDouble(0, "TotalTemporalFeedBacks", defaultValue: 0).Value;
            financialFeedbackStatus = table.GetDouble(0, "FinancialFeedBackStatus", defaultValue: 0).Value;
            temporalFeedbackStatus = table.GetDouble(0, "TemporalFeedBackStatus", defaultValue: 0).Value;
        }

        public static List<NecessaryItem> necessary_items(DBResultSet results)
        {
            List<NecessaryItem> retList = new List<NecessaryItem>();

            RVDataTable table = results.get_table();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                NecessaryItem? itm = table.GetEnum<NecessaryItem>(i, "ItemName");
                if(itm.HasValue) retList.Add(itm.Value);
            }

            return retList;
        }
    }
}
