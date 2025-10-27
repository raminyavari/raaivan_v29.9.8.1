USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_P_InitializeService]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_P_InitializeService]
GO

CREATE PROCEDURE [dbo].[CN_P_InitializeService]
	@ApplicationID	uniqueidentifier,
	@NodeTypeID		uniqueidentifier,
	@_Result		int output
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF EXISTS(
		SELECT TOP(1) * 
		FROM [dbo].[CN_Services] 
		WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeID
	) BEGIN
		UPDATE [dbo].[CN_Services]
			SET Deleted = 0
		WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeID
	END
	ELSE BEGIN
		DECLARE @SeqNo int = 
			ISNULL((
				SELECT MAX(SequenceNumber) 
				FROM [dbo].[CN_Services]
				WHERE ApplicationID = @ApplicationID
			), 0) + 1
		
		INSERT INTO [dbo].[CN_Services](
			ApplicationID,
			NodeTypeID,
			EnableContribution,
			EditableForAdmin,
			SequenceNumber,
			EditableForCreator,
			EditableForOwners,
			EditableForExperts,
			EditableForMembers,
			EditSuggestion,
			Deleted
		)
		VALUES(
			@ApplicationID,
			@NodeTypeID,
			0,
			1,
			@SeqNo,
			1,
			1,
			1,
			0,
			1,
			0
		)
	END
	
	SET @_Result = @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_InitializeService]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_InitializeService]
GO

CREATE PROCEDURE [dbo].[CN_InitializeService]
	@ApplicationID	uniqueidentifier,
	@NodeTypeID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @_Result int = 0
	
	EXEC [dbo].[CN_P_InitializeService] @ApplicationID, @NodeTypeID, @_Result output
	
	SELECT @_Result
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_P_GetServicesByIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_P_GetServicesByIDs]
GO

CREATE PROCEDURE [dbo].[CN_P_GetServicesByIDs]
	@ApplicationID		uniqueidentifier,
	@NodeTypeIDsTemp	KeyLessGuidTableType readonly
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @NodeTypeIDs KeyLessGuidTableType
	INSERT INTO @NodeTypeIDs (Value) SELECT Value FROM @NodeTypeIDsTemp
	
	SELECT S.NodeTypeID,
		   NT.Name AS NodeType,
		   S.ServiceTitle,
		   S.ServiceDescription,
		   S.AdminType,
		   S.AdminNodeID,
		   S.MaxAcceptableAdminLevel,
		   S.LimitAttachedFilesTo,
		   S.MaxAttachedFileSize,
		   S.MaxAttachedFilesCount,
		   S.EnableContribution,
		   S.NoContent,
		   S.IsDocument,
		   S.EnablePreviousVersionSelect,
		   S.IsKnowledge,
		   S.IsTree,
		   S.UniqueMembership,
		   S.UniqueAdminMember,
		   S.DisableAbstractAndKeywords,
		   S.DisableFileUpload,
		   S.DisableRelatedNodesSelect,
		   S.EditableForAdmin,
		   S.EditableForCreator,
		   S.EditableForOwners,
		   S.EditableForExperts,
		   S.EditableForMembers,
		   ISNULL(S.EditSuggestion, 1) AS EditSuggestion
	FROM @NodeTypeIDs AS Ref
		INNER JOIN [dbo].[CN_Services] AS S
		ON S.ApplicationID = @ApplicationID AND S.NodeTypeID = Ref.Value
		INNER JOIN [dbo].[CN_NodeTypes] AS NT
		ON NT.ApplicationID = @ApplicationID AND NT.NodeTypeID = S.NodeTypeID
	ORDER BY Ref.SequenceNumber ASC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_GetServicesByIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_GetServicesByIDs]
GO

CREATE PROCEDURE [dbo].[CN_GetServicesByIDs]
	@ApplicationID	uniqueidentifier,
	@strNodeTypeIDs	varchar(max),
	@delimiter		char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @NodeTypeIDs KeyLessGuidTableType
	
	INSERT INTO @NodeTypeIDs (Value)
	SELECT Ref.Value
	FROM [dbo].[GFN_StrToGuidTable](@strNodeTypeIDs, @delimiter) AS Ref
	
	EXEC [dbo].[CN_P_GetServicesByIDs] @ApplicationID, @NodeTypeIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_GetServices]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_GetServices]
GO

CREATE PROCEDURE [dbo].[CN_GetServices]
	@ApplicationID	uniqueidentifier,
	@NodeTypeID		uniqueidentifier,
	@CurrentUserID	uniqueidentifier,
	@IsDocument		bit,
	@IsKnowledge	bit,
	@CheckPrivacy	bit,
	@Now			datetime,
	@DefaultPrivacy varchar(20)
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF @NodeTypeID IS NOT NULL BEGIN
		SET @NodeTypeID = ISNULL(
			(
				SELECT TOP(1) NodeTypeID 
				FROM [dbo].[CN_Nodes] 
				WHERE ApplicationID = @ApplicationID AND NodeID = @NodeTypeID
			),
			@NodeTypeID
		)
	END
	
	DECLARE @tempIDs KeyLessGuidTableType
	
	INSERT INTO @tempIDs (Value)
	SELECT S.NodeTypeID
	FROM [dbo].[CN_Services] AS S
		INNER JOIN [dbo].[CN_NodeTypes] AS NT
		ON NT.ApplicationID = @ApplicationID AND 
			NT.NodeTypeID = S.NodeTypeID AND ISNULL(NT.Deleted, 0) = 0
	WHERE S.ApplicationID = @ApplicationID AND
		(@NodeTypeID IS NULL OR S.NodeTypeID = @NodeTypeID) AND
		(@IsDocument IS NULL OR S.IsDocument = @IsDocument) AND
		(@IsKnowledge IS NULL OR S.IsKnowledge = @IsKnowledge) AND
		(@NodeTypeID IS NOT NULL OR (S.ServiceTitle IS NOT NULL AND S.ServiceTitle <> N'')) AND 
		S.Deleted = 0
	ORDER BY NT.SequenceNumber ASC, NT.CreationDate DESC
	
	DECLARE @IDs KeyLessGuidTableType
	
	IF @NodeTypeID IS NULL AND @CheckPrivacy = 1 BEGIN
		DECLARE	@PermissionTypes StringPairTableType
		
		INSERT INTO @PermissionTypes (FirstValue, SecondValue)
		VALUES (N'Create', @DefaultPrivacy)

		INSERT INTO @IDs (Value)
		SELECT Ref.ID
		FROM [dbo].[PRVC_FN_CheckAccess](@ApplicationID, @CurrentUserID, 
			@tempIDs, N'NodeType', @Now, @PermissionTypes) AS Ref
			INNER JOIN @tempIDs AS T
			ON T.Value = Ref.ID
		ORDER BY T.SequenceNumber ASC
	END
	ELSE BEGIN
		INSERT INTO @IDs (Value)
		SELECT Ref.Value
		FROM @tempIDs AS Ref
		WHERE (@NodeTypeID IS NULL OR Ref.Value = @NodeTypeID)
		ORDER BY Ref.SequenceNumber ASC
	END
	
	EXEC [dbo].[CN_P_GetServicesByIDs] @ApplicationID, @IDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_SetServiceTitle]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_SetServiceTitle]
