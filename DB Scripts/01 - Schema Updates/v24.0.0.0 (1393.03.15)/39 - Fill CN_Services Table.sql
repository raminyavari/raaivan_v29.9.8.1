USE [EKM_App]
GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_InitializeService]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_InitializeService]
GO

CREATE PROCEDURE [dbo].[CN_InitializeService]
	@NodeTypeID		uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF EXISTS(SELECT TOP(1) * FROM [dbo].[CN_Services] WHERE NodeTypeID = @NodeTypeID) BEGIN
		UPDATE [dbo].[CN_Services]
			SET Deleted = 0
		WHERE NodeTypeID = @NodeTypeID
	END
	ELSE BEGIN
		DECLARE @SeqNo int = 
			ISNULL((SELECT MAX(SequenceNumber) FROM [dbo].[CN_Services]), 0) + 1
		
		INSERT INTO [dbo].[CN_Services](
			NodeTypeID,
			EnableContribution,
			EditableForAdmin,
			SequenceNumber,
			EditableForCreator,
			EditableForOwners,
			EditableForExperts,
			EditableForMembers,
			Deleted
		)
		VALUES(
			@NodeTypeID,
			0,
			1,
			@SeqNo,
			1,
			1,
			1,
			0,
			0
		)
	END
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_SetServiceTitle]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_SetServiceTitle]
GO

CREATE PROCEDURE [dbo].[CN_SetServiceTitle]
	@NodeTypeID		uniqueidentifier,
	@Title			nvarchar(512)
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[CN_Services]
		SET ServiceTitle = [dbo].[GFN_VerifyString](@Title)
	WHERE NodeTypeID = @NodeTypeID
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_SetFormOwner]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_SetFormOwner]
GO

CREATE PROCEDURE [dbo].[FG_SetFormOwner]
	@OwnerID		uniqueidentifier,
	@FormID			uniqueidentifier,
	@CreatorUserID	uniqueidentifier,
	@CreationDate	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF EXISTS(SELECT TOP(1) * FROM [dbo].[FG_FormOwners]
		WHERE OwnerID = @OwnerID) BEGIN
		UPDATE [dbo].[FG_FormOwners]
			SET FormID = @FormID,
				Deleted = 0,
				LastModifierUserID = @CreatorUserID,
				LastModificationDate = @CreationDate
		WHERE OwnerID = @OwnerID
	END
	ELSE BEGIN
		INSERT INTO [dbo].[FG_FormOwners](
			OwnerID,
			FormID,
			CreatorUserID,
			CreationDate,
			Deleted
		)
		VALUES(
			@OwnerID,
			@FormID,
			@CreatorUserID,
			@CreationDate,
			0
		)
	END
	
	SELECT @@ROWCOUNT
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_SetServiceDescription]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_SetServiceDescription]
GO

CREATE PROCEDURE [dbo].[CN_SetServiceDescription]
	@NodeTypeID		uniqueidentifier,
	@Description	nvarchar(4000)
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	UPDATE [dbo].[CN_Services]
		SET ServiceDescription = [dbo].[GFN_VerifyString](@Description)
	WHERE NodeTypeID = @NodeTypeID
	
	SELECT @@ROWCOUNT
END

GO




DECLARE @UserID uniqueidentifier = (SELECT UserId FROM [dbo].[aspnet_Users] WHERE LoweredUserName = N'admin')

DECLARE @NodeTypeIDs Table (ID int identity(1,1) primary key clustered, 
	NodeTypeID uniqueidentifier)
INSERT INTO @NodeTypeIDs (NodeTypeID)
SELECT DISTINCT NodeTypeID
FROM [dbo].[WF_Services]
WHERE Deleted = 0

DECLARE @Count int = (SELECT COUNT(*) FROM @NodeTypeIDs)

WHILE @Count > 0 BEGIN
	DECLARE @NTID uniqueidentifier = (SELECT NodeTypeID FROM @NodeTypeIDs WHERE ID = @Count)
	DECLARE @Title nvarchar(2000), @Description nvarchar(2000), @FormID uniqueidentifier,
		@Now datetime = GETDATE()
	
	SELECT TOP(1) @Title = Title, @Description  = [Description], @FormID = FormID
	FROM [dbo].[WF_Services]
	WHERE NodeTypeID = @NTID
	
	EXEC [dbo].[CN_InitializeService] @NTID
	
	EXEC [dbo].[CN_SetServiceTitle] @NTID, @Title
	
	EXEC [dbo].[CN_SetServiceDescription] @NTID, @Description
	
	IF @FormID IS NOT NULL EXEC [dbo].[FG_SetFormOwner] @NTID, @FormID, @UserID, @Now
	
	SET @Count = @Count - 1
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_InitializeService]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_InitializeService]
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_SetServiceTitle]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_SetServiceTitle]
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_SetFormOwner]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_SetFormOwner]
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_SetServiceDescription]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_SetServiceDescription]
GO