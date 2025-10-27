USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[PRVC_Settings](
	[ObjectID] [uniqueidentifier] NOT NULL,
	[ConfidentialityID] [uniqueidentifier] NULL,
	[CalculateHierarchy] [bit] NULL,
	[CreatorUserID] [uniqueidentifier] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[ApplicationID] [uniqueidentifier] NULL
 CONSTRAINT [PK_PRVC_Settings] PRIMARY KEY CLUSTERED 
(
	[ObjectID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[PRVC_Settings]  WITH CHECK ADD  CONSTRAINT [FK_PRVC_Settings_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[PRVC_Settings] CHECK CONSTRAINT [FK_PRVC_Settings_aspnet_Applications]
GO

ALTER TABLE [dbo].[PRVC_Settings]  WITH CHECK ADD  CONSTRAINT [FK_PRVC_Settings_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[PRVC_Settings] CHECK CONSTRAINT [FK_PRVC_Settings_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[PRVC_Settings]  WITH CHECK ADD  CONSTRAINT [FK_PRVC_Settings_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[PRVC_Settings] CHECK CONSTRAINT [FK_PRVC_Settings_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[PRVC_Settings]  WITH CHECK ADD  CONSTRAINT [FK_PRVC_Settings_PRVC_ConfidentialityLevels] FOREIGN KEY([ConfidentialityID])
REFERENCES [dbo].[PRVC_ConfidentialityLevels] ([ID])
GO

ALTER TABLE [dbo].[PRVC_Settings] CHECK CONSTRAINT [FK_PRVC_Settings_PRVC_ConfidentialityLevels]
GO


