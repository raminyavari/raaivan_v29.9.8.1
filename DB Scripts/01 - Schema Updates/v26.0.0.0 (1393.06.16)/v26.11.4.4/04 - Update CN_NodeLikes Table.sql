USE [EKM_App]
GO

/****** Object:  Table [dbo].[CN_NodeLikes]    Script Date: 08/01/2015 20:15:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[CN_TMPNodeLikes](
	[NodeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[LikeDate] [datetime] NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_CN_TMPNodeLikes] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[CN_TMPNodeLikes](
	NodeID,
	UserID,
	LikeDate,
	Deleted
)
SELECT NodeID, UserID, LikeDate, Deleted
FROM [dbo].[CN_NodeLikes]

GO


DROP TABLE [dbo].[CN_NodeLikes]
GO


CREATE TABLE [dbo].[CN_NodeLikes](
	[NodeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[LikeDate] [datetime] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[UniqueID] [uniqueidentifier] NOT NULL
 CONSTRAINT [PK_CN_NodeLikes] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[CN_NodeLikes](
	NodeID,
	UserID,
	LikeDate,
	Deleted,
	UniqueID
)
SELECT NodeID, UserID, LikeDate, Deleted, NEWID()
FROM [dbo].[CN_TMPNodeLikes]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[CN_NodeLikes]  WITH CHECK ADD  CONSTRAINT [FK_CN_NodeLikes_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_NodeLikes] CHECK CONSTRAINT [FK_CN_NodeLikes_aspnet_Users]
GO

ALTER TABLE [dbo].[CN_NodeLikes]  WITH CHECK ADD  CONSTRAINT [FK_CN_NodeLikes_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[CN_NodeLikes] CHECK CONSTRAINT [FK_CN_NodeLikes_CN_Nodes]
GO


DROP TABLE [dbo].[CN_TMPNodeLikes]
GO