using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using RaaiVan.Modules.GlobalUtilities;

namespace RaaiVan.Modules.Log
{
    public class LogUtilities
    {
        public static List<Action> get_actions()
        {
            List<Action> actions = new List<Action>();

            Array arr = Enum.GetValues(typeof(Action));
            for (int i = 0, lnt = arr.Length; i < lnt; ++i)
                if ((Action)arr.GetValue(i) != Action.None) actions.Add((Action)arr.GetValue(i));

            return actions;
        }
    }

    public enum LogLevel
    {
        None,

        Debug,
        Info,
        Warn,
        Error,
        Fatal,
        Trace
    }

    //initial '_' means 'deleted'
    public enum Action
    {
        None,

        //////////--> RV <--//////////
        NotAuthorizedAnonymousRequest,
        PotentialCSRFAttack,
        PotentialReplayAttack,
        JobStarted,
        JobDone,
        CreateWorkspace,
        RenameWorkspace,
        RemoveWorkspace,
        CreateApplication,
        ModifyApplication,
        RemoveApplication,
        RecycleApplication,
        RemoveUserFromApplication,
        UnsubscribeFromApplication,
        RemoveUserFromWorkspace,
        AddSystemAdmin,
        RemoveSystemAdmin,
        //////////--> end of RV <--//////////

