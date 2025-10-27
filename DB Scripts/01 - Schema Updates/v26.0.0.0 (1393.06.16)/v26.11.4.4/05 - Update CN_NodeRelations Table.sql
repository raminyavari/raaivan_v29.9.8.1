USE [EKM_App]
GO

/****** Object:  Table [dbo].[CN_NodeRelations]    Script Date: 08/01/2015 20:15:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(select * FROM sys.views where name = 'CN_View_NodeRelations')
DROP VIEW [dbo].[CN_View_NodeRelations]
GO

IF EXISTS(select * FROM sys.views where name = 'CN_View_InRelatedNodes')
DROP VIEW [dbo].[CN_View_InRelatedNodes]
GO

IF EXISTS(select * FROM sys.views where name = 'CN_View_OutRelatedNodes')
DROP VIEW [dbo].[CN_View_OutRelatedNodes]
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[CN_TMPNodeRelations](
	[SourceNodeID] [uniqueidentifier] NOT NULL,
	[DestinationNodeID] [uniqueidentifier] NOT NULL,
	[PropertyID] [uniqueidentifier] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[NominalValue] [nvarchar](255) NULL,
	[NumericalValue] [float] NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_CN_TMPNodeRelations] PRIMARY KEY CLUSTERED 
(
	[SourceNodeID] ASC,
	[DestinationNodeID] ASC,
	[PropertyID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[CN_TMPNodeRelations](
	SourceNodeID,
	DestinationNodeID,
	PropertyID,
	CreatorUserID,
	CreationDate,
	LastModifierUserID,
	LastModificationDate,
	NominalValue,
	NumericalValue,
	Deleted
)
SELECT SourceNodeID, DestinationNodeID, PropertyID, CreatorUserID, CreationDate,
	LastModifierUserID, LastModificationDate, NominalValue, NumericalValue, Deleted
FROM [dbo].[CN_NodeRelations]

GO


DROP TABLE [dbo].[CN_NodeRelations]
GO


CREATE TABLE [dbo].[CN_NodeRelations](
	[SourceNodeID] [uniqueidentifier] NOT NULL,
	[DestinationNodeID] [uniqueidentifier] NOT NULL,
	[PropertyID] [uniqueidentifier] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[NominalValue] [nvarchar](255) NULL,
	[NumericalValue] [float] NULL,
	[Deleted] [bit] NOT NULL,
	[UniqueID] [uniqueidentifier] NOT NULL
 CONSTRAINT [PK_CN_NodeRelations] PRIMARY KEY CLUSTERED 
(
	[SourceNodeID] ASC,
	[DestinationNodeID] ASC,
	[PropertyID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[CN_NodeRelations](
	SourceNodeID,
	DestinationNodeID,
	PropertyID,
	CreatorUserID,
	CreationDate,
	LastModifierUserID,
	LastModificationDate,
	NominalValue,
	NumericalValue,
	Deleted,
	UniqueID
)
SELECT SourceNodeID, DestinationNodeID, PropertyID, CreatorUserID, CreationDate,
	LastModifierUserID, LastModificationDate, NominalValue, NumericalValue, Deleted, NEWID()
FROM [dbo].[CN_TMPNodeRelations]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[CN_NodeRelations]  WITH CHECK ADD  CONSTRAINT [FK_CN_NodeRelations_CN_Nodes_Destination] FOREIGN KEY([DestinationNodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[CN_NodeRelations] CHECK CONSTRAINT [FK_CN_NodeRelations_CN_Nodes_Destination]
GO

ALTER TABLE [dbo].[CN_NodeRelations]  WITH CHECK ADD  CONSTRAINT [FK_CN_NodeRelations_CN_Nodes_Source] FOREIGN KEY([SourceNodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[CN_NodeRelations] CHECK CONSTRAINT [FK_CN_NodeRelations_CN_Nodes_Source]
GO

ALTER TABLE [dbo].[CN_NodeRelations]  WITH CHECK ADD  CONSTRAINT [FK_CN_NodeRelations_CN_Properties] FOREIGN KEY([PropertyID])
REFERENCES [dbo].[CN_Properties] ([PropertyID])
GO

ALTER TABLE [dbo].[CN_NodeRelations] CHECK CONSTRAINT [FK_CN_NodeRelations_CN_Properties]
GO


DROP TABLE [dbo].[CN_TMPNodeRelations]
GO