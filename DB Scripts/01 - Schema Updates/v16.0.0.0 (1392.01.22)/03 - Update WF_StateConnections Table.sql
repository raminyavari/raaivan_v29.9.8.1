USE [EKM_App]
GO

/****** Object:  Table [dbo].[WF_StateConnections]    Script Date: 04/09/2013 10:33:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE TABLE [dbo].[WF_TMPStateConnections](
	[WorkFlowID] [uniqueidentifier] NOT NULL,
	[InStateID] [uniqueidentifier] NOT NULL,
	[OutStateID] [uniqueidentifier] NOT NULL,
	[SequenceNumber] [int] NOT NULL,
	[Label] [nvarchar](255) NOT NULL,
	[AttachmentRequired] [bit] NOT NULL,
	[AttachmentTitle] [nvarchar](255) NULL,
	[NodeRequired] [bit] NOT NULL,
	[NodeTypeID] [uniqueidentifier] NULL,
	[NodeTypeDescription] [nvarchar](2000) NULL,
	[PattAttachmentID] [uniqueidentifier] NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_WF_TMPStateConnection] PRIMARY KEY CLUSTERED 
(
	[WorkFlowID] ASC,
	[InStateID] ASC,
	[OutStateID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[WF_TMPStateConnections](
	WorkFlowID,
	InStateID,
	OutStateID,
	SequenceNumber,
	Label,
	AttachmentRequired,
	AttachmentTitle,
	NodeRequired,
	NodeTypeID,
	NodeTypeDescription,
	PattAttachmentID,
	CreatorUserID,
	CreationDate,
	LastModifierUserID,
	LastModificationDate,
	Deleted
)
SELECT WorkFlowID, InStateID, OutStateID, 1, Label, AttachmentRequired, AttachmentTitle,
	NodeRequired, NodeTypeID, NodeTypeDescription, PattAttachmentID, CreatorUserID,
	CreationDate, LastModifierUserID, LastModificationDate, Deleted
FROM [dbo].[WF_StateConnections]
GO


DROP TABLE [dbo].[WF_StateConnections]
GO


CREATE TABLE [dbo].[WF_StateConnections](
	[WorkFlowID] [uniqueidentifier] NOT NULL,
	[InStateID] [uniqueidentifier] NOT NULL,
	[OutStateID] [uniqueidentifier] NOT NULL,
	[SequenceNumber] [int] NOT NULL,
	[Label] [nvarchar](255) NOT NULL,
	[AttachmentRequired] [bit] NOT NULL,
	[AttachmentTitle] [nvarchar](255) NULL,
	[NodeRequired] [bit] NOT NULL,
	[NodeTypeID] [uniqueidentifier] NULL,
	[NodeTypeDescription] [nvarchar](2000) NULL,
	[PattAttachmentID] [uniqueidentifier] NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_WF_StateConnections] PRIMARY KEY CLUSTERED 
(
	[WorkFlowID] ASC,
	[InStateID] ASC,
	[OutStateID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[WF_StateConnections]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateConnections_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WF_StateConnections] CHECK CONSTRAINT [FK_WF_StateConnections_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[WF_StateConnections]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateConnections_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WF_StateConnections] CHECK CONSTRAINT [FK_WF_StateConnections_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[WF_StateConnections]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateConnections_CN_NodeTypes] FOREIGN KEY([NodeTypeID])
REFERENCES [dbo].[CN_NodeTypes] ([NodeTypeID])
GO

ALTER TABLE [dbo].[WF_StateConnections] CHECK CONSTRAINT [FK_WF_StateConnections_CN_NodeTypes]
GO

ALTER TABLE [dbo].[WF_StateConnections]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateConnections_WF_States_In] FOREIGN KEY([InStateID])
REFERENCES [dbo].[WF_States] ([StateID])
GO

ALTER TABLE [dbo].[WF_StateConnections] CHECK CONSTRAINT [FK_WF_StateConnections_WF_States_In]
GO

ALTER TABLE [dbo].[WF_StateConnections]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateConnections_WF_States_Out] FOREIGN KEY([OutStateID])
REFERENCES [dbo].[WF_States] ([StateID])
GO

ALTER TABLE [dbo].[WF_StateConnections] CHECK CONSTRAINT [FK_WF_StateConnections_WF_States_Out]
GO

ALTER TABLE [dbo].[WF_StateConnections]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateConnections_WF_WorkFlows] FOREIGN KEY([WorkFlowID])
REFERENCES [dbo].[WF_WorkFlows] ([WorkFlowID])
GO

ALTER TABLE [dbo].[WF_StateConnections] CHECK CONSTRAINT [FK_WF_StateConnections_WF_WorkFlows]
GO


INSERT INTO [dbo].[WF_StateConnections]
SELECT * FROM [dbo].[WF_TMPStateConnections]
GO

DROP TABLE [dbo].[WF_TMPStateConnections]
GO



DECLARE @TIDs Table(WFID uniqueidentifier, INSTID uniqueidentifier,
	OTSTID uniqueidentifier, SN int IDENTITY(1,1))
	
INSERT INTO @TIDs(WFID, INSTID, OTSTID)
SELECT WorkFlowID, InStateID, OutStateID
FROM [dbo].[WF_StateConnections]

UPDATE [dbo].[WF_StateConnections]
	SET SequenceNumber = IDS.SN
FROM @TIDs AS IDS
	INNER JOIN [dbo].[WF_StateConnections]
	ON IDS.WFID = [dbo].[WF_StateConnections].[WorkFlowID] AND 
		IDS.INSTID = [dbo].[WF_StateConnections].[InStateID] AND 
		IDS.OTSTID = [dbo].[WF_StateConnections].[OutStateID]

GO