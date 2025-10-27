USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[USR_FN_GetMutualFriendsCount]') 
            AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[USR_FN_GetMutualFriendsCount]
GO

CREATE FUNCTION [dbo].[USR_FN_GetMutualFriendsCount]
(
	@ApplicationID	uniqueidentifier,
	@UserID			uniqueidentifier,
	@OtherUserIDs	GuidTableType readonly
)
RETURNS
@outputTable TABLE
(
	UserID				uniqueidentifier,
    MutualFriendsCount	int
)
WITH ENCRYPTION
AS
BEGIN
	INSERT INTO @outputTable (UserID, MutualFriendsCount)
	SELECT O.Value AS UserID, COUNT(F.FriendID) AS MutualsCount
	FROM [dbo].[USR_View_Friends] AS F
		INNER JOIN @OtherUserIDs AS O
		INNER JOIN [dbo].[USR_View_Friends] AS F2
		ON F2.ApplicationId = @ApplicationID AND F2.UserID = O.Value
		ON F2.FriendID = F.FriendID
	WHERE F.ApplicationId = @ApplicationID AND F.UserID = @UserID AND 
		F.AreFriends = 1 AND F2.AreFriends = 1 AND 
		F2.FriendID <> F.UserID AND F.FriendID <> F2.UserID
	GROUP BY F.UserID, O.Value
   
    RETURN 
END

GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[USR_FN_GetFriendIDs]') 
            AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[USR_FN_GetFriendIDs]
GO

CREATE FUNCTION [dbo].[USR_FN_GetFriendIDs]
(
	@ApplicationID	uniqueidentifier,
	@UserID			uniqueidentifier,
	@AreFriends		bit,
    @Sent			bit,
    @Received		bit
)
RETURNS
@outputTable TABLE
(
	UserID		uniqueidentifier primary key clustered
)
WITH ENCRYPTION
AS
BEGIN
	INSERT INTO @outputTable (UserID)
	SELECT F.FriendID AS ID
	FROM [dbo].[USR_View_Friends] AS F
	WHERE F.ApplicationId = @ApplicationID AND F.UserID = @UserID AND 
		(@AreFriends IS NULL OR F.AreFriends = @AreFriends) AND
		((@Received = 1 AND F.IsSender = 0) OR (@Sent = 1 AND F.IsSender = 1))
   
    RETURN 
END

GO
