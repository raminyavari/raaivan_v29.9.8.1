/****** Object:  StoredProcedure [dbo].[AddFolder]    Script Date: 03/14/2012 11:38:59 ******/
USE [EKM_App]

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CalendarKnowledges_KKnowledges]') AND parent_object_id = OBJECT_ID(N'[dbo].[CalendarKnowledges]'))
ALTER TABLE [dbo].[CalendarKnowledges] DROP CONSTRAINT [FK_CalendarKnowledges_KKnowledges]
GO

ALTER TABLE [dbo].[CalendarKnowledges]  WITH CHECK ADD  CONSTRAINT [FK_CalendarKnowledges_KW_Knowledges] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[KW_Knowledges] ([KnowledgeID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[CalendarKnowledges] CHECK CONSTRAINT [FK_CalendarKnowledges_KW_Knowledges]
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_KWF_Paraphs_KKnowledges]') AND parent_object_id = OBJECT_ID(N'[dbo].[KWF_Paraphs]'))
ALTER TABLE [dbo].[KWF_Paraphs] DROP CONSTRAINT [FK_KWF_Paraphs_KKnowledges]
GO

ALTER TABLE [dbo].[KWF_Paraphs]  WITH CHECK ADD  CONSTRAINT [FK_KWF_Paraphs_KW_Knowledges] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[KW_Knowledges] ([KnowledgeID])
GO

ALTER TABLE [dbo].[KWF_Paraphs] CHECK CONSTRAINT [FK_KWF_Paraphs_KW_Knowledges]
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AddedForms_ContentTreeNodes]') AND parent_object_id = OBJECT_ID(N'[dbo].[AddedForms]'))
ALTER TABLE [dbo].[AddedForms] DROP CONSTRAINT [FK_AddedForms_ContentTreeNodes]
GO

ALTER TABLE [dbo].[AddedForms]  WITH CHECK ADD  CONSTRAINT [FK_AddedForms_DCT_TreeNodes] FOREIGN KEY([TreeNodeID])
REFERENCES [dbo].[DCT_TreeNodes] ([TreeNodeID])
ON UPDATE SET NULL
ON DELETE SET NULL
GO

ALTER TABLE [dbo].[AddedForms] CHECK CONSTRAINT [FK_AddedForms_DCT_TreeNodes]
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_NQuestions_KKnowledgeType]') AND parent_object_id = OBJECT_ID(N'[dbo].[NQuestions]'))
ALTER TABLE [dbo].[NQuestions] DROP CONSTRAINT [FK_NQuestions_KKnowledgeType]
GO

ALTER TABLE [dbo].[NQuestions]
ADD [KnowledgeTypeID] [int] NULL
GO

UPDATE [dbo].[NQuestions]
	SET KnowledgeTypeID = KnowledgeType
GO

ALTER TABLE [dbo].[NQuestions]
DROP COLUMN [KnowledgeType]
GO

ALTER TABLE [dbo].[NQuestions]  WITH CHECK ADD  CONSTRAINT [FK_NQuestions_KW_KnowledgeTypes] FOREIGN KEY([KnowledgeTypeID])
REFERENCES [dbo].[KW_KnowledgeTypes] ([KnowledgeTypeID])
GO

ALTER TABLE [dbo].[NQuestions] CHECK CONSTRAINT [FK_NQuestions_KW_KnowledgeTypes]
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_NGeneralQuestions_KKnowledgeType]') AND parent_object_id = OBJECT_ID(N'[dbo].[NGeneralQuestions]'))
ALTER TABLE [dbo].[NGeneralQuestions] DROP CONSTRAINT [FK_NGeneralQuestions_KKnowledgeType]
GO

ALTER TABLE [dbo].[NGeneralQuestions]
ADD [KnowledgeTypeID] [int] NULL
GO

UPDATE [dbo].[NGeneralQuestions]
	SET KnowledgeTypeID = KnowledgeType
GO

ALTER TABLE [dbo].[NGeneralQuestions]
DROP COLUMN [KnowledgeType]
GO

ALTER TABLE [dbo].[NGeneralQuestions]  WITH CHECK ADD  CONSTRAINT [FK_NGeneralQuestions_KW_KnowledgeTypes] FOREIGN KEY([KnowledgeTypeID])
REFERENCES [dbo].[KW_KnowledgeTypes] ([KnowledgeTypeID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[NGeneralQuestions] CHECK CONSTRAINT [FK_NGeneralQuestions_KW_KnowledgeTypes]
GO


DROP TABLE [dbo].[KContentTreeNodeContents]
GO


DROP VIEW [dbo].[ContentFileExtensions]
GO


DROP TABLE [dbo].[ContentTreeNodes]
GO


DROP TABLE [dbo].[ContentTrees]
GO


DROP TABLE [dbo].[ForumPosts]
GO


DROP TABLE [dbo].[ForumTopics]
GO


DROP TABLE [dbo].[ForumSections]
GO


DROP TABLE [dbo].[KContentUsers]
GO


DROP TABLE [dbo].[KContentVersions]
GO


DROP TABLE [dbo].[KExperiences]
GO


DROP TABLE [dbo].[Keywords]
GO


DROP TABLE [dbo].[KKnowledgeRelations]
GO


DROP TABLE [dbo].[KLearningMethodaspnet_Users]
GO


DROP TABLE [dbo].[KLearningMethods]
GO


DROP TABLE [dbo].[KnowledgeCards]
GO


DROP TABLE [dbo].[KnowledgeDepartments]
GO


DROP TABLE [dbo].[KnowledgeNodes]
GO


DROP TABLE [dbo].[KSkills]
GO


DROP TABLE [dbo].[KTripFormCompanies]
GO


DROP TABLE [dbo].[KContentTripForm]
GO


DROP TABLE [dbo].[KContents]
GO


DROP TABLE [dbo].[KKnowledges]
GO


DROP TABLE [dbo].[KKnowledgeType]
GO


DROP TABLE [dbo].[UsersClassifications]
GO


DROP TABLE [dbo].[Classifications]
GO


DROP TABLE [dbo].[BulletinBoards]
GO
