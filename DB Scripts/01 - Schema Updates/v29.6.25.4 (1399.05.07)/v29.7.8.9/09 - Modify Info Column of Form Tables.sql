USE [EKM_App]
GO

/****** Object:  Table [dbo].[LG_Logs]    Script Date: 8/7/2021 4:30:42 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER TABLE [dbo].[FG_ExtendedFormElements]
ADD [Info2] nvarchar(max) NULL
GO

ALTER TABLE [dbo].[FG_InstanceElements]
ADD [Info2] nvarchar(max) NULL
GO

UPDATE [dbo].[FG_ExtendedFormElements]
	SET Info2 = Info
GO

UPDATE [dbo].[FG_InstanceElements]
	SET Info2 = Info
GO

ALTER TABLE [dbo].[FG_ExtendedFormElements]
DROP COLUMN [Info]
GO

ALTER TABLE [dbo].[FG_InstanceElements]
DROP COLUMN [Info]
GO

ALTER TABLE [dbo].[FG_ExtendedFormElements]
ADD [Info] nvarchar(max) NULL
GO

ALTER TABLE [dbo].[FG_InstanceElements]
ADD [Info] nvarchar(max) NULL
GO

UPDATE [dbo].[FG_ExtendedFormElements]
	SET Info = Info2
GO

UPDATE [dbo].[FG_InstanceElements]
	SET Info = Info2
GO

ALTER TABLE [dbo].[FG_ExtendedFormElements]
DROP COLUMN [Info2]
GO

ALTER TABLE [dbo].[FG_InstanceElements]
DROP COLUMN [Info2]
GO