USE [EKM_App]
GO

/****** Object:  Table [dbo].[WK_Changes]    Script Date: 10/28/2012 14:27:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[WK_TMP_Paragraphs](
	[ParagraphID] [uniqueidentifier] NOT NULL,
	[TitleID] [uniqueidentifier] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[SequenceNo] [int] NULL,
	[Title] [nvarchar](500) NULL,
	[BodyText] [nvarchar](max) NOT NULL,
	[IsRichText] [bit] NOT NULL,
	[Status] [varchar](20) NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_WK_TMP_Paragraphs] PRIMARY KEY CLUSTERED 
(
	[ParagraphID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[WK_TMP_Changes](
	[ChangeID] [uniqueidentifier] NOT NULL,
	[ParagraphID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[SendDate] [datetime] NOT NULL,
	[LastModificationDate] [datetime] NULL,
	[Title] [nvarchar](500) NULL,
	[BodyText] [nvarchar](max) NOT NULL,
	[Applied] [bit] NOT NULL,
	[ApplicationDate] [datetime] NULL,
	[Status] [varchar](20) NOT NULL,
	[AcceptionDate] [datetime] NULL,
	[EvaluatorUserID] [uniqueidentifier] NULL,
	[EvaluationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_WK_TMP_Changes] PRIMARY KEY CLUSTERED 
(
	[ChangeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


INSERT INTO [dbo].[WK_TMP_Paragraphs]
SELECT * FROM [dbo].[WK_Paragraphs]
GO


INSERT INTO [dbo].[WK_TMP_Changes]
SELECT * FROM [dbo].[WK_Changes]
GO




DROP TABLE [dbo].[WK_Changes]
GO

DROP TABLE [dbo].[WK_Paragraphs]
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[WK_Paragraphs](
	[ParagraphID] [uniqueidentifier] NOT NULL,
	[TitleID] [uniqueidentifier] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[SequenceNo] [int] NULL,
	[Title] [nvarchar](500) NULL,
	[BodyText] [nvarchar](max) NOT NULL,
	[IsRichText] [bit] NOT NULL,
	[Status] [varchar](20) NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_WK_Paragraphs] PRIMARY KEY CLUSTERED 
(
	[ParagraphID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[WK_Paragraphs]  WITH CHECK ADD  CONSTRAINT [FK_WK_Paragraphs_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WK_Paragraphs] CHECK CONSTRAINT [FK_WK_Paragraphs_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[WK_Paragraphs]  WITH CHECK ADD  CONSTRAINT [FK_WK_Paragraphs_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WK_Paragraphs] CHECK CONSTRAINT [FK_WK_Paragraphs_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[WK_Paragraphs]  WITH CHECK ADD  CONSTRAINT [FK_WK_Paragraphs_WK_Titles] FOREIGN KEY([TitleID])
REFERENCES [dbo].[WK_Titles] ([TitleID])
GO

ALTER TABLE [dbo].[WK_Paragraphs] CHECK CONSTRAINT [FK_WK_Paragraphs_WK_Titles]
GO


SET ANSI_PADDING ON
GO


CREATE TABLE [dbo].[WK_Changes](
	[ChangeID] [uniqueidentifier] NOT NULL,
	[ParagraphID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[SendDate] [datetime] NOT NULL,
	[LastModificationDate] [datetime] NULL,
	[Title] [nvarchar](500) NULL,
	[BodyText] [nvarchar](max) NOT NULL,
	[Applied] [bit] NOT NULL,
	[ApplicationDate] [datetime] NULL,
	[Status] [varchar](20) NOT NULL,
	[AcceptionDate] [datetime] NULL,
	[EvaluatorUserID] [uniqueidentifier] NULL,
	[EvaluationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_WK_Changes] PRIMARY KEY CLUSTERED 
(
	[ChangeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[WK_Changes]  WITH CHECK ADD  CONSTRAINT [FK_WK_Changes_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WK_Changes] CHECK CONSTRAINT [FK_WK_Changes_aspnet_Users]
GO

ALTER TABLE [dbo].[WK_Changes]  WITH CHECK ADD  CONSTRAINT [FK_WK_Changes_aspnet_Users_Evaluator] FOREIGN KEY([EvaluatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WK_Changes] CHECK CONSTRAINT [FK_WK_Changes_aspnet_Users_Evaluator]
GO

ALTER TABLE [dbo].[WK_Changes]  WITH CHECK ADD  CONSTRAINT [FK_WK_Changes_WK_Paragraphs] FOREIGN KEY([ParagraphID])
REFERENCES [dbo].[WK_Paragraphs] ([ParagraphID])
GO

ALTER TABLE [dbo].[WK_Changes] CHECK CONSTRAINT [FK_WK_Changes_WK_Paragraphs]
GO


INSERT INTO [dbo].[WK_Paragraphs]
SELECT * FROM [dbo].[WK_TMP_Paragraphs]
GO


INSERT INTO [dbo].[WK_Changes]
SELECT * FROM [dbo].[WK_TMP_Changes]
GO


DROP TABLE [dbo].[WK_TMP_Changes]
GO

DROP TABLE [dbo].[WK_TMP_Paragraphs]
GO