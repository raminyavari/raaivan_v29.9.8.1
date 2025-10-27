USE [EKM_App]
GO

/****** Object:  Table [dbo].[WallPostNode]    Script Date: 02/17/2012 15:37:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WallPostNodeTemp](
	[WallPostID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_WallPostNodeTemp] PRIMARY KEY CLUSTERED 
(
	[WallPostId] ASC,
	[NodeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[WallPostNodeTemp]
           ([WallPostID]
			,[NodeID])
		SELECT  dbo.WallPostNode.WallPostId, dbo.CN_Nodes.NodeID
		FROM    dbo.CN_Nodes INNER JOIN dbo.WallPostNode ON dbo.CN_Nodes.ID = dbo.WallPostNode.NodeId
GO


/* Drop old table */
DROP TABLE [dbo].[WallPostNode]
GO

/* Create old new table */
CREATE TABLE [dbo].[WallPostNode](
	[WallPostID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_WallPostNode] PRIMARY KEY CLUSTERED 
(
	[WallPostId] ASC,
	[NodeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[WallPostNode]  WITH CHECK ADD  CONSTRAINT [FK_WallPostNode_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[WallPostNode] CHECK CONSTRAINT [FK_WallPostNode_CN_Nodes]
GO

ALTER TABLE [dbo].[WallPostNode]  WITH CHECK ADD  CONSTRAINT [FK_WallPostNode_WallPosts] FOREIGN KEY([WallPostID])
REFERENCES [dbo].[WallPosts] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[WallPostNode] CHECK CONSTRAINT [FK_WallPostNode_WallPosts]
GO


INSERT INTO [dbo].[WallPostNode]
	SELECT * FROM dbo.WallPostNodeTemp
GO

DROP TABLE [dbo].[WallPostNodeTemp]
GO