USE [EKM_App]
GO

ALTER TABLE [dbo].[CN_NodeTypes]
ADD [ParentID] uniqueidentifier NULL,
	[FormID] uniqueidentifier NULL
GO