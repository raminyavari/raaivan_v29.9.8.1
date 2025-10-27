USE [EKM_App]
GO

/****** Object:  Table [dbo].[BulletinBoards]    Script Date: 02/17/2012 14:15:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BulletinBoardsTemp](
	[ID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](500) NOT NULL,
	[Description] [nvarchar](2000) NULL,
	[CreationDate] [datetime] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NOT NULL,
	[Status] [nvarchar](255) NULL,
 CONSTRAINT [PK_BulletinBoardsTemp] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[BulletinBoardsTemp]
           ([ID]
			,[NodeID]
			,[Title]
			,[Description]
			,[CreationDate]
			,[CreatorUserID]
			,[Status])
		SELECT  dbo.BulletinBoards.ID, dbo.CN_Nodes.NodeID, dbo.BulletinBoards.Title, dbo.BulletinBoards.Description,
				dbo.BulletinBoards.CreationDate, dbo.BulletinBoards.CreatorUserID, dbo.BulletinBoards.Status
		FROM    dbo.CN_Nodes INNER JOIN dbo.BulletinBoards ON dbo.CN_Nodes.ID = dbo.BulletinBoards.NodeID
GO


/* Drop old table */
DROP TABLE [dbo].[BulletinBoards]
GO

/* Create old new table */
CREATE TABLE [dbo].[BulletinBoards](
	[ID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](500) NOT NULL,
	[Description] [nvarchar](2000) NULL,
	[CreationDate] [datetime] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NOT NULL,
	[Status] [nvarchar](255) NULL,
 CONSTRAINT [PK_BulletinBoards] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[BulletinBoards]  WITH CHECK ADD  CONSTRAINT [FK_BulletinBoards_aspnet_Users] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[BulletinBoards] CHECK CONSTRAINT [FK_BulletinBoards_aspnet_Users]
GO

ALTER TABLE [dbo].[BulletinBoards]  WITH CHECK ADD  CONSTRAINT [FK_BulletinBoards_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[BulletinBoards] CHECK CONSTRAINT [FK_BulletinBoards_CN_Nodes]
GO


INSERT INTO [dbo].[BulletinBoards]
	SELECT * FROM dbo.BulletinBoardsTemp
GO

DROP TABLE [dbo].[BulletinBoardsTemp]
GO