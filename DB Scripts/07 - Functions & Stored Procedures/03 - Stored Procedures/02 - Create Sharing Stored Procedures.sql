USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- EXEC SH_AddPost ''

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[SH_AddPost]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SH_AddPost]
GO

CREATE PROCEDURE [dbo].[SH_AddPost]
	@ApplicationID		uniqueidentifier,
    @ShareID 			uniqueidentifier,
    @PostTypeID         int,
    @Description        nvarchar(4000),
    @SharedObjectID     uniqueidentifier,
    @SenderUserID       uniqueidentifier,
    @SendDate		    datetime,
    @OwnerID			uniqueidentifier,
    @OwnerType			varchar(20),
    @HasPicture			bit,
    @Privacy            varchar(20) = N'Public'
WITH ENCRYPTION
AS
BEGIN TRANSACTION
	SET NOCOUNT ON

	SET @Description = [dbo].[GFN_VerifyString](@Description)
	
	DECLARE @_PostID uniqueidentifier
	SET @_PostID = NEWID()

    INSERT INTO [dbo].[SH_Posts] (
		[ApplicationID],
        [PostID],
		[PostTypeID],
		[Description],
		[SharedObjectID],
		[SenderUserID],
		[SendDate],
		[HasPicture],
		[Deleted]
    )
    VALUES (
		@ApplicationID,
        @_PostID,
        @PostTypeID,
        @Description,
        @SharedObjectID,
        @SenderUserID,
        @SendDate,
        ISNULL(@HasPicture, 0),
        0
    )
    
    DECLARE @_firstCount int
    SET @_firstCount = @@rowcount
    
    INSERT INTO [dbo].[SH_PostShares] (
		[ApplicationID],
		[ShareID],
        [PostID],
		[OwnerID],
		[SenderUserID],
		[SendDate],
		[ScoreDate],
		[Privacy],
		[Deleted],
		[OwnerType]
    )
    VALUES (
		@ApplicationID,
		@ShareID,
        @_PostID,
        @OwnerID,
        @SenderUserID,
        @SendDate,
        @SendDate,
        @Privacy,
        0,
        @OwnerType
    )
    
    DECLARE @_secondCount int
    SET @_secondCount = @@rowcount
    
IF(@_firstCount <> @_secondCount) BEGIN
	SELECT -1
	ROLLBACK TRANSACTION
END
ELSE BEGIN
	SELECT @_secondCount
	COMMIT TRANSACTION
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[SH_UpdatePost]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SH_UpdatePost]
GO

