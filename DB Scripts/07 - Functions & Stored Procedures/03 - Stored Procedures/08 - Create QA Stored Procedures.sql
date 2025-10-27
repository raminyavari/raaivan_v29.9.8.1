USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_AddNewWorkFlow]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_AddNewWorkFlow]
GO

CREATE PROCEDURE [dbo].[QA_AddNewWorkFlow]
	@ApplicationID	uniqueidentifier,
    @WorkFlowID 	uniqueidentifier,
    @Name			nvarchar(200),
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

	SET @Name = [dbo].[GFN_VerifyString](@Name)
	
	DECLARE @SeqNo int = ISNULL((
		SELECT MAX(SequenceNumber) 
		FROM [dbo].[QA_WorkFlows]
		WHERE ApplicationID = @ApplicationID
	), 0) + 1

    INSERT INTO [dbo].[QA_WorkFlows] (
		[ApplicationID],
		[WorkFlowID],
        [Name],
        [SequenceNumber],
        [InitialCheckNeeded],
        [FinalConfirmationNeeded],
        [RemovableAfterConfirmation],
        [DisableComments],
        [DisableQuestionLikes],
        [DisableAnswerLikes],
        [DisableCommentLikes],
        [DisableBestAnswer],
        [CreatorUserID],
        [CreationDate],
        [Deleted]
    )
    VALUES (
		@ApplicationID,
		@WorkFlowID,
        @Name,
        @SeqNo,
        0,
        0,
        1,
        0,
        0,
        0,
        0,
        0,
        @CurrentUserID,
        @Now,
        0
    )
    
    SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_RenameWorkFlow]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_RenameWorkFlow]
GO

CREATE PROCEDURE [dbo].[QA_RenameWorkFlow]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@Name			nvarchar(200),
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SET @Name = [dbo].[GFN_VerifyString](@Name)
	
	UPDATE [dbo].[QA_WorkFlows]
	SET Name = @Name,
		LastModifierUserID = @CurrentUserID,
		LastModificationDate = @Now
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_SetWorkFlowDescription]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_SetWorkFlowDescription]
GO

CREATE PROCEDURE [dbo].[QA_SetWorkFlowDescription]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@Description	nvarchar(2000),
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SET @Description = [dbo].[GFN_VerifyString](@Description)
	
	UPDATE [dbo].[QA_WorkFlows]
	SET [Description] = @Description,
		LastModifierUserID = @CurrentUserID,
		LastModificationDate = @Now
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_SetWorkFlowsOrder]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_SetWorkFlowsOrder]
GO

CREATE PROCEDURE [dbo].[QA_SetWorkFlowsOrder]
	@ApplicationID	uniqueidentifier,
	@strWorkFlowIDs	varchar(max),
	@delimiter		char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @WorkFlowIDs TABLE (
		SequenceNo int identity(1, 1) primary key, 
		WorkFlowID uniqueidentifier
	)
	
	INSERT INTO @WorkFlowIDs (WorkFlowID)
	SELECT Ref.Value
	FROM [dbo].[GFN_StrToGuidTable](@strWorkFlowIDs, @delimiter) AS Ref
	
	INSERT INTO @WorkFlowIDs (WorkFlowID)
	SELECT W.WorkFlowID
	FROM @WorkFlowIDs AS Ref
		RIGHT JOIN [dbo].[QA_WorkFlows] AS W
		ON W.ApplicationID = @ApplicationID AND W.WorkFlowID = Ref.WorkFlowID
	WHERE W.ApplicationID = @ApplicationID AND Ref.WorkFlowID IS NULL
	ORDER BY W.SequenceNumber
	
	UPDATE [dbo].[QA_WorkFlows]
		SET SequenceNumber = Ref.SequenceNo
	FROM @WorkFlowIDs AS Ref
		INNER JOIN [dbo].[QA_WorkFlows] AS W
		ON W.WorkFlowID = Ref.WorkFlowID
	WHERE W.ApplicationID = @ApplicationID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_SetWorkFlowInitialCheckNeeded]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_SetWorkFlowInitialCheckNeeded]
GO

CREATE PROCEDURE [dbo].[QA_SetWorkFlowInitialCheckNeeded]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@Value			bit,
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[QA_WorkFlows]
		SET InitialCheckNeeded = ISNULL(@Value, 0)
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID
		
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_SetWorkFlowFinalConfirmationNeeded]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_SetWorkFlowFinalConfirmationNeeded]
GO

CREATE PROCEDURE [dbo].[QA_SetWorkFlowFinalConfirmationNeeded]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@Value			bit,
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[QA_WorkFlows]
		SET FinalConfirmationNeeded = ISNULL(@Value, 0)
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID
		
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_SetWorkFlowActionDeadline]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_SetWorkFlowActionDeadline]
GO

CREATE PROCEDURE [dbo].[QA_SetWorkFlowActionDeadline]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@Value			int,
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[QA_WorkFlows]
		SET ActionDeadline = CASE WHEN ISNULL(@Value, 0) <= 0 THEN 0 ELSE @Value END
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID
		
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_SetWorkFlowAnswerBy]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_SetWorkFlowAnswerBy]
GO

CREATE PROCEDURE [dbo].[QA_SetWorkFlowAnswerBy]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@Value			varchar(50),
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[QA_WorkFlows]
		SET AnswerBy = @Value
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID
		
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_SetWorkFlowPublishAfter]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_SetWorkFlowPublishAfter]
GO

CREATE PROCEDURE [dbo].[QA_SetWorkFlowPublishAfter]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@Value			varchar(50),
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[QA_WorkFlows]
		SET PublishAfter = @Value
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID
		
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_SetWorkFlowRemovableAfterConfirmation]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_SetWorkFlowRemovableAfterConfirmation]
GO

CREATE PROCEDURE [dbo].[QA_SetWorkFlowRemovableAfterConfirmation]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@Value			bit,
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[QA_WorkFlows]
		SET RemovableAfterConfirmation = ISNULL(@Value, 0)
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID
		
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_SetWorkFlowNodeSelectType]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_SetWorkFlowNodeSelectType]
GO

CREATE PROCEDURE [dbo].[QA_SetWorkFlowNodeSelectType]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@Value			varchar(50),
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[QA_WorkFlows]
		SET NodeSelectType = @Value
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID
		
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_SetWorkFlowDisableComments]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_SetWorkFlowDisableComments]
GO

CREATE PROCEDURE [dbo].[QA_SetWorkFlowDisableComments]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@Value			bit,
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[QA_WorkFlows]
		SET DisableComments = ISNULL(@Value, 0)
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID
		
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_SetWorkFlowDisableQuestionLikes]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_SetWorkFlowDisableQuestionLikes]
GO

CREATE PROCEDURE [dbo].[QA_SetWorkFlowDisableQuestionLikes]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@Value			bit,
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[QA_WorkFlows]
		SET DisableQuestionLikes = ISNULL(@Value, 0)
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID
		
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_SetWorkFlowDisableAnswerLikes]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_SetWorkFlowDisableAnswerLikes]
GO

CREATE PROCEDURE [dbo].[QA_SetWorkFlowDisableAnswerLikes]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@Value			bit,
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[QA_WorkFlows]
		SET DisableAnswerLikes = ISNULL(@Value, 0)
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID
		
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_SetWorkFlowDisableCommentLikes]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_SetWorkFlowDisableCommentLikes]
GO

CREATE PROCEDURE [dbo].[QA_SetWorkFlowDisableCommentLikes]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@Value			bit,
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[QA_WorkFlows]
		SET DisableCommentLikes = ISNULL(@Value, 0)
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID
		
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_SetWorkFlowDisableBestAnswer]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_SetWorkFlowDisableBestAnswer]
GO

CREATE PROCEDURE [dbo].[QA_SetWorkFlowDisableBestAnswer]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@Value			bit,
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[QA_WorkFlows]
		SET DisableBestAnswer = ISNULL(@Value, 0)
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID
		
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_RemoveWorkFlow]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_RemoveWorkFlow]
GO

CREATE PROCEDURE [dbo].[QA_RemoveWorkFlow]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@CurrentUserID	uniqueidentifier,
	@Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[QA_WorkFlows]
		SET Deleted = 1,
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_RecycleWorkFlow]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_RecycleWorkFlow]
GO

CREATE PROCEDURE [dbo].[QA_RecycleWorkFlow]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@CurrentUserID	uniqueidentifier,
	@Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[QA_WorkFlows]
		SET Deleted = 0,
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_P_GetWorkFlowsByIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_P_GetWorkFlowsByIDs]
GO

CREATE PROCEDURE [dbo].[QA_P_GetWorkFlowsByIDs]
	@ApplicationID		uniqueidentifier,
	@WorkFlowIDsTemp	KeyLessGuidTableType readonly
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @WorkFlowIDs KeyLessGuidTableType
	INSERT INTO @WorkFlowIDs (Value) SELECT Value FROM @WorkFlowIDsTemp
	
	SELECT	W.WorkFlowID,
			W.Name,
			W.[Description],
			W.InitialCheckNeeded,
			W.FinalConfirmationNeeded,
			W.ActionDeadline,
			W.AnswerBy,
			W.PublishAfter,
			W.RemovableAfterConfirmation,
			W.NodeSelectType,
			W.DisableComments,
			W.DisableQuestionLikes,
			W.DisableAnswerLikes,
			W.DisableCommentLikes,
			W.DisableBestAnswer
	FROM @WorkFlowIDs AS I
		INNER JOIN [dbo].[QA_WorkFlows] AS W
		ON W.ApplicationID = @ApplicationID AND W.WorkFlowID = I.Value
	ORDER BY I.SequenceNumber ASC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_GetWorkFlows]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_GetWorkFlows]
GO

CREATE PROCEDURE [dbo].[QA_GetWorkFlows]
	@ApplicationID	uniqueidentifier,
	@Archive		bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @WorkFlowIDs KeyLessGuidTableType
	
	INSERT INTO @WorkFlowIDs (Value)
	SELECT	W.WorkFlowID
	FROM [dbo].[QA_WorkFlows] AS W
	WHERE W.ApplicationID = @ApplicationID AND W.Deleted = ISNULL(@Archive, 0)
	ORDER BY ISNULL(W.SequenceNumber, 1000000) ASC, W.CreationDate ASC
	
	EXEC [dbo].[QA_P_GetWorkFlowsByIDs] @ApplicationID, @WorkFlowIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_GetWorkFlow]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_GetWorkFlow]
GO

CREATE PROCEDURE [dbo].[QA_GetWorkFlow]
	@ApplicationID						uniqueidentifier,
	@WorkFlowIDOrQuestionIDOrAnswerID	uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @WorkFlowIDs KeyLessGuidTableType
	
	SELECT TOP(1) @WorkFlowIDOrQuestionIDOrAnswerID = ISNULL(Q.WorkFlowID, @WorkFlowIDOrQuestionIDOrAnswerID)
	FROM [dbo].[QA_Answers] AS A
		INNER JOIN [dbo].[QA_Questions] AS Q
		ON Q.ApplicationID = @ApplicationID AND Q.QuestionID = A.QuestionID
	WHERE A.ApplicationID = @ApplicationID AND A.AnswerID = @WorkFlowIDOrQuestionIDOrAnswerID
	
	SELECT TOP(1) @WorkFlowIDOrQuestionIDOrAnswerID = ISNULL(Q.WorkFlowID, @WorkFlowIDOrQuestionIDOrAnswerID)
	FROM [dbo].[QA_Questions] AS Q
	WHERE Q.ApplicationID = @ApplicationID AND Q.QuestionID = @WorkFlowIDOrQuestionIDOrAnswerID
	
	INSERT INTO @WorkFlowIDs (Value)
	VALUES (@WorkFlowIDOrQuestionIDOrAnswerID)
	
	EXEC [dbo].[QA_P_GetWorkFlowsByIDs] @ApplicationID, @WorkFlowIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_IsWorkFlow]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_IsWorkFlow]
GO

CREATE PROCEDURE [dbo].[QA_IsWorkFlow]
	@ApplicationID	uniqueidentifier,
    @strIDs			varchar(max),
    @delimter		char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT W.WorkFlowID AS ID
	FROM [dbo].[GFN_StrToGuidTable](@strIDs, @delimter) AS Ref
		INNER JOIN [dbo].[QA_WorkFlows] AS W
		ON W.ApplicationID = @ApplicationID AND W.WorkFlowID = Ref.Value
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_AddWorkFlowAdmin]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_AddWorkFlowAdmin]
GO

CREATE PROCEDURE [dbo].[QA_AddWorkFlowAdmin]
	@ApplicationID	uniqueidentifier,
	@UserID			uniqueidentifier,
    @WorkFlowID 	uniqueidentifier,
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[QA_Admins]
	SET LastModifierUserID = @CurrentUserID,
		LastModificationDate = @Now,
		Deleted = 0
	WHERE ApplicationID = @ApplicationID AND UserID = @UserID AND
		((WorkFlowID IS NULL AND @WorkFlowID IS NULL) OR (WorkFlowID = @WorkFlowID))
	
    IF @@ROWCOUNT = 0 BEGIN
		INSERT INTO [dbo].[QA_Admins] (
			ApplicationID,
			UserID,
			WorkFlowID,
			CreatorUserID,
			CreationDate,
			Deleted
		)
		VALUES (
			@ApplicationID,
			@UserID,
			@WorkFlowID,
			@CurrentUserID,
			@Now,
			0
		)
		
		SELECT @@ROWCOUNT
    END
    ELSE SELECT 1
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_RemoveWorkFlowAdmin]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_RemoveWorkFlowAdmin]
GO

CREATE PROCEDURE [dbo].[QA_RemoveWorkFlowAdmin]
	@ApplicationID	uniqueidentifier,
	@UserID			uniqueidentifier,
    @WorkFlowID 	uniqueidentifier,
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[QA_Admins]
	SET Deleted = 1,
		LastModifierUserID = @CurrentUserID,
		LastModificationDate = @Now
	WHERE ApplicationID = @ApplicationID AND UserID = @UserID AND
		((WorkFlowID IS NULL AND @WorkFlowID IS NULL) OR (WorkFlowID = @WorkFlowID))
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_IsWorkFlowAdmin]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_IsWorkFlowAdmin]
GO

CREATE PROCEDURE [dbo].[QA_IsWorkFlowAdmin]
	@ApplicationID						uniqueidentifier,
	@UserID								uniqueidentifier,
    @WorkFlowIDOrQuestionIDOrAnswerID	uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF @WorkFlowIDOrQuestionIDOrAnswerID IS NOT NULL BEGIN
		SELECT @WorkFlowIDOrQuestionIDOrAnswerID = ISNULL(Q.WorkFlowID, @WorkFlowIDOrQuestionIDOrAnswerID)
		FROM [dbo].[QA_Answers] AS A
			INNER JOIN [dbo].[QA_Questions] AS Q
			ON Q.ApplicationID = @ApplicationID AND Q.QuestionID = A.QuestionID
		WHERE A.ApplicationID = @ApplicationID AND A.AnswerID = @WorkFlowIDOrQuestionIDOrAnswerID
		
		SELECT @WorkFlowIDOrQuestionIDOrAnswerID = ISNULL(WorkFlowID, @WorkFlowIDOrQuestionIDOrAnswerID)
		FROM [dbo].[QA_Questions]
		WHERE ApplicationID = @ApplicationID AND QuestionID = @WorkFlowIDOrQuestionIDOrAnswerID
	END
	
	SELECT 
		CASE 
			WHEN EXISTS (
				SELECT TOP(1) UserID
				FROM [dbo].[QA_Admins]
				WHERE ApplicationID = @ApplicationID AND UserID = @UserID AND
					(
						(WorkFlowID IS NULL AND @WorkFlowIDOrQuestionIDOrAnswerID IS NULL) OR 
						(WorkFlowID = @WorkFlowIDOrQuestionIDOrAnswerID)
					) AND Deleted = 0
			) THEN 1
			ELSE 0 
		END
			
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_GetWorkFlowAdminIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_GetWorkFlowAdminIDs]
GO

CREATE PROCEDURE [dbo].[QA_GetWorkFlowAdminIDs]
	@ApplicationID						uniqueidentifier,
    @WorkFlowIDOrQuestionIDOrAnswerID	uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF @WorkFlowIDOrQuestionIDOrAnswerID IS NOT NULL BEGIN
		SELECT @WorkFlowIDOrQuestionIDOrAnswerID = ISNULL(Q.WorkFlowID, @WorkFlowIDOrQuestionIDOrAnswerID)
		FROM [dbo].[QA_Answers] AS A
			INNER JOIN [dbo].[QA_Questions] AS Q
			ON Q.ApplicationID = @ApplicationID AND Q.QuestionID = A.QuestionID
		WHERE A.ApplicationID = @ApplicationID AND A.AnswerID = @WorkFlowIDOrQuestionIDOrAnswerID
		
		SELECT @WorkFlowIDOrQuestionIDOrAnswerID = ISNULL(WorkFlowID, @WorkFlowIDOrQuestionIDOrAnswerID)
		FROM [dbo].[QA_Questions]
		WHERE ApplicationID = @ApplicationID AND QuestionID = @WorkFlowIDOrQuestionIDOrAnswerID
	END
	
	SELECT AD.UserID AS ID
	FROM [dbo].[QA_Admins] AS AD
		INNER JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND UN.UserID = AD.UserID AND UN.IsApproved = 1
	WHERE AD.ApplicationID = @ApplicationID AND
		(
			(AD.WorkFlowID IS NULL AND @WorkFlowIDOrQuestionIDOrAnswerID IS NULL) OR 
			(AD.WorkFlowID = @WorkFlowIDOrQuestionIDOrAnswerID)
		) AND AD.Deleted = 0
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_SetCandidateRelations]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_SetCandidateRelations]
GO

