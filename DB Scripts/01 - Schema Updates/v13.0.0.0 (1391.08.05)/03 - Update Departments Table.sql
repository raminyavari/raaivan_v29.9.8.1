USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


INSERT INTO [dbo].[CN_Nodes]
           ([NodeID]
		   ,[NodeTypeID]
           ,[Name]
           ,[ParentNodeID]
           ,[Deleted])
SELECT  DP.[DepartmentID], [dbo].[CN_NodeTypes].[NodeTypeID],
		DP.[Title], DP.[ParentID], 0
FROM    [dbo].[Departments] AS DP INNER JOIN [dbo].[CN_NodeTypes] ON
		[dbo].[CN_NodeTypes].[AdditionalID] = '6'
GO


ALTER TABLE [dbo].[DepartmentGroupsLink]
DROP CONSTRAINT [FK_DepartmentGroupsLink_Departments]
GO

ALTER TABLE [dbo].[DepartmentGroupsLink]  WITH CHECK ADD  CONSTRAINT [FK_DepartmentGroupsLink_CN_Nodes] FOREIGN KEY([DepartmentID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[DepartmentGroupsLink] CHECK CONSTRAINT [FK_DepartmentGroupsLink_CN_Nodes]
GO


ALTER TABLE [dbo].[DepartmentUsers]
DROP CONSTRAINT [FK_DepartmentUsers_Departments]
GO

ALTER TABLE [dbo].[DepartmentUsers]  WITH NOCHECK ADD  CONSTRAINT [FK_DepartmentUsers_CN_Nodes] FOREIGN KEY([DepartmentID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[DepartmentUsers] CHECK CONSTRAINT [FK_DepartmentUsers_CN_Nodes]
GO


-- Update KW_RelatedDepartments
UPDATE [dbo].[KW_RelatedDepartments]
	SET Deleted = 0
WHERE Deleted IS NULL

DECLARE @_KRD Table(firstV uniqueidentifier, secondV uniqueidentifier, del bit)
INSERT INTO @_KRD(firstV, secondV, del)
SELECT DISTINCT Ref.KnowledgeID, Ref.DepartmentID, Ref.Deleted
FROM [dbo].[KW_RelatedDepartments] AS Ref


DECLARE @_KRDRTID uniqueidentifier
SET @_KRDRTID = (SELECT PropertyID FROM [dbo].[CN_Properties]
	WHERE AdditionalID = '3')
		
INSERT INTO [dbo].[CN_NodeRelations](SourceNodeID, DestinationNodeID, PropertyID, Deleted)
SELECT Ref.firstV, Ref.secondV, @_KRDRTID, Ref.del
FROM @_KRD AS Ref

INSERT INTO [dbo].[CN_NodeRelations](SourceNodeID, DestinationNodeID, PropertyID, Deleted)
SELECT Ref.secondV, Ref.firstV, @_KRDRTID, Ref.del
FROM @_KRD AS Ref


DROP TABLE [dbo].[KW_RelatedDepartments]
GO
-- end of Update KW_RelatedDepartments


-- Update CNDP_NodeDepartments
DECLARE @_KRN Table(firstV uniqueidentifier, secondV uniqueidentifier)
INSERT INTO @_KRN(firstV, secondV)
SELECT DISTINCT Ref.NodeID, Ref.DepartmentID
FROM [dbo].[CNDP_NodeDepartments] AS Ref


DECLARE @_KRNRTID uniqueidentifier
SET @_KRNRTID = (SELECT PropertyID FROM [dbo].[CN_Properties]
	WHERE AdditionalID = '3')
		
INSERT INTO [dbo].[CN_NodeRelations](SourceNodeID, DestinationNodeID, PropertyID, Deleted)
SELECT Ref.firstV, Ref.secondV, @_KRNRTID, 0
FROM @_KRN AS Ref

INSERT INTO [dbo].[CN_NodeRelations](SourceNodeID, DestinationNodeID, PropertyID, Deleted)
SELECT Ref.secondV, Ref.firstV, @_KRNRTID, 0
FROM @_KRN AS Ref


DROP TABLE [dbo].[CNDP_NodeDepartments]
GO
-- end of Update CNDP_NodeDepartments


DROP TABLE [dbo].[Departments]
GO


-- Update DepartmentUsers
INSERT INTO [dbo].[CN_NodeMembers](
	NodeID,
	UserID,
	MembershipDate,
	IsAdmin,
	Status,
	AcceptionDate,
	Deleted
)
SELECT DepartmentID, UserID, '2011-10-08 18:42:16.003', 0,	N'Accepted', 
	'2011-10-08 18:42:16.003', 0
FROM [dbo].[DepartmentUsers]
WHERE UserType IS NULL OR UserType <> N'DepartmentManager'

INSERT INTO [dbo].[CN_NodeMembers](
	NodeID,
	UserID,
	MembershipDate,
	IsAdmin,
	Status,
	AcceptionDate,
	Deleted
)
SELECT DepartmentID, UserID, '2011-10-08 18:42:16.003', 1,	N'Accepted', 
	'2011-10-08 18:42:16.003', 0
FROM [dbo].[DepartmentUsers]
WHERE UserType = N'DepartmentManager'

DROP TABLE [dbo].[DepartmentUsers]
GO
-- end of Update DepartmentUsers