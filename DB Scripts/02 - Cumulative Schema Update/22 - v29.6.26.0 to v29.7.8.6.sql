USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.26.0' BEGIN
	ALTER TABLE [dbo].[FG_ExtendedForms]
	ADD TemplateFormID uniqueidentifier NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.26.0' BEGIN
	ALTER TABLE [dbo].[FG_ExtendedForms]  WITH CHECK ADD  CONSTRAINT [FK_FG_ExtendedForms_FG_ExtendedForms_TemplateID] FOREIGN KEY([TemplateFormID])
	REFERENCES [dbo].[FG_ExtendedForms] ([FormID])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.26.0' BEGIN
	ALTER TABLE [dbo].[FG_ExtendedForms] CHECK CONSTRAINT [FK_FG_ExtendedForms_FG_ExtendedForms_TemplateID]
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.26.0' BEGIN
	ALTER TABLE [dbo].[FG_ExtendedFormElements]
	ADD TemplateElementID uniqueidentifier NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.26.0' BEGIN
	ALTER TABLE [dbo].[FG_ExtendedFormElements]  WITH CHECK ADD  CONSTRAINT [FK_FG_ExtendedFormElements_FG_ExtendedFormElements_TemplateID] FOREIGN KEY([TemplateElementID])
	REFERENCES [dbo].[FG_ExtendedFormElements] ([ElementID])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.26.0' BEGIN
	ALTER TABLE [dbo].[FG_ExtendedFormElements] CHECK CONSTRAINT [FK_FG_ExtendedFormElements_FG_ExtendedFormElements_TemplateID]
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.26.0' BEGIN
	ALTER TABLE [dbo].[CN_NodeTypes]
	ADD TemplateTypeID uniqueidentifier NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.26.0' BEGIN
	ALTER TABLE [dbo].[CN_NodeTypes]  WITH CHECK ADD  CONSTRAINT [FK_CN_NodeTypes_CN_NodeTypes_TemplateID] FOREIGN KEY([TemplateTypeID])
	REFERENCES [dbo].[CN_NodeTypes] ([NodeTypeID])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.26.0' BEGIN
	ALTER TABLE [dbo].[CN_NodeTypes] CHECK CONSTRAINT [FK_CN_NodeTypes_CN_NodeTypes_TemplateID]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.26.0' BEGIN
	ALTER TABLE [dbo].[FG_ExtendedFormElements]
	ADD InitialValue nvarchar(max) NULL
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.26.0' BEGIN
	ALTER TABLE [dbo].[CN_NodeTypes]
	ADD AvatarName varchar(50) NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.26.0' BEGIN
	ALTER TABLE [dbo].[CN_Nodes]
	ADD AvatarName varchar(50) NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.26.0' BEGIN
	ALTER TABLE [dbo].[USR_Profile]
	ADD AvatarName varchar(50) NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.26.0' BEGIN
	ALTER TABLE [dbo].[aspnet_Applications]
	ADD AvatarName varchar(50) NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.26.0' BEGIN
	ALTER TABLE [dbo].[USR_Profile]
	ADD TwoStepAuthentication varchar(50) NULL
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.26.0' BEGIN
	CREATE TABLE [dbo].[RV_WorkSpaces](
		WorkSpaceID uniqueidentifier NOT NULL,
		Name nvarchar(255) NOT NULL,
		[Description] nvarchar(2000) NULL,
		AvatarName varchar(50) NULL,
		CreatorUserID uniqueidentifier NOT NULL,
		CreationDate datetime NOT NULL,
		LastModifierUserID uniqueidentifier NULL,
		LastModificationDate datetime NULL,
		Deleted bit NOT NULL
	 CONSTRAINT [PK_RV_WorkSpaces] PRIMARY KEY CLUSTERED 
	(
		[WorkSpaceID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.26.0' BEGIN
	ALTER TABLE [dbo].[RV_WorkSpaces]  WITH CHECK ADD  CONSTRAINT [FK_RV_WorkSpaces_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
	REFERENCES [dbo].[aspnet_Users] ([UserId])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.26.0' BEGIN
	ALTER TABLE [dbo].[RV_WorkSpaces] CHECK CONSTRAINT [FK_RV_WorkSpaces_aspnet_Users_Creator]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.26.0' BEGIN
	ALTER TABLE [dbo].[RV_WorkSpaces]  WITH CHECK ADD  CONSTRAINT [FK_RV_WorkSpaces_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
	REFERENCES [dbo].[aspnet_Users] ([UserId])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.26.0' BEGIN
	ALTER TABLE [dbo].[RV_WorkSpaces] CHECK CONSTRAINT [FK_RV_WorkSpaces_aspnet_Users_Modifier]
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.26.0' BEGIN
	CREATE TABLE [dbo].[RV_WorkSpaceApplications](
		[WorkSpaceID] uniqueidentifier NOT NULL,
		[ApplicationID] uniqueidentifier NOT NULL
	 CONSTRAINT [PK_RV_WorkSpaceApplications] PRIMARY KEY CLUSTERED 
	(
		[WorkSpaceID] ASC,
		[ApplicationID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.26.0' BEGIN
	ALTER TABLE [dbo].[RV_WorkSpaceApplications]  WITH CHECK ADD  CONSTRAINT [FK_RV_WorkSpaceApplications_RV_WorkSpaces] FOREIGN KEY([WorkSpaceID])
	REFERENCES [dbo].[RV_WorkSpaces] ([WorkSpaceID])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.26.0' BEGIN
	ALTER TABLE [dbo].[RV_WorkSpaceApplications] CHECK CONSTRAINT [FK_RV_WorkSpaceApplications_RV_WorkSpaces]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.26.0' BEGIN
	ALTER TABLE [dbo].[RV_WorkSpaceApplications]  WITH CHECK ADD  CONSTRAINT [FK_RV_WorkSpaceApplications_aspnet_Applications] FOREIGN KEY([ApplicationID])
	REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.26.0' BEGIN
	ALTER TABLE [dbo].[RV_WorkSpaceApplications] CHECK CONSTRAINT [FK_RV_WorkSpaceApplications_aspnet_Applications]
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.26.0' BEGIN
	ALTER TABLE [dbo].[aspnet_Applications]
	ADD [InvitationID] uniqueidentifier NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.26.0' BEGIN
	ALTER TABLE [dbo].[aspnet_Applications]
	ADD [EnableInvitationLink] bit NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.26.0' BEGIN
	ALTER TABLE [dbo].[aspnet_Applications]
	ADD [Language] varchar(50) NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.26.0' BEGIN
	ALTER TABLE [dbo].[aspnet_Applications]
	ADD [Calendar] varchar(50) NULL
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.26.0' BEGIN
	ALTER TABLE [dbo].[USR_Profile]
	ADD [EnableNewsLetter] bit NULL
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.26.0' BEGIN
	UPDATE [dbo].[AppSetting]
		SET [Version] = 'v29.7.8.6' -- 14000227
END
GO