        //////////--> CN <--//////////
        Node_AccessDenied,
        PendingMembers_AccessDenied,
        AddNodeType,
        AddNodeType_PermissionFailed,
        RenameNodeType,
        RenameNodeType_PermissionFailed,
        ChangeNodeType,
        ChangeNodeType_PermissionFailed,
        SetNodeTypeAdditionalID,
        SetNodeTypeAdditionalID_PermissionFailed,
        SetNodeTypeAdditionalIDPattern,
        SetNodeTypeAdditionalIDPattern_PermissionFailed,
        MoveNodeType,
        MoveNodeType_PermissionFailed,
        RemoveNodeType,
        RemoveNodeType_PermissionFailed,
        RecoverNodeType,
        RecoverNodeType_PermissionFailed,
        AddRelationType,
        AddRelationType_PermissionFailed,
        ModifyRelationType,
        ModifyRelationType_PermissionFailed,
        RemoveRelationType,
        RemoveRelationType_PermissionFailed,
        AddNode,
        AddNode_PermissionFailed,
        ModifyNode,
        ModifyNode_PermissionFailed,
        SetDocumentTreeNodeID,
        SetDocumentTreeNodeID_PermissionFailed,
        ModifyNodeName,
        ModifyNodeName_PermissionFailed,
        ModifyNodeDescription,
        ModifyNodeDescription_PermissionFailed,
        ModifyNodePublicDescription,
        ModifyNodePublicDescription_PermissionFailed,
        SetNodeExpirationDate,
        SetNodeExpirationDate_PermissionFailed,
        SetPreviousVersion,
        SetPreviousVersion_PermissionFailed,
        ModifyNodeTags,
        ModifyNodeTags_PermissionFailed,
        SetNodeSearchability,
        SetNodeSearchability_PermissionFailed,
        LockNode,
        UnlockNode,
        RemoveNode,
        RemoveNode_PermissionFailed,
        RecycleNode,
        RecycleNode_PermissionFailed,
        SortNodeTypes,
        SortNodeTypes_PermissionFailed,
        SortNodes,
        SortNodes_PermissionFailed,
        SetNodeDirectParent,
        SetNodeDirectParent_PermissionFailed,
        AddNodeRelation,
        AddNodeRelation_PermissionFailed,
        SaveRelations,
        SaveRelations_PermissionFailed,
        RemoveNodeRelation,
        RemoveNodeRelation_PermissionFailed,
        AddComplex,
        AddComplex_PermissionFailed,
        ModifyComplex,
        ModifyComplex_PermissionFailed,
        RemoveComplex,
        RemoveComplex_PermissionFailed,
        AddNodeToComplex,
        AddNodeToComplex_PermissionFailed,
        RemoveComplexNode,
        RemoveComplexNode_PermissionFailed,
        AddComplexAdmin,
        AddComplexAdmin_PermissionFailed,
        RemoveComplexAdmin,
        RemoveComplexAdmin_PermissionFailed,
        LikeNode,
        UnlikeNode,
        AddNodeMember,
        AddNodeMember_PermissionFailed,
        RemoveNodeMember,
        RemoveNodeMember_PermissionFailed,
        AcceptNodeMember,
        AcceptNodeMember_PermissionFailed,
        SaveNodeMembers,
        SetUserAsNodeAdmin,
        SetUserAsNodeAdmin_PermissionFailed,
        RemoveNodeAdmin,
        RemoveNodeAdmin_PermissionFailed,
        AddExpert,
        AddExpert_PermissionFailed,
        RemoveExpert,
        RemoveExpert_PermissionFailed,
        EnableExtension,
        EnableExtension_PermissionFailed,
        DisableExtension,
        DisableExtension_PermissionFailed,
        SetExtensionTitle,
        SetExtensionTitle_PermissionFailed,
        MoveExtension,
        MoveExtension_PermissionFailed,
        SetServiceTitle,
        SetServiceTitle_PermissionFailed,
        SetServiceDescription,
        SetServiceDescription_PermissionFailed,
        SetServiceHelp,
        SetServiceHelp_PermissionFailed,
        SetServiceSuccessMessage,
        SetServiceSuccessMessage_PermissionFailed,
        SetServiceAdminType,
        SetServiceAdminType_PermissionFailed,
        SetServiceMaxAcceptableAdminLevel,
        SetServiceMaxAcceptableAdminLevel_PermissionFailed,
        SetContributionLimits,
        SetContributionLimits_PermissionFailed,
        SetServiceEnableContribution,
        SetServiceEnableContribution_PermissionFailed,
        SetServiceNoContent,
        SetServiceNoContent_PermissionFailed,
        SetServiceIsKnowledge,
        SetServiceIsKnowledge_PermissionFailed,
        SetServiceIsDocument,
        SetServiceIsDocument_PermissionFailed,
        SetServiceIsCommunityPage,
        SetServiceEnableComments,
        SetServiceEnablePreviousVersionSelect,
        SetServiceEnablePreviousVersionSelect_PermissionFailed,
        SetServiceIsTree,
        SetServiceIsTree_PermissionFailed,
        SetServiceUniqueMembership,
        SetServiceUniqueMembership_PermissionFailed,
        SetServiceUniqueAdminMember,
        SetServiceUniqueAdminMember_PermissionFailed,
        SetServiceEditableForAdmin,
        SetServiceEditableForAdmin_PermissionFailed,
        SetServiceEditableForCreator,
        SetServiceEditableForCreator_PermissionFailed,
        SetServiceEditableForOwners,
        SetServiceEditableForOwners_PermissionFailed,
        SetServiceEditableForExperts,
        SetServiceEditableForExperts_PermissionFailed,
        SetServiceEditableForMembers,
        SetServiceEditableForMembers_PermissionFailed,
        SetServiceEditSuggestion,
        SetServiceEditSuggestion_PermissionFailed,
        AddServiceFreeUser,
        AddServiceFreeUser_PermissionFailed,
        RemoveServiceFreeUser,
        RemoveServiceFreeUser_PermissionFailed,
        AddServiceAdmin,
        AddServiceAdmin_PermissionFailed,
        RemoveServiceAdmin,
        RemoveServiceAdmin_PermissionFailed,
        SetAdminArea,
        SetAdminArea_PermissionFailed,
        SetContributors,
        SetContributors_PermissionFailed,
        //////////--> end of CN <--//////////

