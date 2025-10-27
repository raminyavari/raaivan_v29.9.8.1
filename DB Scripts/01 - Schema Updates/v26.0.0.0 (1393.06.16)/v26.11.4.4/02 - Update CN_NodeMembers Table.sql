USE [EKM_App]
GO

/****** Object:  Table [dbo].[CN_NodeMembers]    Script Date: 08/01/2015 20:15:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(select * FROM sys.views where name = 'CN_View_ExpertiseReferrals')
DROP VIEW [dbo].[CN_View_ExpertiseReferrals]
GO

IF EXISTS(select * FROM sys.views where name = 'CN_View_NodeMembers')
DROP VIEW [dbo].[CN_View_NodeMembers]
GO

IF EXISTS(select * FROM sys.views where name = 'CN_View_ListMembers')
DROP VIEW [dbo].[CN_View_ListMembers]
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[CN_TMPNodeMembers](
	[NodeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[MembershipDate] [datetime] NOT NULL,
	[IsAdmin] [bit] NOT NULL,
	[Status] [varchar](20) NOT NULL,
	[AcceptionDate] [datetime] NULL,
	[Position] [nvarchar](255) NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_CN_TMPNodeMembers] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[CN_TMPNodeMembers](
	NodeID,
	UserID,
	MembershipDate,
	IsAdmin,
	[Status],
	[AcceptionDate],
	Position,
	Deleted
)
SELECT NodeID, UserID, MembershipDate, IsAdmin, [Status], AcceptionDate, Position, Deleted
FROM [dbo].[CN_NodeMembers]

GO


DROP TABLE [dbo].[CN_NodeMembers]
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
	[UniqueID] [uniqueidentifier] NOT NULL
 CONSTRAINT [PK_CN_NodeMembers] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[CN_NodeMembers](
	NodeID,
	UserID,
	MembershipDate,
	IsAdmin,
	[Status],
	[AcceptionDate],
	Position,
	Deleted,
	UniqueID
)
SELECT NodeID, UserID, MembershipDate, IsAdmin, [Status], AcceptionDate, Position, Deleted, NEWID()
FROM [dbo].[CN_TMPNodeMembers]

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


DROP TABLE [dbo].[CN_TMPNodeMembers]
GO