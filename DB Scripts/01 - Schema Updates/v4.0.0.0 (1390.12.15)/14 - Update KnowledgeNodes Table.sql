USE [EKM_App]
GO

/****** Object:  Table [dbo].[KnowledgeNodes]    Script Date: 02/17/2012 14:42:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[KnowledgeNodesTemp](
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NOT NULL,
	[Score] [float] NULL,
	[ScoresWeight] [float] NULL,
	[StatusID] [bigint] NULL,
 CONSTRAINT [PK_KnowledgeNodesTemp] PRIMARY KEY CLUSTERED 
(
	[KnowledgeID] ASC,
	[NodeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[KnowledgeNodesTemp]
           ([KnowledgeID]
			,[NodeID]
			,[Score]
			,[ScoresWeight]
			,[StatusID])
		SELECT  dbo.KnowledgeNodes.KnowledgeID, dbo.CN_Nodes.NodeID, dbo.KnowledgeNodes.Score, 
				dbo.KnowledgeNodes.ScoresWeight, dbo.KnowledgeNodes.StatusID
		FROM    dbo.CN_Nodes INNER JOIN dbo.KnowledgeNodes ON dbo.CN_Nodes.ID = dbo.KnowledgeNodes.NodeID
GO


/* Drop old table */
DROP TABLE [dbo].[KnowledgeNodes]
GO

/* Create old new table */
CREATE TABLE [dbo].[KnowledgeNodes](
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NOT NULL,
	[Score] [float] NULL,
	[ScoresWeight] [float] NULL,
	[StatusID] [bigint] NULL,
 CONSTRAINT [PK_KnowledgeNodes] PRIMARY KEY CLUSTERED 
(
	[KnowledgeID] ASC,
	[NodeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[KnowledgeNodes]  WITH CHECK ADD  CONSTRAINT [FK_KnowledgeNodes_KKnowledges] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[KKnowledges] ([ID])
GO

ALTER TABLE [dbo].[KnowledgeNodes] CHECK CONSTRAINT [FK_KnowledgeNodes_KKnowledges]
GO

ALTER TABLE [dbo].[KnowledgeNodes]  WITH CHECK ADD  CONSTRAINT [FK_KnowledgeNodes_KWF_Statuses] FOREIGN KEY([StatusID])
REFERENCES [dbo].[KWF_Statuses] ([StatusID])
GO

ALTER TABLE [dbo].[KnowledgeNodes] CHECK CONSTRAINT [FK_KnowledgeNodes_KWF_Statuses]
GO

ALTER TABLE [dbo].[KnowledgeNodes]  WITH CHECK ADD  CONSTRAINT [FK_KnowledgeNodes_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[KnowledgeNodes] CHECK CONSTRAINT [FK_KnowledgeNodes_CN_Nodes]
GO


INSERT INTO [dbo].[KnowledgeNodes]
	SELECT * FROM dbo.KnowledgeNodesTemp
GO

DROP TABLE [dbo].[KnowledgeNodesTemp]
GO