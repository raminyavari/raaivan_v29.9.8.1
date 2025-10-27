USE [EKM_App]
GO

/****** Object:  Table [dbo].[Phrases]    Script Date: 04/26/2013 20:38:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TABLE [dbo].[DCT_FileContents](
	[FileID] [uniqueidentifier] NOT NULL,
	[Content] [nvarchar](max) NULL,
	[NotExtractable] [bit] NOT NULL,
	[FileNotFound] [bit] NOT NULL,
	[Duration] [bigint] NOT NULL,
	[ExtractionDate] [datetime] NOT NULL,
	[IndexLastUpdateDate] [datetime] NULL
 CONSTRAINT [PK_DCT_FileContents] PRIMARY KEY CLUSTERED 
(
	[FileID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