        //////////--> DCT <--//////////
        CreateDocumentTree,
        CreateDocumentTree_PermissionFailed,
        ModifyDocumentTree,
        ModifyDocumentTree_PermissionFailed,
        RemoveDocumentTree,
        RemoveDocumentTree_PermissionFailed,
        RecycleDocumentTree,
        RecycleDocumentTree_PermissionFailed,
        CreateDocumentTreeNode,
        CreateDocumentTreeNode_PermissionFailed,
        ModifyDocumentTreeNodeName,
        ModifyDocumentTreeNodeName_PermissionFailed,
        MoveDocumentTreeNode,
        MoveDocumentTreeNode_PermissionFailed,
        RemoveDocumentTreeNode,
        RemoveDocumentTreeNode_PermissionFailed,
        SortDocumentTreeNodes,
        SortDocumentTreeNodes_PermissionFailed,
        MoveDocuments,
        MoveDocuments_PermissionFailed,
        Download,
        Download_AccessDenied,
        AddOwnerTree,
        AddOwnerTree_PermissionFailed,
        RemoveOwnerTree,
        RemoveOwnerTree_PermissionFailed,
        AddTreeNodeContents,
        AddTreeNodeContents_PermissionFailed,
        RemoveTreeNodeContents,
        RemoveTreeNodeContents_PermissionFailed,
        //////////--> end of DCT <--//////////

        //////////--> EVT <--//////////
        RegisterNewEvent,
        RemoveEventUser,
        ChangeEventUserStatus,
        //////////--> end of EVT <--//////////

        //////////--> FG <--//////////
        CreateForm,
        CreateForm_PermissionFailed,
        ModifyFormTitle,
        ModifyFormTitle_PermissionFailed,
        ModifyFormName,
        ModifyFormName_PermissionFailed,
        ModifyFormDescription,
        ModifyFormDescription_PermissionFailed,
        RemoveForm,
        RemoveForm_PermissionFailed,
        RecycleForm,
        RecycleForm_PermissionFailed,
        AddFormElement,
        AddFormElement_PermissionFailed,
        ModifyFormElement,
        ModifyFormElement_PermissionFailed,
        MoveFormElementUp,
        MoveFormElementUp_PermissionFailed,
        MoveFormElementDown,
        MoveFormElementDown_PermissionFailed,
        SetFormElementNecessity,
        SetFormElementNecessity_PermissionFailed,
        SetFormElementUniqueness,
        SetFormElementUniqueness_PermissionFailed,
        RemoveFormElement,
        RemoveFormElement_PermissionFailed,
        SaveFormElements,
        SaveFormElements_PermissionFailed,
        CreateFormInstance,
        RemoveFormInstance,
        RemoveOwnerFormInstances,
        RecoverFormInstance,
        ModifyFormInstanceElements,
        SetFormInstanceAsFilled,
        SetFormInstanceAsNotFilled,
        SetFormOwner,
        RemoveFormOwner,
        SetElementLimits,
        SetElementLimitNecessity,
        RemoveElementLimit,
        AddPoll,
        AddPoll_PermissionFailed,
        RenamePoll,
        RenamePoll_PermissionFailed,
        RemovePoll,
        RemovePoll_PermissionFailed,
        RecyclePoll,
        RecyclePoll_PermissionFailed,
        SetPollDescription,
        SetPollDescription_PermissionFailed,
        SetPollBeginDate,
        SetPollBeginDate_PermissionFailed,
        SetPollFinishDate,
        SetPollFinishDate_PermissionFailed,
        SetPollShowSummary,
        SetPollShowSummary_PermissionFailed,
        SetPollHideContributors,
        SetPollHideContributors_PermissionFailed,
        //////////--> end of FG <--//////////

