USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[MSG_GetThreads]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[MSG_GetThreads]
GO

CREATE PROCEDURE [dbo].[MSG_GetThreads]
	@ApplicationID	uniqueidentifier,
    @UserID			UNIQUEIDENTIFIER,
    @Count			INT,
    @LastID			INT
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT TOP(ISNULL(@Count, 10))
		D.ThreadID, 
		UN.UserName, 
		UN.FirstName, 
		UN.LastName,
		CAST((CASE WHEN UN.UserID IS NULL THEN 1 ELSE 0 END) AS bit) AS IsGroup,
		D.MessagesCount,
		D.SentCount,
		D.NotSeenCount,
		D.RowNumber
	FROM (
			SELECT ROW_NUMBER() OVER (ORDER BY Ref.MaxID DESC) AS RowNumber, Ref.*
			FROM (
					SELECT MD.ThreadID, MAX(MD.ID) AS MaxID,
						COUNT(MD.ID) AS MessagesCount, 
						SUM(CAST(MD.IsSender AS int)) AS SentCount,
						SUM(
							CAST((CASE WHEN MD.IsSender = 0 AND MD.Seen = 0 THEN 1 ELSE 0 END) AS int)
						) AS NotSeenCount
					FROM [dbo].[MSG_MessageDetails] AS MD
					WHERE MD.ApplicationID = @ApplicationID AND 
						MD.UserID = @UserID AND MD.Deleted = 0
					GROUP BY MD.ThreadID
				) AS Ref
		) AS D
		LEFT JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND UN.UserID = D.ThreadID
	WHERE (@LastID IS NULL OR D.RowNumber > @LastID)
	ORDER BY D.RowNumber ASC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[MSG_GetThreadInfo]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[MSG_GetThreadInfo]
GO

CREATE PROCEDURE [dbo].[MSG_GetThreadInfo]
	@ApplicationID	uniqueidentifier,
    @UserID			UNIQUEIDENTIFIER,
    @ThreadID		UNIQUEIDENTIFIER
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT	COUNT(MD.ID) AS MessagesCount, 
			SUM(CAST(MD.IsSender AS int)) AS SentCount,
			SUM(
				CAST((CASE WHEN MD.IsSender = 0 AND MD.Seen = 0 THEN 1 ELSE 0 END) AS int)
			) AS NotSeenCount
	FROM [dbo].[MSG_MessageDetails] AS MD
	WHERE MD.ApplicationID = @ApplicationID AND 
		MD.UserID = @UserID AND MD.ThreadID = @ThreadID AND MD.Deleted = 0
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[MSG_GetMessages]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[MSG_GetMessages]
GO

CREATE PROCEDURE [dbo].[MSG_GetMessages]
	@ApplicationID	uniqueidentifier,
    @UserID			UNIQUEIDENTIFIER,
    @ThreadID		UNIQUEIDENTIFIER,
    @Sent			BIT,
    @Count			INT,
    @MinID			BIGINT
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	--@Sent IS NULL --> Sent and Received Messages
	--@Sent IS NOT NULL --> @Sent = 1 --> Sent Messages
	--                  --> @Sent = 0 --> Received Messages
	
	SELECT 
		M.MessageID,
		M.Title,
		M.MessageText,
		M.SendDate,
		M.SenderUserID,
		M.ForwardedFrom,
		D.ID,
		D.IsGroup,
		D.IsSender,
		D.Seen,
		D.ThreadID,
		UN.UserName,
		UN.FirstName,
		UN.LastName,
		M.HasAttachment
	FROM (
			SELECT TOP(ISNULL(@Count, 20)) *
			FROM [dbo].[MSG_MessageDetails] AS MD
			WHERE MD.ApplicationID = @ApplicationID AND
				(@MinID IS NULL OR  MD.ID < @MinID) AND 
				MD.UserID = @UserID AND
				(MD.ThreadID IS NULL OR ThreadID = @ThreadID) AND 
				(@Sent IS NULL OR IsSender = @Sent) AND 
				MD.Deleted = 0
			ORDER BY MD.ID DESC
		)AS D
		INNER JOIN [dbo].[MSG_Messages] AS M
		ON M.ApplicationID = @ApplicationID AND M.MessageID = D.MessageID
		INNER JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND UN.UserID = M.SenderUserID
	ORDER BY D.ID ASC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[MSG_HasMessage]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[MSG_HasMessage]
GO

