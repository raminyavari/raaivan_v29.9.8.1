USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_FormsListReport]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_FormsListReport]
GO

CREATE PROCEDURE [dbo].[FG_FormsListReport]
	@ApplicationID			uniqueidentifier,
	@CurrentUserID			uniqueidentifier,
    @FormID					uniqueidentifier,
    @LowerCreationDateLimit	datetime,
    @UpperCreationDateLimit	datetime,
    @FormFiltersTemp		FormFilterTableType readonly,
    @delimiter				char
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @FormFilters FormFilterTableType
	INSERT INTO @FormFilters SELECT * FROM @FormFiltersTemp
	
	DECLARE @Results Table (
		InstanceID_Hide uniqueidentifier primary key clustered,
		CreationDate datetime,
		CreatorUserID_Hide uniqueidentifier,
		CreatorName nvarchar(1000),
		CreatorUserName nvarchar(1000)
	)
	
	INSERT INTO @Results (
		InstanceID_Hide, 
		CreationDate, 
		CreatorUserID_Hide, 
		CreatorName, 
		CreatorUserName
	)
	SELECT	FI.InstanceID, 
			FI.CreationDate,
			FI.CreatorUserID,
			LTRIM(RTRIM(ISNULL(UN.FirstName, N'') + N' ' + ISNULL(UN.LastName, N''))),
			UN.UserName
	FROM [dbo].[FG_FormInstances] AS FI
		LEFT JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND UN.UserID = FI.CreatorUserID
	WHERE FI.ApplicationID = @ApplicationID AND FI.FormID = @FormID AND
		(@LowerCreationDateLimit IS NULL OR FI.CreationDate >= @LowerCreationDateLimit) AND
		(@UpperCreationDateLimit IS NULL OR FI.CreationDate <= @UpperCreationDateLimit) AND
		FI.Deleted = 0
	
	DECLARE @InstanceIDs GuidTableType
		
	INSERT INTO @InstanceIDs
	SELECT Ref.InstanceID_Hide
	FROM @Results AS Ref
	
	IF (SELECT COUNT(Ref.ElementID) FROM @FormFilters AS Ref) > 0 BEGIN
		DELETE I
		FROM @InstanceIDs AS I
			LEFT JOIN [dbo].[FG_FN_FilterInstances](
				@ApplicationID, NULL, @InstanceIDs, @FormFilters, @delimiter, 1
			) AS Ref
			ON Ref.InstanceID = I.Value
		WHERE Ref.InstanceID IS NULL
	END
	
	SELECT R.*
	FROM @InstanceIDs AS I
		INNER JOIN @Results AS R
		ON R.InstanceID_Hide = I.Value
	ORDER BY R.CreationDate DESC
	
	SELECT ('{' +
			'"CreatorName": {"Action": "Link", "Type": "User",' +
				'"Requires": {"UID": "CreatorUserID_Hide"}' +
			'}' +
		   '}') AS Actions

	
	-- Second Part: Describes the Third Part
	SELECT CAST(EFE.ElementID AS varchar(50)) AS ColumnName, EFE.Title AS Translation,
		CASE
			WHEN EFE.[Type] = N'Binary' THEN N'bool'
			WHEN EFE.[Type] = N'Number' THEN N'double'
			WHEN EFE.[Type] = N'Date' THEN N'datetime'
			WHEN EFE.[Type] = N'User' THEN N'user'
			WHEN EFE.[Type] = N'Node' THEN N'node'
			ELSE N'string'
		END AS [Type]
	FROM [dbo].[FG_ExtendedFormElements] AS EFE
	WHERE EFE.ApplicationID = @ApplicationID AND 
		EFE.FormID = @FormID AND EFE.Deleted = 0
	ORDER BY EFE.SequenceNumber ASC
	
	SELECT ('{"IsDescription": "true"}') AS Info
	-- end of Second Part
	
	-- Third Part: The Form Info
	DECLARE @ElementIDs GuidTableType
	DECLARE @FakeOwnerIDs GuidTableType, @FakeFilters FormFilterTableType
	
	EXEC [dbo].[FG_P_GetFormRecords] @ApplicationID, @FormID, @ElementIDs, 
		@InstanceIDs, @FakeOwnerIDs, @FakeFilters, NULL, 1000000, NULL, NULL
	
	SELECT ('{' +
		'"ColumnsMap": "InstanceID_Hide:InstanceID",' +
		'"ColumnsToTransfer": "' + STUFF((
			SELECT ',' + CAST(EFE.ElementID AS varchar(50))
			FROM [dbo].[FG_ExtendedFormElements] AS EFE
			WHERE EFE.ApplicationID = @ApplicationID AND 
				EFE.FormID = @FormID AND EFE.Deleted = 0
			ORDER BY EFE.SequenceNumber ASC
			FOR xml path('a'), type
		).value('.','nvarchar(max)'), 1, 1, '') + '"' +
	   '}') AS Info
	-- End of Third Part
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_PollDetailReport]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_PollDetailReport]
GO

