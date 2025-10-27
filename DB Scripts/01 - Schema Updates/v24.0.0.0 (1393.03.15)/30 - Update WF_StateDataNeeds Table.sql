USE [EKM_App]
GO

/****** Object:  Table [dbo].[WF_WorkFlowStates]    Script Date: 06/04/2014 12:52:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[WF_TMPStateDataNeeds](
	[ID] [uniqueidentifier] NOT NULL,
	[WorkFlowID] [uniqueidentifier] NOT NULL,
	[StateID] [uniqueidentifier] NOT NULL,
	[NodeTypeID] [uniqueidentifier] NOT NULL,
	[Description] [nvarchar](2000) NULL,
	[MultipleSelect] [bit] NOT NULL,
	[Admin] [bit] NOT NULL,
	[Necessary] [bit] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_WF_TMPStateDataNeeds] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


INSERT INTO [dbo].[WF_TMPStateDataNeeds](
	[ID], [WorkFlowID], [StateID], [NodeTypeID], [Description], [MultipleSelect],
	[Admin], [Necessary], [CreatorUserID], [CreationDate], [LastModifierUserID],
	[LastModificationDate], [Deleted]
)
SELECT NEWID(), WorkFlowID, StateID, NodeTypeID, [Description], [MultipleSelect], 
	[Admin], Necessary, CreatorUserID, CreationDate, LastModifierUserID, 
	LastModificationDate, Deleted
FROM [dbo].[WF_StateDataNeeds]

GO


INSERT INTO [dbo].[FG_FormOwners](
	OwnerID, FormID, CreatorUserID, CreationDate, 
	LastModifierUserID, LastModificationDate, Deleted
)
SELECT  T.ID, SD.FormID, T.CreatorUserID, T.CreationDate,
		T.LastModifierUserID, T.LastModificationDate, 0
FROM [dbo].[WF_StateDataNeeds] AS SD
	INNER JOIN [dbo].[WF_TMPStateDataNeeds] AS T
	ON SD.WorkFlowID = T.WorkFlowID AND SD.StateID = T.StateID AND SD.NodeTypeID = T.NodeTypeID
WHERE SD.FormID IS NOT NULL

GO


DROP TABLE [dbo].[WF_StateDataNeeds]
GO


CREATE TABLE [dbo].[WF_StateDataNeeds](
	[ID] [uniqueidentifier] NOT NULL,
	[WorkFlowID] [uniqueidentifier] NOT NULL,
	[StateID] [uniqueidentifier] NOT NULL,
	[NodeTypeID] [uniqueidentifier] NOT NULL,
	[Description] [nvarchar](2000) NULL,
	[MultipleSelect] [bit] NOT NULL,
	[Admin] [bit] NOT NULL,
	[Necessary] [bit] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_WF_StateDataNeeds] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[WF_StateDataNeeds] ADD  CONSTRAINT [UK_WF_StateDataNeeds] UNIQUE NONCLUSTERED 
(
	[WorkFlowID] ASC,
	[StateID] ASC,
	[NodeTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
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

ALTER TABLE [dbo].[WF_StateDataNeeds]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateDataNeeds_CN_NodeTypes] FOREIGN KEY([NodeTypeID])
REFERENCES [dbo].[CN_NodeTypes] ([NodeTypeID])
GO

ALTER TABLE [dbo].[WF_StateDataNeeds] CHECK CONSTRAINT [FK_WF_StateDataNeeds_CN_NodeTypes]
GO

ALTER TABLE [dbo].[WF_StateDataNeeds]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateDataNeeds_WF_States] FOREIGN KEY([StateID])
REFERENCES [dbo].[WF_States] ([StateID])
GO

ALTER TABLE [dbo].[WF_StateDataNeeds] CHECK CONSTRAINT [FK_WF_StateDataNeeds_WF_States]
GO

ALTER TABLE [dbo].[WF_StateDataNeeds]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateDataNeeds_WF_WorkFlows] FOREIGN KEY([WorkFlowID])
REFERENCES [dbo].[WF_WorkFlows] ([WorkFlowID])
GO

ALTER TABLE [dbo].[WF_StateDataNeeds] CHECK CONSTRAINT [FK_WF_StateDataNeeds_WF_WorkFlows]
GO


INSERT INTO [dbo].[WF_StateDataNeeds]
SELECT *
FROM [dbo].[WF_TMPStateDataNeeds]
GO


DROP TABLE [dbo].[WF_TMPStateDataNeeds]
GO