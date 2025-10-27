USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_P_CreateForm]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_P_CreateForm]
GO

CREATE PROCEDURE [dbo].[FG_P_CreateForm]
	@ApplicationID		uniqueidentifier,
    @FormID				uniqueidentifier,
    @TemplateFormID		uniqueidentifier,
	@Title				nvarchar(255),
	@CreatorUserID		uniqueidentifier,
	@CreationDate		datetime,
	@_Result			int output
WITH ENCRYPTION
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	DECLARE @_RetVal int
	SET @_RetVal = 0
	
	SET @Title = [dbo].[GFN_VerifyString](@Title)
	
	IF EXISTS (
		SELECT TOP(1) * 
		FROM [dbo].[FG_ExtendedForms]
		WHERE ApplicationID = @ApplicationID AND Title = @Title AND Deleted = 1
	) BEGIN
			UPDATE [dbo].[FG_ExtendedForms]
			SET TemplateFormID = @TemplateFormID,
				LastModifierUserID = @CreatorUserID,
				LastModificationDate = @CreationDate,
				Deleted = 0
			WHERE ApplicationID = @ApplicationID AND Title = @Title AND Deleted = 1
			
		SET @_RetVal = @@ROWCOUNT
	END
	ELSE IF EXISTS (
		SELECT TOP(1) * 
		FROM [dbo].[FG_ExtendedForms]
		WHERE ApplicationID = @ApplicationID AND Title = @Title AND Deleted = 0
	) BEGIN
			SET @_RetVal = -1
	END	
	ELSE BEGIN
		INSERT INTO [dbo].[FG_ExtendedForms](
			ApplicationID,
			FormID,
			TemplateFormID,
			Title,
			CreatorUserID,
			CreationDate,
			Deleted
		)
		VALUES(
			@ApplicationID,
			@FormID,
			@TemplateFormID,
			@Title,
			@CreatorUserID,
			@CreationDate,
			0
		)
		SET @_RetVal = @@ROWCOUNT
	END
	
	IF (@@ROWCOUNT <= 0 AND @_RetVal = 0) BEGIN
		SET @_Result = @_RetVal
		ROLLBACK TRANSACTION
		RETURN
	END
	
	SET @_Result = @_RetVal
	
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_CreateForm]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_CreateForm]
GO

CREATE PROCEDURE [dbo].[FG_CreateForm]
	@ApplicationID		uniqueidentifier,
    @FormID				uniqueidentifier,
    @TemplateFormID		uniqueidentifier,
	@Title				nvarchar(255),
	@CreatorUserID		uniqueidentifier,
	@CreationDate		datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @_Result int
	
	EXEC [dbo].[FG_P_CreateForm] @ApplicationID, @FormID, @TemplateFormID,
		@Title, @CreatorUserID, @CreationDate, @_Result output
		
	SELECT @_Result
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_SetFormTitle]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_SetFormTitle]
GO

CREATE PROCEDURE [dbo].[FG_SetFormTitle]
	@ApplicationID			uniqueidentifier,
    @FormID					uniqueidentifier,
    @Title					nvarchar(255),
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF EXISTS (
		SELECT TOP(1) * 
		FROM [dbo].[FG_ExtendedForms]
		WHERE ApplicationID = @ApplicationID AND Title = @Title AND FormID <> @FormID AND Deleted = 0
	) BEGIN
		SELECT -1
		RETURN
	END
	
	UPDATE [dbo].[FG_ExtendedForms]
		SET Title = [dbo].[GFN_VerifyString](@Title),
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND FormID = @FormID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_SetFormName]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_SetFormName]
GO

CREATE PROCEDURE [dbo].[FG_SetFormName]
	@ApplicationID	uniqueidentifier,
    @FormID			uniqueidentifier,
    @Name			varchar(100),
	@CurrentUserID	uniqueidentifier,
	@Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF ISNULL(@Name, N'') <> N'' AND EXISTS(
		SELECT TOP(1) *
		FROM [dbo].[FG_ExtendedForms] AS F
		WHERE F.ApplicationID = @ApplicationID AND F.Deleted = 0 AND 
			LOWER(F.Name) = LOWER(@Name) AND F.FormID <> @FormID
	) BEGIN
		SELECT -1, N'NameAlreadyExists'
		RETURN
	END
	
	UPDATE [dbo].[FG_ExtendedForms]
		SET Name = @Name,
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	WHERE ApplicationID = @ApplicationID AND FormID = @FormID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_SetFormDescription]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_SetFormDescription]
GO

CREATE PROCEDURE [dbo].[FG_SetFormDescription]
	@ApplicationID			uniqueidentifier,
    @FormID					uniqueidentifier,
    @Description			nvarchar(2000),
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[FG_ExtendedForms]
		SET [Description] = @Description,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND FormID = @FormID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_ArithmeticDeleteForm]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_ArithmeticDeleteForm]
GO

CREATE PROCEDURE [dbo].[FG_ArithmeticDeleteForm]
	@ApplicationID			uniqueidentifier,
    @FormID					uniqueidentifier,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[FG_ExtendedForms]
		SET LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate,
			Deleted = 1
	WHERE ApplicationID = @ApplicationID AND FormID = @FormID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_RecycleForm]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_RecycleForm]
GO

CREATE PROCEDURE [dbo].[FG_RecycleForm]
	@ApplicationID			uniqueidentifier,
    @FormID					uniqueidentifier,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[FG_ExtendedForms]
		SET LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate,
			Deleted = 0
	WHERE ApplicationID = @ApplicationID AND FormID = @FormID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_P_GetFormsByIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_P_GetFormsByIDs]
GO

CREATE PROCEDURE [dbo].[FG_P_GetFormsByIDs]
	@ApplicationID	uniqueidentifier,
    @FormIDsTemp	GuidTableType readonly
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @FormIDs GuidTableType
	INSERT INTO @FormIDs SELECT * FROM @FormIDsTemp
	
	SELECT EF.FormID,
		   EF.Title,
		   EF.Name,
		   EF.[Description]
	FROM @FormIDs AS ExternalIDs
		INNER JOIN [dbo].[FG_ExtendedForms] AS EF
		ON EF.ApplicationID = @ApplicationID AND EF.FormID = ExternalIDs.Value
	ORDER BY EF.CreationDate DESC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetFormsByIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetFormsByIDs]
GO

CREATE PROCEDURE [dbo].[FG_GetFormsByIDs]
	@ApplicationID	uniqueidentifier,
    @FormIDsTemp	GuidTableType readonly
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @FormIDs GuidTableType

	INSERT INTO @FormIDs ([Value])
	SELECT T.[Value]
	FROM @FormIDsTemp AS T

	EXEC [dbo].[FG_P_GetFormsByIDs] @ApplicationID, @FormIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetForms]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetForms]
GO

CREATE PROCEDURE [dbo].[FG_GetForms]
	@ApplicationID	uniqueidentifier,
	@FormName		varchar(255),
	@SearchText		nvarchar(1000),
	@Count			int,
	@LowerBoundary	int,
	@HasName		bit,
	@Archive		bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SET @Archive = ISNULL(@Archive, 0)
	
	DECLARE @FormIDs GuidTableType

	IF @FormName = '' SET @FormName = NULL
	
	IF ISNULL(@SearchText, N'') = N'' BEGIN
		INSERT INTO @FormIDs (Value)
		SELECT TOP(ISNULL(@Count, 1000000)) X.FormID
		FROM (
				SELECT	ROW_NUMBER() OVER (ORDER BY F.FormID ASC) AS RowNumber,
						F.FormID 
				FROM [dbo].[FG_ExtendedForms] AS F
				WHERE F.ApplicationID = @ApplicationID AND F.Deleted = @Archive AND
					(@FormName IS NULL OR LOWER(ISNULL(F.[Name], '')) = LOWER(@FormName)) AND
					(@HasName IS NULL OR (@HasName = 0 AND ISNULL(F.[Name], N'') = N'') OR 
						(@HasName = 1 AND ISNULL(F.[Name], N'') <> N''))
			) AS X
		WHERE X.RowNumber >= ISNULL(@LowerBoundary, 0)
		ORDER BY X.RowNumber ASC
	END
	ELSE BEGIN
		INSERT INTO @FormIDs (Value)
		SELECT TOP(ISNULL(@Count, 1000000)) X.FormID
		FROM (
				SELECT	ROW_NUMBER() OVER (ORDER BY SRCH.[Rank] DESC, F.FormID ASC) AS RowNumber,
						F.FormID 
				FROM CONTAINSTABLE([dbo].[FG_ExtendedForms], (Title), @SearchText) AS SRCH
					INNER JOIN [dbo].[FG_ExtendedForms] AS F
					ON F.ApplicationID = @ApplicationID AND F.FormID = SRCH.[Key] AND
						F.Deleted = @Archive AND
						(@FormName IS NULL OR LOWER(ISNULL(F.[Name], '')) = LOWER(@FormName)) AND
						(@HasName IS NULL OR (@HasName = 0 AND ISNULL(F.[Name], N'') = N'') OR 
							(@HasName = 1 AND ISNULL(F.[Name], N'') <> N''))
			) AS X
		WHERE X.RowNumber >= ISNULL(@LowerBoundary, 0)
		ORDER BY X.RowNumber ASC
	END
	
	EXEC [dbo].[FG_P_GetFormsByIDs] @ApplicationID, @FormIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_AddFormElement]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_AddFormElement]
GO

CREATE PROCEDURE [dbo].[FG_AddFormElement]
	@ApplicationID		uniqueidentifier,
	@ElementID			uniqueidentifier,
	@TemplateElementID	uniqueidentifier,
	@FormID				uniqueidentifier,
	@Title				nvarchar(2000),
	@Name				varchar(100),
	@Help				nvarchar(2000),
	@SequenceNumber		int,
	@Type				varchar(20),
	@Info				nvarchar(max),
	@CreatorUserID		uniqueidentifier,
	@CreationDate		datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF ISNULL(@Name, N'') <> N'' AND EXISTS(
		SELECT TOP(1) *
		FROM [dbo].[FG_ExtendedFormElements] AS F
		WHERE F.ApplicationID = @ApplicationID AND F.FormID = @FormID AND 
			F.Deleted = 0 AND LOWER(F.Name) = LOWER(@Name)
	) BEGIN
		SELECT -1, N'NameAlreadyExists'
		RETURN
	END
	
	INSERT INTO [dbo].[FG_ExtendedFormElements](
		ApplicationID,
		ElementID,
		TemplateElementID,
		FormID,
		Title,
		Name,
		Help,
		SequenceNumber,
		[Type],
		Info,
		CreatorUserID,
		CreationDate,
		Deleted
	)
	VALUES(
		@ApplicationID,
		@ElementID,
		@TemplateElementID,
		@FormID,
		[dbo].[GFN_VerifyString](@Title),
		@Name,
		[dbo].[GFN_VerifyString](@Help),
		@SequenceNumber,
		@Type,
		@Info,
		@CreatorUserID,
		@CreationDate,
		0
	)
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_ModifyFormElement]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_ModifyFormElement]
GO

CREATE PROCEDURE [dbo].[FG_ModifyFormElement]
	@ApplicationID			uniqueidentifier,
	@ElementID				uniqueidentifier,
	@Title					nvarchar(2000),
	@Name					varchar(100),
	@Help					nvarchar(2000),
	@Info					nvarchar(max),
	@Weight					float,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @FormID uniqueidentifier = (
		SELECT TOP(1) FormID
		FROM [dbo].[FG_ExtendedFormElements] AS E
		WHERE E.ApplicationID = @ApplicationID AND E.ElementID = @ElementID
	)
	
	IF ISNULL(@Name, N'') <> N'' AND EXISTS(
		SELECT TOP(1) *
		FROM [dbo].[FG_ExtendedFormElements] AS F
		WHERE F.ApplicationID = @ApplicationID AND F.FormID = @FormID AND F.Deleted = 0 AND 
			LOWER(F.Name) = LOWER(@Name) AND F.ElementID <> @ElementID
	) BEGIN
		SELECT -1, N'NameAlreadyExists'
		RETURN
	END
	
	UPDATE [dbo].[FG_ExtendedFormElements]
		SET Title = [dbo].[GFN_VerifyString](@Title),
			Name = @Name,
			Help = [dbo].[GFN_VerifyString](@Help),
			Info = @Info,
			[Weight] = @Weight,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND ElementID = @ElementID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_SetElementsOrder]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_SetElementsOrder]
GO

CREATE PROCEDURE [dbo].[FG_SetElementsOrder]
	@ApplicationID	uniqueidentifier,
	@strElementIDs	varchar(max),
	@delimiter		char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @ElementIDs TABLE (SequenceNo int identity(1, 1) primary key, ElementID uniqueidentifier)
	
	INSERT INTO @ElementIDs (ElementID)
	SELECT Ref.Value
	FROM [dbo].[GFN_StrToGuidTable](@strElementIDs, @delimiter) AS Ref
	
	DECLARE @FormID uniqueidentifier
	
	SELECT @FormID = FormID
	FROM [dbo].[FG_ExtendedFormElements]
	WHERE ApplicationID = @ApplicationID AND 
		ElementID = (SELECT TOP (1) Ref.ElementID FROM @ElementIDs AS Ref)
	
	IF @FormID IS NULL BEGIN
		SELECT -1
		RETURN
	END
	
	INSERT INTO @ElementIDs (ElementID)
	SELECT E.ElementID
	FROM @ElementIDs AS Ref
		RIGHT JOIN [dbo].[FG_ExtendedFormElements] AS E
		ON E.ElementID = Ref.ElementID
	WHERE E.ApplicationID = @ApplicationID AND E.FormID = @FormID AND Ref.ElementID IS NULL
	ORDER BY E.SequenceNumber
	
	UPDATE [dbo].[FG_ExtendedFormElements]
		SET SequenceNumber = Ref.SequenceNo
	FROM @ElementIDs AS Ref
		INNER JOIN [dbo].[FG_ExtendedFormElements] AS E
		ON E.ElementID = Ref.ElementID
	WHERE E.ApplicationID = @ApplicationID AND E.FormID = @FormID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_SetFormElementNecessity]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_SetFormElementNecessity]
GO

CREATE PROCEDURE [dbo].[FG_SetFormElementNecessity]
	@ApplicationID	uniqueidentifier,
	@ElementID		uniqueidentifier,
	@Necessity		bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[FG_ExtendedFormElements]
		SET Necessary = @Necessity
	WHERE ApplicationID = @ApplicationID AND ElementID = @ElementID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_SetFormElementUniqueness]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_SetFormElementUniqueness]
GO

CREATE PROCEDURE [dbo].[FG_SetFormElementUniqueness]
	@ApplicationID	uniqueidentifier,
	@ElementID		uniqueidentifier,
	@Value			bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[FG_ExtendedFormElements]
		SET UniqueValue = @Value
	WHERE ApplicationID = @ApplicationID AND ElementID = @ElementID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_ArithmeticDeleteFormElement]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_ArithmeticDeleteFormElement]
GO

CREATE PROCEDURE [dbo].[FG_ArithmeticDeleteFormElement]
	@ApplicationID			uniqueidentifier,
	@ElementID				uniqueidentifier,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[FG_ExtendedFormElements]
		SET LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate,
			Deleted = 1
	WHERE ApplicationID = @ApplicationID AND ElementID = @ElementID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_SaveFormElements]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_SaveFormElements]
GO

CREATE PROCEDURE [dbo].[FG_SaveFormElements]
	@ApplicationID	uniqueidentifier,
	@FormID			uniqueidentifier,
	@Title			nvarchar(255),
	@Name			varchar(100),
	@Description	nvarchar(2000),
	@ElementsTemp	FormElementTableType readonly,
	@CurrentUserID	uniqueidentifier,
	@Now			datetime
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @Elements FormElementTableType
	INSERT INTO @Elements SELECT * FROM @ElementsTemp
	
	-- Update Form
	SET @Title = [dbo].[GFN_VerifyString](LTRIM(RTRIM(ISNULL(@Title, N''))))
	SET @Name = LTRIM(RTRIM(ISNULL(@Name, '')))
	SET @Description = [dbo].[GFN_VerifyString](LTRIM(RTRIM(ISNULL(@Description, N''))))
	
	UPDATE [dbo].[FG_ExtendedForms]
		SET Title = CASE WHEN @Title = N'' THEN Title ELSE @Title END,
			Name = CASE WHEN @Name = N'' THEN Name ELSE @Name END,
			[Description] = CASE WHEN @Description = N'' THEN [Description] ELSE @Description END
	WHERE ApplicationID = @ApplicationID AND FormID = @FormID
	-- end of Update Form
	
	
	-- Update Existing Data
	UPDATE X
		SET	Title = CASE WHEN E.ElementID IS NULL THEN X.Title ELSE E.Title END,
			Info = CASE WHEN E.ElementID IS NULL THEN X.Info ELSE E.Info END,
			SequenceNumber = CASE WHEN E.ElementID IS NULL THEN X.SequenceNumber ELSE E.SequenceNubmer END,
			LastModifierUserID = CASE WHEN E.ElementID IS NULL THEN X.LastModifierUserID ELSE @CurrentUserID END,
			LastModificationDate = CASE WHEN E.ElementID IS NULL THEN X.LastModificationDate ELSE @Now END,
			Deleted = CASE WHEN E.ElementID IS NULL THEN 1 ELSE 0 END,
			Necessary = CASE WHEN E.ElementID IS NULL THEN X.Necessary ELSE E.Necessary END,
			[Weight] = CASE WHEN E.ElementID IS NULL THEN X.[Weight] ELSE E.[Weight] END,
			Name = CASE WHEN E.ElementID IS NULL THEN X.Name ELSE E.Name END,
			UniqueValue = CASE WHEN E.ElementID IS NULL THEN X.UniqueValue ELSE E.UniqueValue END,
			Help = CASE WHEN E.ElementID IS NULL THEN X.Help ELSE E.Help END
	FROM [dbo].[FG_ExtendedFormElements] AS X
		LEFT JOIN @Elements AS E
		ON E.ElementID = X.ElementID
	WHERE X.ApplicationID = @ApplicationID AND X.FormID = @FormID
	-- end of Update Existing Data
	
	-- Insert New Data
	INSERT INTO [dbo].[FG_ExtendedFormElements] (
		ApplicationID,
		ElementID,
		TemplateElementID,
		FormID,
		Title,
		[Type],
		Info,
		SequenceNumber,
		CreatorUserID,
		CreationDate,
		Deleted,
		Necessary,
		[Weight],
		Name,
		UniqueValue,
		Help
	)
	SELECT	@ApplicationID,
			E.ElementID,
			E.TemplateElementID,
			@FormID,
			E.Title,
			E.[Type],
			E.Info,
			E.SequenceNubmer,
			@CurrentUserID,
			@Now,
			0,
			E.Necessary,
			E.[Weight],
			E.Name,
			E.UniqueValue,
			E.Help
	FROM @Elements AS E
		LEFT JOIN [dbo].[FG_ExtendedFormElements] AS X
		ON X.ApplicationID = @ApplicationID AND X.ElementID = E.ElementID
	WHERE X.ElementID IS NULL
	-- end of Insert New Data
	
	SELECT 1
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_P_GetFormElements]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_P_GetFormElements]
GO

