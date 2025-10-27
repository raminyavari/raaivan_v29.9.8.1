USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/* Create temp table */
CREATE TABLE [dbo].[ConnectionsTemp](
	[ID] [bigint] NOT NULL,
	[ConnectionTypeID] [bigint] NULL,
	[TypeID] [uniqueidentifier] NOT NULL,
	[Weight] [float] NULL,
	[PNodeID] [uniqueidentifier] NOT NULL,
	[CNodeID] [uniqueidentifier] NOT NULL,
	[WDescription] [nvarchar](max) NULL,
	[WhyConnection] [nvarchar](max) NULL,
	[UserSetWeight] [nvarchar](max) NULL,
 CONSTRAINT [PK_ConnectionsTemp] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[ConnectionsTemp]
           ([ID]
			,[ConnectionTypeID]
			,[Weight]
			,[WDescription]
			,[WhyConnection]
			,[UserSetWeight]
			,[PNodeID]
			,[CNodeID]
			,[TypeID])
SELECT  dbo.Connections.ID, dbo.Connections.ConnectionTypeID, 
        dbo.Connections.Weight, dbo.Connections.WDescription, dbo.Connections.WhyConnection, dbo.Connections.UserSetWeight,
        dbo.CN_Nodes.NodeID, CN_Nodes_1.NodeID AS Expr1, dbo.CN_Properties.PropertyID
FROM    dbo.CN_Nodes INNER JOIN
        dbo.Connections ON dbo.CN_Nodes.ID = dbo.Connections.PNodeID INNER JOIN
        dbo.CN_Nodes AS CN_Nodes_1 ON dbo.Connections.CNodeID = CN_Nodes_1.ID INNER JOIN
        dbo.CN_Properties ON dbo.Connections.ConnectionTypeID = dbo.CN_Properties.ID
GO
/* end of temp table */

/* Fill CN_NodeRelations */
INSERT INTO [dbo].[CN_NodeRelations]
           ([SourceNodeID]
			,[DestinationNodeID]
			,[PropertyID]
			,[NominalValue])
SELECT  dbo.ConnectionsTemp.PNodeID, dbo.ConnectionsTemp.CNodeID, dbo.ConnectionsTemp.TypeID,
		dbo.ConnectionsTemp.Weight
FROM    dbo.ConnectionsTemp
WHERE   dbo.ConnectionsTemp.ConnectionTypeID = 1
	 
GO


INSERT INTO [dbo].[CN_NodeRelations]
           ([SourceNodeID]
			,[DestinationNodeID]
			,[PropertyID]
			,[NominalValue])
SELECT  dbo.ConnectionsTemp.CNodeID, dbo.ConnectionsTemp.PNodeID, dbo.CN_Properties.PropertyID,
		dbo.ConnectionsTemp.Weight
FROM    dbo.ConnectionsTemp INNER JOIN dbo.CN_Properties ON dbo.CN_Properties.ID = 2
WHERE   dbo.ConnectionsTemp.ConnectionTypeID = 1
	 
GO


INSERT INTO [dbo].[CN_NodeRelations]
           ([SourceNodeID]
			,[DestinationNodeID]
			,[PropertyID]
			,[NominalValue])
SELECT  dbo.ConnectionsTemp.CNodeID, dbo.ConnectionsTemp.PNodeID, dbo.ConnectionsTemp.TypeID,
		dbo.ConnectionsTemp.Weight
FROM    dbo.ConnectionsTemp
WHERE   dbo.ConnectionsTemp.ConnectionTypeID = 2
	 
GO


INSERT INTO [dbo].[CN_NodeRelations]
           ([SourceNodeID]
			,[DestinationNodeID]
			,[PropertyID]
			,[NominalValue])
SELECT  dbo.ConnectionsTemp.PNodeID, dbo.ConnectionsTemp.CNodeID, dbo.CN_Properties.PropertyID,
		dbo.ConnectionsTemp.Weight
FROM    dbo.ConnectionsTemp INNER JOIN dbo.CN_Properties ON dbo.CN_Properties.ID = 1
WHERE   dbo.ConnectionsTemp.ConnectionTypeID = 2
	 
GO


INSERT INTO [dbo].[CN_NodeRelations]
           ([SourceNodeID]
			,[DestinationNodeID]
			,[PropertyID]
			,[NominalValue])
SELECT  dbo.ConnectionsTemp.PNodeID, dbo.ConnectionsTemp.CNodeID, dbo.ConnectionsTemp.TypeID,
		dbo.ConnectionsTemp.Weight
FROM    dbo.ConnectionsTemp
WHERE   dbo.ConnectionsTemp.ConnectionTypeID = 3
	 
GO


INSERT INTO [dbo].[CN_NodeRelations]
           ([SourceNodeID]
			,[DestinationNodeID]
			,[PropertyID]
			,[NominalValue])
SELECT  dbo.ConnectionsTemp.CNodeID, dbo.ConnectionsTemp.PNodeID, dbo.ConnectionsTemp.TypeID,
		dbo.ConnectionsTemp.Weight
FROM    dbo.ConnectionsTemp
WHERE   dbo.ConnectionsTemp.ConnectionTypeID = 3
	 
GO
/* end of 'Fill CN_NodeRelations' */


/* Drop temp table */
DROP TABLE [dbo].[ConnectionsTemp]
GO