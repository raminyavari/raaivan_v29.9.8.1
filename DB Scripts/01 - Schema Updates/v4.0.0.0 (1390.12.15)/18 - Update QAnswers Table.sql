USE [EKM_App]
GO

/****** Object:  Table [dbo].[QAnswers]    Script Date: 02/17/2012 15:17:07 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[QAnswersTemp](
	[AnswerID] [uniqueidentifier] NOT NULL,
	[QuestionID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[Answer] [nvarchar](max) NOT NULL,
	[Status] [nvarchar](max) NULL,
	[CreationDate] [datetime] NOT NULL,
	[Rate] [int] NULL,
	[NodeID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QAnswersTemp] PRIMARY KEY CLUSTERED 
(
	[AnswerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[QAnswersTemp]
           ([AnswerID]
			,[QuestionID]
			,[UserID]
			,[Answer]
			,[Status]
			,[CreationDate]
			,[Rate])
		SELECT  dbo.QAnswers.ID, dbo.QAnswers.QuestionId, dbo.QAnswers.UserId, dbo.QAnswers.Answer,
				dbo.QAnswers.Status, dbo.QAnswers.Date, dbo.QAnswers.Rate
		FROM    dbo.QAnswers
GO


/* Drop old table */
DROP TABLE [dbo].[QAnswers]
GO

/* Create old new table */
CREATE TABLE [dbo].[QAnswers](
	[AnswerID] [uniqueidentifier] NOT NULL,
	[QuestionID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[Answer] [nvarchar](max) NOT NULL,
	[Status] [nvarchar](max) NULL,
	[CreationDate] [datetime] NOT NULL,
	[Rate] [int] NULL,
	[NodeID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QAnswers] PRIMARY KEY CLUSTERED 
(
	[AnswerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[QAnswers]  WITH CHECK ADD  CONSTRAINT [FK_QAnswers_QA] FOREIGN KEY([QuestionID])
REFERENCES [dbo].[QQuestions] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[QAnswers] CHECK CONSTRAINT [FK_QAnswers_QA]
GO

ALTER TABLE [dbo].[QAnswers]  WITH CHECK ADD  CONSTRAINT [FK_QAnswers_ProfileCommon] FOREIGN KEY([UserID])
REFERENCES [dbo].[ProfileCommon] ([UserId])
GO

ALTER TABLE [dbo].[QAnswers] CHECK CONSTRAINT [FK_QAnswers_ProfileCommon]
GO

ALTER TABLE [dbo].[QAnswers]  WITH CHECK ADD  CONSTRAINT [FK_QAnswers_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
ON UPDATE CASCADE
ON DELETE SET NULL
GO

ALTER TABLE [dbo].[QAnswers] CHECK CONSTRAINT [FK_QAnswers_CN_Nodes]
GO


INSERT INTO [dbo].[QAnswers]
	SELECT * FROM dbo.QAnswersTemp
GO

DROP TABLE [dbo].[QAnswersTemp]
GO