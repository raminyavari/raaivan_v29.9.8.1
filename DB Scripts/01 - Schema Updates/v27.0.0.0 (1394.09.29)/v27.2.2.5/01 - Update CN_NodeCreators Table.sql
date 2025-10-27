USE [EKM_App]
GO

/****** Object:  Table [dbo].[CN_NodeCreators]    Script Date: 04/17/2016 09:38:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[CN_TMPNodeCreators](
	[NodeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[CollaborationShare] [float] NULL,
	[Status] [varchar](20) NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
	[UniqueID] [uniqueidentifier] NOT NULL
 CONSTRAINT [PK_CN_TMPNodeCreators] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

INSERT INTO [dbo].[CN_TMPNodeCreators](
	NodeID,
	UserID,
	CollaborationShare,
	[Status],
	CreatorUserID,
	CreationDate,
	LastModifierUserID,
	LastModificationDate,
	Deleted,
	UniqueID
)
SELECT NodeID, UserID, CollaborationShare, [Status], CreatorUserID, CreationDate,
	LastModifierUserID, LastModificationDate, Deleted, NEWID()
FROM [dbo].[CN_NodeCreators]

GO

DROP TABLE [dbo].[CN_NodeCreators]
GO


SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[CN_NodeCreators](
	[NodeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[CollaborationShare] [float] NULL,
	[Status] [varchar](20) NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
	[UniqueID] [uniqueidentifier] NOT NULL
 CONSTRAINT [PK_CN_NodeCreators] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


ALTER TABLE [dbo].[CN_NodeCreators]  WITH CHECK ADD  CONSTRAINT [FK_CN_NodeCreators_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_NodeCreators] CHECK CONSTRAINT [FK_CN_NodeCreators_aspnet_Users]
GO

ALTER TABLE [dbo].[CN_NodeCreators]  WITH CHECK ADD  CONSTRAINT [FK_CN_NodeCreators_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_NodeCreators] CHECK CONSTRAINT [FK_CN_NodeCreators_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[CN_NodeCreators]  WITH CHECK ADD  CONSTRAINT [FK_CN_NodeCreators_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_NodeCreators] CHECK CONSTRAINT [FK_CN_NodeCreators_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[CN_NodeCreators]  WITH CHECK ADD  CONSTRAINT [FK_CN_NodeCreators_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[CN_NodeCreators] CHECK CONSTRAINT [FK_CN_NodeCreators_CN_Nodes]
GO


INSERT INTO [dbo].[CN_NodeCreators]
SELECT * FROM [dbo].[CN_TMPNodeCreators]
GO


DROP TABLE [dbo].[CN_TMPNodeCreators]
GO