CREATE PROCEDURE [dbo].[FG_P_GetFormElements]
	@ApplicationID	uniqueidentifier,
	@ElementIDsTemp	GuidTableType readonly
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @ElementIDs GuidTableType
	INSERT INTO @ElementIDs SELECT * FROM @ElementIDsTemp
	
	SELECT FE.ElementID,
		   FE.FormID,
		   FE.Title,
		   FE.Name,
		   FE.Help,
		   ISNULL(FE.Necessary, CAST(0 AS bit)) AS Necessary,
		   ISNULL(FE.UniqueValue, CAST(0 AS bit)) AS UniqueValue,
		   FE.SequenceNumber,
		   FE.[Type],
		   FE.Info,
		   FE.[Weight]
	FROM @ElementIDs AS Ref
		INNER JOIN [dbo].[FG_ExtendedFormElements] AS FE
		ON FE.ApplicationID = @ApplicationID AND FE.ElementID = Ref.Value
	ORDER BY FE.SequenceNumber ASC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetFormElementsByIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetFormElementsByIDs]
GO

CREATE PROCEDURE [dbo].[FG_GetFormElementsByIDs]
	@ApplicationID	uniqueidentifier,
	@strElementIDs	varchar(max),
	@delimiter		char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @ElementIDs GuidTableType
	
	INSERT INTO @ElementIDs
	SELECT Ref.Value
	FROM [dbo].[GFN_StrToGuidTable](@strElementIDs, @delimiter) AS Ref
	
	EXEC [dbo].[FG_P_GetFormElements] @ApplicationID, @ElementIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetFormElements]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetFormElements]
GO

CREATE PROCEDURE [dbo].[FG_GetFormElements]	
	@ApplicationID	uniqueidentifier,
	@FormID			uniqueidentifier,
	@OwnerID		uniqueidentifier,
	@Type			varchar(50)
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF @FormID IS NULL AND @OwnerID IS NOT NULL BEGIN
		SELECT TOP(1) @FormID = FormID
		FROM [dbo].[FG_FormOwners] 
		WHERE ApplicationID = @ApplicationID AND 
			OwnerID = @OwnerID AND Deleted = 0
	END
	
	DECLARE @ElementIDs GuidTableType
	
	INSERT INTO @ElementIDs
	SELECT ElementID
	FROM [dbo].[FG_ExtendedFormElements]
	WHERE ApplicationID = @ApplicationID AND FormID = @FormID AND 
		(@Type IS NULL OR [Type] = @Type) AND Deleted = 0
	
	EXEC [dbo].[FG_P_GetFormElements] @ApplicationID, @ElementIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetFormElementIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetFormElementIDs]
GO

CREATE PROCEDURE [dbo].[FG_GetFormElementIDs]	
	@ApplicationID		uniqueidentifier,
	@FormID				uniqueidentifier,
	@strElementNames	nvarchar(max),
	@delimiter			char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @Names StringTableType
	
	INSERT INTO @Names (Value)
	SELECT DISTINCT LOWER(Ref.Value)
	FROM [dbo].[GFN_StrToStringTable](@strElementNames, @delimiter) AS Ref
	WHERE ISNULL(Ref.Value, N'') <> N''
	
	SELECT E.Name, E.ElementID
	FROM @Names AS N
		INNER JOIN [dbo].[FG_ExtendedFormElements] AS E
		ON E.ApplicationID = @ApplicationID AND E.FormID = @FormID AND 
			LOWER(ISNULL(E.Name, N'')) = N.Value
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_IsFormElement]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_IsFormElement]
GO

CREATE PROCEDURE [dbo].[FG_IsFormElement]
	@ApplicationID	uniqueidentifier,
	@strIDs			varchar(max),
	@delimiter		char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT Ref.Value AS ID
	FROM [dbo].[GFN_StrToGuidTable](@strIDs, @delimiter) AS Ref
		INNER JOIN [dbo].[FG_ExtendedFormElements] AS E
		ON E.ApplicationID = @ApplicationID AND E.ElementID = Ref.Value
		
	UNION
	
	SELECT Ref.Value AS ID
	FROM [dbo].[GFN_StrToGuidTable](@strIDs, @delimiter) AS Ref
		INNER JOIN [dbo].[FG_InstanceElements] AS E
		ON E.ApplicationID = @ApplicationID AND E.ElementID = Ref.Value
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_P_CreateFormInstance]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_P_CreateFormInstance]
GO

CREATE PROCEDURE [dbo].[FG_P_CreateFormInstance]
	@ApplicationID	uniqueidentifier,
	@InstancesTemp	FormInstanceTableType readonly,
	@CreatorUserID	uniqueidentifier,
	@CreationDate	datetime,
	@_Result		int output
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @Instances FormInstanceTableType
	INSERT INTO @Instances SELECT * FROM @InstancesTemp
	
	INSERT INTO [dbo].[FG_FormInstances](
		ApplicationID,
		InstanceID,
		FormID,
		OwnerID,
		DirectorID,
		[Admin],
		Filled,
		IsTemporary,
		CreatorUserID,
		CreationDate,
		Deleted
	)
	SELECT	@ApplicationID, 
			I.InstanceID, 
			I.FormID, 
			I.OwnerID, 
			I.DirectorID, 
			ISNULL(I.[Admin], 0), 
			0,
			I.IsTemporary,
			@CreatorUserID, 
			@CreationDate, 
			0
	FROM @Instances AS I
	
	SET @_Result = @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_CreateFormInstance]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_CreateFormInstance]
GO

CREATE PROCEDURE [dbo].[FG_CreateFormInstance]
	@ApplicationID	uniqueidentifier,
	@InstancesTemp	FormInstanceTableType readonly,
	@CreatorUserID	uniqueidentifier,
	@CreationDate	datetime
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @Instances FormInstanceTableType
	INSERT INTO @Instances SELECT * FROM @InstancesTemp
	
	DECLARE @_Result int
	
	EXEC [dbo].[FG_P_CreateFormInstance] @ApplicationID, @Instances, @CreatorUserID, @CreationDate, @_Result output
		
	SELECT @_Result
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_P_CopyFormInstances]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_P_CopyFormInstances]
GO

CREATE PROCEDURE [dbo].[FG_P_CopyFormInstances]
	@ApplicationID	uniqueidentifier,
	@OldOwnerID		uniqueidentifier,
	@NewOwnerID		uniqueidentifier,
	@NewFormID		uniqueidentifier,
	@CreatorUserID	uniqueidentifier,
	@CreationDate	datetime,
	@_Result		int output
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF EXISTS(
		SELECT TOP(1) * 
		FROM [dbo].[FG_FormInstances]
		WHERE ApplicationID = @ApplicationID AND OwnerID = @OldOwnerID AND Deleted = 0
	) BEGIN
		INSERT INTO [dbo].[FG_FormInstances](
			ApplicationID,
			InstanceID,
			FormID,
			OwnerID,
			DirectorID,
			Filled,
			CreatorUserID,
			CreationDate,
			Deleted
		)
		SELECT	@ApplicationID,
				NEWID(),
				ISNULL(@NewFormID, FormID),
				@NewOwnerID,
				DirectorID,
				0,
				@CreatorUserID,
				@CreationDate,
				0
		FROM [dbo].[FG_FormInstances]
		WHERE ApplicationID = @ApplicationID AND OwnerID = @OldOwnerID AND Deleted = 0
		
		SET @_Result = @@ROWCOUNT	
	END
	ELSE
		SET @_Result = 1
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_P_RemoveFormInstances]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_P_RemoveFormInstances]
GO

CREATE PROCEDURE [dbo].[FG_P_RemoveFormInstances]
	@ApplicationID		uniqueidentifier,
	@InstanceIDsTemp	GuidTableType readonly,
	@CurrentUserID		uniqueidentifier,
	@Now				datetime,
	@_Result			int output
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @InstanceIDs GuidTableType
	INSERT INTO @InstanceIDs SELECT * FROM @InstanceIDsTemp
	
	UPDATE FI
		SET Deleted = 1,
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	FROM @InstanceIDs AS ExternalIDs
		INNER JOIN [dbo].[FG_FormInstances] AS FI
		ON FI.[InstanceID] = ExternalIDs.Value
	WHERE FI.ApplicationID = @ApplicationID AND FI.[Deleted] = 0
	
	SET @_Result = @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_RemoveFormInstances]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_RemoveFormInstances]
GO

CREATE PROCEDURE [dbo].[FG_RemoveFormInstances]
	@ApplicationID	uniqueidentifier,
	@strInstanceIDs	varchar(max),
	@delimiter		char,
	@CurrentUserID	uniqueidentifier,
	@Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @InstanceIDs GuidTableType
	INSERT INTO @InstanceIDs
	SELECT DISTINCT Ref.Value FROM [dbo].[GFN_StrToGuidTable](@strInstanceIDs, @delimiter) AS Ref
	
	DECLARE @_Result int = 0
	
	EXEC [dbo].[FG_P_RemoveFormInstances] @ApplicationID, @InstanceIDs, @CurrentUserID, @Now, @_Result output 
	
	SELECT @_Result
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_RemoveOwnerFormInstances]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_RemoveOwnerFormInstances]
GO

CREATE PROCEDURE [dbo].[FG_RemoveOwnerFormInstances]
	@ApplicationID	uniqueidentifier,
	@OwnerID		uniqueidentifier,
	@FormID			uniqueidentifier,
	@CurrentUserID	uniqueidentifier,
	@Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @InstanceIDs GuidTableType
	
	INSERT INTO @InstanceIDs (Value)
	SELECT I.InstanceID
	FROM [dbo].[FG_FormInstances] AS I
	WHERE I.ApplicationID = @ApplicationID AND I.FormID = @FormID AND 
		I.OwnerID = @OwnerID AND I.Deleted = 0
	
	DECLARE @_Result int = 0
	
	EXEC [dbo].[FG_P_RemoveFormInstances] @ApplicationID, @InstanceIDs, @CurrentUserID, @Now, @_Result output 
	
	SELECT @_Result
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_P_GetFormInstancesByIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_P_GetFormInstancesByIDs]
GO

CREATE PROCEDURE [dbo].[FG_P_GetFormInstancesByIDs]
	@ApplicationID		uniqueidentifier,
	@InstanceIDsTemp	GuidTableType readonly
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @InstanceIDs GuidTableType
	INSERT INTO @InstanceIDs SELECT * FROM @InstanceIDsTemp
	
	SELECT FI.InstanceID AS InstanceID,
		   FI.FormID AS FormID,
		   FI.OwnerID AS OwnerID,
		   FI.DirectorID AS DirectorID,
		   FI.Filled AS Filled,
		   FI.FillingDate AS FillingDate,
		   EF.Title AS FormTitle,
		   EF.[Description] AS [Description],
		   FI.CreatorUserID AS CreatorUserID,
		   UN.UserName AS CreatorUserName,
		   UN.FirstName AS CreatorFirstName,
		   UN.LastName AS CreatorLastName
	FROM @InstanceIDs AS ExternalIDs
		INNER JOIN [dbo].[FG_FormInstances] AS FI
		ON FI.ApplicationID = @ApplicationID AND FI.InstanceID = ExternalIDs.Value
		INNER JOIN [dbo].[FG_ExtendedForms] AS EF
		ON EF.ApplicationID = @ApplicationID AND EF.FormID = FI.FormID
		LEFT JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND UN.UserID = FI.CreatorUserID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_P_GetOwnerFormInstances]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_P_GetOwnerFormInstances]
GO

CREATE PROCEDURE [dbo].[FG_P_GetOwnerFormInstances]
	@ApplicationID	uniqueidentifier,
	@OwnerIDsTemp	GuidTableType readonly,
	@FormID			uniqueidentifier,
	@IsTemporary	bit,
	@CreatorUserID	uniqueidentifier
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @OwnerIDs GuidTableType
	INSERT INTO @OwnerIDs SELECT * FROM @OwnerIDsTemp
	
	DECLARE @InstanceIDs GuidTableType
	
	INSERT INTO @InstanceIDs
	SELECT X.InstanceID
	FROM [dbo].[FG_FN_GetOwnerFormInstanceIDs](@ApplicationID, @OwnerIDs, @FormID, @IsTemporary, @CreatorUserID) AS X
	
	EXEC [dbo].[FG_P_GetFormInstancesByIDs] @ApplicationID, @InstanceIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetOwnerFormInstances]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetOwnerFormInstances]
GO

CREATE PROCEDURE [dbo].[FG_GetOwnerFormInstances]
	@ApplicationID	uniqueidentifier,
	@strOwnerIDs	varchar(max),
	@delimiter		char,
	@FormID			uniqueidentifier,
	@IsTemporary	bit,
	@CreatorUserID	uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @OwnerIDs GuidTableType
	INSERT INTO @OwnerIDs
	SELECT DISTINCT Ref.Value FROM [dbo].[GFN_StrToGuidTable](@strOwnerIDs, @delimiter) AS Ref
	
	EXEC [dbo].[FG_P_GetOwnerFormInstances] @ApplicationID, @OwnerIDs, @FormID, @IsTemporary, @CreatorUserID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetFormInstanceOwnerID]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetFormInstanceOwnerID]
GO

CREATE PROCEDURE [dbo].[FG_GetFormInstanceOwnerID]
	@ApplicationID			uniqueidentifier,
	@InstanceIDOrElementID	uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @OwnerID uniqueidentifier
	
	SELECT TOP(1) @OwnerID = FI.OwnerID
	FROM [dbo].[FG_FormInstances] AS FI
	WHERE FI.ApplicationID = @ApplicationID AND 
		FI.InstanceID = @InstanceIDOrElementID
	
	IF @OwnerID IS NULL BEGIN
		SELECT TOP(1) @OwnerID = FI.OwnerID
		FROM [dbo].[FG_InstanceElements] AS IE
			INNER JOIN [dbo].[FG_FormInstances] AS FI
			ON FI.ApplicationID = @ApplicationID AND FI.InstanceID = IE.InstanceID
		WHERE IE.ApplicationID = @ApplicationID AND 
			IE.ElementID = @InstanceIDOrElementID
	END
	
	SELECT @OwnerID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetFormInstanceHierarchyOwnerID]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetFormInstanceHierarchyOwnerID]
GO

CREATE PROCEDURE [dbo].[FG_GetFormInstanceHierarchyOwnerID]
	@ApplicationID	uniqueidentifier,
	@InstanceID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	;WITH hierarchy (OwnerID, [Level])
	AS
	(
		SELECT I.OwnerID, 0 AS [Level]
		FROM [dbo].[FG_FormInstances] AS I
		WHERE I.ApplicationID = @ApplicationID AND I.InstanceID = @InstanceID
		
		UNION ALL
		
		SELECT I.OwnerID, [Level] + 1
		FROM hierarchy AS HR
			INNER JOIN [dbo].[FG_InstanceElements] AS E
			ON E.ApplicationID = @ApplicationID AND E.ElementID = HR.OwnerID
			INNER JOIN [dbo].[FG_FormInstances] AS I
			ON I.ApplicationID = @ApplicationID AND I.InstanceID = E.InstanceID
		WHERE HR.OwnerID IS NOT NULL AND I.OwnerID <> HR.OwnerID
	)

	SELECT TOP(1) HR.OwnerID AS ID
	FROM hierarchy AS HR
		INNER JOIN (
			SELECT TOP(1) MAX([Level]) AS [Level]
			FROM hierarchy
		) AS A
		ON A.[Level] = HR.[Level]
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_ValidateNewName]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_ValidateNewName]
GO

CREATE PROCEDURE [dbo].[FG_ValidateNewName]
	@ApplicationID	uniqueidentifier,
	@ObjectID		uniqueidentifier,
	@FormID			uniqueidentifier,
	@Name			varchar(100)
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT [dbo].[FG_FN_ValidateNewName](@ApplicationID, @ObjectID, @FormID, @Name) AS Value
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_MeetsUniqueConstraint]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_MeetsUniqueConstraint]
GO