GO

CREATE PROCEDURE [dbo].[CN_SetServiceTitle]
	@ApplicationID	uniqueidentifier,
	@NodeTypeID		uniqueidentifier,
	@Title			nvarchar(512)
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[CN_Services]
		SET ServiceTitle = [dbo].[GFN_VerifyString](@Title)
	WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_SetServiceDescription]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_SetServiceDescription]
GO

CREATE PROCEDURE [dbo].[CN_SetServiceDescription]
	@ApplicationID	uniqueidentifier,
	@NodeTypeID		uniqueidentifier,
	@Description	nvarchar(4000)
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[CN_Services]
		SET ServiceDescription = [dbo].[GFN_VerifyString](@Description)
	WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_SetServiceSuccessMessage]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_SetServiceSuccessMessage]
GO

CREATE PROCEDURE [dbo].[CN_SetServiceSuccessMessage]
	@ApplicationID	uniqueidentifier,
	@NodeTypeID		uniqueidentifier,
	@Message		nvarchar(4000)
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[CN_Services]
		SET [SuccessMessage] = @Message
	WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_GetServiceSuccessMessage]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_GetServiceSuccessMessage]
GO

CREATE PROCEDURE [dbo].[CN_GetServiceSuccessMessage]
	@ApplicationID	uniqueidentifier,
	@NodeTypeID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT SuccessMessage
	FROM [dbo].[CN_Services]
	WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_SetServiceAdminType]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_SetServiceAdminType]
GO

CREATE PROCEDURE [dbo].[CN_SetServiceAdminType]
	@ApplicationID			uniqueidentifier,
	@NodeTypeID				uniqueidentifier,
	@AdminType				varchar(20),
	@AdminNodeID			uniqueidentifier,
	@strLimitNodeTypeIDs	varchar(max),
	@delimiter				char,
	@CreatorUserID			uniqueidentifier,
	@CreationDate			datetime
WITH ENCRYPTION
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	UPDATE [dbo].[CN_Services]
		SET AdminType = @AdminType,
			AdminNodeID = ISNULL(@AdminNodeID, AdminNodeID)
	WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeID
	
	IF @@ROWCOUNT <= 0 BEGIN
		SELECT -1
		ROLLBACK TRANSACTION
		RETURN
	END
	
	UPDATE [dbo].[CN_AdminTypeLimits]
		SET Deleted = 1
	WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeID
	
	DECLARE @NodeTypeIDs GuidTableType
	
	INSERT INTO @NodeTypeIDs
	SELECT Ref.Value FROM [dbo].[GFN_StrToGuidTable](@strLimitNodeTypeIDs, @delimiter) AS Ref
	
	DECLARE @ExistingIDs GuidTableType
	
	INSERT INTO @ExistingIDs
	SELECT Ref.Value
	FROM @NodeTypeIDs AS Ref
		INNER JOIN [dbo].[CN_AdminTypeLimits] AS ATL
		ON ATL.LimitNodeTypeID = Ref.Value
	WHERE ATL.ApplicationID = @ApplicationID AND ATL.NodeTypeID = @NodeTypeID
	
	DECLARE @ExisintgCount int = (SELECT COUNT(*) FROM @ExistingIDs)
	
	IF @ExisintgCount > 0 BEGIN
		UPDATE ATL
			SET LastModifierUserID = @CreatorUserID,
				LastModificationDate = @CreationDate,
				Deleted = 0
		FROM @ExistingIDs AS Ref
			INNER JOIN [dbo].[CN_AdminTypeLimits] AS ATL
			ON ATL.LimitNodeTypeID = Ref.Value
		WHERE ATL.ApplicationID = @ApplicationID AND ATL.NodeTypeID = @NodeTypeID
		
		IF @@ROWCOUNT <= 0 BEGIN
			SELECT -1
			ROLLBACK TRANSACTION
			RETURN
		END
	END
	
	IF (SELECT COUNT(*) FROM @NodeTypeIDs) > @ExisintgCount BEGIN
		INSERT INTO [dbo].[CN_AdminTypeLimits](
			ApplicationID,
			NodeTypeID,
			LimitNodeTypeID,
			CreatorUserID,
			CreationDate,
			Deleted
		)
		SELECT @ApplicationID, @NodeTypeID, Ref.Value, @CreatorUserID, @CreationDate, 0
		FROM @NodeTypeIDs AS Ref
		WHERE Ref.Value NOT IN(SELECT E.Value FROM @ExistingIDs AS E)
		
		IF @@ROWCOUNT <= 0 BEGIN
			SELECT -1
			ROLLBACK TRANSACTION
			RETURN
		END
	END
	
	SELECT 1
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_GetAdminAreaLimits]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_GetAdminAreaLimits]
GO

