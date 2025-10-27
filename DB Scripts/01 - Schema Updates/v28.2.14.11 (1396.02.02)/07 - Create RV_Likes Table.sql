USE [EKM_App]
GO


/****** Object:  Table [dbo].[RV_Likes]    Script Date: 11/29/2016 09:33:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[RV_Likes](
	[UserID] [uniqueidentifier] NOT NULL,
	[LikedID] [uniqueidentifier] NOT NULL,
	[Like] [bit] NOT NULL,
	[ActionDate] [datetime] NOT NULL,
	[ApplicationID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_RV_Likes] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[LikedID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[RV_Likes]  WITH CHECK ADD  CONSTRAINT [FK_RV_Likes_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[RV_Likes] CHECK CONSTRAINT [FK_RV_Likes_aspnet_Applications]
GO

ALTER TABLE [dbo].[RV_Likes]  WITH CHECK ADD  CONSTRAINT [FK_RV_Likes_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[RV_Likes] CHECK CONSTRAINT [FK_RV_Likes_aspnet_Users]
GO

