USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.38.4.2' BEGIN
	DELETE [dbo].[PRVC_DefaultPermissions]
	WHERE DefaultValue NOT IN (N'Public', N'Restricted')
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.38.4.2' BEGIN
	ALTER TABLE [dbo].[KW_KnowledgeTypes]
	ADD TextOptions nvarchar(max) NULL
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.38.4.2' BEGIN
	ALTER TABLE [dbo].[KW_KnowledgeTypes]
	ADD EvaluationsEditableForAdmin bit NULL
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.38.4.2' BEGIN
	ALTER TABLE [dbo].[KW_QuestionAnswers]
	ADD AdminScore float NULL
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.38.4.2' BEGIN
	ALTER TABLE [dbo].[KW_QuestionAnswers]
	ADD AdminSelectedOptionID uniqueidentifier NULL
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.38.4.2' BEGIN
	ALTER TABLE [dbo].[KW_QuestionAnswers]
	ADD AdminID uniqueidentifier NULL
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.38.4.2' BEGIN
	EXEC ('UPDATE [dbo].[KW_QuestionAnswers] SET AdminID = [ResponderUserID]')
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.38.4.2' BEGIN
	ALTER TABLE [dbo].[KW_QuestionAnswers]
	DROP COLUMN [ResponderUserID]
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.38.4.2' BEGIN
	ALTER TABLE [dbo].[KW_QuestionAnswersHistory]
	ADD AdminScore float NULL
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.38.4.2' BEGIN
	ALTER TABLE [dbo].[KW_QuestionAnswersHistory]
	ADD AdminSelectedOptionID uniqueidentifier NULL
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.38.4.2' BEGIN
	ALTER TABLE [dbo].[KW_QuestionAnswersHistory]
	ADD AdminID uniqueidentifier NULL
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.38.4.2' BEGIN
	ALTER TABLE [dbo].[KW_QuestionAnswersHistory]
	ADD WFVersionID int NULL
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.38.4.2' BEGIN
	EXEC ('UPDATE [dbo].[KW_QuestionAnswersHistory] SET AdminID = ResponderUserID')
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.38.4.2' BEGIN
	ALTER TABLE [dbo].[KW_QuestionAnswersHistory]
	DROP COLUMN [ResponderUserID]
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.38.4.2' BEGIN
	UPDATE [dbo].[KW_QuestionAnswersHistory]
		SET WFVersionID = 1
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.38.4.2' BEGIN
	ALTER TABLE [dbo].[KW_History]
	ADD WFVersionID int NULL
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.38.4.2' BEGIN
	ALTER TABLE [dbo].[KW_History]
	ADD TextOptions nvarchar(1000) NULL
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.38.4.2' BEGIN
	ALTER TABLE [dbo].[KW_History]
	ADD DeputyUserID uniqueidentifier NULL
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.38.4.2' BEGIN
	ALTER TABLE [dbo].[KW_History]
	ADD UniqueID uniqueidentifier NULL
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.38.4.2' BEGIN
	UPDATE [dbo].[KW_History]
		SET WFVersionID = 1
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.38.4.2' BEGIN
	UPDATE H
		SET WFVersionID = 2
	FROM [dbo].[KW_History] AS H
		INNER JOIN (
			SELECT A.KnowledgeID, DATEADD(SECOND, 10, MAX(A.EvaluationDate)) AS DT
			FROM [dbo].[KW_QuestionAnswersHistory] AS A
			GROUP BY A.KnowledgeID
		) AS X
		ON X.KnowledgeID = H.KnowledgeID AND H.ActionDate > X.DT AND H.[Action] <> N'TerminateEvaluation'
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.38.4.2' BEGIN
	ALTER TABLE [dbo].[CN_Services]
	ADD EnablePreviousVersionSelect bit NULL
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.38.4.2' BEGIN
	UPDATE [dbo].[CN_Services]
		SET EnablePreviousVersionSelect = IsDocument
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.38.4.2' BEGIN
	UPDATE [dbo].[AppSetting]
		SET [Version] = 'v28.43.25.4'
END
GO