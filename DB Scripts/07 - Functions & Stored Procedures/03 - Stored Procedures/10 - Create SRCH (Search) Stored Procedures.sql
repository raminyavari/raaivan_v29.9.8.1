USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[SRCH_GetIndexQueueItems]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SRCH_GetIndexQueueItems]
GO

CREATE PROCEDURE [dbo].[SRCH_GetIndexQueueItems]
	@ApplicationID	uniqueidentifier,
    @Count			int,
	@ItemType		varchar(20)
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF @Count IS NULL SET @Count = 10
	IF @ItemType IS NULL SET @ItemType = N'Node'

	IF @ItemType = N'Node' BEGIN
		SELECT TOP(@Count) 
			ND.NodeID AS ID,
			CASE
				WHEN ND.Deleted = 1 OR ISNULL(ND.Searchable, 1) = 0 OR ISNULL(S.NoContent, 0) = 1 
					THEN CAST(1 AS bit)
				ELSE CAST(0 AS bit)
			END AS Deleted,
			CASE
				WHEN ND.Deleted = 1 OR ISNULL(ND.Searchable, 1) = 0 OR ISNULL(S.NoContent, 0) = 1 THEN NULL
				ELSE ND.NodeTypeID
			END AS TypeID,
			CASE
				WHEN ND.Deleted = 1 OR ISNULL(ND.Searchable, 1) = 0 OR ISNULL(S.NoContent, 0) = 1 THEN NULL
				ELSE ND.TypeName
			END AS [Type],
			CASE
				WHEN ND.Deleted = 1 OR ISNULL(ND.Searchable, 1) = 0 OR ISNULL(S.NoContent, 0) = 1 THEN NULL
				ELSE ND.NodeAdditionalID
			END AS AdditionalID,
			CASE
				WHEN ND.Deleted = 1 OR ISNULL(ND.Searchable, 1) = 0 OR ISNULL(S.NoContent, 0) = 1 THEN NULL
				ELSE ND.NodeName
			END AS Title,
			CASE
				WHEN ND.Deleted = 1 OR ISNULL(ND.Searchable, 1) = 0 OR ISNULL(S.NoContent, 0) = 1 THEN NULL
				ELSE ND.[Description]
			END AS [Description],
			CASE
				WHEN ND.Deleted = 1 OR ISNULL(ND.Searchable, 1) = 0 OR ISNULL(S.NoContent, 0) = 1 THEN NULL
				ELSE REPLACE(ND.Tags, N'~', N' ')
			END AS Tags,
			CASE
				WHEN ND.Deleted = 1 OR ISNULL(ND.Searchable, 1) = 0 OR ISNULL(S.NoContent, 0) = 1 THEN NULL
				ELSE ISNULL([dbo].[WK_FN_GetWikiContent](@ApplicationID, ND.NodeID), N'') +  N' ' +
					ISNULL([dbo].[FG_FN_GetOwnerFormContents](@ApplicationID, ND.NodeID, 3), N'')
			END AS Content,
			CASE
				WHEN ND.Deleted = 1 OR ISNULL(ND.Searchable, 1) = 0 OR ISNULL(S.NoContent, 0) = 1 THEN NULL
				ELSE [dbo].[CN_FN_GetNodeFileContents](@ApplicationID, ND.NodeID)
			END AS FileContent
		FROM [dbo].[CN_View_Nodes_Normal] AS ND
			LEFT JOIN [dbo].[CN_Services] AS S
			ON S.ApplicationID = @ApplicationID AND S.NodeTypeID = ND.NodeTypeID AND S.Deleted = 0
		WHERE ND.ApplicationID = @ApplicationID
		ORDER BY ISNULL(ND.IndexLastUpdateDate, N'1977-01-01 00:00:00.000')
	END
	ELSE IF @ItemType = N'NodeType' BEGIN
		SELECT TOP(@Count) 
			NT.NodeTypeID AS ID,
			CASE
				WHEN NT.Deleted = 1 OR ISNULL(S.NoContent, 0) = 1 THEN CAST(1 AS bit)
				ELSE CAST(0 AS bit)
			END AS Deleted,
			CASE
				WHEN NT.Deleted = 1 OR ISNULL(S.NoContent, 0) = 1 THEN NULL
				ELSE NT.Name
			END AS Title,
			CASE
				WHEN NT.Deleted = 1 OR ISNULL(S.NoContent, 0) = 1 THEN NULL
				ELSE NT.[Description]
			END AS [Description]
		FROM [dbo].[CN_NodeTypes] AS NT
			LEFT JOIN [dbo].[CN_Services] AS S
			ON S.ApplicationID = @ApplicationID AND S.NodeTypeID = NT.NodeTypeID AND S.Deleted = 0
		WHERE NT.ApplicationID = @ApplicationID
		ORDER BY ISNULL(NT.IndexLastUpdateDate, N'1977-01-01 00:00:00.000')
	END
	ELSE IF @ItemType = N'Question' BEGIN
		SELECT TOP(@Count) 
			QA.QuestionID AS ID,
			QA.Deleted AS Deleted,
			CASE
				WHEN QA.Deleted = 1 THEN NULL
				ELSE QA.[Title]
			END AS Title,
			CASE
				WHEN QA.Deleted = 1 THEN NULL
				ELSE QA.[Description]
			END AS [Description],
			CASE
				WHEN QA.Deleted = 1 THEN NULL
				ELSE [dbo].[QA_FN_GetQuestionContent](@ApplicationID, QA.QuestionID)
			END AS Content
		FROM [dbo].[QA_Questions] AS QA
		WHERE QA.ApplicationID = @ApplicationID
		ORDER BY ISNULL(QA.IndexLastUpdateDate, N'1977-01-01 00:00:00.000')
	END
	ELSE IF @ItemType = N'File' BEGIN
		;WITH X (ID, OwnerID, [Type], Title, FileContent)
		AS
		(
			SELECT TOP(@Count) 
				FC.FileID AS ID,
				AF.OwnerID,
				AF.Extension AS [Type],
				AF.[FileName] AS Title,
				FC.Content AS [FileContent]
			FROM [dbo].[DCT_FileContents] AS FC
				INNER JOIN (
					SELECT DISTINCT OwnerID, FileNameGuid, [FileName], Extension
					FROM [dbo].[DCT_Files] AS AF
					WHERE AF.ApplicationID = @ApplicationID
				) AS AF
				ON AF.FileNameGuid = FC.FileID
			WHERE FC.ApplicationID = @ApplicationID AND 
				FC.NotExtractable = 0 AND FC.FileNotFound = 0
			ORDER BY ISNULL(FC.IndexLastUpdateDate, N'1977-01-01 00:00:00.000')
		)
		(
			SELECT	X.ID,
					CAST(B.Deleted AS bit) AS Deleted,
					CASE WHEN B.Deleted = 1 THEN NULL ELSE X.[Type] END AS [Type],
					CASE WHEN B.Deleted = 1 THEN NULL ELSE X.Title END AS Title,
					CASE WHEN B.Deleted = 1 THEN NULL ELSE X.FileContent END AS FileContent
			FROM X
				LEFT JOIN (
					SELECT A.ID, MAX(A.[Type]) AS [Type], MAX(A.Title) AS Title, 
						MAX(A.FileContent) AS FileContent, MIN(A.Deleted) AS Deleted
					FROM (
							SELECT X.ID, X.[Type], X.Title, X.FileContent, ND.Deleted
							FROM X
								INNER JOIN [dbo].[CN_Nodes] AS ND
								ON ND.ApplicationID = @ApplicationID AND ND.NodeID = X.OwnerID
								
							UNION ALL
							
							SELECT X.ID, X.[Type], X.Title, X.FileContent,
								(CASE WHEN E.Deleted = 1 OR I.Deleted = 1 OR ND.Deleted = 1 THEN 1 ELSE 0 END)
							FROM X
								INNER JOIN [dbo].[FG_InstanceElements] AS E
								ON E.ApplicationID = @ApplicationID AND E.ElementID = X.OwnerID
								INNER JOIN [dbo].[FG_FormInstances] AS I
								ON I.ApplicationID = @ApplicationID AND I.InstanceID = E.InstanceID
								INNER JOIN [dbo].[CN_Nodes] AS ND
								ON ND.ApplicationID = @ApplicationID AND ND.NodeID = I.OwnerID
						) A
					GROUP BY A.ID
				) AS B
				ON B.ID = X.ID
		)
	END
	ELSE IF @ItemType = N'User' BEGIN
		SELECT TOP(@Count) 
			UN.UserID AS ID,
			CASE
				WHEN UN.IsApproved = 1 THEN CAST(0 AS bit)
				ELSE CAST(1 AS bit)
			END AS [Deleted],
			UN.UserName AS AdditionalID,
			ISNULL(UN.FirstName, N'') + N' ' + ISNULL(UN.LastName, N'') AS Title
		FROM [dbo].[Users_Normal] AS UN
		WHERE UN.ApplicationID = @ApplicationID
		ORDER BY ISNULL(UN.IndexLastUpdateDate, N'1977-01-01 00:00:00.000')
	END
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[SRCH_SetIndexLastUpdateDate]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SRCH_SetIndexLastUpdateDate]
GO

