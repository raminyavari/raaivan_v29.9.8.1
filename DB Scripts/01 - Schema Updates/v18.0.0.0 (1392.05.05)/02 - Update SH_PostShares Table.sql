USE [EKM_App]
GO

/****** Object:  Table [dbo].[SH_PostShares]    Script Date: 07/21/2013 10:31:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE [dbo].[SH_ShareAudience]
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[SH_TMPPostShares](
	[ShareID] [uniqueidentifier] NOT NULL,
	[ParentShareID] [uniqueidentifier] NULL,
	[PostID] [uniqueidentifier] NOT NULL,
	[OwnerID] [uniqueidentifier] NOT NULL,
	[Description] [nvarchar](4000) NULL,
	[SenderUserID] [uniqueidentifier] NULL,
	[SendDate] [datetime] NOT NULL,
	[ScoreDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Privacy] [varchar](20) NOT NULL,
	[OwnerType] [varchar](20) NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_SH_TMPPostShares] PRIMARY KEY CLUSTERED 
(
	[ShareID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


INSERT INTO [dbo].[SH_TMPPostShares](
	ShareID,
	ParentShareID,
	PostID,
	OwnerID,
	[Description],
	SenderUserID,
	SendDate,
	ScoreDate,
	LastModifierUserID,
	LastModificationDate,
	Privacy,
	OwnerType,
	Deleted
)
SELECT ShareID, ParentShareID, PostID, OwnerID, [Description], SenderUserID, SendDate,
	SendDate, LastModifierUserID, LastModificationDate, Privacy, OwnerType, Deleted
FROM [dbo].[SH_PostShares]

GO


ALTER TABLE [dbo].[SH_ShareLikes]
DROP CONSTRAINT [FK_SH_ShareLikes_SH_PostShares]
GO


ALTER TABLE [dbo].[SH_Comments]
DROP CONSTRAINT [FK_SH_Comments_SH_PostShares]
GO


DROP TABLE [dbo].[SH_PostShares]
GO


SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[SH_PostShares](
	[ShareID] [uniqueidentifier] NOT NULL,
	[ParentShareID] [uniqueidentifier] NULL,
	[PostID] [uniqueidentifier] NOT NULL,
	[OwnerID] [uniqueidentifier] NOT NULL,
	[Description] [nvarchar](4000) NULL,
	[SenderUserID] [uniqueidentifier] NULL,
	[SendDate] [datetime] NOT NULL,
	[ScoreDate] [datetime] NOT NULL,
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

SET ANSI_PADDING OFF
GO


ALTER TABLE [dbo].[SH_PostShares]  WITH CHECK ADD  CONSTRAINT [FK_SH_PostShares_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[SH_PostShares] CHECK CONSTRAINT [FK_SH_PostShares_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[SH_PostShares]  WITH CHECK ADD  CONSTRAINT [FK_SH_PostShares_aspnet_Users_Sender] FOREIGN KEY([SenderUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[SH_PostShares] CHECK CONSTRAINT [FK_SH_PostShares_aspnet_Users_Sender]
GO

ALTER TABLE [dbo].[SH_PostShares]  WITH CHECK ADD  CONSTRAINT [FK_SH_PostShares_SH_Posts] FOREIGN KEY([PostID])
REFERENCES [dbo].[SH_Posts] ([PostID])
GO

ALTER TABLE [dbo].[SH_PostShares] CHECK CONSTRAINT [FK_SH_PostShares_SH_Posts]
GO

ALTER TABLE [dbo].[SH_PostShares]  WITH CHECK ADD  CONSTRAINT [FK_SH_PostShares_SH_PostShares] FOREIGN KEY([ParentShareID])
REFERENCES [dbo].[SH_PostShares] ([ShareID])
GO

ALTER TABLE [dbo].[SH_PostShares] CHECK CONSTRAINT [FK_SH_PostShares_SH_PostShares]
GO


INSERT INTO [dbo].[SH_PostShares]
SELECT * FROM [dbo].[SH_TMPPostShares]
GO


ALTER TABLE [dbo].[SH_ShareLikes]  WITH CHECK ADD  CONSTRAINT [FK_SH_ShareLikes_SH_PostShares] FOREIGN KEY([ShareID])
REFERENCES [dbo].[SH_PostShares] ([ShareID])
GO

ALTER TABLE [dbo].[SH_ShareLikes] CHECK CONSTRAINT [FK_SH_ShareLikes_SH_PostShares]
GO


ALTER TABLE [dbo].[SH_Comments]  WITH CHECK ADD  CONSTRAINT [FK_SH_Comments_SH_PostShares] FOREIGN KEY([ShareID])
REFERENCES [dbo].[SH_PostShares] ([ShareID])
GO

ALTER TABLE [dbo].[SH_Comments] CHECK CONSTRAINT [FK_SH_Comments_SH_PostShares]
GO


DROP TABLE [dbo].[SH_TMPPostShares]
GO