CREATE PROCEDURE [dbo].[QA_SetCandidateRelations]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@strNodeTypeIDs	varchar(max),
	@strNodeIDs		varchar(max),
	@delimiter		char,
	@CreatorUserID	uniqueidentifier,
	@CreationDate	datetime
WITH ENCRYPTION
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	DECLARE @IDs Table(NodeTypeID uniqueidentifier, NodeID uniqueidentifier)
	
	INSERT INTO @IDs (NodeTypeID)
	SELECT Ref.Value FROM [dbo].[GFN_StrToGuidTable](@strNodeTypeIDs, @delimiter) AS Ref
	
	INSERT INTO @IDs (NodeID)
	SELECT Ref.Value FROM [dbo].[GFN_StrToGuidTable](@strNodeIDs, @delimiter) AS Ref
	
	DECLARE @ExistingIDs Table(NodeTypeID uniqueidentifier, NodeID uniqueidentifier)
	
	INSERT INTO @ExistingIDs(NodeTypeID, NodeID)
	SELECT CR.NodeTypeID, CR.NodeID
	FROM @IDs AS Ref
		INNER JOIN [dbo].[QA_CandidateRelations] AS CR
		ON CR.NodeTypeID = Ref.NodeTypeID OR CR.NodeID = Ref.NodeID
	WHERE CR.ApplicationID = @ApplicationID AND CR.WorkFlowID = @WorkFlowID
	
	DECLARE @Count int = (SELECT COUNT(*) FROM @IDs)
	DECLARE @ExistingCount int = (SELECT COUNT(*) FROM @ExistingIDs)
	
	IF EXISTS(SELECT * FROM [dbo].[QA_CandidateRelations]
		WHERE WorkFlowID = @WorkFlowID) BEGIN
		
		UPDATE [dbo].[QA_CandidateRelations]
			SET LastModifierUserID = @CreatorUserID,
				LastModificationDate = @CreationDate,
				Deleted = 1
		WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID
		
		IF @@ROWCOUNT <= 0 BEGIN
			SELECT -1
			ROLLBACK TRANSACTION
			RETURN
		END
	END
	
	IF @ExistingCount > 0 BEGIN
		UPDATE CR
			SET LastModifierUserID = @CreatorUserID,
				LastModificationDate = @CreationDate,
				Deleted = 0
		FROM @ExistingIDs AS Ref
			INNER JOIN [dbo].[QA_CandidateRelations] AS CR
			ON CR.NodeTypeID = Ref.NodeTypeID OR CR.NodeID = Ref.NodeID
		WHERE CR.ApplicationID = @ApplicationID AND CR.WorkFlowID = @WorkFlowID
		
		IF @@ROWCOUNT <= 0 BEGIN
			SELECT -1
			ROLLBACK TRANSACTION
			RETURN
		END
	END
	
	IF @Count > @ExistingCount BEGIN
		INSERT INTO [dbo].[QA_CandidateRelations](
			ApplicationID,
			ID,
			WorkFlowID,
			NodeID,
			NodeTypeID,
			CreatorUserID,
			CreationDate,
			Deleted
		)
		SELECT @ApplicationID, NEWID(), @WorkFlowID, Ref.NodeID, Ref.NodeTypeID, 
			@CreatorUserID, @CreationDate, 0
		FROM (
				SELECT I.*
				FROM @IDs AS I
					LEFT JOIN @ExistingIDs AS E
					ON I.NodeID = E.NodeID OR I.NodeTypeID = E.NodeTypeID
				WHERE E.NodeID IS NULL AND E.NodeTypeID IS NULL
			) AS Ref
		
		IF @@ROWCOUNT <= 0 BEGIN
		select 6
			SELECT -1
			ROLLBACK TRANSACTION
			RETURN
		END
	END
	
	SELECT 1
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_GetCandidateNodeRelationIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_GetCandidateNodeRelationIDs]
GO

CREATE PROCEDURE [dbo].[QA_GetCandidateNodeRelationIDs]
	@ApplicationID						uniqueidentifier,
	@WorkFlowIDOrQuestionIDOrAnswerID	uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF @WorkFlowIDOrQuestionIDOrAnswerID IS NOT NULL BEGIN
		SELECT @WorkFlowIDOrQuestionIDOrAnswerID = ISNULL(Q.WorkFlowID, @WorkFlowIDOrQuestionIDOrAnswerID)
		FROM [dbo].[QA_Answers] AS A
			INNER JOIN [dbo].[QA_Questions] AS Q
			ON Q.ApplicationID = @ApplicationID AND Q.QuestionID = A.QuestionID
		WHERE A.ApplicationID = @ApplicationID AND A.AnswerID = @WorkFlowIDOrQuestionIDOrAnswerID
		
		SELECT @WorkFlowIDOrQuestionIDOrAnswerID = ISNULL(WorkFlowID, @WorkFlowIDOrQuestionIDOrAnswerID)
		FROM [dbo].[QA_Questions]
		WHERE ApplicationID = @ApplicationID AND QuestionID = @WorkFlowIDOrQuestionIDOrAnswerID
	END
	
	DECLARE @WorkFlowID uniqueidentifier = @WorkFlowIDOrQuestionIDOrAnswerID
	
	SELECT NodeID AS ID
	FROM [dbo].[QA_CandidateRelations]
	WHERE ApplicationID = @ApplicationID AND 
		WorkFlowID = @WorkFlowID AND NodeID IS NOT NULL AND Deleted = 0
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_GetCandidateNodeTypeRelationIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_GetCandidateNodeTypeRelationIDs]
GO

CREATE PROCEDURE [dbo].[QA_GetCandidateNodeTypeRelationIDs]
	@ApplicationID						uniqueidentifier,
	@WorkFlowIDOrQuestionIDOrAnswerID	uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF @WorkFlowIDOrQuestionIDOrAnswerID IS NOT NULL BEGIN
		SELECT @WorkFlowIDOrQuestionIDOrAnswerID = ISNULL(Q.WorkFlowID, @WorkFlowIDOrQuestionIDOrAnswerID)
		FROM [dbo].[QA_Answers] AS A
			INNER JOIN [dbo].[QA_Questions] AS Q
			ON Q.ApplicationID = @ApplicationID AND Q.QuestionID = A.QuestionID
		WHERE A.ApplicationID = @ApplicationID AND A.AnswerID = @WorkFlowIDOrQuestionIDOrAnswerID
		
		SELECT @WorkFlowIDOrQuestionIDOrAnswerID = ISNULL(WorkFlowID, @WorkFlowIDOrQuestionIDOrAnswerID)
		FROM [dbo].[QA_Questions]
		WHERE ApplicationID = @ApplicationID AND QuestionID = @WorkFlowIDOrQuestionIDOrAnswerID
	END
	
	DECLARE @WorkFlowID uniqueidentifier = @WorkFlowIDOrQuestionIDOrAnswerID
	
	SELECT NodeTypeID AS ID
	FROM [dbo].[QA_CandidateRelations]
	WHERE ApplicationID = @ApplicationID AND 
		WorkFlowID = @WorkFlowID AND NodeTypeID IS NOT NULL AND Deleted = 0
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_CreateFAQCategory]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_CreateFAQCategory]
GO

CREATE PROCEDURE [dbo].[QA_CreateFAQCategory]
	@ApplicationID	uniqueidentifier,
	@CategoryID		uniqueidentifier,
	@ParentID		uniqueidentifier,
	@Name			nvarchar(200),
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SET @Name = [dbo].[GFN_VerifyString](@Name)
	
	DECLARE @SeqNo int = ISNULL((
		SELECT MAX(SequenceNumber) 
		FROM [dbo].[QA_FAQCategories]
		WHERE ApplicationID = @ApplicationID AND 
			((ParentID IS NULL AND @ParentID IS NULL) OR (ParentID = @ParentID))
	), 0) + 1
	
	INSERT INTO [dbo].[QA_FAQCategories] (
		ApplicationID,
		CategoryID,
		ParentID,
		SequenceNumber,
		Name,
		CreatorUserID,
		CreationDate,
		Deleted
	)
	VALUES (
		@ApplicationID,
		@CategoryID,
		@ParentID,
		@SeqNo,
		@Name,
		@CurrentUserID,
		@Now,
		0
	)
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_RenameFAQCategory]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_RenameFAQCategory]
GO

CREATE PROCEDURE [dbo].[QA_RenameFAQCategory]
	@ApplicationID	uniqueidentifier,
	@CategoryID		uniqueidentifier,
	@Name			nvarchar(200),
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SET @Name = [dbo].[GFN_VerifyString](@Name)
	
	UPDATE [dbo].[QA_FAQCategories]
	SET Name = @Name,
		LastModifierUserID = @CurrentUserID,
		LastModificationDate = @Now
	WHERE ApplicationID = @ApplicationID AND CategoryID = @CategoryID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_MoveFAQCategories]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_MoveFAQCategories]
GO

CREATE PROCEDURE [dbo].[QA_MoveFAQCategories]
	@ApplicationID	uniqueidentifier,
    @strCategoryIDs	varchar(max),
	@delimiter		char,
    @NewParentID	uniqueidentifier,
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @CategoryIDs GuidTableType
	INSERT INTO @CategoryIDs
	SELECT Ref.Value FROM [dbo].[GFN_StrToGuidTable](@strCategoryIDs, @delimiter) AS Ref
	
	DECLARE @ParentHierarchy NodesHierarchyTableType
	
	IF @NewParentID IS NOT NULL BEGIN
		INSERT INTO @ParentHierarchy
		SELECT *
		FROM [dbo].[QA_FN_GetParentCategoryHierarchy](@ApplicationID, @NewParentID)
	END
	
	IF EXISTS(
		SELECT TOP(1) 1
		FROM @ParentHierarchy AS P
			INNER JOIN @CategoryIDs AS C
			ON C.Value = P.NodeID
	) BEGIN
		SELECT -1, N'CannotTransferToChilds'
		RETURN
	END
	
	UPDATE C
		SET LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now,
			ParentID = @NewParentID
	FROM @CategoryIDs AS Ref
		INNER JOIN [dbo].[QA_FAQCategories] AS C
		ON C.[CategoryID] = Ref.Value
	WHERE C.ApplicationID = @ApplicationID AND 
		(@NewParentID IS NULL OR C.[CategoryID] <> @NewParentID)
	
    SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_SetFAQCategoriesOrder]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_SetFAQCategoriesOrder]
GO

CREATE PROCEDURE [dbo].[QA_SetFAQCategoriesOrder]
	@ApplicationID	uniqueidentifier,
	@strCategoryIDs	varchar(max),
	@delimiter		char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @CategoryIDs TABLE (
		SequenceNo int identity(1, 1) primary key, 
		CategoryID uniqueidentifier
	)
	
	INSERT INTO @CategoryIDs (CategoryID)
	SELECT Ref.Value
	FROM [dbo].[GFN_StrToGuidTable](@strCategoryIDs, @delimiter) AS Ref
	
	DECLARE @ParentID uniqueidentifier = NULL
	
	SELECT TOP(1) @ParentID = ParentID
	FROM [dbo].[QA_FAQCategories]
	WHERE ApplicationID = @ApplicationID AND 
		CategoryID = (SELECT TOP (1) Ref.CategoryID FROM @CategoryIDs AS Ref)
	
	INSERT INTO @CategoryIDs (CategoryID)
	SELECT C.CategoryID
	FROM @CategoryIDs AS Ref
		RIGHT JOIN [dbo].[QA_FAQCategories] AS C
		ON C.ApplicationID = @ApplicationID AND C.CategoryID = Ref.CategoryID
	WHERE C.ApplicationID = @ApplicationID AND ((C.ParentID IS NULL AND @ParentID IS NULL) OR C.ParentID = @ParentID) AND 
		Ref.CategoryID IS NULL
	ORDER BY C.SequenceNumber
	
	UPDATE [dbo].[QA_FAQCategories]
		SET SequenceNumber = Ref.SequenceNo
	FROM @CategoryIDs AS Ref
		INNER JOIN [dbo].[QA_FAQCategories] AS C
		ON C.CategoryID = Ref.CategoryID
	WHERE C.ApplicationID = @ApplicationID AND 
		((C.ParentID IS NULL AND @ParentID IS NULL) OR C.ParentID = @ParentID)
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_RemoveFAQCategories]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_RemoveFAQCategories]
GO

CREATE PROCEDURE [dbo].[QA_RemoveFAQCategories]
	@ApplicationID		uniqueidentifier,
    @strCategoryIDs		varchar(max),
    @delimiter			char,
    @RemoveHierarchy	bit,
    @CurrentUserID		uniqueidentifier,
    @Now				datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @CategoryIDs GuidTableType
	
	INSERT INTO @CategoryIDs
	SELECT DISTINCT Ref.Value FROM GFN_StrToGuidTable(@strCategoryIDs, @delimiter) AS Ref
	
	IF ISNULL(@RemoveHierarchy, 0) = 0 BEGIN
		UPDATE C
			SET Deleted = 1,
				LastModifierUserID = @CurrentUserID,
				LastModificationDate = @Now
		FROM @CategoryIDs AS Ref
			INNER JOIN [dbo].[QA_FAQCategories] AS C
			ON C.CategoryID = Ref.Value
		WHERE C.ApplicationID = @ApplicationID AND C.[Deleted] = 0
			
		DECLARE @_Result int = @@ROWCOUNT
			
		UPDATE [dbo].[QA_FAQCategories]
			SET ParentID = NULL
		WHERE ApplicationID = @ApplicationID AND ParentID IN(SELECT * FROM @CategoryIDs)
		
		SELECT @_Result
	END
	ELSE BEGIN
		UPDATE C
			SET Deleted = 1,
				LastModifierUserID = @CurrentUserID,
				LastModificationDate = @Now
		FROM [dbo].[QA_FN_GetChildCategoriesHierarchy](@ApplicationID, @CategoryIDs) AS Ref
			INNER JOIN [dbo].[QA_FAQCategories] AS C
			ON C.CategoryID = Ref.CategoryID
		WHERE C.ApplicationID = @ApplicationID
			
		SELECT @@ROWCOUNT
	END
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_GetChildFAQCategories]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_GetChildFAQCategories]
GO

CREATE PROCEDURE [dbo].[QA_GetChildFAQCategories]
	@ApplicationID		uniqueidentifier,
    @ParentID			uniqueidentifier,
    @CurrentUserID		uniqueidentifier,
    @CheckAccess		bit,
    @DefaultPrivacy		varchar(50),
    @Now				datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @Categories Table (
		CategoryID uniqueidentifier primary key clustered, 
		Name nvarchar(200),
		SequenceNumber int,
		HasChild bit
	)
	
	INSERT INTO @Categories (CategoryID, Name, SequenceNumber, HasChild)
	SELECT	C.CategoryID, 
			C.Name, 
			C.SequenceNumber,
			(
				SELECT CAST(1 AS bit)
				WHERE EXISTS (
						SELECT TOP(1) *
						FROM [dbo].[QA_FAQCategories] AS P
						WHERE P.ApplicationID = @ApplicationID AND 
							P.ParentID = C.CategoryID AND P.Deleted = 0
					)
			) AS HasChild
	FROM [dbo].[QA_FAQCategories] AS C
	WHERE C.ApplicationID = @ApplicationID AND C.Deleted = 0 AND
		((C.ParentID IS NULL AND @ParentID IS NULL) OR (C.ParentID = @ParentID))
		
	IF @CheckAccess = 1 BEGIN
		DECLARE @CIDs KeyLessGuidTableType
		
		INSERT INTO @CIDs (Value)
		SELECT C.CategoryID
		FROM @Categories AS C
			
		DECLARE	@PermissionTypes StringPairTableType
		
		INSERT INTO @PermissionTypes (FirstValue, SecondValue)
		VALUES (N'View', @DefaultPrivacy)
		
		DELETE C
		FROM @Categories AS C
			LEFT JOIN [dbo].[PRVC_FN_CheckAccess](@ApplicationID, @CurrentUserID, 
				@CIDs, N'FAQCategory', @Now, @PermissionTypes) AS A
			ON A.ID = C.CategoryID
		WHERE A.ID IS NULL
	END
	
	SELECT	C.CategoryID,
			C.Name,
			C.HasChild
	FROM @Categories AS C
	ORDER BY C.SequenceNumber ASC, C.CategoryID ASC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_IsFAQCategory]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_IsFAQCategory]
