USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.44.8.6' BEGIN
	UPDATE [dbo].[AppSetting]
		SET [Version] = 'v29.7.8.6'
END
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	ALTER TABLE [dbo].[aspnet_Applications]
	ADD [CreationDate] datetime NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	ALTER TABLE [dbo].[aspnet_Applications]
	ADD [Size] varchar(100) NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	ALTER TABLE [dbo].[aspnet_Applications]
	ADD [ExpertiseFieldID] uniqueidentifier NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	ALTER TABLE [dbo].[aspnet_Applications]
	ADD [ExpertiseFieldName] nvarchar(255)
END
GO



SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	ALTER TABLE [dbo].[USR_UserApplications]
	ADD [Organization] [nvarchar](255) NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	ALTER TABLE [dbo].[USR_UserApplications]
	ADD [Department] [nvarchar](255) NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	ALTER TABLE [dbo].[USR_UserApplications]
	ADD [JobTitle] [nvarchar](255) NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	ALTER TABLE [dbo].[USR_UserApplications]
	ADD [EmploymentType] [varchar](50) NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	ALTER TABLE [dbo].[USR_UserApplications]
	ADD [CreationDate] datetime NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	ALTER TABLE [dbo].[USR_UserApplications]
	ADD [LastModificationDate] datetime NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	ALTER TABLE [dbo].[USR_UserApplications]
	ADD [Deleted] bit NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	EXEC ('UPDATE [dbo].[USR_UserApplications] SET Deleted = 0')
END
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	ALTER TABLE [dbo].[USR_Profile]
	ADD [AboutMe] [nvarchar](2000) NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	ALTER TABLE [dbo].[USR_Profile]
	ADD [CountryOfResidence] [nvarchar](255) NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	ALTER TABLE [dbo].[USR_Profile]
	ADD [Province] [nvarchar](255) NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	ALTER TABLE [dbo].[USR_Profile]
	ADD [City] [nvarchar](255) NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	ALTER TABLE [dbo].[USR_Profile]
	ADD [Settings] [nvarchar](max) NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	EXEC ('UPDATE A ' +
		'SET CreationDate = X.MinDate ' +
	'FROM [dbo].[aspnet_Applications] AS A ' +
		'INNER JOIN ( ' +
			'SELECT lg.ApplicationID, MIN(lg.[Date]) AS MinDate ' +
			'FROM [dbo].[LG_Logs] AS lg ' +
			'GROUP BY lg.[ApplicationID] ' +
		') AS X ' +
		'ON A.ApplicationID = A.ApplicationID ' +
	'WHERE A.CreationDate IS NULL')
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	EXEC ('UPDATE A ' +
		'SET CreationDate = ISNULL(X.MinDate, DATEADD(DAY, -10, GETDATE())) ' +
	'FROM [dbo].[USR_UserApplications] AS A ' +
		'LEFT JOIN ( ' +
			'SELECT lg.ApplicationID, lg.UserID, MIN(lg.[Date]) AS MinDate ' +
			'FROM [dbo].[LG_Logs] AS lg ' +
			'GROUP BY lg.[ApplicationID], lg.UserID ' +
		') AS X ' +
		'ON A.ApplicationID = A.ApplicationID AND X.UserID = A.UserID ' +
	'WHERE A.CreationDate IS NULL ')
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	EXEC ('UPDATE A ' +
		'SET JobTitle = P.JobTitle ' +
	'FROM [dbo].[USR_UserApplications] AS A ' +
		'INNER JOIN [dbo].[USR_Profile] AS P ' +
		'ON P.UserID = A.UserID')
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	IF EXISTS(select * FROM sys.views where name = 'USR_View_Users')
	DROP VIEW [dbo].[USR_View_Users]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	IF EXISTS(select * FROM sys.views where name = 'Users_Normal')
	DROP VIEW [dbo].[Users_Normal]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	ALTER TABLE [dbo].[USR_Profile]
	DROP COLUMN JobTitle
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	EXEC ('UPDATE A ' +
		'SET EmploymentType = P.EmploymentType ' +
	'FROM [dbo].[USR_UserApplications] AS A ' +
		'INNER JOIN [dbo].[USR_Profile] AS P ' +
		'ON P.UserID = A.UserID')
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	IF EXISTS(select * FROM sys.views where name = 'USR_View_Users')
	DROP VIEW [dbo].[USR_View_Users]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	IF EXISTS(select * FROM sys.views where name = 'Users_Normal')
	DROP VIEW [dbo].[Users_Normal]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	ALTER TABLE [dbo].[USR_Profile]
	DROP COLUMN EmploymentType
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	CREATE TABLE [dbo].[LG_RawLogs](
		[LogID] [bigint] IDENTITY(1,1) NOT NULL,
		[UserID] [uniqueidentifier] NULL,
		[ApplicationID] [uniqueidentifier] NULL,
		[Date] [datetime] NOT NULL,
		[Info] [nvarchar](max) NULL
	 CONSTRAINT [PK_LG_RawLogs] PRIMARY KEY CLUSTERED 
	(
		[LogID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	ALTER TABLE [dbo].[LG_RawLogs]  WITH CHECK ADD  CONSTRAINT [FK_LG_RawLogs_aspnet_Applications] FOREIGN KEY([ApplicationID])
	REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	ALTER TABLE [dbo].[LG_RawLogs] CHECK CONSTRAINT [FK_LG_RawLogs_aspnet_Applications]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	ALTER TABLE [dbo].[LG_RawLogs]  WITH CHECK ADD  CONSTRAINT [FK_LG_RawLogs_aspnet_Users] FOREIGN KEY([UserID])
	REFERENCES [dbo].[aspnet_Users] ([UserID])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
ALTER TABLE [dbo].[LG_RawLogs] CHECK CONSTRAINT [FK_LG_RawLogs_aspnet_Users]
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	ALTER TABLE [dbo].[FG_ExtendedFormElements]
	ADD [Info2] nvarchar(max) NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	ALTER TABLE [dbo].[FG_InstanceElements]
	ADD [Info2] nvarchar(max) NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	EXEC ('UPDATE [dbo].[FG_ExtendedFormElements] SET Info2 = Info')
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	EXEC ('UPDATE [dbo].[FG_InstanceElements] SET Info2 = Info')
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	ALTER TABLE [dbo].[FG_ExtendedFormElements]
	DROP COLUMN [Info]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	ALTER TABLE [dbo].[FG_InstanceElements]
	DROP COLUMN [Info]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	ALTER TABLE [dbo].[FG_ExtendedFormElements]
	ADD [Info] nvarchar(max) NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	ALTER TABLE [dbo].[FG_InstanceElements]
	ADD [Info] nvarchar(max) NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	EXEC ('UPDATE [dbo].[FG_ExtendedFormElements] SET Info = Info2')
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	EXEC ('UPDATE [dbo].[FG_InstanceElements] SET Info = Info2')
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	ALTER TABLE [dbo].[FG_ExtendedFormElements]
	DROP COLUMN [Info2]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	ALTER TABLE [dbo].[FG_InstanceElements]
	DROP COLUMN [Info2]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	ALTER TABLE [dbo].[WF_WorkFlowStates]
	ADD [UserID] uniqueidentifier NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	UPDATE [dbo].[AppSetting]
		SET [Version] = 'v29.7.8.9' -- 14000517
END
GO