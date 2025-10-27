using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaaiVan.Modules.CoreNetwork;
using RaaiVan.Modules.GlobalUtilities;

namespace RaaiVan.Modules.QA
{
    public class QAController
    {
        private static string GetFullyQualifiedName(string name)
        {
            return "[dbo]." + "[QA_" + name + "]"; //'[dbo].' is database owner and 'QA_' is module qualifier
        }

        public static bool add_new_workflow(Guid applicationId, Guid workflowId, string name, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AddNewWorkFlow"),
                applicationId, workflowId, name, currentUserId, DateTime.Now);
        }

        public static bool rename_workflow(Guid applicationId, Guid workflowId, string name, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RenameWorkFlow"),
                applicationId, workflowId, name, currentUserId, DateTime.Now);
        }

        public static bool set_workflow_description(Guid applicationId,
            Guid workflowId, string description, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetWorkFlowDescription"),
                applicationId, workflowId, description, currentUserId, DateTime.Now);
        }

        public static bool set_workflows_order(Guid applicationId, List<Guid> workflowIds)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetWorkFlowsOrder"),
                applicationId, ProviderUtil.list_to_string<Guid>(workflowIds), ',');
        }

        public static bool set_workflow_initial_check_needed(Guid applicationId,
            Guid workflowId, bool value, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetWorkFlowInitialCheckNeeded"),
                applicationId, workflowId, value, currentUserId, DateTime.Now);
        }

        public static bool set_workflow_final_confirmation_needed(Guid applicationId, Guid workflowId, bool value, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetWorkFlowFinalConfirmationNeeded"),
                applicationId, workflowId, value, currentUserId, DateTime.Now);
        }

        public static bool set_workflow_action_deadline(Guid applicationId, Guid workflowId, int value, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetWorkFlowActionDeadline"),
                applicationId, workflowId, value, currentUserId, DateTime.Now);
        }

        public static bool set_workflow_answer_by(Guid applicationId, Guid workflowId, AnswerBy value, Guid currentUserId)
        {
            string strValue = null;
            if (value != AnswerBy.None) strValue = value.ToString();

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetWorkFlowAnswerBy"),
                applicationId, workflowId, strValue, currentUserId, DateTime.Now);
        }

        public static bool set_workflow_publish_after(Guid applicationId, Guid workflowId, PublishAfter value, Guid currentUserId)
        {
            string strValue = null;
            if (value != PublishAfter.None) strValue = value.ToString();

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetWorkFlowPublishAfter"),
                applicationId, workflowId, strValue, currentUserId, DateTime.Now);
        }

        public static bool set_workflow_removable_after_confirmation(Guid applicationId,
            Guid workflowId, bool value, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetWorkFlowRemovableAfterConfirmation"),
                applicationId, workflowId, value, currentUserId, DateTime.Now);
        }

        public static bool set_workflow_node_select_type(Guid applicationId, Guid workflowId, NodeSelectType value, Guid currentUserId)
        {
            string strValue = null;
            if (value != NodeSelectType.None) strValue = value.ToString();

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetWorkFlowNodeSelectType"),
                applicationId, workflowId, strValue, currentUserId, DateTime.Now);
        }

        public static bool set_workflow_disable_comments(Guid applicationId, Guid workflowId, bool? value, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetWorkFlowDisableComments"),
                applicationId, workflowId, value, currentUserId, DateTime.Now);
        }

        public static bool set_workflow_disable_question_likes(Guid applicationId, Guid workflowId, bool? value, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetWorkFlowDisableQuestionLikes"),
                applicationId, workflowId, value, currentUserId, DateTime.Now);
        }

        public static bool set_workflow_disable_answer_likes(Guid applicationId, Guid workflowId, bool? value, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetWorkFlowDisableAnswerLikes"),
                applicationId, workflowId, value, currentUserId, DateTime.Now);
        }

        public static bool set_workflow_disable_comment_likes(Guid applicationId, Guid workflowId, bool? value, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetWorkFlowDisableCommentLikes"),
                applicationId, workflowId, value, currentUserId, DateTime.Now);
        }

        public static bool set_workflow_disable_best_answer(Guid applicationId, Guid workflowId, bool? value, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetWorkFlowDisableBestAnswer"),
                applicationId, workflowId, value, currentUserId, DateTime.Now);
        }

        public static bool remove_workflow(Guid applicationId, Guid workflowId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RemoveWorkFlow"),
                applicationId, workflowId, currentUserId, DateTime.Now);
        }

        public static bool recycle_workflow(Guid applicationId, Guid workflowId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RecycleWorkFlow"),
                applicationId, workflowId, currentUserId, DateTime.Now);
        }

        public static List<QAWorkFlow> get_workflows(Guid applicationId, bool archive = false)
        {
            return QAParsers.workflows(DBConnector.read(applicationId, GetFullyQualifiedName("GetWorkFlows"), applicationId, archive));
        }

        public static QAWorkFlow get_workflow(Guid applicationId, Guid workflowIdOrQuestionIdOrAnswerId)
        {
            return QAParsers.workflows(DBConnector.read(applicationId, GetFullyQualifiedName("GetWorkFlow"),
                applicationId, workflowIdOrQuestionIdOrAnswerId)).FirstOrDefault();
        }

        public static List<Guid> is_workflow(Guid applicationId, List<Guid> ids)
        {
            return DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("IsWorkFlow"),
                applicationId, ProviderUtil.list_to_string<Guid>(ids), ',');
        }

        public static bool is_workflow(Guid applicationId, Guid id)
        {
            return is_workflow(applicationId, new List<Guid>() { id }).Count == 1;
        }

        public static bool add_workflow_admin(Guid applicationId, Guid userId, Guid? workflowId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AddWorkFlowAdmin"),
                applicationId, userId, workflowId, currentUserId, DateTime.Now);
        }

        public static bool remove_workflow_admin(Guid applicationId, Guid userId, Guid? workflowId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RemoveWorkFlowAdmin"),
                applicationId, userId, workflowId, currentUserId, DateTime.Now);
        }

        public static bool is_workflow_admin(Guid applicationId, Guid userId, Guid? workflowIdOrQuestionIdOrAnswerId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("IsWorkFlowAdmin"),
                applicationId, userId, workflowIdOrQuestionIdOrAnswerId);
        }

        public static List<Guid> get_workflow_admin_ids(Guid applicationId, Guid? workflowIdOrQuestionIdOrAnswerId)
        {
            return DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("GetWorkFlowAdminIDs"),
                applicationId, workflowIdOrQuestionIdOrAnswerId);
        }
        
        public static bool set_candidate_relations(Guid applicationId, Guid workflowId,
            List<Guid> nodeTypeIds, List<Guid> nodeIds, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetCandidateRelations"),
                applicationId, workflowId, ProviderUtil.list_to_string<Guid>(nodeTypeIds),
                ProviderUtil.list_to_string<Guid>(nodeIds), ',', currentUserId, DateTime.Now);
        }

        public static List<Node> get_candidate_node_relations(Guid applicationId, Guid workflowIdOrQuestionIdOrAnswerId)
        {
            List<Guid> nodeIds = DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("GetCandidateNodeRelationIDs"),
                applicationId, workflowIdOrQuestionIdOrAnswerId);

            return CNController.get_nodes(applicationId, nodeIds, full: null, currentUserId: null);
        }

        public static List<NodeType> get_candidate_node_type_relations(Guid applicationId, Guid workflowIdOrQuestionIdOrAnswerId)
        {
            List<Guid> nodeTypeIds = DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("GetCandidateNodeTypeRelationIDs"),
                applicationId, workflowIdOrQuestionIdOrAnswerId);

            return CNController.get_node_types(applicationId, nodeTypeIds);
        }

        public static bool create_faq_category(Guid applicationId, Guid categoryId, Guid? parentId, string name, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("CreateFAQCategory"),
                applicationId, categoryId, parentId, name, currentUserId, DateTime.Now);
        }

        public static bool rename_faq_category(Guid applicationId, Guid categoryId, string name, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RenameFAQCategory"),
                applicationId, categoryId, name, currentUserId, DateTime.Now);
        }

        public static bool move_faq_categories(Guid applicationId, List<Guid> categoryIds, Guid? newParentId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("MoveFAQCategories"),
                applicationId, ProviderUtil.list_to_string<Guid>(categoryIds), ',', newParentId, currentUserId, DateTime.Now);
        }

        public static bool set_faq_categories_order(Guid applicationId, List<Guid> categoryIds)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetFAQCategoriesOrder"),
                applicationId, ProviderUtil.list_to_string<Guid>(categoryIds), ',');
        }

        public static bool remove_faq_categories(Guid applicationId, List<Guid> categoryIds, bool? removeHierarchy, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RemoveFAQCategories"),
                applicationId, ProviderUtil.list_to_string<Guid>(categoryIds), ',', removeHierarchy, currentUserId, DateTime.Now);
        }

        public static List<FAQCategory> get_child_faq_categories(Guid applicationId, 
            Guid? parentId, Guid? currentUserId, bool? checkAccess)
        {
            return QAParsers.faq_categories(DBConnector.read(applicationId, GetFullyQualifiedName("GetChildFAQCategories"),
                applicationId, parentId, currentUserId, checkAccess, RaaiVanSettings.DefaultPrivacy(applicationId), DateTime.Now));
        }

        public static List<Guid> is_faq_category(Guid applicationId, List<Guid> ids)
        {
            return DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("IsFAQCategory"),
                applicationId, ProviderUtil.list_to_string<Guid>(ids), ',');
        }

        public static bool is_faq_category(Guid applicationId, Guid id)
        {
            return is_faq_category(applicationId, new List<Guid>() { id }).Count == 1;
        }

        public static bool add_faq_items(Guid applicationId, Guid categoryId, List<Guid> questionIds, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AddFAQItems"),
                applicationId, categoryId, ProviderUtil.list_to_string<Guid>(questionIds), ',', currentUserId, DateTime.Now);
        }

        public static bool add_question_to_faq_categories(Guid applicationId, 
            Guid questionId, List<Guid> categoryIds, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AddQuestionToFAQCategories"),
                applicationId, questionId, ProviderUtil.list_to_string<Guid>(categoryIds), ',', currentUserId, DateTime.Now);
        }

        public static bool remove_faq_item(Guid applicationId, Guid categoryId, Guid questionId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RemoveFAQItem"),
                applicationId, categoryId, questionId, currentUserId, DateTime.Now);
        }

        public static bool set_faq_items_order(Guid applicationId, Guid categoryId, List<Guid> questionIds)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetFAQItemsOrder"),
                applicationId, categoryId, ProviderUtil.list_to_string<Guid>(questionIds), ',');
        }

        public static bool add_question(Guid applicationId, Guid questionId, string title, string description,
            QuestionStatus status, DateTime? publicationDate, List<Guid> relatedNodeIds,
            Guid? workflowId, Guid? adminId, Guid currentUserId, ref List<Dashboard> dashboards)
        {
            string strStatus = null;
            if (status != QuestionStatus.None) strStatus = status.ToString();

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AddQuestion"),
                applicationId, questionId, title, description, strStatus, publicationDate, 
                ProviderUtil.list_to_string<Guid>(relatedNodeIds), ',', workflowId, adminId, currentUserId, DateTime.Now);
        }

        public static bool edit_question_title(Guid applicationId, Guid questionId, string title, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("EditQuestionTitle"),
                applicationId, questionId, title, currentUserId, DateTime.Now);
        }

        public static bool edit_question_description(Guid applicationId, Guid questionId, string description, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("EditQuestionDescription"),
                applicationId, questionId, description, currentUserId, DateTime.Now);
        }

        public static bool is_question(Guid applicationId, Guid id)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("IsQuestion"), applicationId, id);
        }

        public static bool is_answer(Guid applicationId, Guid id)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("IsAnswer"), applicationId, id);
        }

        public static bool confirm_question(Guid applicationId, Guid questionId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("ConfirmQuestion"),
                applicationId, questionId, currentUserId, DateTime.Now);
        }
        
        public static bool set_the_best_answer(Guid applicationId, Guid questionId, Guid answerId, bool publish, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetTheBestAnswer"),
                applicationId, questionId, answerId, publish, currentUserId, DateTime.Now);
        }

        public static bool set_question_status(Guid applicationId, Guid questionId,
            QuestionStatus status, bool publish, Guid currentUserId)
        {
            string strStatus = null;
            if (status != QuestionStatus.None) strStatus = status.ToString();

            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SetQuestionStatus"),
                applicationId, questionId, strStatus, publish, currentUserId, DateTime.Now);
        }

        public static bool remove_question(Guid applicationId, Guid questionId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RemoveQuestion"),
                applicationId, questionId, currentUserId, DateTime.Now);
        }

        public static List<Question> get_questions(Guid applicationId, List<Guid> questionIds, Guid? currentUserId)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetQuestionsByIDs"),
                applicationId, ProviderUtil.list_to_string<Guid>(questionIds), ',', currentUserId);
            return QAParsers.questions(results, full: true);
        }

        public static Question get_question(Guid applicationId, Guid questionId, Guid? currentUserId)
        {
            return get_questions(applicationId, new List<Guid>() { questionId }, currentUserId).FirstOrDefault();
        }

        public static List<Question> get_questions(Guid applicationId, string searchText, bool startWithSearch,
            DateTime? dateFrom, DateTime? dateTo, int? count, double? lowerBoundary, ref long totalCount)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetQuestions"),
                applicationId, ProviderUtil.get_search_text(searchText, startWithSearch), dateFrom, dateTo, count, lowerBoundary);
            return QAParsers.questions(results, full: false, totalCount: ref totalCount);
        }

        public static List<Question> get_questions(Guid applicationId, Guid nodeId, string searchText, bool startWithSearch, 
            DateTime? dateFrom, DateTime? dateTo,  int? count, double? lowerBoundary, ref long totalCount)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetQuestionsRelatedToNode"),
                applicationId, nodeId, ProviderUtil.get_search_text(searchText, startWithSearch), dateFrom, dateTo, count, lowerBoundary);
            return QAParsers.questions(results, full: false, totalCount: ref totalCount);
        }

        public static List<Question> find_related_questions(Guid applicationId, 
            Guid questionId, int? count, double? lowerBoundary, ref long totalCount)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("FindRelatedQuestions"),
                applicationId, questionId, count, lowerBoundary);
            return QAParsers.questions(results, full: false, totalCount: ref totalCount);
        }

        public static List<Question> get_related_questions(Guid applicationId, Guid userId, bool? groups, 
            bool? expertiseDomains, bool? favorites, bool? properties, bool? fromFriends,
            int? count, double? lowerBoundary, ref long totalCount)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetRelatedQuestions"),
                applicationId, userId, groups, expertiseDomains, favorites, properties, fromFriends, count, lowerBoundary);
            return QAParsers.questions(results, full: false, totalCount: ref totalCount);
        }

        public static List<Question> my_favorite_questions(Guid applicationId,
            Guid userId, int? count, double? lowerBoundary, ref long totalCount)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("MyFavoriteQuestions"),
                applicationId, userId, count, lowerBoundary);
            return QAParsers.questions(results, full: false, totalCount: ref totalCount);
        }

        public static List<Question> my_asked_questions(Guid applicationId,
            Guid userId, int? count, double? lowerBoundary, ref long totalCount)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("MyAskedQuestions"),
                applicationId, userId, count, lowerBoundary);
            return QAParsers.questions(results, full: false, totalCount: ref totalCount);
        }

        public static List<Question> questions_asked_of_me(Guid applicationId,
            Guid userId, int? count, double? lowerBoundary, ref long totalCount)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("QuestionsAskedOfMe"),
                applicationId, userId, count, lowerBoundary);
            return QAParsers.questions(results, full: false, totalCount: ref totalCount);
        }

        public static List<Question> get_faq_items(Guid applicationId,
            Guid categoryId, int? count, double? lowerBoundary, ref long totalCount)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("GetFAQItems"),
                applicationId, categoryId, count, lowerBoundary);
            return QAParsers.questions(results, full: false, totalCount: ref totalCount);
        }

        private static List<RelatedNode> _group_questions_by_related_nodes(Guid applicationId, Guid? currentUserId, 
            Guid? questionId, string searchText, bool? checkAccess, int? count, double? lowerBoundary, ref long totalCount)
        {
            DBResultSet resuts = DBConnector.read(applicationId, GetFullyQualifiedName("GroupQuestionsByRelatedNodes"),
                applicationId, currentUserId, questionId, ProviderUtil.get_search_text(searchText),
                RaaiVanSettings.DefaultPrivacy(applicationId), checkAccess, DateTime.Now, count, lowerBoundary);

            return QAParsers.related_nodes(resuts, ref totalCount);
        }

        public static List<RelatedNode> group_questions_by_related_nodes(Guid applicationId, Guid? currentUserId, 
            string searchText,  bool? checkAccess, int? count, double? lowerBoundary, ref long totalCount)
        {
            return _group_questions_by_related_nodes(applicationId, currentUserId, questionId: null, 
                searchText, checkAccess, count, lowerBoundary, ref totalCount);
        }

        public static List<RelatedNode> get_related_nodes(Guid applicationId,
            Guid? currentUserId, Guid? questionId, string searchText, bool? checkAccess,
            int? count, double? lowerBoundary, ref long totalCount)
        {
            return _group_questions_by_related_nodes(applicationId, currentUserId, questionId,
                searchText, checkAccess, count, lowerBoundary, ref totalCount);
        }

        public static List<RelatedNode> find_related_tags(Guid applicationId,
            Guid nodeId, int? count, double? lowerBoundary, ref long totalCount)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("FindRelatedTags"),
                applicationId, nodeId, count, lowerBoundary);

            return QAParsers.related_nodes(results, ref totalCount);
        }

        public static List<RelatedNode> check_nodes(Guid applicationId, List<Guid> nodeIds)
        {
            return QAParsers.related_nodes(DBConnector.read(applicationId, GetFullyQualifiedName("CheckNodes"),
                applicationId, ProviderUtil.list_to_string<Guid>(nodeIds), ','));
        }

        public static List<RelatedNode> search_nodes(Guid applicationId,  string searchText, bool exactSearch,
            bool startWithSearch, bool orderByRank, int? count, long? lowerBoundary, ref long totalCount)
        {
            if (!exactSearch) searchText = ProviderUtil.get_search_text(searchText, startWithSearch);

            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("SearchNodes"),
                applicationId, searchText, exactSearch, orderByRank, count, lowerBoundary);

            return QAParsers.related_nodes(results, ref totalCount);
        }

        public static bool save_related_nodes(Guid applicationId, Guid questionId, List<Guid> nodeIds, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SaveRelatedNodes"),
                applicationId, questionId, ProviderUtil.list_to_string<Guid>(nodeIds), ',', currentUserId, DateTime.Now);
        }

        public static bool add_related_nodes(Guid applicationId, Guid questionId, List<Guid> nodeIds, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("AddRelatedNodes"),
                applicationId, questionId, ProviderUtil.list_to_string<Guid>(nodeIds), ',', currentUserId, DateTime.Now);
        }

        public static bool remove_related_nodes(Guid applicationId, Guid questionId, List<Guid> nodeIds, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RemoveRelatedNodes"),
                applicationId, questionId, ProviderUtil.list_to_string<Guid>(nodeIds), ',', currentUserId, DateTime.Now);
        }

        public static bool is_question_owner(Guid applicationId, Guid questionIdOrAnswerId, Guid userId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("IsQuestionOwner"), 
                applicationId, questionIdOrAnswerId, userId);
        }

        public static bool is_answer_owner(Guid applicationId, Guid answerId, Guid userId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("IsAnswerOwner"), applicationId, answerId, userId);
        }

        public static bool is_comment_owner(Guid applicationId, Guid commentId, Guid userId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("IsCommentOwner"), applicationId, commentId, userId);
        }

        public static bool is_related_user(Guid applicationId, Guid questionId, Guid userId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("IsRelatedUser"), applicationId, questionId, userId);
        }

        public static bool is_related_expert_or_member(Guid applicationId, 
            Guid questionId, Guid userId, bool experts, bool members, bool checkCandidates)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("IsRelatedExpertOrMember"),
                applicationId, questionId, userId, experts, members, checkCandidates);
        }

        public static bool send_answer(Guid applicationId, Guid answerId, Guid questionId, string answerBody, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SendAnswer"),
                applicationId, answerId, questionId, answerBody, currentUserId, DateTime.Now);
        }

        public static bool edit_answer(Guid applicationId, Guid answerId, string answerBody, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("EditAnswer"),
                applicationId, answerId, answerBody, currentUserId, DateTime.Now);
        }

        public static bool remove_answer(Guid applicationId, Guid answerId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RemoveAnswer"),
                applicationId, answerId, currentUserId, DateTime.Now);
        }

        public static List<Answer> get_answers(Guid applicationId, List<Guid> answerIds, Guid? currentUserId)
        {
            return QAParsers.answers(DBConnector.read(applicationId, GetFullyQualifiedName("GetAnswersByIDs"),
                applicationId, ProviderUtil.list_to_string<Guid>(answerIds), ',', currentUserId));
        }

        public static Answer get_answer(Guid applicationId, Guid answerId, Guid? currentUserId)
        {
            return get_answers(applicationId, new List<Guid>() { answerId }, currentUserId).FirstOrDefault();
        }

        public static List<Answer> get_answers(Guid applicationId, Guid questionId, Guid? currentUserId)
        {
            return QAParsers.answers(DBConnector.read(applicationId, GetFullyQualifiedName("GetAnswers"),
                applicationId, questionId, currentUserId));
        }

        public static bool send_comment(Guid applicationId,
            Guid commentId, Guid ownerId, Guid? replyToCommentId, string bodyText, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("SendComment"),
                applicationId, commentId, ownerId, replyToCommentId, bodyText, currentUserId, DateTime.Now);
        }

        public static bool edit_comment(Guid applicationId, Guid commentId, string bodyText, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("EditComment"),
                applicationId, commentId, bodyText, currentUserId, DateTime.Now);
        }

        public static bool remove_comment(Guid applicationId, Guid commentId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RemoveComment"),
                applicationId, commentId, currentUserId, DateTime.Now);
        }

        public static List<Comment> get_comments(Guid applicationId, Guid questionId, Guid? currentUserId)
        {
            return QAParsers.comments(DBConnector.read(applicationId, GetFullyQualifiedName("GetComments"),
                applicationId, questionId, currentUserId));
        }

        public static Guid? get_comment_owner_id(Guid applicationId, Guid commentId)
        {
            return DBConnector.get_guid(applicationId, GetFullyQualifiedName("GetCommentOwnerID"), applicationId, commentId);
        }

        public static bool add_knowledgable_user(Guid applicationId, Guid questionId, Guid userId,
            Guid currentUserId, ref List<Dashboard> dashboards)
        {
            return DBConnector.get_dashboards(applicationId, ref dashboards, GetFullyQualifiedName("AddKnowledgableUser"),
                applicationId, questionId, userId, currentUserId, DateTime.Now) > 0;
        }

        public static bool remove_knowledgable_user(Guid applicationId, Guid questionId, Guid userId, Guid currentUserId)
        {
            return DBConnector.succeed(applicationId, GetFullyQualifiedName("RemoveKnowledgableUser"),
                applicationId, questionId, userId, currentUserId, DateTime.Now);
        }

        public static List<Guid> get_knowledgable_user_ids(Guid applicationId, Guid questionId)
        {
            return DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("GetKnowledgableUserIDs"), applicationId, questionId);
        }

        public static List<Guid> get_related_expert_and_member_ids(Guid applicationId, Guid questionId)
        {
            return DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("GetRelatedExpertAndMemberIDs"), 
                applicationId, questionId);
        }

        public static List<Guid> find_knowledgeable_user_ids(Guid applicationId, Guid questionId, int? count, long? lowerBoundary)
        {
            return DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("FindKnowledgeableUserIDs"),
                applicationId, questionId, count, lowerBoundary);
        }

        public static Guid? get_question_asker_id(Guid applicationId, Guid questionId)
        {
            return DBConnector.get_guid(applicationId, GetFullyQualifiedName("GetQuestionAskerID"), applicationId, questionId);
        }

        public static List<Question> search_questions(Guid applicationId, 
            string searchText, Guid? userId, Guid? minId, int? count = 10)
        {
            DBResultSet results = DBConnector.read(applicationId, GetFullyQualifiedName("SearchQuestions"),
                applicationId, ProviderUtil.get_search_text(searchText), userId, count, minId);

            return QAParsers.questions(results, full: true);
        }

        public static long get_questions_count(Guid applicationId,
            bool published = false, DateTime? creationDateLowerLimit = null, DateTime? creationDateUpperLimit = null)
        {
            return DBConnector.get_long(applicationId, GetFullyQualifiedName("GetQuestionsCount"),
                applicationId, published, creationDateLowerLimit, creationDateUpperLimit);
        }

        public static List<Guid> get_answer_sender_ids(Guid applicationId, Guid questionId)
        {
            return DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("GetAnswerSenderIDs"), applicationId, questionId);
        }

        public static List<Guid> get_existing_question_ids(Guid applicationId, List<Guid> questionIds)
        {
            return DBConnector.get_guid_list(applicationId, GetFullyQualifiedName("GetExistingQuestionIDs"),
                applicationId, ProviderUtil.list_to_string(questionIds), ',');
        }
    }
}
