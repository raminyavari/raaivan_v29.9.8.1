USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.45.0.0' BEGIN
	UPDATE [dbo].[AppSetting]
		SET [Version] = 'v29.9.8.1' -- 14000725
END
GO