CREATE PROCEDURE [dbo].[SH_UpdatePost]
	@ApplicationID			uniqueidentifier,
    @ShareID				uniqueidentifier,
	@Description			nvarchar(4000),
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SET @Description = [dbo].[GFN_VerifyString](@Description)
	
	-- Create a temp table TO store the select results
    CREATE TABLE #Temp
    (
        ShareID uniqueidentifier NOT NULL,
        ParentShareID uniqueidentifier NULL,
        PostID uniqueidentifier NOT NULL
    )
    
    INSERT INTO #Temp
    SELECT PS.ShareID, PS.ParentShareID, PS.PostID
    FROM [dbo].[SH_PostShares] AS PS
    WHERE PS.ApplicationID = @ApplicationID AND PS.ShareID = @ShareID
    
    DECLARE @_parentShareID uniqueidentifier
    DECLARE @_postID uniqueidentifier
    SET @_parentShareID = (SELECT #Temp.ParentShareID FROM #Temp)
    SET @_postID = (SELECT #Temp.PostID FROM #Temp)
    
    DROP TABLE #Temp
	
	IF (@_parentShareID IS NULL OR (@_parentShareID = @ShareID))
		UPDATE [dbo].[SH_Posts]
		SET [Description] = @Description,
			[LastModifierUserID] = @LastModifierUserID,
			[LastModificationDate] = @LastModificationDate
		WHERE ApplicationID = @ApplicationID AND PostID = @_postID
	ELSE
		UPDATE [dbo].[SH_PostShares]
		SET [Description] = @Description,
			[LastModifierUserID] = @LastModifierUserID,
			[LastModificationDate] = @LastModificationDate
		WHERE ApplicationID = @ApplicationID AND ShareID = @ShareID
	
	SELECT @@rowcount
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[SH_ArithmeticDeletePost]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SH_ArithmeticDeletePost]
GO

CREATE PROCEDURE [dbo].[SH_ArithmeticDeletePost]
	@ApplicationID		uniqueidentifier,
    @ShareID	 		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

    UPDATE [dbo].[SH_PostShares]
	SET [Deleted] = 1
	WHERE ApplicationID = @ApplicationID AND ShareID = @ShareID
	
	SELECT @@rowcount
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[SH_P_GetPostsByIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SH_P_GetPostsByIDs]
GO

CREATE PROCEDURE [dbo].[SH_P_GetPostsByIDs]
	@ApplicationID	uniqueidentifier,
    @ShareIDs		GuidTableType readonly,
    @UserID			uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT PS.[ShareID] AS PostID,
		   PS.[ParentShareID] AS RefPostID,
		   P.[PostTypeID] AS PostTypeID,
		   PS.[Description] AS [Description],
		   P.[Description] AS OriginalDescription,
		   P.[SharedObjectID] AS SharedObjectID,
		   PS.[SenderUserID] AS SenderUserID,
		   PS.[SendDate] AS SendDate,
		   [ShareSender].[FirstName] AS FirstName,
		   [ShareSender].[LastName] AS LastName,
		   [ShareSender].[JobTitle] AS JobTitle,
		   P.[SenderUserID] AS OriginalSenderUserID,
		   P.[SendDate] AS OriginalSendDate,
		   [PostSender].[FirstName] AS OriginalFirstName,
		   [PostSender].[LastName] AS OriginalLastName,
		   [PostSender].[JobTitle] AS OriginalJobTitle,
		   PS.[LastModificationDate] AS LastModificationDate,
		   PS.[OwnerID] AS OwnerID,
		   PS.[OwnerType] AS OwnerType,
		   PS.[Privacy] AS Privacy,
		   P.[HasPicture] AS HasPicture,
		   (SELECT COUNT(*) FROM [dbo].[SH_Comments] AS C
			WHERE C.ApplicationID = @ApplicationID AND C.[ShareID] = PS.[ShareID] AND
				C.[Deleted] = 0) AS CommentsCount,
		   (SELECT COUNT(*) FROM [dbo].[SH_ShareLikes] AS L
			WHERE L.ApplicationID = @ApplicationID AND L.[ShareID] = PS.[ShareID] AND
				L.[Like] = 1) AS LikesCount,
			(SELECT COUNT(*) FROM [dbo].[SH_ShareLikes] AS L
			WHERE L.ApplicationID = @ApplicationID AND L.[ShareID] = PS.[ShareID] AND
				L.[Like] = 0) AS DislikesCount,
			(SELECT L.[Like] FROM [dbo].[SH_ShareLikes] AS L
			WHERE L.ApplicationID = @ApplicationID AND L.[ShareID] = PS.[ShareID] AND
				L.[UserID] = @UserID) AS LikeStatus
	FROM @ShareIDs AS ExternalIDs
		INNER JOIN [dbo].[SH_PostShares] AS PS
		ON PS.ApplicationID = @ApplicationID AND PS.[ShareID] = ExternalIDs.Value 
		INNER JOIN [dbo].[SH_Posts] AS P
		ON P.ApplicationID = @ApplicationID AND P.[PostID] = PS.[PostID]
		INNER JOIN [dbo].[Users_Normal] AS ShareSender
		ON ShareSender.ApplicationID = @ApplicationID AND ShareSender.[UserID] = PS.[SenderUserID]
		INNER JOIN [dbo].[Users_Normal] AS PostSender
		ON PostSender.ApplicationID = @ApplicationID AND PostSender.[UserID] = P.[SenderUserID]
	WHERE PS.[Deleted] = 0 AND ShareSender.IsApproved = 1
	ORDER BY PS.[ScoreDate] DESC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[SH_GetPosts]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SH_GetPosts]
GO

