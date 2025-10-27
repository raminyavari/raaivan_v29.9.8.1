USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_P_SendDashboards]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_P_SendDashboards]
GO

CREATE PROCEDURE [dbo].[WF_P_SendDashboards]
	@ApplicationID		uniqueidentifier,
	@HistoryID			uniqueidentifier,
	@NodeID				uniqueidentifier,
	@WorkFlowID			uniqueidentifier,
	@StateID			uniqueidentifier,
	@DirectorUserID		uniqueidentifier,
	@DirectorNodeID		uniqueidentifier,
	@DataNeedInstanceID	uniqueidentifier,
	@SendDate			datetime,
	@_Result			int output
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @OnlySendDataNeed bit = 0
	IF @DataNeedInstanceID IS NOT NULL BEGIN
		SET @OnlySendDataNeed = 1
		
		IF @HistoryID IS NULL SET @HistoryID = (
			SELECT TOP(1) HistoryID
			FROM [dbo].[WF_StateDataNeedInstances]
			WHERE ApplicationID = @ApplicationID AND InstanceID = @DataNeedInstanceID
		)
		
		IF @WorkFlowID IS NULL OR @StateID IS NULL OR @NodeID IS NULL BEGIN
			SELECT @WorkFlowID = WorkFlowID, @StateID = StateID, @NodeID = OwnerID
			FROM [dbo].[WF_History]
			WHERE ApplicationID = @ApplicationID AND HistoryID = @HistoryID
		END
	END
	
	DECLARE @Dashboards DashboardTableType
	
	DECLARE @WorkFlowName nvarchar(1000) = (
		SELECT TOP(1) Name 
		FROM [dbo].[WF_WorkFlows] 
		WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID
	)
	
	DECLARE @StateTitle nvarchar(1000) = (
		SELECT TOP(1) Title 
		FROM [dbo].[WF_States] 
		WHERE ApplicationID = @ApplicationID AND StateID = @StateID
	)
	
	INSERT INTO @Dashboards(UserID, NodeID, RefItemID, [Type], Info, Removable, SendDate)
	SELECT	NM.UserID, 
			@NodeID, 
			SDNI.InstanceID, 
			N'WorkFlow',
			[dbo].[WF_FN_GetDashboardInfo](@WorkFlowName, @StateTitle, SDNI.InstanceID),
			0,
			@SendDate
	FROM [dbo].[WF_StateDataNeedInstances] AS SDNI
		INNER JOIN [dbo].[CN_View_NodeMembers] AS NM
		ON NM.ApplicationID = @ApplicationID AND NM.NodeID = SDNI.NodeID AND NM.IsPending = 0
	WHERE SDNI.ApplicationID = @ApplicationID AND (
			(@OnlySendDataNeed = 1 AND SDNI.InstanceID = @DataNeedInstanceID) OR
			(@OnlySendDataNeed = 0 AND SDNI.HistoryID = @HistoryID)
		) AND
		(SDNI.[Admin] = 0 OR SDNI.[Admin] = NM.IsAdmin) AND SDNI.Deleted = 0
	
	IF @OnlySendDataNeed = 0 BEGIN
		DECLARE @Info nvarchar(max) = 
			[dbo].[WF_FN_GetDashboardInfo](@WorkFlowName, @StateTitle, @DataNeedInstanceID)
	
		IF @DirectorUserID IS NOT NULL BEGIN
			INSERT INTO @Dashboards(UserID, NodeID, RefItemID, [Type], Info, Removable, SendDate)
			VALUES (@DirectorUserID, @NodeID, @HistoryID, N'WorkFlow', @Info, 0, @SendDate)
		END
	
		IF @DirectorNodeID IS NOT NULL BEGIN
			DECLARE @IsDirectorNodeAdmin bit = (
				SELECT TOP(1) [Admin]
				FROM [dbo].[WF_WorkFlowStates]
				WHERE ApplicationID = @ApplicationID AND 
					WorkFlowID = @WorkFlowID AND StateID = @StateID AND Deleted = 0
			)
		
			INSERT INTO @Dashboards(UserID, NodeID, RefItemID, [Type], Info, Removable, SendDate)
			SELECT	NM.UserID, @NodeID, @HistoryID, N'WorkFlow', @Info, 
				CASE
					WHEN ISNULL(@IsDirectorNodeAdmin, 0) = 0 OR NM.IsAdmin = 1 THEN CAST(0 AS bit)
					ELSE CAST(1 AS bit)
				END,
				@SendDate
			FROM [dbo].[CN_View_NodeMembers] AS NM
			WHERE ApplicationID = @ApplicationID AND NodeID = @DirectorNodeID AND 
				NM.IsPending = 0 AND
				NOT EXISTS(
					SELECT TOP(1) * 
					FROM @Dashboards AS Ref 
					WHERE Ref.UserID = NM.UserID
				)
		END
		
		EXEC [dbo].[NTFN_P_ArithmeticDeleteDashboards] @ApplicationID, 
			NULL, @NodeID, NULL, N'WorkFlow', NULL, @_Result output
		
		IF @_Result <= 0 RETURN
	END  -- end of 'IF @OnlySendDataNeed = 1 BEGIN'
	
	IF (SELECT COUNT(*) FROM @Dashboards) = 0 BEGIN
		SET @_Result = -1
		RETURN
	END
	
	EXEC [dbo].[NTFN_P_SendDashboards] @ApplicationID, @Dashboards, @_Result output
	
	IF @_Result <= 0 RETURN
	
	IF @_Result > 0 BEGIN
		SELECT * 
		FROM @Dashboards
	END
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_CreateState]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_CreateState]
GO

CREATE PROCEDURE [dbo].[WF_CreateState]
	@ApplicationID		uniqueidentifier,
	@StateID			uniqueidentifier,
	@Title				nvarchar(255),
	@CreatorUserID		uniqueidentifier,
	@CreationDate		datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @_RetVal int
	SET @_RetVal = 0
	
	SET @Title = [dbo].[GFN_VerifyString](@Title)
	
	INSERT INTO [dbo].[WF_States](
		ApplicationID,
		StateID,
		Title,
		CreatorUserID,
		CreationDate,
		Deleted
	)
	VALUES(
		@ApplicationID,
		@StateID,
		@Title,
		@CreatorUserID,
		@CreationDate,
		0
	)
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_ModifyState]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_ModifyState]
GO

CREATE PROCEDURE [dbo].[WF_ModifyState]
	@ApplicationID			uniqueidentifier,
	@StateID				uniqueidentifier,
	@Title					nvarchar(255),
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SET @Title = [dbo].[GFN_VerifyString](@Title)
	
	UPDATE [dbo].[WF_States]
		SET Title = @Title,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND StateID = @StateID
		
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_ArithmeticDeleteState]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_ArithmeticDeleteState]
GO

CREATE PROCEDURE [dbo].[WF_ArithmeticDeleteState]
	@ApplicationID			uniqueidentifier,
	@StateID				uniqueidentifier,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[WF_States]
		SET LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate,
			Deleted = 1
	WHERE ApplicationID = @ApplicationID AND StateID = @StateID
		
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_P_GetStatesByIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_P_GetStatesByIDs]
GO

CREATE PROCEDURE [dbo].[WF_P_GetStatesByIDs]
	@ApplicationID	uniqueidentifier,
	@StateIDsTemp	GuidTableType readonly
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @StateIDs GuidTableType
	INSERT INTO @StateIDs SELECT * FROM @StateIDsTemp
	
	SELECT ST.StateID AS StateID,
		   ST.Title AS Title
	FROM @StateIDs AS ExternalIDs
		INNER JOIN [dbo].[WF_States] AS ST
		ON ST.ApplicationID = @ApplicationID AND ST.StateID = ExternalIDs.Value
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetStatesByIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetStatesByIDs]
GO

CREATE PROCEDURE [dbo].[WF_GetStatesByIDs]
	@ApplicationID	uniqueidentifier,
	@strStateIDs	varchar(max),
	@delimiter		char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @StateIDs GuidTableType
	INSERT @StateIDs
	SELECT DISTINCT Ref.Value FROM [dbo].[GFN_StrToGuidTable](@strStateIDs, @delimiter) AS Ref
	
	EXEC [dbo].[WF_P_GetStatesByIDs] @ApplicationID, @StateIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetStates]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetStates]
GO

CREATE PROCEDURE [dbo].[WF_GetStates]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @StateIDs GuidTableType
	
	IF @WorkFlowID IS NULL BEGIN
		INSERT INTO @StateIDs
		SELECT StateID
		FROM [dbo].[WF_States]
		WHERE ApplicationID = @ApplicationID AND Deleted = 0
	END
	ELSE BEGIN
		INSERT INTO @StateIDs
		SELECT StateID
		FROM [dbo].[WF_WorkFlowStates]
		WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND Deleted = 0
	END
	
	EXEC [dbo].[WF_P_GetStatesByIDs] @ApplicationID, @StateIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_CreateWorkFlow]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_CreateWorkFlow]
GO

CREATE PROCEDURE [dbo].[WF_CreateWorkFlow]
	@ApplicationID		uniqueidentifier,
    @WorkFlowID			uniqueidentifier,
	@Name				nvarchar(255),
	@Description		nvarchar(2000),
	@CreatorUserID		uniqueidentifier,
	@CreationDate		datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @_RetVal int
	SET @_RetVal = 0
	
	SET @Name = [dbo].[GFN_VerifyString](@Name)
	SET @Description = [dbo].[GFN_VerifyString](@Description)
	
	IF EXISTS (
		SELECT TOP(1) * 
		FROM [dbo].[WF_WorkFlows]
		WHERE ApplicationID = @ApplicationID AND Name = @Name AND Deleted = 1
	) BEGIN
		UPDATE [dbo].[WF_WorkFlows]
			SET LastModifierUserID = @CreatorUserID,
				LastModificationDate = @CreationDate,
				Deleted = 0
		WHERE ApplicationID = @ApplicationID AND Name = @Name AND Deleted = 1
			
		SET @_RetVal = @@ROWCOUNT
	END
	
	IF EXISTS (
		SELECT TOP(1) * 
		FROM [dbo].[WF_WorkFlows]
		WHERE ApplicationID = @ApplicationID AND Name = @Name AND Deleted = 0
	) BEGIN
		SET @_RetVal = -1
	END
	ELSE BEGIN
		INSERT INTO [dbo].[WF_WorkFlows](
			ApplicationID,
			WorkFlowID,
			Name,
			[Description],
			CreatorUserID,
			CreationDate,
			Deleted
		)
		VALUES(
			@ApplicationID,
			@WorkFlowID,
			@Name,
			@Description,
			@CreatorUserID,
			@CreationDate,
			0
		)
		
		SET @_RetVal = @@ROWCOUNT
	END
	
	SELECT @_RetVal
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_ModifyWorkFlow]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_ModifyWorkFlow]
GO

CREATE PROCEDURE [dbo].[WF_ModifyWorkFlow]
	@ApplicationID			uniqueidentifier,
	@WorkFlowID				uniqueidentifier,
	@Name					nvarchar(255),
	@Description			nvarchar(2000),
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SET @Name = [dbo].[GFN_VerifyString](@Name)
	SET @Description = [dbo].[GFN_VerifyString](@Description)
	
	UPDATE [dbo].[WF_WorkFlows]
		SET Name = @Name,
			[Description] = @Description,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID
		
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_ArithmeticDeleteWorkFlow]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_ArithmeticDeleteWorkFlow]
GO

CREATE PROCEDURE [dbo].[WF_ArithmeticDeleteWorkFlow]
	@ApplicationID			uniqueidentifier,
	@WorkFlowID				uniqueidentifier,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[WF_WorkFlows]
		SET LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate,
			Deleted = 1
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID
		
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_P_GetWorkFlowsByIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_P_GetWorkFlowsByIDs]
GO

CREATE PROCEDURE [dbo].[WF_P_GetWorkFlowsByIDs]
	@ApplicationID		uniqueidentifier,
	@WorkFlowIDsTemp	GuidTableType readonly
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @WorkFlowIDs GuidTableType
	INSERT INTO @WorkFlowIDs SELECT * FROM @WorkFlowIDsTemp
	
	SELECT WF.WorkFlowID AS WorkFlowID,
		   WF.Name AS Name,
		   WF.[Description] AS [Description]
	FROM @WorkFlowIDs AS ExternalIDs
		INNER JOIN [dbo].[WF_WorkFlows] AS WF
		ON WF.ApplicationID = @ApplicationID AND WF.WorkFlowID = ExternalIDs.Value
	ORDER BY WF.CreationDate DESC
END

GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetWorkFlowsByIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetWorkFlowsByIDs]
GO

CREATE PROCEDURE [dbo].[WF_GetWorkFlowsByIDs]
	@ApplicationID		uniqueidentifier,
	@strWorkFlowIDs		varchar(max),
	@delimiter			char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @WorkFlowIDs GuidTableType
	INSERT @WorkFlowIDs
	SELECT Ref.Value FROM [dbo].[GFN_StrToGuidTable](@strWorkFlowIDs, @delimiter) AS Ref
	
	EXEC [dbo].[WF_P_GetWorkFlowsByIDs] @ApplicationID, @WorkFlowIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetWorkFlows]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetWorkFlows]
GO

CREATE PROCEDURE [dbo].[WF_GetWorkFlows]
	@ApplicationID	uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @WorkFlowIDs GuidTableType
	
	INSERT INTO @WorkFlowIDs
	SELECT WorkFlowID
	FROM [dbo].[WF_WorkFlows]
	WHERE ApplicationID = @ApplicationID AND Deleted = 0
	
	EXEC [dbo].[WF_P_GetWorkFlowsByIDs] @ApplicationID, @WorkFlowIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_AddWorkFlowState]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_AddWorkFlowState]
GO

CREATE PROCEDURE [dbo].[WF_AddWorkFlowState]
	@ApplicationID	uniqueidentifier,
	@ID				uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@StateID		uniqueidentifier,
	@CreatorUserID	uniqueidentifier,
	@CreationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF EXISTS(
		SELECT TOP(1) * 
		FROM [dbo].[WF_WorkFlowStates]
		WHERE ApplicationID = @ApplicationID AND 
			WorkFlowID = @WorkFlowID AND StateID = @StateID
	) BEGIN
		UPDATE [dbo].[WF_WorkFlowStates]
			SET Deleted = 0,
				LastModifierUserID = @CreatorUserID,
				LastModificationDate = @CreationDate
		WHERE ApplicationID = @ApplicationID AND 
			WorkFlowID = @WorkFlowID AND StateID = @StateID AND Deleted = 1
	END
	ELSE BEGIN
		INSERT INTO [dbo].[WF_WorkFlowStates](
			ApplicationID,
			ID,
			WorkFlowID,
			StateID,
			ResponseType,
			[Admin],
			DescriptionNeeded,
			HideOwnerName,
			FreeDataNeedRequests,
			EditPermission,
			CreatorUserID,
			CreationDate,
			Deleted
		)
		VALUES(
			@ApplicationID,
			@ID,
			@WorkFlowID,
			@StateID,
			NULL,
			0,
			1,
			0,
			0,
			0,
			@CreatorUserID,
			@CreationDate,
			0
		)
	END
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_ArithmeticDeleteWorkFlowState]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_ArithmeticDeleteWorkFlowState]
GO

