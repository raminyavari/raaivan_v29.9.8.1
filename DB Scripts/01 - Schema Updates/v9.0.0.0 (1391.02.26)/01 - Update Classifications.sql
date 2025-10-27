USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/****** Object:  Table [dbo].[Classifications]    Script Date: 05/09/2012 12:01:00 ******/
CREATE TABLE [dbo].[TempClassifications](
	[ClassificationID] [int] NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_TempClassifications] PRIMARY KEY CLUSTERED 
(
	[ClassificationID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[TempClassifications]
			([ClassificationID]
			,[Title])
SELECT  dbo.Classifications.OrderNum, dbo.Classifications.Title
FROM    dbo.Classifications
GO


/****** Object:  Table [dbo].[ClassificationUsers]    Script Date: 05/09/2012 12:01:18 ******/
CREATE TABLE [dbo].[TempClassificationUsers](
	[UserID] [uniqueidentifier] NOT NULL,
	[ClassificationID] [int] NOT NULL,
 CONSTRAINT [PK_TempClassificationUsers] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[TempClassificationUsers]
			([ClassificationID]
			,[UserID])
SELECT  dbo.Classifications.OrderNum, dbo.ClassificationUsers.UserId
FROM    dbo.Classifications INNER JOIN dbo.ClassificationUsers ON
		dbo.Classifications.ID = dbo.ClassificationUsers.ClassificationId
GO



/****** Object:  Table [dbo].[KKnowledges]    Script Date: 05/09/2012 12:01:18 ******/
ALTER TABLE [dbo].[KKnowledges]
ADD [TempClassificationID] [int] NULL;
GO

UPDATE [dbo].[KKnowledges]
SET TempClassificationID = (SELECT dbo.Classifications.OrderNum FROM dbo.Classifications
							WHERE dbo.Classifications.ID = dbo.KKnowledges.ClassificationID)
GO

ALTER TABLE [dbo].[KKnowledges]
DROP CONSTRAINT [FK_KKnowledges_Classifications]
GO

ALTER TABLE [dbo].[KKnowledges]
DROP COLUMN [ClassificationID]
GO


DROP TABLE [dbo].[ClassificationUsers]
GO

DROP TABLE [dbo].[Classifications]
GO


/****** Object:  Table [dbo].[Classifications]    Script Date: 05/09/2012 12:01:00 ******/
CREATE TABLE [dbo].[Classifications](
	[ClassificationID] [int] NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_Classifications] PRIMARY KEY CLUSTERED 
(
	[ClassificationID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[Classifications]
SELECT * FROM dbo.TempClassifications
GO

DROP TABLE [dbo].[TempClassifications]
GO


/****** Object:  Table [dbo].[ClassificationUsers]    Script Date: 05/09/2012 12:01:18 ******/
CREATE TABLE [dbo].[UsersClassifications](
	[UserID] [uniqueidentifier] NOT NULL,
	[ClassificationID] [int] NOT NULL,
 CONSTRAINT [PK_UsersClassifications] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[UsersClassifications]  WITH CHECK ADD  CONSTRAINT [FK_UsersClassifications_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[UsersClassifications] CHECK CONSTRAINT [FK_UsersClassifications_aspnet_Users]
GO

ALTER TABLE [dbo].[UsersClassifications]  WITH CHECK ADD  CONSTRAINT [FK_UsersClassifications_Classifications] FOREIGN KEY([ClassificationID])
REFERENCES [dbo].[Classifications] ([ClassificationID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[UsersClassifications] CHECK CONSTRAINT [FK_UsersClassifications_Classifications]
GO


INSERT INTO [dbo].[UsersClassifications]
SELECT * FROM dbo.TempClassificationUsers
GO


DROP TABLE [dbo].[TempClassificationUsers]
GO


/****** Object:  Table [dbo].[KKnowledges]    Script Date: 05/09/2012 12:01:18 ******/
ALTER TABLE [dbo].[KKnowledges]
ADD [ClassificationID] [int] NULL;
GO

UPDATE [dbo].[KKnowledges]
SET ClassificationID = TempClassificationID
GO

ALTER TABLE [dbo].[KKnowledges]
DROP COLUMN [TempClassificationID]
GO

ALTER TABLE [dbo].[KKnowledges]  WITH CHECK ADD  CONSTRAINT [FK_KKnowledges_Classifications] FOREIGN KEY([ClassificationID])
REFERENCES [dbo].[Classifications] ([ClassificationID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[KKnowledges] CHECK CONSTRAINT [FK_KKnowledges_Classifications]
GO