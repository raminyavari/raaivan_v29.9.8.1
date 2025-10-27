USE [EKM_App]
GO

ALTER TABLE [dbo].[TMP_KW_TypeQuestions]
ADD [Weight] FLOAT NULL

GO


ALTER TABLE [dbo].[TMP_KW_QuestionAnswers]
ADD SelectedOptionID uniqueidentifier NULL

GO