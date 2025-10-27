USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FG_FN_CheckElementValue]') 
    AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[FG_FN_CheckElementValue]
GO

CREATE FUNCTION [dbo].[FG_FN_CheckElementValue](
	@Type varchar(20),
	@RefText nvarchar(2000), 
	@RefFloat float, 
	@RefBit bit,
	@RefDate datetime,
	@Text nvarchar(max),
	@TextItems StringTableType readonly,
	@Or bit,
	@Exact bit,
	@DateFrom datetime,
	@DateTo datetime,
	@FloatFrom float, 
	@FloatTo float, 
	@Bit bit,
	@NoTextItem bit
)
RETURNS float
WITH ENCRYPTION
AS
BEGIN
	IF @Type = N'Text' OR @Type = N'File' BEGIN
		RETURN 
			CASE 
				WHEN @NoTextItem = 1 THEN (CASE WHEN ISNULL(@RefText, N'') = N'' THEN 1 ELSE 0 END)
				WHEN [dbo].[GFN_LikeMatch](@RefText, @TextItems, @Or, @Exact) = 1 THEN 1 
				ELSE 0 
			END
	END
	ELSE IF @Type = N'MultiLevel' BEGIN
		RETURN CASE 
				WHEN ISNULL(@NoTextItem, 0) = 0 AND EXISTS(SELECT TOP(1) * FROM @TextItems AS T WHERE T.Value = @RefText) THEN 1 
				ELSE 0 
			END
	END
	ELSE IF @Type = N'Checkbox' OR @Type = N'Select' BEGIN
		IF @Type = N'Select' SET @Or = 1
	
		RETURN
			CASE
				WHEN @NoTextItem = 1 THEN (CASE WHEN ISNULL(@RefText, N'') = N'' THEN 1 ELSE 0 END)
				WHEN ISNULL((
					SELECT SUM(CASE WHEN [dbo].[GFN_LikeMatch](LTRIM(RTRIM(ISNULL(Ref.Value, N''))), @TextItems, @Or, @Exact) = 1 THEN 1 ELSE 0 END)
					FROM [dbo].[GFN_StrToStringTable](ISNULL(@RefText, N''), N'~') AS Ref
				), 0) > 0 THEN 1
				ELSE 0
			END
	END
	ELSE IF @Type = N'Date' BEGIN
		RETURN 
			CASE 
				WHEN (@DateFrom IS NULL OR @RefDate >= @DateFrom) AND 
					(@DateTo IS NULL OR @RefDate <= @DateTo) THEN 1 
				ELSE 0 
			END
	END
	ELSE IF @Type = N'Binary' BEGIN
		RETURN 
			CASE 
				WHEN @Bit IS NULL THEN (CASE WHEN @RefBit IS NULL THEN 1 ELSE 0 END)
				WHEN @RefBit LIKE @Bit THEN 1 
				ELSE 0 
			END
	END
	ELSE IF @Type = N'Numeric' BEGIN
		RETURN 
			CASE 
				WHEN (@FloatFrom IS NULL OR @RefFloat >= @FloatFrom) AND 
					(@FloatTo IS NULL OR @RefFloat <= @FloatTo) THEN 1 
				ELSE 0 
			END
	END
	
	RETURN 0
END

GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FG_FN_FilterInstances]') 
    AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[FG_FN_FilterInstances]
GO

