USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[USR_UserApplications](
	[UserID] [uniqueidentifier] NOT NULL,
	[ApplicationID] [uniqueidentifier] NOT NULL
 CONSTRAINT [PK_USR_UserApplications] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[ApplicationID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[USR_UserApplications]  WITH CHECK ADD  CONSTRAINT [FK_USR_UserApplications_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[USR_UserApplications] CHECK CONSTRAINT [FK_USR_UserApplications_aspnet_Applications]
GO

ALTER TABLE [dbo].[USR_UserApplications]  WITH CHECK ADD  CONSTRAINT [FK_USR_UserApplications_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[USR_UserApplications] CHECK CONSTRAINT [FK_USR_UserApplications_aspnet_Users]
GO




