USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[DCT_P_CopyAttachments]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DCT_P_CopyAttachments]
GO

CREATE PROCEDURE [dbo].[DCT_P_CopyAttachments]
	@FromOwnerID	uniqueidentifier,
    @ToOwnerID		uniqueidentifier,
    @ToOwnerType	varchar(20),
    @_Result		int output
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	IF NOT EXISTS(
		SELECT TOP(1) AT.ID
		FROM [dbo].[AttachmentFiles] AS AF
			INNER JOIN [dbo].[Attachments] AS AT
			ON AT.ID = AF.AttachmentID
		WHERE AT.ObjectID = @FromOwnerID AND AT.Deleted = 0 AND AF.Deleted = 0
	) BEGIN
		SET @_Result = 0
		RETURN
	END
	
	DECLARE @NewAttachmentID uniqueidentifier
	
	SELECT TOP(1) @NewAttachmentID = ID
	FROM [dbo].[Attachments] AS A
	WHERE A.ObjectID = @ToOwnerID AND A.Deleted = 0
	
	IF @NewAttachmentID IS NULL BEGIN
		SET @NewAttachmentID = NEWID()
		
		INSERT INTO [dbo].[Attachments] (ID, ObjectType, ObjectID, Deleted)
		VALUES (@NewAttachmentID, @ToOwnerType, @ToOwnerID, 0)
	END
	
	INSERT INTO [dbo].[AttachmentFiles] (ID, AttachmentID, Extension, [FileName], 
		FileNameGuid, MIME, Size, Deleted)
	SELECT NEWID(), @NewAttachmentID, AF.Extension, AF.[FileName],
		AF.FileNameGuid, AF.MIME, AF.Size, AF.Deleted
	FROM [dbo].[AttachmentFiles] AS AF
		INNER JOIN [dbo].[Attachments] AS AT
		ON AT.ID = AF.AttachmentID
	WHERE AT.ObjectID = @FromOwnerID AND AT.Deleted = 0 AND AF.Deleted = 0
	
	SET @_Result = @@ROWCOUNT
END

GO


DECLARE @TBL table (ID int identity(1,1) primary key clustered,
	messageId uniqueidentifier, forwardedFrom uniqueidentifier)
	
INSERT INTO @TBL (messageId, forwardedFrom)
SELECT M.MessageID, M.ForwardedFrom
FROM [dbo].[MSG_Messages] AS M
WHERE M.ForwardedFrom IS NOT NULL AND 
	NOT EXISTS(
		SELECT TOP(1) *
		FROM [dbo].[Attachments] AS A
		WHERE A.ObjectID = M.MessageID AND A.Deleted = 0
	) AND EXISTS(
		SELECT TOP(1) *
		FROM [dbo].[Attachments] AS A
			INNER JOIN [dbo].[AttachmentFiles] AS F
			ON F.AttachmentID = A.ID
		WHERE A.ObjectID = M.ForwardedFrom AND A.Deleted = 0 AND F.Deleted = 0
	)
	
DECLARE @CNT int = (SELECT MAX(ID) FROM @TBL)

WHILE @CNT > 0 BEGIN
	DECLARE @MID uniqueidentifier, @FID uniqueidentifier
	
	SELECT @MID = messageId, @FID = forwardedFrom
	FROM @TBL AS T
	WHERE T.ID = @CNT
	
	DECLARE @R int
	
	EXEC [dbo].[DCT_P_CopyAttachments] @FID, @MID, N'Message', @R output
	
	UPDATE [dbo].[MSG_Messages]
		SET HasAttachment = 1
	WHERE MessageID = @MID
	
	SET @CNT = @CNT - 1
END

GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[DCT_P_CopyAttachments]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DCT_P_CopyAttachments]
GO