        //////////--> KW <--//////////
        AddKnowledgeType,
        AddKnowledgeType_PermissionFailed,
        RemoveKnowledgeType,
        RemoveKnowledgeType_PermissionFailed,
        SetKnowledgeTypeEvaluationType,
        SetKnowledgeTypeEvaluationType_PermissionFailed,
        SetKnowledgeTypeEvaluators,
        SetKnowledgeTypeEvaluators_PermissionFailed,
        SetKnowledgeTypePreEvaluateByOwner,
        SetKnowledgeTypePreEvaluateByOwner_PermissionFailed,
        SetKnowledgeTypeForceEvaluatorsDescribe,
        SetKnowledgeTypeForceEvaluatorsDescribe_PermissionFailed,
        SetKnowledgeTypeNodeSelectType,
        SetKnowledgeTypeNodeSelectType_PermissionFailed,
        SetKnowledgeTypeSearchabilityType,
        SetKnowledgeTypeSearchabilityType_PermissionFailed,
        SetKnowledgeTypeScoreScale,
        SetKnowledgeTypeScoreScale_PermissionFailed,
        SetKnowledgeTypeMinAcceptableScore,
        SetKnowledgeTypeMinAcceptableScore_PermissionFailed,
        SetKnowledgeTypeConvertEvaluatorsToExperts,
        SetKnowledgeTypeConvertEvaluatorsToExperts_PermissionFailed,
        SetKnowledgeTypeEvaluationsEditable,
        SetKnowledgeTypeEvaluationsEditable_PermissionFailed,
        SetKnowledgeTypeEvaluationsEditableForAdmin,
        SetKnowledgeTypeEvaluationsEditableForAdmin_PermissionFailed,
        SetKnowledgeTypeEvaluationsRemovable,
        SetKnowledgeTypeEvaluationsRemovable_PermissionFailed,
        SetKnowledgeTypeUnhideEvaluators,
        SetKnowledgeTypeUnhideEvaluators_PermissionFailed,
        SetKnowledgeTypeUnhideEvaluations,
        SetKnowledgeTypeUnhideEvaluations_PermissionFailed,
        SetKnowledgeTypeUnhideNodeCreators,
        SetKnowledgeTypeUnhideNodeCreators_PermissionFailed,
        SetKnowledgeTypeTextOptions,
        SetKnowledgeTypeTextOptions_PermissionFailed,
        SetKnowledgeTypeCandidateRelations,
        SetKnowledgeTypeCandidateRelations_PermissionFailed,
        AddKnowledgeTypeQuestion,
        AddKnowledgeTypeQuestion_PermissionFailed,
        ModifyKnowledgeTypeQuestion,
        ModifyKnowledgeTypeQuestion_PermissionFailed,
        RemoveKnowledgeTypeQuestion,
        RemoveKnowledgeTypeQuestion_PermissionFailed,
        RemoveKnowledgeTypeRelatedNodeQuestions,
        RemoveKnowledgeTypeRelatedNodeQuestions_PermissionFailed,
        AddKnowledgeTypeAnswerOption,
        AddKnowledgeTypeAnswerOption_PermissionFailed,
        ModifyKnowledgeTypeAnswerOption,
        ModifyKnowledgeTypeAnswerOption_PermissionFailed,
        RemoveKnowledgeTypeAnswerOption,
        RemoveKnowledgeTypeAnswerOption_PermissionFailed,
        SetKnowledgeTypeQuestionWeight,
        SetKnowledgeTypeQuestionWeight_PermissionFailed,
        AcceptKnowledge,
        AcceptKnowledge_PermissionFailed,
        RejectKnowledge,
        RejectKnowledge_PermissionFailed,
        SendKnowledgeToAdmin,
        SendKnowledgeToAdmin_PermissionFailed,
        SendKnowledgeBackForRevision,
        SendKnowledgeBackForRevision_PermissionFailed,
        SendKnowledgeToEvaluators,
        SendKnowledgeToEvaluators_PermissionFailed,
        KnowledgeComment,
        KnowledgeComment_PermissionFailed,
        KnowledgeEvaluation,
        KnowledgeEvaluation_PermissionFailed,
        RemoveKnowledgeEvaluator,
        RemoveKnowledgeEvaluator_PermissionFailed,
        RefuseKnowledgeEvaluation,
        RefuseKnowledgeEvaluation_PermissionFailed,
        TerminateKnowledgeEvaluation,
        TerminateKnowledgeEvaluation_PermissionFailed,
        ActivateKnowledgeNecessaryItem,
        ActivateKnowledgeNecessaryItem_PermissionFailed,
        DeactiveKnowledgeNecessaryItem,
        DeactiveKnowledgeNecessaryItem_PermissionFailed,
        //////////--> end of KW <--//////////

        //////////--> NTFN <--//////////
        SetMessageTemplate,
        RemoveMessageTemplate,
        SetDefaultMessageTemplate,
        //////////--> end of NTFN <--//////////

