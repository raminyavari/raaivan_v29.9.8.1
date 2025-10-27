using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using RaaiVan.Modules.CoreNetwork;
using RaaiVan.Modules.Knowledge;
using RaaiVan.Modules.GlobalUtilities;
using RaaiVan.Modules.Users;
using RaaiVan.Modules.Log;
using RaaiVan.Modules.NotificationCenter;
using RaaiVan.Modules.Privacy;

namespace RaaiVan.Web.API
{
    /// <summary>
    /// Summary description for Knowledge
    /// </summary>
    public class KnowledgeAPI : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        public ParamsContainer paramsContainer = null;

        public void ProcessRequest(HttpContext context)
        {
            paramsContainer = new ParamsContainer(context, nullTenantResponse: true);
            if (!paramsContainer.ApplicationID.HasValue) return;
            
            string responseText = string.Empty;
            string command = PublicMethods.parse_string(context.Request.Params["Command"], false);
            
            switch (command)
            {
                case "AddKnowledgeType":
                    add_knowledge_type(PublicMethods.parse_guid(context.Request.Params["KnowledgeTypeID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RemoveKnowledgeType":
                    remove_knowledge_type(PublicMethods.parse_guid(context.Request.Params["KnowledgeTypeID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetKnowledgeTypes":
                    get_knowledge_types(ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetKnowledgeType":
                    bool? detail = PublicMethods.parse_bool(context.Request.Params["Detail"]);
                    get_knowledge_type(PublicMethods.parse_guid(context.Request.Params["KnowledgeTypeID"]), detail, ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetEvaluationType":
                    KnowledgeEvaluationType evaluationType = KnowledgeEvaluationType.NotSet;
                    if (!Enum.TryParse<KnowledgeEvaluationType>(context.Request.Params["EvaluationType"], out evaluationType))
                        evaluationType = KnowledgeEvaluationType.NotSet;

                    set_evaluation_type(PublicMethods.parse_guid(context.Request.Params["KnowledgeTypeID"]),
                        evaluationType, ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetEvaluators":
                    KnowledgeEvaluators evaluators = KnowledgeEvaluators.NotSet;
                    if (!Enum.TryParse<KnowledgeEvaluators>(context.Request.Params["Evaluators"], out evaluators))
                        evaluators = KnowledgeEvaluators.NotSet;

                    set_evaluators(PublicMethods.parse_guid(context.Request.Params["KnowledgeTypeID"]), evaluators,
                        PublicMethods.parse_int(context.Request.Params["MinEvaluationsCount"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetPreEvaluateByOwner":
                    set_pre_evaluate_by_owner(PublicMethods.parse_guid(context.Request.Params["KnowledgeTypeID"]),
                        PublicMethods.parse_bool(context.Request.Params["Value"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetForceEvaluatorsDescribe":
                    set_force_evaluators_describe(PublicMethods.parse_guid(context.Request.Params["KnowledgeTypeID"]),
                        PublicMethods.parse_bool(context.Request.Params["Value"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetNodeSelectType":
                    KnowledgeNodeSelectType nodeSelectType = KnowledgeNodeSelectType.NotSet;
                    if (!Enum.TryParse<KnowledgeNodeSelectType>(context.Request.Params["NodeSelectType"], out nodeSelectType))
                        nodeSelectType = KnowledgeNodeSelectType.NotSet;

                    set_node_select_type(PublicMethods.parse_guid(context.Request.Params["KnowledgeTypeID"]),
                        nodeSelectType, ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetSearchabilityType":
                    SearchableAfter searchableAfter = SearchableAfter.NotSet;
                    if (!Enum.TryParse<SearchableAfter>(context.Request.Params["SearchableAfter"], out searchableAfter))
                        searchableAfter = SearchableAfter.NotSet;

                    set_searchability_type(PublicMethods.parse_guid(context.Request.Params["KnowledgeTypeID"]),
                        searchableAfter, ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetScoreScale":
                    set_score_scale(PublicMethods.parse_guid(context.Request.Params["KnowledgeTypeID"]),
                        PublicMethods.parse_int(context.Request.Params["ScoreScale"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetMinAcceptableScore":
                    set_min_acceptable_score(PublicMethods.parse_guid(context.Request.Params["KnowledgeTypeID"]),
                        PublicMethods.parse_double(context.Request.Params["MinAcceptableScore"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetConvertEvaluatorsToExperts":
                    set_convert_evaluators_to_experts(PublicMethods.parse_guid(context.Request.Params["KnowledgeTypeID"]),
                        PublicMethods.parse_bool(context.Request.Params["Value"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetEvaluationsEditable":
                    set_evaluations_editable(PublicMethods.parse_guid(context.Request.Params["KnowledgeTypeID"]),
                        PublicMethods.parse_bool(context.Request.Params["Value"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetEvaluationsEditableForAdmin":
                    set_evaluations_editable_for_admin(PublicMethods.parse_guid(context.Request.Params["KnowledgeTypeID"]),
                        PublicMethods.parse_bool(context.Request.Params["Value"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetEvaluationsRemovable":
                    set_evaluations_removable(PublicMethods.parse_guid(context.Request.Params["KnowledgeTypeID"]),
                        PublicMethods.parse_bool(context.Request.Params["Value"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetUnhideEvaluators":
                    set_unhide_evaluators(PublicMethods.parse_guid(context.Request.Params["KnowledgeTypeID"]),
                        PublicMethods.parse_bool(context.Request.Params["Value"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetUnhideEvaluations":
                    set_unhide_evaluations(PublicMethods.parse_guid(context.Request.Params["KnowledgeTypeID"]),
                        PublicMethods.parse_bool(context.Request.Params["Value"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetUnhideNodeCreators":
                    set_unhide_node_creators(PublicMethods.parse_guid(context.Request.Params["KnowledgeTypeID"]),
                        PublicMethods.parse_bool(context.Request.Params["Value"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetTextOptions":
                    set_text_options(PublicMethods.parse_guid(context.Request.Params["KnowledgeTypeID"]),
                        PublicMethods.parse_string(context.Request.Params["Value"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetCandidateRelations":
                    set_candidate_relations(PublicMethods.parse_guid(context.Request.Params["KnowledgeTypeID"]),
                        ListMaker.get_guid_items(context.Request.Params["NodeTypeIDs"], '|'),
                        ListMaker.get_guid_items(context.Request.Params["NodeIDs"], '|'), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetCandidateRelations":
                    Guid? kTypeId = PublicMethods.parse_guid(context.Request.Params["KnowledgeTypeID"]);
                    Guid? kId = PublicMethods.parse_guid(context.Request.Params["KnowledgeID"]);

                    get_candidate_relations(!kTypeId.HasValue ? kId : kTypeId, ref responseText);
                    _return_response(ref responseText);
                    return;
                case "AddQuestion":
                    add_question(PublicMethods.parse_guid(context.Request.Params["KnowledgeTypeID"]),
                        PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                        PublicMethods.parse_string(context.Request.Params["QuestionBody"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "ModifyQuestion":
                    modify_question(PublicMethods.parse_guid(context.Request.Params["ID"]),
                        PublicMethods.parse_string(context.Request.Params["QuestionBody"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetQuestionsOrder":
                    set_questions_order(ListMaker.get_guid_items(context.Request.Params["IDs"], '|'), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetQuestionWeight":
                    set_question_weight(PublicMethods.parse_guid(context.Request.Params["ID"]),
                        PublicMethods.parse_double(context.Request.Params["Weight"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RemoveQuestion":
                    remove_question(PublicMethods.parse_guid(context.Request.Params["ID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RemoveRelatedNodeQuestions":
                    remove_related_node_questions(PublicMethods.parse_guid(context.Request.Params["KnowledgeTypeID"]),
                        PublicMethods.parse_guid(context.Request.Params["NodeID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "AddAnswerOption":
                    add_answer_option(PublicMethods.parse_guid(context.Request.Params["TypeQuestionID"]),
                        PublicMethods.parse_string(context.Request.Params["Title"]),
                        PublicMethods.parse_double(context.Request.Params["Value"]),
                        ref responseText);
                    _return_response(ref responseText);
                    return;
                case "ModifyAnswerOption":
                    modify_answer_option(PublicMethods.parse_guid(context.Request.Params["ID"]),
                        PublicMethods.parse_string(context.Request.Params["Title"]),
                        PublicMethods.parse_double(context.Request.Params["Value"]),
                        ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SetAnswerOptionsOrder":
                    set_answer_options_order(ListMaker.get_guid_items(context.Request.Params["IDs"], '|'), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RemoveAnswerOption":
                    remove_answer_option(PublicMethods.parse_guid(context.Request.Params["ID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetQuestions":
                    get_questions(PublicMethods.parse_guid(context.Request.Params["KnowledgeTypeID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SearchQuestions":
                    search_questions(PublicMethods.parse_string(context.Request.Params["SearchText"]),
                        PublicMethods.parse_int(context.Request.Params["Count"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetEvaluations":
                    get_evaluations(PublicMethods.parse_guid(context.Request.Params["KnowledgeID"]),
                        PublicMethods.parse_bool(context.Request.Params["Done"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetEvaluationFormQuestions":
                    get_evaluation_form_questions(PublicMethods.parse_guid(context.Request.Params["KnowledgeID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetFilledEvaluationForm":
                    get_filled_evaluation_form(PublicMethods.parse_guid(context.Request.Params["KnowledgeID"]),
                        PublicMethods.parse_guid(context.Request.Params["UserID"]),
                        PublicMethods.parse_int(context.Request.Params["WFVersionID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "AcceptKnowledge":
                    workflow_action(PublicMethods.parse_string(context.Request.Params["NodeTypeID"], false),
                        PublicMethods.parse_string(context.Request.Params["NodeID"], false),
                        PublicMethods.parse_guid(context.Request.Params["UserID"]), 
                        ActionType.Accept,
                        PublicMethods.parse_string(context.Request.Params["Description"]), null, ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RejectKnowledge":
                    workflow_action(PublicMethods.parse_string(context.Request.Params["NodeTypeID"], false),
                        PublicMethods.parse_string(context.Request.Params["NodeID"], false),
                        PublicMethods.parse_guid(context.Request.Params["UserID"]),
                        ActionType.Reject,
                        PublicMethods.parse_string(context.Request.Params["Description"]),
                        null,
                        ref responseText,
                        ListMaker.get_string_items(context.Request.Params["TextOptions"], '|').Select(u => Base64.decode(u)).ToList());
                    _return_response(ref responseText);
                    return;
                case "SendToAdmin":
                case "send_to_admin":
                    workflow_action(PublicMethods.parse_string(context.Request.Params["NodeTypeID"], false),
                        PublicMethods.parse_string(context.Request.Params["NodeID"], false),
                        PublicMethods.parse_guid(context.Request.Params["UserID"]), 
                        ActionType.SendToAdmin,
                        PublicMethods.parse_string(context.Request.Params["Description"]), null, ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SendBackForRevision":
                    workflow_action(PublicMethods.parse_string(context.Request.Params["NodeTypeID"], false),
                        PublicMethods.parse_string(context.Request.Params["NodeID"], false),
                        PublicMethods.parse_guid(context.Request.Params["UserID"]), 
                        ActionType.SendBackForRevision,
                        PublicMethods.parse_string(context.Request.Params["Description"]), 
                        null, 
                        ref responseText,
                        ListMaker.get_string_items(context.Request.Params["TextOptions"], '|').Select(u => Base64.decode(u)).ToList());
                    _return_response(ref responseText);
                    return;
                case "SendToEvaluators":
                    workflow_action(PublicMethods.parse_string(context.Request.Params["NodeTypeID"], false),
                        PublicMethods.parse_string(context.Request.Params["NodeID"], false),
                        PublicMethods.parse_guid(context.Request.Params["UserID"]), 
                        ActionType.SendToEvaluators,
                        PublicMethods.parse_string(context.Request.Params["Description"]), null, ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SendKnowledgeComment":
                    workflow_action(PublicMethods.parse_string(context.Request.Params["NodeTypeID"], false),
                        PublicMethods.parse_string(context.Request.Params["NodeID"], false),
                        PublicMethods.parse_guid(context.Request.Params["UserID"]), 
                        ActionType.Comment,
                        PublicMethods.parse_string(context.Request.Params["Description"]),
                        PublicMethods.parse_long(context.Request.Params["ReplyToHistoryID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "EditKnowledgeComment":
                    edit_knowledge_comment(PublicMethods.parse_long(context.Request.Params["ID"]),
                        PublicMethods.parse_string(context.Request.Params["Description"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "SaveEvaluationForm":
                    workflow_action(PublicMethods.parse_string(context.Request.Params["NodeTypeID"], false),
                        PublicMethods.parse_string(context.Request.Params["NodeID"], false),
                        PublicMethods.parse_guid(context.Request.Params["UserID"]),
                        ActionType.Evaluation,
                        PublicMethods.parse_string(context.Request.Params["Description"]), 
                        null, 
                        ref responseText,
                        ListMaker.get_string_items(context.Request.Params["TextOptions"], '|').Select(u => Base64.decode(u)).ToList());
                    _return_response(ref responseText);
                    return;
                case "RemoveEvaluator":
                    workflow_action(PublicMethods.parse_string(context.Request.Params["NodeTypeID"], false),
                        PublicMethods.parse_string(context.Request.Params["NodeID"], false),
                        PublicMethods.parse_guid(context.Request.Params["UserID"]),
                        ActionType.RemoveEvaluator,
                        PublicMethods.parse_string(context.Request.Params["Description"]), null, ref responseText);
                    _return_response(ref responseText);
                    return;
                case "RefuseEvaluation":
                    workflow_action(PublicMethods.parse_string(context.Request.Params["NodeTypeID"], false),
                        PublicMethods.parse_string(context.Request.Params["NodeID"], false),
                        PublicMethods.parse_guid(context.Request.Params["UserID"]), 
                        ActionType.RefuseEvaluation,
                        PublicMethods.parse_string(context.Request.Params["Description"]), null, ref responseText);
                    _return_response(ref responseText);
                    return;
                case "TerminateEvaluation":
                    workflow_action(PublicMethods.parse_string(context.Request.Params["NodeTypeID"], false),
                        PublicMethods.parse_string(context.Request.Params["NodeID"], false),
                        PublicMethods.parse_guid(context.Request.Params["UserID"]),
                        ActionType.TerminateEvaluation,
                        PublicMethods.parse_string(context.Request.Params["Description"]), null, ref responseText);
                    _return_response(ref responseText);
                    return;
                case "NewEvaluators":
                    new_evaluators(PublicMethods.parse_guid(context.Request.Params["NodeID"]),
                        ListMaker.get_guid_items(context.Request.Params["UserIDs"], '|'), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "EditHistoryDescription":
                    edit_history_description(PublicMethods.parse_long(context.Request.Params["ID"]), 
                        PublicMethods.parse_string(context.Request.Params["Description"]),
                        ref responseText);
                    _return_response(ref responseText);
                    return;
                case "GetHistory":
                    get_history(PublicMethods.parse_guid(context.Request.Params["KnowledgeID"]), ref responseText);
                    _return_response(ref responseText);
                    return;
                case "AddFeedBack":
                case "ModifyFeedBack":
                case "RemoveFeedBack":
                    if (command == "AddFeedBack")
                    {
                        FeedBackTypes feedbackType = FeedBackTypes.None;
                        if (!Enum.TryParse<FeedBackTypes>(context.Request.Params["FeedBackType"], out feedbackType))
                            feedbackType = FeedBackTypes.None;

                        add_feedback(PublicMethods.parse_guid(context.Request.Params["KnowledgeID"]), feedbackType,
                            PublicMethods.parse_double(context.Request.Params["Value"]),
                            PublicMethods.parse_string(context.Request.Params["Description"]), ref responseText);
                    }
                    else if (command == "ModifyFeedBack")
                    {
                        modify_feedback(PublicMethods.parse_long(context.Request.Params["FeedBackID"]),
                            PublicMethods.parse_double(context.Request.Params["Value"]),
                            PublicMethods.parse_string(context.Request.Params["Description"]), ref responseText);
                    }
                    else remove_feedback(PublicMethods.parse_long(context.Request.Params["FeedBackID"]), ref responseText);

                    _return_response(ref responseText);
                    return;
                case "GetKnowledgeFeedBacks":
                    {
                        FeedBackTypes feedbackType = FeedBackTypes.None;
                        if (!Enum.TryParse<FeedBackTypes>(context.Request.Params["FeedBackType"], out feedbackType))
                            feedbackType = FeedBackTypes.None;

                        get_knowledge_feedbacks(PublicMethods.parse_guid(context.Request.Params["KnowledgeID"]),
                            PublicMethods.parse_guid(context.Request.Params["UserID"]), feedbackType, ref responseText);
                        _return_response(ref responseText);
                    }
                    return;
                case "ActivateNecessaryItem":
                    {
                        NecessaryItem itm = NecessaryItem.Abstract;
                        if (Enum.TryParse<NecessaryItem>(context.Request.Params["ItemName"], out itm))
                        {
                            activate_necessary_item(PublicMethods.parse_guid(context.Request.Params["KnowledgeTypeID"]),
                                itm, ref responseText);
                        }
                        _return_response(ref responseText);
                        return;
                    }
                case "DeactiveNecessaryItem":
                    {
                        NecessaryItem itm = NecessaryItem.Abstract;
                        if (Enum.TryParse<NecessaryItem>(context.Request.Params["ItemName"], out itm))
                        {
                            deactive_necessary_item(PublicMethods.parse_guid(context.Request.Params["KnowledgeTypeID"]),
                                itm, ref responseText);
                        }
                        _return_response(ref responseText);
                        return;
                    }
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
                    ModuleIdentifier = ModuleIdentifier.KW
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

        protected void add_knowledge_type(Guid? knowledgeTypeId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!knowledgeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (knowledgeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.AddKnowledgeType_PermissionFailed, knowledgeTypeId);
                return;
            }

            bool result = KnowledgeController.add_knowledge_type(paramsContainer.Tenant.Id,
                knowledgeTypeId.Value, paramsContainer.CurrentUserID.Value);

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
                    Action = Modules.Log.Action.AddKnowledgeType,
                    SubjectID = knowledgeTypeId,
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        protected void remove_knowledge_type(Guid? knowledgeTypeId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!knowledgeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (knowledgeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.RemoveKnowledgeType_PermissionFailed, knowledgeTypeId);
                return;
            }

            bool result = KnowledgeController.remove_knowledge_type(paramsContainer.Tenant.Id,
                knowledgeTypeId.Value, paramsContainer.CurrentUserID.Value);

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
                    Action = Modules.Log.Action.RemoveKnowledgeType,
                    SubjectID = knowledgeTypeId,
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        protected void get_knowledge_types(ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            List<KnowledgeType> knowledgeTypes = KnowledgeController.get_knowledge_types(paramsContainer.Tenant.Id);

            responseText = "{\"KnowledgeTypes\":[";

            bool isFirst = true;
            foreach (KnowledgeType kt in knowledgeTypes)
            {
                responseText += (isFirst ? string.Empty : ",") +
                    "{\"KnowledgeTypeID\":\"" + kt.KnowledgeTypeID.Value.ToString() + "\"" +
                    ",\"KnowledgeType\":\"" + Base64.encode(kt.Name) + "\"" + "}";
                isFirst = false;
            }

            responseText += "]}";
        }

        protected void get_knowledge_type(Guid? knowledgeTypeId, bool? detail, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!knowledgeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            KnowledgeType knowledgeType =
                KnowledgeController.get_knowledge_type(paramsContainer.Tenant.Id, knowledgeTypeId.Value);

            if (detail.HasValue && detail.Value)
            {
                knowledgeType.CandidateNodeTypes =
                    KnowledgeController.get_candidate_node_type_relations(paramsContainer.Tenant.Id, knowledgeTypeId.Value);
                knowledgeType.CandidateNodes =
                    KnowledgeController.get_candidate_node_relations(paramsContainer.Tenant.Id, knowledgeTypeId.Value);
            }

            knowledgeType.NecessaryItems = KnowledgeController.get_necessary_items(
                paramsContainer.Tenant.Id, knowledgeTypeId.Value);

            responseText = knowledgeType == null ? "{}" : knowledgeType.toJson();
        }

        protected void set_evaluation_type(Guid? knowledgeTypeId, KnowledgeEvaluationType evaluationType, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!knowledgeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (knowledgeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.SetKnowledgeTypeEvaluationType_PermissionFailed, knowledgeTypeId);
                return;
            }

            bool result = KnowledgeController.set_evaluation_type(paramsContainer.Tenant.Id,
                knowledgeTypeId.Value, evaluationType);

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
                    Action = Modules.Log.Action.SetKnowledgeTypeEvaluationType,
                    SubjectID = knowledgeTypeId,
                    Info = "{\"EvaluationType\":\"" + evaluationType.ToString() + "\"}",
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        protected void set_evaluators(Guid? knowledgeTypeId, KnowledgeEvaluators evaluators, int? minEvaluationsCount, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!knowledgeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (knowledgeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.SetKnowledgeTypeEvaluators_PermissionFailed, knowledgeTypeId);
                return;
            }

            bool result = KnowledgeController.set_evaluators(paramsContainer.Tenant.Id,
                knowledgeTypeId.Value, evaluators, minEvaluationsCount);

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
                    Action = Modules.Log.Action.SetKnowledgeTypeEvaluators,
                    SubjectID = knowledgeTypeId,
                    Info = "{\"Evaluators\":\"" + evaluators.ToString() +
                        "\",\"MinEvaluationsCount\":" + (minEvaluationsCount.HasValue ? minEvaluationsCount.ToString() : "null") + "}",
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        protected void set_pre_evaluate_by_owner(Guid? knowledgeTypeId, bool? value, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!knowledgeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (knowledgeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.SetKnowledgeTypePreEvaluateByOwner_PermissionFailed, knowledgeTypeId);
                return;
            }

            bool result = KnowledgeController.set_pre_evaluate_by_owner(paramsContainer.Tenant.Id,
                knowledgeTypeId.Value, value.HasValue && value.Value);

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
                    Action = Modules.Log.Action.SetKnowledgeTypePreEvaluateByOwner,
                    SubjectID = knowledgeTypeId,
                    Info = "{\"Value\":" + (value.HasValue && value.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        protected void set_force_evaluators_describe(Guid? knowledgeTypeId, bool? value, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!knowledgeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (knowledgeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.SetKnowledgeTypeForceEvaluatorsDescribe_PermissionFailed, knowledgeTypeId);
                return;
            }

            bool result = KnowledgeController.set_force_evaluators_describe(paramsContainer.Tenant.Id,
                knowledgeTypeId.Value, value.HasValue && value.Value);

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
                    Action = Modules.Log.Action.SetKnowledgeTypeForceEvaluatorsDescribe,
                    SubjectID = knowledgeTypeId,
                    Info = "{\"Value\":" + (value.HasValue && value.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        protected void set_node_select_type(Guid? knowledgeTypeId, KnowledgeNodeSelectType nodeSelectType, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!knowledgeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (knowledgeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.SetKnowledgeTypeNodeSelectType_PermissionFailed, knowledgeTypeId);
                return;
            }

            bool result = KnowledgeController.set_node_select_type(paramsContainer.Tenant.Id,
                knowledgeTypeId.Value, nodeSelectType);

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
                    Action = Modules.Log.Action.SetKnowledgeTypeNodeSelectType,
                    SubjectID = knowledgeTypeId,
                    Info = "{\"NodeSelectType\":\"" + nodeSelectType.ToString() + "\"}",
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        protected void set_searchability_type(Guid? knowledgeTypeId, SearchableAfter searchableAfter, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!knowledgeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (knowledgeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.SetKnowledgeTypeSearchabilityType_PermissionFailed, knowledgeTypeId);
                return;
            }

            bool result = KnowledgeController.set_searchability_type(paramsContainer.Tenant.Id,
                knowledgeTypeId.Value, searchableAfter);

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
                    Action = Modules.Log.Action.SetKnowledgeTypeSearchabilityType,
                    SubjectID = knowledgeTypeId,
                    Info = "{\"SearchableAfter\":\"" + searchableAfter.ToString() + "\"}",
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        protected void set_score_scale(Guid? knowledgeTypeId, int? scoreScale, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!knowledgeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (knowledgeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.SetKnowledgeTypeScoreScale_PermissionFailed, knowledgeTypeId);
                return;
            }

            if (scoreScale <= 0) scoreScale = null;

            bool result = KnowledgeController.set_score_scale(paramsContainer.Tenant.Id,
                knowledgeTypeId.Value, scoreScale);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\",\"ScoreScale\":" +
                (scoreScale.HasValue ? scoreScale.ToString() : "null") + "}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetKnowledgeTypeScoreScale,
                    SubjectID = knowledgeTypeId,
                    Info = "{\"ScoreScale\":" + (scoreScale.HasValue ? scoreScale.ToString() : "null") + "}",
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        protected void set_min_acceptable_score(Guid? knowledgeTypeId, double? minAcceptableScore, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!knowledgeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (knowledgeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.SetKnowledgeTypeMinAcceptableScore_PermissionFailed, knowledgeTypeId);
                return;
            }

            if (minAcceptableScore < 0) minAcceptableScore = null;

            bool result = KnowledgeController.set_min_acceptable_score(paramsContainer.Tenant.Id,
                knowledgeTypeId.Value, minAcceptableScore);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\",\"MinAcceptableScore\":" +
                (minAcceptableScore.HasValue ? minAcceptableScore.ToString() : "null") + "}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetKnowledgeTypeMinAcceptableScore,
                    SubjectID = knowledgeTypeId,
                    Info = "{\"MinAcceptableScore\":" + (minAcceptableScore.HasValue ? minAcceptableScore.ToString() : "null") + "}",
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        protected void set_convert_evaluators_to_experts(Guid? knowledgeTypeId, bool? value, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!knowledgeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (knowledgeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.SetKnowledgeTypeConvertEvaluatorsToExperts_PermissionFailed, knowledgeTypeId);
                return;
            }

            bool result = KnowledgeController.set_convert_evaluators_to_experts(paramsContainer.Tenant.Id,
                knowledgeTypeId.Value, value.HasValue && value.Value);

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
                    Action = Modules.Log.Action.SetKnowledgeTypeConvertEvaluatorsToExperts,
                    SubjectID = knowledgeTypeId,
                    Info = "{\"Value\":" + (value.HasValue && value.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        protected void set_evaluations_editable(Guid? knowledgeTypeId, bool? value, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!knowledgeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (knowledgeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.SetKnowledgeTypeEvaluationsEditable_PermissionFailed, knowledgeTypeId);
                return;
            }

            bool result = KnowledgeController.set_evaluations_editable(paramsContainer.Tenant.Id,
                knowledgeTypeId.Value, value.HasValue && value.Value);

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
                    Action = Modules.Log.Action.SetKnowledgeTypeEvaluationsEditable,
                    SubjectID = knowledgeTypeId,
                    Info = "{\"Value\":" + (value.HasValue && value.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        protected void set_evaluations_editable_for_admin(Guid? knowledgeTypeId, bool? value, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!knowledgeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (knowledgeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.SetKnowledgeTypeEvaluationsEditableForAdmin_PermissionFailed, knowledgeTypeId);
                return;
            }

            bool result = KnowledgeController.set_evaluations_editable_for_admin(paramsContainer.Tenant.Id,
                knowledgeTypeId.Value, value.HasValue && value.Value);

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
                    Action = Modules.Log.Action.SetKnowledgeTypeEvaluationsEditableForAdmin,
                    SubjectID = knowledgeTypeId,
                    Info = "{\"Value\":" + (value.HasValue && value.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        protected void set_evaluations_removable(Guid? knowledgeTypeId, bool? value, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!knowledgeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (knowledgeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.SetKnowledgeTypeEvaluationsRemovable_PermissionFailed, knowledgeTypeId);
                return;
            }

            bool result = KnowledgeController.set_evaluations_removable(paramsContainer.Tenant.Id,
                knowledgeTypeId.Value, value.HasValue && value.Value);

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
                    Action = Modules.Log.Action.SetKnowledgeTypeEvaluationsRemovable,
                    SubjectID = knowledgeTypeId,
                    Info = "{\"Value\":" + (value.HasValue && value.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        protected void set_unhide_evaluators(Guid? knowledgeTypeId, bool? value, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!knowledgeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (knowledgeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.SetKnowledgeTypeUnhideEvaluators_PermissionFailed, knowledgeTypeId);
                return;
            }

            bool result = KnowledgeController.set_unhide_evaluators(paramsContainer.Tenant.Id,
                knowledgeTypeId.Value, value.HasValue && value.Value);

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
                    Action = Modules.Log.Action.SetKnowledgeTypeUnhideEvaluators,
                    SubjectID = knowledgeTypeId,
                    Info = "{\"Value\":" + (value.HasValue && value.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        protected void set_unhide_evaluations(Guid? knowledgeTypeId, bool? value, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!knowledgeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (knowledgeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.SetKnowledgeTypeUnhideEvaluations_PermissionFailed, knowledgeTypeId);
                return;
            }

            bool result = KnowledgeController.set_unhide_evaluations(paramsContainer.Tenant.Id,
                knowledgeTypeId.Value, value.HasValue && value.Value);

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
                    Action = Modules.Log.Action.SetKnowledgeTypeUnhideEvaluations,
                    SubjectID = knowledgeTypeId,
                    Info = "{\"Value\":" + (value.HasValue && value.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        protected void set_unhide_node_creators(Guid? knowledgeTypeId, bool? value, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!knowledgeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (knowledgeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.SetKnowledgeTypeUnhideNodeCreators_PermissionFailed, knowledgeTypeId);
                return;
            }

            bool result = KnowledgeController.set_unhide_node_creators(paramsContainer.Tenant.Id,
                knowledgeTypeId.Value, value.HasValue && value.Value);

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
                    Action = Modules.Log.Action.SetKnowledgeTypeUnhideNodeCreators,
                    SubjectID = knowledgeTypeId,
                    Info = "{\"Value\":" + (value.HasValue && value.Value).ToString().ToLower() + "}",
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        protected void set_text_options(Guid? knowledgeTypeId, string value, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!knowledgeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (knowledgeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.SetKnowledgeTypeTextOptions_PermissionFailed, knowledgeTypeId);
                return;
            }

            bool result = KnowledgeController.set_text_options(paramsContainer.Tenant.Id, knowledgeTypeId.Value, value);

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
                    Action = Modules.Log.Action.SetKnowledgeTypeTextOptions,
                    SubjectID = knowledgeTypeId,
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        protected void set_candidate_relations(Guid? knowledgeTypeId,
            List<Guid> nodeTypeIds, List<Guid> nodeIds, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!knowledgeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (knowledgeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.SetKnowledgeTypeCandidateRelations_PermissionFailed, knowledgeTypeId);
                return;
            }

            bool result = KnowledgeController.set_candidate_relations(paramsContainer.Tenant.Id,
                knowledgeTypeId.Value, nodeTypeIds, nodeIds, paramsContainer.CurrentUserID.Value);

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
                    Action = Modules.Log.Action.SetKnowledgeTypeCandidateRelations,
                    SubjectID = knowledgeTypeId,
                    Info = "{\"NodeTypeIDs\":[" +
                        ProviderUtil.list_to_string<string>(nodeTypeIds.Select(u => "\"" + u.ToString() + "\"").ToList(), ',') +
                        "],\"NodeIDs\":[" +
                        ProviderUtil.list_to_string<string>(nodeIds.Select(u => "\"" + u.ToString() + "\"").ToList(), ',') + "]}",
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        protected void get_candidate_relations(Guid? knowledgeTypeIdOrKnowledgeId, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            List<NodeType> candidateNodeTypes = !knowledgeTypeIdOrKnowledgeId.HasValue ? new List<NodeType>() :
                KnowledgeController.get_candidate_node_type_relations(paramsContainer.Tenant.Id,
                knowledgeTypeIdOrKnowledgeId.Value);

            responseText = "{\"NodeTypes\":[";

            bool isFirst = true;
            foreach (NodeType nt in candidateNodeTypes)
            {
                responseText += (isFirst ? string.Empty : ",") +
                    "{\"NodeTypeID\":\"" + nt.NodeTypeID.ToString() + "\"" +
                    ",\"TypeName\":\"" + Base64.encode(nt.Name) + "\"" +
                    ",\"ParentID\":\"" + (nt.ParentID.HasValue ? nt.ParentID.ToString() : string.Empty) + "\"" + "}";
                isFirst = false;
            }

            responseText += "],\"Nodes\":[";

            List<Node> candidateNodes = !knowledgeTypeIdOrKnowledgeId.HasValue ? new List<Node>() :
                KnowledgeController.get_candidate_node_relations(paramsContainer.Tenant.Id, knowledgeTypeIdOrKnowledgeId.Value);

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

        protected void add_question(Guid? knowledgeTypeId, Guid? nodeId, string questionBody, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!knowledgeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (knowledgeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.AddKnowledgeTypeQuestion_PermissionFailed, knowledgeTypeId, nodeId);
                return;
            }

            KnowledgeTypeQuestion question = new KnowledgeTypeQuestion()
            {
                ID = Guid.NewGuid(),
                KnowledgeTypeID = knowledgeTypeId,
                QuestionBody = questionBody,
                CreationDate = DateTime.Now
            };

            question.RelatedNode.NodeID = nodeId;
            question.Creator.UserID = paramsContainer.CurrentUserID.Value;

            string errorMessage = string.Empty;

            bool result = KnowledgeController.add_question(paramsContainer.Tenant.Id, question, ref errorMessage);

            string questionJson = result ? question.toJson() : "{}";

            responseText = !result ?
                "{\"ErrorText\":\"" + (string.IsNullOrEmpty(errorMessage) ?
                Messages.OperationFailed.ToString() : errorMessage) + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\",\"Question\":" + questionJson + "}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = question.CreationDate,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.AddKnowledgeTypeQuestion,
                    SubjectID = question.ID,
                    Info = questionJson,
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        protected void modify_question(Guid? id, string questionBody, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!id.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (id.HasValue) _save_error_log(Modules.Log.Action.ModifyKnowledgeTypeQuestion_PermissionFailed, id);
                return;
            }

            KnowledgeTypeQuestion question = new KnowledgeTypeQuestion()
            {
                ID = id,
                QuestionBody = questionBody,
                LastModificationDate = DateTime.Now
            };

            question.LastModifier.UserID = paramsContainer.CurrentUserID.Value;

            bool result = KnowledgeController.modify_question(paramsContainer.Tenant.Id, question);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = question.LastModificationDate,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.ModifyKnowledgeTypeQuestion,
                    SubjectID = id,
                    Info = "{\"QuestionBody\":\"" + Base64.encode(questionBody) + "\"}",
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        protected void set_questions_order(List<Guid> ids, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            bool result = KnowledgeController.set_questions_order(paramsContainer.Tenant.Id, ids);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void set_question_weight(Guid? id, double? weight, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (id.HasValue) _save_error_log(Modules.Log.Action.SetKnowledgeTypeQuestionWeight_PermissionFailed, id);
                return;
            }

            string errorMessage = string.Empty;

            if (!weight.HasValue) weight = 0;

            bool result = !id.HasValue ? false :
                KnowledgeController.set_question_weight(paramsContainer.Tenant.Id,
                id.Value, weight.Value, ref errorMessage);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + (string.IsNullOrEmpty(errorMessage) ?
                Messages.OperationFailed.ToString() : errorMessage) + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.SetKnowledgeTypeQuestionWeight,
                    SubjectID = id,
                    Info = "{\"Weight\":" + weight.Value.ToString() + "}",
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        protected void remove_question(Guid? id, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!id.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (id.HasValue) _save_error_log(Modules.Log.Action.RemoveKnowledgeTypeQuestion_PermissionFailed, id);
                return;
            }

            bool result = KnowledgeController.remove_question(paramsContainer.Tenant.Id,
                id.Value, paramsContainer.CurrentUserID.Value);

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
                    Action = Modules.Log.Action.RemoveKnowledgeTypeQuestion,
                    SubjectID = id,
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        protected void remove_related_node_questions(Guid? knowledgeTypeId, Guid? nodeId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!knowledgeTypeId.HasValue || !nodeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (knowledgeTypeId.HasValue && nodeId.HasValue)
                    _save_error_log(Modules.Log.Action.RemoveKnowledgeTypeRelatedNodeQuestions_PermissionFailed,
                        knowledgeTypeId, nodeId);
                return;
            }

            bool result = KnowledgeController.remove_related_node_questions(paramsContainer.Tenant.Id,
                knowledgeTypeId.Value, nodeId.Value, paramsContainer.CurrentUserID.Value);

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
                    Action = Modules.Log.Action.RemoveKnowledgeTypeRelatedNodeQuestions,
                    SubjectID = knowledgeTypeId,
                    Info = "{\"NodeID\":\"" + nodeId.ToString() + "\"}",
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        protected void add_answer_option(Guid? typeQuestionId, string title, double? value, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(title) && title.Length > 1900)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(title))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            if (!AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.AddKnowledgeTypeAnswerOption_PermissionFailed, typeQuestionId);
                return;
            }

            Guid id = Guid.NewGuid();

            string errorMessage = string.Empty;

            bool result = !typeQuestionId.HasValue || !value.HasValue ? false :
                KnowledgeController.add_answer_option(paramsContainer.Tenant.Id, id,
                typeQuestionId.Value, title, value.Value, paramsContainer.CurrentUserID.Value, ref errorMessage);

            responseText = !result ?
                "{\"ErrorText\":\"" + (string.IsNullOrEmpty(errorMessage) ?
                    Messages.OperationFailed.ToString() : errorMessage) + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully.ToString() + "\"" +
                ",\"Option\":" + (new AnswerOption()
                {
                    ID = id,
                    TypeQuestionID = typeQuestionId,
                    Title = title,
                    Value = value
                }).toJson() +
                "}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.AddKnowledgeTypeAnswerOption,
                    SubjectID = id,
                    Info = "{\"Title\":\"" + Base64.encode(title) + "\",\"Value\":" + value.ToString() + "}",
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        protected void modify_answer_option(Guid? id, string title, double? value, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(title) && title.Length > 1900)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }
            else if (!PublicMethods.is_secure_title(title))
            {
                responseText = "{\"ErrorText\":\"" + Messages.TheTextIsFormattedBadly + "\"}";
                return;
            }

            if (!AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.ModifyKnowledgeTypeAnswerOption_PermissionFailed, id);
                return;
            }

            string errorMessage = string.Empty;

            bool result = !id.HasValue || !value.HasValue ? false :
                KnowledgeController.modify_answer_option(paramsContainer.Tenant.Id, id.Value,
                title, value.Value, paramsContainer.CurrentUserID.Value, ref errorMessage);

            responseText = !result ?
                "{\"ErrorText\":\"" + (string.IsNullOrEmpty(errorMessage) ?
                    Messages.OperationFailed.ToString() : errorMessage) + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully.ToString() + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.ModifyKnowledgeTypeAnswerOption,
                    SubjectID = id,
                    Info = "{\"Title\":\"" + Base64.encode(title) + "\",\"Value\":" + value.ToString() + "}",
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        protected void set_answer_options_order(List<Guid> ids, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            bool result = KnowledgeController.set_answer_options_order(paramsContainer.Tenant.Id, ids);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void remove_answer_option(Guid? id, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                _save_error_log(Modules.Log.Action.RemoveKnowledgeTypeAnswerOption_PermissionFailed, id);
                return;
            }

            bool result = !id.HasValue ? false :
                KnowledgeController.remove_answer_option(paramsContainer.Tenant.Id, id.Value,
                paramsContainer.CurrentUserID.Value);

            responseText = !result ? "{\"ErrorText\":\"" + Messages.OperationFailed.ToString() + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully.ToString() + "\"}";

            //Save Log
            if (result)
            {
                LogController.save_log(paramsContainer.Tenant.Id, new Log()
                {
                    UserID = paramsContainer.CurrentUserID.Value,
                    Date = DateTime.Now,
                    HostAddress = PublicMethods.get_client_ip(HttpContext.Current),
                    HostName = PublicMethods.get_client_host_name(HttpContext.Current),
                    Action = Modules.Log.Action.RemoveKnowledgeTypeAnswerOption,
                    SubjectID = id,
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        protected void get_questions(Guid? knowledgeTypeId, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            List<KnowledgeTypeQuestion> questions = !knowledgeTypeId.HasValue ? new List<KnowledgeTypeQuestion>() :
                KnowledgeController.get_questions(paramsContainer.Tenant.Id, knowledgeTypeId.Value);

            Dictionary<Guid, List<KnowledgeTypeQuestion>> nodeQuestions =
                new Dictionary<Guid, List<KnowledgeTypeQuestion>>();

            List<AnswerOption> options = KnowledgeController.get_answer_options(paramsContainer.Tenant.Id,
                questions.Select(u => u.ID.Value).ToList());

            responseText = "{\"Questions\":[";

            bool isFirst = true;
            foreach (KnowledgeTypeQuestion q in questions)
            {
                if (q.RelatedNode.NodeID.HasValue)
                {
                    if (!nodeQuestions.ContainsKey(q.RelatedNode.NodeID.Value))
                        nodeQuestions[q.RelatedNode.NodeID.Value] = new List<KnowledgeTypeQuestion>();
                    nodeQuestions[q.RelatedNode.NodeID.Value].Add(q);
                    continue;
                }

                q.Options = options.Where(u => u.TypeQuestionID == q.ID).ToList();

                responseText += (isFirst ? string.Empty : ",") + q.toJson();
                isFirst = false;
            }

            responseText += "],\"RelatedNodes\":[";

            isFirst = true;
            foreach (Guid nodeId in nodeQuestions.Keys)
            {
                responseText += (isFirst ? string.Empty : ",") + "{\"NodeID\":\"" + nodeId.ToString() + "\"" +
                    ",\"Name\":\"" + Base64.encode(nodeQuestions[nodeId][0].RelatedNode.Name) + "\"" +
                    ",\"Questions\":[";
                isFirst = false;

                nodeQuestions[nodeId].ForEach(q => q.Options = options.Where(u => u.TypeQuestionID == q.ID).ToList());

                bool _if = true;
                foreach (KnowledgeTypeQuestion q in nodeQuestions[nodeId])
                {
                    responseText += (_if ? string.Empty : ",") + q.toJson();
                    _if = false;
                }

                responseText += "]}";
            }

            responseText += "]}";
        }

        protected void search_questions(string searchText, int? count, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            List<string> results = count.HasValue ?
                KnowledgeController.search_questions(paramsContainer.Tenant.Id, searchText, count) :
                KnowledgeController.search_questions(paramsContainer.Tenant.Id, searchText);

            responseText = "{\"Results\":[";

            bool isFirst = true;
            foreach (string str in results)
            {
                responseText += (isFirst ? string.Empty : ",") + "\"" + Base64.encode(str) + "\"";
                isFirst = false;
            }

            responseText += "]}";
        }

        protected void get_evaluations(Guid? knowledgeId, bool? done, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!knowledgeId.HasValue || (
                !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) &&
                !CNController.is_service_admin(paramsContainer.Tenant.Id, knowledgeId.Value, paramsContainer.CurrentUserID.Value) &&
                !CNController.is_node_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, knowledgeId.Value, null, null, null) &&
                !CNController.is_node_creator(paramsContainer.Tenant.Id, knowledgeId.Value, paramsContainer.CurrentUserID.Value)))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied.ToString() + "\"}";
                return;
            }

            int? wfVersionId = KnowledgeController.get_last_history_version_id(paramsContainer.Tenant.Id, knowledgeId.Value);

            List<KnowledgeEvaluation> evaluations = !done.HasValue || done.Value ?
                KnowledgeController.get_evaluations_done(paramsContainer.Tenant.Id, knowledgeId.Value, wfVersionId) : 
                new List<KnowledgeEvaluation>();

            List<User> notDone = done.HasValue && done.Value ? new List<User>() :
                UsersController.get_users(paramsContainer.Tenant.Id,
                NotificationController.get_dashboards(paramsContainer.Tenant.Id,
                null, null, knowledgeId, null, DashboardType.Knowledge,
                    DashboardSubType.Evaluator, null, false, null, null, null, null, 10000).Select(x => x.UserID.Value).ToList());

            KnowledgeType kt = KnowledgeController.get_knowledge_type(paramsContainer.Tenant.Id, knowledgeId.Value);

            evaluations.ForEach(e => e.Removable = kt != null && kt.EvaluationsRemovable.HasValue && kt.EvaluationsRemovable.Value);

            evaluations.AddRange(notDone.Select(u => new KnowledgeEvaluation() { Evaluator = u, Removable = true }));

            responseText = "[" + string.Join(",", evaluations.Select(u => u.toJson(paramsContainer.Tenant.Id))) + "]";
        }

        protected void get_evaluation_form_questions(Guid? knowledgeId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            KnowledgeType kType = null;

            bool hasAccess = knowledgeId.HasValue && (
                AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID) ||
                NotificationController.dashboard_exists(paramsContainer.Tenant.Id,
                    paramsContainer.CurrentUserID.Value, knowledgeId,
                    DashboardType.Knowledge, DashboardSubType.Evaluator, null, null)
                );

            if (!hasAccess && knowledgeId.HasValue)
            {
                Node nodeObject = CNController.get_node(paramsContainer.Tenant.Id, knowledgeId.Value, full: null);
                kType = KnowledgeController.get_knowledge_type(paramsContainer.Tenant.Id, knowledgeId.Value);

                if (nodeObject != null && kType != null && kType.PreEvaluateByOwner.HasValue &&
                    kType.PreEvaluateByOwner.Value && nodeObject.isPersonal(paramsContainer.CurrentUserID.Value) &&
                    nodeObject.Creator.UserID.HasValue && nodeObject.Creator.UserID == paramsContainer.CurrentUserID &&
                    !KnowledgeController.has_evaluated(paramsContainer.Tenant.Id, knowledgeId.Value, paramsContainer.CurrentUserID.Value))
                {
                    hasAccess = true;
                }
            }

            if (!hasAccess)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                return;
            }

            if (kType == null)
                kType = KnowledgeController.get_knowledge_type(paramsContainer.Tenant.Id, knowledgeId.Value);

            List<Node> relatedNodes = CNController.get_related_nodes(paramsContainer.Tenant.Id,
                knowledgeId.Value, null, null, false, true);

            List<KnowledgeTypeQuestion> questions =
                KnowledgeController.get_questions(paramsContainer.Tenant.Id, kType.KnowledgeTypeID.Value);

            questions = questions.Where(u => !u.RelatedNode.NodeID.HasValue || relatedNodes.Any(v => v.NodeID == u.RelatedNode.NodeID)).ToList();

            List<KnowledgeTypeQuestion> validQuestions = new List<KnowledgeTypeQuestion>();

            switch (kType.EvaluationType)
            {
                case KnowledgeEvaluationType.EN:
                    List<Guid> exDomains = CNController.get_expertise_domain_ids(paramsContainer.Tenant.Id,
                        paramsContainer.CurrentUserID.Value, true, true);

                    foreach (KnowledgeTypeQuestion q in questions)
                    {
                        if (!q.RelatedNode.NodeID.HasValue || exDomains.Any(u => u == q.RelatedNode.NodeID.Value))
                            validQuestions.Add(q);
                    }
                    break;
                case KnowledgeEvaluationType.MN:
                    List<Guid> mDomains = CNController.get_member_nodes(paramsContainer.Tenant.Id,
                        paramsContainer.CurrentUserID.Value).Select(u => u.Node.NodeID.Value).ToList();

                    foreach (KnowledgeTypeQuestion q in questions)
                    {
                        if (!q.RelatedNode.NodeID.HasValue || mDomains.Any(u => u == q.RelatedNode.NodeID.Value))
                            validQuestions.Add(q);
                    }
                    break;
                default:
                    validQuestions = questions;
                    break;
            }
            
            List<AnswerOption> options = KnowledgeController.get_answer_options(paramsContainer.Tenant.Id,
                validQuestions.Select(u => u.ID.Value).ToList());

            validQuestions.ForEach(u => u.Options = options.Where(x => x.TypeQuestionID == u.ID).ToList());
            
            responseText = "{\"ScoreScale\":" + (kType.ScoreScale.HasValue ? kType.ScoreScale.Value : 10).ToString() +
                ",\"Questions\":[" + string.Join(",", validQuestions.Select(u => u.toJson())) + "]}";
        }

        protected void get_filled_evaluation_form(Guid? knowledgeId, Guid? userId, int? wfVersionId, ref string responseText)
        {
            if (!paramsContainer.GBEdit) return;

            List<EvaluationAnswer> answers = !userId.HasValue || !knowledgeId.HasValue ? new List<EvaluationAnswer>() :
                KnowledgeController.get_filled_evaluation_form(paramsContainer.Tenant.Id, knowledgeId.Value, userId.Value, wfVersionId);

            DateTime? evaluationDate = null;
            if (answers.Any(u => u.EvaluationDate.HasValue))
                evaluationDate = answers.Where(x => x.EvaluationDate.HasValue).Select(y => y.EvaluationDate.Value)
                    .OrderByDescending(a => a).FirstOrDefault();

            KWFHistory hist = !knowledgeId.HasValue ? null : KnowledgeController.get_history(paramsContainer.Tenant.Id,
                knowledgeId.Value, userId, "Evaluation", wfVersionId: wfVersionId).FirstOrDefault();

            responseText = "{\"EvaluationDate\":\"" + (!evaluationDate.HasValue ? string.Empty :
                    GenericDate.get_local_date(evaluationDate.Value)) + "\"" +
                ",\"TextOptions\":[" + (hist == null ? string.Empty : 
                    string.Join(",", hist.TextOptions.Select(u => "\"" + Base64.encode(u) + "\""))) + "]" +
                ",\"Description\":\"" + (hist == null ? string.Empty : Base64.encode(hist.Description)) + "\"" +
                ",\"Answers\":[" + string.Join(",", answers.Select(
                u => "{\"QuestionID\":\"" + u.QuestionID.ToString() + "\"" +
                    ",\"Title\":\"" + Base64.encode(u.Title) + "\"" +
                    ",\"TextValue\":\"" + Base64.encode(u.TextValue) + "\"" +
                    ",\"Score\":" + (!u.Score.HasValue ? 0 : u.Score.Value).ToString() +
                "}")) + "]}";
        }

        public Guid? get_main_admin(Node nodeObject,
            List<Guid> contributors, List<HierarchyAdmin> admins)
        {
            Guid? mainAdmin = null;
            Guid creatorUserId = nodeObject.Creator.UserID.Value;

            if (admins.Count > 0)
            {
                List<Guid> creatorIds = contributors;
                if (!contributors.Any(u => u == creatorUserId)) creatorIds.Add(creatorUserId);

                for (int i = 0, lnt = admins.Count; i < lnt; ++i)
                    if (!admins[i].Level.HasValue) admins[i].Level = 0;

                int min = admins.Min(u => u.Level.Value);
                int secondMin = !admins.Any(x => x.Level.Value != min) ? -1 :
                    admins.Where(x => x.Level.Value != min).Min(u => u.Level.Value);

                List<Guid> firstLevelAdminIds =
                    admins.Where(u => u.Level == min).Select(x => x.User.UserID.Value).ToList();
                List<Guid> secondLevelAdminIds = secondMin < min ? new List<Guid>() :
                    admins.Where(u => u.Level == secondMin).Select(x => x.User.UserID.Value).ToList();

                bool isCreatorInFirstLevel = firstLevelAdminIds.Any(u => creatorIds.Any(x => x == u));
                List<Guid> firstLevelNotCreatorIds = firstLevelAdminIds.Where(u => !creatorIds.Any(x => x == u)).ToList();

                if (isCreatorInFirstLevel && secondLevelAdminIds.Count > 0)
                    mainAdmin = secondLevelAdminIds[PublicMethods.get_random_number(0, secondLevelAdminIds.Count - 1)];
                else if (isCreatorInFirstLevel && firstLevelNotCreatorIds.Count > 0)
                    mainAdmin = firstLevelNotCreatorIds[PublicMethods.get_random_number(0, firstLevelNotCreatorIds.Count - 1)];
                else
                    mainAdmin = firstLevelAdminIds[PublicMethods.get_random_number(0, firstLevelAdminIds.Count - 1)];
            }

            return mainAdmin;
        }

        protected void workflow_action(string strNodeTypeId, string strNodeId, Guid? userId, ActionType action,
            string description, long? replyToHistoryId, ref string responseText, List<string> textOptions = null)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (textOptions == null) textOptions = new List<string>();

            Guid? nodeId = PublicMethods.parse_guid(strNodeId);

            if (!nodeId.HasValue && !string.IsNullOrEmpty(strNodeTypeId) && !string.IsNullOrEmpty(strNodeId))
            {
                Guid? nodeTypeId = CNController.get_node_type_id(paramsContainer.Tenant.Id, strNodeTypeId);
                if (nodeTypeId.HasValue)
                    nodeId = CNController.get_node_id(paramsContainer.Tenant.Id, strNodeId, nodeTypeId.Value);
            }

            if (!nodeId.HasValue)
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }

            if (!userId.HasValue) userId = paramsContainer.CurrentUserID;

            Node nodeObject = CNController.get_node(paramsContainer.Tenant.Id, nodeId.Value);

            if (nodeObject == null)
            {
                responseText = "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
                return;
            }

            Guid creatorUserId = nodeObject.Creator.UserID.Value;
            List<Guid> contributors = CNController.get_node_creators(paramsContainer.Tenant.Id,
                nodeId.Value).Select(u => u.User.UserID.Value).ToList();

            Service service = CNController.get_service(paramsContainer.Tenant.Id, nodeId.Value);

            List<HierarchyAdmin> admins =
                CNController.get_node_admins(paramsContainer.Tenant.Id, nodeId.Value, nodeObject, service);

            List<Dashboard> curAdmins = NotificationController.get_dashboards(paramsContainer.Tenant.Id, userId: null, 
                nodeTypeId: null, nodeId: nodeId.Value, nodeAdditionalId: null, dashboardType: DashboardType.Knowledge,
                subType: DashboardSubType.Admin, subTypeTitle: null, done: false, dateFrom: null, dateTo: null,
                searchText: null, lowerBoundary: null, count: 10);

            if (curAdmins != null)
            {
                curAdmins.Where(a => !admins.Any(itm => itm.User.UserID == a.UserID)).ToList()
                    .ForEach(a => admins.Add(new HierarchyAdmin() { User = new User() { UserID = a.UserID }, Level = 0 }));
            }

            if (admins == null || admins.Count == 0)
            {
                responseText = "{\"ErrorText\":\"" + Messages.CannotFindAdmins + "\"}";
                return;
            }

            List<Guid> adminIds = admins.Select(u => u.User.UserID.Value).ToList();

            Guid? mainAdmin = get_main_admin(nodeObject, contributors, admins);

            bool adminLevelAccess = admins.Any(u => u.User.UserID == paramsContainer.CurrentUserID.Value) ||
                PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) ||
                CNController.is_service_admin(paramsContainer.Tenant.Id,
                nodeObject.NodeTypeID.Value, paramsContainer.CurrentUserID.Value);
            bool isCreator = contributors.Any(u => u == paramsContainer.CurrentUserID.Value) ||
                nodeObject.Creator.UserID == paramsContainer.CurrentUserID.Value;

            bool result = false;
            bool accepted = false;
            bool searchabilityActivated = false;

            Modules.Log.Action logAction = Modules.Log.Action.None;
            Modules.Log.Action failLogAction = Modules.Log.Action.None;
            List<Dashboard> sentDashboards = new List<Dashboard>();

            string message = string.Empty;
            Status newStatus = Status.NotSet;

            //Check Necessary Items
            string strNecessaryItemsResponse = string.Empty;

            if (action == ActionType.Accept ||
                action == ActionType.SendToAdmin)
            {
                List<NecessaryItem> necessaryItems =
                    KnowledgeController.get_necessary_items(paramsContainer.Tenant.Id, nodeObject.NodeTypeID.Value);

                if (necessaryItems.Any(u => u == NecessaryItem.Wiki) &&
                    !CNController.has_extension(paramsContainer.Tenant.Id, nodeObject.NodeTypeID.Value, ExtensionType.Wiki))
                    necessaryItems.Remove(NecessaryItem.Wiki);

                if (necessaryItems.Any(u => u == NecessaryItem.DocumentTree) &&
                    (!Modules.RaaiVanConfig.Modules.Documents(paramsContainer.Tenant.Id) ||
                    !CNController.is_document(paramsContainer.Tenant.Id, nodeObject.NodeTypeID.Value)))
                    necessaryItems.Remove(NecessaryItem.DocumentTree);

                if (necessaryItems.Any(u => u == NecessaryItem.NecessaryFieldsOfForm) &&
                    (!Modules.RaaiVanConfig.Modules.FormGenerator(paramsContainer.Tenant.Id) ||
                    !CNController.has_extension(paramsContainer.Tenant.Id, nodeObject.NodeTypeID.Value, ExtensionType.Form)))
                    necessaryItems.Remove(NecessaryItem.NecessaryFieldsOfForm);

                List<NecessaryItem> existingNecessaryItems =
                    KnowledgeController.check_necessary_items(paramsContainer.Tenant.Id, nodeId.Value);

                List<NecessaryItem> notExisting =
                    necessaryItems.Where(u => !existingNecessaryItems.Any(x => x == u)).Select(u => u).ToList();

                if (notExisting.Count > 0)
                {
                    strNecessaryItemsResponse = ",\"NecessaryItems\":[" +
                        ProviderUtil.list_to_string<string>(notExisting.Select(u => "\"" + u.ToString() + "\"").ToList()) +
                        "]";
                }
            }
            //end of Check Necessary Items

            KnowledgeType kType = KnowledgeController.get_knowledge_type(paramsContainer.Tenant.Id, nodeId.Value);

            if (kType == null)
            {
                responseText = "{\"ErrorText\":\"" + Messages.KnowledgeTypeSettingsNotFound + "\"}";
                return;
            }

            if (kType.PreEvaluateByOwner.HasValue && kType.PreEvaluateByOwner.Value && action == ActionType.SendToAdmin &&
                nodeObject.Creator.UserID.HasValue && nodeObject.Creator.UserID == paramsContainer.CurrentUserID &&
                !KnowledgeController.has_evaluated(paramsContainer.Tenant.Id, nodeId.Value, paramsContainer.CurrentUserID.Value))
            {
                responseText = "{\"ShowEvaluationForm\":" + true.ToString().ToLower() + "}";
                return;
            }

            bool accessDenied = false;
            bool invalidRequest = false;

            switch (action)
            {
                case ActionType.Accept:
                    bool isFreeUser = CNController.is_free_user(paramsContainer.Tenant.Id,
                        nodeObject.NodeTypeID.Value, paramsContainer.CurrentUserID.Value);

                    invalidRequest = !adminLevelAccess &&(nodeObject.Status != Status.SentToAdmin) && 
                        !(isFreeUser && nodeObject.isPersonal(paramsContainer.CurrentUserID.Value));

                    if (invalidRequest) break;
                    else if (!adminLevelAccess && !isFreeUser)
                    {
                        accessDenied = true;
                        break;
                    }

                    logAction = Modules.Log.Action.AcceptKnowledge;
                    failLogAction = Modules.Log.Action.AcceptKnowledge_PermissionFailed;

                    result = !string.IsNullOrEmpty(strNecessaryItemsResponse) ? false :
                        KnowledgeController.accept_knowledge(paramsContainer.Tenant.Id,
                        nodeId.Value, paramsContainer.CurrentUserID.Value, textOptions, description);

                    if (result) newStatus = Status.Accepted;
                    break;
                case ActionType.Reject:
                    invalidRequest = nodeObject.Status != Status.SentToAdmin;

                    if (invalidRequest) break;
                    else if (!adminLevelAccess)
                    {
                        accessDenied = true;
                        break;
                    }

                    logAction = Modules.Log.Action.RejectKnowledge;
                    failLogAction = Modules.Log.Action.RejectKnowledge_PermissionFailed;

                    result = KnowledgeController.reject_knowledge(paramsContainer.Tenant.Id,
                        nodeId.Value, paramsContainer.CurrentUserID.Value, textOptions, description);
                    if (result) newStatus = Status.Rejected;
                    break;
                case ActionType.SendToAdmin:
                    {
                        invalidRequest = !(new List<Status>() { Status.NotSet, Status.Personal, Status.Rejected, Status.SentBackForRevision })
                            .Any(u => u == nodeObject.Status);

                        if (invalidRequest) break;
                        else if (!adminLevelAccess && !isCreator)
                        {
                            accessDenied = true;
                            break;
                        }

                        logAction = Modules.Log.Action.SendKnowledgeToAdmin;
                        failLogAction = Modules.Log.Action.SendKnowledgeToAdmin_PermissionFailed;

                        result = !string.IsNullOrEmpty(strNecessaryItemsResponse) ? false :
                            (mainAdmin.HasValue ?
                            KnowledgeController.send_to_admin(paramsContainer.Tenant.Id, nodeId.Value, mainAdmin.Value,
                                paramsContainer.CurrentUserID.Value, description, ref sentDashboards, ref message) :
                            KnowledgeController.send_to_admin(paramsContainer.Tenant.Id, nodeId.Value, adminIds,
                                paramsContainer.CurrentUserID.Value, description, ref sentDashboards, ref message));

                        if (result) newStatus = Status.SentToAdmin;

                        if (result)
                        { //Update related nodes, because it is needed in the evaluation process
                            PublicMethods.set_timeout(() =>
                            {
                                CNController.update_form_and_wiki_tags(paramsContainer.Tenant.Id,
                                    nodeId.Value, paramsContainer.CurrentUserID.Value);
                            }, 0);
                        }
                    }
                    break;
                case ActionType.SendBackForRevision:
                    invalidRequest = nodeObject.Status != Status.SentToAdmin;

                    if (invalidRequest) break;
                    else if (!adminLevelAccess)
                    {
                        accessDenied = true;
                        break;
                    }

                    logAction = Modules.Log.Action.SendKnowledgeBackForRevision;
                    failLogAction = Modules.Log.Action.SendKnowledgeBackForRevision_PermissionFailed;

                    result = KnowledgeController.send_back_for_revision(paramsContainer.Tenant.Id,
                        nodeId.Value, paramsContainer.CurrentUserID.Value, textOptions, description, ref sentDashboards, ref message);
                    if (result) newStatus = Status.SentBackForRevision;
                    break;
                case ActionType.SendToEvaluators:
                    invalidRequest = !(new List<Status>() { Status.SentToAdmin, Status.SentToEvaluators })
                            .Any(u => u == nodeObject.Status);

                    if (invalidRequest) break;
                    else if (!adminLevelAccess)
                    {
                        accessDenied = true;
                        break;
                    }

                    logAction = Modules.Log.Action.SendKnowledgeToEvaluators;
                    failLogAction = Modules.Log.Action.SendKnowledgeToEvaluators_PermissionFailed;

                    List<Guid> evaluatorUserIds = kType == null || kType.Evaluators != KnowledgeEvaluators.KnowledgeAdmin ?
                        ListMaker.get_guid_items(HttpContext.Current.Request.Params["UserIDs"], '|') : adminIds;
                    result = KnowledgeController.send_to_evaluators(paramsContainer.Tenant.Id, nodeId.Value,
                        evaluatorUserIds, paramsContainer.CurrentUserID.Value, description, ref sentDashboards, ref message);
                    if (result) newStatus = Status.SentToEvaluators;
                    break;
                case ActionType.Comment:
                    if (string.IsNullOrEmpty(description)) break;

                    invalidRequest = nodeObject.Status != Status.SentToEvaluators;
                    if (invalidRequest) break;
                    
                    if (!NotificationController.dashboard_exists(paramsContainer.Tenant.Id,
                        paramsContainer.CurrentUserID.Value, nodeId,
                        DashboardType.Knowledge, DashboardSubType.Evaluator, null, false) && 
                        !adminLevelAccess && !isCreator)
                    {
                        accessDenied = true;
                        break;
                    }
                    
                    logAction = Modules.Log.Action.KnowledgeComment;
                    failLogAction = Modules.Log.Action.KnowledgeComment_PermissionFailed;

                    //Comment Audience: notify all of the admins and evaluators
                    List<Guid> commentAudienceIds = adminIds;

                    if(nodeObject.Creator.UserID.HasValue) commentAudienceIds.Add(nodeObject.Creator.UserID.Value);

                    commentAudienceIds.AddRange(NotificationController.get_dashboards(paramsContainer.Tenant.Id,
                        null, null, nodeId.Value, null, DashboardType.Knowledge, DashboardSubType.Evaluator,
                        null, null, null, null, null, null, 1000).Select(u => u.UserID.Value));

                    commentAudienceIds = commentAudienceIds.Where(u => u != paramsContainer.CurrentUserID)
                        .Distinct().ToList();
                    //end of 'Comment Audience: notify all of the admins and evaluators'

                    result = KnowledgeController.send_knowledge_comment(paramsContainer.Tenant.Id, 
                        nodeId.Value, userId.Value, replyToHistoryId, commentAudienceIds, description, ref sentDashboards);

                    if (result) newStatus = Status.SentToEvaluators;
                    break;
                case ActionType.Evaluation:
                    if (kType.ForceEvaluatorsDescribe.HasValue && kType.ForceEvaluatorsDescribe.Value &&
                        string.IsNullOrEmpty(description)) break;

                    bool isPreEvaluation = kType.PreEvaluateByOwner.HasValue && kType.PreEvaluateByOwner.Value &&
                        nodeObject.Creator.UserID.HasValue && nodeObject.Creator.UserID == paramsContainer.CurrentUserID &&
                        (nodeObject.Status == Status.NotSet || nodeObject.Status == Status.Personal ||
                        nodeObject.Status == Status.Rejected);
                    
                    if (!isPreEvaluation && !NotificationController.dashboard_exists(paramsContainer.Tenant.Id,
                        paramsContainer.CurrentUserID.Value, nodeId,
                        DashboardType.Knowledge, DashboardSubType.Evaluator, null, null))
                    {
                        accessDenied = true;
                        break;
                    }

                    logAction = Modules.Log.Action.KnowledgeEvaluation;
                    failLogAction = Modules.Log.Action.KnowledgeEvaluation_PermissionFailed;

                    string[] strAnswers = string.IsNullOrEmpty(HttpContext.Current.Request.Params["Answers"]) ?
                        new string[0] : HttpContext.Current.Request.Params["Answers"].Split('|');
                    List<KeyValuePair<Guid, double>> answers = new List<KeyValuePair<Guid, double>>();
                    foreach (string str in strAnswers)
                    {
                        string[] ans = str.Split(':');
                        answers.Add(new KeyValuePair<Guid, double>(Guid.Parse(ans[0]), double.Parse(ans[1])));
                    }

                    double score = string.IsNullOrEmpty(HttpContext.Current.Request.Params["Score"]) ? 0 : 
                        double.Parse(HttpContext.Current.Request.Params["Score"]);

                    string strStatus = string.Empty;

                    result = userId.HasValue && (mainAdmin.HasValue ?
                        KnowledgeController.save_evaluation_form(paramsContainer.Tenant.Id,
                            nodeId.Value, userId.Value, answers, paramsContainer.CurrentUserID.Value, score,
                            mainAdmin.Value, textOptions, description, ref sentDashboards, ref strStatus, ref searchabilityActivated) :
                        KnowledgeController.save_evaluation_form(paramsContainer.Tenant.Id,
                            nodeId.Value, userId.Value, answers, paramsContainer.CurrentUserID.Value, score,
                            adminIds, textOptions, description, ref sentDashboards, ref strStatus, ref searchabilityActivated));

                    Status tmpStatus = Status.NotSet;
                    Enum.TryParse<Status>(strStatus, out tmpStatus);

                    if (result && !isPreEvaluation)
                        newStatus = tmpStatus == Status.NotSet ? Status.SentToEvaluators : tmpStatus;
                    break;
                case ActionType.RemoveEvaluator:
                    invalidRequest = nodeObject.Status != Status.SentToEvaluators;

                    if (invalidRequest) break;
                    else if (!adminLevelAccess)
                    {
                        accessDenied = true;
                        break;
                    }

                    logAction = Modules.Log.Action.RemoveKnowledgeEvaluator;
                    failLogAction = Modules.Log.Action.RemoveKnowledgeEvaluator_PermissionFailed;

                    result = nodeId.HasValue && userId.HasValue &&
                        KnowledgeController.remove_evaluator(paramsContainer.Tenant.Id, nodeId.Value, userId.Value);
                    if (result) newStatus = Status.SentToEvaluators;
                    break;
                case ActionType.RefuseEvaluation:
                    invalidRequest = nodeObject.Status != Status.SentToEvaluators;

                    if (invalidRequest) break;
                    else if (!NotificationController.dashboard_exists(paramsContainer.Tenant.Id,
                        paramsContainer.CurrentUserID.Value, nodeId,
                        DashboardType.Knowledge, DashboardSubType.Evaluator, null, false))
                    {
                        accessDenied = true;
                        break;
                    }

                    logAction = Modules.Log.Action.RefuseKnowledgeEvaluation;
                    failLogAction = Modules.Log.Action.RefuseKnowledgeEvaluation_PermissionFailed;

                    result = mainAdmin.HasValue ?
                        KnowledgeController.refuse_evaluation(paramsContainer.Tenant.Id,
                        nodeId.Value, paramsContainer.CurrentUserID.Value, mainAdmin.Value, description, ref sentDashboards) :
                        KnowledgeController.refuse_evaluation(paramsContainer.Tenant.Id,
                        nodeId.Value, paramsContainer.CurrentUserID.Value, adminIds, description, ref sentDashboards);

                    if (result) newStatus = Status.SentToEvaluators;
                    break;
                case ActionType.TerminateEvaluation:
                    invalidRequest = nodeObject.Status != Status.SentToEvaluators;

                    if (invalidRequest) break;
                    else if (!adminLevelAccess)
                    {
                        accessDenied = true;
                        break;
                    }

                    logAction = Modules.Log.Action.TerminateKnowledgeEvaluation;
                    failLogAction = Modules.Log.Action.TerminateKnowledgeEvaluation_PermissionFailed;

                    result = KnowledgeController.terminate_evaluation(paramsContainer.Tenant.Id,
                        nodeId.Value, paramsContainer.CurrentUserID.Value, description,
                        ref accepted, ref searchabilityActivated);
                    if (result) newStatus = accepted ? Status.Accepted : Status.Rejected;
                    break;
            }

            if (invalidRequest) {
                responseText = "{\"ErrorText\":\"" + Messages.InvalidRequest.ToString() + "\"}";
                return;
            }
            else if (accessDenied)
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied.ToString() + "\"}";
                _save_error_log(failLogAction, nodeId);
                return;
            }

            if (result && newStatus == Status.Accepted && kType != null &&
                kType.ConvertEvaluatorsToExperts.HasValue && kType.ConvertEvaluatorsToExperts.Value)
            {
                int? wfVersionId = KnowledgeController.get_last_history_version_id(paramsContainer.Tenant.Id, nodeId.Value);

                List<Guid> evaluatorIds = KnowledgeController.get_evaluations_done(paramsContainer.Tenant.Id,
                    nodeId.Value, wfVersionId).Select(u => u.Evaluator.UserID.Value).Distinct().ToList();

                CNController.add_experts(paramsContainer.Tenant.Id, nodeId.Value, evaluatorIds);
            }

            //Update previous or first version id or version counter part of AdditionalID for the published item
            if (result && searchabilityActivated)
                CNAPI.update_additional_id(paramsContainer.Tenant.Id, nodeId.Value, paramsContainer.CurrentUserID.Value);

            //Unpublish the previous version
            if (result && newStatus == Status.Accepted)
            {
                List<Node> previousVersions = CNController.get_previous_versions(paramsContainer.Tenant.Id, nodeObject.NodeID.Value);

                if (previousVersions.Count > 0)
                    CNController.set_nodes_searchability(paramsContainer.Tenant.Id, 
                        previousVersions.Select(p => p.NodeID.Value).ToList(), false, paramsContainer.CurrentUserID.Value);
            }

            responseText = !result ?
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"" + strNecessaryItemsResponse + "}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"" +
                ",\"Status\":\"" + (newStatus == Status.NotSet ? string.Empty : newStatus.ToString()) + "\"" +
                (searchabilityActivated ? ",\"Searchable\":" + true.ToString().ToLower() : string.Empty) +
                (accepted ? ",\"Publicated\":" + true.ToString().ToLower() : string.Empty) +
                ",\"IsEvaluator\":" + (newStatus == Status.SentToEvaluators &&
                    NotificationController.dashboard_exists(paramsContainer.Tenant.Id,
                    paramsContainer.CurrentUserID.Value, nodeId, DashboardType.Knowledge, DashboardSubType.Evaluator, seen: null, done: null)).ToString().ToLower() +
                ",\"HasEvaluated\":" + (newStatus == Status.SentToEvaluators &&
                    NotificationController.dashboard_exists(paramsContainer.Tenant.Id,
                    paramsContainer.CurrentUserID.Value, nodeId, DashboardType.Knowledge, DashboardSubType.Evaluator, seen: null, done: true)).ToString().ToLower() +
                "}";

            if (result && sentDashboards.Count > 0)
            {
                NotificationController.transfer_dashboards(paramsContainer.Tenant.Id, sentDashboards);
                Modules.Jobs.Notifier.send_dashboards(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID, sentDashboards);
            }

            //Send Notification
            if (result && newStatus == Status.Accepted)
            {
                Notification not = new Notification()
                {
                    SubjectID = nodeId,
                    RefItemID = nodeId,
                    SubjectType = SubjectType.Node,
                    SubjectName = nodeObject.Name,
                    Description = nodeObject.Name,
                    Action = ActionType.Accept
                };
                
                not.Sender.UserID = nodeObject.Creator.UserID;
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
                    Action = logAction,
                    SubjectID = nodeId,
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        protected void edit_knowledge_comment(long? id, string commentBody, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            KWFHistory hist = !id.HasValue ? null : KnowledgeController.get_history(paramsContainer.Tenant.Id, id.Value);

            Node knowledge = hist == null || !hist.KnowledgeID.HasValue ? null :
                CNController.get_node(paramsContainer.Tenant.Id, hist.KnowledgeID.Value);

            if (knowledge == null || hist.Action != "Comment" || knowledge.Status != Status.SentToEvaluators || 
                hist.Actor.UserID != paramsContainer.CurrentUserID.Value) {
                responseText = "{\"ErrorText\":\"" + Messages.InvalidRequest + "\"}";
                return;
            }

            bool result = KnowledgeController.edit_history_description(paramsContainer.Tenant.Id, id.Value, commentBody);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void new_evaluators(Guid? nodeId, List<Guid> userIds, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!nodeId.HasValue || (
                !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) &&
                !CNController.is_service_admin(paramsContainer.Tenant.Id, nodeId.Value, paramsContainer.CurrentUserID.Value) &&
                !CNController.is_node_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, nodeId.Value, null, null, null)))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied.ToString() + "\"}";
                _save_error_log(Modules.Log.Action.SendKnowledgeToEvaluators_PermissionFailed, nodeId);
                return;
            }

            List<Dashboard> sentDashboards = new List<Dashboard>();
            string message = string.Empty;

            bool result = KnowledgeController.new_evaluators(paramsContainer.Tenant.Id,
                nodeId.Value, userIds, ref sentDashboards, ref message);

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
                    Action = Modules.Log.Action.SendKnowledgeToEvaluators,
                    SubjectID = nodeId,
                    Info = "{\"UserIDs\":[" + ProviderUtil.list_to_string<string>(userIds.Select(u => "\"" + u + "\"").ToList()) + "]}",
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        protected void edit_history_description(long? id, string description, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            KWFHistory hist = !id.HasValue ? null : KnowledgeController.get_history(paramsContainer.Tenant.Id, id.Value);

            if (hist == null || !hist.KnowledgeID.HasValue || (
                !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) &&
                !CNController.is_service_admin(paramsContainer.Tenant.Id, hist.KnowledgeID.Value, paramsContainer.CurrentUserID.Value) &&
                !CNController.is_node_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, hist.KnowledgeID.Value, null, null, null)))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied.ToString() + "\"}";
                return;
            }

            bool result = KnowledgeController.edit_history_description(paramsContainer.Tenant.Id, id.Value, description);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void get_history(Guid? knowledgeId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;
            
            if (!knowledgeId.HasValue || (
                !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) &&
                !CNController.is_service_admin(paramsContainer.Tenant.Id, knowledgeId.Value, paramsContainer.CurrentUserID.Value) &&
                !CNController.is_node_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, knowledgeId.Value, null, null, null) &&
                !CNController.is_node_creator(paramsContainer.Tenant.Id, knowledgeId.Value, paramsContainer.CurrentUserID.Value) &&
                !NotificationController.dashboard_exists(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value, knowledgeId.Value, DashboardType.Knowledge, DashboardSubType.Evaluator, null, null)))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied.ToString() + "\"}";
                return;
            }

            List<KWFHistory> historyTemp = KnowledgeController.get_history(paramsContainer.Tenant.Id, knowledgeId.Value);
            List<KnowledgeEvaluation> evaluations =
                KnowledgeController.get_evaluations_done(paramsContainer.Tenant.Id, knowledgeId.Value, wfVersionId: null);

            List<KWFHistory> history = new List<KWFHistory>();

            historyTemp.ForEach(h =>
            {
                bool isEvaluation = h.Action == "Evaluation";

                if (isEvaluation && history.Any(x => x.Action == "Evaluation" && x.Actor.UserID == h.Actor.UserID)) return;
                else history.Add(h);

                h.Sub = history.Where(x => h.ID.HasValue && x.ReplyToHistoryID == h.ID).ToList();

                if (isEvaluation)
                {
                    h.Evaluation = evaluations.Where(e => e.Evaluator.UserID.HasValue &&
                        e.Evaluator.UserID == h.Actor.UserID && e.WFVersionID == h.WFVersionID).FirstOrDefault();
                }
            });

            responseText = "[" + string.Join(",", 
                history.Where(h => !h.ReplyToHistoryID.HasValue).Select(u => u.toJson(paramsContainer.Tenant.Id))) + "]";
        }

        protected void add_feedback(Guid? knowledgeId, FeedBackTypes feedbackType, double? value,
            string description, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(description) && description.Length > 1900)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }

            if (!knowledgeId.HasValue || !PrivacyController.check_access(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, knowledgeId.Value, PrivacyObjectType.Node, PermissionType.View))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied.ToString() + "\"}";
                return;
            }

            bool isSystemAdmin = PublicMethods.is_system_admin(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value);

            FeedBack newFeedBack = new FeedBack();
            newFeedBack.KnowledgeID = knowledgeId;
            newFeedBack.User.UserID = paramsContainer.CurrentUserID;
            newFeedBack.FeedBackType = feedbackType;
            newFeedBack.SendDate = DateTime.Now;
            newFeedBack.Value = value;
            newFeedBack.Description = description;

            long result = KnowledgeController.add_feedback(paramsContainer.Tenant.Id, newFeedBack);

            string feedBackJson = "{}";
            if (result > 0)
            {
                newFeedBack.FeedBackID = result;
                FeedBack addedFeedBack = KnowledgeController.get_knowledge_feedback(paramsContainer.Tenant.Id, result);
                feedBackJson = _get_feedbacks_json_text((addedFeedBack == null ? newFeedBack : addedFeedBack), paramsContainer.CurrentUserID, isSystemAdmin);
            }

            responseText = result > 0 ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\",\"FeedBack\":" + feedBackJson + "}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void modify_feedback(long? feedbackId, double? value, string description, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!string.IsNullOrEmpty(description) && description.Length > 1900)
            {
                responseText = "{\"ErrorText\":\"" + Messages.MaxAllowedInputLengthExceeded + "\"}";
                return;
            }

            FeedBack feedback = !feedbackId.HasValue ? null :
                KnowledgeController.get_knowledge_feedback(paramsContainer.Tenant.Id, feedbackId.Value);

            if (feedback == null || (
                (feedback.User.UserID != paramsContainer.CurrentUserID.Value) &&
                !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) &&
                !CNController.is_service_admin(paramsContainer.Tenant.Id,
                feedback.KnowledgeID.Value, paramsContainer.CurrentUserID.Value) &&
                !CNController.is_node_admin(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, feedback.KnowledgeID.Value, null, null, null)))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied.ToString() + "\"}";
                return;
            }

            FeedBack newFeedBack = new FeedBack();
            newFeedBack.FeedBackID = feedbackId;
            newFeedBack.Value = value;
            newFeedBack.Description = description;

            bool result = KnowledgeController.modify_feedback(paramsContainer.Tenant.Id, newFeedBack);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected void remove_feedback(long? feedbackId, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            FeedBack feedback = !feedbackId.HasValue ? null :
                KnowledgeController.get_knowledge_feedback(paramsContainer.Tenant.Id, feedbackId.Value);

            if (feedback == null || (
                (feedback.User.UserID != paramsContainer.CurrentUserID.Value) &&
                !PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value) &&
                !CNController.is_service_admin(paramsContainer.Tenant.Id,
                feedback.KnowledgeID.Value, paramsContainer.CurrentUserID.Value) &&
                !CNController.is_node_admin(paramsContainer.Tenant.Id,
                paramsContainer.CurrentUserID.Value, feedback.KnowledgeID.Value, null, null, null)))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied.ToString() + "\"}";
                return;
            }

            bool result = feedbackId.HasValue &&
                KnowledgeController.remove_feedback(paramsContainer.Tenant.Id, feedbackId.Value);

            responseText = result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed + "\"}";
        }

        protected string _get_feedbacks_json_text(FeedBack feedback, Guid? currentUserId, bool isSystemAdmin)
        {
            if (currentUserId == Guid.Empty) currentUserId = null;

            bool mine = currentUserId.HasValue && feedback.User.UserID == currentUserId;

            return "{\"FeedBackID\":\"" + feedback.FeedBackID.ToString() + "\"" +
                ",\"KnowledgeID\":\"" + feedback.KnowledgeID.ToString() + "\"" +
                ",\"FeedBackType\":\"" + feedback.FeedBackType.ToString() + "\"" +
                ",\"SendDate\":\"" + GenericDate.get_local_date(feedback.SendDate.Value, true) + "\"" +
                ",\"GregorianSendDate\":\"" + feedback.SendDate.Value.ToString() + "\"" +
                ",\"Value\":\"" + feedback.Value.Value.ToString() + "\"" +
                ",\"Description\":\"" + Base64.encode(feedback.Description) + "\"" +
                ",\"UserID\":\"" + feedback.User.UserID.Value.ToString() + "\"" +
                ",\"UserName\":\"" + Base64.encode(feedback.User.UserName) + "\"" +
                ",\"FirstName\":\"" + Base64.encode(feedback.User.FirstName) + "\"" +
                ",\"LastName\":\"" + Base64.encode(feedback.User.LastName) + "\"" +
                ",\"Editable\":" + (isSystemAdmin || mine).ToString().ToLower() +
                ",\"Mine\":" + mine.ToString().ToLower() +
                ",\"ProfileImageURL\":\"" + DocumentUtilities.get_personal_image_address(paramsContainer.Tenant.Id,
                    feedback.User.UserID.Value) + "\"" +
                "}";
        }

        protected void get_knowledge_feedbacks(Guid? knowledgeId, Guid? userId, FeedBackTypes feedbackType, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBView) return;

            if (!knowledgeId.HasValue)
            {
                responseText = "{\"FeedBacks\":[]}";
                return;
            }

            bool isSystemAdmin = paramsContainer.IsAuthenticated &&
                PublicMethods.is_system_admin(paramsContainer.Tenant.Id, paramsContainer.CurrentUserID.Value);

            List<FeedBack> feedBacks = (!userId.HasValue || userId == Guid.Empty) ?
                KnowledgeController.get_knowledge_feedbacks(paramsContainer.Tenant.Id, knowledgeId.Value, feedbackType: feedbackType) :
                KnowledgeController.get_knowledge_feedbacks(paramsContainer.Tenant.Id,
                    knowledgeId.Value, userId: userId.Value, feedbackType: feedbackType);

            responseText = "{\"FeedBacks\":[";

            bool isFirst = true;
            foreach (FeedBack _feedback in feedBacks)
            {
                responseText += (isFirst ? string.Empty : ",") + _get_feedbacks_json_text(_feedback, paramsContainer.CurrentUserID, isSystemAdmin);
                isFirst = false;
            }

            responseText += "]}";
        }

        protected void activate_necessary_item(Guid? knowledgeTypeId, NecessaryItem itm, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!knowledgeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (knowledgeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.ActivateKnowledgeNecessaryItem_PermissionFailed, knowledgeTypeId);
                return;
            }

            bool result = KnowledgeController.activate_necessary_item(paramsContainer.Tenant.Id,
                knowledgeTypeId.Value, itm, paramsContainer.CurrentUserID.Value);

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
                    Action = Modules.Log.Action.ActivateKnowledgeNecessaryItem,
                    SubjectID = knowledgeTypeId,
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        protected void deactive_necessary_item(Guid? knowledgeTypeId, NecessaryItem itm, ref string responseText)
        {
            //Privacy Check: OK
            if (!paramsContainer.GBEdit) return;

            if (!knowledgeTypeId.HasValue ||
                !AuthorizationManager.has_right(AccessRoleName.KnowledgeAdmin, paramsContainer.CurrentUserID))
            {
                responseText = "{\"ErrorText\":\"" + Messages.AccessDenied + "\"}";
                if (knowledgeTypeId.HasValue)
                    _save_error_log(Modules.Log.Action.DeactiveKnowledgeNecessaryItem_PermissionFailed, knowledgeTypeId);
                return;
            }

            bool result = KnowledgeController.deactive_necessary_item(paramsContainer.Tenant.Id,
                knowledgeTypeId.Value, itm, paramsContainer.CurrentUserID.Value);

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
                    Action = Modules.Log.Action.DeactiveKnowledgeNecessaryItem,
                    SubjectID = knowledgeTypeId,
                    ModuleIdentifier = ModuleIdentifier.KW
                });
            }
            //end of Save Log
        }

        //Jobs
        protected void _check_expired_knowledges(Guid applicationId)
        {
            CNController.set_expired_nodes_as_not_searchable(applicationId);

            List<Guid> nodeIds = CNController.get_node_ids_that_will_be_expired_soon(applicationId);

            User systemUser = UserUtilities.SystemUser(applicationId);

            if (systemUser == null || !systemUser.UserID.HasValue) return;

            List<Dashboard> dash = new List<Dashboard>();

            foreach (Guid id in nodeIds)
            {
                Node node = CNController.get_node(applicationId, id);

                List<Guid> contributors = CNController.get_node_creators(applicationId, id)
                    .Select(u => u.User.UserID.Value).ToList();

                Service service = CNController.get_service(applicationId, id);

                List<HierarchyAdmin> admins = CNController.get_node_admins(applicationId, id, node, service);

                Guid? mainAdmin = get_main_admin(node, contributors, admins);

                if (mainAdmin.HasValue)
                    CNController.notify_node_expiration(applicationId, id, systemUser.UserID.Value, ref dash);
            }
        }

        public void check_expired_knowledges(object rvThread)
        {
            RVJob trd = (RVJob)rvThread;

            if (!trd.TenantID.HasValue || !Modules.RaaiVanConfig.Modules.Knowledge(trd.TenantID.Value)) return;

            if (!trd.StartTime.HasValue) trd.StartTime = new DateTime(2000, 1, 1, 0, 0, 0);
            if (!trd.EndTime.HasValue) trd.EndTime = new DateTime(2000, 1, 1, 23, 59, 59);

            while (true)
            {
                if (!trd.Interval.HasValue) trd.Interval = 86400000; //86400000 Miliseconds Equals to 1 Day
                else System.Threading.Thread.Sleep(trd.Interval.Value);

                if (!trd.check_time()) continue;

                System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                sw.Start();

                _check_expired_knowledges(trd.TenantID.Value);

                trd.LastActivityDate = DateTime.Now;

                sw.Stop();
                trd.LastActivityDuration = sw.ElapsedMilliseconds;
            }
        }
        //end of Jobs

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}