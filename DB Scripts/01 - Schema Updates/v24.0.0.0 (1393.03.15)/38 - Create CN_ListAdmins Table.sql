USE [EKM_App]
GO

/****** Object:  Table [dbo].[CN_Lists]    Script Date: 02/10/2014 11:02:07 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[CN_ListAdmins](
	[ListID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_CN_ListAdmins] PRIMARY KEY CLUSTERED 
(
	[ListID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[CN_ListAdmins]  WITH CHECK ADD  CONSTRAINT [FK_CN_ListAdmins_CN_Lists] FOREIGN KEY([ListID])
REFERENCES [dbo].[CN_Lists] ([ListID])
GO

ALTER TABLE [dbo].[CN_ListAdmins] CHECK CONSTRAINT [FK_CN_ListAdmins_CN_Lists]
GO

ALTER TABLE [dbo].[CN_ListAdmins]  WITH CHECK ADD  CONSTRAINT [FK_CN_ListAdmins_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_ListAdmins] CHECK CONSTRAINT [FK_CN_ListAdmins_aspnet_Users]
GO

ALTER TABLE [dbo].[CN_ListAdmins]  WITH CHECK ADD  CONSTRAINT [FK_CN_ListAdmins_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_ListAdmins] CHECK CONSTRAINT [FK_CN_ListAdmins_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[CN_ListAdmins]  WITH CHECK ADD  CONSTRAINT [FK_CN_ListAdmins_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_ListAdmins] CHECK CONSTRAINT [FK_CN_ListAdmins_aspnet_Users_Modifier]
GO