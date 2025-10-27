USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[DE_UpdateNodes]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DE_UpdateNodes]
GO

CREATE PROCEDURE [dbo].[DE_UpdateNodes]
	@ApplicationID			uniqueidentifier,
	@NodeTypeID				uniqueidentifier,
	@NodeTypeAdditionalID	varchar(50),
    @NodesTemp				ExchangeNodeTableType readonly,
    @CreatorUserID			uniqueidentifier,
    @CreationDate			datetime
WITH ENCRYPTION, RECOMPILE
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	DECLARE @Nodes ExchangeNodeTableType
	INSERT INTO @Nodes SELECT * FROM @NodesTemp
	
	IF @NodeTypeID IS NULL 
		SET @NodeTypeID = [dbo].[CN_FN_GetNodeTypeID](@ApplicationID, @NodeTypeAdditionalID)
	
	DECLARE @NotExisting ExchangeNodeTableType
	
	INSERT INTO @NotExisting
	SELECT * 
	FROM @Nodes AS ExternalNodes
	WHERE NOT((ExternalNodes.NodeID IS NULL OR ISNULL(ExternalNodes.NodeAdditionalID, N'') = N'') AND
		(ExternalNodes.Name IS NULL OR ExternalNodes.Name = N'')) AND
		NOT EXISTS(
			SELECT TOP(1) * 
			FROM [dbo].[CN_Nodes] AS ND
			WHERE ND.ApplicationID = @ApplicationID AND ND.NodeTypeID = @NodeTypeID AND 
				ExternalNodes.NodeAdditionalID IS NOT NULL AND ND.AdditionalID = ExternalNodes.NodeAdditionalID
		)
		
	DECLARE @_Count int
	SET @_Count = (SELECT COUNT(*) FROM @NotExisting)
	
	IF @_Count > 0 BEGIN
		INSERT INTO [dbo].[CN_Nodes](
			ApplicationID,
			NodeID,
			NodeTypeID,
			AdditionalID,
			Name,
			[Description],
			Tags,
			CreatorUserID,
			CreationDate,
			Deleted
		)
		SELECT @ApplicationID, ISNULL(NE.NodeID, NEWID()), @NodeTypeID, NE.NodeAdditionalID, 
			[dbo].[GFN_VerifyString](NE.Name), [dbo].[GFN_VerifyString](NE.Abstract), 
			[dbo].[GFN_VerifyString](NE.Tags), @CreatorUserID, @CreationDate, 0
		FROM @NotExisting AS NE
		
		IF @@ROWCOUNT <= 0 BEGIN
			SELECT -1
			ROLLBACK TRANSACTION
			RETURN
		END
	END
	
	IF EXISTS(
		SELECT TOP(1) * 
		FROM @Nodes 
		WHERE ISNULL(NodeAdditionalID, N'') <> N'' AND ISNULL(Name, N'') <> N''
	) BEGIN
		UPDATE ND
			SET Name = [dbo].[GFN_VerifyString](ExternalNodes.Name),
				Tags = ISNULL([dbo].[GFN_VerifyString](ExternalNodes.Tags), ND.Tags),
				[Description] = ISNULL([dbo].[GFN_VerifyString](ExternalNodes.Abstract), ND.[Description])
		FROM @Nodes AS ExternalNodes
			INNER JOIN [dbo].[CN_Nodes] AS ND
			ON ND.[AdditionalID] = ExternalNodes.NodeAdditionalID
		WHERE ND.ApplicationID = @ApplicationID AND 
			ISNULL(ExternalNodes.NodeAdditionalID, N'') <> N'' AND
			ND.[NodeTypeID] = @NodeTypeID AND ISNULL(ExternalNodes.Name, N'') <> N''
			
		IF @@ROWCOUNT <= 0 BEGIN
			SELECT -1
			ROLLBACK TRANSACTION
			RETURN
		END
	END
	
	-- Update Sequence Number
	UPDATE ND
		SET SequenceNumber = X.RowNum
	FROM (
			SELECT	ROW_NUMBER() OVER (ORDER BY (SELECT 1) ASC) AS RowNum,
					N.*
			FROM @Nodes AS N
		) AS X
		INNER JOIN [dbo].[CN_Nodes] AS ND
		ON ND.ApplicationID = @ApplicationID AND ND.NodeTypeID = @NodeTypeID AND
			ISNULL(ND.AdditionalID, N'') <> N'' AND ND.AdditionalID = X.NodeAdditionalID
	-- end of Update Sequence Number
	
	DECLARE @HaveParent ExchangeNodeTableType
	INSERT INTO @HaveParent(NodeAdditionalID, ParentAdditionalID)
	SELECT ND.NodeAdditionalID, ND.ParentAdditionalID
	FROM @Nodes AS ND
	WHERE ISNULL(ND.NodeAdditionalID, N'') <> N'' AND ISNULL(ND.ParentAdditionalID, N'') <> ''
	
	IF EXISTS(SELECT TOP(1) * FROM @HaveParent) BEGIN
		UPDATE ND
			SET ParentNodeID = OT.NodeID,
				LastModifierUserID = @CreatorUserID,
				LastModificationDate = @CreationDate
		FROM @HaveParent AS ExternalNodes
			INNER JOIN [dbo].[CN_Nodes] AS ND
			ON ND.[AdditionalID] = ExternalNodes.NodeAdditionalID
			INNER JOIN [dbo].[CN_Nodes] AS OT
			ON OT.AdditionalID = ExternalNodes.ParentAdditionalID
		WHERE ND.ApplicationID = @ApplicationID AND OT.ApplicationID = @ApplicationID AND
			ND.[NodeTypeID] = @NodeTypeID AND OT.NodeTypeID = @NodeTypeID
		
		IF @@ROWCOUNT <= 0 BEGIN
			SELECT -1
			ROLLBACK TRANSACTION
			RETURN
		END
	END
	
	DECLARE @HaveNotParent ExchangeNodeTableType
	INSERT INTO @HaveNotParent(NodeAdditionalID, ParentAdditionalID)
	SELECT ND.NodeAdditionalID, ND.ParentAdditionalID
	FROM @Nodes AS ND
	WHERE ISNULL(ND.NodeAdditionalID, N'') <> N'' AND ISNULL(ND.ParentAdditionalID, N'') = ''
	
	IF EXISTS(SELECT TOP(1) * FROM @HaveNotParent) BEGIN
		UPDATE ND
			SET ParentNodeID = NULL,
				LastModifierUserID = @CreatorUserID,
				LastModificationDate = @CreationDate
		FROM @HaveNotParent AS ExternalNodes
			INNER JOIN [dbo].[CN_Nodes] AS ND
			ON ND.[AdditionalID] = ExternalNodes.NodeAdditionalID
		WHERE ND.ApplicationID = @ApplicationID AND ND.[NodeTypeID] = @NodeTypeID
		
		IF @@ROWCOUNT <= 0 BEGIN
			SELECT -1
			ROLLBACK TRANSACTION
			RETURN
		END
	END
	
	SELECT 1
	
	SELECT N.NodeID AS ID
	FROM @NotExisting AS N
	WHERE N.NodeID IS NOT NULL
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[DE_UpdateNodeIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DE_UpdateNodeIDs]
GO

