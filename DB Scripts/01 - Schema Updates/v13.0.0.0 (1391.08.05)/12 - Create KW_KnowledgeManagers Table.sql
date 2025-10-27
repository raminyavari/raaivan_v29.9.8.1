USE [EKM_App]
GO

/****** Object:  Table [dbo].[KW_KnowledgeManagers]    Script Date: 09/15/2012 16:40:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[KW_KnowledgeManagers](
	[ListID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_KW_KnowledgeManagers] PRIMARY KEY CLUSTERED 
(
	[ListID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[KW_KnowledgeManagers]  WITH CHECK ADD  CONSTRAINT [FK_KW_KnowledgeManagers_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KW_KnowledgeManagers] CHECK CONSTRAINT [FK_KW_KnowledgeManagers_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[KW_KnowledgeManagers]  WITH CHECK ADD  CONSTRAINT [FK_KW_KnowledgeManagers_aspnet_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KW_KnowledgeManagers] CHECK CONSTRAINT [FK_KW_KnowledgeManagers_aspnet_Modifier]
GO

ALTER TABLE [dbo].[KW_KnowledgeManagers]  WITH CHECK ADD  CONSTRAINT [FK_KW_KnowledgeManagers_CN_Lists] FOREIGN KEY([ListID])
REFERENCES [dbo].[CN_Lists] ([ListID])
GO

ALTER TABLE [dbo].[KW_KnowledgeManagers] CHECK CONSTRAINT [FK_KW_KnowledgeManagers_CN_Lists]
GO

ALTER TABLE [dbo].[KW_KnowledgeManagers]  WITH CHECK ADD  CONSTRAINT [FK_KW_KnowledgeManagers_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KW_KnowledgeManagers] CHECK CONSTRAINT [FK_KW_KnowledgeManagers_aspnet_Users]
GO