        //////////--> PRVC <--//////////
        SetPrivacyAudience,
        SetPrivacyAudience_PermissionFailed,
        AddConfidentialityLevel,
        AddConfidentialityLevel_PermissionFailed,
        ModifyConfidentialityLevel,
        ModifyConfidentialityLevel_PermissionFailed,
        RemoveConfidentialityLevel,
        RemoveConfidentialityLevel_PermissionFailed,
        SetConfidentialityLevel,
        SetConfidentialityLevel_PermissionFailed,
        UnsetConfidentialityLevel,
        UnsetConfidentialityLevel_PermissionFailed,
        //////////--> end of PRVC <--//////////

        //////////--> QA <--//////////
        AddQAWorkFlow,
        AddQAWorkFlow_PermissionFailed,
        RenameQAWorkFlow,
        RenameQAWorkFlow_PermissionFailed,
        SetQAWorkFlowDescription,
        SetQAWorkFlowDescription_PermissionFailed,
        SetQAWorkFlowsOrder_PermissionFailed,
        SetQAWorkFlowInitialCheckNeeded,
        SetQAWorkFlowInitialCheckNeeded_PermissionFailed,
        SetQAWorkFlowFinalConfirmationNeeded,
        SetQAWorkFlowFinalConfirmationNeeded_PermissionFailed,
        SetQAWorkFlowActionDeadline,
        SetQAWorkFlowActionDeadline_PermissionFailed,
        SetQAWorkFlowAnswerBy,
        SetQAWorkFlowAnswerBy_PermissionFailed,
        SetQAWorkFlowPublishAfter,
        SetQAWorkFlowPublishAfter_PermissionFailed,
        SetQAWorkFlowRemovableAfterConfirmation,
        SetQAWorkFlowRemovableAfterConfirmation_PermissionFailed,
        SetQAWorkFlowNodeSelectType,
        SetQAWorkFlowNodeSelectType_PermissionFailed,
        SetQAWorkFlowDisableComments,
        SetQAWorkFlowDisableComments_PermissionFailed,
        SetQAWorkFlowDisableQuestionLikes,
        SetQAWorkFlowDisableQuestionLikes_PermissionFailed,
        SetQAWorkFlowDisableAnswerLikes,
        SetQAWorkFlowDisableAnswerLikes_PermissionFailed,
        SetQAWorkFlowDisableCommentLikes,
        SetQAWorkFlowDisableCommentLikes_PermissionFailed,
        SetQAWorkFlowDisableBestAnswer,
        SetQAWorkFlowDisableBestAnswer_PermissionFailed,
        RemoveQAWorkFlow,
        RemoveQAWorkFlow_PermissionFailed,
        RecycleQAWorkFlow,
        RecycleQAWorkFlow_PermissionFailed,
        AddQAWorkFlowAdmin,
        AddQAWorkFlowAdmin_PermissionFailed,
        RemoveQAWorkFlowAdmin,
        RemoveQAWorkFlowAdmin_PermissionFailed,
        SetQAWorkFlowCandidateRelations,
        SetQAWorkFlowCandidateRelations_PermissionFailed,
        CreateFAQCategory,
        CreateFAQCategory_PermissionFailed,
        RenameFAQCategory,
        RenameFAQCategory_PermissionFailed,
        MoveFAQCategories,
        MoveFAQCategories_PermissionFailed,
        SetFAQCategoriesOrder_PermissionFailed,
        RemoveFAQCategories,
        RemoveFAQCategories_PermissionFailed,
        AddFAQItems,
        AddFAQItems_PermissionFailed,
        AddQuestionToFAQCategories,
        AddQuestionToFAQCategories_PermissionFailed,
        RemoveFAQItem,
        RemoveFAQItem_PermissionFailed,
        SetFAQItemsOrder_PermissionFailed,
        SendQuestion,
        EditQuestionTitle,
        EditQuestionTitle_PermissionFailed,
        EditQuestionDescription,
        EditQuestionDescription_PermissionFailed,
        InitialConfirmQuestion,
        InitialConfirmQuestion_PermissionFailed,
        ConfirmQuestion,
        ConfirmQuestion_PermissionFailed,
        SetTheBestAnswer,
        SetTheBestAnswer_PermissionFailed,
        RemoveQuestion,
        RemoveQuestion_PermissionFailed,
        AddQuestionTag,
        AddQuestionTag_PermissionFailed,
        SaveQuestionRelatedNodes,
        SaveQuestionRelatedNodes_PermissionFailed,
        RemoveQuestionRelatedNodes,
        RemoveQuestionRelatedNodes_PermissionFailed,
        SendAnswer,
        SendAnswer_PermissionFailed,
        EditAnswer,
        EditAnswer_PermissionFailed,
        RemoveAnswer,
        RemoveAnswer_PermissionFailed,
        SendQAComment,
        EditQAComment,
        EditQAComment_PermissionFailed,
        RemoveQAComment,
        RemoveQAComment_PermissionFailed,
        AddQAKnowledgableUser,
        AddQAKnowledgableUser_PermissionFailed,
        RemoveQAKnowledgableUser,
        RemoveQAKnowledgableUser_PermissionFailed,
        //////////--> end of QA <--//////////

