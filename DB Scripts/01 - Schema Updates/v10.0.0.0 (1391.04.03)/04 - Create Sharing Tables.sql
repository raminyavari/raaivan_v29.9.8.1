USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/****** Object:  Table [dbo].[SH_PostTypes]    Script Date: 05/31/2012 21:32:31 ******/
CREATE TABLE [dbo].[SH_PostTypes](
	[PostTypeID] [int] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[PersianName] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_SH_PostTypes] PRIMARY KEY CLUSTERED 
(
	[PostTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO



/****** Object:  Table [dbo].[SH_Posts]    Script Date: 05/31/2012 21:32:31 ******/
CREATE TABLE [dbo].[SH_Posts](
	[PostID] [uniqueidentifier] NOT NULL,
	[PostTypeID] [int] NOT NULL,
	[Description] [nvarchar](4000) NULL,
	[SharedObjectID] [uniqueidentifier] NULL,
	[SenderUserID] [uniqueidentifier] NULL,
	[SendDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_SH_Posts] PRIMARY KEY CLUSTERED 
(
	[PostID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[SH_Posts]  WITH CHECK ADD  CONSTRAINT [FK_SH_Posts_ProfileCommon_Sender] FOREIGN KEY([SenderUserID])
REFERENCES [dbo].[ProfileCommon] ([UserId])
GO

ALTER TABLE [dbo].[SH_Posts] CHECK CONSTRAINT [FK_SH_Posts_ProfileCommon_Sender]
GO

ALTER TABLE [dbo].[SH_Posts]  WITH CHECK ADD  CONSTRAINT [FK_SH_Posts_ProfileCommon_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[ProfileCommon] ([UserId])
GO

ALTER TABLE [dbo].[SH_Posts] CHECK CONSTRAINT [FK_SH_Posts_ProfileCommon_Modifier]
GO



/****** Object:  Table [dbo].[SH_PostShares]    Script Date: 05/31/2012 21:32:31 ******/
CREATE TABLE [dbo].[SH_PostShares](
	[ShareID] [uniqueidentifier] NOT NULL,
	[ParentShareID] [uniqueidentifier] NULL,
	[PostID] [uniqueidentifier] NOT NULL,
	[OwnerID] [uniqueidentifier] NOT NULL,
	[Description] [nvarchar](4000) NULL,
	[SenderUserID] [uniqueidentifier] NULL,
	[SendDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Privacy] [varchar](20) NOT NULL,
	[OwnerType] [varchar](20) NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_SH_PostShares] PRIMARY KEY CLUSTERED 
(
	[ShareID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[SH_PostShares]  WITH CHECK ADD  CONSTRAINT [FK_SH_PostShares_SH_PostShares] FOREIGN KEY([ParentShareID])
REFERENCES [dbo].[SH_PostShares] ([ShareID])
GO

ALTER TABLE [dbo].[SH_PostShares] CHECK CONSTRAINT [FK_SH_PostShares_SH_PostShares]
GO

ALTER TABLE [dbo].[SH_PostShares]  WITH CHECK ADD  CONSTRAINT [FK_SH_PostShares_SH_Posts] FOREIGN KEY([PostID])
REFERENCES [dbo].[SH_Posts] ([PostID])
GO

ALTER TABLE [dbo].[SH_PostShares] CHECK CONSTRAINT [FK_SH_PostShares_SH_Posts]
GO

ALTER TABLE [dbo].[SH_PostShares]  WITH CHECK ADD  CONSTRAINT [FK_SH_PostShares_ProfileCommon_Sender] FOREIGN KEY([SenderUserID])
REFERENCES [dbo].[ProfileCommon] ([UserId])
GO

ALTER TABLE [dbo].[SH_PostShares] CHECK CONSTRAINT [FK_SH_PostShares_ProfileCommon_Sender]
GO

ALTER TABLE [dbo].[SH_PostShares]  WITH CHECK ADD  CONSTRAINT [FK_SH_PostShares_ProfileCommon_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[ProfileCommon] ([UserId])
GO

ALTER TABLE [dbo].[SH_PostShares] CHECK CONSTRAINT [FK_SH_PostShares_ProfileCommon_Modifier]
GO



/****** Object:  Table [dbo].[SH_ShareAudience]    Script Date: 05/31/2012 21:32:31 ******/
CREATE TABLE [dbo].[SH_ShareAudience](
	[ShareID] [uniqueidentifier] NOT NULL,
	[AudienceID] [uniqueidentifier] NOT NULL,
	[AudienceType] [varchar](20) NOT NULL,
	[Deny] [bit] NOT NULL,
 CONSTRAINT [PK_SH_ShareAudience] PRIMARY KEY CLUSTERED 
(
	[ShareID] ASC,
	[AudienceID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[SH_ShareAudience]  WITH CHECK ADD  CONSTRAINT [FK_SH_ShareAudience_SH_PostShares] FOREIGN KEY([ShareID])
REFERENCES [dbo].[SH_PostShares] ([ShareID])
GO

ALTER TABLE [dbo].[SH_ShareAudience] CHECK CONSTRAINT [FK_SH_ShareAudience_SH_PostShares]
GO



/****** Object:  Table [dbo].[SH_Comments]    Script Date: 05/31/2012 21:32:31 ******/
CREATE TABLE [dbo].[SH_Comments](
	[CommentID] [uniqueidentifier] NOT NULL,
	[ShareID] [uniqueidentifier] NOT NULL,
	[Description] [nvarchar](4000) NULL,
	[SenderUserID] [uniqueidentifier] NULL,
	[SendDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_SH_Comments] PRIMARY KEY CLUSTERED 
(
	[CommentID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[SH_Comments]  WITH CHECK ADD  CONSTRAINT [FK_SH_Comments_SH_PostShares] FOREIGN KEY([ShareID])
REFERENCES [dbo].[SH_PostShares] ([ShareID])
GO

ALTER TABLE [dbo].[SH_Comments] CHECK CONSTRAINT [FK_SH_Comments_SH_PostShares]
GO

ALTER TABLE [dbo].[SH_Comments]  WITH CHECK ADD  CONSTRAINT [FK_SH_Comments_ProfileCommon_Sender] FOREIGN KEY([SenderUserID])
REFERENCES [dbo].[ProfileCommon] ([UserId])
GO

ALTER TABLE [dbo].[SH_Comments] CHECK CONSTRAINT [FK_SH_Comments_ProfileCommon_Sender]
GO

ALTER TABLE [dbo].[SH_Comments]  WITH CHECK ADD  CONSTRAINT [FK_SH_Comments_ProfileCommon_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[ProfileCommon] ([UserId])
GO

ALTER TABLE [dbo].[SH_Comments] CHECK CONSTRAINT [FK_SH_Comments_ProfileCommon_Modifier]
GO



/****** Object:  Table [dbo].[SH_ShareLikes]    Script Date: 05/31/2012 21:32:31 ******/
CREATE TABLE [dbo].[SH_ShareLikes](
	[ShareID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[Like] [bit] NOT NULL,
	[Score] [float] NOT NULL,
	[Date] [datetime] NOT NULL,
 CONSTRAINT [PK_SH_ShareLikes] PRIMARY KEY CLUSTERED 
(
	[ShareID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[SH_ShareLikes]  WITH CHECK ADD  CONSTRAINT [FK_SH_ShareLikes_SH_PostShares] FOREIGN KEY([ShareID])
REFERENCES [dbo].[SH_PostShares] ([ShareID])
GO

ALTER TABLE [dbo].[SH_ShareLikes] CHECK CONSTRAINT [FK_SH_ShareLikes_SH_PostShares]
GO

ALTER TABLE [dbo].[SH_ShareLikes]  WITH CHECK ADD  CONSTRAINT [FK_SH_ShareLikes_ProfileCommon] FOREIGN KEY([UserID])
REFERENCES [dbo].[ProfileCommon] ([UserId])
GO

ALTER TABLE [dbo].[SH_ShareLikes] CHECK CONSTRAINT [FK_SH_ShareLikes_ProfileCommon]
GO



/****** Object:  Table [dbo].[SH_CommentLikes]    Script Date: 05/31/2012 21:32:31 ******/
CREATE TABLE [dbo].[SH_CommentLikes](
	[CommentID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[Like] [bit] NOT NULL,
	[Score] [float] NOT NULL,
	[Date] [datetime] NOT NULL,
 CONSTRAINT [PK_SH_CommentLikes] PRIMARY KEY CLUSTERED 
(
	[CommentID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[SH_CommentLikes]  WITH CHECK ADD  CONSTRAINT [FK_SH_CommentLikes_SH_Comments] FOREIGN KEY([CommentID])
REFERENCES [dbo].[SH_Comments] ([CommentID])
GO

ALTER TABLE [dbo].[SH_CommentLikes] CHECK CONSTRAINT [FK_SH_CommentLikes_SH_Comments]
GO

ALTER TABLE [dbo].[SH_CommentLikes]  WITH CHECK ADD  CONSTRAINT [FK_SH_CommentLikes_ProfileCommon] FOREIGN KEY([UserID])
REFERENCES [dbo].[ProfileCommon] ([UserId])
GO

ALTER TABLE [dbo].[SH_CommentLikes] CHECK CONSTRAINT [FK_SH_CommentLikes_ProfileCommon]
GO