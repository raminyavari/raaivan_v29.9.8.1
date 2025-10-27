USE [EKM_App]
GO


CREATE TABLE [dbo].[FG_InstanceElements](
	[ElementID] [uniqueidentifier] NOT NULL,
	[InstanceID] [uniqueidentifier] NOT NULL,
	[RefElementID] [uniqueidentifier] NULL,
	[Title] [nvarchar](2000) NOT NULL,
	[BodyText] [nvarchar](max) NOT NULL,
	[SequenceNumber] [int] NOT NULL,
	[Type] [varchar](20) NOT NULL,
	[Info] [nvarchar](4000) NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
CONSTRAINT [PK_FG_InstanceElements] PRIMARY KEY CLUSTERED 
(
	[ElementID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[FG_InstanceElements]  WITH CHECK ADD  CONSTRAINT [FK_FG_InstanceElements_FG_FormInstances] FOREIGN KEY([InstanceID])
REFERENCES [dbo].[FG_FormInstances] ([InstanceID])
GO

ALTER TABLE [dbo].[FG_InstanceElements] CHECK CONSTRAINT [FK_FG_InstanceElements_FG_FormInstances]
GO

ALTER TABLE [dbo].[FG_InstanceElements]  WITH CHECK ADD  CONSTRAINT [FK_FG_InstanceElements_FG_ExtendedFormElements] FOREIGN KEY([RefElementID])
REFERENCES [dbo].[FG_ExtendedFormElements] ([ElementID])
GO

ALTER TABLE [dbo].[FG_InstanceElements] CHECK CONSTRAINT [FK_FG_InstanceElements_FG_ExtendedFormElements]
GO

ALTER TABLE [dbo].[FG_InstanceElements]  WITH CHECK ADD  CONSTRAINT [FK_FG_InstanceElements_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[FG_InstanceElements] CHECK CONSTRAINT [FK_FG_InstanceElements_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[FG_InstanceElements]  WITH CHECK ADD  CONSTRAINT [FK_FG_InstanceElements_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[FG_InstanceElements] CHECK CONSTRAINT [FK_FG_InstanceElements_aspnet_Users_Modifier]
GO


