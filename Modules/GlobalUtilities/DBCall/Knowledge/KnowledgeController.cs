using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaaiVan.Modules.CoreNetwork;
using RaaiVan.Modules.Users;
using RaaiVan.Modules.Documents;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.GlobalUtilities.DBCompositeTypes;

namespace RaaiVan.Modules.Knowledge
{
    public static class KnowledgeController
    {
        private static string GetFullyQualifiedName(string name)
        {
            return "[dbo]." + "[KW_" + name + "]"; //'[dbo].' is database owner and 'KW_' is module qualifier
        }

        public static bool initialize(Guid applicationId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("Initialize"), applicationId);
        }

        public static bool add_knowledge_type(Guid applicationId, Guid knowledgeTypeId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AddKnowledgeType"),
                applicationId, knowledgeTypeId, currentUserId, DateTime.Now);
        }

        public static bool remove_knowledge_type(Guid applicationId, Guid knowledgeTypeId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteKnowledgeType"),
                applicationId, knowledgeTypeId, currentUserId, DateTime.Now);
        }

        public static List<KnowledgeType> get_knowledge_types(Guid applicationId, List<Guid> knowledgeTypeIds)
        {
            return KWParsers.knowledge_types(DBConnector.read(applicationId, GetFullyQualifiedName("GetKnowledgeTypes"),
                applicationId, ProviderUtil.list_to_string<Guid>(knowledgeTypeIds), ','));
        }

        public static KnowledgeType get_knowledge_type(Guid applicationId, Guid knowledgeTypeIdOrKnowledgeId)
        {
            return get_knowledge_types(applicationId, new List<Guid>() { knowledgeTypeIdOrKnowledgeId }).FirstOrDefault();
        }

        public static List<KnowledgeType> get_knowledge_types(Guid applicationId)
        {
            List<Guid> knowledgeTypeIds = new List<Guid>();
            return get_knowledge_types(applicationId, knowledgeTypeIds);
        }

        public static bool set_evaluation_type(Guid applicationId, Guid knowledgeTypeId, KnowledgeEvaluationType evaluationType)
        {
            string strEvaluationType = evaluationType == KnowledgeEvaluationType.NotSet ? string.Empty : evaluationType.ToString();

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetEvaluationType"), 
                applicationId, knowledgeTypeId, strEvaluationType);
        }

        public static bool set_evaluators(Guid applicationId, 
            Guid knowledgeTypeId, KnowledgeEvaluators evaluators, int? minEvaluationsCount)
        {
            string strEvaluators = evaluators == KnowledgeEvaluators.NotSet ? string.Empty : evaluators.ToString();

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetEvaluators"),
                applicationId, knowledgeTypeId, strEvaluators, minEvaluationsCount);
        }

        public static bool set_pre_evaluate_by_owner(Guid applicationId, Guid knowledgeTypeId, bool value)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetPreEvaluateByOwner"),
                applicationId, knowledgeTypeId, value);
        }

        public static bool set_force_evaluators_describe(Guid applicationId, Guid knowledgeTypeId, bool value)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetForceEvaluatorsDescribe"),
                applicationId, knowledgeTypeId, value);
        }

        public static bool set_node_select_type(Guid applicationId, Guid knowledgeTypeId, KnowledgeNodeSelectType nodeSelectType)
        {
            string strSelectType = nodeSelectType == KnowledgeNodeSelectType.NotSet ? string.Empty : nodeSelectType.ToString();

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetNodeSelectType"),
                applicationId, knowledgeTypeId, strSelectType);
        }

        public static bool set_submission_type(Guid applicationId, Guid knowledgeTypeId, SubmissionType submissionType)
        {
            string strSubmissionType = submissionType == SubmissionType.NotSet ? string.Empty : submissionType.ToString();

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetSubmissionType"),
                applicationId, knowledgeTypeId, strSubmissionType);
        }

        public static bool set_searchability_type(Guid applicationId, Guid knowledgeTypeId, SearchableAfter searchableAfter)
        {
            string strSearchable = searchableAfter == SearchableAfter.NotSet ? string.Empty : searchableAfter.ToString();

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetSearchabilityType"),
                applicationId, knowledgeTypeId, strSearchable);
        }

        public static bool set_score_scale(Guid applicationId, Guid knowledgeTypeId, int? scoreScale)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetScoreScale"), 
                applicationId, knowledgeTypeId, scoreScale);
        }

        public static bool set_min_acceptable_score(Guid applicationId, Guid knowledgeTypeId, double? minAcceptableScore)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetMinAcceptableScore"),
                applicationId, knowledgeTypeId, minAcceptableScore);
        }

        public static bool set_convert_evaluators_to_experts(Guid applicationId, Guid knowledgeTypeId, bool value)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetConvertEvaluatorsToExperts"),
                applicationId, knowledgeTypeId, value);
        }

        public static bool set_evaluations_editable(Guid applicationId, Guid knowledgeTypeId, bool value)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetEvaluationsEditable"),
                applicationId, knowledgeTypeId, value);
        }

        public static bool set_evaluations_editable_for_admin(Guid applicationId, Guid knowledgeTypeId, bool value)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetEvaluationsEditableForAdmin"),
                applicationId, knowledgeTypeId, value);
        }

        public static bool set_evaluations_removable(Guid applicationId, Guid knowledgeTypeId, bool value)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetEvaluationsRemovable"),
                applicationId, knowledgeTypeId, value);
        }

        public static bool set_unhide_evaluators(Guid applicationId, Guid knowledgeTypeId, bool value)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetUnhideEvaluators"),
                applicationId, knowledgeTypeId, value);
        }

        public static bool set_unhide_evaluations(Guid applicationId, Guid knowledgeTypeId, bool value)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetUnhideEvaluations"),
                applicationId, knowledgeTypeId, value);
        }

        public static bool set_unhide_node_creators(Guid applicationId, Guid knowledgeTypeId, bool value)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetUnhideNodeCreators"),
                applicationId, knowledgeTypeId, value);
        }

        public static bool set_text_options(Guid applicationId, Guid knowledgeTypeId, string value)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetTextOptions"),
                applicationId, knowledgeTypeId, value);
        }

        public static bool set_candidate_relations(Guid applicationId, Guid knowledgeTypeId, 
            List<Guid> nodeTypeIds, List<Guid> nodeIds, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetCandidateRelations"),
                applicationId, knowledgeTypeId, ProviderUtil.list_to_string<Guid>(nodeTypeIds),
                ProviderUtil.list_to_string<Guid>(nodeIds), ',', currentUserId, DateTime.Now);
        }

        public static List<Node> get_candidate_node_relations(Guid applicationId, Guid knowledgeTypeIdOrKnowledgeId)
        {
            List<Guid> nodeIds = DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("GetCandidateNodeRelationIDs"),
                applicationId, knowledgeTypeIdOrKnowledgeId);
            return CNController.get_nodes(applicationId, nodeIds, full: null, currentUserId: null);
        }

        public static List<NodeType> get_candidate_node_type_relations(Guid applicationId, 
            Guid knowledgeTypeIdOrKnowledgeId)
        {
            List<Guid> nodeTypeIds = DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("GetCandidateNodeTypeRelationIDs"),
                applicationId, knowledgeTypeIdOrKnowledgeId);
            return CNController.get_node_types(applicationId, nodeTypeIds);
        }

        public static bool add_question(Guid applicationId, KnowledgeTypeQuestion question, ref string errorMessage)
        {
            return DBConnector.succeed(applicationId, ref errorMessage, GetFullyQualifiedName("AddQuestion"),
                applicationId, question.ID, question.KnowledgeTypeID, question.RelatedNode.NodeID, 
                question.QuestionBody, question.Creator.UserID, DateTime.Now);
        }

        public static bool modify_question(Guid applicationId, KnowledgeTypeQuestion question)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ModifyQuestion"),
                applicationId, question.ID, question.QuestionBody, question.LastModifier.UserID, DateTime.Now);
        }

        public static bool set_questions_order(Guid applicationId, List<Guid> ids)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetQuestionsOrder"),
                applicationId, ProviderUtil.list_to_string<Guid>(ids), ',');
        }
        
        public static bool set_question_weight(Guid applicationId, Guid id, double weight, ref string errorMessage)
        {
            return DBConnector.succeed(applicationId, ref errorMessage, GetFullyQualifiedName("SetQuestionWeight"),
                applicationId, id, weight);
        }

        public static bool remove_question(Guid applicationId, Guid id, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteQuestion"),
                applicationId, id, currentUserId, DateTime.Now);
        }

        public static bool remove_related_node_questions(Guid applicationId, 
            Guid knowledgeTypeId, Guid nodeId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteRelatedNodeQuestions"),
                applicationId, knowledgeTypeId, nodeId, currentUserId, DateTime.Now);
        }

        public static bool add_answer_option(Guid applicationId, Guid id,
            Guid typeQuestionId, string title, double value, Guid currentUserId, ref string errorMessage)
        {
            return DBConnector.succeed(applicationId, ref errorMessage, GetFullyQualifiedName("AddAnswerOption"),
                applicationId, id, typeQuestionId, title, value, currentUserId, DateTime.Now);
        }

        public static bool modify_answer_option(Guid applicationId, Guid id,
            string title, double value, Guid currentUserId, ref string errorMessage)
        {
            return DBConnector.succeed(applicationId, ref errorMessage, GetFullyQualifiedName("ModifyAnswerOption"),
                applicationId, id, title, value, currentUserId, DateTime.Now);
        }

        public static bool set_answer_options_order(Guid applicationId, List<Guid> ids)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetAnswerOptionsOrder"),
                applicationId, ProviderUtil.list_to_string<Guid>(ids), ',');
        }
        
        public static bool remove_answer_option(Guid applicationId, Guid id, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteAnswerOption"),
                applicationId, id, currentUserId, DateTime.Now);
        }

        public static List<KnowledgeTypeQuestion> get_questions(Guid applicationId, Guid knowledgeTypeId)
        {
            return KWParsers.questions(DBConnector.read(applicationId, GetFullyQualifiedName("GetQuestions"),
                applicationId, knowledgeTypeId));
        }

        public static List<string> search_questions(Guid applicationId, string searchText, int? count = 20)
        {
            return DBConnector.get_string_list(applicationId, GetFullyQualifiedName("SearchQuestions"),
                applicationId, ProviderUtil.get_search_text(searchText), count);
        }

        public static List<AnswerOption> get_answer_options(Guid applicationId, List<Guid> typeQuestionIds)
        {
            return KWParsers.answer_options(DBConnector.read(applicationId, GetFullyQualifiedName("GetAnswerOptions"),
                applicationId, ProviderUtil.list_to_string<Guid>(typeQuestionIds), ','));
        }

        public static List<EvaluationAnswer> get_filled_evaluation_form(Guid applicationId, 
            Guid knowledgeId, Guid userId, int? wfVersionId = null)
        {
            return KWParsers.answers(DBConnector.read(applicationId, GetFullyQualifiedName("GetFilledEvaluationForm"),
                applicationId, knowledgeId, userId, wfVersionId));
        }

        public static List<KnowledgeEvaluation> get_evaluations_done(Guid applicationId, Guid knowledgeId, int? wfVersionId)
        {
            return KWParsers.evaluations(DBConnector.read(applicationId, GetFullyQualifiedName("GetEvaluationsDone"),
                applicationId, knowledgeId, wfVersionId));
        }

        public static bool has_evaluated(Guid applicationId, Guid knowledgeId, Guid userId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("HasEvaluated"),
                applicationId, knowledgeId, userId);
        }

        private static bool _accept_reject_knowledge(Guid applicationId,
            Guid nodeId, Guid currentUserId, bool accept, List<string> textOptions, string description)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AcceptRejectKnowledge"),
                applicationId, nodeId, currentUserId, accept, string.Join(" ~ ", textOptions), description, DateTime.Now);
        }

        public static bool accept_knowledge(Guid applicationId, 
            Guid nodeId, Guid currentUserId, List<string> textOptions, string description)
        {
            return _accept_reject_knowledge(applicationId, nodeId, currentUserId, accept: true, textOptions, description);
        }

        public static bool reject_knowledge(Guid applicationId, 
            Guid nodeId, Guid currentUserId, List<string> textOptions, string description)
        {
            return _accept_reject_knowledge(applicationId, nodeId, currentUserId, accept: false, textOptions, description);
        }

        public static bool send_to_admin(Guid applicationId, Guid nodeId, List<Guid> adminUserIds, 
            Guid currentUserId, string description, ref List<Dashboard> dashboards, ref string message)
        {
            return DBConnector.get_dashboards(applicationId, ref message, ref dashboards, GetFullyQualifiedName("SendToAdmin"),
                applicationId, nodeId, currentUserId, ProviderUtil.list_to_string<Guid>(adminUserIds), ',', description, DateTime.Now) > 0;
        }

        public static bool send_to_admin(Guid applicationId, Guid nodeId, Guid adminUserId, Guid currentUserId, 
            string description, ref List<Dashboard> dashboards, ref string message)
        {
            return send_to_admin(applicationId, nodeId, new List<Guid>() { adminUserId }, 
                currentUserId, description, ref dashboards, ref message);
        }

        public static bool send_back_for_revision(Guid applicationId, Guid nodeId, Guid currentUserId, 
            List<string> textOptions, string description, ref List<Dashboard> dashboards, ref string message)
        {
            return DBConnector.get_dashboards(applicationId, ref message, ref dashboards, GetFullyQualifiedName("SendBackForRevision"),
                applicationId, nodeId, currentUserId, string.Join(" ~ ", textOptions), description, DateTime.Now) > 0;
        }

        public static bool send_to_evaluators(Guid applicationId, Guid nodeId, List<Guid> evaluatorUserIds, 
            Guid currentUserId, string description, ref List<Dashboard> dashboards, ref string message)
        {
            return DBConnector.get_dashboards(applicationId, ref message, ref dashboards, GetFullyQualifiedName("SendToEvaluators"),
                applicationId, nodeId, currentUserId, ProviderUtil.list_to_string<Guid>(evaluatorUserIds), ',', description, DateTime.Now) > 0;
        }

        public static bool new_evaluators(Guid applicationId, Guid nodeId, 
            List<Guid> evaluatorUserIds, ref List<Dashboard> dashboards, ref string message)
        {
            return DBConnector.get_dashboards(applicationId, ref message, ref dashboards, GetFullyQualifiedName("NewEvaluators"),
                applicationId, nodeId, ProviderUtil.list_to_string<Guid>(evaluatorUserIds), ',', DateTime.Now) > 0;
        }

        public static bool send_knowledge_comment(Guid applicationId, Guid nodeId, Guid userId, long? replyToHistoryId,
            List<Guid> adminUserIds, string description, ref List<Dashboard> retDashboards)
        {
            return DBConnector.get_dashboards(applicationId, ref retDashboards, GetFullyQualifiedName("SendKnowledgeComment"),
                applicationId, nodeId, userId, replyToHistoryId, string.Join(",", adminUserIds), ',', description, DateTime.Now) > 0;
        }

        public static bool save_evaluation_form(Guid applicationId, Guid nodeId, Guid userId, List<KeyValuePair<Guid, double>> answers, 
            Guid currentUserId, double score, List<Guid> adminUserIds, List<string> textOptions, string description, 
            ref List<Dashboard> retDashboards, ref string status, ref bool searchabilityActivated)
        {
            if (userId == Guid.Empty) userId = currentUserId;
            if (answers == null) answers = new List<KeyValuePair<Guid, double>>();

            DBCompositeType<GuidFloatTableType> answersParam = new DBCompositeType<GuidFloatTableType>()
                .add(answers.Select(a => new GuidFloatTableType(a.Key, a.Value)).ToList());

            string strTextOptions = textOptions == null || textOptions.Count == 0 ? null : string.Join(" ~ ", textOptions);

            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("SaveEvaluationForm"),
                applicationId, nodeId, userId, currentUserId, answersParam, score, DateTime.Now,
                GuidTableType.getCompositeType(adminUserIds), strTextOptions, description);

            bool succeed = !(searchabilityActivated = false); //--> succeed: true, accepted: false, searchabilityActivated: false
            bool accepted = false;

            retDashboards = KWParsers.save_evaluation_form(results, 
                result: ref succeed, ref accepted, ref searchabilityActivated, ref status);

            return succeed;
        }

        public static bool save_evaluation_form(Guid applicationId, Guid nodeId, Guid userId, List<KeyValuePair<Guid, double>> answers, 
            Guid currentUserId, double score, Guid adminUserId, List<string> textOptions, string description,
            ref List<Dashboard> retDashboards, ref string status, ref bool searchabilityActivated)
        {
            return save_evaluation_form(applicationId, nodeId, userId, answers, currentUserId, score, new List<Guid>() { adminUserId }, 
                textOptions, description, ref retDashboards, ref status, ref searchabilityActivated);
        }

        public static bool remove_evaluator(Guid applicationId, Guid nodeId, Guid userId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RemoveEvaluator"), applicationId, nodeId, userId);
        }

        public static bool refuse_evaluation(Guid applicationId, Guid nodeId, Guid currentUserId, 
            List<Guid> adminUserIds, string description, ref List<Dashboard> retDashboards)
        {
            return DBConnector.get_dashboards(applicationId, ref retDashboards, GetFullyQualifiedName("RefuseEvaluation"),
                applicationId, nodeId, currentUserId, DateTime.Now, ProviderUtil.list_to_string<Guid>(adminUserIds), ',', description) > 0;
        }

        public static bool refuse_evaluation(Guid applicationId, Guid nodeId, Guid currentUserId, 
            Guid adminUserId, string description, ref List<Dashboard> retDashboards)
        {
            return refuse_evaluation(applicationId, nodeId, currentUserId, 
                new List<Guid>() { adminUserId }, description, ref retDashboards);
        }

        public static bool terminate_evaluation(Guid applicationId, Guid nodeId, Guid currentUserId,
            string description, ref bool accepted, ref bool searchabilityActivated)
        {
            RVDataTable table = DBConnector.read(applicationId, GetFullyQualifiedName("TerminateEvaluation"),
                applicationId, nodeId, currentUserId, description, DateTime.Now).get_table();

            bool result = false;
            string status = null;

            KWParsers.workflow_action_results(table, ref result, ref accepted, ref searchabilityActivated, ref status);

            return result;
        }

        public static int? get_last_history_version_id(Guid applicationId, Guid knowledgeId)
        {
            int? ret = DBConnector.get_int(applicationId, GetFullyQualifiedName("GetLastHistoryVersionID"), applicationId, knowledgeId);
            return ret.HasValue && ret == 0 ? null : ret;
        }

        public static bool edit_history_description(Guid applicationId, long id, string description)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("EditHistoryDescription"), applicationId, id, description);
        }

        public static List<KWFHistory> get_history(Guid applicationId, Guid knowledgeId, 
            Guid? userId = null, string action = null, int? wfVersionId = null)
        {
            if (string.IsNullOrEmpty(action)) action = null;

            return KWParsers.history(DBConnector.read(applicationId, GetFullyQualifiedName("GetHistory"),
                applicationId, knowledgeId, userId, action, wfVersionId));
        }

        public static KWFHistory get_history(Guid applicationId, long id)
        {
            return KWParsers.history(DBConnector.read(applicationId, GetFullyQualifiedName("GetHistoryByID"),
                applicationId, id)).FirstOrDefault();
        }

        public static long add_feedback(Guid applicationId, FeedBack info)
        {
            int ftypeId = info.FeedBackType == FeedBackTypes.Financial ? 1 : 2;

            return DBConnector.get_long(applicationId, GetFullyQualifiedName("AddFeedBack"),
                applicationId, info.KnowledgeID, info.User.UserID, ftypeId, info.SendDate, info.Value, info.Description);
        }

        public static bool modify_feedback(Guid applicationId, FeedBack info)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ModifyFeedBack"),
                applicationId, info.FeedBackID, info.Value, info.Description);
        }

        public static bool remove_feedback(Guid applicationId, long feedbackId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ArithmeticDeleteFeedBack"), applicationId, feedbackId);

        }

        public static List<FeedBack> get_knowledge_feedbacks(Guid applicationId, Guid knowledgeId, Guid? userId = null,
            FeedBackTypes? feedbackType = null, DateTime? sendDateLowerThreshold = null, DateTime? sendDateUpperThreshold = null)
        {
            int? feedbackTypeId = null;
            if (feedbackType.HasValue) feedbackTypeId = feedbackType.Value == FeedBackTypes.Financial ? 1 : 2;

            return KWParsers.feedbacks(DBConnector.read(applicationId, GetFullyQualifiedName("GetKnowledgeFeedBacks"),
                applicationId, knowledgeId, userId, feedbackTypeId, sendDateLowerThreshold, sendDateUpperThreshold));
        }

        public static List<FeedBack> get_knowledge_feedbacks(Guid applicationId, List<long> feedbackIds)
        {
            return KWParsers.feedbacks(DBConnector.read(applicationId, GetFullyQualifiedName("GetFeedBacksByIDs"),
                applicationId, ProviderUtil.list_to_string<long>(feedbackIds), ','));
        }

        public static FeedBack get_knowledge_feedback(Guid applicationId, long feedbackId)
        {
            return get_knowledge_feedbacks(applicationId, new List<long>() { feedbackId }).FirstOrDefault();
        }

        public static void get_feedback_status(Guid applicationId, Guid knowledgeId, Guid? userId, 
            ref double totalFinancialFeedbacks, ref double totalTemporalFeedbacks, 
            ref double financialFeedbackStatus, ref double temporalFeedbackStatus)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetFeedBackStatus"),
                applicationId, knowledgeId, userId);

            KWParsers.feedback_status(results, ref totalFinancialFeedbacks, ref totalTemporalFeedbacks,
                ref financialFeedbackStatus, ref temporalFeedbackStatus);
        }

        public static List<NecessaryItem> get_necessary_items(Guid applicationId, Guid nodeTypeIdOrNodeId)
        {
            return KWParsers.necessary_items(DBConnector.read(applicationId, GetFullyQualifiedName("GetNecessaryItems"),
                applicationId, nodeTypeIdOrNodeId));
        }

        public static bool activate_necessary_item(Guid applicationId, Guid knowledgeTypeId, NecessaryItem itm, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ActivateNecessaryItem"),
                applicationId, knowledgeTypeId, itm.ToString(), currentUserId, DateTime.Now);
        }

        public static bool deactive_necessary_item(Guid applicationId, Guid knowledgeTypeId, NecessaryItem itm, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("DeactiveNecessaryItem"),
                applicationId, knowledgeTypeId, itm.ToString(), currentUserId, DateTime.Now);
        }

        public static List<NecessaryItem> check_necessary_items(Guid applicationId, Guid knowledgeId)
        {
            return KWParsers.necessary_items(DBConnector.read(applicationId, GetFullyQualifiedName("CheckNecessaryItems"),
                applicationId, knowledgeId));
        }
    }
}
