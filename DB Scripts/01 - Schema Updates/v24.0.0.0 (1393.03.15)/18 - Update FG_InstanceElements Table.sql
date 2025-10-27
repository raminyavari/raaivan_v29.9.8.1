USE [EKM_App]
GO

/****** Object:  Table [dbo].[FG_InstanceElements]    Script Date: 04/30/2014 18:41:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[FG_TMPInstanceElements](
	[ElementID] [uniqueidentifier] NOT NULL,
	[InstanceID] [uniqueidentifier] NOT NULL,
	[RefElementID] [uniqueidentifier] NULL,
	[Title] [nvarchar](2000) NOT NULL,
	[SequenceNumber] [int] NOT NULL,
	[Type] [varchar](20) NOT NULL,
	[Info] [nvarchar](4000) NULL,
	[TextValue] [nvarchar](max) NULL,
	[FloatValue] [float] NULL,
	[BitValue] [bit] NULL,
	[DateValue] [datetime] NULL,
	[GuidValue] [uniqueidentifier] NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_FG_TMPInstanceElements] PRIMARY KEY CLUSTERED 
(
	[ElementID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


INSERT INTO [dbo].[FG_TMPInstanceElements]([ElementID], [InstanceID], [RefElementID], 
	[Title], [SequenceNumber], [Type], [Info], [TextValue], [CreatorUserID], 
	[CreationDate], [LastModifierUserID], [LastModificationDate], [Deleted]
)
SELECT ElementID, InstanceID, RefElementID, Title, SequenceNumber, [Type], Info, BodyText, 
	CreatorUserID, CreationDate, LastModifierUserID, LastModificationDate, Deleted
FROM [dbo].[FG_InstanceElements]

GO


DROP TABLE [dbo].[FG_InstanceElements]
GO


CREATE TABLE [dbo].[FG_InstanceElements](
	[ElementID] [uniqueidentifier] NOT NULL,
	[InstanceID] [uniqueidentifier] NOT NULL,
	[RefElementID] [uniqueidentifier] NULL,
	[Title] [nvarchar](2000) NOT NULL,
	[SequenceNumber] [int] NOT NULL,
	[Type] [varchar](20) NOT NULL,
	[Info] [nvarchar](4000) NULL,
	[TextValue] [nvarchar](max) NULL,
	[FloatValue] [float] NULL,
	[BitValue] [bit] NULL,
	[DateValue] [datetime] NULL,
	[GuidValue] [uniqueidentifier] NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_FG_InstanceElements] PRIMARY KEY CLUSTERED 
(
	[ElementID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

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

ALTER TABLE [dbo].[FG_InstanceElements]  WITH CHECK ADD  CONSTRAINT [FK_FG_InstanceElements_FG_ExtendedFormElements] FOREIGN KEY([RefElementID])
REFERENCES [dbo].[FG_ExtendedFormElements] ([ElementID])
GO

ALTER TABLE [dbo].[FG_InstanceElements] CHECK CONSTRAINT [FK_FG_InstanceElements_FG_ExtendedFormElements]
GO

ALTER TABLE [dbo].[FG_InstanceElements]  WITH CHECK ADD  CONSTRAINT [FK_FG_InstanceElements_FG_FormInstances] FOREIGN KEY([InstanceID])
REFERENCES [dbo].[FG_FormInstances] ([InstanceID])
GO

ALTER TABLE [dbo].[FG_InstanceElements] CHECK CONSTRAINT [FK_FG_InstanceElements_FG_FormInstances]
GO


INSERT INTO [dbo].[FG_InstanceElements]
SELECT *
FROM [dbo].[FG_TMPInstanceElements]
GO

DROP TABLE [dbo].[FG_TMPInstanceElements]
GO