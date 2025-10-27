USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_P_SendDashboards]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_P_SendDashboards]
GO

CREATE PROCEDURE [dbo].[WK_P_SendDashboards]
	@ApplicationID		uniqueidentifier,
	@RefItemID			uniqueidentifier,
	@NodeID				uniqueidentifier,
	@AdminUserIDsTemp	GuidTableType readonly,
	@SendDate			datetime,
	@_Result			int output
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @AdminUserIDs GuidTableType
	INSERT INTO @AdminUserIDs SELECT * FROM @AdminUserIDsTemp
	
	IF (SELECT COUNT(*) FROM @AdminUserIDs) = 0 BEGIN
		SET @_Result = 1
		RETURN
	END
	
	DECLARE @UIDs TABLE(UserID uniqueidentifier primary key clustered, [Exists] bit)
	
	INSERT INTO @UIDs (UserID, [Exists])
	SELECT A.Value, MAX(CASE WHEN D.ID IS NULL THEN 0 ELSE 1 END)
	FROM @AdminUserIDs AS A
		LEFT JOIN [dbo].[NTFN_Dashboards] AS D
		ON D.ApplicationID = @ApplicationID AND D.UserID = A.Value AND 
			D.NodeID = @NodeID AND D.[Type] = N'Wiki' AND D.Deleted = 0 AND D.Done = 0
	GROUP BY A.Value
		
	IF EXISTS(SELECT TOP(1) * FROM @UIDs WHERE [Exists] = 1) BEGIN
		UPDATE [dbo].[NTFN_Dashboards]
			SET Seen = 0
		FROM @UIDs AS UIDs
			INNER JOIN [dbo].[NTFN_Dashboards] AS D
			ON D.UserID = UIDs.UserID
		WHERE D.ApplicationID = @ApplicationID AND UIDS.[Exists] = 1 AND 
			D.NodeID = @NodeID AND D.[Type] = N'Wiki' AND D.Done = 0 AND D.Deleted = 0
			
		SET @_Result = @@ROWCOUNT
		IF @_Result <= 0 RETURN
	END
	
	IF EXISTS(SELECT TOP(1) * FROM @UIDs WHERE [Exists] = 0) BEGIN
		DECLARE @Dashboards DashboardTableType
		
		INSERT INTO @Dashboards(UserID, NodeID, RefItemID, [Type], Removable, SendDate)
		SELECT	UIDs.UserID, @NodeID, @RefItemID, N'Wiki', 1, @SendDate
		FROM @UIDs AS UIDs
		WHERE UIDs.[Exists] = 0
		
		EXEC [dbo].[NTFN_P_SendDashboards] @ApplicationID, @Dashboards, @_Result output
		
		IF @_Result <= 0 RETURN
		
		IF @_Result > 0 BEGIN
			SELECT * 
			FROM @Dashboards
		END
	END
	
	SET @_Result = 1
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_SetTitlesOrder]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_SetTitlesOrder]
GO

CREATE PROCEDURE [dbo].[WK_SetTitlesOrder]
	@ApplicationID	uniqueidentifier,
	@strTitleIDs	varchar(max),
	@delimiter		char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @TitleIDs TABLE (SequenceNo int identity(1, 1) primary key, TitleID uniqueidentifier)
	
	INSERT INTO @TitleIDs (TitleID)
	SELECT Ref.Value
	FROM [dbo].[GFN_StrToGuidTable](@strTitleIDs, @delimiter) AS Ref
	
	DECLARE @OwnerID uniqueidentifier
	
	SELECT @OwnerID = OwnerID
	FROM [dbo].[WK_Titles]
	WHERE ApplicationID = @ApplicationID AND 
		TitleID = (SELECT TOP (1) Ref.TitleID FROM @TitleIDs AS Ref)
	
	IF @OwnerID IS NULL BEGIN
		SELECT -1
		RETURN
	END
	
	INSERT INTO @TitleIDs (TitleID)
	SELECT TT.TitleID
	FROM @TitleIDs AS Ref
		RIGHT JOIN [dbo].[WK_Titles] AS TT
		ON TT.TitleID = Ref.TitleID
	WHERE TT.ApplicationID = @ApplicationID AND TT.OwnerID = @OwnerID AND Ref.TitleID IS NULL
	ORDER BY TT.SequenceNo
	
	UPDATE [dbo].[WK_Titles]
		SET SequenceNo = Ref.SequenceNo
	FROM @TitleIDs AS Ref
		INNER JOIN [dbo].[WK_Titles] AS TT
		ON TT.TitleID = Ref.TitleID
	WHERE TT.ApplicationID = @ApplicationID AND TT.OwnerID = @OwnerID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_SetParagraphsOrder]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_SetParagraphsOrder]
GO

CREATE PROCEDURE [dbo].[WK_SetParagraphsOrder]
	@ApplicationID		uniqueidentifier,
	@strParagraphIDs	varchar(max),
	@delimiter			char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @ParagraphIDs TABLE (SequenceNo int identity(1, 1) primary key, ParagraphID uniqueidentifier)
	
	INSERT INTO @ParagraphIDs (ParagraphID)
	SELECT Ref.Value
	FROM [dbo].[GFN_StrToGuidTable](@strParagraphIDs, @delimiter) AS Ref
	
	DECLARE @TitleID uniqueidentifier
	
	SELECT @TitleID = TitleID
	FROM [dbo].[WK_Paragraphs]
	WHERE ApplicationID = @ApplicationID AND 
		ParagraphID = (SELECT TOP (1) Ref.ParagraphID FROM @ParagraphIDs AS Ref)
	
	IF @TitleID IS NULL BEGIN
		SELECT -1
		RETURN
	END
	
	INSERT INTO @ParagraphIDs(ParagraphID)
	SELECT P.ParagraphID
	FROM @ParagraphIDs AS Ref
		RIGHT JOIN [dbo].[WK_Paragraphs] AS P
		ON P.ParagraphID = Ref.ParagraphID
	WHERE P.ApplicationID = @ApplicationID AND P.TitleID = @TitleID AND Ref.ParagraphID IS NULL
	ORDER BY P.SequenceNo
	
	UPDATE [dbo].[WK_Paragraphs]
		SET SequenceNo = Ref.SequenceNo
	FROM @ParagraphIDs AS Ref
		INNER JOIN [dbo].[WK_Paragraphs] AS P
		ON P.ParagraphID = Ref.ParagraphID
	WHERE P.ApplicationID = @ApplicationID AND P.TitleID = @TitleID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_P_AddTitle]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_P_AddTitle]