CREATE PROCEDURE [dbo].[DE_UpdateNodeIDs]
	@ApplicationID	uniqueidentifier,
	@NodeTypeID		uniqueidentifier,
	@strNodeIDs		nvarchar(max),
	@innerDelimiter	char,
	@outerDelimiter	char,
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @Values TABLE (NodeID uniqueidentifier, NewAdditionalID nvarchar(200))
	
	INSERT INTO @Values (NodeID, NewAdditionalID)
	SELECT DISTINCT ND.NodeID, Ref.SecondValue
	FROM [dbo].[GFN_StrToStringPairTable](@strNodeIDs, @innerDelimiter, @outerDelimiter) AS Ref
		INNER JOIN [dbo].[CN_Nodes] AS ND
		ON ND.ApplicationID = @ApplicationID AND 
			ND.NodeTypeID = @NodeTypeID AND ND.AdditionalID = Ref.FirstValue
	
	UPDATE ND
		SET AdditionalID = X.NewAdditionalID,
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	FROM (
			SELECT V.*
			FROM @Values AS V
				LEFT JOIN [dbo].[CN_Nodes] AS N
				ON N.ApplicationID = @ApplicationID AND N.NodeTypeID = @NodeTypeID AND
					N.AdditionalID = V.NewAdditionalID AND N.NodeID <> V.NodeID
			WHERE ISNULL(V.NewAdditionalID, N'') <> N'' AND N.NodeID IS NULL
		) AS X
		INNER JOIN [dbo].[CN_Nodes] AS ND
		ON ND.ApplicationId = @ApplicationID AND ND.NodeID = X.NodeID
		
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[DE_RemoveNodes]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DE_RemoveNodes]
GO

