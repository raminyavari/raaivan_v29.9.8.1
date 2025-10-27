USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF  EXISTS (SELECT * FROM sys.objects 
            WHERE object_id = OBJECT_ID(N'[dbo].[DCT_FN_GetChildNodesHierarchy]') 
            AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[DCT_FN_GetChildNodesHierarchy]
GO

CREATE FUNCTION [dbo].[DCT_FN_GetChildNodesHierarchy](
	@ApplicationID		uniqueidentifier,
	@NodeIDsOrTreeIDs	GuidTableType readonly,
	@Archive			bit
)	
RETURNS @OutputTable TABLE (
	NodeID uniqueidentifier primary key clustered,
	ParentID uniqueidentifier,
	[Level] int,
	Name nvarchar(2000),
	SequenceNumber int
)
WITH ENCRYPTION
AS
BEGIN
	INSERT INTO @OutputTable(NodeID, ParentID, [Level], Name, SequenceNumber)
	SELECT DISTINCT 
		X.TreeNodeID AS ID,
		X.ParentNodeID AS ParentID,
		0 AS [Level],
		X.Name,
		X.SequenceNumber
	FROM (
			SELECT	TN.*
			FROM @NodeIDsOrTreeIDs AS N
				INNER JOIN [dbo].[DCT_TreeNodes] AS TN
				ON TN.ApplicationID = @ApplicationID AND TN.TreeNodeID = N.Value
				
			UNION ALL
			
			SELECT TN.*
			FROM @NodeIDsOrTreeIDs AS N
				INNER JOIN [dbo].[DCT_Trees] AS T
				ON T.ApplicationID = @ApplicationID AND T.TreeID = N.Value
				INNER JOIN [dbo].[DCT_TreeNodes] AS TN
				ON TN.ApplicationID = @ApplicationID AND 
					TN.TreeID = T.TreeID AND TN.ParentNodeID IS NULL AND 
					(@Archive IS NULL OR TN.Deleted = @Archive)
		) AS X
	
	INSERT INTO @OutputTable(NodeID, ParentID, [Level], Name, SequenceNumber)
	SELECT TN.TreeNodeID AS ID, TN.ParentNodeID AS ParentID, 
		[Level] + 1, TN.Name AS Name, TN.SequenceNumber
	FROM [dbo].[DCT_TreeNodes] AS TN
		INNER JOIN @OutputTable AS HR
		ON HR.NodeID = TN.ParentNodeID
	WHERE TN.ApplicationID = @ApplicationID AND TN.TreeNodeID <> HR.NodeID AND 
		(@Archive IS NULL OR TN.Deleted = @Archive)
	
	RETURN
END

GO


IF  EXISTS (SELECT * FROM sys.objects 
            WHERE object_id = OBJECT_ID(N'[dbo].[DCT_FN_GetChildNodesDeepHierarchy]') 
            AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[DCT_FN_GetChildNodesDeepHierarchy]
GO

CREATE FUNCTION [dbo].[DCT_FN_GetChildNodesDeepHierarchy](
	@ApplicationID	uniqueidentifier,
	@NodeIDs		GuidTableType readonly
)	
RETURNS @OutputTable TABLE (
	NodeID uniqueidentifier primary key clustered,
	ParentID uniqueidentifier,
	[Level] int,
	Name nvarchar(2000),
	SequenceNumber int
)
WITH ENCRYPTION
AS
BEGIN
	;WITH hierarchy (ID, ParentID, [Level], Name)
	AS
	(
		SELECT TN.TreeNodeID AS ID, TN.ParentNodeID AS ParentID, 0 AS [Level], TN.Name AS Name
		FROM @NodeIDs AS N
			INNER JOIN [dbo].[DCT_TreeNodes] AS TN
			ON TN.ApplicationID = @ApplicationID AND TN.TreeNodeID = N.Value
		
		UNION ALL
		
		SELECT Node.TreeNodeID AS ID, Node.ParentNodeID AS ParentID, [Level] + 1, Node.Name
		FROM [dbo].[DCT_TreeNodes] AS Node
			INNER JOIN hierarchy AS HR
			ON HR.ID = Node.ParentNodeID
		WHERE Node.ApplicationID = @ApplicationID AND 
			Node.TreeNodeID <> HR.ID AND Node.Deleted = 0
	)
	
	INSERT INTO @OutputTable (NodeID, ParentID, [Level], Name)
	SELECT H.ID, H.ParentID, H.[Level], H.Name
	FROM hierarchy AS H
	ORDER BY H.[Level] ASC
	
	RETURN
END

GO


IF  EXISTS (SELECT * FROM sys.objects 
            WHERE object_id = OBJECT_ID(N'[dbo].[DCT_FN_GetParentNodesHierarchy]') 
            AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[DCT_FN_GetParentNodesHierarchy]
GO

CREATE FUNCTION [dbo].[DCT_FN_GetParentNodesHierarchy](
	@ApplicationID	uniqueidentifier,
	@NodeIDs		GuidTableType readonly
)	
RETURNS @OutputTable TABLE (
	NodeID uniqueidentifier primary key clustered,
	ParentID uniqueidentifier,
	[Level] int,
	Name nvarchar(2000)
)
WITH ENCRYPTION
AS
BEGIN
	INSERT INTO @OutputTable(NodeID, ParentID, [Level], Name)
	SELECT	TN.TreeNodeID AS ID, TN.ParentNodeID AS ParentID, 0 AS [Level], TN.Name
	FROM @NodeIDs AS N
		INNER JOIN [dbo].[DCT_TreeNodes] AS TN
		ON TN.ApplicationID = @ApplicationID AND TN.TreeNodeID = N.Value
	
	INSERT INTO @OutputTable(NodeID, ParentID, [Level], Name)
	SELECT TN.TreeNodeID AS ID, TN.ParentNodeID AS ParentID, [Level] + 1, TN.Name AS Name
	FROM [dbo].[DCT_TreeNodes] AS TN
		INNER JOIN @OutputTable AS HR
		ON HR.ParentID = TN.TreeNodeID
	WHERE TN.ApplicationID = @ApplicationID AND TN.TreeNodeID <> HR.NodeID
	
	RETURN
END

GO


IF  EXISTS (SELECT * FROM sys.objects 
            WHERE object_id = OBJECT_ID(N'[dbo].[DCT_FN_HasFile]') 
            AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[DCT_FN_HasFile]
GO

CREATE FUNCTION [dbo].[DCT_FN_HasFile](
	@ApplicationID	uniqueidentifier,
	@OwnerID		uniqueidentifier
)	
RETURNS bit
WITH ENCRYPTION
AS
BEGIN
	RETURN CAST(ISNULL((
		SELECT 1
		FROM [dbo].[DCT_Files] AS F
		WHERE F.ApplicationID = @ApplicationID AND F.OwnerID = @OwnerID AND F.Deleted = 0
	), 0) AS bit)
END

GO


IF  EXISTS (SELECT * FROM sys.objects 
            WHERE object_id = OBJECT_ID(N'[dbo].[DCT_FN_FilesCount]') 
            AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[DCT_FN_FilesCount]
GO

CREATE FUNCTION [dbo].[DCT_FN_FilesCount](
	@ApplicationID	uniqueidentifier,
	@OwnerID		uniqueidentifier
)	
RETURNS int
WITH ENCRYPTION
AS
BEGIN
	RETURN CAST(ISNULL((
		SELECT COUNT(F.ID)
		FROM [dbo].[DCT_Files] AS F
		WHERE F.ApplicationID = @ApplicationID AND F.OwnerID = @OwnerID AND F.Deleted = 0
	), 0) AS int)
END

GO


IF  EXISTS (SELECT * FROM sys.objects 
            WHERE object_id = OBJECT_ID(N'[dbo].[DCT_FN_GetFileOwnerNodes]') 
            AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[DCT_FN_GetFileOwnerNodes]
GO

CREATE FUNCTION [dbo].[DCT_FN_GetFileOwnerNodes](
	@ApplicationID	uniqueidentifier,
	@FileIDs		GuidTableType readonly
)	
RETURNS @OutputTable TABLE (
	FileID uniqueidentifier,
	NodeID uniqueidentifier,
	NodeTypeID uniqueidentifier,
	NodeName nvarchar(500),
	NodeType nvarchar(500),
	[FileName] nvarchar(500),
	Extension nvarchar(20)
)
WITH ENCRYPTION
AS
BEGIN
	DECLARE @Files TABLE (ID uniqueidentifier, OwnerID uniqueidentifier,
		OwnerType varchar(20), [FileName] nvarchar(500), Extension nvarchar(20))

	INSERT INTO @Files (ID, OwnerID, OwnerType, [FileName], Extension)
	SELECT ID.Value, F.OwnerID, F.OwnerType, F.[FileName], F.Extension
	FROM @FileIDs AS ID
		INNER JOIN [dbo].[DCT_Files] AS F
		ON F.ApplicationID = @ApplicationID AND 
			(F.ID = ID.Value OR F.FileNameGuid = ID.Value) AND F.Deleted = 0
		
	INSERT INTO @OutputTable (FileID, NodeID, NodeTypeID, 
		NodeName, NodeType, [FileName], Extension)
	(
		SELECT	F.ID AS FileID, 
				ND.NodeID, 
				ND.NodeTypeID,
				ND.NodeName AS Name,
				ND.TypeName AS NodeType,
				F.[FileName],
				F.Extension
		FROM @Files AS F
			INNER JOIN [dbo].[CN_View_Nodes_Normal] AS ND
			ON ND.ApplicationID = @ApplicationID AND ND.NodeID = F.OwnerID AND ND.Deleted = 0
		WHERE F.OwnerType = N'Node' OR F.OwnerType = N'WikiContent'

		UNION ALL

		SELECT	F.ID AS FileID, 
				ND.NodeID,
				ND.NodeTypeID,
				ND.NodeName AS Name,
				ND.TypeName AS NodeType,
				F.[FileName],
				F.Extension
		FROM @Files AS F
			INNER JOIN [dbo].[FG_InstanceElements] AS E
			ON E.ApplicationID = @ApplicationID AND E.ElementID = F.OwnerID AND 
				E.[Type] = N'File' AND E.Deleted = 0
			INNER JOIN [dbo].[FG_FormInstances] AS I
			ON I.ApplicationID = @ApplicationID AND I.InstanceID = E.InstanceID AND I.Deleted = 0
			INNER JOIN [dbo].[CN_View_Nodes_Normal] AS ND
			ON ND.ApplicationID = @ApplicationID AND ND.NodeID = I.OwnerID AND ND.Deleted = 0
		WHERE F.OwnerType = N'FormElement'
	)
	
	RETURN
END

GO


IF  EXISTS (SELECT * FROM sys.objects 
            WHERE object_id = OBJECT_ID(N'[dbo].[DCT_FN_ListDeepAttachments]') 
            AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[DCT_FN_ListDeepAttachments]
GO

CREATE FUNCTION [dbo].[DCT_FN_ListDeepAttachments](
	@ApplicationIDs	GuidTableType readonly,
	@NodeIDs		GuidTableType readonly,
	@Archive		bit
)	
RETURNS @OutputTable TABLE (
	ApplicationID	uniqueidentifier,
	FileID			uniqueidentifier,
	CreationDate	datetime,
	CreatorUserID	uniqueidentifier,
	NodeID			uniqueidentifier,
	NodeTypeID		uniqueidentifier,
	NodeName		nvarchar(500),
	NodeType		nvarchar(500),
	[FileName]		nvarchar(500),
	Extension		nvarchar(20),
	Size			bigint,
	OwnerType		varchar(100),
	Deleted			bit
)
WITH ENCRYPTION
AS
BEGIN
	DECLARE @AppsCount int = (SELECT COUNT(*) FROM @ApplicationIDs AS A)
	DECLARE @NodesCount int = (SELECT COUNT(*) FROM @NodeIDs AS N)

	;WITH Files
	AS
	(
		SELECT F.ApplicationID, F.ID, F.OwnerID, F.OwnerType, F.[FileName], F.Extension, F.Size, F.Deleted, F.CreatorUserID, F.CreationDate
		FROM [dbo].[DCT_Files] AS F
		WHERE (@AppsCount = 0 OR F.ApplicationID IN (SELECT App.[Value] FROM @ApplicationIDs AS App)) AND
			(@Archive IS NULL OR F.Deleted = @Archive)
	)
	
	INSERT INTO @OutputTable (ApplicationID, FileID, CreationDate, CreatorUserID, NodeID, NodeTypeID, 
		NodeName, NodeType, [FileName], Extension, Size, OwnerType, Deleted)
	(
		SELECT	F.ApplicationID,
				F.ID AS FileID, 
				F.CreationDate,
				F.CreatorUserID,
				ND.NodeID, 
				ND.NodeTypeID,
				ND.NodeName AS [Name],
				ND.TypeName AS NodeType,
				F.[FileName],
				F.Extension,
				F.Size,
				F.OwnerType,
				CAST((CASE WHEN ND.Deleted = 1 OR F.Deleted = 1 THEN 1 ELSE 0 END) AS bit) AS Deleted
		FROM Files AS F
			LEFT JOIN [dbo].[CN_View_Nodes_Normal] AS ND
			ON ND.ApplicationID = F.ApplicationID AND ND.NodeID = F.OwnerID AND 
				(@Archive IS NULL OR ND.Deleted = @Archive) AND
				(@NodesCount = 0 OR ND.NodeID IN (SELECT N.[Value] FROM @NodeIDs AS N))
		WHERE F.OwnerType = N'Node' OR F.OwnerType = N'WikiContent'

		UNION ALL

		SELECT	F.ApplicationID,
				F.ID AS FileID, 
				F.CreationDate,
				F.CreatorUserID,
				ND.NodeID,
				ND.NodeTypeID,
				ND.NodeName AS Name,
				ND.TypeName AS NodeType,
				F.[FileName],
				F.Extension,
				F.Size,
				F.OwnerType,
				CAST((CASE WHEN E.Deleted = 1 OR I.Deleted = 1 OR ND.Deleted = 1 OR F.Deleted = 1 THEN 1 ELSE 0 END) AS bit) AS Deleted
		FROM Files AS F
			INNER JOIN [dbo].[FG_InstanceElements] AS E
			ON E.ApplicationID = F.ApplicationID AND E.ElementID = F.OwnerID AND 
				E.[Type] = N'File' AND (@Archive IS NULL OR E.Deleted = @Archive)
			INNER JOIN [dbo].[FG_FormInstances] AS I
			ON I.ApplicationID = F.ApplicationID AND I.InstanceID = E.InstanceID AND 
				(@Archive IS NULL OR I.Deleted = @Archive)
			INNER JOIN [dbo].[CN_View_Nodes_Normal] AS ND
			ON ND.ApplicationID = F.ApplicationID AND ND.NodeID = I.OwnerID AND 
				(@Archive IS NULL OR ND.Deleted = @Archive) AND
				(@NodesCount = 0 OR ND.NodeID IN (SELECT N.[Value] FROM @NodeIDs AS N))
		WHERE F.OwnerType = N'FormElement'
	)
	
	RETURN
END

GO