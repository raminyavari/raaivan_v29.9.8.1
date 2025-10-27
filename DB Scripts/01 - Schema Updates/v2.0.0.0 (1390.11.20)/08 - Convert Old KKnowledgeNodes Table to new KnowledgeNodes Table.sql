USE [EKM_APP]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


INSERT INTO [dbo].[KnowledgeNodes]
           ([KnowledgeID]
           ,[NodeID]
           ,[Score]
           ,[ScoresWeight])
SELECT DISTINCT	KKnowledgeID, NodeID, 0, 0
FROM    dbo.KKnowledgeNodes
WHERE KKnowledgeID > '00000000-0000-0000-0000-000000000000' AND NodeID > 0
GO

/* Drop [dbo].[KKnowledgeNodes] Table */
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_KKnowledgeNodes_KKnowledges]') AND parent_object_id = OBJECT_ID(N'[dbo].[KKnowledgeNodes]'))
ALTER TABLE [dbo].[KKnowledgeNodes] DROP CONSTRAINT [FK_KKnowledgeNodes_KKnowledges]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_KKnowledgeNodes_Nodes]') AND parent_object_id = OBJECT_ID(N'[dbo].[KKnowledgeNodes]'))
ALTER TABLE [dbo].[KKnowledgeNodes] DROP CONSTRAINT [FK_KKnowledgeNodes_Nodes]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[KKnowledgeNodes]') AND type in (N'U'))
DROP TABLE [dbo].[KKnowledgeNodes]
GO


