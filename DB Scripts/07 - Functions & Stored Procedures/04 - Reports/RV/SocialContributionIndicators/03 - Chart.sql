USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[RV_SocialContributionIndicatorsReport_Chart]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[RV_SocialContributionIndicatorsReport_Chart]
GO

CREATE PROCEDURE [dbo].[RV_SocialContributionIndicatorsReport_Chart]
	@ApplicationID			uniqueidentifier,
	@CurrentUserID			uniqueidentifier,
	@Period					varchar(50),
	@CalendarType			varchar(50),
	@PeriodList				BigIntTableType readonly,
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

	IF @AllUsers = 0 BEGIN
		INSERT INTO @UserIDs ([Value])
		SELECT DISTINCT NM.UserID
		FROM [dbo].[CN_Nodes] AS Groups
			INNER JOIN [dbo].[CN_View_NodeMembers] AS NM
			ON NM.ApplicationID = @ApplicationID AND NM.NodeID = Groups.NodeID AND NM.IsPending = 0
		WHERE Groups.ApplicationID = @ApplicationID AND (
				(@CreatorCount > 0 AND Groups.NodeID IN (SELECT * FROM @CreatorNodeIDs)) OR
				(@CreatorCount = 0 AND
					(@CreatorNodeTypeID IS NULL OR Groups.NodeTypeID = @CreatorNodeTypeID)
				)
			) AND Groups.Deleted = 0
	END

	;WITH Social AS (
		SELECT	X.UserID, 
				X.PostID, 
				X.CommentID, 
				[dbo].[GFN_GetTimePeriod](X.PostSendDate, @Period, @CalendarType) AS PostPeriod,
				[dbo].[GFN_GetTimePeriod](X.CommentSendDate, @Period, @CalendarType) AS CommentPeriod
		FROM [dbo].[RV_FN_SocialContributionIndicatorsReport](@ApplicationID, @CurrentUserID, @UserIDs, 
			@AllUsers, @LowerCreationDateLimit, @UpperCreationDateLimit) AS X
	)
	SELECT	R.[Period],
			MAX(R.PostsCount) AS PostsCount,
			MAX(R.CommentsCount) AS CommentsCount
	FROM (
			SELECT P.[Value] AS [Period], ISNULL(COUNT(DISTINCT S.PostID), 0) AS PostsCount, 0 AS CommentsCount
			FROM @PeriodList AS P
				LEFT JOIN Social AS S
				ON S.PostPeriod = P.[Value]
			GROUP BY P.[Value]

			UNION ALL

			SELECT P.[Value] AS [Period], 0 AS PostsCount, ISNULL(COUNT(DISTINCT S.CommentID), 0) AS CommentsCount
			FROM @PeriodList AS P
				LEFT JOIN Social AS S
				ON S.CommentPeriod = P.[Value]
			GROUP BY P.[Value]
		) AS R
	GROUP BY R.[Period]
	ORDER BY R.[Period]
END

GO