GO

CREATE PROCEDURE [dbo].[WK_P_AddTitle]
	@ApplicationID		uniqueidentifier,
    @TitleID	 		uniqueidentifier,
    @OwnerID			uniqueidentifier,
    @Title				nvarchar(500),
    @SequenceNo			int,
    @CreatorUserID		uniqueidentifier,
    @CreationDate		datetime,
    @OwnerType			varchar(20),
    @Accept				bit,
    @_Result			int output
WITH ENCRYPTION
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	SET @Title = [dbo].[GFN_VerifyString](LTRIM(RTRIM(@Title)))
	
	IF ISNULL(@Title, N'') = N'' BEGIN
		SET @_Result = -1
		ROLLBACK TRANSACTION
		RETURN
	END
	
	DECLARE @Status varchar(20)
	
	IF @Accept = 1 SET @Status = N'Accepted'
	ELSE SET @Status = N'CitationNeeded'
	
	-- Update All Sequence Numbers
	UPDATE TT
		SET SequenceNo = Ref.SequenceNo
	FROM (
			SELECT	T.TitleID,
					((ROW_NUMBER() OVER (ORDER BY T.Deleted ASC, T.SequenceNo ASC)) * 2) AS SequenceNo
			FROM [dbo].[WK_Titles] AS T
			WHERE T.ApplicationID = @ApplicationID AND T.OwnerID = @OwnerID
		) AS Ref
		INNER JOIN [dbo].[WK_Titles] AS TT
		ON TT.ApplicationID = @ApplicationID AND TT.TitleID = Ref.TitleID
		
	IF ISNULL(@SequenceNo, 0) <= 0 SET @SequenceNo = 1
	
	SET @SequenceNo = (@SequenceNo * 2) - 1
	-- end of Update All Sequence Numbers
	
	INSERT INTO [dbo].[WK_Titles](
		ApplicationID,
		TitleID,
		OwnerID,
		CreatorUserID,
		CreationDate,
		SequenceNo,
		Title,
		[Status],
		OwnerType,
		Deleted
	)
	VALUES(
		@ApplicationID,
		@TitleID,
		@OwnerID,
		@CreatorUserID,
		@CreationDate,
		@SequenceNo,
		@Title,
		@Status,
		@OwnerType,
		0
	)
	
	SET @_Result = 1
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_AddTitle]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_AddTitle]
GO

CREATE PROCEDURE [dbo].[WK_AddTitle]
	@ApplicationID		uniqueidentifier,
    @TitleID	 		uniqueidentifier,
    @OwnerID			uniqueidentifier,
    @Title				nvarchar(500),
    @SequenceNo			int,
    @CreatorUserID		uniqueidentifier,
    @CreationDate		datetime,
    @OwnerType			varchar(20),
    @Accept				bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @_Result int = -1
	
	EXEC [dbo].[WK_P_AddTitle] @ApplicationID, @TitleID, @OwnerID, @Title, 
		@SequenceNo, @CreatorUserID, @CreationDate, @OwnerType, @Accept, @_Result output
	
	SELECT @_Result
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_ModifyTitle]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_ModifyTitle]
GO

CREATE PROCEDURE [dbo].[WK_ModifyTitle]
	@ApplicationID			uniqueidentifier,
    @TitleID 				uniqueidentifier,
    @Title					nvarchar(500),
    @LastModifierUserID		uniqueidentifier,
    @LastModificationDate	datetime,
    @Accept					bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SET @Title = [dbo].[GFN_VerifyString](LTRIM(RTRIM(@Title)))
	
	IF ISNULL(@Title, N'') = N'' BEGIN
		SELECT -1
		RETURN
	END
	
	DECLARE @Status varchar(20)
	
	IF @Accept = 1 SET @Status = N'Accepted'
	ELSE SET @Status = N'CitationNeeded'
	
	UPDATE [dbo].[WK_Titles]
		SET Title = @Title,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate,
			[Status] = @Status
	WHERE ApplicationID = @ApplicationID AND TitleID = @TitleID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_ArithmeticDeleteTitle]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_ArithmeticDeleteTitle]
GO

CREATE PROCEDURE [dbo].[WK_ArithmeticDeleteTitle]
	@ApplicationID			uniqueidentifier,
	@TitleID				uniqueidentifier,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[WK_Titles]
		SET [Deleted] = 1,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND TitleID = @TitleID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_RecycleTitle]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_RecycleTitle]
GO

CREATE PROCEDURE [dbo].[WK_RecycleTitle]
	@ApplicationID			uniqueidentifier,
	@TitleID				uniqueidentifier,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[WK_Titles]
		SET [Deleted] = 0,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND TitleID = @TitleID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_P_AddParagraph]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_P_AddParagraph]
GO

CREATE PROCEDURE [dbo].[WK_P_AddParagraph]
	@ApplicationID		uniqueidentifier,
    @ParagraphID 		uniqueidentifier,
    @TitleID	 		uniqueidentifier,
    @Title				nvarchar(500),
    @BodyText			nvarchar(max),
    @SequenceNo			int,
    @CreatorUserID		uniqueidentifier,
    @CreationDate		datetime,
    @IsRichText			bit,
    @SendToAdmins		bit,
    @HasAdmin			bit,
    @_Result			int output
