USE [EKM_App]

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



/****** Object:  Table [dbo].[DCT_TreeTypes]    Script Date: 06/23/2012 00:20:28 ******/
CREATE TABLE [dbo].[DCT_TreeTypes](
	[TreeTypeID] [int] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[Description] [nvarchar](1000) NULL,
	[CreatorUserID] [uniqueidentifier] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
CONSTRAINT [PK_DCT_TreeTypes] PRIMARY KEY CLUSTERED 
(
	[TreeTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[DCT_TreeTypes]  WITH CHECK ADD  CONSTRAINT [FK_DCT_TreeTypes_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[DCT_TreeTypes] CHECK CONSTRAINT [FK_DCT_TreeTypes_aspnet_Users_Creator]
GO


ALTER TABLE [dbo].[DCT_TreeTypes]  WITH CHECK ADD  CONSTRAINT [FK_DCT_TreeTypes_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[DCT_TreeTypes] CHECK CONSTRAINT [FK_DCT_TreeTypes_aspnet_Users_Modifier]
GO



/****** Object:  Table [dbo].[DCT_Trees]    Script Date: 06/23/2012 00:20:28 ******/
CREATE TABLE [dbo].[DCT_Trees](
	[TreeID] [uniqueidentifier] NOT NULL,
	[TreeTypeID] [int] NOT NULL,
	[IsPrivate] [bit] NOT NULL,
	[OwnerID] [uniqueidentifier] NULL,
	[Name] [nvarchar](256) NOT NULL,
	[Description] [nvarchar](1000) NULL,
	[CreatorUserID] [uniqueidentifier] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Privacy] [varchar](20) NULL,
	[Deleted] [bit] NOT NULL
CONSTRAINT [PK_DCT_Trees] PRIMARY KEY CLUSTERED 
(
	[TreeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[DCT_Trees]  WITH CHECK ADD  CONSTRAINT [FK_DCT_Trees_DCT_TreeTypes] FOREIGN KEY([TreeTypeID])
REFERENCES [dbo].[DCT_TreeTypes] ([TreeTypeID])
GO

ALTER TABLE [dbo].[DCT_Trees] CHECK CONSTRAINT [FK_DCT_Trees_DCT_TreeTypes]
GO


ALTER TABLE [dbo].[DCT_Trees]  WITH CHECK ADD  CONSTRAINT [FK_DCT_Trees_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[DCT_Trees] CHECK CONSTRAINT [FK_DCT_Trees_aspnet_Users_Creator]
GO


ALTER TABLE [dbo].[DCT_Trees]  WITH CHECK ADD  CONSTRAINT [FK_DCT_Trees_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[DCT_Trees] CHECK CONSTRAINT [FK_DCT_Trees_aspnet_Users_Modifier]
GO



/****** Object:  Table [dbo].[DCT_TreeNodes]    Script Date: 06/23/2012 00:20:28 ******/
CREATE TABLE [dbo].[DCT_TreeNodes](
	[TreeNodeID] [uniqueidentifier] NOT NULL,
	[TreeID] [uniqueidentifier] NOT NULL,
	[ParentNodeID] [uniqueidentifier] NULL,
	[Name] [nvarchar](256) NOT NULL,
	[Description] [nvarchar](1000) NULL,
	[CreatorUserID] [uniqueidentifier] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Privacy] [varchar](20) NULL,
	[Deleted] [bit] NOT NULL
CONSTRAINT [PK_DCT_TreeNodes] PRIMARY KEY CLUSTERED 
(
	[TreeNodeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[DCT_TreeNodes]  WITH CHECK ADD  CONSTRAINT [FK_DCT_TreeNodes_DCT_Trees] FOREIGN KEY([TreeID])
REFERENCES [dbo].[DCT_Trees] ([TreeID])
GO

ALTER TABLE [dbo].[DCT_TreeNodes] CHECK CONSTRAINT [FK_DCT_TreeNodes_DCT_Trees]
GO


ALTER TABLE [dbo].[DCT_TreeNodes]  WITH CHECK ADD  CONSTRAINT [FK_DCT_TreeNodes_DCT_TreeNodes] FOREIGN KEY([ParentNodeID])
REFERENCES [dbo].[DCT_TreeNodes] ([TreeNodeID])
GO

ALTER TABLE [dbo].[DCT_TreeNodes] CHECK CONSTRAINT [FK_DCT_TreeNodes_DCT_TreeNodes]
GO


ALTER TABLE [dbo].[DCT_TreeNodes]  WITH CHECK ADD  CONSTRAINT [FK_DCT_TreeNodes_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[DCT_TreeNodes] CHECK CONSTRAINT [FK_DCT_TreeNodes_aspnet_Users_Creator]
GO


ALTER TABLE [dbo].[DCT_TreeNodes]  WITH CHECK ADD  CONSTRAINT [FK_DCT_TreeNodes_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[DCT_TreeNodes] CHECK CONSTRAINT [FK_DCT_TreeNodes_aspnet_Users_Modifier]
GO