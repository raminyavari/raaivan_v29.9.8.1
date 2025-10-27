using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Log;
using RaaiVan.Modules.Privacy;
using RaaiVan.Modules.Users;
using RaaiVan.Modules.NotificationCenter;
using RaaiVan.Modules.QA;
using RaaiVan.Modules.CoreNetwork;
using RaaiVan.Modules.Messaging;

namespace RaaiVan.Web.API
{
    /// <summary>
    /// Summary description for QA
    /// </summary>
    public class QAAPI : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        private ParamsContainer paramsContainer = null;

        public void ProcessRequest(HttpContext context)
        {
            paramsContainer = new ParamsContainer(HttpContext.Current, nullTenantResponse: true);
            if (!paramsContainer.ApplicationID.HasValue) return;

            string responseText = string.Empty;
            string command = PublicMethods.parse_string(context.Request.Params["Command"], false);

            switch (command)
            {
                case "AddNewWorkFlow":
                    add_new_workflow(PublicMethods.parse_string(context.Request.Params["Name"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "RenameWorkFlow":
                    rename_workflow(PublicMethods.parse_guid(context.Request.Params["WorkFlowID"]),
                        PublicMethods.parse_string(context.Request.Params["Name"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "SetWorkFlowDescription":
                    set_workflow_description(PublicMethods.parse_guid(context.Request.Params["WorkFlowID"]),
                        PublicMethods.parse_string(context.Request.Params["Description"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "SetWorkFlowsOrder":
                    set_workflows_order(ListMaker.get_guid_items(context.Request.Params["WorkFlowIDs"], '|'), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "SetWorkFlowInitialCheckNeeded":
                    set_workflow_initial_check_needed(PublicMethods.parse_guid(context.Request.Params["WorkFlowID"]),
                        PublicMethods.parse_bool(context.Request.Params["Value"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "SetWorkFlowFinalConfirmationNeeded":
                    set_workflow_final_confirmation_needed(PublicMethods.parse_guid(context.Request.Params["WorkFlowID"]),
                        PublicMethods.parse_bool(context.Request.Params["Value"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "SetWorkFlowActionDeadline":
                    set_workflow_action_deadline(PublicMethods.parse_guid(context.Request.Params["WorkFlowID"]),
                        PublicMethods.parse_int(context.Request.Params["Value"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "SetWorkFlowAnswerBy":
                    {
                        AnswerBy ansBy = AnswerBy.None;
                        if (!Enum.TryParse<AnswerBy>(context.Request.Params["Value"], out ansBy)) ansBy = AnswerBy.None;

                        set_workflow_answer_by(PublicMethods.parse_guid(context.Request.Params["WorkFlowID"]),
                            ansBy, ref responseText);
                        _return_response(ref responseText);
                        break;
                    }
                case "SetWorkFlowPublishAfter":
                    {
                        PublishAfter pubAfter = PublishAfter.None;
                        if (!Enum.TryParse<PublishAfter>(context.Request.Params["Value"], out pubAfter))
                            pubAfter = PublishAfter.None;

                        set_workflow_publish_after(PublicMethods.parse_guid(context.Request.Params["WorkFlowID"]),
                            pubAfter, ref responseText);
                        _return_response(ref responseText);
                        break;
                    }
                case "SetWorkFlowRemovableAfterConfirmation":
                    set_workflow_removable_after_confirmation(PublicMethods.parse_guid(context.Request.Params["WorkFlowID"]),
                        PublicMethods.parse_bool(context.Request.Params["Value"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "SetWorkFlowNodeSelectType":
                    {
                        NodeSelectType nsType = NodeSelectType.None;
                        if (!Enum.TryParse<NodeSelectType>(context.Request.Params["Value"], out nsType))
                            nsType = NodeSelectType.None;

                        set_workflow_node_select_type(PublicMethods.parse_guid(context.Request.Params["WorkFlowID"]),
                            nsType, ref responseText);
                        _return_response(ref responseText);
                        break;
                    }
                case "SetWorkFlowDisableComments":
                    set_workflow_disable_comments(PublicMethods.parse_guid(context.Request.Params["WorkFlowID"]),
                        PublicMethods.parse_bool(context.Request.Params["Value"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "SetWorkFlowDisableQuestionLikes":
                    set_workflow_disable_question_likes(PublicMethods.parse_guid(context.Request.Params["WorkFlowID"]),
                        PublicMethods.parse_bool(context.Request.Params["Value"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "SetWorkFlowDisableAnswerLikes":
                    set_workflow_disable_answer_likes(PublicMethods.parse_guid(context.Request.Params["WorkFlowID"]),
                        PublicMethods.parse_bool(context.Request.Params["Value"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "SetWorkFlowDisableCommentLikes":
                    set_workflow_disable_comment_likes(PublicMethods.parse_guid(context.Request.Params["WorkFlowID"]),
                        PublicMethods.parse_bool(context.Request.Params["Value"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "SetWorkFlowDisableBestAnswer":
                    set_workflow_disable_best_answer(PublicMethods.parse_guid(context.Request.Params["WorkFlowID"]),
                        PublicMethods.parse_bool(context.Request.Params["Value"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "RemoveWorkFlow":
                    remove_workflow(PublicMethods.parse_guid(context.Request.Params["WorkFlowID"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "RecycleWorkFlow":
                    recycle_workflow(PublicMethods.parse_guid(context.Request.Params["WorkFlowID"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "GetWorkFlows":
                    get_workflows(PublicMethods.parse_bool(context.Request.Params["Archive"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "AddWorkFlowAdmin":
                    add_workflow_admin(PublicMethods.parse_guid(context.Request.Params["UserID"]),
                        PublicMethods.parse_guid(context.Request.Params["WorkFlowID"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "RemoveWorkFlowAdmin":
                    remove_workflow_admin(PublicMethods.parse_guid(context.Request.Params["UserID"]),
                        PublicMethods.parse_guid(context.Request.Params["WorkFlowID"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "GetWorkFlowAdmins":
                    get_workflow_admins(PublicMethods.parse_guid(context.Request.Params["WorkFlowID"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "SetCandidateRelations":
                    set_candidate_relations(PublicMethods.parse_guid(context.Request.Params["WorkFlowID"]), 
                        ListMaker.get_guid_items(context.Request.Params["NodeTypeIDs"], '|'),
                        ListMaker.get_guid_items(context.Request.Params["NodeIDs"], '|'), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetCandidateRelations":
                    get_candidate_relations(PublicMethods.parse_guid(context.Request.Params["ID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "CreateFAQCategory":
                    create_faq_category(PublicMethods.parse_guid(context.Request.Params["ParentID"]),
                        PublicMethods.parse_string(context.Request.Params["Name"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "RenameFAQCategory":
                    rename_faq_category(PublicMethods.parse_guid(context.Request.Params["CategoryID"]),
                        PublicMethods.parse_string(context.Request.Params["Name"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "MoveFAQCategories":
                    move_faq_categories(ListMaker.get_guid_items(context.Request.Params["CategoryIDs"], '|'),
                        PublicMethods.parse_guid(context.Request.Params["ParentID"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "SetFAQCategoriesOrder":
                    set_faq_categories_order(ListMaker.get_guid_items(context.Request.Params["CategoryIDs"], '|'), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "RemoveFAQCategories":
                    remove_faq_categories(ListMaker.get_guid_items(context.Request.Params["CategoryIDs"], '|'),
                        PublicMethods.parse_bool(context.Request.Params["RemoveHierarchy"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "GetChildFAQCategories":
                    get_child_faq_categories(PublicMethods.parse_guid(context.Request.Params["CategoryID"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "AddFAQItems":
                    add_faq_items(PublicMethods.parse_guid(context.Request.Params["CategoryID"]),
                        ListMaker.get_guid_items(context.Request.Params["QuestionIDs"], '|'), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "AddQuestionToFAQCategories":
                    add_question_to_faq_categories(PublicMethods.parse_guid(context.Request.Params["QuestionID"]),
                        ListMaker.get_guid_items(context.Request.Params["CategoryIDs"], '|'), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "RemoveFAQItem":
                    remove_faq_item(PublicMethods.parse_guid(context.Request.Params["CategoryID"]),
                        PublicMethods.parse_guid(context.Request.Params["QuestionID"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "SetFAQItemsOrder":
                    set_faq_items_order(PublicMethods.parse_guid(context.Request.Params["CategoryID"]),
                        ListMaker.get_guid_items(context.Request.Params["QuestionIDs"], '|'), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "AddQuestion":
                    add_question(PublicMethods.parse_string(context.Request.Params["Title"]),
                        PublicMethods.parse_string(context.Request.Params["Description"]),
                        ListMaker.get_guid_items(context.Request.Params["NodeIDs"], '|'), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "EditQuestionTitle":
                    edit_question_title(PublicMethods.parse_guid(context.Request.Params["QuestionID"]),
                        PublicMethods.parse_string(context.Request.Params["Title"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "EditQuestionDescription":
                    edit_question_description(PublicMethods.parse_guid(context.Request.Params["QuestionID"]),
                        PublicMethods.parse_string(context.Request.Params["Description"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "InitialConfirmQuestion":
                case "ConfirmQuestion":
                    {
                        QuestionStatus status = command == "InitialConfirmQuestion" ? 
                            QuestionStatus.Registered : QuestionStatus.Accepted;

                        set_question_status(PublicMethods.parse_guid(context.Request.Params["QuestionID"]),
                            status, ref responseText);
                        _return_response(ref responseText);
                        break;
                    }
                case "SetTheBestAnswer":
                    set_the_best_answer(PublicMethods.parse_guid(context.Request.Params["QuestionID"]),
                        PublicMethods.parse_guid(context.Request.Params["AnswerID"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "RemoveQuestion":
                    remove_question(PublicMethods.parse_guid(context.Request.Params["QuestionID"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "GetQuestion":
                    get_question(PublicMethods.parse_guid(context.Request.Params["QuestionID"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "GetQuestions":
                    get_questions(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                        PublicMethods.parse_string(context.Request.Params["SearchText"]),
                        PublicMethods.parse_bool(context.Request.Params["StartWithSearch"]),
                        PublicMethods.parse_date(context.Request.Params["DateFrom"]),
                        PublicMethods.parse_date(context.Request.Params["DateTo"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_double(context.Request.Params["LowerBoundary"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "FindRelatedQuestions":
                    find_related_questions(PublicMethods.parse_guid(context.Request.Params["QuestionID"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_double(context.Request.Params["LowerBoundary"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "GetRelatedQuestions":
                    get_related_questions(PublicMethods.parse_bool(context.Request.Params["Groups"]),
                        PublicMethods.parse_bool(context.Request.Params["ExpertiseDomains"]),
                        PublicMethods.parse_bool(context.Request.Params["Favorites"]),
                        PublicMethods.parse_bool(context.Request.Params["Properties"]),
                        PublicMethods.parse_bool(context.Request.Params["FromFriends"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_double(context.Request.Params["LowerBoundary"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "MyFavoriteQuestions":
                    my_favorite_questions(PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_double(context.Request.Params["LowerBoundary"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "MyAskedQuestions":
                    my_asked_questions(PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_double(context.Request.Params["LowerBoundary"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "QuestionsAskedOfMe":
                    questions_asked_of_me(PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_double(context.Request.Params["LowerBoundary"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "GetFAQItems":
                    get_faq_items(PublicMethods.parse_guid(context.Request.Params["CategoryID"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_double(context.Request.Params["LowerBoundary"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "GroupQuestionsByRelatedNodes":
                    group_questions_by_related_nodes(PublicMethods.parse_string(context.Request.Params["SearchText"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_double(context.Request.Params["LowerBoundary"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "GetRelatedTags":
                    get_related_tags(PublicMethods.parse_guid(context.Request.Params["QuestionID"]),
                        PublicMethods.parse_string(context.Request.Params["SearchText"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_double(context.Request.Params["LowerBoundary"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "FindRelatedTags":
                    find_related_tags(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_double(context.Request.Params["LowerBoundary"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "AddQuestionTag":
                    add_question_tag(PublicMethods.parse_guid(context.Request.Params["QuestionID"]),
                        PublicMethods.parse_string(context.Request.Params["Tag"]),
                        PublicMethods.parse_bool(context.Request.Params["IsNewQuestion"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "SaveRelatedNodes":
                case "AddRelatedNodes":
                    save_related_nodes(PublicMethods.parse_guid(context.Request.Params["QuestionID"]),
                        ListMaker.get_guid_items(context.Request.Params["NodeIDs"], '|'), 
                        command == "AddRelatedNodes", ref responseText);
                    _return_response(ref responseText);
                    break;
                case "RemoveRelatedNodes":
                    remove_related_nodes(PublicMethods.parse_guid(context.Request.Params["QuestionID"]),
                        ListMaker.get_guid_items(context.Request.Params["NodeIDs"], '|'), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "CheckNodes":
                    check_nodes(ListMaker.get_guid_items(context.Request.Params["NodeIDs"], '|'), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "SearchNodes":
                    search_nodes(PublicMethods.parse_string(context.Request.Params["SearchText"]),
                        PublicMethods.parse_bool(context.Request.Params["StartWithSearch"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]),
                        PublicMethods.parse_long(context.Request.Params["LowerBoundary"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "SendAnswer":
                    send_answer(PublicMethods.parse_guid(context.Request.Params["QuestionID"]),
                        PublicMethods.parse_string(context.Request.Params["AnswerBody"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "EditAnswer":
                    edit_answer(PublicMethods.parse_guid(context.Request.Params["AnswerID"]),
                        PublicMethods.parse_string(context.Request.Params["AnswerBody"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "RemoveAnswer":
                    remove_answer(PublicMethods.parse_guid(context.Request.Params["AnswerID"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "GetAnswers":
                    get_answers(PublicMethods.parse_guid(context.Request.Params["QuestionID"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "SendComment":
                    send_comment(PublicMethods.parse_guid(context.Request.Params["OwnerID"]),
                        PublicMethods.parse_guid(context.Request.Params["ReplyToCommentID"]),
                        PublicMethods.parse_string(context.Request.Params["BodyText"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "EditComment":
                    edit_comment(PublicMethods.parse_guid(context.Request.Params["CommentID"]),
                        PublicMethods.parse_string(context.Request.Params["BodyText"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "RemoveComment":
                    remove_comment(PublicMethods.parse_guid(context.Request.Params["CommentID"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "AddKnowledgableUser":
                    add_knowledgable_user(PublicMethods.parse_guid(context.Request.Params["QuestionID"]),
                        PublicMethods.parse_guid(context.Request.Params["UserID"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "RemoveKnowledgableUser":
                    remove_knowledgable_user(PublicMethods.parse_guid(context.Request.Params["QuestionID"]),
                        PublicMethods.parse_guid(context.Request.Params["UserID"]), ref responseText);
                    _return_response(ref responseText);
                    break;
                case "GetKnowledgableUsers":
                    get_knowledgable_users(PublicMethods.parse_guid(context.Request.Params["QuestionID"]),
                        PublicMethods.parse_bool(context.Request.Params["Suggestions"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]), ref responseText);
                    _return_response(ref responseText);
                    break;
            }

            paramsContainer.return_response(PublicConsts.BadRequestResponse);
        }

        protected void _return_response(ref string responseText)
        {
            paramsContainer.return_response(ref responseText);
        }

        protected void _save_error_log(Modules.Log.Action action, List<Guid> subjectIds,
            Guid? secondSubjectId = null, string info = null)
        {
            try
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = action,
                    SubjectIDs = subjectIds,
                    SecondSubjectID = secondSubjectId,
                    Info = info,
                    ModuleIdentifier = ModuleIdentifier.DCT
                });
            }
            catch { }
        }

        protected void _save_error_log(Modules.Log.Action action, Guid? subjectId,
            Guid? secondSubjectId = null, string info = null)
        {
            if (!subjectId.HasValue) return;
            _save_error_log(action, new List<Guid>() { subjectId.Value }, secondSubjectId, info);
        }

        protected void add_new_workflow(string name, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(name) && name.Length > 250)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(name))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) || 
                !AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.AddQAWorkFlow_PermissionFailed, Guid.Empty);
                return;
            }

            Guid workflowId = Guid.NewGuid();

            bool result = QAController.add_new_workflow(paramsContainer.Tenant.Id,
                workflowId, name, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"" +
                ",\"WorkFlowID\":\"" + workflowId.ToString() + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.AddQAWorkFlow,
                    SubjectID = workflowId,
                    Info = "{\"Name\":\"" + Base64.encode(name) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void rename_workflow(Guid? workflowId, string name, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(name) && name.Length > 250)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(name))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) || 
                !AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.RenameQAWorkFlow_PermissionFailed, workflowId);
                return;
            }
            
            bool result = !workflowId.HasValue ? false : QAController.rename_workflow(paramsContainer.Tenant.Id,
                workflowId.Value, name, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RenameQAWorkFlow,
                    SubjectID = workflowId,
                    Info = "{\"Name\":\"" + Base64.encode(name) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void set_workflow_description(Guid? workflowId, string description, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;
            
            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) ||
                !AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.SetQAWorkFlowDescription_PermissionFailed, workflowId);
                return;
            }

            bool result = !workflowId.HasValue ? false : QAController.set_workflow_description(
                paramsContainer.Tenant.Id, workflowId.Value, description, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetQAWorkFlowDescription,
                    SubjectID = workflowId,
                    Info = "{\"Description\":\"" + Base64.encode(description) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void set_workflows_order(List<Guid> workflowIds, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) ||
                !AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.SetQAWorkFlowsOrder_PermissionFailed, Guid.Empty);
                return;
            }

            bool result = QAController.set_workflows_order(paramsContainer.Tenant.Id, workflowIds);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";
        }

        protected void set_workflow_initial_check_needed(Guid? workflowId, bool? value, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) ||
                !AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.SetQAWorkFlowInitialCheckNeeded_PermissionFailed, workflowId);
                return;
            }

            bool result = !workflowId.HasValue ? false : QAController.set_workflow_initial_check_needed(
                paramsContainer.Tenant.Id, workflowId.Value, value.HasValue && value.Value, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetQAWorkFlowInitialCheckNeeded,
                    SubjectID = workflowId,
                    Info = "{\"Value\":" + (value.HasValue && value.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void set_workflow_final_confirmation_needed(Guid? workflowId, bool? value, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) ||
                !AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.SetQAWorkFlowFinalConfirmationNeeded_PermissionFailed, workflowId);
                return;
            }

            bool result = !workflowId.HasValue ? false : QAController.set_workflow_final_confirmation_needed(
                paramsContainer.Tenant.Id, workflowId.Value, value.HasValue && value.Value, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetQAWorkFlowFinalConfirmationNeeded,
                    SubjectID = workflowId,
                    Info = "{\"Value\":" + (value.HasValue && value.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void set_workflow_action_deadline(Guid? workflowId, int? value, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) ||
                !AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.SetQAWorkFlowActionDeadline_PermissionFailed, workflowId);
                return;
            }

            if (!value.HasValue || value.Value < 0) value = 0;

            bool result = workflowId.HasValue && QAController.set_workflow_action_deadline(
                paramsContainer.Tenant.Id, workflowId.Value, value.Value, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetQAWorkFlowActionDeadline,
                    SubjectID = workflowId,
                    Info = "{\"Value\":" + value.Value.ToString() + "}",
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void set_workflow_answer_by(Guid? workflowId, AnswerBy value, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) ||
                !AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.SetQAWorkFlowAnswerBy_PermissionFailed, workflowId);
                return;
            }

            bool result = !workflowId.HasValue ? false : QAController.set_workflow_answer_by(
                paramsContainer.Tenant.Id, workflowId.Value, value, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetQAWorkFlowAnswerBy,
                    SubjectID = workflowId,
                    Info = "{\"Value\":\"" + value.ToString() + "\"}",
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void set_workflow_publish_after(Guid? workflowId, PublishAfter value, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) ||
                !AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.SetQAWorkFlowPublishAfter_PermissionFailed, workflowId);
                return;
            }

            bool result = !workflowId.HasValue ? false : QAController.set_workflow_publish_after(
                paramsContainer.Tenant.Id, workflowId.Value, value, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetQAWorkFlowPublishAfter,
                    SubjectID = workflowId,
                    Info = "{\"Value\":\"" + value.ToString() + "\"}",
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void set_workflow_removable_after_confirmation(Guid? workflowId, bool? value, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) ||
                !AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.SetQAWorkFlowRemovableAfterConfirmation_PermissionFailed, workflowId);
                return;
            }

            bool result = !workflowId.HasValue ? false : QAController.set_workflow_removable_after_confirmation(
                paramsContainer.Tenant.Id, workflowId.Value, value.HasValue && value.Value, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetQAWorkFlowRemovableAfterConfirmation,
                    SubjectID = workflowId,
                    Info = "{\"Value\":" + (value.HasValue && value.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void set_workflow_node_select_type(Guid? workflowId, NodeSelectType value, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) ||
                !AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.SetQAWorkFlowNodeSelectType_PermissionFailed, workflowId);
                return;
            }

            bool result = !workflowId.HasValue ? false : QAController.set_workflow_node_select_type(
                paramsContainer.Tenant.Id, workflowId.Value, value, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetQAWorkFlowNodeSelectType,
                    SubjectID = workflowId,
                    Info = "{\"Value\":\"" + value.ToString() + "\"}",
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void set_workflow_disable_comments(Guid? workflowId, bool? value, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) ||
                !AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.SetQAWorkFlowDisableComments_PermissionFailed, workflowId);
                return;
            }

            bool result = workflowId.HasValue && QAController.set_workflow_disable_comments(
                paramsContainer.Tenant.Id, workflowId.Value, value, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetQAWorkFlowDisableComments,
                    SubjectID = workflowId,
                    Info = "{\"Value\":" + (value.HasValue && value.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void set_workflow_disable_question_likes(Guid? workflowId, bool? value, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) ||
                !AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.SetQAWorkFlowDisableQuestionLikes_PermissionFailed, workflowId);
                return;
            }

            bool result = workflowId.HasValue && QAController.set_workflow_disable_question_likes(
                paramsContainer.Tenant.Id, workflowId.Value, value, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetQAWorkFlowDisableQuestionLikes,
                    SubjectID = workflowId,
                    Info = "{\"Value\":" + (value.HasValue && value.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void set_workflow_disable_answer_likes(Guid? workflowId, bool? value, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) ||
                !AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.SetQAWorkFlowDisableAnswerLikes_PermissionFailed, workflowId);
                return;
            }

            bool result = workflowId.HasValue && QAController.set_workflow_disable_answer_likes(
                paramsContainer.Tenant.Id, workflowId.Value, value, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetQAWorkFlowDisableAnswerLikes,
                    SubjectID = workflowId,
                    Info = "{\"Value\":" + (value.HasValue && value.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void set_workflow_disable_comment_likes(Guid? workflowId, bool? value, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) ||
                !AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.SetQAWorkFlowDisableCommentLikes_PermissionFailed, workflowId);
                return;
            }

            bool result = workflowId.HasValue && QAController.set_workflow_disable_comment_likes(
                paramsContainer.Tenant.Id, workflowId.Value, value, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetQAWorkFlowDisableCommentLikes,
                    SubjectID = workflowId,
                    Info = "{\"Value\":" + (value.HasValue && value.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void set_workflow_disable_best_answer(Guid? workflowId, bool? value, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) ||
                !AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.SetQAWorkFlowDisableBestAnswer_PermissionFailed, workflowId);
                return;
            }

            bool result = workflowId.HasValue && QAController.set_workflow_disable_best_answer(
                paramsContainer.Tenant.Id, workflowId.Value, value, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetQAWorkFlowDisableBestAnswer,
                    SubjectID = workflowId,
                    Info = "{\"Value\":" + (value.HasValue && value.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void remove_workflow(Guid? workflowId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) ||
                !AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.RemoveQAWorkFlow_PermissionFailed, workflowId);
                return;
            }

            bool result = workflowId.HasValue && QAController.remove_workflow(
                paramsContainer.Tenant.Id, workflowId.Value, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RemoveQAWorkFlow,
                    SubjectID = workflowId,
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void recycle_workflow(Guid? workflowId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) ||
                !AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.RecycleQAWorkFlow_PermissionFailed, workflowId);
                return;
            }

            bool result = workflowId.HasValue && QAController.recycle_workflow(
                paramsContainer.Tenant.Id, workflowId.Value, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RecycleQAWorkFlow,
                    SubjectID = workflowId,
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void get_workflows(bool? archive, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) ||
                !AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            List<QAWorkFlow> workflows = 
                QAController.get_workflows(paramsContainer.Tenant.Id, archive.HasValue && archive.Value);

            responseText = "{\"WorkFlows\":[" + ProviderUtil.list_to_string<string>(workflows.Select(
                u => "{\"WorkFlowID\":\"" + u.WorkFlowID.ToString() + "\"" +
                ",\"Name\":\"" + Base64.encode(u.Name) + "\"" +
                ",\"Description\":\"" + Base64.encode(u.Description) + "\"" +
                ",\"InitialCheckNeeded\":" +
                    (u.InitialCheckNeeded.HasValue && u.InitialCheckNeeded.Value).ToString().ToLower() +
                ",\"FinalConfirmationNeeded\":" +
                    (u.FinalConfirmationNeeded.HasValue && u.FinalConfirmationNeeded.Value).ToString().ToLower() +
                ",\"ActionDeadline\":" +
                    (!u.ActionDeadline.HasValue || u.ActionDeadline.Value < 0 ? 0 : u.ActionDeadline.Value).ToString() +
                ",\"RemovableAfterConfirmation\":" +
                    (u.RemovableAfterConfirmation.HasValue && u.RemovableAfterConfirmation.Value).ToString().ToLower() +
                ",\"AnswerBy\":\"" + (u.AnswerBy == AnswerBy.None ? string.Empty : u.AnswerBy.ToString()) + "\"" +
                ",\"PublishAfter\":\"" + 
                    (u.PublishAfter == PublishAfter.None ? string.Empty : u.PublishAfter.ToString()) + "\"" +
                ",\"NodeSelectType\":\"" + 
                    (u.NodeSelectType == NodeSelectType.None ? string.Empty : u.NodeSelectType.ToString()) + "\"" +
                ",\"DisableComments\":" +
                    (u.DisableComments.HasValue && u.DisableComments.Value).ToString().ToLower() +
                ",\"DisableQuestionLikes\":" +
                    (u.DisableQuestionLikes.HasValue && u.DisableQuestionLikes.Value).ToString().ToLower() +
                ",\"DisableAnswerLikes\":" +
                    (u.DisableAnswerLikes.HasValue && u.DisableAnswerLikes.Value).ToString().ToLower() +
                ",\"DisableCommentLikes\":" +
                    (u.DisableCommentLikes.HasValue && u.DisableCommentLikes.Value).ToString().ToLower() +
                "}").ToList()) + "]}";
        }

        protected void add_workflow_admin(Guid? userId, Guid? workflowId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) ||
                !AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.AddQAWorkFlowAdmin_PermissionFailed, userId, workflowId);
                return;
            }

            bool result = !userId.HasValue ? false : QAController.add_workflow_admin(
                paramsContainer.Tenant.Id, userId.Value, workflowId, paramsContainer.CurrentUserID.Value);

            User user = !result ? null : UsersController.get_user(paramsContainer.Tenant.Id, userId.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"" +
                ",\"User\":" + (user == null ? "{}" : user.toJson(paramsContainer.Tenant.Id, true)) + "}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.AddQAWorkFlowAdmin,
                    SubjectID = userId,
                    Info = "{\"WorkFlowID\":\"" + (workflowId.HasValue ? workflowId.ToString() : string.Empty) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void remove_workflow_admin(Guid? userId, Guid? workflowId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) ||
                !AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.RemoveQAWorkFlowAdmin_PermissionFailed, userId, workflowId);
                return;
            }

            bool result = !userId.HasValue ? false : QAController.remove_workflow_admin(
                paramsContainer.Tenant.Id, userId.Value, workflowId, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RemoveQAWorkFlowAdmin,
                    SubjectID = userId,
                    Info = "{\"WorkFlowID\":\"" + (workflowId.HasValue ? workflowId.ToString() : string.Empty) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void get_workflow_admins(Guid? workflowId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) ||
                !AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }
            
            List<User> admins = UsersController.get_users(paramsContainer.Tenant.Id,
                QAController.get_workflow_admin_ids(paramsContainer.Tenant.Id, workflowId));

            responseText = "{\"Admins\":[" + string.Join(",", 
                admins.Select(u => u.toJson(paramsContainer.Tenant.Id, true))) + "]}";
        }

        protected void set_candidate_relations(Guid? workflowId, 
            List<Guid> nodeTypeIds, List<Guid> nodeIds, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) ||
                !AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.SetQAWorkFlowCandidateRelations_PermissionFailed, workflowId);
                return;
            }

            bool result = workflowId.HasValue && QAController.set_candidate_relations(paramsContainer.Tenant.Id,
                workflowId.Value, nodeTypeIds, nodeIds, paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetQAWorkFlowCandidateRelations,
                    SubjectID = workflowId,
                    Info = "{\"NodeTypeIDs\":[" +
                        ProviderUtil.list_to_string<string>(nodeTypeIds.Select(u => "\"" + u.ToString() + "\"").ToList(), ',') +
                        "],\"NodeIDs\":[" +
                        ProviderUtil.list_to_string<string>(nodeIds.Select(u => "\"" + u.ToString() + "\"").ToList(), ',') + "]}",
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        protected void get_candidate_relations(Guid? id, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            List<NodeType> candidateNodeTypes = 
                !Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) || !id.HasValue ? new List<NodeType>() :
                QAController.get_candidate_node_type_relations(paramsContainer.Tenant.Id, id.Value);
            responseText = "{\"NodeTypes\":[";

            bool isFirst = true;
            foreach (NodeType nt in candidateNodeTypes)
            {
                responseText += (isFirst ? string.Empty : ",") +
                    "{\"NodeTypeID\":\"" + nt.NodeTypeID.ToString() + "\",\"TypeName\":\"" + Base64.encode(nt.Name) + "\"}";
                isFirst = false;
            }

            responseText += "],\"Nodes\":[";

            List<Node> candidateNodes =
                !Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) || !id.HasValue ? new List<Node>() :
                QAController.get_candidate_node_relations(paramsContainer.Tenant.Id, id.Value);

            isFirst = true;
            foreach (Node nd in candidateNodes)
            {
                responseText += (isFirst ? string.Empty : ",") +
                    "{\"NodeID\":\"" + nd.NodeID.ToString() + "\",\"Name\":\"" + Base64.encode(nd.Name) +
                        "\",\"NodeTypeID\":\"" + nd.NodeTypeID.ToString() + "\",\"NodeType\":\"" + Base64.encode(nd.NodeType) +
                    "\"}";
                isFirst = false;
            }

            responseText += "]}";
        }

        protected void create_faq_category(Guid? parentId, string name, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(name) && name.Length > 250)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(name))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) ||
                !AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.CreateFAQCategory_PermissionFailed, Guid.Empty);
                return;
            }

            Guid categoryId = Guid.NewGuid();

            bool result = QAController.create_faq_category(paramsContainer.Tenant.Id, 
                categoryId, parentId, name, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"" +
                ",\"CategoryID\":\"" + categoryId.ToString() + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.CreateFAQCategory,
                    SubjectID = categoryId,
                    Info = "{\"ParentID\":\"" + (parentId.HasValue ? parentId.ToString() : string.Empty) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void rename_faq_category(Guid? categoryId, string name, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(name) && name.Length > 250)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(name))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) ||
                !AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.RenameFAQCategory_PermissionFailed, categoryId);
                return;
            }
            
            bool result = !categoryId.HasValue ? false : QAController.rename_faq_category(paramsContainer.Tenant.Id,
                categoryId.Value, name, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RenameFAQCategory,
                    SubjectID = categoryId,
                    Info = "{\"Name\":\"" + Base64.encode(name) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void move_faq_categories(List<Guid> categoryIds, Guid? parentId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) ||
                !AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.MoveFAQCategories_PermissionFailed, categoryIds);
                return;
            }

            bool result = QAController.move_faq_categories(paramsContainer.Tenant.Id,
                categoryIds, parentId, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.MoveFAQCategories,
                    SubjectIDs = categoryIds,
                    Info = "{\"ParentID\":\"" + (parentId.HasValue ? parentId.ToString() : string.Empty) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void set_faq_categories_order(List<Guid> categoryIds, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) ||
                !AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.SetFAQCategoriesOrder_PermissionFailed, Guid.Empty);
                return;
            }

            bool result = QAController.set_faq_categories_order(paramsContainer.Tenant.Id, categoryIds);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";
        }

        protected void remove_faq_categories(List<Guid> categoryIds, bool? removeHierarchy, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) ||
                !AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.RemoveFAQCategories_PermissionFailed, categoryIds);
                return;
            }

            bool result = QAController.remove_faq_categories(paramsContainer.Tenant.Id,
                categoryIds, removeHierarchy, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RemoveFAQCategories,
                    SubjectIDs = categoryIds,
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void get_child_faq_categories(Guid? categoryId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;

            bool checkAccess = !AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID);

            List<FAQCategory> categories = !Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) ? new List<FAQCategory>() :
                QAController.get_child_faq_categories(paramsContainer.Tenant.Id, 
                categoryId, paramsContainer.CurrentUserID, checkAccess);

            responseText = "{\"Categories\":[" +
                ProviderUtil.list_to_string<string>(categories.Select(
                    u => "{\"CategoryID\":\"" + u.CategoryID.ToString() + "\"" +
                        ",\"Name\":\"" + Base64.encode(u.Name) + "\"" +
                        ",\"HasChild\":" + (u.HasChild.HasValue && u.HasChild.Value).ToString().ToLower() +
                        "}").ToList()) +
                "]}";
        }

        protected void add_faq_items(Guid? categoryId, List<Guid> questionIds, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) ||
                (!AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID) && 
                !QAController.is_workflow_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, null)))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.AddFAQItems_PermissionFailed, questionIds, categoryId);
                return;
            }
            
            bool result = !categoryId.HasValue || questionIds.Count == 0 ? false : 
                QAController.add_faq_items(paramsContainer.Tenant.Id,
                categoryId.Value, questionIds, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.AddFAQItems,
                    SubjectIDs = questionIds,
                    SecondSubjectID = categoryId,
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void add_question_to_faq_categories(Guid? questionId, List<Guid> categoryIds, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) ||
                (!AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID) &&
                !QAController.is_workflow_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, null)))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.AddQuestionToFAQCategories_PermissionFailed, categoryIds, questionId);
                return;
            }
            
            bool result = !questionId.HasValue || categoryIds.Count == 0 ? false :
                QAController.add_question_to_faq_categories(paramsContainer.Tenant.Id,
                questionId.Value, categoryIds, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.AddQuestionToFAQCategories_PermissionFailed,
                    SubjectIDs = categoryIds,
                    SecondSubjectID = questionId,
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void remove_faq_item(Guid? categoryId, Guid? questionId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) ||
                (!AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID) &&
                !QAController.is_workflow_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, null)))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.RemoveFAQItem_PermissionFailed, categoryId, questionId);
                return;
            }

            bool result = !categoryId.HasValue || !questionId.HasValue ? false :
                QAController.remove_faq_item(paramsContainer.Tenant.Id,
                categoryId.Value, questionId.Value, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RemoveFAQItem,
                    SubjectID = categoryId,
                    SecondSubjectID = questionId,
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void set_faq_items_order(Guid? categoryId, List<Guid> questionIds, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!Modules.RaaiVanConfig.Modules.QAAdmin(paramsContainer.Tenant.Id) ||
                !AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.SetFAQItemsOrder_PermissionFailed, Guid.Empty);
                return;
            }

            bool result = !categoryId.HasValue ? false : QAController.set_faq_items_order(paramsContainer.Tenant.Id,
                categoryId.Value, questionIds);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";
        }

        protected void add_question(string title, string description, 
            List<Guid> nodeIds, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(title) && title.Length > 250)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(title))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            Guid? workflowId = null;
            QuestionStatus status = QuestionStatus.None;
            DateTime? publicationDate = null;
            
            List<QAWorkFlow> workflows = QAController.get_workflows(paramsContainer.Tenant.Id);

            List<Guid> ids = PrivacyController.check_access(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID,
                workflows.Select(u => u.WorkFlowID.Value).ToList(), PrivacyObjectType.QAWorkFlow, PermissionType.Create);

            Guid? adminId = null;

            if (ids.Count > 0)
            {
                QAWorkFlow wf = workflows.Where(u => ids.Any(x => x == u.WorkFlowID.Value)).First();

                workflowId = wf.WorkFlowID;

                if ((wf.InitialCheckNeeded.HasValue && wf.InitialCheckNeeded.Value) ||
                    (wf.FinalConfirmationNeeded.HasValue && wf.FinalConfirmationNeeded.Value) ||
                    wf.AnswerBy == AnswerBy.SelectedUsers)
                {
                    List<Guid> adminIds =
                        QAController.get_workflow_admin_ids(paramsContainer.Tenant.Id, workflowId.Value);

                    if (adminIds.Count > 1 && adminIds.Any(u => u == paramsContainer.CurrentUserID.Value))
                        adminIds.Remove(paramsContainer.CurrentUserID.Value);

                    if(adminIds.Count == 0)
                    {
                        responseText = "{\"ErrorText\":\"" + Messages.CannotFindAdmins + "\"}";
                        return;
                    }

                    adminId = adminIds[PublicMethods.get_random_number(0, adminIds.Count - 1)];
                }
                
                status = wf.InitialCheckNeeded.HasValue && wf.InitialCheckNeeded.Value ? QuestionStatus.Pending :
                    (wf.FinalConfirmationNeeded.HasValue && wf.FinalConfirmationNeeded.Value ?
                    QuestionStatus.Registered : QuestionStatus.Accepted);

                if ((wf.PublishAfter == PublishAfter.None && status == QuestionStatus.Accepted) ||
                    (wf.PublishAfter == PublishAfter.Registration) ||
                    (wf.PublishAfter == PublishAfter.InitialCheck && status == QuestionStatus.Registered) ||
                    (wf.PublishAfter == PublishAfter.FinalConfirmation && status == QuestionStatus.Accepted))
                    publicationDate = DateTime.Now;
            }
            else if (workflows.Count == 0)
            {
                status = QuestionStatus.Accepted;
                publicationDate = DateTime.Now;
            }
            
            Guid questionId = Guid.NewGuid();
            
            List<Dashboard> sentDashboards = new List<Dashboard>();

            bool result = !string.IsNullOrEmpty(title) &&
                QAController.add_question(paramsContainer.Tenant.Id, questionId, title, description,
                status, publicationDate, nodeIds, workflowId, adminId,
                paramsContainer.CurrentUserID.Value, ref sentDashboards);

            List<Guid> knowledgeableUsers = new List<Guid>();

            if (result)
            {
                knowledgeableUsers = QAController.find_knowledgeable_user_ids(paramsContainer.Tenant.Id, questionId, 50, 0);
                
                long totalCount = 0;

                List<Expert> experts = 
                    CNController.get_experts(paramsContainer.Tenant.Id, nodeIds, null, 20, 0, ref totalCount);

                List<NodeMember> members = CNController.get_members(paramsContainer.Tenant.Id, 
                    nodeIds, false, null, null, 20, 0, ref totalCount);

                foreach (Expert ex in experts)
                {
                    if (ex.User.UserID != paramsContainer.CurrentUserID &&
                        !knowledgeableUsers.Any(u => u == ex.User.UserID)) knowledgeableUsers.Add(ex.User.UserID.Value);
                }

                foreach (NodeMember nm in members)
                {
                    if (nm.Member.UserID != paramsContainer.CurrentUserID && 
                        !knowledgeableUsers.Any(u => u == nm.Member.UserID)) knowledgeableUsers.Add(nm.Member.UserID.Value);
                }

                //Send Notification
                if (result && (knowledgeableUsers.Count + nodeIds.Count) > 0)
                {
                    Notification not = new Notification()
                    {
                        SubjectID = questionId,
                        RefItemID = questionId,
                        SubjectType = SubjectType.Question,
                        Action = Modules.NotificationCenter.ActionType.Question,
                        SubjectName = title,
                        Description = description,
                        ReceiverUserIDs = knowledgeableUsers
                    };
                    not.Sender.UserID = paramsContainer.CurrentUserID;
                    NotificationController.send_notification(paramsContainer.Tenant.Id, not);
                }
                //end of Send Notification

                User su = UserUtilities.SystemUser(paramsContainer.Tenant.Id);

                if (su != null && su.UserID.HasValue)
                {
                    string text = "همکار گرامی،" + "\n" +
                        "یکی از کاربران، پرسشی را با عنوان" +
                        " @[[" + questionId.ToString() + ":Question:" + Base64.encode(title) + "]] " +
                        "مطرح کرده است." + "\n" +
                        "رای ون شما را به عنوان فردی مطلع شناسایی نموده است. مشارکت شما در پاسخگویی به پرسش مطرح شده، علاوه بر یاری رسانی به همکارتان، موجب شناسایی شما به عنوان یک خبره در سازمان خواهد شد.";

                    MSGController.bulk_send_message(paramsContainer.Tenant.Id,
                        su.UserID.Value, knowledgeableUsers, "پرسش جدید", text);
                }
            }

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SendQuestion,
                    SubjectID = questionId,
                    Info = "{\"Title\":\"" + Base64.encode(title) + "\"" + 
                        ",\"Description\":\"" + Base64.encode(description) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void edit_question_title(Guid? questionId, string title, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(title) && title.Length > 250)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(title))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            if (questionId.HasValue && !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) &&
                !QAController.is_question_owner(paramsContainer.Tenant.Id, questionId.Value, paramsContainer.CurrentUserID.Value) &&
                !QAController.is_workflow_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, questionId))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.EditQuestionTitle_PermissionFailed, questionId);
                return;
            }
            
            bool result = !questionId.HasValue ? false : QAController.edit_question_title(paramsContainer.Tenant.Id,
                questionId.Value, title, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.EditQuestionTitle,
                    SubjectID = questionId,
                    Info = "{\"Title\":\"" + Base64.encode(title) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void edit_question_description(Guid? questionId, string description, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (questionId.HasValue && !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) &&
                !QAController.is_question_owner(paramsContainer.Tenant.Id, questionId.Value, paramsContainer.CurrentUserID.Value) &&
                !QAController.is_workflow_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, questionId))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.EditQuestionDescription_PermissionFailed, questionId);
                return;
            }

            bool result = !questionId.HasValue ? false : QAController.edit_question_description(paramsContainer.Tenant.Id,
                questionId.Value, description, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.EditQuestionDescription,
                    SubjectID = questionId,
                    Info = "{\"Description\":\"" + Base64.encode(description) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void set_question_status(Guid? questionId, QuestionStatus status, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            Modules.Log.Action failAction = Modules.Log.Action.None;
            Modules.Log.Action action = Modules.Log.Action.None;

            QAWorkFlow wf = null;
            if (questionId.HasValue) QAController.get_workflow(paramsContainer.Tenant.Id, questionId.Value);

            bool publish = false;

            switch (status)
            {
                case QuestionStatus.Registered:
                    failAction = Modules.Log.Action.InitialConfirmQuestion_PermissionFailed;
                    action = Modules.Log.Action.InitialConfirmQuestion;
                    if (wf == null || wf.PublishAfter == PublishAfter.None ||
                        wf.PublishAfter == PublishAfter.Registration ||
                        wf.PublishAfter == PublishAfter.InitialCheck) publish = true;
                    if (wf != null && !(wf.FinalConfirmationNeeded.HasValue && wf.FinalConfirmationNeeded.Value) &&
                        wf.PublishAfter == PublishAfter.FinalConfirmation) publish = true;
                    break;
                case QuestionStatus.Accepted:
                    failAction = Modules.Log.Action.ConfirmQuestion_PermissionFailed;
                    action = Modules.Log.Action.ConfirmQuestion;
                    if (wf == null || wf.PublishAfter == PublishAfter.None || 
                        wf.PublishAfter == PublishAfter.Registration || 
                        wf.PublishAfter == PublishAfter.InitialCheck || 
                        wf.PublishAfter == PublishAfter.FinalConfirmation) publish = true;
                    break;
                default:
                    return;
            }

            if (questionId.HasValue && !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) &&
                !QAController.is_workflow_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, questionId))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(failAction, questionId);
                return;
            }

            Question question = null;
            
            if (questionId.HasValue)
            {
                question = QAController.get_question(paramsContainer.Tenant.Id,
                    questionId.Value, paramsContainer.CurrentUserID.Value);

                if (question == null || (question.Status == QuestionStatus.Accepted && status != question.Status))
                {
                    responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                    return;
                }
            }

            if (wf == null) status = QuestionStatus.Accepted;
            else if (status == QuestionStatus.Registered &&
                !(wf.FinalConfirmationNeeded.HasValue && wf.FinalConfirmationNeeded.Value))
                status = QuestionStatus.Accepted;

            bool result = questionId.HasValue && QAController.set_question_status(paramsContainer.Tenant.Id,
                questionId.Value, status, publish, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Send Notification
            if (result && publish)
            {
                Notification not = new Notification()
                {
                    SubjectID = questionId,
                    RefItemID = questionId,
                    SubjectType = SubjectType.Question,
                    Action = Modules.NotificationCenter.ActionType.Publish,
                    Description = question.Title,
                    Sender = new User() { UserID = paramsContainer.CurrentUserID }
                };
                NotificationController.send_notification(paramsContainer.Tenant.Id, not);
            }
            //end of Send Notification

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = action,
                    SubjectID = questionId,
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }
        
        protected void set_the_best_answer(Guid? questionId, Guid? answerId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            bool hasAccess = questionId.HasValue;

            QAWorkFlow wf = QAController.get_workflow(paramsContainer.Tenant.Id, questionId.Value);

            if (hasAccess && wf != null && wf.DisableBestAnswer.HasValue && wf.DisableBestAnswer.Value)
                hasAccess = false;

            if (hasAccess && !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) &&
                !QAController.is_question_owner(paramsContainer.Tenant.Id, questionId.Value, paramsContainer.CurrentUserID.Value) &&
                !QAController.is_workflow_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, questionId))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.SetTheBestAnswer_PermissionFailed, questionId, answerId);
                return;
            }

            bool publish = wf == null || wf.PublishAfter == PublishAfter.ChoosingTheBestAnswer;

            bool result = questionId.HasValue && answerId.HasValue && 
                QAController.set_the_best_answer(paramsContainer.Tenant.Id,
                questionId.Value, answerId.Value, publish, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetTheBestAnswer,
                    SubjectID = questionId,
                    SecondSubjectID = answerId,
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void remove_question(Guid? questionId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            QAWorkFlow wf = null;
            if (questionId.HasValue) wf = QAController.get_workflow(paramsContainer.Tenant.Id, questionId.Value);

            if (questionId.HasValue && 
                !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) &&
                !QAController.is_question_owner(paramsContainer.Tenant.Id, questionId.Value, paramsContainer.CurrentUserID.Value) &&
                !QAController.is_workflow_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, questionId))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.RemoveQuestion_PermissionFailed, questionId);
                return;
            }

            bool result = questionId.HasValue && QAController.remove_question(paramsContainer.Tenant.Id,
                questionId.Value, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RemoveQuestion,
                    SubjectID = questionId,
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void get_question(Guid? questionId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;

            Question question = null;
            if (questionId.HasValue) question = QAController.get_question(paramsContainer.Tenant.Id,
                questionId.Value, paramsContainer.CurrentUserID);

            if (question == null)
            {
                responseText = "{}";
                return;
            }

            QAWorkFlow workflow = null;

            if (question.WorkFlowID.HasValue)
                workflow = QAController.get_workflow(paramsContainer.Tenant.Id, question.WorkFlowID.Value);

            bool isSystemAdmin = paramsContainer.CurrentUserID.HasValue &&
                PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value);
            bool isOwner = question.Sender.UserID.HasValue && question.Sender.UserID == paramsContainer.CurrentUserID;
            bool isWorkFlowAdmin = paramsContainer.CurrentUserID.HasValue &&
                (AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID.Value) ||
                QAController.is_workflow_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, questionId));

            bool isQAAdmin = AuthorizationManager.has_right(AccessRoleName.ManageQA, paramsContainer.CurrentUserID);

            bool perWFLevel = isSystemAdmin || isWorkFlowAdmin || isQAAdmin;
            bool perOwnerLevel = perWFLevel || isOwner;
            
            //Check Access
            if(!question.PublicationDate.HasValue && !perOwnerLevel)
            {
                bool hasAccess = workflow != null && paramsContainer.CurrentUserID.HasValue &&
                    NotificationController.dashboard_exists(paramsContainer.Tenant.Id,
                    paramsContainer.CurrentUserID.Value, questionId, DashboardType.Question, done: false);

                if(!hasAccess && workflow != null && paramsContainer.CurrentUserID.HasValue)
                {
                    if (workflow.AnswerBy == AnswerBy.SelectedUsers)
                    {
                        hasAccess = QAController.is_related_user(paramsContainer.Tenant.Id,
                            questionId.Value, paramsContainer.CurrentUserID.Value);
                    }
                }

                if (!hasAccess)
                {
                    responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"" + 
                        ",\"Title\":\"" + Base64.encode(question.Title) + "\"}";
                    return;
                }
            }
            //end of Check Access


            question.Answers = QAController.get_answers(paramsContainer.Tenant.Id,
                questionId.Value, paramsContainer.CurrentUserID);

            List<Comment> comments = workflow != null && workflow.DisableComments.HasValue &&
                    workflow.DisableComments.Value ? new List<Comment>() : 
                QAController.get_comments(paramsContainer.Tenant.Id,
                    questionId.Value, paramsContainer.CurrentUserID);

            question.Comments = comments.Where(u => u.OwnerID == question.QuestionID).ToList();
            question.Answers.ForEach(ans => ans.Comments = comments.Where(u => u.OwnerID == ans.AnswerID).ToList());

            long totalCount = 0;

            question.RelatedNodes = QAController.get_related_nodes(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID, questionId, string.Empty, !perOwnerLevel, 1000000, 0, ref totalCount);

            responseText = question.toJson(paramsContainer.Tenant.Id, 
                paramsContainer.CurrentUserID, isSystemAdmin || isWorkFlowAdmin, workflow);
        }

        protected void get_questions(Guid? nodeId, string searchText, bool? startWithSearch, 
            DateTime? dateFrom, DateTime? dateTo, int? count, double? lowerBoundary, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;

            long totalCount = 0;

            List<Question> questions = nodeId.HasValue && nodeId != Guid.Empty ?
                QAController.get_questions(paramsContainer.Tenant.Id, nodeId.Value, searchText, 
                    startWithSearch.HasValue && startWithSearch.Value, dateFrom, dateTo, count, lowerBoundary, ref totalCount) :
                QAController.get_questions(paramsContainer.Tenant.Id, searchText, 
                    startWithSearch.HasValue && startWithSearch.Value, dateFrom, dateTo, count, lowerBoundary, ref totalCount);

            responseText = "{\"TotalCount\":" + totalCount.ToString() + ",\"Questions\":[" +
                string.Join(",", questions.Select(u => u.toJson(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID, false))) + "]}";
        }

        protected void find_related_questions(Guid? questionId,
            int? count, double? lowerBoundary, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;

            long totalCount = 0;

            List<Question> questions = !questionId.HasValue ? new List<Question>() :
                QAController.find_related_questions(paramsContainer.Tenant.Id,
                questionId.Value, count, lowerBoundary, ref totalCount);

            responseText = "{\"TotalCount\":" + totalCount.ToString() + ",\"Questions\":[" +
                string.Join(",", questions.Select(u => u.toJson(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID, false))) + "]}";
        }

        protected void get_related_questions(bool? groups, bool? expertiseDomains, bool? favorites,
            bool? properties, bool? fromFriends, int? count, double? lowerBoundary, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            long totalCount = 0;

            List<Question> questions = QAController.get_related_questions(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, groups, expertiseDomains, favorites, properties, fromFriends,
                count, lowerBoundary, ref totalCount);

            responseText = "{\"TotalCount\":" + totalCount.ToString() + ",\"Questions\":[" +
                string.Join(",", questions.Select(u => u.toJson(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID, false))) + "]}";
        }

        protected void my_favorite_questions(int? count, double? lowerBoundary, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            long totalCount = 0;

            List<Question> questions = QAController.my_favorite_questions(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, count, lowerBoundary, ref totalCount);

            responseText = "{\"TotalCount\":" + totalCount.ToString() + ",\"Questions\":[" +
                string.Join(",", questions.Select(u => u.toJson(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID, false))) + "]}";
        }

        protected void my_asked_questions(int? count, double? lowerBoundary, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            long totalCount = 0;

            List<Question> questions = QAController.my_asked_questions(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, count, lowerBoundary, ref totalCount);

            responseText = "{\"TotalCount\":" + totalCount.ToString() + ",\"Questions\":[" +
                string.Join(",", questions.Select(u => u.toJson(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID, false))) + "]}";
        }

        protected void questions_asked_of_me(int? count, double? lowerBoundary, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            long totalCount = 0;

            List<Question> questions = QAController.questions_asked_of_me(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, count, lowerBoundary, ref totalCount);

            responseText = "{\"TotalCount\":" + totalCount.ToString() + ",\"Questions\":[" +
                string.Join(",", questions.Select(u => u.toJson(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID, false))) + "]}";
        }
        
        protected void get_faq_items(Guid? categoryId, int? count, double? lowerBoundary, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;

            bool access = categoryId.HasValue;

            if (access && !(paramsContainer.CurrentUserID.HasValue &&
                PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value)))
                access = PrivacyController.check_access(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID,
                    categoryId.Value, PrivacyObjectType.FAQCategory, PermissionType.View);
            
            long totalCount = 0;

            List<Question> questions = !access ? new List<Question>() :
                QAController.get_faq_items(paramsContainer.Tenant.Id,
                categoryId.Value, count, lowerBoundary, ref totalCount);
            
            responseText = "{\"TotalCount\":" + totalCount.ToString() + ",\"Questions\":[" +
                string.Join(",", questions.Select(u => u.toJson(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID, false))) + "]}";
        }

        protected void group_questions_by_related_nodes(string searchText,
            int? count, double? lowerBoundary, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;
            
            //bool checkAccess = paramsContainer.CurrentUserID.HasValue &&
            //    PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value);
            bool checkAccess = false;

            long totalCount = 0;
            
            List<RelatedNode> nodes = QAController.group_questions_by_related_nodes(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID, searchText, checkAccess, count, lowerBoundary, ref totalCount);

            responseText = "{\"TotalCount\":" + totalCount.ToString() + 
                ",\"Nodes\":[" + string.Join(",", nodes.Select(u => u.toJson())) + "]}";
        }

        protected void get_related_tags(Guid? questionId, string searchText,
            int? count, double? lowerBoundary, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;

            long totalCount = 0;

            List<RelatedNode> nodes = !questionId.HasValue ? new List<RelatedNode>() :
                QAController.get_related_nodes(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value,
                    questionId.Value, searchText, false, count, lowerBoundary, ref totalCount);

            responseText = "{\"TotalCount\":" + totalCount.ToString() + 
                ",\"Nodes\":[" + string.Join(",", nodes.Select(u => u.toJson())) + "]}";
        }

        protected void find_related_tags(Guid? nodeId,
            int? count, double? lowerBoundary, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;
            
            long totalCount = 0;

            List<RelatedNode> nodes = !nodeId.HasValue ? new List<RelatedNode>() : 
                QAController.find_related_tags(paramsContainer.Tenant.Id, nodeId.Value, count, lowerBoundary, ref totalCount);

            responseText = "{\"TotalCount\":" + totalCount.ToString() + 
                ",\"Nodes\":[" + string.Join(",", nodes.Select(u => u.toJson())) + "]}";
        }

        protected void add_question_tag(Guid? questionId, string tag, bool? isNewQuestion, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(tag) && tag.Length > 100)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(tag))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            if (questionId.HasValue && !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) &&
                !QAController.is_question_owner(paramsContainer.Tenant.Id, questionId.Value, paramsContainer.CurrentUserID.Value) &&
                !QAController.is_workflow_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, questionId))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.AddQuestionTag_PermissionFailed, questionId);
                return;
            }

            isNewQuestion = isNewQuestion.HasValue && isNewQuestion.Value;

            bool result = questionId.HasValue || isNewQuestion.Value;

            long totalCount = 0;

            List<RelatedNode> nodes = !result ? new List<RelatedNode>() :
                QAController.search_nodes(paramsContainer.Tenant.Id, tag, true, false, false, 100, 0, ref totalCount);

            if(nodes.Count > 1)
            {
                responseText = "{\"Tags\":[" + string.Join(",", nodes.Select(u => u.toJson())) + "]}";
                return;
            }

            if (result)
            {
                if(nodes.Count == 0)
                {
                    Guid nodeId = Guid.NewGuid();

                    result = CNController.add_node(paramsContainer.Tenant.Id, new Node()
                    {
                        NodeID = nodeId,
                        Name = tag,
                        Searchable = false,
                        Creator = new User() { UserID = paramsContainer.CurrentUserID },
                        CreationDate = DateTime.Now
                    }, NodeTypes.Tag);

                    nodes = QAController.check_nodes(paramsContainer.Tenant.Id, new List<Guid>() { nodeId });
                }

                result = isNewQuestion.Value || 
                    QAController.add_related_nodes(paramsContainer.Tenant.Id, questionId.Value,
                        new List<Guid>() { nodes[0].NodeID.Value }, paramsContainer.CurrentUserID.Value);

                if (result && isNewQuestion.Value) ++nodes[0].Count;
            }

            if (result) responseText = "[" + string.Join(",", nodes.Select(u => u.toJson())) + "]";

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\",\"Tags\":" + responseText + "}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.AddQuestionTag,
                    SubjectID = questionId,
                    Info = "{\"Tag\":\"" + Base64.encode(tag) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void save_related_nodes(Guid? questionId, List<Guid> nodeIds, bool add, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (questionId.HasValue && !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) &&
                !QAController.is_question_owner(paramsContainer.Tenant.Id, questionId.Value, paramsContainer.CurrentUserID.Value) &&
                !QAController.is_workflow_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, questionId))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.SaveQuestionRelatedNodes_PermissionFailed, questionId);
                return;
            }

            bool result = questionId.HasValue && (add ? 
                QAController.add_related_nodes(paramsContainer.Tenant.Id,
                    questionId.Value, nodeIds, paramsContainer.CurrentUserID.Value) : 
                QAController.save_related_nodes(paramsContainer.Tenant.Id,
                    questionId.Value, nodeIds, paramsContainer.CurrentUserID.Value));

            if (result)
            {
                long totalCount = 0;

                List<RelatedNode> relatedNodes = add ? QAController.check_nodes(paramsContainer.Tenant.Id, nodeIds) :
                    QAController.get_related_nodes(paramsContainer.Tenant.Id,
                        paramsContainer.CurrentUserID, questionId, string.Empty, false, 1000000, 0, ref totalCount);

                responseText = "[" + string.Join(",", relatedNodes.Select(u => u.toJson())) + "]";
            }

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\",\"Tags\":" + responseText + "}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SaveQuestionRelatedNodes,
                    SubjectID = questionId,
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void remove_related_nodes(Guid? questionId, List<Guid> nodeIds, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (questionId.HasValue && !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) &&
                !QAController.is_question_owner(paramsContainer.Tenant.Id, questionId.Value, paramsContainer.CurrentUserID.Value) &&
                !QAController.is_workflow_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, questionId))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.RemoveQuestionRelatedNodes_PermissionFailed, questionId);
                return;
            }

            bool result = questionId.HasValue && QAController.remove_related_nodes(paramsContainer.Tenant.Id,
                questionId.Value, nodeIds, paramsContainer.CurrentUserID.Value);
            
            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RemoveQuestionRelatedNodes,
                    SubjectID = questionId,
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void check_nodes(List<Guid> nodeIds, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            List<RelatedNode> nodes = QAController.check_nodes(paramsContainer.Tenant.Id, nodeIds);

            responseText = "{\"Tags\":[" + string.Join(",", nodes.Select(u => u.toJson())) + "]}";
        }

        protected void search_nodes(string searchText, bool? startWithSearch, 
            int? count, long? lowerBoundary, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!count.HasValue) count = 20;

            long totalCount = 0;

            List<RelatedNode> countSort = QAController.search_nodes(paramsContainer.Tenant.Id, searchText, false,
                startWithSearch.HasValue && startWithSearch.Value, false, count, lowerBoundary, ref totalCount);
            List<RelatedNode> rankSort = QAController.search_nodes(paramsContainer.Tenant.Id, searchText, false,
                startWithSearch.HasValue && startWithSearch.Value, true, count, lowerBoundary, ref totalCount);

            List<RelatedNode> nodes = new List<RelatedNode>();

            for(int i = 0, lnt = Math.Max(countSort.Count, rankSort.Count); i < lnt; ++i)
            {
                if (nodes.Count >= count) break;

                if (i < countSort.Count && !nodes.Any(u => u.NodeID == countSort[i].NodeID)) nodes.Add(countSort[i]);
                if (i < rankSort.Count && !nodes.Any(u => u.NodeID == rankSort[i].NodeID)) nodes.Add(rankSort[i]);
            }
            
            responseText = "{\"TotalCount\":" + totalCount.ToString() + 
                ",\"Tags\":[" + string.Join(",", nodes.Select(u => u.toJson())) + "]}";
        }

        protected void send_answer(Guid? questionId, string answerBody, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!questionId.HasValue)
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }

            QAWorkFlow workflow = QAController.get_workflow(paramsContainer.Tenant.Id, questionId.Value);

            bool isOwner = false;
            bool isWfAdmin = false;
            
            if(workflow != null && workflow.AnswerBy == AnswerBy.SelectedUsers)
            {
                isOwner = QAController.is_question_owner(paramsContainer.Tenant.Id, 
                    questionId.Value, paramsContainer.CurrentUserID.Value);
                isWfAdmin = PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) || 
                    QAController.is_workflow_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, null) ||
                    QAController.is_workflow_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, questionId);

                if (!isOwner && !isWfAdmin &&
                    !QAController.is_related_user(paramsContainer.Tenant.Id, questionId.Value, paramsContainer.CurrentUserID.Value))
                {
                    responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                    _save_error_log(Modules.Log.Action.SendAnswer_PermissionFailed, questionId);
                    return;
                }
            }

            Guid answerId = Guid.NewGuid();

            bool result = questionId.HasValue && !string.IsNullOrEmpty(answerBody) && 
                QAController.send_answer(paramsContainer.Tenant.Id, answerId, 
                questionId.Value, answerBody, paramsContainer.CurrentUserID.Value);

            Answer answer = null;

            if (result)
            {
                User sender = UsersController.get_user(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value);

                answer = new Answer()
                {
                    AnswerID = answerId,
                    AnswerBody = answerBody,
                    Sender = sender == null ? new User() : sender
                };
            }

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"" +
                ",\"Answer\":" + answer.toJson(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID, isWfAdmin) + "}";

            //Send Notification
            if (result)
            {
                Notification not = new Notification()
                {
                    SubjectID = answerId,
                    RefItemID = questionId,
                    SubjectType = SubjectType.Answer,
                    Action = ActionType.Answer,
                    Description = answerBody
                };
                not.Sender.UserID = paramsContainer.CurrentUserID;
                NotificationController.send_notification(paramsContainer.Tenant.Id, not);
            }
            //end of Send Notification

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SendAnswer,
                    SubjectID = answerId,
                    Info = "{\"QuestionID\":\"" + questionId.ToString() + "\"" +
                        ",\"AnswerBody\":\"" + Base64.encode(answerBody) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void edit_answer(Guid? answerId, string answerBody, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!answerId.HasValue)
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }
            
            if (!PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) &&
                !QAController.is_answer_owner(paramsContainer.Tenant.Id, answerId.Value, paramsContainer.CurrentUserID.Value) &&
                !QAController.is_workflow_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, null) &&
                !QAController.is_workflow_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, answerId))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.EditAnswer_PermissionFailed, answerId);
                return;
            }
            
            bool result = answerId.HasValue && QAController.edit_answer(paramsContainer.Tenant.Id,
                answerId.Value, answerBody, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.EditAnswer,
                    SubjectID = answerId,
                    Info = "{\"AnswerBody\":\"" + Base64.encode(answerBody) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void remove_answer(Guid? answerId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!answerId.HasValue)
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }
            
            if (!PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) &&
                !QAController.is_answer_owner(paramsContainer.Tenant.Id, answerId.Value, paramsContainer.CurrentUserID.Value) &&
                !QAController.is_workflow_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, null) &&
                !QAController.is_workflow_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, answerId))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.RemoveAnswer_PermissionFailed, answerId);
                return;
            }

            bool result = answerId.HasValue && QAController.remove_answer(paramsContainer.Tenant.Id,
                answerId.Value, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RemoveAnswer,
                    SubjectID = answerId,
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void get_answers(Guid? questionId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;

            List<Answer> answers = !questionId.HasValue ? new List<Answer>() :
                QAController.get_answers(paramsContainer.Tenant.Id, questionId.Value, paramsContainer.CurrentUserID.Value);

            responseText = "{\"Answers\":[" + string.Join(",", answers.Select(
                u => u.toJson(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID, false))) + "]}";
        }

        protected void send_comment(Guid? ownerId, Guid? replyToCommentId, string bodyText, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            bool hasAccess = ownerId.HasValue;

            if (hasAccess)
            {
                QAWorkFlow wf = QAController.get_workflow(paramsContainer.Tenant.Id, ownerId.Value);
                if (wf != null && wf.DisableComments.HasValue && wf.DisableComments.Value) hasAccess = false;
            }

            bool isQuestion = ownerId.HasValue && QAController.is_question(paramsContainer.Tenant.Id, ownerId.Value);
            bool isAnswer = !isQuestion && ownerId.HasValue && 
                QAController.is_answer(paramsContainer.Tenant.Id, ownerId.Value);

            if (!hasAccess || (!isQuestion && !isAnswer))
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }
            
            Guid commentId = Guid.NewGuid();

            bool result = ownerId.HasValue && !string.IsNullOrEmpty(bodyText) && 
                QAController.send_comment(paramsContainer.Tenant.Id, commentId, 
                ownerId.Value, replyToCommentId, bodyText, paramsContainer.CurrentUserID.Value);

            Comment comment = null;

            if (result)
            {
                User sender = UsersController.get_user(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value);

                comment = new Comment()
                {
                    CommentID = commentId,
                    BodyText = bodyText,
                    Sender = sender == null ? new User() : sender
                };
            }

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"" +
                ",\"Comment\":" + comment.toJson(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID, false) + "}";

            //Send Notification
            if (result)
            {
                Guid refItemId = ownerId.Value;

                if (isAnswer)
                {
                    Answer ans = QAController.get_answer(paramsContainer.Tenant.Id, 
                        ownerId.Value, paramsContainer.CurrentUserID);

                    if (ans != null) refItemId = ans.QuestionID.Value;
                    else refItemId = Guid.Empty;
                }

                if (refItemId != Guid.Empty)
                {
                    Notification not = new Notification()
                    {
                        SubjectID = commentId,
                        RefItemID = refItemId,
                        SubjectType = isQuestion ? SubjectType.Question : SubjectType.Answer,
                        Action = Modules.NotificationCenter.ActionType.Comment,
                        Description = bodyText
                    };
                    not.Sender.UserID = paramsContainer.CurrentUserID;
                    NotificationController.send_notification(paramsContainer.Tenant.Id, not);
                }
            }
            //end of Send Notification

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SendQAComment,
                    SubjectID = commentId,
                    Info = "{\"OwnerID\":\"" + ownerId.ToString() + "\"" +
                        ",\"BodyText\":\"" + Base64.encode(bodyText) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void edit_comment(Guid? commentId, string bodyText, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!commentId.HasValue)
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }

            Guid? ownerId = QAController.get_comment_owner_id(paramsContainer.Tenant.Id, commentId.Value);

            if (!ownerId.HasValue || (!PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) &&
                !QAController.is_comment_owner(paramsContainer.Tenant.Id, commentId.Value, paramsContainer.CurrentUserID.Value) &&
                !QAController.is_workflow_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, null) &&
                !QAController.is_workflow_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, ownerId)))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.EditQAComment_PermissionFailed, commentId);
                return;
            }

            bool result = commentId.HasValue && QAController.edit_comment(paramsContainer.Tenant.Id,
                commentId.Value, bodyText, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.EditQAComment,
                    SubjectID = commentId,
                    Info = "{\"BodyText\":\"" + Base64.encode(bodyText) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void remove_comment(Guid? commentId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!commentId.HasValue)
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }

            Guid? ownerId = QAController.get_comment_owner_id(paramsContainer.Tenant.Id, commentId.Value);

            if (!ownerId.HasValue || (!PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) &&
                !QAController.is_question_owner(paramsContainer.Tenant.Id, ownerId.Value, paramsContainer.CurrentUserID.Value) &&
                !QAController.is_answer_owner(paramsContainer.Tenant.Id, ownerId.Value, paramsContainer.CurrentUserID.Value) &&
                !QAController.is_comment_owner(paramsContainer.Tenant.Id, commentId.Value, paramsContainer.CurrentUserID.Value) &&
                !QAController.is_workflow_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, null) &&
                !QAController.is_workflow_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, ownerId)))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.RemoveQAComment_PermissionFailed, commentId);
                return;
            }

            bool result = commentId.HasValue && QAController.remove_comment(paramsContainer.Tenant.Id,
                commentId.Value, paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RemoveQAComment,
                    SubjectID = commentId,
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void add_knowledgable_user(Guid? questionId, Guid? userId, ref string responseText) {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (questionId.HasValue && !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) &&
                !QAController.is_workflow_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, questionId))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.AddQAKnowledgableUser_PermissionFailed, questionId, userId);
                return;
            }

            List<Dashboard> dash = new List<Dashboard>();

            bool result = questionId.HasValue && userId.HasValue && QAController.add_knowledgable_user(
                paramsContainer.Tenant.Id, questionId.Value, userId.Value, paramsContainer.CurrentUserID.Value, ref dash);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.AddQAKnowledgableUser,
                    SubjectID = questionId,
                    SecondSubjectID = userId,
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected void remove_knowledgable_user(Guid? questionId, Guid? userId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (questionId.HasValue && !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) &&
                !QAController.is_workflow_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, questionId))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.RemoveQAKnowledgableUser_PermissionFailed, questionId, userId);
                return;
            }
            
            bool result = questionId.HasValue && userId.HasValue && QAController.remove_knowledgable_user(
                paramsContainer.Tenant.Id, questionId.Value, userId.Value, paramsContainer.CurrentUserID.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RemoveQAKnowledgableUser,
                    SubjectID = questionId,
                    SecondSubjectID = userId,
                    ModuleIdentifier = ModuleIdentifier.QA
                });
            }
            //end of Save Log
        }

        protected string _get_user_json(User user)
        {
            return "{\"UserID\":\"" + user.UserID.ToString() + "\"" +
                ",\"UserName\":\"" + Base64.encode(user.UserName) + "\"" +
                ",\"FirstName\":\"" + Base64.encode(user.FirstName) + "\"" +
                ",\"LastName\":\"" + Base64.encode(user.LastName) + "\"" +
                ",\"ProfileImageURL\":\"" + 
                    DocumentUtilities.get_personal_image_address(paramsContainer.Tenant.Id, user.UserID.Value) + "\"" +
                "}";
        }

        protected void get_knowledgable_users(Guid? questionId, bool? suggestions, int? count, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (questionId.HasValue && !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) &&
                !QAController.is_workflow_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, questionId))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            if (!count.HasValue || count.Value <= 0) count = 20;

            List<User> users = !questionId.HasValue ? new List<User>() :
                UsersController.get_users(paramsContainer.Tenant.Id, 
                QAController.get_knowledgable_user_ids(paramsContainer.Tenant.Id, questionId.Value));

            List<Guid> suggestedIds = !questionId.HasValue ? new List<Guid>() :
                QAController.get_related_expert_and_member_ids(paramsContainer.Tenant.Id, questionId.Value)
                .Except(users.Select(u => u.UserID.Value)).ToList();

            if(suggestedIds.Count < count.Value && questionId.HasValue)
            {
                suggestedIds.AddRange(QAController.find_knowledgeable_user_ids(paramsContainer.Tenant.Id,
                    questionId.Value, count * 3, null).Except(suggestedIds)
                    .Except(users.Select(u => u.UserID.Value)).Take(count.Value - suggestedIds.Count).ToList());
            }

            List<User> suggestedUsers = UsersController.get_users(paramsContainer.Tenant.Id, suggestedIds);

            responseText = "{\"Users\":[" + 
                ProviderUtil.list_to_string<string>(users.Select(u => _get_user_json(u)).ToList()) + "]" + 
                ",\"Suggested\":[" + 
                ProviderUtil.list_to_string<string>(suggestedUsers.Select(u => _get_user_json(u)).ToList()) + "]}";
        }
        
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}