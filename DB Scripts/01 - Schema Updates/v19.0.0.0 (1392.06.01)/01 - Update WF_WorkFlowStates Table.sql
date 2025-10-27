USE [EKM_App]
GO

/****** Object:  Table [dbo].[WF_WorkFlowStates]    Script Date: 06/17/2013 10:19:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER TABLE [dbo].[WF_WorkFlowStates]
ADD [MaxAllowedRejections] [int] NULL,
	[RejectionTitle] [nvarchar](255) NULL,
	[RejectionRefStateID] [uniqueidentifier] NULL
GO

ALTER TABLE [dbo].[WF_WorkFlowStates]  WITH CHECK ADD  CONSTRAINT [FK_WF_WorkFlowStates_WF_States_Rejection] FOREIGN KEY([RejectionRefStateID])
REFERENCES [dbo].[WF_States] ([StateID])
GO

ALTER TABLE [dbo].[WF_WorkFlowStates] CHECK CONSTRAINT [FK_WF_WorkFlowStates_WF_States_Rejection]
GO