CREATE PROCEDURE [dbo].[FG_MeetsUniqueConstraint]
	@ApplicationID	uniqueidentifier,
	@InstanceID		uniqueidentifier,
	@ElementID		uniqueidentifier,
	@TextValue		nvarchar(max),
	@FloatValue		float
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @RefElementID uniqueidentifier = (
		SELECT TOP(1) E.RefElementID
		FROM [dbo].[FG_InstanceElements] AS E
		WHERE E.ApplicationID = @ApplicationID AND E.ElementID = @ElementID
	)
	
	DECLARE @Elements FormElementTableType
	
	INSERT INTO @Elements (ElementID, InstanceID, RefElementID, TextValue, FloatValue, SequenceNubmer, [Type])
	SELECT @ElementID, @InstanceID, @RefElementID, @TextValue, @FloatValue, 0, N''
	
	SELECT TOP(1) 1
	WHERE @InstanceID IS NOT NULL AND ((ISNULL(@TextValue, N'') = N'' AND @FloatValue IS NULL) OR NOT EXISTS (
			SELECT TOP(1) X.ElementID
			FROM [dbo].[FG_FN_CheckUniqueConstraint](@ApplicationID, @Elements) AS X
		))
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_SaveFormInstanceElements]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_SaveFormInstanceElements]
GO

CREATE PROCEDURE [dbo].[FG_SaveFormInstanceElements]
	@ApplicationID			uniqueidentifier,
	@ElementsTemp			FormElementTableType readonly,
	@GuidItemsTemp			GuidPairTableType readonly,
	@ElementsToClearTemp	GuidTableType readonly,
	@FilesTemp				DocFileInfoTableType readonly,
	@CreatorUserID			uniqueidentifier,
	@CreationDate			datetime
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @Elements FormElementTableType
	INSERT INTO @Elements SELECT * FROM @ElementsTemp
	
	DECLARE @GuidItems GuidPairTableType
	INSERT INTO @GuidItems SELECT * FROM @GuidItemsTemp
	
	DECLARE @ElementsToClear GuidTableType
	INSERT INTO @ElementsToClear SELECT * FROM @ElementsToClearTemp
	
	DECLARE @Files DocFileInfoTableType
	INSERT INTO @Files SELECT * FROM @FilesTemp
	
	IF EXISTS(
		SELECT TOP(1) *
		FROM [dbo].[FG_FN_CheckUniqueConstraint](@ApplicationID, @Elements) AS Ref
	) BEGIN
		SELECT -1, N'UniqueConstriantHasNotBeenMet'
		RETURN
	END
	
	-- Find Main ElementIDs
	DECLARE @MainElementIDs TABLE (ElementID uniqueidentifier, MainElementID uniqueidentifier)
	
	INSERT INTO @MainElementIDs (ElementID, MainElementID)
	SELECT DISTINCT Ref.ElementID, IE1.ElementID
	FROM @Elements AS Ref
		LEFT JOIN [dbo].[FG_InstanceElements] AS IE
		ON IE.ApplicationID = @ApplicationID AND IE.[ElementID] = Ref.ElementID
		INNER JOIN [dbo].[FG_InstanceElements] AS IE1
		ON IE1.ApplicationID = @ApplicationID AND
			Ref.RefElementID IS NOT NULL AND Ref.InstanceID IS NOT NULL AND
			IE1.RefElementID = Ref.RefElementID AND IE1.InstanceID = Ref.InstanceID
	WHERE IE.ElementID IS NULL
	
	/*
	UPDATE E
		SET ElementID = M.MainElementID
	FROM @Elements AS E
		INNER JOIN @MainElementIDs AS M
		ON M.ElementID = E.ElementID
	*/
		
	UPDATE E
		SET FirstValue = M.MainElementID
	FROM @GuidItems AS E
		INNER JOIN @MainElementIDs AS M
		ON M.ElementID = E.FirstValue
	-- end of Find Main ElementIDs
	
	
	-- Save Changes
	INSERT INTO [dbo].[FG_Changes] (ApplicationID, ElementID, TextValue, 
		FloatValue, BitValue, DateValue, CreatorUserID, CreationDate, Deleted)
	SELECT @ApplicationID, ISNULL(E.ElementID, X.ElementID), [dbo].[GFN_VerifyString](X.TextValue), 
		X.FloatValue, X.BitValue, X.DateValue, @CreatorUserID, @CreationDate, 0
	FROM (
			SELECT	C.Value AS ElementID, 
					I.RefElementID AS RefElementID,
					I.InstanceID,
					CAST(NULL AS varchar(max)) AS TextValue,
					CAST(NULL AS float) AS FloatValue,
					CAST(NULL AS bit) AS BitValue,
					CAST(NULL AS datetime) AS DateValue
			FROM @ElementsToClear AS C
				LEFT JOIN @Elements AS E
				ON E.ElementID = C.Value
				INNER JOIN [dbo].[FG_InstanceElements] AS I
				ON I.ApplicationID = @ApplicationID AND I.ElementID = C.Value
			WHERE E.ElementID IS NULL
			
			UNION ALL
			
			SELECT E.ElementID, E.RefElementID, E.InstanceID,
				E.TextValue, E.FloatValue, E.BitValue, E.DateValue
			FROM @Elements AS E
		) AS X
		LEFT JOIN [dbo].[FG_InstanceElements] AS E -- First part checks ElementID. We split them for performance reasons
		ON E.ApplicationID = @ApplicationID AND E.ElementID = X.ElementID
		LEFT JOIN [dbo].[FG_InstanceElements] AS E1 -- Second part checks InstanceID and RefElementID
		ON E1.ApplicationID = @ApplicationID AND
			X.RefElementID IS NOT NULL AND X.InstanceID IS NOT NULL AND 
			E1.RefElementID = X.RefElementID AND E1.InstanceID = X.InstanceID
	WHERE (ISNULL(E.ElementID, E1.ElementID) IS NULL AND 
			NOT (X.TextValue IS NULL AND X.FloatValue IS NULL AND
				X.BitValue IS NULL AND X.DateValue IS NULL
			)
		) OR 
		(X.TextValue IS NULL AND ISNULL(E.TextValue, E1.TextValue) IS NOT NULL) OR
		(X.TextValue IS NOT NULL AND ISNULL(E.TextValue, E1.TextValue) IS NULL) OR
		(X.TextValue IS NOT NULL AND ISNULL(E.TextValue, E1.TextValue) IS NOT NULL AND 
			X.TextValue <> ISNULL(E.TextValue, E1.TextValue)) OR
		(X.FloatValue IS NULL AND ISNULL(E.FloatValue, E1.FloatValue) IS NOT NULL) OR
		(X.FloatValue IS NOT NULL AND ISNULL(E.FloatValue, E1.FloatValue) IS NULL) OR
		(X.FloatValue IS NOT NULL AND ISNULL(E.FloatValue, E1.FloatValue) IS NOT NULL AND 
			X.FloatValue <> ISNULL(E.FloatValue, E1.FloatValue)) OR
		(X.BitValue IS NULL AND ISNULL(E.BitValue, E1.BitValue) IS NOT NULL) OR
		(X.BitValue IS NOT NULL AND ISNULL(E.BitValue, E1.BitValue) IS NULL) OR
		(X.BitValue IS NOT NULL AND ISNULL(E.BitValue, E1.BitValue) IS NOT NULL AND 
			X.BitValue <> ISNULL(E.BitValue, E1.BitValue)) OR
		(X.DateValue IS NULL AND ISNULL(E.DateValue, E1.DateValue) IS NOT NULL) OR
		(X.DateValue IS NOT NULL AND ISNULL(E.DateValue, E1.DateValue) IS NULL) OR
		(X.DateValue IS NOT NULL AND ISNULL(E.DateValue, E1.DateValue) IS NOT NULL AND 
			X.DateValue <> ISNULL(E.DateValue, E1.DateValue))
	-- end of Save Changes

	-- Update Existing Data
	-- A: Update based on ElementID. We split them for performance reasons
	UPDATE IE
		SET TextValue = [dbo].[GFN_VerifyString](Ref.TextValue),
			FloatValue = Ref.FloatValue,
			BitValue = Ref.BitValue,
			DateValue = Ref.DateValue,
			LastModifierUserID = @CreatorUserID,
			LastModificationDate = @CreationDate,
			Deleted = 0
	FROM @Elements AS Ref
		INNER JOIN [dbo].[FG_InstanceElements] AS IE
		ON IE.ApplicationID = @ApplicationID AND IE.[ElementID] = Ref.ElementID
	
	-- B: Update based on RefElementID and InstanceID
	UPDATE IE
		SET TextValue = [dbo].[GFN_VerifyString](Ref.TextValue),
			FloatValue = Ref.FloatValue,
			BitValue = Ref.BitValue,
			DateValue = Ref.DateValue,
			LastModifierUserID = @CreatorUserID,
			LastModificationDate = @CreationDate,
			Deleted = 0
	FROM @Elements AS Ref
		INNER JOIN [dbo].[FG_InstanceElements] AS IE
		ON IE.ApplicationID = @ApplicationID AND
			IE.RefElementID = Ref.RefElementID AND IE.InstanceID = Ref.InstanceID
	-- end of Update Existing Data
	
	DECLARE @Count int = @@ROWCOUNT
	
	-- Clear Empty Elements
	UPDATE IE
		SET TextValue = NULL,
			FloatValue = NULL,
			BitValue = NULL,
			DateValue = NULL,
			LastModifierUserID = @CreatorUserID,
			LastModificationDate = @CreationDate,
			Deleted = 0
	FROM @ElementsToClear AS Ref
		INNER JOIN [dbo].[FG_InstanceElements] AS IE
		ON IE.ApplicationID = @ApplicationID AND IE.[ElementID] = Ref.Value
		
	SET @Count = @@ROWCOUNT + @Count
	
	DECLARE @FileOwnerIDs GuidTableType
	DECLARE @_Result int = 0
	
	INSERT INTO @FileOwnerIDs (Value)
	SELECT DISTINCT OwnerIDs.Value
	FROM (
			SELECT C.Value
			FROM @ElementsToClear AS C
			
			UNION ALL 
			
			SELECT F.OwnerID
			FROM @Files AS F
		) AS OwnerIDs
		
	EXEC [dbo].[DCT_P_RemoveOwnersFiles] @ApplicationID, @FileOwnerIDs, @_Result output
	-- end of Clear Empty Elements
	
	INSERT INTO [dbo].[FG_InstanceElements](
		ApplicationID,
		ElementID,
		InstanceID,
		RefElementID,
		Title,
		SequenceNumber,
		[Type],
		Info,
		TextValue,
		FloatValue,
		BitValue,
		DateValue,
		CreatorUserID,
		CreationDate,
		Deleted
	)
	SELECT @ApplicationID,
		   Ref.ElementID,
		   Ref.InstanceID,
		   Ref.RefElementID,
		   EFE.Title,
		   ISNULL(EFE.SequenceNumber, Ref.SequenceNubmer),
		   EFE.[Type],
		   EFE.Info,
		   [dbo].[GFN_VerifyString](Ref.TextValue),
		   Ref.FloatValue,
		   Ref.BitValue,
		   Ref.DateValue,
		   @CreatorUserID,
		   @CreationDate,
		   0
	FROM @Elements AS Ref
		INNER JOIN [dbo].[FG_ExtendedFormElements] AS EFE
		ON EFE.ApplicationID = @ApplicationID AND EFE.ElementID = Ref.RefElementID
		LEFT JOIN [dbo].[FG_InstanceElements] AS IE
		ON IE.ApplicationID = @ApplicationID AND IE.[ElementID] = Ref.ElementID
		LEFT JOIN [dbo].[FG_InstanceElements] AS IE1
		ON IE1.ApplicationID = @ApplicationID AND
			IE1.RefElementID = Ref.RefElementID AND IE1.InstanceID = Ref.InstanceID
	WHERE ISNULL(IE.ElementID, IE1.ElementID) IS NULL
	
	SET @Count = @@ROWCOUNT + @Count + 1
	
	EXEC [dbo].[DCT_P_AddFiles] @ApplicationID, NULL, NULL, @Files, @CreatorUserID, @CreationDate, @_Result output
	
	-- Set Selected Guids
	UPDATE S
		SET Deleted = 1,
			LastModifierUserID = @CreatorUserID,
			LastModificationDate = @CreationDate
	FROM (
			SELECT DISTINCT A.ElementID
			FROM @Elements AS A
			
			UNION
			
			SELECT DISTINCT C.Value
			FROM @ElementsToClear AS C
		) AS E
		INNER JOIN [dbo].[FG_SelectedItems] AS S
		ON S.ApplicationID = @ApplicationID AND S.ElementID = E.ElementID
	
	UPDATE S
		SET Deleted = 0,
			LastModifierUserID = @CreatorUserID,
			LastModificationDate = @CreationDate
	FROM @GuidItems AS G
		INNER JOIN [dbo].[FG_SelectedItems] AS S
		ON S.ApplicationID = @ApplicationID AND 
			S.ElementID = G.FirstValue AND S.SelectedID = G.SecondValue
	
	INSERT INTO [dbo].[FG_SelectedItems] (ApplicationID, ElementID, 
		SelectedID, LastModifierUserID, LastModificationDate, Deleted)
	SELECT @ApplicationID, G.FirstValue, G.SecondValue, @CreatorUserID, @CreationDate, 0
	FROM @GuidItems AS G
		INNER JOIN [dbo].[FG_InstanceElements] AS E
		ON E.ApplicationID = @ApplicationID AND E.ElementID = G.FirstValue
		LEFT JOIN [dbo].[FG_SelectedItems] AS S
		ON S.ApplicationID = @ApplicationID AND 
			S.ElementID = G.FirstValue AND S.SelectedID = G.SecondValue
	WHERE S.ElementID IS NULL
	-- end of Set Selected Guids
	
	SELECT @Count
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetFormInstances]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetFormInstances]
GO

CREATE PROCEDURE [dbo].[FG_GetFormInstances]
	@ApplicationID		uniqueidentifier,
	@strInstanceIDs		varchar(max),
	@delimiter			char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @InstanceIDs GuidTableType
	INSERT INTO @InstanceIDs
	SELECT DISTINCT Ref.Value FROM [dbo].[GFN_StrToGuidTable](@strInstanceIDs, @delimiter) AS Ref
	
	EXEC [dbo].[FG_P_GetFormInstancesByIDs] @ApplicationID, @InstanceIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetFormInstanceElements]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetFormInstanceElements]
GO

CREATE PROCEDURE [dbo].[FG_GetFormInstanceElements]
	@ApplicationID	uniqueidentifier,
	@strInstanceIDs	varchar(max),
	@Filled			bit,
	@strElementIDs	varchar(max),
	@delimiter		char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @InstanceIDs GuidTableType
	
	INSERT INTO @InstanceIDs (Value)
	SELECT DISTINCT Ref.Value
	FROM [dbo].[GFN_StrToGuidTable](@strInstanceIDs, @delimiter) AS Ref
	
	DECLARE @ElementIDs GuidTableType
	INSERT INTO @ElementIDs
	SELECT Ref.Value FROM [dbo].[GFN_StrToGuidTable](@strElementIDs, @delimiter) AS Ref
	
	DECLARE @ELCount int = (SELECT COUNT(*) FROM @ElementIDs)
	
	SELECT	IE.ElementID,
			IE.InstanceID,
			IE.RefElementID,
			ISNULL(EFE.Title, IE.Title) AS Title,
			EFE.Name,
			EFE.Help,
			EFE.SequenceNumber,
			EFE.[Type],
			ISNULL(EFE.Info, IE.Info) AS Info,
			EFE.[Weight],
			IE.TextValue,
			IE.FloatValue,
			IE.BitValue,
			IE.DateValue,
			CAST(1 AS bit) AS Filled,
			ISNULL(EFE.Necessary, 0) AS Necessary,
			EFE.UniqueValue,
			CAST((
				SELECT COUNT(C.ID)
				FROM [dbo].[FG_Changes] AS C
				WHERE C.ApplicationID = @ApplicationID AND C.ElementID = IE.ElementID AND Deleted = 0
			) AS int) AS EditionsCount,
			UN.UserID AS CreatorUserID,
			UN.UserName AS CreatorUserName,
			UN.FirstName AS CreatorFirstName,
			UN.LastName AS CreatorLastName
	FROM @InstanceIDs AS INS
		INNER JOIN [dbo].[FG_InstanceElements] AS IE
		ON IE.ApplicationID = @ApplicationID AND IE.InstanceID = INS.Value
		LEFT JOIN [dbo].[FG_ExtendedFormElements] AS EFE
		ON EFE.ApplicationID = @ApplicationID AND EFE.ElementID = IE.RefElementID
		LEFT JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND UN.UserID = IE.CreatorUserID
	WHERE (@Filled IS NULL OR @Filled = 1) AND IE.Deleted = 0 AND
		(@ELCount = 0 OR IE.ElementID IN (SELECT Ref.Value FROM @ElementIDs AS Ref))
	
	UNION ALL
	
	SELECT	EFE.ElementID,
			FI.InstanceID,
			NULL AS RefElementID,
			EFE.Title,
			EFE.Name,
			EFE.Help,
			EFE.SequenceNumber,
			EFE.[Type],
			EFE.Info,
			EFE.[Weight],
			NULL AS TextValue,
			NULL AS FloatValue,
			NULL AS BitValue,
			NULL AS DateValue,
			CAST(0 AS bit) AS Filled,
			ISNULL(EFE.Necessary, 0) AS Necessary,
			EFE.UniqueValue,
			CAST(0 AS int) AS EditionsCount,
			NULL AS CreatorUserID,
			NULL AS CreatorUserName,
			NULL AS CreatorFirstName,
			NULL AS CreatorLastName
	FROM @InstanceIDs AS INS
		INNER JOIN [dbo].[FG_FormInstances] AS FI
		ON FI.ApplicationID = @ApplicationID AND FI.InstanceID = INS.Value
		INNER JOIN [FG_ExtendedFormElements] AS EFE
		ON EFE.ApplicationID = @ApplicationID AND EFE.FormID = FI.FormID
		LEFT JOIN [dbo].[FG_InstanceElements] AS IE
		ON IE.ApplicationID = @ApplicationID AND IE.InstanceID = FI.InstanceID AND 
			IE.RefElementID = EFE.ElementID AND IE.Deleted = 0
	WHERE IE.ElementID IS NULL AND (@Filled IS NULL OR @Filled = 0) AND EFE.Deleted = 0 AND
		(@ELCount = 0 OR EFE.ElementID IN (SELECT Ref.Value FROM @ElementIDs AS Ref))
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetSelectedGuids]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetSelectedGuids]
GO

