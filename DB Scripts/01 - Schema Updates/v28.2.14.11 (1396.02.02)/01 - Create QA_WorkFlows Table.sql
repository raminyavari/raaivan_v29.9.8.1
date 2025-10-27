USE [EKM_App]
GO


/****** Object:  Table [dbo].[QA_WorkFlows]    Script Date: 11/29/2016 08:54:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[QA_WorkFlows](
	[WorkFlowID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Description] [nvarchar](2000) NULL,
	[SequenceNumber] [int] NOT NULL,
	[InitialCheckNeeded] [bit] NOT NULL,
	[FinalConfirmationNeeded] [bit] NOT NULL,
	[ActionDeadline] [int] NULL,
	[AnswerBy] [varchar](50) NULL,
	[PublishAfter] [varchar](50) NULL,
	[RemovableAfterConfirmation] [bit] NOT NULL,
	[NodeSelectType] [varchar](50) NULL,
	[DisableComments] [bit] NOT NULL,
	[DisableQuestionLikes] [bit] NOT NULL,
	[DisableAnswerLikes] [bit] NOT NULL,
	[DisableCommentLikes] [bit] NOT NULL,
	[DisableBestAnswer] [bit] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
	[ApplicationID] [uniqueidentifier] NULL
 CONSTRAINT [PK_QA_WorkFlows] PRIMARY KEY CLUSTERED 
(
	[WorkFlowID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


ALTER TABLE [dbo].[QA_WorkFlows]  WITH CHECK ADD  CONSTRAINT [FK_QA_WorkFlows_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[QA_WorkFlows] CHECK CONSTRAINT [FK_QA_WorkFlows_aspnet_Applications]
GO

ALTER TABLE [dbo].[QA_WorkFlows]  WITH CHECK ADD  CONSTRAINT [FK_QA_WorkFlows_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[QA_WorkFlows] CHECK CONSTRAINT [FK_QA_WorkFlows_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[QA_WorkFlows]  WITH CHECK ADD  CONSTRAINT [FK_QA_WorkFlows_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[QA_WorkFlows] CHECK CONSTRAINT [FK_QA_WorkFlows_aspnet_Users_Modifier]
GO


