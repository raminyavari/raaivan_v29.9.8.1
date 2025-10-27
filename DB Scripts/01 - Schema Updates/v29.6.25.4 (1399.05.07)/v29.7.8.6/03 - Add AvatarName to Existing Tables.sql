USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER TABLE [dbo].[CN_NodeTypes]
ADD AvatarName varchar(50) NULL
GO

ALTER TABLE [dbo].[CN_Nodes]
ADD AvatarName varchar(50) NULL
GO

ALTER TABLE [dbo].[USR_Profile]
ADD AvatarName varchar(50) NULL
GO

ALTER TABLE [dbo].[aspnet_Applications]
ADD AvatarName varchar(50) NULL
GO
