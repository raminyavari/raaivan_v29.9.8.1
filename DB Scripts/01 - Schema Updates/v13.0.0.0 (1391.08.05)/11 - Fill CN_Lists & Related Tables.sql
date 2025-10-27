USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


DECLARE @ListTypeID uniqueidentifier
SET @ListTypeID = (SELECT ListTypeID FROM [dbo].[CN_ListTypes]
		WHERE AdditionalID = N'100')
		

INSERT INTO [dbo].[CN_Lists](
	ListID,
	ListTypeID,
	Name,
	Description,
	Deleted
)
SELECT ID, @ListTypeID, Name, Description, 0
FROM [dbo].[DepartmentGroups]
GO


INSERT INTO [dbo].[CN_ListNodes](
	ListID,
	NodeID,
	Deleted
)
SELECT GroupID, DepartmentID, 0
FROM [dbo].[DepartmentGroupsLink]
GO


ALTER TABLE [dbo].[CN_Nodes]
DROP CONSTRAINT [FK_CN_Nodes_DepartmentGroups]
GO

ALTER TABLE [dbo].[CN_Nodes]  WITH CHECK ADD  CONSTRAINT [FK_CN_Nodes_CN_Lists] FOREIGN KEY([DepartmentGroupID])
REFERENCES [dbo].[CN_Lists] ([ListID])
GO

ALTER TABLE [dbo].[CN_Nodes] CHECK CONSTRAINT [FK_CN_Nodes_CN_Lists]
GO


ALTER TABLE [dbo].[KnowledgeManagers]
DROP CONSTRAINT [FK_KnowledgeManagers_aspnet_Users]
GO

ALTER TABLE [dbo].[KnowledgeManagers]
DROP CONSTRAINT [FK_KnowledgeManagers_DepartmentGroups]
GO


DROP TABLE [dbo].[DepartmentGroupsLink]
GO

DROP TABLE [dbo].[DepartmentGroups]
GO