CREATE FUNCTION [dbo].[FG_FN_FilterInstances](
	@ApplicationID	uniqueidentifier,
	@OwnerElementID	uniqueidentifier,
	@InstanceIDs	GuidTableType readonly,
	@FormFilters	FormFilterTableType readonly,
	@delimiter		char,
	@MatchAll		bit
)
RETURNS @Values Table(InstanceID uniqueidentifier primary key clustered, [Rank] float)
WITH ENCRYPTION
AS
BEGIN
	DECLARE @ElementID uniqueidentifier
	DECLARE @Type varchar(20)
	DECLARE @Text nvarchar(max)
	DECLARE @strTextItems nvarchar(max)
	DECLARE @Or bit
	DECLARE @Exact bit
	DECLARE @DateFrom datetime
	DECLARE @DateTo datetime
	DECLARE @FloatFrom float
	DECLARE @FloatTo float
	DECLARE @Bit bit
	DECLARE @strGuidItems varchar(max)
	DECLARE @Compulsory bit
	
	DECLARE @Fltrs Table(ID int IDENTITY(1,1), ElementID uniqueidentifier, [Type] varchar(20), Compulsory bit)
	
	INSERT INTO @Fltrs (ElementID, [Type], Compulsory)
	SELECT Ref.ElementID, E.[Type], Ref.Compulsory
	FROM @FormFilters AS Ref
		INNER JOIN [dbo].[FG_ExtendedFormElements] AS E
		ON E.ApplicationID = @ApplicationID AND E.ElementID = Ref.ElementID
	WHERE (Ref.OwnerID IS NULL AND @OwnerElementID IS NULL) OR Ref.OwnerID = @OwnerElementID
	
	DECLARE @Iter int = (SELECT COUNT(*) FROM @Fltrs)
	DECLARE @Count int = @Iter
	DECLARE @CompulsoryCount int = (SELECT COUNT(F.ID) FROM @Fltrs AS F WHERE ISNULL(F.Compulsory, 0) = 1)
	
	DECLARE @RetInstIDs TABLE (
		InstanceID uniqueidentifier primary key clustered,
		[Rank] float,
		MatchCount int,
		CompulsoryCount int
	)
	
	INSERT INTO @RetInstIDs (InstanceID, [Rank], MatchCount, CompulsoryCount)
	SELECT Ref.Value, 0, 0, 0
	FROM @InstanceIDs AS Ref
	
	WHILE @Iter > 0 BEGIN
		SELECT	@ElementID = FF.ElementID,
				@Type = F.[Type],
				@Text = FF.[Text],
				@strTextItems = FF.TextItems,
				@Or = FF.[Or],
				@Exact = FF.Exact,
				@DateFrom = FF.DateFrom,
				@DateTo = FF.DateTo,
				@FloatFrom = FF.FloatFrom,
				@FloatTo = FF.FloatTo,
				@Bit = FF.[Bit],
				@strGuidItems = FF.GuidItems,
				@Compulsory = F.Compulsory
		FROM @Fltrs AS F
			INNER JOIN @FormFilters AS FF
			ON F.ElementID = FF.ElementID
		WHERE F.ID = @Iter
		
		IF @Text = N'' SET @Text = NULL
		
		DECLARE @TextItems StringTableType
		DECLARE @GuidItems GuidTableType
		DECLARE @NoTextItem bit
		DECLARE @NoGuidItem bit
		
		DELETE @TextItems
		DELETE @GuidItems
		
		SET @NoTextItem = 0
		SET @NoGuidItem = 0
		
		INSERT INTO @TextItems
		SELECT Ref.Value
		FROM [dbo].[GFN_StrToStringTable](@strTextItems, @delimiter) AS Ref
		
		IF (SELECT COUNT(*) FROM @TextItems) = 0 SET @NoTextItem = 1
		
		INSERT INTO @GuidItems
		SELECT Ref.Value 
		FROM [dbo].[GFN_StrToGuidTable](@strGuidItems, @delimiter) AS Ref
		
		IF (SELECT COUNT(*) FROM @GuidItems) = 0 SET @NoGuidItem = 1
		
		IF @Type = N'Form' BEGIN
			DECLARE @InstElems TABLE (ElementID uniqueidentifier, InstanceID uniqueidentifier, Score float)
			DECLARE @OwnerIDs GuidTableType
			
			INSERT INTO @InstElems (ElementID, InstanceID)
			SELECT IE.ElementID, Ref.InstanceID
			FROM @RetInstIDs AS Ref
				INNER JOIN [dbo].[FG_InstanceElements] AS IE
				ON IE.ApplicationID = @ApplicationID AND IE.InstanceID = Ref.InstanceID
			WHERE IE.RefElementID = @ElementID AND IE.Deleted = 0
			
			INSERT INTO @OwnerIDs (Value)
			SELECT DISTINCT I.ElementID
			FROM @InstElems AS I
			
			UPDATE @InstElems
				SET Score = Ref.[Rank]
			FROM @InstElems AS I
				INNER JOIN [dbo].[FG_FN_FilterInstanceOwners](@ApplicationID, 
					@ElementID, @OwnerIDs, @FormFilters, @MatchAll) AS Ref
				ON Ref.OwnerID = I.ElementID
				
			DECLARE @MaxScore float = (SELECT MAX(Score) FROM @InstElems)
			IF @MaxScore IS NULL OR @MaxScore <= 0 SET @MaxScore = 1
			
			UPDATE @RetInstIDs
				SET [Rank] = [Rank] + (I.Score / @MaxScore),
					MatchCount = MatchCount + 1,
					CompulsoryCount = CompulsoryCount + CAST(ISNULL(@Compulsory, 0) AS int)
			FROM @RetInstIDs AS R
				INNER JOIN @InstElems AS I
				ON I.InstanceID = R.InstanceID
			WHERE ISNULL(I.Score, 0) > 0
		END
		ELSE IF @Type = N'File' BEGIN
			UPDATE @RetInstIDs
				SET [Rank] = [Rank] + X.Score,
					MatchCount = MatchCount + 1,
					CompulsoryCount = CompulsoryCount + CAST(ISNULL(@Compulsory, 0) AS int)
			FROM @RetInstIDs AS Ref
				INNER JOIN (
					SELECT	Ref.InstanceID,
							[dbo].[FG_FN_CheckElementValue](
								IE.[Type], F.[FileName], NULL, NULL, NULL, 
								@Text, @TextItems, @Or, @Exact, @DateFrom, @DateTo, 
								@FloatFrom, @FloatTo, @Bit, @NoTextItem
							) AS Score
					FROM @RetInstIDs AS Ref
						INNER JOIN [dbo].[FG_InstanceElements] AS IE
						ON IE.ApplicationID = @ApplicationID AND IE.InstanceID = Ref.InstanceID
						INNER JOIN [dbo].[DCT_Files] AS F
						ON F.ApplicationID = @ApplicationID AND F.OwnerID = IE.ElementID
					WHERE IE.RefElementID = @ElementID AND IE.Deleted = 0
				) AS X
				ON X.InstanceID = Ref.InstanceID
			WHERE X.Score > 0			
		END
		ELSE IF @Type = N'Node' OR @Type = N'User' BEGIN
			UPDATE @RetInstIDs
				SET [Rank] = [Rank] + X.Score,
					MatchCount = MatchCount + 1,
					CompulsoryCount = CompulsoryCount + CAST(ISNULL(@Compulsory, 0) AS int)
			FROM @RetInstIDs AS Ref
				INNER JOIN (
					SELECT	Ref.InstanceID,
							CAST(COUNT(G.Value) AS float) AS Score
					FROM @RetInstIDs AS Ref
						INNER JOIN [dbo].[FG_InstanceElements] AS IE
						ON IE.ApplicationID = @ApplicationID AND IE.InstanceID = Ref.InstanceID
						INNER JOIN [dbo].[FG_SelectedItems] AS S
						ON S.ApplicationID = @ApplicationID AND 
							S.ElementID = IE.ElementID AND S.Deleted = 0
						INNER JOIN @GuidItems AS G
						ON G.Value = S.SelectedID
					WHERE IE.RefElementID = @ElementID AND IE.Deleted = 0
					GROUP BY Ref.InstanceID
				) AS X
				ON X.InstanceID = Ref.InstanceID
			WHERE @NoGuidItem = 0 AND X.Score > 0
		END
		ELSE BEGIN
			UPDATE @RetInstIDs
				SET [Rank] = [Rank] + X.Score,
					MatchCount = MatchCount + 1,
					CompulsoryCount = CompulsoryCount + CAST(ISNULL(@Compulsory, 0) AS int)
			FROM @RetInstIDs AS Ref
				INNER JOIN (
					SELECT	Ref.InstanceID,
							[dbo].[FG_FN_CheckElementValue](
								ISNULL(IE.[Type], @Type), IE.TextValue, IE.FloatValue, IE.BitValue, IE.DateValue, 
								@Text, @TextItems, @Or, @Exact, @DateFrom, @DateTo, 
								@FloatFrom, @FloatTo, @Bit, @NoTextItem
							) AS Score
					FROM @RetInstIDs AS Ref
						LEFT JOIN [dbo].[FG_InstanceElements] AS IE
						ON IE.ApplicationID = @ApplicationID AND IE.InstanceID = Ref.InstanceID AND
							IE.RefElementID = @ElementID AND IE.Deleted = 0
				) AS X
				ON X.InstanceID = Ref.InstanceID
			WHERE X.Score > 0
		END
		
		SET @Iter = @Iter - 1
	END
	
	INSERT INTO @Values (InstanceID, [Rank])
	SELECT Ref.InstanceID, ref.[Rank]
	FROM @RetInstIDs AS Ref
	WHERE Ref.[Rank] > 0 AND (ISNULL(@MatchAll, 0) = 0 OR Ref.MatchCount = @Count) AND 
		Ref.CompulsoryCount = @CompulsoryCount AND (
			CASE
				WHEN @Count > @CompulsoryCount THEN 
					(CASE WHEN Ref.MatchCount > Ref.CompulsoryCount THEN 1 ELSE 0 END)
				ELSE 1
			END
		) = 1
	
	RETURN
