USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/****** Object:  Table [dbo].[CN_NodeTypes]    Script Date: 11/20/2011 15:09:46 ******/
CREATE TABLE [dbo].[CN_NodeTypes](
	[NodeTypeID] [uniqueidentifier] NOT NULL,
	[AdditionalID] [nvarchar](20) NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Deleted] [bit] NOT NULL,
	[TypeID] [bigint] NOT NULL, /* To be removed */
 CONSTRAINT [PK_NodeTypes_NodeTypeID] PRIMARY KEY CLUSTERED 
(
	[NodeTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UK_NodeTypes_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


/****** Object:  Table [dbo].[CN_Nodes]    Script Date: 11/20/2011 15:11:00 ******/
CREATE TABLE [dbo].[CN_Nodes](
	[NodeID] [uniqueidentifier] NOT NULL,
	[AdditionalID] [nvarchar](20) NULL,
	[NodeTypeID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
	[TSkill] [float] NULL,
	[TExperience] [float] NULL,
	[TContent] [float] NULL,
	[Status] [nvarchar](255) NULL,
	[DepartmentGroupID] [uniqueidentifier] NULL,
	[ParentNodeID] [uniqueidentifier] NULL,
	[ID] [bigint] NOT NULL, /* To be removed */
	[TypeID] [bigint] NOT NULL, /* To be removed */
 CONSTRAINT [PK_Nodes_NodeID] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[CN_Nodes]  WITH CHECK ADD  CONSTRAINT [FK_CN_Nodes_CN_NodeTypes] FOREIGN KEY([NodeTypeID])
REFERENCES [dbo].[CN_NodeTypes] ([NodeTypeID])
GO

ALTER TABLE [dbo].[CN_Nodes] CHECK CONSTRAINT [FK_CN_Nodes_CN_NodeTypes]
GO

ALTER TABLE [dbo].[CN_Nodes]  WITH CHECK ADD  CONSTRAINT [FK_CN_Nodes_aspnet_Users] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_Nodes] CHECK CONSTRAINT [FK_CN_Nodes_aspnet_Users]
GO

ALTER TABLE [dbo].[CN_Nodes]  WITH CHECK ADD  CONSTRAINT [FK_CN_Nodes_DepartmentGroups] FOREIGN KEY([DepartmentGroupID])
REFERENCES [dbo].[DepartmentGroups] ([ID])
GO

ALTER TABLE [dbo].[CN_Nodes] CHECK CONSTRAINT [FK_CN_Nodes_DepartmentGroups]
GO

ALTER TABLE [dbo].[CN_Nodes]  WITH CHECK ADD  CONSTRAINT [FK_CN_Nodes_CN_Nodes] FOREIGN KEY([ParentNodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[CN_Nodes] CHECK CONSTRAINT [FK_CN_Nodes_CN_Nodes]
GO


/****** Object:  Table [dbo].[CN_Properties]    Script Date: 11/20/2011 15:12:03 ******/
CREATE TABLE [dbo].[CN_Properties](
	[PropertyID] [uniqueidentifier] NOT NULL,
	[AdditionalID] [nvarchar](20) NULL,
	[NodeTypeID] [uniqueidentifier] NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](max) NULL,
	[Deleted] [bit] NOT NULL,
	[ID] [bigint] NOT NULL, /* To be removed */
 CONSTRAINT [PK_Properties_PropertyID] PRIMARY KEY CLUSTERED 
(
	[PropertyID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UK_Properties_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[CN_Properties]  WITH CHECK ADD  CONSTRAINT [FK_CN_Properties_CN_NodeTypes] FOREIGN KEY([NodeTypeID])
REFERENCES [dbo].[CN_NodeTypes] ([NodeTypeID])
GO

ALTER TABLE [dbo].[CN_Properties] CHECK CONSTRAINT [FK_CN_Properties_CN_NodeTypes]
GO


/****** Object:  Table [dbo].[CN_NodeProperties]    Script Date: 11/20/2011 15:13:15 ******/
CREATE TABLE [dbo].[CN_NodeProperties](
	[NodeID] [uniqueidentifier] NOT NULL,
	[PropertyID] [uniqueidentifier] NOT NULL,
	[NominalValue] [nvarchar](255) NULL,
	[NumericalValue] [float] NULL,
 CONSTRAINT [PK_CN_NodeProperties] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC,
	[PropertyID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[CN_NodeProperties]  WITH CHECK ADD  CONSTRAINT [FK_CN_NodeProperties_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[CN_NodeProperties] CHECK CONSTRAINT [FK_CN_NodeProperties_CN_Nodes]
GO

ALTER TABLE [dbo].[CN_NodeProperties]  WITH CHECK ADD  CONSTRAINT [FK_CN_NodeProperties_CN_Properties] FOREIGN KEY([PropertyID])
REFERENCES [dbo].[CN_Properties] ([PropertyID])
GO

ALTER TABLE [dbo].[CN_NodeProperties] CHECK CONSTRAINT [FK_CN_NodeProperties_CN_Properties]
GO


/****** Object:  Table [dbo].[CN_NodeRelations]    Script Date: 11/20/2011 15:13:48 ******/
CREATE TABLE [dbo].[CN_NodeRelations](
	[SourceNodeID] [uniqueidentifier] NOT NULL,
	[DestinationNodeID] [uniqueidentifier] NOT NULL,
	[PropertyID] [uniqueidentifier] NOT NULL,
	[NominalValue] [nvarchar](255) NULL,
	[NumericalValue] [float] NULL,
 CONSTRAINT [PK_NodeRelations] PRIMARY KEY CLUSTERED 
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


/****** Object:  Table [dbo].[CN_Lists]    Script Date: 11/20/2011 15:14:35 ******/
CREATE TABLE [dbo].[CN_Lists](
	[ListID] [uniqueidentifier] NOT NULL,
	[AdditionalID] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[OwnerNodeID] [uniqueidentifier] NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_Lists_ListID] PRIMARY KEY CLUSTERED 
(
	[ListID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[CN_Lists]  WITH CHECK ADD  CONSTRAINT [FK_CN_Lists_CN_Nodes] FOREIGN KEY([OwnerNodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[CN_Lists] CHECK CONSTRAINT [FK_CN_Lists_CN_Nodes]
GO


/****** Object:  Table [dbo].[CN_ListItems]    Script Date: 11/20/2011 15:14:35 ******/
CREATE TABLE [dbo].[CN_ListItems](
	[ListItemID] [uniqueidentifier] NOT NULL,
	[ListID] [uniqueidentifier] NOT NULL,
	[AdditionalID] [nvarchar](20) NULL,
	[ParentListItemID] [uniqueidentifier] NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_ListItems_ListItemID] PRIMARY KEY CLUSTERED 
(
	[ListItemID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[CN_ListItems]  WITH CHECK ADD  CONSTRAINT [FK_CN_ListItems_CN_Lists] FOREIGN KEY([ListID])
REFERENCES [dbo].[CN_Lists] ([ListID])
GO

ALTER TABLE [dbo].[CN_ListItems] CHECK CONSTRAINT [FK_CN_ListItems_CN_Lists]
GO

ALTER TABLE [dbo].[CN_ListItems]  WITH CHECK ADD  CONSTRAINT [FK_CN_ListItems_CN_ListItems] FOREIGN KEY([ListItemID])
REFERENCES [dbo].[CN_ListItems] ([ListItemID])
GO

ALTER TABLE [dbo].[CN_ListItems] CHECK CONSTRAINT [FK_CN_ListItems_CN_ListItems]
GO


/****** Object:  Table [dbo].[CN_ListNodes]    Script Date: 11/20/2011 15:15:13 ******/
CREATE TABLE [dbo].[CN_ListItemNodes](
	[ListItemID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_CN_ListNodes] PRIMARY KEY CLUSTERED 
(
	[ListItemID] ASC,
	[NodeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[CN_ListItemNodes]  WITH CHECK ADD  CONSTRAINT [FK_CN_ListItemNodes_CN_ListItems] FOREIGN KEY([ListItemID])
REFERENCES [dbo].[CN_ListItems] ([ListItemID])
GO

ALTER TABLE [dbo].[CN_ListItemNodes] CHECK CONSTRAINT [FK_CN_ListItemNodes_CN_ListItems]
GO

ALTER TABLE [dbo].[CN_ListItemNodes]  WITH CHECK ADD  CONSTRAINT [FK_CN_ListItemNodes_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[CN_ListItemNodes] CHECK CONSTRAINT [FK_CN_ListItemNodes_CN_Nodes]
GO