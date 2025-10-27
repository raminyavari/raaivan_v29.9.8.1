USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[PRVC_DefaultPermissions](
	[ObjectID] [uniqueidentifier] NOT NULL,
	[PermissionType] [varchar](20) NOT NULL,
	[DefaultValue] [varchar](20) NOT NULL,
	[CreatorUserID] [uniqueidentifier] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[ApplicationID] [uniqueidentifier] NULL
 CONSTRAINT [PK_PRVC_DefaultPermissions] PRIMARY KEY CLUSTERED 
(
	[ObjectID] ASC,
	[PermissionType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[PRVC_DefaultPermissions]  WITH CHECK ADD  CONSTRAINT [FK_PRVC_DefaultPermissions_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[PRVC_DefaultPermissions] CHECK CONSTRAINT [FK_PRVC_DefaultPermissions_aspnet_Applications]
GO

ALTER TABLE [dbo].[PRVC_DefaultPermissions]  WITH CHECK ADD  CONSTRAINT [FK_PRVC_DefaultPermissions_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[PRVC_DefaultPermissions] CHECK CONSTRAINT [FK_PRVC_DefaultPermissions_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[PRVC_DefaultPermissions]  WITH CHECK ADD  CONSTRAINT [FK_PRVC_DefaultPermissions_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[PRVC_DefaultPermissions] CHECK CONSTRAINT [FK_PRVC_DefaultPermissions_aspnet_Users_Modifier]
GO
