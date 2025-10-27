USE [EKM_App]
GO

/****** Object:  Table [dbo].[FG_FormInstances]    Script Date: 05/14/2014 18:19:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

ALTER TABLE [dbo].[FG_InstanceElements] 
DROP CONSTRAINT [FK_FG_InstanceElements_FG_FormInstances]
GO

CREATE TABLE [dbo].[FG_TMPFormInstances](
	[InstanceID] [uniqueidentifier] NOT NULL,
	[FormID] [uniqueidentifier] NOT NULL,
	[OwnerID] [uniqueidentifier] NOT NULL,
	[OwnerType] [varchar](20) NULL,
	[DirectorID] [uniqueidentifier] NULL,
	[Admin] [bit] NOT NULL,
	[Filled] [bit] NOT NULL,
	[FillingDate] [datetime] NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_FG_TMPFormInstances] PRIMARY KEY CLUSTERED 
(
	[InstanceID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


INSERT INTO [dbo].[FG_TMPFormInstances](InstanceID, FormID, OwnerID, OwnerType,
	DirectorID, [Admin], Filled, FillingDate, CreatorUserID, CreationDate,
	LastModifierUserID, LastModificationDate, Deleted)
SELECT InstanceID, FormID, OwnerID, OwnerType, DirectorID, [Admin], Filled, FillingDate, 
	CreatorUserID, CreationDate, LastModifierUserID, LastModificationDate, Deleted
FROM [dbo].[FG_FormInstances]

GO

DROP TABLE [dbo].[FG_FormInstances]
GO

CREATE TABLE [dbo].[FG_FormInstances](
	[InstanceID] [uniqueidentifier] NOT NULL,
	[FormID] [uniqueidentifier] NOT NULL,
	[OwnerID] [uniqueidentifier] NOT NULL,
	[OwnerType] [varchar](20) NULL,
	[DirectorID] [uniqueidentifier] NULL,
	[Admin] [bit] NOT NULL,
	[Filled] [bit] NOT NULL,
	[FillingDate] [datetime] NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_FG_FormInstances] PRIMARY KEY CLUSTERED 
(
	[InstanceID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[FG_FormInstances]  WITH CHECK ADD  CONSTRAINT [FK_FG_FormInstances_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[FG_FormInstances] CHECK CONSTRAINT [FK_FG_FormInstances_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[FG_FormInstances]  WITH CHECK ADD  CONSTRAINT [FK_FG_FormInstances_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[FG_FormInstances] CHECK CONSTRAINT [FK_FG_FormInstances_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[FG_FormInstances]  WITH CHECK ADD  CONSTRAINT [FK_FG_FormInstances_FG_ExtendedForms] FOREIGN KEY([FormID])
REFERENCES [dbo].[FG_ExtendedForms] ([FormID])
GO

ALTER TABLE [dbo].[FG_FormInstances] CHECK CONSTRAINT [FK_FG_FormInstances_FG_ExtendedForms]
GO



INSERT INTO [dbo].[FG_FormInstances]
SELECT * FROM [dbo].[FG_TMPFormInstances]

GO

DROP TABLE [dbo].[FG_TMPFormInstances]
GO


ALTER TABLE [dbo].[FG_InstanceElements]  WITH CHECK ADD  CONSTRAINT [FK_FG_InstanceElements_FG_FormInstances] FOREIGN KEY([InstanceID])
REFERENCES [dbo].[FG_FormInstances] ([InstanceID])
GO

ALTER TABLE [dbo].[FG_InstanceElements] CHECK CONSTRAINT [FK_FG_InstanceElements_FG_FormInstances]
GO