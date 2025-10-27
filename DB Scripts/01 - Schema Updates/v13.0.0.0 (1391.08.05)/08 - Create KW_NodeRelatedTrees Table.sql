USE [EKM_App]
GO

/****** Object:  Table [dbo].[KW_NodeRelatedTrees]    Script Date: 09/15/2012 16:40:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[KW_NodeRelatedTrees](
	[NodeID] [uniqueidentifier] NOT NULL,
	[TreeID] [uniqueidentifier] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_KW_NodeRelatedTrees] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC,
	[TreeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[KW_NodeRelatedTrees]  WITH CHECK ADD  CONSTRAINT [FK_KW_NodeRelatedTrees_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KW_NodeRelatedTrees] CHECK CONSTRAINT [FK_KW_NodeRelatedTrees_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[KW_NodeRelatedTrees]  WITH CHECK ADD  CONSTRAINT [FK_KW_NodeRelatedTrees_aspnet_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KW_NodeRelatedTrees] CHECK CONSTRAINT [FK_KW_NodeRelatedTrees_aspnet_Modifier]
GO

ALTER TABLE [dbo].[KW_NodeRelatedTrees]  WITH CHECK ADD  CONSTRAINT [FK_KW_NodeRelatedTrees_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[KW_NodeRelatedTrees] CHECK CONSTRAINT [FK_KW_NodeRelatedTrees_CN_Nodes]
GO

ALTER TABLE [dbo].[KW_NodeRelatedTrees]  WITH CHECK ADD  CONSTRAINT [FK_KW_NodeRelatedTrees_DCT_Trees] FOREIGN KEY([TreeID])
REFERENCES [dbo].[DCT_Trees] ([TreeID])
GO

ALTER TABLE [dbo].[KW_NodeRelatedTrees] CHECK CONSTRAINT [FK_KW_NodeRelatedTrees_DCT_Trees]
GO


