USE [EKM_App]
GO

/****** Object:  Table [dbo].[CN_Experts]    Script Date: 08/01/2015 20:15:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(select * FROM sys.views where name = 'CN_View_ExpertiseReferrals')
DROP VIEW [dbo].[CN_View_ExpertiseReferrals]
GO

IF EXISTS(select * FROM sys.views where name = 'CN_View_Experts')
DROP VIEW [dbo].[CN_View_Experts]
GO

IF EXISTS(select * FROM sys.views where name = 'CN_View_ListExperts')
DROP VIEW [dbo].[CN_View_ListExperts]
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[CN_TMPExperts](
	[NodeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[Approved] [bit] NOT NULL,
	[ReferralsCount] [int] NOT NULL,
	[ConfirmsPercentage] [float] NOT NULL,
	[SocialApproved] [bit] NULL,
 CONSTRAINT [PK_CN_TMPExperts] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[CN_TMPExperts](
	NodeID,
	UserID,
	Approved,
	ReferralsCount,
	ConfirmsPercentage,
	SocialApproved
)
SELECT NodeID, UserID, Approved, ReferralsCount, ConfirmsPercentage, SocialApproved
FROM [dbo].[CN_Experts]

GO


DROP TABLE [dbo].[CN_Experts]
GO


CREATE TABLE [dbo].[CN_Experts](
	[NodeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[Approved] [bit] NOT NULL,
	[ReferralsCount] [int] NOT NULL,
	[ConfirmsPercentage] [float] NOT NULL,
	[SocialApproved] [bit] NULL,
	[UniqueID] [uniqueidentifier] NOT NULL
 CONSTRAINT [PK_CN_Experts] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[CN_Experts](
	NodeID,
	UserID,
	Approved,
	ReferralsCount,
	ConfirmsPercentage,
	SocialApproved,
	UniqueID
)
SELECT NodeID, UserID, Approved, ReferralsCount, ConfirmsPercentage, SocialApproved, NEWID()
FROM [dbo].[CN_TMPExperts]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[CN_Experts]  WITH CHECK ADD  CONSTRAINT [FK_CN_Experts_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_Experts] CHECK CONSTRAINT [FK_CN_Experts_aspnet_Users]
GO

ALTER TABLE [dbo].[CN_Experts]  WITH CHECK ADD  CONSTRAINT [FK_CN_Experts_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[CN_Experts] CHECK CONSTRAINT [FK_CN_Experts_CN_Nodes]
GO


DROP TABLE [dbo].[CN_TMPExperts]
GO