USE [EKM_App]
GO

/****** Object:  Table [dbo].[Phrases]    Script Date: 04/26/2013 20:38:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER TABLE [dbo].[CN_Nodes]
DROP COLUMN [Status]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CN_Nodes_CN_Lists]') AND parent_object_id = OBJECT_ID(N'[dbo].[CN_Nodes]'))
ALTER TABLE [dbo].[CN_Nodes] DROP CONSTRAINT [FK_CN_Nodes_CN_Lists]
GO

ALTER TABLE [dbo].[CN_Nodes]
DROP COLUMN [DepartmentGroupID]
GO

ALTER TABLE [dbo].[CN_Nodes]
ADD [Searchable] [bit] NULL
GO