CREATE PROCEDURE [dbo].[CN_GetAdminAreaLimits]
	@ApplicationID		uniqueidentifier,
    @NodeIDOrTypeID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @NodeTypeIDs KeyLessGuidTableType
	
	DECLARE @NodeTypeID uniqueidentifier = (
		SELECT NodeTypeID 
		FROM [dbo].[CN_Nodes]
		WHERE ApplicationID = @ApplicationID AND NodeID = @NodeIDOrTypeID
	)
	
	IF @NodeTypeID IS NULL SET @NodeTypeID = @NodeIDOrTypeID
	
	INSERT INTO @NodeTypeIDs (Value)
	SELECT LimitNodeTypeID
	FROM [dbo].[CN_AdminTypeLimits]
	WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeID AND Deleted = 0
	
	EXEC [dbo].[CN_P_GetNodeTypesByIDs] @ApplicationID, @NodeTypeIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_SetMaxAcceptableAdminLevel]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_SetMaxAcceptableAdminLevel]
GO

CREATE PROCEDURE [dbo].[CN_SetMaxAcceptableAdminLevel]
	@ApplicationID				uniqueidentifier,
	@NodeTypeID					uniqueidentifier,
	@MaxAcceptableAdminLevel	int
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[CN_Services]
		SET MaxAcceptableAdminLevel = @MaxAcceptableAdminLevel
	WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_SetContributionLimits]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_SetContributionLimits]
GO

CREATE PROCEDURE [dbo].[CN_SetContributionLimits]
	@ApplicationID			uniqueidentifier,
	@NodeTypeID				uniqueidentifier,
	@strLimitNodeTypeIDs	varchar(max),
	@delimiter				char,
	@CreatorUserID			uniqueidentifier,
	@CreationDate			datetime
WITH ENCRYPTION
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	UPDATE [dbo].[CN_ContributionLimits]
		SET Deleted = 1
	WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeID
	
	DECLARE @NodeTypeIDs GuidTableType
	
	INSERT INTO @NodeTypeIDs
	SELECT DISTINCT Ref.Value 
	FROM [dbo].[GFN_StrToGuidTable](@strLimitNodeTypeIDs, @delimiter) AS Ref
	
	DECLARE @ExistingIDs GuidTableType
	
	INSERT INTO @ExistingIDs
	SELECT Ref.Value
	FROM @NodeTypeIDs AS Ref
		INNER JOIN [dbo].[CN_ContributionLimits] AS CL
		ON CL.LimitNodeTypeID = Ref.Value
	WHERE CL.ApplicationID = @ApplicationID AND CL.NodeTypeID = @NodeTypeID
	
	DECLARE @ExisintgCount int = (SELECT COUNT(*) FROM @ExistingIDs)
	
	IF @ExisintgCount > 0 BEGIN
		UPDATE CL
			SET LastModifierUserID = @CreatorUserID,
				LastModificationDate = @CreationDate,
				Deleted = 0
		FROM @ExistingIDs AS Ref
			INNER JOIN [dbo].[CN_ContributionLimits] AS CL
			ON CL.LimitNodeTypeID = Ref.Value
		WHERE CL.ApplicationID = @ApplicationID AND CL.NodeTypeID = @NodeTypeID
		
		IF @@ROWCOUNT <= 0 BEGIN
			SELECT -1
			ROLLBACK TRANSACTION
			RETURN
		END
	END
	
	IF (SELECT COUNT(*) FROM @NodeTypeIDs) > @ExisintgCount BEGIN
		INSERT INTO [dbo].[CN_ContributionLimits](
			ApplicationID,
			NodeTypeID,
			LimitNodeTypeID,
			CreatorUserID,
			CreationDate,
			Deleted
		)
		SELECT @ApplicationID, @NodeTypeID, Ref.Value, @CreatorUserID, @CreationDate, 0
		FROM @NodeTypeIDs AS Ref
		WHERE Ref.Value NOT IN(SELECT E.Value FROM @ExistingIDs AS E)
		
		IF @@ROWCOUNT <= 0 BEGIN
			SELECT -1
			ROLLBACK TRANSACTION
			RETURN
		END
	END
	
	SELECT 1
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_GetContributionLimits]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_GetContributionLimits]
GO

CREATE PROCEDURE [dbo].[CN_GetContributionLimits]
	@ApplicationID		uniqueidentifier,
    @NodeIDOrTypeID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @NodeTypeIDs KeyLessGuidTableType
	
	DECLARE @NodeTypeID uniqueidentifier = (
		SELECT NodeTypeID 
		FROM [dbo].[CN_Nodes]
		WHERE ApplicationID = @ApplicationID AND NodeID = @NodeIDOrTypeID
	)
	
	IF @NodeTypeID IS NULL SET @NodeTypeID = @NodeIDOrTypeID
	
	INSERT INTO @NodeTypeIDs (Value)
	SELECT LimitNodeTypeID
	FROM [dbo].[CN_ContributionLimits]
	WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeID AND Deleted = 0
	
	EXEC [dbo].[CN_P_GetNodeTypesByIDs] @ApplicationID, @NodeTypeIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_EnableContribution]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_EnableContribution]
GO

CREATE PROCEDURE [dbo].[CN_EnableContribution]
	@ApplicationID	uniqueidentifier,
	@NodeTypeID		uniqueidentifier,
	@Enable			bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[CN_Services]
		SET EnableContribution = @Enable
	WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_NoContentService]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_NoContentService]
GO

