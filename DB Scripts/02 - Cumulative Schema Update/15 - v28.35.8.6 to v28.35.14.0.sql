USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.35.8.6' BEGIN
	ALTER TABLE [dbo].[CN_Services]
	ADD [DisableRelatedNodesSelect] [bit] NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.35.8.6' BEGIN
	ALTER TABLE [dbo].[CN_Services]
	ADD [DisableAbstractAndKeywords] [bit] NULL
END
GO

SET ANSI_PADDING OFF
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.35.8.6' BEGIN
	UPDATE [dbo].[AppSetting]
		SET [Version] = 'v28.35.14.0' -- 13980827
END
GO