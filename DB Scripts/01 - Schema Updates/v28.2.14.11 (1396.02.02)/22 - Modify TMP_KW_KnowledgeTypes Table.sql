USE [EKM_App]
GO

ALTER TABLE [dbo].[TMP_KW_KnowledgeTypes]
ADD [ConvertEvaluatorsToExperts] bit NULL
GO


UPDATE [dbo].[TMP_KW_KnowledgeTypes]
SET ConvertEvaluatorsToExperts = 0
GO