CREATE PROCEDURE [dbo].[CN_NoContentService]
	@ApplicationID			uniqueidentifier,
	@NodeTypeIDOrNodeID		uniqueidentifier,
	@NoContent				bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SET @NodeTypeIDOrNodeID = ISNULL(
		(
			SELECT TOP(1) NodeTypeID 
			FROM [dbo].[CN_Nodes] 
			WHERE ApplicationID = @ApplicationID AND NodeID = @NodeTypeIDOrNodeID
		), @NodeTypeIDOrNodeID
	)
	
	IF @NoContent IS NULL BEGIN
		SELECT TOP(1) NoContent
		FROM [dbo].[CN_Services]
		WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeIDOrNodeID
	END
	ELSE BEGIN
		UPDATE [dbo].[CN_Services]
			SET NoContent = @NoContent
		WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeIDOrNodeID
		
		SELECT @@ROWCOUNT
	END
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_IsKnowledge]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_IsKnowledge]
GO

CREATE PROCEDURE [dbo].[CN_IsKnowledge]
	@ApplicationID			uniqueidentifier,
	@NodeTypeIDOrNodeID		uniqueidentifier,
	@IsKnowledge			bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SET @NodeTypeIDOrNodeID = ISNULL(
		(
			SELECT TOP(1) NodeTypeID 
			FROM [dbo].[CN_Nodes] 
			WHERE ApplicationID = @ApplicationID AND NodeID = @NodeTypeIDOrNodeID
		), @NodeTypeIDOrNodeID
	)
	
	IF @IsKnowledge IS NULL BEGIN
		SELECT TOP(1) IsKnowledge
		FROM [dbo].[CN_Services]
		WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeIDOrNodeID
	END
	ELSE BEGIN
		UPDATE [dbo].[CN_Services]
			SET IsKnowledge = @IsKnowledge
		WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeIDOrNodeID
		
		SELECT @@ROWCOUNT
	END
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_IsDocument]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_IsDocument]
GO

CREATE PROCEDURE [dbo].[CN_IsDocument]
	@ApplicationID			uniqueidentifier,
	@NodeTypeIDOrNodeID		uniqueidentifier,
	@IsDocument				bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SET @NodeTypeIDOrNodeID = ISNULL(
		(
			SELECT TOP(1) NodeTypeID 
			FROM [dbo].[CN_Nodes] 
			WHERE ApplicationID = @ApplicationID AND NodeID = @NodeTypeIDOrNodeID
		), @NodeTypeIDOrNodeID
	)
	
	IF @IsDocument IS NULL BEGIN
		SELECT TOP(1) IsDocument
		FROM [dbo].[CN_Services]
		WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeIDOrNodeID
	END
	ELSE BEGIN
		UPDATE [dbo].[CN_Services]
			SET IsDocument = @IsDocument
		WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeIDOrNodeID
		
		SELECT @@ROWCOUNT
	END
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_EnablePreviousVersionSelect]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_EnablePreviousVersionSelect]
GO

CREATE PROCEDURE [dbo].[CN_EnablePreviousVersionSelect]
	@ApplicationID			uniqueidentifier,
	@NodeTypeIDOrNodeID		uniqueidentifier,
	@Value					bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SET @NodeTypeIDOrNodeID = ISNULL(
		(
			SELECT TOP(1) NodeTypeID 
			FROM [dbo].[CN_Nodes] 
			WHERE ApplicationID = @ApplicationID AND NodeID = @NodeTypeIDOrNodeID
		), @NodeTypeIDOrNodeID
	)
	
	IF @Value IS NULL BEGIN
		SELECT TOP(1) EnablePreviousVersionSelect
		FROM [dbo].[CN_Services]
		WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeIDOrNodeID
	END
	ELSE BEGIN
		UPDATE [dbo].[CN_Services]
			SET EnablePreviousVersionSelect = @Value
		WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeIDOrNodeID
		
		SELECT @@ROWCOUNT
	END
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_IsTree]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_IsTree]
GO

CREATE PROCEDURE [dbo].[CN_IsTree]
	@ApplicationID			uniqueidentifier,
	@strNodeTypeOrNodeIDs	varchar(max),
	@delimiter				char,
	@IsTree					bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @NodeTypeIDs TABLE (FirstValue uniqueidentifier not null primary key clustered,
		SecondValue uniqueidentifier)
	
	INSERT INTO @NodeTypeIDs (FirstValue, SecondValue)
	SELECT DISTINCT ISNULL(ND.NodeTypeID, IDs.Value), ND.NodeID
	FROM [dbo].[GFN_StrToGuidTable](@strNodeTypeOrNodeIDs, @delimiter) AS IDs
		LEFT JOIN [dbo].[CN_Nodes] AS ND
		ON ND.ApplicationID = @ApplicationID AND ND.NodeID = IDs.Value
	
	IF @IsTree IS NULL BEGIN
		SELECT ISNULL(NT.SecondValue, NT.FirstValue) AS ID
		FROM @NodeTypeIDs AS NT
			INNER JOIN [dbo].[CN_Services] AS S
			ON S.NodeTypeID = NT.FirstValue
		WHERE S.ApplicationID = @ApplicationID AND S.IsTree = 1
	END
	ELSE BEGIN
		UPDATE S
			SET IsTree = @IsTree
		FROM @NodeTypeIDs AS NT
			INNER JOIN [dbo].[CN_Services] AS S
			ON S.NodeTypeID = NT.FirstValue
		WHERE S.ApplicationID = @ApplicationID AND NT.SecondValue IS NULL
		
		SELECT @@ROWCOUNT
	END
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_HasUniqueMembership]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_HasUniqueMembership]
GO

