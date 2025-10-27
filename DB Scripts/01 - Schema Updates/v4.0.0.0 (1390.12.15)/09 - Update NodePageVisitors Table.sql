USE [EKM_App]
GO

/****** Object:  Table [dbo].[NodePageVisitors]    Script Date: 02/17/2012 13:31:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[NodePageVisitorsTemp](
	[VisitID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[VisitDate] [datetime] NULL,
 CONSTRAINT [PK_NodePageVisitorsTemp] PRIMARY KEY CLUSTERED 
(
	[VisitID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[NodePageVisitorsTemp]
           ([VisitID]
			,[NodeID]
			,[UserID]
			,[VisitDate])
		SELECT  dbo.NodePageVisitors.ID, dbo.CN_Nodes.NodeID, dbo.NodePageVisitors.VisitorUserId, dbo.NodePageVisitors.Date
		FROM    dbo.CN_Nodes INNER JOIN dbo.NodePageVisitors ON dbo.CN_Nodes.ID = dbo.NodePageVisitors.NodeId
GO


/* Drop old table */
DROP TABLE [dbo].[NodePageVisitors]
GO

/* Create old new table */
CREATE TABLE [dbo].[NodePageVisitors](
	[VisitID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[VisitDate] [datetime] NULL,
 CONSTRAINT [PK_NodePageVisitors] PRIMARY KEY CLUSTERED 
(
	[VisitID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[NodePageVisitors]  WITH CHECK ADD  CONSTRAINT [FK_NodePageVisitors_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[NodePageVisitors] CHECK CONSTRAINT [FK_NodePageVisitors_aspnet_Users]
GO

ALTER TABLE [dbo].[NodePageVisitors]  WITH CHECK ADD  CONSTRAINT [FK_NodePageVisitors_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[NodePageVisitors] CHECK CONSTRAINT [FK_NodePageVisitors_CN_Nodes]
GO


INSERT INTO [dbo].[NodePageVisitors]
	SELECT * FROM dbo.NodePageVisitorsTemp
GO

DROP TABLE [dbo].[NodePageVisitorsTemp]
GO