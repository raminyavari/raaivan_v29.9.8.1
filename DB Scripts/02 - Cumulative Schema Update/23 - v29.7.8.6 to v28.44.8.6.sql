USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v29.7.8.6' BEGIN
	UPDATE [dbo].[AppSetting]
		SET [Version] = 'v28.44.8.6' -- 14000227
END
GO