CREATE PROCEDURE [dbo].[WF_ArithmeticDeleteWorkFlowState]
	@ApplicationID			uniqueidentifier,
	@WorkFlowID				uniqueidentifier,
	@StateID				uniqueidentifier,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[WF_WorkFlowStates]
		SET Deleted = 1,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND 
		WorkFlowID = @WorkFlowID AND StateID = @StateID AND Deleted = 0
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_SetWorkFlowStateDescription]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_SetWorkFlowStateDescription]
GO

CREATE PROCEDURE [dbo].[WF_SetWorkFlowStateDescription]
	@ApplicationID			uniqueidentifier,
	@WorkFlowID				uniqueidentifier,
	@StateID				uniqueidentifier,
	@Description			nvarchar(2000),
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SET @Description = [dbo].[GFN_VerifyString](@Description)
	
	UPDATE [dbo].[WF_WorkFlowStates]
		SET [Description] = @Description,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND StateID = @StateID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_SetWorkFlowStateTag]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_SetWorkFlowStateTag]
GO

CREATE PROCEDURE [dbo].[WF_SetWorkFlowStateTag]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@StateID		uniqueidentifier,
	@Tag			nvarchar(450),
	@CreatorUserID	uniqueidentifier,
	@CreationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @TagID uniqueidentifier
	
	DECLARE @Tags StringTableType
	INSERT INTO @Tags (Value) VALUES(@Tag)
	
	EXEC [dbo].[CN_P_AddTags] @ApplicationID, 
		@Tags, @CreatorUserID, @CreationDate, @TagID output
	
	UPDATE [dbo].[WF_WorkFlowStates]
		SET TagID = @TagID
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND StateID = @StateID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_RemoveWorkFlowStateTag]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_RemoveWorkFlowStateTag]
GO

CREATE PROCEDURE [dbo].[WF_RemoveWorkFlowStateTag]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@StateID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[WF_WorkFlowStates]
		SET TagID = NULL
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND StateID = @StateID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetWorkFlowTags]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetWorkFlowTags]
GO

CREATE PROCEDURE [dbo].[WF_GetWorkFlowTags]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT WFS.TagID, TG.Tag
	FROM [dbo].[WF_WorkFlowStates] AS WFS
		INNER JOIN [dbo].[CN_Tags] AS TG
		ON TG.ApplicationID = @ApplicationID AND TG.TagID = WFS.TagID
	WHERE WFS.ApplicationID = @ApplicationID AND 
		WFS.WorkFlowID = @WorkFlowID AND WFS.Deleted = 0
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_SaveWorkFlowActions]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_SaveWorkFlowActions]
GO

CREATE PROCEDURE [dbo].[WF_SaveWorkFlowActions]
	@ApplicationID	uniqueidentifier,
	@ConnectionID	uniqueidentifier,
	@ActionsTemp	StringTableType readonly,
	@CurrentUserID	uniqueidentifier,
	@Now			datetime
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @Actions StringTableType
	INSERT INTO @Actions SELECT * FROM @ActionsTemp 

	INSERT INTO [dbo].[WF_WorkFlowActions] (ApplicationID, ConnectionID, [Action], CreatorUserID, CreationDate, Deleted)
	SELECT @ApplicationID, @ConnectionID, C.[Value], @CurrentUserID, @Now, 0
	FROM @Actions AS C
		LEFT JOIN [dbo].[WF_WorkFlowActions] AS AC
		ON AC.ApplicationID = @ApplicationID AND AC.ConnectionID = @ConnectionID AND LOWER(AC.[Action]) = LOWER(C.[Value])
	WHERE AC.ConnectionID IS NULL

	;WITH [Data] AS (
		SELECT	ROW_NUMBER() OVER (ORDER BY (SELECT 1) ASC) AS Seq,
				A.[Value] AS [Action]
		FROM @Actions AS A
	)
	UPDATE AC
	SET Deleted = CASE WHEN C.[Action] IS NULL THEN 1 ELSE 0 END,
		SequenceNumber = ISNULL(C.Seq, AC.SequenceNumber),
		LastModifierUserID = @CurrentUserID,
		LastModificationDate = @Now
	FROM [dbo].[WF_WorkFlowActions] AS AC
		LEFT JOIN [Data] AS C
		ON LOWER(C.[Action]) = LOWER(AC.[Action])
	WHERE AC.ApplicationID = @ApplicationID AND AC.ConnectionID = @ConnectionID
	
	SELECT 1
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetWorkFlowActions]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetWorkFlowActions]
GO

CREATE PROCEDURE [dbo].[WF_GetWorkFlowActions]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@ConnectionID	uniqueidentifier
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	IF @ConnectionID IS NOT NULL BEGIN
		SELECT	AC.ConnectionID,
				AC.[Action]
		FROM [dbo].[WF_WorkFlowActions] AS AC
		WHERE AC.ApplicationID = @ApplicationID AND AC.ConnectionID = @ConnectionID AND AC.Deleted = 0
		ORDER BY AC.SequenceNumber ASC, AC.CreationDate ASC
	END
	ELSE IF @WorkFlowID IS NOT NULL BEGIN
		SELECT	AC.ConnectionID,
				AC.[Action]
		FROM [dbo].[WF_StateConnections] AS S
			INNER JOIN [dbo].[WF_WorkFlowActions] AS AC
			ON AC.ApplicationID = @ApplicationID AND AC.ConnectionID = S.ID AND AC.Deleted = 0
		WHERE S.ApplicationID = @ApplicationID AND S.WorkFlowID = @WorkFlowID AND S.Deleted = 0
		ORDER BY AC.SequenceNumber ASC, AC.CreationDate ASC
	END
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetNextStateActions]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetNextStateActions]
GO

CREATE PROCEDURE [dbo].[WF_GetNextStateActions]
	@ApplicationID	uniqueidentifier,
	@HistoryID		uniqueidentifier,
	@NextStateID	uniqueidentifier
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON

	SELECT	AC.ConnectionID,
			AC.[Action]
	FROM [dbo].[WF_History] AS H
		INNER JOIN [dbo].[WF_StateConnections] AS S
		ON S.ApplicationID = @ApplicationID AND 
			S.WorkFlowID = H.WorkFlowID AND S.OutStateID = @NextStateID AND S.InStateID = H.StateID
		INNER JOIN [dbo].[WF_WorkFlowActions] AS AC
		ON AC.ApplicationID = @ApplicationID AND AC.ConnectionID = S.ID AND AC.Deleted = 0
	WHERE H.ApplicationID = @ApplicationID AND H.HistoryID = @HistoryID
	ORDER BY AC.SequenceNumber ASC, AC.CreationDate ASC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_SetStateDirector]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_SetStateDirector]
GO

CREATE PROCEDURE [dbo].[WF_SetStateDirector]
	@ApplicationID			uniqueidentifier,
	@WorkFlowID				uniqueidentifier,
	@StateID				uniqueidentifier,
	@ResponseType			varchar(20),
	@RefStateID				uniqueidentifier,
	@NodeID					uniqueidentifier,
	@Admin					bit,
	@UserID					uniqueidentifier,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[WF_WorkFlowStates]
		SET ResponseType = @ResponseType,
			RefStateID = @RefStateID,
			NodeID = @NodeID,
			[Admin] = @Admin,
			UserID = @UserID,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND StateID = @StateID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_SetStatePoll]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_SetStatePoll]
GO

CREATE PROCEDURE [dbo].[WF_SetStatePoll]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@StateID		uniqueidentifier,
	@PollID			uniqueidentifier,
	@CurrentUserID	uniqueidentifier,
	@Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[WF_WorkFlowStates]
		SET PollID = @PollID,
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND StateID = @StateID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_SetStateDataNeedsType]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_SetStateDataNeedsType]
GO

CREATE PROCEDURE [dbo].[WF_SetStateDataNeedsType]
	@ApplicationID			uniqueidentifier,
	@WorkFlowID				uniqueidentifier,
	@StateID				uniqueidentifier,
	@DataNeedsType			varchar(20),
	@RefStateID				uniqueidentifier,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF @RefStateID IS NULL BEGIN
		UPDATE [dbo].[WF_WorkFlowStates]
			SET DataNeedsType = @DataNeedsType,
				LastModifierUserID = @LastModifierUserID,
				LastModificationDate = @LastModificationDate
		WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND StateID = @StateID
	END
	ELSE BEGIN
		UPDATE [dbo].[WF_WorkFlowStates]
			SET DataNeedsType = @DataNeedsType,
				RefDataNeedsStateID = @RefStateID,
				LastModifierUserID = @LastModifierUserID,
				LastModificationDate = @LastModificationDate
		WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND StateID = @StateID
	END
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_SetStateDataNeedsDescription]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_SetStateDataNeedsDescription]
GO

CREATE PROCEDURE [dbo].[WF_SetStateDataNeedsDescription]
	@ApplicationID			uniqueidentifier,
	@WorkFlowID				uniqueidentifier,
	@StateID				uniqueidentifier,
	@Description			nvarchar(2000),
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SET @Description = [dbo].[GFN_VerifyString](@Description)
	
	UPDATE [dbo].[WF_WorkFlowStates]
		SET DataNeedsDescription = @Description,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND StateID = @StateID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_SetStateDescriptionNeeded]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_SetStateDescriptionNeeded]
GO

CREATE PROCEDURE [dbo].[WF_SetStateDescriptionNeeded]
	@ApplicationID			uniqueidentifier,
	@WorkFlowID				uniqueidentifier,
	@StateID				uniqueidentifier,
	@DescriptionNeeded		bit,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[WF_WorkFlowStates]
		SET DescriptionNeeded = @DescriptionNeeded,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND StateID = @StateID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_SetStateHideOwnerName]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_SetStateHideOwnerName]
GO

CREATE PROCEDURE [dbo].[WF_SetStateHideOwnerName]
	@ApplicationID			uniqueidentifier,
	@WorkFlowID				uniqueidentifier,
	@StateID				uniqueidentifier,
	@HideOwnerName			bit,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[WF_WorkFlowStates]
		SET HideOwnerName = @HideOwnerName,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND StateID = @StateID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_SetStateEditPermission]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_SetStateEditPermission]
GO

CREATE PROCEDURE [dbo].[WF_SetStateEditPermission]
	@ApplicationID			uniqueidentifier,
	@WorkFlowID				uniqueidentifier,
	@StateID				uniqueidentifier,
	@EditPermission			bit,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[WF_WorkFlowStates]
		SET EditPermission = @EditPermission,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND StateID = @StateID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_SetFreeDataNeedRequests]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_SetFreeDataNeedRequests]
GO

CREATE PROCEDURE [dbo].[WF_SetFreeDataNeedRequests]
	@ApplicationID			uniqueidentifier,
	@WorkFlowID				uniqueidentifier,
	@StateID				uniqueidentifier,
	@FreeDataNeedRequests	bit,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[WF_WorkFlowStates]
		SET FreeDataNeedRequests = @FreeDataNeedRequests,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND StateID = @StateID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_SetStateDataNeed]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_SetStateDataNeed]
GO

CREATE PROCEDURE [dbo].[WF_SetStateDataNeed]
	@ApplicationID	uniqueidentifier,
	@ID				uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@StateID		uniqueidentifier,
	@NodeTypeID		uniqueidentifier,
	@PreNodeTypeID	uniqueidentifier,
	@FormID			uniqueidentifier,
	@Description	nvarchar(2000),
	@MultipleSelect bit,
	@Admin			bit,
	@Necessary		bit,
	@CreatorUserID	uniqueidentifier,
	@CreationDate	datetime
WITH ENCRYPTION
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	SET @Description = [dbo].[GFN_VerifyString](@Description)
	
	IF @PreNodeTypeID IS NOT NULL AND @PreNodeTypeID <> @NodeTypeID BEGIN
		UPDATE [dbo].[WF_StateDataNeeds]
			SET LastModifierUserID = @CreatorUserID,
				LastModificationDate = @CreationDate,
				Deleted = 1
		WHERE ApplicationID = @ApplicationID AND 
			WorkFlowID = @WorkFlowID AND StateID = @StateID AND NodeTypeID = @PreNodeTypeID
	END
	
	IF EXISTS(
		SELECT TOP(1) * 
		FROM [dbo].[WF_StateDataNeeds]
		WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND 
			StateID = @StateID AND NodeTypeID = @NodeTypeID
	) BEGIN
		UPDATE [dbo].[WF_StateDataNeeds]
			SET [Description] = @Description,
				MultipleSelect = @MultipleSelect,
				[Admin] = @Admin,
				Necessary = @Necessary,
				LastModifierUserID = @CreatorUserID,
				LastModificationDate = @CreationDate,
				Deleted = 0
		WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND 
			StateID = @StateID AND NodeTypeID = @NodeTypeID
	END
	ELSE BEGIN
		INSERT INTO [dbo].[WF_StateDataNeeds](
			ApplicationID,
			ID,
			WorkFlowID,
			StateID,
			NodeTypeID,
			[Description],
			MultipleSelect,
			[Admin],
			Necessary,
			CreatorUserID,
			CreationDate,
			Deleted
		)
		VALUES(
			@ApplicationID,
			@ID,
			@WorkFlowID,
			@StateID,
			@NodeTypeID,
			@Description,
			@MultipleSelect,
			@Admin,
			@Necessary,
			@CreatorUserID,
			@CreationDate,
			0
		)
	END
	
	DECLARE @_Result int = @@ROWCOUNT
	
	IF @_Result <= 0 BEGIN
		SELECT -1
		ROLLBACK TRANSACTION
		RETURN
	END
	
	IF @FormID IS NOT NULL BEGIN
		EXEC [dbo].[FG_P_SetFormOwner] @ApplicationID, @ID, @FormID, 
			@CreatorUserID, @CreationDate, @_Result output
		
		IF @_Result <= 0 BEGIN
			SELECT -1
			ROLLBACK TRANSACTION
			RETURN
		END
	END
	
	SELECT @_Result
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_ArithmeticDeleteStateDataNeed]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_ArithmeticDeleteStateDataNeed]
GO