CREATE PROCEDURE [dbo].[SH_GetPosts]
	@ApplicationID	uniqueidentifier,
    @OwnerID	 	uniqueidentifier,
    @UserID			uniqueidentifier,
    @News			bit,
    @MaxDate		datetime,
    @MinDate		datetime,
    @Count			int
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @ShareIDs GuidTableType
	
	IF @UserID IS NULL OR @UserID <> @OwnerID OR ISNULL(@News, 0) = 0 BEGIN
		INSERT INTO @ShareIDs
		SELECT TOP (@Count)	PS.[ShareID] 
		FROM [dbo].[SH_PostShares] AS PS
		WHERE PS.ApplicationID = @ApplicationID AND PS.OwnerID = @OwnerID AND 
			  (@MaxDate IS NULL OR PS.SendDate <= @MaxDate) AND 
			  (@MinDate IS NULL OR PS.SendDate >= @MinDate) AND 
			  PS.Deleted = 0
		ORDER BY PS.[ScoreDate] DESC
	END
	ELSE BEGIN
		DECLARE @TempIDs Table(Value uniqueidentifier, [Date] DateTime)
		
		/* Public Posts */
		INSERT INTO @TempIDs
		SELECT TOP (@Count) PS.[ShareID], PS.[ScoreDate]
		FROM [dbo].[SH_PostShares] AS PS
		WHERE PS.ApplicationID = @ApplicationID AND 
			PS.[OwnerType] = N'User' AND PS.[Privacy] = N'Public' AND 
			(@MaxDate IS NULL OR PS.[SendDate] <= @MaxDate) AND 
			(@MinDate IS NULL OR PS.[SendDate] >= @MinDate) AND 
			PS.[Deleted] = 0
		ORDER BY PS.[ScoreDate] DESC
		/* end of Public Posts */
		
		DECLARE @MinPostsDate datetime, @CurCount int
		
		SET @MinPostsDate = (SELECT MIN(Ref.[Date]) FROM @TempIDs AS Ref)
		SET @CurCount = (SELECT DISTINCT COUNT(*) FROM @TempIDs)
		
		/* User's Posts */
		IF @Count <= @CurCount BEGIN
			INSERT INTO @TempIDs
			SELECT TOP (@Count)	PS.[ShareID], PS.[ScoreDate]
			FROM [dbo].[SH_PostShares] AS PS
			WHERE PS.ApplicationID = @ApplicationID AND 
				(PS.OwnerID = @OwnerID OR PS.SenderUserID = @UserID) AND 
				PS.[OwnerType] = N'User' AND
				(@MaxDate IS NULL OR PS.SendDate <= @MaxDate) AND 
				(@MinDate IS NULL OR PS.SendDate >= @MinDate) AND 
				PS.SendDate >= @MinPostsDate AND PS.Deleted = 0
			ORDER BY PS.[ScoreDate] DESC
		END
		ELSE BEGIN
			INSERT INTO @TempIDs
			SELECT TOP (@Count)	PS.[ShareID], PS.[ScoreDate]
			FROM [dbo].[SH_PostShares] AS PS
			WHERE PS.ApplicationID = @ApplicationID AND
				(PS.OwnerID = @OwnerID OR PS.SenderUserID = @UserID) AND 
				PS.[OwnerType] = N'User' AND
				(@MaxDate IS NULL OR PS.SendDate <= @MaxDate) AND
				(@MinDate IS NULL OR PS.SendDate >= @MinDate) AND
				PS.Deleted = 0
			ORDER BY PS.[ScoreDate] DESC
		END
		/* end of User's Posts */
		
		SET @MinPostsDate = (SELECT MIN(Ref.Date) FROM @TempIDs AS Ref)
		SET @CurCount = (SELECT DISTINCT COUNT(*) FROM @TempIDs)
		
		/* User's Friend's Posts */
		DECLARE @FriendIDs Table(Value uniqueidentifier primary key clustered)
		INSERT INTO @FriendIDs
		SELECT Ref.UserID
		FROM [dbo].[USR_FN_GetFriendIDs](@ApplicationID, @UserID, 1, 1, 1) AS Ref
		
		IF @Count <= @CurCount BEGIN
			INSERT INTO @TempIDs
			SELECT TOP (@Count) PS.[ShareID], PS.[ScoreDate]
			FROM @FriendIDs AS Friends
				INNER JOIN [dbo].[SH_PostShares] AS PS
				ON (PS.[SenderUserID] = Friends.Value OR PS.[OwnerID] = Friends.Value)
			WHERE PS.ApplicationID = @ApplicationID AND PS.[OwnerType] = N'User' AND
				  PS.[Privacy] = N'Friends' AND 
				  (@MaxDate IS NULL OR PS.[SendDate] <= @MaxDate) AND 
				  (@MinDate IS NULL OR PS.[SendDate] >= @MinDate) AND 
				  PS.SendDate >= @MinPostsDate AND
				  PS.[Deleted] = 0
			ORDER BY PS.[ScoreDate] DESC
		END
		ELSE BEGIN
			INSERT INTO @TempIDs
			SELECT TOP (@Count) PS.[ShareID], PS.[ScoreDate]
			FROM @FriendIDs AS Friends
				INNER JOIN [dbo].[SH_PostShares] AS PS
				ON (PS.[SenderUserID] = Friends.Value OR PS.[OwnerID] = Friends.Value)
			WHERE PS.ApplicationID = @ApplicationID AND PS.[OwnerType] = N'User' AND
				  PS.[Privacy] = N'Friends' AND 
				  (@MaxDate IS NULL OR PS.[SendDate] <= @MaxDate) AND 
				  (@MinDate IS NULL OR PS.[SendDate] >= @MinDate) AND 
				  PS.[Deleted] = 0
			ORDER BY PS.[ScoreDate] DESC
		END
		/* end of User's Friend's Posts */
		
		INSERT INTO @ShareIDs
		SELECT TOP (@Count) Ref.Value FROM @TempIDs AS Ref
		GROUP BY Ref.Value
		ORDER BY min(Ref.[Date]) DESC
	END
	
	EXEC [dbo].[SH_P_GetPostsByIDs] @ApplicationID, @ShareIDs, @UserID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[SH_GetPostsByIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SH_GetPostsByIDs]
