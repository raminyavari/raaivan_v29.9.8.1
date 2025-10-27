USE [EKM_App]
GO

/****** Object:  Table [dbo].[KWF_Experts]    Script Date: 02/17/2012 14:46:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[KWF_ExpertsTemp](
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NOT NULL,
	[SenderUserID] [uniqueidentifier] NULL,
	[Evaluated] [bit] NOT NULL,
	[Score] [float] NULL,
	[EntranceDate] [datetime] NULL,
	[ExpirationDate] [datetime] NULL,
	[EvaluationDate] [datetime] NULL,
 CONSTRAINT [PK_KWF_ExpertsTemp] PRIMARY KEY CLUSTERED 
(
	[KnowledgeID] ASC,
	[UserID] ASC,
	[NodeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


INSERT INTO [dbo].[KWF_ExpertsTemp]
           ([KnowledgeID]
			,[UserID]
			,[NodeID]
			,[SenderUserID]
			,[Evaluated]
			,[Score]
			,[EntranceDate]
			,[ExpirationDate]
			,[EvaluationDate])
		SELECT  dbo.KWF_Experts.KnowledgeID, dbo.KWF_Experts.UserID, dbo.CN_Nodes.NodeID, dbo.KWF_Experts.SenderUserID,
				dbo.KWF_Experts.Evaluated, dbo.KWF_Experts.Score, dbo.KWF_Experts.EntranceDate, 
				dbo.KWF_Experts.ExpirationDate, dbo.KWF_Experts.EvaluationDate
		FROM    dbo.CN_Nodes INNER JOIN dbo.KWF_Experts ON dbo.CN_Nodes.ID = dbo.KWF_Experts.NodeID
GO


/* Drop old table */
DROP TABLE [dbo].[KWF_Experts]
GO

/* Create old new table */
CREATE TABLE [dbo].[KWF_Experts](
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NOT NULL,
	[SenderUserID] [uniqueidentifier] NULL,
	[Evaluated] [bit] NOT NULL,
	[Score] [float] NULL,
	[EntranceDate] [datetime] NULL,
	[ExpirationDate] [datetime] NULL,
	[EvaluationDate] [datetime] NULL,
 CONSTRAINT [PK_KWF_Experts] PRIMARY KEY CLUSTERED 
(
	[KnowledgeID] ASC,
	[UserID] ASC,
	[NodeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[KWF_Experts]  WITH CHECK ADD  CONSTRAINT [FK_KWF_Experts_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KWF_Experts] CHECK CONSTRAINT [FK_KWF_Experts_aspnet_Users]
GO

ALTER TABLE [dbo].[KWF_Experts]  WITH CHECK ADD  CONSTRAINT [FK_KWF_Experts_aspnet_Users_Sender] FOREIGN KEY([SenderUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KWF_Experts] CHECK CONSTRAINT [FK_KWF_Experts_aspnet_Users_Sender]
GO

ALTER TABLE [dbo].[KWF_Experts]  WITH CHECK ADD  CONSTRAINT [FK_KWF_Experts_KKnowledges] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[KKnowledges] ([ID])
GO

ALTER TABLE [dbo].[KWF_Experts] CHECK CONSTRAINT [FK_KWF_Experts_KKnowledges]
GO

ALTER TABLE [dbo].[KWF_Experts]  WITH CHECK ADD  CONSTRAINT [FK_KWF_Experts_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[KWF_Experts] CHECK CONSTRAINT [FK_KWF_Experts_CN_Nodes]
GO


INSERT INTO [dbo].[KWF_Experts]
	SELECT * FROM dbo.KWF_ExpertsTemp
GO

DROP TABLE [dbo].[KWF_ExpertsTemp]
GO