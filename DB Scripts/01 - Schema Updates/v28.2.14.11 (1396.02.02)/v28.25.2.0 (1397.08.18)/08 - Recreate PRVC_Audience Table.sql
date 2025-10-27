USE [EKM_App]
GO

/****** Object:  Table [dbo].[PRVC_Audience]    Script Date: 11/04/2018 12:25:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PRVC_TempAudience](
	[ObjectID] [uniqueidentifier] NOT NULL,
	[RoleID] [uniqueidentifier] NOT NULL,
	[PermissionType] [nvarchar](50) NOT NULL,
	[Allow] [bit] NOT NULL,
	[ExpirationDate] [datetime] NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
	[ApplicationID] [uniqueidentifier] NULL
 CONSTRAINT [PK_PRVC_TempAudience] PRIMARY KEY CLUSTERED 
(
	[ObjectID] ASC,
	[RoleID] ASC,
	[PermissionType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[PRVC_TempAudience] (ObjectID, RoleID, PermissionType, Allow, ExpirationDate,
	CreatorUserID, CreationDate, LastModifierUserID, LastModificationDate, Deleted, ApplicationID)
SELECT A.ObjectID, A.RoleID, ISNULL(A.PermissionType, N'View'), A.Allow, A.ExpirationDate, 
	A.CreatorUserID, A.CreationDate, A.LastModifierUserID, A.LastModificationDate, A.Deleted, A.ApplicationID
FROM [dbo].[PRVC_Audience] AS A
GO

DROP TABLE [dbo].[PRVC_Audience]
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[PRVC_Audience](
	[ObjectID] [uniqueidentifier] NOT NULL,
	[RoleID] [uniqueidentifier] NOT NULL,
	[PermissionType] [nvarchar](50) NOT NULL,
	[Allow] [bit] NOT NULL,
	[ExpirationDate] [datetime] NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
	[ApplicationID] [uniqueidentifier] NULL
 CONSTRAINT [PK_PRVC_Audience] PRIMARY KEY CLUSTERED 
(
	[ObjectID] ASC,
	[RoleID] ASC,
	[PermissionType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[PRVC_Audience]  WITH CHECK ADD  CONSTRAINT [FK_PRVC_Audience_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[PRVC_Audience] CHECK CONSTRAINT [FK_PRVC_Audience_aspnet_Applications]
GO

ALTER TABLE [dbo].[PRVC_Audience]  WITH CHECK ADD  CONSTRAINT [FK_PRVC_Audience_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[PRVC_Audience] CHECK CONSTRAINT [FK_PRVC_Audience_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[PRVC_Audience]  WITH CHECK ADD  CONSTRAINT [FK_PRVC_Audience_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[PRVC_Audience] CHECK CONSTRAINT [FK_PRVC_Audience_aspnet_Users_Modifier]
GO

INSERT INTO [dbo].[PRVC_Audience]
SELECT *
FROM [dbo].[PRVC_TempAudience]
GO


DROP TABLE [dbo].[PRVC_TempAudience]
GO