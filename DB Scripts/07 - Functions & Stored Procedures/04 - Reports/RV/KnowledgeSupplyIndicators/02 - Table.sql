USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[RV_KnowledgeSupplyIndicatorsReport]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[RV_KnowledgeSupplyIndicatorsReport]
GO

CREATE PROCEDURE [dbo].[RV_KnowledgeSupplyIndicatorsReport]
	@ApplicationID			uniqueidentifier,
	@CurrentUserID			uniqueidentifier,
	@ContentTypeIDsTemp		GuidTableType readonly,
	@CreatorNodeTypeID		uniqueidentifier,
	@NodeIDsTemp			GuidTableType readonly,
	@LowerCreationDateLimit datetime,
	@UpperCreationDateLimit datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @ContentTypeIDs GuidTableType
	INSERT INTO @ContentTypeIDs (Value) SELECT Ref.Value FROM @ContentTypeIDsTemp AS Ref
	
	DECLARE @CreatorNodeIDs GuidTableType

	INSERT INTO @CreatorNodeIDs ([Value])
	SELECT Ref.[Value]
	FROM @NodeIDsTemp AS Ref

	DECLARE @CreatorCount int = (SELECT COUNT(*) FROM @CreatorNodeIDs)

	DECLARE @UserIDs GuidTableType

	DECLARE @AllUsers bit = 0

	IF @CreatorNodeTypeID IS NULL AND NOT EXISTS (SELECT TOP(1) * FROM @CreatorNodeIDs) SET @AllUsers = 1

	DECLARE @GroupMembers TABLE (
		GroupID				uniqueidentifier, 
		GroupName			nvarchar(500), 
		UserID				uniqueidentifier,
		LastActivityDate	datetime
	)

	IF @AllUsers = 0 BEGIN
		INSERT INTO @GroupMembers (GroupID, GroupName, UserID, LastActivityDate)
		SELECT Groups.NodeID, Groups.[Name], NM.UserID, UN.LastActivityDate
		FROM [dbo].[CN_Nodes] AS Groups
			INNER JOIN [dbo].[CN_View_NodeMembers] AS NM
			ON NM.ApplicationID = @ApplicationID AND NM.NodeID = Groups.NodeID AND NM.IsPending = 0
			INNER JOIN [dbo].[Users_Normal] AS UN
			ON UN.ApplicationID = @ApplicationID AND UN.UserID = NM.UserID
		WHERE Groups.ApplicationID = @ApplicationID AND (
				(@CreatorCount > 0 AND Groups.NodeID IN (SELECT * FROM @CreatorNodeIDs)) OR
				(@CreatorCount = 0 AND
					(@CreatorNodeTypeID IS NULL OR Groups.NodeTypeID = @CreatorNodeTypeID)
				)
			) AND Groups.Deleted = 0 AND UN.IsApproved = 1

		INSERT INTO @UserIDs ([Value])
		SELECT DISTINCT G.UserID
		FROM @GroupMembers AS G
	END

	IF @AllUsers = 0 BEGIN
		;WITH Content AS (
			SELECT X.UserID, X.ContentID, X.ContentCollaborationShare, X.ContentStatus,
				X.ContentScore, X.ContentPublished, X.AnswerID, X.WikiParagraphID, X.[Date]
			FROM [dbo].[RV_FN_KnowledgeSupplyIndicatorsReport](@ApplicationID, @CurrentUserID, @ContentTypeIDs, 
				@UserIDs, @AllUsers, @LowerCreationDateLimit, @UpperCreationDateLimit) AS X
		),
		SummaryCol AS (
			SELECT G.GroupID, C.ContentID, SUM(ISNULL(C.ContentCollaborationShare, 0)) AS CollaborationShare
			FROM @GroupMembers AS G
				INNER JOIN Content AS C
				ON C.UserID = G.UserID
			GROUP BY G.GroupID, C.ContentID
		),
		SummaryScore AS (
			SELECT G.GroupID, C.ContentID,
				MAX(CASE WHEN C.[ContentStatus] = N'Accepted' THEN ISNULL(C.ContentScore, 0) ELSE 0 END) AS [Score]
			FROM @GroupMembers AS G
				INNER JOIN Content AS C
				ON C.UserID = G.UserID
			GROUP BY G.GroupID, C.ContentID
		)
		SELECT	G.GroupID AS GroupID_Hide, 
				MAX(G.GroupName) AS GroupName,
				COUNT(DISTINCT G.UserID) AS MembersCount,
				COUNT(DISTINCT C.ContentID) AS ContentsCount,
				CASE WHEN COUNT(DISTINCT C.ContentID) = 0 THEN 0 ELSE MAX(S.CollaborationShare) / COUNT(DISTINCT C.ContentID) END AS AverageCollaborationShare,
				COUNT(DISTINCT (CASE WHEN C.ContentStatus = N'Accepted' THEN C.ContentID ELSE NULL END)) AS AcceptedCount,
				CASE WHEN COUNT(DISTINCT C.ContentID) = 0 THEN 0 ELSE MAX(SC.Score) / COUNT(DISTINCT C.ContentID) END AS AverageAcceptedScore,
				COUNT(DISTINCT (CASE WHEN C.ContentPublished = 1 THEN C.ContentID ELSE NULL END)) AS PublishedCount,
				COUNT(DISTINCT C.AnswerID) AS AnswersCount,
				COUNT(DISTINCT C.WikiParagraphID) AS WikiChangesCount
		FROM @GroupMembers AS G
			INNER JOIN Content AS C
			ON C.UserID = G.UserID
			INNER JOIN SummaryCol AS S
			ON S.GroupID = G.GroupID AND S.ContentID = C.ContentID
			INNER JOIN SummaryScore AS SC
			ON SC.GroupID = G.GroupID AND SC.ContentID = C.ContentID
		GROUP BY G.GroupID
	END
	ELSE BEGIN
		;WITH Content AS (
			SELECT X.UserID, X.ContentID, X.ContentCollaborationShare, X.ContentStatus,
				X.ContentScore, X.ContentPublished, X.AnswerID, X.WikiParagraphID, X.[Date]
			FROM [dbo].[RV_FN_KnowledgeSupplyIndicatorsReport](@ApplicationID, @CurrentUserID, @ContentTypeIDs, 
				@UserIDs, @AllUsers, @LowerCreationDateLimit, @UpperCreationDateLimit) AS X
		)
		SELECT	X.UserID AS UserID_Hide,
				LTRIM(RTRIM(ISNULL(UN.FirstName, N' ') + N' ' + ISNULL(UN.LastName, N' '))) AS FullName,
				X.ContentsCount,
				X.AverageCollaborationShare,
				X.AcceptedCount,
				X.AverageAcceptedScore,
				X.PublishedCount,
				X.AnswersCount,
				X.WikiChangesCount
		FROM (
				SELECT	C.UserID, 
						COUNT(DISTINCT C.ContentID) AS ContentsCount,
						CASE 
							WHEN COUNT(DISTINCT C.ContentID) = 0 THEN 0 
							ELSE SUM(ISNULL(C.ContentCollaborationShare, 0)) / COUNT(DISTINCT C.ContentID) 
						END AS AverageCollaborationShare,
						COUNT(DISTINCT (CASE WHEN C.ContentStatus = N'Accepted' THEN C.ContentID ELSE NULL END)) AS AcceptedCount,
						CASE 
							WHEN COUNT(DISTINCT C.ContentID) = 0 THEN 0 
							ELSE SUM(ISNULL(C.ContentScore, 0)) / COUNT(DISTINCT C.ContentID) 
						END AS AverageAcceptedScore,
						COUNT(DISTINCT (CASE WHEN C.ContentPublished = 1 THEN C.ContentID ELSE NULL END)) AS PublishedCount,
						COUNT(DISTINCT C.AnswerID) AS AnswersCount,
						COUNT(DISTINCT C.WikiParagraphID) AS WikiChangesCount
				FROM Content AS C
				GROUP BY C.UserID
			) AS X
			INNER JOIN [dbo].[Users_Normal] AS UN
			ON UN.ApplicationID = @ApplicationID AND UN.UserID = X.UserID
	END
	
	SELECT ('{' +
		'"GroupName": {"Action": "Link", "Type": "Node",' +
			'"Requires": {"ID": "GroupID_Hide"}' +
		'},' +
		'"FullName": {"Action": "Link", "Type": "User",' +
			'"Requires": {"ID": "UserID_Hide"}' +
		'}' +
	   '}') AS Actions
END

GO