GO

CREATE PROCEDURE [dbo].[QA_IsFAQCategory]
	@ApplicationID	uniqueidentifier,
    @strIDs			varchar(max),
    @delimiter		char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT C.CategoryID AS ID
	FROM [dbo].[GFN_StrToGuidTable](@strIDs, @delimiter) AS Ref
		INNER JOIN [dbo].[QA_FAQCategories] AS C
		ON C.ApplicationID = @ApplicationID AND C.CategoryID = Ref.Value
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_AddFAQItems]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_AddFAQItems]
GO

CREATE PROCEDURE [dbo].[QA_AddFAQItems]
	@ApplicationID	uniqueidentifier,
    @CategoryID		uniqueidentifier,
    @strQuestionIDs	varchar(max),
    @delimiter		char,
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @QuestionIDs TABLE (Seq int IDENTITY(1, 1), QuestionID uniqueidentifier)
	
	INSERT INTO @QuestionIDs (QuestionID)
	SELECT DISTINCT Ref.Value
	FROM [dbo].[GFN_StrToGuidTable](@strQuestionIDs, @delimiter) AS Ref
		LEFT JOIN [dbo].[QA_FAQItems] AS I
		ON I.ApplicationID = @ApplicationID AND 
			I.CategoryID = @CategoryID AND I.QuestionID = Ref.Value
	WHERE I.QuestionID IS NULL OR I.Deleted = 1
	
	DECLARE @SeqNo int = ISNULL((
		SELECT MAX(SequenceNumber) 
		FROM [dbo].[QA_FAQItems]
		WHERE ApplicationID = @ApplicationID AND CategoryID = @CategoryID
	), 0)
	
	UPDATE I
		SET Deleted = 0,
			SequenceNumber = IDs.Seq + @SeqNo,
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	FROM @QuestionIDs AS IDs
		INNER JOIN [dbo].[QA_FAQItems] AS I
		ON I.ApplicationID = @ApplicationID AND 
			I.CategoryID = @CategoryID AND I.QuestionID = IDs.QuestionID
			
	INSERT INTO [dbo].[QA_FAQItems] (
		ApplicationID,
		CategoryID,
		QuestionID,
		SequenceNumber,
		CreatorUserID,
		CreationDate,
		Deleted
	) 
	SELECT	@ApplicationID, 
			@CategoryID, 
			IDs.QuestionID, 
			IDs.Seq + @SeqNo, 
			@CurrentUserID, 
			@Now, 
			0
	FROM @QuestionIDs AS IDs
		LEFT JOIN [dbo].[QA_FAQItems] AS I
		ON I.ApplicationID = @ApplicationID AND 
			I.CategoryID = @CategoryID AND I.QuestionID = IDs.QuestionID
	WHERE I.QuestionID IS NULL
	
	SELECT 1
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_AddQuestionToFAQCategories]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_AddQuestionToFAQCategories]
GO

CREATE PROCEDURE [dbo].[QA_AddQuestionToFAQCategories]
	@ApplicationID	uniqueidentifier,
    @QuestionID		uniqueidentifier,
    @strCategoryIDs	varchar(max),
    @delimiter		char,
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @CategoryIDs TABLE (CategoryID uniqueidentifier, Seq int)
	
	;WITH C (CategoryID)
	AS
	(
		SELECT DISTINCT C.Value AS CategoryID
		FROM [dbo].[GFN_StrToGuidTable](@strCategoryIDs, @delimiter) AS C
			LEFT JOIN [dbo].[QA_FAQItems] AS I
			ON I.ApplicationID = @ApplicationID AND 
				I.CategoryID = C.Value AND I.QuestionID = @QuestionID
		WHERE I.CategoryID IS NULL OR I.Deleted = 1
	)
	INSERT INTO @CategoryIDs (CategoryID, Seq)
	SELECT C.CategoryID, ISNULL(MAX(I.SequenceNumber), 0) + 1 AS Seq
	FROM C
		LEFT JOIN [dbo].[QA_FAQItems] AS I
		ON I.ApplicationID = @ApplicationID AND I.CategoryID = C.CategoryID
	GROUP BY C.CategoryID
	
	UPDATE I
		SET Deleted = 0,
			SequenceNumber = IDs.Seq,
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	FROM @CategoryIDs AS IDs
		INNER JOIN [dbo].[QA_FAQItems] AS I
		ON I.ApplicationID = @ApplicationID AND 
			I.CategoryID = IDs.CategoryID AND I.QuestionID = @QuestionID
			
	INSERT INTO [dbo].[QA_FAQItems] (
		ApplicationID,
		CategoryID,
		QuestionID,
		SequenceNumber,
		CreatorUserID,
		CreationDate,
		Deleted
	) 
	SELECT	@ApplicationID, 
			IDs.CategoryID, 
			@QuestionID, 
			IDs.Seq, 
			@CurrentUserID, 
			@Now, 
			0
	FROM @CategoryIDs AS IDs
		LEFT JOIN [dbo].[QA_FAQItems] AS I
		ON I.ApplicationID = @ApplicationID AND 
			I.CategoryID = IDs.CategoryID AND I.QuestionID = @QuestionID
	WHERE I.CategoryID IS NULL
	
	SELECT 1
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_RemoveFAQItem]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_RemoveFAQItem]
GO

CREATE PROCEDURE [dbo].[QA_RemoveFAQItem]
	@ApplicationID	uniqueidentifier,
    @CategoryID		uniqueidentifier,
    @QuestionID		uniqueidentifier,
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[QA_FAQItems]
	SET Deleted = 1,
		LastModifierUserID = @CurrentUserID,
		LastModificationDate = @Now
	WHERE ApplicationID = @ApplicationID AND 
		CategoryID = @CategoryID AND QuestionID = @QuestionID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_SetFAQItemsOrder]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_SetFAQItemsOrder]
GO

CREATE PROCEDURE [dbo].[QA_SetFAQItemsOrder]
	@ApplicationID	uniqueidentifier,
	@CategoryID		uniqueidentifier,
	@strQuestionIDs	varchar(max),
	@delimiter		char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @QuestionIDs TABLE (
		SequenceNo int identity(1, 1) primary key, 
		QuestionID uniqueidentifier
	)
	
	INSERT INTO @QuestionIDs (QuestionID)
	SELECT Ref.Value
	FROM [dbo].[GFN_StrToGuidTable](@strQuestionIDs, @delimiter) AS Ref
	
	INSERT INTO @QuestionIDs (QuestionID)
	SELECT I.QuestionID
	FROM @QuestionIDs AS Ref
		RIGHT JOIN [dbo].[QA_FAQItems] AS I
		ON I.ApplicationID = @ApplicationID AND I.QuestionID = Ref.QuestionID
	WHERE I.ApplicationID = @ApplicationID AND I.CategoryID = @CategoryID AND Ref.QuestionID IS NULL
	ORDER BY I.SequenceNumber
	
	UPDATE [dbo].[QA_FAQItems]
		SET SequenceNumber = Ref.SequenceNo
	FROM @QuestionIDs AS Ref
		INNER JOIN [dbo].[QA_FAQItems] AS I
		ON I.QuestionID = Ref.QuestionID
	WHERE I.ApplicationID = @ApplicationID AND I.CategoryID = @CategoryID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_AddQuestion]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_AddQuestion]
GO

CREATE PROCEDURE [dbo].[QA_AddQuestion]
	@ApplicationID		uniqueidentifier,
    @QuestionID 		uniqueidentifier,
    @Title				nvarchar(500),
    @Description		nvarchar(max),
    @Status				varchar(20),
    @PublicationDate	datetime,
    @strNodeIDs			varchar(max),
    @delimiter			char,
    @WorkFlowID			uniqueidentifier,
    @AdminID			uniqueidentifier,
    @CurrentUserID		uniqueidentifier,
    @Now   				datetime
WITH ENCRYPTION
AS
BEGIN TRANSACTION
	SET NOCOUNT ON

	SET @Title = [dbo].[GFN_VerifyString](@Title)
	SET @Description = [dbo].[GFN_VerifyString](@Description)

    INSERT INTO [dbo].[QA_Questions] (
		[ApplicationID],
        [QuestionID],
		[Title],
		[Description],
		[Status],
		[PublicationDate],
		[WorkFlowID],
		[SenderUserID],
		[SendDate],
		[Deleted]
    )
    VALUES (
		@ApplicationID,
        @QuestionID,
        @Title,
        @Description,
        @Status,
        @PublicationDate,
        @WorkFlowID,
        @CurrentUserID,
        @Now,
        0
    )
    
    /*     convert string ids to guid     */	
	DECLARE @NodeIDs GuidTableType
	
	INSERT INTO @NodeIDs
	SELECT DISTINCT Ref.Value FROM GFN_StrToGuidTable(@strNodeIDs, @delimiter) AS Ref
	/*     end of convert string ids to guid     */
    
    /*     insert related nodes     */
    DECLARE @_NodesCount int
    SET @_NodesCount = (SELECT COUNT(*) FROM @NodeIDs)
    
    INSERT INTO [dbo].[QA_RelatedNodes](
		ApplicationID,
		NodeID, 
		QuestionID, 
		CreatorUserID,
		CreationDate,
		Deleted
	)
    SELECT @ApplicationID, Ref.Value, @QuestionID, @CurrentUserID, @Now, 0
    FROM @NodeIDs AS Ref
    
    IF @_NodesCount > 0 AND @@ROWCOUNT <= 0 BEGIN
		SELECT -1
		ROLLBACK TRANSACTION
		RETURN
    END
    /*     end of insert related nodes     */
    
    
    SELECT (1 + @_NodesCount)
    
    -- Send new dashboards
    IF @AdminID IS NOT NULL BEGIN
		DECLARE @Dashboards DashboardTableType
		
		INSERT INTO @Dashboards(UserID, NodeID, RefItemID, [Type], SubType, Removable, SendDate)
		VALUES (@AdminID, @QuestionID, @QuestionID, N'Question', N'Admin', 0, @Now)
		
		DECLARE @_Result int = 0
		
		EXEC [dbo].[NTFN_P_SendDashboards] @ApplicationID, @Dashboards, @_Result output
		
		IF @_Result <= 0 BEGIN
			SELECT -1
			ROLLBACK TRANSACTION
			RETURN
		END
		ELSE BEGIN
			SELECT * 
			FROM @Dashboards
		END
	END
	-- end of send new dashboards
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_EditQuestionTitle]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_EditQuestionTitle]
GO

CREATE PROCEDURE [dbo].[QA_EditQuestionTitle]
	@ApplicationID	uniqueidentifier,
    @QuestionID		uniqueidentifier,
	@Title			nvarchar(500),
	@CurrentUserID	uniqueidentifier,
	@Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SET @Title = [dbo].[GFN_VerifyString](@Title)
	
	UPDATE [dbo].[QA_Questions]
		SET [Title] = @Title,
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	WHERE ApplicationID = @ApplicationID AND QuestionID = @QuestionID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_EditQuestionDescription]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_EditQuestionDescription]
GO

CREATE PROCEDURE [dbo].[QA_EditQuestionDescription]
	@ApplicationID	uniqueidentifier,
    @QuestionID		uniqueidentifier,
	@Description	nvarchar(max),
	@CurrentUserID	uniqueidentifier,
	@Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SET @Description = [dbo].[GFN_VerifyString](@Description)
	
	UPDATE [dbo].[QA_Questions]
		SET [Description] = @Description,
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	WHERE ApplicationID = @ApplicationID AND QuestionID = @QuestionID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_IsQuestion]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_IsQuestion]
GO

CREATE PROCEDURE [dbo].[QA_IsQuestion]
	@ApplicationID	uniqueidentifier,
    @ID				uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT TOP(1) CAST(1 AS int)
	FROM [dbo].[QA_Questions]
	WHERE ApplicationID = @ApplicationID AND QuestionID = @ID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_IsAnswer]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_IsAnswer]
GO

CREATE PROCEDURE [dbo].[QA_IsAnswer]
	@ApplicationID	uniqueidentifier,
    @ID				uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT TOP(1) CAST(1 AS int)
	FROM [dbo].[QA_Answers]
	WHERE ApplicationID = @ApplicationID AND AnswerID = @ID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_ConfirmQuestion]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_ConfirmQuestion]
GO

CREATE PROCEDURE [dbo].[QA_ConfirmQuestion]
	@ApplicationID	uniqueidentifier,
    @QuestionID		uniqueidentifier,
	@CurrentUserID	uniqueidentifier,
	@Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[QA_Questions]
		SET [Status] = N'GettingAnswers',
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	WHERE ApplicationID = @ApplicationID AND QuestionID = @QuestionID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_SetTheBestAnswer]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_SetTheBestAnswer]
GO

CREATE PROCEDURE [dbo].[QA_SetTheBestAnswer]
	@ApplicationID	uniqueidentifier,
    @QuestionID		uniqueidentifier,
    @AnswerID		uniqueidentifier,
    @Publish		bit,
	@CurrentUserID	uniqueidentifier,
	@Now			datetime
WITH ENCRYPTION
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	UPDATE [dbo].[QA_Questions]
		SET BestAnswerID = @AnswerID,
			PublicationDate = CASE WHEN @Publish = 1
				THEN ISNULL(PublicationDate, @Now) ELSE PublicationDate END,
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	WHERE @AnswerID IS NOT NULL AND ApplicationID = @ApplicationID AND QuestionID = @QuestionID
	
	IF @@ROWCOUNT <= 0 BEGIN
		SELECT -1
		ROLLBACK TRANSACTION
		RETURN
	END
	
	-- remove dashboards
	IF @Publish = 1 BEGIN
		DECLARE @_Result int = 0
    
		EXEC [dbo].[NTFN_P_ArithmeticDeleteDashboards] @ApplicationID, 
			NULL, @QuestionID, NULL, N'Question', NULL, 
			@_Result output
			
		IF @_Result <= 0 BEGIN
			SELECT -1
			ROLLBACK TRANSACTION
			RETURN
		END
	END
	-- end of remove dashboards
	
	SELECT 1
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_SetQuestionStatus]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_SetQuestionStatus]
GO

CREATE PROCEDURE [dbo].[QA_SetQuestionStatus]
	@ApplicationID		uniqueidentifier,
    @QuestionID	 		uniqueidentifier,
    @Status				varchar(50),
    @Publish			bit,
    @CurrentUserID		uniqueidentifier,
    @Now				datetime
WITH ENCRYPTION
AS
BEGIN TRANSACTION
	SET NOCOUNT ON

    UPDATE [dbo].[QA_Questions]
		SET [Status] = @Status,
			PublicationDate = CASE WHEN @Publish = 1
				THEN ISNULL(PublicationDate, @Now) ELSE PublicationDate END,
			LastModifierUserID = ISNULL(@CurrentUserID, LastModifierUserID),
			LastModificationDate = @Now
	WHERE ApplicationID = @ApplicationID AND QuestionID = @QuestionID
	
	IF @@ROWCOUNT <= 0 BEGIN
		SELECT -1
		ROLLBACK TRANSACTION
		RETURN
	END
	
	-- remove dashboards
	IF @Publish = 1 BEGIN
		DECLARE @_Result int = 0
    
		EXEC [dbo].[NTFN_P_ArithmeticDeleteDashboards] @ApplicationID, 
			NULL, @QuestionID, NULL, N'Question', NULL, 
			@_Result output
			
		IF @_Result <= 0 BEGIN
			SELECT -1
			ROLLBACK TRANSACTION
			RETURN
		END
	END
	-- end of remove dashboards
	
	SELECT 1
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_RemoveQuestion]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_RemoveQuestion]
GO

CREATE PROCEDURE [dbo].[QA_RemoveQuestion]
	@ApplicationID		uniqueidentifier,
    @QuestionID	 		uniqueidentifier,
    @CurrentUserID		uniqueidentifier,
    @Now				datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

    UPDATE [dbo].[QA_Questions]
		SET [Deleted] = 1,
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	WHERE ApplicationID = @ApplicationID AND QuestionID = @QuestionID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_P_GetQuestionsByIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_P_GetQuestionsByIDs]
GO

