USE [EKM_App]
GO

/****** Object:  Table [dbo].[Phrases]    Script Date: 04/26/2013 20:38:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TABLE [dbo].[TMP_KW_CandidateRelations](
	[ID] [uniqueidentifier] NOT NULL,
	[KnowledgeTypeID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NULL,
	[NodeTypeID] [uniqueidentifier] NULL,
	[CreatorUserID] [uniqueidentifier] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_TMP_KW_CandidateRelations] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[TMP_KW_CandidateRelations]  WITH CHECK ADD  CONSTRAINT [FK_TMP_KW_CandidateRelations_TMP_KW_KnowledgeTypes] FOREIGN KEY([KnowledgeTypeID])
REFERENCES [dbo].[TMP_KW_KnowledgeTypes] ([KnowledgeTypeID])
GO

ALTER TABLE [dbo].[TMP_KW_CandidateRelations] CHECK CONSTRAINT [FK_TMP_KW_CandidateRelations_TMP_KW_KnowledgeTypes]
GO

ALTER TABLE [dbo].[TMP_KW_CandidateRelations]  WITH CHECK ADD  CONSTRAINT [FK_TMP_KW_CandidateRelations_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[TMP_KW_CandidateRelations] CHECK CONSTRAINT [FK_TMP_KW_CandidateRelations_CN_Nodes]
GO

ALTER TABLE [dbo].[TMP_KW_CandidateRelations]  WITH CHECK ADD  CONSTRAINT [FK_TMP_KW_CandidateRelations_CN_NodeTypes] FOREIGN KEY([NodeTypeID])
REFERENCES [dbo].[CN_NodeTypes] ([NodeTypeID])
GO

ALTER TABLE [dbo].[TMP_KW_CandidateRelations] CHECK CONSTRAINT [FK_TMP_KW_CandidateRelations_CN_NodeTypes]
GO

ALTER TABLE [dbo].[TMP_KW_CandidateRelations]  WITH CHECK ADD  CONSTRAINT [FK_TMP_KW_CandidateRelations_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[TMP_KW_CandidateRelations] CHECK CONSTRAINT [FK_TMP_KW_CandidateRelations_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[TMP_KW_CandidateRelations]  WITH CHECK ADD  CONSTRAINT [FK_TMP_KW_CandidateRelations_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[TMP_KW_CandidateRelations] CHECK CONSTRAINT [FK_TMP_KW_CandidateRelations_aspnet_Users_Modifier]
GO