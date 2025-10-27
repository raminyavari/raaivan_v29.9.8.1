USE [EKM_App]
GO

/****** Object:  Table [dbo].[OrganStructUsers]    Script Date: 04/18/2012 19:53:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE [dbo].[OrganStructUsers]
GO

DROP TABLE [dbo].[NodeOrganStructs]
GO

DROP TABLE [dbo].[KKnowledgeOrganStructs]
GO

DROP TABLE [dbo].[DepartmentsGroupsLink]
GO

DROP TABLE [dbo].[OrganStructs]
GO

ALTER TABLE [dbo].[Departments]
DROP COLUMN [ID]
GO

ALTER TABLE [dbo].[Departments]
DROP COLUMN [OldParentID]
GO