CREATE PROCEDURE [dbo].[QA_P_GetQuestionsByIDs]
	@ApplicationID		uniqueidentifier,
    @QuestionIDsTemp	KeyLessGuidTableType readonly,
    @CurrentUserID		uniqueidentifier
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @QuestionIDs KeyLessGuidTableType
	INSERT INTO @QuestionIDs (Value) SELECT Value FROM @QuestionIDsTemp
	
	SELECT	Q.QuestionID,
			Q.WorkFlowID,
			Q.Title,
			Q.[Description],
			Q.SendDate,
			Q.BestAnswerID,
			Q.SenderUserID,
			UN.UserName AS SenderUserName,
			UN.FirstName AS SenderFirstName,
			UN.LastName AS SenderLastName,
			Q.[Status],
			Q.PublicationDate,
			(
				SELECT COUNT(A.AnswerID)
				FROM [dbo].[QA_Answers] AS A
				WHERE A.ApplicationID = @ApplicationID AND 
					A.QuestionID = Q.QuestionID AND A.Deleted = 0
			) AS AnswersCount,
			(
				SELECT COUNT(L.UserID)
				FROM [dbo].[RV_Likes] AS L
				WHERE L.ApplicationID = @ApplicationID AND 
					L.LikedID = Q.QuestionID AND L.[Like] = 1
			) AS LikesCount,
			(
				SELECT COUNT(L.UserID)
				FROM [dbo].[RV_Likes] AS L
				WHERE L.ApplicationID = @ApplicationID AND 
					L.LikedID = Q.QuestionID AND L.[Like] = 0
			) AS DislikesCount,
			(
				SELECT TOP(1) L.[Like]
				FROM [dbo].[RV_Likes] AS L
				WHERE L.ApplicationID = @ApplicationID AND 
					L.LikedID = Q.QuestionID AND L.UserID = @CurrentUserID
			) AS LikeStatus,
			(
				SELECT TOP(1) CAST(1 AS bit)
				FROM [dbo].[RV_Followers] AS F
				WHERE F.ApplicationID = @ApplicationID AND 
					F.FollowedID = Q.QuestionID AND F.UserID = @CurrentUserID
			) AS FollowStatus
	FROM @QuestionIDs AS IDs
		INNER JOIN [dbo].[QA_Questions] AS Q
		ON Q.ApplicationID = @ApplicationID AND Q.QuestionID = IDs.Value
		INNER JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND UN.UserID = Q.SenderUserID
	ORDER BY IDs.SequenceNumber ASC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_GetQuestionsByIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_GetQuestionsByIDs]
GO

CREATE PROCEDURE [dbo].[QA_GetQuestionsByIDs]
	@ApplicationID		uniqueidentifier,
    @strQuestionIDs		varchar(max),
    @delimiter			char,
    @CurrentUserID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @QuestionIDs KeyLessGuidTableType
	
	INSERT INTO @QuestionIDs (Value)
	SELECT Ref.Value
	FROM [dbo].[GFN_StrToGuidTable](@strQuestionIDs, @delimiter) AS Ref
	
	EXEC [dbo].[QA_P_GetQuestionsByIDs] @ApplicationID, @QuestionIDs, @CurrentUserID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_GetRelatedQuestions]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_GetRelatedQuestions]
GO

CREATE PROCEDURE [dbo].[QA_GetRelatedQuestions]
	@ApplicationID		uniqueidentifier,
    @UserID		 		uniqueidentifier,
    @Groups				bit,
	@ExpertiseDomains	bit,
	@Favorites			bit,
	@Properties			bit,
	@FromFriends		bit,
	@Count				int,
	@LowerBoundary		bigint
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT TOP(ISNULL(@Count, 1000000))
		Questions.*,
		(Questions.RowNumber + Questions.RevRowNumber - 1) AS TotalCount,
		UN.UserName AS SenderUserName,
		UN.FirstName AS SenderFirstName,
		UN.LastName AS SenderLastName,
		(
			SELECT COUNT(A.AnswerID)
			FROM [dbo].[QA_Answers] AS A
			WHERE A.ApplicationID = @ApplicationID AND 
				A.QuestionID = Questions.QuestionID AND A.Deleted = 0
		) AS AnswersCount,
		(
			SELECT COUNT(L.UserID)
			FROM [dbo].[RV_Likes] AS L
			WHERE L.ApplicationID = @ApplicationID AND 
				L.LikedID = Questions.QuestionID AND L.[Like] = 1
		) AS LikesCount,
		(
			SELECT COUNT(L.UserID)
			FROM [dbo].[RV_Likes] AS L
			WHERE L.ApplicationID = @ApplicationID AND 
				L.LikedID = Questions.QuestionID AND L.[Like] = 0
		) AS DislikesCount
	FROM (
			SELECT	ROW_NUMBER() OVER (ORDER BY MAX(QS.SendDate) DESC, QS.QuestionID ASC) AS RowNumber,
					ROW_NUMBER() OVER (ORDER BY MAX(QS.SendDate) ASC, QS.QuestionID DESC) AS RevRowNumber,
					QS.QuestionID,
					MAX(QS.Title) AS Title,
					MAX(QS.SendDate) AS SendDate,
					CAST(MAX(CAST(QS.SenderUserID AS varchar(50))) AS uniqueidentifier) AS SenderUserID,
					CAST(MAX(QS.HasBestAnswer) AS bit) AS HasBestAnswer,
					MAX(QS.[Status]) AS [Status],
					COUNT(QS.RelatedNodeID) AS RelatedNodesCount,
					CAST(MAX(QS.IsGroup) AS bit) AS IsGroup,
					CAST(MAX(QS.IsExpertise) AS bit) AS IsExpertiseDomain,
					CAST(MAX(QS.IsFavorite) AS bit) AS IsFavorite,
					CAST(MAX(QS.IsProperty) AS bit) AS IsProperty,
					CAST(MAX(QS.FromFriend) AS bit) AS FromFriend
			FROM (
					SELECT	Q.QuestionID, 
							Q.Title,
							Q.SendDate,
							Q.SenderUserID,
							(CASE WHEN Q.BestAnswerID IS NULL THEN 0 ELSE 1 END) AS HasBestAnswer,
							Q.[Status],
							Nodes.NodeID AS RelatedNodeID,
							Nodes.IsGroup,
							Nodes.IsExpertise,
							Nodes.IsFavorite,
							Nodes.IsProperty,
							0 AS FromFriend
					FROM (
							SELECT	X.NodeID, 
									MAX(X.IsGroup) AS IsGroup, 
									MAX(X.IsExpertise) AS IsExpertise, 
									MAX(X.IsFavorite) AS IsFavorite, 
									MAX(X.IsProperty) AS IsProperty
							FROM (
									SELECT NodeID, 1 AS IsGroup, 0 AS IsExpertise, 0 AS IsFavorite, 0 AS IsProperty
									FROM [dbo].[CN_View_NodeMembers]
									WHERE @Groups = 1 AND ApplicationID = @ApplicationID AND UserID = @UserID AND
										ISNULL(IsPending, 0) = 0
										
									UNION ALL

									SELECT NodeID, 0 AS IsGroup, 1 AS IsExpertise, 0 AS IsFavorite, 0 AS IsProperty
									FROM [dbo].[CN_View_Experts] AS X
									WHERE @ExpertiseDomains = 1 AND ApplicationID = @ApplicationID AND UserID = @UserID

									UNION ALL

									SELECT NL.NodeID, 0 AS IsGroup, 0 AS IsExpertise, 1 AS IsFavorite, 0 AS IsProperty
									FROM [dbo].[CN_NodeLikes] AS NL
										INNER JOIN [dbo].[CN_Nodes] AS ND
										ON ND.ApplicationID = @ApplicationID AND ND.NodeID = NL.NodeID AND ND.Deleted = 0
									WHERE @Favorites = 1 AND NL.ApplicationID = @ApplicationID AND 
										NL.UserID = @UserID AND NL.Deleted = 0

									UNION ALL

									SELECT NC.NodeID, 0 AS IsGroup, 0 AS IsExpertise, 0 AS IsFavorite, 1 AS IsProperty
									FROM [dbo].[CN_NodeCreators] AS NC
										INNER JOIN [dbo].[CN_Nodes] AS ND
										ON ND.ApplicationID = @ApplicationID AND ND.NodeID = NC.NodeID AND ND.Deleted = 0
									WHERE @Properties = 1 AND NC.ApplicationID = @ApplicationID AND 
										NC.UserID = @UserID AND NC.Deleted = 0
								) AS X
							GROUP BY X.NodeID
						) AS Nodes
						INNER JOIN [dbo].[QA_RelatedNodes] AS ND
						ON ND.ApplicationID = @ApplicationID AND ND.NodeID = Nodes.NodeID
						INNER JOIN [dbo].[QA_Questions] AS Q
						ON Q.ApplicationID = @ApplicationID AND Q.QuestionID = ND.QuestionID AND 
							Q.PublicationDate IS NOT NULL AND Q.Deleted = 0
					WHERE ND.Deleted = 0

					UNION ALL

					SELECT	Q.QuestionID, 
							Q.Title,
							Q.SendDate, 
							Q.SenderUserID,
							(CASE WHEN Q.BestAnswerID IS NULL THEN 0 ELSE 1 END) AS HasBestAnswer,
							Q.[Status],
							NULL AS RelatedNodeID,
							0 AS IsGroup,
							0 AS IsExpertise,
							0 AS IsFavorite,
							0 AS IsProperty,
							1 AS FromFriend
					FROM [dbo].[QA_Questions] AS Q
						INNER JOIN [dbo].[USR_View_Friends] AS F
						ON F.ApplicationId = @ApplicationID AND F.UserID = @UserID AND 
							F.FriendID = Q.SenderUserID AND F.AreFriends = 1
					WHERE @FromFriends = 1 AND Q.PublicationDate IS NOT NULL AND Q.Deleted = 0
				) AS QS
			GROUP BY QS.QuestionID
		) AS Questions
		INNER JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND UN.UserID = Questions.SenderUserID
	WHERE Questions.RowNumber >= ISNULL(@LowerBoundary, 0)
	ORDER BY Questions.RowNumber ASC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_MyFavoriteQuestions]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_MyFavoriteQuestions]
GO

CREATE PROCEDURE [dbo].[QA_MyFavoriteQuestions]
	@ApplicationID	uniqueidentifier,
	@UserID			uniqueidentifier,
	@Count			int,
	@LowerBoundary	bigint
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT TOP(ISNULL(@Count, 1000000))
		Questions.*,
		(Questions.RowNumber + Questions.RevRowNumber - 1) AS TotalCount,
		UN.UserName AS SenderUserName,
		UN.FirstName AS SenderFirstName,
		UN.LastName AS SenderLastName,
		(
			SELECT COUNT(R.NodeID)
			FROM [dbo].[QA_RelatedNodes] AS R
			WHERE R.ApplicationID = @ApplicationID AND 
				R.QuestionID = Questions.QuestionID AND R.Deleted = 0
		) AS RelatedNodesCount,
		(
			SELECT COUNT(A.AnswerID)
			FROM [dbo].[QA_Answers] AS A
			WHERE A.ApplicationID = @ApplicationID AND 
				A.QuestionID = Questions.QuestionID AND A.Deleted = 0
		) AS AnswersCount,
		(
			SELECT COUNT(L.UserID)
			FROM [dbo].[RV_Likes] AS L
			WHERE L.ApplicationID = @ApplicationID AND 
				L.LikedID = Questions.QuestionID AND L.[Like] = 1
		) AS LikesCount,
		(
			SELECT COUNT(L.UserID)
			FROM [dbo].[RV_Likes] AS L
			WHERE L.ApplicationID = @ApplicationID AND 
				L.LikedID = Questions.QuestionID AND L.[Like] = 0
		) AS DislikesCount
	FROM (
			SELECT	ROW_NUMBER() OVER (ORDER BY Q.SendDate DESC, Q.QuestionID ASC) AS RowNumber,
					ROW_NUMBER() OVER (ORDER BY Q.SendDate ASC, Q.QuestionID DESC) AS RevRowNumber,
					Q.QuestionID, 
					Q.Title, 
					Q.SendDate, 
					Q.SenderUserID,
					CAST((CASE WHEN Q.BestAnswerID IS NULL THEN 0 ELSE 1 END) AS bit) AS HasBestAnswer,
					Q.[Status]
			FROM [dbo].[RV_Followers] AS F
				INNER JOIN [dbo].[QA_Questions] AS Q
				ON F.ApplicationID = @ApplicationID AND 
					Q.QuestionID = F.FollowedID AND Q.Deleted = 0
			WHERE F.ApplicationID = @ApplicationID AND F.UserID = @UserID -- AND L.[Like] = 1
		) AS Questions
		INNER JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND UN.UserID = Questions.SenderUserID
	WHERE Questions.RowNumber >= ISNULL(@LowerBoundary, 0)
	ORDER BY Questions.RowNumber ASC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_MyAskedQuestions]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_MyAskedQuestions]
GO

CREATE PROCEDURE [dbo].[QA_MyAskedQuestions]
	@ApplicationID	uniqueidentifier,
	@UserID			uniqueidentifier,
	@Count			int,
	@LowerBoundary	bigint
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT TOP(ISNULL(@Count, 1000000))
		Questions.*,
		(Questions.RowNumber + Questions.RevRowNumber - 1) AS TotalCount,
		UN.UserName AS SenderUserName,
		UN.FirstName AS SenderFirstName,
		UN.LastName AS SenderLastName,
		(
			SELECT COUNT(R.NodeID)
			FROM [dbo].[QA_RelatedNodes] AS R
			WHERE R.ApplicationID = @ApplicationID AND 
				R.QuestionID = Questions.QuestionID AND R.Deleted = 0
		) AS RelatedNodesCount,
		(
			SELECT COUNT(A.AnswerID)
			FROM [dbo].[QA_Answers] AS A
			WHERE A.ApplicationID = @ApplicationID AND 
				A.QuestionID = Questions.QuestionID AND A.Deleted = 0
		) AS AnswersCount,
		(
			SELECT COUNT(L.UserID)
			FROM [dbo].[RV_Likes] AS L
			WHERE L.ApplicationID = @ApplicationID AND 
				L.LikedID = Questions.QuestionID AND L.[Like] = 1
		) AS LikesCount,
		(
			SELECT COUNT(L.UserID)
			FROM [dbo].[RV_Likes] AS L
			WHERE L.ApplicationID = @ApplicationID AND 
				L.LikedID = Questions.QuestionID AND L.[Like] = 0
		) AS DislikesCount
	FROM (
			SELECT	ROW_NUMBER() OVER (ORDER BY Q.SendDate DESC, Q.QuestionID ASC) AS RowNumber,
					ROW_NUMBER() OVER (ORDER BY Q.SendDate ASC, Q.QuestionID DESC) AS RevRowNumber,
					Q.QuestionID, 
					Q.Title, 
					Q.SendDate, 
					Q.SenderUserID,
					CAST((CASE WHEN Q.BestAnswerID IS NULL THEN 0 ELSE 1 END) AS bit) AS HasBestAnswer,
					Q.[Status]
			FROM [dbo].[QA_Questions] AS Q
			WHERE Q.SenderUserID = @UserID AND Q.Deleted = 0
		) AS Questions
		INNER JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND UN.UserID = Questions.SenderUserID
	WHERE Questions.RowNumber >= ISNULL(@LowerBoundary, 0)
	ORDER BY Questions.RowNumber ASC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_QuestionsAskedOfMe]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_QuestionsAskedOfMe]
GO

CREATE PROCEDURE [dbo].[QA_QuestionsAskedOfMe]
	@ApplicationID	uniqueidentifier,
	@UserID			uniqueidentifier,
	@Count			int,
	@LowerBoundary	bigint
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT TOP(ISNULL(@Count, 1000000))
		Questions.*,
		(Questions.RowNumber + Questions.RevRowNumber - 1) AS TotalCount,
		UN.UserName AS SenderUserName,
		UN.FirstName AS SenderFirstName,
		UN.LastName AS SenderLastName,
		(
			SELECT COUNT(R.NodeID)
			FROM [dbo].[QA_RelatedNodes] AS R
			WHERE R.ApplicationID = @ApplicationID AND 
				R.QuestionID = Questions.QuestionID AND R.Deleted = 0
		) AS RelatedNodesCount,
		(
			SELECT COUNT(A.AnswerID)
			FROM [dbo].[QA_Answers] AS A
			WHERE A.ApplicationID = @ApplicationID AND 
				A.QuestionID = Questions.QuestionID AND A.Deleted = 0
		) AS AnswersCount,
		(
			SELECT COUNT(L.UserID)
			FROM [dbo].[RV_Likes] AS L
			WHERE L.ApplicationID = @ApplicationID AND 
				L.LikedID = Questions.QuestionID AND L.[Like] = 1
		) AS LikesCount,
		(
			SELECT COUNT(L.UserID)
			FROM [dbo].[RV_Likes] AS L
			WHERE L.ApplicationID = @ApplicationID AND 
				L.LikedID = Questions.QuestionID AND L.[Like] = 0
		) AS DislikesCount
	FROM (
			SELECT	ROW_NUMBER() OVER (ORDER BY Q.SendDate DESC, Q.QuestionID ASC) AS RowNumber,
					ROW_NUMBER() OVER (ORDER BY Q.SendDate ASC, Q.QuestionID DESC) AS RevRowNumber,
					Q.QuestionID, 
					Q.Title, 
					Q.SendDate, 
					Q.SenderUserID,
					CAST((CASE WHEN Q.BestAnswerID IS NULL THEN 0 ELSE 1 END) AS bit) AS HasBestAnswer,
					Q.[Status]
			FROM [dbo].[QA_RelatedUsers] AS U
				INNER JOIN [dbo].[QA_Questions] AS Q
				ON Q.ApplicationID = @ApplicationID AND 
					Q.QuestionID = U.QuestionID AND Q.Deleted = 0
			WHERE U.ApplicationID = @ApplicationID AND U.UserID = @UserID AND U.Deleted = 0
		) AS Questions
		INNER JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND UN.UserID = Questions.SenderUserID
	WHERE Questions.RowNumber >= ISNULL(@LowerBoundary, 0)
	ORDER BY Questions.RowNumber ASC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_GetFAQItems]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_GetFAQItems]
