USE [EKM_App]
GO

/****** Object:  Table [dbo].[CalendarNodes]    Script Date: 02/17/2012 14:07:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CalenderNodesTemp](
	[CalenderID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_CalendarNodesTemp] PRIMARY KEY CLUSTERED 
(
	[CalenderID] ASC,
	[NodeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[CalenderNodesTemp]
           ([CalenderID]
			,[NodeID])
		SELECT  dbo.CalendarNodes.CalendarID, dbo.CN_Nodes.NodeID
		FROM    dbo.CN_Nodes INNER JOIN dbo.CalendarNodes ON dbo.CN_Nodes.ID = dbo.CalendarNodes.NodeID
GO


/* Drop old table */
DROP TABLE [dbo].[CalendarNodes]
GO

/* Create old new table */
CREATE TABLE [dbo].[CalenderNodes](
	[CalenderID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_CalendarNodes] PRIMARY KEY CLUSTERED 
(
	[CalenderID] ASC,
	[NodeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[CalenderNodes]  WITH CHECK ADD  CONSTRAINT [FK_CalenderNodes_Calendars] FOREIGN KEY([CalenderID])
REFERENCES [dbo].[Calendars] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[CalenderNodes] CHECK CONSTRAINT [FK_CalenderNodes_Calendars]
GO

ALTER TABLE [dbo].[CalenderNodes]  WITH CHECK ADD  CONSTRAINT [FK_CalenderNodes_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[CalenderNodes] CHECK CONSTRAINT [FK_CalenderNodes_CN_Nodes]
GO


INSERT INTO [dbo].[CalenderNodes]
	SELECT * FROM dbo.CalenderNodesTemp
GO

DROP TABLE [dbo].[CalenderNodesTemp]
GO