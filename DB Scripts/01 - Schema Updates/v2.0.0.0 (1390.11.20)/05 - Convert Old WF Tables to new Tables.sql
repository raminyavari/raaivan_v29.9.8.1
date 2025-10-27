USE [EKM_APP]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


INSERT INTO [dbo].[KWF_Managers]
           ([KnowledgeID]
           ,[UserID]
           ,[EntranceDate]
           ,[Sent])
	 SELECT DISTINCT dbo.KKnowledgeWFTemplateNodes.KKnowledgeID, dbo.WFTemplateNodes.UserID, '2011-09-24 21:38:34.190', 0
	 FROM   dbo.KKnowledges INNER JOIN
            dbo.KKnowledgeWFTemplateNodes ON dbo.KKnowledges.ID = dbo.KKnowledgeWFTemplateNodes.KKnowledgeID INNER JOIN
            dbo.WFTemplateNodes ON dbo.KKnowledgeWFTemplateNodes.WFTemplateNodeID = dbo.WFTemplateNodes.ID
     WHERE dbo.WFTemplateNodes.ServiceType = 'DepartmantManager' AND
		   dbo.KKnowledgeWFTemplateNodes.KKnowledgeID > '00000000-0000-0000-0000-000000000000' AND
		   dbo.KKnowledges.Status != 'ExpertEvaluation';
GO


INSERT INTO [dbo].[KWF_Managers]
           ([KnowledgeID]
           ,[UserID]
           ,[EntranceDate]
           ,[Sent])
	 SELECT DISTINCT dbo.KKnowledgeWFTemplateNodes.KKnowledgeID, dbo.WFTemplateNodes.UserID, '2011-09-24 21:38:34.190', 1
	 FROM   dbo.KKnowledges INNER JOIN
            dbo.KKnowledgeWFTemplateNodes ON dbo.KKnowledges.ID = dbo.KKnowledgeWFTemplateNodes.KKnowledgeID INNER JOIN
            dbo.WFTemplateNodes ON dbo.KKnowledgeWFTemplateNodes.WFTemplateNodeID = dbo.WFTemplateNodes.ID
     WHERE dbo.WFTemplateNodes.ServiceType = 'DepartmantManager' AND
		   dbo.KKnowledgeWFTemplateNodes.KKnowledgeID > '00000000-0000-0000-0000-000000000000' AND
		   dbo.KKnowledges.Status = 'ExpertEvaluation';
GO


INSERT INTO [dbo].[KWF_Experts]
           ([KnowledgeID]
           ,[UserID]
           ,[NodeID]
           ,[Evaluated]
           ,[Score]
           ,[EntranceDate]
           ,[ExpirationDate])
	 SELECT DISTINCT dbo.KKnowledgeWFRequests.KKnowledgeID, dbo.KRequestKnowledgeDomainExperts.ExpertUserID, dbo.KRequestKnowledgeDomainExperts.NodeID,
					 0, 0, '2011-09-24 21:38:34.190', '2012-09-24 21:38:34.190'
	 FROM dbo.KRequestKnowledgeDomainExperts INNER JOIN
          dbo.KKnowledgeWFRequests ON dbo.KRequestKnowledgeDomainExperts.KKnowledgeWFRequestID = dbo.KKnowledgeWFRequests.WFRequestID INNER JOIN
          dbo.WFRequests ON dbo.KKnowledgeWFRequests.WFRequestID = dbo.WFRequests.ID INNER JOIN
          dbo.WFNodeRecords ON dbo.WFRequests.ID = dbo.WFNodeRecords.WFRequestID
     WHERE dbo.WFNodeRecords.ServiceType = 'Expert' AND
		   dbo.KKnowledgeWFRequests.KKnowledgeID > '00000000-0000-0000-0000-000000000000';
GO


/***** Drop Old Tables *****/

/* [dbo].[KKnowledgeWFTemplateNodes] */
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_KKnowledgeWFTemplateNodes_KKnowledges]') AND parent_object_id = OBJECT_ID(N'[dbo].[KKnowledgeWFTemplateNodes]'))
ALTER TABLE [dbo].[KKnowledgeWFTemplateNodes] DROP CONSTRAINT [FK_KKnowledgeWFTemplateNodes_KKnowledges]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_KKnowledgeWFTemplateNodes_WFTemplateNodes]') AND parent_object_id = OBJECT_ID(N'[dbo].[KKnowledgeWFTemplateNodes]'))
ALTER TABLE [dbo].[KKnowledgeWFTemplateNodes] DROP CONSTRAINT [FK_KKnowledgeWFTemplateNodes_WFTemplateNodes]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[KKnowledgeWFTemplateNodes]') AND type in (N'U'))
DROP TABLE [dbo].[KKnowledgeWFTemplateNodes]
GO

