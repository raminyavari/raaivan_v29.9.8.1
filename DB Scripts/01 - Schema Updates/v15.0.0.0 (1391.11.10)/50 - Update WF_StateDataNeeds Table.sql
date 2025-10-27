USE [EKM_App]
GO


ALTER TABLE [dbo].[WF_StateDataNeeds]
DROP CONSTRAINT [FK_WF_StateDataNeeds_FG_ExtendedForms]
GO


ALTER TABLE [dbo].[WF_StateDataNeeds]
ADD [TMPFormID] [uniqueidentifier] NULL
GO

UPDATE [dbo].[WF_StateDataNeeds]
	SET TMPFormID = FormID
GO

ALTER TABLE [dbo].[WF_StateDataNeeds]
DROP COLUMN FormID
GO


ALTER TABLE [dbo].[WF_StateDataNeeds]
ADD [FormID] [uniqueidentifier] NULL
GO

UPDATE [dbo].[WF_StateDataNeeds]
	SET FormID = TMPFormID
GO

ALTER TABLE [dbo].[WF_StateDataNeeds]
DROP COLUMN TMPFormID
GO


ALTER TABLE [dbo].[WF_StateDataNeeds]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateDataNeeds_FG_ExtendedForms] FOREIGN KEY([FormID])
REFERENCES [dbo].[FG_ExtendedForms] ([FormID])
GO

ALTER TABLE [dbo].[WF_StateDataNeeds] CHECK CONSTRAINT [FK_WF_StateDataNeeds_FG_ExtendedForms]
GO