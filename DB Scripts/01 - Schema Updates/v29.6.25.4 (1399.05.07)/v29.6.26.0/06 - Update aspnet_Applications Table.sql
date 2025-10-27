USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER TABLE [dbo].[aspnet_Applications]
ADD CreatorUserID uniqueidentifier NULL
GO

ALTER TABLE [dbo].[aspnet_Applications]
ADD Deleted bit NULL
GO

ALTER TABLE [dbo].[aspnet_Applications]
ADD Title nvarchar(255) NULL
GO