CREATE PROCEDURE [dbo].[MSG_HasMessage]
	@ApplicationID	uniqueidentifier,
	@ID				BIGINT,
    @UserID			UNIQUEIDENTIFIER,
    @ThreadID		UNIQUEIDENTIFIER,
    @MessageID		UNIQUEIDENTIFIER
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT
		CASE
			WHEN EXISTS(
				SELECT TOP(1) ID
				FROM [dbo].[MSG_MessageDetails]
				WHERE ApplicationID = @ApplicationID AND (@ID IS NULL OR ID = @ID) AND
					UserID = @UserID AND 
					(@ThreadID IS NULL OR ThreadID = @ThreadID) AND
					(@MessageID IS NULL OR MessageID = @MessageID)
			) THEN 1
			ELSE 0
		END
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[MSG_SendNewMessage]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[MSG_SendNewMessage]
GO

CREATE PROCEDURE [dbo].[MSG_SendNewMessage]
	@ApplicationID		uniqueidentifier,
    @UserID				UNIQUEIDENTIFIER,
    @ThreadID			UNIQUEIDENTIFIER,
    @MessageID			UNIQUEIDENTIFIER,
    @ForwardedFrom		UNIQUEIDENTIFIER,
    @Title				NVARCHAR(500),
    @MessageText		NVARCHAR(MAX),
    @IsGroup			BIT,
    @Now				DATETIME,
    @ReceiversTemp		GuidTableType readonly,
    @AttachedFilesTemp	DocFileInfoTableType readonly
WITH ENCRYPTION, RECOMPILE
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	DECLARE @Receivers GuidTableType
	INSERT INTO @Receivers SELECT * FROM @ReceiversTemp
    
    DECLARE @AttachedFiles DocFileInfoTableType
    INSERT INTO @AttachedFiles SELECT * FROM @AttachedFilesTemp
	
	IF @IsGroup IS NULL SET @IsGroup = 0
	
	IF @ThreadID IS NOT NULL BEGIN
		SET @IsGroup = ISNULL(
			(
				SELECT TOP(1) MD.IsGroup
				FROM [dbo].[MSG_MessageDetails] AS MD
				WHERE MD.ApplicationID = @ApplicationID AND MD.ThreadID = @ThreadID
			), @IsGroup
		)
	END
	
	DECLARE @ReceiverUserIDs GuidTableType
	
	INSERT INTO @ReceiverUserIDs SELECT * FROM @Receivers
	
	DECLARE @Count int = (SELECT COUNT(*) FROM @ReceiverUserIDs)
	
	IF @Count = 1 SET @IsGroup = 0
	
	IF(@Count > 1) DELETE FROM @ReceiverUserIDs WHERE Value = @UserID --Farzane Added
	
	IF (@ThreadID IS NULL AND @IsGroup = 0) AND @Count = 0 BEGIN
		SELECT -1
		RETURN
	END
	
	IF @ThreadID IS NOT NULL AND @Count = 0 AND 
		EXISTS(
			SELECT TOP(1) UserID 
			FROM [dbo].[Users_Normal] 
			WHERE ApplicationID = @ApplicationID AND UserID = @ThreadID
	) BEGIN
		INSERT INTO @ReceiverUserIDs (Value)
		VALUES (@ThreadID)
		
		SET @Count = 1
	END
	
	IF @IsGroup = 1 BEGIN
		IF @Count = 1 SET @ThreadID = (SELECT TOP(1) Ref.Value FROM @ReceiverUserIDs AS Ref)
		ELSE IF (@ThreadID IS NULL AND @Count > 0) SET @ThreadID = NEWID()
	END
	
	IF @Count = 0 BEGIN
		INSERT INTO @ReceiverUserIDs
		SELECT DISTINCT MD.UserID 
		FROM [dbo].[MSG_MessageDetails] AS MD
		WHERE MD.ApplicationID = @ApplicationID AND MD.ThreadID = @ThreadID
		EXCEPT (SELECT @UserID)
		
		SET @Count = (SELECT COUNT(*) FROM @ReceiverUserIDs)
	END
	
	DECLARE @AttachmentsCount int = (SELECT COUNT(*) FROM @AttachedFiles)
	
	INSERT INTO [dbo].[MSG_Messages](
		ApplicationID,
		MessageID,
		Title,
		MessageText,
		SenderUserID,
		SendDate,
		ForwardedFrom,
		HasAttachment
	)
	VALUES(
		@ApplicationID,
		@MessageID,
		@Title,
		@MessageText,
		@UserID,
		@Now,
		@ForwardedFrom,
		CASE WHEN @AttachmentsCount > 0 THEN 1 ELSE 0 END
	)
	
	IF @@ROWCOUNT <= 0 BEGIN
		SELECT -1
		ROLLBACK TRANSACTION
		RETURN
	END
	
	DECLARE @_Result int
	
	IF @AttachmentsCount > 0 BEGIN
		EXEC [dbo].[DCT_P_AddFiles] @ApplicationID, @MessageID, 
			N'Message', @AttachedFiles, @UserID, @Now, @_Result output
		
		IF @_Result <= 0 BEGIN
			SELECT -1
			ROLLBACK TRANSACTION
			RETURN
		END
	END
	
	IF @ForwardedFrom IS NOT NULL BEGIN
		EXEC [dbo].[DCT_P_CopyAttachments] @ApplicationID, @ForwardedFrom, 
			@MessageID, N'Message', @UserID, @Now, @_Result output
		
		IF @_Result > 0 BEGIN
			UPDATE [dbo].[MSG_Messages]
				SET HasAttachment = 1
			WHERE ApplicationID = @ApplicationID AND MessageID = @MessageID
		END 
	END
	
	INSERT INTO [dbo].[MSG_MessageDetails](
		ApplicationID,
		UserID,
		ThreadID,
		MessageID,
		Seen,
		IsSender,
		IsGroup,
		Deleted
	)
	(
		SELECT	TOP(CASE WHEN @IsGroup = 1 THEN 1 ELSE 1000000000 END)
				@ApplicationID,
				@UserID,
				CASE WHEN @IsGroup = 0 THEN R.Value ELSE @ThreadID END,
				@MessageID,
				1,
				1,
				@IsGroup,
				0
		FROM @ReceiverUserIDs AS R
		
		UNION ALL
		
		SELECT	@ApplicationID,
				R.Value,
				CASE WHEN @IsGroup = 0 THEN @UserID ELSE @ThreadID END,
				@MessageID,
				0,
				0,
				@IsGroup,
				0
		FROM @ReceiverUserIDs AS R
	)
	
	IF @@ROWCOUNT <= 0 BEGIN
		SELECT -1
		ROLLBACK TRANSACTION
		RETURN
	END
	
	SELECT @@IDENTITY - @Count
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[MSG_BulkSendMessage]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[MSG_BulkSendMessage]
GO

