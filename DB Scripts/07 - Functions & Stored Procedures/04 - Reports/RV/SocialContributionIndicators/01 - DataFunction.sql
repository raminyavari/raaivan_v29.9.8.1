USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RV_FN_SocialContributionIndicatorsReport]') 
    AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[RV_FN_SocialContributionIndicatorsReport]
GO

CREATE FUNCTION [dbo].[RV_FN_SocialContributionIndicatorsReport](
	@ApplicationID			uniqueidentifier,
	@CurrentUserID			uniqueidentifier,
	@UserIDsTemp			GuidTableType readonly,
	@AllUsers				bit,
	@LowerCreationDateLimit datetime,
	@UpperCreationDateLimit datetime
)
RETURNS @outputTable TABLE (
	UserID			uniqueidentifier,
	PostID			uniqueidentifier,
	CommentID		uniqueidentifier,
	PostSendDate	datetime,
	CommentSendDate	datetime
)
WITH ENCRYPTION
AS
BEGIN
	DECLARE @UserIDs GuidTableType

	INSERT INTO @UserIDs ([Value])
	SELECT Ref.[Value]
	FROM @UserIDsTemp AS Ref

	INSERT INTO @outputTable (UserID, PostID, CommentID, PostSendDate, CommentSendDate)
	(
		SELECT	PS.SenderUserID AS UserID,
				PS.ShareID AS PostID,
				NULL AS CommentID,
				PS.SendDate AS PostSendDate,
				NULL AS CommentSendDate
		FROM [dbo].[SH_PostShares] AS PS
			INNER JOIN [dbo].[Users_Normal] AS UN
			ON UN.ApplicationID = @ApplicationID AND UN.UserID = PS.SenderUserID AND UN.IsApproved = 1
		WHERE PS.ApplicationID = @ApplicationID AND 
			(@AllUsers = 1 OR PS.SenderUserID IN (SELECT U.[Value] FROM @UserIDs AS U)) AND PS.Deleted = 0 AND 
			(@LowerCreationDateLimit IS NULL OR PS.SendDate >= @LowerCreationDateLimit) AND
			(@UpperCreationDateLimit IS NULL OR PS.SendDate <= @UpperCreationDateLimit)

		UNION ALL

		SELECT	C.SenderUserID,
				NULL AS PostID,
				C.CommentID,
				NULL AS PostSendDate,
				C.SendDate AS CommentSendDate
		FROM [dbo].[SH_Comments] AS C
			INNER JOIN [dbo].[SH_PostShares] AS PS
			ON PS.ApplicationID = @ApplicationID AND PS.ShareID = C.ShareID
			INNER JOIN [dbo].[Users_Normal] AS UN
			ON UN.ApplicationID = @ApplicationID AND UN.UserID = C.SenderUserID AND UN.IsApproved = 1
		WHERE C.ApplicationID = @ApplicationID AND 
			(@AllUsers = 1 OR C.SenderUserID IN (SELECT U.[Value] FROM @UserIDs AS U)) AND 
			C.Deleted = 0 AND PS.Deleted = 0 AND 
			(@LowerCreationDateLimit IS NULL OR C.SendDate >= @LowerCreationDateLimit) AND
			(@UpperCreationDateLimit IS NULL OR C.SendDate <= @UpperCreationDateLimit)
	)

	RETURN
END

GO