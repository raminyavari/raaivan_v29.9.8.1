USE [EKM_App]
GO

/****** Object:  Table [dbo].[Nodes]    Script Date: 10/16/2011 13:35:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER TABLE [dbo].[Nodes]
ADD [DepartmentGroupID] [uniqueidentifier] NULL;
GO

ALTER TABLE [dbo].[Nodes]  WITH CHECK ADD  CONSTRAINT [FK_Nodes_DepartmentGroups] FOREIGN KEY([DepartmentGroupID])
REFERENCES [dbo].[DepartmentGroups] ([ID])
GO

ALTER TABLE [dbo].[Nodes] CHECK CONSTRAINT [FK_Nodes_DepartmentGroups]
GO

ALTER TABLE [dbo].[Nodes] DROP CONSTRAINT [FK_Nodes_OrganStructs]
GO

ALTER TABLE [dbo].[Nodes]
DROP COLUMN [OwnerOganStructId]
GO