CREATE PROCEDURE [dbo].[CN_HasUniqueMembership]
	@ApplicationID			uniqueidentifier,
	@strNodeTypeOrNodeIDs	varchar(max),
	@delimiter				char,
	@Value					bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @NodeTypeIDs TABLE (FirstValue uniqueidentifier not null primary key clustered,
		SecondValue uniqueidentifier)
	
	INSERT INTO @NodeTypeIDs (FirstValue, SecondValue)
	SELECT DISTINCT ISNULL(ND.NodeTypeID, IDs.Value), ND.NodeID
	FROM [dbo].[GFN_StrToGuidTable](@strNodeTypeOrNodeIDs, @delimiter) AS IDs
		LEFT JOIN [dbo].[CN_Nodes] AS ND
		ON ND.ApplicationID = @ApplicationID AND ND.NodeID = IDs.Value
	
	IF @Value IS NULL BEGIN
		SELECT ISNULL(NT.SecondValue, NT.FirstValue) AS ID
		FROM @NodeTypeIDs AS NT
			INNER JOIN [dbo].[CN_Services] AS S
			ON S.NodeTypeID = NT.FirstValue
		WHERE S.ApplicationID = @ApplicationID AND S.UniqueMembership = 1
	END
	ELSE BEGIN
		UPDATE S
			SET UniqueMembership = @Value
		FROM @NodeTypeIDs AS NT
			INNER JOIN [dbo].[CN_Services] AS S
			ON S.NodeTypeID = NT.FirstValue
		WHERE S.ApplicationID = @ApplicationID AND NT.SecondValue IS NULL
		
		SELECT @@ROWCOUNT
	END
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_HasUniqueAdminMember]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_HasUniqueAdminMember]
GO

CREATE PROCEDURE [dbo].[CN_HasUniqueAdminMember]
	@ApplicationID			uniqueidentifier,
	@strNodeTypeOrNodeIDs	varchar(max),
	@delimiter				char,
	@Value					bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @NodeTypeIDs TABLE (FirstValue uniqueidentifier not null primary key clustered,
		SecondValue uniqueidentifier)
	
	INSERT INTO @NodeTypeIDs (FirstValue, SecondValue)
	SELECT DISTINCT ISNULL(ND.NodeTypeID, IDs.Value), ND.NodeID
	FROM [dbo].[GFN_StrToGuidTable](@strNodeTypeOrNodeIDs, @delimiter) AS IDs
		LEFT JOIN [dbo].[CN_Nodes] AS ND
		ON ND.ApplicationID = @ApplicationID AND ND.NodeID = IDs.Value
	
	IF @Value IS NULL BEGIN
		SELECT ISNULL(NT.SecondValue, NT.FirstValue) AS ID
		FROM @NodeTypeIDs AS NT
			INNER JOIN [dbo].[CN_Services] AS S
			ON S.NodeTypeID = NT.FirstValue
		WHERE S.ApplicationID = @ApplicationID AND S.UniqueAdminMember = 1
	END
	ELSE BEGIN
		UPDATE S
			SET UniqueAdminMember = @Value
		FROM @NodeTypeIDs AS NT
			INNER JOIN [dbo].[CN_Services] AS S
			ON S.NodeTypeID = NT.FirstValue
		WHERE S.ApplicationID = @ApplicationID AND NT.SecondValue IS NULL
		
		SELECT @@ROWCOUNT
	END
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_AbstractAndKeywordsDisabled]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_AbstractAndKeywordsDisabled]
GO

CREATE PROCEDURE [dbo].[CN_AbstractAndKeywordsDisabled]
	@ApplicationID			uniqueidentifier,
	@strNodeTypeOrNodeIDs	varchar(max),
	@delimiter				char,
	@Value					bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @NodeTypeIDs TABLE (FirstValue uniqueidentifier not null primary key clustered,
		SecondValue uniqueidentifier)
	
	INSERT INTO @NodeTypeIDs (FirstValue, SecondValue)
	SELECT DISTINCT ISNULL(ND.NodeTypeID, IDs.Value), ND.NodeID
	FROM [dbo].[GFN_StrToGuidTable](@strNodeTypeOrNodeIDs, @delimiter) AS IDs
		LEFT JOIN [dbo].[CN_Nodes] AS ND
		ON ND.ApplicationID = @ApplicationID AND ND.NodeID = IDs.Value
	
	IF @Value IS NULL BEGIN
		SELECT ISNULL(NT.SecondValue, NT.FirstValue) AS ID
		FROM @NodeTypeIDs AS NT
			INNER JOIN [dbo].[CN_Services] AS S
			ON S.NodeTypeID = NT.FirstValue
		WHERE S.ApplicationID = @ApplicationID AND S.DisableAbstractAndKeywords = 1
	END
	ELSE BEGIN
		UPDATE S
			SET DisableAbstractAndKeywords = @Value
		FROM @NodeTypeIDs AS NT
			INNER JOIN [dbo].[CN_Services] AS S
			ON S.NodeTypeID = NT.FirstValue
		WHERE S.ApplicationID = @ApplicationID AND NT.SecondValue IS NULL
		
		SELECT @@ROWCOUNT
	END
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_FileUploadDisabled]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_FileUploadDisabled]
GO

CREATE PROCEDURE [dbo].[CN_FileUploadDisabled]
	@ApplicationID			uniqueidentifier,
	@strNodeTypeOrNodeIDs	varchar(max),
	@delimiter				char,
	@Value					bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @NodeTypeIDs TABLE (FirstValue uniqueidentifier not null primary key clustered,
		SecondValue uniqueidentifier)
	
	INSERT INTO @NodeTypeIDs (FirstValue, SecondValue)
	SELECT DISTINCT ISNULL(ND.NodeTypeID, IDs.Value), ND.NodeID
	FROM [dbo].[GFN_StrToGuidTable](@strNodeTypeOrNodeIDs, @delimiter) AS IDs
		LEFT JOIN [dbo].[CN_Nodes] AS ND
		ON ND.ApplicationID = @ApplicationID AND ND.NodeID = IDs.Value
	
	IF @Value IS NULL BEGIN
		SELECT ISNULL(NT.SecondValue, NT.FirstValue) AS ID
		FROM @NodeTypeIDs AS NT
			INNER JOIN [dbo].[CN_Services] AS S
			ON S.NodeTypeID = NT.FirstValue
		WHERE S.ApplicationID = @ApplicationID AND S.DisableFileUpload = 1
	END
	ELSE BEGIN
		UPDATE S
			SET DisableFileUpload = @Value
		FROM @NodeTypeIDs AS NT
			INNER JOIN [dbo].[CN_Services] AS S
			ON S.NodeTypeID = NT.FirstValue
		WHERE S.ApplicationID = @ApplicationID AND NT.SecondValue IS NULL
		
		SELECT @@ROWCOUNT
	END
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_RelatedNodesSelectDisabled]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_RelatedNodesSelectDisabled]
GO

