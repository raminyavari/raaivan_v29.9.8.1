USE [EKM_App]
GO


ALTER TABLE [dbo].[KW_KnowledgeTypes]
ADD PreEvaluateByOwner bit NULL
GO

ALTER TABLE [dbo].[KW_KnowledgeTypes]
ADD ForceEvaluatorsDescribe bit NULL
GO