CREATE PROCEDURE [dbo].[DE_RemoveNodes]
	@ApplicationID	uniqueidentifier,
	@strNodeIDs		nvarchar(max),
	@innerDelimiter	char,
	@outerDelimiter	char,
    @CurrentUserID	uniqueidentifier,
    @Now			datetime
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @NodeIDs GuidTableType
	
	INSERT INTO @NodeIDs(Value)
	SELECT DISTINCT ND.NodeID
	FROM [dbo].[GFN_StrToStringPairTable](@strNodeIDs, @innerDelimiter, @outerDelimiter) AS Ref
		INNER JOIN [dbo].[CN_View_Nodes_Normal] AS ND
		ON ND.ApplicationID = @ApplicationID AND 
			ND.TypeAdditionalID = Ref.FirstValue AND ND.NodeAdditionalID = Ref.SecondValue
	
	UPDATE ND
		SET Deleted = 1,
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	FROM @NodeIDs AS X
		INNER JOIN [dbo].[CN_Nodes] AS ND
		ON ND.ApplicationId = @ApplicationID AND ND.NodeID = X.Value
		
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[DE_UpdateUsers]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DE_UpdateUsers]
GO

CREATE PROCEDURE [dbo].[DE_UpdateUsers]
	@ApplicationID	uniqueidentifier,
    @UsersTemp		ExchangeUserTableType readonly,
    @Now			datetime
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @Users ExchangeUserTableType
	INSERT INTO @Users SELECT * FROM @UsersTemp
	
	DECLARE @_Result int = 0
	
	DECLARE @TempUsers TABLE(ID int IDENTITY(1,1) PRIMARY KEY CLUSTERED, 
		UserID uniqueidentifier, NewUserName nvarchar(255),
		FirstName nvarchar(255), LastName nvarchar(255), EmploymentType varchar(50))
	
	INSERT INTO @TempUsers(
		UserID, NewUserName, FirstName, LastName, EmploymentType)
	SELECT USR.UserID, Ref.NewUserName, [dbo].[GFN_VerifyString](Ref.FirstName),
		[dbo].[GFN_VerifyString](Ref.LastName), Ref.EmploymentType
	FROM @Users AS Ref
		INNER JOIN [dbo].[aspnet_Users] AS USR
		ON USR.LoweredUserName = LOWER(Ref.UserName)
		INNER JOIN [dbo].[USR_UserApplications] AS App
		ON App.ApplicationID = @ApplicationID AND App.UserID = USR.UserID
	
	-- Create New Users
	DECLARE @NewUsers ExchangeUserTableType
	DECLARE @FirstPasswords GuidStringTableType
	
	INSERT INTO @NewUsers (UserID, UserName, FirstName, LastName, 
		[Password], PasswordSalt, EncryptedPassword)
	SELECT NEWID(), Ref.UserName, Ref.FirstName, Ref.LastName, 
		Ref.[Password], Ref.PasswordSalt, Ref.EncryptedPassword
	FROM @Users AS Ref
		LEFT JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND LOWER(UN.UserName) = LOWER(Ref.UserName) AND
			ISNULL(Ref.NewUserName, N'') = N''
	WHERE ISNULL(Ref.[Password], N'') <> N'' AND ISNULL(Ref.PasswordSalt, N'') <> N'' AND
		ISNULL(Ref.EncryptedPassword, N'') <> N'' AND UN.UserID IS NULL
	
	INSERT INTO @FirstPasswords (FirstValue, SecondValue)
	SELECT U.UserID, U.EncryptedPassword
	FROM @NewUsers AS U
	
	DECLARE @_ErrorMessage nvarchar(255)
	
	EXEC [dbo].[USR_P_CreateUsers] @ApplicationID, @NewUsers, @Now, 
		@_Result output, @_ErrorMessage output
	
	EXEC [dbo].[USR_P_SavePasswordHistoryBulk] @FirstPasswords, 1, @Now, @_Result output
	-- end of Create New Users
	
	-- Reset passwords
	DECLARE @ChangePassUsers ExchangeUserTableType
	
	INSERT INTO @ChangePassUsers (UserID, [Password], PasswordSalt, EncryptedPassword)
	SELECT UN.UserID, Ref.[Password], Ref.PasswordSalt, Ref.EncryptedPassword
	FROM @Users AS Ref
		INNER JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND LOWER(UN.UserName) = LOWER(Ref.UserName)
		LEFT JOIN @FirstPasswords AS F
		ON F.FirstValue = UN.UserID
	WHERE Ref.ResetPassword = 1 AND F.FirstValue IS NULL AND 
		ISNULL(Ref.[Password], N'') <> N'' AND ISNULL(Ref.PasswordSalt, N'') <> N''
	
	UPDATE M
		SET [Password] = C.[Password],
			PasswordSalt = C.PasswordSalt
	FROM @ChangePassUsers AS C
		INNER JOIN [dbo].[aspnet_Membership] AS M
		ON M.UserId = C.UserID
	
	DECLARE @ChangedPasswords GuidStringTableType
	
	INSERT INTO @ChangedPasswords(FirstValue, SecondValue)
	SELECT U.UserID, U.EncryptedPassword
	FROM @ChangePassUsers AS U
	
	EXEC [dbo].[USR_P_SavePasswordHistoryBulk] @ChangedPasswords , 1, @Now, @_Result output
	-- end of Reset passwords
	
	
	UPDATE P
		SET FirstName = Ref.FirstName
	FROM @TempUsers AS Ref
		INNER JOIN [dbo].[USR_Profile] AS P
		ON P.[UserID] = Ref.UserID
	WHERE ISNULL(Ref.FirstName, N'') <> N''
	
	UPDATE P
		SET LastName = Ref.LastName
	FROM @TempUsers AS Ref
		INNER JOIN [dbo].[USR_Profile] AS P
		ON P.[UserID] = Ref.UserID
	WHERE ISNULL(Ref.LastName, N'') <> N''
	
	UPDATE P
		SET EmploymentType = Ref.EmploymentType
	FROM @TempUsers AS Ref
		INNER JOIN [dbo].[USR_UserApplications] AS P
		ON P.ApplicationID = @ApplicationID AND P.[UserID] = Ref.UserID
	WHERE ISNULL(Ref.EmploymentType, N'') <> N''
	
	UPDATE USR
		SET UserName = X.NewUserName,
			LoweredUserName = LOWER(X.NewUserName)
	FROM (
			SELECT U.*
			FROM @TempUsers AS U
				LEFT JOIN [dbo].[Users_Normal] AS UN
				ON UN.ApplicationID = @ApplicationID AND 
					UN.LoweredUserName = LOWER(U.NewUserName) AND U.UserID <> UN.UserId
			WHERE ISNULL(U.NewUserName, N'') <> N'' AND UN.UserId IS NULL
		) AS X
		INNER JOIN [dbo].[aspnet_Users] AS USR
		ON USR.UserId = X.UserID
	
	SELECT 1
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[DE_UpdateMembers]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DE_UpdateMembers]
GO

