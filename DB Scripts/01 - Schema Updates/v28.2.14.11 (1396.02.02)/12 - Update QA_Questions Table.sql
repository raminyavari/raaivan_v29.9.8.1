USE [EKM_App]
GO

/****** Object:  Table [dbo].[QA_Questions]    Script Date: 12/03/2016 12:38:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER TABLE [dbo].[QA_Answers] DROP CONSTRAINT [FK_QA_Answers_QA_Questions]
GO

ALTER TABLE [dbo].[QA_FAQItems] DROP CONSTRAINT [FK_QA_FAQItems_QA_Questions]
GO

ALTER TABLE [dbo].[QA_RefNodes] DROP CONSTRAINT [FK_QA_RefNodes_QA_Questions]
GO

ALTER TABLE [dbo].[QA_RefUsers] DROP CONSTRAINT [FK_QA_RefUsers_QA_Questions]
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[QA_TMPQuestions](
	[QuestionID] [uniqueidentifier] NOT NULL,
	[WorkFlowID] [uniqueidentifier] NULL,
	[Title] [nvarchar](500) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Status] [varchar](20) NOT NULL,
	[PublicationDate] [datetime] NULL,
	[BestAnswerID] [uniqueidentifier] NULL,
	[SenderUserID] [uniqueidentifier] NOT NULL,
	[SendDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
	[IndexLastUpdateDate] [datetime] NULL,
	[ApplicationID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QA_TMPQuestions] PRIMARY KEY CLUSTERED 
(
	[QuestionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

INSERT INTO [dbo].[QA_TMPQuestions] (
	QuestionID, WorkFlowID, Title, [Description], [Status], PublicationDate,
	BestAnswerID, SenderUserID, SendDate, LastModifierUserID, LastModificationDate,
	Deleted, IndexLastUpdateDate, ApplicationID
)
SELECT QuestionID, WorkFlowID, Title, [Description], [Status], AcceptionDate,
	BestAnswerID, SenderUserID, SendDate, LastModifierUserID, LastModificationDate,
	Deleted, IndexLastUpdateDate, ApplicationID
FROM [dbo].[QA_Questions]

GO

DROP TABLE [dbo].[QA_Questions]
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[QA_Questions](
	[QuestionID] [uniqueidentifier] NOT NULL,
	[WorkFlowID] [uniqueidentifier] NULL,
	[Title] [nvarchar](500) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Status] [varchar](20) NOT NULL,
	[PublicationDate] [datetime] NULL,
	[BestAnswerID] [uniqueidentifier] NULL,
	[SenderUserID] [uniqueidentifier] NOT NULL,
	[SendDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
	[IndexLastUpdateDate] [datetime] NULL,
	[ApplicationID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QA_Questions] PRIMARY KEY CLUSTERED 
(
	[QuestionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

INSERT INTO [dbo].[QA_Questions]
SELECT *
FROM [dbo].[QA_TMPQuestions]

GO

DROP TABLE [dbo].[QA_TMPQuestions]
GO

ALTER TABLE [dbo].[QA_Questions]  WITH CHECK ADD  CONSTRAINT [FK_QA_Questions_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[QA_Questions] CHECK CONSTRAINT [FK_QA_Questions_aspnet_Applications]
GO

ALTER TABLE [dbo].[QA_Questions]  WITH CHECK ADD  CONSTRAINT [FK_QA_Questions_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[QA_Questions] CHECK CONSTRAINT [FK_QA_Questions_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[QA_Questions]  WITH CHECK ADD  CONSTRAINT [FK_QA_Questions_aspnet_Users_Sender] FOREIGN KEY([SenderUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[QA_Questions] CHECK CONSTRAINT [FK_QA_Questions_aspnet_Users_Sender]
GO

ALTER TABLE [dbo].[QA_Questions]  WITH CHECK ADD  CONSTRAINT [FK_QA_Questions_QA_Answers] FOREIGN KEY([BestAnswerID])
REFERENCES [dbo].[QA_Answers] ([AnswerID])
GO

ALTER TABLE [dbo].[QA_Questions] CHECK CONSTRAINT [FK_QA_Questions_QA_Answers]
GO


ALTER TABLE [dbo].[QA_Answers]  WITH CHECK ADD  CONSTRAINT [FK_QA_Answers_QA_Questions] FOREIGN KEY([QuestionID])
REFERENCES [dbo].[QA_Questions] ([QuestionID])
GO

ALTER TABLE [dbo].[QA_Answers] CHECK CONSTRAINT [FK_QA_Answers_QA_Questions]
GO

ALTER TABLE [dbo].[QA_FAQItems]  WITH CHECK ADD  CONSTRAINT [FK_QA_FAQItems_QA_Questions] FOREIGN KEY([QuestionID])
REFERENCES [dbo].[QA_Questions] ([QuestionID])
GO

ALTER TABLE [dbo].[QA_FAQItems] CHECK CONSTRAINT [FK_QA_FAQItems_QA_Questions]
GO

ALTER TABLE [dbo].[QA_RefNodes]  WITH CHECK ADD  CONSTRAINT [FK_QA_RefNodes_QA_Questions] FOREIGN KEY([QuestionID])
REFERENCES [dbo].[QA_Questions] ([QuestionID])
GO

ALTER TABLE [dbo].[QA_RefNodes] CHECK CONSTRAINT [FK_QA_RefNodes_QA_Questions]
GO

ALTER TABLE [dbo].[QA_RefUsers]  WITH CHECK ADD  CONSTRAINT [FK_QA_RefUsers_QA_Questions] FOREIGN KEY([QuestionID])
REFERENCES [dbo].[QA_Questions] ([QuestionID])
GO

ALTER TABLE [dbo].[QA_RefUsers] CHECK CONSTRAINT [FK_QA_RefUsers_QA_Questions]
GO