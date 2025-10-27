USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER TABLE [dbo].[KW_QuestionAnswers]
ADD AdminScore float NULL
GO

ALTER TABLE [dbo].[KW_QuestionAnswers]
ADD AdminSelectedOptionID uniqueidentifier NULL
GO

ALTER TABLE [dbo].[KW_QuestionAnswers]
ADD AdminID uniqueidentifier NULL
GO

UPDATE [dbo].[KW_QuestionAnswers]
	SET AdminID = ResponderUserID
GO

ALTER TABLE [dbo].[KW_QuestionAnswers]
DROP COLUMN [ResponderUserID]
GO



ALTER TABLE [dbo].[KW_QuestionAnswersHistory]
ADD AdminScore float NULL
GO

ALTER TABLE [dbo].[KW_QuestionAnswersHistory]
ADD AdminSelectedOptionID uniqueidentifier NULL
GO

ALTER TABLE [dbo].[KW_QuestionAnswersHistory]
ADD AdminID uniqueidentifier NULL
GO

ALTER TABLE [dbo].[KW_QuestionAnswersHistory]
ADD WFVersionID int NULL
GO

UPDATE [dbo].[KW_QuestionAnswersHistory]
	SET AdminID = ResponderUserID
GO

ALTER TABLE [dbo].[KW_QuestionAnswersHistory]
DROP COLUMN [ResponderUserID]
GO

UPDATE [dbo].[KW_QuestionAnswersHistory]
	SET WFVersionID = 1
GO
