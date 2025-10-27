USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PRVC_FN_DefaultValueToBoolean]') 
            AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[PRVC_FN_DefaultValueToBoolean]
GO

CREATE FUNCTION [dbo].[PRVC_FN_DefaultValueToBoolean]
(
	@Value	varchar(20)
)
RETURNS bit
WITH ENCRYPTION
AS
BEGIN
	IF ISNULL(@Value, N'') = N'Public' RETURN 1
	ELSE IF ISNULL(@Value, N'') <> N'' RETURN 0
	
	RETURN NULL
END

GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PRVC_FN_CheckNodePermission]') 
            AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[PRVC_FN_CheckNodePermission]
GO

CREATE FUNCTION [dbo].[PRVC_FN_CheckNodePermission]
(
	@Node				bit,
	@NodeType			bit,
	@DocumentTreeNode	bit,
	@DocumentTree		bit
)
RETURNS bit
WITH ENCRYPTION
AS
BEGIN
	RETURN CASE
		WHEN @Node IS NOT NULL THEN @Node
		WHEN @NodeType IS NULL THEN ISNULL(@DocumentTreeNode, @DocumentTree)
		WHEN @DocumentTreeNode IS NULL AND @DocumentTree IS NULL THEN @NodeType
		WHEN @NodeType < ISNULL(@DocumentTreeNode, @DocumentTree) THEN @NodeType
		ELSE ISNULL(@DocumentTreeNode, @DocumentTree)
	END
	
	RETURN NULL
END

GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PRVC_FN_CheckAccess]') 
    AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[PRVC_FN_CheckAccess]
GO