CREATE PROCEDURE [dbo].[CN_RelatedNodesSelectDisabled]
	@ApplicationID			uniqueidentifier,
	@strNodeTypeOrNodeIDs	varchar(max),
	@delimiter				char,
	@Value					bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @NodeTypeIDs TABLE (FirstValue uniqueidentifier not null primary key clustered,
		SecondValue uniqueidentifier)
	
	INSERT INTO @NodeTypeIDs (FirstValue, SecondValue)
	SELECT DISTINCT ISNULL(ND.NodeTypeID, IDs.Value), ND.NodeID
	FROM [dbo].[GFN_StrToGuidTable](@strNodeTypeOrNodeIDs, @delimiter) AS IDs
		LEFT JOIN [dbo].[CN_Nodes] AS ND
		ON ND.ApplicationID = @ApplicationID AND ND.NodeID = IDs.Value
	
	IF @Value IS NULL BEGIN
		SELECT ISNULL(NT.SecondValue, NT.FirstValue) AS ID
		FROM @NodeTypeIDs AS NT
			INNER JOIN [dbo].[CN_Services] AS S
			ON S.NodeTypeID = NT.FirstValue
		WHERE S.ApplicationID = @ApplicationID AND S.DisableRelatedNodesSelect = 1
	END
	ELSE BEGIN
		UPDATE S
			SET DisableRelatedNodesSelect = @Value
		FROM @NodeTypeIDs AS NT
			INNER JOIN [dbo].[CN_Services] AS S
			ON S.NodeTypeID = NT.FirstValue
		WHERE S.ApplicationID = @ApplicationID AND NT.SecondValue IS NULL
		
		SELECT @@ROWCOUNT
	END
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_EditableForAdmin]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_EditableForAdmin]
GO

CREATE PROCEDURE [dbo].[CN_EditableForAdmin]
	@ApplicationID	uniqueidentifier,
	@NodeTypeID		uniqueidentifier,
	@Editable		bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[CN_Services]
		SET EditableForAdmin = @Editable
	WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_EditableForCreator]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_EditableForCreator]
GO

CREATE PROCEDURE [dbo].[CN_EditableForCreator]
	@ApplicationID	uniqueidentifier,
	@NodeTypeID		uniqueidentifier,
	@Editable		bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[CN_Services]
		SET EditableForCreator = @Editable
	WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_EditableForOwners]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_EditableForOwners]
GO

CREATE PROCEDURE [dbo].[CN_EditableForOwners]
	@ApplicationID	uniqueidentifier,
	@NodeTypeID		uniqueidentifier,
	@Editable		bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[CN_Services]
		SET EditableForOwners = @Editable
	WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_EditableForExperts]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_EditableForExperts]
GO

CREATE PROCEDURE [dbo].[CN_EditableForExperts]
	@ApplicationID	uniqueidentifier,
	@NodeTypeID		uniqueidentifier,
	@Editable		bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[CN_Services]
		SET EditableForExperts = @Editable
	WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_EditableForMembers]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_EditableForMembers]
GO

CREATE PROCEDURE [dbo].[CN_EditableForMembers]
	@ApplicationID	uniqueidentifier,
	@NodeTypeID		uniqueidentifier,
	@Editable		bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[CN_Services]
		SET EditableForMembers = @Editable
	WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_EditSuggestion]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_EditSuggestion]
GO

CREATE PROCEDURE [dbo].[CN_EditSuggestion]
	@ApplicationID	uniqueidentifier,
	@NodeTypeID		uniqueidentifier,
	@Enable			bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[CN_Services]
		SET EditSuggestion = @Enable
	WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_AddFreeUser]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_AddFreeUser]
GO

CREATE PROCEDURE [dbo].[CN_AddFreeUser]
	@ApplicationID		uniqueidentifier,
	@NodeTypeID			uniqueidentifier,
	@UserID				uniqueidentifier,
	@CreatorUserID		uniqueidentifier,
	@CreationDate		datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF EXISTS(
		SELECT TOP(1) * 
		FROM [dbo].[CN_FreeUsers]
		WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeID AND UserID = @UserID
	) BEGIN	
		UPDATE [dbo].[CN_FreeUsers]
			SET LastModifierUserID = @CreatorUserID,
				LastModificationDate = @CreationDate,
				Deleted = 0
		WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeID AND UserID = @UserID
	END
	ELSE BEGIN
		INSERT INTO [dbo].[CN_FreeUsers](
			ApplicationID,
			NodeTypeID,
			UserID,
			CreatorUserID,
			CreationDate,
			Deleted
		)
		VALUES(
			@ApplicationID,
			@NodeTypeID,
			@UserID,
			@CreatorUserID,
			@CreationDate,
			0
		)
	END
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_ArithmeticDeleteFreeUser]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_ArithmeticDeleteFreeUser]
GO

CREATE PROCEDURE [dbo].[CN_ArithmeticDeleteFreeUser]
	@ApplicationID			uniqueidentifier,
	@NodeTypeID				uniqueidentifier,
	@UserID					uniqueidentifier,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[CN_FreeUsers]
		SET LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate,
			Deleted = 1
	WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeID AND UserID = @UserID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_GetFreeUserIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_GetFreeUserIDs]
GO

CREATE PROCEDURE [dbo].[CN_GetFreeUserIDs]
	@ApplicationID	uniqueidentifier,
	@NodeTypeID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT UserID AS ID
	FROM [dbo].[CN_FreeUsers]
	WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeID AND Deleted = 0
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_IsFreeUser]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_IsFreeUser]
GO