CREATE PROCEDURE [dbo].[DE_UpdateMembers]
	@ApplicationID	uniqueidentifier,
    @MembersTemp	ExchangeMemberTableType readonly,
    @Now			datetime
WITH ENCRYPTION, RECOMPILE
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	DECLARE @Members ExchangeMemberTableType
	INSERT INTO @Members SELECT * FROM @MembersTemp
	
	DECLARE @MBRS Table(
		NodeID uniqueidentifier,
		UserID uniqueidentifier,
		IsAdmin bit,
		UniqueAdmin bit
	)
	
	INSERT INTO @MBRS (NodeID, UserID, IsAdmin, UniqueAdmin)
	SELECT	ND.NodeID,
			UN.UserID,
			CAST(MAX(CAST(ISNULL(M.IsAdmin, 0) AS int)) AS bit),
			CAST(MAX(CAST(ISNULL(S.UniqueAdminMember, 0) AS int)) AS bit)
	FROM @Members AS M
		INNER JOIN [dbo].[CN_View_Nodes_Normal] AS ND
		ON ND.ApplicationID = @ApplicationID AND 
			(M.NodeID IS NULL AND ND.TypeAdditionalID = M.NodeTypeAdditionalID AND
			ND.NodeAdditionalID = M.NodeAdditionalID) OR
			(M.NodeID IS NOT NULL AND ND.NodeID = M.NodeID)
		LEFT JOIN [dbo].[CN_Services] AS S
		ON S.ApplicationID = @ApplicationID AND S.NodeTypeID = ND.NodeTypeID
		INNER JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND LOWER(UN.UserName) = LOWER(M.UserName)
	GROUP BY ND.NodeID, UN.UserID
	
	DECLARE @_Result int
	
	-- Add members
	DECLARE @MIDs GuidPairTableType
	
	INSERT INTO @MIDs (FirstValue, SecondValue)
	SELECT NodeID, UserID FROM @MBRS
	
	EXEC [dbo].[CN_P_AddAcceptedMembers] @ApplicationID, @MIDs, @Now, @_Result output
	
	IF @_Result <= 0 BEGIN
		SELECT -1
		ROLLBACK TRANSACTION
		RETURN
	END
	--end of Add members
	
	--Update admins
	UPDATE NM
		SET IsAdmin = (
			CASE
				WHEN NIDs.UniqueAdmin = 0 THEN ISNULL(Ref.IsAdmin, NM.IsAdmin) 
				WHEN Ref.NodeID IS NULL
					THEN (CASE WHEN NIDs.AdminsCount = 0 THEN NM.IsAdmin ELSE 0 END)
				ELSE (CASE WHEN NIDs.AdminsCount <= 1 THEN Ref.IsAdmin ELSE 0 END)
			END
		)
	FROM (
			SELECT	X.NodeID, 
					CAST(MAX(CAST(X.UniqueAdmin AS int)) AS bit) AS UniqueAdmin, 
					SUM(CAST(X.IsAdmin AS int)) AS AdminsCount
			FROM @MBRS AS X
			GROUP BY X.NodeID
		) AS NIDs
		INNER JOIN [dbo].[CN_NodeMembers] AS NM
		ON NM.ApplicationID = @ApplicationID AND NM.NodeID = NIDs.NodeID AND NM.Deleted = 0
		LEFT JOIN @MBRS AS Ref
		ON Ref.NodeID = NM.NodeID AND Ref.UserID = NM.UserID
	--end of Update admins
	
	SELECT 1
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[DE_UpdateExperts]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DE_UpdateExperts]
GO

