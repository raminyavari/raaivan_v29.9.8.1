USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/****** Object:  Table [dbo].[WK_Subjects]    Script Date: 07/25/2012 13:21:13 ******/
CREATE TABLE [dbo].[WK_Subjects](
	[SubjectID] [uniqueidentifier] NOT NULL,
	[OwnerID] [uniqueidentifier] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[SequenceNo] [int] NULL,
	[Title] [nvarchar](500) NOT NULL,
	[Description] [nvarchar](4000) NULL,
	[Status] [varchar](20) NOT NULL,
	[OwnerType] [varchar](20) NOT NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_WK_Subjects] PRIMARY KEY CLUSTERED 
(
	[SubjectID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[WK_Subjects]  WITH CHECK ADD  CONSTRAINT [FK_WK_Subjects_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WK_Subjects] CHECK CONSTRAINT [FK_WK_Subjects_aspnet_Users_Creator]
GO


ALTER TABLE [dbo].[WK_Subjects]  WITH CHECK ADD  CONSTRAINT [FK_WK_Subjects_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WK_Subjects] CHECK CONSTRAINT [FK_WK_Subjects_aspnet_Users_Modifier]
GO



/****** Object:  Table [dbo].[WK_SuggestedChanges]    Script Date: 07/25/2012 13:34:52 ******/
CREATE TABLE [dbo].[WK_Changes](
	[ChangeID] [bigint] NOT NULL IDENTITY(1,1),
	[SubjectID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[SendDate] [datetime] NOT NULL,
	[LastModificationDate] [datetime] NULL,
	[Title] [nvarchar](500) NULL,
	[Description] [nvarchar](4000) NOT NULL,
	[Applied] [bit] NOT NULL,
	[ApplicationDate] [datetime] NULL,
	[Status] [varchar](20) NOT NULL,
	[AcceptionDate] [datetime] NULL,
	[EvaluatorUserID] [uniqueidentifier] NULL,
	[EvaluationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_WK_Changes] PRIMARY KEY CLUSTERED 
(
	[ChangeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[WK_Changes]  WITH CHECK ADD  CONSTRAINT [FK_WK_Changes_WK_Subjects] FOREIGN KEY([SubjectID])
REFERENCES [dbo].[WK_Subjects] ([SubjectID])
GO

ALTER TABLE [dbo].[WK_Changes] CHECK CONSTRAINT [FK_WK_Changes_WK_Subjects]
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



/****** Object:  Table [dbo].[WK_Evaluators]    Script Date: 07/25/2012 14:07:16 ******/
CREATE TABLE [dbo].[WK_Evaluators](
	[UserID] [uniqueidentifier] NOT NULL,
	[SubjectID] [uniqueidentifier] NOT NULL,
	[SenderUserID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_WK_Evaluators] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[SubjectID] ASC
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

ALTER TABLE [dbo].[WK_Evaluators]  WITH CHECK ADD  CONSTRAINT [FK_WK_Evaluators_WK_Subjects] FOREIGN KEY([SubjectID])
REFERENCES [dbo].[WK_Subjects] ([SubjectID])
GO

ALTER TABLE [dbo].[WK_Evaluators] CHECK CONSTRAINT [FK_WK_Evaluators_WK_Subjects]
GO


