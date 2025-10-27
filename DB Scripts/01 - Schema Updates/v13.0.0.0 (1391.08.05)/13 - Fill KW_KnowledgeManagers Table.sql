USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


INSERT INTO [dbo].[KW_KnowledgeManagers](
	ListID,
	UserID,
	Deleted
)
SELECT KM.DepartmentGroupID, KM.UserID, 0
FROM [dbo].[KnowledgeManagers] AS KM
GO


DROP TABLE [dbo].[KnowledgeManagers]
GO