CREATE PROCEDURE [dbo].[DE_UpdateExperts]
	@ApplicationID	uniqueidentifier,
    @ExpertsTemp	ExchangeMemberTableType readonly,
    @Now			datetime
WITH ENCRYPTION, RECOMPILE
AS
BEGIN TRANSACTION
	SET NOCOUNT ON
	
	DECLARE @Experts ExchangeMemberTableType
	INSERT INTO @Experts SELECT * FROM @ExpertsTemp
	
	DECLARE @XPRTS Table(
		NodeID uniqueidentifier,
		UserID uniqueidentifier,
		[Exists] bit,
		AdditionalID varchar(50)
	)
	
	INSERT INTO @XPRTS (NodeID, UserID, AdditionalID)
	SELECT	ND.NodeID,
			UN.UserID,
			X.NodeAdditionalID
	FROM @Experts AS X
		INNER JOIN [dbo].[CN_View_Nodes_Normal] AS ND
		ON ND.ApplicationID = @ApplicationID AND 
			(X.NodeID IS NULL AND ND.TypeAdditionalID = X.NodeTypeAdditionalID AND
			ND.NodeAdditionalID = X.NodeAdditionalID) OR
			(X.NodeID IS NOT NULL AND ND.NodeID = X.NodeID)
		INNER JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND LOWER(UN.UserName) = LOWER(X.UserName)
		
	UPDATE X
		SET [Exists] = 1
	FROM @XPRTS AS X
		INNER JOIN [dbo].[CN_Experts] AS EX
		ON EX.ApplicationID = @ApplicationID AND 
			EX.NodeID = X.NodeID AND EX.UserID = X.UserID
		
	DECLARE @Count int, @ExistingCount int
	
	SELECT	@Count = COUNT(X.NodeID),
			@ExistingCount = SUM(CAST((CASE WHEN X.[Exists] = 1 THEN 1 ELSE 0 END) AS int))
	FROM @XPRTS AS X
	
	IF @ExistingCount > 0 BEGIN
		UPDATE EX
			SET Approved = 1
		FROM @XPRTS AS X
			INNER JOIN [dbo].[CN_Experts] AS EX
			ON EX.NodeID = X.NodeID AND EX.UserID = X.UserID
		WHERE EX.ApplicationID = @ApplicationID AND X.[Exists] = 1
		
		IF @@ROWCOUNT <= 0 BEGIN
			SELECT -1
			ROLLBACK TRANSACTION
			RETURN
		END
	END
	
	IF (@Count - @ExistingCount) > 0 BEGIN
		INSERT INTO [dbo].[CN_Experts](
			ApplicationID,
			NodeID,
			UserID,
			Approved,
			ReferralsCount,
			ConfirmsPercentage,
			SocialApproved,
			UniqueID
		)
		SELECT	@ApplicationID,
				X.NodeID,
				X.UserID,
				1,
				0,
				0,
				0,
				NEWID()
		FROM @XPRTS AS X
		WHERE ISNULL(X.[Exists], 0) = 0
		
		IF @@ROWCOUNT <= 0 BEGIN
			SELECT -1
			ROLLBACK TRANSACTION
			RETURN
		END
	END
	
	SELECT 1
COMMIT TRANSACTION

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[DE_UpdateRelations]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DE_UpdateRelations]
GO

