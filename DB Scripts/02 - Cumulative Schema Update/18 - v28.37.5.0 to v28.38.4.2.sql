USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.37.5.0' BEGIN
	ALTER TABLE [dbo].[FG_ExtendedFormElements]
	ADD [Help] [nvarchar](2000) NULL
END
GO

SET ANSI_PADDING OFF
GO



IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.37.5.0' BEGIN
	UPDATE [dbo].[AppSetting]
		SET [Version] = 'v28.38.4.2' -- 13990118
END
GO