WITH ENCRYPTION
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	SET @Title = [dbo].[GFN_VerifyString](@Title)
	SET @BodyText = [dbo].[GFN_VerifyString](@BodyText)
	
	IF @HasAdmin IS NULL SET @HasAdmin = 0
	
	DECLARE @Status varchar(20)
	
	IF @SendToAdmins IS NULL OR @SendToAdmins = 0 SET @Status = N'Accepted'
	ELSE IF @HasAdmin = 0 SET @Status = N'CitationNeeded'
	ELSE SET @Status = N'Pending'
	
	-- Update All Sequence Numbers
	UPDATE P
		SET SequenceNo = Ref.SequenceNo
	FROM (
			SELECT	P.ParagraphID,
					((ROW_NUMBER() OVER (ORDER BY P.Deleted ASC, P.SequenceNo ASC)) * 2) AS SequenceNo
			FROM [dbo].[WK_Paragraphs] AS P
			WHERE P.ApplicationID = @ApplicationID AND P.TitleID = @TitleID
		) AS Ref
		INNER JOIN [dbo].[WK_Paragraphs] AS P
		ON P.ApplicationID = @ApplicationID AND P.ParagraphID = Ref.ParagraphID
		
	IF ISNULL(@SequenceNo, 0) <= 0 SET @SequenceNo = 1
	
	SET @SequenceNo = (@SequenceNo * 2) - 1
	-- end of Update All Sequence Numbers
	
	INSERT INTO [dbo].[WK_Paragraphs](
		ApplicationID,
		ParagraphID,
		TitleID,
		CreatorUserID,
		CreationDate,
		Title,
		BodyText,
		SequenceNo,
		IsRichText,
		[Status],
		Deleted
	)
	VALUES(
		@ApplicationID,
		@ParagraphID,
		@TitleID,
		@CreatorUserID,
		@CreationDate,
		@Title,
		@BodyText,
		@SequenceNo,
		@IsRichText,
		@Status,
		0
	)
	
	IF @@ROWCOUNT <= 0 BEGIN
		SET @_Result = -1
		RETURN
	END
	
	DECLARE @_ChangeID uniqueidentifier = NEWID()
	
	INSERT INTO [dbo].[WK_Changes](
		ApplicationID,
		ChangeID,
		ParagraphID,
		UserID,
		SendDate,
		Title,
		BodyText,
		Applied,
		[Status],
		Deleted
	)
	VALUES(
		@ApplicationID,
		@_ChangeID,
		@ParagraphID,
		@CreatorUserID,
		@CreationDate,
		@Title,
		@BodyText,
		(CASE WHEN @Status = N'CitationNeeded' OR @Status = N'Accepted' THEN 1 ELSE 0 END),
		(CASE WHEN @Status = N'CitationNeeded' OR @Status = N'Accepted' THEN N'Accepted' ELSE N'Pending' END),
		0
	)
	
	IF @@ROWCOUNT <= 0 BEGIN
		SET @_Result = -1
		RETURN
	END
	
	SET @_Result = 1
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_AddParagraph]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_AddParagraph]
GO

CREATE PROCEDURE [dbo].[WK_AddParagraph]
	@ApplicationID		uniqueidentifier,
    @ParagraphID 		uniqueidentifier,
    @TitleID	 		uniqueidentifier,
    @Title				nvarchar(500),
    @BodyText			nvarchar(max),
    @SequenceNo			int,
    @CreatorUserID		uniqueidentifier,
    @CreationDate		datetime,
    @IsRichText			bit,
    @SendToAdmins		bit,
    @HasAdmin			bit,
    @AdminUserIDsTemp	GuidTableType readonly
WITH ENCRYPTION, RECOMPILE
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	DECLARE @AdminUserIDs GuidTableType
	INSERT INTO @AdminUserIDs SELECT * FROM @AdminUserIDsTemp
	
	DECLARE @_Result int = -1
	
	EXEC [dbo].[WK_P_AddParagraph] @ApplicationID, @ParagraphID, @TitleID, @Title, @BodyText,
		@SequenceNo, @CreatorUserID, @CreationDate, @IsRichText, @SendToAdmins, 
		@HasAdmin, @_Result output
	
	-- Send Dashboards
	DECLARE @UIDs GuidTableType
	
	INSERT INTO @UIDs
	SELECT Ref.Value
	FROM @AdminUserIDs AS Ref
	WHERE Ref.Value <> @CreatorUserID
	
	IF @SendToAdmins = 1 AND (SELECT COUNT(*) FROM @UIDs) > 0 BEGIN
		DECLARE @OwnerID uniqueidentifier = (
			SELECT TOP(1) OwnerID
			FROM [dbo].[WK_Titles]
			WHERE ApplicationID = @ApplicationID AND TitleID = @TitleID
		)
		
		EXEC [dbo].[WK_P_SendDashboards] @ApplicationID, @ParagraphID, @OwnerID, @UIDs, 
			@CreationDate, @_Result output
		
		IF @_Result <= 0 BEGIN
			SELECT -1
			ROLLBACK TRANSACTION 
			RETURN
		END
	END
	-- end of Send Dashboards
	
	SELECT 1
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_ModifyParagraph]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_ModifyParagraph]
GO

CREATE PROCEDURE [dbo].[WK_ModifyParagraph]
	@ApplicationID			uniqueidentifier,
    @ParagraphID			uniqueidentifier,
    @ChangeID2Accept		uniqueidentifier,
    @Title					nvarchar(500),
    @BodyText				nvarchar(max),
    @LastModifierUserID		uniqueidentifier,
    @LastModificationDate	datetime,
    @CitationNeeded			bit,
    @Apply					bit,
    @Accept					bit,
    @HasAdmin				bit,
    @AdminUserIDsTemp		GuidTableType readonly
