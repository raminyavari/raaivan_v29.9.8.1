USE [EKM_App]
GO

/****** Object:  Table [dbo].[QA_CandidateRelations]    Script Date: 02/21/2017 16:16:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[QA_CandidateRelations](
	[ID] [uniqueidentifier] NOT NULL,
	[WorkFlowID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NULL,
	[NodeTypeID] [uniqueidentifier] NULL,
	[CreatorUserID] [uniqueidentifier] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
	[ApplicationID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QA_CandidateRelations] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[QA_CandidateRelations]  WITH CHECK ADD  CONSTRAINT [FK_QA_CandidateRelations_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[QA_CandidateRelations] CHECK CONSTRAINT [FK_QA_CandidateRelations_aspnet_Applications]
GO

ALTER TABLE [dbo].[QA_CandidateRelations]  WITH CHECK ADD  CONSTRAINT [FK_QA_CandidateRelations_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[QA_CandidateRelations] CHECK CONSTRAINT [FK_QA_CandidateRelations_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[QA_CandidateRelations]  WITH CHECK ADD  CONSTRAINT [FK_QA_CandidateRelations_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[QA_CandidateRelations] CHECK CONSTRAINT [FK_QA_CandidateRelations_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[QA_CandidateRelations]  WITH CHECK ADD  CONSTRAINT [FK_QA_CandidateRelations_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[QA_CandidateRelations] CHECK CONSTRAINT [FK_QA_CandidateRelations_CN_Nodes]
GO

ALTER TABLE [dbo].[QA_CandidateRelations]  WITH CHECK ADD  CONSTRAINT [FK_QA_CandidateRelations_CN_NodeTypes] FOREIGN KEY([NodeTypeID])
REFERENCES [dbo].[CN_NodeTypes] ([NodeTypeID])
GO

ALTER TABLE [dbo].[QA_CandidateRelations] CHECK CONSTRAINT [FK_QA_CandidateRelations_CN_NodeTypes]
GO

ALTER TABLE [dbo].[QA_CandidateRelations]  WITH CHECK ADD  CONSTRAINT [FK_QA_CandidateRelations_QA_WorkFlows] FOREIGN KEY([WorkFlowID])
REFERENCES [dbo].[QA_WorkFlows] ([WorkFlowID])
GO

ALTER TABLE [dbo].[QA_CandidateRelations] CHECK CONSTRAINT [FK_QA_CandidateRelations_QA_WorkFlows]
GO


