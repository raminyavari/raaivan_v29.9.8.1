USE [EKM_App]
GO

/****** Object:  Table [dbo].[Phrases]    Script Date: 04/26/2013 20:38:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TABLE [dbo].[TMP_KW_QuestionAnswers](
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[QuestionID] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](2000) NOT NULL,
	[Score] [float] NOT NULL,
	[ResponderUserID] [uniqueidentifier] NULL,
	[EvaluationDate] [datetime] NOT NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_TMP_KW_QuestionAnswers] PRIMARY KEY CLUSTERED 
(
	[KnowledgeID] ASC,
	[UserID] ASC,
	[QuestionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[TMP_KW_QuestionAnswers]  WITH CHECK ADD  CONSTRAINT [FK_TMP_KW_QuestionAnswers_CN_Nodes] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[TMP_KW_QuestionAnswers] CHECK CONSTRAINT [FK_TMP_KW_QuestionAnswers_CN_Nodes]
GO

ALTER TABLE [dbo].[TMP_KW_QuestionAnswers]  WITH CHECK ADD  CONSTRAINT [FK_TMP_KW_QuestionAnswers_TMP_KW_Questions] FOREIGN KEY([QuestionID])
REFERENCES [dbo].[TMP_KW_Questions] ([QuestionID])
GO

ALTER TABLE [dbo].[TMP_KW_QuestionAnswers] CHECK CONSTRAINT [FK_TMP_KW_QuestionAnswers_TMP_KW_Questions]
GO

ALTER TABLE [dbo].[TMP_KW_QuestionAnswers]  WITH CHECK ADD  CONSTRAINT [FK_TMP_KW_QuestionAnswers_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[TMP_KW_QuestionAnswers] CHECK CONSTRAINT [FK_TMP_KW_QuestionAnswers_aspnet_Users]
GO