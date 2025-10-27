USE [EKM_App]
GO

/****** Object:  Table [dbo].[WF_Services]    Script Date: 01/30/2013 12:05:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WF_Services](
	[WorkFlowID] [uniqueidentifier] NOT NULL,
	[NodeTypeID] [uniqueidentifier] NOT NULL,
	[FormID] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_WF_Services] PRIMARY KEY CLUSTERED 
(
	[WorkFlowID] ASC,
	[NodeTypeID] ASC,
	[FormID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[WF_Services]  WITH CHECK ADD  CONSTRAINT [FK_WF_Services_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WF_Services] CHECK CONSTRAINT [FK_WF_Services_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[WF_Services]  WITH CHECK ADD  CONSTRAINT [FK_WF_Services_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WF_Services] CHECK CONSTRAINT [FK_WF_Services_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[WF_Services]  WITH CHECK ADD  CONSTRAINT [FK_WF_Services_CN_NodeTypes_NodeTypeID] FOREIGN KEY([NodeTypeID])
REFERENCES [dbo].[CN_NodeTypes] ([NodeTypeID])
GO

ALTER TABLE [dbo].[WF_Services] CHECK CONSTRAINT [FK_WF_Services_CN_NodeTypes_NodeTypeID]
GO

ALTER TABLE [dbo].[WF_Services]  WITH CHECK ADD  CONSTRAINT [FK_WF_Services_FG_ExtendedForms_FormID] FOREIGN KEY([FormID])
REFERENCES [dbo].[FG_ExtendedForms] ([FormID])
GO

ALTER TABLE [dbo].[WF_Services] CHECK CONSTRAINT [FK_WF_Services_FG_ExtendedForms_FormID]
GO

ALTER TABLE [dbo].[WF_Services]  WITH CHECK ADD  CONSTRAINT [FK_WF_Services_WF_WorkFlows_WorkFlowID] FOREIGN KEY([WorkFlowID])
REFERENCES [dbo].[WF_WorkFlows] ([WorkFlowID])
GO

ALTER TABLE [dbo].[WF_Services] CHECK CONSTRAINT [FK_WF_Services_WF_WorkFlows_WorkFlowID]
GO



INSERT INTO [dbo].[WF_Services]
SELECT * FROM [dbo].[FG_ConceptForms]
GO


DROP TABLE [dbo].[FG_ConceptForms]
GO