USE [EKM_App]
GO

/****** Object:  Table [dbo].[AttachmentFiles]    Script Date: 02/03/2018 08:59:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

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

GO

ALTER TABLE [dbo].[DCT_Files]  WITH CHECK ADD  CONSTRAINT [FK_DCT_Files_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[DCT_Files] CHECK CONSTRAINT [FK_DCT_Files_aspnet_Applications]
GO

ALTER TABLE [dbo].[DCT_Files]  WITH CHECK ADD  CONSTRAINT [FK_DCT_Files_aspnet_Users] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[DCT_Files] CHECK CONSTRAINT [FK_DCT_Files_aspnet_Users]
GO