CREATE PROCEDURE [dbo].[CN_IsFreeUser]
	@ApplicationID			uniqueidentifier,
	@NodeTypeIDOrNodeID		uniqueidentifier,
	@UserID					uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF NOT EXISTS(
		SELECT TOP(1) NodeTypeID 
		FROM [dbo].[CN_NodeTypes] 
		WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeIDOrNodeID
	) BEGIN
		SELECT TOP(1) @NodeTypeIDOrNodeID = NodeTypeID
		FROM [dbo].[CN_Nodes]
		WHERE ApplicationID = @ApplicationID AND NodeID = @NodeTypeIDOrNodeID
	END
	
	SELECT 
		CASE 
			WHEN EXISTS(
				SELECT TOP(1) UserID
				FROM [dbo].[CN_FreeUsers]
				WHERE ApplicationID = @ApplicationID AND 
					NodeTypeID = @NodeTypeIDOrNodeID AND UserID = @UserID AND Deleted = 0
			) THEN 1
			ELSE 0
		END
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_AddServiceAdmin]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_AddServiceAdmin]
GO

CREATE PROCEDURE [dbo].[CN_AddServiceAdmin]
	@ApplicationID		uniqueidentifier,
	@NodeTypeID			uniqueidentifier,
	@UserID				uniqueidentifier,
	@CreatorUserID		uniqueidentifier,
	@CreationDate		datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF EXISTS(
		SELECT TOP(1) * 
		FROM [dbo].[CN_ServiceAdmins]
		WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeID AND UserID = @UserID
	) BEGIN
		
		UPDATE [dbo].[CN_ServiceAdmins]
			SET LastModifierUserID = @CreatorUserID,
				LastModificationDate = @CreationDate,
				Deleted = 0
		WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeID AND UserID = @UserID
	END
	ELSE BEGIN
		INSERT INTO [dbo].[CN_ServiceAdmins](
			ApplicationID,
			NodeTypeID,
			UserID,
			CreatorUserID,
			CreationDate,
			Deleted
		)
		VALUES(
			@ApplicationID,
			@NodeTypeID,
			@UserID,
			@CreatorUserID,
			@CreationDate,
			0
		)
	END
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_ArithmeticDeleteServiceAdmin]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_ArithmeticDeleteServiceAdmin]
GO

CREATE PROCEDURE [dbo].[CN_ArithmeticDeleteServiceAdmin]
	@ApplicationID			uniqueidentifier,
	@NodeTypeID				uniqueidentifier,
	@UserID					uniqueidentifier,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[CN_ServiceAdmins]
		SET LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate,
			Deleted = 1
	WHERE ApplicationID = @ApplicationID AND NodeTypeID = @NodeTypeID AND UserID = @UserID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_GetServiceAdminIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_GetServiceAdminIDs]
GO

CREATE PROCEDURE [dbo].[CN_GetServiceAdminIDs]
	@ApplicationID	uniqueidentifier,
	@NodeTypeID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT UserID AS ID
	FROM [dbo].[CN_ServiceAdmins]
	WHERE ApplicationID = @ApplicationID AND 
		(@NodeTypeID IS NULL OR NodeTypeID = @NodeTypeID) AND Deleted = 0
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_IsServiceAdmin]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_IsServiceAdmin]
GO

CREATE PROCEDURE [dbo].[CN_IsServiceAdmin]
	@ApplicationID	uniqueidentifier,
	@strIDs			varchar(max),
	@delimiter		char,
	@UserID			uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @IDs GuidTableType
	
	INSERT INTO @IDs (Value)
	SELECT Ref.Value
	FROM [dbo].[GFN_StrToGuidTable](@strIDs, @delimiter) AS Ref
	
	SELECT I.Value AS ID
	FROM @IDs AS I
		LEFT JOIN [dbo].[CN_Nodes] AS ND
		ON ND.ApplicationID = @ApplicationID AND ND.NodeID = I.Value
		INNER JOIN [dbo].[CN_ServiceAdmins] AS SA
		ON SA.ApplicationID = @ApplicationID AND 
			SA.NodeTypeID = ISNULL(ND.NodeTypeID, I.Value) AND 
			SA.UserID = @UserID AND SA.Deleted = 0
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_RegisterNewNode]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_RegisterNewNode]
GO

CREATE PROCEDURE [dbo].[CN_RegisterNewNode]
	@ApplicationID		uniqueidentifier,
	@NodeID				uniqueidentifier,
    @NodeTypeID			uniqueidentifier,
    @AdditionalID_Main	nvarchar(300),
    @AdditionalID		nvarchar(50),
    @ParentNodeID		uniqueidentifier,
    @DocumentTreeNodeID	uniqueidentifier,
    @PreviousVersionID	uniqueidentifier,
	@Name				nvarchar(255),
	@Description		nvarchar(max),
	@Tags				nvarchar(max),
	@CreatorUserID		uniqueidentifier,
	@CreationDate		datetime,
	@ContributorsTemp	GuidFloatTableType readonly,
	@OwnerID			uniqueidentifier,
	@WorkFlowID			uniqueidentifier,
	@AdminAreaID		uniqueidentifier,
	@FormInstanceID		uniqueidentifier,
	@WFDirectorNodeID	uniqueidentifier,
	@WFDirectorUserID	uniqueidentifier
