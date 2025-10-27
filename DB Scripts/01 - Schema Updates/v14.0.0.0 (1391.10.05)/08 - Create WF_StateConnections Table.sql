USE [EKM_APP]
GO

/****** Object:  Table [dbo].[WF_StateConnections]    Script Date: 11/04/2012 14:47:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WF_StateConnections](
	[WorkFlowID] [uniqueidentifier] NOT NULL,
	[InStateID] [uniqueidentifier] NOT NULL,
	[OutStateID] [uniqueidentifier] NOT NULL,
	[Label] [nvarchar](255) NOT NULL,
	[AttachmentRequired] [bit] NOT NULL,
	[AttachmentTitle] [nvarchar](255) NULL,
	[NodeRequired] [bit] NOT NULL, /*Role Required*/
	[NodeTypeID] [uniqueidentifier] NULL,
	
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_WF_StateConnection] PRIMARY KEY CLUSTERED 
(
	[WorkFlowID] ASC,
	[InStateID] ASC,
	[OutStateID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[WF_StateConnections]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateConnections_WF_WorkFlows] FOREIGN KEY([WorkFlowID])
REFERENCES [dbo].[WF_WorkFlows] ([WorkFlowID])
GO
ALTER TABLE [dbo].[WF_StateConnections] CHECK CONSTRAINT [FK_WF_StateConnections_WF_WorkFlows]
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

--ALTER TABLE [dbo].[WF_StateConnections]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateConnections_CN_NodeTypes] FOREIGN KEY([NodeTypeID])
--REFERENCES [dbo].[CN_NodeTypes] ([NodeTypeID])
--GO
--ALTER TABLE [dbo].[WF_StateConnections] CHECK CONSTRAINT [FK_WF_StateConnections_CN_NodeTypes]
--GO


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


