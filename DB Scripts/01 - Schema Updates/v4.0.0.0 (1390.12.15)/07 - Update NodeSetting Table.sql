USE [EKM_App]
GO

/****** Object:  Table [dbo].[NodeSetting]    Script Date: 02/17/2012 13:10:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[NodeSettingTemp](
	[NodeID] [uniqueidentifier] NOT NULL,
	[MembershipType] [nvarchar](max) NULL,
 CONSTRAINT [PK_NodeSettingTemp] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[NodeSettingTemp]
           ([NodeID]
            ,[MembershipType])
		SELECT  dbo.CN_Nodes.NodeID, dbo.NodeSetting.MembershipType
		FROM    dbo.CN_Nodes INNER JOIN dbo.NodeSetting ON dbo.CN_Nodes.ID = dbo.NodeSetting.NodeId
GO


/* Drop old table */
DROP TABLE [dbo].[NodeSetting]
GO

/* Create old new table */
CREATE TABLE [dbo].[NodeSetting](
	[NodeID] [uniqueidentifier] NOT NULL,
	[MembershipType] [nvarchar](max) NULL,
 CONSTRAINT [PK_NodeSetting] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[NodeSetting]  WITH CHECK ADD  CONSTRAINT [FK_NodeSetting_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[NodeSetting] CHECK CONSTRAINT [FK_NodeSetting_CN_Nodes]
GO


INSERT INTO [dbo].[NodeSetting]
	SELECT * FROM dbo.NodeSettingTemp
GO

DROP TABLE [dbo].[NodeSettingTemp]
GO