END

GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FG_FN_FilterInstanceOwners]') 
    AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[FG_FN_FilterInstanceOwners]
GO

CREATE FUNCTION [dbo].[FG_FN_FilterInstanceOwners](
	@ApplicationID	uniqueidentifier,
	@OwnerElementID	uniqueidentifier, -- OwnerElement is a FormElement of type 'Form'
	@OwnerIDs		GuidTableType readonly,
	@FormFilters	FormFilterTableType readonly,
	@MatchAll		bit
)
RETURNS @Values Table(
	OwnerID uniqueidentifier,
	[Rank] float
)
WITH ENCRYPTION
AS
BEGIN
	DECLARE @FormInstanceOwners Table (InstanceID uniqueidentifier, OwnerID uniqueidentifier)
	DECLARE @InstanceIDs GuidTableType
	
	INSERT INTO @FormInstanceOwners (InstanceID, OwnerID)
	SELECT ISNULL(X.InstanceID, [dbo].[GFN_NewGuid]()), Ref.Value
	FROM @OwnerIDs AS Ref 
		LEFT JOIN [dbo].[FG_FN_GetOwnerFormInstanceIDs](@ApplicationID, @OwnerIDs, NULL, NULL, NULL) AS X
		ON X.OwnerID = Ref.Value
	
	INSERT INTO @InstanceIDs (Value)
	SELECT DISTINCT Ref.InstanceID
	FROM @FormInstanceOwners AS Ref
	
	INSERT INTO @Values (OwnerID, [Rank])
	SELECT O.OwnerID, SUM(Ref.[Rank])
	FROM @FormInstanceOwners AS O
		INNER JOIN [dbo].[FG_FN_FilterInstances](
			@ApplicationID, @OwnerElementID, @InstanceIDs, @FormFilters, N',', @MatchAll
		) AS Ref
		ON Ref.InstanceID = O.InstanceID
	GROUP BY O.OwnerID
	
	RETURN