CREATE PROCEDURE [dbo].[FG_GetSelectedGuids]
	@ApplicationID	uniqueidentifier,
	@strElementIDs	varchar(max),
	@delimiter		char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @ElementIDs GuidTableType
	INSERT INTO @ElementIDs
	SELECT Ref.Value FROM [dbo].[GFN_StrToGuidTable](@strElementIDs, @delimiter) AS Ref

	SELECT	S.ElementID,
			S.SelectedID AS ID,
			CASE
				WHEN ND.NodeID IS NOT NULL THEN ND.Name
				ELSE LTRIM(RTRIM(ISNULL(UN.FirstName, N'') + N' ' + ISNULL(UN.LastName, N'')))
			END AS Name
	FROM @ElementIDs AS IDs
		INNER JOIN [dbo].[FG_SelectedItems] AS S
		ON S.ApplicationID = @ApplicationID AND S.ElementID = IDs.Value AND S.Deleted = 0
		LEFT JOIN [dbo].[CN_Nodes] AS ND
		ON ND.ApplicationID = @ApplicationID AND ND.NodeID = S.SelectedID AND ND.Deleted = 0
		LEFT JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND UN.UserID = S.SelectedID
	WHERE ND.NodeID IS NOT NULL OR UN.UserID IS NOT NULL
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetElementChanges]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetElementChanges]
GO

CREATE PROCEDURE [dbo].[FG_GetElementChanges]
	@ApplicationID	uniqueidentifier,
	@ElementID		uniqueidentifier,
	@Count			int,
	@LowerBoundary	int
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT TOP(ISNULL(@Count, 10000)) *
	FROM (
			SELECT	ROW_NUMBER() OVER (ORDER BY C.ID DESC) AS RowNumber,
					C.ID,
					C.ElementID,
					EFE.Info,
					C.TextValue,
					C.BitValue,
					C.FloatValue,
					C.DateValue,
					C.CreationDate,
					C.CreatorUserID,
					UN.UserName AS CreatorUserName,
					UN.FirstName AS CreatorFirstName,
					UN.LastName AS CreatorLastName
			FROM [dbo].[FG_Changes] AS C
				LEFT JOIN [dbo].[Users_Normal] AS UN
				ON UN.ApplicationID = @ApplicationID AND UN.UserID = C.CreatorUserID
				INNER JOIN [dbo].[FG_InstanceElements] AS E
				ON E.ApplicationID = @ApplicationID AND E.ElementID = C.ElementID
				INNER JOIN [dbo].[FG_ExtendedFormElements] AS EFE
				ON EFE.ApplicationID = @ApplicationID AND EFE.ElementID = E.RefElementID
			WHERE C.ApplicationID = @ApplicationID AND C.ElementID = @ElementID AND C.Deleted = 0
		) AS X
	WHERE X.RowNumber >= ISNULL(@LowerBoundary, 0)
	ORDER BY X.RowNumber ASC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_P_SetFormInstanceAsFilled]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_P_SetFormInstanceAsFilled]
GO

CREATE PROCEDURE [dbo].[FG_P_SetFormInstanceAsFilled]
	@ApplicationID		uniqueidentifier,
	@InstanceID			uniqueidentifier,
	@FillingDate		datetime,
	@LastModifierUserID	uniqueidentifier,
	@_Result			int output
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[FG_FormInstances]
		SET Filled = 1,
			FillingDate = @FillingDate,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @FillingDate
	WHERE ApplicationID = @ApplicationID AND InstanceID = @InstanceID AND Filled = 0
	
	SET @_Result = @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_SetFormInstanceAsFilled]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_SetFormInstanceAsFilled]
GO

CREATE PROCEDURE [dbo].[FG_SetFormInstanceAsFilled]
	@ApplicationID		uniqueidentifier,
	@InstanceID			uniqueidentifier,
	@FillingDate		datetime,
	@LastModifierUserID	uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @_Result int
	
	EXEC [dbo].[FG_P_SetFormInstanceAsFilled] @ApplicationID, @InstanceID, 
		@FillingDate, @LastModifierUserID, @_Result output
	
	SELECT @_Result
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_P_SetFormInstanceAsNotFilled]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_P_SetFormInstanceAsNotFilled]
GO

CREATE PROCEDURE [dbo].[FG_P_SetFormInstanceAsNotFilled]
	@ApplicationID		uniqueidentifier,
	@InstanceID			uniqueidentifier,
	@LastModifierUserID	uniqueidentifier,
	@_Result			int output
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[FG_FormInstances]
		SET Filled = 0,
			LastModifierUserID = @LastModifierUserID
	WHERE ApplicationID = @ApplicationID AND InstanceID = @InstanceID AND Filled = 1 
	
	SET @_Result = @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_SetFormInstanceAsNotFilled]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_SetFormInstanceAsNotFilled]
GO

CREATE PROCEDURE [dbo].[FG_SetFormInstanceAsNotFilled]
	@ApplicationID		uniqueidentifier,
	@InstanceID			uniqueidentifier,
	@LastModifierUserID	uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @_Result int
	
	EXEC [dbo].[FG_P_SetFormInstanceAsNotFilled] @ApplicationID, 
		@InstanceID, @LastModifierUserID, @_Result output
	
	SELECT @_Result
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_IsDirector]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_IsDirector]
GO

CREATE PROCEDURE [dbo].[FG_IsDirector]
	@ApplicationID	uniqueidentifier,
	@InstanceID		uniqueidentifier,
	@UserID			uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF NOT EXISTS(
		SELECT TOP(1) * 
		FROM [dbo].[FG_FormInstances] 
		WHERE ApplicationID = @ApplicationID AND InstanceID = @InstanceID
	) BEGIN
		SET @InstanceID = (
			SELECT TOP(1) InstanceID 
			FROM [dbo].[FG_InstanceElements] 
			WHERE ApplicationID = @ApplicationID AND ElementID = @InstanceID
		)
	END
	
	DECLARE @DirectorID uniqueidentifier, @IsAdmin bit
	
	SELECT @DirectorID = DirectorID, @IsAdmin = [Admin]
	FROM [dbo].[FG_FormInstances]
	WHERE ApplicationID = @ApplicationID AND InstanceID = @InstanceID
	
	IF @IsAdmin = 0 SET @IsAdmin = NULL
	
	IF @DirectorID IS NOT NULL AND @DirectorID = @UserID BEGIN
		SELECT CAST(1 as bit)
		RETURN
	END
	
	DECLARE @NodeIDs GuidTableType
	INSERT INTO @NodeIDs
	EXEC [dbo].[CN_P_GetMemberNodeIDs] @ApplicationID, @UserID, NULL, N'Accepted', @IsAdmin
	
	IF @DirectorID IS NOT NULL AND @DirectorID IN(SELECT * FROM @NodeIDs)
		SELECT CAST(1 as bit)
	ELSE
		SELECT CAST(0 as bit)
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_P_SetFormOwner]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_P_SetFormOwner]
GO

CREATE PROCEDURE [dbo].[FG_P_SetFormOwner]
	@ApplicationID	uniqueidentifier,
	@OwnerID		uniqueidentifier,
	@FormID			uniqueidentifier,
	@CreatorUserID	uniqueidentifier,
	@CreationDate	datetime,
	@_Result		int output
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF EXISTS(
		SELECT TOP(1) * 
		FROM [dbo].[FG_FormOwners]
		WHERE ApplicationID = @ApplicationID AND OwnerID = @OwnerID
	) BEGIN
		UPDATE [dbo].[FG_FormOwners]
			SET FormID = @FormID,
				Deleted = 0,
				LastModifierUserID = @CreatorUserID,
				LastModificationDate = @CreationDate
		WHERE ApplicationID = @ApplicationID AND OwnerID = @OwnerID
	END
	ELSE BEGIN
		INSERT INTO [dbo].[FG_FormOwners](
			ApplicationID,
			OwnerID,
			FormID,
			CreatorUserID,
			CreationDate,
			Deleted
		)
		VALUES(
			@ApplicationID,
			@OwnerID,
			@FormID,
			@CreatorUserID,
			@CreationDate,
			0
		)
	END
	
	SET @_Result = @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_SetFormOwner]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_SetFormOwner]
GO

CREATE PROCEDURE [dbo].[FG_SetFormOwner]
	@ApplicationID	uniqueidentifier,
	@OwnerID		uniqueidentifier,
	@FormID			uniqueidentifier,
	@CreatorUserID	uniqueidentifier,
	@CreationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @_Result int
	
	EXEC [dbo].[FG_P_SetFormOwner] @ApplicationID, @OwnerID, 
		@FormID, @CreatorUserID, @CreationDate, @_Result output
	
	SELECT @_Result
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_ArithmeticDeleteFormOwner]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_ArithmeticDeleteFormOwner]
GO

CREATE PROCEDURE [dbo].[FG_ArithmeticDeleteFormOwner]
	@ApplicationID			uniqueidentifier,
	@OwnerID				uniqueidentifier,
	@FormID					uniqueidentifier,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[FG_FormOwners]
		SET Deleted = 1,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND OwnerID = @OwnerID AND FormID = @FormID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetOwnerForm]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetOwnerForm]
GO

CREATE PROCEDURE [dbo].[FG_GetOwnerForm]
	@ApplicationID	uniqueidentifier,
	@OwnerID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @FormIDs GuidTableType
	
	INSERT INTO @FormIDs
	SELECT FormID
	FROM [dbo].[FG_FormOwners]
	WHERE ApplicationID = @ApplicationID AND OwnerID = @OwnerID AND Deleted = 0
	
	EXEC [dbo].[FG_P_GetFormsByIDs] @ApplicationID, @FormIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_InitializeOwnerFormInstance]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_InitializeOwnerFormInstance]
GO

CREATE PROCEDURE [dbo].[FG_InitializeOwnerFormInstance]
	@ApplicationID	uniqueidentifier,
	@OwnerID		uniqueidentifier,
	@FormID			uniqueidentifier,
	@CreatorUserID	uniqueidentifier,
	@CreationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF @FormID IS NULL BEGIN
		SET @FormID = (
			SELECT TOP(1) FormID 
			FROM [dbo].[FG_FormOwners]
			WHERE ApplicationID = @ApplicationID AND OwnerID = @OwnerID AND Deleted = 0
		)
	END
	
	IF @FormID IS NULL BEGIN
		SELECT @FormID = FormID 
		FROM [dbo].[FG_FormOwners] AS FO
			INNER JOIN [dbo].[CN_Nodes] AS ND
			ON ND.ApplicationID = @ApplicationID AND ND.NodeTypeID = FO.OwnerID
		WHERE FO.ApplicationID = @ApplicationID AND ND.NodeID = @OwnerID AND FO.Deleted = 0
	END
	
	IF @FormID IS NULL OR @OwnerID IS NULL SELECT NULL
	ELSE BEGIN
		DECLARE @InstanceID uniqueidentifier = (
			SELECT TOP(1) InstanceID
			FROM [dbo].[FG_FormInstances]
			WHERE ApplicationID = @ApplicationID AND 
				FormID = @FormID AND OwnerID = @OwnerID AND Deleted = 0
		)
			
		IF @InstanceID IS NOT NULL SELECT @InstanceID
		ELSE BEGIN
			SET @InstanceID = NEWID()
			
			DECLARE @_Result int
			
			DECLARE @Instances FormInstanceTableType
			
			INSERT INTO @Instances (InstanceID, FormID, OwnerID, DirectorID, [Admin])
			VALUES (@InstanceID, @FormID, @OwnerID, NULL, NULL)
			
			EXEC [dbo].[FG_P_CreateFormInstance] @ApplicationID, @Instances, @CreatorUserID, @CreationDate, @_Result output
				
			IF @_Result > 0 SELECT @InstanceID
		END
	END
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_SetElementLimits]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_SetElementLimits]
GO

CREATE PROCEDURE [dbo].[FG_SetElementLimits]
	@ApplicationID		uniqueidentifier,
	@OwnerID			uniqueidentifier,
	@strElementIDs		varchar(max),
	@delimiter			char,
    @CreatorUserID		uniqueidentifier,
    @CreationDate		datetime
WITH ENCRYPTION
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	DECLARE @ElementIDs GuidTableType
	
	INSERT INTO @ElementIDs
	SELECT Ref.Value FROM [dbo].[GFN_StrToGuidTable](@strElementIDs, @delimiter) AS Ref
	
	DECLARE @ExistingIDs GuidTableType, @NotExistingIDs GuidTableType
	DECLARE @Count int
	
	INSERT INTO @ExistingIDs
	SELECT Ref.Value
	FROM @ElementIDs AS Ref
		INNER JOIN [dbo].[FG_ElementLimits] AS EL
		ON EL.ElementID = Ref.Value
	WHERE EL.ApplicationID = @ApplicationID AND EL.OwnerID = @OwnerID
	
	SET @Count = (SELECT COUNT(*) FROM @ExistingIDs)
	
	UPDATE [dbo].[FG_ElementLimits]
		SET LastModifierUserID = @CreatorUserID,
			LastModificationDate = @CreationDate,
			Deleted = 1
	WHERE ApplicationID = @ApplicationID AND OwnerID = @OwnerID
	
	IF @Count > 0 BEGIN
		UPDATE EL
			SET LastModifierUserID = @CreatorUserID,
				LastModificationDate = @CreationDate,
				Deleted = 0
		FROM @ExistingIDs AS E
			INNER JOIN [dbo].[FG_ElementLimits] AS EL
			ON EL.ElementID = E.Value
		WHERE EL.ApplicationID = @ApplicationID AND EL.OwnerID = @OwnerID
		
		IF @@ROWCOUNT <= 0 BEGIN
			ROLLBACK TRANSACTION
			RETURN
		END
	END
	
	INSERT INTO @NotExistingIDs
	SELECT E.Value
	FROM @ElementIDs AS E
	WHERE E.Value NOT IN(SELECT Ref.Value FROM @ExistingIDs AS Ref)
	
	SET @Count = (SELECT COUNT(*) FROM @NotExistingIDs)
	
	IF @Count > 0 BEGIN
		INSERT INTO [dbo].[FG_ElementLimits](
			ApplicationID,
			OwnerID,
			ElementID,
			Necessary,
			CreatorUserID,
			CreationDate,
			Deleted
		)
		SELECT @ApplicationID, @OwnerID, NE.Value, 
			ISNULL(EFE.Necessary, 0), @CreatorUserID, @CreationDate, 0
		FROM @NotExistingIDs AS NE
			INNER JOIN [dbo].[FG_ExtendedFormElements] AS EFE
			ON EFE.ApplicationID = @ApplicationID AND EFE.ElementID = NE.Value
	
		IF @@ROWCOUNT <= 0 BEGIN
			ROLLBACK TRANSACTION
			RETURN
		END
	END
	
	SELECT 1
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetElementLimits]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetElementLimits]
GO

CREATE PROCEDURE [dbo].[FG_GetElementLimits]
	@ApplicationID	uniqueidentifier,
	@OwnerID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @FormID uniqueidentifier = (
		SELECT TOP(1) FormID 
		FROM [dbo].[FG_FormOwners]
		WHERE ApplicationID = @ApplicationID AND OwnerID = @OwnerID AND Deleted = 0
	)
		
	IF @FormID IS NULL BEGIN
		SELECT @FormID = FormID, @OwnerID = ND.NodeTypeID
		FROM [dbo].[FG_FormOwners] AS FO
			INNER JOIN [dbo].[CN_Nodes] AS ND
			ON ND.ApplicationID = @ApplicationID AND ND.NodeTypeID = FO.OwnerID
		WHERE FO.ApplicationID = @ApplicationID AND ND.NodeID = @OwnerID AND FO.Deleted = 0
	END
	
	IF @FormID IS NOT NULL BEGIN
		SELECT EL.ElementID,
			   EFE.Title,
			   EL.Necessary,
			   EFE.[Type],
			   EFE.Info
		FROM [dbo].[FG_ElementLimits] AS EL
			INNER JOIN [dbo].[FG_ExtendedFormElements] AS EFE
			ON EFE.ApplicationID = @ApplicationID AND 
				EFE.ElementID = EL.ElementID AND EFE.Deleted = 0
		WHERE EL.ApplicationID = @ApplicationID AND 
			EL.OwnerID = @OwnerID AND EFE.FormID = @FormID AND EL.Deleted = 0
		ORDER BY EFE.SequenceNumber
	END
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_SetElementLimitNecessity]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_SetElementLimitNecessity]
GO

