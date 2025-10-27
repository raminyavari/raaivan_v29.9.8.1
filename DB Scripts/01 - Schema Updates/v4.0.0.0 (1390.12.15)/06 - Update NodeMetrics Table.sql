USE [EKM_App]
GO

/****** Object:  Table [dbo].[NodeMetricsTemp]    Script Date: 02/17/2012 12:50:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[NodeMetricsTemp](
	[NodeID] [uniqueidentifier] NOT NULL,
	[PRc] [float] NULL,
	[DIc] [float] NULL,
	[COc] [float] NULL,
	[VAc] [float] NULL,
	[PRd] [float] NULL,
	[DId] [float] NULL,
	[COd] [float] NULL,
	[VAd] [float] NULL,
	[Gpr] [float] NULL,
	[Gdi] [float] NULL,
	[Gco] [float] NULL,
	[Gva] [float] NULL,
 CONSTRAINT [PK_NodeMetricsTemp] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[NodeMetricsTemp]  WITH CHECK ADD  CONSTRAINT [FK_NodeMetricsTemp_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[NodeMetricsTemp] CHECK CONSTRAINT [FK_NodeMetricsTemp_CN_Nodes]
GO


INSERT INTO [dbo].[NodeMetricsTemp]
           ([NodeID]
            ,[PRc]
			,[DIc]
			,[COc]
			,[VAc]
			,[PRd]
			,[DId]
			,[COd]
			,[VAd]
			,[Gpr]
			,[Gdi]
			,[Gco]
			,[Gva])
		SELECT  dbo.CN_Nodes.NodeID, dbo.NodeMetrics.PRc, dbo.NodeMetrics.DIc, dbo.NodeMetrics.COc, dbo.NodeMetrics.VAc, 
				dbo.NodeMetrics.PRd, dbo.NodeMetrics.DId, dbo.NodeMetrics.COd, dbo.NodeMetrics.VAd, dbo.NodeMetrics.Gpr, 
				dbo.NodeMetrics.Gdi, dbo.NodeMetrics.Gco, dbo.NodeMetrics.Gva
		FROM    dbo.CN_Nodes INNER JOIN dbo.NodeMetrics ON dbo.CN_Nodes.ID = dbo.NodeMetrics.NodeID
GO


/* Drop old table */
DROP TABLE [dbo].[NodeMetrics]
GO

/* Create old new table */
CREATE TABLE [dbo].[NodeMetrics](
	[NodeID] [uniqueidentifier] NOT NULL,
	[PRc] [float] NULL,
	[DIc] [float] NULL,
	[COc] [float] NULL,
	[VAc] [float] NULL,
	[PRd] [float] NULL,
	[DId] [float] NULL,
	[COd] [float] NULL,
	[VAd] [float] NULL,
	[Gpr] [float] NULL,
	[Gdi] [float] NULL,
	[Gco] [float] NULL,
	[Gva] [float] NULL,
 CONSTRAINT [PK_NodeMetrics] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[NodeMetrics]  WITH CHECK ADD  CONSTRAINT [FK_NodeMetrics_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[NodeMetrics] CHECK CONSTRAINT [FK_NodeMetrics_CN_Nodes]
GO


INSERT INTO [dbo].[NodeMetrics]
	SELECT * FROM dbo.NodeMetricsTemp
GO

DROP TABLE [dbo].[NodeMetricsTemp]
GO