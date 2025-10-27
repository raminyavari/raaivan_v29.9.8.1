USE EKM_App
GO

DECLARE @NodeIDs Table(ID int IDENTITY(1,1), NodeID uniqueidentifier)

INSERT INTO @NodeIDs (NodeID)
SELECT NodeID
FROM [dbo].[CN_Nodes]

DECLARE @Count int = (SELECT COUNT(*) FROM @NodeIDs)

WHILE @Count > 0 BEGIN
	DECLARE @NID uniqueidentifier = (SELECT NodeID FROM @NodeIDs WHERE ID = @Count)
	DECLARE @HIR Table(NodeID uniqueidentifier, 
		ParentNodeID uniqueidentifier, [Level] int primary key)
	
	INSERT INTO @HIR
	EXEC [dbo].[CN_P_GetNodeHirarchy] @NID, NULL
	
	IF EXISTS(
		SELECT *
		FROM (SELECT TOP(1)* FROM @HIR ORDER BY [Level] DESC) AS Ref
		WHERE ParentNodeID IS NOT NULL
	) BEGIN
		DECLARE @RootID uniqueidentifier  = 
			(SELECT Ref.NodeID
			FROM (SELECT TOP(1) * FROM @HIR ORDER BY [Level] DESC) AS Ref
			WHERE Ref.ParentNodeID IS NOT NULL)
		
		UPDATE [dbo].[CN_Nodes]
			SET ParentNodeID = NULL
		WHERE NodeID = @RootID
	END
	
	DELETE @HIR
	
	SET @Count = @Count - 1
END

GO