GO

CREATE PROCEDURE [dbo].[SH_GetPostsByIDs]
	@ApplicationID	uniqueidentifier,
    @strShareIDs 	varchar(8000),
    @delimiter		char,
    @UserID			uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @TempIDs GuidTableType
	
	INSERT INTO @TempIDs
	SELECT DISTINCT Ref.Value FROM GFN_StrToGuidTable(@strShareIDs, @delimiter) AS Ref
	
	EXEC [dbo].[SH_P_GetPostsByIDs] @ApplicationID, @TempIDs, @UserID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[SH_GetPostOwnerID]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SH_GetPostOwnerID]
GO

CREATE PROCEDURE [dbo].[SH_GetPostOwnerID]
	@ApplicationID		uniqueidentifier,
    @PostIDOrCommentID	uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF NOT EXISTS(
		SELECT TOP(1) ShareID
		FROM [dbo].[SH_PostShares]
		WHERE ApplicationID = @ApplicationID AND ShareID = @PostIDOrCommentID
	) BEGIN
		SELECT TOP(1) @PostIDOrCommentID = ShareID
		FROM [dbo].[SH_Comments]
		WHERE ApplicationID = @ApplicationID AND CommentID = @PostIDOrCommentID
	END
	
	SELECT OwnerID
	FROM [dbo].[SH_PostShares]
	WHERE ApplicationID = @ApplicationID AND ShareID = @PostIDOrCommentID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[SH_GetPostSenderID]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SH_GetPostSenderID]
GO

CREATE PROCEDURE [dbo].[SH_GetPostSenderID]
	@ApplicationID		uniqueidentifier,
    @PostIDOrCommentID	uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF NOT EXISTS(
		SELECT TOP(1) ShareID
		FROM [dbo].[SH_PostShares]
		WHERE ApplicationID = @ApplicationID AND ShareID = @PostIDOrCommentID
	) BEGIN
		SELECT TOP(1) @PostIDOrCommentID = ShareID
		FROM [dbo].[SH_Comments]
		WHERE ApplicationID = @ApplicationID AND CommentID = @PostIDOrCommentID
	END
	
	SELECT SenderUserID
	FROM [dbo].[SH_PostShares]
	WHERE ApplicationID = ApplicationID AND ShareID = @PostIDOrCommentID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[SH_Share]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SH_Share]
GO

