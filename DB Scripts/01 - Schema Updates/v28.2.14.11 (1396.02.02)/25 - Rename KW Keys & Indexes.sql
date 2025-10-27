USE [EKM_App]
GO



ALTER TABLE [dbo].[KW_AnswerOptions]  
DROP CONSTRAINT [FK_TMP_KW_AnswerOptions_aspnet_Applications]
GO

ALTER TABLE [dbo].[KW_AnswerOptions] 
DROP CONSTRAINT [FK_TMP_KW_AnswerOptions_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[KW_AnswerOptions] 
DROP CONSTRAINT [FK_TMP_KW_AnswerOptions_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[KW_AnswerOptions] 
DROP CONSTRAINT [FK_TMP_KW_AnswerOptions_TMP_KW_TypeQuestions]
GO

ALTER TABLE [dbo].[KW_CandidateRelations] 
DROP CONSTRAINT [FK_TMP_KW_CandidateRelations_aspnet_Applications]
GO

ALTER TABLE [dbo].[KW_CandidateRelations] 
DROP CONSTRAINT [FK_TMP_KW_CandidateRelations_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[KW_CandidateRelations] 
DROP CONSTRAINT [FK_TMP_KW_CandidateRelations_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[KW_CandidateRelations] 
DROP CONSTRAINT [FK_TMP_KW_CandidateRelations_CN_Nodes]
GO

ALTER TABLE [dbo].[KW_CandidateRelations] 
DROP CONSTRAINT [FK_TMP_KW_CandidateRelations_CN_NodeTypes]
GO

ALTER TABLE [dbo].[KW_CandidateRelations] 
DROP CONSTRAINT [FK_TMP_KW_CandidateRelations_TMP_KW_KnowledgeTypes]
GO

ALTER TABLE [dbo].[KW_FeedBacks] 
DROP CONSTRAINT [FK_TMP_KW_FeedBacks_aspnet_Applications]
GO

ALTER TABLE [dbo].[KW_FeedBacks] 
DROP CONSTRAINT [FK_TMP_KW_FeedBacks_aspnet_Users]
GO

ALTER TABLE [dbo].[KW_FeedBacks] 
DROP CONSTRAINT [FK_TMP_KW_FeedBacks_CN_Nodes]
GO

ALTER TABLE [dbo].[KW_History] 
DROP CONSTRAINT [FK_TMP_KW_History_aspnet_Applications]
GO

ALTER TABLE [dbo].[KW_History] 
DROP CONSTRAINT [FK_TMP_KW_History_aspnet_Users]
GO

ALTER TABLE [dbo].[KW_History] 
DROP CONSTRAINT [FK_TMP_KW_History_CN_Nodes]
GO

ALTER TABLE [dbo].[KW_KnowledgeTypes] 
DROP CONSTRAINT [FK_TMP_KW_KnowledgeTypes_aspnet_Applications]
GO

ALTER TABLE [dbo].[KW_KnowledgeTypes] 
DROP CONSTRAINT [FK_TMP_KW_KnowledgeTypes_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[KW_KnowledgeTypes] 
DROP CONSTRAINT [FK_TMP_KW_KnowledgeTypes_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[KW_KnowledgeTypes] 
DROP CONSTRAINT [FK_TMP_KW_KnowledgeTypes_CN_NodeTypes]
GO

ALTER TABLE [dbo].[KW_NecessaryItems] 
DROP CONSTRAINT [FK_TMP_KW_NecessaryItems_aspnet_Applications]
GO

ALTER TABLE [dbo].[KW_NecessaryItems] 
DROP CONSTRAINT [FK_TMP_KW_NecessaryItems_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[KW_NecessaryItems] 
DROP CONSTRAINT [FK_TMP_KW_NecessaryItems_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[KW_QuestionAnswers] 
DROP CONSTRAINT [FK_TMP_KW_QuestionAnswers_aspnet_Applications]
GO

ALTER TABLE [dbo].[KW_QuestionAnswers] 
DROP CONSTRAINT [FK_TMP_KW_QuestionAnswers_aspnet_Users]
GO

ALTER TABLE [dbo].[KW_QuestionAnswers] 
DROP CONSTRAINT [FK_TMP_KW_QuestionAnswers_CN_Nodes]
GO

ALTER TABLE [dbo].[KW_QuestionAnswers] 
DROP CONSTRAINT [FK_TMP_KW_QuestionAnswers_TMP_KW_Questions]
GO

ALTER TABLE [dbo].[KW_Questions] 
DROP CONSTRAINT [FK_TMP_KW_Questions_aspnet_Applications]
GO

ALTER TABLE [dbo].[KW_Questions] 
DROP CONSTRAINT [FK_TMP_KW_Questions_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[KW_Questions] 
DROP CONSTRAINT [FK_TMP_KW_Questions_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[KW_TempKnowledgeTypeIDs] 
DROP CONSTRAINT [FK_TMP_KW_TempKnowledgeTypeIDs_aspnet_Applications]
GO

ALTER TABLE [dbo].[KW_TypeQuestions] 
DROP CONSTRAINT [FK_TMP_KW_TypeQuestions_aspnet_Applications]
GO

ALTER TABLE [dbo].[KW_TypeQuestions] 
DROP CONSTRAINT [FK_TMP_KW_TypeQuestions_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[KW_TypeQuestions] 
DROP CONSTRAINT [FK_TMP_KW_TypeQuestions_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[KW_TypeQuestions] 
DROP CONSTRAINT [FK_TMP_KW_TypeQuestions_CN_Nodes]
GO

ALTER TABLE [dbo].[KW_TypeQuestions] 
DROP CONSTRAINT [FK_TMP_KW_TypeQuestions_TMP_KW_KnowledgeTypes]
GO

ALTER TABLE [dbo].[KW_TypeQuestions] 
DROP CONSTRAINT [FK_TMP_KW_TypeQuestions_TMP_KW_Questions]
GO







ALTER TABLE [dbo].[KW_AnswerOptions] 
DROP CONSTRAINT [PK_TMP_KW_AnswerOptions]
GO

ALTER TABLE [dbo].[KW_CandidateRelations] 
DROP CONSTRAINT [PK_TMP_KW_CandidateRelations]
GO

ALTER TABLE [dbo].[KW_FeedBacks] 
DROP CONSTRAINT [PK_TMP_KW_FeedBacks]
GO

ALTER TABLE [dbo].[KW_History] 
DROP CONSTRAINT [PK_TMP_KW_History]
GO

ALTER TABLE [dbo].[KW_KnowledgeTypes] 
DROP CONSTRAINT [PK_TMP_KW_KnowledgeTypes]
GO

ALTER TABLE [dbo].[KW_NecessaryItems] 
DROP CONSTRAINT [PK_TMP_KW_NecessaryItems]
GO

ALTER TABLE [dbo].[KW_QuestionAnswers] 
DROP CONSTRAINT [PK_TMP_KW_QuestionAnswers]
GO

ALTER TABLE [dbo].[KW_TempKnowledgeTypeIDs] 
DROP CONSTRAINT [PK_TMP_KW_TempKnowledgeTypeIDs]
GO

ALTER TABLE [dbo].[KW_TypeQuestions] 
DROP CONSTRAINT [PK_TMP_KW_TypeQuestions]
GO






ALTER TABLE [dbo].[KW_AnswerOptions] ADD  CONSTRAINT [PK_KW_AnswerOptions] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

ALTER TABLE [dbo].[KW_CandidateRelations] ADD  CONSTRAINT [PK_KW_CandidateRelations] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

ALTER TABLE [dbo].[KW_FeedBacks] ADD  CONSTRAINT [PK_KW_FeedBacks] PRIMARY KEY CLUSTERED 
(
	[FeedBackID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

ALTER TABLE [dbo].[KW_History] ADD  CONSTRAINT [PK_KW_History] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

ALTER TABLE [dbo].[KW_KnowledgeTypes] ADD  CONSTRAINT [PK_KW_KnowledgeTypes] PRIMARY KEY CLUSTERED 
(
	[KnowledgeTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

ALTER TABLE [dbo].[KW_NecessaryItems] ADD  CONSTRAINT [PK_KW_NecessaryItems] PRIMARY KEY CLUSTERED 
(
	[NodeTypeID] ASC,
	[ItemName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

ALTER TABLE [dbo].[KW_QuestionAnswers] ADD  CONSTRAINT [PK_KW_QuestionAnswers] PRIMARY KEY CLUSTERED 
(
	[KnowledgeID] ASC,
	[UserID] ASC,
	[QuestionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

ALTER TABLE [dbo].[KW_TempKnowledgeTypeIDs] ADD  CONSTRAINT [PK_KW_TempKnowledgeTypeIDs] PRIMARY KEY CLUSTERED 
(
	[IntID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

ALTER TABLE [dbo].[KW_TypeQuestions] ADD  CONSTRAINT [PK_KW_TypeQuestions] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO




ALTER TABLE [dbo].[KW_AnswerOptions]  WITH CHECK ADD  CONSTRAINT [FK_KW_AnswerOptions_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[KW_AnswerOptions] CHECK CONSTRAINT [FK_KW_AnswerOptions_aspnet_Applications]
GO

ALTER TABLE [dbo].[KW_AnswerOptions]  WITH CHECK ADD  CONSTRAINT [FK_KW_AnswerOptions_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KW_AnswerOptions] CHECK CONSTRAINT [FK_KW_AnswerOptions_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[KW_AnswerOptions]  WITH CHECK ADD  CONSTRAINT [FK_KW_AnswerOptions_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KW_AnswerOptions] CHECK CONSTRAINT [FK_KW_AnswerOptions_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[KW_AnswerOptions]  WITH CHECK ADD  CONSTRAINT [FK_KW_AnswerOptions_KW_TypeQuestions] FOREIGN KEY([TypeQuestionID])
REFERENCES [dbo].[KW_TypeQuestions] ([ID])
GO

ALTER TABLE [dbo].[KW_AnswerOptions] CHECK CONSTRAINT [FK_KW_AnswerOptions_KW_TypeQuestions]
GO

ALTER TABLE [dbo].[KW_CandidateRelations]  WITH CHECK ADD  CONSTRAINT [FK_KW_CandidateRelations_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[KW_CandidateRelations] CHECK CONSTRAINT [FK_KW_CandidateRelations_aspnet_Applications]
GO

ALTER TABLE [dbo].[KW_CandidateRelations]  WITH CHECK ADD  CONSTRAINT [FK_KW_CandidateRelations_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KW_CandidateRelations] CHECK CONSTRAINT [FK_KW_CandidateRelations_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[KW_CandidateRelations]  WITH CHECK ADD  CONSTRAINT [FK_KW_CandidateRelations_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KW_CandidateRelations] CHECK CONSTRAINT [FK_KW_CandidateRelations_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[KW_CandidateRelations]  WITH CHECK ADD  CONSTRAINT [FK_KW_CandidateRelations_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[KW_CandidateRelations] CHECK CONSTRAINT [FK_KW_CandidateRelations_CN_Nodes]
GO

ALTER TABLE [dbo].[KW_CandidateRelations]  WITH CHECK ADD  CONSTRAINT [FK_KW_CandidateRelations_CN_NodeTypes] FOREIGN KEY([NodeTypeID])
REFERENCES [dbo].[CN_NodeTypes] ([NodeTypeID])
GO

ALTER TABLE [dbo].[KW_CandidateRelations] CHECK CONSTRAINT [FK_KW_CandidateRelations_CN_NodeTypes]
GO

ALTER TABLE [dbo].[KW_CandidateRelations]  WITH CHECK ADD  CONSTRAINT [FK_KW_CandidateRelations_KW_KnowledgeTypes] FOREIGN KEY([KnowledgeTypeID])
REFERENCES [dbo].[KW_KnowledgeTypes] ([KnowledgeTypeID])
GO

ALTER TABLE [dbo].[KW_CandidateRelations] CHECK CONSTRAINT [FK_KW_CandidateRelations_KW_KnowledgeTypes]
GO

ALTER TABLE [dbo].[KW_FeedBacks]  WITH CHECK ADD  CONSTRAINT [FK_KW_FeedBacks_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[KW_FeedBacks] CHECK CONSTRAINT [FK_KW_FeedBacks_aspnet_Applications]
GO

ALTER TABLE [dbo].[KW_FeedBacks]  WITH CHECK ADD  CONSTRAINT [FK_KW_FeedBacks_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KW_FeedBacks] CHECK CONSTRAINT [FK_KW_FeedBacks_aspnet_Users]
GO

ALTER TABLE [dbo].[KW_FeedBacks]  WITH CHECK ADD  CONSTRAINT [FK_KW_FeedBacks_CN_Nodes] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[KW_FeedBacks] CHECK CONSTRAINT [FK_KW_FeedBacks_CN_Nodes]
GO

ALTER TABLE [dbo].[KW_History]  WITH CHECK ADD  CONSTRAINT [FK_KW_History_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[KW_History] CHECK CONSTRAINT [FK_KW_History_aspnet_Applications]
GO

ALTER TABLE [dbo].[KW_History]  WITH CHECK ADD  CONSTRAINT [FK_KW_History_aspnet_Users] FOREIGN KEY([ActorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KW_History] CHECK CONSTRAINT [FK_KW_History_aspnet_Users]
GO

ALTER TABLE [dbo].[KW_History]  WITH CHECK ADD  CONSTRAINT [FK_KW_History_CN_Nodes] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[KW_History] CHECK CONSTRAINT [FK_KW_History_CN_Nodes]
GO

ALTER TABLE [dbo].[KW_KnowledgeTypes]  WITH CHECK ADD  CONSTRAINT [FK_KW_KnowledgeTypes_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[KW_KnowledgeTypes] CHECK CONSTRAINT [FK_KW_KnowledgeTypes_aspnet_Applications]
GO

ALTER TABLE [dbo].[KW_KnowledgeTypes]  WITH CHECK ADD  CONSTRAINT [FK_KW_KnowledgeTypes_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KW_KnowledgeTypes] CHECK CONSTRAINT [FK_KW_KnowledgeTypes_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[KW_KnowledgeTypes]  WITH CHECK ADD  CONSTRAINT [FK_KW_KnowledgeTypes_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KW_KnowledgeTypes] CHECK CONSTRAINT [FK_KW_KnowledgeTypes_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[KW_KnowledgeTypes]  WITH CHECK ADD  CONSTRAINT [FK_KW_KnowledgeTypes_CN_NodeTypes] FOREIGN KEY([KnowledgeTypeID])
REFERENCES [dbo].[CN_NodeTypes] ([NodeTypeID])
GO

ALTER TABLE [dbo].[KW_KnowledgeTypes] CHECK CONSTRAINT [FK_KW_KnowledgeTypes_CN_NodeTypes]
GO

ALTER TABLE [dbo].[KW_NecessaryItems]  WITH CHECK ADD  CONSTRAINT [FK_KW_NecessaryItems_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[KW_NecessaryItems] CHECK CONSTRAINT [FK_KW_NecessaryItems_aspnet_Applications]
GO

ALTER TABLE [dbo].[KW_NecessaryItems]  WITH CHECK ADD  CONSTRAINT [FK_KW_NecessaryItems_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KW_NecessaryItems] CHECK CONSTRAINT [FK_KW_NecessaryItems_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[KW_NecessaryItems]  WITH CHECK ADD  CONSTRAINT [FK_KW_NecessaryItems_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KW_NecessaryItems] CHECK CONSTRAINT [FK_KW_NecessaryItems_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[KW_QuestionAnswers]  WITH CHECK ADD  CONSTRAINT [FK_KW_QuestionAnswers_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[KW_QuestionAnswers] CHECK CONSTRAINT [FK_KW_QuestionAnswers_aspnet_Applications]
GO

ALTER TABLE [dbo].[KW_QuestionAnswers]  WITH CHECK ADD  CONSTRAINT [FK_KW_QuestionAnswers_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KW_QuestionAnswers] CHECK CONSTRAINT [FK_KW_QuestionAnswers_aspnet_Users]
GO

ALTER TABLE [dbo].[KW_QuestionAnswers]  WITH CHECK ADD  CONSTRAINT [FK_KW_QuestionAnswers_CN_Nodes] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[KW_QuestionAnswers] CHECK CONSTRAINT [FK_KW_QuestionAnswers_CN_Nodes]
GO

ALTER TABLE [dbo].[KW_QuestionAnswers]  WITH CHECK ADD  CONSTRAINT [FK_KW_QuestionAnswers_KW_Questions] FOREIGN KEY([QuestionID])
REFERENCES [dbo].[KW_Questions] ([QuestionID])
GO

ALTER TABLE [dbo].[KW_QuestionAnswers] CHECK CONSTRAINT [FK_KW_QuestionAnswers_KW_Questions]
GO

ALTER TABLE [dbo].[KW_Questions]  WITH CHECK ADD  CONSTRAINT [FK_KW_Questions_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[KW_Questions] CHECK CONSTRAINT [FK_KW_Questions_aspnet_Applications]
GO

ALTER TABLE [dbo].[KW_Questions]  WITH CHECK ADD  CONSTRAINT [FK_KW_Questions_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KW_Questions] CHECK CONSTRAINT [FK_KW_Questions_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[KW_Questions]  WITH CHECK ADD  CONSTRAINT [FK_KW_Questions_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KW_Questions] CHECK CONSTRAINT [FK_KW_Questions_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[KW_TempKnowledgeTypeIDs]  WITH CHECK ADD  CONSTRAINT [FK_KW_TempKnowledgeTypeIDs_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[KW_TempKnowledgeTypeIDs] CHECK CONSTRAINT [FK_KW_TempKnowledgeTypeIDs_aspnet_Applications]
GO

ALTER TABLE [dbo].[KW_TypeQuestions]  WITH CHECK ADD  CONSTRAINT [FK_KW_TypeQuestions_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[KW_TypeQuestions] CHECK CONSTRAINT [FK_KW_TypeQuestions_aspnet_Applications]
GO

ALTER TABLE [dbo].[KW_TypeQuestions]  WITH CHECK ADD  CONSTRAINT [FK_KW_TypeQuestions_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KW_TypeQuestions] CHECK CONSTRAINT [FK_KW_TypeQuestions_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[KW_TypeQuestions]  WITH CHECK ADD  CONSTRAINT [FK_KW_TypeQuestions_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KW_TypeQuestions] CHECK CONSTRAINT [FK_KW_TypeQuestions_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[KW_TypeQuestions]  WITH CHECK ADD  CONSTRAINT [FK_KW_TypeQuestions_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[KW_TypeQuestions] CHECK CONSTRAINT [FK_KW_TypeQuestions_CN_Nodes]
GO

ALTER TABLE [dbo].[KW_TypeQuestions]  WITH CHECK ADD  CONSTRAINT [FK_KW_TypeQuestions_KW_KnowledgeTypes] FOREIGN KEY([KnowledgeTypeID])
REFERENCES [dbo].[KW_KnowledgeTypes] ([KnowledgeTypeID])
GO

ALTER TABLE [dbo].[KW_TypeQuestions] CHECK CONSTRAINT [FK_KW_TypeQuestions_KW_KnowledgeTypes]
GO

ALTER TABLE [dbo].[KW_TypeQuestions]  WITH CHECK ADD  CONSTRAINT [FK_KW_TypeQuestions_KW_Questions] FOREIGN KEY([QuestionID])
REFERENCES [dbo].[KW_Questions] ([QuestionID])
GO

ALTER TABLE [dbo].[KW_TypeQuestions] CHECK CONSTRAINT [FK_KW_TypeQuestions_KW_Questions]
GO