CREATE PROCEDURE [dbo].[WF_ArithmeticDeleteStateDataNeed]
	@ApplicationID			uniqueidentifier,
	@WorkFlowID				uniqueidentifier,
	@StateID				uniqueidentifier,
	@NodeTypeID				uniqueidentifier,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[WF_StateDataNeeds]
		SET Deleted = 1,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND 
		WorkFlowID = @WorkFlowID AND StateID = @StateID AND 
		NodeTypeID = @NodeTypeID AND Deleted = 0
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_SetRejectionSettings]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_SetRejectionSettings]
GO

CREATE PROCEDURE [dbo].[WF_SetRejectionSettings]
	@ApplicationID			uniqueidentifier,
	@WorkFlowID				uniqueidentifier,
	@StateID				uniqueidentifier,
	@MaxAllowedRejections	int,
	@RejectionTitle			nvarchar(255),
	@RejectionRefStateID	uniqueidentifier,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[WF_WorkFlowStates]
		SET MaxAllowedRejections = @MaxAllowedRejections,
			RejectionTitle = @RejectionTitle,
			RejectionRefStateID = @RejectionRefStateID,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND 
		WorkFlowID = @WorkFlowID AND StateID = @StateID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_SetMaxAllowedRejections]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_SetMaxAllowedRejections]
GO

CREATE PROCEDURE [dbo].[WF_SetMaxAllowedRejections]
	@ApplicationID			uniqueidentifier,
	@WorkFlowID				uniqueidentifier,
	@StateID				uniqueidentifier,
	@MaxAllowedRejections	int,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[WF_WorkFlowStates]
		SET MaxAllowedRejections = @MaxAllowedRejections,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND 
		WorkFlowID = @WorkFlowID AND StateID = @StateID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetRejectionsCount]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetRejectionsCount]
GO

CREATE PROCEDURE [dbo].[WF_GetRejectionsCount]
	@ApplicationID	uniqueidentifier,
	@OwnerID		uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@StateID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT COUNT(HistoryID)
	FROM [dbo].[WF_History]
	WHERE ApplicationID = @ApplicationID AND OwnerID = @OwnerID AND 
		WorkFlowID = @WorkFlowID AND StateID = @StateID AND Rejected = 1 AND Deleted = 0
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_AddStateConnection]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_AddStateConnection]
GO

CREATE PROCEDURE [dbo].[WF_AddStateConnection]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@InStateID		uniqueidentifier,
	@OutStateID		uniqueidentifier,
	@CreatorUserID	uniqueidentifier,
	@CreationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @SequenceNumber int = (
		SELECT ISNULL(MAX(SequenceNumber), 0) 
		FROM [dbo].[WF_StateConnections]
		WHERE ApplicationID = @ApplicationID AND 
			WorkFlowID = @WorkFlowID AND InStateID = @InStateID
	) + 1
	
	DECLARE @ID uniqueidentifier = (
		SELECT TOP(1) ID
		FROM [dbo].[WF_StateConnections]
		WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND 
			InStateID = @InStateID AND OutStateID = @OutStateID AND Deleted = 1
	)
	
	IF @ID IS NOT NULL BEGIN
		UPDATE [dbo].[WF_StateConnections]
			SET Deleted = 0,
				LastModifierUserID = @CreatorUserID,
				LastModificationDate = @CreationDate
		WHERE ApplicationID = @ApplicationID AND ID = @ID
	END
	ELSE BEGIN
		SET @ID = NEWID()
	
		INSERT INTO [dbo].[WF_StateConnections](
			ApplicationID,
			ID,
			WorkFlowID,
			InStateID,
			OutStateID,
			SequenceNumber,
			Label,
			AttachmentRequired,
			NodeRequired,
			CreatorUserID,
			CreationDate,
			Deleted
		)
		VALUES(
			@ApplicationID,
			@ID,
			@WorkFlowID,
			@InStateID,
			@OutStateID,
			@SequenceNumber,
			N'',
			0,
			0,
			@CreatorUserID,
			@CreationDate,
			0
		)
	END
	
	SELECT @ID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_SortStateConnections]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_SortStateConnections]
GO

CREATE PROCEDURE [dbo].[WF_SortStateConnections]
	@ApplicationID	uniqueidentifier,
	@strIDs			varchar(max),
	@delimiter		char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @IDs TABLE (SequenceNo int identity(1, 1) primary key, ID uniqueidentifier)
	
	INSERT INTO @IDs (ID)
	SELECT Ref.Value
	FROM [dbo].[GFN_StrToGuidTable](@strIDs, @delimiter) AS Ref
	
	DECLARE @WorkFlowID uniqueidentifier, @StateID uniqueidentifier
	
	SELECT @WorkFlowID = WorkFlowID, @StateID = InStateID
	FROM [dbo].[WF_StateConnections]
	WHERE ApplicationID = @ApplicationID AND ID = (SELECT TOP (1) Ref.ID FROM @IDs AS Ref)
	
	IF @WorkFlowID IS NULL OR @StateID IS NULL BEGIN
		SELECT -1
		RETURN
	END
	
	INSERT INTO @IDs (ID)
	SELECT SC.ID
	FROM @IDs AS Ref
		RIGHT JOIN [dbo].[WF_StateConnections] AS SC
		ON SC.ID = Ref.ID
	WHERE SC.ApplicationID = @ApplicationID AND 
		SC.WorkFlowID = @WorkFlowID AND SC.InStateID = @StateID AND Ref.ID IS NULL
	ORDER BY SC.SequenceNumber
	
	UPDATE [dbo].[WF_StateConnections]
		SET SequenceNumber = Ref.SequenceNo
	FROM @IDs AS Ref
		INNER JOIN [dbo].[WF_StateConnections] AS SC
		ON SC.ID = Ref.ID
	WHERE SC.ApplicationID = @ApplicationID AND SC.WorkFlowID = @WorkFlowID AND SC.InStateID = @StateID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_MoveStateConnection]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_MoveStateConnection]
GO

CREATE PROCEDURE [dbo].[WF_MoveStateConnection]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@InStateID		uniqueidentifier,
	@OutStateID		uniqueidentifier,
	@MoveDown		bit
WITH ENCRYPTION
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	DECLARE @SequenceNo int = (
		SELECT TOP(1) SequenceNumber
		FROM [dbo].[WF_StateConnections]
		WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND 
			InStateID = @InStateID AND OutStateID = @OutStateID
	)
		
	DECLARE @OtherOutStateID uniqueidentifier
	DECLARE @OtherSequenceNumber int
	
	IF @MoveDown = 1 BEGIN
		SELECT TOP(1) @OtherOutStateID = OutStateID, @OtherSequenceNumber = SequenceNumber
		FROM [dbo].[WF_StateConnections]
		WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND 
			InStateID = @InStateID AND SequenceNumber > @SequenceNo
		ORDER BY SequenceNumber
	END
	ELSE BEGIN
		SELECT TOP(1) @OtherOutStateID = OutStateID, @OtherSequenceNumber = SequenceNumber
		FROM [dbo].[WF_StateConnections]
		WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND 
			InStateID = @InStateID AND SequenceNumber < @SequenceNo
		ORDER BY SequenceNumber DESC
	END
	
	IF @OtherOutStateID IS NULL BEGIN
		SELECT -1
		ROLLBACK TRANSACTION
		RETURN
	END
	
	UPDATE [dbo].[WF_StateConnections]
		SET SequenceNumber = @OtherSequenceNumber
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND 
		InStateID = @InStateID AND OutStateID = @OutStateID
	
	IF @@ROWCOUNT <= 0 BEGIN
		SELECT -1
		ROLLBACK TRANSACTION
		RETURN
	END
	
	UPDATE [dbo].[WF_StateConnections]
		SET SequenceNumber = @SequenceNo
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND 
		InStateID = @InStateID AND OutStateID = @OtherOutStateID
	
	IF @@ROWCOUNT <= 0 BEGIN
		SELECT -1
		ROLLBACK TRANSACTION
		RETURN
	END
	
	SELECT 1
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_ArithmeticDeleteStateConnection]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_ArithmeticDeleteStateConnection]
GO

CREATE PROCEDURE [dbo].[WF_ArithmeticDeleteStateConnection]
	@ApplicationID			uniqueidentifier,
	@WorkFlowID				uniqueidentifier,
	@InStateID				uniqueidentifier,
	@OutStateID				uniqueidentifier,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[WF_StateConnections]
		SET Deleted = 1,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND 
		InStateID = @InStateID AND OutStateID = @OutStateID AND Deleted = 0
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_SetStateConnectionLabel]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_SetStateConnectionLabel]
GO

CREATE PROCEDURE [dbo].[WF_SetStateConnectionLabel]
	@ApplicationID			uniqueidentifier,
	@WorkFlowID				uniqueidentifier,
	@InStateID				uniqueidentifier,
	@OutStateID				uniqueidentifier,
	@Label					nvarchar(255),
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SET @Label = [dbo].[GFN_VerifyString](@Label)
	
	UPDATE [dbo].[WF_StateConnections]
		SET Label = @Label,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND 
		InStateID = @InStateID AND OutStateID = @OutStateID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_SetStateConnectionAttachmentStatus]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_SetStateConnectionAttachmentStatus]
GO

CREATE PROCEDURE [dbo].[WF_SetStateConnectionAttachmentStatus]
	@ApplicationID			uniqueidentifier,
	@WorkFlowID				uniqueidentifier,
	@InStateID				uniqueidentifier,
	@OutStateID				uniqueidentifier,
	@AttachmentRequired		bit,
	@AttachmentTitle		nvarchar(255),
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SET @AttachmentTitle = [dbo].[GFN_VerifyString](@AttachmentTitle)
	
	IF @AttachmentRequired IS NULL SET @AttachmentRequired = 0
	
	UPDATE [dbo].[WF_StateConnections]
		SET AttachmentRequired = @AttachmentRequired,
			AttachmentTitle = @AttachmentTitle,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND 
		InStateID = @InStateID AND OutStateID = @OutStateID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_SetStateConnectionDirector]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_SetStateConnectionDirector]
GO

CREATE PROCEDURE [dbo].[WF_SetStateConnectionDirector]
	@ApplicationID			uniqueidentifier,
	@WorkFlowID				uniqueidentifier,
	@InStateID				uniqueidentifier,
	@OutStateID				uniqueidentifier,
	@NodeRequired			bit,
	@NodeTypeID				uniqueidentifier,
	@NodeTypeDescription	nvarchar(2000),
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF @NodeRequired IS NULL SET @NodeRequired = 0
	
	UPDATE [dbo].[WF_StateConnections]
		SET NodeRequired = @NodeRequired,
			NodeTypeID = @NodeTypeID,
			NodeTypeDescription = @NodeTypeDescription,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND 
		InStateID = @InStateID AND OutStateID = @OutStateID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_SetStateConnectionForm]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_SetStateConnectionForm]
GO

CREATE PROCEDURE [dbo].[WF_SetStateConnectionForm]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@InStateID		uniqueidentifier,
	@OutStateID		uniqueidentifier,
	@FormID			uniqueidentifier,
	@Description	nvarchar(4000),
	@Necessary		bit,
	@CreatorUserID	uniqueidentifier,
	@CreationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SET @Description = [dbo].[GFN_VerifyString](@Description)
	
	IF @Necessary IS NULL SET @Necessary = 0
	
	IF EXISTS(
		SELECT TOP(1) * 
		FROM [dbo].[WF_StateConnectionForms]
		WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND 
			InStateID = @InStateID AND OutStateID = @OutStateID AND FormID = @FormID
	) BEGIN
		UPDATE [dbo].[WF_StateConnectionForms]
			SET [Description] = @Description,
				Necessary = @Necessary,
				LastModifierUserID = @CreatorUserID,
				LastModificationDate = @CreationDate,
				Deleted = 0
		WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND 
			InStateID = @InStateID AND OutStateID = @OutStateID AND FormID = @FormID
	END
	ELSE BEGIN
		INSERT INTO [dbo].[WF_StateConnectionForms](
			ApplicationID,
			WorkFlowID,
			InStateID,
			OutStateID,
			FormID,
			[Description],
			Necessary,
			CreatorUserID,
			CreationDate,
			Deleted
		)
		VALUES(
			@ApplicationID,
			@WorkFlowID,
			@InStateID,
			@OutStateID,
			@FormID,
			@Description,
			@Necessary,
			@CreatorUserID,
			@CreationDate,
			0
		)
	END
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_ArithmeticDeleteStateConnectionForm]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_ArithmeticDeleteStateConnectionForm]
GO

CREATE PROCEDURE [dbo].[WF_ArithmeticDeleteStateConnectionForm]
	@ApplicationID			uniqueidentifier,
	@WorkFlowID				uniqueidentifier,
	@InStateID				uniqueidentifier,
	@OutStateID				uniqueidentifier,
	@FormID					uniqueidentifier,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[WF_StateConnectionForms]
		SET LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate,
			Deleted = 1
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND 
		InStateID = @InStateID AND OutStateID = @OutStateID AND FormID = @FormID AND Deleted = 0
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_AddAutoMessage]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_AddAutoMessage]
GO

CREATE PROCEDURE [dbo].[WF_AddAutoMessage]
	@ApplicationID	uniqueidentifier,
	@AutoMessageID	uniqueidentifier,
	@OwnerID		uniqueidentifier,
	@BodyText		nvarchar(4000),
	@AudienceType	varchar(20),
	@RefStateID		uniqueidentifier,
	@NodeID			uniqueidentifier,
	@Admin			bit,
	@CreatorUserID	uniqueidentifier,
	@CreationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SET @BodyText = [dbo].[GFN_VerifyString](@BodyText)
	
	IF @Admin IS NULL SET @Admin = 0
	
	INSERT INTO [dbo].[WF_AutoMessages](
		ApplicationID,
		AutoMessageID,
		OwnerID,
		BodyText,
		AudienceType,
		RefStateID,
		NodeID,
		[Admin],
		CreatorUserID,
		CreationDate,
		Deleted
	)
	VALUES(
		@ApplicationID,
		@AutoMessageID,
		@OwnerID,
		@BodyText,
		@AudienceType,
		@RefStateID,
		@NodeID,
		@Admin,
		@CreatorUserID,
		@CreationDate,
		0
	)
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_ModifyAutoMessage]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_ModifyAutoMessage]
GO

