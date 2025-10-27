USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO


/****** Object:  Table [dbo].[QA_Questions]    Script Date: 06/30/2012 22:54:53 ******/
CREATE TABLE [dbo].[QA_Questions](
	[QuestionID] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](500) NOT NULL,
	[Description] [nvarchar](2000) NULL,
	[SenderUserID] [uniqueidentifier] NOT NULL,
	[SendDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Status] [varchar](20) NOT NULL,
	[AcceptionDate] [datetime] NULL,
	[VisitsCount] [int] NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_QA_Questions] PRIMARY KEY CLUSTERED 
(
	[QuestionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[QA_Questions]  WITH CHECK ADD  CONSTRAINT [FK_QA_Questions_ProfileCommon_Sender] FOREIGN KEY([SenderUserID])
REFERENCES [dbo].[ProfileCommon] ([UserId])
GO

ALTER TABLE [dbo].[QA_Questions] CHECK CONSTRAINT [FK_QA_Questions_ProfileCommon_Sender]
GO


ALTER TABLE [dbo].[QA_Questions]  WITH CHECK ADD  CONSTRAINT [FK_QA_Questions_ProfileCommon_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[ProfileCommon] ([UserId])
GO

ALTER TABLE [dbo].[QA_Questions] CHECK CONSTRAINT [FK_QA_Questions_ProfileCommon_Modifier]
GO


/****** Object:  Table [dbo].[QA_Answers]    Script Date: 06/30/2012 22:55:50 ******/
CREATE TABLE [dbo].[QA_Answers](
	[AnswerID] [uniqueidentifier] NOT NULL,
	[QuestionID] [uniqueidentifier] NOT NULL,
	[SenderUserID] [uniqueidentifier] NOT NULL,
	[SendDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[AnswerBody] [nvarchar](max) NOT NULL,
	[Status] [varchar](20) NOT NULL,
	[AcceptionDate] [datetime] NULL,
	[Rate] [int] NOT NULL,
	[NodeID] [uniqueidentifier] NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_QA_Answers] PRIMARY KEY CLUSTERED 
(
	[AnswerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[QA_Answers]  WITH CHECK ADD  CONSTRAINT [FK_QA_Answers_QA_Questions] FOREIGN KEY([QuestionID])
REFERENCES [dbo].[QA_Questions] ([QuestionID])
GO

ALTER TABLE [dbo].[QA_Answers] CHECK CONSTRAINT [FK_QA_Answers_QA_Questions]
GO


ALTER TABLE [dbo].[QA_Answers]  WITH CHECK ADD  CONSTRAINT [FK_QA_Answers_ProfileCommon_Sender] FOREIGN KEY([SenderUserID])
REFERENCES [dbo].[ProfileCommon] ([UserId])
GO

ALTER TABLE [dbo].[QA_Answers] CHECK CONSTRAINT [FK_QA_Answers_ProfileCommon_Sender]
GO


ALTER TABLE [dbo].[QA_Answers]  WITH CHECK ADD  CONSTRAINT [FK_QA_Answers_ProfileCommon_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[ProfileCommon] ([UserId])
GO

ALTER TABLE [dbo].[QA_Answers] CHECK CONSTRAINT [FK_QA_Answers_ProfileCommon_Modifier]
GO


ALTER TABLE [dbo].[QA_Answers]  WITH CHECK ADD  CONSTRAINT [FK_QA_Answers_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[QA_Answers] CHECK CONSTRAINT [FK_QA_Answers_CN_Nodes]
GO


/****** Object:  Table [dbo].[QA_QuestionLikes]    Script Date: 06/30/2012 22:56:30 ******/
CREATE TABLE [dbo].[QA_QuestionLikes](
	[QuestionID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[Like] [bit] NOT NULL,
	[Score] [float] NOT NULL,
	[Date] [datetime] NOT NULL,
 CONSTRAINT [PK_QA_QuestionLikes] PRIMARY KEY CLUSTERED 
(
	[QuestionID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[QA_QuestionLikes]  WITH CHECK ADD  CONSTRAINT [FK_QA_QuestionLikes_QA_Questions] FOREIGN KEY([QuestionID])
REFERENCES [dbo].[QA_Questions] ([QuestionID])
GO

ALTER TABLE [dbo].[QA_QuestionLikes] CHECK CONSTRAINT [FK_QA_QuestionLikes_QA_Questions]
GO


ALTER TABLE [dbo].[QA_QuestionLikes]  WITH CHECK ADD  CONSTRAINT [FK_QA_QuestionLikes_ProfileCommon] FOREIGN KEY([UserID])
REFERENCES [dbo].[ProfileCommon] ([UserId])
GO

ALTER TABLE [dbo].[QA_QuestionLikes] CHECK CONSTRAINT [FK_QA_QuestionLikes_ProfileCommon]
GO


/****** Object:  Table [dbo].[QA_RefNodes]    Script Date: 06/30/2012 22:56:57 ******/
CREATE TABLE [dbo].[QA_RefNodes](
	[NodeID] [uniqueidentifier] NOT NULL,
	[QuestionID] [uniqueidentifier] NOT NULL,
	[SendDate] [datetime] NOT NULL,
 CONSTRAINT [PK_QA_RefNodes] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC,
	[QuestionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[QA_RefNodes]  WITH CHECK ADD  CONSTRAINT [FK_QA_RefNodes_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[QA_RefNodes] CHECK CONSTRAINT [FK_QA_RefNodes_CN_Nodes]
GO

ALTER TABLE [dbo].[QA_RefNodes]  WITH CHECK ADD  CONSTRAINT [FK_QA_RefNodes_QA_Questions] FOREIGN KEY([QuestionID])
REFERENCES [dbo].[QA_Questions] ([QuestionID])
GO

ALTER TABLE [dbo].[QA_RefNodes] CHECK CONSTRAINT [FK_QA_RefNodes_QA_Questions]
GO


/****** Object:  Table [dbo].[QA_RefUsers]    Script Date: 06/30/2012 22:57:17 ******/
CREATE TABLE [dbo].[QA_RefUsers](
	[UserID] [uniqueidentifier] NOT NULL,
	[QuestionID] [uniqueidentifier] NOT NULL,
	[SendDate] [datetime] NOT NULL,
	[Seen] [bit] NOT NULL,
 CONSTRAINT [PK_QA_RefUsers] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[QuestionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[QA_RefUsers]  WITH CHECK ADD  CONSTRAINT [FK_QA_RefUsers_ProfileCommon] FOREIGN KEY([UserID])
REFERENCES [dbo].[ProfileCommon] ([UserId])
GO

ALTER TABLE [dbo].[QA_RefUsers] CHECK CONSTRAINT [FK_QA_RefUsers_ProfileCommon]
GO

ALTER TABLE [dbo].[QA_RefUsers]  WITH CHECK ADD  CONSTRAINT [FK_QA_RefUsers_QA_Questions] FOREIGN KEY([QuestionID])
REFERENCES [dbo].[QA_Questions] ([QuestionID])
GO

ALTER TABLE [dbo].[QA_RefUsers] CHECK CONSTRAINT [FK_QA_RefUsers_QA_Questions]
GO


