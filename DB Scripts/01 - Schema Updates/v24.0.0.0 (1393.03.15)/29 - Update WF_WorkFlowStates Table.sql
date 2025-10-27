USE [EKM_App]
GO

/****** Object:  Table [dbo].[WF_WorkFlowStates]    Script Date: 06/04/2014 12:52:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[WF_TMPWorkFlowStates](
	[ID] [uniqueidentifier] NOT NULL,
	[WorkFlowID] [uniqueidentifier] NOT NULL,
	[StateID] [uniqueidentifier] NOT NULL,
	[ResponseType] [varchar](20) NULL,
	[RefStateID] [uniqueidentifier] NULL,
	[NodeID] [uniqueidentifier] NULL,
	[Admin] [bit] NOT NULL,
	[Description] [nvarchar](2000) NULL,
	[DescriptionNeeded] [bit] NOT NULL,
	[HideOwnerName] [bit] NOT NULL,
	[EditPermission] [bit] NOT NULL,
	[DataNeedsType] [varchar](20) NULL,
	[RefDataNeedsStateID] [uniqueidentifier] NULL,
	[DataNeedsDescription] [nvarchar](2000) NULL,
	[FreeDataNeedRequests] [bit] NOT NULL,
	[TagID] [uniqueidentifier] NULL,
	[MaxAllowedRejections] [int] NULL,
	[RejectionTitle] [nvarchar](255) NULL,
	[RejectionRefStateID] [uniqueidentifier] NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_WF_TMPWorkFlowStates] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


INSERT INTO [dbo].[WF_TMPWorkFlowStates](
	[ID], [WorkFlowID], [StateID], [ResponseType], [RefStateID], [NodeID], [Admin],
	[Description], [DescriptionNeeded], [HideOwnerName], [EditPermission], [DataNeedsType],
	[RefDataNeedsStateID], [DataNeedsDescription], [FreeDataNeedRequests], [TagID],
	[MaxAllowedRejections], [RejectionTitle], [RejectionRefStateID], [CreatorUserID],
	[CreationDate], [LastModifierUserID], [LastModificationDate], [Deleted]
)
SELECT NEWID(), WorkFlowID, StateID, ResponseType, RefStateID, NodeID, [Admin],
	[Description], DescriptionNeeded, 
	CASE WHEN ShowOwnerName = 1 THEN 0 ELSE 1 END, 0, DataNeedsType,
	RefDataNeedsStateID, DataNeedsDescription, FreeDataNeedRequests, TagID,
	MaxAllowedRejections, RejectionTitle, RejectionRefStateID, CreatorUserID,
	CreationDate, LastModifierUserID, LastModificationDate, Deleted
FROM [dbo].[WF_WorkFlowStates]

GO


INSERT INTO [dbo].[FG_FormOwners](
	OwnerID, FormID, CreatorUserID, CreationDate, 
	LastModifierUserID, LastModificationDate, Deleted
)
SELECT  T.ID, WS.FormID, T.CreatorUserID, T.CreationDate,
		T.LastModifierUserID, T.LastModificationDate, 0
FROM [dbo].[WF_WorkFlowStates] AS WS
	INNER JOIN [dbo].[WF_TMPWorkFlowStates] AS T
	ON WS.WorkFlowID = T.WorkFlowID AND WS.StateID = T.StateID
WHERE WS.FormID IS NOT NULL

GO


DROP TABLE [dbo].[WF_WorkFlowStates]
GO


CREATE TABLE [dbo].[WF_WorkFlowStates](
	[ID] [uniqueidentifier] NOT NULL,
	[WorkFlowID] [uniqueidentifier] NOT NULL,
	[StateID] [uniqueidentifier] NOT NULL,
	[ResponseType] [varchar](20) NULL,
	[RefStateID] [uniqueidentifier] NULL,
	[NodeID] [uniqueidentifier] NULL,
	[Admin] [bit] NOT NULL,
	[Description] [nvarchar](2000) NULL,
	[DescriptionNeeded] [bit] NOT NULL,
	[HideOwnerName] [bit] NOT NULL,
	[EditPermission] [bit] NOT NULL,
	[DataNeedsType] [varchar](20) NULL,
	[RefDataNeedsStateID] [uniqueidentifier] NULL,
	[DataNeedsDescription] [nvarchar](2000) NULL,
	[FreeDataNeedRequests] [bit] NOT NULL,
	[TagID] [uniqueidentifier] NULL,
	[MaxAllowedRejections] [int] NULL,
	[RejectionTitle] [nvarchar](255) NULL,
	[RejectionRefStateID] [uniqueidentifier] NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_WF_WorkFlowStates] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[WF_WorkFlowStates] ADD  CONSTRAINT [UK_WF_WorkFlowStates] UNIQUE NONCLUSTERED 
(
	[WorkFlowID] ASC,
	[StateID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
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

ALTER TABLE [dbo].[WF_WorkFlowStates]  WITH CHECK ADD  CONSTRAINT [FK_WF_WorkFlowStates_WF_States_Rejection] FOREIGN KEY([RejectionRefStateID])
REFERENCES [dbo].[WF_States] ([StateID])
GO

ALTER TABLE [dbo].[WF_WorkFlowStates] CHECK CONSTRAINT [FK_WF_WorkFlowStates_WF_States_Rejection]
GO

ALTER TABLE [dbo].[WF_WorkFlowStates]  WITH CHECK ADD  CONSTRAINT [FK_WF_WorkFlowStates_WF_WorkFlows_WorkFlow] FOREIGN KEY([WorkFlowID])
REFERENCES [dbo].[WF_WorkFlows] ([WorkFlowID])
GO

ALTER TABLE [dbo].[WF_WorkFlowStates] CHECK CONSTRAINT [FK_WF_WorkFlowStates_WF_WorkFlows_WorkFlow]
GO


INSERT INTO [dbo].[WF_WorkFlowStates]
SELECT *
FROM [dbo].[WF_TMPWorkFlowStates]
GO


DROP TABLE [dbo].[WF_TMPWorkFlowStates]
GO