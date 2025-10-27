USE EKM_App
GO


DECLARE @parent uniqueidentifier = [dbo].[CN_FN_GetParentRelationTypeID](),
	@child uniqueidentifier = [dbo].[CN_FN_GetChildRelationTypeID]()
DECLARE @relations Table(sourceid uniqueidentifier, destid uniqueidentifier)
	
INSERT INTO @relations(sourceid, destid)
SELECT DISTINCT * FROM
(
	SELECT ref.SourceNodeID AS SourceNodeID, ref.DestinationNodeID AS DestinationNodeID
	FROM [dbo].[CN_NodeRelations] AS ref
		INNER JOIN [dbo].[CN_Nodes] AS refsnd
		ON ref.SourceNodeID = refsnd.NodeID
		INNER JOIN [dbo].[CN_Nodes] AS refdnd
		ON ref.DestinationNodeID = refdnd.NodeID
	WHERE ref.Deleted = 0 AND ref.PropertyID = @child AND
		refsnd.NodeTypeID = refdnd.NodeTypeID AND refsnd.Deleted = 0 AND refdnd.Deleted = 0 AND
		NOT EXISTS(SELECT TOP(1) * 
			FROM [dbo].[CN_NodeRelations] AS nr
				INNER JOIN [dbo].[CN_Nodes] AS snd
				ON nr.SourceNodeID = snd.NodeID
				INNER JOIN [dbo].[CN_Nodes] AS dnd
				ON nr.DestinationNodeID = dnd.NodeID
			WHERE nr.Deleted = 0 AND snd.Deleted = 0 AND dnd.Deleted = 0 AND
				snd.NodeTypeID = dnd.NodeTypeID AND
				(nr.SourceNodeID = ref.SourceNodeID AND nr.PropertyID = @child AND
				nr.DestinationNodeID <> ref.DestinationNodeID) OR
				(nr.DestinationNodeID = ref.SourceNodeID AND nr.PropertyID = @parent AND
				nr.SourceNodeID <> ref.DestinationNodeID)
		)

	UNION ALL

	SELECT ref.DestinationNodeID AS SourceNodeID, ref.SourceNodeID AS DestinationNodeID
	FROM [dbo].[CN_NodeRelations] AS ref
		INNER JOIN [dbo].[CN_Nodes] AS refsnd
		ON ref.SourceNodeID = refsnd.NodeID
		INNER JOIN [dbo].[CN_Nodes] AS refdnd
		ON ref.DestinationNodeID = refdnd.NodeID
	WHERE ref.Deleted = 0 AND ref.PropertyID = @parent AND
		refsnd.NodeTypeID = refdnd.NodeTypeID AND refsnd.Deleted = 0 AND refdnd.Deleted = 0 AND
		NOT EXISTS(SELECT TOP(1) * 
			FROM [dbo].[CN_NodeRelations] AS nr
				INNER JOIN [dbo].[CN_Nodes] AS snd
				ON nr.SourceNodeID = snd.NodeID
				INNER JOIN [dbo].[CN_Nodes] AS dnd
				ON nr.DestinationNodeID = dnd.NodeID
			WHERE nr.Deleted = 0 AND snd.Deleted = 0 AND dnd.Deleted = 0 AND
				snd.NodeTypeID = dnd.NodeTypeID AND
				(nr.SourceNodeID = ref.DestinationNodeID AND nr.PropertyID = @child AND
				nr.DestinationNodeID <> ref.SourceNodeID) OR
				(nr.DestinationNodeID = ref.DestinationNodeID AND nr.PropertyID = @parent AND
				nr.SourceNodeID <> ref.SourceNodeID)
		)
) AS a


UPDATE [dbo].[CN_Nodes]
	SET ParentNodeID = ref.destid
FROM @relations AS ref
	INNER JOIN [dbo].[CN_Nodes]
	ON ref.sourceid = [dbo].[CN_Nodes].[NodeID]


UPDATE [dbo].[CN_NodeRelations]
	SET Deleted = 1,
		LastModificationDate = GETDATE()
FROM @relations AS ref
	INNER JOIN [dbo].[CN_NodeRelations]
	ON ([dbo].[CN_NodeRelations].[SourceNodeID] = ref.sourceid AND
	   [dbo].[CN_NodeRelations].[DestinationNodeID] = ref.destid AND
	   [dbo].[CN_NodeRelations].[PropertyID] = @child) OR
	   ([dbo].[CN_NodeRelations].[SourceNodeID] = ref.destid AND
	   [dbo].[CN_NodeRelations].[DestinationNodeID] = ref.sourceid AND
	   [dbo].[CN_NodeRelations].[PropertyID] = @parent)

GO