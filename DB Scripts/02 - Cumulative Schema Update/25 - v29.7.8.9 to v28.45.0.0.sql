USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.9' BEGIN
	CREATE TABLE [dbo].[WF_WorkFlowActions](
		[ConnectionID] [uniqueidentifier] NOT NULL,
		[Action] [varchar](255) NOT NULL,
		[SequenceNumber] [int] NULL,
		[CreatorUserID] [uniqueidentifier] NULL,
		[CreationDate] [datetime] NULL,
		[LastModifierUserID] [uniqueidentifier] NULL,
		[LastModificationDate] [datetime] NULL,
		[Deleted] [bit] NOT NULL,
		[ApplicationID] [uniqueidentifier] NULL
	 CONSTRAINT [PK_WF_WorkFlowActions] PRIMARY KEY CLUSTERED 
	(
		[ConnectionID] ASC,
		[Action] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
	) ON [PRIMARY]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.9' BEGIN
	ALTER TABLE [dbo].[WF_WorkFlowActions]  WITH CHECK ADD  CONSTRAINT [FK_WF_WorkFlowActions_aspnet_Applications] FOREIGN KEY([ApplicationID])
	REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.9' BEGIN
	ALTER TABLE [dbo].[WF_WorkFlowActions] CHECK CONSTRAINT [FK_WF_WorkFlowActions_aspnet_Applications]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.9' BEGIN
	ALTER TABLE [dbo].[WF_WorkFlowActions]  WITH CHECK ADD  CONSTRAINT [FK_WF_WorkFlowActions_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
	REFERENCES [dbo].[aspnet_Users] ([UserId])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.9' BEGIN
	ALTER TABLE [dbo].[WF_WorkFlowActions] CHECK CONSTRAINT [FK_WF_WorkFlowActions_aspnet_Users_Creator]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.9' BEGIN
	ALTER TABLE [dbo].[WF_WorkFlowActions]  WITH CHECK ADD  CONSTRAINT [FK_WF_WorkFlowActions_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
	REFERENCES [dbo].[aspnet_Users] ([UserId])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.9' BEGIN
	ALTER TABLE [dbo].[WF_WorkFlowActions] CHECK CONSTRAINT [FK_WF_WorkFlowActions_aspnet_Users_Modifier]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.9' BEGIN
	ALTER TABLE [dbo].[WF_WorkFlowActions]  WITH CHECK ADD  CONSTRAINT [FK_WF_WorkFlowActions_WF_StateConnections] FOREIGN KEY([ConnectionID])
	REFERENCES [dbo].[WF_StateConnections] ([ID])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.9' BEGIN
	ALTER TABLE [dbo].[WF_WorkFlowActions] CHECK CONSTRAINT [FK_WF_WorkFlowActions_WF_StateConnections]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.9' BEGIN
	UPDATE [dbo].[AppSetting]
		SET [Version] = 'v28.45.0.0' -- 14000725
END
GO



