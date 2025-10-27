USE [EKM_App]
GO



CREATE TABLE [dbo].[WF_StateConnectionAudience](
	[AudienceID] [uniqueidentifier] NOT NULL,
	[WorkFlowID] [uniqueidentifier] NOT NULL,
	[InStateID] [uniqueidentifier] NOT NULL,
	[OutStateID] [uniqueidentifier] NOT NULL,
	[BodyText] [nvarchar](4000) NOT NULL,
	[AudienceType] [varchar](20) NULL, -- SendToOwner, SpecificNode, RefState
	[RefStateID] [uniqueidentifier] NULL,
	[NodeID] [uniqueidentifier] NULL,
	[Admin] [bit] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
CONSTRAINT [PK_WF_StateConnectionAudience] PRIMARY KEY CLUSTERED 
(
	[AudienceID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[WF_StateConnectionAudience]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateConnectionAudience_WF_WorkFlows] FOREIGN KEY([WorkFlowID])
REFERENCES [dbo].[WF_WorkFlows] ([WorkFlowID])
GO

ALTER TABLE [dbo].[WF_StateConnectionAudience] CHECK CONSTRAINT [FK_WF_StateConnectionAudience_WF_WorkFlows]
GO

ALTER TABLE [dbo].[WF_StateConnectionAudience]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateConnectionAudience_WF_States_In] FOREIGN KEY([InStateID])
REFERENCES [dbo].[WF_States] ([StateID])
GO

ALTER TABLE [dbo].[WF_StateConnectionAudience] CHECK CONSTRAINT [FK_WF_StateConnectionAudience_WF_States_In]
GO

ALTER TABLE [dbo].[WF_StateConnectionAudience]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateConnectionAudience_WF_States_Out] FOREIGN KEY([OutStateID])
REFERENCES [dbo].[WF_States] ([StateID])
GO

ALTER TABLE [dbo].[WF_StateConnectionAudience] CHECK CONSTRAINT [FK_WF_StateConnectionAudience_WF_States_Out]
GO

ALTER TABLE [dbo].[WF_StateConnectionAudience]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateConnectionAudience_WF_States_Ref] FOREIGN KEY([RefStateID])
REFERENCES [dbo].[WF_States] ([StateID])
GO

ALTER TABLE [dbo].[WF_StateConnectionAudience] CHECK CONSTRAINT [FK_WF_StateConnectionAudience_WF_States_Ref]
GO

ALTER TABLE [dbo].[WF_StateConnectionAudience]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateConnectionAudience_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[WF_StateConnectionAudience] CHECK CONSTRAINT [FK_WF_StateConnectionAudience_CN_Nodes]
GO

ALTER TABLE [dbo].[WF_StateConnectionAudience]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateConnectionAudience_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WF_StateConnectionAudience] CHECK CONSTRAINT [FK_WF_StateConnectionAudience_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[WF_StateConnectionAudience]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateConnectionAudience_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WF_StateConnectionAudience] CHECK CONSTRAINT [FK_WF_StateConnectionAudience_aspnet_Users_Modifier]
GO

