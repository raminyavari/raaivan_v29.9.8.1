USE [EKM_APP]
GO

/****** Object:  Table [dbo].[KnowledgeNodes]    Script Date: 12/17/2011 12:30:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[KnowledgeNodes](
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[NodeID] [bigint] NOT NULL,
	[Score] [float] NULL,
	[ScoresWeight] [float] NULL,
	[StatusID] [bigint] NULL
 CONSTRAINT [PK_KnowledgeNodes] PRIMARY KEY CLUSTERED 
(
	[KnowledgeID] ASC,
	[NodeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[KnowledgeNodes]  WITH CHECK ADD  CONSTRAINT [FK_KnowledgeNodes_KKnowledges] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[KKnowledges] ([ID])
GO

ALTER TABLE [dbo].[KnowledgeNodes] CHECK CONSTRAINT [FK_KnowledgeNodes_KKnowledges]
GO

ALTER TABLE [dbo].[KnowledgeNodes]  WITH CHECK ADD  CONSTRAINT [FK_KnowledgeNodes_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[Nodes] ([ID])
GO

ALTER TABLE [dbo].[KnowledgeNodes] CHECK CONSTRAINT [FK_KnowledgeNodes_Nodes]
GO

ALTER TABLE [dbo].[KnowledgeNodes]  WITH CHECK ADD  CONSTRAINT [FK_KnowledgeNodes_KWF_Statuses] FOREIGN KEY([StatusID])
REFERENCES [dbo].[KWF_Statuses] ([StatusID])
GO

ALTER TABLE [dbo].[KnowledgeNodes] CHECK CONSTRAINT [FK_KnowledgeNodes_KWF_Statuses]
GO

