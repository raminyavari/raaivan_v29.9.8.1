USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER TABLE [dbo].[KW_History]
ADD WFVersionID int NULL
GO

ALTER TABLE [dbo].[KW_History]
ADD TextOptions nvarchar(1000) NULL
GO

ALTER TABLE [dbo].[KW_History]
ADD DeputyUserID uniqueidentifier NULL
GO

ALTER TABLE [dbo].[KW_History]
ADD UniqueID uniqueidentifier NULL
GO


UPDATE [dbo].[KW_History]
	SET WFVersionID = 1
GO

UPDATE H
	SET WFVersionID = 2
FROM [dbo].[KW_History] AS H
	INNER JOIN (
		SELECT A.KnowledgeID, DATEADD(SECOND, 10, MAX(A.EvaluationDate)) AS DT
		FROM [dbo].[KW_QuestionAnswersHistory] AS A
		GROUP BY A.KnowledgeID
	) AS X
	ON X.KnowledgeID = H.KnowledgeID AND H.ActionDate > X.DT AND H.[Action] <> N'TerminateEvaluation'
GO