CREATE PROCEDURE [dbo].[SH_Share]
	@ApplicationID	uniqueidentifier,
	@ShareID		uniqueidentifier,
	@ParentShareID	uniqueidentifier,
    @OwnerID		uniqueidentifier,
    @Description	nvarchar(4000),
    @SenderUserID	uniqueidentifier,
    @SendDate		datetime,
    @OwnerType		varchar(20),
    @Privacy		varchar(20) = N'Public'
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SET @Description = [dbo].[GFN_VerifyString](@Description)
	
	DECLARE @_PostID uniqueidentifier  = (
		SELECT TOP(1) PostID 
		FROM [dbo].[SH_PostShares]
		WHERE ApplicationID = @ApplicationID AND [ShareID] = @ParentShareID
	)

    INSERT INTO [dbo].[SH_PostShares] (
		[ApplicationID],
		[ShareID],
		[ParentShareID],
        [PostID],
		[OwnerID],
		[Description],
		[SenderUserID],
		[SendDate],
		[ScoreDate],
		[Privacy],
		[OwnerType],
		[Deleted]
    )
    VALUES (
		@ApplicationID,
		@ShareID,
		@ParentShareID,
        @_PostID,
        @OwnerID,
        @Description,
        @SenderUserID,
        @SendDate,
        @SendDate,
        @Privacy,
        @OwnerType,
        0
    )
    
    SELECT @@rowcount
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[SH_AddComment]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SH_AddComment]
GO

CREATE PROCEDURE [dbo].[SH_AddComment]
	@ApplicationID	uniqueidentifier,
	@CommentID		uniqueidentifier,
    @ShareID		uniqueidentifier,
	@Description	nvarchar(4000),
	@SenderUserID	uniqueidentifier,
	@SendDate		datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SET @Description = [dbo].[GFN_VerifyString](@Description)
	
    INSERT INTO [dbo].[SH_Comments] (
		[ApplicationID],
		[CommentID],
		[ShareID],
        [Description],
		[SenderUserID],
		[SendDate],
		[Deleted]
    )
    VALUES (
		@ApplicationID,
		@CommentID,
		@ShareID,
		@Description,
        @SenderUserID,
        @SendDate,
        0
    )
	
	SELECT @@rowcount
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[SH_UpdateComment]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SH_UpdateComment]
GO

CREATE PROCEDURE [dbo].[SH_UpdateComment]
	@ApplicationID			uniqueidentifier,
    @CommentID				uniqueidentifier,
	@Description			nvarchar(4000),
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SET @Description = [dbo].[GFN_VerifyString](@Description)
	
	UPDATE [dbo].[SH_Comments]
		SET [Description] = @Description,
			[LastModifierUserID] = @LastModifierUserID,
			[LastModificationDate] = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND CommentID = @CommentID
	
	SELECT @@rowcount
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[SH_ArithmeticDeleteComment]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SH_ArithmeticDeleteComment]
GO

CREATE PROCEDURE [dbo].[SH_ArithmeticDeleteComment]
	@ApplicationID	uniqueidentifier,
    @CommentID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

    UPDATE [dbo].[SH_Comments]
		SET [Deleted] = 1
	WHERE ApplicationID = @ApplicationID AND CommentID = @CommentID
	
    SELECT @@rowcount
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[SH_P_GetCommentsByIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SH_P_GetCommentsByIDs]
GO

CREATE PROCEDURE [dbo].[SH_P_GetCommentsByIDs]
	@ApplicationID	uniqueidentifier,
    @CommentIDs		GuidTableType readonly,
    @UserID	 		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
    SELECT C.[CommentID] AS CommentID,
		   C.[ShareID] AS PostID,
		   C.[Description] AS [Description],
		   C.[SenderUserID] AS SenderUserID,
		   C.[SendDate] AS SendDate,
		   UN.[FirstName] AS FirstName,
		   UN.[Lastname] AS LastName,
		   (
				SELECT COUNT(*) 
				FROM [dbo].[SH_CommentLikes] AS CL
				WHERE CL.ApplicationID = @ApplicationID AND 
					CL.[CommentID] = C.[CommentID] AND CL.[Like] = 1
			) AS LikesCount,
			(
				SELECT COUNT(*) 
				FROM [dbo].[SH_CommentLikes] AS CL
				WHERE CL.ApplicationID = @ApplicationID AND 
					CL.[CommentID] = C.[CommentID] AND CL.[Like] = 0
			) AS DislikesCount,
			(
				SELECT CL.[Like] 
				FROM [dbo].[SH_CommentLikes] AS CL
				WHERE CL.ApplicationID = @ApplicationID AND CL.[CommentID] = 
					C.[CommentID] AND CL.[UserID] = @UserID
			) AS LikeStatus
	FROM @CommentIDs AS ExternalIDs
		INNER JOIN [dbo].[SH_Comments] AS C
		ON C.ApplicationID = @ApplicationID AND C.[CommentID] = ExternalIDs.Value
		INNER JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND UN.[UserId] = C.[SenderUserID]
	ORDER BY C.SendDate
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[SH_GetCommentsByIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SH_GetCommentsByIDs]
GO

