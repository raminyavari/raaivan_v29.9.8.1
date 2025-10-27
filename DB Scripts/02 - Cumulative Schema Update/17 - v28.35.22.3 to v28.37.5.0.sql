USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.35.22.3' BEGIN
	ALTER TABLE [dbo].[KW_History]
	ADD [ReplyToHistoryID] [bigint] NULL
END
GO

SET ANSI_PADDING OFF
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.35.22.3' BEGIN
	ALTER TABLE [dbo].[KW_History]  WITH CHECK ADD  CONSTRAINT [FK_KW_History_KW_History_ReplyToHistoryID] FOREIGN KEY([ReplyToHistoryID])
	REFERENCES [dbo].[KW_History] ([ID])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.35.22.3' BEGIN
	ALTER TABLE [dbo].[KW_History] CHECK CONSTRAINT [FK_KW_History_KW_History_ReplyToHistoryID]
END
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.35.22.3' BEGIN
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
END

GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.35.22.3' BEGIN
	ALTER TABLE [dbo].[KW_QuestionAnswersHistory]  WITH CHECK ADD  CONSTRAINT [FK_KW_QuestionAnswersHistory_aspnet_Applications] FOREIGN KEY([ApplicationID])
	REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.35.22.3' BEGIN
	ALTER TABLE [dbo].[KW_QuestionAnswersHistory] CHECK CONSTRAINT [FK_KW_QuestionAnswersHistory_aspnet_Applications]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.35.22.3' BEGIN
	ALTER TABLE [dbo].[KW_QuestionAnswersHistory]  WITH CHECK ADD  CONSTRAINT [FK_KW_QuestionAnswersHistory_aspnet_Users] FOREIGN KEY([UserID])
	REFERENCES [dbo].[aspnet_Users] ([UserId])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.35.22.3' BEGIN
	ALTER TABLE [dbo].[KW_QuestionAnswersHistory] CHECK CONSTRAINT [FK_KW_QuestionAnswersHistory_aspnet_Users]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.35.22.3' BEGIN
	ALTER TABLE [dbo].[KW_QuestionAnswersHistory]  WITH CHECK ADD  CONSTRAINT [FK_KW_QuestionAnswersHistory_CN_Nodes] FOREIGN KEY([KnowledgeID])
	REFERENCES [dbo].[CN_Nodes] ([NodeID])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.35.22.3' BEGIN
	ALTER TABLE [dbo].[KW_QuestionAnswersHistory] CHECK CONSTRAINT [FK_KW_QuestionAnswersHistory_CN_Nodes]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.35.22.3' BEGIN
	ALTER TABLE [dbo].[KW_QuestionAnswersHistory]  WITH CHECK ADD  CONSTRAINT [FK_KW_QuestionAnswersHistory_KW_Questions] FOREIGN KEY([QuestionID])
	REFERENCES [dbo].[KW_Questions] ([QuestionID])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.35.22.3' BEGIN
	ALTER TABLE [dbo].[KW_QuestionAnswersHistory] CHECK CONSTRAINT [FK_KW_QuestionAnswersHistory_KW_Questions]
END
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.35.22.3' BEGIN
	UPDATE [dbo].[AppSetting]
		SET [Version] = 'v28.37.5.0' -- 13981204
END
GO