CREATE PROCEDURE [dbo].[FG_PollDetailReport]
	@ApplicationID	uniqueidentifier,
	@CurrentUserID	uniqueidentifier,
    @PollID			uniqueidentifier,
    @NodeTypeID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT	A.Title, 
			A.UserID AS UserID_Hide,
			LTRIM(RTRIM(ISNULL(A.FirstName, N'') + N' ' + ISNULL(A.LastName, N''))) AS FullName, 
			A.UserName, 
			A.MembershipNodeID AS NodeID_Hide,
			ND.Name AS NodeName,
			A.Value,
			A.NumberValue
	FROM (
			SELECT	MAX(X.Seq) AS Seq,
					X.UserID,
					MAX(UN.FirstName) AS FirstName, 
					MAX(UN.LastName) AS LastName, 
					MAX(UN.UserName) AS UserName,
					MAX(X.Title) AS Title,
					MAX(X.Value) AS Value,
					MAX(X.FloatValue) AS NumberValue,
					CAST(MAX(CAST(NM.NodeID AS varchar(50))) AS uniqueidentifier) AS MembershipNodeID
			FROM (
					SELECT	I.CreatorUserID AS UserID, 
							FE.ElementID AS ElementID,
							FE.Title,
							FE.SequenceNumber AS Seq,
							[dbo].[FG_FN_ToString](@ApplicationID, E.ElementID, E.[Type], 
								E.TextValue, E.FloatValue, E.BitValue, E.DateValue) AS Value,
							E.FloatValue
					FROM [dbo].[FG_FormInstances] AS I
						INNER JOIN [dbo].[FG_InstanceElements] AS E
						ON E.ApplicationID = @ApplicationID AND E.InstanceID = I.InstanceID AND E.Deleted = 0
						INNER JOIN [dbo].[FG_ExtendedFormElements] AS FE
						ON FE.ApplicationID = @ApplicationID AND FE.ElementID = E.RefElementID AND FE.Deleted = 0
					WHERE I.ApplicationID = @ApplicationID AND I.OwnerID = @PollID AND I.Deleted = 0
				) AS X
				INNER JOIN [dbo].[Users_Normal] AS UN
				ON UN.ApplicationID = @ApplicationID AND UN.UserID = X.UserID
				LEFT JOIN [dbo].[CN_View_NodeMembers] AS NM
				ON @NodeTypeID IS NOT NULL AND NM.ApplicationID = @ApplicationID AND
					NM.NodeTypeID = @NodeTypeID AND NM.UserID = UN.UserID AND NM.IsPending = 0
			WHERE ISNULL(X.Value, N'') <> N''
			GROUP BY X.ElementID, X.UserID
		) AS A
		LEFT JOIN [dbo].[CN_Nodes] AS ND
		ON @NodeTypeID IS NOT NULL AND ND.ApplicationID = @ApplicationID AND ND.NodeID = A.MembershipNodeID
	ORDER BY A.FirstName ASC, A.LastName ASC, A.Seq ASC
	
	SELECT ('{' +
			'"FullName": {"Action": "Link", "Type": "User",' +
				'"Requires": {"UID": "UserID_Hide"}' +
			'},' +
			'"NodeName": {"Action": "Link", "Type": "Node",' +
				'"Requires": {"ID": "NodeID_Hide"}' +
			'}' +
		   '}') AS Actions
END

GO