        //////////--> SH <--//////////
        SendPost,
        ModifyPost,
        RemovePost,
        SharePost,
        SendComment,
        ModifyComment,
        RemoveComment,
        LikePost,
        DislikePost,
        UnlikePost,
        LikeComment,
        DislikeComment,
        UnlikeComment,
        //////////--> end of SH <--//////////

        //////////--> SRCH <--//////////
        Search,
        //////////--> end of SRCH <--//////////

        //////////--> USR <--//////////
        AcceptFriendRequest,
        RejectFriendRequest,
        CreateUserGroup,
        ModifyUserGroup,
        RemoveUserGroup,
        AddUserGroupMember,
        RemoveUserGroupMember,
        SetUserGroupPermission,
        RemoveUserGroupPermission,
        SetUserGeneralInfo,
        Login,
        Login_Failed,
        UserLockedOut,
        Logout,
        AddRemoteServer,
        ModifyRemoteServer,
        RemoveRemoteServer,
        AddZServer,
        ModifyZServer,
        RemoveZServer,
        SetFirstAndLastName,
        SetUserName,
        SetNationalID,
        SetPersonnelID,
        SetUserIsApproved,
        SetJobTitle,
        SetEmploymentType,
        SetBirthday,
        SetPhoneNumber,
        ModifyPhoneNumber,
        RemovePhoneNumber,
        SetMainPhoneNumber,
        SetEmailAddress,
        ModifyEmailAddress,
        RemoveEmailAddress,
        SetMainEmailAddress,
        //////////--> end of USR <--//////////

        //////////--> WK <--//////////
        Wiki_AccessDenied,
        AddWikiTitle,
        AddWikiTitle_PermissionFailed,
        ModifyWikiTitle,
        ModifyWikiTitle_PermissionFailed,
        SetWikiTitlesOrder,
        SetWikiTitlesOrder_PermissionFailed,
        RemoveWikiTitle,
        RemoveWikiTitle_PermissionFailed,
        RecycleWikiTitle,
        RecycleWikiTitle_PermissionFailed,
        AddWikiParagraph,
        AddWikiParagraph_PermissionFailed,
        ModifyWikiParagraph,
        ModifyWikiParagraph_PermissionFailed,
        SuggestWikiParagraphChange,
        SuggestWikiParagraphChange_PermissionFailed,
        RemoveWikiParagraph,
        RemoveWikiParagraph_PermissionFailed,
        RecycleWikiParagraph,
        RecycleWikiParagraph_PermissionFailed,
        SetWikiParagraphsOrder,
        SetWikiParagraphsOrder_PermissionFailed,
        AcceptWikiParagraphChange,
        AcceptWikiParagraphChange_PermissionFailed,
        RejectWikiParagraphChange,
        RejectWikiParagraphChange_PermissionFailed,
        RemoveWikiParagraphChange,
        RemoveWikiParagraphChange_PermissionFailed,
        //////////--> end of WK <--//////////

