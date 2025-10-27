USE [EKM_App]
GO

SET NUMERIC_ROUNDABORT OFF;
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT,
    QUOTED_IDENTIFIER, ANSI_NULLS ON;


IF EXISTS(select * FROM sys.views where name = 'KW_View_ContentFileExtensions')
DROP VIEW [dbo].[KW_View_ContentFileExtensions]
GO

IF EXISTS(select * FROM sys.views where name = 'KW_View_Knowledges')
DROP VIEW [dbo].[KW_View_Knowledges]
GO


CREATE VIEW [dbo].[KW_View_Knowledges] WITH SCHEMABINDING, ENCRYPTION
AS
SELECT  ND.NodeID AS KnowledgeID,
		ND.ApplicationID,
		ND.AdditionalID AS AdditionalID,
		NT.NodeTypeID AS KnowledgeTypeID,
		NT.AdditionalID AS TypeAdditionalID,
		NT.Name AS KnowledgeType,
		ND.AreaID,
		ND.OwnerID AS OwnerID,
		ND.DocumentTreeNodeID AS TreeNodeID,
		ND.PreviousVersionID AS PreviousVersionID,
		ND.Name AS Title,
		ND.CreatorUserID AS CreatorUserID,
		ND.CreationDate AS CreationDate,
		ND.[Status] AS [Status],
		ND.Score AS Score,
		ND.Searchable AS Searchable,
		ND.PublicationDate AS PublicationDate,
		ND.Deleted AS Deleted
FROM    [dbo].[CN_Nodes] AS ND
		INNER JOIN [dbo].[CN_Services] AS S
		INNER JOIN [dbo].[CN_NodeTypes] AS NT
		ON NT.ApplicationID = S.ApplicationID AND NT.NodeTypeID = S.NodeTypeID
		ON NT.ApplicationID = ND.ApplicationID AND NT.NodeTypeID = ND.NodeTypeID
WHERE S.IsKnowledge = 1

GO


CREATE UNIQUE CLUSTERED INDEX PK_KW_View_Knowledges_KnowledgeID ON [dbo].[KW_View_Knowledges]
(
	[ApplicationID] ASC,
	[KnowledgeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO