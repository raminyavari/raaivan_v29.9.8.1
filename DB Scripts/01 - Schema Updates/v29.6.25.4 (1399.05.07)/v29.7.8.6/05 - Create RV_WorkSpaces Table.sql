USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TABLE [dbo].[RV_WorkSpaces](
	WorkSpaceID uniqueidentifier NOT NULL,
	Name nvarchar(255) NOT NULL,
	[Description] nvarchar(2000) NULL,
	AvatarName varchar(50) NULL,
	CreatorUserID uniqueidentifier NOT NULL,
	CreationDate datetime NOT NULL,
	LastModifierUserID uniqueidentifier NULL,
	LastModificationDate datetime NULL,
	Deleted bit NOT NULL
 CONSTRAINT [PK_RV_WorkSpaces] PRIMARY KEY CLUSTERED 
(
	[WorkSpaceID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[RV_WorkSpaces]  WITH CHECK ADD  CONSTRAINT [FK_RV_WorkSpaces_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[RV_WorkSpaces] CHECK CONSTRAINT [FK_RV_WorkSpaces_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[RV_WorkSpaces]  WITH CHECK ADD  CONSTRAINT [FK_RV_WorkSpaces_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[RV_WorkSpaces] CHECK CONSTRAINT [FK_RV_WorkSpaces_aspnet_Users_Modifier]
GO