CREATE PROCEDURE [dbo].[SH_GetCommentsByIDs]
	@ApplicationID	uniqueidentifier,
    @strCommentIDs	varchar(max),
    @delimiter		char,
    @UserID	 		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @CommentIDs GuidTableType
	INSERT INTO @CommentIDs
	SELECT Ref.Value FROM [dbo].[GFN_StrToGuidTable](@strCommentIDs, @delimiter) AS Ref
	
	EXEC [dbo].[SH_P_GetCommentsByIDs] @ApplicationID, @CommentIDs, @UserID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[SH_GetComments]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SH_GetComments]
GO

CREATE PROCEDURE [dbo].[SH_GetComments]
	@ApplicationID	uniqueidentifier,
    @strShareIDs	varchar(max),
    @delimiter		char,
    @UserID	 		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @ShareIDs GuidTableType
	INSERT INTO @ShareIDs
	SELECT Ref.Value FROM [dbo].[GFN_StrToGuidTable](@strShareIDs, @delimiter) AS Ref
	
	DECLARE @CommentIDs GuidTableType
	
	INSERT INTO @CommentIDs
    SELECT C.[CommentID]
	FROM @ShareIDs AS ExternalIDs
		INNER JOIN [dbo].[SH_Comments] AS C
		ON C.[ShareID] = ExternalIDs.Value
	WHERE C.ApplicationID = @ApplicationID AND C.Deleted = 0
	
	EXEC [dbo].[SH_P_GetCommentsByIDs] @ApplicationID, @CommentIDs, @UserID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[SH_GetCommentSenderID]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SH_GetCommentSenderID]
GO

CREATE PROCEDURE [dbo].[SH_GetCommentSenderID]
	@ApplicationID	uniqueidentifier,
    @CommentID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT SenderUserID
	FROM [dbo].[SH_Comments]
	WHERE ApplicationID = @ApplicationID AND CommentID = @CommentID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[SH_GetCommentSenderIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SH_GetCommentSenderIDs]
GO

CREATE PROCEDURE [dbo].[SH_GetCommentSenderIDs]
	@ApplicationID	uniqueidentifier,
    @PostID			uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT SenderUserID AS ID
	FROM [dbo].[SH_Comments]
	WHERE ApplicationID = @ApplicationID AND ShareID = @PostID AND Deleted = 0
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[SH_LikeDislikePost]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SH_LikeDislikePost]
GO

CREATE PROCEDURE [dbo].[SH_LikeDislikePost]
	@ApplicationID	uniqueidentifier,
    @ShareID		uniqueidentifier,
    @UserID			uniqueidentifier,
    @Like			bit,
    @Score			float,
    @Date			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

	IF EXISTS (
		SELECT TOP(1) * 
		FROM [dbo].[SH_ShareLikes] 
		WHERE ApplicationID = @ApplicationID AND ShareID = @ShareID AND UserID = @UserID
	) BEGIN
		UPDATE [dbo].[SH_ShareLikes]
		SET [Like] = @Like,
			[Score] = ISNULL(@Score, 0),
			[Date] = @Date
		WHERE ApplicationID = @ApplicationID AND ShareID = @ShareID AND UserID = @UserID
	END
	ELSE BEGIN
		INSERT INTO [dbo].[SH_ShareLikes] (
			[ApplicationID],
			[ShareID],
			[UserID],
			[Like],
			[Score],
			[Date]
		)
		VALUES (
			@ApplicationID,
			@ShareID,
			@UserID,
			@Like,
			ISNULL(@Score, 0),
			@Date
		)
	END
	
    SELECT @@rowcount
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[SH_UnlikePost]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SH_UnlikePost]
GO

