USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	SET ANSI_PADDING ON
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	CREATE TABLE [dbo].[RV_SystemSettings](
		[ID] [bigint] IDENTITY(1,1) NOT NULL,
		[Name] [varchar](100) NOT NULL,
		[Value] [nvarchar](max) NOT NULL,
		[LastModifierUserID] [uniqueidentifier] NULL,
		[LastModificationDate] [datetime] NULL,
		[ApplicationID] [uniqueidentifier] NOT NULL,
	 CONSTRAINT [PK_RV_SystemSettings] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	SET ANSI_PADDING OFF
END

GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	ALTER TABLE [dbo].[RV_SystemSettings]  WITH CHECK ADD  CONSTRAINT [FK_RV_SystemSettings_aspnet_Applications] FOREIGN KEY([ApplicationID])
	REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
END

GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	ALTER TABLE [dbo].[RV_SystemSettings] CHECK CONSTRAINT [FK_RV_SystemSettings_aspnet_Applications]
END

GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	ALTER TABLE [dbo].[RV_SystemSettings]  WITH CHECK ADD  CONSTRAINT [FK_RV_SystemSettings_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
	REFERENCES [dbo].[aspnet_Users] ([UserId])
END

GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	ALTER TABLE [dbo].[RV_SystemSettings] CHECK CONSTRAINT [FK_RV_SystemSettings_aspnet_Users_Modifier]
END

GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	SET ANSI_PADDING ON
END

GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	CREATE TABLE [dbo].[FG_SelectedItems](
		[ElementID] [uniqueidentifier] NOT NULL,
		[SelectedID] [uniqueidentifier] NOT NULL,
		[LastModifierUserID] [uniqueidentifier] NOT NULL,
		[LastModificationDate] [datetime] NOT NULL,
		[Deleted] [bit] NOT NULL,
		[ApplicationID] [uniqueidentifier] NOT NULL,
	 CONSTRAINT [PK_FG_SelectedItems] PRIMARY KEY CLUSTERED 
	(
		[ElementID] ASC,
		[SelectedID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	SET ANSI_PADDING OFF
END

GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	ALTER TABLE [dbo].[FG_SelectedItems]  WITH CHECK ADD  CONSTRAINT [FK_FG_SelectedItems_aspnet_Applications] FOREIGN KEY([ApplicationID])
	REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
END

GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	ALTER TABLE [dbo].[FG_SelectedItems] CHECK CONSTRAINT [FK_FG_SelectedItems_aspnet_Applications]
END

GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	ALTER TABLE [dbo].[FG_SelectedItems]  WITH CHECK ADD  CONSTRAINT [FK_FG_SelectedItems_FG_InstanceElements] FOREIGN KEY([ElementID])
	REFERENCES [dbo].[FG_InstanceElements] ([ElementID])
END

GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	ALTER TABLE [dbo].[FG_SelectedItems] CHECK CONSTRAINT [FK_FG_SelectedItems_FG_InstanceElements]
END

GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	ALTER TABLE [dbo].[FG_SelectedItems]  WITH CHECK ADD  CONSTRAINT [FK_FG_SelectedItems_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
	REFERENCES [dbo].[aspnet_Users] ([UserId])
END

GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	ALTER TABLE [dbo].[FG_SelectedItems] CHECK CONSTRAINT [FK_FG_SelectedItems_aspnet_Users_Modifier]
END

GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	DECLARE @str varchar(max) = N'INSERT INTO [dbo].[FG_SelectedItems] (ApplicationID, ElementID, SelectedID, ' +
		'LastModifierUserID, LastModificationDate, Deleted) ' +
	'SELECT E.ApplicationID, E.ElementID, E.GuidValue, ' +
		'ISNULL(E.LastModifierUserID, E.CreatorUserID), ISNULL(E.LastModificationDate, E.CreationDate), E.Deleted ' +
	'FROM [dbo].[FG_InstanceElements] AS E ' +
		'LEFT JOIN [dbo].[FG_SelectedItems] AS G ' +
		'ON G.ApplicationID = E.ApplicationID AND ' +
			'G.ElementID = E.ElementID AND G.SelectedID = E.GuidValue ' +
	'WHERE E.GuidValue IS NOT NULL AND E.CreatorUserID IS NOT NULL AND ' +
		'E.CreationDate IS NOT NULL AND G.ElementID IS NULL';
	
	EXEC (@str)
END

GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	ALTER TABLE [dbo].[FG_InstanceElements]
	DROP COLUMN GuidValue
END

GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	ALTER TABLE [dbo].[FG_Changes]
	DROP COLUMN GuidValue
END

GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	SET ANSI_PADDING ON
END

GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	CREATE TABLE [dbo].[FG_PollAdmins](
		[PollID] [uniqueidentifier] NOT NULL,
		[UserID] [uniqueidentifier] NOT NULL,
		[LastModifierUserID] [uniqueidentifier] NOT NULL,
		[LastModificationDate] [datetime] NOT NULL,
		[Deleted] [bit] NOT NULL,
		[ApplicationID] [uniqueidentifier] NOT NULL,
	 CONSTRAINT [PK_FG_PollAdmins] PRIMARY KEY CLUSTERED 
	(
		[PollID] ASC,
		[UserID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	SET ANSI_PADDING OFF
END

GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	ALTER TABLE [dbo].[FG_PollAdmins]  WITH CHECK ADD  CONSTRAINT [FK_FG_PollAdmins_aspnet_Applications] FOREIGN KEY([ApplicationID])
	REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
END

GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	ALTER TABLE [dbo].[FG_PollAdmins] CHECK CONSTRAINT [FK_FG_PollAdmins_aspnet_Applications]
END

GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	ALTER TABLE [dbo].[FG_PollAdmins]  WITH CHECK ADD  CONSTRAINT [FK_FG_PollAdmins_FG_Polls] FOREIGN KEY([PollID])
	REFERENCES [dbo].[FG_Polls] ([PollID])
END

GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	ALTER TABLE [dbo].[FG_PollAdmins] CHECK CONSTRAINT [FK_FG_PollAdmins_FG_Polls]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	ALTER TABLE [dbo].[FG_PollAdmins]  WITH CHECK ADD  CONSTRAINT [FK_FG_PollAdmins_aspnet_Users] FOREIGN KEY([UserID])
	REFERENCES [dbo].[aspnet_Users] ([UserId])
END

GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	ALTER TABLE [dbo].[FG_PollAdmins] CHECK CONSTRAINT [FK_FG_PollAdmins_aspnet_Users]
END

GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	ALTER TABLE [dbo].[FG_PollAdmins]  WITH CHECK ADD  CONSTRAINT [FK_FG_PollAdmins_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
	REFERENCES [dbo].[aspnet_Users] ([UserId])
END

GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	ALTER TABLE [dbo].[FG_PollAdmins] CHECK CONSTRAINT [FK_FG_PollAdmins_aspnet_Users_Modifier]
END

GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	ALTER TABLE [dbo].[FG_ExtendedFormElements]
	ADD IsUnique bit NULL
END

GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.16.1' BEGIN
	UPDATE [dbo].[AppSetting]
		SET [Version] = 'v28.24.21.2'
END

GO