CREATE PROCEDURE [dbo].[WF_ModifyAutoMessage]
	@ApplicationID			uniqueidentifier,
	@AutoMessageID			uniqueidentifier,
	@BodyText				nvarchar(4000),
	@AudienceType			varchar(20),
	@RefStateID				uniqueidentifier,
	@NodeID					uniqueidentifier,
	@Admin					bit,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SET @BodyText = [dbo].[GFN_VerifyString](@BodyText)
	
	IF @Admin IS NULL SET @Admin = 0
	
	UPDATE [dbo].[WF_AutoMessages]
		SET	BodyText = @BodyText,
			AudienceType = @AudienceType,
			RefStateID = @RefStateID,
			NodeID = @NodeID,
			[Admin] = @Admin,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND AutoMessageID = @AutoMessageID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_ArithmeticDeleteAutoMessage]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_ArithmeticDeleteAutoMessage]
GO

CREATE PROCEDURE [dbo].[WF_ArithmeticDeleteAutoMessage]
	@ApplicationID			uniqueidentifier,
	@AutoMessageID			uniqueidentifier,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[WF_AutoMessages]
		SET	Deleted = 1,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND AutoMessageID = @AutoMessageID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_P_GetOwnerAutoMessages]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_P_GetOwnerAutoMessages]
GO

CREATE PROCEDURE [dbo].[WF_P_GetOwnerAutoMessages]
	@ApplicationID	uniqueidentifier,
	@OwnerIDsTemp	GuidTableType readonly
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @OwnerIDs GuidTableType
	INSERT INTO @OwnerIDs SELECT * FROM @OwnerIDsTemp
	
	SELECT AM.AutoMessageID AS AutoMessageID,
		   AM.OwnerID AS OwnerID,
		   AM.BodyText AS BodyText,
		   AM.AudienceType AS AudienceType,
		   AM.RefStateID AS RefStateID,
		   ST.Title AS RefStateTitle,
		   AM.NodeID AS NodeID,
		   ND.NodeName AS NodeName,
		   ND.NodeTypeID AS NodeTypeID,
		   ND.TypeName AS NodeType,
		   AM.[Admin] AS [Admin]
	FROM @OwnerIDs AS Ref
		INNER JOIN [dbo].[WF_AutoMessages] AS AM
		ON AM.OwnerID = Ref.Value
		LEFT JOIN [dbo].[WF_States] AS ST
		ON ST.ApplicationID = @ApplicationID AND ST.StateID = AM.RefStateID
		LEFT JOIN [dbo].[CN_View_Nodes_Normal] AS ND
		ON ND.ApplicationID = @ApplicationID AND ND.NodeID = AM.NodeID
	WHERE AM.ApplicationID = @ApplicationID AND AM.Deleted = 0
	ORDER BY AM.CreationDate ASC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetOwnerAutoMessages]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetOwnerAutoMessages]
GO

CREATE PROCEDURE [dbo].[WF_GetOwnerAutoMessages]
	@ApplicationID	uniqueidentifier,
	@strOwnerIDs	varchar(max),
	@delimiter		char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @OwnerIDs GuidTableType
	INSERT INTO @OwnerIDs
	SELECT Ref.Value FROM [dbo].[GFN_StrToGuidTable](@strOwnerIDs, @delimiter) AS Ref
	
	EXEC [dbo].[WF_P_GetOwnerAutoMessages] @ApplicationID, @OwnerIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetWorkFlowAutoMessages]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetWorkFlowAutoMessages]
GO

CREATE PROCEDURE [dbo].[WF_GetWorkFlowAutoMessages]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @OwnerIDs GuidTableType
	
	INSERT INTO @OwnerIDs
	SELECT ID
	FROM [dbo].[WF_StateConnections]
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND Deleted = 0
	
	EXEC [dbo].[WF_P_GetOwnerAutoMessages] @ApplicationID, @OwnerIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetConnectionAutoMessages]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetConnectionAutoMessages]
GO

CREATE PROCEDURE [dbo].[WF_GetConnectionAutoMessages]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@InStateID		uniqueidentifier,
	@OutStateID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @OwnerIDs GuidTableType
	
	INSERT INTO @OwnerIDs
	SELECT ID
	FROM [dbo].[WF_StateConnections]
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND 
		InStateID = @InStateID AND OutStateID = @OutStateID
	
	EXEC [dbo].[WF_P_GetOwnerAutoMessages] @ApplicationID, @OwnerIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_P_GetWorkFlowStates]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_P_GetWorkFlowStates]
GO

CREATE PROCEDURE [dbo].[WF_P_GetWorkFlowStates]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@StateIDsTemp	GuidTableType readonly
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @StateIDs GuidTableType
	INSERT INTO @StateIDs SELECT * FROM @StateIDsTemp
	
	SELECT WFS.ID,
		   WFS.StateID,
		   WFS.WorkFlowID,
		   WFS.[Description],
		   TG.Tag,
		   WFS.DataNeedsType,
		   WFS.RefDataNeedsStateID,
		   WFS.DataNeedsDescription,
		   WFS.DescriptionNeeded,
		   WFS.HideOwnerName,
		   WFS.EditPermission,
		   WFS.ResponseType,
		   WFS.RefStateID,
		   WFS.NodeID,
		   ND.NodeName,
		   ND.NodeTypeID,
		   ND.TypeName AS NodeType,
		   WFS.[Admin],
		   WFS.UserID,
		   UN.UserName,
		   UN.FirstName,
		   UN.LastName,
		   WFS.FreeDataNeedRequests,
		   WFS.MaxAllowedRejections,
		   WFS.RejectionTitle,
		   WFS.RejectionRefStateID,
		   RS.Title AS RejectionRefStateTitle,
		   PL.PollID,
		   PL.[Name] AS PollName
	FROM @StateIDs AS ExternalIDs
		INNER JOIN [dbo].[WF_WorkFlowStates] AS WFS
		ON WFS.StateID = ExternalIDs.Value
		LEFT JOIN [dbo].[CN_View_Nodes_Normal] AS ND
		ON ND.ApplicationID = @ApplicationID AND ND.NodeID = WFS.NodeID AND ND.Deleted = 0
		LEFT JOIN [dbo].[CN_Tags] AS TG
		ON TG.ApplicationID = @ApplicationID AND TG.TagID = WFS.TagID
		LEFT JOIN [dbo].[WF_States] AS RS
		ON RS.ApplicationID = @ApplicationID AND RS.StateID = WFS.RejectionRefStateID
		LEFT JOIN [dbo].[FG_Polls] AS PL
		ON PL.ApplicationID = @ApplicationID AND PL.PollID = WFS.PollID
		LEFT JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND UN.UserID = WFS.UserID
	WHERE WFS.ApplicationID = @ApplicationID AND WFS.WorkFlowID = @WorkFlowID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetWorkFlowStates]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetWorkFlowStates]
GO

CREATE PROCEDURE [dbo].[WF_GetWorkFlowStates]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@strStateIDs	varchar(max),
	@delimiter		char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @All bit = NULL, @StateIDs GuidTableType
	
	IF @strStateIDs IS NULL BEGIN
		INSERT INTO @StateIDs
		SELECT StateID
		FROM [dbo].[WF_WorkFlowStates]
		WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND Deleted = 0
	END
	ELSE BEGIN
		INSERT INTO @StateIDs
		SELECT DISTINCT Ref.Value 
		FROM [dbo].[GFN_StrToGuidTable](@strStateIDs, @delimiter) AS Ref
	END
	
	EXEC [dbo].[WF_P_GetWorkFlowStates] @ApplicationID, @WorkFlowID, @StateIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetFirstWorkFlowState]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetFirstWorkFlowState]
GO

CREATE PROCEDURE [dbo].[WF_GetFirstWorkFlowState]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @StateIDs GuidTableType
	
	INSERT INTO @StateIDs
	SELECT WFS.StateID
	FROM [dbo].[WF_WorkFlowStates] AS WFS
	WHERE WFS.ApplicationID = @ApplicationID AND WFS.WorkFlowID = @WorkFlowID AND WFS.Deleted = 0 AND
		NOT EXISTS(
			SELECT TOP(1) * 
			FROM [dbo].[WF_StateConnections] AS SC
				INNER JOIN [dbo].[WF_WorkFlowStates] AS Ref
				ON Ref.ApplicationID = @ApplicationID AND Ref.StateID = SC.InStateID
			WHERE SC.ApplicationID = @ApplicationID AND SC.WorkFlowID = @WorkFlowID AND 
				SC.OutStateID = WFS.StateID AND SC.Deleted = 0 AND Ref.Deleted = 0
		)
		
	IF (SELECT COUNT(*) FROM @StateIDs) = 1
		EXEC [dbo].[WF_P_GetWorkFlowStates] @ApplicationID, @WorkFlowID, @StateIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_P_GetStateDataNeedsByIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_P_GetStateDataNeedsByIDs]
GO

CREATE PROCEDURE [dbo].[WF_P_GetStateDataNeedsByIDs]
	@ApplicationID	uniqueidentifier,
	@IDsTemp		GuidTripleTableType readonly --First:WorkFlowID, Second:StateID, Third:NodeTypeID
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @IDs GuidTripleTableType
	INSERT INTO @IDs SELECT * FROM @IDsTemp
	
	SELECT SDN.ID AS ID,
		   SDN.StateID AS StateID,
		   SDN.WorkFlowID AS WorkFlowID,
		   SDN.NodeTypeID AS NodeTypeID,
		   EF.FormID AS FormID,
		   EF.Title AS FormTitle,
		   SDN.[Description] AS [Description],
		   NT.Name AS NodeType,
		   SDN.MultipleSelect AS MultiSelect,
		   SDN.[Admin] AS [Admin],
		   SDN.Necessary AS Necessary
	FROM @IDs AS ExternalIDs
		INNER JOIN [dbo].[WF_StateDataNeeds] AS SDN
		ON SDN.ApplicationID = @ApplicationID AND 
			ExternalIDs.FirstValue = SDN.WorkFlowID AND
			ExternalIDs.SecondValue = SDN.StateID AND 
			ExternalIDs.ThirdValue = SDN.NodeTypeID
		LEFT JOIN [dbo].[CN_NodeTypes] AS NT
		ON NT.ApplicationID = @ApplicationID AND NT.NodeTypeID = SDN.NodeTypeID
		LEFT JOIN [dbo].[FG_FormOwners] AS FO
		INNER JOIN [dbo].[FG_ExtendedForms] EF
		ON EF.ApplicationID = @ApplicationID AND EF.FormID = FO.FormID
		ON FO.ApplicationID = @ApplicationID AND FO.OwnerID = SDN.ID AND FO.Deleted = 0
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_P_GetStateDataNeeds]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_P_GetStateDataNeeds]
GO

CREATE PROCEDURE [dbo].[WF_P_GetStateDataNeeds]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@StateIDsTemp	GuidTableType readonly
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @StateIDs GuidTableType
	INSERT INTO @StateIDs SELECT * FROM @StateIDsTemp
	
	DECLARE @IDs GuidTripleTableType
	
	INSERT INTO @IDs
	SELECT @WorkFlowID, SDN.StateID, SDN.NodeTypeID
	FROM @StateIDs AS ExternalIDs
		INNER JOIN [dbo].[WF_StateDataNeeds] AS SDN
		ON SDN.StateID = ExternalIDs.Value
	WHERE SDN.ApplicationID = @ApplicationID AND 
		SDN.WorkFlowID = @WorkFlowID AND SDN.Deleted = 0
	
	EXEC [dbo].[WF_P_GetStateDataNeedsByIDs] @ApplicationID, @IDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetStateDataNeeds]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetStateDataNeeds]
GO

CREATE PROCEDURE [dbo].[WF_GetStateDataNeeds]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@strStateIDs	varchar(max),
	@delimiter		char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @All bit = NULL, @StateIDs GuidTableType
	
	IF @strStateIDs IS NULL 
		INSERT INTO @StateIDs
		SELECT StateID
		FROM [dbo].[WF_WorkFlowStates]
		WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND Deleted = 0
	ELSE BEGIN
		INSERT INTO @StateIDs
		SELECT DISTINCT Ref.Value 
		FROM [dbo].[GFN_StrToGuidTable](@strStateIDs, @delimiter) AS Ref
	END
	
	EXEC [dbo].[WF_P_GetStateDataNeeds] @ApplicationID, @WorkFlowID, @StateIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetStateDataNeed]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetStateDataNeed]
GO

CREATE PROCEDURE [dbo].[WF_GetStateDataNeed]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@StateID		uniqueidentifier,
	@NodeTypeID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @IDs GuidTripleTableType
	INSERT INTO @IDs(FirstValue, SecondValue, ThirdValue)
	VALUES(@WorkFlowID, @StateID, @NodeTypeID)
	
	EXEC [dbo].[WF_P_GetStateDataNeedsByIDs] @ApplicationID, @IDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetCurrentStateDataNeeds]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetCurrentStateDataNeeds]
GO

CREATE PROCEDURE [dbo].[WF_GetCurrentStateDataNeeds]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@StateID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @StateIDs GuidTableType, @DataNeedsType varchar(20), 
		@RefDataNeedsStateID uniqueidentifier
	
	SELECT @DataNeedsType = DataNeedsType, @RefDataNeedsStateID = RefDataNeedsStateID
	FROM [dbo].[WF_WorkFlowStates]
	WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND StateID = @StateID
	
	IF @DataNeedsType = 'RefState'
		INSERT INTO @StateIDs(Value) VALUES(@RefDataNeedsStateID)
	ELSE
		INSERT INTO @StateIDs(Value) VALUES(@StateID)
	
	EXEC [dbo].[WF_P_GetStateDataNeeds] @ApplicationID, @WorkFlowID, @StateIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_CreateStateDataNeedInstance]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_CreateStateDataNeedInstance]
GO

CREATE PROCEDURE [dbo].[WF_CreateStateDataNeedInstance]
	@ApplicationID	uniqueidentifier,
	@InstanceID		uniqueidentifier,
	@HistoryID		uniqueidentifier,
	@NodeID			uniqueidentifier,
	@Admin			bit,
	@FormID			uniqueidentifier,
	@CreatorUserID	uniqueidentifier,
	@CreationDate	datetime