CREATE PROCEDURE [dbo].[DE_UpdateRelations]
	@ApplicationID	uniqueidentifier,
	@CurrentUserID	uniqueidentifier,
    @RelationsTemp	ExchangeRelationTableType readonly,
    @Now			datetime
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @Relations ExchangeRelationTableType
	INSERT INTO @Relations SELECT * FROM @RelationsTemp
	
	DECLARE @RelationTypeID uniqueidentifier = 
		[dbo].[CN_FN_GetRelatedRelationTypeID](@ApplicationID)
	
	UPDATE NR
		SET LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now,
			Deleted = 0
	FROM (
			SELECT DISTINCT *
			FROM (
				SELECT	SourceTypeAdditionalID,
						SourceAdditionalID,
						DestinationTypeAdditionalID,
						DestinationAdditionalID
				FROM @Relations AS R
				
				UNION ALL
				
				SELECT	DestinationTypeAdditionalID,
						DestinationAdditionalID,
						SourceTypeAdditionalID,
						SourceAdditionalID
				FROM @Relations AS R
				WHERE R.Bidirectional = 1
			) AS X
		) AS R
		INNER JOIN [dbo].[CN_NodeTypes] AS SNT
		ON SNT.ApplicationID = @ApplicationID AND SNT.AdditionalID = R.SourceTypeAdditionalID
		INNER JOIN [dbo].[CN_Nodes] AS SND
		ON SND.ApplicationID = @ApplicationID AND SND.NodeTypeID = SNT.NodeTypeID AND
			SND.AdditionalID = R.SourceAdditionalID
		INNER JOIN [dbo].[CN_NodeTypes] AS DNT
		ON DNT.ApplicationID = @ApplicationID AND 
			DNT.AdditionalID = R.DestinationTypeAdditionalID
		INNER JOIN [dbo].[CN_Nodes] AS DND
		ON DND.ApplicationID = @ApplicationID AND DND.NodeTypeID = DNT.NodeTypeID AND
			DND.AdditionalID = R.DestinationAdditionalID
		INNER JOIN [dbo].[CN_NodeRelations] AS NR
		ON NR.ApplicationID = @ApplicationID AND NR.SourceNodeID = SND.NodeID AND
			NR.DestinationNodeID = DND.NodeID AND NR.PropertyID = @RelationTypeID
	
	DECLARE @CNT int = @@ROWCOUNT
	
	INSERT INTO [dbo].[CN_NodeRelations] (
		ApplicationID,
		SourceNodeID,
		DestinationNodeID,
		PropertyID,
		CreatorUserID,
		CreationDate,
		Deleted,
		UniqueID
	)
	SELECT	@ApplicationID, 
			SND.NodeID, 
			DND.NodeID, 
			@RelationTypeID, 
			@CurrentUserID, 
			@Now, 
			0, 
			NEWID()
	FROM (
			SELECT DISTINCT *
			FROM (
				SELECT	SourceTypeAdditionalID,
						SourceAdditionalID,
						DestinationTypeAdditionalID,
						DestinationAdditionalID
				FROM @Relations AS R
				
				UNION ALL
				
				SELECT	DestinationTypeAdditionalID,
						DestinationAdditionalID,
						SourceTypeAdditionalID,
						SourceAdditionalID
				FROM @Relations AS R
				WHERE R.Bidirectional = 1
			) AS X
		) AS R
		INNER JOIN [dbo].[CN_NodeTypes] AS SNT
		ON SNT.ApplicationID = @ApplicationID AND SNT.AdditionalID = R.SourceTypeAdditionalID
		INNER JOIN [dbo].[CN_Nodes] AS SND
		ON SND.ApplicationID = @ApplicationID AND SND.NodeTypeID = SNT.NodeTypeID AND
			SND.AdditionalID = R.SourceAdditionalID
		INNER JOIN [dbo].[CN_NodeTypes] AS DNT
		ON DNT.ApplicationID = @ApplicationID AND 
			DNT.AdditionalID = R.DestinationTypeAdditionalID
		INNER JOIN [dbo].[CN_Nodes] AS DND
		ON DND.ApplicationID = @ApplicationID AND DND.NodeTypeID = DNT.NodeTypeID AND
			DND.AdditionalID = R.DestinationAdditionalID
		LEFT JOIN [dbo].[CN_NodeRelations] AS NR
		ON NR.ApplicationID = @ApplicationID AND NR.SourceNodeID = SND.NodeID AND
			NR.DestinationNodeID = DND.NodeID AND NR.PropertyID = @RelationTypeID
	WHERE NR.SourceNodeID IS NULL
	
	SELECT @@ROWCOUNT + @CNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[DE_UpdateAuthors]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DE_UpdateAuthors]
