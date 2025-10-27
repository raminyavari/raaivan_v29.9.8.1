USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[RV_KnowledgeSupplyIndicatorsReport_Chart]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[RV_KnowledgeSupplyIndicatorsReport_Chart]
GO

CREATE PROCEDURE [dbo].[RV_KnowledgeSupplyIndicatorsReport_Chart]
	@ApplicationID			uniqueidentifier,
	@CurrentUserID			uniqueidentifier,
	@Period					varchar(50),
	@CalendarType			varchar(50),
	@PeriodList				BigIntTableType readonly,
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

	;WITH Content AS (
		SELECT X.UserID, X.ContentID, X.ContentCollaborationShare, X.ContentStatus,
			X.ContentScore, X.ContentPublished, X.AnswerID, X.WikiParagraphID, 
			[dbo].[GFN_GetTimePeriod](X.[Date], @Period, @CalendarType) AS [Period]
		FROM [dbo].[RV_FN_KnowledgeSupplyIndicatorsReport](@ApplicationID, @CurrentUserID, @ContentTypeIDs, 
			@UserIDs, @AllUsers, @LowerCreationDateLimit, @UpperCreationDateLimit) AS X
	)
	SELECT	P.[Value] AS [Period],
			COUNT(DISTINCT C.ContentID) AS RegisteredCount,
			COUNT(DISTINCT (CASE WHEN C.ContentPublished = 1 THEN C.ContentID ELSE NULL END)) AS PublishedCount
	FROM @PeriodList AS P
		LEFT JOIN Content AS C
		ON C.[Period] = P.[Value]
	GROUP BY P.[Value]
	ORDER BY P.[Value]
END

GO

