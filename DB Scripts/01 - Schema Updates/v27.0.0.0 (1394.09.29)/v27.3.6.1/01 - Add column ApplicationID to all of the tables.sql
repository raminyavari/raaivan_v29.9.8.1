USE [EKM_App]
GO


ALTER TABLE [dbo].[NodeMetrics] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[USR_FriendSuggestions] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[USR_HonorsAndAwards] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[DCT_Trees] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[CN_Nodes] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[WF_StateDataNeeds] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[DCT_TreeNodes] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[Attachments] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[CN_ContributionLimits] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[USR_PasswordsHistory] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[KWF_Statuses] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[KW_ConfidentialityLevels] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[AddedForms] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[KW_UsersConfidentialityLevels] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[WF_History] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[LG_ErrorLogs] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[SH_PostShares] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[PRVC_ConfidentialityLevels] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[KW_KnowledgeTypes] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[PRVC_Confidentialities] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[KW_Knowledges] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[USR_EmailAddresses] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[NTFN_Notifications] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[LG_Logs] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[USR_PhoneNumbers] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[CN_NodeCreators] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[KW_CreatorUsers] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[CN_Lists] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[KW_KnowledgeCards] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[CN_Services] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[CN_AdminTypeLimits] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[AccessRoles] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[CN_Extensions] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[KW_SkillLevels] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[CN_ServiceAdmins] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[KW_KnowledgeAssets] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[RV_TaggedItems] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[OrganProfile] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[KW_NodeRelatedTrees] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[NQuestions] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[DCT_FileContents] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[CN_FreeUsers] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[KW_RefrenceUsers] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[MSG_Messages] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[RV_ID2Guid] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[UserGroups] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[NTFN_MessageTemplates] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[UserGroupUsers] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[MSG_MessageDetails] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[KW_LearningMethods] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[NewsObjectTypes] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[FG_ExtendedFormElements] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[USR_TemporaryUsers] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[PersonalNews] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[TMP_KW_KnowledgeTypes] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[FG_FormOwners] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[USR_Invitations] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[KW_TripForms] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[MetricsConst] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[KW_Companies] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[CN_ListNodes] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[TMP_KW_Questions] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[FG_ElementLimits] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[KW_RelatedNodes] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[TMP_KW_TypeQuestions] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[GapColorConst] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[SH_PostTypes] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[PRVC_PrivacyType] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[KW_KnowledgeManagers] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[SH_Posts] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[WF_StateDataNeedInstances] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[KW_ExperienceHolders] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[TMP_KW_CandidateRelations] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[FG_InstanceElements] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[WK_Titles] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[Countries] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[ProfileScientific] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[KW_FeedBacks] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[WF_AutoMessages] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[ProfileJobs] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[ProfileInstitute] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[TMP_KW_QuestionAnswers] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[NTFN_Dashboards] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[ProfileEducation] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[KWF_Managers] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[RV_DeletedStates] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[SH_Comments] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[TMP_KW_History] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[CN_NodeMembers] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[FG_FormInstances] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[FG_ExtendedForms] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[KWF_Paraphs] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[KWF_Experts] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[SH_ShareLikes] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[RV_Variables] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[TMP_KW_TempKnowledgeTypeIDs] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[SH_CommentLikes] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[CN_Experts] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[WK_Paragraphs] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[TMP_KW_FeedBacks] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[WF_WorkFlowOwners] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[CN_Properties] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[CN_ExpertiseReferrals] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[KWF_Evaluators] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[WF_States] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[QA_Questions] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[WK_Changes] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[CN_NodeLikes] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[CN_NodeProperties] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[WF_WorkFlows] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[QA_Answers] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[AttachmentFiles] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[WF_StateConnections] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[PRVC_Audience] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[NGeneralQuestions] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[CN_NodeRelations] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[USR_PassResetTickets] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[EVT_Events] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[USR_Profile] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[UserGroupAccessRoles] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[WF_StateConnectionForms] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[QA_QuestionLikes] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[USR_LanguageNames] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[EVT_RelatedUsers] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[USR_UserLanguages] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[Cities] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[CN_ListAdmins] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[QA_RefNodes] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[USR_ItemVisits] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[NTFN_NotificationMessageTemplates] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[CN_Tags] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[WF_WorkFlowStates] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[EVT_RelatedNodes] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[WF_HistoryFormInstances] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[QA_RefUsers] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[USR_JobExperiences] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[RV_EmailQueue] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[USR_EmailContacts] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[NodeSetting] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[NTFN_UserMessagingActivation] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[RV_SentEmails] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[USR_Friends] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[USR_EducationalExperiences] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[CN_NodeTypes] ADD [ApplicationID] uniqueidentifier NULL 
GO
ALTER TABLE [dbo].[DCT_TreeTypes] ADD [ApplicationID] uniqueidentifier NULL 
GO