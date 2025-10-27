USE [EKM_App]
GO

/****** Object:  Table [dbo].[NQuestions]    Script Date: 02/17/2012 13:19:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[NQuestionsTemp](
	[QuestionID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NULL,
	[Question] [nvarchar](max) NOT NULL,
	[KnowledgeType] [bigint] NOT NULL,
	[DateTime] [datetime] NULL,
 CONSTRAINT [PK_NQuestionsTemp] PRIMARY KEY CLUSTERED 
(
	[QuestionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[NQuestionsTemp]
           ([QuestionID]
            ,[NodeID]
			,[UserID]
			,[Question]
			,[KnowledgeType]
			,[DateTime])
		SELECT  dbo.NQuestions.ID, dbo.CN_Nodes.NodeID, dbo.NQuestions.UserId, dbo.NQuestions.Question, 
				dbo.NQuestions.KnowledgeType, dbo.NQuestions.DateTime
		FROM    dbo.CN_Nodes INNER JOIN dbo.NQuestions ON dbo.CN_Nodes.ID = dbo.NQuestions.NodeId
GO


/* Drop old table */
DROP TABLE [dbo].[NQuestions]
GO

/* Create old new table */
CREATE TABLE [dbo].[NQuestions](
	[QuestionID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NULL,
	[Question] [nvarchar](max) NOT NULL,
	[KnowledgeType] [bigint] NOT NULL,
	[DateTime] [datetime] NULL,
 CONSTRAINT [PK_NQuestions] PRIMARY KEY CLUSTERED 
(
	[QuestionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[NQuestions]  WITH CHECK ADD  CONSTRAINT [FK_NQuestions_KKnowledgeType] FOREIGN KEY([KnowledgeType])
REFERENCES [dbo].[KKnowledgeType] ([ID])
GO

ALTER TABLE [dbo].[NQuestions] CHECK CONSTRAINT [FK_NQuestions_KKnowledgeType]
GO

ALTER TABLE [dbo].[NQuestions]  WITH CHECK ADD  CONSTRAINT [FK_NQuestions_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[NQuestions] CHECK CONSTRAINT [FK_NQuestions_CN_Nodes]
GO


INSERT INTO [dbo].[NQuestions]
	SELECT * FROM dbo.NQuestionsTemp
GO

DROP TABLE [dbo].[NQuestionsTemp]
GO