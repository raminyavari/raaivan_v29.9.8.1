USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[DE_UpdateMembers]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DE_UpdateMembers]
GO

IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'ExchangeMemberTableType')
DROP TYPE dbo.ExchangeMemberTableType
GO



/****** Object:  Table [dbo].[CN_Experts]    Script Date: 08/01/2015 20:15:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

ALTER TABLE [dbo].[SH_Posts]
ADD [HasPicture] [bit] NULL

GO



/****** Object:  Table [dbo].[CN_Experts]    Script Date: 08/01/2015 20:15:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

ALTER TABLE [dbo].[LG_Logs]
ADD [NotAuthorized] [bit] NULL

GO




UPDATE [dbo].[AppSetting]
	SET [Version] = 'v27.0.0.0' -- 13940929
GO