WITH ENCRYPTION
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	INSERT INTO [dbo].[WF_StateDataNeedInstances](
		ApplicationID,
		InstanceID,
		HistoryID,
		NodeID,
		[Admin],
		Filled,
		AttachmentID,
		CreatorUserID,
		CreationDate,
		Deleted
	)
	VALUES(
		@ApplicationID,
		@InstanceID,
		@HistoryID,
		@NodeID,
		@Admin,
		0,
		NEWID(),
		@CreatorUserID,
		@CreationDate,
		0
	)
	
	IF @@ROWCOUNT <= 0 BEGIN
		SELECT 0
		ROLLBACK TRANSACTION
		RETURN
	END
	
	IF @FormID IS NOT NULL BEGIN
		DECLARE @FormInstanceID uniqueidentifier = NEWID(), @_Result int
		
		DECLARE @FormInstances FormInstanceTableType
			
		INSERT INTO @FormInstances (InstanceID, FormID, OwnerID, DirectorID, [Admin])
		VALUES (@FormInstanceID, @FormID, @InstanceID, @NodeID, @Admin)
		
		EXEC [dbo].[FG_P_CreateFormInstance] @ApplicationID, @FormInstances, @CreatorUserID, @CreationDate, @_Result output
			
		IF @_Result <= 0 BEGIN
			SELECT 0
			ROLLBACK TRANSACTION
			RETURN
		END
	END
	
	-- Send Dashboards
	EXEC [dbo].[WF_P_SendDashboards] @ApplicationID, @HistoryID, NULL, NULL, NULL, 
		NULL, NULL, @InstanceID, @CreationDate, @_Result output
	
	IF @_Result <= 0 BEGIN
		SELECT -1, N'CannotDetermineDirector'
		ROLLBACK TRANSACTION 
		RETURN
	END
	-- end of Send Dashboards
	
	SELECT 1
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_SetStateDataNeedInstanceAsFilled]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_SetStateDataNeedInstanceAsFilled]
GO

CREATE PROCEDURE [dbo].[WF_SetStateDataNeedInstanceAsFilled]
	@ApplicationID			uniqueidentifier,
	@InstanceID				uniqueidentifier,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	UPDATE [dbo].[WF_StateDataNeedInstances]
		SET Filled = 1,
			FillingDate = @LastModificationDate,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND InstanceID = @InstanceID AND Filled = 0
	
	IF @@ROWCOUNT <= 0 BEGIN
		SELECT -1
		ROLLBACK TRANSACTION
		RETURN
	END
	
	DECLARE @FormInstanceID uniqueidentifier = (
		SELECT TOP(1) FI.InstanceID
		FROM [dbo].[WF_StateDataNeedInstances] AS DN
			INNER JOIN [dbo].[FG_FormInstances] AS FI
			ON FI.ApplicationID = @ApplicationID AND FI.OwnerID = DN.InstanceID
		WHERE DN.ApplicationID = @ApplicationID AND 
			DN.InstanceID = @InstanceID AND FI.Filled = 0 AND FI.Deleted = 0
	)
	
	DECLARE @_Result int = 0
	
	IF @FormInstanceID IS NOT NULL BEGIN	
		EXEC [dbo].[FG_P_SetFormInstanceAsFilled] @ApplicationID, @FormInstanceID, 
			@LastModificationDate, @LastModifierUserID, @_Result output
		
		IF @_Result <= 0 BEGIN
			SELECT -1
			ROLLBACK TRANSACTION
			RETURN
		END
	END
	
	EXEC [dbo].[NTFN_P_SetDashboardsAsDone] @ApplicationID, NULL, NULL, @InstanceID, 
		N'WorkFlow', NULL, @LastModificationDate, @_Result output
	
	IF @_Result <= 0 BEGIN
		SELECT -1
		ROLLBACK TRANSACTION
		RETURN
	END
	
	SELECT 1
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_SetStateDataNeedInstanceAsNotFilled]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_SetStateDataNeedInstanceAsNotFilled]
GO

CREATE PROCEDURE [dbo].[WF_SetStateDataNeedInstanceAsNotFilled]
	@ApplicationID			uniqueidentifier,
	@InstanceID				uniqueidentifier,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	UPDATE [dbo].[WF_StateDataNeedInstances]
		SET Filled = 0,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND InstanceID = @InstanceID AND Filled = 1
	
	IF @@ROWCOUNT <= 0 BEGIN
		SELECT -1
		ROLLBACK TRANSACTION
		RETURN
	END
	
	DECLARE @FormInstanceID uniqueidentifier = (
		SELECT TOP(1) FI.InstanceID
		FROM [dbo].[WF_StateDataNeedInstances] AS DN
			INNER JOIN [dbo].[FG_FormInstances] AS FI
			ON FI.ApplicationID = @ApplicationID AND FI.OwnerID = DN.InstanceID
		WHERE DN.ApplicationID = @ApplicationID AND
			DN.InstanceID = @InstanceID AND FI.Filled = 1 AND FI.Deleted = 0
	)
		
	DECLARE @_Result int
		
	IF @FormInstanceID IS NOT NULL BEGIN
		EXEC [dbo].[FG_P_SetFormInstanceAsNotFilled] @ApplicationID, 
			@FormInstanceID, @LastModifierUserID, @_Result output
		
		IF @_Result <= 0 BEGIN
			SELECT -1
			ROLLBACK TRANSACTION
			RETURN
		END
	END
	
	EXEC [dbo].[WF_P_SendDashboards] @ApplicationID, NULL, NULL, NULL, NULL, NULL, NULL, 
		@InstanceID, @LastModificationDate, @_Result output
	
	IF @_Result <= 0 BEGIN
		SELECT -1, N'CannotDetermineDirector'
		ROLLBACK TRANSACTION
		RETURN
	END
	
	SELECT 1
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_ArithmeticDeleteStateDataNeedInstance]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_ArithmeticDeleteStateDataNeedInstance]
GO

CREATE PROCEDURE [dbo].[WF_ArithmeticDeleteStateDataNeedInstance]
	@ApplicationID			uniqueidentifier,
	@InstanceID				uniqueidentifier,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	UPDATE [dbo].[WF_StateDataNeedInstances]
		SET Deleted = 1,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND InstanceID = @InstanceID AND Deleted = 0
	
	IF @@ROWCOUNT <= 0 BEGIN
		SELECT -1
		ROLLBACK TRANSACTION
		RETURN
	END
	
	DECLARE @_Result int = 0
	
	EXEC [dbo].[NTFN_P_ArithmeticDeleteDashboards] @ApplicationID, 
		NULL, NULL, @InstanceID, N'WorkFlow', NULL, @_Result output
	
	IF @_Result <= 0 BEGIN
		SELECT -1
		ROLLBACK TRANSACTION
		RETURN
	END
	
	SELECT 1
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_P_GetStateDataNeedInstances]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_P_GetStateDataNeedInstances]
GO

CREATE PROCEDURE [dbo].[WF_P_GetStateDataNeedInstances]
	@ApplicationID		uniqueidentifier,
	@InstanceIDsTemp	GuidTableType readonly
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @InstanceIDs GuidTableType
	INSERT INTO @InstanceIDs SELECT * FROM @InstanceIDsTemp
	
	SELECT SDNI.InstanceID AS InstanceID,
		   SDNI.HistoryID AS HistoryID,
		   SDNI.NodeID AS NodeID,
		   ND.NodeName AS NodeName,
		   ND.NodeTypeID AS NodeTypeID,
		   SDNI.Filled AS Filled,
		   SDNI.FillingDate AS FillingDate,
		   SDNI.AttachmentID AS AttachmentID
	FROM @InstanceIDs AS ExternalIDs
		INNER JOIN [dbo].[WF_StateDataNeedInstances] AS SDNI
		ON SDNI.InstanceID = ExternalIDs.Value
		LEFT JOIN [dbo].[CN_View_Nodes_Normal] AS ND
		ON ND.ApplicationID = @ApplicationID AND ND.NodeID = SDNI.NodeID
	WHERE SDNI.ApplicationID = @ApplicationID AND SDNI.Deleted = 0
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetStateDataNeedInstance]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetStateDataNeedInstance]
GO

CREATE PROCEDURE [dbo].[WF_GetStateDataNeedInstance]
	@ApplicationID	uniqueidentifier,
	@InstanceID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @InstanceIDs GuidTableType
	
	INSERT INTO @InstanceIDs(Value)
	VALUES(@InstanceID)
	
	EXEC [dbo].[WF_P_GetStateDataNeedInstances] @ApplicationID, @InstanceIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetStateDataNeedInstances]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetStateDataNeedInstances]
GO

CREATE PROCEDURE [dbo].[WF_GetStateDataNeedInstances]
	@ApplicationID	uniqueidentifier,
	@strHistoryIDs	varchar(max),
	@delimiter		char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @HistoryIDs GuidTableType, @InstanceIDs GuidTableType
	
	INSERT INTO @HistoryIDs
	SELECT DISTINCT Ref.Value FROM [dbo].[GFN_StrToGuidTable](@strHistoryIDs, @delimiter) AS Ref
	
	INSERT INTO @InstanceIDs
	SELECT SDNI.InstanceID
	FROM @HistoryIDs AS ExternalIDs
		INNER JOIN [dbo].[WF_StateDataNeedInstances] AS SDNI
		ON SDNI.HistoryID = ExternalIDs.Value
	WHERE SDNI.ApplicationID = @ApplicationID AND SDNI.Deleted = 0
	
	EXEC [dbo].[WF_P_GetStateDataNeedInstances] @ApplicationID, @InstanceIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_P_GetWorkFlowConnections]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_P_GetWorkFlowConnections]
GO

CREATE PROCEDURE [dbo].[WF_P_GetWorkFlowConnections]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@InStateIDsTemp	GuidTableType readonly
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @InStateIDs GuidTableType
	INSERT INTO @InStateIDs SELECT * FROM @InStateIDsTemp
	
	SELECT SC.ID AS ID,
		   SC.WorkFlowID AS WorkFlowID,
		   SC.InStateID AS InStateID,
		   SC.OutStateID AS OutStateID,
		   SC.SequenceNumber AS SequenceNumber,
		   SC.Label AS ConnectionLabel,
		   SC.AttachmentRequired AS AttachmentRequired,
		   SC.AttachmentTitle AS AttachmentTitle,
		   SC.NodeRequired AS NodeRequired,
		   SC.NodeTypeID AS NodeTypeID,
		   NT.Name AS NodeType,
		   SC.NodeTypeDescription AS NodeTypeDescription
	FROM @InStateIDs AS ExternalIDs
		INNER JOIN [dbo].[WF_StateConnections] AS SC
		ON SC.ApplicationID = @ApplicationID AND SC.WorkFlowID = @WorkFlowID AND
			SC.InStateID = ExternalIDs.Value AND SC.Deleted  = 0
		INNER JOIN [dbo].[WF_States] AS S
		ON S.ApplicationID = @ApplicationID AND S.StateID = SC.OutStateID AND S.Deleted  = 0
		LEFT JOIN [dbo].[CN_NodeTypes] AS NT
		ON NT.ApplicationID = @ApplicationID AND NT.NodeTypeID = SC.NodeTypeID
	ORDER BY ISNULL(SC.SequenceNumber, 1000000) ASC, SC.CreationDate ASC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetWorkFlowConnections]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetWorkFlowConnections]
GO

CREATE PROCEDURE [dbo].[WF_GetWorkFlowConnections]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@strInStateIDs	varchar(max),
	@delimiter		char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @All bit = NULL, @InStateIDs GuidTableType
	
	IF @strInStateIDs IS NULL
		INSERT INTO @InStateIDs
		SELECT StateID
		FROM [dbo].[WF_WorkFlowStates]
		WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND Deleted = 0
	ELSE BEGIN
		INSERT INTO @InStateIDs
		SELECT DISTINCT Ref.Value 
		FROM [dbo].[GFN_StrToGuidTable](@strInStateIDs, @delimiter) AS Ref
	END
	
	EXEC [dbo].[WF_P_GetWorkFlowConnections] @ApplicationID, @WorkFlowID, @InStateIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_P_GetWorkFlowConnectionForms]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_P_GetWorkFlowConnectionForms]
GO

CREATE PROCEDURE [dbo].[WF_P_GetWorkFlowConnectionForms]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@InStateIDsTemp	GuidTableType readonly
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @InStateIDs GuidTableType
	INSERT INTO @InStateIDs SELECT * FROM @InStateIDsTemp
	
	SELECT SCF.WorkFlowID AS WorkFlowID,
		   SCF.InStateID AS InStateID,
		   SCF.OutStateID AS OutStateID,
		   SCF.FormID AS FormID,
		   EF.Title AS FormTitle,
		   SCF.[Description] AS [Description],
		   SCF.Necessary AS Necessary
	FROM @InStateIDs AS ExternalIDs
		INNER JOIN [dbo].[WF_StateConnectionForms] AS SCF
		ON SCF.InStateID = ExternalIDs.Value
		LEFT JOIN [dbo].[FG_ExtendedForms] AS EF
		ON EF.ApplicationID = @ApplicationID AND EF.FormID = SCF.FormID
	WHERE SCF.ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND SCF.Deleted = 0
END

GO

	
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetWorkFlowConnectionForms]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetWorkFlowConnectionForms]
GO

CREATE PROCEDURE [dbo].[WF_GetWorkFlowConnectionForms]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@strInStateIDs	varchar(max),
	@delimiter		char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @InStateIDs GuidTableType
	
	IF @strInStateIDs IS NULL BEGIN
		INSERT INTO @InStateIDs
		SELECT StateID
		FROM [dbo].[WF_WorkFlowStates]
		WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND Deleted = 0
	END
	ELSE BEGIN
		INSERT INTO @InStateIDs
		SELECT DISTINCT Ref.Value
		FROM [dbo].[GFN_StrToGuidTable](@strInStateIDs, @delimiter) AS Ref
	END
	
	EXEC [dbo].[WF_P_GetWorkFlowConnectionForms] @ApplicationID, @WorkFlowID, @InStateIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_P_GetHistoryByIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_P_GetHistoryByIDs]
GO

