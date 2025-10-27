USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.33.3.1' BEGIN
	ALTER TABLE [dbo].[CN_Services]
	ADD [UniqueAdminMember] [bit] NULL
END
GO

SET ANSI_PADDING OFF
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.33.3.1' BEGIN
	UPDATE S
		SET UniqueAdminMember = 1
	FROM [dbo].[CN_NodeTypes] AS NT
		INNER JOIN [dbo].[CN_Services] AS S
		ON S.ApplicationID = NT.ApplicationID AND S.NodeTypeID = NT.NodeTypeID
	WHERE NT.AdditionalID = N'2'
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.33.3.1' BEGIN
	UPDATE S
		SET UniqueMembership = 1,
			UniqueAdminMember = 1
	FROM [dbo].[CN_NodeTypes] AS NT
		INNER JOIN [dbo].[CN_Services] AS S
		ON S.ApplicationID = NT.ApplicationID AND S.NodeTypeID = NT.NodeTypeID
	WHERE NT.AdditionalID = N'6'
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.33.3.1' BEGIN
	UPDATE [dbo].[AppSetting]
		SET [Version] = 'v28.35.8.6' -- 13980704
END
GO