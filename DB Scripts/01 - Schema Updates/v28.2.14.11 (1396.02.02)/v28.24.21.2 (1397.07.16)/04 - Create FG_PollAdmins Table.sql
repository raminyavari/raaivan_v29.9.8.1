USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[FG_PollAdmins](
	[PollID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NOT NULL,
	[LastModificationDate] [datetime] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[ApplicationID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_FG_PollAdmins] PRIMARY KEY CLUSTERED 
(
	[PollID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[FG_PollAdmins]  WITH CHECK ADD  CONSTRAINT [FK_FG_PollAdmins_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[FG_PollAdmins] CHECK CONSTRAINT [FK_FG_PollAdmins_aspnet_Applications]
GO

ALTER TABLE [dbo].[FG_PollAdmins]  WITH CHECK ADD  CONSTRAINT [FK_FG_PollAdmins_FG_Polls] FOREIGN KEY([PollID])
REFERENCES [dbo].[FG_Polls] ([PollID])
GO

ALTER TABLE [dbo].[FG_PollAdmins] CHECK CONSTRAINT [FK_FG_PollAdmins_FG_Polls]
GO

ALTER TABLE [dbo].[FG_PollAdmins]  WITH CHECK ADD  CONSTRAINT [FK_FG_PollAdmins_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[FG_PollAdmins] CHECK CONSTRAINT [FK_FG_PollAdmins_aspnet_Users]
GO

ALTER TABLE [dbo].[FG_PollAdmins]  WITH CHECK ADD  CONSTRAINT [FK_FG_PollAdmins_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[FG_PollAdmins] CHECK CONSTRAINT [FK_FG_PollAdmins_aspnet_Users_Modifier]
GO