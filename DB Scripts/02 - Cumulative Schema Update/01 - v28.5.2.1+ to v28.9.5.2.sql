USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.5.2.1' BEGIN
	UPDATE [dbo].[AppSetting]
		SET [Version] = 'v28.7.2.1'
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.7.2.1' BEGIN
	CREATE TABLE [dbo].[FG_Polls](
		[PollID] [uniqueidentifier] NOT NULL,
		[IsCopyOfPollID] [uniqueidentifier] NULL,
		[OwnerID] [uniqueidentifier] NULL,
		[Name] [nvarchar](255) NULL,
		[Description] [nvarchar](2000) NULL,
		[BeginDate] [datetime] NULL,
		[FinishDate] [datetime] NULL,
		[ShowSummary] [bit] NOT NULL,
		[HideContributors] [bit] NOT NULL,
		[CreatorUserID] [uniqueidentifier] NOT NULL,
		[CreationDate] [datetime] NOT NULL,
		[LastModifierUserID] [uniqueidentifier] NULL,
		[LastModificationDate] [datetime] NULL,
		[Deleted] [bit] NOT NULL,
		[ApplicationID] [uniqueidentifier] NOT NULL,
	 CONSTRAINT [PK_FG_Polls] PRIMARY KEY CLUSTERED 
	(
		[PollID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.7.2.1' BEGIN
	ALTER TABLE [dbo].[FG_Polls]  WITH CHECK ADD  CONSTRAINT [FK_FG_Polls_aspnet_Applications] FOREIGN KEY([ApplicationID])
	REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.7.2.1' BEGIN
	ALTER TABLE [dbo].[FG_Polls] CHECK CONSTRAINT [FK_FG_Polls_aspnet_Applications]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.7.2.1' BEGIN
	ALTER TABLE [dbo].[FG_Polls]  WITH CHECK ADD  CONSTRAINT [FK_FG_Polls_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
	REFERENCES [dbo].[aspnet_Users] ([UserId])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.7.2.1' BEGIN
	ALTER TABLE [dbo].[FG_Polls] CHECK CONSTRAINT [FK_FG_Polls_aspnet_Users_Creator]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.7.2.1' BEGIN
	ALTER TABLE [dbo].[FG_Polls]  WITH CHECK ADD  CONSTRAINT [FK_FG_Polls_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
	REFERENCES [dbo].[aspnet_Users] ([UserId])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.7.2.1' BEGIN
	ALTER TABLE [dbo].[FG_Polls] CHECK CONSTRAINT [FK_FG_Polls_aspnet_Users_Modifier]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.7.2.1' BEGIN
	ALTER TABLE [dbo].[FG_Polls]  WITH CHECK ADD  CONSTRAINT [FK_FG_Polls_FG_Polls] FOREIGN KEY([IsCopyOfPollID])
	REFERENCES [dbo].[FG_Polls] ([PollID])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.7.2.1' BEGIN
	ALTER TABLE [dbo].[FG_Polls] CHECK CONSTRAINT [FK_FG_Polls_FG_Polls]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.7.2.1' BEGIN
	ALTER TABLE [dbo].[WF_WorkFlowStates]
	ADD PollID uniqueidentifier NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.7.2.1' BEGIN
	ALTER TABLE [dbo].[WF_WorkFlowStates]  WITH CHECK ADD  CONSTRAINT [FK_WF_WorkFlowStates_FG_Polls] FOREIGN KEY([PollID])
	REFERENCES [dbo].[FG_Polls] ([PollID])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.7.2.1' BEGIN
	ALTER TABLE [dbo].[WF_WorkFlowStates] CHECK CONSTRAINT [FK_WF_WorkFlowStates_FG_Polls]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.7.2.1' BEGIN
	ALTER TABLE [dbo].[FG_ExtendedFormElements]
	ADD [Weight] float NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.7.2.1' BEGIN
	UPDATE [dbo].[AppSetting]
		SET [Version] = 'v28.9.5.2'
END
GO