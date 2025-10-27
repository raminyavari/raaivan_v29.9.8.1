USE [EKM_App]
GO

/****** Object:  Table [dbo].[QQuestions]    Script Date: 03/02/2012 16:40:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Questions](
	[QuestionID] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](1000) NULL,
	[Description] [nvarchar](max) NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[Status] [nvarchar](255) NULL,
	[AcceptionDate] [datetime] NULL,
	[VisitsCount] [bigint] NULL,
 CONSTRAINT [PK_Questions] PRIMARY KEY CLUSTERED 
(
	[QuestionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Questions]  WITH CHECK ADD  CONSTRAINT [FK_Questions_ProfileCommon] FOREIGN KEY([UserID])
REFERENCES [dbo].[ProfileCommon] ([UserId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Questions] CHECK CONSTRAINT [FK_Questions_ProfileCommon]
GO


INSERT INTO [dbo].[Questions]
SELECT * FROM dbo.QQuestions
GO

/* Drop old table */
ALTER TABLE [dbo].[QAnswers] 
DROP CONSTRAINT [FK_QAnswers_QA]
GO

ALTER TABLE [dbo].[QQuestion_Keywords] 
DROP CONSTRAINT [FK_QQuestion_Keywords_QQuestions]
GO

ALTER TABLE [dbo].[QQuestion_Users] 
DROP CONSTRAINT [FK_QA_Users_QA]
GO

ALTER TABLE [dbo].[QuestionNodes] 
DROP CONSTRAINT [FK_QuestionNodes_QQuestions]
GO

ALTER TABLE [dbo].[QuestionLikes] 
DROP CONSTRAINT [FK_QuestionLikes_QQuestions]
GO

ALTER TABLE [dbo].[WallPostQQuestion] 
DROP CONSTRAINT [FK_WallPostQQuestion_QQuestions]
GO


DROP TABLE [dbo].[QQuestions]
GO


ALTER TABLE [dbo].[QAnswers]  WITH CHECK ADD  CONSTRAINT [FK_QAnswers_Questions] FOREIGN KEY([QuestionID])
REFERENCES [dbo].[Questions] ([QuestionID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[QAnswers] CHECK CONSTRAINT [FK_QAnswers_Questions]
GO


ALTER TABLE [dbo].[QQuestion_Keywords]  WITH CHECK ADD  CONSTRAINT [FK_QQuestion_Keywords_Questions] FOREIGN KEY([QuestionId])
REFERENCES [dbo].[Questions] ([QuestionID])
GO

ALTER TABLE [dbo].[QQuestion_Keywords] CHECK CONSTRAINT [FK_QQuestion_Keywords_Questions]
GO


ALTER TABLE [dbo].[QQuestion_Users]  WITH CHECK ADD  CONSTRAINT [FK_QA_Users_Questions] FOREIGN KEY([QuestionId])
REFERENCES [dbo].[Questions] ([QuestionID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[QQuestion_Users] CHECK CONSTRAINT [FK_QA_Users_Questions]
GO


ALTER TABLE [dbo].[QuestionNodes]  WITH CHECK ADD  CONSTRAINT [FK_QuestionNodes_Questions] FOREIGN KEY([QuestionID])
REFERENCES [dbo].[Questions] ([QuestionID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[QuestionNodes] CHECK CONSTRAINT [FK_QuestionNodes_Questions]
GO


ALTER TABLE [dbo].[QuestionLikes]  WITH CHECK ADD  CONSTRAINT [FK_QuestionLikes_Questions] FOREIGN KEY([QuestionID])
REFERENCES [dbo].[Questions] ([QuestionID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[QuestionLikes] CHECK CONSTRAINT [FK_QuestionLikes_Questions]
GO

ALTER TABLE [dbo].[WallPostQQuestion]  WITH CHECK ADD  CONSTRAINT [FK_WallPostQQuestion_Questions] FOREIGN KEY([QQuestionId])
REFERENCES [dbo].[Questions] ([QuestionID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[WallPostQQuestion] CHECK CONSTRAINT [FK_WallPostQQuestion_Questions]
GO