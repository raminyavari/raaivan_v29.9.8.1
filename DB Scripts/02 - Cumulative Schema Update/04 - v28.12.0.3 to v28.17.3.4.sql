USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.12.0.3' BEGIN
	UPDATE [dbo].[DCT_Files]
		SET Size = Size * 1024
	WHERE OwnerType = N'Message' OR OwnerType = N'Wiki' OR OwnerType = N'WorkFlow'
	
	UPDATE [dbo].[AppSetting]
		SET [Version] = 'v28.17.3.4' -- 13970319
END

GO

