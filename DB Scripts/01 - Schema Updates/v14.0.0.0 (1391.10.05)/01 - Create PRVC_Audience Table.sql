USE [EKM_App]

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT * FROM EKM_App.sys.tables where name = 'PRVC_Audience')
DROP TABLE [dbo].[PRVC_Audience]
GO

CREATE TABLE [dbo].[PRVC_Audience](
	[RoleID] [uniqueidentifier] NOT NULL,
	[ObjectID] [uniqueidentifier] NOT NULL,
	[Allow] [bit] NOT NULL,
	[ModificationRight] [bit] NOT NULL,
	[ExpirationDate] [datetime] NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
CONSTRAINT [PK_PRVC_Audience] PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC,
	[ObjectID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

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