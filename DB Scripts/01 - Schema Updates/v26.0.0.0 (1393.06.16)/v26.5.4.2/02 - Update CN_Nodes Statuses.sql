USE [EKM_App]
GO

SET ANSI_PADDING ON
GO

UPDATE [dbo].[CN_Nodes]
	SET [Status] = N'SentToAdmin'
WHERE [Status] = N'ManagerEvaluation'

GO


UPDATE [dbo].[CN_Nodes]
	SET [Status] = N'SentToEvaluators'
WHERE [Status] = N'ExpertEvaluation'

GO