USE [EKM_App]
GO

ALTER TABLE [dbo].[AppSetting] CHECK CONSTRAINT [FK_AppSetting_aspnet_Applications] 
GO
ALTER TABLE [dbo].[NodeMetrics] CHECK CONSTRAINT [FK_NodeMetrics_aspnet_Applications] 
GO
ALTER TABLE [dbo].[USR_FriendSuggestions] CHECK CONSTRAINT [FK_USR_FriendSuggestions_aspnet_Applications] 
GO
ALTER TABLE [dbo].[USR_HonorsAndAwards] CHECK CONSTRAINT [FK_USR_HonorsAndAwards_aspnet_Applications] 
GO
ALTER TABLE [dbo].[DCT_Trees] CHECK CONSTRAINT [FK_DCT_Trees_aspnet_Applications] 
GO
ALTER TABLE [dbo].[CN_Nodes] CHECK CONSTRAINT [FK_CN_Nodes_aspnet_Applications] 
GO
ALTER TABLE [dbo].[WF_StateDataNeeds] CHECK CONSTRAINT [FK_WF_StateDataNeeds_aspnet_Applications] 
GO
ALTER TABLE [dbo].[DCT_TreeNodes] CHECK CONSTRAINT [FK_DCT_TreeNodes_aspnet_Applications] 
GO
ALTER TABLE [dbo].[Attachments] CHECK CONSTRAINT [FK_Attachments_aspnet_Applications] 
GO
ALTER TABLE [dbo].[CN_ContributionLimits] CHECK CONSTRAINT [FK_CN_ContributionLimits_aspnet_Applications] 
GO
ALTER TABLE [dbo].[USR_PasswordsHistory] CHECK CONSTRAINT [FK_USR_PasswordsHistory_aspnet_Applications] 
GO
ALTER TABLE [dbo].[KWF_Statuses] CHECK CONSTRAINT [FK_KWF_Statuses_aspnet_Applications] 
GO
ALTER TABLE [dbo].[KW_ConfidentialityLevels] CHECK CONSTRAINT [FK_KW_ConfidentialityLevels_aspnet_Applications] 
GO
ALTER TABLE [dbo].[AddedForms] CHECK CONSTRAINT [FK_AddedForms_aspnet_Applications] 
GO
ALTER TABLE [dbo].[KW_UsersConfidentialityLevels] CHECK CONSTRAINT [FK_KW_UsersConfidentialityLevels_aspnet_Applications] 
GO
ALTER TABLE [dbo].[WF_History] CHECK CONSTRAINT [FK_WF_History_aspnet_Applications] 
GO
ALTER TABLE [dbo].[LG_ErrorLogs] CHECK CONSTRAINT [FK_LG_ErrorLogs_aspnet_Applications] 
GO
ALTER TABLE [dbo].[SH_PostShares] CHECK CONSTRAINT [FK_SH_PostShares_aspnet_Applications] 
GO
ALTER TABLE [dbo].[PRVC_ConfidentialityLevels] CHECK CONSTRAINT [FK_PRVC_ConfidentialityLevels_aspnet_Applications] 
GO
ALTER TABLE [dbo].[KW_KnowledgeTypes] CHECK CONSTRAINT [FK_KW_KnowledgeTypes_aspnet_Applications] 
GO
ALTER TABLE [dbo].[PRVC_Confidentialities] CHECK CONSTRAINT [FK_PRVC_Confidentialities_aspnet_Applications] 
GO
ALTER TABLE [dbo].[KW_Knowledges] CHECK CONSTRAINT [FK_KW_Knowledges_aspnet_Applications] 
GO
ALTER TABLE [dbo].[USR_EmailAddresses] CHECK CONSTRAINT [FK_USR_EmailAddresses_aspnet_Applications] 
GO
ALTER TABLE [dbo].[NTFN_Notifications] CHECK CONSTRAINT [FK_NTFN_Notifications_aspnet_Applications] 
GO
ALTER TABLE [dbo].[LG_Logs] CHECK CONSTRAINT [FK_LG_Logs_aspnet_Applications] 
GO
ALTER TABLE [dbo].[USR_PhoneNumbers] CHECK CONSTRAINT [FK_USR_PhoneNumbers_aspnet_Applications] 
GO
ALTER TABLE [dbo].[CN_NodeCreators] CHECK CONSTRAINT [FK_CN_NodeCreators_aspnet_Applications] 
GO
ALTER TABLE [dbo].[KW_CreatorUsers] CHECK CONSTRAINT [FK_KW_CreatorUsers_aspnet_Applications] 
GO
ALTER TABLE [dbo].[CN_Lists] CHECK CONSTRAINT [FK_CN_Lists_aspnet_Applications] 
GO
ALTER TABLE [dbo].[KW_KnowledgeCards] CHECK CONSTRAINT [FK_KW_KnowledgeCards_aspnet_Applications] 
GO
ALTER TABLE [dbo].[CN_Services] CHECK CONSTRAINT [FK_CN_Services_aspnet_Applications] 
GO
ALTER TABLE [dbo].[CN_AdminTypeLimits] CHECK CONSTRAINT [FK_CN_AdminTypeLimits_aspnet_Applications] 
GO
ALTER TABLE [dbo].[AccessRoles] CHECK CONSTRAINT [FK_AccessRoles_aspnet_Applications] 
GO
ALTER TABLE [dbo].[CN_Extensions] CHECK CONSTRAINT [FK_CN_Extensions_aspnet_Applications] 
GO
ALTER TABLE [dbo].[KW_SkillLevels] CHECK CONSTRAINT [FK_KW_SkillLevels_aspnet_Applications] 
GO
ALTER TABLE [dbo].[CN_ServiceAdmins] CHECK CONSTRAINT [FK_CN_ServiceAdmins_aspnet_Applications] 
GO
ALTER TABLE [dbo].[KW_KnowledgeAssets] CHECK CONSTRAINT [FK_KW_KnowledgeAssets_aspnet_Applications] 
GO
ALTER TABLE [dbo].[RV_TaggedItems] CHECK CONSTRAINT [FK_RV_TaggedItems_aspnet_Applications] 
GO
ALTER TABLE [dbo].[OrganProfile] CHECK CONSTRAINT [FK_OrganProfile_aspnet_Applications] 
GO
ALTER TABLE [dbo].[KW_NodeRelatedTrees] CHECK CONSTRAINT [FK_KW_NodeRelatedTrees_aspnet_Applications] 
GO
ALTER TABLE [dbo].[NQuestions] CHECK CONSTRAINT [FK_NQuestions_aspnet_Applications] 
GO
ALTER TABLE [dbo].[DCT_FileContents] CHECK CONSTRAINT [FK_DCT_FileContents_aspnet_Applications] 
GO
ALTER TABLE [dbo].[CN_FreeUsers] CHECK CONSTRAINT [FK_CN_FreeUsers_aspnet_Applications] 
GO
ALTER TABLE [dbo].[KW_RefrenceUsers] CHECK CONSTRAINT [FK_KW_RefrenceUsers_aspnet_Applications] 
GO
ALTER TABLE [dbo].[MSG_Messages] CHECK CONSTRAINT [FK_MSG_Messages_aspnet_Applications] 
GO
ALTER TABLE [dbo].[RV_ID2Guid] CHECK CONSTRAINT [FK_RV_ID2Guid_aspnet_Applications] 
GO
ALTER TABLE [dbo].[UserGroups] CHECK CONSTRAINT [FK_UserGroups_aspnet_Applications] 
GO
ALTER TABLE [dbo].[NTFN_MessageTemplates] CHECK CONSTRAINT [FK_NTFN_MessageTemplates_aspnet_Applications] 
GO
ALTER TABLE [dbo].[UserGroupUsers] CHECK CONSTRAINT [FK_UserGroupUsers_aspnet_Applications] 
GO
ALTER TABLE [dbo].[MSG_MessageDetails] CHECK CONSTRAINT [FK_MSG_MessageDetails_aspnet_Applications] 
GO
ALTER TABLE [dbo].[KW_LearningMethods] CHECK CONSTRAINT [FK_KW_LearningMethods_aspnet_Applications] 
GO
ALTER TABLE [dbo].[NewsObjectTypes] CHECK CONSTRAINT [FK_NewsObjectTypes_aspnet_Applications] 
GO
ALTER TABLE [dbo].[FG_ExtendedFormElements] CHECK CONSTRAINT [FK_FG_ExtendedFormElements_aspnet_Applications] 
GO
ALTER TABLE [dbo].[USR_TemporaryUsers] CHECK CONSTRAINT [FK_USR_TemporaryUsers_aspnet_Applications] 
GO
ALTER TABLE [dbo].[PersonalNews] CHECK CONSTRAINT [FK_PersonalNews_aspnet_Applications] 
GO
ALTER TABLE [dbo].[TMP_KW_KnowledgeTypes] CHECK CONSTRAINT [FK_TMP_KW_KnowledgeTypes_aspnet_Applications] 
GO
ALTER TABLE [dbo].[FG_FormOwners] CHECK CONSTRAINT [FK_FG_FormOwners_aspnet_Applications] 
GO
ALTER TABLE [dbo].[USR_Invitations] CHECK CONSTRAINT [FK_USR_Invitations_aspnet_Applications] 
GO
ALTER TABLE [dbo].[KW_TripForms] CHECK CONSTRAINT [FK_KW_TripForms_aspnet_Applications] 
GO
ALTER TABLE [dbo].[MetricsConst] CHECK CONSTRAINT [FK_MetricsConst_aspnet_Applications] 
GO
ALTER TABLE [dbo].[KW_Companies] CHECK CONSTRAINT [FK_KW_Companies_aspnet_Applications] 
GO
ALTER TABLE [dbo].[CN_ListNodes] CHECK CONSTRAINT [FK_CN_ListNodes_aspnet_Applications] 
GO
ALTER TABLE [dbo].[TMP_KW_Questions] CHECK CONSTRAINT [FK_TMP_KW_Questions_aspnet_Applications] 
GO
ALTER TABLE [dbo].[FG_ElementLimits] CHECK CONSTRAINT [FK_FG_ElementLimits_aspnet_Applications] 
GO
ALTER TABLE [dbo].[KW_RelatedNodes] CHECK CONSTRAINT [FK_KW_RelatedNodes_aspnet_Applications] 
GO
ALTER TABLE [dbo].[TMP_KW_TypeQuestions] CHECK CONSTRAINT [FK_TMP_KW_TypeQuestions_aspnet_Applications] 
GO
ALTER TABLE [dbo].[GapColorConst] CHECK CONSTRAINT [FK_GapColorConst_aspnet_Applications] 
GO
ALTER TABLE [dbo].[SH_PostTypes] CHECK CONSTRAINT [FK_SH_PostTypes_aspnet_Applications] 
GO
ALTER TABLE [dbo].[PRVC_PrivacyType] CHECK CONSTRAINT [FK_PRVC_PrivacyType_aspnet_Applications] 
GO
ALTER TABLE [dbo].[KW_KnowledgeManagers] CHECK CONSTRAINT [FK_KW_KnowledgeManagers_aspnet_Applications] 
GO
ALTER TABLE [dbo].[SH_Posts] CHECK CONSTRAINT [FK_SH_Posts_aspnet_Applications] 
GO
ALTER TABLE [dbo].[WF_StateDataNeedInstances] CHECK CONSTRAINT [FK_WF_StateDataNeedInstances_aspnet_Applications] 
GO
ALTER TABLE [dbo].[KW_ExperienceHolders] CHECK CONSTRAINT [FK_KW_ExperienceHolders_aspnet_Applications] 
GO
ALTER TABLE [dbo].[TMP_KW_CandidateRelations] CHECK CONSTRAINT [FK_TMP_KW_CandidateRelations_aspnet_Applications] 
GO
ALTER TABLE [dbo].[FG_InstanceElements] CHECK CONSTRAINT [FK_FG_InstanceElements_aspnet_Applications] 
GO
ALTER TABLE [dbo].[WK_Titles] CHECK CONSTRAINT [FK_WK_Titles_aspnet_Applications] 
GO
ALTER TABLE [dbo].[Countries] CHECK CONSTRAINT [FK_Countries_aspnet_Applications] 
GO
ALTER TABLE [dbo].[ProfileScientific] CHECK CONSTRAINT [FK_ProfileScientific_aspnet_Applications] 
GO
ALTER TABLE [dbo].[KW_FeedBacks] CHECK CONSTRAINT [FK_KW_FeedBacks_aspnet_Applications] 
GO
ALTER TABLE [dbo].[WF_AutoMessages] CHECK CONSTRAINT [FK_WF_AutoMessages_aspnet_Applications] 
GO
ALTER TABLE [dbo].[ProfileJobs] CHECK CONSTRAINT [FK_ProfileJobs_aspnet_Applications] 
GO
ALTER TABLE [dbo].[ProfileInstitute] CHECK CONSTRAINT [FK_ProfileInstitute_aspnet_Applications] 
GO
ALTER TABLE [dbo].[TMP_KW_QuestionAnswers] CHECK CONSTRAINT [FK_TMP_KW_QuestionAnswers_aspnet_Applications] 
GO
ALTER TABLE [dbo].[NTFN_Dashboards] CHECK CONSTRAINT [FK_NTFN_Dashboards_aspnet_Applications] 
GO
ALTER TABLE [dbo].[ProfileEducation] CHECK CONSTRAINT [FK_ProfileEducation_aspnet_Applications] 
GO
ALTER TABLE [dbo].[KWF_Managers] CHECK CONSTRAINT [FK_KWF_Managers_aspnet_Applications] 
GO
ALTER TABLE [dbo].[RV_DeletedStates] CHECK CONSTRAINT [FK_RV_DeletedStates_aspnet_Applications] 
GO
ALTER TABLE [dbo].[SH_Comments] CHECK CONSTRAINT [FK_SH_Comments_aspnet_Applications] 
GO
ALTER TABLE [dbo].[TMP_KW_History] CHECK CONSTRAINT [FK_TMP_KW_History_aspnet_Applications] 
GO
ALTER TABLE [dbo].[CN_NodeMembers] CHECK CONSTRAINT [FK_CN_NodeMembers_aspnet_Applications] 
GO
ALTER TABLE [dbo].[FG_FormInstances] CHECK CONSTRAINT [FK_FG_FormInstances_aspnet_Applications] 
GO
ALTER TABLE [dbo].[FG_ExtendedForms] CHECK CONSTRAINT [FK_FG_ExtendedForms_aspnet_Applications] 
GO
ALTER TABLE [dbo].[KWF_Paraphs] CHECK CONSTRAINT [FK_KWF_Paraphs_aspnet_Applications] 
GO
ALTER TABLE [dbo].[KWF_Experts] CHECK CONSTRAINT [FK_KWF_Experts_aspnet_Applications] 
GO
ALTER TABLE [dbo].[SH_ShareLikes] CHECK CONSTRAINT [FK_SH_ShareLikes_aspnet_Applications] 
GO
ALTER TABLE [dbo].[RV_Variables] CHECK CONSTRAINT [FK_RV_Variables_aspnet_Applications] 
GO
ALTER TABLE [dbo].[TMP_KW_TempKnowledgeTypeIDs] CHECK CONSTRAINT [FK_TMP_KW_TempKnowledgeTypeIDs_aspnet_Applications] 
GO
ALTER TABLE [dbo].[SH_CommentLikes] CHECK CONSTRAINT [FK_SH_CommentLikes_aspnet_Applications] 
GO
ALTER TABLE [dbo].[CN_Experts] CHECK CONSTRAINT [FK_CN_Experts_aspnet_Applications] 
GO
ALTER TABLE [dbo].[WK_Paragraphs] CHECK CONSTRAINT [FK_WK_Paragraphs_aspnet_Applications] 
GO
ALTER TABLE [dbo].[TMP_KW_FeedBacks] CHECK CONSTRAINT [FK_TMP_KW_FeedBacks_aspnet_Applications] 
GO
ALTER TABLE [dbo].[WF_WorkFlowOwners] CHECK CONSTRAINT [FK_WF_WorkFlowOwners_aspnet_Applications] 
GO
ALTER TABLE [dbo].[CN_Properties] CHECK CONSTRAINT [FK_CN_Properties_aspnet_Applications] 
GO
ALTER TABLE [dbo].[CN_ExpertiseReferrals] CHECK CONSTRAINT [FK_CN_ExpertiseReferrals_aspnet_Applications] 
GO
ALTER TABLE [dbo].[KWF_Evaluators] CHECK CONSTRAINT [FK_KWF_Evaluators_aspnet_Applications] 
GO
ALTER TABLE [dbo].[WF_States] CHECK CONSTRAINT [FK_WF_States_aspnet_Applications] 
GO
ALTER TABLE [dbo].[QA_Questions] CHECK CONSTRAINT [FK_QA_Questions_aspnet_Applications] 
GO
ALTER TABLE [dbo].[WK_Changes] CHECK CONSTRAINT [FK_WK_Changes_aspnet_Applications] 
GO
ALTER TABLE [dbo].[CN_NodeLikes] CHECK CONSTRAINT [FK_CN_NodeLikes_aspnet_Applications] 
GO
ALTER TABLE [dbo].[CN_NodeProperties] CHECK CONSTRAINT [FK_CN_NodeProperties_aspnet_Applications] 
GO
ALTER TABLE [dbo].[WF_WorkFlows] CHECK CONSTRAINT [FK_WF_WorkFlows_aspnet_Applications] 
GO
ALTER TABLE [dbo].[QA_Answers] CHECK CONSTRAINT [FK_QA_Answers_aspnet_Applications] 
GO
ALTER TABLE [dbo].[AttachmentFiles] CHECK CONSTRAINT [FK_AttachmentFiles_aspnet_Applications] 
GO
ALTER TABLE [dbo].[WF_StateConnections] CHECK CONSTRAINT [FK_WF_StateConnections_aspnet_Applications] 
GO
ALTER TABLE [dbo].[PRVC_Audience] CHECK CONSTRAINT [FK_PRVC_Audience_aspnet_Applications] 
GO
ALTER TABLE [dbo].[NGeneralQuestions] CHECK CONSTRAINT [FK_NGeneralQuestions_aspnet_Applications] 
GO
ALTER TABLE [dbo].[CN_NodeRelations] CHECK CONSTRAINT [FK_CN_NodeRelations_aspnet_Applications] 
GO
ALTER TABLE [dbo].[USR_PassResetTickets] CHECK CONSTRAINT [FK_USR_PassResetTickets_aspnet_Applications] 
GO
ALTER TABLE [dbo].[EVT_Events] CHECK CONSTRAINT [FK_EVT_Events_aspnet_Applications] 
GO
ALTER TABLE [dbo].[USR_Profile] CHECK CONSTRAINT [FK_USR_Profile_aspnet_Applications] 
GO
ALTER TABLE [dbo].[UserGroupAccessRoles] CHECK CONSTRAINT [FK_UserGroupAccessRoles_aspnet_Applications] 
GO
ALTER TABLE [dbo].[WF_StateConnectionForms] CHECK CONSTRAINT [FK_WF_StateConnectionForms_aspnet_Applications] 
GO
ALTER TABLE [dbo].[QA_QuestionLikes] CHECK CONSTRAINT [FK_QA_QuestionLikes_aspnet_Applications] 
GO
ALTER TABLE [dbo].[USR_LanguageNames] CHECK CONSTRAINT [FK_USR_LanguageNames_aspnet_Applications] 
GO
ALTER TABLE [dbo].[EVT_RelatedUsers] CHECK CONSTRAINT [FK_EVT_RelatedUsers_aspnet_Applications] 
GO
ALTER TABLE [dbo].[USR_UserLanguages] CHECK CONSTRAINT [FK_USR_UserLanguages_aspnet_Applications] 
GO
ALTER TABLE [dbo].[Cities] CHECK CONSTRAINT [FK_Cities_aspnet_Applications] 
GO
ALTER TABLE [dbo].[CN_ListAdmins] CHECK CONSTRAINT [FK_CN_ListAdmins_aspnet_Applications] 
GO
ALTER TABLE [dbo].[QA_RefNodes] CHECK CONSTRAINT [FK_QA_RefNodes_aspnet_Applications] 
GO
ALTER TABLE [dbo].[USR_ItemVisits] CHECK CONSTRAINT [FK_USR_ItemVisits_aspnet_Applications] 
GO
ALTER TABLE [dbo].[NTFN_NotificationMessageTemplates] CHECK CONSTRAINT [FK_NTFN_NotificationMessageTemplates_aspnet_Applications] 
GO
ALTER TABLE [dbo].[CN_Tags] CHECK CONSTRAINT [FK_CN_Tags_aspnet_Applications] 
GO
ALTER TABLE [dbo].[WF_WorkFlowStates] CHECK CONSTRAINT [FK_WF_WorkFlowStates_aspnet_Applications] 
GO
ALTER TABLE [dbo].[EVT_RelatedNodes] CHECK CONSTRAINT [FK_EVT_RelatedNodes_aspnet_Applications] 
GO
ALTER TABLE [dbo].[WF_HistoryFormInstances] CHECK CONSTRAINT [FK_WF_HistoryFormInstances_aspnet_Applications] 
GO
ALTER TABLE [dbo].[QA_RefUsers] CHECK CONSTRAINT [FK_QA_RefUsers_aspnet_Applications] 
GO
ALTER TABLE [dbo].[USR_JobExperiences] CHECK CONSTRAINT [FK_USR_JobExperiences_aspnet_Applications] 
GO
ALTER TABLE [dbo].[RV_EmailQueue] CHECK CONSTRAINT [FK_RV_EmailQueue_aspnet_Applications] 
GO
ALTER TABLE [dbo].[USR_EmailContacts] CHECK CONSTRAINT [FK_USR_EmailContacts_aspnet_Applications] 
GO
ALTER TABLE [dbo].[NodeSetting] CHECK CONSTRAINT [FK_NodeSetting_aspnet_Applications] 
GO
ALTER TABLE [dbo].[NTFN_UserMessagingActivation] CHECK CONSTRAINT [FK_NTFN_UserMessagingActivation_aspnet_Applications] 
GO
ALTER TABLE [dbo].[RV_SentEmails] CHECK CONSTRAINT [FK_RV_SentEmails_aspnet_Applications] 
GO
ALTER TABLE [dbo].[USR_Friends] CHECK CONSTRAINT [FK_USR_Friends_aspnet_Applications] 
GO
ALTER TABLE [dbo].[USR_EducationalExperiences] CHECK CONSTRAINT [FK_USR_EducationalExperiences_aspnet_Applications]
GO
ALTER TABLE [dbo].[CN_NodeTypes] CHECK CONSTRAINT [FK_CN_NodeTypes_aspnet_Applications] 
GO
ALTER TABLE [dbo].[DCT_TreeTypes] CHECK CONSTRAINT [FK_DCT_TreeTypes_aspnet_Applications] 
GO