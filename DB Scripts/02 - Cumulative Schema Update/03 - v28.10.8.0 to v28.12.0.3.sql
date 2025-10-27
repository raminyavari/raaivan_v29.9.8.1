USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.10.8.0' BEGIN
	DELETE [dbo].[DCT_FileContents]
	WHERE FileNotFound = 1

	ALTER TABLE [dbo].[FG_ExtendedForms]
	ADD Name varchar(100) NULL

	ALTER TABLE [dbo].[FG_ExtendedFormElements]
	ADD Name varchar(100) NULL

	UPDATE [dbo].[AppSetting]
		SET [Version] = 'v28.12.0.3' -- 13961222
END

GO