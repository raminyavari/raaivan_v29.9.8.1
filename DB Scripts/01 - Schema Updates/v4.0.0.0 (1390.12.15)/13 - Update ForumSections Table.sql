USE [EKM_App]
GO

/****** Object:  Table [dbo].[ForumSections]    Script Date: 02/17/2012 14:27:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ForumSectionsTemp](
	[ID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](500) NULL,
	[Hidden] [bit] NULL,
	[DateAdded] [datetime] NOT NULL,
	[DateLastUpdated] [datetime] NULL,
	[DateLastClicked] [datetime] NULL,
	[Hit] [int] NULL,
	[FullText] [nvarchar](max) NULL,
 CONSTRAINT [PK_ForumSectionsTemp] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


INSERT INTO [dbo].[ForumSectionsTemp]
           ([ID]
			,[NodeID]
			,[UserID]
			,[Title]
			,[Hidden]
			,[DateAdded]
			,[DateLastUpdated]
			,[DateLastClicked]
			,[Hit]
			,[FullText])
		SELECT  dbo.ForumSections.ID, dbo.CN_Nodes.NodeID, dbo.ForumSections.UserId, dbo.ForumSections.Title,
				dbo.ForumSections.Hidden, dbo.ForumSections.DateAdded, dbo.ForumSections.DateLastUpdated,
				dbo.ForumSections.DateLastClicked, dbo.ForumSections.Hit, dbo.ForumSections.FullText
		FROM    dbo.CN_Nodes INNER JOIN dbo.ForumSections ON dbo.CN_Nodes.ID = dbo.ForumSections.NodeId
GO



/* Drop old table */
ALTER TABLE [dbo].[ForumTopics]
DROP CONSTRAINT [FK_ForumTopics_ForumSections]
GO

DROP TABLE [dbo].[ForumSections]
GO

/* Create old new table */
CREATE TABLE [dbo].[ForumSections](
	[ID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](500) NULL,
	[Hidden] [bit] NULL,
	[DateAdded] [datetime] NOT NULL,
	[DateLastUpdated] [datetime] NULL,
	[DateLastClicked] [datetime] NULL,
	[Hit] [int] NULL,
	[FullText] [nvarchar](max) NULL,
 CONSTRAINT [PK_ForumSections] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ForumSections]  WITH CHECK ADD  CONSTRAINT [FK_ForumSections_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ForumSections] CHECK CONSTRAINT [FK_ForumSections_CN_Nodes]
GO


INSERT INTO [dbo].[ForumSections]
	SELECT * FROM dbo.ForumSectionsTemp
GO

DROP TABLE [dbo].[ForumSectionsTemp]
GO


ALTER TABLE [dbo].[ForumTopics]  WITH CHECK ADD  CONSTRAINT [FK_ForumTopics_ForumSections] FOREIGN KEY([SectionId])
REFERENCES [dbo].[ForumSections] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ForumTopics] CHECK CONSTRAINT [FK_ForumTopics_ForumSections]
GO