GO

CREATE PROCEDURE [dbo].[QA_GetFAQItems]
	@ApplicationID	uniqueidentifier,
	@CategoryID		uniqueidentifier,
	@Count			int,
	@LowerBoundary	bigint
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT TOP(ISNULL(@Count, 1000000))
		Questions.*,
		(Questions.RowNumber + Questions.RevRowNumber - 1) AS TotalCount,
		UN.UserName AS SenderUserName,
		UN.FirstName AS SenderFirstName,
		UN.LastName AS SenderLastName,
		(
			SELECT COUNT(R.NodeID)
			FROM [dbo].[QA_RelatedNodes] AS R
			WHERE R.ApplicationID = @ApplicationID AND 
				R.QuestionID = Questions.QuestionID AND R.Deleted = 0
		) AS RelatedNodesCount,
		(
			SELECT COUNT(A.AnswerID)
			FROM [dbo].[QA_Answers] AS A
			WHERE A.ApplicationID = @ApplicationID AND 
				A.QuestionID = Questions.QuestionID AND A.Deleted = 0
		) AS AnswersCount,
		(
			SELECT COUNT(L.UserID)
			FROM [dbo].[RV_Likes] AS L
			WHERE L.ApplicationID = @ApplicationID AND 
				L.LikedID = Questions.QuestionID AND L.[Like] = 1
		) AS LikesCount,
		(
			SELECT COUNT(L.UserID)
			FROM [dbo].[RV_Likes] AS L
			WHERE L.ApplicationID = @ApplicationID AND 
				L.LikedID = Questions.QuestionID AND L.[Like] = 0
		) AS DislikesCount
	FROM (
			SELECT	ROW_NUMBER() OVER (ORDER BY I.SequenceNumber ASC, Q.SendDate DESC, I.QuestionID ASC) AS RowNumber,
					ROW_NUMBER() OVER (ORDER BY I.SequenceNumber DESC, Q.SendDate ASC, I.QuestionID DESC) AS RevRowNumber,
					Q.QuestionID, 
					Q.Title, 
					Q.SendDate, 
					Q.SenderUserID,
					CAST((CASE WHEN Q.BestAnswerID IS NULL THEN 0 ELSE 1 END) AS bit) AS HasBestAnswer,
					Q.[Status]
			FROM [dbo].[QA_FAQItems] AS I
				INNER JOIN [dbo].[QA_Questions] AS Q
				ON Q.ApplicationID = @ApplicationID AND Q.QuestionID = I.QuestionID AND
					Q.PublicationDate IS NOT NULL AND Q.Deleted = 0
			WHERE I.ApplicationID = @ApplicationID AND I.CategoryID = @CategoryID AND I.Deleted = 0
		) AS Questions
		INNER JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND UN.UserID = Questions.SenderUserID
	WHERE Questions.RowNumber >= ISNULL(@LowerBoundary, 0)
	ORDER BY Questions.RowNumber ASC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_GetQuestions]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_GetQuestions]
GO

CREATE PROCEDURE [dbo].[QA_GetQuestions]
	@ApplicationID	uniqueidentifier,
	@SearchText		nvarchar(1000),
	@DateFrom		datetime,
	@DateTo			datetime,
	@Count			int,
	@LowerBoundary	bigint
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF @SearchText IS NULL OR @SearchText = N'' SET @SearchText = NULL
	
	IF @SearchText IS NULL BEGIN
		SELECT TOP(ISNULL(@Count, 1000000))
			Questions.*,
			(Questions.RowNumber + Questions.RevRowNumber - 1) AS TotalCount,
			UN.UserName AS SenderUserName,
			UN.FirstName AS SenderFirstName,
			UN.LastName AS SenderLastName,
			(
				SELECT COUNT(R.NodeID)
				FROM [dbo].[QA_RelatedNodes] AS R
				WHERE R.ApplicationID = @ApplicationID AND 
					R.QuestionID = Questions.QuestionID AND R.Deleted = 0
			) AS RelatedNodesCount,
			(
				SELECT COUNT(A.AnswerID)
				FROM [dbo].[QA_Answers] AS A
				WHERE A.ApplicationID = @ApplicationID AND 
					A.QuestionID = Questions.QuestionID AND A.Deleted = 0
			) AS AnswersCount,
			(
				SELECT COUNT(L.UserID)
				FROM [dbo].[RV_Likes] AS L
				WHERE L.ApplicationID = @ApplicationID AND 
					L.LikedID = Questions.QuestionID AND L.[Like] = 1
			) AS LikesCount,
			(
				SELECT COUNT(L.UserID)
				FROM [dbo].[RV_Likes] AS L
				WHERE L.ApplicationID = @ApplicationID AND 
					L.LikedID = Questions.QuestionID AND L.[Like] = 0
			) AS DislikesCount
		FROM (
				SELECT	ROW_NUMBER() OVER (ORDER BY Q.SendDate DESC, Q.QuestionID ASC) AS RowNumber,
						ROW_NUMBER() OVER (ORDER BY Q.SendDate ASC, Q.QuestionID DESC) AS RevRowNumber,
						Q.QuestionID, 
						Q.Title, 
						Q.SendDate, 
						Q.SenderUserID,
						CAST((CASE WHEN Q.BestAnswerID IS NULL THEN 0 ELSE 1 END) AS bit) AS HasBestAnswer,
						Q.[Status]
				FROM [dbo].[QA_Questions] AS Q
				WHERE Q.ApplicationID = @ApplicationID AND Q.Deleted = 0 AND
					Q.PublicationDate IS NOT NULL AND
					(@DateFrom IS NULL OR Q.SendDate >= @DateFrom) AND
					(@DateTo IS NULL OR Q.SendDate <= @DateTo)
			) AS Questions
			INNER JOIN [dbo].[Users_Normal] AS UN
			ON UN.ApplicationID = @ApplicationID AND UN.UserID = Questions.SenderUserID
		WHERE Questions.RowNumber >= ISNULL(@LowerBoundary, 0)
		ORDER BY Questions.RowNumber ASC
	END
	ELSE BEGIN
		SELECT TOP(ISNULL(@Count, 1000000))
			Questions.*,
			(Questions.RowNumber + Questions.RevRowNumber - 1) AS TotalCount,
			UN.UserName AS SenderUserName,
			UN.FirstName AS SenderFirstName,
			UN.LastName AS SenderLastName,
			(
				SELECT COUNT(R.NodeID)
				FROM [dbo].[QA_RelatedNodes] AS R
				WHERE R.ApplicationID = @ApplicationID AND 
					R.QuestionID = Questions.QuestionID AND R.Deleted = 0
			) AS RelatedNodesCount,
			(
				SELECT COUNT(A.AnswerID)
				FROM [dbo].[QA_Answers] AS A
				WHERE A.ApplicationID = @ApplicationID AND 
					A.QuestionID = Questions.QuestionID AND A.Deleted = 0
			) AS AnswersCount,
			(
				SELECT COUNT(L.UserID)
				FROM [dbo].[RV_Likes] AS L
				WHERE L.ApplicationID = @ApplicationID AND 
					L.LikedID = Questions.QuestionID AND L.[Like] = 1
			) AS LikesCount,
			(
				SELECT COUNT(L.UserID)
				FROM [dbo].[RV_Likes] AS L
				WHERE L.ApplicationID = @ApplicationID AND 
					L.LikedID = Questions.QuestionID AND L.[Like] = 0
			) AS DislikesCount
		FROM (
				SELECT	ROW_NUMBER() OVER (ORDER BY SRCH.[Rank] DESC, SRCH.[Key] ASC) AS RowNumber,
						ROW_NUMBER() OVER (ORDER BY SRCH.[Rank] ASC, SRCH.[Key] DESC) AS RevRowNumber,
						Q.QuestionID, 
						Q.Title, 
						Q.SendDate, 
						Q.SenderUserID,
						CAST((CASE WHEN Q.BestAnswerID IS NULL THEN 0 ELSE 1 END) AS bit) AS HasBestAnswer,
						Q.[Status]
				FROM CONTAINSTABLE([dbo].[QA_Questions], ([Title], [Description]), @SearchText) AS SRCH
					INNER JOIN [dbo].[QA_Questions] AS Q
					ON Q.ApplicationID = @ApplicationID AND Q.QuestionID = SRCH.[Key]
				WHERE Q.Deleted = 0 AND Q.PublicationDate IS NOT NULL AND
					(@DateFrom IS NULL OR Q.SendDate >= @DateFrom) AND
					(@DateTo IS NULL OR Q.SendDate <= @DateTo)
			) AS Questions
			INNER JOIN [dbo].[Users_Normal] AS UN
			ON UN.ApplicationID = @ApplicationID AND UN.UserID = Questions.SenderUserID
		WHERE Questions.RowNumber >= ISNULL(@LowerBoundary, 0)
		ORDER BY Questions.RowNumber ASC
	END
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_FindRelatedQuestions]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_FindRelatedQuestions]
GO

CREATE PROCEDURE [dbo].[QA_FindRelatedQuestions]
	@ApplicationID	uniqueidentifier,
	@QuestionID		uniqueidentifier,
	@Count			int,
	@LowerBoundary	bigint
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT TOP(ISNULL(@Count, 1000000))
		Questions.*,
		(Questions.RowNumber + Questions.RevRowNumber - 1) AS TotalCount,
		UN.UserName AS SenderUserName,
		UN.FirstName AS SenderFirstName,
		UN.LastName AS SenderLastName,
		(
			SELECT COUNT(R.NodeID)
			FROM [dbo].[QA_RelatedNodes] AS R
			WHERE R.ApplicationID = @ApplicationID AND 
				R.QuestionID = Questions.QuestionID AND R.Deleted = 0
		) AS RelatedNodesCount,
		(
			SELECT COUNT(A.AnswerID)
			FROM [dbo].[QA_Answers] AS A
			WHERE A.ApplicationID = @ApplicationID AND 
				A.QuestionID = Questions.QuestionID AND A.Deleted = 0
		) AS AnswersCount,
		(
			SELECT COUNT(L.UserID)
			FROM [dbo].[RV_Likes] AS L
			WHERE L.ApplicationID = @ApplicationID AND 
				L.LikedID = Questions.QuestionID AND L.[Like] = 1
		) AS LikesCount,
		(
			SELECT COUNT(L.UserID)
			FROM [dbo].[RV_Likes] AS L
			WHERE L.ApplicationID = @ApplicationID AND 
				L.LikedID = Questions.QuestionID AND L.[Like] = 0
		) AS DislikesCount
	FROM (
			SELECT	ROW_NUMBER() OVER (ORDER BY IDs.[Count] DESC, IDs.QuestionID ASC) AS RowNumber,
					ROW_NUMBER() OVER (ORDER BY IDs.[Count] ASC, IDs.QuestionID DESC) AS RevRowNumber,
					Q.QuestionID,
					Q.Title,
					Q.SendDate,
					Q.SenderUserID,
					CAST((CASE WHEN Q.BestAnswerID IS NULL THEN 0 ELSE 1 END) AS bit) AS HasBestAnswer,
					Q.[Status]
			FROM (
					SELECT R2.QuestionID, COUNT(R2.NodeID) AS [Count]
					FROM [dbo].[QA_RelatedNodes] AS R
						INNER JOIN [dbo].[QA_RelatedNodes] AS R2
						ON R2.ApplicationID = @ApplicationID AND 
							R2.NodeID = R.NodeID AND R2.Deleted = 0
					WHERE R.ApplicationID = @ApplicationID AND 
						R.QuestionID = @QuestionID AND R.Deleted = 0
					GROUP BY R2.QuestionID
				) AS IDs
				INNER JOIN [dbo].[QA_Questions] AS Q
				ON Q.ApplicationID = @ApplicationID AND Q.QuestionID = IDs.QuestionID AND 
					Q.PublicationDate IS NOT NULL AND Q.Deleted = 0
		) AS Questions
		INNER JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND UN.UserID = Questions.SenderUserID
	WHERE Questions.RowNumber >= ISNULL(@LowerBoundary, 0)
	ORDER BY Questions.RowNumber ASC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_GetQuestionsRelatedToNode]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_GetQuestionsRelatedToNode]
GO

CREATE PROCEDURE [dbo].[QA_GetQuestionsRelatedToNode]
	@ApplicationID	uniqueidentifier,
	@NodeID			uniqueidentifier,
	@SearchText		nvarchar(1000),
	@DateFrom		datetime,
	@DateTo			datetime,
	@Count			int,
	@LowerBoundary	bigint
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF @SearchText IS NULL OR @SearchText = N'' SET @SearchText = NULL
	
	IF @SearchText IS NULL BEGIN
		SELECT TOP(ISNULL(@Count, 1000000))
			Questions.*,
			(Questions.RowNumber + Questions.RevRowNumber - 1) AS TotalCount,
			UN.UserName AS SenderUserName,
			UN.FirstName AS SenderFirstName,
			UN.LastName AS SenderLastName,
			(
				SELECT COUNT(R.NodeID)
				FROM [dbo].[QA_RelatedNodes] AS R
				WHERE R.ApplicationID = @ApplicationID AND 
					R.QuestionID = Questions.QuestionID AND R.Deleted = 0
			) AS RelatedNodesCount,
			(
				SELECT COUNT(A.AnswerID)
				FROM [dbo].[QA_Answers] AS A
				WHERE A.ApplicationID = @ApplicationID AND 
					A.QuestionID = Questions.QuestionID AND A.Deleted = 0
			) AS AnswersCount,
			(
				SELECT COUNT(L.UserID)
				FROM [dbo].[RV_Likes] AS L
				WHERE L.ApplicationID = @ApplicationID AND 
					L.LikedID = Questions.QuestionID AND L.[Like] = 1
			) AS LikesCount,
			(
				SELECT COUNT(L.UserID)
				FROM [dbo].[RV_Likes] AS L
				WHERE L.ApplicationID = @ApplicationID AND 
					L.LikedID = Questions.QuestionID AND L.[Like] = 0
			) AS DislikesCount
		FROM (
				SELECT	ROW_NUMBER() OVER (ORDER BY Q.SendDate DESC, Q.QuestionID ASC) AS RowNumber,
						ROW_NUMBER() OVER (ORDER BY Q.SendDate ASC, Q.QuestionID DESC) AS RevRowNumber,
						Q.QuestionID, 
						Q.Title, 
						Q.SendDate, 
						Q.SenderUserID,
						CAST((CASE WHEN Q.BestAnswerID IS NULL THEN 0 ELSE 1 END) AS bit) AS HasBestAnswer,
						Q.[Status]
				FROM [dbo].[QA_RelatedNodes] AS R
					INNER JOIN [dbo].[QA_Questions] AS Q
					ON Q.ApplicationID = @ApplicationID AND 
						Q.QuestionID = R.QuestionID AND Q.Deleted = 0
				WHERE R.ApplicationID = @ApplicationID AND R.NodeID = @NodeID AND 
					R.Deleted = 0 AND Q.PublicationDate IS NOT NULL AND
					(@DateFrom IS NULL OR Q.SendDate >= @DateFrom) AND
					(@DateTo IS NULL OR Q.SendDate <= @DateTo)
			) AS Questions
			INNER JOIN [dbo].[Users_Normal] AS UN
			ON UN.ApplicationID = @ApplicationID AND UN.UserID = Questions.SenderUserID
		WHERE Questions.RowNumber >= ISNULL(@LowerBoundary, 0)
		ORDER BY Questions.RowNumber ASC
	END
	ELSE BEGIN
		SELECT TOP(ISNULL(@Count, 1000000))
			Questions.*,
			(Questions.RowNumber + Questions.RevRowNumber - 1) AS TotalCount,
			UN.UserName AS SenderUserName,
			UN.FirstName AS SenderFirstName,
			UN.LastName AS SenderLastName,
			(
				SELECT COUNT(R.NodeID)
				FROM [dbo].[QA_RelatedNodes] AS R
				WHERE R.ApplicationID = @ApplicationID AND 
					R.QuestionID = Questions.QuestionID AND R.Deleted = 0
			) AS RelatedNodesCount,
			(
				SELECT COUNT(A.AnswerID)
				FROM [dbo].[QA_Answers] AS A
				WHERE A.ApplicationID = @ApplicationID AND 
					A.QuestionID = Questions.QuestionID AND A.Deleted = 0
			) AS AnswersCount,
			(
				SELECT COUNT(L.UserID)
				FROM [dbo].[RV_Likes] AS L
				WHERE L.ApplicationID = @ApplicationID AND 
					L.LikedID = Questions.QuestionID AND L.[Like] = 1
			) AS LikesCount,
			(
				SELECT COUNT(L.UserID)
				FROM [dbo].[RV_Likes] AS L
				WHERE L.ApplicationID = @ApplicationID AND 
					L.LikedID = Questions.QuestionID AND L.[Like] = 0
			) AS DislikesCount
		FROM (
				SELECT	ROW_NUMBER() OVER (ORDER BY SRCH.[Rank] DESC, SRCH.[Key] ASC) AS RowNumber,
						ROW_NUMBER() OVER (ORDER BY SRCH.[Rank] ASC, SRCH.[Key] DESC) AS RevRowNumber,
						Q.QuestionID, 
						Q.Title, 
						Q.SendDate, 
						Q.SenderUserID,
						CAST((CASE WHEN Q.BestAnswerID IS NULL THEN 0 ELSE 1 END) AS bit) AS HasBestAnswer,
						Q.[Status]
				FROM CONTAINSTABLE([dbo].[QA_Questions], ([Title], [Description]), @SearchText) AS SRCH
					INNER JOIN [dbo].[QA_RelatedNodes] AS R
					INNER JOIN [dbo].[QA_Questions] AS Q
					ON Q.ApplicationID = @ApplicationID AND 
						Q.QuestionID = R.QuestionID AND Q.Deleted = 0
					ON R.ApplicationID = @ApplicationID AND 
						R.NodeID = @NodeID AND Q.QuestionID = SRCH.[Key]
				WHERE R.Deleted = 0 AND Q.PublicationDate IS NOT NULL AND
					(@DateFrom IS NULL OR Q.SendDate >= @DateFrom) AND
					(@DateTo IS NULL OR Q.SendDate <= @DateTo)
			) AS Questions
			INNER JOIN [dbo].[Users_Normal] AS UN
			ON UN.ApplicationID = @ApplicationID AND UN.UserID = Questions.SenderUserID
		WHERE Questions.RowNumber >= ISNULL(@LowerBoundary, 0)
		ORDER BY Questions.RowNumber ASC
	END
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_GroupQuestionsByRelatedNodes]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_GroupQuestionsByRelatedNodes]
GO

