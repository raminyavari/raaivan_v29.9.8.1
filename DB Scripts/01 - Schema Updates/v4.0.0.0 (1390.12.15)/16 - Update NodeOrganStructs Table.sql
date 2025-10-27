USE [EKM_App]
GO

/****** Object:  Table [dbo].[NodeOrganStructs]    Script Date: 02/17/2012 14:55:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[NodeOrganStructsTemp](
	[NodeID] [uniqueidentifier] NOT NULL,
	[DepartmentID] [bigint] NOT NULL,
 CONSTRAINT [PK_NodeOrganStructsTemp] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC,
	[DepartmentID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[NodeOrganStructsTemp]
           ([NodeID]
			,[DepartmentID])
		SELECT  dbo.CN_Nodes.NodeID, dbo.NodeOrganStructs.OrganStructId
		FROM    dbo.CN_Nodes INNER JOIN dbo.NodeOrganStructs ON dbo.CN_Nodes.ID = dbo.NodeOrganStructs.NodeId
GO


/* Drop old table */
DROP TABLE [dbo].[NodeOrganStructs]
GO

/* Create old new table */
CREATE TABLE [dbo].[NodeOrganStructs](
	[NodeID] [uniqueidentifier] NOT NULL,
	[DepartmentID] [bigint] NOT NULL,
 CONSTRAINT [PK_NodeOrganStructs] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC,
	[DepartmentID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[NodeOrganStructs]  WITH CHECK ADD  CONSTRAINT [FK_NodeOrganStructs_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[NodeOrganStructs] CHECK CONSTRAINT [FK_NodeOrganStructs_CN_Nodes]
GO

ALTER TABLE [dbo].[NodeOrganStructs]  WITH CHECK ADD  CONSTRAINT [FK_NodeOrganStructs_OrganStructs] FOREIGN KEY([DepartmentID])
REFERENCES [dbo].[OrganStructs] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[NodeOrganStructs] CHECK CONSTRAINT [FK_NodeOrganStructs_OrganStructs]
GO


INSERT INTO [dbo].[NodeOrganStructs]
	SELECT * FROM dbo.NodeOrganStructsTemp
GO

DROP TABLE [dbo].[NodeOrganStructsTemp]
GO