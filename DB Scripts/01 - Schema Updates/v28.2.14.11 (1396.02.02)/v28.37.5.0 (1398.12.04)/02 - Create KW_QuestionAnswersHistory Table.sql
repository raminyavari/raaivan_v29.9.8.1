USE [EKM_App]
GO

/****** Object:  Table [dbo].[KW_QuestionAnswersHistory]    Script Date: 02/23/2020 12:53:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[KW_QuestionAnswersHistory](
	[VersionID] [uniqueidentifier] NOT NULL,
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[QuestionID] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](2000) NOT NULL,
	[Score] [float] NOT NULL,
	[ResponderUserID] [uniqueidentifier] NULL,
	[EvaluationDate] [datetime] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[ApplicationID] [uniqueidentifier] NULL,
	[SelectedOptionID] [uniqueidentifier] NULL,
	[VersionDate] [datetime] NOT NULL
 CONSTRAINT [PK_KW_QuestionAnswersHistory] PRIMARY KEY CLUSTERED 
(
	[VersionID] ASC,
	[KnowledgeID] ASC,
	[UserID] ASC,
	[QuestionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[KW_QuestionAnswersHistory]  WITH CHECK ADD  CONSTRAINT [FK_KW_QuestionAnswersHistory_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[KW_QuestionAnswersHistory] CHECK CONSTRAINT [FK_KW_QuestionAnswersHistory_aspnet_Applications]
GO

ALTER TABLE [dbo].[KW_QuestionAnswersHistory]  WITH CHECK ADD  CONSTRAINT [FK_KW_QuestionAnswersHistory_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KW_QuestionAnswersHistory] CHECK CONSTRAINT [FK_KW_QuestionAnswersHistory_aspnet_Users]
GO

ALTER TABLE [dbo].[KW_QuestionAnswersHistory]  WITH CHECK ADD  CONSTRAINT [FK_KW_QuestionAnswersHistory_CN_Nodes] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[KW_QuestionAnswersHistory] CHECK CONSTRAINT [FK_KW_QuestionAnswersHistory_CN_Nodes]
GO

ALTER TABLE [dbo].[KW_QuestionAnswersHistory]  WITH CHECK ADD  CONSTRAINT [FK_KW_QuestionAnswersHistory_KW_Questions] FOREIGN KEY([QuestionID])
REFERENCES [dbo].[KW_Questions] ([QuestionID])
GO

ALTER TABLE [dbo].[KW_QuestionAnswersHistory] CHECK CONSTRAINT [FK_KW_QuestionAnswersHistory_KW_Questions]
GO


