USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- Drop Old Tables
DROP TABLE [dbo].[CN_ListItemNodes]
GO

DROP TABLE [dbo].[CN_ListItems]
GO

DROP TABLE [dbo].[CN_Lists]
GO
-- end of Drop Old Tables



/****** Object:  Table [dbo].[CN_ListTypes]    Script Date: 09/16/2012 17:26:16 ******/
CREATE TABLE [dbo].[CN_ListTypes](
	[ListTypeID] [uniqueidentifier] NOT NULL,
	[AdditionalID] [nvarchar](20) NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2000) NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_CN_ListTypes] PRIMARY KEY CLUSTERED 
(
	[ListTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UK_ListTypes_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[CN_ListTypes]  WITH CHECK ADD  CONSTRAINT [FK_CN_ListTypes_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_ListTypes] CHECK CONSTRAINT [FK_CN_ListTypes_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[CN_ListTypes]  WITH CHECK ADD  CONSTRAINT [FK_CN_ListTypes_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_ListTypes] CHECK CONSTRAINT [FK_CN_ListTypes_aspnet_Users_Modifier]
GO



/****** Object:  Table [dbo].[CN_Lists]    Script Date: 09/16/2012 17:26:16 ******/
CREATE TABLE [dbo].[CN_Lists](
	[ListID] [uniqueidentifier] NOT NULL,
	[ListTypeID] [uniqueidentifier] NOT NULL,
	[AdditionalID] [nvarchar](20) NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2000) NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[ParentListID] [uniqueidentifier] NULL,
	[OwnerID] [uniqueidentifier] NULL,
	[OwnerType] [varchar](20) NULL,
	[Privacy] [varchar](20) NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_CN_Lists] PRIMARY KEY CLUSTERED 
(
	[ListID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[CN_Lists]  WITH CHECK ADD  CONSTRAINT [FK_CN_Lists_CN_Lists] FOREIGN KEY([ParentListID])
REFERENCES [dbo].[CN_Lists] ([ListID])
GO

ALTER TABLE [dbo].[CN_Lists] CHECK CONSTRAINT [FK_CN_Lists_CN_Lists]
GO

ALTER TABLE [dbo].[CN_Lists]  WITH CHECK ADD  CONSTRAINT [FK_CN_Lists_CN_ListTypes] FOREIGN KEY([ListTypeID])
REFERENCES [dbo].[CN_ListTypes] ([ListTypeID])
GO

ALTER TABLE [dbo].[CN_Lists] CHECK CONSTRAINT [FK_CN_Lists_CN_ListTypes]
GO

ALTER TABLE [dbo].[CN_Lists]  WITH CHECK ADD  CONSTRAINT [FK_CN_Lists_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_Lists] CHECK CONSTRAINT [FK_CN_Lists_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[CN_Lists]  WITH CHECK ADD  CONSTRAINT [FK_CN_Lists_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_Lists] CHECK CONSTRAINT [FK_CN_Lists_aspnet_Users_Modifier]
GO



/****** Object:  Table [dbo].[CN_ListNodes]    Script Date: 09/16/2012 17:26:16 ******/
CREATE TABLE [dbo].[CN_ListNodes](
	[ListID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_CN_ListNodes] PRIMARY KEY CLUSTERED 
(
	[ListID] ASC,
	[NodeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[CN_ListNodes]  WITH CHECK ADD  CONSTRAINT [FK_CN_ListNodes_CN_Lists] FOREIGN KEY([ListID])
REFERENCES [dbo].[CN_Lists] ([ListID])
GO

ALTER TABLE [dbo].[CN_ListNodes] CHECK CONSTRAINT [FK_CN_ListNodes_CN_Lists]
GO

ALTER TABLE [dbo].[CN_ListNodes]  WITH CHECK ADD  CONSTRAINT [FK_CN_ListNodes_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[CN_ListNodes] CHECK CONSTRAINT [FK_CN_ListNodes_CN_Nodes]
GO

ALTER TABLE [dbo].[CN_ListNodes]  WITH CHECK ADD  CONSTRAINT [FK_CN_ListNodes_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_ListNodes] CHECK CONSTRAINT [FK_CN_ListNodes_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[CN_ListNodes]  WITH CHECK ADD  CONSTRAINT [FK_CN_ListNodes_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_ListNodes] CHECK CONSTRAINT [FK_CN_ListNodes_aspnet_Users_Modifier]
GO