WITH ENCRYPTION, RECOMPILE
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	DECLARE @AdminUserIDs GuidTableType
	INSERT INTO @AdminUserIDs SELECT * FROM @AdminUserIDsTemp
	
	SET @Title = [dbo].[GFN_VerifyString](@Title)
	SET @BodyText = [dbo].[GFN_VerifyString](@BodyText)
	
	IF @HasAdmin IS NULL SET @HasAdmin = 0
	
	DECLARE @AcceptionDate datetime, @Applied bit, @ChangeStatus varchar(20)
	
	SET @AcceptionDate = NULL
	IF @Accept = 1 SET @AcceptionDate = @LastModificationDate
	
	SET @Applied = 0
	IF @Apply = 1 SET @Applied = 1
	
	SET @ChangeStatus = N'Pending'
	IF @Apply = 1 SET @ChangeStatus = N'Accepted'
	
	DECLARE @_ChangeID uniqueidentifier = (
		SELECT TOP(1) ChangeID 
		FROM [dbo].[WK_Changes]
		WHERE ApplicationID = @ApplicationID AND ParagraphID = @ParagraphID 
			AND UserID = @LastModifierUserID AND [Status] = N'Pending' AND Deleted = 0
	)
	
	IF @_ChangeID IS NOT NULL BEGIN	
		UPDATE [dbo].[WK_Changes]
			SET Title = @Title,
				BodyText = @BodyText,
				LastModificationDate = @LastModificationDate,
				[Status] = @ChangeStatus
		WHERE ApplicationID = @ApplicationID AND ChangeID = @_ChangeID
	END
	ELSE BEGIN
		SET @_ChangeID = NEWID()
	
		INSERT INTO [dbo].[WK_Changes](
			ApplicationID,
			ChangeID,
			ParagraphID,
			UserID,
			SendDate,
			Title,
			BodyText,
			Applied,
			ApplicationDate,
			[Status],
			AcceptionDate,
			Deleted
		)
		VALUES(
			@ApplicationID,
			@_ChangeID,
			@ParagraphID,
			@LastModifierUserID,
			@LastModificationDate,
			@Title,
			@BodyText,
			@Applied,
			@LastModificationDate,
			@ChangeStatus,
			@AcceptionDate,
			0
		)
	END
	
	IF @@ROWCOUNT <= 0 BEGIN
		SELECT -1
		ROLLBACK TRANSACTION
		RETURN
	END
	
	IF @Apply = 1 BEGIN
		DECLARE @_Result int
	
		IF @ChangeID2Accept IS NOT NULL BEGIN
			EXEC [dbo].[WK_P_AcceptChange] @ApplicationID, @ChangeID2Accept, 
				@LastModifierUserID, @LastModificationDate, @_Result output
				
			IF @_Result <= 0 BEGIN
				SELECT -1
				ROLLBACK TRANSACTION 
				RETURN
			END
		END
	
		DECLARE @SubjectStatus varchar(20)
		SET @SubjectStatus = N'Accepted'
		IF @CitationNeeded = 1 SET @SubjectStatus = N'CitationNeeded'
		
		UPDATE [dbo].[WK_Paragraphs]
			SET Title = @Title,
				BodyText = @BodyText,
				LastModifierUserID = @LastModifierUserID,
				LastModificationDate = @LastModificationDate,
				[Status] = @SubjectStatus
		WHERE ApplicationID = @ApplicationID AND ParagraphID = @ParagraphID
		
		IF @@ROWCOUNT <= 0 BEGIN
			SELECT -1
			ROLLBACK TRANSACTION
			RETURN
		END
	END
	
	-- Send Dashboards
	DECLARE @UIDs GuidTableType
	
	INSERT INTO @UIDs
	SELECT Ref.Value
	FROM @AdminUserIDs AS Ref
	WHERE Ref.Value <> @LastModifierUserID
	
	IF ISNULL(@Apply, 0) = 0 AND @HasAdmin = 1 AND (SELECT COUNT(*) FROM @UIDs) > 0 BEGIN
		DECLARE @OwnerID uniqueidentifier = (
			SELECT TOP(1) OwnerID
			FROM [dbo].[WK_Paragraphs] AS P
				INNER JOIN [dbo].[WK_Titles] AS T
				ON T.ApplicationID = @ApplicationID AND T.TitleID = P.TitleID
			WHERE P.ApplicationID = @ApplicationID AND P.ParagraphID = @ParagraphID
		)
		
		EXEC [dbo].[WK_P_SendDashboards] @ApplicationID, @ParagraphID, @OwnerID, @UIDs, 
			@LastModificationDate, @_Result output
		
		IF @_Result <= 0 BEGIN
			SET @_Result = -1
			ROLLBACK TRANSACTION 
			RETURN
		END
	END
	-- end of Send Dashboards
	
	SELECT 1
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_ArithmeticDeleteParagraph]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_ArithmeticDeleteParagraph]
GO

CREATE PROCEDURE [dbo].[WK_ArithmeticDeleteParagraph]
	@ApplicationID			uniqueidentifier,
	@ParagraphID			uniqueidentifier,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[WK_Paragraphs]
		SET [Deleted] = 1,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND ParagraphID = @ParagraphID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_RecycleParagraph]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_RecycleParagraph]
GO

CREATE PROCEDURE [dbo].[WK_RecycleParagraph]
	@ApplicationID			uniqueidentifier,
	@ParagraphID			uniqueidentifier,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[WK_Paragraphs]
		SET [Deleted] = 0,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND ParagraphID = @ParagraphID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_P_AcceptChange]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_P_AcceptChange]
GO

CREATE PROCEDURE [dbo].[WK_P_AcceptChange]
	@ApplicationID		uniqueidentifier,
	@ChangeID			uniqueidentifier,
	@EvaluatorUserID	uniqueidentifier,
	@EvaluationDate		datetime,
	@_Result			int output
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[WK_Changes]
		SET [Status] = N'Accepted',
			[AcceptionDate] = @EvaluationDate,
			[EvaluatorUserID] = @EvaluatorUserID,
			[EvaluationDate] = @EvaluationDate
	WHERE ApplicationID = @ApplicationID AND ChangeID = @ChangeID
	
	SET @_Result = @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_AcceptChange]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_AcceptChange]
GO

CREATE PROCEDURE [dbo].[WK_AcceptChange]
	@ApplicationID		uniqueidentifier,
	@ChangeID			uniqueidentifier,
	@EvaluatorUserID	uniqueidentifier,
	@EvaluationDate		datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @_Result int
	
	EXEC [dbo].[WK_P_AcceptChange] @ApplicationID, @ChangeID, 
		@EvaluatorUserID, @EvaluationDate, @_Result output
	
	SELECT @_Result
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_RejectChange]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_RejectChange]
GO

CREATE PROCEDURE [dbo].[WK_RejectChange]
	@ApplicationID		uniqueidentifier,
	@ChangeID			uniqueidentifier,
	@EvaluatorUserID	uniqueidentifier,
	@EvaluationDate		datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[WK_Changes]
		SET [Status] = N'Rejected',
			[EvaluatorUserID] = @EvaluatorUserID,
			[EvaluationDate] = @EvaluationDate
	WHERE ApplicationID = @ApplicationID AND ChangeID = @ChangeID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_ArithmeticDeleteChange]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_ArithmeticDeleteChange]
GO

CREATE PROCEDURE [dbo].[WK_ArithmeticDeleteChange]
	@ApplicationID	uniqueidentifier,
	@ChangeID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[WK_Changes]
		SET [Deleted] = 1
	WHERE ApplicationID = @ApplicationID AND ChangeID = @ChangeID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_P_GetTitlesByIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_P_GetTitlesByIDs]
GO

