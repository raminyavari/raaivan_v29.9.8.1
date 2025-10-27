USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[FG_SelectedItems](
	[ElementID] [uniqueidentifier] NOT NULL,
	[SelectedID] [uniqueidentifier] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NOT NULL,
	[LastModificationDate] [datetime] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[ApplicationID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_FG_SelectedItems] PRIMARY KEY CLUSTERED 
(
	[ElementID] ASC,
	[SelectedID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[FG_SelectedItems]  WITH CHECK ADD  CONSTRAINT [FK_FG_SelectedItems_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[FG_SelectedItems] CHECK CONSTRAINT [FK_FG_SelectedItems_aspnet_Applications]
GO

ALTER TABLE [dbo].[FG_SelectedItems]  WITH CHECK ADD  CONSTRAINT [FK_FG_SelectedItems_FG_InstanceElements] FOREIGN KEY([ElementID])
REFERENCES [dbo].[FG_InstanceElements] ([ElementID])
GO

ALTER TABLE [dbo].[FG_SelectedItems] CHECK CONSTRAINT [FK_FG_SelectedItems_FG_InstanceElements]
GO

ALTER TABLE [dbo].[FG_SelectedItems]  WITH CHECK ADD  CONSTRAINT [FK_FG_SelectedItems_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[FG_SelectedItems] CHECK CONSTRAINT [FK_FG_SelectedItems_aspnet_Users_Modifier]
GO