        //////////--> WF <--//////////
        CreateState,
        CreateState_PermissionFailed,
        ModifyState,
        ModifyState_PermissionFailed,
        RemoveState,
        RemoveState_PermissionFailed,
        CreateWorkFlow,
        CreateWorkFlow_PermissionFailed,
        ModifyWorkFlow,
        ModifyWorkFlow_PermissionFailed,
        RemoveWorkFlow,
        RemoveWorkFlow_PermissionFailed,
        GetWorkFlow_PermissionFailed,
        AddWorkFlowState,
        AddWorkFlowState_PermissionFailed,
        RemoveWorkFlowState,
        RemoveWorkFlowState_PermissionFailed,
        ModifyWorkFlowStateDescription,
        ModifyWorkFlowStateDescription_PermissionFailed,
        SetWorkFlowStateTag,
        SetWorkFlowStateTag_PermissionFailed,
        RemoveWorkFlowStateTag,
        RemoveWorkFlowStateTag_PermissionFailed,
        AddWorkFlowAction,
        ModifyWorkFlowAction,
        RemoveWorkFlowAction,
        SetWorkFlowStateDirector,
        SetWorkFlowStateDirector_PermissionFailed,
        SetWorkFlowStatePoll,
        SetWorkFlowStatePoll_PermissionFailed,
        SetWorkFlowStateDataNeedsType,
        SetWorkFlowStateDataNeedsType_PermissionFailed,
        SetWorkFlowStateDataNeedsDescription,
        SetWorkFlowStateDataNeedsDescription_PermissionFailed,
        SetWorkFlowStateDescriptionAsNeeded,
        SetWorkFlowStateDescriptionAsNeeded_PermissionFailed,
        SetWorkFlowStateDescriptionAsNotNeeded,
        SetWorkFlowStateDescriptionAsNotNeeded_PermissionFailed,
        SetWorkFlowStateHideOwnerNameAsNeeded,
        SetWorkFlowStateHideOwnerNameAsNeeded_PermissionFailed,
        SetWorkFlowStateHideOwnerNameAsNotNeeded,
        SetWorkFlowStateHideOwnerNameAsNotNeeded_PermissionFailed,
        SetWorkFlowStateEditPermission,
        SetWorkFlowStateEditPermission_PermissionFailed,
        SetDataNeedRequestsAsFree,
        SetDataNeedRequestsAsFree_PermissionFailed,
        SetDataNeedRequestsAsNotFree,
        SetDataNeedRequestsAsNotFree_PermissionFailed,
        SetStateDataNeed,
        SetStateDataNeed_PermissionFailed,
        RemoveStateDataNeed,
        RemoveStateDataNeed_PermissionFailed,
        SetRejectionSettings,
        SetRejectionSettings_PermissionFailed,
        SetMaxAllowedRejections,
        SetMaxAllowedRejections_PermissionFailed,
        SetStateDataNeedInstanceAsFilled,
        SetStateDataNeedInstanceAsFilled_PermissionFailed,
        SetStateDataNeedInstanceAsNotFilled,
        SetStateDataNeedInstanceAsNotFilled_PermissionFailed,
        RemoveStateDataNeedInstance,
        RemoveStateDataNeedInstance_PermissionFailed,
        AddStateConnection,
        AddStateConnection_PermissionFailed,
        MoveStateConnectionUp,
        MoveStateConnectionUp_PermissionFailed,
        MoveStateConnectionDown,
        MoveStateConnectionDown_PermissionFailed,
        RemoveStateConnection,
        RemoveStateConnection_PermissionFailed,
        SetStateConnectionLabel,
        SetStateConnectionLabel_PermissionFailed,
        SetStateConnectionAttachmentStatus,
        SetStateConnectionAttachmentStatus_PermissionFailed,
        SetStateConnectionDirector,
        SetStateConnectionDirector_PermissionFailed,
        SetStateConnectionForm,
        SetStateConnectionForm_PermissionFailed,
        RemoveStateConnectionForm,
        RemoveStateConnectionForm_PermissionFailed,
        AddAutoMessage,
        AddAutoMessage_PermissionFailed,
        ModifyAutoMessage,
        ModifyAutoMessage_PermissionFailed,
        RemoveAutoMessage,
        RemoveAutoMessage_PermissionFailed,
        TerminateWorkFlow,
        TerminateWorkFlow_PermissionFailed,
        RestartWorkFlow,
        RestartWorkFlow_PermissionFailed,
        AddOwnerWorkFlow,
        AddOwnerWorkFlow_PermissionFailed,
        RemoveOwnerWorkFlow,
        RemoveOwnerWorkFlow_PermissionFailed
        //////////--> end of WF <--//////////
    }

