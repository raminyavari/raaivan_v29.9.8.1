USE [EKM_App]
GO

/****** Object:  Table [dbo].[Nodes]    Script Date: 02/17/2012 10:53:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER TABLE [dbo].[Nodes]
ADD [NodeID] [uniqueidentifier] NULL;
GO

UPDATE [dbo].[Nodes]
SET NodeID = NEWID()
GO


ALTER TABLE [dbo].[CN_Nodes]
ADD [PNodeId] [bigint] NULL;
GO


INSERT INTO [dbo].[CN_Nodes]
           ([NodeID]
			,[NodeTypeID]
			,[Name]
			,[Description]
			,[CreatorUserID]
			,[CreationDate]
			,[LastModificationDate]
			,[Deleted]
			,[TSkill]
			,[TExperience]
			,[TContent]
			,[Status]
			,[DepartmentGroupID]
			,[ID]
			,[TypeID]
			,[PNodeId])
SELECT      dbo.Nodes.NodeID, dbo.CN_NodeTypes.NodeTypeID, dbo.Nodes.Name, dbo.Nodes.Description, dbo.Nodes.UserId, 
			dbo.Nodes.DateCreated, dbo.Nodes.LastModified, 0, dbo.Nodes.TSkill, dbo.Nodes.TExperience, dbo.Nodes.TContent, 
			dbo.Nodes.Status, dbo.Nodes.DepartmentGroupID, dbo.Nodes.ID, dbo.CN_NodeTypes.TypeID, dbo.Nodes.ParentNodeId
FROM        dbo.Nodes INNER JOIN dbo.CN_NodeTypes ON dbo.Nodes.TypeID = dbo.CN_NodeTypes.TypeID
GO


DELETE FROM [dbo].[CN_Nodes]
      WHERE PNodeId > 0
GO


INSERT INTO [dbo].[CN_Nodes]
           ([NodeID]
			,[NodeTypeID]
			,[Name]
			,[Description]
			,[CreatorUserID]
			,[CreationDate]
			,[LastModificationDate]
			,[Deleted]
			,[TSkill]
			,[TExperience]
			,[TContent]
			,[Status]
			,[DepartmentGroupID]
			,[ParentNodeID]
			,[ID]
			,[TypeID])
SELECT      dbo.Nodes.NodeID, dbo.CN_NodeTypes.NodeTypeID, dbo.Nodes.Name, dbo.Nodes.Description, dbo.Nodes.UserId, 
			dbo.Nodes.DateCreated, dbo.Nodes.LastModified, 0, dbo.Nodes.TSkill, dbo.Nodes.TExperience, dbo.Nodes.TContent, 
			dbo.Nodes.Status, dbo.Nodes.DepartmentGroupID, Nodes_1.NodeID AS Expr1, dbo.Nodes.ID, dbo.CN_NodeTypes.TypeID
FROM        dbo.Nodes INNER JOIN dbo.Nodes AS Nodes_1 ON dbo.Nodes.ParentNodeId = Nodes_1.ID INNER JOIN
            dbo.CN_NodeTypes ON dbo.Nodes.TypeID = dbo.CN_NodeTypes.TypeID
GO


ALTER TABLE [dbo].[Nodes]
DROP COLUMN [NodeID]
GO


ALTER TABLE [dbo].[CN_Nodes]
DROP COLUMN [PNodeId]
GO