CREATE PROCEDURE [dbo].[SRCH_SetIndexLastUpdateDate]
	@ApplicationID	uniqueidentifier,
	@ItemType		varchar(20),
	@strIDs			varchar(max),
	@delimiter		char,
	@Date			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @IDs GuidTableType
	INSERT INTO @IDs
	SELECT DISTINCT * FROM [dbo].[GFN_StrToGuidTable](@strIDs, @delimiter)
	
	IF @ItemType = N'Node' BEGIN
		UPDATE Ref
			SET IndexLastUpdateDate = @Date
		FROM @IDs AS IDs
			INNER JOIN [dbo].[CN_Nodes] AS Ref
			ON Ref.ApplicationID = @ApplicationID AND Ref.NodeID = IDs.Value
	END
	ELSE IF @ItemType = N'NodeType' BEGIN
		UPDATE Ref
			SET IndexLastUpdateDate = @Date
		FROM @IDs AS IDs
			INNER JOIN [dbo].[CN_NodeTypes] AS Ref
			ON Ref.ApplicationID = @ApplicationID AND Ref.NodeTypeID = IDs.Value
	END
	ELSE IF @ItemType = N'Question' BEGIN
		UPDATE Ref
			SET IndexLastUpdateDate = @Date
		FROM @IDs AS IDs
			INNER JOIN [dbo].[QA_Questions] AS Ref
			ON Ref.ApplicationID = @ApplicationID AND Ref.QuestionID = IDs.Value
	END
	ELSE IF @ItemType = N'File' BEGIN
		UPDATE Ref
			SET IndexLastUpdateDate = @Date
		FROM @IDs AS IDs
			INNER JOIN [dbo].[DCT_FileContents] AS Ref
			ON Ref.ApplicationID = @ApplicationID AND Ref.FileID = IDs.Value
	END
	ELSE IF @ItemType = N'User' BEGIN
		UPDATE Ref
			SET IndexLastUpdateDate = @Date
		FROM @IDs AS IDs
			INNER JOIN [dbo].[USR_Profile] AS Ref
			ON Ref.UserID = IDs.Value
	END
	
	SELECT @@ROWCOUNT
END

GO


