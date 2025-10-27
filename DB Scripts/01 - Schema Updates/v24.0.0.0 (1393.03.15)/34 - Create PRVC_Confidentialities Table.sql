USE [EKM_App]
GO

/****** Object:  Table [dbo].[Phrases]    Script Date: 04/26/2013 20:38:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TABLE [dbo].[PRVC_Confidentialities](
	[LevelID] [uniqueidentifier] NOT NULL,
	[ItemID] [uniqueidentifier] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_PRVC_Confidentialities] PRIMARY KEY CLUSTERED 
(
	[LevelID] ASC,
	[ItemID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[PRVC_Confidentialities]  WITH CHECK ADD  CONSTRAINT [FK_PRVC_Confidentialities_PRVC_ConfidentialityLevels] FOREIGN KEY([LevelID])
REFERENCES [dbo].[PRVC_ConfidentialityLevels] ([ID])
GO

ALTER TABLE [dbo].[PRVC_Confidentialities] CHECK CONSTRAINT [FK_PRVC_Confidentialities_PRVC_ConfidentialityLevels]
GO

ALTER TABLE [dbo].[PRVC_Confidentialities]  WITH CHECK ADD  CONSTRAINT [FK_PRVC_Confidentialities_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[PRVC_Confidentialities] CHECK CONSTRAINT [FK_PRVC_Confidentialities_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[PRVC_Confidentialities]  WITH CHECK ADD  CONSTRAINT [FK_PRVC_Confidentialities_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[PRVC_Confidentialities] CHECK CONSTRAINT [FK_PRVC_Confidentialities_aspnet_Users_Modifier]
GO
