USE [EKM_App]
GO


CREATE TABLE [dbo].[FG_FormInstances](
	[InstanceID] [uniqueidentifier] NOT NULL,
	[FormID] [uniqueidentifier] NOT NULL,
	[OwnerID] [uniqueidentifier] NOT NULL,
	[DirectorID] [uniqueidentifier] NOT NULL,
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


ALTER TABLE [dbo].[FG_FormInstances]  WITH CHECK ADD  CONSTRAINT [FK_FG_FormInstances_FG_ExtendedForms] FOREIGN KEY([FormID])
REFERENCES [dbo].[FG_ExtendedForms] ([FormID])
GO

ALTER TABLE [dbo].[FG_FormInstances] CHECK CONSTRAINT [FK_FG_FormInstances_FG_ExtendedForms]
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


