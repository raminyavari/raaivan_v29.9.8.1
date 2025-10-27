USE [EKM_App]
GO


/****** Object:  Table [dbo].[Connections]    Script Date: 02/19/2012 01:03:52 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Connections_ConnectionType]') AND parent_object_id = OBJECT_ID(N'[dbo].[Connections]'))
ALTER TABLE [dbo].[Connections] DROP CONSTRAINT [FK_Connections_ConnectionType]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Connections_Nodes]') AND parent_object_id = OBJECT_ID(N'[dbo].[Connections]'))
ALTER TABLE [dbo].[Connections] DROP CONSTRAINT [FK_Connections_Nodes]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Connections_Nodes1]') AND parent_object_id = OBJECT_ID(N'[dbo].[Connections]'))
ALTER TABLE [dbo].[Connections] DROP CONSTRAINT [FK_Connections_Nodes1]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Connections]') AND type in (N'U'))
DROP TABLE [dbo].[Connections]
GO


/****** Object:  Table [dbo].[ConnectionType]    Script Date: 02/19/2012 01:03:44 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ConnectionType]') AND type in (N'U'))
DROP TABLE [dbo].[ConnectionType]
GO


/****** Object:  Table [dbo].[Nodes]    Script Date: 02/19/2012 01:05:08 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Nodes_aspnet_Users]') AND parent_object_id = OBJECT_ID(N'[dbo].[Nodes]'))
ALTER TABLE [dbo].[Nodes] DROP CONSTRAINT [FK_Nodes_aspnet_Users]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Nodes_DepartmentGroups]') AND parent_object_id = OBJECT_ID(N'[dbo].[Nodes]'))
ALTER TABLE [dbo].[Nodes] DROP CONSTRAINT [FK_Nodes_DepartmentGroups]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Nodes_Nodes]') AND parent_object_id = OBJECT_ID(N'[dbo].[Nodes]'))
ALTER TABLE [dbo].[Nodes] DROP CONSTRAINT [FK_Nodes_Nodes]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Nodes_NodeType]') AND parent_object_id = OBJECT_ID(N'[dbo].[Nodes]'))
ALTER TABLE [dbo].[Nodes] DROP CONSTRAINT [FK_Nodes_NodeType]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Nodes_NodeType1]') AND parent_object_id = OBJECT_ID(N'[dbo].[Nodes]'))
ALTER TABLE [dbo].[Nodes] DROP CONSTRAINT [FK_Nodes_NodeType1]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Nodes]') AND type in (N'U'))
DROP TABLE [dbo].[Nodes]
GO


/****** Object:  Table [dbo].[NodeType]    Script Date: 02/19/2012 01:04:53 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NodeType]') AND type in (N'U'))
DROP TABLE [dbo].[NodeType]
GO


/* Drop unnecessary columns */
ALTER TABLE [dbo].[CN_Nodes]
DROP COLUMN ID
GO


ALTER TABLE [dbo].[CN_Nodes]
DROP COLUMN TypeID
GO


ALTER TABLE [dbo].[CN_NodeTypes]
DROP COLUMN TypeID
GO


ALTER TABLE [dbo].[CN_Properties]
DROP COLUMN ID
GO