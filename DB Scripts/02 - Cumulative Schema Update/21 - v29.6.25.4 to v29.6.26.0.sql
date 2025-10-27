USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

WHILE EXISTS(SELECT TOP(1) * FROM sys.views) BEGIN
	DECLARE @ViewName varchar(500)
	DECLARE Cur cursor 

	FOR SELECT [Name] 
	FROM sys.views
	OPEN Cur
	FETCH NEXT FROM Cur INTO @ViewName
	WHILE @@FETCH_STATUS = 0
	BEGIN
		BEGIN TRY
			EXEC('DROP VIEW ' + @ViewName)
		END TRY
		BEGIN CATCH
		END CATCH
		
		FETCH NEXT FROM Cur INTO @ViewName
	END
	CLOSE Cur
	DEALLOCATE Cur
END

GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.25.4' BEGIN
	ALTER TABLE [dbo].[USR_EmailAddresses]
	ADD Validated bit NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.25.4' BEGIN
	ALTER TABLE [dbo].[USR_EmailAddresses]
	DROP CONSTRAINT [FK_USR_EmailAddresses_aspnet_Applications]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.25.4' BEGIN
	ALTER TABLE [dbo].[USR_EmailAddresses]
	DROP COLUMN ApplicationID
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.25.4' BEGIN
	ALTER TABLE [dbo].[USR_PhoneNumbers]
	ADD Validated bit NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.25.4' BEGIN
	ALTER TABLE [dbo].[USR_PhoneNumbers]
	DROP CONSTRAINT [FK_USR_PhoneNumbers_aspnet_Applications]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.25.4' BEGIN
	ALTER TABLE [dbo].[USR_PhoneNumbers]
	DROP COLUMN ApplicationID
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.25.4' BEGIN
	CREATE TABLE [dbo].[USR_UserApplications](
		[UserID] [uniqueidentifier] NOT NULL,
		[ApplicationID] [uniqueidentifier] NOT NULL
	 CONSTRAINT [PK_USR_UserApplications] PRIMARY KEY CLUSTERED 
	(
		[UserID] ASC,
		[ApplicationID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.25.4' BEGIN
	ALTER TABLE [dbo].[USR_UserApplications]  WITH CHECK ADD  CONSTRAINT [FK_USR_UserApplications_aspnet_Applications] FOREIGN KEY([ApplicationID])
	REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.25.4' BEGIN
	ALTER TABLE [dbo].[USR_UserApplications] CHECK CONSTRAINT [FK_USR_UserApplications_aspnet_Applications]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.25.4' BEGIN
	ALTER TABLE [dbo].[USR_UserApplications]  WITH CHECK ADD  CONSTRAINT [FK_USR_UserApplications_aspnet_Users] FOREIGN KEY([UserID])
	REFERENCES [dbo].[aspnet_Users] ([UserId])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.25.4' BEGIN
	ALTER TABLE [dbo].[USR_UserApplications] CHECK CONSTRAINT [FK_USR_UserApplications_aspnet_Users]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.25.4' BEGIN
	DECLARE @str varchar(max) = 
		'INSERT INTO [dbo].[USR_UserApplications] (ApplicationID, UserID) ' +
		'SELECT P.ApplicationID, P.UserID ' +
		'FROM [dbo].[USR_Profile] AS P'
		
	EXEC (@str)
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.25.4' BEGIN
	ALTER TABLE [dbo].[aspnet_Users]
	DROP COLUMN [ApplicationID]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.25.4' BEGIN
	ALTER TABLE [dbo].[aspnet_Membership]
	DROP CONSTRAINT [FK__aspnet_Me__Appli__009508B4]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.25.4' BEGIN
	ALTER TABLE [dbo].[aspnet_Membership]
	DROP COLUMN [ApplicationID]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.25.4' BEGIN
	ALTER TABLE [dbo].[USR_Profile]
	DROP CONSTRAINT [FK_USR_Profile_aspnet_Applications]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.25.4' BEGIN
	ALTER TABLE [dbo].[USR_Profile]
	DROP COLUMN [ApplicationID]
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.25.4' BEGIN
	ALTER TABLE [dbo].[USR_EmailContacts]
	DROP CONSTRAINT [FK_USR_EmailContacts_aspnet_Applications]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.25.4' BEGIN
	ALTER TABLE [dbo].[USR_EmailContacts]
	DROP COLUMN [ApplicationID]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.25.4' BEGIN
	ALTER TABLE [dbo].[USR_PassResetTickets]
	DROP CONSTRAINT [FK_USR_PassResetTickets_aspnet_Applications]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.25.4' BEGIN
	ALTER TABLE [dbo].[USR_PassResetTickets]
	DROP COLUMN [ApplicationID]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.25.4' BEGIN
	ALTER TABLE [dbo].[USR_PasswordsHistory]
	DROP CONSTRAINT [FK_USR_PasswordsHistory_aspnet_Applications]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.25.4' BEGIN
	ALTER TABLE [dbo].[USR_PasswordsHistory]
	DROP COLUMN [ApplicationID]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.25.4' BEGIN
	ALTER TABLE [dbo].[USR_TemporaryUsers]
	DROP CONSTRAINT [FK_USR_TemporaryUsers_aspnet_Applications]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.25.4' BEGIN
	ALTER TABLE [dbo].[USR_TemporaryUsers]
	DROP COLUMN [ApplicationID]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.25.4' BEGIN
	ALTER TABLE [dbo].[USR_TemporaryUsers]
	ADD PhoneNumber varchar(20) NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.25.4' BEGIN
	ALTER TABLE [dbo].[aspnet_Applications]
	ADD CreatorUserID uniqueidentifier NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.25.4' BEGIN
	ALTER TABLE [dbo].[aspnet_Applications]
	ADD Deleted bit NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.25.4' BEGIN
	ALTER TABLE [dbo].[aspnet_Applications]
	ADD Title nvarchar(255) NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.6.25.4' BEGIN
	UPDATE [dbo].[AppSetting]
		SET [Version] = 'v29.6.26.0' -- 13990906
END
GO


