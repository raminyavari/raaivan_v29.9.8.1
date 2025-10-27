USE [EKM_APP]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/****** Object:  Table [dbo].[KWF_Managers]    Script Date: 12/16/2011 23:13:02 ******/
CREATE TABLE [dbo].[KWF_Managers](
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[EntranceDate] [datetime] NULL,
	[Sent] [bit] NOT NULL,
 CONSTRAINT [PK_KWF_Managers] PRIMARY KEY CLUSTERED 
(
	[KnowledgeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[KWF_Managers]  WITH CHECK ADD  CONSTRAINT [FK_KWF_Managers_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KWF_Managers] CHECK CONSTRAINT [FK_KWF_Managers_aspnet_Users]
GO

ALTER TABLE [dbo].[KWF_Managers]  WITH CHECK ADD  CONSTRAINT [FK_KWF_Managers_KKnowledges] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[KKnowledges] ([ID])
GO

ALTER TABLE [dbo].[KWF_Managers] CHECK CONSTRAINT [FK_KWF_Managers_KKnowledges]
GO


/****** Object:  Table [dbo].[KWF_Experts]    Script Date: 12/16/2011 23:13:46 ******/
CREATE TABLE [dbo].[KWF_Experts](
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[NodeID] [bigint] NOT NULL,
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

ALTER TABLE [dbo].[KWF_Experts]  WITH CHECK ADD  CONSTRAINT [FK_KWF_Experts_KKnowledges] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[KKnowledges] ([ID])
GO

ALTER TABLE [dbo].[KWF_Experts] CHECK CONSTRAINT [FK_KWF_Experts_KKnowledges]
GO

ALTER TABLE [dbo].[KWF_Experts]  WITH CHECK ADD  CONSTRAINT [FK_KWF_Experts_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[Nodes] ([ID])
GO

ALTER TABLE [dbo].[KWF_Experts] CHECK CONSTRAINT [FK_KWF_Experts_Nodes]
GO

ALTER TABLE [dbo].[KWF_Experts]  WITH CHECK ADD  CONSTRAINT [FK_KWF_Experts_aspnet_Users_Sender] FOREIGN KEY([SenderUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KWF_Experts] CHECK CONSTRAINT [FK_KWF_Experts_aspnet_Users_Sender]
GO