/* [dbo].[WFNodeRecords] */
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WFNodeRecords_aspnet_Users]') AND parent_object_id = OBJECT_ID(N'[dbo].[WFNodeRecords]'))
ALTER TABLE [dbo].[WFNodeRecords] DROP CONSTRAINT [FK_WFNodeRecords_aspnet_Users]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WFNodeRecords_WFRequests]') AND parent_object_id = OBJECT_ID(N'[dbo].[WFNodeRecords]'))
ALTER TABLE [dbo].[WFNodeRecords] DROP CONSTRAINT [FK_WFNodeRecords_WFRequests]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WFNodeRecords_WFTemplateNodes]') AND parent_object_id = OBJECT_ID(N'[dbo].[WFNodeRecords]'))
ALTER TABLE [dbo].[WFNodeRecords] DROP CONSTRAINT [FK_WFNodeRecords_WFTemplateNodes]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WFNodeRecords]') AND type in (N'U'))
DROP TABLE [dbo].[WFNodeRecords]
GO

/* [dbo].[KRequestKnowledgeDomainExperts] */
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_KRequestKnowledgeDomainExperts_aspnet_Users]') AND parent_object_id = OBJECT_ID(N'[dbo].[KRequestKnowledgeDomainExperts]'))
ALTER TABLE [dbo].[KRequestKnowledgeDomainExperts] DROP CONSTRAINT [FK_KRequestKnowledgeDomainExperts_aspnet_Users]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_KRequestKnowledgeDomainExperts_KKnowledgeWFRequests]') AND parent_object_id = OBJECT_ID(N'[dbo].[KRequestKnowledgeDomainExperts]'))
ALTER TABLE [dbo].[KRequestKnowledgeDomainExperts] DROP CONSTRAINT [FK_KRequestKnowledgeDomainExperts_KKnowledgeWFRequests]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_KRequestKnowledgeDomainExperts_Nodes]') AND parent_object_id = OBJECT_ID(N'[dbo].[KRequestKnowledgeDomainExperts]'))
ALTER TABLE [dbo].[KRequestKnowledgeDomainExperts] DROP CONSTRAINT [FK_KRequestKnowledgeDomainExperts_Nodes]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[KRequestKnowledgeDomainExperts]') AND type in (N'U'))
DROP TABLE [dbo].[KRequestKnowledgeDomainExperts]
GO

/* [dbo].[KKnowledgeWFRequests] */
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_KKnowledgeWFRequests_KKnowledges]') AND parent_object_id = OBJECT_ID(N'[dbo].[KKnowledgeWFRequests]'))
ALTER TABLE [dbo].[KKnowledgeWFRequests] DROP CONSTRAINT [FK_KKnowledgeWFRequests_KKnowledges]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_KKnowledgeWFRequests_WFRequests]') AND parent_object_id = OBJECT_ID(N'[dbo].[KKnowledgeWFRequests]'))
ALTER TABLE [dbo].[KKnowledgeWFRequests] DROP CONSTRAINT [FK_KKnowledgeWFRequests_WFRequests]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[KKnowledgeWFRequests]') AND type in (N'U'))
DROP TABLE [dbo].[KKnowledgeWFRequests]
GO

/* [dbo].[WFRequests] */
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WFRequests]') AND type in (N'U'))
DROP TABLE [dbo].[WFRequests]
GO

/* [dbo].[WFTemplateNodes] */
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WFTemplateNodes_WFTemplates]') AND parent_object_id = OBJECT_ID(N'[dbo].[WFTemplateNodes]'))
ALTER TABLE [dbo].[WFTemplateNodes] DROP CONSTRAINT [FK_WFTemplateNodes_WFTemplates]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WFTemplateNodes]') AND type in (N'U'))
DROP TABLE [dbo].[WFTemplateNodes]
GO

/* [dbo].[WFTemplates] */
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WFTemplates]') AND type in (N'U'))
DROP TABLE [dbo].[WFTemplates]
GO

