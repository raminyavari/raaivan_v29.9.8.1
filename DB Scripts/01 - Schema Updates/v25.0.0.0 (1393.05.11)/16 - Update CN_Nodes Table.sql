USE [EKM_App]
GO

/****** Object:  Table [dbo].[Phrases]    Script Date: 04/26/2013 20:38:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER TABLE [dbo].[CN_Nodes]
DROP COLUMN TSkill
GO

ALTER TABLE [dbo].[CN_Nodes]
DROP COLUMN TExperience
GO

ALTER TABLE [dbo].[CN_Nodes]
DROP COLUMN TContent
GO
