USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RV_FN_KnowledgeDemandIndicatorsReport]') 
    AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[RV_FN_KnowledgeDemandIndicatorsReport]
GO

CREATE FUNCTION [dbo].[RV_FN_KnowledgeDemandIndicatorsReport](
	@ApplicationID			uniqueidentifier,
	@CurrentUserID			uniqueidentifier,
	@ContentTypeIDsTemp		GuidTableType readonly,
	@UserIDsTemp			GuidTableType readonly,
	@AllUsers				bit,
	@LowerCreationDateLimit datetime,
	@UpperCreationDateLimit datetime
)
RETURNS @outputTable TABLE (
	UserID		uniqueidentifier,
	VisitedID	uniqueidentifier,
	SearchID	bigint,
	QuestionID	uniqueidentifier,
	PostID		uniqueidentifier,
	CommentID	uniqueidentifier,
	[Date]		datetime
)
WITH ENCRYPTION
AS
BEGIN
	DECLARE @ContentTypeIDs GuidTableType
	INSERT INTO @ContentTypeIDs (Value) SELECT Ref.Value FROM @ContentTypeIDsTemp AS Ref

	DECLARE @ContentTypesCount int = (SELECT COUNT(*) FROM @ContentTypeIDs)

	DECLARE @UserIDs GuidTableType

	INSERT INTO @UserIDs ([Value])
	SELECT Ref.[Value]
	FROM @UserIDsTemp AS Ref

	INSERT INTO @outputTable (UserID, VisitedID, SearchID, QuestionID, PostID, CommentID, [Date])
	(
		SELECT	IV.UserID,
				IV.ItemID AS VisitedID,
				NULL AS SearchID,
				NULL AS QuestionID,
				NULL AS PostID,
				NULL AS CommentID,
				IV.VisitDate AS [Date]
		FROM [dbo].[USR_ItemVisits] AS IV
			INNER JOIN [dbo].[CN_Nodes] AS ND
			ON ND.ApplicationID = @ApplicationID AND ND.NodeID = IV.ItemID AND ND.Deleted = 0 AND
				(@ContentTypesCount = 0 OR ND.NodeTypeID IN (SELECT Ref.[Value] FROM @ContentTypeIDs AS Ref))
			INNER JOIN [dbo].[Users_Normal] AS UN
			ON UN.ApplicationID = @ApplicationID AND UN.UserID = IV.UserID AND UN.IsApproved = 1
		WHERE IV.ApplicationID = @ApplicationID AND
			(@AllUsers = 1 OR IV.UserID IN (SELECT U.[Value] FROM @UserIDs AS U)) AND 
			(@LowerCreationDateLimit IS NULL OR IV.VisitDate >= @LowerCreationDateLimit) AND
			(@UpperCreationDateLimit IS NULL OR IV.VisitDate < @UpperCreationDateLimit)

		UNION ALL

		SELECT	L.UserID AS UserID,
				NULL AS VisitedID,
				L.LogID AS SearchID,
				NULL AS QuestionID,
				NULL AS PostID,
				NULL AS CommentID,
				L.[Date]
		FROM [dbo].[LG_Logs] AS L
			INNER JOIN [dbo].[Users_Normal] AS UN
			ON UN.ApplicationID = @ApplicationID AND UN.UserID = L.UserID AND UN.IsApproved = 1
		WHERE L.ApplicationID = @ApplicationID AND L.[Action] = N'Search' AND 
			(@AllUsers = 1 OR L.UserID IN (SELECT U.[Value] FROM @UserIDs AS U)) AND 
			(@LowerCreationDateLimit IS NULL OR L.[Date] >= @LowerCreationDateLimit) AND
			(@UpperCreationDateLimit IS NULL OR L.[Date] < @UpperCreationDateLimit)

		UNION ALL

		SELECT	Q.SenderUserID AS UserID,
				NULL AS VisitedID,
				NULL AS SearchID,
				Q.QuestionID AS QuestionID,
				NULL AS PostID,
				NULL AS CommentID,
				Q.SendDate AS [Date]
		FROM [dbo].[QA_Questions] AS Q
			INNER JOIN [dbo].[Users_Normal] AS UN
			ON UN.ApplicationID = @ApplicationID AND UN.UserID = Q.SenderUserID AND UN.IsApproved = 1
		WHERE Q.ApplicationID = @ApplicationID AND 
			(@AllUsers = 1 OR Q.SenderUserID IN (SELECT U.[Value] FROM @UserIDs AS U)) AND Q.Deleted = 0 AND
			(@LowerCreationDateLimit IS NULL OR Q.SendDate >= @LowerCreationDateLimit) AND
			(@UpperCreationDateLimit IS NULL OR Q.SendDate < @UpperCreationDateLimit)

		UNION ALL

		SELECT	PS.SenderUserID AS UserID,
				NULL AS VisitedID,
				NULL AS SearchID,
				NULL AS QuestionID,
				PS.ShareID AS PostID,
				NULL AS CommentID,
				PS.SendDate AS [Date]
		FROM [dbo].[SH_PostShares] AS PS
			INNER JOIN [dbo].[CN_Nodes] AS ND
			ON ND.ApplicationID = @ApplicationID AND ND.NodeID = PS.OwnerID AND ND.Deleted = 0 AND
				(@ContentTypesCount = 0 OR ND.NodeTypeID IN (SELECT Ref.[Value] FROM @ContentTypeIDs AS Ref))
			INNER JOIN [dbo].[Users_Normal] AS UN
			ON UN.ApplicationID = @ApplicationID AND UN.UserID = PS.SenderUserID AND UN.IsApproved = 1
		WHERE PS.ApplicationID = @ApplicationID AND 
			(@AllUsers = 1 OR PS.SenderUserID IN (SELECT U.[Value] FROM @UserIDs AS U)) AND PS.Deleted = 0 AND 
			(@LowerCreationDateLimit IS NULL OR PS.SendDate >= @LowerCreationDateLimit) AND
			(@UpperCreationDateLimit IS NULL OR PS.SendDate < @UpperCreationDateLimit)

		UNION ALL

		SELECT	C.SenderUserID AS UserID,
				NULL AS VisitedID,
				NULL AS SearchID,
				NULL AS QuestionID,
				NULL AS PostID,
				C.CommentID,
				C.SendDate AS [Date]
		FROM [dbo].[SH_Comments] AS C
			INNER JOIN [dbo].[SH_PostShares] AS PS
			ON PS.ApplicationID = @ApplicationID AND PS.ShareID = C.ShareID
			INNER JOIN [dbo].[CN_Nodes] AS ND
			ON ND.ApplicationID = @ApplicationID AND ND.NodeID = PS.OwnerID AND ND.Deleted = 0 AND
				(@ContentTypesCount = 0 OR ND.NodeTypeID IN (SELECT Ref.[Value] FROM @ContentTypeIDs AS Ref))
			INNER JOIN [dbo].[Users_Normal] AS UN
			ON UN.ApplicationID = @ApplicationID AND UN.UserID = C.SenderUserID AND UN.IsApproved = 1
		WHERE C.ApplicationID = @ApplicationID AND 
			(@AllUsers = 1 OR C.SenderUserID IN (SELECT U.[Value] FROM @UserIDs AS U)) AND PS.Deleted = 0 AND 
			(@LowerCreationDateLimit IS NULL OR C.SendDate >= @LowerCreationDateLimit) AND
			(@UpperCreationDateLimit IS NULL OR C.SendDate < @UpperCreationDateLimit)
	)

	RETURN
END

GO