CREATE PROCEDURE [dbo].[FG_SetElementLimitNecessity]
	@ApplicationID			uniqueidentifier,
	@OwnerID				uniqueidentifier,
	@ElementID				uniqueidentifier,
	@Necessary				bit,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[FG_ElementLimits]
		SET Necessary = ISNULL(@Necessary, CAST(0 AS bit)),
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND OwnerID = @OwnerID AND ElementID = @ElementID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_ArithmeticDeleteElementLimit]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_ArithmeticDeleteElementLimit]
GO

CREATE PROCEDURE [dbo].[FG_ArithmeticDeleteElementLimit]
	@ApplicationID			uniqueidentifier,
	@OwnerID				uniqueidentifier,
	@ElementID				uniqueidentifier,
	@LastModifierUserID		uniqueidentifier,
	@LastModificationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[FG_ElementLimits]
		SET Deleted = 1,
			LastModifierUserID = @LastModifierUserID,
			LastModificationDate = @LastModificationDate
	WHERE ApplicationID = @ApplicationID AND OwnerID = @OwnerID AND ElementID = @ElementID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetCommonFormInstanceIDs]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetCommonFormInstanceIDs]
GO

CREATE PROCEDURE [dbo].[FG_GetCommonFormInstanceIDs]
	@ApplicationID	uniqueidentifier,
	@OwnerID		uniqueidentifier,
	@FilledOwnerID	uniqueidentifier,
	@HasLimit		bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT FI.InstanceID AS ID
	FROM [dbo].[FG_FormInstances] AS FI
		INNER JOIN (
			SELECT Ref.FormID
			FROM (
					SELECT FO.FormID, COUNT(EL.ElementID) AS CNT
					FROM [dbo].[FG_FormOwners] AS FO
						LEFT JOIN [dbo].[FG_ElementLimits] AS EL
						INNER JOIN [dbo].[FG_ExtendedFormElements] AS EFE
						ON EFE.ApplicationID = @ApplicationID AND EFE.ElementID = EL.ElementID
						ON EL.ApplicationID = @ApplicationID AND 
							EL.OwnerID = FO.OwnerID AND EFE.FormID = FO.FormID AND EL.Deleted = 0
					WHERE FO.ApplicationID = @ApplicationID AND 
						FO.OwnerID = @OwnerID AND FO.Deleted = 0
					GROUP BY FO.FormID
				) AS Ref
			WHERE @HasLimit IS NULL OR @HasLimit = 0 OR Ref.CNT > 0
		) AS FID
		ON FI.FormID = FID.FormID
	WHERE FI.ApplicationID = @ApplicationID AND FI.OwnerID = @FilledOwnerID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_P_GetFormRecords]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_P_GetFormRecords]
GO

