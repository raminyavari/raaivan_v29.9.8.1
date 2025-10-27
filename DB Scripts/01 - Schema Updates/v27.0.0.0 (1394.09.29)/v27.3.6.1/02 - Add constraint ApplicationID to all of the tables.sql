USE [EKM_App]
GO

UPDATE [dbo].[AppSetting] SET ApplicationID = '08C72552-4F2C-473F-B3B0-C2DACF8CD6A9' 
GO

ALTER TABLE [dbo].[AppSetting]  WITH CHECK ADD CONSTRAINT [FK_AppSetting_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[NodeMetrics]  WITH CHECK ADD CONSTRAINT [FK_NodeMetrics_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[USR_FriendSuggestions]  WITH CHECK ADD CONSTRAINT [FK_USR_FriendSuggestions_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[USR_HonorsAndAwards]  WITH CHECK ADD CONSTRAINT [FK_USR_HonorsAndAwards_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[DCT_Trees]  WITH CHECK ADD CONSTRAINT [FK_DCT_Trees_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[CN_Nodes]  WITH CHECK ADD CONSTRAINT [FK_CN_Nodes_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[WF_StateDataNeeds]  WITH CHECK ADD CONSTRAINT [FK_WF_StateDataNeeds_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[DCT_TreeNodes]  WITH CHECK ADD CONSTRAINT [FK_DCT_TreeNodes_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[Attachments]  WITH CHECK ADD CONSTRAINT [FK_Attachments_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[CN_ContributionLimits]  WITH CHECK ADD CONSTRAINT [FK_CN_ContributionLimits_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[USR_PasswordsHistory]  WITH CHECK ADD CONSTRAINT [FK_USR_PasswordsHistory_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[KWF_Statuses]  WITH CHECK ADD CONSTRAINT [FK_KWF_Statuses_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[KW_ConfidentialityLevels]  WITH CHECK ADD CONSTRAINT [FK_KW_ConfidentialityLevels_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[AddedForms]  WITH CHECK ADD CONSTRAINT [FK_AddedForms_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[KW_UsersConfidentialityLevels]  WITH CHECK ADD CONSTRAINT [FK_KW_UsersConfidentialityLevels_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[WF_History]  WITH CHECK ADD CONSTRAINT [FK_WF_History_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[LG_ErrorLogs]  WITH CHECK ADD CONSTRAINT [FK_LG_ErrorLogs_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[SH_PostShares]  WITH CHECK ADD CONSTRAINT [FK_SH_PostShares_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[PRVC_ConfidentialityLevels]  WITH CHECK ADD CONSTRAINT [FK_PRVC_ConfidentialityLevels_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[KW_KnowledgeTypes]  WITH CHECK ADD CONSTRAINT [FK_KW_KnowledgeTypes_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[PRVC_Confidentialities]  WITH CHECK ADD CONSTRAINT [FK_PRVC_Confidentialities_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[KW_Knowledges]  WITH CHECK ADD CONSTRAINT [FK_KW_Knowledges_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[USR_EmailAddresses]  WITH CHECK ADD CONSTRAINT [FK_USR_EmailAddresses_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[NTFN_Notifications]  WITH CHECK ADD CONSTRAINT [FK_NTFN_Notifications_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[LG_Logs]  WITH CHECK ADD CONSTRAINT [FK_LG_Logs_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[USR_PhoneNumbers]  WITH CHECK ADD CONSTRAINT [FK_USR_PhoneNumbers_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[CN_NodeCreators]  WITH CHECK ADD CONSTRAINT [FK_CN_NodeCreators_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[KW_CreatorUsers]  WITH CHECK ADD CONSTRAINT [FK_KW_CreatorUsers_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[CN_Lists]  WITH CHECK ADD CONSTRAINT [FK_CN_Lists_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[KW_KnowledgeCards]  WITH CHECK ADD CONSTRAINT [FK_KW_KnowledgeCards_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[CN_Services]  WITH CHECK ADD CONSTRAINT [FK_CN_Services_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[CN_AdminTypeLimits]  WITH CHECK ADD CONSTRAINT [FK_CN_AdminTypeLimits_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[AccessRoles]  WITH CHECK ADD CONSTRAINT [FK_AccessRoles_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[CN_Extensions]  WITH CHECK ADD CONSTRAINT [FK_CN_Extensions_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[KW_SkillLevels]  WITH CHECK ADD CONSTRAINT [FK_KW_SkillLevels_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[CN_ServiceAdmins]  WITH CHECK ADD CONSTRAINT [FK_CN_ServiceAdmins_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[KW_KnowledgeAssets]  WITH CHECK ADD CONSTRAINT [FK_KW_KnowledgeAssets_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[RV_TaggedItems]  WITH CHECK ADD CONSTRAINT [FK_RV_TaggedItems_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[OrganProfile]  WITH CHECK ADD CONSTRAINT [FK_OrganProfile_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[KW_NodeRelatedTrees]  WITH CHECK ADD CONSTRAINT [FK_KW_NodeRelatedTrees_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[NQuestions]  WITH CHECK ADD CONSTRAINT [FK_NQuestions_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[DCT_FileContents]  WITH CHECK ADD CONSTRAINT [FK_DCT_FileContents_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[CN_FreeUsers]  WITH CHECK ADD CONSTRAINT [FK_CN_FreeUsers_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[KW_RefrenceUsers]  WITH CHECK ADD CONSTRAINT [FK_KW_RefrenceUsers_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[MSG_Messages]  WITH CHECK ADD CONSTRAINT [FK_MSG_Messages_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[RV_ID2Guid]  WITH CHECK ADD CONSTRAINT [FK_RV_ID2Guid_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[UserGroups]  WITH CHECK ADD CONSTRAINT [FK_UserGroups_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[NTFN_MessageTemplates]  WITH CHECK ADD CONSTRAINT [FK_NTFN_MessageTemplates_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[UserGroupUsers]  WITH CHECK ADD CONSTRAINT [FK_UserGroupUsers_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[MSG_MessageDetails]  WITH CHECK ADD CONSTRAINT [FK_MSG_MessageDetails_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[KW_LearningMethods]  WITH CHECK ADD CONSTRAINT [FK_KW_LearningMethods_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[NewsObjectTypes]  WITH CHECK ADD CONSTRAINT [FK_NewsObjectTypes_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[FG_ExtendedFormElements]  WITH CHECK ADD CONSTRAINT [FK_FG_ExtendedFormElements_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[USR_TemporaryUsers]  WITH CHECK ADD CONSTRAINT [FK_USR_TemporaryUsers_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[PersonalNews]  WITH CHECK ADD CONSTRAINT [FK_PersonalNews_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[TMP_KW_KnowledgeTypes]  WITH CHECK ADD CONSTRAINT [FK_TMP_KW_KnowledgeTypes_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[FG_FormOwners]  WITH CHECK ADD CONSTRAINT [FK_FG_FormOwners_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[USR_Invitations]  WITH CHECK ADD CONSTRAINT [FK_USR_Invitations_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[KW_TripForms]  WITH CHECK ADD CONSTRAINT [FK_KW_TripForms_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[MetricsConst]  WITH CHECK ADD CONSTRAINT [FK_MetricsConst_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[KW_Companies]  WITH CHECK ADD CONSTRAINT [FK_KW_Companies_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[CN_ListNodes]  WITH CHECK ADD CONSTRAINT [FK_CN_ListNodes_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[TMP_KW_Questions]  WITH CHECK ADD CONSTRAINT [FK_TMP_KW_Questions_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[FG_ElementLimits]  WITH CHECK ADD CONSTRAINT [FK_FG_ElementLimits_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[KW_RelatedNodes]  WITH CHECK ADD CONSTRAINT [FK_KW_RelatedNodes_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[TMP_KW_TypeQuestions]  WITH CHECK ADD CONSTRAINT [FK_TMP_KW_TypeQuestions_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[GapColorConst]  WITH CHECK ADD CONSTRAINT [FK_GapColorConst_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[SH_PostTypes]  WITH CHECK ADD CONSTRAINT [FK_SH_PostTypes_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[PRVC_PrivacyType]  WITH CHECK ADD CONSTRAINT [FK_PRVC_PrivacyType_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[KW_KnowledgeManagers]  WITH CHECK ADD CONSTRAINT [FK_KW_KnowledgeManagers_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[SH_Posts]  WITH CHECK ADD CONSTRAINT [FK_SH_Posts_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[WF_StateDataNeedInstances]  WITH CHECK ADD CONSTRAINT [FK_WF_StateDataNeedInstances_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[KW_ExperienceHolders]  WITH CHECK ADD CONSTRAINT [FK_KW_ExperienceHolders_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[TMP_KW_CandidateRelations]  WITH CHECK ADD CONSTRAINT [FK_TMP_KW_CandidateRelations_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[FG_InstanceElements]  WITH CHECK ADD CONSTRAINT [FK_FG_InstanceElements_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[WK_Titles]  WITH CHECK ADD CONSTRAINT [FK_WK_Titles_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[Countries]  WITH CHECK ADD CONSTRAINT [FK_Countries_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[ProfileScientific]  WITH CHECK ADD CONSTRAINT [FK_ProfileScientific_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[KW_FeedBacks]  WITH CHECK ADD CONSTRAINT [FK_KW_FeedBacks_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[WF_AutoMessages]  WITH CHECK ADD CONSTRAINT [FK_WF_AutoMessages_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[ProfileJobs]  WITH CHECK ADD CONSTRAINT [FK_ProfileJobs_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[ProfileInstitute]  WITH CHECK ADD CONSTRAINT [FK_ProfileInstitute_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[TMP_KW_QuestionAnswers]  WITH CHECK ADD CONSTRAINT [FK_TMP_KW_QuestionAnswers_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[NTFN_Dashboards]  WITH CHECK ADD CONSTRAINT [FK_NTFN_Dashboards_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[ProfileEducation]  WITH CHECK ADD CONSTRAINT [FK_ProfileEducation_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[KWF_Managers]  WITH CHECK ADD CONSTRAINT [FK_KWF_Managers_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[RV_DeletedStates]  WITH CHECK ADD CONSTRAINT [FK_RV_DeletedStates_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[SH_Comments]  WITH CHECK ADD CONSTRAINT [FK_SH_Comments_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[TMP_KW_History]  WITH CHECK ADD CONSTRAINT [FK_TMP_KW_History_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[CN_NodeMembers]  WITH CHECK ADD CONSTRAINT [FK_CN_NodeMembers_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[FG_FormInstances]  WITH CHECK ADD CONSTRAINT [FK_FG_FormInstances_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[FG_ExtendedForms]  WITH CHECK ADD CONSTRAINT [FK_FG_ExtendedForms_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[KWF_Paraphs]  WITH CHECK ADD CONSTRAINT [FK_KWF_Paraphs_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[KWF_Experts]  WITH CHECK ADD CONSTRAINT [FK_KWF_Experts_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[SH_ShareLikes]  WITH CHECK ADD CONSTRAINT [FK_SH_ShareLikes_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[RV_Variables]  WITH CHECK ADD CONSTRAINT [FK_RV_Variables_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[TMP_KW_TempKnowledgeTypeIDs]  WITH CHECK ADD CONSTRAINT [FK_TMP_KW_TempKnowledgeTypeIDs_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[SH_CommentLikes]  WITH CHECK ADD CONSTRAINT [FK_SH_CommentLikes_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[CN_Experts]  WITH CHECK ADD CONSTRAINT [FK_CN_Experts_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[WK_Paragraphs]  WITH CHECK ADD CONSTRAINT [FK_WK_Paragraphs_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[TMP_KW_FeedBacks]  WITH CHECK ADD CONSTRAINT [FK_TMP_KW_FeedBacks_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[WF_WorkFlowOwners]  WITH CHECK ADD CONSTRAINT [FK_WF_WorkFlowOwners_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[CN_Properties]  WITH CHECK ADD CONSTRAINT [FK_CN_Properties_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[CN_ExpertiseReferrals]  WITH CHECK ADD CONSTRAINT [FK_CN_ExpertiseReferrals_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[KWF_Evaluators]  WITH CHECK ADD CONSTRAINT [FK_KWF_Evaluators_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[WF_States]  WITH CHECK ADD CONSTRAINT [FK_WF_States_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[QA_Questions]  WITH CHECK ADD CONSTRAINT [FK_QA_Questions_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[WK_Changes]  WITH CHECK ADD CONSTRAINT [FK_WK_Changes_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[CN_NodeLikes]  WITH CHECK ADD CONSTRAINT [FK_CN_NodeLikes_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[CN_NodeProperties]  WITH CHECK ADD CONSTRAINT [FK_CN_NodeProperties_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[WF_WorkFlows]  WITH CHECK ADD CONSTRAINT [FK_WF_WorkFlows_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[QA_Answers]  WITH CHECK ADD CONSTRAINT [FK_QA_Answers_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[AttachmentFiles]  WITH CHECK ADD CONSTRAINT [FK_AttachmentFiles_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[WF_StateConnections]  WITH CHECK ADD CONSTRAINT [FK_WF_StateConnections_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[PRVC_Audience]  WITH CHECK ADD CONSTRAINT [FK_PRVC_Audience_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[NGeneralQuestions]  WITH CHECK ADD CONSTRAINT [FK_NGeneralQuestions_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[CN_NodeRelations]  WITH CHECK ADD CONSTRAINT [FK_CN_NodeRelations_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[USR_PassResetTickets]  WITH CHECK ADD CONSTRAINT [FK_USR_PassResetTickets_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[EVT_Events]  WITH CHECK ADD CONSTRAINT [FK_EVT_Events_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[USR_Profile]  WITH CHECK ADD CONSTRAINT [FK_USR_Profile_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[UserGroupAccessRoles]  WITH CHECK ADD CONSTRAINT [FK_UserGroupAccessRoles_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[WF_StateConnectionForms]  WITH CHECK ADD CONSTRAINT [FK_WF_StateConnectionForms_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[QA_QuestionLikes]  WITH CHECK ADD CONSTRAINT [FK_QA_QuestionLikes_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[USR_LanguageNames]  WITH CHECK ADD CONSTRAINT [FK_USR_LanguageNames_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[EVT_RelatedUsers]  WITH CHECK ADD CONSTRAINT [FK_EVT_RelatedUsers_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[USR_UserLanguages]  WITH CHECK ADD CONSTRAINT [FK_USR_UserLanguages_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[Cities]  WITH CHECK ADD CONSTRAINT [FK_Cities_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[CN_ListAdmins]  WITH CHECK ADD CONSTRAINT [FK_CN_ListAdmins_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[QA_RefNodes]  WITH CHECK ADD CONSTRAINT [FK_QA_RefNodes_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[USR_ItemVisits]  WITH CHECK ADD CONSTRAINT [FK_USR_ItemVisits_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[NTFN_NotificationMessageTemplates]  WITH CHECK ADD CONSTRAINT [FK_NTFN_NotificationMessageTemplates_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[CN_Tags]  WITH CHECK ADD CONSTRAINT [FK_CN_Tags_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[WF_WorkFlowStates]  WITH CHECK ADD CONSTRAINT [FK_WF_WorkFlowStates_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[EVT_RelatedNodes]  WITH CHECK ADD CONSTRAINT [FK_EVT_RelatedNodes_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[WF_HistoryFormInstances]  WITH CHECK ADD CONSTRAINT [FK_WF_HistoryFormInstances_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[QA_RefUsers]  WITH CHECK ADD CONSTRAINT [FK_QA_RefUsers_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[USR_JobExperiences]  WITH CHECK ADD CONSTRAINT [FK_USR_JobExperiences_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[RV_EmailQueue]  WITH CHECK ADD CONSTRAINT [FK_RV_EmailQueue_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[USR_EmailContacts]  WITH CHECK ADD CONSTRAINT [FK_USR_EmailContacts_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[NodeSetting]  WITH CHECK ADD CONSTRAINT [FK_NodeSetting_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[NTFN_UserMessagingActivation]  WITH CHECK ADD CONSTRAINT [FK_NTFN_UserMessagingActivation_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[RV_SentEmails]  WITH CHECK ADD CONSTRAINT [FK_RV_SentEmails_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[USR_Friends]  WITH CHECK ADD CONSTRAINT [FK_USR_Friends_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[USR_EducationalExperiences]  WITH CHECK ADD CONSTRAINT [FK_USR_EducationalExperiences_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[CN_NodeTypes]  WITH CHECK ADD CONSTRAINT [FK_CN_NodeTypes_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO
ALTER TABLE [dbo].[DCT_TreeTypes]  WITH CHECK ADD CONSTRAINT [FK_DCT_TreeTypes_aspnet_Applications] FOREIGN KEY([ApplicationID]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId]) 
GO