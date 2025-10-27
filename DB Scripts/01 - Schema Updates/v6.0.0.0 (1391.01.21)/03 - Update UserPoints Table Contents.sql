USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


DELETE FROM [dbo].[UserPoints]
      WHERE PointType = 'ContentAccept' OR PointType = 'KnowledgeAccept'
GO


INSERT INTO [dbo].[UserPoints]
           ([ID]
           ,[PointType]
           ,[Value]
           ,[Date]
           ,[UserId]
           ,[EntityType]
           ,[EntityId]
           ,[EntityIdType])
SELECT NEWID(), 'KnowledgeRegistration', 1, '2011-11-05 17:58:00.927',
	   dbo.KKnowledges.OwnerUserID, '', dbo.KKnowledges.ID, 'System.Guid' 
FROM [dbo].[KKnowledges]
GO


INSERT INTO [dbo].[UserPoints]
           ([ID]
           ,[PointType]
           ,[Value]
           ,[Date]
           ,[UserId]
           ,[EntityType]
           ,[EntityId]
           ,[EntityIdType])
SELECT NEWID(), 'KnowledgeAccept', 1, '2011-11-05 17:58:00.927',
	   dbo.KKnowledges.OwnerUserID, '', dbo.KKnowledges.ID, 'System.Guid' 
FROM [dbo].[KKnowledges]
WHERE [dbo].[KKnowledges].StatusID = 6
GO


