USE [EKM_App]
GO

/****** Object:  Table [dbo].[QQuestion_Nodes]    Script Date: 02/17/2012 15:23:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[QQuestionNodesTemp](
	[QuestionID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NOT NULL,
	[Date] [datetime] NULL,
 CONSTRAINT [PK_QQuestionNodesTemp] PRIMARY KEY CLUSTERED 
(
	[QuestionID] ASC,
	[NodeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[QQuestionNodesTemp]
           ([QuestionID]
			,[NodeID]
			,[Date])
		SELECT  dbo.QQuestion_Nodes.QuestionId, dbo.CN_Nodes.NodeID, dbo.QQuestion_Nodes.Date
		FROM    dbo.CN_Nodes INNER JOIN dbo.QQuestion_Nodes ON dbo.CN_Nodes.ID = dbo.QQuestion_Nodes.NodeId
GO


/* Drop old table */
DROP TABLE [dbo].[QQuestion_Nodes]
GO

/* Create old new table */
CREATE TABLE [dbo].[QuestionNodes](
	[QuestionID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NOT NULL,
	[Date] [datetime] NULL,
 CONSTRAINT [PK_QuestionNodes] PRIMARY KEY CLUSTERED 
(
	[QuestionID] ASC,
	[NodeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[QuestionNodes]  WITH CHECK ADD  CONSTRAINT [FK_QuestionNodes_QQuestions] FOREIGN KEY([QuestionID])
REFERENCES [dbo].[QQuestions] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[QuestionNodes] CHECK CONSTRAINT [FK_QuestionNodes_QQuestions]
GO

ALTER TABLE [dbo].[QuestionNodes]  WITH CHECK ADD  CONSTRAINT [FK_QuestionNodes_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[QuestionNodes] CHECK CONSTRAINT [FK_QuestionNodes_CN_Nodes]
GO


INSERT INTO [dbo].[QuestionNodes]
	SELECT * FROM dbo.QQuestionNodesTemp
GO

DROP TABLE [dbo].[QQuestionNodesTemp]
GO