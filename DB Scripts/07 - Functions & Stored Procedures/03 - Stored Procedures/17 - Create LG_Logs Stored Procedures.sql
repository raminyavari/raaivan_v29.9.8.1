USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[LG_SaveLog]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[LG_SaveLog]
GO

CREATE PROCEDURE [dbo].[LG_SaveLog]
	@ApplicationID		uniqueidentifier,
	@UserID				uniqueidentifier,
	@HostAddress		varchar(100),
	@HostName			nvarchar(255),
	@Action				varchar(100),
	@Level				varchar(20),
	@NotAuthorized		bit,
	@strSubjectIDs		varchar(max),
	@delimiter			char,
	@SecondSubjectID	uniqueidentifier,
	@ThirdSubjectID		uniqueidentifier,
	@FourthSubjectID	uniqueidentifier,
	@Date				datetime,
	@Info				nvarchar(max),
	@ModuleIdentifier	varchar(20)
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @SubjectIDs GuidTableType
	INSERT INTO @SubjectIDs
	SELECT Ref.Value FROM [dbo].[GFN_StrToGuidTable](@strSubjectIDs, @delimiter) AS Ref

	INSERT INTO [dbo].[LG_Logs](
		ApplicationID,
		UserID,
		HostAddress,
		HostName,
		[Action],
		[Level],
		NotAuthorized,
		SubjectID,
		SecondSubjectID,
		ThirdSubjectID,
		FourthSubjectID,
		[Date],
		Info,
		ModuleIdentifier
	)
	SELECT @ApplicationID, ISNULL(@UserID, [dbo].[GFN_GuidEmpty]()), @HostAddress, @HostName, @Action, @Level, @NotAuthorized, Ref.Value, 
		@SecondSubjectID, @ThirdSubjectID, @FourthSubjectID, @Date, @Info, @ModuleIdentifier
	FROM @SubjectIDs AS Ref
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[LG_P_GetLogs]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[LG_P_GetLogs]
GO

CREATE PROCEDURE [dbo].[LG_P_GetLogs]
	@ApplicationID	uniqueidentifier,
    @UserIDsTemp	GuidTableType readonly,
    @ActionsTemp	StringTableType readonly,
    @BeginDate		datetime,
    @FinishDate		datetime,
    @LastID			bigint,
    @Count			int
WITH ENCRYPTION, RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @UserIDs GuidTableType
	INSERT INTO @UserIDs SELECT * FROM @UserIDsTemp
	
    DECLARE @Actions StringTableType
    INSERT INTO @Actions SELECT * FROM @ActionsTemp
	
	DECLARE @ActionsCount int = (SELECT COUNT(*) FROM @Actions)
	DECLARE @UsersCount int = (SELECT COUNT(*) FROM @UserIDs)
	SET @Count = ISNULL(@Count, 100)
	
	IF @UsersCount = 0 BEGIN
		SELECT TOP(@Count) 
			LG.LogID,
			LG.UserID,
			UN.UserName,
			UN.FirstName,
			UN.LastName,
			LG.HostAddress,
			LG.HostName,
			LG.[Action],
			LG.[Date],
			LG.Info,
			LG.ModuleIdentifier
		FROM [dbo].[LG_Logs] AS LG
			LEFT JOIN [dbo].[USR_View_Users] AS UN
			ON UN.UserID = LG.UserID
		WHERE (@ApplicationID IS NULL OR LG.ApplicationID = @ApplicationID) AND
			(@LastID IS NULL OR LogID > @LastID) AND
			(@FinishDate IS NULL OR LG.[Date] < @FinishDate) AND
			(@BeginDate IS NULL OR LG.[Date] > @BeginDate) AND
			(@ActionsCount = 0 OR LG.[Action] IN (SELECT * FROM @Actions))
		ORDER BY LG.LogID DESC
	END
	ELSE BEGIN
		SELECT TOP(@Count)
			LG.LogID,
			LG.UserID,
			UN.UserName,
			UN.FirstName,
			UN.LastName,
			LG.HostAddress,
			LG.HostName,
			LG.[Action],
			LG.[Date],
			LG.Info,
			LG.ModuleIdentifier
		FROM @UserIDs AS USR
			INNER JOIN [dbo].[LG_Logs] AS LG
			ON (@ApplicationID IS NULL OR LG.ApplicationID = @ApplicationID) AND
				LG.UserID = USR.Value
			LEFT JOIN [dbo].[USR_View_Users] AS UN
			ON UN.UserID = LG.UserID
		WHERE (@LastID IS NULL OR LogID > @LastID) AND
			(@FinishDate IS NULL OR LG.[Date] < @FinishDate) AND
			(@BeginDate IS NULL OR LG.[Date] > @BeginDate) AND
			(@ActionsCount = 0 OR LG.[Action] IN (SELECT * FROM @Actions))
		ORDER BY LG.LogID DESC
	END
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[LG_GetLogs]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[LG_GetLogs]
GO

CREATE PROCEDURE [dbo].[LG_GetLogs]
	@ApplicationID	uniqueidentifier,
    @strUserIDs 	varchar(max),
    @strActions		varchar(max),
    @delimiter		char,
    @BeginDate		datetime,
    @FinishDate		datetime,
    @LastID			bigint,
    @Count			int
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @UserIDs GuidTableType
	INSERT INTO @UserIDs
	SELECT Ref.Value FROM [dbo].[GFN_StrToGuidTable](@strUserIDs, @delimiter) AS Ref
	
	DECLARE @Actions StringTableType
	INSERT INTO @Actions
	SELECT Ref.Value FROM [dbo].[GFN_StrToStringTable](@strActions, @delimiter) AS Ref
	
	EXEC [dbo].[LG_P_GetLogs] @ApplicationID, @UserIDs, 
		@Actions, @BeginDate, @FinishDate, @LastID, @Count
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[LG_SaveErrorLog]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[LG_SaveErrorLog]
GO

CREATE PROCEDURE [dbo].[LG_SaveErrorLog]
	@ApplicationID		uniqueidentifier,
	@UserID				uniqueidentifier,
	@Subject			varchar(1000),
	@Description		nvarchar(2000),
	@Date				datetime,
	@ModuleIdentifier	varchar(20),
	@Level				varchar(20)
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

	INSERT INTO [dbo].[LG_ErrorLogs](
		ApplicationID,
		UserID,
		[Subject],
		[Description],
		[Date],
		ModuleIdentifier,
		[Level]
	)
	VALUES (
		@ApplicationID,
		@UserID, 
		@Subject, 
		@Description, 
		@Date, 
		@ModuleIdentifier,
		@Level
	)
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[LG_SaveRawLog]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[LG_SaveRawLog]
GO

CREATE PROCEDURE [dbo].[LG_SaveRawLog]
	@ApplicationID		uniqueidentifier,
	@UserID				uniqueidentifier,
	@Date				datetime,
	@Info				nvarchar(max)
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

	INSERT INTO [dbo].[LG_RawLogs] (
		ApplicationID,
		UserID,
		[Date],
		Info
	)
	VALUES (
		@ApplicationID,
		@UserID, 
		@Date, 
		@Info
	)
	
	SELECT @@ROWCOUNT
END

GO