CREATE FUNCTION [dbo].[PRVC_FN_CheckAccess](
	@ApplicationID		uniqueidentifier,
	@UserID				uniqueidentifier,
	@ObjectIDs			KeyLessGuidTableType readonly,
	@ObjectType			varchar(50),
	@Now				datetime,
	@PermissionTypes	StringPairTableType readonly -- FirstValue: Type, SecondValue: DefaultPrivacy
)
RETURNS
@OutputTable TABLE
(
	ID		uniqueidentifier,
	[Type]	varchar(50)
)
WITH ENCRYPTION
AS
BEGIN
	DECLARE @Yesterday datetime
	IF @Now IS NOT NULL SET @Yesterday = DATEADD(DAY, -1, @Now)

	-- Find Confidentiality of the User
	DECLARE @UserConf int

	IF @UserID IS NULL SET @UserConf = 0
	ELSE SET @UserConf = ISNULL(
		(
			SELECT TOP(1) C.LevelID 
			FROM [dbo].[PRVC_View_Confidentialities] AS C
			WHERE C.ApplicationID = @ApplicationID  AND C.ObjectID = @UserID
		),
		ISNULL((
			SELECT MIN(C.LevelID) 
			FROM [dbo].[PRVC_ConfidentialityLevels] AS C
			WHERE C.ApplicationID = @ApplicationID AND C.Deleted = 0
		), 0)
	)
	-- end of Find Confidentiality of the User

	DECLARE @GroupIDs GuidTableType

	INSERT INTO @GroupIDs (Value)
	SELECT NM.NodeID
	FROM [dbo].[CN_View_NodeMembers] AS NM
	WHERE NM.ApplicationID = @ApplicationID AND NM.UserID = @UserID AND NM.IsPending = 0

	DECLARE @ItemIDs TABLE (ID uniqueidentifier, [Level] int)

	INSERT INTO @ItemIDs(ID, [Level])
	SELECT X.NodeID, MIN(ISNULL(X.[Level], 0)) + 1
	FROM [dbo].[CN_FN_GetNodesHierarchy](@ApplicationID, @GroupIDs) AS X
	GROUP BY X.NodeID


	IF ISNULL(@ObjectType, N'') <> N'Node' BEGIN
		INSERT INTO @OutputTable (ID, [Type])
		SELECT O.Value, P.FirstValue
		FROM (
				SELECT Ref.ObjectID AS ID, Ref.[Type], Ref.Allow
				FROM (
						SELECT	ROW_NUMBER() OVER (PARTITION BY X.ObjectID, X.[Type]
									ORDER BY X.[Level] ASC, X.Allow ASC) AS RowNumber,
								X.ObjectID,
								X.[Type],
								X.Allow
						FROM (
								SELECT	O.Value AS ObjectID, 
										A.PermissionType AS [Type], 
										ISNULL(I.[Level], 0) AS [Level], 
										A.Allow
								FROM @ObjectIDs AS O
									INNER JOIN [dbo].[PRVC_Audience] AS A
									ON A.ApplicationID = @ApplicationID AND A.ObjectID = O.Value AND A.Deleted = 0 AND
										A.PermissionType IN (SELECT P.FirstValue FROM @PermissionTypes AS P) AND
										(A.ExpirationDate IS NULL OR (@Yesterday IS NOT NULL AND A.ExpirationDate >= @Yesterday))
									LEFT JOIN [dbo].[PRVC_Settings] AS S
									ON S.ApplicationID = @ApplicationID AND S.ObjectID = O.Value
									LEFT JOIN [dbo].[PRVC_ConfidentialityLevels] AS CL
									ON CL.ApplicationID = @ApplicationID AND CL.ID = S.ConfidentialityID AND CL.Deleted = 0
									LEFT JOIN @ItemIDs AS I
									ON I.ID = A.RoleID
								WHERE (I.ID IS NOT NULL OR A.RoleID = @UserID) AND
									(S.ObjectID IS NULL OR ISNULL(S.CalculateHierarchy, 0) = 1 OR ISNULL(I.[Level], 0) <= 1) AND
									@UserConf >= ISNULL(CL.LevelID, 0)
							) AS X
					) AS Ref
				WHERE Ref.RowNumber = 1
			) AS A
			RIGHT JOIN @ObjectIDs AS O
			CROSS JOIN @PermissionTypes AS P
			LEFT JOIN [dbo].[PRVC_DefaultPermissions] AS D
			ON D.ApplicationID = @ApplicationID AND D.ObjectID = O.Value AND D.PermissionType = P.FirstValue
			ON O.Value = A.ID AND P.FirstValue = A.[Type]
		WHERE ISNULL(A.Allow, 0) = 1 OR (A.ID IS NULL AND ISNULL(ISNULL(D.DefaultValue, P.SecondValue), N'') = N'Public')
	END
	ELSE BEGIN
		DECLARE @Nodes TABLE (
			NodeID uniqueidentifier, NodeTypeID uniqueidentifier, 
			DocumentTreeNodeID uniqueidentifier, DocumentTreeID uniqueidentifier, 
			PermissionType varchar(50),  
			DefaultValue bit, DefaultN bit, DefaultNT bit, DefaultDTN bit, DefaultDT bit,
			NAllow bit, NTAllow bit, DTNAllow bit, DTAllow bit
		)

		INSERT INTO @Nodes (NodeID, NodeTypeID, DocumentTreeNodeID, DocumentTreeID, PermissionType, DefaultValue)
		SELECT ND.NodeID, ND.NodeTypeID, ND.DocumentTreeNodeID, TN.TreeID, 
			P.FirstValue, [dbo].[PRVC_FN_DefaultValueToBoolean](P.SecondValue)
		FROM @ObjectIDs AS O
			CROSS JOIN @PermissionTypes AS P
			INNER JOIN [dbo].[CN_Nodes] AS ND
			ON ND.ApplicationID = @ApplicationID AND ND.NodeID = O.Value
			LEFT JOIN [dbo].[DCT_TreeNodes] AS TN
			ON ND.DocumentTreeNodeID IS NOT NULL AND TN.ApplicationID = @ApplicationID AND 
				TN.TreeNodeID = ND.DocumentTreeNodeID AND TN.Deleted = 0
			LEFT JOIN [dbo].[DCT_Trees] AS T
			ON ND.DocumentTreeNodeID IS NOT NULL AND T.ApplicationID = @ApplicationID AND 
				T.TreeID = TN.TreeID AND T.Deleted = 0
				
		DECLARE @IDs TABLE (ID uniqueidentifier, [Type] varchar(20), 
			DefaultValue bit, PRIMARY KEY CLUSTERED (ID ASC, [Type] ASC))
		
		INSERT INTO @IDs (ID, [Type], DefaultValue)
		SELECT DISTINCT I.Value, PT.FirstValue, [dbo].[PRVC_FN_DefaultValueToBoolean](DP.DefaultValue)
		FROM (
				SELECT X.Value FROM @ObjectIDs AS X
				UNION ALL 
				SELECT DISTINCT N.NodeTypeID FROM @Nodes AS N WHERE N.NodeTypeID IS NOT NULL
				UNION ALL
				SELECT DISTINCT N.DocumentTreeNodeID FROM @Nodes AS N WHERE N.DocumentTreeNodeID IS NOT NULL
				UNION ALL
				SELECT DISTINCT N.DocumentTreeID FROM @Nodes AS N WHERE N.DocumentTreeID IS NOT NULL
			) AS I
			CROSS JOIN @PermissionTypes AS PT
			LEFT JOIN [dbo].[PRVC_DefaultPermissions] AS DP
			ON DP.ApplicationID = @ApplicationID AND DP.ObjectID = I.Value AND DP.PermissionType = PT.FirstValue
			
		UPDATE @Nodes SET DefaultN = IDs.DefaultValue
		FROM @Nodes AS N INNER JOIN @IDs AS IDs ON IDs.ID = N.NodeID AND IDs.[Type] = N.PermissionType
			
		UPDATE @Nodes SET DefaultNT = IDs.DefaultValue
		FROM @Nodes AS N INNER JOIN @IDs AS IDs ON IDs.ID = N.NodeTypeID AND IDs.[Type] = N.PermissionType
		
		UPDATE @Nodes SET DefaultDTN = IDs.DefaultValue
		FROM @Nodes AS N INNER JOIN @IDs AS IDs ON IDs.ID = N.DocumentTreeNodeID AND IDs.[Type] = N.PermissionType
		
		UPDATE @Nodes SET DefaultDT = IDs.DefaultValue
		FROM @Nodes AS N INNER JOIN @IDs AS IDs ON IDs.ID = N.DocumentTreeID AND IDs.[Type] = N.PermissionType
		
		DECLARE @Values TABLE (ID uniqueidentifier, [Type] varchar(20), Allow bit)

		INSERT INTO @Values (ID, [Type], Allow)
		SELECT Ref.ObjectID, Ref.[Type], Ref.Allow
		FROM (
				SELECT	ROW_NUMBER() OVER (PARTITION BY X.ObjectID, X.[Type]
							ORDER BY X.[Level] ASC, X.Allow ASC) AS RowNumber,
						X.ObjectID,
						X.[Type],
						X.Allow
				FROM (
						SELECT	O.Value AS ObjectID, 
								A.PermissionType AS [Type], 
								ISNULL(I.[Level], 0) AS [Level], 
								A.Allow
						FROM (
								SELECT DISTINCT IDs.ID AS Value
								FROM @IDs AS IDs
							) AS O
							LEFT JOIN [dbo].[PRVC_Settings] AS S
							ON S.ApplicationID = @ApplicationID AND S.ObjectID = O.Value
							INNER JOIN [dbo].[PRVC_Audience] AS A
							ON A.ApplicationID = @ApplicationID AND A.ObjectID = O.Value AND A.Deleted = 0 AND
								A.PermissionType IN (SELECT P.FirstValue FROM @PermissionTypes AS P) AND
								(A.ExpirationDate IS NULL OR (@Yesterday IS NOT NULL AND A.ExpirationDate >= @Yesterday))
							LEFT JOIN @ItemIDs AS I
							ON I.ID = A.RoleID
						WHERE (I.ID IS NOT NULL OR A.RoleID = @UserID) AND 
							(S.ObjectID IS NULL OR ISNULL(S.CalculateHierarchy, 0) = 1 OR ISNULL(I.[Level], 0) <= 1)
					) AS X
			) AS Ref
		WHERE Ref.RowNumber = 1
		
		UPDATE @Nodes
			SET NAllow = V.Allow
		FROM @Nodes AS N
			INNER JOIN @Values AS V
			ON V.ID = N.NodeID AND V.[Type] = N.PermissionType
			
		UPDATE @Nodes
			SET NTAllow = V.Allow
		FROM @Nodes AS N
			INNER JOIN @Values AS V
			ON V.ID = N.NodeTypeID AND V.[Type] = N.PermissionType
			
		UPDATE @Nodes
			SET DTNAllow = V.Allow
		FROM @Nodes AS N
			INNER JOIN @Values AS V
			ON V.ID = N.DocumentTreeNodeID AND V.[Type] = N.PermissionType
			
		UPDATE @Nodes
			SET DTAllow = V.Allow
		FROM @Nodes AS N
			INNER JOIN @Values AS V
			ON V.ID = N.DocumentTreeID AND V.[Type] = N.PermissionType
		
		INSERT INTO @OutputTable (ID, [Type])
		SELECT	N.NodeID AS ID, 
				N.PermissionType AS [Type]
		FROM @Nodes AS N
			LEFT JOIN [dbo].[PRVC_View_Confidentialities] AS S
			ON S.ApplicationID = @ApplicationID AND S.ObjectID = N.NodeID
		WHERE @UserConf >= ISNULL(S.LevelID, 0) AND 
			ISNULL(ISNULL(
				[dbo].[PRVC_FN_CheckNodePermission](N.NAllow, N.NTAllow, N.DTNAllow, N.DTAllow),
				[dbo].[PRVC_FN_CheckNodePermission](N.DefaultN, N.DefaultNT, N.DefaultDTN, N.DefaultDT)
			), N.DefaultValue) = 1
	END
	
	RETURN
END

GO
