USE [EKM_App]
GO

/****** Object:  Table [dbo].[KWF_Evaluators]    Script Date: 05/31/2012 21:32:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[KWF_Evaluators](
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[SenderUserID] [uniqueidentifier] NULL,
	[Evaluated] [bit] NOT NULL,
	[Score] [float] NULL,
	[EntranceDate] [datetime] NULL,
	[ExpirationDate] [datetime] NULL,
	[EvaluationDate] [datetime] NULL,
 CONSTRAINT [PK_KWF_Evaluators] PRIMARY KEY CLUSTERED 
(
	[KnowledgeID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[KWF_Evaluators]  WITH CHECK ADD  CONSTRAINT [FK_KWF_Evaluators_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KWF_Evaluators] CHECK CONSTRAINT [FK_KWF_Evaluators_aspnet_Users]
GO

ALTER TABLE [dbo].[KWF_Evaluators]  WITH CHECK ADD  CONSTRAINT [FK_KWF_Evaluators_KKnowledges] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[KKnowledges] ([ID])
GO

ALTER TABLE [dbo].[KWF_Evaluators] CHECK CONSTRAINT [FK_KWF_Evaluators_KKnowledges]
GO


