USE [EKM_App]
GO


CREATE TABLE [dbo].[WF_StateDataNeeds](
	[WorkFlowID] [uniqueidentifier] NOT NULL,
	[StateID] [uniqueidentifier] NOT NULL,
	[NodeTypeID] [uniqueidentifier] NOT NULL,
	[Description] [nvarchar] (2000) NULL,
	[MultipleSelect] [bit] NOT NULL,
	[Admin] [bit] NOT NULL,
	[Necessary] [bit] NOT NULL,
	[FormID] [uniqueidentifier] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
CONSTRAINT [PK_WF_StateDataNeeds] PRIMARY KEY CLUSTERED 
(
	[WorkFlowID] ASC,
	[StateID] ASC,
	[NodeTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[WF_StateDataNeeds]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateDataNeeds_WF_WorkFlows] FOREIGN KEY([WorkFlowID])
REFERENCES [dbo].[WF_WorkFlows] ([WorkFlowID])
GO

ALTER TABLE [dbo].[WF_StateDataNeeds] CHECK CONSTRAINT [FK_WF_StateDataNeeds_WF_WorkFlows]
GO

ALTER TABLE [dbo].[WF_StateDataNeeds]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateDataNeeds_WF_States] FOREIGN KEY([StateID])
REFERENCES [dbo].[WF_States] ([StateID])
GO

ALTER TABLE [dbo].[WF_StateDataNeeds] CHECK CONSTRAINT [FK_WF_StateDataNeeds_WF_States]
GO

ALTER TABLE [dbo].[WF_StateDataNeeds]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateDataNeeds_CN_NodeTypes] FOREIGN KEY([NodeTypeID])
REFERENCES [dbo].[CN_NodeTypes] ([NodeTypeID])
GO

ALTER TABLE [dbo].[WF_StateDataNeeds] CHECK CONSTRAINT [FK_WF_StateDataNeeds_CN_NodeTypes]
GO

ALTER TABLE [dbo].[WF_StateDataNeeds]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateDataNeeds_FG_ExtendedForms] FOREIGN KEY([FormID])
REFERENCES [dbo].[FG_ExtendedForms] ([FormID])
GO

ALTER TABLE [dbo].[WF_StateDataNeeds] CHECK CONSTRAINT [FK_WF_StateDataNeeds_FG_ExtendedForms]
GO

ALTER TABLE [dbo].[WF_StateDataNeeds]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateDataNeeds_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WF_StateDataNeeds] CHECK CONSTRAINT [FK_WF_StateDataNeeds_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[WF_StateDataNeeds]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateDataNeeds_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WF_StateDataNeeds] CHECK CONSTRAINT [FK_WF_StateDataNeeds_aspnet_Users_Modifier]
GO