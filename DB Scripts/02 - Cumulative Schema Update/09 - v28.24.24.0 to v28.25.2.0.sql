USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	SET ANSI_PADDING ON
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	CREATE TABLE [dbo].[PRVC_DefaultPermissions](
		[ObjectID] [uniqueidentifier] NOT NULL,
		[PermissionType] [varchar](20) NOT NULL,
		[DefaultValue] [varchar](20) NOT NULL,
		[CreatorUserID] [uniqueidentifier] NOT NULL,
		[CreationDate] [datetime] NOT NULL,
		[LastModifierUserID] [uniqueidentifier] NULL,
		[LastModificationDate] [datetime] NULL,
		[ApplicationID] [uniqueidentifier] NULL
	 CONSTRAINT [PK_PRVC_DefaultPermissions] PRIMARY KEY CLUSTERED 
	(
		[ObjectID] ASC,
		[PermissionType] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	SET ANSI_PADDING OFF
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	ALTER TABLE [dbo].[PRVC_DefaultPermissions]  WITH CHECK ADD  CONSTRAINT [FK_PRVC_DefaultPermissions_aspnet_Applications] FOREIGN KEY([ApplicationID])
	REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	ALTER TABLE [dbo].[PRVC_DefaultPermissions] CHECK CONSTRAINT [FK_PRVC_DefaultPermissions_aspnet_Applications]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	ALTER TABLE [dbo].[PRVC_DefaultPermissions]  WITH CHECK ADD  CONSTRAINT [FK_PRVC_DefaultPermissions_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
	REFERENCES [dbo].[aspnet_Users] ([UserId])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	ALTER TABLE [dbo].[PRVC_DefaultPermissions] CHECK CONSTRAINT [FK_PRVC_DefaultPermissions_aspnet_Users_Creator]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	ALTER TABLE [dbo].[PRVC_DefaultPermissions]  WITH CHECK ADD  CONSTRAINT [FK_PRVC_DefaultPermissions_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
	REFERENCES [dbo].[aspnet_Users] ([UserId])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	ALTER TABLE [dbo].[PRVC_DefaultPermissions] CHECK CONSTRAINT [FK_PRVC_DefaultPermissions_aspnet_Users_Modifier]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	INSERT INTO [dbo].[PRVC_DefaultPermissions] (ApplicationID, ObjectID, PermissionType, DefaultValue,
		CreatorUserID, CreationDate, LastModifierUserID, LastModificationDate)
	SELECT P.ApplicationID, P.ObjectID, N'Create', P.PrivacyType, P.CreatorUserID, P.CreationDate,
		P.LastModifierUserID, P.LastModificationDate
	FROM [dbo].[PRVC_PrivacyType] AS P
		INNER JOIN [dbo].[CN_NodeTypes] AS NT
		ON NT.ApplicationID = P.ApplicationID AND NT.NodeTypeID = P.ObjectID
	WHERE ISNULL(P.PrivacyType, N'') <> N''
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	INSERT INTO [dbo].[PRVC_DefaultPermissions] (ApplicationID, ObjectID, PermissionType, DefaultValue,
		CreatorUserID, CreationDate, LastModifierUserID, LastModificationDate)
	SELECT P.ApplicationID, P.ObjectID, N'View', P.PrivacyType, P.CreatorUserID, P.CreationDate,
		P.LastModifierUserID, P.LastModificationDate
	FROM [dbo].[PRVC_PrivacyType] AS P
		INNER JOIN [dbo].[CN_Nodes] AS NT
		ON NT.ApplicationID = P.ApplicationID AND NT.NodeID = P.ObjectID
	WHERE ISNULL(P.PrivacyType, N'') <> N''
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	INSERT INTO [dbo].[PRVC_DefaultPermissions] (ApplicationID, ObjectID, PermissionType, DefaultValue,
		CreatorUserID, CreationDate, LastModifierUserID, LastModificationDate)
	SELECT P.ApplicationID, P.ObjectID, N'View', P.PrivacyType, P.CreatorUserID, P.CreationDate,
		P.LastModifierUserID, P.LastModificationDate
	FROM [dbo].[PRVC_PrivacyType] AS P
		LEFT JOIN [dbo].[PRVC_DefaultPermissions] AS NT
		ON NT.ApplicationID = P.ApplicationID AND NT.ObjectID = P.ObjectID
	WHERE ISNULL(P.PrivacyType, N'') <> N'' AND NT.ObjectID IS NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	UPDATE D
		SET DefaultValue = N'Create'
	FROM [dbo].[PRVC_DefaultPermissions] AS D
		INNER JOIN [dbo].[CN_Services] AS S
		ON S.ApplicationID = D.ApplicationID AND S.NodeTypeID = D.ObjectID
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	UPDATE D
		SET DefaultValue = N'Create'
	FROM [dbo].[PRVC_DefaultPermissions] AS D
		INNER JOIN [dbo].[QA_WorkFlows] AS W
		ON W.ApplicationID = D.ApplicationID AND W.WorkFlowID = D.ObjectID
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	UPDATE [dbo].[PRVC_DefaultPermissions]
		SET DefaultValue = N'Public'
	WHERE DefaultValue = N'PublicAsDefault'
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	SET ANSI_PADDING ON
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	CREATE TABLE [dbo].[PRVC_Settings](
		[ObjectID] [uniqueidentifier] NOT NULL,
		[ConfidentialityID] [uniqueidentifier] NULL,
		[CalculateHierarchy] [bit] NULL,
		[CreatorUserID] [uniqueidentifier] NOT NULL,
		[CreationDate] [datetime] NOT NULL,
		[LastModifierUserID] [uniqueidentifier] NULL,
		[LastModificationDate] [datetime] NULL,
		[ApplicationID] [uniqueidentifier] NULL
	 CONSTRAINT [PK_PRVC_Settings] PRIMARY KEY CLUSTERED 
	(
		[ObjectID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	SET ANSI_PADDING OFF
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	ALTER TABLE [dbo].[PRVC_Settings]  WITH CHECK ADD  CONSTRAINT [FK_PRVC_Settings_aspnet_Applications] FOREIGN KEY([ApplicationID])
	REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	ALTER TABLE [dbo].[PRVC_Settings] CHECK CONSTRAINT [FK_PRVC_Settings_aspnet_Applications]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	ALTER TABLE [dbo].[PRVC_Settings]  WITH CHECK ADD  CONSTRAINT [FK_PRVC_Settings_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
	REFERENCES [dbo].[aspnet_Users] ([UserId])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	ALTER TABLE [dbo].[PRVC_Settings] CHECK CONSTRAINT [FK_PRVC_Settings_aspnet_Users_Creator]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	ALTER TABLE [dbo].[PRVC_Settings]  WITH CHECK ADD  CONSTRAINT [FK_PRVC_Settings_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
	REFERENCES [dbo].[aspnet_Users] ([UserId])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	ALTER TABLE [dbo].[PRVC_Settings] CHECK CONSTRAINT [FK_PRVC_Settings_aspnet_Users_Modifier]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	ALTER TABLE [dbo].[PRVC_Settings]  WITH CHECK ADD  CONSTRAINT [FK_PRVC_Settings_PRVC_ConfidentialityLevels] FOREIGN KEY([ConfidentialityID])
	REFERENCES [dbo].[PRVC_ConfidentialityLevels] ([ID])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	ALTER TABLE [dbo].[PRVC_Settings] CHECK CONSTRAINT [FK_PRVC_Settings_PRVC_ConfidentialityLevels]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	INSERT INTO [dbo].[PRVC_Settings](ApplicationID, ObjectID, ConfidentialityID, CalculateHierarchy,
		CreatorUserID, CreationDate, LastModifierUserID, LastModificationDate)
	SELECT PT.ApplicationID, PT.ObjectID, PT.ConfidentialityID, PT.CalculateHierarchy,
		PT.CreatorUserID, PT.CreationDate, PT.LastModifierUserID, PT.LastModificationDate
	FROM [dbo].[PRVC_PrivacyType] AS PT
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	IF EXISTS(select * FROM sys.views where name = 'PRVC_View_Confidentialities')
	DROP VIEW [dbo].[PRVC_View_Confidentialities]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	DROP TABLE [dbo].[PRVC_PrivacyType]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	SET ANSI_PADDING ON
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	ALTER TABLE [dbo].[PRVC_Audience]
	ADD [PermissionType] [nvarchar](50) NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	SET ANSI_PADDING OFF
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	EXEC ('UPDATE [dbo].[PRVC_Audience] ' +
		'SET PermissionType = PrivacyType')
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	ALTER TABLE [dbo].[PRVC_Audience]
	DROP COLUMN [PrivacyType]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	UPDATE [dbo].[PRVC_Audience]
		SET PermissionType = N'View'
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	UPDATE A
		SET PermissionType = N'Create'
	FROM [dbo].[PRVC_Audience] AS A
		INNER JOIN [dbo].[CN_NodeTypes] AS NT
		ON NT.ApplicationID = A.ApplicationID AND NT.NodeTypeID = A.ObjectID
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	UPDATE A
		SET PermissionType = N'Create'
	FROM [dbo].[PRVC_Audience] AS A
		INNER JOIN [dbo].[QA_WorkFlows] AS NT
		ON NT.ApplicationID = A.ApplicationID AND NT.WorkFlowID = A.ObjectID
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	CREATE TABLE [dbo].[PRVC_TempAudience](
		[ObjectID] [uniqueidentifier] NOT NULL,
		[RoleID] [uniqueidentifier] NOT NULL,
		[PermissionType] [nvarchar](50) NOT NULL,
		[Allow] [bit] NOT NULL,
		[ExpirationDate] [datetime] NULL,
		[CreatorUserID] [uniqueidentifier] NULL,
		[CreationDate] [datetime] NULL,
		[LastModifierUserID] [uniqueidentifier] NULL,
		[LastModificationDate] [datetime] NULL,
		[Deleted] [bit] NOT NULL,
		[ApplicationID] [uniqueidentifier] NULL
	 CONSTRAINT [PK_PRVC_TempAudience] PRIMARY KEY CLUSTERED 
	(
		[ObjectID] ASC,
		[RoleID] ASC,
		[PermissionType] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	INSERT INTO [dbo].[PRVC_TempAudience] (ObjectID, RoleID, PermissionType, Allow, ExpirationDate,
		CreatorUserID, CreationDate, LastModifierUserID, LastModificationDate, Deleted, ApplicationID)
	SELECT A.ObjectID, A.RoleID, ISNULL(A.PermissionType, N'View'), A.Allow, A.ExpirationDate, 
		A.CreatorUserID, A.CreationDate, A.LastModifierUserID, A.LastModificationDate, A.Deleted, A.ApplicationID
	FROM [dbo].[PRVC_Audience] AS A
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	DROP TABLE [dbo].[PRVC_Audience]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	SET ANSI_PADDING ON
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	CREATE TABLE [dbo].[PRVC_Audience](
		[ObjectID] [uniqueidentifier] NOT NULL,
		[RoleID] [uniqueidentifier] NOT NULL,
		[PermissionType] [nvarchar](50) NOT NULL,
		[Allow] [bit] NOT NULL,
		[ExpirationDate] [datetime] NULL,
		[CreatorUserID] [uniqueidentifier] NULL,
		[CreationDate] [datetime] NULL,
		[LastModifierUserID] [uniqueidentifier] NULL,
		[LastModificationDate] [datetime] NULL,
		[Deleted] [bit] NOT NULL,
		[ApplicationID] [uniqueidentifier] NULL
	 CONSTRAINT [PK_PRVC_Audience] PRIMARY KEY CLUSTERED 
	(
		[ObjectID] ASC,
		[RoleID] ASC,
		[PermissionType] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	SET ANSI_PADDING OFF
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	ALTER TABLE [dbo].[PRVC_Audience]  WITH CHECK ADD  CONSTRAINT [FK_PRVC_Audience_aspnet_Applications] FOREIGN KEY([ApplicationID])
	REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	ALTER TABLE [dbo].[PRVC_Audience] CHECK CONSTRAINT [FK_PRVC_Audience_aspnet_Applications]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	ALTER TABLE [dbo].[PRVC_Audience]  WITH CHECK ADD  CONSTRAINT [FK_PRVC_Audience_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
	REFERENCES [dbo].[aspnet_Users] ([UserId])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	ALTER TABLE [dbo].[PRVC_Audience] CHECK CONSTRAINT [FK_PRVC_Audience_aspnet_Users_Creator]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	ALTER TABLE [dbo].[PRVC_Audience]  WITH CHECK ADD  CONSTRAINT [FK_PRVC_Audience_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
	REFERENCES [dbo].[aspnet_Users] ([UserId])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	ALTER TABLE [dbo].[PRVC_Audience] CHECK CONSTRAINT [FK_PRVC_Audience_aspnet_Users_Modifier]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	INSERT INTO [dbo].[PRVC_Audience]
	SELECT *
	FROM [dbo].[PRVC_TempAudience]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	DROP TABLE [dbo].[PRVC_TempAudience]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	SET ANSI_PADDING ON
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	ALTER TABLE [dbo].[FG_ExtendedFormElements]
	ADD [UniqueValue] [bit] NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	SET ANSI_PADDING OFF
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.24.24.0' BEGIN
	UPDATE [dbo].[AppSetting]
		SET [Version] = 'v28.25.2.0' -- 13970818
END
GO