CREATE PROCEDURE [dbo].[MSG_BulkSendMessage]
	@ApplicationID	uniqueidentifier,
    @MessagesTemp	MessageTableType readonly,
    @ReceiversTemp	GuidPairTableType readonly,
    @Now			DATETIME
WITH ENCRYPTION, RECOMPILE
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	DECLARE @Messages MessageTableType
	INSERT INTO @Messages SELECT * FROM @MessagesTemp
    
    DECLARE @Receivers GuidPairTableType
    INSERT INTO @Receivers SELECT * FROM @ReceiversTemp
	
	INSERT INTO [dbo].[MSG_Messages](
		ApplicationID,
		MessageID,
		Title,
		MessageText,
		SenderUserID,
		SendDate,
		HasAttachment
	)
	SELECT	@ApplicationID,
			M.MessageID,
			M.Title,
			M.MessageText,
			M.SenderUserID,
			@Now,
			0
	FROM @Messages AS M
	WHERE M.MessageID IN (SELECT DISTINCT R.FirstValue FROM @Receivers AS R)
	
	IF @@ROWCOUNT <= 0 BEGIN
		SELECT -1
		ROLLBACK TRANSACTION
		RETURN
	END
	
	INSERT INTO [dbo].[MSG_MessageDetails](
		ApplicationID,
		UserID,
		ThreadID,
		MessageID,
		Seen,
		IsSender,
		IsGroup,
		Deleted
	)
	SELECT *
	FROM (
			SELECT	@ApplicationID AS ApplicationID,
					M.SenderUserID,
					R.SecondValue,
					M.MessageID,
					1 AS Seen,
					1 AS IsSender,
					0 AS IsGroup,
					0 AS Deleted
			FROM @Messages AS M
				INNER JOIN @Receivers AS R
				ON R.FirstValue = M.MessageID
			
			UNION ALL
			
			SELECT	@ApplicationID,
					R.SecondValue,
					M.SenderUserID,
					M.MessageID,
					0,
					0,
					0,
					0
			FROM @Messages AS M
				INNER JOIN @Receivers AS R
				ON R.FirstValue = M.MessageID
		) AS Ref
	ORDER BY Ref.IsSender DESC
	
	IF @@ROWCOUNT <= 0 BEGIN
		SELECT -1
		ROLLBACK TRANSACTION
		RETURN
	END
	
	SELECT @@IDENTITY
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[MSG_GetThreadUsers]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[MSG_GetThreadUsers]
GO