CREATE PROCEDURE [dbo].[WF_P_GetHistoryByIDs]
	@ApplicationID	uniqueidentifier,
	@HistoryIDsTemp	GuidTableType readonly
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @HistoryIDs GuidTableType
	INSERT INTO @HistoryIDs SELECT * FROM @HistoryIDsTemp
	
	SELECT H.HistoryID AS HistoryID,
		   H.PreviousHistoryID AS PreviousHistoryID,
		   H.OwnerID AS OwnerID,
		   H.WorkFlowID AS WorkFlowID,
		   H.DirectorNodeID,
		   H.DirectorUserID,
		   N.NodeName AS DirectorNodeName,
		   N.TypeName AS DirectorNodeType,
		   H.StateID AS StateID,
		   S.Title AS StateTitle,
		   H.SelectedOutStateID AS SelectedOutStateID,
		   H.[Description] AS [Description],
		   H.ActorUserID AS SenderUserID,
		   U.UserName AS SenderUserName,
		   U.FirstName AS SenderFirstName,
		   U.LastName AS SenderLastName,
		   H.SendDate AS SendDate,
		   (
				SELECT TOP(1) PollID
				FROM [dbo].[FG_Polls] AS P
				WHERE P.ApplicationID = @ApplicationID AND 
					P.OwnerID = H.HistoryID AND P.Deleted = 0
				ORDER BY P.CreationDate DESC
		   ) AS PollID,
		   (
				SELECT TOP(1) Ref.Name
				FROM [dbo].[FG_Polls] AS P
					INNER JOIN [dbo].[FG_Polls] AS Ref
					ON Ref.ApplicationID = @ApplicationID AND Ref.PollID = P.IsCopyOfPollID
				WHERE P.ApplicationID = @ApplicationID AND 
					P.OwnerID = H.HistoryID AND P.Deleted = 0
				ORDER BY P.CreationDate DESC
		   ) AS PollName
	FROM @HistoryIDs AS ExternalIDs
		INNER JOIN [dbo].[WF_History] AS H
		ON H.ApplicationID = @ApplicationID AND H.HistoryID = ExternalIDs.Value
		INNER JOIN [dbo].[WF_States] AS S
		ON S.ApplicationID = @ApplicationID AND S.StateID = H.StateID
		LEFT JOIN [dbo].[CN_View_Nodes_Normal] AS N
		ON N.ApplicationID = @ApplicationID AND N.NodeID = H.DirectorNodeID
		LEFT JOIN [dbo].[Users_Normal] AS U
		ON U.ApplicationID = @ApplicationID AND U.UserID = H.ActorUserID
	ORDER BY H.ID DESC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetHistoryByIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetHistoryByIDs]
GO

CREATE PROCEDURE [dbo].[WF_GetHistoryByIDs]
	@ApplicationID	uniqueidentifier,
	@strHistoryIDs	varchar(max),
	@delimiter		char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @HistoryIDs GuidTableType
	INSERT INTO @HistoryIDs 
	SELECT DISTINCT Ref.Value FROM [dbo].[GFN_StrToGuidTable](@strHistoryIDs, @delimiter) AS Ref
	
	EXEC [dbo].[WF_P_GetHistoryByIDs] @ApplicationID, @HistoryIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetHistory]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetHistory]
GO

CREATE PROCEDURE [dbo].[WF_GetHistory]
	@ApplicationID	uniqueidentifier,
	@OwnerID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @HistoryIDs GuidTableType
	
	INSERT INTO @HistoryIDs	
	SELECT HistoryID
	FROM [dbo].[WF_History]
	WHERE ApplicationID = @ApplicationID AND OwnerID = @OwnerID AND Deleted = 0
	
	EXEC [dbo].[WF_P_GetHistoryByIDs] @ApplicationID, @HistoryIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetLastHistory]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetLastHistory]
GO

CREATE PROCEDURE [dbo].[WF_GetLastHistory]
	@ApplicationID	uniqueidentifier,
	@OwnerID		uniqueidentifier,
	@StateID		uniqueidentifier,
	@Done			bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @HistoryIDs GuidTableType
	
	INSERT INTO @HistoryIDs	
	SELECT TOP(1) HistoryID
	FROM [dbo].[WF_History]
	WHERE ApplicationID = @ApplicationID AND OwnerID = @OwnerID AND 
		(@StateID IS NULL OR StateID = @StateID) AND Deleted = 0 AND
		(ISNULL(@Done, 0) = 0 OR ActorUserID IS NOT NULL)
	ORDER BY ID DESC
	
	EXEC [dbo].[WF_P_GetHistoryByIDs] @ApplicationID, @HistoryIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetLastSelectedStateID]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetLastSelectedStateID]
GO

CREATE PROCEDURE [dbo].[WF_GetLastSelectedStateID]
	@ApplicationID	uniqueidentifier,
	@OwnerID		uniqueidentifier,
	@InStateID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF @InStateID IS NULL BEGIN
		SELECT TOP(1) StateID AS ID
		FROM [dbo].[WF_History]
		WHERE ApplicationID = @ApplicationID AND OwnerID = @OwnerID AND Deleted = 0
		ORDER BY ID DESC
	END
	ELSE BEGIN
		DECLARE @SendDate datetime
		
		SET @SendDate = (
			SELECT TOP(1) SendDate 
			FROM [dbo].[WF_History]
			WHERE ApplicationID = @ApplicationID AND OwnerID = @OwnerID AND 
				StateID = @InStateID AND Deleted = 0
			ORDER BY ID DESC
		)
		
		IF @SendDate IS NOT NULL BEGIN
			SELECT TOP(1) StateID AS ID
			FROM [dbo].[WF_History]
			WHERE ApplicationID = @ApplicationID AND OwnerID = @OwnerID AND 
				SendDate > @SendDate AND Deleted = 0
			ORDER BY ID DESC
		END
	END
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetHistoryOwnerID]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetHistoryOwnerID]
GO

CREATE PROCEDURE [dbo].[WF_GetHistoryOwnerID]
	@ApplicationID	uniqueidentifier,
	@HistoryID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT OwnerID AS ID
	FROM [dbo].[WF_History]
	WHERE ApplicationID = @ApplicationID AND HistoryID = @HistoryID 
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_CreateHistoryFormInstance]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_CreateHistoryFormInstance]
GO

CREATE PROCEDURE [dbo].[WF_CreateHistoryFormInstance]	
	@ApplicationID	uniqueidentifier,
	@HistoryID		uniqueidentifier,
	@OutStateID		uniqueidentifier,
	@FormID			uniqueidentifier,
	@CreatorUserID	uniqueidentifier,
	@CreationDate	datetime
WITH ENCRYPTION
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	DECLARE @FormOwnerID uniqueidentifier = NULL, @FormDirectorID uniqueidentifier,
		@FormInstanceID uniqueidentifier = NEWID(), @Admin bit,
		@WorkFlowID uniqueidentifier, @StateID uniqueidentifier
	
	SELECT @FormDirectorID = ISNULL(DirectorNodeID, DirectorUserID), 
		@WorkFlowID = WorkFlowID, @StateID = StateID
	FROM [dbo].[WF_History] 
	WHERE ApplicationID = @ApplicationID AND HistoryID = @HistoryID
	
	SET @Admin = (
		SELECT TOP(1) [Admin] 
		FROM [dbo].[WF_WorkFlowStates]
		WHERE ApplicationID = @ApplicationID AND 
			WorkFlowID = @WorkFlowID AND StateID = @StateID
	)
	
	IF EXISTS(
		SELECT TOP(1) * 
		FROM [dbo].[WF_HistoryFormInstances]
		WHERE ApplicationID = @ApplicationID AND 
			HistoryID = @HistoryID AND OutStateID = @OutStateID
	) BEGIN
		UPDATE [dbo].[WF_HistoryFormInstances]
			SET Deleted = 0,
				LastModifierUserID = @CreatorUserID,
				LastModificationDate = @CreationDate
		WHERE ApplicationID = @ApplicationID AND 
			HistoryID = @HistoryID AND OutStateID = @OutStateID
	END
	ELSE BEGIN
		SET @FormOwnerID = NEWID()
		
		INSERT INTO [dbo].[WF_HistoryFormInstances](
			ApplicationID,
			HistoryID,
			OutStateID,
			FormsID,
			CreatorUserID,
			CreationDate,
			Deleted
		)
		VALUES(
			@ApplicationID,
			@HistoryID,
			@OutStateID,
			@FormOwnerID,
			@CreatorUserID,
			@CreationDate,
			0
		)
	END
	
	IF @@ROWCOUNT <= 0 BEGIN
		SELECT NULL AS ID
		ROLLBACK TRANSACTION
		RETURN
	END
	
	IF @FormOwnerID IS NULL BEGIN
		SET @FormOwnerID = (
			SELECT FormsID 
			FROM [dbo].[WF_HistoryFormInstances]
			WHERE ApplicationID = @ApplicationID AND 
				HistoryID = @HistoryID AND OutStateID = @OutStateID
		)
	END
	
	DECLARE @_Result int
	
	DECLARE @FormInstances FormInstanceTableType
			
	INSERT INTO @FormInstances (InstanceID, FormID, OwnerID, DirectorID, [Admin])
	VALUES (@FormInstanceID, @FormID, @FormOwnerID, @FormDirectorID, @Admin)
	
	EXEC [dbo].[FG_P_CreateFormInstance] @ApplicationID, @FormInstances, @CreatorUserID, @CreationDate, @_Result output
		
	IF @_Result <= 0 BEGIN
		SELECT NULL AS ID
		ROLLBACK TRANSACTION
		RETURN
	END
	
	SELECT @FormInstanceID AS ID
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetHistoryFormInstances]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetHistoryFormInstances]
GO

CREATE PROCEDURE [dbo].[WF_GetHistoryFormInstances]
	@ApplicationID	uniqueidentifier,
	@strHistoryIDs	varchar(max),
	@delimiter		char,
	@Selected		bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF @Selected = 0 SET @Selected = NULL
	
	DECLARE @HistoryIDs GuidTableType
	INSERT INTO @HistoryIDs 
	SELECT DISTINCT Ref.Value FROM [dbo].[GFN_StrToGuidTable](@strHistoryIDs, @delimiter) AS Ref
	
	SELECT HFI.HistoryID AS HistoryID,
		   HFI.OutStateID AS OutStateID,
		   HFI.FormsID AS FormsID
	FROM @HistoryIDs AS ExternalIDs
		INNER JOIN [dbo].[WF_History] AS HS
		ON HS.ApplicationID = @ApplicationID AND HS.HistoryID = ExternalIDs.Value
		INNER JOIN [dbo].[WF_HistoryFormInstances] AS HFI
		ON HFI.ApplicationID = @ApplicationID AND HFI.HistoryID = HS.HistoryID
	WHERE (@Selected IS NULL OR 
		(HS.SelectedOutStateID IS NOT NULL AND HS.SelectedOutStateID = HFI.OutStateID)) AND
		HS.Deleted = 0 AND HFI.Deleted = 0
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_SendToNextState]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_SendToNextState]
GO

CREATE PROCEDURE [dbo].[WF_SendToNextState]
	@ApplicationID		uniqueidentifier,
    @PrevHistoryID		uniqueidentifier,
    @StateID			uniqueidentifier,
    @DirectorNodeID		uniqueidentifier,
    @DirectorUserID		uniqueidentifier,
    @Description		nvarchar(2000),
    @Reject				bit,
    @SenderUserID		uniqueidentifier,
    @SendDate			datetime,
	@AttachedFilesTemp	DocFileInfoTableType ReadOnly
