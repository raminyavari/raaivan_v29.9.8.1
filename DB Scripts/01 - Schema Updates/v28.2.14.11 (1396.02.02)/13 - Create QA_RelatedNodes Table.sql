USE [EKM_App]
GO

/****** Object:  Table [dbo].[QA_RelatedNodes]    Script Date: 11/29/2016 09:33:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[QA_RelatedNodes](
	[NodeID] [uniqueidentifier] NOT NULL,
	[QuestionID] [uniqueidentifier] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
	[ApplicationID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QA_RelatedNodes] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC,
	[QuestionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[QA_RelatedNodes]  WITH CHECK ADD  CONSTRAINT [FK_QA_RelatedNodes_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[QA_RelatedNodes] CHECK CONSTRAINT [FK_QA_RelatedNodes_aspnet_Applications]
GO

ALTER TABLE [dbo].[QA_RelatedNodes]  WITH CHECK ADD  CONSTRAINT [FK_QA_RelatedNodes_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[QA_RelatedNodes] CHECK CONSTRAINT [FK_QA_RelatedNodes_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[QA_RelatedNodes]  WITH CHECK ADD  CONSTRAINT [FK_QA_RelatedNodes_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[QA_RelatedNodes] CHECK CONSTRAINT [FK_QA_RelatedNodes_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[QA_RelatedNodes]  WITH CHECK ADD  CONSTRAINT [FK_QA_RelatedNodes_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[QA_RelatedNodes] CHECK CONSTRAINT [FK_QA_RelatedNodes_CN_Nodes]
GO

ALTER TABLE [dbo].[QA_RelatedNodes]  WITH CHECK ADD  CONSTRAINT [FK_QA_RelatedNodes_QA_Questions] FOREIGN KEY([QuestionID])
REFERENCES [dbo].[QA_Questions] ([QuestionID])
GO

ALTER TABLE [dbo].[QA_RelatedNodes] CHECK CONSTRAINT [FK_QA_RelatedNodes_QA_Questions]
GO