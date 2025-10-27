USE [EKM_App]
GO

/****** Object:  Table [dbo].[NQuestions]    Script Date: 10/21/2011 14:11:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER TABLE [dbo].[NQuestions]  WITH CHECK ADD  CONSTRAINT [FK_NQuestions_KKnowledgeType] FOREIGN KEY([KnowledgeType])
REFERENCES [dbo].[KKnowledgeType] ([ID])
GO

ALTER TABLE [dbo].[NQuestions] CHECK CONSTRAINT [FK_NQuestions_KKnowledgeType]
GO


