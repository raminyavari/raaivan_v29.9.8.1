USE [EKM_App]
GO


UPDATE [dbo].[QA_Questions]
SET [Status] = CASE WHEN [Status] = N'Final' THEN N'Accepted' ELSE N'GettingAnswers' END

GO