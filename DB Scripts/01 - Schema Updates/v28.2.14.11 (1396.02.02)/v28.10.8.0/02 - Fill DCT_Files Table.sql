USE [EKM_App]
GO


DECLARE @Now datetime = N'2000-01-01 00:00:00.000'

INSERT INTO [dbo].[DCT_Files] (
	ApplicationID,
	ID,
	OwnerID,
	OwnerType,
	FileNameGuid,
	Extension,
	[FileName],
	MIME,
	Size,
	CreatorUserID,
	CreationDate,
	Deleted
)
SELECT	F.ApplicationID,
		F.ID,
		A.ObjectID,
		A.ObjectType,
		F.FileNameGuid,
		F.Extension,
		F.[FileName],
		F.MIME,
		F.Size,
		U.UserId,
		@Now,
		CASE WHEN ISNULL(F.Deleted, 0) = 0 AND ISNULL(A.Deleted, 0) = 0 THEN 0 ELSE 1 END
FROM [dbo].[Attachments] AS A
	INNER JOIN [dbo].[AttachmentFiles] AS F
	ON F.AttachmentID = A.ID
	INNER JOIN [dbo].[aspnet_Applications] AS P
	ON P.ApplicationId = F.ApplicationID
	INNER JOIN [dbo].[aspnet_Users] AS U
	ON U.ApplicationId = P.ApplicationId AND LOWER(U.UserName) = N'admin'
	
GO