USE [EKM_App]
GO



IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.28.8.1' BEGIN
	UPDATE [dbo].[AppSetting]
		SET [Version] = 'v28.30.9.3'
END
GO


