USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER TABLE [dbo].[WF_WorkFlowStates]
ADD PollID uniqueidentifier NULL
GO

ALTER TABLE [dbo].[WF_WorkFlowStates]  WITH CHECK ADD  CONSTRAINT [FK_WF_WorkFlowStates_FG_Polls] FOREIGN KEY([PollID])
REFERENCES [dbo].[FG_Polls] ([PollID])
GO

ALTER TABLE [dbo].[WF_WorkFlowStates] CHECK CONSTRAINT [FK_WF_WorkFlowStates_FG_Polls]
GO