CREATE PROCEDURE [dbo].[QA_GroupQuestionsByRelatedNodes]
	@ApplicationID	uniqueidentifier,
	@CurrentUserID	uniqueidentifier,
	@QuestionID		uniqueidentifier,
	@SearchText		nvarchar(1000),
	@DefaultPrivacy	varchar(50),
	@CheckAccess	bit,
	@Now			datetime,
	@Count			int,
	@LowerBoundary	bigint
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @Nodes Table (NodeID uniqueidentifier primary key clustered, [Count] int)
	
	INSERT INTO @Nodes (NodeID, [Count])
	SELECT N.NodeID, COUNT(Q.QuestionID)
	FROM [dbo].[QA_RelatedNodes] AS N
		INNER JOIN [dbo].[QA_Questions] AS Q
		ON Q.ApplicationID = @ApplicationID AND Q.QuestionID = N.QuestionID AND 
			Q.PublicationDate IS NOT NULL AND Q.Deleted = 0
	WHERE N.ApplicationID = @ApplicationID AND N.Deleted = 0
	GROUP BY N.NodeID
	
	IF @QuestionID IS NOT NULL BEGIN
		DELETE @Nodes
		WHERE NodeID NOT IN (
				SELECT R.NodeID
				FROM [dbo].[QA_RelatedNodes] AS R
				WHERE R.ApplicationID = @ApplicationID AND 
						R.QuestionID = @QuestionID AND R.Deleted = 0
			)
	END
	
	IF @CheckAccess = 1 BEGIN
		DECLARE @NodeIDs KeyLessGuidTableType
	
		INSERT INTO @NodeIDs (Value)
		SELECT NodeID
		FROM @Nodes
		
		DECLARE	@PermissionTypes StringPairTableType
		
		INSERT INTO @PermissionTypes (FirstValue, SecondValue)
		VALUES (N'View', @DefaultPrivacy)
	
		DELETE N
		FROM @Nodes AS N
			LEFT JOIN [dbo].[PRVC_FN_CheckAccess](@ApplicationID, @CurrentUserID, 
				@NodeIDs, N'FAQCategory', @Now, @PermissionTypes) AS A
			ON A.ID = N.NodeID
		WHERE A.ID IS NULL
	END
	
	IF @SearchText IS NULL OR @SearchText = N'' SET @SearchText = NULL
	
	IF @SearchText IS NULL BEGIN
		SELECT TOP(ISNULL(@Count, 1000000))
			X.*,
			(X.RowNumber + x.RevRowNumber - 1) AS TotalCount,
			ND.NodeName,
			ND.TypeName AS NodeType,
			ND.Deleted
		FROM (
				SELECT	ROW_NUMBER() OVER (ORDER BY N.[Count] DESC, N.NodeID ASC) AS RowNumber,
						ROW_NUMBER() OVER (ORDER BY N.[Count] ASC, N.NodeID DESC) AS RevRowNumber,
						N.NodeID,
						N.[Count]
				FROM @Nodes AS N
			) AS X
			INNER JOIN [dbo].[CN_View_Nodes_Normal] AS ND
			ON ND.ApplicationID = @ApplicationID AND ND.NodeID = X.NodeID
		WHERE X.RowNumber >= ISNULL(@LowerBoundary, 0)
		ORDER BY X.RowNumber ASC
	END
	ELSE BEGIN
		SELECT TOP(ISNULL(@Count, 1000000))
			X.*,
			(X.RowNumber + x.RevRowNumber - 1) AS TotalCount,
			ND.NodeName,
			ND.TypeName AS NodeType,
			ND.Deleted
		FROM (
				SELECT	ROW_NUMBER() OVER (ORDER BY SRCH.[Rank] DESC, SRCH.[Key] DESC) AS RowNumber,
						ROW_NUMBER() OVER (ORDER BY SRCH.[Rank] ASC, SRCH.[Key] ASC) AS RevRowNumber,
						N.NodeID,
						N.[Count]
				FROM CONTAINSTABLE([dbo].[CN_Nodes], ([Name], [AdditionalID]), @SearchText) AS SRCH 
					INNER JOIN @Nodes AS N
					ON SRCH.[Key] = N.NodeID
			) AS X
			INNER JOIN [dbo].[CN_View_Nodes_Normal] AS ND
			ON ND.ApplicationID = @ApplicationID AND ND.NodeID = X.NodeID
		WHERE X.RowNumber >= ISNULL(@LowerBoundary, 0)
		ORDER BY X.RowNumber ASC
	END
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_FindRelatedTags]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_FindRelatedTags]
GO

CREATE PROCEDURE [dbo].[QA_FindRelatedTags]
	@ApplicationID	uniqueidentifier,
	@NodeID			uniqueidentifier,
	@Count			int,
	@LowerBoundary	bigint
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

	SELECT TOP(ISNULL(@Count, 1000000))
		X.NodeID,
		X.QuestionsCount AS [Count],
		(X.RowNumber + x.RevRowNumber - 1) AS TotalCount,
		ND.NodeName,
		ND.TypeName AS NodeType,
		ND.Deleted
	FROM (
			SELECT	ROW_NUMBER() OVER (ORDER BY Tag.[Count] DESC, Tag.QuestionsCount DESC, Tag.NodeID ASC) AS RowNumber,
					ROW_NUMBER() OVER (ORDER BY Tag.[Count] ASC, Tag.QuestionsCount ASC, Tag.NodeID DESC) AS RevRowNumber,
					Tag.*		
			FROM (
					SELECT	IDs.NodeID,
							MAX(IDs.[Count]) AS [Count],
							COUNT(Q.QuestionID) AS QuestionsCount
					FROM (
							SELECT R2.NodeID, COUNT(R2.QuestionID) AS [Count]
							FROM [dbo].[QA_RelatedNodes] AS R
								INNER JOIN [dbo].[QA_RelatedNodes] AS R2
								ON R2.ApplicationID = @ApplicationID AND 
									R2.QuestionID = R.QuestionID AND R2.Deleted = 0
							WHERE R.ApplicationID = @ApplicationID AND 
								R.NodeID = @NodeID AND R.Deleted = 0
							GROUP BY R2.NodeID
						) AS IDs
						INNER JOIN [dbo].[QA_RelatedNodes] AS R
						ON R.ApplicationID = @ApplicationID AND 
							R.NodeID = IDs.NodeID AND R.Deleted = 0
						INNER JOIN [dbo].[QA_Questions] AS Q
						ON Q.ApplicationID = @ApplicationID AND Q.QuestionID = R.QuestionID AND 
							Q.PublicationDate IS NOT NULL AND Q.Deleted = 0
					GROUP BY IDs.NodeID
				) AS Tag
		) AS X
		INNER JOIN [dbo].[CN_View_Nodes_Normal] AS ND
		ON ND.ApplicationID = @ApplicationID AND ND.NodeID = X.NodeID
	WHERE X.RowNumber >= ISNULL(@LowerBoundary, 0)
	ORDER BY X.RowNumber ASC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_CheckNodes]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_CheckNodes]
GO

CREATE PROCEDURE [dbo].[QA_CheckNodes]
	@ApplicationID	uniqueidentifier,
    @strNodeIDs		varchar(max),
    @delimiter		char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @NodeIDs GuidTableType
	
	INSERT INTO @NodeIDs (Value)
	SELECT DISTINCT Ref.Value
	FROM [dbo].[GFN_StrToGuidTable](@strNodeIDs, @delimiter) AS Ref
	
	SELECT	X.NodeID,
			X.QuestionsCount AS [Count],
			CAST(0 AS bigint) AS TotalCount,
			ND.NodeName,
			ND.TypeName AS NodeType,
			ND.Deleted
	FROM (
			SELECT	IDs.Value AS NodeID,
					COUNT(Q.QuestionID) AS QuestionsCount
			FROM @NodeIDs AS IDs
				LEFT JOIN [dbo].[QA_RelatedNodes] AS R
				ON R.ApplicationID = @ApplicationID AND 
					R.NodeID = IDs.Value AND R.Deleted = 0
				LEFT JOIN [dbo].[QA_Questions] AS Q
				ON Q.ApplicationID = @ApplicationID AND Q.QuestionID = R.QuestionID AND 
					Q.PublicationDate IS NOT NULL AND Q.Deleted = 0
			GROUP BY IDs.Value
		) AS X
		INNER JOIN [dbo].[CN_View_Nodes_Normal] AS ND
		ON ND.ApplicationID = @ApplicationID AND ND.NodeID = X.NodeID
	ORDER BY X.QuestionsCount DESC, X.NodeID ASC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_SearchNodes]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_SearchNodes]
GO

CREATE PROCEDURE [dbo].[QA_SearchNodes]
	@ApplicationID	uniqueidentifier,
    @SearchText		nvarchar(500),
    @ExactSearch	bit,
    @OrderByRank	bit,
    @Count			int,
    @LowerBoundary	bigint
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF ISNULL(@SearchText, N'') = N'' RETURN
	
	IF @ExactSearch = 1 BEGIN
		SELECT TOP(ISNULL(@Count, 1000000))
				X.NodeID,
				X.QuestionsCount AS [Count],
				(X.RowNumber + x.RevRowNumber - 1) AS TotalCount,
				X.NodeName,
				X.NodeType,
				X.Deleted
		FROM (
				SELECT	ROW_NUMBER() OVER (ORDER BY IDs.QuestionsCount DESC, IDs.NodeID ASC) AS RowNumber,
						ROW_NUMBER() OVER (ORDER BY IDs.QuestionsCount ASC, IDs.NodeID DESC) AS RevRowNumber,
						IDs.*
				FROM (
						SELECT	ND.NodeID,
								COUNT(Q.QuestionID) AS QuestionsCount,
								MAX(ND.NodeName) AS NodeName,
								MAX(ND.TypeName) AS NodeType,
								CAST(MAX(CAST(ND.Deleted AS int)) AS bit) AS Deleted
						FROM [dbo].[CN_View_Nodes_Normal] AS ND
							LEFT JOIN [dbo].[QA_RelatedNodes] AS R
							ON R.ApplicationID = @ApplicationID AND 
								R.NodeID = ND.NodeID AND R.Deleted = 0
							LEFT JOIN [dbo].[QA_Questions] AS Q
							ON Q.ApplicationID = @ApplicationID AND Q.QuestionID = R.QuestionID AND 
								Q.PublicationDate IS NOT NULL AND Q.Deleted = 0
						WHERE ND.ApplicationID = @ApplicationID AND ND.NodeName = @SearchText
						GROUP BY ND.NodeID
					) AS IDs
			) AS X
		WHERE X.RowNumber >= ISNULL(@LowerBoundary, 0)
		ORDER BY X.RowNumber ASC
	END
	ELSE BEGIN
		SELECT TOP(ISNULL(@Count, 1000000))
				X.NodeID,
				X.QuestionsCount AS [Count],
				(X.RowNumber + x.RevRowNumber - 1) AS TotalCount,
				ND.NodeName,
				ND.TypeName AS NodeType,
				ND.Deleted
		FROM (
				SELECT	ROW_NUMBER() OVER (ORDER BY (CASE WHEN @OrderByRank = 1 THEN IDs.[Rank] ELSE IDs.QuestionsCount END) DESC, IDs.NodeID ASC) AS RowNumber,
						ROW_NUMBER() OVER (ORDER BY (CASE WHEN @OrderByRank = 1 THEN IDs.[Rank] ELSE IDs.QuestionsCount END) ASC, IDs.NodeID DESC) AS RevRowNumber,
						IDs.*
				FROM (
						SELECT	SRCH.[Key] AS NodeID,
								COUNT(Q.QuestionID) AS QuestionsCount,
								MAX(SRCH.[Rank]) AS [Rank]
						FROM CONTAINSTABLE([dbo].[CN_Nodes], ([Name]), @SearchText) AS SRCH
							INNER JOIN [dbo].[CN_View_Nodes_Normal] AS ND
							ON ND.ApplicationID = @ApplicationID AND 
								ND.NodeID = SRCH.[Key] AND LEN(ND.NodeName) < 25
							LEFT JOIN [dbo].[QA_RelatedNodes] AS R
							ON R.ApplicationID = @ApplicationID AND R.NodeID = ND.NodeID AND R.Deleted = 0
							LEFT JOIN [dbo].[QA_Questions] AS Q
							ON Q.ApplicationID = @ApplicationID AND Q.QuestionID = R.QuestionID AND 
								Q.PublicationDate IS NOT NULL AND Q.Deleted = 0
						GROUP BY SRCH.[Key]
					) AS IDs
			) AS X
			INNER JOIN [dbo].[CN_View_Nodes_Normal] AS ND
			ON ND.ApplicationID = @ApplicationID AND ND.NodeID = X.NodeID
		WHERE X.RowNumber >= ISNULL(@LowerBoundary, 0)
		ORDER BY X.RowNumber ASC
	END
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_SaveRelatedNodes]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_SaveRelatedNodes]
GO

CREATE PROCEDURE [dbo].[QA_SaveRelatedNodes]
	@ApplicationID	uniqueidentifier,
    @QuestionID		uniqueidentifier,
    @strNodeIDs		varchar(max),
    @delimiter		char,
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @NodeIDs GuidTableType
	
	INSERT INTO @NodeIDs (Value)
	SELECT Ref.Value
	FROM [dbo].[GFN_StrToGuidTable](@strNodeIDs, @delimiter) AS Ref
	
	UPDATE R
		SET Deleted = (CASE WHEN N.Value IS NULL THEN 1 ELSE 0 END),
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	FROM @NodeIDs AS N
		RIGHT JOIN [dbo].[QA_RelatedNodes] AS R
		ON R.NodeID = N.Value
	WHERE R.ApplicationID = @ApplicationID AND R.QuestionID = @QuestionID
			
	INSERT INTO [dbo].[QA_RelatedNodes] (
		ApplicationID, 
		QuestionID, 
		NodeID,
		CreatorUserID,
		CreationDate,
		Deleted
	)
	SELECT @ApplicationID, @QuestionID, N.Value, @CurrentUserID, @Now, 0
	FROM @NodeIDs AS N
		LEFT JOIN [dbo].[QA_RelatedNodes] AS R
		ON R.ApplicationID = @ApplicationID AND 
			R.QuestionID = @QuestionID AND R.NodeID = N.Value
	WHERE R.NodeID IS NULL
	
	SELECT 1
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_AddRelatedNodes]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_AddRelatedNodes]
GO

CREATE PROCEDURE [dbo].[QA_AddRelatedNodes]
	@ApplicationID	uniqueidentifier,
    @QuestionID		uniqueidentifier,
    @strNodeIDs		varchar(max),
    @delimiter		char,
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @NodeIDs GuidTableType
	
	INSERT INTO @NodeIDs (Value)
	SELECT Ref.Value
	FROM [dbo].[GFN_StrToGuidTable](@strNodeIDs, @delimiter) AS Ref
	
	UPDATE R
		SET Deleted = 0,
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	FROM @NodeIDs AS N
		INNER JOIN [dbo].[QA_RelatedNodes] AS R
		ON R.ApplicationID = @ApplicationID AND 
			R.NodeID = N.Value AND R.QuestionID = @QuestionID
			
	INSERT INTO [dbo].[QA_RelatedNodes] (
		ApplicationID, 
		QuestionID, 
		NodeID,
		CreatorUserID,
		CreationDate,
		Deleted
	)
	SELECT @ApplicationID, @QuestionID, N.Value, @CurrentUserID, @Now, 0
	FROM @NodeIDs AS N
		LEFT JOIN [dbo].[QA_RelatedNodes] AS R
		ON R.ApplicationID = @ApplicationID AND 
			R.QuestionID = @QuestionID AND R.NodeID = N.Value
	WHERE R.NodeID IS NULL
	
	SELECT 1
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_RemoveRelatedNodes]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_RemoveRelatedNodes]
GO

