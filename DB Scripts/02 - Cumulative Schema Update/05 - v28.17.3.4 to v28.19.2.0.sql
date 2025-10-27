USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.17.3.4' BEGIN
	SET ANSI_PADDING ON
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.17.3.4' BEGIN
	CREATE TABLE [dbo].[FG_Changes](
		[ID] [bigint] IDENTITY(1, 1) NOT NULL,
		[ElementID] [uniqueidentifier] NOT NULL,
		[TextValue] [nvarchar](max) NULL,
		[FloatValue] [float] NULL,
		[BitValue] [bit] NULL,
		[DateValue] [datetime] NULL,
		[GuidValue] [uniqueidentifier] NULL,
		[CreatorUserID] [uniqueidentifier] NOT NULL,
		[CreationDate] [datetime] NOT NULL,
		[Deleted] [bit] NOT NULL,
		[ApplicationID] [uniqueidentifier] NOT NULL
	 CONSTRAINT [PK_FG_Changes] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.17.3.4' BEGIN
	CREATE UNIQUE INDEX UK_FG_Changes_ElementID ON [dbo].[FG_Changes]
	(
		[ApplicationID] ASC,
		[ElementID] ASC,
		[Deleted] ASC,
		[CreationDate] ASC,
		[CreatorUserID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.17.3.4' BEGIN
	SET ANSI_PADDING OFF
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.17.3.4' BEGIN
	ALTER TABLE [dbo].[FG_Changes]  WITH CHECK ADD  CONSTRAINT [FK_FG_Changes_aspnet_Applications] FOREIGN KEY([ApplicationID])
	REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.17.3.4' BEGIN
	ALTER TABLE [dbo].[FG_Changes] CHECK CONSTRAINT [FK_FG_Changes_aspnet_Applications]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.17.3.4' BEGIN
	ALTER TABLE [dbo].[FG_Changes]  WITH CHECK ADD  CONSTRAINT [FK_FG_Changes_aspnet_Users] FOREIGN KEY([CreatorUserID])
	REFERENCES [dbo].[aspnet_Users] ([UserId])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.17.3.4' BEGIN
	ALTER TABLE [dbo].[FG_Changes] CHECK CONSTRAINT [FK_FG_Changes_aspnet_Users]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.17.3.4' BEGIN
	SET ANSI_PADDING ON
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.17.3.4' BEGIN
	CREATE TABLE [dbo].[RV_VariablesWithOwner](
		[ID] [bigint] IDENTITY(1, 1) NOT NULL,
		[OwnerID] [uniqueidentifier] NOT NULL,
		[Name] [varchar](100) NOT NULL,
		[Value] [nvarchar](max) NOT NULL,
		[CreatorUserID] [uniqueidentifier] NOT NULL,
		[CreationDate] [datetime] NOT NULL,
		[LastModifierUserID] [uniqueidentifier] NULL,
		[LastModificationDate] [datetime] NULL,
		[Deleted] [bit] NOT NULL,
		[ApplicationID] [uniqueidentifier] NOT NULL,
	 CONSTRAINT [PK_RV_VariablesWithOwner] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.17.3.4' BEGIN
	SET ANSI_PADDING OFF
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.17.3.4' BEGIN
	ALTER TABLE [dbo].[RV_VariablesWithOwner]  WITH CHECK ADD  CONSTRAINT [FK_RV_VariablesWithOwner_aspnet_Applications] FOREIGN KEY([ApplicationID])
	REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.17.3.4' BEGIN
	ALTER TABLE [dbo].[RV_VariablesWithOwner] CHECK CONSTRAINT [FK_RV_VariablesWithOwner_aspnet_Applications]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.17.3.4' BEGIN
	ALTER TABLE [dbo].[RV_VariablesWithOwner]  WITH CHECK ADD  CONSTRAINT [FK_RV_VariablesWithOwner_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
	REFERENCES [dbo].[aspnet_Users] ([UserId])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.17.3.4' BEGIN
	ALTER TABLE [dbo].[RV_VariablesWithOwner] CHECK CONSTRAINT [FK_RV_VariablesWithOwner_aspnet_Users_Creator]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.17.3.4' BEGIN
	ALTER TABLE [dbo].[RV_VariablesWithOwner]  WITH CHECK ADD  CONSTRAINT [FK_RV_VariablesWithOwner_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
	REFERENCES [dbo].[aspnet_Users] ([UserId])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.17.3.4' BEGIN
	ALTER TABLE [dbo].[RV_VariablesWithOwner] CHECK CONSTRAINT [FK_RV_VariablesWithOwner_aspnet_Users_Modifier]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.17.3.4' BEGIN
	UPDATE [dbo].[AppSetting]
		SET [Version] = 'v28.19.2.0'
END
GO