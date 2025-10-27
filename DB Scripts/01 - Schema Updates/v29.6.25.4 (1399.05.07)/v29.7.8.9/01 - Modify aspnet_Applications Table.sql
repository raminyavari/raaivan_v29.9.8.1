USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER TABLE [dbo].[aspnet_Applications]
ADD [CreationDate] datetime NULL
GO

ALTER TABLE [dbo].[aspnet_Applications]
ADD [Size] varchar(100) NULL
GO

ALTER TABLE [dbo].[aspnet_Applications]
ADD [ExpertiseFieldID] uniqueidentifier NULL
GO

ALTER TABLE [dbo].[aspnet_Applications]
ADD [ExpertiseFieldName] nvarchar(255)
GO