CREATE PROCEDURE [dbo].[QA_RemoveRelatedNodes]
	@ApplicationID	uniqueidentifier,
    @QuestionID		uniqueidentifier,
    @strNodeIDs		varchar(max),
    @delimiter		char,
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @NodeIDs GuidTableType
	
	INSERT INTO @NodeIDs (Value)
	SELECT Ref.Value
	FROM [dbo].[GFN_StrToGuidTable](@strNodeIDs, @delimiter) AS Ref
	
	UPDATE R
		SET Deleted = 1,
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	FROM @NodeIDs AS N
		INNER JOIN [dbo].[QA_RelatedNodes] AS R
		ON R.ApplicationID = @ApplicationID AND 
			R.NodeID = N.Value AND R.QuestionID = @QuestionID
	
	SELECT 1
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_IsQuestionOwner]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_IsQuestionOwner]
GO

CREATE PROCEDURE [dbo].[QA_IsQuestionOwner]
	@ApplicationID				uniqueidentifier,
    @QuestionIDOrAnswerID		uniqueidentifier,
	@UserID						uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT TOP(1) @QuestionIDOrAnswerID = ISNULL(QuestionID, @QuestionIDOrAnswerID)
	FROM [dbo].[QA_Answers]
	WHERE ApplicationID = @ApplicationID AND AnswerID = @QuestionIDOrAnswerID
	
	SELECT
		CASE
			WHEN EXISTS(
				SELECT TOP(1) QuestionID
				FROM [dbo].[QA_Questions]
				WHERE ApplicationID = @ApplicationID AND 
					QuestionID = @QuestionIDOrAnswerID AND SenderUserID = @UserID
			) THEN 1
			ELSE 0
		END
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_IsAnswerOwner]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_IsAnswerOwner]
GO

CREATE PROCEDURE [dbo].[QA_IsAnswerOwner]
	@ApplicationID	uniqueidentifier,
    @AnswerID		uniqueidentifier,
	@UserID			uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT
		CASE
			WHEN EXISTS(
				SELECT TOP(1) QuestionID
				FROM [dbo].[QA_Answers]
				WHERE ApplicationID = @ApplicationID AND 
					AnswerID = @AnswerID AND SenderUserID = @UserID
			) THEN 1
			ELSE 0
		END
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_IsCommentOwner]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_IsCommentOwner]
GO

CREATE PROCEDURE [dbo].[QA_IsCommentOwner]
	@ApplicationID	uniqueidentifier,
    @CommentID		uniqueidentifier,
	@UserID			uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT
		CASE
			WHEN EXISTS(
				SELECT TOP(1) OwnerID
				FROM [dbo].[QA_Comments]
				WHERE ApplicationID = @ApplicationID AND 
					CommentID = @CommentID AND SenderUserID = @UserID
			) THEN 1
			ELSE 0
		END
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_IsRelatedUser]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_IsRelatedUser]
GO

CREATE PROCEDURE [dbo].[QA_IsRelatedUser]
	@ApplicationID	uniqueidentifier,
    @QuestionID		uniqueidentifier,
	@UserID			uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT
		CASE
			WHEN EXISTS(
				SELECT TOP(1) UserID
				FROM [dbo].[QA_RelatedUsers]
				WHERE ApplicationID = @ApplicationID AND 
					UserID = @UserID AND QuestionID = @QuestionID AND Deleted = 0
			) THEN 1
			ELSE 0
		END
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_IsRelatedExpertOrMember]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_IsRelatedExpertOrMember]
GO

CREATE PROCEDURE [dbo].[QA_IsRelatedExpertOrMember]
	@ApplicationID		uniqueidentifier,
	@QuestionID			uniqueidentifier,
	@UserID				uniqueidentifier,
	@Experts			bit,
	@Members			bit,
	@CheckCandidates	bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SET @Experts = ISNULL(@Experts, 0)
	SET @Members = ISNULL(@Members, 0)
	SET @CheckCandidates = ISNULL(@CheckCandidates, 0)

	DECLARE @Exists bit = 0

	IF @Exists = 0 AND @Experts = 1 AND @CheckCandidates = 1 BEGIN
		SELECT TOP(1) @Exists = 1
		FROM [dbo].[QA_Questions] AS Q
			INNER JOIN [dbo].[QA_RelatedNodes] AS RN
			ON Q.ApplicationID = @ApplicationID AND RN.QuestionID = Q.QuestionID
			INNER JOIN [dbo].[CN_Nodes] AS ND
			ON ND.ApplicationID = @ApplicationID AND ND.NodeID = RN.NodeID
			INNER JOIN [dbo].[QA_CandidateRelations] AS CR
			ON CR.ApplicationID = @ApplicationID AND 
				(CR.NodeID = ND.NodeID OR CR.NodeTypeID = ND.NodeTypeID) AND CR.Deleted = 0
			INNER JOIN [dbo].[CN_View_NodeMembers] AS NM
			ON NM.ApplicationID = @ApplicationID AND 
				NM.NodeID = RN.NodeID AND NM.UserID = @UserID
		WHERE RN.ApplicationID = @ApplicationID AND Q.QuestionID = @QuestionID
	END

	IF @Exists = 0 AND @Members = 1 AND @CheckCandidates = 1 BEGIN
		SELECT TOP(1) @Exists = 1
		FROM [dbo].[QA_Questions] AS Q
			INNER JOIN [dbo].[QA_RelatedNodes] AS RN
			ON Q.ApplicationID = @ApplicationID AND RN.QuestionID = Q.QuestionID
			INNER JOIN [dbo].[CN_Nodes] AS ND
			ON ND.ApplicationID = @ApplicationID AND ND.NodeID = RN.NodeID
			INNER JOIN [dbo].[QA_CandidateRelations] AS CR
			ON CR.ApplicationID = @ApplicationID AND 
				(CR.NodeID = ND.NodeID OR CR.NodeTypeID = ND.NodeTypeID) AND CR.Deleted = 0
			INNER JOIN [dbo].[CN_View_Experts] AS EX
			ON EX.ApplicationID = @ApplicationID AND 
				EX.NodeID = RN.NodeID AND EX.UserID = @UserID
		WHERE RN.ApplicationID = @ApplicationID AND Q.QuestionID = @QuestionID
	END

	IF @Exists = 0 AND @Experts = 1 AND @CheckCandidates = 0 BEGIN
		SELECT TOP(1) @Exists = 1
		FROM [dbo].[QA_RelatedNodes] AS RN
			INNER JOIN [dbo].[CN_View_Experts] AS EX
			ON EX.ApplicationID = @ApplicationID AND 
				EX.NodeID = RN.NodeID AND EX.UserID = @UserID
		WHERE RN.ApplicationID = @ApplicationID AND RN.QuestionID = QuestionID
	END

	IF @Exists = 0 AND @Members = 1 AND @CheckCandidates = 0 BEGIN
		SELECT TOP(1) @Exists = 1
		FROM [dbo].[QA_RelatedNodes] AS RN
			INNER JOIN [dbo].[CN_View_NodeMembers] AS NM
			ON NM.ApplicationID = @ApplicationID AND 
				NM.NodeID = RN.NodeID AND NM.UserID = @UserID
		WHERE RN.ApplicationID = @ApplicationID AND RN.QuestionID = QuestionID
	END

	SELECT @Exists
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_SendAnswer]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_SendAnswer]
GO

CREATE PROCEDURE [dbo].[QA_SendAnswer]
	@ApplicationID	uniqueidentifier,
	@AnswerID	 	uniqueidentifier,
    @QuestionID 	uniqueidentifier,
    @AnswerBody		nvarchar(max),
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

	SET @AnswerBody = [dbo].[GFN_VerifyString](@AnswerBody)

    INSERT INTO [dbo].[QA_Answers] (
		[ApplicationID],
		[AnswerID],
        [QuestionID],
        [SenderUserID],
        [SendDate],
        [AnswerBody],
        [Deleted]
    )
    VALUES (
		@ApplicationID,
		@AnswerID,
        @QuestionID,
        @CurrentUserID,
        @Now,
        @AnswerBody,
        0
    )
    
    SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_EditAnswer]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_EditAnswer]
GO

CREATE PROCEDURE [dbo].[QA_EditAnswer]
	@ApplicationID	uniqueidentifier,
	@AnswerID	 	uniqueidentifier,
	@AnswerBody		nvarchar(max),
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

	SET @AnswerBody = [dbo].[GFN_VerifyString](@AnswerBody)
	
	UPDATE [dbo].[QA_Answers]
		SET AnswerBody = @AnswerBody,
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	WHERE ApplicationID = @ApplicationID AND AnswerID = @AnswerID

    SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_RemoveAnswer]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_RemoveAnswer]
GO

CREATE PROCEDURE [dbo].[QA_RemoveAnswer]
	@ApplicationID	uniqueidentifier,
	@AnswerID	 	uniqueidentifier,
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

	UPDATE [dbo].[QA_Answers]
		SET Deleted = 1,
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	WHERE ApplicationID = @ApplicationID AND AnswerID = @AnswerID

    SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_P_GetAnswersByIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_P_GetAnswersByIDs]
GO

CREATE PROCEDURE [dbo].[QA_P_GetAnswersByIDs]
	@ApplicationID	uniqueidentifier,
	@AnswerIDsTemp 	KeyLessGuidTableType readonly,
	@CurrentUserID	uniqueidentifier
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @AnswerIDs KeyLessGuidTableType
	INSERT INTO @AnswerIDs (Value) SELECT Value FROM @AnswerIDsTemp

	SELECT	A.AnswerID,
			A.QuestionID,
			A.AnswerBody,
			A.SenderUserID,
			UN.UserName AS SenderUserName,
			UN.FirstName AS SenderFirstName,
			UN.LastName AS SenderLastName,
			A.SendDate,
			(
				SELECT COUNT(L.UserID)
				FROM [dbo].[RV_Likes] AS L
				WHERE L.ApplicationID = @ApplicationID AND 
					L.LikedID = A.AnswerID AND L.[Like] = 1
			) AS LikesCount,
			(
				SELECT COUNT(L.UserID)
				FROM [dbo].[RV_Likes] AS L
				WHERE L.ApplicationID = @ApplicationID AND 
					L.LikedID = A.AnswerID AND L.[Like] = 0
			) AS DislikesCount,
			(
				SELECT TOP(1) L.[Like]
				FROM [dbo].[RV_Likes] AS L
				WHERE L.ApplicationID = @ApplicationID AND 
					L.UserID = @CurrentUserID AND L.LikedID = A.AnswerID
			) AS LikeStatus
	FROM @AnswerIDs AS IDs
		INNER JOIN [dbo].[QA_Answers] AS A
		ON A.ApplicationID = @ApplicationID AND A.AnswerID = IDs.Value
		INNER JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND UN.UserID = A.SenderUserID
	ORDER BY IDs.SequenceNumber ASC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_GetAnswersByIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_GetAnswersByIDs]
GO

CREATE PROCEDURE [dbo].[QA_GetAnswersByIDs]
	@ApplicationID	uniqueidentifier,
	@strAnswerIDs	varchar(max),
	@delimiter		char,
	@CurrentUserID	uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @AnswerIDs KeyLessGuidTableType
	
	INSERT INTO @AnswerIDs (Value)
	SELECT DISTINCT Ref.Value
	FROM [dbo].[GFN_StrToGuidTable](@strAnswerIDs, @delimiter) AS Ref
	
	EXEC [dbo].[QA_P_GetAnswersByIDs] @ApplicationID, @AnswerIDs, @CurrentUserID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_GetAnswers]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_GetAnswers]
GO

CREATE PROCEDURE [dbo].[QA_GetAnswers]
	@ApplicationID	uniqueidentifier,
	@QuestionID	 	uniqueidentifier,
	@CurrentUserID	uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @AnswerIDs KeyLessGuidTableType
	
	INSERT INTO @AnswerIDs (Value)
	SELECT	A.AnswerID
	FROM [dbo].[QA_Answers] AS A
	WHERE A.ApplicationID = @ApplicationID AND 
		A.QuestionID = @QuestionID AND A.Deleted = 0
	ORDER BY A.SendDate ASC, A.AnswerID ASC
	
	EXEC [dbo].[QA_P_GetAnswersByIDs] @ApplicationID, @AnswerIDs, @CurrentUserID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_SendComment]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_SendComment]
GO

CREATE PROCEDURE [dbo].[QA_SendComment]
	@ApplicationID		uniqueidentifier,
	@CommentID	 		uniqueidentifier,
    @OwnerID	 		uniqueidentifier,
    @ReplyToCommentID	uniqueidentifier,
    @BodyText			nvarchar(max),
    @CurrentUserID		uniqueidentifier,
    @Now				datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

	SET @BodyText = [dbo].[GFN_VerifyString](@BodyText)

    INSERT INTO [dbo].[QA_Comments] (
		[ApplicationID],
		[CommentID],
        [OwnerID],
        [ReplyToCommentID],
        [BodyText],
        [SenderUserID],
        [SendDate],
        [Deleted]
    )
    VALUES (
		@ApplicationID,
		@CommentID,
        @OwnerID,
        @ReplyToCommentID,
        @BodyText,
        @CurrentUserID,
        @Now,
        0
    )
    
    SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_EditComment]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_EditComment]
GO

CREATE PROCEDURE [dbo].[QA_EditComment]
	@ApplicationID	uniqueidentifier,
	@CommentID	 	uniqueidentifier,
	@BodyText		nvarchar(max),
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

	SET @BodyText = [dbo].[GFN_VerifyString](@BodyText)
	
	UPDATE [dbo].[QA_Comments]
		SET BodyText = @BodyText,
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	WHERE ApplicationID = @ApplicationID AND CommentID = @CommentID

    SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_RemoveComment]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_RemoveComment]
GO

CREATE PROCEDURE [dbo].[QA_RemoveComment]
	@ApplicationID	uniqueidentifier,
	@CommentID	 	uniqueidentifier,
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

	UPDATE [dbo].[QA_Comments]
		SET Deleted = 1,
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	WHERE ApplicationID = @ApplicationID AND CommentID = @CommentID

    SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_GetComments]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_GetComments]
GO

CREATE PROCEDURE [dbo].[QA_GetComments]
	@ApplicationID	uniqueidentifier,
	@QuestionID	 	uniqueidentifier,
	@CurrentUserID	uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT *
	FROM (
			SELECT	C.CommentID,
					C.OwnerID,
					C.ReplyToCommentID,
					C.BodyText,
					C.SenderUserID,
					UN.UserName AS SenderUserName,
					UN.FirstName AS SenderFirstName,
					UN.LastName AS SenderLastName,
					C.SendDate,
					(
						SELECT COUNT(L.UserID)
						FROM [dbo].[RV_Likes] AS L
						WHERE L.ApplicationID = @ApplicationID AND 
							L.LikedID = C.CommentID AND L.[Like] = 1
					) AS LikesCount,
					(
						SELECT TOP(1) L.[Like]
						FROM [dbo].[RV_Likes] AS L
						WHERE L.ApplicationID = @ApplicationID AND 
							L.UserID = @CurrentUserID AND L.LikedID = C.CommentID
					) AS LikeStatus
			FROM [dbo].[QA_Comments] AS C
				INNER JOIN [dbo].[Users_Normal] AS UN
				ON UN.ApplicationID = @ApplicationID AND UN.UserID = C.SenderUserID
			WHERE C.ApplicationID = @ApplicationID AND C.OwnerID = @QuestionID AND C.Deleted = 0
			
			UNION ALL
			
			SELECT	C.CommentID,
					C.OwnerID,
					C.ReplyToCommentID,
					C.BodyText,
					C.SenderUserID,
					UN.UserName AS SenderUserName,
					UN.FirstName AS SenderFirstName,
					UN.LastName AS SenderLastName,
					C.SendDate,
					(
						SELECT COUNT(L.UserID)
						FROM [dbo].[RV_Likes] AS L
						WHERE L.ApplicationID = @ApplicationID AND 
							L.LikedID = C.CommentID AND L.[Like] = 1
					) AS LikesCount,
					(
						SELECT TOP(1) L.[Like]
						FROM [dbo].[RV_Likes] AS L
						WHERE L.ApplicationID = @ApplicationID AND 
							L.UserID = @CurrentUserID AND L.LikedID = C.CommentID
					) AS LikeStatus
			FROM [dbo].[QA_Answers] AS A
				INNER JOIN [dbo].[QA_Comments] AS C
				ON C.ApplicationID = @ApplicationID AND C.OwnerID = A.AnswerID AND C.Deleted = 0
				INNER JOIN [dbo].[Users_Normal] AS UN
				ON UN.ApplicationID = @ApplicationID AND UN.UserID = C.SenderUserID
			WHERE A.ApplicationID = @ApplicationID AND A.QuestionID = @QuestionID AND A.Deleted = 0
		) AS X
	ORDER BY X.SendDate ASC, X.CommentID ASC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_GetCommentOwnerID]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_GetCommentOwnerID]
