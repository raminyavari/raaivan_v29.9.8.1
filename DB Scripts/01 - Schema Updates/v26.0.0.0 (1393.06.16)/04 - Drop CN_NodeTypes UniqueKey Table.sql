USE [EKM_App]
GO


/****** Object:  Index [UK_NodeTypes_Name]    Script Date: 09/07/2014 14:45:37 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CN_NodeTypes]') AND name = N'UK_NodeTypes_Name')
ALTER TABLE [dbo].[CN_NodeTypes] DROP CONSTRAINT [UK_NodeTypes_Name]
GO