CREATE PROCEDURE [dbo].[SH_UnlikePost]
	@ApplicationID	uniqueidentifier,
    @ShareID		uniqueidentifier,
    @UserID			uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

	DELETE FROM [dbo].[SH_ShareLikes]
	WHERE ApplicationID = @ApplicationID AND [ShareID] = @ShareID AND [UserID] = @UserID
	
    SELECT @@rowcount
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[SH_GetPostFanIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SH_GetPostFanIDs]
GO

CREATE PROCEDURE [dbo].[SH_GetPostFanIDs]
	@ApplicationID	uniqueidentifier,
    @ShareID		uniqueidentifier,
    @LikeStatus		bit,
    @Count			int,
    @LowerBoundary	bigint
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF ISNULL(@Count, 0) <= 0 SET @Count = 1000000
	
	SELECT TOP(@Count)
		Ref.RowNumber AS [Order],
		(Ref.RowNumber + Ref.RevRowNumber - 1) AS TotalCount,
		Ref.UserID AS UserID
	FROM (
			SELECT	ROW_NUMBER() OVER (ORDER BY SL.[Date] DESC) AS RowNumber,
					ROW_NUMBER() OVER (ORDER BY SL.[Date] ASC) AS RevRowNumber,
					SL.UserID
			FROM [dbo].[SH_ShareLikes] AS SL
			WHERE SL.ApplicationID = @ApplicationID AND SL.ShareID = @ShareID AND 
				ISNULL(SL.[Like], 0) = ISNULL(@LikeStatus, 0)
		) AS Ref
	WHERE Ref.RowNumber >= ISNULL(@LowerBoundary, 0)
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[SH_GetCommentFanIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SH_GetCommentFanIDs]
GO

CREATE PROCEDURE [dbo].[SH_GetCommentFanIDs]
	@ApplicationID	uniqueidentifier,
    @CommentID		uniqueidentifier,
    @LikeStatus		bit,
    @Count			int,
    @LowerBoundary	bigint
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF ISNULL(@Count, 0) <= 0 SET @Count = 1000000
	
	SELECT TOP(@Count)
		Ref.RowNumber AS [Order],
		(Ref.RowNumber + Ref.RevRowNumber - 1) AS TotalCount,
		Ref.UserID AS UserID
	FROM (
			SELECT	ROW_NUMBER() OVER (ORDER BY CL.[Date] DESC) AS RowNumber,
					ROW_NUMBER() OVER (ORDER BY CL.[Date] ASC) AS RevRowNumber,
					CL.UserID
			FROM [dbo].[SH_CommentLikes] AS CL
			WHERE CL.ApplicationID = @ApplicationID AND CL.CommentID = @CommentID AND 
				ISNULL(CL.[Like], 0) = ISNULL(@LikeStatus, 0)
		) AS Ref
	WHERE Ref.RowNumber >= ISNULL(@LowerBoundary, 0)
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[SH_LikeDislikeComment]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SH_LikeDislikeComment]
GO

CREATE PROCEDURE [dbo].[SH_LikeDislikeComment]
	@ApplicationID	uniqueidentifier,
    @CommentID		uniqueidentifier,
    @UserID			uniqueidentifier,
    @Like			bit,
    @Score			float,
    @Date			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

	IF EXISTS (
		SELECT * 
		FROM [dbo].[SH_CommentLikes] 
		WHERE ApplicationID = @ApplicationID AND CommentID = @CommentID AND UserID = @UserID
	) 
	BEGIN
		UPDATE [dbo].[SH_CommentLikes]
		SET [Like] = @Like,
			[Score] = ISNULL(@Score, 0),
			[Date] = @Date
		WHERE ApplicationID = @ApplicationID AND CommentID = @CommentID AND UserID = @UserID
	END
	ELSE BEGIN
		INSERT INTO [dbo].[SH_CommentLikes] (
			[ApplicationID],
			[CommentID],
			[UserID],
			[Like],
			[Score],
			[Date]
		)
		VALUES (
			@ApplicationID,
			@CommentID,
			@UserID,
			@Like,
			ISNULL(@Score, 0),
			@Date
		)
	END
	
    SELECT @@rowcount
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[SH_UnlikeComment]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SH_UnlikeComment]
GO

