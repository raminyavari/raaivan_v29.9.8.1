USE [EKM_App]
GO

/****** Object:  Table [dbo].[WK_Changes]    Script Date: 10/20/2012 10:42:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
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
	[BodyText] [nvarchar](4000) NOT NULL,
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


INSERT INTO [dbo].[WK_TMP_Changes](
	ChangeID,
	ParagraphID,
	UserID,
	SendDate,
	LastModificationDate,
	Title,
	BodyText,
	Applied,
	ApplicationDate,
	Status,
	AcceptionDate,
	EvaluatorUserID,
	EvaluationDate,
	Deleted
)
SELECT NEWID(), ParagraphID, UserID, SendDate, LastModificationDate, Title,
	BodyText, Applied, ApplicationDate, Status, AcceptionDate, EvaluatorUserID, 
	EvaluationDate, Deleted
FROM [dbo].[WK_Changes]
GO


DROP TABLE [dbo].[WK_Changes]
GO


CREATE TABLE [dbo].[WK_Changes](
	[ChangeID] [uniqueidentifier] NOT NULL,
	[ParagraphID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[SendDate] [datetime] NOT NULL,
	[LastModificationDate] [datetime] NULL,
	[Title] [nvarchar](500) NULL,
	[BodyText] [nvarchar](4000) NOT NULL,
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


INSERT INTO [dbo].[WK_Changes]
SELECT * FROM [dbo].[WK_TMP_Changes]
GO


DROP TABLE [dbo].[WK_TMP_Changes]
GO