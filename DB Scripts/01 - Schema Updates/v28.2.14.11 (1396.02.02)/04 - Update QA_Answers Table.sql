USE [EKM_App]
GO

ALTER TABLE [dbo].[QA_Answers]
DROP COLUMN [Status]
GO

ALTER TABLE [dbo].[QA_Answers]
DROP COLUMN [AcceptionDate]
GO

ALTER TABLE [dbo].[QA_Answers]
DROP COLUMN [Rate]
GO

ALTER TABLE [dbo].[QA_Answers]
DROP CONSTRAINT [FK_QA_Answers_CN_Nodes]
GO

ALTER TABLE [dbo].[QA_Answers]
DROP COLUMN [NodeID]
GO