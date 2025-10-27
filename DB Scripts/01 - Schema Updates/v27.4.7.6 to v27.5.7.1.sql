USE [EKM_App]
GO

ALTER TABLE [dbo].[TMP_KW_TypeQuestions]
ADD [Weight] FLOAT NULL

GO


ALTER TABLE [dbo].[TMP_KW_QuestionAnswers]
ADD SelectedOptionID uniqueidentifier NULL

GO


/****** Object:  Table [dbo].[CN_Extensions]    Script Date: 06/01/2016 14:34:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[TMP_KW_AnswerOptions](
	[ID] [uniqueidentifier] NOT NULL,
	[TypeQuestionID] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](2000) NOT NULL,
	[Value] [float] NOT NULL,
	[SequenceNumber] [int] NULL,
	[CreatorUserID] [uniqueidentifier] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
	[ApplicationID] [uniqueidentifier] NULL
 CONSTRAINT [PK_TMP_KW_AnswerOptions] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[TMP_KW_AnswerOptions]  WITH CHECK ADD  CONSTRAINT [FK_TMP_KW_AnswerOptions_TMP_KW_TypeQuestions] FOREIGN KEY([TypeQuestionID])
REFERENCES [dbo].[TMP_KW_TypeQuestions] ([ID])
GO

ALTER TABLE [dbo].[TMP_KW_AnswerOptions] CHECK CONSTRAINT [FK_TMP_KW_AnswerOptions_TMP_KW_TypeQuestions]
GO

ALTER TABLE [dbo].[TMP_KW_AnswerOptions]  WITH CHECK ADD  CONSTRAINT [FK_TMP_KW_AnswerOptions_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[TMP_KW_AnswerOptions] CHECK CONSTRAINT [FK_TMP_KW_AnswerOptions_aspnet_Applications]
GO

ALTER TABLE [dbo].[TMP_KW_AnswerOptions]  WITH CHECK ADD  CONSTRAINT [FK_TMP_KW_AnswerOptions_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[TMP_KW_AnswerOptions] CHECK CONSTRAINT [FK_TMP_KW_AnswerOptions_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[TMP_KW_AnswerOptions]  WITH CHECK ADD  CONSTRAINT [FK_TMP_KW_AnswerOptions_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[TMP_KW_AnswerOptions] CHECK CONSTRAINT [FK_TMP_KW_AnswerOptions_aspnet_Users_Modifier]
GO




SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



UPDATE [dbo].[AppSetting]
	SET [Version] = 'v27.5.7.1' -- 13950519
GO