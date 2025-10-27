USE [EKM_APP]
GO

/****** Object:  Table [dbo].[WF_WorkFlows]    Script Date: 11/04/2012 14:47:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TABLE [dbo].[WF_WorkFlows](
	[WorkFlowID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2000) NULL,
	
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_WF_WorkFlows] PRIMARY KEY CLUSTERED 
(
	[WorkFlowID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[WF_WorkFlows]  WITH CHECK ADD  CONSTRAINT [FK_WF_WorkFlows_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO
ALTER TABLE [dbo].[WF_WorkFlows] CHECK CONSTRAINT [FK_WF_WorkFlows_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[WF_WorkFlows]  WITH CHECK ADD  CONSTRAINT [FK_WF_WorkFlows_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO
ALTER TABLE [dbo].[WF_WorkFlows] CHECK CONSTRAINT [FK_WF_WorkFlows_aspnet_Users_Modifier]
GO