CREATE PROCEDURE [dbo].[WK_P_GetTitlesByIDs]
	@ApplicationID	uniqueidentifier,
	@TitleIDsTemp	GuidTableType readonly
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @TitleIDs GuidTableType
	INSERT INTO @TitleIDs SELECT * FROM @TitleIDsTemp
	
	SELECT TT.TitleID AS TitleID,
		   TT.OwnerID AS OwnerID,
		   TT.Title AS Title,
		   TT.SequenceNo AS SequenceNumber,
		   TT.CreatorUserID AS CreatorUserID,
		   TT.CreationDate AS CreationDate,
		   TT.LastModificationDate AS LastModificationDate,
		   TT.[Status] AS [Status]
	FROM @TitleIDs AS ExternalIDs
		INNER JOIN [dbo].[WK_Titles] AS TT
		ON TT.ApplicationID = @ApplicationID AND ExternalIDs.Value = TT.TitleID
	ORDER BY TT.SequenceNo
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_GetTitlesByIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_GetTitlesByIDs]
GO

CREATE PROCEDURE [dbo].[WK_GetTitlesByIDs]
	@ApplicationID	uniqueidentifier,
	@strTitleIDs	varchar(max),
	@delimiter		char,
	@ViewerUserID	uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @TitleIDs GuidTableType
	INSERT INTO @TitleIDs
	SELECT DISTINCT Ref.Value FROM [dbo].[GFN_StrToGuidTable](@strTitleIDs, @delimiter) AS Ref
	
	EXEC [dbo].[WK_P_GetTitlesByIDs] @ApplicationID, @TitleIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_GetTitles]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_GetTitles]
GO

CREATE PROCEDURE [dbo].[WK_GetTitles]
	@ApplicationID	uniqueidentifier,
	@OwnerID		uniqueidentifier,
	@IsAdmin		bit,
	@ViewerUserID	uniqueidentifier,
	@Deleted		bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF @IsAdmin IS NULL SET @IsAdmin = 0
	IF @Deleted IS NULL SET @Deleted = 0
	
	DECLARE @TitleIDs GuidTableType
	
	INSERT INTO @TitleIDs
	SELECT T.TitleID
	FROM [dbo].[WK_Titles] AS T
		LEFT JOIN [dbo].[WK_Paragraphs] AS P
		ON P.ApplicationID = @ApplicationID AND P.TitleID = T.TitleID AND (
			@IsAdmin = 1 OR P.[Status] = N'Accepted' OR P.[Status] = N'CitationNeeded' OR (
				P.[Status] = N'Pending' AND @ViewerUserID IS NOT NULL AND 
				P.CreatorUserID = @ViewerUserID
			)
		) AND P.Deleted = @Deleted
	WHERE T.ApplicationID = @ApplicationID AND T.OwnerID = @OwnerID AND (
			@IsAdmin = 1 OR T.[Status] = N'Accepted' OR (
				T.[Status] = N'CitationNeeded' AND @ViewerUserID IS NOT NULL AND 
				T.CreatorUserID = @ViewerUserID
			) OR P.ParagraphID IS NOT NULL
		) AND T.Deleted = @Deleted
	GROUP BY T.TitleID
	
	EXEC [dbo].[WK_P_GetTitlesByIDs] @ApplicationID, @TitleIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_HasTitle]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_HasTitle]
GO

CREATE PROCEDURE [dbo].[WK_HasTitle]
	@ApplicationID		uniqueidentifier,
	@OwnerID			uniqueidentifier,
	@ViewerUserID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT TOP(1) 1
	FROM [dbo].[WK_Titles] AS T
	WHERE T.ApplicationID = @ApplicationID AND T.OwnerID = @OwnerID AND
		(@ViewerUserID IS NULL OR T.[Status] = N'Accepted' OR 
		T.[Status] = N'CitationNeeded' OR T.CreatorUserID = @ViewerUserID) AND T.Deleted = 0
	
	SELECT -1
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_P_GetParagraphsByIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_P_GetParagraphsByIDs]
GO

CREATE PROCEDURE [dbo].[WK_P_GetParagraphsByIDs]
	@ApplicationID		uniqueidentifier,
	@ParagraphIDsTemp	GuidTableType readonly
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @ParagraphIDs GuidTableType
	INSERT INTO @ParagraphIDs SELECT * FROM @ParagraphIDsTemp
	
	SELECT PG.ParagraphID AS ParagraphID,
		   PG.TitleID AS TitleID,
		   PG.Title AS Title,
		   PG.BodyText AS BodyText,
		   PG.SequenceNo AS SequenceNumber,
		   PG.IsRichText AS IsRichText,
		   PG.CreatorUserID AS CreatorUserID,
		   PG.CreationDate AS CreationDate,
		   PG.LastModificationDate AS LastModificationDate,
		   PG.[Status] AS [Status]
	FROM @ParagraphIDs AS ExternalIDs
		INNER JOIN [dbo].[WK_Paragraphs] AS PG
		ON PG.ApplicationID = @ApplicationID AND PG.ParagraphID = ExternalIDs.Value
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_GetParagraphsByIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_GetParagraphsByIDs]
GO

CREATE PROCEDURE [dbo].[WK_GetParagraphsByIDs]
	@ApplicationID		uniqueidentifier,
	@strParagraphIDs	varchar(max),
	@delimiter			char,
	@ViewerUserID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @ParagraphIDs GuidTableType
	INSERT INTO @ParagraphIDs
	SELECT DISTINCT Ref.Value 
	FROM [dbo].[GFN_StrToGuidTable](@strParagraphIDs, @delimiter) AS Ref
	
	EXEC [dbo].[WK_P_GetParagraphsByIDs] @ApplicationID, @ParagraphIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_GetParagraphs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_GetParagraphs]
GO