WITH ENCRYPTION, RECOMPILE
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	DECLARE @AttachedFiles DocFileInfoTableType
	INSERT INTO @AttachedFiles SELECT * FROM @AttachedFilesTemp
	
	SET @Description = [dbo].[GFN_VerifyString](@Description)
	
	IF @Reject IS NULL SET @Reject = 0
	
	DECLARE @HistoryID uniqueidentifier, @OwnerID uniqueidentifier,
		@WorkFlowID uniqueidentifier, @PrevStateID uniqueidentifier
	
	SELECT @HistoryID = NEWID(), @OwnerID = OwnerID, 
		@WorkFlowID = WorkFlowID, @PrevStateID = StateID
	FROM [dbo].[WF_History]
	WHERE ApplicationID = @ApplicationID AND HistoryID = @PrevHistoryID
	
	IF @Reject = 0 BEGIN
		IF @DirectorNodeID IS NULL AND @DirectorUserID IS NULL BEGIN
			SELECT -5, N'NoDirectorIsSet'
			ROLLBACK TRANSACTION
			RETURN
		END
	END
	ELSE BEGIN
		DECLARE @MaxAllowedRejections int, @RejectionRefStateID uniqueidentifier
		
		SELECT @MaxAllowedRejections = MaxAllowedRejections, 
			   @RejectionRefStateID = RejectionRefStateID
		FROM [dbo].[WF_WorkFlowStates]
		WHERE ApplicationID = @ApplicationID AND WorkFlowID = @WorkFlowID AND StateID = @PrevStateID
		
		IF @MaxAllowedRejections IS NULL OR @MaxAllowedRejections <= 0 BEGIN
			SELECT -11, N'RejectionIsNotAllowed'
			ROLLBACK TRANSACTION
			RETURN
		END
		ELSE IF (
			SELECT COUNT(*) 
			FROM [dbo].[WF_History] 
			WHERE ApplicationID = @ApplicationID AND OwnerID = @OwnerID AND 
				WorkFlowID = @WorkFlowID AND StateID = @PrevHistoryID AND Rejected = 1 AND Deleted = 0
		) >= @MaxAllowedRejections BEGIN
			SELECT -12, N'MaxAllowedRejectionsExceeded'
			ROLLBACK TRANSACTION
			RETURN	
		END
		
		SELECT TOP(1) @DirectorNodeID = DirectorNodeID, @DirectorUserID = DirectorUserID,
			@StateID = StateID
		FROM [dbo].[WF_History]
		WHERE ApplicationID = @ApplicationID AND 
			OwnerID = @OwnerID AND WorkFlowID = @WorkFlowID AND 
			(@RejectionRefStateID IS NULL OR StateID = @RejectionRefStateID) AND
			HistoryID <> @PrevHistoryID AND Deleted = 0
		ORDER BY ID DESC
	END
	
	UPDATE [dbo].[WF_History]
		SET Rejected = @Reject,
			SelectedOutStateID = @StateID,
			[Description] = @Description,
			ActorUserID = @SenderUserID
	WHERE ApplicationID = @ApplicationID AND HistoryID = @PrevHistoryID
	
	IF @@ROWCOUNT <= 0 BEGIN
		SELECT -6, N'HistoryUpdateFailed'
		ROLLBACK TRANSACTION
		RETURN
	END
	
	INSERT INTO [dbo].[WF_History](
		ApplicationID,
		HistoryID,
		PreviousHistoryID,
		OwnerID,
		WorkFlowID,
		StateID,
		DirectorNodeID,
		DirectorUserID,
		Rejected,
		Terminated,
		SenderUserID,
		SendDate,
		Deleted
	)
	VALUES(
		@ApplicationID,
		@HistoryID,
		@PrevHistoryID,
		@OwnerID, 
		@WorkFlowID, 
		@StateID, 
		@DirectorNodeID, 
		@DirectorUserID, 
		0,
		0,
		@SenderUserID, 
		@SendDate, 
		0
	)
	
	IF @@ROWCOUNT <= 0 BEGIN
		SELECT -7, N'HistoryUpdateFailed'
		ROLLBACK TRANSACTION
		RETURN
	END
	
	DECLARE @DataNeedsType varchar(20), @RefDataNeedsStateID uniqueidentifier,
		@FormID uniqueidentifier
		
	SELECT @DataNeedsType = WFS.DataNeedsType, 
		   @RefDataNeedsStateID = WFS.RefDataNeedsStateID,
		   @FormID = FO.FormID
	FROM [dbo].[WF_WorkFlowStates] AS WFS
		LEFT JOIN [dbo].[FG_FormOwners] AS FO
		ON FO.ApplicationID = @ApplicationID AND FO.OwnerID = WFS.ID
	WHERE WFS.ApplicationID = @ApplicationID AND 
		WFS.WorkFlowID = @WorkFlowID AND WFS.StateID = @StateID
	
	DECLARE @_Result int
	
	IF @DataNeedsType = 'RefState' BEGIN
		IF EXISTS(
			SELECT TOP(1) * 
			FROM [dbo].[WF_StateDataNeedInstances]
			WHERE ApplicationID = @ApplicationID AND HistoryID = @PrevHistoryID AND Deleted = 0
		) BEGIN
			DECLARE @_NewNeeds Table(InstanceID uniqueidentifier, 
				NodeID uniqueidentifier, [Admin] bit)
		
			INSERT INTO [dbo].[WF_StateDataNeedInstances](
				ApplicationID,
				InstanceID,
				HistoryID,
				NodeID,
				[Admin],
				Filled,
				CreatorUserID,
				CreationDate,
				Deleted
			)
			SELECT @ApplicationID, NEWID(), @HistoryID, NodeID, 
				[Admin], 0, @SenderUserID, @SendDate, 0
			FROM [dbo].[WF_StateDataNeedInstances]
			WHERE ApplicationID = @ApplicationID AND HistoryID = @PrevHistoryID AND Deleted = 0
			
			IF @@ROWCOUNT <= 0 BEGIN
				SELECT -8, N'HistoryDateNeedsCreationFailed'
				ROLLBACK TRANSACTION
				RETURN
			END
		END
		
		EXEC [dbo].[FG_P_CopyFormInstances] @ApplicationID, @PrevHistoryID, 
			@HistoryID, @FormID, @SenderUserID, @SendDate, @_Result output
		
		IF @_Result <= 0 BEGIN
			SELECT -9, N'HistoryFormInstancesCopyFailed'
			ROLLBACK TRANSACTION
			RETURN	
		END
	END
	
	IF EXISTS(SELECT TOP(1) * FROM @AttachedFiles) BEGIN
		EXEC [dbo].[DCT_P_AddFiles] @ApplicationID, 
			@PrevHistoryID, N'WorkFlow', @AttachedFiles, @SenderUserID, @SendDate, @_Result output
		
		IF @_Result <= 0 BEGIN
			SELECT -10, N'FileAttachmentFailed'
			ROLLBACK TRANSACTION 
			RETURN
		END
	END
	
	-- Update WFState in CN_Nodes Table
	DECLARE @StateTitle nvarchar(1000) = (
		SELECT S.Title
		FROM [dbo].[WF_States] AS S
		WHERE S.ApplicationID = @ApplicationID AND S.StateID = @StateID
	)
	
	DECLARE @HideContributors bit = (
		SELECT TOP(1) ISNULL(S.HideOwnerName, 0)
		FROM [dbo].[WF_WorkFlowStates] AS S
		WHERE S.ApplicationID = @ApplicationID AND 
			S.WorkFlowID = @WorkFlowID AND S.StateID = @StateID
	)
	
	EXEC [dbo].[CN_P_ModifyNodeWFState] @ApplicationID, @OwnerID, 
		@StateTitle, @HideContributors, @SenderUserID, @SendDate, @_Result output
		
	IF @_Result <= 0 BEGIN
		SELECT -11, N'StatusUpdateFailed'
		ROLLBACK TRANSACTION 
		RETURN
	END
	-- end of Update WFState in CN_Nodes Table
	
	-- Send Dashboards
	EXEC [dbo].[NTFN_P_SetDashboardsAsDone] @ApplicationID, @SenderUserID, @OwnerID, 
		@PrevHistoryID, N'WorkFlow', NULL, @SendDate, @_Result output 
	
	IF @_Result <= 0 BEGIN
		SELECT -1, N'UpdatingDashboardsFailed'
		ROLLBACK TRANSACTION 
		RETURN
	END
	
	EXEC [dbo].[WF_P_SendDashboards] @ApplicationID, @HistoryID, @OwnerID, @WorkFlowID, 
		@StateID, @DirectorUserID, @DirectorNodeID, NULL, @SendDate, @_Result output
	
	IF @_Result <= 0 BEGIN
		SELECT -1, N'CannotDetermineDirector'
		ROLLBACK TRANSACTION 
		RETURN
	END
	-- end of Send Dashboards
	
	SELECT 1
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_TerminateWorkFlow]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_TerminateWorkFlow]
GO

CREATE PROCEDURE [dbo].[WF_TerminateWorkFlow]
	@ApplicationID		uniqueidentifier,
    @PrevHistoryID		uniqueidentifier,
    @Description		nvarchar(2000),
    @SenderUserID		uniqueidentifier,
    @SendDate			datetime
WITH ENCRYPTION
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	SET @Description = [dbo].[GFN_VerifyString](@Description)
	
	DECLARE @OwnerID uniqueidentifier, @WorkFlowID uniqueidentifier
	
	SELECT @OwnerID = OwnerID, @WorkFlowID = WorkFlowID
	FROM [dbo].[WF_History]
	WHERE ApplicationID = @ApplicationID AND HistoryID = @PrevHistoryID
	
	UPDATE [dbo].[WF_History]
		SET Terminated = 1,
			[Description] = @Description,
			ActorUserID = @SenderUserID
	WHERE ApplicationID = @ApplicationID AND HistoryID = @PrevHistoryID
	
	IF @@ROWCOUNT <= 0 BEGIN
		SELECT -6
		ROLLBACK TRANSACTION
		RETURN
	END
	
	-- Send Dashboards
	DECLARE @_Result int
	
	EXEC [dbo].[NTFN_P_SetDashboardsAsDone] @ApplicationID, @SenderUserID, @OwnerID, 
		@PrevHistoryID, N'WorkFlow', NULL, @SendDate, @_Result output 
	
	IF @_Result <= 0 BEGIN
		SELECT -1, N'RemovingDashboardsFailed'
		ROLLBACK TRANSACTION 
		RETURN
	END
	
	EXEC [dbo].[NTFN_P_ArithmeticDeleteDashboards] @ApplicationID, 
		NULL, @OwnerID, NULL, N'WorkFlow', NULL, @_Result output 
	
	IF @_Result <= 0 BEGIN
		SELECT -1, N'RemovingDashboardsFailed'
		ROLLBACK TRANSACTION 
		RETURN
	END
	-- end of Send Dashboards
	
	SELECT 1
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetViewerStatus]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetViewerStatus]
GO

CREATE PROCEDURE [dbo].[WF_GetViewerStatus]
	@ApplicationID	uniqueidentifier,
	@UserID			uniqueidentifier,
	@OwnerID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @HasWorkFlow bit = 0
	SELECT @HasWorkFlow = 1 
	WHERE EXISTS(
		SELECT TOP(1) * 
		FROM [dbo].[WF_History] 
		WHERE ApplicationID = @ApplicationID AND [OwnerID] = @OwnerID AND Deleted = 0
	)
		
	IF @HasWorkFlow = 0 BEGIN
		SELECT N'NotInWorkFlow'
		RETURN
	END
	
	DECLARE @IsOwner bit
	EXEC [dbo].[CN_P_IsNodeCreator] @ApplicationID, @OwnerID, @UserID, @IsOwner output
	
	IF @IsOwner IS NULL SET @IsOwner = 0
	
	DECLARE @WorkFlowID uniqueidentifier, @StateID uniqueidentifier,
		@DirectorNodeID uniqueidentifier, @DirectorUserID uniqueidentifier
	
	SELECT TOP(1) @WorkFlowID = WorkFlowID, @StateID = StateID,
		@DirectorNodeID = DirectorNodeID, @DirectorUserID = DirectorUserID
	FROM [dbo].[WF_History]
	WHERE ApplicationID = @ApplicationID AND OwnerID = @OwnerID AND Deleted = 0
	ORDER BY ID DESC
	
	IF @UserID = @DirectorUserID 
		SELECT N'Director'
	ELSE BEGIN
		DECLARE @IsAdminFromWorkFlow bit, @IsNodeMember bit, @IsAdmin bit = 0
		
		EXEC [dbo].[CN_P_IsNodeMember] @ApplicationID, 
			@DirectorNodeID, @UserID, @IsAdmin, 'Accepted', @IsNodeMember output
		
		IF @IsNodeMember = 1 BEGIN
			SET @IsAdminFromWorkFlow = (
				SELECT [Admin] 
				FROM [dbo].[WF_WorkFlowStates]
				WHERE ApplicationID = @ApplicationID AND 
					WorkFlowID = @WorkFlowID AND StateID = @StateID
			)
			
			IF @IsAdminFromWorkFlow IS NULL SET @IsAdminFromWorkFlow = 0
			
			IF @IsAdminFromWorkFlow = 1 BEGIN
				EXEC [dbo].[CN_P_IsNodeAdmin] @ApplicationID, 
					@DirectorNodeID, @UserID, @IsAdmin output
			END
		END
		ELSE BEGIN
			IF @IsOwner = 1 SELECT N'Owner'
			ELSE SELECT N'None'
			
			RETURN
		END
		
		IF (@IsAdminFromWorkFlow = 0 OR @IsAdmin = 1) SELECT N'Director'
		ELSE SELECT N'DirectorNodeMember'
	END	
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetNextStateParams]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetNextStateParams]
GO

CREATE PROCEDURE [dbo].[WF_GetNextStateParams]
	@ApplicationID	uniqueidentifier,
	@NodeID			uniqueidentifier,
    @WorkFlowID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @StartStateID uniqueidentifier
	DECLARE @ResponseType varchar(20)
	DECLARE @DirectorNodeID uniqueidentifier
	DECLARE @DirectorUserID uniqueidentifier
	
	DECLARE @_StartStateIDs GuidTableType
	INSERT INTO @_StartStateIDs
	SELECT WFS.StateID
	FROM [dbo].[WF_WorkFlowStates] AS WFS
	WHERE WFS.ApplicationID = @ApplicationID AND 
		WFS.WorkFlowID = @WorkFlowID AND WFS.Deleted = 0 AND
		NOT EXISTS(
			SELECT TOP(1) * 
			FROM [dbo].[WF_StateConnections] AS SC
				INNER JOIN [dbo].[WF_WorkFlowStates] AS Ref
				ON Ref.ApplicationID = @ApplicationID AND Ref.StateID = SC.InStateID
			WHERE SC.ApplicationID = @ApplicationID AND SC.WorkFlowID = @WorkFlowID AND 
				SC.OutStateID = WFS.StateID AND SC.Deleted = 0 AND Ref.Deleted = 0
		)
		
	SET @StartStateID = (SELECT TOP(1) Ref.Value FROM @_StartStateIDs AS Ref)
		
	SELECT @ResponseType = ResponseType, @DirectorNodeID = NodeID
	FROM [dbo].[WF_WorkFlowStates]
	WHERE ApplicationID = @ApplicationID AND 
		WorkFlowID = @WorkFlowID AND StateID = @StartStateID AND Deleted = 0
	
	IF @ResponseType = 'SendToOwner' BEGIN
		SET @DirectorNodeID = NULL
		
		SET @DirectorUserID = (
			SELECT TOP(1) CreatorUserID 
			FROM [dbo].[CN_Nodes] 
			WHERE ApplicationID = @ApplicationID AND NodeID = @NodeID
		)
	END
	ELSE IF @ResponseType = 'RefState'
		SET @DirectorNodeID = NULL
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_P_StartNewWorkFlow]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_P_StartNewWorkFlow]
GO

CREATE PROCEDURE [dbo].[WF_P_StartNewWorkFlow]
	@ApplicationID	uniqueidentifier,
	@NodeID			uniqueidentifier,
    @WorkFlowID		uniqueidentifier,
    @DirectorNodeID	uniqueidentifier,
	@DirectorUserID	uniqueidentifier,
	@CreatorUserID	uniqueidentifier,
	@CreationDate	datetime,
	@_Result		int output,
	@_Message		varchar(500) output
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @Terminated bit = NULL
	DECLARE @PreviousHistoryID uniqueidentifier = NULL
	
	SELECT TOP(1) @Terminated = Terminated, @PreviousHistoryID = HistoryID
	FROM [dbo].[WF_History] 
	WHERE ApplicationID = @ApplicationID AND OwnerID = @NodeID AND Deleted = 0
	ORDER BY ID DESC
	
	IF @Terminated = 0 BEGIN -- If result is null or equals 1, workflow doesn't exist or has been terminated
		SET @_Result = -1
		SET @_Message = N'TheNodeIsAlreadyInWorkFlow'
		RETURN
	END
	
	DECLARE @StartStateID uniqueidentifier
	DECLARE @ResponseType varchar(20)
	
	DECLARE @_StartStateIDs GuidTableType
	
	INSERT INTO @_StartStateIDs
	SELECT WFS.StateID
	FROM [dbo].[WF_WorkFlowStates] AS WFS
	WHERE WFS.ApplicationID = @ApplicationID AND WFS.WorkFlowID = @WorkFlowID AND WFS.Deleted = 0 AND
		NOT EXISTS(
			SELECT TOP(1) * 
			FROM [dbo].[WF_StateConnections] AS SC
				INNER JOIN [dbo].[WF_WorkFlowStates] AS Ref
				ON Ref.ApplicationID = @ApplicationID AND Ref.StateID = SC.InStateID
			WHERE SC.ApplicationID = @ApplicationID AND SC.WorkFlowID = @WorkFlowID AND 
				SC.OutStateID = WFS.StateID AND SC.Deleted = 0 AND Ref.Deleted = 0
		)
		
	IF (SELECT COUNT(*) FROM @_StartStateIDs) <> 1 BEGIN
		SET @_Result = -1
		SET @_Message = N'WorkFlowStateNotFound'
		RETURN
	END
	ELSE
		SET @StartStateID = (SELECT TOP(1) Ref.Value FROM @_StartStateIDs AS Ref)
	
	DECLARE @HistoryID uniqueidentifier = NEWID()
	
	INSERT INTO [dbo].[WF_History](
		ApplicationID,
		HistoryID,
		OwnerID,
		WorkFlowID,
		StateID,
		PreviousHistoryID,
		DirectorNodeID,
		DirectorUserID,
		Rejected,
		Terminated,
		SenderUserID,
		SendDate,
		Deleted
	)
	VALUES(
		@ApplicationID,
		@HistoryID,
		@NodeID,
		@WorkFlowID,
		@StartStateID,
		@PreviousHistoryID,
		@DirectorNodeID,
		@DirectorUserID,
		0,
		0,
		@CreatorUserID,
		@CreationDate,
		0
	)
	
	IF @@ROWCOUNT <= 0 BEGIN
		SET @_Result = -1
		SET @_Message = NULL
		RETURN
	END
	
	-- Update WFState in CN_Nodes Table
	DECLARE @StateTitle nvarchar(1000) = (
		SELECT Title 
		FROM [dbo].[WF_States] 
		WHERE ApplicationID = @ApplicationID AND StateID = @StartStateID
	)
	
	DECLARE @HideContributors bit = (
		SELECT TOP(1) ISNULL(S.HideOwnerName, 0)
		FROM [dbo].[WF_WorkFlowStates] AS S
		WHERE S.ApplicationID = @ApplicationID AND 
			S.WorkFlowID = @WorkFlowID AND S.StateID = @StartStateID
	)
	
	EXEC [dbo].[CN_P_ModifyNodeWFState] @ApplicationID, @NodeID, 
		@StateTitle, @HideContributors, @CreatorUserID, @CreationDate, @_Result output
		
	IF @_Result <= 0 BEGIN
		SET @_Result = -11
		SET @_Message = N'StatusUpdateFailed'
		RETURN
	END
	-- end of Update WFState in CN_Nodes Table
	
	-- Send Dashboards
	EXEC [dbo].[WF_P_SendDashboards] @ApplicationID, @HistoryID, @NodeID, @WorkFlowID, 
		@StartStateID, @DirectorUserID, @DirectorNodeID, NULL, @CreationDate, @_Result output
	
	IF @_Result <= 0 BEGIN
		SET @_Result = -1
		SET @_Message = N'CannotDetermineDirector'
		RETURN
	END
	-- end of Send Dashboards
	
	SET @_Result = 1
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_RestartWorkFlow]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_RestartWorkFlow]
GO