    public class Log
    {
        public Log()
        {
            SubjectIDs = new List<Guid>();
        }

        public long? LogID;
        public Guid? UserID;
        public string HostAddress;
        public string HostName;
        public Action? Action;
        public Guid? SubjectID;
        public List<Guid> SubjectIDs;
        public Guid? SecondSubjectID;
        public Guid? ThirdSubjectID;
        public Guid? FourthSubjectID;
        public DateTime? Date;
        public string Info;
        public ModuleIdentifier? ModuleIdentifier;
        public bool? NotAuthorized;

        public string toJson()
        {
            return "{\"Action\":\"" + (!Action.HasValue ? string.Empty : Action.ToString()) + "\"" +
                ",\"Date\":\"" + (!Date.HasValue ? string.Empty : GenericDate.get_local_date(Date.Value, true)) + "\"" +
                ",\"HostAddress\":\"" + (string.IsNullOrEmpty(HostAddress) ? string.Empty : HostAddress) + "\"" +
                ",\"HostName\":\"" + (string.IsNullOrEmpty(HostName) ? string.Empty : HostName) + "\"" +
                "}";
        }
    }

    public class ErrorLog
    {
        public long? LogID;
        public Guid? UserID;
        public string Subject;
        public string Description;
        public DateTime? Date;
        public ModuleIdentifier? ModuleIdentifier;
        public LogLevel Level;

        public ErrorLog() {
            Level = LogLevel.None;
        }
    }

    public class LevelOfTheLog
    {
        private static ConcurrentDictionary<Action, LogLevel> Dic = new ConcurrentDictionary<Action, LogLevel>();
        private static bool _Inited = false;

        private static void init() {
            if (_Inited) return;
            else _Inited = true;

            lock (Dic) {
                Array arr = Enum.GetValues(typeof(Action));

                List<string> errorList = "_accessdenied,_permissionfailed,_failed".Split(',').ToList();
                List<string> traceList = ("accept,activate,add,change,confirm,create,deactive,disable,dislike" + 
                    ",edit,enable,like,modify,move,recover,recycle,register,reject,remove,rename,save,send,set,sort" + 
                    ",terminate,unlike,unset").Split(',').ToList();
                
                foreach (Action acn in arr)
                {
                    string strAcn = acn.ToString().ToLower();

                    if (errorList.Any(u => strAcn.EndsWith(u))) Dic[acn] = LogLevel.Error;
                    else if (traceList.Any(u => strAcn.StartsWith(u))) Dic[acn] = LogLevel.Trace;
                }
                
                Dic[Action.NotAuthorizedAnonymousRequest] = LogLevel.Warn;
                Dic[Action.PotentialCSRFAttack] = LogLevel.Warn;
                Dic[Action.PotentialReplayAttack] = LogLevel.Warn;
                Dic[Action.JobStarted] = LogLevel.Info;
                Dic[Action.UserLockedOut] = LogLevel.Info;
                Dic[Action.Download] = LogLevel.Trace;
                Dic[Action.KnowledgeEvaluation] = LogLevel.Trace;
                Dic[Action.RefuseKnowledgeEvaluation] = LogLevel.Trace;
                Dic[Action.InitialConfirmQuestion] = LogLevel.Trace;
                Dic[Action.SharePost] = LogLevel.Trace;
                Dic[Action.Search] = LogLevel.Trace;
                Dic[Action.Login] = LogLevel.Trace;
                Dic[Action.Logout] = LogLevel.Trace;
                Dic[Action.SuggestWikiParagraphChange] = LogLevel.Trace;
                Dic[Action.RestartWorkFlow] = LogLevel.Trace;
            }
        }

        public static LogLevel get(Action action)
        {
            init();
            return Dic.ContainsKey(action) ? Dic[action] : LogLevel.None;
        }
    }
}