END

GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FG_FN_ToString]') 
    AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[FG_FN_ToString]
GO

CREATE FUNCTION [dbo].[FG_FN_ToString](
	@ApplicationID	uniqueidentifier,
	@ElementID		uniqueidentifier,
	@Type			varchar(50),
	@TextValue		nvarchar(max),
	@FloatValue		float,
	@BitValue		bit,
	@DateValue		datetime
)
RETURNS nvarchar(max)
WITH ENCRYPTION
AS
BEGIN
	IF @Type = N'File' BEGIN
		RETURN CAST([dbo].[DCT_FN_FilesCount](@ApplicationID, @ElementID) AS nvarchar(max))
	END
	ELSE IF @Type = N'Form' BEGIN
		RETURN CAST(ISNULL((
			SELECT COUNT(FI.InstanceID)
			FROM [dbo].[FG_FormInstances] AS FI
			WHERE FI.ApplicationID = @ApplicationID AND FI.OwnerID = @ElementID AND FI.Deleted = 0
		), 0) AS nvarchar(max))
	END
	ELSE IF @Type = N'Node' OR @Type = N'User' BEGIN
		RETURN @TextValue
		
		/*
		RETURN CAST(ISNULL((
			SELECT COUNT(S.SelectedID)
			FROM [dbo].[FG_SelectedItems] AS S
			WHERE S.ApplicationID = @ApplicationID AND S.ElementID = @ElementID AND S.Deleted = 0
		), 0) AS nvarchar(max))
		*/
	END
	ELSE IF @TextValue IS NOT NULL RETURN @TextValue
	ELSE IF @FloatValue IS NOT NULL RETURN CAST(@FloatValue AS nvarchar(max))
	ELSE IF @BitValue IS NOT NULL RETURN CAST(@BitValue AS nvarchar(max))
	ELSE IF @DateValue IS NOT NULL RETURN CAST(@DateValue AS nvarchar(max))
	
	RETURN CAST(N'' AS varchar(max))