CREATE PROCEDURE [dbo].[FG_P_GetFormRecords]
	@ApplicationID		uniqueidentifier,
	@FormID				uniqueidentifier,
	@ElementIDsTemp		GuidTableType readonly,
	@InstanceIDsTemp	GuidTableType readonly,
	@OwnerIDsTemp		GuidTableType readonly,
	@FiltersTemp		FormFilterTableType readonly,
	@LowerBoundary		int,
	@Count				int,
	@SortByElementID	uniqueidentifier,
	@DESC				bit
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @ElementIDs GuidTableType
	INSERT INTO @ElementIDs SELECT * FROM @ElementIDsTemp
	
	DECLARE @InstanceIDs GuidTableType
	INSERT INTO @InstanceIDs SELECT * FROM @InstanceIDsTemp
	
	DECLARE @OwnerIDs GuidTableType
	INSERT INTO @OwnerIDs SELECT * FROM @OwnerIDsTemp
	
	DECLARE @Filters FormFilterTableType
	INSERT INTO @Filters SELECT * FROM @FiltersTemp
	
	-- Preparing
	
	CREATE TABLE #Owners (Value uniqueidentifier primary key clustered)
	
	INSERT INTO #Owners (Value) SELECT O.Value FROM @OwnerIDs AS O
	
	DECLARE @HasOwner bit = CASE WHEN (SELECT TOP(1) * FROM @OwnerIDs) IS NOT NULL THEN 1 ELSE 0 END

	
	DECLARE @_ElemIDs KeyLessGuidTableType
	
	INSERT INTO @_ElemIDs (Value)
	SELECT EFE.ElementID
	FROM @ElementIDs AS E
		INNER JOIN [dbo].[FG_ExtendedFormElements] AS EFE
		ON EFE.ApplicationID = @ApplicationID AND EFE.ElementID = E.Value
	WHERE EFE.FormID = @FormID
	ORDER BY EFE.SequenceNumber
	
	IF (SELECT TOP(1) * FROM @ElementIDs) IS NULL BEGIN
		INSERT INTO @_ElemIDs (Value)
		SELECT ElementID 
		FROM [dbo].[FG_ExtendedFormElements]
		WHERE ApplicationID = @ApplicationID AND FormID = @FormID AND Deleted = 0
		ORDER BY SequenceNumber ASC
	END
	
	IF @SortByElementID IS NULL SET @SortByElementID = (SELECT TOP(1) Value FROM @_ElemIDs)
	
	IF @Count IS NULL SET @Count = 10000
	IF ISNULL(@LowerBoundary, 0) < 1 SET @LowerBoundary = 1
	DECLARE @UpperBoundary int = @LowerBoundary + @Count - 1
	
	CREATE TABLE #InstanceIDs (InstanceID uniqueidentifier primary key clustered,
		OwnerID uniqueidentifier, RowNum bigint)
		
	DECLARE @InstancesCount bigint = 0
	
	DECLARE @_STR_SBEID varchar(100), @_STR_FORMID varchar(100),
		@_STR_LB varchar(100), @_STR_UB varchar(100)
	SELECT @_STR_SBEID = CAST(@SortByElementID as varchar(100)),
		@_STR_FORMID = CAST(@FormID as varchar(100))
	
	IF EXISTS(SELECT TOP(1) * FROM @InstanceIDs) BEGIN
		INSERT INTO #InstanceIDs (InstanceID, OwnerID, RowNum)
		SELECT	I.Value, 
				FI.OwnerID, 
				ROW_NUMBER() OVER(ORDER BY I.Value ASC) AS RowNum
		FROM @InstanceIDs AS I
			LEFT JOIN [dbo].[FG_FormInstances] AS FI
			ON FI.ApplicationID = @ApplicationID AND FI.InstanceID = I.Value
	
		SET @InstancesCount = (SELECT COUNT(*) FROM #InstanceIDs)
	END
	ELSE BEGIN	
		DECLARE @_Proc varchar(max)
		
		SET @_Proc = 'INSERT INTO #InstanceIDs SELECT InstanceID, OwnerID, RowNum FROM ' + 
			'(SELECT ROW_NUMBER() OVER(ORDER BY R.RowNum) AS RowNum, R.InstanceID, R.OwnerID ' +
			'FROM (' +
			'SELECT ROW_NUMBER() OVER(PARTITION BY FI.InstanceID ORDER BY FI.InstanceID) AS P, ' + 
			'ROW_NUMBER() OVER(ORDER BY ' +
			(CASE WHEN @SortByElementID IS NULL THEN 'FI.InstanceID ' ELSE 'IE.ElementID ' END) +
			(CASE WHEN @DESC = 1 THEN 'DESC ' ELSE '' END) +
			') RowNum, FI.InstanceID, FI.OwnerID FROM ' +
			(CASE WHEN @HasOwner = 1 THEN '#Owners AS OW INNER JOIN ' ELSE '' END) +
			'[dbo].[FG_FormInstances] AS FI ' +
			(CASE WHEN @HasOwner = 1 THEN 'ON FI.ApplicationID = ''' + 
				CAST(@ApplicationID as varchar(50)) + ''' AND FI.OwnerID = OW.Value ' ELSE '' END) +
			(
				CASE 
					WHEN @SortByElementID IS NOT NULL
						THEN 'LEFT JOIN [dbo].[FG_InstanceElements] AS IE ' +
							'ON IE.ApplicationID = ''' + CAST(@ApplicationID as varchar(50)) + 
								''' AND IE.InstanceID = FI.InstanceID AND ' +
							'IE.RefElementID = N''' + @_STR_SBEID + ''' AND IE.Deleted = 0 '
					ELSE '' 
				END
			) +
			'WHERE FI.FormID = N''' + @_STR_FORMID + ''' AND FI.Deleted = 0 ' +
			')  AS R WHERE R.P = 1 ' +
			') AS Ref'
		
		EXEC (@_Proc)
		
		SET @InstancesCount = @@ROWCOUNT
	END
	
	IF @InstancesCount > 0 AND EXISTS(SELECT TOP(1) * FROM @Filters) BEGIN
		DECLARE @CurInstanceIDs GuidTableType
		
		INSERT INTO @CurInstanceIDs (Value)
		SELECT I.InstanceID
		FROM #InstanceIDs AS I
	
		DELETE I
		FROM #InstanceIDs AS I
			LEFT JOIN [dbo].[FG_FN_FilterInstances](
				@ApplicationID, NULL, @CurInstanceIDs, @Filters, ',', 1
			) AS Ret
			ON Ret.InstanceID = I.InstanceID
		WHERE Ret.InstanceID IS NULL
	END
	
	UPDATE I
		SET RowNum = CASE WHEN X.InstanceID IS NULL THEN 0 ELSE X.RowNum END
	FROM #InstanceIDs AS I
		LEFT JOIN (
			SELECT	D.InstanceID,
					D.OwnerID,
					ROW_NUMBER() OVER (ORDER BY D.RowNum ASC) AS RowNum
			FROM (
					SELECT	I.InstanceID,
							I.OwnerID,
							ROW_NUMBER() OVER (ORDER BY I.RowNum ASC) AS RowNum
					FROM #InstanceIDs AS I
				) AS D
			WHERE D.RowNum BETWEEN @LowerBoundary AND @UpperBoundary
		) AS X
		ON X.InstanceID = I.InstanceID
	
	DELETE #InstanceIDs
	WHERE RowNum = 0
	
	SET @InstancesCount = (SELECT COUNT(*) FROM #InstanceIDs)
	
	-- End of Preparing
	
	
	CREATE TABLE #Result
	(
		InstanceID		uniqueidentifier,
		OwnerID			uniqueidentifier,
		RefElementID	uniqueidentifier,
		CreationDate	datetime,
		BodyText		nvarchar(max),
		RowNum			bigint
	)

	INSERT INTO #Result(InstanceID, OwnerID, RefElementID, CreationDate, BodyText, RowNum)
	SELECT	FI.InstanceID, 
			INSTIDS.OwnerID,
			ELIDS.Value, 
			FI.CreationDate,
			[dbo].[FG_FN_ToString](@ApplicationID, IE.ElementID, IE.[Type], 
				IE.TextValue, IE.FloatValue, IE.BitValue, IE.DateValue),
			INSTIDS.RowNum
	FROM #InstanceIDs AS INSTIDS
		LEFT JOIN [dbo].[FG_FormInstances] AS FI
		ON FI.ApplicationID = @ApplicationID AND FI.InstanceID = INSTIDS.InstanceID
		LEFT JOIN @_ElemIDs AS ELIDS
		LEFT JOIN [dbo].[FG_InstanceElements] AS IE
		ON IE.ApplicationID = @ApplicationID AND IE.RefElementID = ELIDS.Value
		ON IE.InstanceID = FI.InstanceID
		

	DECLARE @lst varchar(max)
	SELECT @lst = ISNULL(@lst + ', ', '') + '[' + CAST(q.Value AS varchar(max)) + ']'
	FROM (SELECT Ref.Value FROM @_ElemIDs AS Ref) AS q
	
	SET @_Proc = 'SELECT * FROM ('
	
	DECLARE @BatchSize bigint = 1000
	DECLARE @Lower bigint = 0
	
	WHILE @InstancesCount >= 0 BEGIN
		IF @Lower > 0 SET @_Proc = @_Proc + ' UNION ALL '
		
		SET @_Proc = @_Proc + 'SELECT InstanceID, OwnerID, CreationDate, '+ @lst +  
			'FROM (SELECT InstanceID, OwnerID, RefElementID, CreationDate, BodyText
			FROM #Result ' + 
			'WHERE RowNum > ' + CAST(@Lower AS varchar(20)) + ' AND ' + 
				'RowNum <= ' + CAST(@Lower + @BatchSize AS varchar(20)) + ') P
			PIVOT (MAX(BodyText) FOR RefElementID IN('+ @lst + ' )) AS pvt '
			
		SET @InstancesCount = @InstancesCount - @BatchSize
		SET @Lower = @Lower + @BatchSize
	END
	
	SET @_Proc = @_Proc + ') AS TableName'
	IF @SortByElementID IS NOT NULL AND @_STR_SBEID IS NOT NULL BEGIN
		SET @_Proc = @_Proc + ' ORDER BY [' + @_STR_SBEID + ']'
		IF @DESC = 1 SET @_Proc = @_Proc + ' DESC'
	END
	
	EXEC(@_Proc)
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetFormRecords]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetFormRecords]
GO

CREATE PROCEDURE [dbo].[FG_GetFormRecords]
	@ApplicationID		uniqueidentifier,
	@FormID				uniqueidentifier,
	@ElementIDsTemp		GuidTableType readonly,
	@InstanceIDsTemp	GuidTableType readonly,
	@OwnerIDsTemp		GuidTableType readonly,
	@FiltersTemp		FormFilterTableType readonly,
	@LowerBoundary		int,
	@Count				int,
	@SortByElementID	uniqueidentifier,
	@DESC				bit
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @ElementIDs GuidTableType
	INSERT INTO @ElementIDs SELECT * FROM @ElementIDsTemp
	
	DECLARE @InstanceIDs GuidTableType
	INSERT INTO @InstanceIDs SELECT * FROM @InstanceIDsTemp
	
	DECLARE @OwnerIDs GuidTableType
	INSERT INTO @OwnerIDs SELECT * FROM @OwnerIDsTemp
	
	DECLARE @Filters FormFilterTableType
	INSERT INTO @Filters SELECT * FROM @FiltersTemp
	
	EXEC [dbo].[FG_P_GetFormRecords] @ApplicationID, @FormID, @ElementIDs, @InstanceIDs, 
		@OwnerIDs, @Filters, @LowerBoundary, @Count, @SortByElementID, @DESC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetFormStatistics]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetFormStatistics]
GO

CREATE PROCEDURE [dbo].[FG_GetFormStatistics]
	@ApplicationID	uniqueidentifier,
	@OwnerID		uniqueidentifier,
	@InstanceID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT	SUM(X.[Weight]) AS [WeightSum],
			SUM(X.[Avg]) AS [Sum],
			SUM(X.[WeightedAvg]) AS [WeightedSum],
			AVG(X.[Avg]) AS [Avg],
			CASE 
				WHEN SUM(X.[Weight]) = 0 THEN AVG(X.[Avg]) 
				ELSE SUM(X.[WeightedAvg]) / SUM(X.[Weight]) 
			END AS [WeightedAvg],
			MIN(X.[Avg]) AS [Min],
			MAX(X.[Avg]) AS [Max],
			VAR(X.[Avg]) AS [Var],
			STDEV(X.[AVG]) AS [StDev]
	FROM (
			SELECT	EFE.ElementID,
					ISNULL(MAX(EFE.[Weight]), 0) AS [Weight],
					MIN(IE.FloatValue) AS [Min],
					MAX(IE.FloatValue) AS [Max],
					AVG(IE.FloatValue) AS [Avg],
					(AVG(IE.FloatValue) * ISNULL(MAX(EFE.[Weight]), 0)) AS [WeightedAvg],
					ISNULL(VAR(IE.FloatValue), 0) AS [Var],
					ISNULL(STDEV(IE.FloatValue), 0) AS [StDev]
			FROM [dbo].[FG_FormInstances] AS FI
				INNER JOIN [dbo].[FG_InstanceElements] AS IE
				ON IE.ApplicationID = @ApplicationID AND IE.InstanceID = FI.InstanceID AND 
					IE.FloatValue IS NOT NULL AND IE.Deleted = 0
				INNER JOIN [dbo].[FG_ExtendedFormElements] AS EFE
				ON EFE.ApplicationID = @ApplicationID AND 
					EFE.ElementID = IE.RefElementID AND EFE.Deleted = 0
			WHERE FI.ApplicationID = @ApplicationID AND FI.Deleted = 0 AND
				(@OwnerID IS NOT NULL OR @InstanceID IS NOT NULL) AND
				(@OwnerID IS NULL OR FI.OwnerID = @OwnerID) AND
				(@InstanceID IS NULL OR FI.InstanceID = @InstanceID)
			GROUP BY EFE.ElementID
		) AS X
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_ConvertFormToTable]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_ConvertFormToTable]
GO

CREATE PROCEDURE [dbo].[FG_ConvertFormToTable]
	@ApplicationID	uniqueidentifier,
	@FormID			uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @TableName varchar(200)
	DECLARE @ElementIDs TABLE (Seq int IDENTITY(1, 1) primary key clustered, Value uniqueidentifier, Name varchar(200) )

	SELECT TOP(1) @TableName = 'FG_FRM_' + F.Name
	FROM [dbo].[FG_ExtendedForms] AS F
	WHERE F.ApplicationID = @ApplicationID AND F.FormID = @FormID AND 
		ISNULL(F.Name, N'') <> N'' AND F.Deleted = 0
	
	IF ISNULL(@TableName, N'') = N'' BEGIN
		SELECT -1
		RETURN
	END

	INSERT INTO @ElementIDs(Value, Name)
	SELECT EFE.ElementID, N'Col_' + EFE.Name
	FROM [dbo].[FG_ExtendedFormElements] AS EFE
	WHERE EFE.ApplicationID = @ApplicationID AND EFE.FormID = @FormID AND 
		ISNULL(EFE.Name, N'') <> N'' AND EFE.Deleted = 0
	ORDER BY EFE.SequenceNumber
	
	IF (SELECT COUNT(*) FROM @ElementIDs) = 0 BEGIN
		SELECT -1
		RETURN
	END

	CREATE TABLE #InstanceIDs (
		InstanceID uniqueidentifier primary key clustered, 
		OwnerID uniqueidentifier,
		CreatorID uniqueidentifier,
		CreationDate datetime,
		RowNum bigint
	)
		
	DECLARE @InstancesCount bigint = 0

	DECLARE @_Proc varchar(max)
		
	INSERT INTO #InstanceIDs (RowNum, InstanceID, OwnerID, CreatorID, CreationDate)
	SELECT	ROW_NUMBER() OVER(ORDER BY FI.CreationDate ASC, FI.InstanceID ASC) RowNum, 
			FI.InstanceID,
			FI.OwnerID,
			FI.CreatorUserID,
			Fi.CreationDate
	FROM [dbo].[FG_FormInstances] AS FI 
		LEFT JOIN [dbo].[CN_Nodes] AS ND
		ON ND.ApplicationID = @ApplicationID AND ND.NodeID = FI.OwnerID
	WHERE FI.ApplicationID = @ApplicationID AND FI.FormID = @FormID AND FI.Deleted = 0 AND 
		(ND.NodeID IS NULL OR ISNULL(ND.Deleted, 0) = 0)

	UPDATE I
		SET RowNum = CASE WHEN X.InstanceID IS NULL THEN 0 ELSE X.RowNum END
	FROM #InstanceIDs AS I
		LEFT JOIN (
			SELECT	D.InstanceID,
					ROW_NUMBER() OVER (ORDER BY D.RowNum ASC) AS RowNum
			FROM (
					SELECT	I.InstanceID,
							ROW_NUMBER() OVER (ORDER BY I.RowNum ASC) AS RowNum
					FROM #InstanceIDs AS I
				) AS D
		) AS X
		ON X.InstanceID = I.InstanceID

	DELETE #InstanceIDs
	WHERE RowNum = 0

	SET @InstancesCount = (SELECT COUNT(*) FROM #InstanceIDs)

	-- End of Preparing


	CREATE TABLE #Result (InstanceID uniqueidentifier, Name varchar(200), Value nvarchar(max), RowNum bigint)

	INSERT INTO #Result(InstanceID, Name, Value, RowNum)
	SELECT	FI.InstanceID, 
			ELIDS.Name, 
			[dbo].[FG_FN_ToString](@ApplicationID, IE.ElementID, IE.[Type], 
				IE.TextValue, IE.FloatValue, IE.BitValue, IE.DateValue),
			INSTIDS.RowNum
	FROM #InstanceIDs AS INSTIDS
		LEFT JOIN [dbo].[FG_FormInstances] AS FI
		ON FI.ApplicationID = @ApplicationID AND FI.InstanceID = INSTIDS.InstanceID
		LEFT JOIN @ElementIDs AS ELIDS
		LEFT JOIN [dbo].[FG_InstanceElements] AS IE
		ON IE.ApplicationID = @ApplicationID AND IE.RefElementID = ELIDS.Value
		ON IE.InstanceID = FI.InstanceID

	INSERT INTO #Result(InstanceID, Name, Value, RowNum)
	SELECT	FI.InstanceID, 
			ELIDS.Name + '_id', 
			CAST(IE.ElementID AS nvarchar(max)),
			INSTIDS.RowNum
	FROM #InstanceIDs AS INSTIDS
		LEFT JOIN [dbo].[FG_FormInstances] AS FI
		ON FI.ApplicationID = @ApplicationID AND FI.InstanceID = INSTIDS.InstanceID
		LEFT JOIN @ElementIDs AS ELIDS
		LEFT JOIN [dbo].[FG_InstanceElements] AS IE
		ON IE.ApplicationID = @ApplicationID AND IE.RefElementID = ELIDS.Value
		ON IE.InstanceID = FI.InstanceID

	INSERT INTO #Result(InstanceID, Name, Value, RowNum)
	SELECT	FI.InstanceID, 
			ELIDS.Name + '_text', 
			CAST(IE.TextValue AS nvarchar(max)),
			INSTIDS.RowNum
	FROM #InstanceIDs AS INSTIDS
		LEFT JOIN [dbo].[FG_FormInstances] AS FI
		ON FI.ApplicationID = @ApplicationID AND FI.InstanceID = INSTIDS.InstanceID
		LEFT JOIN @ElementIDs AS ELIDS
		LEFT JOIN [dbo].[FG_InstanceElements] AS IE
		ON IE.ApplicationID = @ApplicationID AND IE.RefElementID = ELIDS.Value
		ON IE.InstanceID = FI.InstanceID

	INSERT INTO #Result(InstanceID, Name, Value, RowNum)	
	SELECT	FI.InstanceID, 
			ELIDS.Name + '_float', 
			CAST(IE.FloatValue AS nvarchar(max)),
			INSTIDS.RowNum
	FROM #InstanceIDs AS INSTIDS
		LEFT JOIN [dbo].[FG_FormInstances] AS FI
		ON FI.ApplicationID = @ApplicationID AND FI.InstanceID = INSTIDS.InstanceID
		LEFT JOIN @ElementIDs AS ELIDS
		LEFT JOIN [dbo].[FG_InstanceElements] AS IE
		ON IE.ApplicationID = @ApplicationID AND IE.RefElementID = ELIDS.Value
		ON IE.InstanceID = FI.InstanceID
			
	INSERT INTO #Result(InstanceID, Name, Value, RowNum)
	SELECT	FI.InstanceID, 
			ELIDS.Name + '_bit', 
			CAST(IE.BitValue AS nvarchar(max)),
			INSTIDS.RowNum
	FROM #InstanceIDs AS INSTIDS
		LEFT JOIN [dbo].[FG_FormInstances] AS FI
		ON FI.ApplicationID = @ApplicationID AND FI.InstanceID = INSTIDS.InstanceID
		LEFT JOIN @ElementIDs AS ELIDS
		LEFT JOIN [dbo].[FG_InstanceElements] AS IE
		ON IE.ApplicationID = @ApplicationID AND IE.RefElementID = ELIDS.Value
		ON IE.InstanceID = FI.InstanceID
		
	INSERT INTO #Result(InstanceID, Name, Value, RowNum)
	SELECT	FI.InstanceID, 
			ELIDS.Name + '_date', 
			CAST(IE.DateValue AS nvarchar(max)),
			INSTIDS.RowNum
	FROM #InstanceIDs AS INSTIDS
		LEFT JOIN [dbo].[FG_FormInstances] AS FI
		ON FI.ApplicationID = @ApplicationID AND FI.InstanceID = INSTIDS.InstanceID
		LEFT JOIN @ElementIDs AS ELIDS
		LEFT JOIN [dbo].[FG_InstanceElements] AS IE
		ON IE.ApplicationID = @ApplicationID AND IE.RefElementID = ELIDS.Value
		ON IE.InstanceID = FI.InstanceID

	DECLARE @lst varchar(max), @selectLst varchar(max)

	SELECT @lst = ISNULL(@lst + ', ', '') + '[' + q.Name + ']' + ',[' + q.Name + '_id]' + + ',[' + q.Name + '_text]' +
		',[' + q.Name + '_float]' + ',[' + q.Name + '_bit]' + ',[' + q.Name + '_date]'
	FROM (SELECT Ref.Name FROM @ElementIDs AS Ref) AS q

	SELECT @selectLst = ISNULL(@selectLst + ', ', '') + 
		'pvt.[' + q.Name + ']' + 
		',cast(pvt.[' + q.Name + '_id] as uniqueidentifier) as [' + q.Name + '_id]' + 
		',pvt.[' + q.Name + '_text]' +
		',cast(pvt.[' + q.Name + '_float] as float) as [' + q.Name + '_float]' + 
		',cast(pvt.[' + q.Name + '_bit] as bit) as [' + q.Name + '_bit]' + 
		',cast(pvt.[' + q.Name + '_date] as datetime) as [' + q.Name + '_date]'
	FROM (SELECT Ref.Name FROM @ElementIDs AS Ref) AS q

	IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES 
		WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = @TableName))
	BEGIN
		EXEC ('DROP TABLE dbo.' + @TableName)
	END

	SET @_Proc = 'SELECT * into dbo.' + @TableName + ' FROM ('

	DECLARE @BatchSize bigint = 1000
	DECLARE @Lower bigint = 0

	WHILE @InstancesCount >= 0 BEGIN
		IF @Lower > 0 SET @_Proc = @_Proc + ' UNION ALL '
		
		SET @_Proc = @_Proc + 
			'SELECT pvt.InstanceID, I.OwnerID, I.CreationDate, UN.UserID, UN.UserName, UN.FirstName, UN.LastName, '+ @selectLst +  
			'FROM (SELECT InstanceID, Name, Value
			FROM #Result ' + 
			'WHERE RowNum > ' + CAST(@Lower AS varchar(20)) + ' AND ' + 
				'RowNum <= ' + CAST(@Lower + @BatchSize AS varchar(20)) + ') P
			PIVOT (MAX(Value) FOR Name IN('+ @lst + ' )) AS pvt ' +
			'INNER JOIN #InstanceIDs AS I ' +
			'ON I.InstanceID = pvt.InstanceID ' +
			'LEFT JOIN [dbo].[USR_View_Users] AS UN ' +
			'ON UN.UserID = I.CreatorID'
			
		SET @InstancesCount = @InstancesCount - @BatchSize
		SET @Lower = @Lower + @BatchSize
	END

	SET @_Proc = @_Proc + ') AS TableName'


	EXEC (@_Proc)
	
	SELECT 1
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetTemplateStatus]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetTemplateStatus]
GO

CREATE PROCEDURE [dbo].[FG_GetTemplateStatus]
	@ApplicationID		uniqueidentifier,
	@RefApplicationID	uniqueidentifier,
	@strTemplateIDs		varchar(max),
	@delimiter			char
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @TemplateIDs GuidTableType

	INSERT INTO @TemplateIDs ([Value])
	SELECT DISTINCT Ref.[Value]
	FROM [dbo].[GFN_StrToGuidTable](@strTemplateIDs, @delimiter) AS Ref
	
	SELECT	CAST(MAX(CAST(RNT.NodeTypeID AS varchar(50))) AS uniqueidentifier) AS TemplateID,
			MAX(RNT.[Name]) AS TemplateName,
			CAST(MAX(CAST(NT.NodeTypeID AS varchar(50))) AS uniqueidentifier) AS ActivatedID,
			MAX(NT.[Name]) AS ActivatedName,
			MAX(NT.CreationDate) AS ActivationDate,
			CAST(MAX(CAST(USR.UserID AS varchar(50))) AS uniqueidentifier) AS ActivatorUserID,
			MAX(USR.UserName) AS ActivatorUserName,
			MAX(USR.FirstName) AS ActivatorFirstName,
			MAX(USR.LastName) AS ActivatorLastName,
			COUNT(DISTINCT CASE WHEN Elems.RefDeleted = 0 THEN Elems.RefElementID ELSE NULL END) AS TemplateElementsCount,
			COUNT(DISTINCT CASE WHEN Elems.Deleted = 0 THEN Elems.ElementID ELSE NULL END) AS ElementsCount,
			COUNT(DISTINCT
				CASE
					WHEN Elems.RefElementID IS NOT NULL AND Elems.RefDeleted = 0 AND Elems.ElementID IS NULL THEN Elems.RefElementID
					ELSE NULL
				END
			) AS NewTemplateElementsCount,
			COUNT(DISTINCT
				CASE
					WHEN Elems.RefElementID IS NOT NULL AND Elems.ElementID IS NOT NULL AND 
						Elems.RefDeleted = 1 AND Elems.Deleted = 0 THEN Elems.RefElementID
					ELSE NULL
				END
			) AS RemovedTemplateElementsCount,
			COUNT(DISTINCT
				CASE
					WHEN Elems.RefElementID IS NULL AND Elems.ElementID IS NOT NULL AND Elems.Deleted = 0 THEN Elems.ElementID
					ELSE NULL
				END
			) AS NewCustomElementsCount
	FROM @TemplateIDs AS T 
		INNER JOIN [dbo].[FG_ExtendedForms] AS RF
		ON RF.ApplicationID = @RefApplicationID AND RF.FormID = T.[Value]
		INNER JOIN [dbo].[FG_ExtendedForms] AS F
		ON F.ApplicationID = @ApplicationID AND F.TemplateFormID = RF.FormID
		INNER JOIN [dbo].[FG_FormOwners] AS RO
		ON RO.ApplicationID = @RefApplicationID AND RO.FormID = RF.FormID AND RO.Deleted = 0
		INNER JOIN [dbo].[CN_NodeTypes] AS RNT
		ON RNT.ApplicationID = @RefApplicationID AND RNT.NodeTypeID = RO.OwnerID AND RNT.Deleted = 0
		INNER JOIN [dbo].[FG_FormOwners] AS O
		ON O.ApplicationID = @ApplicationID AND O.FormID = F.FormID AND O.Deleted = 0
		INNER JOIN [dbo].[CN_NodeTypes] AS NT
		ON NT.ApplicationID = @ApplicationID AND NT.NodeTypeID = O.OwnerID AND NT.Deleted = 0
		INNER JOIN [dbo].[USR_View_Users] AS USR
		ON USR.UserID = NT.CreatorUserID
		INNER JOIN (
			SELECT	RE.ApplicationID AS RefAppID, 
					RE.FormID AS RefFormID,
					RE.ElementID AS RefElementID,
					ISNULL(RE.Deleted, 0) AS RefDeleted,
					E.ApplicationID AS AppID,
					E.FormID AS FormID,
					E.ElementID AS ElementID,
					ISNULL(E.Deleted, 0) AS Deleted
			FROM [dbo].[FG_ExtendedFormElements] AS RE
				FULL OUTER JOIN [dbo].[FG_ExtendedFormElements] AS E
				ON E.TemplateElementID = RE.ElementID
			WHERE (RE.ApplicationID IS NULL OR RE.ApplicationID = @RefApplicationID) AND
				(E.ApplicationID IS NULL OR E.ApplicationID = @ApplicationID)
		) AS Elems
		ON (Elems.RefFormID = RF.FormID AND (Elems.FormID IS NULL OR Elems.FormID = F.FormID)) OR 
			(Elems.FormID = F.FormID AND (Elems.RefFormID IS NULL OR Elems.RefFormID = RF.FormID))
	GROUP BY RF.FormID, F.FormID
END

GO


-- Polls

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_P_GetPollsByIDs]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_P_GetPollsByIDs]
GO

CREATE PROCEDURE [dbo].[FG_P_GetPollsByIDs]
	@ApplicationID	uniqueidentifier,
	@PollIDsTemp	KeyLessGuidTableType readonly
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @PollIDs KeyLessGuidTableType
	INSERT INTO @PollIDs (Value) SELECT Value FROM @PollIDsTemp
	
	SELECT	P.PollID, 
			P.IsCopyOfPollID,
			P.OwnerID,
			P.[Name], 
			P2.[Name] AS RefName,
			P.[Description], 
			P2.[Description] AS RefDescription, 
			P.BeginDate, 
			P.FinishDate,
			CASE
				WHEN P.OwnerID IS NULL THEN P.ShowSummary
				ELSE ISNULL(P2.ShowSummary, P.ShowSummary)
			END AS ShowSummary,
			CASE
				WHEN P.OwnerID IS NULL THEN P.HideContributors
				ELSE ISNULL(P2.HideContributors, P.HideContributors)
			END AS HideContributors,
			P.Deleted AS Archived
	FROM @PollIDs AS IDs
		INNER JOIN [dbo].[FG_Polls] AS P
		ON P.ApplicationID = @ApplicationID AND P.PollID = IDs.Value
		LEFT JOIN [dbo].[FG_Polls] AS P2
		ON P2.ApplicationID = @ApplicationID AND P2.PollID = P.IsCopyOfPollID
	ORDER BY IDs.SequenceNumber ASC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetPollsByIDs]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetPollsByIDs]
GO

CREATE PROCEDURE [dbo].[FG_GetPollsByIDs]
	@ApplicationID	uniqueidentifier,
	@strPollIDs		varchar(max),
	@delimiter		char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @PollIDs KeyLessGuidTableType
	
	INSERT INTO @PollIDs (Value)
	SELECT Ref.Value
	FROM [dbo].[GFN_StrToGuidTable](@strPollIDs, @delimiter) AS Ref
	
	EXEC [dbo].[FG_P_GetPollsByIDs] @ApplicationID, @PollIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetPolls]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetPolls]
GO

CREATE PROCEDURE [dbo].[FG_GetPolls]
	@ApplicationID	uniqueidentifier,
	@IsCopyOfPollID	uniqueidentifier,
	@OwnerID		uniqueidentifier,
	@Archive		bit,
	@SearchText		nvarchar(500),
	@Count			int,
	@LowerBoundary	bigint
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @TempIDs KeyLessGuidTableType
	
	SET @Archive = ISNULL(@Archive, 0)
	SET @Count = ISNULL(@Count, 20)
	
	IF ISNULL(@SearchText, N'') = N'' BEGIN
		INSERT INTO @TempIDs (Value)
		SELECT N.PollID
		FROM (
				SELECT	ROW_NUMBER() OVER (ORDER BY P.CreationDate DESC, P.PollID DESC) AS RowNumber,
						ROW_NUMBER() OVER (ORDER BY P.CreationDate ASC, P.PollID ASC) AS RevRowNumber,
						P.PollID
				FROM [dbo].[FG_Polls] AS P
				WHERE P.ApplicationID = @ApplicationID AND 
					(@Archive IS NULL OR P.Deleted = @Archive) AND
					((@IsCopyOfPollID IS NULL AND P.IsCopyOfPollID IS NULL) OR 
					(@IsCopyOfPollID IS NOT NULL AND P.IsCopyOfPollID = @IsCopyOfPollID)) AND
					((@OwnerID IS NULL AND P.OwnerID IS NULL) OR 
					(@OwnerID IS NOT NULL AND P.OwnerID = @OwnerID))
			) AS N
		ORDER BY N.RowNumber ASC
	END
	ELSE BEGIN
		INSERT INTO @TempIDs (Value)
		SELECT N.PollID
		FROM (
				SELECT	ROW_NUMBER() OVER (ORDER BY SRCH.[Rank] DESC, SRCH.[Key] DESC) AS RowNumber,
						ROW_NUMBER() OVER (ORDER BY SRCH.[Rank] ASC, SRCH.[Key] ASC) AS RevRowNumber,
						P.PollID
				FROM CONTAINSTABLE([dbo].[FG_Polls], ([Name]), @SearchText) AS SRCH
					INNER JOIN [dbo].[FG_Polls] AS P
					ON SRCH.[Key] = P.PollID
				WHERE P.ApplicationID = @ApplicationID AND 
					(@Archive IS NULL OR P.Deleted = @Archive) AND
					((@IsCopyOfPollID IS NULL AND P.IsCopyOfPollID IS NULL) OR 
					(@IsCopyOfPollID IS NOT NULL AND P.IsCopyOfPollID = @IsCopyOfPollID)) AND
					((@OwnerID IS NULL AND P.OwnerID IS NULL) OR 
					(@OwnerID IS NOT NULL AND P.OwnerID = @OwnerID))
			) AS N
		ORDER BY N.RowNumber ASC
	END
	
	DECLARE @PollIDs KeyLessGuidTableType
	
	INSERT INTO @PollIDs (Value)
	SELECT TOP(@Count) IDs.Value
	FROM @TempIDs AS IDs
	WHERE IDs.SequenceNumber >= ISNULL(@LowerBoundary, 0)
	ORDER BY IDs.SequenceNumber ASC
	
	EXEC [dbo].[FG_P_GetPollsByIDs] @ApplicationID, @PollIDs
	
	SELECT TOP(1) COUNT(T.Value) AS TotalCount 
	FROM @TempIDs AS T
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetPollsByFormID]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetPollsByFormID]
GO

CREATE PROCEDURE [dbo].[FG_GetPollsByFormID]
	@ApplicationID	uniqueidentifier,
	@FormID			uniqueidentifier,
	@Archive		bit
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @PollIDs KeyLessGuidTableType
	
	INSERT INTO @PollIDs ([Value])
	SELECT DISTINCT P.PollID
	FROM [dbo].[FG_Polls] AS P
		INNER JOIN [dbo].[FG_FormOwners] AS O
		ON O.ApplicationID = @ApplicationID AND O.FormID = @FormID AND O.OwnerID = P.PollID
	WHERE P.ApplicationID = @ApplicationID AND (@Archive IS NULL OR P.Deleted = @Archive)
	
	EXEC [dbo].[FG_P_GetPollsByIDs] @ApplicationID, @PollIDs
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_P_AddPoll]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_P_AddPoll]
GO

CREATE PROCEDURE [dbo].[FG_P_AddPoll]
	@ApplicationID	uniqueidentifier,
	@PollID			uniqueidentifier,
	@CopyFromPollID	uniqueidentifier,
	@OwnerID		uniqueidentifier,
	@Name			nvarchar(255),
	@CurrentUserID	uniqueidentifier,
	@Now			datetime,
	@_Result		int output
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF @PollID = @CopyFromPollID BEGIN
		SET @_Result = -1
		RETURN
	END
	
	SET @Name = [dbo].[GFN_VerifyString](@Name)
	
	INSERT INTO [dbo].[FG_Polls] (
		ApplicationID,
		PollID,
		IsCopyOfPollID,
		OwnerID,
		Name,
		ShowSummary,
		HideContributors,
		CreatorUserID,
		CreationDate,
		Deleted
	)
	SELECT	@ApplicationID, 
			@PollID, 
			@CopyFromPollID, 
			@OwnerID,
			@Name, 
			ISNULL(P.ShowSummary, 1),
			ISNULL(P.HideContributors, 0),
			@CurrentUserID, 
			@Now,
			0
	FROM (SELECT @CopyFromPollID AS ID) AS Ref
		LEFT JOIN [dbo].[FG_Polls] AS P
		ON P.ApplicationID = @ApplicationID AND P.PollID = Ref.ID
	
	SET @_Result = @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_AddPoll]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_AddPoll]
GO

CREATE PROCEDURE [dbo].[FG_AddPoll]
	@ApplicationID	uniqueidentifier,
	@PollID			uniqueidentifier,
	@CopyFromPollID	uniqueidentifier,
	@OwnerID		uniqueidentifier,
	@Name			nvarchar(255),
	@CurrentUserID	uniqueidentifier,
	@Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @_Result int = 0
	
	EXEC [dbo].[FG_P_AddPoll] @ApplicationID, @PollID, @CopyFromPollID, 
		@OwnerID, @Name, @CurrentUserID, @Now, @_Result output
	
	SELECT @_Result
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetPollInstance]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetPollInstance]
GO

CREATE PROCEDURE [dbo].[FG_GetPollInstance]
	@ApplicationID	uniqueidentifier,
	@PollID			uniqueidentifier,
	@CopyFromPollID	uniqueidentifier,
	@OwnerID		uniqueidentifier,
	@CurrentUserID	uniqueidentifier,
	@Now			datetime
WITH ENCRYPTION
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	DECLARE @_Result int = 0
	
	IF NOT EXISTS (
		SELECT TOP(1) 1
		FROM [dbo].[FG_Polls]
		WHERE PollID = @PollID
	) BEGIN
		EXEC [dbo].[FG_P_AddPoll] @ApplicationID, @PollID, @CopyFromPollID, 
			@OwnerID, NULL, @CurrentUserID, @Now, @_Result output
		
		IF @_Result <= 0 BEGIN
			SELECT NULL
			ROLLBACK TRANSACTION
			RETURN
		END
	END
	
	DECLARE @FormID uniqueidentifier = (
		SELECT TOP(1) FO.FormID
		FROM [dbo].[FG_Polls] AS P
			INNER JOIN [dbo].[FG_FormOwners] AS FO
			ON FO.ApplicationID = @ApplicationID AND 
				FO.OwnerID = P.PollID AND FO.Deleted = 0
		WHERE P.ApplicationID = @ApplicationID AND P.PollID = @CopyFromPollID
	)
	
	IF @FormID IS NULL BEGIN
		SELECT NULL
		ROLLBACK TRANSACTION
		RETURN
	END
	
	DECLARE @InstanceID uniqueidentifier = (
		SELECT TOP(1) FI.InstanceID
		FROM [dbo].[FG_FormInstances] AS FI
		WHERE FI.ApplicationID = @ApplicationID AND FI.OwnerID = @PollID AND
			FI.DirectorID = @CurrentUserID AND FI.Deleted = 0
		ORDER BY FI.CreationDate DESC
	)
	
	IF @InstanceID IS NULL BEGIN
		SET @InstanceID = NEWID()
	
		SET @_Result = 0
		
		DECLARE @Instances FormInstanceTableType
			
		INSERT INTO @Instances (InstanceID, FormID, OwnerID, DirectorID, [Admin])
		VALUES (@InstanceID, @FormID, @PollID, @CurrentUserID, 0)
		
		EXEC [dbo].[FG_P_CreateFormInstance] @ApplicationID, @Instances, @CurrentUserID, @Now, @_Result output
			
		IF @_Result <= 0 BEGIN
			SELECT NULL
			ROLLBACK TRANSACTION
			RETURN
		END
	END
	
	SELECT @InstanceID
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetOwnerPollIDs]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetOwnerPollIDs]
GO

CREATE PROCEDURE [dbo].[FG_GetOwnerPollIDs]
	@ApplicationID	uniqueidentifier,
	@IsCopyOfPollID	uniqueidentifier,
	@OwnerID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT PollID AS ID
	FROM [dbo].[FG_Polls]
	WHERE ApplicationID = @ApplicationID AND IsCopyOfPollID = @IsCopyOfPollID AND 
		OwnerID = @OwnerID AND Deleted = 0
	ORDER BY CreationDate DESC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_RenamePoll]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_RenamePoll]
GO

CREATE PROCEDURE [dbo].[FG_RenamePoll]
	@ApplicationID	uniqueidentifier,
	@PollID			uniqueidentifier,
	@Name			nvarchar(255),
	@CurrentUserID	uniqueidentifier,
	@Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SET @Name = [dbo].[GFN_VerifyString](@Name)
	
	UPDATE [dbo].[FG_Polls]
		SET Name = @Name,
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	WHERE ApplicationID = @ApplicationID AND PollID = @PollID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_SetPollDescription]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_SetPollDescription]
GO

CREATE PROCEDURE [dbo].[FG_SetPollDescription]
	@ApplicationID	uniqueidentifier,
	@PollID			uniqueidentifier,
	@Description	nvarchar(2000),
	@CurrentUserID	uniqueidentifier,
	@Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SET @Description = [dbo].[GFN_VerifyString](@Description)
	
	UPDATE [dbo].[FG_Polls]
		SET [Description] = @Description,
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	WHERE ApplicationID = @ApplicationID AND PollID = @PollID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_SetPollBeginDate]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_SetPollBeginDate]
GO

CREATE PROCEDURE [dbo].[FG_SetPollBeginDate]
	@ApplicationID	uniqueidentifier,
	@PollID			uniqueidentifier,
	@BeginDate		datetime,
	@CurrentUserID	uniqueidentifier,
	@Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[FG_Polls]
		SET BeginDate = @BeginDate,
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	WHERE ApplicationID = @ApplicationID AND PollID = @PollID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_SetPollFinishDate]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_SetPollFinishDate]
GO

CREATE PROCEDURE [dbo].[FG_SetPollFinishDate]
	@ApplicationID	uniqueidentifier,
	@PollID			uniqueidentifier,
	@FinishDate		datetime,
	@CurrentUserID	uniqueidentifier,
	@Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[FG_Polls]
		SET FinishDate = @FinishDate,
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	WHERE ApplicationID = @ApplicationID AND PollID = @PollID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_SetPollShowSummary]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_SetPollShowSummary]
GO

CREATE PROCEDURE [dbo].[FG_SetPollShowSummary]
	@ApplicationID	uniqueidentifier,
	@PollID			uniqueidentifier,
	@ShowSummary	bit,
	@CurrentUserID	uniqueidentifier,
	@Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[FG_Polls]
		SET ShowSummary = ISNULL(@ShowSummary, 0),
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	WHERE ApplicationID = @ApplicationID AND PollID = @PollID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_SetPollHideContributors]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_SetPollHideContributors]
GO

CREATE PROCEDURE [dbo].[FG_SetPollHideContributors]
	@ApplicationID		uniqueidentifier,
	@PollID				uniqueidentifier,
	@HideContributors	bit,
	@CurrentUserID		uniqueidentifier,
	@Now				datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[FG_Polls]
		SET HideContributors = ISNULL(@HideContributors, 0),
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	WHERE ApplicationID = @ApplicationID AND PollID = @PollID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_RemovePoll]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_RemovePoll]
GO

CREATE PROCEDURE [dbo].[FG_RemovePoll]
	@ApplicationID	uniqueidentifier,
	@PollID			uniqueidentifier,
	@CurrentUserID	uniqueidentifier,
	@Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[FG_Polls]
		SET Deleted = 1,
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	WHERE ApplicationID = @ApplicationID AND PollID = @PollID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_RecyclePoll]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_RecyclePoll]
GO

CREATE PROCEDURE [dbo].[FG_RecyclePoll]
	@ApplicationID	uniqueidentifier,
	@PollID			uniqueidentifier,
	@CurrentUserID	uniqueidentifier,
	@Now			datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[FG_Polls]
		SET Deleted = 0,
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	WHERE ApplicationID = @ApplicationID AND PollID = @PollID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetPollStatus]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetPollStatus]
GO

CREATE PROCEDURE [dbo].[FG_GetPollStatus]
	@ApplicationID	uniqueidentifier,
	@PollID			uniqueidentifier,
	@IsCopyOfPollID	uniqueidentifier,
	@CurrentUserID	uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @Description nvarchar(2000)
	DECLARE @BeginDate datetime
	DECLARE @FinishDate datetime
	DECLARE @InstanceID uniqueidentifier
	DECLARE @ElementsCount int
	DECLARE @FilledElementsCount int
	DECLARE @AllFilledFormsCount int

	IF @PollID IS NULL BEGIN
		SELECT TOP(1) 
			@Description = P.[Description]
		FROM [dbo].[FG_Polls] AS P
		WHERE P.ApplicationID = @ApplicationID AND P.PollID = @IsCopyOfPollID
	END
	ELSE BEGIN
		SELECT TOP(1) 
			@Description = ISNULL(P.[Description], Ref.[Description]), 
			@BeginDate = P.BeginDate,
			@FinishDate = P.FinishDate,
			@InstanceID = FI.InstanceID
		FROM [dbo].[FG_Polls] AS P
			INNER JOIN [dbo].[FG_Polls] AS Ref
			ON Ref.ApplicationID = @ApplicationID AND Ref.PollID = P.IsCopyOfPollID
			INNER JOIN [dbo].[FG_FormOwners] AS FO
			ON FO.ApplicationID = @ApplicationID AND FO.OwnerID = Ref.PollID AND FO.Deleted = 0
			LEFT JOIN [dbo].[FG_FormInstances] AS FI
			ON FI.ApplicationID = @ApplicationID AND FI.FormID = FO.FormID AND 
				FI.OwnerID = @PollID AND FI.DirectorID = @CurrentUserID
		WHERE P.ApplicationID = @ApplicationID AND P.PollID = @PollID
		ORDER BY FI.CreationDate DESC
	END

	DECLARE @LimitedElements GuidTableType
	
	INSERT INTO @LimitedElements (Value)
	SELECT Ref.ElementID
	FROM [dbo].[FG_FN_GetLimitedElements](@ApplicationID, @IsCopyOfPollID) AS Ref
	
	SELECT TOP(1)	
		@ElementsCount = COUNT(DISTINCT L.Value),
		@FilledElementsCount = COUNT(DISTINCT IE.ElementID)
	FROM @LimitedElements AS L
		INNER JOIN [dbo].[FG_ExtendedFormElements] AS EFE
		ON EFE.ApplicationID = @ApplicationID AND EFE.ElementID = L.Value AND
			[dbo].[FG_FN_IsFillable](EFE.[Type]) = 1
		LEFT JOIN [dbo].[FG_InstanceElements] AS IE
		ON IE.ApplicationID = @ApplicationID AND IE.InstanceID = @InstanceID AND 
			IE.RefElementID = L.Value AND IE.Deleted = 0 AND
			ISNULL([dbo].[FG_FN_ToString](@ApplicationID, IE.ElementID, 
				IE.[Type], IE.TextValue, IE.FloatValue, IE.BitValue, IE.DateValue), N'') <> N''

	SELECT @AllFilledFormsCount = COUNT(DISTINCT FI.DirectorID)
	FROM [dbo].[FG_FormInstances] AS FI
		INNER JOIN [dbo].[FG_InstanceElements] AS IE
		ON IE.ApplicationID = @ApplicationID AND 
			IE.InstanceID = FI.InstanceID AND IE.Deleted = 0 AND
			ISNULL([dbo].[FG_FN_ToString](@ApplicationID, IE.ElementID, IE.[Type], 
				IE.TextValue, IE.FloatValue, IE.BitValue, IE.DateValue), N'') <> N''
		INNER JOIN @LimitedElements AS L
		ON L.Value = IE.RefElementID
	WHERE FI.ApplicationID = @ApplicationID AND FI.OwnerID = @PollID AND FI.Deleted = 0
			
	SELECT	@Description AS [Description],
			@BeginDate AS BeginDate,
			@FinishDate AS FinishDate,
			@InstanceID AS InstanceID,
			@ElementsCount AS ElementsCount, 
			@FilledElementsCount AS FilledElementsCount,
			@AllFilledFormsCount AS AllFilledFormsCount
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetPollElementsInstanceCount]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetPollElementsInstanceCount]
GO

CREATE PROCEDURE [dbo].[FG_GetPollElementsInstanceCount]
	@ApplicationID	uniqueidentifier,
	@PollID			uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @IsCopyOfPollID uniqueidentifier = (
		SELECT TOP(1) IsCopyOfPollID
		FROM [dbo].[FG_Polls]
		WHERE PollID = @PollID
	)

	IF @IsCopyOfPollID IS NULL RETURN

	SELECT IE.RefElementID AS ID, COUNT(DISTINCT FI.DirectorID) AS [Count]
	FROM [dbo].[FG_FormInstances] AS FI
		INNER JOIN [dbo].[FG_InstanceElements] AS IE
		ON IE.ApplicationID = @ApplicationID AND 
			IE.InstanceID = FI.InstanceID AND IE.Deleted = 0 AND
			ISNULL([dbo].[FG_FN_ToString](@ApplicationID, IE.ElementID, IE.[Type], 
				IE.TextValue, IE.FloatValue, IE.BitValue, IE.DateValue), N'') <> N''
		INNER JOIN (
			SELECT Ref.ElementID
			FROM [dbo].[FG_FN_GetLimitedElements](@ApplicationID, @IsCopyOfPollID) as ref
		) AS E
		ON E.ElementID = IE.RefElementID
	WHERE FI.ApplicationID = @ApplicationID AND 
		FI.OwnerID = @PollID AND FI.Deleted = 0
	GROUP BY IE.RefElementID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetPollAbstractText]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetPollAbstractText]
GO

CREATE PROCEDURE [dbo].[FG_GetPollAbstractText]
	@ApplicationID	uniqueidentifier,
	@PollID			uniqueidentifier,
	@strElementIDs	varchar(max),
	@delimiter		char,
	@Count			int,
	@LowerBoundary	int
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @ElementIDs GuidTableType
	
	INSERT INTO @ElementIDs (Value)
	SELECT Ref.Value
	FROM [dbo].[GFN_StrToGuidTable](@strElementIDs, @delimiter) AS Ref
	
	DECLARE @Temp Table(ElementID uniqueidentifier, Value nvarchar(max), Number float,
		Seq int IDENTITY(1,1) primary key clustered)
	DECLARE @TBL Table(ElementID uniqueidentifier, Value nvarchar(max), Number float)

	INSERT INTO @Temp (ElementID, Value, Number)
	SELECT IDs.Value, IE.TextValue, IE.FloatValue
	FROM [dbo].[FG_FormInstances] AS FI
		INNER JOIN [dbo].[FG_InstanceElements] AS IE
		ON IE.ApplicationID = @ApplicationID AND 
			IE.InstanceID = FI.InstanceID AND IE.Deleted = 0 AND
			LTRIM(RTRIM(ISNULL(IE.TextValue, N''))) <> N''
		INNER JOIN @ElementIDs AS IDs
		ON IDs.Value = IE.RefElementID
	WHERE FI.ApplicationID = @ApplicationID AND FI.OwnerID = @PollID AND FI.Deleted = 0

	DECLARE @Cur int = (SELECT MAX(Seq) FROM @Temp)

	WHILE @Cur > 0 BEGIN
		DECLARE @EID uniqueidentifier
		DECLARE @Val nvarchar(max)
		DECLARE @Num float
		
		SELECT TOP(1) @EID = T.ElementID, @Val = T.Value, @Num = T.Number
		FROM @Temp AS T
		WHERE T.Seq = @Cur

		INSERT INTO @TBL (ElementID, Value, Number)
		SELECT @EID, Ref.Value, @Num
		FROM [dbo].[GFN_StrToStringTable](@Val, N'~') AS Ref
		
		SET @Cur = @Cur - 1
	END

	SELECT TOP(ISNULL(@Count, 5))
		CAST((V.RowNumber + V.RevRowNumber - 1) AS int) AS TotalValuesCount,
		V.ElementID,
		V.Value,
		V.[Count]
	FROM (
			SELECT	ROW_NUMBER() OVER(PARTITION BY X.ElementID ORDER BY X.[Count] DESC, X.Value DESC) AS RowNumber,
					ROW_NUMBER() OVER(PARTITION BY X.ElementID ORDER BY X.[Count] ASC, X.Value ASC) AS RevRowNumber,
					X.*
			FROM (
					SELECT T.ElementID, T.Value, COUNT(T.ElementID) AS [Count]
					FROM @TBL AS T
					GROUP BY T.ElementID, T.Value
				) AS X
		) AS V
	WHERE V.RowNumber >= ISNULL(@LowerBoundary, 0)
	ORDER BY V.RowNumber ASC
	
	SELECT	T.ElementID, 
			MIN(T.Number) AS [Min], 
			MAX(T.Number) AS [Max], 
			AVG(T.Number) AS [Avg], 
			ISNULL(VAR(T.Number), 0) AS [Var], 
			ISNULL(STDEV(T.Number), 0) AS [StDev]
	FROM @TBL AS T
	WHERE T.Number IS NOT NULL
	GROUP BY T.ElementID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetPollAbstractGuid]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetPollAbstractGuid]
GO

CREATE PROCEDURE [dbo].[FG_GetPollAbstractGuid]
	@ApplicationID	uniqueidentifier,
	@PollID			uniqueidentifier,
	@strElementIDs	varchar(max),
	@delimiter		char,
	@Count			int,
	@LowerBoundary	int
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @ElementIDs GuidTableType
	
	INSERT INTO @ElementIDs (Value)
	SELECT Ref.Value
	FROM [dbo].[GFN_StrToGuidTable](@strElementIDs, @delimiter) AS Ref
	
	DECLARE @TBL Table(ElementID uniqueidentifier, Value uniqueidentifier)

	INSERT INTO @TBL (ElementID, Value)
	SELECT IDs.Value, S.SelectedID
	FROM [dbo].[FG_FormInstances] AS FI
		INNER JOIN [dbo].[FG_InstanceElements] AS IE
		ON IE.ApplicationID = @ApplicationID AND 
			IE.InstanceID = FI.InstanceID AND IE.Deleted = 0
		INNER JOIN @ElementIDs AS IDs
		ON IDs.Value = IE.RefElementID
		INNER JOIN [dbo].[FG_SelectedItems] AS S
		ON S.ApplicationID = @ApplicationID AND S.ElementID = IE.ElementID AND S.Deleted = 0
	WHERE FI.ApplicationID = @ApplicationID AND FI.OwnerID = @PollID AND FI.Deleted = 0

	SELECT TOP(ISNULL(@Count, 5))
		CAST((V.RowNumber + V.RevRowNumber - 1) AS int) AS TotalValuesCount,
		V.ElementID,
		V.Value,
		V.[Count]
	FROM (
			SELECT	ROW_NUMBER() OVER(PARTITION BY X.ElementID ORDER BY X.[Count] DESC, X.Value DESC) AS RowNumber,
					ROW_NUMBER() OVER(PARTITION BY X.ElementID ORDER BY X.[Count] ASC, X.Value ASC) AS RevRowNumber,
					X.*
			FROM (
					SELECT T.ElementID, T.Value, COUNT(T.ElementID) AS [Count]
					FROM @TBL AS T
					GROUP BY T.ElementID, T.Value
				) AS X
		) AS V
	WHERE V.RowNumber >= ISNULL(@LowerBoundary, 0)
	ORDER BY V.RowNumber ASC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetPollAbstractBool]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetPollAbstractBool]
GO

CREATE PROCEDURE [dbo].[FG_GetPollAbstractBool]
	@ApplicationID	uniqueidentifier,
	@PollID			uniqueidentifier,
	@strElementIDs	varchar(max),
	@delimiter		char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @ElementIDs GuidTableType
	
	INSERT INTO @ElementIDs (Value)
	SELECT Ref.Value
	FROM [dbo].[GFN_StrToGuidTable](@strElementIDs, @delimiter) AS Ref
	
	DECLARE @TBL Table(ElementID uniqueidentifier, Value bit)

	INSERT INTO @TBL (ElementID, Value)
	SELECT IDs.Value, IE.BitValue
	FROM [dbo].[FG_FormInstances] AS FI
		INNER JOIN [dbo].[FG_InstanceElements] AS IE
		ON IE.ApplicationID = @ApplicationID AND 
			IE.InstanceID = FI.InstanceID AND IE.Deleted = 0 AND IE.BitValue IS NOT NULL
		INNER JOIN @ElementIDs AS IDs
		ON IDs.Value = IE.RefElementID
	WHERE FI.ApplicationID = @ApplicationID AND FI.OwnerID = @PollID AND FI.Deleted = 0

	SELECT T.ElementID, T.Value, COUNT(T.ElementID) AS [Count], NULL AS TotalValuesCount
	FROM @TBL AS T
	GROUP BY T.ElementID, T.Value
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetPollAbstractNumber]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetPollAbstractNumber]
GO

CREATE PROCEDURE [dbo].[FG_GetPollAbstractNumber]
	@ApplicationID	uniqueidentifier,
	@PollID			uniqueidentifier,
	@strElementIDs	varchar(max),
	@delimiter		char,
	@Count			int,
	@LowerBoundary	int
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @ElementIDs GuidTableType
	
	INSERT INTO @ElementIDs (Value)
	SELECT Ref.Value
	FROM [dbo].[GFN_StrToGuidTable](@strElementIDs, @delimiter) AS Ref
	
	DECLARE @TBL Table(ElementID uniqueidentifier, Value float)

	INSERT INTO @TBL (ElementID, Value)
	SELECT IDs.Value, IE.FloatValue
	FROM [dbo].[FG_FormInstances] AS FI
		INNER JOIN [dbo].[FG_InstanceElements] AS IE
		ON IE.ApplicationID = @ApplicationID AND 
			IE.InstanceID = FI.InstanceID AND IE.Deleted = 0 AND IE.FloatValue IS NOT NULL
		INNER JOIN @ElementIDs AS IDs
		ON IDs.Value = IE.RefElementID
	WHERE FI.ApplicationID = @ApplicationID AND FI.OwnerID = @PollID AND FI.Deleted = 0

	SELECT TOP(ISNULL(@Count, 5))
		CAST((V.RowNumber + V.RevRowNumber - 1) AS int) AS TotalValuesCount,
		V.ElementID,
		CAST(V.Value AS float) AS Value,
		V.[Count]
	FROM (
			SELECT	ROW_NUMBER() OVER(PARTITION BY X.ElementID ORDER BY X.[Count] DESC, X.Value DESC) AS RowNumber,
					ROW_NUMBER() OVER(PARTITION BY X.ElementID ORDER BY X.[Count] ASC, X.Value ASC) AS RevRowNumber,
					X.*
			FROM (
					SELECT T.ElementID, T.Value, COUNT(T.ElementID) AS [Count]
					FROM @TBL AS T
					GROUP BY T.ElementID, T.Value
				) AS X
		) AS V
	WHERE V.RowNumber >= ISNULL(@LowerBoundary, 0)
	ORDER BY V.RowNumber ASC
	
	SELECT	T.ElementID, 
			MIN(T.Value) AS [Min], 
			MAX(T.Value) AS [Max], 
			AVG(T.Value) AS [Avg], 
			ISNULL(VAR(T.Value), 0) AS [Var], 
			ISNULL(STDEV(T.Value), 0) AS [StDev]
	FROM @TBL AS T
	GROUP BY T.ElementID
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetPollElementInstances]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetPollElementInstances]
GO

CREATE PROCEDURE [dbo].[FG_GetPollElementInstances]
	@ApplicationID	uniqueidentifier,
	@PollID			uniqueidentifier,
	@ElementID		uniqueidentifier,
	@Count			int,
	@LowerBoundary	int
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT TOP(ISNULL(@Count, 20)) *
	FROM (
			SELECT	ROW_NUMBER() OVER(ORDER BY ISNULL(IE.LastModificationDate, IE.CreationDate) DESC, 
						IE.ElementID DESC) AS RowNumber,
					UN.UserID,
					UN.UserName,
					UN.FirstName,
					UN.LastName,
					IE.ElementID,
					IE.RefElementID,
					IE.[Type],
					ISNULL(IE.TextValue, N'') AS TextValue,
					IE.FloatValue,
					IE.BitValue,
					IE.DateValue,
					IE.CreationDate,
					IE.LastModificationDate
			FROM [dbo].[FG_FormInstances] AS FI
				INNER JOIN [dbo].[FG_InstanceElements] AS IE
				ON IE.ApplicationID = @ApplicationID AND IE.InstanceID = FI.InstanceID AND
					IE.RefElementID = @ElementID AND IE.Deleted = 0 AND
					ISNULL([dbo].[FG_FN_ToString](@ApplicationID, IE.ElementID, IE.[Type], 
						IE.TextValue, IE.FloatValue, IE.BitValue, IE.DateValue), N'') <> N''
				INNER JOIN [dbo].[Users_Normal] AS UN
				ON UN.ApplicationID = @ApplicationID AND UN.UserID = FI.DirectorID
			WHERE FI.ApplicationID = @ApplicationID AND FI.OwnerID = @PollID AND 
				FI.DirectorID IS NOT NULL AND FI.Deleted = 0
		) AS X
	WHERE X.RowNumber >= ISNULL(@LowerBoundary, 0)
	ORDER BY X.RowNumber ASC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetCurrentPollsCount]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetCurrentPollsCount]
GO

CREATE PROCEDURE [dbo].[FG_GetCurrentPollsCount]
	@ApplicationID	uniqueidentifier,
	@CurrentUserID	uniqueidentifier,
	@Now			datetime,
	@DefaultPrivacy	varchar(50)
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @PollIDs KeyLessGuidTableType

	INSERT INTO @PollIDs (Value)
	SELECT P.PollID
	FROM [dbo].[FG_Polls] AS P
	WHERE P.ApplicationID = @ApplicationID AND 
		P.IsCopyOfPollID IS NOT NULL AND P.OwnerID IS NULL AND P.Deleted = 0 AND 
		(P.BeginDate IS NOT NULL OR P.FinishDate IS NOT NULL) AND
		(P.BeginDate IS NULL OR P.BeginDate <= @Now) AND
		(P.FinishDate IS NULL OR P.FinishDate >= @Now)
		
	DECLARE	@PermissionTypes StringPairTableType
	
	INSERT INTO @PermissionTypes (FirstValue, SecondValue)
	VALUES (N'View', @DefaultPrivacy)

	SELECT	CAST(COUNT(X.PollID) AS int) AS [Count],
			CAST(SUM(X.Done) AS int) AS DoneCount
	FROM (
			SELECT	P.PollID,
					MAX(CAST((CASE WHEN FI.InstanceID IS NULL THEN 0 ELSE 1 END) AS int)) AS Done
			FROM [dbo].[PRVC_FN_CheckAccess](@ApplicationID, @CurrentUserID, 
				@PollIDs, N'Poll', @Now, @PermissionTypes) AS IDs
				INNER JOIN [dbo].[FG_Polls] AS P
				ON P.ApplicationID = @ApplicationID AND P.PollID = IDs.ID
				LEFT JOIN [dbo].[FG_FormInstances] AS FI
				ON FI.ApplicationID = @ApplicationID AND FI.OwnerID = IDs.ID AND
					FI.DirectorID = @CurrentUserID AND FI.Deleted = 0
			GROUP BY P.PollID
		) AS X
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_GetCurrentPolls]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_GetCurrentPolls]
GO

CREATE PROCEDURE [dbo].[FG_GetCurrentPolls]
	@ApplicationID	uniqueidentifier,
	@CurrentUserID	uniqueidentifier,
	@Now			datetime,
	@DefaultPrivacy	varchar(50),
	@Count			int,
	@LowerBoundary	int
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @PollIDs KeyLessGuidTableType

	INSERT INTO @PollIDs (Value)
	SELECT P.PollID
	FROM [dbo].[FG_Polls] AS P
	WHERE P.ApplicationID = @ApplicationID AND
		P.IsCopyOfPollID IS NOT NULL AND P.OwnerID IS NULL AND P.Deleted = 0 AND 
		(P.BeginDate IS NOT NULL OR P.FinishDate IS NOT NULL) AND
		(P.BeginDate IS NULL OR P.BeginDate <= @Now) AND
		(P.FinishDate IS NULL OR P.FinishDate >= @Now)
	
	DECLARE	@PermissionTypes StringPairTableType
	
	INSERT INTO @PermissionTypes (FirstValue, SecondValue)
	VALUES (N'View', @DefaultPrivacy)

	SELECT TOP(ISNULL(@Count, 20)) X.ID, X.Done AS Value, X.RowNumber + X.RevRowNumber - 1 AS TotalCount
	FROM (
			SELECT	ROW_NUMBER() OVER(ORDER BY MAX(P.BeginDate) DESC, MAX(P.FinishDate) ASC) AS RowNumber,
					ROW_NUMBER() OVER(ORDER BY MAX(P.BeginDate) ASC, MAX(P.FinishDate) DESC) AS RevRowNumber,
					IDs.ID, 
					CAST((CASE WHEN COUNT(FI.InstanceID) > 0 THEN 1 ELSE 0 END) AS bit) AS Done
			FROM [dbo].[PRVC_FN_CheckAccess](@ApplicationID, @CurrentUserID, 
				@PollIDs, N'Poll', @Now, @PermissionTypes) AS IDs
				INNER JOIN [dbo].[FG_Polls] AS P
				ON P.ApplicationID = @ApplicationID AND P.PollID = IDs.ID
				LEFT JOIN [dbo].[FG_FormInstances] AS FI
				ON FI.ApplicationID = @ApplicationID AND FI.OwnerID = IDs.ID AND
					FI.DirectorID = @CurrentUserID AND FI.Deleted = 0
			GROUP BY IDs.ID
		) AS X
	WHERE X.RowNumber >= ISNULL(@LowerBoundary, 0)
	ORDER BY X.RowNumber ASC
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_IsPoll]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_IsPoll]
GO

CREATE PROCEDURE [dbo].[FG_IsPoll]
	@ApplicationID	uniqueidentifier,
	@strIDs			varchar(max),
	@delimiter		char
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT Ref.Value AS ID
	FROM [dbo].[GFN_StrToGuidTable](@strIDs, @delimiter) AS Ref
		INNER JOIN [dbo].[FG_Polls] AS P
		ON P.ApplicationID = @ApplicationID AND P.PollID = Ref.Value
END

GO


-- end of Polls