USE [EKM_App]
GO

/****** Object:  Table [dbo].[KWF_Paraphs]    Script Date: 02/01/2012 15:35:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[KWF_Paraphs](
	[ParaphID] [uniqueidentifier] NOT NULL,
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[ParaphDate] [datetime] NOT NULL,
	[Text] [nvarchar](1000) NOT NULL,
 CONSTRAINT [PK_KWF_Paraphs] PRIMARY KEY CLUSTERED 
(
	[ParaphID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[KWF_Paraphs]  WITH CHECK ADD  CONSTRAINT [FK_KWF_Paraphs_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KWF_Paraphs] CHECK CONSTRAINT [FK_KWF_Paraphs_aspnet_Users]
GO

ALTER TABLE [dbo].[KWF_Paraphs]  WITH CHECK ADD  CONSTRAINT [FK_KWF_Paraphs_KKnowledges] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[KKnowledges] ([ID])
GO

ALTER TABLE [dbo].[KWF_Paraphs] CHECK CONSTRAINT [FK_KWF_Paraphs_KKnowledges]
GO