CREATE PROCEDURE [dbo].[SH_UnlikeComment]
	@ApplicationID	uniqueidentifier,
    @CommentID		uniqueidentifier,
    @UserID			uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

	DELETE FROM [dbo].[SH_CommentLikes]
	WHERE ApplicationID = @ApplicationID AND [CommentID] = @CommentID AND [UserID] = @UserID
	
    SELECT @@rowcount
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[SH_GetPostsCount]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SH_GetPostsCount]
GO

CREATE PROCEDURE [dbo].[SH_GetPostsCount]
	@ApplicationID	uniqueidentifier,
    @OwnerID		uniqueidentifier,
    @SenderUserID	uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
    SELECT COUNT(*)
	FROM [dbo].[SH_PostShares]
	WHERE ApplicationID = @ApplicationID AND (@OwnerID IS NULL OR OwnerID = @OwnerID) AND 
		(@SenderUserID IS NULL OR SenderUserID = @SenderUserID) AND Deleted = 0
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[SH_GetSharesCount]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SH_GetSharesCount]
GO

CREATE PROCEDURE [dbo].[SH_GetSharesCount]
	@ApplicationID	uniqueidentifier,
    @ShareID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @_PostID uniqueidentifier
	
	SET @_PostID = (
		SELECT PostID 
		FROM [dbo].[SH_PostShares] 
		WHERE ApplicationID = @ApplicationID AND [ShareID] = @ShareID
	)
	
    SELECT COUNT(*)
	FROM [dbo].[SH_PostShares]
	WHERE ApplicationID = @ApplicationID AND PostID = @_PostID AND Deleted = 0
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[SH_GetCommentsCount]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SH_GetCommentsCount]
GO

CREATE PROCEDURE [dbo].[SH_GetCommentsCount]
	@ApplicationID	uniqueidentifier,
    @PostID			uniqueidentifier,
    @SenderUserID	uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
    SELECT COUNT(*)
	FROM [dbo].[SH_Comments]
	WHERE ApplicationID = @ApplicationID AND (@PostID IS NULL OR ShareID = @PostID) AND 
		(@SenderUserID IS NULL OR SenderUserID = @SenderUserID) AND Deleted = 0
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[SH_GetUserPostsCount]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SH_GetUserPostsCount]
GO

CREATE PROCEDURE [dbo].[SH_GetUserPostsCount]
	@ApplicationID	uniqueidentifier,
    @UserID			uniqueidentifier,
    @PostTypeID		int = 0
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF @PostTypeID = 0 
		SELECT COUNT(*)
		FROM [dbo].[SH_PostShares]
		WHERE ApplicationID = @ApplicationID AND SenderUserID = @UserID AND Deleted = 0
	ELSE
		SELECT COUNT(*)
		FROM [dbo].[SH_PostShares] AS PS
			INNER JOIN [dbo].[SH_Posts]  AS P
			ON P.ApplicationID = @ApplicationID AND P.[PostID] = PS.[PostID]
		WHERE PS.ApplicationID = @ApplicationID AND PS.[SenderUserID] = @UserID AND 
			PS.[Deleted] = 0 AND P.[PostTypeID] = @PostTypeID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[SH_GetPostLikesDislikesCount]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SH_GetPostLikesDislikesCount]
GO

CREATE PROCEDURE [dbo].[SH_GetPostLikesDislikesCount]
	@ApplicationID	uniqueidentifier,
    @ShareID		uniqueidentifier,
    @Like			bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
    SELECT COUNT(*)
	FROM [dbo].[SH_ShareLikes]
	WHERE ApplicationID = @ApplicationID AND [ShareID] = @ShareID AND [Like] = @Like
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[SH_GetCommentLikesDislikesCount]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SH_GetCommentLikesDislikesCount]
GO

CREATE PROCEDURE [dbo].[SH_GetCommentLikesDislikesCount]
	@ApplicationID	uniqueidentifier,
    @CommentID		uniqueidentifier,
    @Like			bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
    SELECT COUNT(*)
	FROM [dbo].[SH_CommentLikes]
	WHERE ApplicationID = @ApplicationID AND [CommentID] = @CommentID AND [Like] = @Like
END

GO

