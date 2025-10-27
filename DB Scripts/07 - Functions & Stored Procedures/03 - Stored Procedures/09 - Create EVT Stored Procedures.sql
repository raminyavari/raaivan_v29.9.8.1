USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[EVT_CreateEvent]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[EVT_CreateEvent]
GO

CREATE PROCEDURE [dbo].[EVT_CreateEvent]
	@ApplicationID	uniqueidentifier,
    @EventID 		uniqueidentifier,
    @EventType		nvarchar(256),
    @OwnerID		uniqueidentifier,
    @Title			nvarchar(500),
    @Description	nvarchar(2000),
    @BeginDate		datetime,
    @FinishDate		datetime,
    @CreatorUserID	uniqueidentifier,
    @CreationDate	datetime,
    @strNodeIDs		varchar(8000),
    @strUserIDs		varchar(8000),
    @delimiter		char
WITH ENCRYPTION
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	SET @Title = [dbo].[GFN_VerifyString](@Title)
	SET @Description = [dbo].[GFN_VerifyString](@Description)
	
	DECLARE @NodeIDs GuidTableType, @UserIDs GuidTableType
	
	INSERT INTO @NodeIDs
	SELECT DISTINCT Ref.Value FROM GFN_StrToGuidTable(@strNodeIDs, @delimiter) AS Ref
	
	INSERT INTO @UserIDs
	SELECT DISTINCT Ref.Value FROM GFN_StrToGuidTable(@strUserIDs, @delimiter) AS Ref
	WHERE Ref.Value <> @CreatorUserID
	
	DECLARE @_NodesCount int, @_UsersCount int
	SET @_NodesCount = (SELECT COUNT(*) FROM @NodeIDs)
	SET @_UsersCount = (SELECT COUNT(*) FROM @UserIDs)
	
	INSERT INTO [dbo].[EVT_Events](
		ApplicationID,
		EventID,
		EventType,
		OwnerID,
		Title,
		[Description],
		BeginDate,
		FinishDate,
		CreatorUserID,
		CreationDate,
		Deleted
	)
	VALUES(
		@ApplicationID,
		@EventID,
		@EventType,
		@OwnerID,
		@Title,
		@Description,
		@BeginDate,
		@FinishDate,
		@CreatorUserID,
		@CreationDate,
		0
	)
	
	IF @@ROWCOUNT <= 0 BEGIN
		SELECT -1
		ROLLBACK TRANSACTION
		RETURN
	END
	
	IF @CreatorUserID IS NOT NULL BEGIN
		INSERT INTO [dbo].[EVT_RelatedUsers](
			ApplicationID,
			EventID,
			UserID,
			[Status],
			Done,
			Deleted
		)
		VALUES(
			@ApplicationID,
			@EventID,
			@CreatorUserID,
			N'Accept',
			0,
			0
		)
		
		IF @@ROWCOUNT <= 0 BEGIN
			SELECT -1
			ROLLBACK TRANSACTION
			RETURN
		END
	END
	
	IF @_UsersCount > 0 BEGIN
		INSERT INTO [dbo].[EVT_RelatedUsers](
			ApplicationID,
			EventID,
			UserID,
			[Status],
			Done,
			Deleted
		)
		SELECT @ApplicationID, @EventID, Ref.Value, N'Pending', 0, 0
		FROM @UserIDs AS Ref
		
		IF @@ROWCOUNT <= 0 BEGIN
			SELECT -1
			ROLLBACK TRANSACTION
			RETURN
		END
	END
	
	IF @_NodesCount > 0 BEGIN
		INSERT INTO [dbo].[EVT_RelatedNodes](
			ApplicationID,
			EventID,
			NodeID,
			Deleted
		)
		SELECT @ApplicationID, @EventID, Ref.Value, 0
		FROM @NodeIDs AS Ref
		
		IF @@ROWCOUNT <= 0 BEGIN
			SELECT -1
			ROLLBACK TRANSACTION
			RETURN
		END
	END
	
	SELECT (1 + @_NodesCount + @_UsersCount)
COMMIT TRANSACTION

GO



IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[EVT_ArithmeticDeleteEvent]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[EVT_ArithmeticDeleteEvent]
GO

CREATE PROCEDURE [dbo].[EVT_ArithmeticDeleteEvent]
	@ApplicationID	uniqueidentifier,
	@EventID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[EVT_Events]
		SET [Deleted] = 1
	WHERE ApplicationID = @ApplicationID AND EventID = @EventID
	
	SELECT @@ROWCOUNT
END

GO



IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[EVT_P_GetEventsByIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[EVT_P_GetEventsByIDs]
GO

CREATE PROCEDURE [dbo].[EVT_P_GetEventsByIDs]
	@ApplicationID	uniqueidentifier,
	@EventIDsTemp	GuidTableType readonly,
	@Full			bit
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @EventIDs GuidTableType
	INSERT INTO @EventIDs SELECT * FROM @EventIDsTemp
	
	IF @Full IS NULL OR @Full = 0 BEGIN
		SELECT E.[EventID] AS EventID,
			   E.[Title] AS Title
		FROM @EventIDs AS ExternalIDs
			INNER JOIN [dbo].[EVT_Events] AS E
			ON E.ApplicationID = @ApplicationID AND E.[EventID] = ExternalIDs.Value
	END
	ELSE BEGIN
		SELECT E.[EventID] AS EventID,
			   E.[EventType] AS EventType,
			   E.[Title] AS Title,
			   E.[Description] AS [Description],
			   E.[BeginDate] AS BeginDate,
			   E.[FinishDate] AS FinishDate,
			   E.[CreatorUserID] AS CreatorUserID
		FROM @EventIDs AS ExternalIDs
			INNER JOIN [dbo].[EVT_Events] AS E
			ON E.ApplicationID = @ApplicationID AND E.[EventID] = ExternalIDs.Value
	END
END

GO



IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[EVT_GetEventsByIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[EVT_GetEventsByIDs]
GO

CREATE PROCEDURE [dbo].[EVT_GetEventsByIDs]
	@ApplicationID	uniqueidentifier,
	@strEventIDs	varchar(8000),
	@delimiter		char,
	@Full			bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @EventIDs GuidTableType
	INSERT INTO @EventIDs 
	SELECT DISTINCT Ref.Value FROM GFN_StrToGuidTable(@strEventIDs, @delimiter) AS Ref
	
	EXEC [dbo].[EVT_P_GetEventsByIDs] @ApplicationID, @EventIDs, @Full
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[EVT_GetUserFinishedEventsCount]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[EVT_GetUserFinishedEventsCount]
GO

CREATE PROCEDURE [dbo].[EVT_GetUserFinishedEventsCount]
	@ApplicationID	uniqueidentifier,
	@UserID			uniqueidentifier,
	@CurrentDate	datetime,
	@Done			bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT COUNT(EV.[EventID])
	FROM [dbo].[EVT_RelatedUsers] AS RU
		INNER JOIN [dbo].[EVT_Events] AS EV
		ON EV.ApplicationID = @ApplicationID AND EV.[EventID] = RU.[EventID]
	WHERE RU.ApplicationID = @ApplicationID AND 
		RU.[UserID] = @UserID AND EV.[FinishDate] <= @CurrentDate AND
		(@Done IS NULL OR RU.[Done] = @Done) AND EV.[Deleted] = 0 AND RU.[Deleted] = 0
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[EVT_GetUserFinishedEvents]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[EVT_GetUserFinishedEvents]
GO

CREATE PROCEDURE [dbo].[EVT_GetUserFinishedEvents]
	@ApplicationID	uniqueidentifier,
	@UserID			uniqueidentifier,
	@CurrentDate	datetime,
	@Done			bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @EventIDs GuidTableType
	
	INSERT INTO @EventIDs
	SELECT E.[EventID]
	FROM [dbo].[EVT_RelatedUsers] AS RU
		INNER JOIN [dbo].[EVT_Events] AS E
		ON E.ApplicationID = @ApplicationID AND
			RU.[EventID] = E.[EventID]
	WHERE RU.ApplicationID = @ApplicationID AND 
		RU.[UserID] = @UserID AND E.[FinishDate] <= @CurrentDate AND
		(@Done IS NULL OR RU.[Done] = @Done) AND E.[Deleted] = 0 AND RU.[Deleted] = 0
		
	EXEC [dbo].[EVT_P_GetEventsByIDs] @ApplicationID, @EventIDs, 0
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[EVT_GetRelatedUserIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[EVT_GetRelatedUserIDs]
GO

CREATE PROCEDURE [dbo].[EVT_GetRelatedUserIDs]
	@ApplicationID	uniqueidentifier,
	@EventID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT RU.[UserID]
	FROM [dbo].[EVT_RelatedUsers] AS RU
	WHERE RU.ApplicationID = @ApplicationID AND 
		RU.[EventID] = @EventID AND RU.[Deleted] = 0
END

GO



IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[EVT_GetRelatedUsers]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[EVT_GetRelatedUsers]
GO

CREATE PROCEDURE [dbo].[EVT_GetRelatedUsers]
	@ApplicationID	uniqueidentifier,
	@EventID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT RU.[UserID] AS UserID,
		   RU.[EventID] AS EventID,
		   RU.[Status] AS Status,
		   RU.[Done] AS Done,
		   RU.[RealFinishDate] AS RealFinishDate,
		   UN.[UserName] AS UserName,
		   UN.[FirstName] AS FirstName,
		   UN.[LastName] AS LastName
	FROM [dbo].[EVT_RelatedUsers] AS RU
		INNER JOIN [dbo].[Users_Normal] AS UN 
		ON UN.ApplicationID = @ApplicationID AND UN.[UserID] = RU.[UserID]
	WHERE RU.ApplicationID = @ApplicationID AND
		RU.[EventID] = @EventID AND RU.[Deleted] = 0 AND UN.[IsApproved] = 1
END

GO



IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[EVT_ArithmeticDeleteRelatedUser]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[EVT_ArithmeticDeleteRelatedUser]
GO

CREATE PROCEDURE [dbo].[EVT_ArithmeticDeleteRelatedUser]
	@ApplicationID	uniqueidentifier,
	@EventID		uniqueidentifier,
	@UserID			uniqueidentifier
WITH ENCRYPTION
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	DECLARE @_IsOwn bit
	
	SET @_IsOwn = (
			SELECT CAST(1 AS bit) 
			FROM [dbo].[EVT_Events]
			WHERE ApplicationID = @ApplicationID AND 
				[EventID] = @EventID AND [CreatorUserID] = @UserID
		)
			
	IF @_IsOwn IS NULL SET @_IsOwn = 0
	
	UPDATE [dbo].[EVT_RelatedUsers]
		SET [Deleted] = 1
	WHERE ApplicationID = @ApplicationID AND EventID = @EventID AND UserID = @UserID
	
	IF @@ROWCOUNT <= 0 BEGIN
		SELECT -1
		ROLLBACK TRANSACTION
		RETURN
	END
	
	IF @_IsOwn = 1 BEGIN
		UPDATE [dbo].[EVT_Events]
			SET Deleted = 1
		WHERE ApplicationID = @ApplicationID AND [EventID] = @EventID
		
		IF @@ROWCOUNT <= 0 BEGIN
			SELECT -1
			ROLLBACK TRANSACTION
			RETURN
		END
		
		SELECT 2 /* 2: the event is deleted */
	END
	ELSE SELECT 1 /* 1: only the user has been deleted */
COMMIT TRANSACTION

GO



IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[EVT_ChangeUserStatus]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[EVT_ChangeUserStatus]
GO

CREATE PROCEDURE [dbo].[EVT_ChangeUserStatus]
	@ApplicationID	uniqueidentifier,
	@EventID		uniqueidentifier,
	@UserID			uniqueidentifier,
	@NewStatus		varchar(20)
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[EVT_RelatedUsers]
		SET Status = @NewStatus
	WHERE ApplicationID = @ApplicationID AND EventID = @EventID AND UserID = @UserID
	
	SELECT @@ROWCOUNT
END

GO



IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[EVT_GetRelatedNodeIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[EVT_GetRelatedNodeIDs]
GO

CREATE PROCEDURE [dbo].[EVT_GetRelatedNodeIDs]
	@ApplicationID			uniqueidentifier,
	@EventID				uniqueidentifier,
	@NodeTypeAdditionalID	varchar(20)
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT ND.[NodeID] AS ID
	FROM [dbo].[EVT_RelatedNodes] AS RN
		INNER JOIN [dbo].[CN_View_Nodes_Normal] AS ND
		ON ND.ApplicationID = @ApplicationID AND ND.[NodeID] = RN.[NodeID]
	WHERE RN.ApplicationID = @ApplicationID AND RN.[EventID] = @EventID AND 
		(@NodeTypeAdditionalID IS NULL OR  ND.[TypeAdditionalID] = @NodeTypeAdditionalID) AND
		RN.[Deleted] = 0 AND ND.[Deleted] = 0
END

GO



IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[EVT_GetNodeRelatedEvents]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[EVT_GetNodeRelatedEvents]
GO

CREATE PROCEDURE [dbo].[EVT_GetNodeRelatedEvents]
	@ApplicationID	uniqueidentifier,
	@NodeID			uniqueidentifier,
	@CurrentDate	datetime,
	@NotFinished	bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @EventIDs GuidTableType
	
	INSERT INTO @EventIDs
	SELECT E.[EventID]
	FROM [dbo].[EVT_RelatedNodes] AS RN
		INNER JOIN [dbo].[EVT_Events] AS E
		ON E.ApplicationID = @ApplicationID AND E.[EventID] = RN.[EventID]
	WHERE RN.ApplicationID = @ApplicationID AND RN.[NodeID] = @NodeID AND 
		(@CurrentDate IS NULL OR E.[BeginDate] >= @CurrentDate) AND
		(@NotFinished IS NULL OR @NotFinished = 0 OR 
		E.[BeginDate] <= @CurrentDate) AND E.[Deleted] = 0 AND RN.[Deleted] = 0
		
	EXEC [dbo].[EVT_P_GetEventsByIDs] @ApplicationID, @EventIDs, 0
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[EVT_GetUserRelatedEvents]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[EVT_GetUserRelatedEvents]
GO

CREATE PROCEDURE [dbo].[EVT_GetUserRelatedEvents]
	@ApplicationID	uniqueidentifier,
	@UserID			uniqueidentifier,
	@CurrentDate	datetime,
	@NotFinished	bit,
	@Status			varchar(20),
	@NodeID			uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT EV.[EventID] AS EventID,
		   RU.UserID AS UserID,
		   EV.[EventType] AS EventType,
		   EV.[Title] AS Title,
		   EV.[Description] AS [Description],
		   EV.[BeginDate] AS BeginDate,
		   EV.[FinishDate] AS FinishDate,
		   EV.[CreatorUserID] AS CreatorUserID,
		   RU.[Status] AS [Status],
		   RU.Done AS Done,
		   RU.RealFinishDate AS RealFinishDate
	FROM [dbo].[EVT_RelatedUsers] AS RU 
		INNER JOIN [dbo].[EVT_Events] AS EV 
		ON EV.ApplicationID = @ApplicationID AND EV.[EventID] = RU.[EventID]
	WHERE RU.ApplicationID = @ApplicationID AND RU.[UserID] = @UserID AND 
		(@CurrentDate IS NULL OR EV.[BeginDate] >= @CurrentDate) AND
		(@NotFinished IS NULL OR @NotFinished = 0 OR EV.[BeginDate] <= @CurrentDate) AND
		(@Status IS NULL OR RU.[Status] = @Status) AND
		EV.[Deleted] = 0 AND RU.Deleted = 0 AND
		(@NodeID IS NULL OR (
			EXISTS(
					SELECT TOP(1) * 
					FROM [dbo].[EVT_RelatedNodes] AS RN2
						INNER JOIN [dbo].[EVT_Events] AS E2
						ON E2.EventID = RN2.[EventID]
					WHERE RN2.ApplicationID = @ApplicationID AND RN2.[NodeID] = @NodeID
				)
			)
		)
END

GO