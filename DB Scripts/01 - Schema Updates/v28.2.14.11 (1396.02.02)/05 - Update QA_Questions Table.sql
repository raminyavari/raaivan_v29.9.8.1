USE [EKM_App]
GO

ALTER TABLE [dbo].[QA_Questions]
DROP COLUMN [VisitsCount]
GO

ALTER TABLE [dbo].[QA_Questions]
ADD [BestAnswerID] [uniqueidentifier] NULL
GO

ALTER TABLE [dbo].[QA_Questions]
ADD [WorkFlowID] [uniqueidentifier] NULL
GO

ALTER TABLE [dbo].[QA_Questions]  WITH CHECK ADD  CONSTRAINT [FK_QA_Questions_QA_Answers] FOREIGN KEY([BestAnswerID])
REFERENCES [dbo].[QA_Answers] ([AnswerID])
GO

ALTER TABLE [dbo].[QA_Questions] CHECK CONSTRAINT [FK_QA_Questions_QA_Answers]
GO

ALTER TABLE [dbo].[QA_Questions]  WITH CHECK ADD  CONSTRAINT [FK_QA_Questions_QA_WorkFlows] FOREIGN KEY([WorkFlowID])
REFERENCES [dbo].[QA_WorkFlows] ([WorkFlowID])
GO

ALTER TABLE [dbo].[QA_Questions] CHECK CONSTRAINT [FK_QA_Questions_QA_WorkFlows]
GO