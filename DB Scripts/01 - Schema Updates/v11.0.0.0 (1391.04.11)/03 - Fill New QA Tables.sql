USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO


UPDATE [dbo].[Questions]
	SET Status = N'ReceivingAnswers'
WHERE [dbo].[Questions].[Status] IS NULL

UPDATE [dbo].[Questions]
	SET VisitsCount = 0
WHERE [dbo].[Questions].[VisitsCount] IS NULL

UPDATE [dbo].[Questions]
	SET Deleted = 0
WHERE [dbo].[Questions].[Deleted] IS NULL

INSERT INTO [dbo].[QA_Questions](
	QuestionID,
	Title,
	Description,
	SenderUserID,
	SendDate,
	Status,
	AcceptionDate,
	VisitsCount,
	Deleted
)
SELECT [dbo].[Questions].[QuestionID], [dbo].[Questions].[Title],
	   [dbo].[Questions].[Description], [dbo].[Questions].[UserID],
	   [dbo].[Questions].[CreationDate], [dbo].[Questions].[Status],
	   [dbo].[Questions].[AcceptionDate], [dbo].[Questions].[VisitsCount], 0
FROM [dbo].[Questions]



UPDATE [dbo].[QAnswers]
	SET Rate = 0
WHERE [dbo].[QAnswers].[Rate] IS NULL

UPDATE [dbo].[QAnswers]
	SET Deleted = 0
WHERE [dbo].[QAnswers].[Deleted] IS NULL

INSERT INTO [dbo].[QA_Answers](
	AnswerID,
	QuestionID,
	SenderUserID,
	SendDate,
	AnswerBody,
	Status,
	Rate,
	NodeID,
	Deleted
)
SELECT [dbo].[QAnswers].[AnswerID], [dbo].[QAnswers].[QuestionID], 
	   [dbo].[QAnswers].[UserID], [dbo].[QAnswers].[CreationDate], 
	   [dbo].[QAnswers].[Answer], [dbo].[QAnswers].[Status],
	   [dbo].[QAnswers].[Rate], [dbo].[QAnswers].[NodeID], 0
FROM [dbo].[QAnswers]



INSERT INTO [dbo].[QA_QuestionLikes](
	QuestionID,
	UserID,
	[Like],
	Score,
	Date
)
SELECT [dbo].[QuestionLikes].[QuestionID], [dbo].[QuestionLikes].[UserID], 1, 0, GETDATE()
FROM [dbo].[QuestionLikes]



UPDATE [dbo].[QuestionNodes]
	SET Date = GETDATE()
WHERE [dbo].[QuestionNodes].[Date] IS NULL

INSERT INTO [dbo].[QA_RefNodes](
	NodeID,
	QuestionID,
	SendDate
)
SELECT [dbo].[QuestionNodes].[NodeID], [dbo].[QuestionNodes].[QuestionID],
	   [dbo].[QuestionNodes].[Date]
FROM [dbo].[QuestionNodes]



UPDATE [dbo].[QuestionUsers] 
	SET CreationDate = GETDATE()
WHERE [dbo].[QuestionUsers].[CreationDate] IS NULL

UPDATE [dbo].[QuestionUsers] 
	SET Seen = 0
WHERE [dbo].[QuestionUsers].[Seen] IS NULL

INSERT INTO [dbo].[QA_RefUsers](
	UserID,
	QuestionID,
	SendDate,
	Seen
)
SELECT [dbo].[QuestionUsers].[UserID], [dbo].[QuestionUsers].[QuestionID],
	   [dbo].[QuestionUsers].[CreationDate], [dbo].[QuestionUsers].[Seen]
FROM [dbo].[QuestionUsers]