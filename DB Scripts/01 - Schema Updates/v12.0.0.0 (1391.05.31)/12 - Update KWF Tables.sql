USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- Update KWF_Managers
CREATE TABLE [dbo].[TMP_KWF_Managers](
	[ID] [bigint] NOT NULL IDENTITY(1,1),
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[EntranceDate] [datetime] NULL,
	[EvaluationDate] [datetime] NULL,
	[Sent] [bit] NOT NULL,
	[HaveRejectedEvaluators] [bit] NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_TMP_KWF_Managers] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[TMP_KWF_Managers](
	KnowledgeID,
	UserID,
	EntranceDate,
	Sent,
	Deleted
)
SELECT Ref.KnowledgeID, Ref.UserID, Ref.EntranceDate, Ref.Sent, 0
FROM [dbo].[KWF_Managers] AS Ref
GO


DROP TABLE [dbo].[KWF_Managers]
GO


CREATE TABLE [dbo].[KWF_Managers](
	[ID] [bigint] NOT NULL IDENTITY(1,1),
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[EntranceDate] [datetime] NULL,
	[EvaluationDate] [datetime] NULL,
	[Sent] [bit] NOT NULL,
	[HaveRejectedEvaluators] [bit] NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_KWF_Managers] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[KWF_Managers]  WITH CHECK ADD  CONSTRAINT [FK_KWF_Managers_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KWF_Managers] CHECK CONSTRAINT [FK_KWF_Managers_aspnet_Users]
GO

ALTER TABLE [dbo].[KWF_Managers]  WITH CHECK ADD  CONSTRAINT [FK_KWF_Managers_KW_Knowledges] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[KW_Knowledges] ([KnowledgeID])
GO

ALTER TABLE [dbo].[KWF_Managers] CHECK CONSTRAINT [FK_KWF_Managers_KW_Knowledges]
GO


INSERT INTO [dbo].[KWF_Managers](
	KnowledgeID,
	UserID,
	EntranceDate,
	Sent,
	Deleted
)
SELECT Ref.KnowledgeID, Ref.UserID, Ref.EntranceDate, Ref.Sent, 0
FROM [dbo].[TMP_KWF_Managers] AS Ref
GO


DROP TABLE [dbo].[TMP_KWF_Managers]
GO

UPDATE [dbo].[KWF_Managers]
	SET EvaluationDate = EntranceDate
WHERE Sent = 1
GO

-- end of Update KWF_Managers


-- Update KWF_Experts

