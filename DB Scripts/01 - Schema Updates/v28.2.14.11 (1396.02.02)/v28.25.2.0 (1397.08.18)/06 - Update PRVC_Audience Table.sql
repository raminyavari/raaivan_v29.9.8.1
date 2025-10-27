USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

ALTER TABLE [dbo].[PRVC_Audience]
ADD [PermissionType] [nvarchar](50) NULL
GO

SET ANSI_PADDING OFF
GO


UPDATE [dbo].[PRVC_Audience]
	SET PermissionType = PrivacyType
GO


ALTER TABLE [dbo].[PRVC_Audience]
DROP COLUMN [PrivacyType]
GO