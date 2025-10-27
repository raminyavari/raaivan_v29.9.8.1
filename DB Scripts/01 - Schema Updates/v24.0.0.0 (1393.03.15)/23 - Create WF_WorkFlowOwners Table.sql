USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WF_WorkFlowOwners](
	[ID] [uniqueidentifier] NOT NULL,
	[NodeTypeID] [uniqueidentifier] NOT NULL,
	[WorkFlowID] [uniqueidentifier] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_WF_WorkFlowOwners] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[WF_WorkFlowOwners] ADD  CONSTRAINT [UK_WF_WorkFlowOwners_NodeTypeID_WorkFlowID] UNIQUE NONCLUSTERED 
(
	[NodeTypeID] ASC,
	[WorkFlowID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


ALTER TABLE [dbo].[WF_WorkFlowOwners]  WITH CHECK ADD  CONSTRAINT [FK_WF_WorkFlowOwners_CN_NodeTypes_NodeTypeID] FOREIGN KEY([NodeTypeID])
REFERENCES [dbo].[CN_NodeTypes] ([NodeTypeID])
GO

ALTER TABLE [dbo].[WF_WorkFlowOwners] CHECK CONSTRAINT [FK_WF_WorkFlowOwners_CN_NodeTypes_NodeTypeID]
GO

ALTER TABLE [dbo].[WF_WorkFlowOwners]  WITH CHECK ADD  CONSTRAINT [FK_WF_WorkFlowOwners_WF_WorkFlows_WorkFlowID] FOREIGN KEY([WorkFlowID])
REFERENCES [dbo].[WF_WorkFlows] ([WorkFlowID])
GO

ALTER TABLE [dbo].[WF_WorkFlowOwners] CHECK CONSTRAINT [FK_WF_WorkFlowOwners_WF_WorkFlows_WorkFlowID]
GO

ALTER TABLE [dbo].[WF_WorkFlowOwners]  WITH CHECK ADD  CONSTRAINT [FK_WF_WorkFlowOwners_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WF_WorkFlowOwners] CHECK CONSTRAINT [FK_WF_WorkFlowOwners_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[WF_WorkFlowOwners]  WITH CHECK ADD  CONSTRAINT [FK_WF_WorkFlowOwners_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WF_WorkFlowOwners] CHECK CONSTRAINT [FK_WF_WorkFlowOwners_aspnet_Users_Modifier]
GO