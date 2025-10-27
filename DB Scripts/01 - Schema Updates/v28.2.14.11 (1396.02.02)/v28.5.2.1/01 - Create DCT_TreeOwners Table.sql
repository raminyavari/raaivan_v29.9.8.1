USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DCT_TreeOwners](
	[OwnerID] [uniqueidentifier] NOT NULL,
	[TreeID] [uniqueidentifier] NOT NULL,
	[UniqueID] [uniqueidentifier] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
	[ApplicationID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_DCT_TreeOwners] PRIMARY KEY CLUSTERED 
(
	[OwnerID] ASC,
	[TreeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[DCT_TreeOwners]  WITH CHECK ADD  CONSTRAINT [FK_DCT_TreeOwners_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[DCT_TreeOwners] CHECK CONSTRAINT [FK_DCT_TreeOwners_aspnet_Applications]
GO

ALTER TABLE [dbo].[DCT_TreeOwners]  WITH CHECK ADD  CONSTRAINT [FK_DCT_TreeOwners_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[DCT_TreeOwners] CHECK CONSTRAINT [FK_DCT_TreeOwners_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[DCT_TreeOwners]  WITH CHECK ADD  CONSTRAINT [FK_DCT_TreeOwners_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[DCT_TreeOwners] CHECK CONSTRAINT [FK_DCT_TreeOwners_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[DCT_TreeOwners]  WITH CHECK ADD  CONSTRAINT [FK_DCT_TreeOwners_DCT_Trees] FOREIGN KEY([TreeID])
REFERENCES [dbo].[DCT_Trees] ([TreeID])
GO

ALTER TABLE [dbo].[DCT_TreeOwners] CHECK CONSTRAINT [FK_DCT_TreeOwners_DCT_Trees]
GO


