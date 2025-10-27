USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(select * FROM sys.views where name = 'WF_View_CurrentStates')
DROP VIEW [dbo].[WF_View_CurrentStates]
GO

IF EXISTS(select * FROM sys.views where name = 'CN_View_Nodes_Normal')
DROP VIEW [dbo].[CN_View_Nodes_Normal]
GO


CREATE VIEW [dbo].[CN_View_Nodes_Normal] WITH SCHEMABINDING, ENCRYPTION
AS
SELECT  ND.NodeID, 
		ND.ApplicationID,
		ND.Name AS NodeName, 
		ND.[Description],
		ND.AdditionalID_Main AS NodeAdditionalID_Main, 
		ND.AdditionalID AS NodeAdditionalID, 
		ND.NodeTypeID,
		NT.Name AS TypeName, 
		NT.AdditionalID AS TypeAdditionalID, 
        ND.ParentNodeID,
        ND.Deleted, 
        ISNULL(ND.Searchable, 1) AS Searchable,
        ISNULL(ND.HideCreators, 0) AS HideCreators,
        NT.Deleted AS TypeDeleted,
        ND.Tags AS Tags, 
        ND.CreationDate AS CreationDate,
        ND.PublicationDate,
        ND.CreatorUserID AS CreatorUserID,
        ND.OwnerID AS OwnerID,
        ND.AreaID,
        ND.DocumentTreeNodeID,
        ND.Score,
        ND.[Status],
        ND.[WFState],
        ND.SequenceNumber,
        ND.IndexLastUpdateDate
FROM [dbo].[CN_Nodes] AS ND
	INNER JOIN [dbo].[CN_NodeTypes] AS NT
	ON NT.ApplicationID = ND.ApplicationID AND ND.NodeTypeID = NT.NodeTypeID

GO

SET ANSI_PADDING ON
GO

CREATE UNIQUE CLUSTERED INDEX PX_CN_View_Nodes_Normal ON [dbo].[CN_View_Nodes_Normal]
(
	[ApplicationID] ASC,
	[NodeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