WITH ENCRYPTION, RECOMPILE
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	DECLARE @Contributors GuidFloatTableType
	INSERT INTO @Contributors SELECT * FROM @ContributorsTemp
	
	DECLARE @Searchable bit = CASE WHEN @WorkFlowID IS NULL THEN 1 ELSE 0 END
	
	-- Set searchability 
	SELECT TOP(1) @Searchable = (
		CASE
			WHEN ISNULL(S.IsKnowledge, 0) = 0 OR 
				KT.SearchableAfter = N'Registration' THEN CAST(1 AS bit)
			ELSE CAST(0 AS bit)
		END
	)
	FROM [dbo].[CN_Services] AS S
		LEFT JOIN [dbo].[KW_KnowledgeTypes] AS KT
		ON KT.ApplicationID = @ApplicationID AND KT.KnowledgeTypeID = S.NodeTypeID
	WHERE S.ApplicationID = @ApplicationID AND 
		S.NodeTypeID = @NodeTypeID AND S.IsKnowledge = 1

	DECLARE @_Message varchar(1000)
	DECLARE @_Result int = -1
	
	EXEC [dbo].[CN_P_AddNode] @ApplicationID, @NodeID, @AdditionalID, @NodeTypeID, NULL, 
		@DocumentTreeNodeID, @PreviousVersionID, @Name, @Description, @Tags, @Searchable, 
		@CreatorUserID, @CreationDate, @ParentNodeID, @OwnerID, NULL,
		@_Result output, @_Message output
	
	IF @_Result <= 0 BEGIN
		SELECT @_Result, ISNULL(@_Message, N'NodeCreationFailed')
		ROLLBACK TRANSACTION
		RETURN
	END
	
	UPDATE [dbo].[CN_Nodes]
		SET AdditionalID_Main = @AdditionalID_Main,
			AreaID = @AdminAreaID
	WHERE ApplicationID = @ApplicationID AND NodeID = @NodeID
	
	-- Hide Previous Version
	IF (@Searchable = 1 AND @PreviousVersionID IS NOT NULL) BEGIN
		UPDATE [dbo].[CN_Nodes]
			SET Searchable = 0
		WHERE ApplicationID = @ApplicationID AND NodeID = @PreviousVersionID AND Deleted = 0
	END
	-- end of Hide Previous Version
	
	IF @FormInstanceID IS NOT NULL BEGIN
		UPDATE [dbo].[FG_FormInstances]
			SET OwnerID = @NodeID,
				IsTemporary = 0
		WHERE ApplicationID = @ApplicationID AND InstanceID = @FormInstanceID
	END
	
	SET @_Message = NULL
	
	EXEC [dbo].[CN_P_SetNodeCreators] @ApplicationID, @NodeID, @Contributors, 
		N'Accepted', @CreatorUserID, @CreationDate, @_Result output 
	
	IF @_Result <= 0 BEGIN
		SELECT -1, N'ErrorInAddingNodeCreators'
		ROLLBACK TRANSACTION
		RETURN
	END
	
	IF @DocumentTreeNodeID IS NOT NULL BEGIN
		DECLARE @DocIDs GuidTableType
		
		INSERT INTO @DocIDs (Value) VALUES (@NodeID)
		
		EXEC [dbo].[DCT_P_AddTreeNodeContents] @ApplicationID, @DocumentTreeNodeID, 
			@DocIDs, NULL, @CreatorUserID, @CreationDate, @_Result output
			
		IF @_Result <= 0 BEGIN
			SELECT -1, N'ErrorInAddingTreeNodeContents'
			ROLLBACK TRANSACTION
			RETURN
		END
	END
	
	DECLARE @FormID uniqueidentifier = (
		SELECT TOP(1) FW.FormID
		FROM [dbo].[FG_FormOwners] AS FW
		WHERE FW.ApplicationID = @ApplicationID AND 
			FW.OwnerID = @NodeTypeID AND FW.Deleted = 0 AND @FormInstanceID IS NULL
	)
	
	IF @FormID IS NOT NULL BEGIN
		IF EXISTS(
			SELECT TOP(1) * 
			FROM [dbo].[CN_Extensions] 
			WHERE ApplicationID = @ApplicationID AND 
				OwnerID = @NodeTypeID AND Extension = N'Form' AND Deleted = 0
		) BEGIN	
			DECLARE @InstanceID uniqueidentifier = NEWID()
			
			DECLARE @Instances FormInstanceTableType
			
			INSERT INTO @Instances (InstanceID, FormID, OwnerID, DirectorID, [Admin])
			VALUES (@InstanceID, @FormID, @NodeID, NULL, NULL)
		
			EXEC [dbo].[FG_P_CreateFormInstance] @ApplicationID, @Instances, @CreatorUserID, @CreationDate, @_Result output	
				
			IF @_Result <= 0 BEGIN
				SELECT -1, N'FormInstanceInitializationFailed'
				ROLLBACK TRANSACTION
				RETURN
			END
		END
		ELSE BEGIN
			DECLARE @FormElements StringPairTableType
			
			INSERT INTO @FormElements (FirstValue, SecondValue)
			SELECT Title, N''
			FROM [dbo].[FG_ExtendedFormElements]
			WHERE ApplicationID = @ApplicationID AND FormID = @FormID AND Deleted = 0
			ORDER BY SequenceNumber
			
			EXEC [dbo].[WK_P_CreateWiki] @ApplicationID, @NodeID, @FormElements, 1, 
				@CreatorUserID, @CreationDate, @_Result output 
				
			IF @_Result <= 0 BEGIN
				SELECT -1, N'WikiInitializationFailed'
				ROLLBACK TRANSACTION
				RETURN
			END
		END
	END
	
	IF @WorkFlowID IS NOT NULL BEGIN
		EXEC [dbo].[WF_P_StartNewWorkFlow] @ApplicationID, @NodeID, @WorkFlowID, @WFDirectorNodeID, 
			@WFDirectorUserID, @CreatorUserID, @CreationDate, @_Result output, @_Message output
		
		IF @_Result <= 0 BEGIN
			SELECT -1, ISNULL(@_Message, N'WorkFlowInitializationFailed')
			ROLLBACK TRANSACTION
			RETURN
		END
	END

	SELECT 1
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_SetAdminArea]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_SetAdminArea]
GO

CREATE PROCEDURE [dbo].[CN_SetAdminArea]
	@ApplicationID	uniqueidentifier,
    @NodeID			uniqueidentifier,
    @AreaID			uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[CN_Nodes]
		SET AreaID = @AreaID
	WHERE ApplicationID = @ApplicationID AND NodeID = @NodeID
	
	SELECT @@ROWCOUNT
END

GO