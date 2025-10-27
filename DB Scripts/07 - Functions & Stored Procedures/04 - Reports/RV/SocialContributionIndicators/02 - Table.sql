USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[RV_SocialContributionIndicatorsReport]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[RV_SocialContributionIndicatorsReport]
GO

CREATE PROCEDURE [dbo].[RV_SocialContributionIndicatorsReport]
	@ApplicationID			uniqueidentifier,
	@CurrentUserID			uniqueidentifier,
	@CreatorNodeTypeID		uniqueidentifier,
	@NodeIDsTemp			GuidTableType readonly,
	@LowerCreationDateLimit datetime,
	@UpperCreationDateLimit datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
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
		;WITH Social AS (
			SELECT X.UserID, X.PostID, X.CommentID, X.PostSendDate, X.CommentSendDate
			FROM [dbo].[RV_FN_SocialContributionIndicatorsReport](@ApplicationID, @CurrentUserID, @UserIDs, 
				@AllUsers, @LowerCreationDateLimit, @UpperCreationDateLimit) AS X
		)
		SELECT	G.GroupID AS GroupID_Hide,
				MAX(G.GroupName) AS GroupName,
				COUNT(DISTINCT G.UserID) MembersCount,
				MAX(R.ActiveUsersCount) AS ActiveUsersCount,
				MAX(R.PostsCount) AS PostsCount,
				MAX(R.CommentsCount) AS CommentsCount
		FROM (
				SELECT	G.GroupID,
						COUNT(G.UserID) AS ActiveUsersCount,
						0 AS PostsCount,
						0 AS CommentsCount
				FROM @GroupMembers AS G
				WHERE (@LowerCreationDateLimit IS NULL OR G.LastActivityDate >= @LowerCreationDateLimit) AND
					(@UpperCreationDateLimit IS NULL OR G.LastActivityDate <= @UpperCreationDateLimit)
				GROUP BY G.GroupID

				UNION ALL

				SELECT	G.GroupID,
						0 AS ActiveUsersCount,
						COUNT(S.PostID) AS PostsCount,
						0 AS CommentsCount
				FROM @GroupMembers AS G
					INNER JOIN Social AS S
					ON S.UserID = G.UserID
				GROUP BY G.GroupID

				UNION ALL

				SELECT	G.GroupID,
						0 AS ActiveUsersCount,
						0 AS PostsCount,
						COUNT(S.CommentID) AS CommentsCount
				FROM @GroupMembers AS G
					INNER JOIN Social AS S
					ON S.UserID = G.UserID
				GROUP BY G.GroupID
			) AS R
			INNER JOIN @GroupMembers AS G
			ON G.GroupID = R.GroupID
		GROUP BY G.GroupID
	END
	ELSE BEGIN
		;WITH Social AS (
			SELECT X.UserID, X.PostID, X.CommentID, X.PostSendDate, X.CommentSendDate
			FROM [dbo].[RV_FN_SocialContributionIndicatorsReport](@ApplicationID, @CurrentUserID, @UserIDs, 
				@AllUsers, @LowerCreationDateLimit, @UpperCreationDateLimit) AS X
		)
		SELECT	R.UserID AS UserID_Hide,
				LTRIM(RTRIM(ISNULL(MAX(UN.FirstName), N' ') + N' ' + ISNULL(MAX(UN.LastName), N' '))) AS FullName,
				MAX(R.PostsCount) AS PostsCount,
				MAX(R.CommentsCount) AS CommentsCount
		FROM (
				SELECT	S.UserID,
						COUNT(S.PostID) AS PostsCount,
						0 AS CommentsCount
				FROM Social AS S
				GROUP BY S.UserID

				UNION ALL

				SELECT	S.UserID,
						0 AS PostsCount,
						COUNT(S.CommentID) AS CommentsCount
				FROM Social AS S
				GROUP BY S.UserID
			) AS R
			INNER JOIN [dbo].[Users_Normal] AS UN
			ON UN.ApplicationID = @ApplicationID AND UN.UserID = R.UserID
		GROUP BY R.UserID
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