GO

CREATE PROCEDURE [dbo].[DE_UpdateAuthors]
	@ApplicationID	uniqueidentifier,
	@CurrentUserID	uniqueidentifier,
    @AuthorsTemp	ExchangeAuthorTableType readonly,
    @Now			datetime
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @Authors ExchangeAuthorTableType
	INSERT INTO @Authors SELECT * FROM @AuthorsTemp
	
	DECLARE @Shares TABLE (NodeID uniqueidentifier, UserID uniqueidentifier, Percentage int)
	
	INSERT INTO @Shares (NodeID, UserID, Percentage)
	SELECT ND.NodeID, UN.UserID, MAX(A.Percentage)
	FROM @Authors AS A
		INNER JOIN [dbo].[CN_View_Nodes_Normal] AS ND
		ON ND.ApplicationID = @ApplicationID AND ND.TypeAdditionalID = A.NodeTypeAdditionalID AND
			ND.NodeAdditionalID = A.NodeAdditionalID
		INNER JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND 
			UN.UserName IS NOT NULL AND LOWER(UN.UserName) = LOWER(A.UserName)
	WHERE ISNULL(A.NodeTypeAdditionalID, N'') <> N'' AND ISNULL(A.NodeAdditionalID, N'') <> N'' AND 
		A.Percentage IS NOT NULL AND A.Percentage > 0 AND A.Percentage <= 100
	GROUP BY ND.NodeID, UN.UserID
	
	DELETE X
	FROM @Shares AS X
		INNER JOIN (
			SELECT S.NodeID, SUM(S.Percentage) AS Summation
			FROM @Shares AS S
			GROUP BY S.NodeID
		) AS Ref
		ON Ref.NodeID = X.NodeID
	WHERE Ref.Summation <> 100
	
	UPDATE NC
		SET Deleted = 1,
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	FROM (SELECT DISTINCT NodeID FROM @Shares) AS S
		INNER JOIN [dbo].[CN_NodeCreators] AS NC
		ON NC.ApplicationID = @ApplicationID AND NC.NodeID = S.NodeID
		
	UPDATE NC
		SET Deleted = 0,
			CollaborationShare = CAST(S.Percentage AS float),
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	FROM @Shares AS S
		INNER JOIN [dbo].[CN_NodeCreators] AS NC
		ON NC.ApplicationID = @ApplicationID AND NC.NodeID = S.NodeID AND NC.UserID = S.UserID
	
	DECLARE @CNT int = @@ROWCOUNT
	
	INSERT INTO [dbo].[CN_NodeCreators] (ApplicationID, NodeID, UserID, CollaborationShare,
		CreatorUserID, CreationDate, Deleted, UniqueID)
	SELECT	@ApplicationID, S.NodeID, S.UserID, CAST(S.Percentage AS float), 
			@CurrentUserID, @Now, 0, NEWID()
	FROM @Shares AS S
		LEFT JOIN [dbo].[CN_NodeCreators] AS NC
		ON NC.ApplicationID = @ApplicationID AND NC.NodeID = S.NodeID AND NC.UserID = S.UserID
	WHERE NC.NodeID IS NULL
	
	SELECT @@ROWCOUNT + @CNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[DE_UpdateUserConfidentialities]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DE_UpdateUserConfidentialities]
GO

CREATE PROCEDURE [dbo].[DE_UpdateUserConfidentialities]
	@ApplicationID	uniqueidentifier,
	@CurrentUserID	uniqueidentifier,
    @strInput		varchar(max),
    @innerDelimiter	char,
    @outerDelimiter char,
    @Now			datetime
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @Values TABLE (UserID uniqueidentifier, ConfidentialityID uniqueidentifier)
	
	INSERT INTO @Values (UserID, ConfidentialityID)
	SELECT DISTINCT UN.UserID, L.ID
	FROM [dbo].[GFN_StrToFloatStringTable](@strInput, @innerDelimiter, @outerDelimiter) AS Ref
		INNER JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND LOWER(UN.UserName) = LOWER(Ref.SecondValue)
		INNER JOIN [dbo].[PRVC_ConfidentialityLevels] AS L
		ON L.ApplicationID = @ApplicationID AND L.LevelID = CAST(Ref.FirstValue AS int)
		
	UPDATE S
		SET ConfidentialityID = V.ConfidentialityID,
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	FROM @Values AS V
		INNER JOIN [dbo].[PRVC_Settings] AS S
		ON S.ApplicationID = @ApplicationID AND S.ObjectID = V.UserID
	
	DECLARE @CNT int = @@ROWCOUNT
	
	INSERT INTO [dbo].[PRVC_Settings](
		ApplicationID,
		ObjectID,
		ConfidentialityID,
		CreatorUserID,
		CreationDate
	)
	SELECT @ApplicationID, V.UserID, V.ConfidentialityID, @CurrentUserID, @Now
	FROM @Values AS V
		LEFT JOIN [dbo].[PRVC_Settings] AS S
		ON S.ApplicationID = @ApplicationID AND S.ObjectID = V.UserID
	WHERE S.ObjectID IS NULL
	
	SELECT @@ROWCOUNT + @CNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[DE_UpdatePermissions]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DE_UpdatePermissions]