END

GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FG_FN_IsFillable]') 
    AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[FG_FN_IsFillable]
GO

CREATE FUNCTION [dbo].[FG_FN_IsFillable](
	@Type varchar(50)
)
RETURNS bit
WITH ENCRYPTION
AS
BEGIN
	IF @Type = N'Separator' RETURN 0
	ELSE RETURN 1
	
	RETURN 0
END

GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FG_FN_HasElementLimit]') 
    AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[FG_FN_HasElementLimit]
GO

CREATE FUNCTION [dbo].[FG_FN_HasElementLimit](
	@ApplicationID	uniqueidentifier,
	@OwnerID		uniqueidentifier
)
RETURNS bit
WITH ENCRYPTION
AS
BEGIN
	DECLARE @Ret bit = (
		SELECT TOP(1) CAST(1 AS bit)
		FROM [dbo].[FG_ElementLimits] AS L
			INNER JOIN [dbo].[FG_FormOwners] AS FO
			ON FO.ApplicationID = @ApplicationID AND Fo.OwnerID = @OwnerID
			INNER JOIN [dbo].[FG_ExtendedFormElements] AS E
			ON E.ApplicationID = @ApplicationID AND 
				E.ElementID = L.ElementID AND E.Deleted = 0
		WHERE L.ApplicationID = @ApplicationID AND L.OwnerID = @OwnerID AND L.Deleted = 0
	)
	
	RETURN ISNULL(@Ret, 0)
END

GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FG_FN_GetLimitedElements]') 
    AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[FG_FN_GetLimitedElements]
GO

CREATE FUNCTION [dbo].[FG_FN_GetLimitedElements](
	@ApplicationID	uniqueidentifier,
	@OwnerID		uniqueidentifier
)
RETURNS @Values TABLE (ElementID uniqueidentifier)
WITH ENCRYPTION
AS
BEGIN
	DECLARE @HasLimits bit = [dbo].[FG_FN_HasElementLimit](@ApplicationID, @OwnerID)
	
	IF @HasLimits = 1 BEGIN
		INSERT INTO @Values (ElementID)
		SELECT L.ElementID
		FROM [dbo].[FG_ElementLimits] AS L
			INNER JOIN [dbo].[FG_FormOwners] AS FO
			ON FO.ApplicationID = @ApplicationID AND FO.OwnerID = @OwnerID AND FO.Deleted = 0
			INNER JOIN [dbo].[FG_ExtendedFormElements] AS E
			ON E.ApplicationID = @ApplicationID AND E.ElementID = L.ElementID AND E.Deleted = 0
		WHERE L.ApplicationID = @ApplicationID AND L.OwnerID = @OwnerID AND L.Deleted = 0
	END
	ELSE BEGIN
		INSERT INTO @Values (ElementID)
		SELECT E.ElementID
		FROM [dbo].[FG_FormOwners] AS FO
			INNER JOIN [dbo].[FG_ExtendedFormElements] AS E
			ON E.ApplicationID = @ApplicationID AND E.FormID = FO.FormID AND E.Deleted = 0
		WHERE FO.ApplicationID = @ApplicationID AND FO.OwnerID = @OwnerID AND FO.Deleted = 0
	END
	
	RETURN