CREATE TABLE [dbo].[TMP_KWF_Experts](
	[ID] [bigint] NOT NULL IDENTITY(1,1),
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NOT NULL,
	[SenderUserID] [uniqueidentifier] NULL,
	[Evaluated] [bit] NOT NULL,
	[Score] [float] NULL,
	[EntranceDate] [datetime] NULL,
	[ExpirationDate] [datetime] NULL,
	[EvaluationDate] [datetime] NULL,
	[Rejected] [bit] NULL,
	[Deleted] [bit]
 CONSTRAINT [PK_TMP_KWF_Experts] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[TMP_KWF_Experts](
	KnowledgeID,
	UserID,
	NodeID,
	SenderUserID,
	Evaluated,
	Score,
	EntranceDate,
	ExpirationDate,
	EvaluationDate,
	Deleted
)
SELECT Ref.KnowledgeID, Ref.UserID, Ref.NodeID, Ref.SenderUserID, Ref.Evaluated,
	Ref.Score, Ref.EntranceDate, Ref.ExpirationDate, Ref.EvaluationDate, 0
FROM [dbo].[KWF_Experts] AS Ref
GO


DROP TABLE [dbo].[KWF_Experts]
GO


CREATE TABLE [dbo].[KWF_Experts](
	[ID] [bigint] NOT NULL IDENTITY(1,1),
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NOT NULL,
	[SenderUserID] [uniqueidentifier] NULL,
	[Evaluated] [bit] NOT NULL,
	[Score] [float] NULL,
	[EntranceDate] [datetime] NULL,
	[ExpirationDate] [datetime] NULL,
	[EvaluationDate] [datetime] NULL,
	[Rejected] [bit] NULL,
	[Deleted] [bit]
 CONSTRAINT [PK_KWF_Experts] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
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

ALTER TABLE [dbo].[KWF_Experts]  WITH CHECK ADD  CONSTRAINT [FK_KWF_Experts_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[KWF_Experts] CHECK CONSTRAINT [FK_KWF_Experts_CN_Nodes]
GO

ALTER TABLE [dbo].[KWF_Experts]  WITH CHECK ADD  CONSTRAINT [FK_KWF_Experts_KW_Knowledges] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[KW_Knowledges] ([KnowledgeID])
GO

ALTER TABLE [dbo].[KWF_Experts] CHECK CONSTRAINT [FK_KWF_Experts_KW_Knowledges]
GO


INSERT INTO [dbo].[KWF_Experts](
	KnowledgeID,
	UserID,
	NodeID,
	SenderUserID,
	Evaluated,
	Score,
	EntranceDate,
	ExpirationDate,
	EvaluationDate,
	Deleted
)
SELECT Ref.KnowledgeID, Ref.UserID, Ref.NodeID, Ref.SenderUserID, Ref.Evaluated,
	Ref.Score, Ref.EntranceDate, Ref.ExpirationDate, Ref.EvaluationDate, 0
FROM [dbo].[TMP_KWF_Experts] AS Ref
GO


DROP TABLE [dbo].[TMP_KWF_Experts]
GO

-- end of Update KWF_Experts


-- Update KWF_Evaluators

CREATE TABLE [dbo].[TMP_KWF_Evaluators](
	[ID] [bigint] NOT NULL IDENTITY(1,1),
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[SenderUserID] [uniqueidentifier] NULL,
	[Evaluated] [bit] NOT NULL,
	[Score] [float] NULL,
	[EntranceDate] [datetime] NULL,
	[ExpirationDate] [datetime] NULL,
	[EvaluationDate] [datetime] NULL,
	[Rejected] [bit] NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_TMP_KWF_Evaluators] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[TMP_KWF_Evaluators](
	KnowledgeID,
	UserID,
	SenderUserID,
	Evaluated,
	Score,
	EntranceDate,
	ExpirationDate,
	EvaluationDate,
	Deleted
)
SELECT Ref.KnowledgeID, Ref.UserID, Ref.SenderUserID, Ref.Evaluated, Ref.Score,
	Ref.EntranceDate, Ref.ExpirationDate, Ref.EvaluationDate, 0
FROM [dbo].[KWF_Evaluators] AS Ref
GO


DROP TABLE [dbo].[KWF_Evaluators]
GO


CREATE TABLE [dbo].[KWF_Evaluators](
	[ID] [bigint] NOT NULL IDENTITY(1,1),
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[SenderUserID] [uniqueidentifier] NULL,
	[Evaluated] [bit] NOT NULL,
	[Score] [float] NULL,
	[EntranceDate] [datetime] NULL,
	[ExpirationDate] [datetime] NULL,
	[EvaluationDate] [datetime] NULL,
	[Rejected] [bit] NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_KWF_Evaluators] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[KWF_Evaluators]  WITH CHECK ADD  CONSTRAINT [FK_KWF_Evaluators_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KWF_Evaluators] CHECK CONSTRAINT [FK_KWF_Evaluators_aspnet_Users]
GO

ALTER TABLE [dbo].[KWF_Evaluators]  WITH CHECK ADD  CONSTRAINT [FK_KWF_Evaluators_KW_Knowledges] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[KW_Knowledges] ([KnowledgeID])
GO

ALTER TABLE [dbo].[KWF_Evaluators] CHECK CONSTRAINT [FK_KWF_Evaluators_KW_Knowledges]
GO


INSERT INTO [dbo].[KWF_Evaluators](
	KnowledgeID,
	UserID,
	SenderUserID,
	Evaluated,
	Score,
	EntranceDate,
	ExpirationDate,
	EvaluationDate,
	Deleted
)
SELECT Ref.KnowledgeID, Ref.UserID, Ref.SenderUserID, Ref.Evaluated, Ref.Score,
	Ref.EntranceDate, Ref.ExpirationDate, Ref.EvaluationDate, 0
FROM [dbo].[TMP_KWF_Evaluators] AS Ref
GO


DROP TABLE [dbo].[TMP_KWF_Evaluators]
GO

-- end of Update KWF_Evaluators