GO

CREATE PROCEDURE [dbo].[DE_UpdatePermissions]
	@ApplicationID	uniqueidentifier,
	@CurrentUserID	uniqueidentifier,
    @ItemsTemp		ExchangePermissionTableType readonly,
    @Now			datetime
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @Items ExchangePermissionTableType
	INSERT INTO @Items SELECT * FROM @ItemsTemp
	
	DECLARE @Values TABLE (
		ObjectID uniqueidentifier, 
		RoleID uniqueidentifier, 
		PermissionType nvarchar(50), 
		Allow bit,
		DropAll bit
	)
	
	
	INSERT INTO @Values (ObjectID, RoleID, PermissionType, Allow, DropAll)
	SELECT DISTINCT ND.NodeID, ISNULL(UN.UserID, GRP.NodeID), I.PermissionType, I.Allow, I.DropAll
	FROM @Items AS I
		INNER JOIN [dbo].[CN_View_Nodes_Normal] AS ND
		ON ND.ApplicationID = @ApplicationID AND ND.TypeAdditionalID = I.NodeTypeAdditionalID AND
			ND.NodeAdditionalID = I.NodeAdditionalID
		LEFT JOIN [dbo].[CN_View_Nodes_Normal] AS GRP
		ON GRP.ApplicationID = @ApplicationID AND GRP.TypeAdditionalID = I.GroupTypeAdditionalID AND
			GRP.NodeAdditionalID = I.GroupAdditionalID
		LEFT JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND LOWER(UN.UserName) = LOWER(I.UserName)
	WHERE ((GRP.NodeID IS NOT NULL OR UN.UserID IS NOT NULL) AND I.PermissionType IS NOT NULL) OR I.DropAll = 1
	
	
	-- Part 1: Drop All
	UPDATE A
		SET Deleted = 1,
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	FROM [dbo].[PRVC_Audience] AS A
		INNER JOIN (
			SELECT DISTINCT V.ObjectID
			FROM @Values AS V
			WHERE V.DropAll = 1
		) AS Ref
		ON Ref.ObjectID = A.ObjectID
	WHERE A.ApplicationID = @ApplicationID
	-- end of Part 1: Drop All
	
	DECLARE @CNT int = @@ROWCOUNT
	
	-- Part 2: Update Existing Items
	UPDATE A
		SET Allow = ISNULL(V.Allow, 0),
			ExpirationDate = NULL,
			Deleted = 0,
			LastModifierUserID = @CurrentUserID,
			LastModificationDate = @Now
	FROM @Values AS V
		INNER JOIN [dbo].[PRVC_Audience] AS A
		ON A.ApplicationID = @ApplicationID AND A.ObjectID = V.ObjectID AND 
			A.RoleID = V.RoleID AND A.PermissionType = V.PermissionType
	-- end of Part 2: Update Existing Items
	
	SET @CNT = @@ROWCOUNT + @CNT
	
	-- Part 3: Add New Items
	INSERT INTO [dbo].[PRVC_Audience](
		ApplicationID,
		ObjectID,
		RoleID,
		PermissionType,
		Allow,
		CreatorUserID,
		CreationDate,
		Deleted
	)
	SELECT @ApplicationID, V.ObjectID, V.RoleID, V.PermissionType, ISNULL(V.Allow, 0), @CurrentUserID, @Now, 0	
	FROM @Values AS V
		LEFT JOIN [dbo].[PRVC_Audience] AS A
		ON A.ApplicationID = @ApplicationID AND A.ObjectID = V.ObjectID AND 
			A.RoleID = V.RoleID AND A.PermissionType = V.PermissionType
	WHERE V.ObjectID IS NOT NULL AND V.RoleID IS NOT NULL AND 
		V.PermissionType IS NOT NULL AND A.ObjectID IS NULL
	-- end of Part 3: Add New Items
	
	SELECT @@ROWCOUNT + @CNT
END

GO