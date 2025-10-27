USE [EKM_App]
GO

/****** Object:  Table [dbo].[AttachmentFiles]    Script Date: 02/03/2018 08:59:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.9.5.2' BEGIN
	UPDATE [dbo].[AppSetting]
		SET [Version] = 'v28.10.5.3'
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.10.5.3' BEGIN
	CREATE TABLE [dbo].[DCT_Files](
		[ID] [uniqueidentifier] NOT NULL,
		[OwnerID] [uniqueidentifier] NOT NULL,
		[OwnerType] [varchar](50) NOT NULL,
		[FileNameGuid] [uniqueidentifier] NOT NULL,
		[Extension] [nvarchar](50) NULL,
		[FileName] [nvarchar](255) NOT NULL,
		[MIME] [nvarchar](255) NULL,
		[Size] [bigint] NULL,
		[CreatorUserID] [uniqueidentifier] NOT NULL,
		[CreationDate] [datetime] NOT NULL,
		[Deleted] [bit] NOT NULL,
		[ApplicationID] [uniqueidentifier] NOT NULL
	 CONSTRAINT [PK_DCT_Files] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.10.5.3' BEGIN
	ALTER TABLE [dbo].[DCT_Files]  WITH CHECK ADD  CONSTRAINT [FK_DCT_Files_aspnet_Applications] FOREIGN KEY([ApplicationID])
	REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.10.5.3' BEGIN
	ALTER TABLE [dbo].[DCT_Files] CHECK CONSTRAINT [FK_DCT_Files_aspnet_Applications]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.10.5.3' BEGIN
	ALTER TABLE [dbo].[DCT_Files]  WITH CHECK ADD  CONSTRAINT [FK_DCT_Files_aspnet_Users] FOREIGN KEY([CreatorUserID])
	REFERENCES [dbo].[aspnet_Users] ([UserId])
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.10.5.3' BEGIN
	ALTER TABLE [dbo].[DCT_Files] CHECK CONSTRAINT [FK_DCT_Files_aspnet_Users]
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.10.5.3' BEGIN
	DECLARE @Now datetime = N'2000-01-01 00:00:00.000'
	
	DECLARE @Str nvarchar(max) = 
		N'INSERT INTO [dbo].[DCT_Files] ( ' +
			N'ApplicationID, ' +
			N'ID, ' +
			N'OwnerID, ' +
			N'OwnerType, ' +
			N'FileNameGuid, ' +
			N'Extension, ' +
			N'[FileName], ' +
			N'MIME, ' +
			N'Size, ' +
			N'CreatorUserID, ' +
			N'CreationDate, ' +
			N'Deleted ' +
		N') ' +
		N'SELECT	F.ApplicationID, ' +
				N'F.ID, ' +
				N'A.ObjectID, ' +
				N'A.ObjectType, ' +
				N'F.FileNameGuid, ' +
				N'F.Extension, ' +
				N'F.[FileName], ' +
				N'F.MIME, ' +
				N'F.Size, ' +
				N'U.UserId, ' +
				N'N''' + @Now + ''', ' +
				N'CASE WHEN ISNULL(F.Deleted, 0) = 0 AND ISNULL(A.Deleted, 0) = 0 THEN 0 ELSE 1 END ' +
		N'FROM [dbo].[Attachments] AS A ' +
			N'INNER JOIN [dbo].[AttachmentFiles] AS F ' +
			N'ON F.AttachmentID = A.ID ' +
			N'INNER JOIN [dbo].[aspnet_Applications] AS P ' +
			N'ON P.ApplicationId = F.ApplicationID ' +
			N'INNER JOIN [dbo].[aspnet_Users] AS U ' +
			N'ON U.ApplicationId = P.ApplicationId AND LOWER(U.UserName) = N''admin'''
			
	EXEC (@Str)
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.10.5.3' BEGIN
	ALTER TABLE [dbo].[KW_KnowledgeTypes]
	ADD PreEvaluateByOwner bit NULL
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.10.5.3' BEGIN
	ALTER TABLE [dbo].[KW_KnowledgeTypes]
	ADD ForceEvaluatorsDescribe bit NULL
END
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.10.5.3' BEGIN
	UPDATE [dbo].[AppSetting]
		SET [Version] = 'v28.10.8.0'
END
GO

