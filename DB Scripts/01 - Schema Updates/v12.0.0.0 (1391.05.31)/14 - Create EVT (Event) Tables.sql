USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/****** Object:  Table [dbo].[EVT_Events]    Script Date: 07/17/2012 10:20:24 ******/
CREATE TABLE [dbo].[EVT_Events](
	[EventID] [uniqueidentifier] NOT NULL,
	[EventType] [nvarchar](256) NULL,
	[OwnerID] [uniqueidentifier] NULL,
	[Title] [nvarchar](500) NOT NULL,
	[Description] [nvarchar](2000) NULL,
	[BeginDate] [datetime] NULL,
	[FinishDate] [datetime] NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
CONSTRAINT [PK_EVT_Events] PRIMARY KEY CLUSTERED 
(
	[EventID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[EVT_Events]  WITH CHECK ADD  CONSTRAINT [FK_EVT_Events_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[EVT_Events] CHECK CONSTRAINT [FK_EVT_Events_aspnet_Users_Creator]
GO


ALTER TABLE [dbo].[EVT_Events]  WITH CHECK ADD  CONSTRAINT [FK_EVT_Events_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[EVT_Events] CHECK CONSTRAINT [FK_EVT_Events_aspnet_Users_Modifier]
GO


/****** Object:  Table [dbo].[EVT_RelatedUsers]    Script Date: 07/17/2012 10:44:26 ******/
SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[EVT_RelatedUsers](
	[EventID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[Status] [varchar](20) NOT NULL,
	[Done] [bit] NOT NULL,
	[RealFinishDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_EVT_RelatedUsers] PRIMARY KEY CLUSTERED 
(
	[EventID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[EVT_RelatedUsers]  WITH CHECK ADD  CONSTRAINT [FK_EVT_RelatedUsers_EVT_Events] FOREIGN KEY([EventID])
REFERENCES [dbo].[EVT_Events] ([EventID])
GO

ALTER TABLE [dbo].[EVT_RelatedUsers] CHECK CONSTRAINT [FK_EVT_RelatedUsers_EVT_Events]
GO


ALTER TABLE [dbo].[EVT_RelatedUsers]  WITH CHECK ADD  CONSTRAINT [FK_EVT_RelatedUsers_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[EVT_RelatedUsers] CHECK CONSTRAINT [FK_EVT_RelatedUsers_aspnet_Users]
GO


/****** Object:  Table [dbo].[EVT_RelatedNodes]    Script Date: 07/17/2012 10:44:26 ******/
CREATE TABLE [dbo].[EVT_RelatedNodes](
	[EventID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_EVT_RelatedNodes] PRIMARY KEY CLUSTERED 
(
	[EventID] ASC,
	[NodeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[EVT_RelatedNodes]  WITH CHECK ADD  CONSTRAINT [FK_EVT_RelatedNodes_EVT_Events] FOREIGN KEY([EventID])
REFERENCES [dbo].[EVT_Events] ([EventID])
GO

ALTER TABLE [dbo].[EVT_RelatedNodes] CHECK CONSTRAINT [FK_EVT_RelatedNodes_EVT_Events]
GO


ALTER TABLE [dbo].[EVT_RelatedNodes]  WITH CHECK ADD  CONSTRAINT [FK_EVT_RelatedNodes_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[EVT_RelatedNodes] CHECK CONSTRAINT [FK_EVT_RelatedNodes_CN_Nodes]
GO