CREATE PROCEDURE [dbo].[WF_RestartWorkFlow]
	@ApplicationID	uniqueidentifier,
	@OwnerID		uniqueidentifier,
	@DirectorNodeID	uniqueidentifier,
	@DirectorUserID	uniqueidentifier,
	@CreatorUserID	uniqueidentifier,
	@CreationDate	datetime
WITH ENCRYPTION
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	DECLARE @_Result int
	DECLARE @_Message varchar(500) = NULL
	
	DECLARE @WorkFlowID uniqueidentifier
	
	SELECT TOP(1) @WorkFlowID = WorkFlowID
	FROM [dbo].[WF_History] 
	WHERE ApplicationID = @ApplicationID AND OwnerID = @OwnerID
	ORDER BY ID DESC
	
	IF @WorkFlowID IS NULL BEGIN
		SELECT -1, N'WorkFlowNotFound'
	END
	ELSE BEGIN
		EXEC [dbo].[WF_P_StartNewWorkFlow] @ApplicationID, @OwnerID, @WorkFlowID, @DirectorNodeID,
			@DirectorUserID, @CreatorUserID, @CreationDate, @_Result output, @_Message output
	
		IF @_Result > 0 SELECT @_Result
		ELSE BEGIN 
			SELECT @_Result, @_Message
			ROLLBACK TRANSACTION
			RETURN
		END
	END
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_HasWorkFlow]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_HasWorkFlow]
GO

CREATE PROCEDURE [dbo].[WF_HasWorkFlow]
	@ApplicationID	uniqueidentifier,
	@OwnerID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT 1 
	WHERE EXISTS(
		SELECT TOP(1) * 
		FROM [dbo].[WF_History] 
		WHERE ApplicationID = @ApplicationID AND [OwnerID] = @OwnerID AND Deleted = 0
	)
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_IsTerminated]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_IsTerminated]
GO

CREATE PROCEDURE [dbo].[WF_IsTerminated]
	@ApplicationID	uniqueidentifier,
	@OwnerID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT TOP(1) Terminated
	FROM [dbo].[WF_History]
	WHERE ApplicationID = @ApplicationID AND OwnerID = @OwnerID AND Deleted = 0
	ORDER BY ID DESC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetServiceAbstract]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetServiceAbstract]
GO

CREATE PROCEDURE [dbo].[WF_GetServiceAbstract]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@NodeTypeID		uniqueidentifier,
	@UserID			uniqueidentifier,
	@NullTagLabel	nvarchar(256)
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

	SET @NullTagLabel = [dbo].[GFN_VerifyString](@NullTagLabel)

	SELECT ISNULL(TG.Tag, @NullTagLabel) AS Tag, Counts.CNT AS [Count]
	FROM
		(
			SELECT Owners.TagID, COUNT(OwnerID) AS CNT 
			FROM
				(
					SELECT A.OwnerID, WS.TagID
					FROM [dbo].[WF_History] AS A
						INNER JOIN (
							SELECT OwnerID, MAX(ID) AS ID
							FROM [dbo].[WF_History]
							WHERE ApplicationID = @ApplicationID AND Deleted = 0
							GROUP BY OwnerID
						) AS B
						ON B.ID = A.ID AND B.OwnerID = A.OwnerID
						INNER JOIN [dbo].[CN_Nodes] AS ND
						ON ND.ApplicationID = @ApplicationID AND ND.NodeID = A.OwnerID
						INNER JOIN [dbo].[WF_WorkFlowStates] AS WS
						ON WS.ApplicationID = @ApplicationID AND WS.StateID = A.StateID
					WHERE A.ApplicationID = @ApplicationID AND
						(@WorkFlowID IS NULL OR 
							(A.WorkFlowID = @WorkFlowID AND WS.WorkFlowID = @WorkFlowID)
						) AND ND.NodeTypeID = @NodeTypeID AND ND.Deleted = 0 AND
						(@UserID IS NULL OR	
							EXISTS(
								SELECT TOP(1) * 
								FROM [dbo].[CN_NodeCreators]
								WHERE ApplicationID = @ApplicationID AND 
									NodeID = A.OwnerID AND UserID = @UserID AND Deleted = 0
							)
						)
				) AS Owners
			GROUP BY Owners.TagID
		) AS Counts
		LEFT JOIN [dbo].[CN_Tags] AS TG
		ON TG.ApplicationID = @ApplicationID AND TG.TagID = Counts.TagID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetServiceUserIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetServiceUserIDs]
GO

CREATE PROCEDURE [dbo].[WF_GetServiceUserIDs]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@NodeTypeID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT DISTINCT NC.UserID AS ID
	FROM
		(
			SELECT DISTINCT OwnerID
			FROM [dbo].[WF_History]
			WHERE ApplicationID = @ApplicationID AND 
				(@WorkFlowID IS NULL OR WorkFlowID = @WorkFlowID) AND Deleted = 0
		) AS NDS
		INNER JOIN [dbo].[CN_View_Nodes_Normal] AS ND
		ON ND.ApplicationID = @ApplicationID AND ND.NodeID = NDS.OwnerID
		INNER JOIN [dbo].[CN_NodeCreators] AS NC
		ON NC.ApplicationID = @ApplicationID AND NC.NodeID = NDS.OwnerID
	WHERE (@NodeTypeID IS NULL OR ND.NodeTypeID = @NodeTypeID) AND 
		ND.Deleted = 0 AND NC.Deleted = 0
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetWorkFlowItemsCount]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetWorkFlowItemsCount]
GO

CREATE PROCEDURE [dbo].[WF_GetWorkFlowItemsCount]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT CAST(COUNT(DISTINCT H.OwnerID) AS int)
	FROM [dbo].[WF_History] AS H
		INNER JOIN [dbo].[CN_Nodes] AS ND
		ON ND.ApplicationID = @ApplicationID AND ND.NodeID = H.OwnerID AND ND.Deleted = 0
	WHERE H.ApplicationID = @ApplicationID AND H.WorkFlowID = @WorkFlowID AND H.Deleted = 0
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetWorkFlowStateItemsCount]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetWorkFlowStateItemsCount]
GO

CREATE PROCEDURE [dbo].[WF_GetWorkFlowStateItemsCount]
	@ApplicationID	uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@StateID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT CAST(COUNT(DISTINCT H.OwnerID) AS int)
	FROM [dbo].[WF_History] AS H
		INNER JOIN (
			SELECT A.OwnerID, MAX(A.ID) AS ID
			FROM [dbo].[WF_History] AS A
			WHERE A.ApplicationID = @ApplicationID AND A.WorkFlowID = @WorkFlowID AND A.Deleted = 0
			GROUP BY A.OwnerID
		) AS X
		ON X.ID = H.ID
		INNER JOIN [dbo].[CN_Nodes] AS ND
		ON ND.ApplicationID = @ApplicationID AND ND.NodeID = H.OwnerID AND ND.Deleted = 0
	WHERE H.ApplicationID = @ApplicationID AND H.StateID = @StateID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetUserWorkFlowItemsCount]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetUserWorkFlowItemsCount]
GO

CREATE PROCEDURE [dbo].[WF_GetUserWorkFlowItemsCount]
	@ApplicationID			uniqueidentifier,
	@UserID					uniqueidentifier,
	@LowerCreationDateLimit	datetime,
	@UpperCreationDateLimit	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT Counts.NodeTypeID AS NodeTypeID,
		   NT.Name AS NodeType, 
		   Counts.CNT AS [Count]
	FROM (
			SELECT ND.NodeTypeID, COUNT(NC.NodeID) AS CNT
			FROM [dbo].[CN_NodeCreators] AS NC
				INNER JOIN [dbo].[CN_View_Nodes_Normal] AS ND
				ON ND.ApplicationID = @ApplicationID AND ND.NodeID = NC.NodeID
				INNER JOIN [dbo].[CN_Services] AS SR
				ON SR.ApplicationID = @ApplicationID AND SR.NodeTypeID = ND.NodeTypeID
			WHERE NC.ApplicationID = @ApplicationID AND NC.UserID = @UserID AND 
				EXISTS(
					SELECT TOP(1) * 
					FROM [dbo].[WF_History]
					WHERE ApplicationID = @ApplicationID AND OwnerID = ND.NodeID AND Deleted = 0
				) AND NC.Deleted = 0 AND ND.Deleted = 0 AND
				(@LowerCreationDateLimit IS NULL OR ND.CreationDate >= @LowerCreationDateLimit) AND
				(@UpperCreationDateLimit IS NULL OR ND.CreationDate <= @UpperCreationDateLimit)
			GROUP BY ND.NodeTypeID
		) AS Counts
		INNER JOIN [dbo].[CN_NodeTypes] AS NT
		ON NT.ApplicationID = @ApplicationID AND NT.NodeTypeID = Counts.NodeTypeID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_AddOwnerWorkFlow]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_AddOwnerWorkFlow]
GO

CREATE PROCEDURE [dbo].[WF_AddOwnerWorkFlow]
	@ApplicationID	uniqueidentifier,
	@NodeTypeID		uniqueidentifier,
	@WorkFlowID		uniqueidentifier,
	@CreatorUserID	uniqueidentifier,
	@CreationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF EXISTS(
		SELECT TOP(1) * 
		FROM [dbo].[WF_WorkFlowOwners]
		WHERE ApplicationID = @ApplicationID AND
			NodeTypeID = @NodeTypeID AND WorkFlowID = @WorkFlowID
	) BEGIN
		UPDATE [dbo].[WF_WorkFlowOwners]
			SET Deleted = 0,
				LastModifierUserID = @CreatorUserID,
				LastModificationDate = @CreationDate
		WHERE ApplicationID = @ApplicationID AND 
			NodeTypeID = @NodeTypeID AND WorkFlowID = @WorkFlowID
	END
	ELSE BEGIN
		INSERT INTO [dbo].[WF_WorkFlowOwners](
			ApplicationID,
			ID,
			NodeTypeID,
			WorkFlowID,
			CreatorUserID,
			CreationDate,
			Deleted
		)
		VALUES(
			@ApplicationID,
			NEWID(),
			@NodeTypeID,
			@WorkFlowID,
			@CreatorUserID,
			@CreationDate,
			0
		)
	END
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_ArithmeticDeleteOwnerWorkFlow]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_ArithmeticDeleteOwnerWorkFlow]
GO

CREATE PROCEDURE [dbo].[WF_ArithmeticDeleteOwnerWorkFlow]
	@ApplicationID			uniqueidentifier,
	@NodeTypeID				uniqueidentifier,
	@WorkFlowID				uniqueidentifier,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[WF_WorkFlowOwners]
		SET Deleted = 1,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND 
		NodeTypeID = @NodeTypeID AND WorkFlowID = @WorkFlowID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetOwnerWorkFlows]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetOwnerWorkFlows]
GO

CREATE PROCEDURE [dbo].[WF_GetOwnerWorkFlows]
	@ApplicationID	uniqueidentifier,
	@NodeTypeID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @WorkFlowIDs GuidTableType
	
	INSERT INTO @WorkFlowIDs
	SELECT WorkFlowID
	FROM [dbo].[WF_WorkFlowOwners]
	WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeID AND Deleted = 0
	
	EXEC [dbo].[WF_P_GetWorkFlowsByIDs] @ApplicationID, @WorkFlowIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetOwnerWorkFlowPrimaryKey]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetOwnerWorkFlowPrimaryKey]
GO

CREATE PROCEDURE [dbo].[WF_GetOwnerWorkFlowPrimaryKey]
	@ApplicationID	uniqueidentifier,
	@NodeTypeID		uniqueidentifier,
	@WorkFlowID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

	SELECT ID
	FROM [dbo].[WF_WorkFlowOwners]
	WHERE ApplicationID = @ApplicationID AND 
		NodeTypeID = @NodeTypeID AND WorkFlowID = @WorkFlowID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetWorkFlowOwnerIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetWorkFlowOwnerIDs]
GO

CREATE PROCEDURE [dbo].[WF_GetWorkFlowOwnerIDs]
	@ApplicationID	uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT DISTINCT NodeTypeID AS ID
	FROM [dbo].[WF_WorkFlowOwners]
	WHERE ApplicationID = @ApplicationID AND Deleted = 0
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetFormInstanceWorkFlowOwnerID]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetFormInstanceWorkFlowOwnerID]
GO

CREATE PROCEDURE [dbo].[WF_GetFormInstanceWorkFlowOwnerID]
	@ApplicationID		uniqueidentifier,
	@FormInstanceID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT TOP(1) H.OwnerID
	FROM [dbo].[FG_FormInstances] AS FI
		INNER JOIN [dbo].[WF_HistoryFormInstances] AS HFI
		ON HFI.ApplicationID = @ApplicationID AND HFI.FormsID = FI.OwnerID
		INNER JOIN [dbo].[WF_History] AS H
		ON H.ApplicationID = @ApplicationID AND H.HistoryID = HFI.HistoryID
	WHERE FI.ApplicationID = @ApplicationID AND FI.InstanceID = @FormInstanceID
END

GO