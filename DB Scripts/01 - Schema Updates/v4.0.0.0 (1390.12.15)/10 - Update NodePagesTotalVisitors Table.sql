USE [EKM_App]
GO

/****** Object:  Table [dbo].[NodePagesTotalVisitors]    Script Date: 02/17/2012 13:39:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[NodePagesTotalVisitorsTemp](
	[NodeID] [uniqueidentifier] NOT NULL,
	[VisitsCount] [bigint] NOT NULL,
 CONSTRAINT [PK_NodePagesTotalVisitorsTemp] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[NodePagesTotalVisitorsTemp]  WITH CHECK ADD  CONSTRAINT [FK_NodePagesTotalVisitorsTemp_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[NodePagesTotalVisitorsTemp] CHECK CONSTRAINT [FK_NodePagesTotalVisitorsTemp_CN_Nodes]
GO


INSERT INTO [dbo].[NodePagesTotalVisitorsTemp]
           ([NodeID]
			,[VisitsCount])
		SELECT  dbo.CN_Nodes.NodeID, dbo.NodePagesTotalVisitors.VisitorsCount
		FROM    dbo.CN_Nodes INNER JOIN dbo.NodePagesTotalVisitors ON dbo.CN_Nodes.ID = dbo.NodePagesTotalVisitors.NodeID
GO


/* Drop old table */
DROP TABLE [dbo].[NodePagesTotalVisitors]
GO

/* Create old new table */
CREATE TABLE [dbo].[NodePagesTotalVisitors](
	[NodeID] [uniqueidentifier] NOT NULL,
	[VisitsCount] [bigint] NOT NULL,
 CONSTRAINT [PK_NodePagesTotalVisitors] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[NodePagesTotalVisitors]  WITH CHECK ADD  CONSTRAINT [FK_NodePagesTotalVisitors_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[NodePagesTotalVisitors] CHECK CONSTRAINT [FK_NodePagesTotalVisitors_CN_Nodes]
GO


INSERT INTO [dbo].[NodePagesTotalVisitors]
	SELECT * FROM dbo.NodePagesTotalVisitorsTemp
GO

DROP TABLE [dbo].[NodePagesTotalVisitorsTemp]
GO