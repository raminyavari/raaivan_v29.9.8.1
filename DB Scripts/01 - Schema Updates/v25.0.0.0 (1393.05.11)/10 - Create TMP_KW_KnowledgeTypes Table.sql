USE [EKM_App]
GO

/****** Object:  Table [dbo].[Phrases]    Script Date: 04/26/2013 20:38:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TABLE [dbo].[TMP_KW_KnowledgeTypes](
	[KnowledgeTypeID] [uniqueidentifier] NOT NULL,
	[EvaluationType] [varchar](20) NULL,
	[Evaluators] [varchar](20) NULL,
	[MinEvaluationsCount] [int] NULL,
	[NodeSelectType] [varchar](20) NULL,
	[SearchableAfter] [varchar](20) NULL,
	[ScoreScale] [int] NULL,
	[MinAcceptableScore] [int] NULL,
	[CreatorUserID] [uniqueidentifier] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_TMP_KW_KnowledgeTypes] PRIMARY KEY CLUSTERED 
(
	[KnowledgeTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[TMP_KW_KnowledgeTypes]  WITH CHECK ADD  CONSTRAINT [FK_TMP_KW_KnowledgeTypes_CN_NodeTypes] FOREIGN KEY([KnowledgeTypeID])
REFERENCES [dbo].[CN_NodeTypes] ([NodeTypeID])
GO

ALTER TABLE [dbo].[TMP_KW_KnowledgeTypes] CHECK CONSTRAINT [FK_TMP_KW_KnowledgeTypes_CN_NodeTypes]
GO

ALTER TABLE [dbo].[TMP_KW_KnowledgeTypes]  WITH CHECK ADD  CONSTRAINT [FK_TMP_KW_KnowledgeTypes_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[TMP_KW_KnowledgeTypes] CHECK CONSTRAINT [FK_TMP_KW_KnowledgeTypes_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[TMP_KW_KnowledgeTypes]  WITH CHECK ADD  CONSTRAINT [FK_TMP_KW_KnowledgeTypes_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[TMP_KW_KnowledgeTypes] CHECK CONSTRAINT [FK_TMP_KW_KnowledgeTypes_aspnet_Users_Modifier]
GO
