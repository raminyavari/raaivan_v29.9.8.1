USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER TABLE [dbo].[USR_UserApplications]
ADD [Organization] [nvarchar](255) NULL
GO

ALTER TABLE [dbo].[USR_UserApplications]
ADD [Department] [nvarchar](255) NULL
GO

ALTER TABLE [dbo].[USR_UserApplications]
ADD [JobTitle] [nvarchar](255) NULL
GO

ALTER TABLE [dbo].[USR_UserApplications]
ADD [EmploymentType] [varchar](50) NULL
GO

ALTER TABLE [dbo].[USR_UserApplications]
ADD [CreationDate] datetime NULL
GO

ALTER TABLE [dbo].[USR_UserApplications]
ADD [LastModificationDate] datetime NULL
GO

ALTER TABLE [dbo].[USR_UserApplications]
ADD [Deleted] bit NULL
GO

UPDATE [dbo].[USR_UserApplications]
	SET Deleted = 0
GO