END

GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FG_FN_HasFormContent]') 
    AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[FG_FN_HasFormContent]
GO

CREATE FUNCTION [dbo].[FG_FN_HasFormContent](
	@ApplicationID	uniqueidentifier,
	@OwnerID		uniqueidentifier
)
RETURNS bit
WITH ENCRYPTION
AS
BEGIN
	RETURN CAST((
		SELECT TOP(1) 1
		FROM [dbo].[FG_FormInstances] AS I
			INNER JOIN [dbo].[FG_InstanceElements] AS E
			ON E.ApplicationID = @ApplicationID AND E.InstanceID = I.InstanceID
		WHERE I.ApplicationID = @ApplicationID AND I.OwnerID = @OwnerID AND 
			I.Deleted = 0 AND E.Deleted = 0 AND (
				ISNULL([dbo].[FG_FN_ToString](@ApplicationID, E.ElementID, E.[Type], 
					E.TextValue, E.FloatValue, E.BitValue, E.DateValue), N'') <> N''
			)
	) AS bit)
END

GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FG_FN_GetOwnerFormContents]') 
    AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[FG_FN_GetOwnerFormContents]
GO

CREATE FUNCTION [dbo].[FG_FN_GetOwnerFormContents](
	@ApplicationID	uniqueidentifier,
	@OwnerID		uniqueidentifier,
	@MaxLevel		int
)
RETURNS nvarchar(max)
WITH ENCRYPTION
AS
BEGIN
	SET @MaxLevel = ISNULL(@MaxLevel, 0)

	IF @MaxLevel <= 0 RETURN CAST(N'' AS varchar(max))

	DECLARE @Ret nvarchar(max) = NULL

	DECLARE @Exceptions StringTableType

	;WITH Partitioned AS
	(
		SELECT	OwnerID,
				CASE 
					WHEN IE.[Type] = N'Form' 
						THEN [dbo].[FG_FN_GetOwnerFormContents](@ApplicationID, IE.ElementID, @MaxLevel - 1)
					ELSE [dbo].[FG_FN_ToString](@ApplicationID, IE.ElementID, IE.[Type], 
						IE.TextValue, IE.FloatValue, IE.BitValue, IE.DateValue)
				END AS Content,
				ROW_NUMBER() OVER (PARTITION BY @OwnerID ORDER BY IE.ElementID) AS Number,
				COUNT(*) OVER (PARTITION BY @OwnerID) AS [Count]
		FROM [dbo].[FG_FormInstances] AS FI
			INNER JOIN [dbo].[FG_InstanceElements] AS IE
			ON IE.ApplicationID = @ApplicationID AND IE.InstanceID = FI.InstanceID AND 
				IE.[Type] <> N'File' AND IE.Deleted = 0
		WHERE FI.ApplicationID = @ApplicationID AND FI.OwnerID = @OwnerID AND FI.Deleted = 0
	),
	Fetched AS
	(
		SELECT	P.OwnerID, P.Content AS FullContent, P.Content, P.Number, P.[Count] 
		FROM Partitioned AS P
		WHERE P.Number = 1

		UNION ALL

		SELECT	P.OwnerID, ISNULL(C.FullContent, N'') + ' ' + ISNULL(P.Content, N''), P.Content, P.Number, P.[Count]
		FROM Partitioned AS P
			INNER JOIN Fetched AS C 
			ON P.OwnerID = C.OwnerID AND P.Number = C.Number + 1
		WHERE P.Number <= 95
	)
	SELECT TOP(1) @Ret = F.FullContent
	FROM Fetched AS F
	WHERE F.Number = (CASE WHEN F.[Count] > 90 THEN 90 ELSE F.[Count] END)
	
	RETURN @Ret