CREATE PROCEDURE [dbo].[WK_GetParagraphs]
	@ApplicationID		uniqueidentifier,
	@strTitleIDs		varchar(max),
	@delimiter			char,
	@IsAdmin			bit,
	@ViewerUserID		uniqueidentifier,
	@Deleted			bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF @IsAdmin IS NULL SET @IsAdmin = 0
	IF @Deleted IS NULL SET @Deleted = 0
	
	DECLARE @TitleIDs GuidTableType
	INSERT INTO @TitleIDs
	SELECT DISTINCT Ref.Value FROM [dbo].[GFN_StrToGuidTable](@strTitleIDs, @delimiter) AS Ref
	
	DECLARE @ParagraphIDs GuidTableType
	
	INSERT INTO @ParagraphIDs
	SELECT PG.ParagraphID
	FROM @TitleIDs AS ExternalIDs
		INNER JOIN [dbo].[WK_Paragraphs] AS PG
		ON PG.TitleID = ExternalIDs.Value
	WHERE PG.ApplicationID = @ApplicationID AND 
		(@IsAdmin = 1 OR PG.[Status] = N'Accepted' OR PG.[Status] = N'CitationNeeded' OR (
			PG.[Status] = N'Pending' AND @ViewerUserID IS NOT NULL AND 
			PG.CreatorUserID = @ViewerUserID
		)) AND PG.Deleted = @Deleted
		
	EXEC [dbo].[WK_P_GetParagraphsByIDs] @ApplicationID, @ParagraphIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_HasParagraph]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_HasParagraph]
GO

CREATE PROCEDURE [dbo].[WK_HasParagraph]
	@ApplicationID		uniqueidentifier,
	@TitleOrOwnerID		uniqueidentifier,
	@ViewerUserID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF EXISTS(
		SELECT TOP (1) 1 
		FROM [dbo].[WK_Titles] 
		WHERE ApplicationID = @ApplicationID AND TitleID = @TitleOrOwnerID
	) BEGIN
		SELECT TOP(1) 1
		FROM [dbo].[WK_Paragraphs] AS P
		WHERE P.ApplicationID = @ApplicationID AND P.TitleID = @TitleOrOwnerID AND
			(@ViewerUserID IS NULL OR P.[Status] = N'Accepted' OR 
			P.[Status] = N'CitationNeeded' OR P.CreatorUserID = @ViewerUserID) AND P.Deleted = 0
	END
	ELSE BEGIN
		SELECT TOP(1) 1
		FROM [dbo].[WK_Titles] AS T
			INNER JOIN [dbo].[WK_Paragraphs] AS P
			ON P.ApplicationID = @ApplicationID AND P.TitleID = T.TitleID
		WHERE T.ApplicationID = @ApplicationID AND T.OwnerID = @TitleOrOwnerID AND
			(@ViewerUserID IS NULL OR P.[Status] = N'Accepted' OR 
			P.[Status] = N'CitationNeeded' OR P.CreatorUserID = @ViewerUserID) AND P.Deleted = 0
	END
	
	SELECT -1
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_P_GetChangesByIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_P_GetChangesByIDs]
GO

CREATE PROCEDURE [dbo].[WK_P_GetChangesByIDs]
	@ApplicationID	uniqueidentifier,
	@ChangeIDsTemp	GuidTableType readonly
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @ChangeIDs GuidTableType
	INSERT INTO @ChangeIDs SELECT * FROM @ChangeIDsTemp
	
	SELECT C.[ChangeID] AS ChangeID,
		   C.[ParagraphID] AS ParagraphID,
		   C.[Title] AS Title,
		   C.[BodyText] AS BodyText,
		   C.[Status] AS [Status],
		   C.[Applied] AS Applied,
		   C.[SendDate] AS SendDate,
		   C.[UserID] AS SenderUserID,
		   UN.[UserName] AS SenderUserName,
		   UN.[FirstName] AS SenderFirstName,
		   UN.[LastName] AS SenderLastName
	FROM @ChangeIDs AS ExternalIDs
		INNER JOIN [dbo].[WK_Changes] AS C
		ON C.ApplicationID = @ApplicationID AND C.[ChangeID] = ExternalIDs.Value
		INNER JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND UN.[UserID] = C.[UserID]
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_GetChangesByIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_GetChangesByIDs]
GO

CREATE PROCEDURE [dbo].[WK_GetChangesByIDs]
	@ApplicationID	uniqueidentifier,
	@strChangeIDs	varchar(max),
	@delimiter		char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @ChangeIDs GuidTableType
	INSERT INTO @ChangeIDs
	SELECT DISTINCT Ref.Value FROM GFN_StrToGuidTable(@strChangeIDs, @delimiter) AS Ref
	
	EXEC [dbo].[WK_P_GetChangesByIDs] @ApplicationID, @ChangeIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_GetParagraphChanges]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_GetParagraphChanges]
GO

CREATE PROCEDURE [dbo].[WK_GetParagraphChanges]
	@ApplicationID		uniqueidentifier,
	@strParagraphIDs	varchar(max),
	@delimiter			char,
	@CreatorUserID		uniqueidentifier,
	@Status				varchar(20),
	@Applied			bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @ParagraphIDs GuidTableType
	INSERT INTO @ParagraphIDs
	SELECT DISTINCT Ref.Value 
	FROM [dbo].[GFN_StrToGuidTable](@strParagraphIDs, @delimiter) AS Ref
	
	DECLARE @ChangeIDs GuidTableType
	
	INSERT INTO @ChangeIDs
	SELECT DISTINCT CH.ChangeID
	FROM @ParagraphIDs AS ExternalIDs
		INNER JOIN [dbo].[WK_Changes] AS CH
		ON CH.ParagraphID = ExternalIDs.Value
	WHERE CH.ApplicationID = @ApplicationID AND 
		(@CreatorUserID IS NULL OR CH.UserID = @CreatorUserID) AND
		(@Status IS NULL OR CH.[Status] = @Status) AND
		(@Applied IS NULL OR CH.Applied = @Applied) AND CH.Deleted = 0
	
	EXEC [dbo].[WK_P_GetChangesByIDs] @ApplicationID, @ChangeIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_GetLastPendingChange]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_GetLastPendingChange]
GO

CREATE PROCEDURE [dbo].[WK_GetLastPendingChange]
	@ApplicationID	uniqueidentifier,
	@ParagraphID	uniqueidentifier,
	@UserID			uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @ChangeIDs GuidTableType
	
	INSERT INTO @ChangeIDs
	SELECT ChangeID 
	FROM [dbo].[WK_Changes]
	WHERE ApplicationID = @ApplicationID AND ParagraphID = @ParagraphID AND 
		UserID = @UserID AND Deleted = 0 AND [Status] = N'Pending'
	
	EXEC [dbo].[WK_P_GetChangesByIDs] @ApplicationID, @ChangeIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_GetParagraphRelatedUserIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_GetParagraphRelatedUserIDs]
GO

