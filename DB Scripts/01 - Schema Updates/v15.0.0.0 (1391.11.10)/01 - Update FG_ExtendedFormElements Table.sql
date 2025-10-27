USE [EKM_App]
GO


CREATE TABLE [dbo].[FG_EFE](
	[ElementID] [uniqueidentifier] NOT NULL,
	[FormID] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](2000) NOT NULL,
	[SequenceNumber] [int] NOT NULL,
	[Type] [varchar](20) NOT NULL,
	[Info] [nvarchar](4000) NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_FG_EFE] PRIMARY KEY CLUSTERED 
(
	[ElementID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[FG_EFE](
	ElementID,
	FormID,
	Title,
	SequenceNumber,
	[Type],
	CreatorUserID,
	CreationDate,
	LastModifierUserID,
	LastModificationDate,
	Deleted
)
SELECT ElementID, FormID, Title, SequenceNumber, 'Text', CreatorUserID, CreationDate,
	   LastModifierUserID, LastModificationDate, Deleted
FROM [dbo].[FG_ExtendedFormElements]

GO


DROP TABLE [dbo].[FG_ExtendedFormElements]
GO


CREATE TABLE [dbo].[FG_ExtendedFormElements](
	[ElementID] [uniqueidentifier] NOT NULL,
	[FormID] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](2000) NOT NULL,
	[SequenceNumber] [int] NOT NULL,
	[Type] [varchar](20) NOT NULL,
	[Info] [nvarchar](4000) NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_FG_ExtendedFormElements] PRIMARY KEY CLUSTERED 
(
	[ElementID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[FG_ExtendedFormElements]  WITH CHECK ADD  CONSTRAINT [FK_FG_ExtendedFormElements_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[FG_ExtendedFormElements] CHECK CONSTRAINT [FK_FG_ExtendedFormElements_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[FG_ExtendedFormElements]  WITH CHECK ADD  CONSTRAINT [FK_FG_ExtendedFormElements_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[FG_ExtendedFormElements] CHECK CONSTRAINT [FK_FG_ExtendedFormElements_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[FG_ExtendedFormElements]  WITH CHECK ADD  CONSTRAINT [FK_FG_ExtendedFormElements_FG_ExtendedForms_FormID] FOREIGN KEY([FormID])
REFERENCES [dbo].[FG_ExtendedForms] ([FormID])
GO

ALTER TABLE [dbo].[FG_ExtendedFormElements] CHECK CONSTRAINT [FK_FG_ExtendedFormElements_FG_ExtendedForms_FormID]
GO



INSERT INTO [dbo].[FG_ExtendedFormElements]
SELECT * FROM [dbo].[FG_EFE]
GO


DROP TABLE [dbo].[FG_EFE]
GO