END

GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FG_FN_CheckUniqueConstraint]') 
    AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[FG_FN_CheckUniqueConstraint]
GO

CREATE FUNCTION [dbo].[FG_FN_CheckUniqueConstraint](
	@ApplicationID	uniqueidentifier,
	@Elements		FormElementTableType readonly
)
RETURNS @OutputTable TABLE (
	ElementID uniqueidentifier,
	RefElementID uniqueidentifier
)
WITH ENCRYPTION
AS
BEGIN
	DECLARE @OwnerIDs GuidTableType
	
	INSERT INTO @OwnerIDs (Value)
	SELECT DISTINCT I.OwnerID
	FROM @Elements AS E
		INNER JOIN [dbo].[FG_FormInstances] AS I
		ON I.ApplicationID = @ApplicationID AND I.InstanceID = E.InstanceID

	INSERT INTO @OutputTable (ElementID, RefElementID)
	SELECT DISTINCT Ref.ElementID, Ref.RefElementID
	FROM @Elements AS Ref
		INNER JOIN [dbo].[FG_ExtendedFormElements] AS E
		ON E.ApplicationID = @ApplicationID AND 
			(E.ElementID = Ref.ElementID OR E.ElementID = Ref.RefElementID) AND 
			E.UniqueValue = 1 AND (E.[Type] = N'Text' OR E.[Type] = N'Numeric')
		INNER JOIN [dbo].[FG_InstanceElements] AS IE
		ON IE.ApplicationID = @ApplicationID AND IE.RefElementID = E.ElementID AND
			(Ref.ElementID IS NULL OR IE.ElementID <> Ref.ElementID) AND
			(
				CASE 
					WHEN E.[Type] = N'Text' AND ISNULL(Ref.TextValue, N'') <> N'' AND 
						ISNULL(Ref.TextValue, N'') = ISNULL(IE.TextValue, N'') THEN 1
					WHEN E.[Type] = N'Numeric' AND Ref.FloatValue IS NOT NULL AND 
						IE.FloatValue IS NOT NULL AND Ref.FloatValue = IE.FloatValue THEN 1
					ELSE 0
				END
			) = 1
		INNER JOIN (
			SELECT FI.InstanceID
			FROM [dbo].[FG_FormInstances] AS FI
				INNER JOIN (
					SELECT ND.NodeID AS ID
					FROM @OwnerIDs AS IDs 
						INNER JOIN [dbo].[CN_NodeTypes] AS NT
						ON NT.ApplicationID = @ApplicationID AND NT.NodeTypeID = IDs.Value
						INNER JOIN [dbo].[CN_Nodes] AS ND
						ON ND.ApplicationID = @ApplicationID AND 
							ND.NodeTypeID = NT.NodeTypeID AND ND.Deleted = 0

					UNION

					SELECT ND.NodeID
					FROM @OwnerIDs AS IDs
						INNER JOIN [dbo].[CN_Nodes] AS NT
						ON NT.ApplicationID = @ApplicationID AND NT.NodeID = IDs.Value
						INNER JOIN [dbo].[CN_Nodes] AS ND
						ON ND.ApplicationID = @ApplicationID AND 
							ND.NodeTypeID = NT.NodeTypeID AND ND.Deleted = 0

					UNION

					SELECT IDs.Value
					FROM @OwnerIDs AS IDs
				) AS Owners
				ON FI.OwnerID = Owners.ID
			WHERE FI.ApplicationID = @ApplicationID
		) AS X
		ON X.InstanceID = IE.InstanceID
	
	RETURN
END

GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FG_FN_GetOwnerFormInstanceIDs]') 
    AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[FG_FN_GetOwnerFormInstanceIDs]
GO

