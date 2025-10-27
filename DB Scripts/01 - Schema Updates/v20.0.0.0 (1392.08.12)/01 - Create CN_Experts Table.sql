USE [EKM_App]
GO

/****** Object:  Table [dbo].[Phrases]    Script Date: 04/26/2013 20:38:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CN_Experts](
	[NodeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[Approved] [bit] NOT NULL,
	[ReferralsCount] [int] NOT NULL,
	[ConfirmsPercentage] [float] NOT NULL,
	[SocialApproved] [bit] NULL   -- NUll means 'I Am Not Expert'
 CONSTRAINT [PK_CN_Experts] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[CN_Experts]  WITH CHECK ADD  CONSTRAINT [FK_CN_Experts_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[CN_Experts] CHECK CONSTRAINT [FK_CN_Experts_CN_Nodes]
GO

ALTER TABLE [dbo].[CN_Experts]  WITH CHECK ADD  CONSTRAINT [FK_CN_Experts_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_Experts] CHECK CONSTRAINT [FK_CN_Experts_aspnet_Users]
GO

