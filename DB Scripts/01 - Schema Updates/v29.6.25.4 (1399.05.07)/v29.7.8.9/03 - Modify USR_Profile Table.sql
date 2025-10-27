USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER TABLE [dbo].[USR_Profile]
ADD [AboutMe] [nvarchar](2000) NULL
GO

ALTER TABLE [dbo].[USR_Profile]
ADD [CountryOfResidence] [nvarchar](255) NULL
GO

ALTER TABLE [dbo].[USR_Profile]
ADD [Province] [nvarchar](255) NULL
GO

ALTER TABLE [dbo].[USR_Profile]
ADD [City] [nvarchar](255) NULL
GO

ALTER TABLE [dbo].[USR_Profile]
ADD [Settings] [nvarchar](max) NULL
GO