CREATE FUNCTION [dbo].[FG_FN_GetOwnerFormInstanceIDs](
	@ApplicationID	uniqueidentifier,
	@OwnerIDs		GuidTableType readonly,
	@FormID			uniqueidentifier,
	@IsTemporary	bit,
	@CreatorUserID	uniqueidentifier
)
RETURNS @OutputTable TABLE (
	OwnerID uniqueidentifier,
	InstanceID uniqueidentifier
)
WITH ENCRYPTION
AS
BEGIN
	INSERT INTO @OutputTable (OwnerID, InstanceID)
	SELECT FI.OwnerID, FI.InstanceID
	FROM (
			SELECT X.OwnerID, X.FormID
			FROM (
					SELECT	IDs.Value AS OwnerID,
							CAST((CASE WHEN MAX(ND.Name) IS NULL THEN 0 ELSE 1 END) AS bit) AS IsNode,
							ISNULL(@FormID, CAST(MAX(CAST(F.FormID AS varchar(50))) AS uniqueidentifier)) AS FormID,
							CAST((CASE WHEN @FormID IS NULL AND MAX(E.Extension) IS NULL THEN 0 ELSE 1 END) AS bit) AS HasForm
					FROM @OwnerIDs AS IDs
						LEFT JOIN [dbo].[CN_Nodes] AS ND
						ON @FormID IS NULL AND ND.ApplicationID = @ApplicationID AND ND.NodeID = IDs.Value
						LEFT JOIN [dbo].[FG_FormOwners] AS F
						ON @FormID IS NULL AND F.ApplicationID = @ApplicationID AND 
							F.OwnerID = ND.NodeTypeID AND F.Deleted = 0
						LEFT JOIN [dbo].[CN_Extensions] AS E
						ON @FormID IS NULL AND E.ApplicationID = @ApplicationID AND 
							E.OwnerID = ND.NodeTypeID AND E.Extension = N'Form' AND E.Deleted = 0
					GROUP BY IDs.Value
				) AS X
			WHERE (X.FormID IS NOT NULL AND X.HasForm = 1) OR X.IsNode = 0
		) AS ExternalIDs
		INNER JOIN [dbo].[FG_FormInstances] AS FI
		ON FI.ApplicationID = @ApplicationID AND FI.OwnerID = ExternalIDs.OwnerID AND
			(ExternalIDs.FormID IS NULL OR FI.FormID = ExternalIDs.FormID) AND FI.Deleted = 0 AND
			(@IsTemporary IS NULL OR ISNULL(FI.IsTemporary, 0) = @IsTemporary) AND
			(@CreatorUserID IS NULL OR FI.CreatorUserID = @CreatorUserID)
	
	RETURN
END

GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FG_FN_ValidateNewName]') 
    AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[FG_FN_ValidateNewName]
GO

CREATE FUNCTION [dbo].[FG_FN_ValidateNewName](
	@ApplicationID	uniqueidentifier,
	@ObjectID		uniqueidentifier,
	@FormID			uniqueidentifier,
	@Name			varchar(100)
)
RETURNS bit
WITH ENCRYPTION
AS
BEGIN
	SET @Name = LOWER(LTRIM(RTRIM(ISNULL(@Name, ''))))
	
	IF @Name = '' OR @ObjectID IS NULL RETURN 0
	
	DECLARE @Ret bit = 1

	IF @FormID IS NULL BEGIN
		SELECT TOP(1) @Ret = 0
		FROM [dbo].[FG_ExtendedForms] AS F
		WHERE F.ApplicationID = @ApplicationID AND F.Deleted = 0 AND
			F.FormID <> @ObjectID AND LOWER(F.Name) = @Name
	END
	ELSE BEGIN
		SELECT TOP(1) @Ret = 0
		FROM [dbo].[FG_ExtendedFormElements] AS E
		WHERE E.ApplicationID = @ApplicationID AND E.FormID = @FormID AND E.Deleted = 0 AND
			E.ElementID <> @ObjectID AND LOWER(E.Name) = @Name
	END
	
	RETURN ISNULL(@Ret, 0)
END

GO