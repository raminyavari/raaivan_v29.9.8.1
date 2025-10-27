/****** Object:  StoredProcedure [dbo].[AddFolder]    Script Date: 03/14/2012 11:38:59 ******/
USE [EKM_App]

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER TABLE [dbo].[CN_NodeTypes]
ADD [CreatorUserID] [uniqueidentifier] NULL
GO

ALTER TABLE [dbo].[CN_NodeTypes]
ADD [CreationDate] [datetime] NULL
GO

ALTER TABLE [dbo].[CN_NodeTypes]
ADD [LastModifierUserID] [uniqueidentifier] NULL
GO

ALTER TABLE [dbo].[CN_NodeTypes]
ADD [LastModificationDate] [datetime] NULL
GO

ALTER TABLE [dbo].[CN_NodeTypes]  WITH CHECK ADD  CONSTRAINT [FK_CN_NodeTypes_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_NodeTypes] CHECK CONSTRAINT [FK_CN_NodeTypes_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[CN_NodeTypes]  WITH CHECK ADD  CONSTRAINT [FK_CN_NodeTypes_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_NodeTypes] CHECK CONSTRAINT [FK_CN_NodeTypes_aspnet_Users_Modifier]
GO


ALTER TABLE [dbo].[CN_Properties]
ADD [CreatorUserID] [uniqueidentifier] NULL
GO

ALTER TABLE [dbo].[CN_Properties]
ADD [CreationDate] [datetime] NULL
GO

ALTER TABLE [dbo].[CN_Properties]
ADD [LastModifierUserID] [uniqueidentifier] NULL
GO

ALTER TABLE [dbo].[CN_Properties]
ADD [LastModificationDate] [datetime] NULL
GO

ALTER TABLE [dbo].[CN_Properties]  WITH CHECK ADD  CONSTRAINT [FK_CN_Properties_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_Properties] CHECK CONSTRAINT [FK_CN_Properties_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[CN_Properties]  WITH CHECK ADD  CONSTRAINT [FK_CN_Properties_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_Properties] CHECK CONSTRAINT [FK_CN_Properties_aspnet_Users_Modifier]
GO


-- Update CN_NodeRelations
CREATE TABLE [dbo].[TMP_CN_NodeRelations](
	[SourceNodeID] [uniqueidentifier] NOT NULL,
	[DestinationNodeID] [uniqueidentifier] NOT NULL,
	[PropertyID] [uniqueidentifier] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[NominalValue] [nvarchar](255) NULL,
	[NumericalValue] [float] NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_TMP_NodeRelations] PRIMARY KEY CLUSTERED 
(
	[SourceNodeID] ASC,
	[DestinationNodeID] ASC,
	[PropertyID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[TMP_CN_NodeRelations](
	SourceNodeID,
	DestinationNodeID,
	PropertyID,
	NominalValue,
	NumericalValue,
	Deleted
)
SELECT Ref.SourceNodeID, Ref.DestinationNodeID, Ref.PropertyID,
	Ref.NominalValue, Ref.NumericalValue, 0
FROM [dbo].[CN_NodeRelations] AS Ref
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
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_CN_NodeRelations] PRIMARY KEY CLUSTERED 
(
	[SourceNodeID] ASC,
	[DestinationNodeID] ASC,
	[PropertyID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

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


INSERT INTO [dbo].[CN_NodeRelations]
SELECT * FROM [dbo].[TMP_CN_NodeRelations]
GO

DROP TABLE [dbo].[TMP_CN_NodeRelations]
GO
-- end of Update CN_NodeRelations