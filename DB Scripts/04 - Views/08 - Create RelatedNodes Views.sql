USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(select * FROM sys.views where name = 'CN_View_InRelatedNodes')
DROP VIEW [dbo].[CN_View_InRelatedNodes]
GO

IF EXISTS(select * FROM sys.views where name = 'CN_View_OutRelatedNodes')
DROP VIEW [dbo].[CN_View_OutRelatedNodes]
GO


CREATE VIEW [dbo].[CN_View_InRelatedNodes] WITH SCHEMABINDING, ENCRYPTION
AS
SELECT  NR.ApplicationID,
		NR.DestinationNodeID AS NodeID,
		RN.NodeID AS RelatedNodeID,
		RN.NodeTypeID AS RelatedNodeTypeID,
		NR.PropertyID
FROM [dbo].[CN_NodeRelations] AS NR
	INNER JOIN [dbo].[CN_Nodes] AS RN
	ON RN.ApplicationID = NR.ApplicationID AND RN.NodeID = NR.SourceNodeID
WHERE NR.Deleted = 0 AND RN.Deleted = 0

GO

CREATE VIEW [dbo].[CN_View_OutRelatedNodes] WITH SCHEMABINDING, ENCRYPTION
AS
SELECT  NR.ApplicationID,
		NR.SourceNodeID AS NodeID,
		RN.NodeID AS RelatedNodeID,
		RN.NodeTypeID AS RelatedNodeTypeID,
		NR.PropertyID
FROM [dbo].[CN_NodeRelations] AS NR
	INNER JOIN [dbo].[CN_Nodes] AS RN
	ON RN.ApplicationID = NR.ApplicationID AND RN.NodeID = NR.DestinationNodeID
WHERE NR.Deleted = 0 AND RN.Deleted = 0

GO

CREATE UNIQUE CLUSTERED INDEX PX_CN_View_InRelatedNodes ON [dbo].[CN_View_InRelatedNodes]
(
	[ApplicationID] ASC,
	[NodeID] ASC,
	[RelatedNodeID] ASC,
	[RelatedNodeTypeID] ASC,
	[PropertyID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

CREATE UNIQUE CLUSTERED INDEX PX_CN_View_OutRelatedNodes ON [dbo].[CN_View_OutRelatedNodes]
(
	[ApplicationID] ASC,
	[NodeID] ASC,
	[RelatedNodeID] ASC,
	[RelatedNodeTypeID] ASC,
	[PropertyID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
