USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[RV_KnowledgeDemandIndicatorsReport]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[RV_KnowledgeDemandIndicatorsReport]
GO

CREATE PROCEDURE [dbo].[RV_KnowledgeDemandIndicatorsReport]
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
			SELECT X.UserID, X.VisitedID, X.SearchID, X.QuestionID, X.PostID, X.CommentID, X.[Date]
			FROM [dbo].[RV_FN_KnowledgeDemandIndicatorsReport](@ApplicationID, @CurrentUserID, @ContentTypeIDs, 
				@UserIDs, @AllUsers, @LowerCreationDateLimit, @UpperCreationDateLimit) AS X
		)
		SELECT	G.GroupID AS GroupID_Hide, 
				MAX(G.GroupName) AS GroupName,
				COUNT(DISTINCT G.UserID) AS MembersCount,
				COUNT(C.SearchID) AS SearchesCount,
				COUNT(C.QuestionID) AS QuestionsCount,
				COUNT(C.VisitedID) AS ContentVisitsCount,
				COUNT(DISTINCT C.VisitedID) AS DistinctContentVisitsCount,
				COUNT(DISTINCT C.PostID) AS PostsCount,
				COUNT(DISTINCT C.CommentID) AS CommentsCount
		FROM @GroupMembers AS G
			INNER JOIN Content AS C
			ON C.UserID = G.UserID
		GROUP BY G.GroupID
	END
	ELSE BEGIN
		;WITH Content AS (
			SELECT X.UserID, X.VisitedID, X.SearchID, X.QuestionID, X.PostID, X.CommentID, X.[Date]
			FROM [dbo].[RV_FN_KnowledgeDemandIndicatorsReport](@ApplicationID, @CurrentUserID, @ContentTypeIDs, 
				@UserIDs, @AllUsers, @LowerCreationDateLimit, @UpperCreationDateLimit) AS X
		)
		SELECT	X.UserID AS UserID_Hide,
				LTRIM(RTRIM(ISNULL(UN.FirstName, N' ') + N' ' + ISNULL(UN.LastName, N' '))) AS FullName,
				X.SearchesCount,
				X.QuestionsCount,
				X.ContentVisitsCount,
				X.DistinctContentVisitsCount,
				X.PostsCount,
				X.CommentsCount
		FROM (
				SELECT	C.UserID, 
						COUNT(C.SearchID) AS SearchesCount,
						COUNT(C.QuestionID) AS QuestionsCount,
						COUNT(C.VisitedID) AS ContentVisitsCount,
						COUNT(DISTINCT C.VisitedID) AS DistinctContentVisitsCount,
						COUNT(DISTINCT C.PostID) AS PostsCount,
						COUNT(DISTINCT C.CommentID) AS CommentsCount
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