CREATE PROCEDURE [dbo].[WK_GetParagraphRelatedUserIDs]
	@ApplicationID	uniqueidentifier,
	@ParagraphID	uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @UserIDs Table(id uniqueidentifier)
	
	DECLARE @CreatorID uniqueidentifier, @ModifierID uniqueidentifier
	
	SELECT @CreatorID = PH.CreatorUserID, @ModifierID = PH.LastModifierUserID
	FROM [dbo].[WK_Paragraphs] AS PH
	WHERE PH.ApplicationID = @ApplicationID AND PH.ParagraphID = @ParagraphID
	
	INSERT INTO @UserIDs (id) VALUES(@CreatorID)
	IF @ModifierID IS NOT NULL INSERT INTO @UserIDs (id) VALUES(@ModifierID)
	
	INSERT INTO @UserIDs
	SELECT DISTINCT CH.UserID AS ID
	FROM [dbo].[WK_Changes] AS CH
	WHERE CH.ApplicationID = @ApplicationID AND CH.ParagraphID = @ParagraphID AND 
		(CH.Applied = 1 OR CH.[Status] = N'Accepted')
		
	SELECT DISTINCT IDs.id AS ID
	FROM @UserIDs AS IDs
		INNER JOIN [dbo].[Users_Normal] AS USR
		ON IDs.id = USR.UserID
	WHERE USR.ApplicationID = @ApplicationID AND USR.IsApproved = 1
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_GetChangedWikiOwnerIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_GetChangedWikiOwnerIDs]
GO

CREATE PROCEDURE [dbo].[WK_GetChangedWikiOwnerIDs]
	@ApplicationID	uniqueidentifier,
	@strOwnerIDs	varchar(max),
	@delimter		char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @OwnerIDs GuidTableType
	INSERT INTO @OwnerIDs
	SELECT DISTINCT Ref.Value FROM [dbo].[GFN_StrToGuidTable](@strOwnerIDs, @delimter) AS Ref
	
	SELECT ExternalIDs.Value AS ID
	FROM @OwnerIDs AS ExternalIDs
	WHERE EXISTS(
			SELECT TOP(1) * 
			FROM [dbo].[WK_Titles] AS TT
				INNER JOIN [dbo].[WK_Paragraphs] AS PG
				ON PG.ApplicationID = @ApplicationID AND PG.TitleID = TT.TitleID
				INNER JOIN [dbo].[WK_Changes] AS CH
				ON CH.ApplicationID = @ApplicationID AND CH.ParagraphID = PG.ParagraphID
			WHERE TT.ApplicationID = @ApplicationID AND 
				TT.OwnerID = ExternalIDs.Value AND TT.Deleted = 0 AND 
				PG.Deleted = 0 AND CH.[Status] = N'Pending' AND CH.Deleted = 0
		)
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_P_CreateWiki]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_P_CreateWiki]
GO

CREATE PROCEDURE [dbo].[WK_P_CreateWiki]
	@ApplicationID	uniqueidentifier,
	@OwnerID		uniqueidentifier,
	@WikiTemp		StringPairTableType readonly,
	@HasAdmin		bit,
	@CreatorUserID	uniqueidentifier,
	@CreationDate	datetime,
	@_Result		int output
WITH ENCRYPTION, RECOMPILE
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	DECLARE @Wiki StringPairTableType
	INSERT INTO @Wiki SELECT * FROM @WikiTemp
	
	DECLARE @WikiTbl TABLE(SequenceNo int IDENTITY(1,1) PRIMARY KEY CLUSTERED,
		TitleID uniqueidentifier, Title nvarchar(500), 
		ParagraphID uniqueidentifier, Paragraph nvarchar(max))
	
	INSERT INTO @WikiTbl(
		TitleID,
		Title,
		ParagraphID,
		Paragraph
	)
	SELECT NEWID(), Ref.FirstValue, NEWID(), Ref.SecondValue
	FROM @Wiki AS Ref
	
	DECLARE @Count int = (SELECT COUNT(*) FROM @WikiTbl)
	DECLARE @Index int = 1
	
	WHILE @Index <= @Count BEGIN
		DECLARE @SequenceNo int, @TitleID uniqueidentifier, @Title nvarchar(500)
		
		SELECT @SequenceNo = Ref.SequenceNo, @TitleID = Ref.TitleID, @Title = Ref.Title
		FROM @WikiTbl AS Ref
		WHERE Ref.SequenceNo = @Index
			
		EXEC [dbo].[WK_P_AddTitle] @ApplicationID, @TitleID, @OwnerID, @Title, @SequenceNo, 
			@CreatorUserID, @CreationDate, 'Node', 1, @_Result output
			
		IF @_Result <= 0 BEGIN
			SET @_Result = -1
			ROLLBACK TRANSACTION
			RETURN
		END
		
		SET @Index = @Index + 1
	END
	
	SET @Index = 1
	
	WHILE @Index <= @Count BEGIN
		DECLARE @Att DocFileInfoTableType, @_TitleID uniqueidentifier, 
			@ParagraphID uniqueidentifier, @Paragraph nvarchar(max)
		
		SELECT @SequenceNo = Ref.SequenceNo, @_TitleID = Ref.TitleID,
			   @ParagraphID = Ref.ParagraphID, @Paragraph = Ref.Paragraph
		FROM @WikiTbl AS Ref
		WHERE Ref.SequenceNo = @Index
		
		IF @Paragraph IS NOT NULL AND @Paragraph <> N'' BEGIN
			EXEC [dbo].[WK_P_AddParagraph] @ApplicationID, @ParagraphID, @_TitleID, NULL, 
				@Paragraph, 1, @CreatorUserID, @CreationDate, 0, 0, @HasAdmin, @_Result output
				
			IF @_Result <= 0 BEGIN
				SET @_Result = -1
				ROLLBACK TRANSACTION
				RETURN
			END
		END
		
		SET @Index = @Index + 1
	END
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_GetWikiOwner]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_GetWikiOwner]
GO

