USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/****** Object:  Table [dbo].[CN_NodeMembers]    Script Date: 07/29/2012 10:11:41 ******/
SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[CN_NodeMembers](
	[NodeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[MembershipDate] [datetime] NOT NULL,
	[IsAdmin] [bit] NOT NULL,
	[Status] [varchar](20) NOT NULL,
	[AcceptionDate] [datetime] NULL,
	[Position] [nvarchar](255) NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_CN_NodeMembers] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[CN_NodeMembers]  WITH CHECK ADD  CONSTRAINT [FK_CN_NodeMembers_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_NodeMembers] CHECK CONSTRAINT [FK_CN_NodeMembers_aspnet_Users]
GO

ALTER TABLE [dbo].[CN_NodeMembers]  WITH CHECK ADD  CONSTRAINT [FK_CN_NodeMembers_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[CN_NodeMembers] CHECK CONSTRAINT [FK_CN_NodeMembers_CN_Nodes]
GO



/****** Object:  Table [dbo].[CN_NodeLikes]    Script Date: 07/29/2012 10:16:36 ******/
CREATE TABLE [dbo].[CN_NodeLikes](
	[NodeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[LikeDate] [datetime] NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_CN_NodeLikes] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[CN_NodeLikes]  WITH CHECK ADD  CONSTRAINT [FK_CN_NodeLikes_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_NodeLikes] CHECK CONSTRAINT [FK_CN_NodeLikes_aspnet_Users]
GO

ALTER TABLE [dbo].[CN_NodeLikes]  WITH CHECK ADD  CONSTRAINT [FK_CN_NodeLikes_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[CN_NodeLikes] CHECK CONSTRAINT [FK_CN_NodeLikes_CN_Nodes]
GO


