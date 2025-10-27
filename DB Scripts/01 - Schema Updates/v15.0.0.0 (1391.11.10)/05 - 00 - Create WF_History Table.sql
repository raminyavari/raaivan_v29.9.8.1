USE [EKM_App]
GO

/****** Object:  Table [dbo].[WF_History]    Script Date: 01/29/2013 09:56:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE [dbo].[WF_History]
GO

CREATE TABLE [dbo].[WF_History](
	[HistoryID] [uniqueidentifier] NOT NULL,
	[OwnerID] [uniqueidentifier] NOT NULL,
	[WorkFlowID] [uniqueidentifier] NOT NULL,
	[StateID] [uniqueidentifier] NOT NULL,
	[DirectorNodeID] [uniqueidentifier] NULL,
	[DirectorUserID] [uniqueidentifier] NULL,
	[Description] [nvarchar](2000) NULL,
	[SenderUserID] [uniqueidentifier] NULL,
	[SendDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
	[SelectedOutStateID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_WF_History] PRIMARY KEY CLUSTERED 
(
	[HistoryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[WF_History]  WITH CHECK ADD  CONSTRAINT [FK_WF_History_WF_WorkFlows] FOREIGN KEY([WorkFlowID])
REFERENCES [dbo].[WF_WorkFlows] ([WorkFlowID])
GO

ALTER TABLE [dbo].[WF_History] CHECK CONSTRAINT [FK_WF_History_WF_WorkFlows]
GO

ALTER TABLE [dbo].[WF_History]  WITH CHECK ADD  CONSTRAINT [FK_WF_History_WF_States] FOREIGN KEY([StateID])
REFERENCES [dbo].[WF_States] ([StateID])
GO

ALTER TABLE [dbo].[WF_History] CHECK CONSTRAINT [FK_WF_History_WF_States]
GO


ALTER TABLE [dbo].[WF_History]  WITH CHECK ADD  CONSTRAINT [FK_WF_History_aspnet_Users_Director] FOREIGN KEY([DirectorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WF_History] CHECK CONSTRAINT [FK_WF_History_aspnet_Users_Director]
GO

ALTER TABLE [dbo].[WF_History]  WITH CHECK ADD  CONSTRAINT [FK_WF_History_aspnet_Users_Sender] FOREIGN KEY([SenderUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WF_History] CHECK CONSTRAINT [FK_WF_History_aspnet_Users_Sender]
GO

ALTER TABLE [dbo].[WF_History]  WITH CHECK ADD  CONSTRAINT [FK_WF_History_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WF_History] CHECK CONSTRAINT [FK_WF_History_aspnet_Users_Modifier]
GO


ALTER TABLE [dbo].[WF_History]  WITH CHECK ADD  CONSTRAINT [FK_WF_History_CN_Nodes] FOREIGN KEY([DirectorNodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[WF_History] CHECK CONSTRAINT [FK_WF_History_CN_Nodes]
GO