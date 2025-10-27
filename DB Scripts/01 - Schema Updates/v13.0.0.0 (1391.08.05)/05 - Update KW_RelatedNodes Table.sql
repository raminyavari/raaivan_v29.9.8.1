USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- Update RelatedNodes
UPDATE [dbo].[KW_RelatedNodes]
	SET Deleted = 0
WHERE Deleted IS NULL

INSERT INTO [dbo].[CN_NodeRelations](
	SourceNodeID,
	DestinationNodeID,
	PropertyID,
	CreatorUserID,
	CreationDate,
	Deleted
)
SELECT DISTINCT Ref.KnowledgeID, Ref.NodeID, PR.PropertyID, 
	CN.CreatorUserID, CN.CreationDate, Ref.Deleted
FROM [dbo].[KW_RelatedNodes] AS Ref
	INNER JOIN [dbo].[CN_Nodes] AS CN
	ON Ref.NodeID = CN.NodeID
	INNER JOIN [dbo].[CN_NodeTypes] AS CT
	ON CN.NodeTypeID = CT.NodeTypeID
	INNER JOIN [dbo].[CN_Properties] AS PR
	ON PR.AdditionalID = N'3'
WHERE CT.AdditionalID <> N'1'


DELETE FROM [dbo].[KW_RelatedNodes]
FROM [dbo].[KW_RelatedNodes]
	INNER JOIN [dbo].[CN_Nodes] AS CN
	ON [dbo].[KW_RelatedNodes].[NodeID] = CN.NodeID
	INNER JOIN [dbo].[CN_NodeTypes] AS CT
	ON CN.NodeTypeID = CT.NodeTypeID
	INNER JOIN [dbo].[CN_Properties] AS PR
	ON PR.AdditionalID = N'3'
WHERE CT.AdditionalID <> N'1'
-- end of Update RelatedNodes
