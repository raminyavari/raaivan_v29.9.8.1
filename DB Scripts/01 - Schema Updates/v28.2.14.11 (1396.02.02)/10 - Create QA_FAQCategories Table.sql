USE [EKM_App]
GO


/****** Object:  Table [dbo].[QA_FAQCategories]    Script Date: 11/29/2016 09:33:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[QA_FAQCategories](
	[CategoryID] [uniqueidentifier] NOT NULL,
	[ParentID] [uniqueidentifier] NULL,
	[SequenceNumber] [int] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[CreatorUserID]	[uniqueidentifier] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
	[ApplicationID] [uniqueidentifier] NULL
 CONSTRAINT [PK_QA_FAQCategories] PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[QA_FAQCategories]  WITH CHECK ADD  CONSTRAINT [FK_QA_FAQCategories_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[QA_FAQCategories] CHECK CONSTRAINT [FK_QA_FAQCategories_aspnet_Applications]
GO

ALTER TABLE [dbo].[QA_FAQCategories]  WITH CHECK ADD  CONSTRAINT [FK_QA_FAQCategories_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[QA_FAQCategories] CHECK CONSTRAINT [FK_QA_FAQCategories_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[QA_FAQCategories]  WITH CHECK ADD  CONSTRAINT [FK_QA_FAQCategories_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[QA_FAQCategories] CHECK CONSTRAINT [FK_QA_FAQCategories_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[QA_FAQCategories]  WITH CHECK ADD  CONSTRAINT [FK_QA_FAQCategories_QA_FAQCategories] FOREIGN KEY([ParentID])
REFERENCES [dbo].[QA_FAQCategories] ([CategoryID])
GO

ALTER TABLE [dbo].[QA_FAQCategories] CHECK CONSTRAINT [FK_QA_FAQCategories_QA_FAQCategories]
GO