GO

CREATE PROCEDURE [dbo].[QA_GetCommentOwnerID]
	@ApplicationID	uniqueidentifier,
	@CommentID	 	uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT TOP(1) OwnerID AS ID
	FROM [dbo].[QA_Comments]
	WHERE ApplicationID = @ApplicationID AND CommentID = @CommentID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_AddKnowledgableUser]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_AddKnowledgableUser]
GO

CREATE PROCEDURE [dbo].[QA_AddKnowledgableUser]
	@ApplicationID	uniqueidentifier,
	@QuestionID	 	uniqueidentifier,
	@UserID			uniqueidentifier,
	@CurrentUserID	uniqueidentifier,
	@Now			datetime
WITH ENCRYPTION
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	UPDATE [dbo].[QA_RelatedUsers]
		SET Deleted = 0,
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	WHERE ApplicationID = @ApplicationID AND 
		QuestionID = @QuestionID AND UserID = @UserID
		
	IF @@ROWCOUNT = 0 BEGIN
		INSERT INTO [dbo].[QA_RelatedUsers] (
			ApplicationID,
			QuestionID,
			UserID,
			SenderUserID,
			SendDate,
			Seen,
			Deleted
		)
		VALUES (
			@ApplicationID,
			@QuestionID,
			@UserID,
			@CurrentUserID,
			@Now,
			0,
			0
		)
	END
	
    -- Send new dashboards
    IF @UserID IS NOT NULL BEGIN
		DECLARE @_Result int = 0
    
		EXEC [dbo].[NTFN_P_ArithmeticDeleteDashboards] @ApplicationID, 
			@UserID, @QuestionID, NULL, N'Question', N'Knowledgable', 
			@_Result output
			
		IF @_Result <= 0 BEGIN
			SELECT -1
			ROLLBACK TRANSACTION
			RETURN
		END
    
		DECLARE @Dashboards DashboardTableType
		
		INSERT INTO @Dashboards(UserID, NodeID, RefItemID, [Type], SubType, Removable, SendDate)
		VALUES (@UserID, @QuestionID, @QuestionID, N'Question', N'Knowledgable', 0, @Now)
		
		EXEC [dbo].[NTFN_P_SendDashboards] @ApplicationID, @Dashboards, @_Result output
		
		IF @_Result <= 0 BEGIN
			SELECT -1
			ROLLBACK TRANSACTION
			RETURN
		END
		ELSE BEGIN
			SELECT * 
			FROM @Dashboards
		END
	END
	-- end of send new dashboards
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_RemoveKnowledgableUser]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_RemoveKnowledgableUser]
GO

CREATE PROCEDURE [dbo].[QA_RemoveKnowledgableUser]
	@ApplicationID	uniqueidentifier,
	@QuestionID	 	uniqueidentifier,
	@UserID			uniqueidentifier,
	@CurrentUserID	uniqueidentifier,
	@Now			datetime
WITH ENCRYPTION
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	UPDATE [dbo].[QA_RelatedUsers]
		SET Deleted = 1,
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	WHERE ApplicationID = @ApplicationID AND 
		QuestionID = @QuestionID AND UserID = @UserID
	
    -- remove dashboards
    IF @UserID IS NOT NULL BEGIN
		DECLARE @_Result int = 0
    
		EXEC [dbo].[NTFN_P_ArithmeticDeleteDashboards] @ApplicationID, 
			@UserID, @QuestionID, NULL, N'Question', N'Knowledgable', 
			@_Result output
			
		IF @_Result <= 0 BEGIN
			SELECT -1
			ROLLBACK TRANSACTION
			RETURN
		END
		ELSE SELECT 1
	END
	-- end of remove dashboards
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_GetKnowledgableUserIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_GetKnowledgableUserIDs]
GO

CREATE PROCEDURE [dbo].[QA_GetKnowledgableUserIDs]
	@ApplicationID	uniqueidentifier,
	@QuestionID	 	uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT RU.UserID AS ID
	FROM [dbo].[QA_RelatedUsers] AS RU
		INNER JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND 
			UN.UserID = RU.UserID AND UN.IsApproved = 1
	WHERE RU.ApplicationID = @ApplicationID AND 
		RU.QuestionID = @QuestionID AND RU.Deleted = 0
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_GetRelatedExpertAndMemberIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_GetRelatedExpertAndMemberIDs]
GO

CREATE PROCEDURE [dbo].[QA_GetRelatedExpertAndMemberIDs]
	@ApplicationID	uniqueidentifier,
	@QuestionID	 	uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT DISTINCT X.ID
	FROM (
			SELECT NM.UserID AS ID
			FROM [dbo].[QA_RelatedNodes] AS RN
				INNER JOIN [dbo].[CN_View_NodeMembers] AS NM
				ON NM.ApplicationID = @ApplicationID AND 
					NM.NodeID = RN.NodeID AND NM.IsPending = 0
			WHERE RN.ApplicationID = @ApplicationID AND 
				RN.QuestionID = @QuestionID
			
			UNION ALL
			
			SELECT EX.UserID AS ID
			FROM [dbo].[QA_RelatedNodes] AS RN
				INNER JOIN [dbo].[CN_View_Experts] AS EX
				ON EX.ApplicationID = @ApplicationID AND EX.NodeID = RN.NodeID
			WHERE RN.ApplicationID = @ApplicationID AND 
				RN.QuestionID = @QuestionID
		) AS X
		INNER JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND 
			UN.UserID = X.ID AND UN.IsApproved = 1
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_FindKnowledgeableUserIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_FindKnowledgeableUserIDs]
GO

CREATE PROCEDURE [dbo].[QA_FindKnowledgeableUserIDs]
	@ApplicationID	uniqueidentifier,
	@QuestionID	 	uniqueidentifier,
	@Count			int,
	@LowerBoundary	bigint
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @SenderUserID uniqueidentifier = (
		SELECT TOP(1) SenderUserID
		FROM [dbo].[QA_Questions]
		WHERE ApplicationID = @ApplicationID AND QuestionID = @QuestionID
	)
	
	DECLARE @Users TABLE (
		UserID uniqueidentifier, 
		Score float, 
		TagScore float, 
		BestAnswerScore float, 
		LikeScore float
	)
		
	;WITH Questions (QuestionID, CommonTagsCount)
	AS
	(
		SELECT	R2.QuestionID, 
				COUNT(DISTINCT R2.NodeID)
		FROM [dbo].[QA_RelatedNodes] AS R
			INNER JOIN [dbo].[QA_RelatedNodes] AS R2
			ON R2.ApplicationID = @ApplicationID AND 
				R2.QuestionID <> @QuestionID AND R2.NodeID = R.NodeID AND R2.Deleted = 0
		WHERE R.ApplicationID = @ApplicationID AND 
			R.QuestionID = @QuestionID AND R.Deleted = 0
		GROUP BY R2.QuestionID
	)
	INSERT INTO @Users (UserID, Score, TagScore, BestAnswerScore, LikeScore)
	SELECT	Scores.UserID, 
			SUM(Scores.Score), 
			SUM(Scores.TagScore), 
			SUM(Scores.BestAnswerScore),
			SUM(Scores.LikesScore)
	FROM (
			SELECT	Found.QuestionID,
					Found.UserID,
					(
						(2 * (CAST(Found.CommonTagsCount AS float) / CAST(MaxTags.[Count] AS float))) + 
						(1.5 * CAST(Found.IsBestAnswerSender AS float)) +
						(CAST(Found.LikesCount AS float) / CAST((CASE WHEN ISNULL(MaxLikes.[Count], 0) = 0 THEN 1 ELSE MaxLikes.[COUNT] END) AS float))
					) AS Score,
					(CAST(Found.CommonTagsCount AS float) / CAST(MaxTags.[Count] AS float)) AS TagScore,
					CAST(Found.IsBestAnswerSender AS float) AS BestAnswerScore,
					(CAST(Found.LikesCount AS float) / CAST((CASE WHEN ISNULL(MaxLikes.[Count], 0) = 0 THEN 1 ELSE MaxLikes.[COUNT] END) AS float)) AS LikesScore
			FROM (
					SELECT	QU.QuestionID,
							QU.UserID,
							MAX(QU.CommonTagsCount) AS CommonTagsCount,
							MAX(QU.IsBestAnswerSender) AS IsBestAnswerSender,
							MAX(QU.LikesCount) AS LikesCount
					FROM (
						SELECT	A.QuestionID, 
								Questions.CommonTagsCount,
								A.SenderUserID AS UserID, 
								CAST(1 AS int) IsBestAnswerSender,
								0 AS LikesCount
						FROM Questions
							INNER JOIN [dbo].[QA_Questions] AS Q
							ON Q.ApplicationID = @ApplicationID AND 
								Q.QuestionID = Questions.QuestionID AND Q.BestAnswerID IS NOT NULL
							INNER JOIN [dbo].[QA_Answers] AS A
							ON A.ApplicationID = @ApplicationID AND A.AnswerID = Q.BestAnswerID
						
						UNION ALL
						
						SELECT	X.QuestionID, 
								MAX(X.CommonTagsCount) AS CommonTagsCount,
								X.SenderUserID AS UserID, 
								CAST(0 AS int) IsBestAnswerSender,
								MAX(X.LikesCount) AS LikesCount
						FROM (
								SELECT	A.QuestionID, 
										MAX(Questions.CommonTagsCount) AS CommonTagsCount,
										A.SenderUserID, 
										SUM(
											CASE 
												WHEN L.[Like] IS NULL THEN 0
												WHEN L.[Like] = 0 THEN -1
												ELSE 1
											END
										) AS LikesCount
								FROM Questions
									INNER JOIN [dbo].[QA_Answers] AS A
									ON A.ApplicationID = @ApplicationID AND 
										A.QuestionID = Questions.QuestionID AND A.Deleted = 0
									LEFT JOIN [dbo].[RV_Likes] AS L
									ON L.ApplicationID = @ApplicationID AND L.LikedID = A.AnswerID
								GROUP BY A.QuestionID, A.AnswerID, A.SenderUserID
							) AS X
						WHERE X.LikesCount >= 0
						GROUP BY X.QuestionID, X.SenderUserID
					) AS QU
					GROUP BY QU.QuestionID, QU.UserID
				) AS Found
				CROSS JOIN (
					SELECT MAX(CommonTagsCount) AS [Count]
					FROM Questions
				) AS MaxTags
				LEFT JOIN (
					SELECT	X.QuestionID, 
							MAX(X.LikesCount) AS [Count]
					FROM (
							SELECT	A.QuestionID, 
									SUM(
										CASE 
											WHEN L.[Like] IS NULL THEN 0
											WHEN L.[Like] = 0 THEN -1
											ELSE 1
										END
									) AS LikesCount
							FROM Questions
								INNER JOIN [dbo].[QA_Answers] AS A
								ON A.ApplicationID = @ApplicationID AND 
									A.QuestionID = Questions.QuestionID AND A.Deleted = 0
								LEFT JOIN [dbo].[RV_Likes] AS L
								ON L.ApplicationID = @ApplicationID AND L.LikedID = A.AnswerID
							GROUP BY A.QuestionID, A.AnswerID, A.SenderUserID
						) AS X
					WHERE X.LikesCount >= 0
					GROUP BY X.QuestionID
				) AS MaxLikes
				ON MaxLikes.QuestionID = Found.QuestionID
		) AS Scores
	GROUP BY Scores.UserID
	
	SELECT TOP(ISNULL(@Count, 1000000))
		(X.RowNumber + X.RevRowNumber - 1) AS TotalCount,
		X.UserID AS ID
	FROM (
			SELECT	ROW_NUMBER() OVER (ORDER BY U.Score DESC, U.UserID ASC) AS RowNumber,
					ROW_NUMBER() OVER (ORDER BY U.Score ASC, U.UserID DESC) AS RevRowNumber,
					U.*
			FROM @Users AS U
				INNER JOIN [dbo].[Users_Normal] AS UN
				ON UN.ApplicationID = @ApplicationID AND 
					UN.UserID = U.UserID AND UN.IsApproved = 1
			WHERE U.UserID <> @SenderUserID
		) AS X
	WHERE X.RowNumber >= ISNULL(@LowerBoundary, 0)
	ORDER BY X.RowNumber ASC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_GetQuestionAskerID]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_GetQuestionAskerID]
GO

CREATE PROCEDURE [dbo].[QA_GetQuestionAskerID]
	@ApplicationID	uniqueidentifier,
    @QuestionID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT SenderUserID
	FROM [dbo].[QA_Questions]
	WHERE ApplicationID = @ApplicationID AND QuestionID = @QuestionID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_SearchQuestions]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_SearchQuestions]
GO

CREATE PROCEDURE [dbo].[QA_SearchQuestions]
	@ApplicationID	uniqueidentifier,
    @SearchText		nvarchar(512),
    @UserID			uniqueidentifier,
    @Count			int,
    @MinID			uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SET @SearchText = [dbo].[GFN_VerifyString](@SearchText)
	
	DECLARE @TempIDs Table(sqno bigint IDENTITY(1, 1), QuestionID uniqueidentifier)
	DECLARE @QuestionIDs KeyLessGuidTableType
	
	DECLARE @_ST nvarchar(1000) = @SearchText
	IF @SearchText IS NULL OR @SearchText = N'' SET @_ST = NULL
	
	IF @_ST IS NULL BEGIN
		INSERT INTO @TempIDs
		SELECT QU.[QuestionID] 
		FROM [dbo].[QA_Questions] AS QU
		WHERE QU.ApplicationID = @ApplicationID AND 
			QU.PublicationDate IS NOT NULL AND QU.Deleted = 0
	END
	ELSE BEGIN
		INSERT INTO @TempIDs
		SELECT QU.[QuestionID] 
		FROM [dbo].[QA_Questions] AS QU
		WHERE QU.ApplicationID = @ApplicationID AND 
			QU.PublicationDate IS NOT NULL AND QU.Deleted = 0 AND
			CONTAINS((QU.[Title], QU.[Description]), @_ST)
	END
	
	DECLARE @Loc bigint = 0
	IF @MinID IS NOT NULL 
		SET @Loc = (SELECT TOP(1) Ref.sqno FROM @TempIDs AS Ref WHERE Ref.QuestionID = @MinID)
	IF @Loc IS NULL SET @Loc = 0
	
	INSERT INTO @QuestionIDs (Value)
	SELECT TOP(@Count) Ref.QuestionID FROM @TempIDs AS Ref WHERE Ref.sqno > @Loc
	
	EXEC [dbo].[QA_P_GetQuestionsByIDs] @ApplicationID, @QuestionIDs, @UserID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_GetQuestionsCount]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_GetQuestionsCount]
GO

CREATE PROCEDURE [dbo].[QA_GetQuestionsCount]
	@ApplicationID			uniqueidentifier,
    @Published				bit,
    @CreationDateLowerLimit datetime,
    @CreationDateUpperLimit datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF @Published IS NULL SET @Published = 0
	
	SELECT COUNT(*) 
	FROM [dbo].[QA_Questions] AS Q
	WHERE Q.ApplicationID = @ApplicationID AND
		(@CreationDateLowerLimit IS NULL OR Q.[SendDate] >= @CreationDateLowerLimit) AND
		(@CreationDateUpperLimit IS NULL OR Q.[SendDate] <= @CreationDateUpperLimit) AND
		(@Published = 0 OR (@Published = 1 AND Q.PublicationDate IS NOT NULL)) AND 
		Q.[Deleted] = 0
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_GetAnswerSenderIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_GetAnswerSenderIDs]
GO

CREATE PROCEDURE [dbo].[QA_GetAnswerSenderIDs]
	@ApplicationID	uniqueidentifier,
    @QuestionID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

    SELECT SenderUserID AS ID
	FROM [dbo].[QA_Answers]
	WHERE ApplicationID = @ApplicationID AND 
		QuestionID = @QuestionID AND Deleted = 0
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[QA_GetExistingQuestionIDs]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[QA_GetExistingQuestionIDs]
GO

CREATE PROCEDURE [dbo].[QA_GetExistingQuestionIDs]
	@ApplicationID	uniqueidentifier,
	@strQuestionIDs	varchar(max),
	@delimiter		char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT QuestionID AS ID
	FROM [dbo].[GFN_StrToGuidTable](@strQuestionIDs, @delimiter) AS IDs
		INNER JOIN [dbo].[QA_Questions] AS Q
		ON Q.QuestionID = IDs.Value
	WHERE Q.ApplicationID = @ApplicationID AND Q.Deleted = 0
END

GO