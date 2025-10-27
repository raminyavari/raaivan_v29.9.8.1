USE [EKM_App]
GO


IF NOT EXISTS(SELECT * 
FROM sys.indexes WHERE name='IX_KW_CreatorUsers_UserID' AND object_id = OBJECT_ID('KW_CreatorUsers'))
CREATE NONCLUSTERED INDEX [IX_KW_CreatorUsers_UserID] ON [dbo].[KW_CreatorUsers] 
(
	[UserID] ASC,
	[KnowledgeID] ASC,
	[CollaborationShare] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


IF NOT EXISTS(SELECT * 
FROM sys.indexes WHERE name='IX_KW_Experts_UserID' AND object_id = OBJECT_ID('KW_Experts'))
CREATE NONCLUSTERED INDEX [IX_KW_Experts_UserID] ON [dbo].[KW_Experts] 
(
	[UserID] ASC,
	[NodeID] ASC,
	[ExpertiseLevelID] ASC,
	[Deleted] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


IF NOT EXISTS(SELECT * 
FROM sys.indexes WHERE name='IX_KW_ExtendedForms_FormTypeID' AND object_id = OBJECT_ID('KW_ExtendedForms'))
CREATE NONCLUSTERED INDEX [IX_KW_ExtendedForms_FormTypeID] ON [dbo].[KW_ExtendedForms] 
(
	[FormTypeID] ASC,
	[KnowledgeTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


IF NOT EXISTS(SELECT * 
FROM sys.indexes WHERE name='IX_KW_KnowledgeAssets_KnowledgeID' AND object_id = OBJECT_ID('KW_KnowledgeAssets'))
CREATE NONCLUSTERED INDEX [IX_KW_KnowledgeAssets_KnowledgeID] ON [dbo].[KW_KnowledgeAssets] 
(
	[KnowledgeID] ASC,
	[UserID] ASC,
	[TheoricalLevelID] ASC,
	[PracticalLevelID] ASC,
	[Deleted] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


IF NOT EXISTS(SELECT * 
FROM sys.indexes WHERE name='IX_KW_KnowledgeCards_SenderUserID' AND object_id = OBJECT_ID('KW_KnowledgeCards'))
CREATE NONCLUSTERED INDEX [IX_KW_KnowledgeCards_SenderUserID] ON [dbo].[KW_KnowledgeCards] 
(
	[SenderUserID] ASC,
	[ReceiverUserID] ASC,
	[SendDate] ASC,
	[Deleted] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


IF NOT EXISTS(SELECT * 
FROM sys.indexes WHERE name='IX_KW_KnowledgeCards_ReceiverUserID' AND object_id = OBJECT_ID('KW_KnowledgeCards'))
CREATE NONCLUSTERED INDEX [IX_KW_KnowledgeCards_ReceiverUserID] ON [dbo].[KW_KnowledgeCards] 
(
	[ReceiverUserID] ASC,
	[SenderUserID] ASC,
	[SendDate] ASC,
	[Deleted] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


IF NOT EXISTS(SELECT * 
FROM sys.indexes WHERE name='IX_KW_View_Knowledges_TreeNodeID' AND object_id = OBJECT_ID('KW_View_Knowledges'))
CREATE NONCLUSTERED INDEX [IX_KW_View_Knowledges_TreeNodeID] ON [dbo].[KW_View_Knowledges] 
(
	[TreeNodeID] ASC,
	[StatusID] ASC,
	[IsDefault] ASC,
	[ContentType] ASC,
	[KnowledgeID] ASC,
	[ConfidentialityLevelID] ASC,
	[Deleted] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


IF NOT EXISTS(SELECT * 
FROM sys.indexes WHERE name='IX_KW_View_Knowledges_CreatorUserID' AND object_id = OBJECT_ID('KW_View_Knowledges'))
CREATE NONCLUSTERED INDEX [IX_KW_View_Knowledges_CreatorUserID] ON [dbo].[KW_View_Knowledges] 
(
	[CreatorUserID] ASC,
	[StatusID] ASC,
	[IsDefault] ASC,
	[KnowledgeTypeID] ASC,
	[ContentType] ASC,
	[KnowledgeID] ASC,
	[ConfidentialityLevelID] ASC,
	[Deleted] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


IF NOT EXISTS(SELECT * 
FROM sys.indexes WHERE name='IX_KW_RefrenceUsers_UserID' AND object_id = OBJECT_ID('KW_RefrenceUsers'))
CREATE NONCLUSTERED INDEX [IX_KW_RefrenceUsers_UserID] ON [dbo].[KW_RefrenceUsers] 
(
	[UserID] ASC,
	[KnowledgeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


IF NOT EXISTS(SELECT * 
FROM sys.indexes WHERE name='IX_KW_RelatedDepartments_DepartmentID' AND object_id = OBJECT_ID('KW_RelatedDepartments'))
CREATE NONCLUSTERED INDEX [IX_KW_RelatedDepartments_DepartmentID] ON [dbo].[KW_RelatedDepartments] 
(
	[DepartmentID] ASC,
	[KnowledgeID] ASC,
	[Deleted] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


IF NOT EXISTS(SELECT * 
FROM sys.indexes WHERE name='IX_KW_RelatedKnowledges_RelatedKnowledgeID' AND object_id = OBJECT_ID('KW_RelatedKnowledges'))
CREATE NONCLUSTERED INDEX [IX_KW_RelatedKnowledges_RelatedKnowledgeID] ON [dbo].[KW_RelatedKnowledges] 
(
	[RelatedKnowledgeID] ASC,
	[KnowledgeID] ASC,
	[Deleted] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


IF NOT EXISTS(SELECT * 
FROM sys.indexes WHERE name='IX_KW_RelatedNodes_NodeID' AND object_id = OBJECT_ID('KW_RelatedNodes'))
CREATE NONCLUSTERED INDEX [IX_KW_RelatedNodes_NodeID] ON [dbo].[KW_RelatedNodes] 
(
	[NodeID] ASC,
	[KnowledgeID] ASC,
	[Deleted] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


IF NOT EXISTS(SELECT * 
FROM sys.indexes WHERE name='IX_KW_UsersConfidentialityLevels_LevelID' AND object_id = OBJECT_ID('KW_UsersConfidentialityLevels'))
CREATE NONCLUSTERED INDEX [IX_KW_UsersConfidentialityLevels_LevelID] ON [dbo].[KW_UsersConfidentialityLevels] 
(
	[LevelID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


IF NOT EXISTS(SELECT * 
FROM sys.indexes WHERE name='IX_KWF_Evaluators_UserID' AND object_id = OBJECT_ID('KWF_Evaluators'))
CREATE NONCLUSTERED INDEX [IX_KWF_Evaluators_UserID] ON [dbo].[KWF_Evaluators] 
(
	[UserID] ASC,
	[KnowledgeID] ASC,
	[Evaluated] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


IF NOT EXISTS(SELECT * 
FROM sys.indexes WHERE name='IX_KWF_Experts_UserID' AND object_id = OBJECT_ID('KWF_Experts'))
CREATE NONCLUSTERED INDEX [IX_KWF_Experts_UserID] ON [dbo].[KWF_Experts] 
(
	[UserID] ASC,
	[KnowledgeID] ASC,
	[NodeID] ASC,
	[Evaluated] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


IF NOT EXISTS(SELECT * 
FROM sys.indexes WHERE name='IX_KWF_Experts_NodeID' AND object_id = OBJECT_ID('KWF_Experts'))
CREATE NONCLUSTERED INDEX [IX_KWF_Experts_NodeID] ON [dbo].[KWF_Experts] 
(
	[NodeID] ASC,
	[UserID] ASC,
	[KnowledgeID] ASC,
	[Evaluated] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


IF NOT EXISTS(SELECT * 
FROM sys.indexes WHERE name='IX_KWF_Managers_UserID' AND object_id = OBJECT_ID('KWF_Managers'))
CREATE NONCLUSTERED INDEX [IX_KWF_Managers_UserID] ON [dbo].[KWF_Managers] 
(
	[UserID] ASC,
	[KnowledgeID] ASC,
	[EntranceDate] ASC,
	[Sent] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


IF NOT EXISTS(SELECT * 
FROM sys.indexes WHERE name='IX_KWF_Paraphs_KnowledgeID' AND object_id = OBJECT_ID('KWF_Paraphs'))
CREATE NONCLUSTERED INDEX [IX_KWF_Paraphs_KnowledgeID] ON [dbo].[KWF_Paraphs] 
(
	[KnowledgeID] ASC,
	[ParaphID] ASC,
	[UserID] ASC,
	[ParaphDate] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO