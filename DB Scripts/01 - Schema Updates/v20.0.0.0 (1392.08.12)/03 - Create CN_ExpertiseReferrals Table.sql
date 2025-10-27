USE [EKM_App]
GO

/****** Object:  Table [dbo].[Phrases]    Script Date: 04/26/2013 20:38:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CN_ExpertiseReferrals](
	[ReferrerUserID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[Status] [bit] NULL,
	[SendDate] [datetime] NOT NULL
 CONSTRAINT [PK_CN_ExpertiseReferrals] PRIMARY KEY CLUSTERED 
(
	[ReferrerUserID] ASC,
	[NodeID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[CN_ExpertiseReferrals]  WITH CHECK ADD  CONSTRAINT [FK_CN_ExpertiseReferrals_aspnet_Users_Referrer] FOREIGN KEY([ReferrerUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_ExpertiseReferrals] CHECK CONSTRAINT [FK_CN_ExpertiseReferrals_aspnet_Users_Referrer]
GO

ALTER TABLE [dbo].[CN_ExpertiseReferrals]  WITH CHECK ADD  CONSTRAINT [FK_CN_ExpertiseReferrals_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[CN_ExpertiseReferrals] CHECK CONSTRAINT [FK_CN_ExpertiseReferrals_CN_Nodes]
GO

ALTER TABLE [dbo].[CN_ExpertiseReferrals]  WITH CHECK ADD  CONSTRAINT [FK_CN_ExpertiseReferrals_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_ExpertiseReferrals] CHECK CONSTRAINT [FK_CN_ExpertiseReferrals_aspnet_Users]
GO