CREATE PROCEDURE [dbo].[MSG_GetThreadUsers]
	@ApplicationID	uniqueidentifier,
	@UserID			UNIQUEIDENTIFIER,
    @strThreadIDs	VARCHAR(MAX),
    @delimiter		CHAR,
	@Count			INT,
	@LastID			INT
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @ThreadIDs GuidTableType

	INSERT INTO @ThreadIDs
	SELECT Ref.Value FROM [dbo].[GFN_StrToGuidTable](@strThreadIDs, @delimiter) AS Ref
	
	DECLARE @MessageIDs GuidPairTableType

	;WITH X AS (
		SELECT MD.ThreadID, MIN(MD.ID) AS MinID
		FROM @ThreadIDs AS T
			INNER JOIN [dbo].[MSG_MessageDetails] AS MD
			ON MD.ApplicationID = @ApplicationID AND MD.ThreadID = T.Value
		GROUP BY MD.ThreadID
	)
	INSERT INTO @MessageIDs(FirstValue, SecondValue)
	SELECT MD.ThreadID, MD.MessageID
	FROM X
		INNER JOIN [dbo].[MSG_MessageDetails] AS MD
		ON MD.ApplicationID = @ApplicationID AND MD.ID = X.MinID

	;WITH Y AS (
		SELECT *
		FROM (
				SELECT  ROW_NUMBER() OVER (PARTITION BY MD.ThreadID ORDER BY MD.ID DESC) AS RowNumber, 
						ROW_NUMBER() OVER (PARTITION BY MD.ThreadID ORDER BY MD.ID ASC) AS RevRowNumber,
						MD.ThreadID, MD.UserID
				FROM @MessageIDs AS M
					INNER JOIN [dbo].[MSG_MessageDetails] AS MD
					ON MD.ThreadID = M.FirstValue AND MD.MessageID = M.SecondValue
				WHERE MD.ApplicationID = @ApplicationID AND MD.UserID NOT IN (SELECT @UserID)
			) AS Ref
		WHERE Ref.RowNumber > ISNULL(@LastID, 0) AND Ref.RowNumber <= (ISNULL(@LastID, 0) + ISNULL(@Count, 3))
	)
	SELECT	Y.ThreadID, 
			Y.UserID, 
			UN.UserName, 
			UN.FirstName, 
			UN.LastName,
			Y.RevRowNumber
	FROM Y
		INNER JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND UN.UserID = Y.UserID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[MSG_RemoveMessages]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[MSG_RemoveMessages]
GO

CREATE PROCEDURE [dbo].[MSG_RemoveMessages]
	@ApplicationID	uniqueidentifier,
    @UserID			UNIQUEIDENTIFIER,
    @ThreadID		UNIQUEIDENTIFIER,
    @ID				BIGINT
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

	UPDATE [dbo].[MSG_MessageDetails]
		SET Deleted = 1
	WHERE ApplicationID = @ApplicationID AND (@ID IS NOT NULL AND ID = @ID) OR  
		(@ID IS NULL AND UserID = @UserID AND ThreadID = @ThreadID)
		
		
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[MSG_SetMessagesAsSeen]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[MSG_SetMessagesAsSeen]
GO

CREATE PROCEDURE [dbo].[MSG_SetMessagesAsSeen]
	@ApplicationID	uniqueidentifier,
    @UserID			uniqueidentifier,
    @ThreadID		uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

	IF @UserID IS NOT NULL AND @ThreadID IS NOT NULL BEGIN
		UPDATE MD
			SET Seen = 1,
				ViewDate = ISNULL(ViewDate, @Now)
		FROM [dbo].[MSG_MessageDetails] AS MD
		WHERE MD.ApplicationID = @ApplicationID AND
			MD.UserID = @UserID AND MD.ThreadID = @ThreadID AND ViewDate IS NULL
		
		SELECT 1
	END
	ELSE SELECT 0
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[MSG_GetNotSeenMessagesCount]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[MSG_GetNotSeenMessagesCount]
GO

CREATE PROCEDURE [dbo].[MSG_GetNotSeenMessagesCount]
	@ApplicationID	uniqueidentifier,
    @UserID			uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

	SELECT COUNT(MD.ID)
	FROM [dbo].[MSG_MessageDetails] AS MD
	WHERE MD.ApplicationID = @ApplicationID AND
		MD.UserID = @UserID AND MD.IsSender = 0 AND MD.Seen = 0 AND MD.Deleted = 0
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[MSG_GetMessageReceivers]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[MSG_GetMessageReceivers]
GO

