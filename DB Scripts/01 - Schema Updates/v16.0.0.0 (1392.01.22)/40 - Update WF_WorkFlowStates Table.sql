USE [EKM_App]
GO

/****** Object:  Table [dbo].[WF_WorkFlowStates]    Script Date: 06/17/2013 10:19:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[WF_TMPWorkFlowStates](
	[WorkFlowID] [uniqueidentifier] NOT NULL,
	[StateID] [uniqueidentifier] NOT NULL,
	[ResponseType] [varchar](20) NULL,
	[RefStateID] [uniqueidentifier] NULL,
	[NodeID] [uniqueidentifier] NULL,
	[Admin] [bit] NOT NULL,
	[Description] [nvarchar](2000) NULL,
	[DescriptionNeeded] [bit] NOT NULL,
	[ShowOwnerName] [bit] NOT NULL,
	[DataNeedsType] [varchar](20) NULL,
	[RefDataNeedsStateID] [uniqueidentifier] NULL,
	[FormID] [uniqueidentifier] NULL,
	[DataNeedsDescription] [nvarchar](2000) NULL,
	[FreeDataNeedRequests] [bit] NOT NULL,
	[TagID] [uniqueidentifier] NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_WF_TMPWorkFlowStates] PRIMARY KEY CLUSTERED 
(
	[WorkFlowID] ASC,
	[StateID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


INSERT INTO [dbo].[WF_TMPWorkFlowStates](
	WorkFlowID,
	StateID,
	ResponseType,
	RefStateID,
	NodeID,
	[Admin],
	[Description],
	DescriptionNeeded,
	ShowOwnerName,
	DataNeedsType,
	RefDataNeedsStateID,
	FormID,
	DataNeedsDescription,
	FreeDataNeedRequests,
	TagID,
	CreatorUserID ,
	CreationDate,
	LastModifierUserID,
	LastModificationDate,
	Deleted
)
SELECT WorkFlowID, StateID, ResponseType, RefStateID, NodeID, IsAdmin, [Description],
	   DescriptionNeeded, ShowOwnerName, DataNeedsType, RefDataNeedsStateID, FormID,
	   DataNeedsDescription, 0, TagID, CreatorUserID, CreationDate, LastModifierUserID,
	   LastModificationDate, Deleted
FROM [dbo].[WF_WorkFlowStates]

GO


DROP TABLE [dbo].[WF_WorkFlowStates]
GO


SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[WF_WorkFlowStates](
	[WorkFlowID] [uniqueidentifier] NOT NULL,
	[StateID] [uniqueidentifier] NOT NULL,
	[ResponseType] [varchar](20) NULL,
	[RefStateID] [uniqueidentifier] NULL,
	[NodeID] [uniqueidentifier] NULL,
	[Admin] [bit] NOT NULL,
	[Description] [nvarchar](2000) NULL,
	[DescriptionNeeded] [bit] NOT NULL,
	[ShowOwnerName] [bit] NOT NULL,
	[DataNeedsType] [varchar](20) NULL,
	[RefDataNeedsStateID] [uniqueidentifier] NULL,
	[FormID] [uniqueidentifier] NULL,
	[DataNeedsDescription] [nvarchar](2000) NULL,
	[FreeDataNeedRequests] [bit] NOT NULL,
	[TagID] [uniqueidentifier] NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_WF_WorkFlowStates] PRIMARY KEY CLUSTERED 
(
	[WorkFlowID] ASC,
	[StateID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[WF_WorkFlowStates]  WITH CHECK ADD  CONSTRAINT [FK_WF_WorkFlowStates_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WF_WorkFlowStates] CHECK CONSTRAINT [FK_WF_WorkFlowStates_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[WF_WorkFlowStates]  WITH CHECK ADD  CONSTRAINT [FK_WF_WorkFlowStates_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WF_WorkFlowStates] CHECK CONSTRAINT [FK_WF_WorkFlowStates_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[WF_WorkFlowStates]  WITH CHECK ADD  CONSTRAINT [FK_WF_WorkFlowStates_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[WF_WorkFlowStates] CHECK CONSTRAINT [FK_WF_WorkFlowStates_CN_Nodes]
GO

ALTER TABLE [dbo].[WF_WorkFlowStates]  WITH CHECK ADD  CONSTRAINT [FK_WF_WorkFlowStates_CN_Tags] FOREIGN KEY([TagID])
REFERENCES [dbo].[CN_Tags] ([TagID])
GO

ALTER TABLE [dbo].[WF_WorkFlowStates] CHECK CONSTRAINT [FK_WF_WorkFlowStates_CN_Tags]
GO

ALTER TABLE [dbo].[WF_WorkFlowStates]  WITH CHECK ADD  CONSTRAINT [FK_WF_WorkFlowStates_FG_ExtendedForms] FOREIGN KEY([FormID])
REFERENCES [dbo].[FG_ExtendedForms] ([FormID])
GO

ALTER TABLE [dbo].[WF_WorkFlowStates] CHECK CONSTRAINT [FK_WF_WorkFlowStates_FG_ExtendedForms]
GO

ALTER TABLE [dbo].[WF_WorkFlowStates]  WITH CHECK ADD  CONSTRAINT [FK_WF_WorkFlowStates_WF_States] FOREIGN KEY([StateID])
REFERENCES [dbo].[WF_States] ([StateID])
GO

ALTER TABLE [dbo].[WF_WorkFlowStates] CHECK CONSTRAINT [FK_WF_WorkFlowStates_WF_States]
GO

ALTER TABLE [dbo].[WF_WorkFlowStates]  WITH CHECK ADD  CONSTRAINT [FK_WF_WorkFlowStates_WF_States_DataNeeds] FOREIGN KEY([RefDataNeedsStateID])
REFERENCES [dbo].[WF_States] ([StateID])
GO

ALTER TABLE [dbo].[WF_WorkFlowStates] CHECK CONSTRAINT [FK_WF_WorkFlowStates_WF_States_DataNeeds]
GO

ALTER TABLE [dbo].[WF_WorkFlowStates]  WITH CHECK ADD  CONSTRAINT [FK_WF_WorkFlowStates_WF_States_Ref] FOREIGN KEY([RefStateID])
REFERENCES [dbo].[WF_States] ([StateID])
GO

ALTER TABLE [dbo].[WF_WorkFlowStates] CHECK CONSTRAINT [FK_WF_WorkFlowStates_WF_States_Ref]
GO

ALTER TABLE [dbo].[WF_WorkFlowStates]  WITH CHECK ADD  CONSTRAINT [FK_WF_WorkFlowStates_WF_WorkFlows_WorkFlow] FOREIGN KEY([WorkFlowID])
REFERENCES [dbo].[WF_WorkFlows] ([WorkFlowID])
GO

ALTER TABLE [dbo].[WF_WorkFlowStates] CHECK CONSTRAINT [FK_WF_WorkFlowStates_WF_WorkFlows_WorkFlow]
GO


INSERT INTO [dbo].[WF_WorkFlowStates]
SELECT * FROM [dbo].[WF_TMPWorkFlowStates]
GO


DROP TABLE [dbo].[WF_TMPWorkFlowStates]
GO