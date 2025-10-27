USE [EKM_APP]
GO

/****** Object:  Table [dbo].[CN_ListTypes]    Script Date: 11/04/2012 14:47:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FG_ExtendedFormElements](
	[FormID] [uniqueidentifier] NOT NULL,
	[ElementID] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
	[SequenceNumber] [int] NOT NULL,
	
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_FG_ExtendedFormElements] PRIMARY KEY CLUSTERED 
(
	[FormID] ASC,
	[ElementID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[FG_ExtendedFormElements]  WITH CHECK ADD  CONSTRAINT [FK_FG_ExtendedFormElements_FG_ExtendedForms_FormID] FOREIGN KEY([FormID])
REFERENCES [dbo].[FG_ExtendedForms] ([FormID])
GO
ALTER TABLE [dbo].[FG_ExtendedFormElements] CHECK CONSTRAINT [FK_FG_ExtendedFormElements_FG_ExtendedForms_FormID]
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