CREATE PROCEDURE [dbo].[MSG_GetMessageReceivers]
	@ApplicationID	uniqueidentifier,
    @strMessageIDs	NVARCHAR(MAX),
    @delimiter		CHAR,
	@Count			INT,
	@LastID			INT
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @MessageIDs GuidTableType

	INSERT INTO @MessageIDs
	SELECT Ref.Value FROM [dbo].[GFN_StrToGuidTable](@strMessageIDs, @delimiter) AS Ref
	
	;WITH Y AS (
		SELECT *
		FROM (
				SELECT  ROW_NUMBER() OVER (PARTITION BY MD.MessageID ORDER BY MD.ID DESC) AS RowNumber, 
						ROW_NUMBER() OVER (PARTITION BY MD.MessageID ORDER BY MD.ID ASC) AS RevRowNumber,
						MD.MessageID, MD.UserID
				FROM @MessageIDs AS R
					INNER JOIN [dbo].[MSG_MessageDetails] AS MD
					ON MD.MessageID = R.Value
				WHERE MD.ApplicationID = @ApplicationID AND MD.IsSender = 0
			) AS Ref
		WHERE Ref.RowNumber > ISNULL(@LastID, 0) AND Ref.RowNumber <= (ISNULL(@LastID, 0) + ISNULL(@Count, 3))
	)
	SELECT	Y.MessageID, 
			Y.UserID, 
			UN.UserName, 
			UN.FirstName, 
			UN.LastName,
			Y.RevRowNumber
	FROM Y
		INNER JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND UN.UserID = Y.UserID
END

Go


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[MSG_GetForwardedMessages]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[MSG_GetForwardedMessages]
GO

CREATE PROCEDURE [dbo].[MSG_GetForwardedMessages]
	@ApplicationID	uniqueidentifier,
	@MessageID		UNIQUEIDENTIFIER
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @hierarchyMessages AS TABLE (
		MessageID UNIQUEIDENTIFIER,
		IsGroup BIT,
		ForwardedFrom UNIQUEIDENTIFIER,
		[Level] INT
	)
	
	;WITH hierarchy (MessageID, ForwardedFrom, [Level])
	AS
	(
		SELECT m.MessageID AS MessageID, ForwardedFrom, 0 AS [Level]
		FROM [dbo].[MSG_Messages] as m
		WHERE m.ApplicationID = @ApplicationID AND MessageID = @MessageID
		
		UNION ALL
		
		SELECT m.MessageID AS MessageID, m.ForwardedFrom , [Level] + 1
		FROM [dbo].[MSG_Messages] AS m
			INNER JOIN hierarchy AS HR
			ON m.MessageID = HR.ForwardedFrom
		WHERE m.ApplicationID = @ApplicationID AND m.MessageID <> HR.MessageID
	)
	INSERT INTO @hierarchyMessages(
		MessageID, 
		IsGroup, 
		ForwardedFrom, 
		[Level]
	)
	SELECT 
		Ref.MessageID AS MessageID, 
		MD.IsGroup, 
		Ref.ForwardedFrom,
		Ref.[Level]
	FROM (
			SELECT hm.MessageID, hm.ForwardedFrom, hm.[Level] , MAX(MD.ID) AS ID
			FROM hierarchy AS hm
				INNER JOIN [dbo].[MSG_MessageDetails] AS MD
				ON MD.ApplicationID = @ApplicationID AND MD.MessageID = hm.MessageID
			GROUP BY hm.MessageID, hm.ForwardedFrom, hm.[Level]
		) AS Ref
		INNER JOIN [dbo].[MSG_MessageDetails] AS MD
		ON MD.ApplicationID = @ApplicationID AND MD.ID = Ref.ID
	
	SELECT 
		M.MessageID,
		M.MessageText,
		M.Title,
		M.SendDate,
		M.HasAttachment,
		H.ForwardedFrom,
		H.[Level],
		H.IsGroup,
		M.SenderUserID,
		UN.UserName AS SenderUserName,
		UN.FirstName AS SenderFirstName,
		UN.LastName AS SenderLastName
	FROM @hierarchyMessages AS H
		INNER JOIN [dbo].[MSG_Messages] AS M
		ON M.ApplicationID = @ApplicationID AND M.MessageID = H.MessageID
		INNER JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND UN.UserID = M.SenderUserID
	ORDER BY H.[Level] ASC
END

GO