CREATE PROCEDURE [dbo].[WK_GetWikiOwner]
	@ApplicationID	uniqueidentifier,
	@ID				uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @OwnerID uniqueidentifier
	DECLARE @OwnerType varchar(20)
	
	SELECT @OwnerID = OwnerID, @OwnerType = OwnerType
	FROM [dbo].[WK_Titles]
	WHERE ApplicationID = @ApplicationID AND TitleID = @ID
	
	IF @OwnerID IS NULL BEGIN	
		SELECT @OwnerID = TT.OwnerID, @OwnerType = TT.OwnerType
		FROM [dbo].[WK_Titles] AS TT
			INNER JOIN [dbo].[WK_Paragraphs] AS P
			ON P.ApplicationID = @ApplicationID AND P.TitleID = TT.TitleID
		WHERE TT.ApplicationID = @ApplicationID AND P.ParagraphID = @ID
	END
	
	IF @OwnerID IS NULL BEGIN
		SELECT @OwnerID = TT.OwnerID, @OwnerType = TT.OwnerType
		FROM [dbo].[WK_Titles] AS TT
			INNER JOIN [dbo].[WK_Paragraphs] AS P
			ON P.ApplicationID = @ApplicationID AND P.TitleID = TT.TitleID
			INNER JOIN [dbo].[WK_Changes] AS CH
			ON CH.ApplicationID = @ApplicationID AND CH.ParagraphID = P.ParagraphID
		WHERE TT.ApplicationID = @ApplicationID AND CH.ChangeID = @ID
	END
	
	SELECT @OwnerID AS OwnerID, @OwnerType AS OwnerType
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_GetWikiContent]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_GetWikiContent]
GO

CREATE PROCEDURE [dbo].[WK_GetWikiContent]
	@ApplicationID	uniqueidentifier,
	@OwnerID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT [dbo].[WK_FN_GetWikiContent](@ApplicationID, @OwnerID)
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_GetTitlesCount]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_GetTitlesCount]
GO

CREATE PROCEDURE [dbo].[WK_GetTitlesCount]
	@ApplicationID	uniqueidentifier,
	@OwnerID		uniqueidentifier,
	@IsAdmin		bit,
	@CurrentUserID	uniqueidentifier,
	@Removed		bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT COUNT(TitleID)
	FROM [dbo].[WK_Titles] AS T
	WHERE T.ApplicationID = @ApplicationID AND 
		T.OwnerID = @OwnerID AND (@IsAdmin = 1 OR T.[Status] = N'Accepted' OR (
			T.[Status] = N'CitationNeeded' AND @CurrentUserID IS NOT NULL AND 
			T.CreatorUserID = @CurrentUserID
		)) AND (@Removed IS NULL OR T.Deleted = @Removed)
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_GetParagraphsCount]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_GetParagraphsCount]
GO

CREATE PROCEDURE [dbo].[WK_GetParagraphsCount]
	@ApplicationID	uniqueidentifier,
	@strTitleIDs	varchar(max),
	@delimiter		char,
	@IsAdmin		bit,
	@CurrentUserID	uniqueidentifier,
	@Removed		bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @TitleIDs GuidTableType
	
	INSERT INTO @TitleIDs (Value)
	SELECT Ref.Value
	FROM [dbo].[GFN_StrToGuidTable](@strTitleIDs, @delimiter) AS Ref
	
	SELECT T.Value AS ID, COUNT(P.ParagraphID) [Count]
	FROM @TitleIDs AS T
		LEFT JOIN [dbo].[WK_Paragraphs] AS P
		ON P.ApplicationID = @ApplicationID AND P.TitleID = T.Value
	WHERE (@IsAdmin = 1 OR P.[Status] = N'Accepted' OR P.[Status] = N'CitationNeeded' OR (
			P.[Status] = N'Pending' AND @CurrentUserID IS NOT NULL AND 
			P.CreatorUserID = @CurrentUserID
		)) AND (@Removed IS NULL OR P.Deleted = @Removed)
	GROUP BY T.Value
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_GetChangesCount]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_GetChangesCount]
GO

CREATE PROCEDURE [dbo].[WK_GetChangesCount]
	@ApplicationID		uniqueidentifier,
	@strParagraphIDs	varchar(max),
	@delimiter			char,
	@Applied			bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @ParagraphIDs GuidTableType
	
	INSERT INTO @ParagraphIDs(Value)
	SELECT Ref.Value
	FROM [dbo].[GFN_StrToGuidTable](@strParagraphIDs, @delimiter) AS Ref
	
	SELECT P.Value AS ID, COUNT(C.ChangeID) [Count]
	FROM @ParagraphIDs AS P
		LEFT JOIN [dbo].[WK_Changes] AS C
		ON C.ApplicationID = @ApplicationID AND C.ParagraphID = P.Value
	WHERE (C.[Status] = N'Accepted' OR C.[Status] = N'CitationNeeded') AND 
		(@Applied IS NULL OR C.Applied = @Applied) AND C.Deleted = 0
	GROUP BY P.Value
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_LastModificationDate]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_LastModificationDate]
GO

CREATE PROCEDURE [dbo].[WK_LastModificationDate]
	@ApplicationID		uniqueidentifier,
	@OwnerID			uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	-- Last Modification Date for both existing and deleted paragraphs & titles
	-- because delete is a sort of modification
	
	SELECT MAX(P.CreationDate)
	FROM [dbo].[WK_Titles] AS T
		INNER JOIN [dbo].[WK_Paragraphs] AS P
		ON P.ApplicationID = @ApplicationID AND P.TitleID = T.TitleID AND
			(P.[Status] = N'Accepted' OR P.[Status] = N'CitationNeeded')
	WHERE T.ApplicationID = @ApplicationID AND T.OwnerID = @OwnerID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WK_WikiAuthors]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WK_WikiAuthors]
GO

CREATE PROCEDURE [dbo].[WK_WikiAuthors]
	@ApplicationID		uniqueidentifier,
	@OwnerID			uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT *
	FROM (
			SELECT C.UserID AS ID, COUNT(C.ChangeID) AS [Count]
			FROM [dbo].[WK_Titles] AS T
				INNER JOIN [dbo].[WK_Paragraphs] AS P
				ON P.ApplicationID = @ApplicationID AND P.TitleID = T.TitleID AND
					(P.[Status] = N'Accepted' OR P.[Status] = N'CitationNeeded')
				INNER JOIN [dbo].[WK_Changes] AS C
				ON C.ApplicationID = @ApplicationID AND 
					C.ParagraphID = P.ParagraphID AND C.Applied = 1 AND C.Deleted = 0
			WHERE T.ApplicationID = @ApplicationID AND T.OwnerID = @OwnerID AND T.Deleted = 0
			GROUP BY C.UserID
		) AS Ref
	ORDER BY Ref.[Count] DESC
END

GO