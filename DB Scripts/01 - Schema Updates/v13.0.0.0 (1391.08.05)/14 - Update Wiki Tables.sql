USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/****** Object:  Table [dbo].[WK_Titles]    Script Date: 09/23/2012 17:28:20 ******/
SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[WK_Titles](
	[TitleID] [uniqueidentifier] NOT NULL,
	[OwnerID] [uniqueidentifier] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[SequenceNo] [int] NULL,
	[Title] [nvarchar](500) NOT NULL,
	[Status] [varchar](20) NOT NULL,
	[OwnerType] [varchar](20) NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_WK_Titles] PRIMARY KEY CLUSTERED 
(
	[TitleID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[WK_Titles]  WITH CHECK ADD  CONSTRAINT [FK_WK_Titles_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WK_Titles] CHECK CONSTRAINT [FK_WK_Titles_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[WK_Titles]  WITH CHECK ADD  CONSTRAINT [FK_WK_Titles_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WK_Titles] CHECK CONSTRAINT [FK_WK_Titles_aspnet_Users_Modifier]
GO


/****** Object:  Table [dbo].[WK_Paragraphs]    Script Date: 09/23/2012 17:28:20 ******/
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
	[BodyText] [nvarchar](4000) NOT NULL,
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

ALTER TABLE [dbo].[WK_Paragraphs]  WITH CHECK ADD  CONSTRAINT [FK_WK_Paragraphs_WK_Titles] FOREIGN KEY([TitleID])
REFERENCES [dbo].[WK_Titles] ([TitleID])
GO

ALTER TABLE [dbo].[WK_Paragraphs] CHECK CONSTRAINT [FK_WK_Paragraphs_WK_Titles]
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


/****** Object:  Table [dbo].[WK_TempEvaluators]    Script Date: 09/23/2012 17:38:15 ******/
CREATE TABLE [dbo].[WK_TempEvaluators](
	[UserID] [uniqueidentifier] NOT NULL,
	[ParagraphID] [uniqueidentifier] NOT NULL,
	[SenderUserID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_WK_TempEvaluators] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[ParagraphID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[WK_TempEvaluators]  WITH CHECK ADD  CONSTRAINT [FK_WK_TempEvaluators_aspnet_Users_Evaluator] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WK_TempEvaluators] CHECK CONSTRAINT [FK_WK_TempEvaluators_aspnet_Users_Evaluator]
GO

ALTER TABLE [dbo].[WK_TempEvaluators]  WITH CHECK ADD  CONSTRAINT [FK_WK_TempEvaluators_aspnet_Users_Sender] FOREIGN KEY([SenderUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WK_TempEvaluators] CHECK CONSTRAINT [FK_WK_TempEvaluators_aspnet_Users_Sender]
GO

ALTER TABLE [dbo].[WK_TempEvaluators]  WITH CHECK ADD  CONSTRAINT [FK_WK_TempEvaluators_WK_Paragraphs] FOREIGN KEY([ParagraphID])
REFERENCES [dbo].[WK_Paragraphs] ([ParagraphID])
GO

ALTER TABLE [dbo].[WK_TempEvaluators] CHECK CONSTRAINT [FK_WK_TempEvaluators_WK_Paragraphs]
GO


/****** Object:  Table [dbo].[WK_TempChanges]    Script Date: 09/23/2012 17:41:02 ******/
SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[WK_TempChanges](
	[ChangeID] [bigint] IDENTITY(1,1) NOT NULL,
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
 CONSTRAINT [PK_WK_TempChanges] PRIMARY KEY CLUSTERED 
(
	[ChangeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[WK_TempChanges]  WITH CHECK ADD  CONSTRAINT [FK_WK_TempChanges_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WK_TempChanges] CHECK CONSTRAINT [FK_WK_TempChanges_aspnet_Users]
GO

ALTER TABLE [dbo].[WK_TempChanges]  WITH CHECK ADD  CONSTRAINT [FK_WK_TempChanges_aspnet_Users_Evaluator] FOREIGN KEY([EvaluatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WK_TempChanges] CHECK CONSTRAINT [FK_WK_TempChanges_aspnet_Users_Evaluator]
GO

ALTER TABLE [dbo].[WK_TempChanges]  WITH CHECK ADD  CONSTRAINT [FK_WK_TempChanges_WK_Paragraphs] FOREIGN KEY([ParagraphID])
REFERENCES [dbo].[WK_Paragraphs] ([ParagraphID])
GO

ALTER TABLE [dbo].[WK_TempChanges] CHECK CONSTRAINT [FK_WK_TempChanges_WK_Paragraphs]
GO


/* Fill New Tables */
ALTER TABLE [dbo].[WK_Subjects]
ADD [TitleID] [uniqueidentifier] NULL
GO

UPDATE [dbo].[WK_Subjects]
	SET TitleID = NEWID()
GO

INSERT INTO [WK_Titles](
	TitleID,
	OwnerID,
	CreatorUserID,
	CreationDate,
	LastModifierUserID,
	LastModificationDate,
	SequenceNo,
	Title,
	Status,
	OwnerType,
	Deleted
)
SELECT Ref.TitleID, Ref.OwnerID, Ref.CreatorUserID, Ref.CreationDate, 
	Ref.LastModifierUserID, Ref.LastModificationDate, Ref.SequenceNo, Ref.Title,
	Ref.Status, Ref.OwnerType, Ref.Deleted
FROM [dbo].[WK_Subjects] AS Ref
GO

INSERT INTO [WK_Paragraphs](
	ParagraphID,
	TitleID,
	CreatorUserID,
	CreationDate,
	LastModifierUserID,
	LastModificationDate,
	SequenceNo,
	BodyText,
	IsRichText,
	Status,
	Deleted
)
SELECT Ref.SubjectID, Ref.TitleID, Ref.CreatorUserID, Ref.CreationDate, 
	Ref.LastModifierUserID, Ref.LastModificationDate, Ref.SequenceNo, Ref.Description,
	1, Ref.Status, Ref.Deleted
FROM [dbo].[WK_Subjects] AS Ref
GO

INSERT INTO [WK_TempEvaluators](
	UserID,
	ParagraphID,
	SenderUserID
)
SELECT Ref.UserID, Ref.SubjectID, Ref.SenderUserID
FROM [dbo].[WK_Evaluators] AS Ref
GO

INSERT INTO [WK_TempChanges](
	ParagraphID,
	UserID,
	SendDate,
	LastModificationDate,
	BodyText,
	Applied,
	ApplicationDate,
	Status,
	AcceptionDate,
	EvaluatorUserID,
	EvaluationDate,
	Deleted
)
SELECT Ref.SubjectID, Ref.UserID, Ref.SendDate, Ref.LastModificationDate, Ref.Description,
	Ref.Applied, Ref.ApplicationDate, Ref.Status, Ref.AcceptionDate, Ref.EvaluatorUserID,
	Ref.EvaluationDate, Ref.Deleted
FROM [dbo].[WK_Changes] AS Ref
GO
/* end of Fill New Tables */


DROP TABLE [dbo].[WK_Changes]
GO

DROP TABLE [dbo].[WK_Evaluators]
GO

DROP TABLE [dbo].[WK_Subjects]
GO


/****** Object:  Table [dbo].[WK_Evaluators]    Script Date: 09/23/2012 17:38:15 ******/
CREATE TABLE [dbo].[WK_Evaluators](
	[UserID] [uniqueidentifier] NOT NULL,
	[ParagraphID] [uniqueidentifier] NOT NULL,
	[SenderUserID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_WK_Evaluators] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[ParagraphID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[WK_Evaluators]  WITH CHECK ADD  CONSTRAINT [FK_WK_Evaluators_aspnet_Users_Evaluator] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WK_Evaluators] CHECK CONSTRAINT [FK_WK_Evaluators_aspnet_Users_Evaluator]
GO

ALTER TABLE [dbo].[WK_Evaluators]  WITH CHECK ADD  CONSTRAINT [FK_WK_Evaluators_aspnet_Users_Sender] FOREIGN KEY([SenderUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WK_Evaluators] CHECK CONSTRAINT [FK_WK_Evaluators_aspnet_Users_Sender]
GO

ALTER TABLE [dbo].[WK_Evaluators]  WITH CHECK ADD  CONSTRAINT [FK_WK_Evaluators_WK_Paragraphs] FOREIGN KEY([ParagraphID])
REFERENCES [dbo].[WK_Paragraphs] ([ParagraphID])
GO

ALTER TABLE [dbo].[WK_Evaluators] CHECK CONSTRAINT [FK_WK_Evaluators_WK_Paragraphs]
GO


/****** Object:  Table [dbo].[WK_Changes]    Script Date: 09/23/2012 17:41:02 ******/
SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[WK_Changes](
	[ChangeID] [bigint] IDENTITY(1,1) NOT NULL,
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



INSERT INTO [WK_Evaluators](
	UserID,
	ParagraphID,
	SenderUserID
)
SELECT Ref.UserID, Ref.ParagraphID, Ref.SenderUserID
FROM [dbo].[WK_TempEvaluators] AS Ref
GO

INSERT INTO [WK_Changes](
	ParagraphID,
	UserID,
	SendDate,
	LastModificationDate,
	BodyText,
	Applied,
	ApplicationDate,
	Status,
	AcceptionDate,
	EvaluatorUserID,
	EvaluationDate,
	Deleted
)
SELECT Ref.ParagraphID, Ref.UserID, Ref.SendDate, Ref.LastModificationDate, Ref.BodyText,
	Ref.Applied, Ref.ApplicationDate, Ref.Status, Ref.AcceptionDate, Ref.EvaluatorUserID,
	Ref.EvaluationDate, Ref.Deleted
FROM [dbo].[WK_TempChanges] AS Ref
GO


DROP TABLE [dbo].[WK_TempEvaluators]
GO

DROP TABLE [dbo].[WK_TempChanges]
GO



-- Update Sequence Numbers
DECLARE @SequenceNos Table(pid uniqueidentifier primary key, sn int identity(1,1))

INSERT INTO @SequenceNos
SELECT ParagraphID
FROM [dbo].[WK_Paragraphs]

UPDATE [dbo].[WK_Paragraphs]
	SET SequenceNo = Ref.sn
FROM @SequenceNos AS Ref
	INNER JOIN [dbo].[WK_Paragraphs]
	ON Ref.pid = [dbo].[WK_Paragraphs].[ParagraphID]
	

DELETE FROM @SequenceNos
	
INSERT INTO @SequenceNos
SELECT TitleID
FROM [dbo].[WK_Titles]

UPDATE [dbo].[WK_Titles]
	SET SequenceNo = Ref.sn
FROM @SequenceNos AS Ref
	INNER JOIN [dbo].[WK_Titles]
	ON Ref.pid = [dbo].[WK_Titles].[TitleID]
GO
-- end of Update Sequence Numbers


DROP TABLE [dbo].[WK_Evaluators]
GO