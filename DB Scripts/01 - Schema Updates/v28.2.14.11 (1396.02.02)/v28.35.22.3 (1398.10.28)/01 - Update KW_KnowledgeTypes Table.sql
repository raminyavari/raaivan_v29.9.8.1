USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

ALTER TABLE [dbo].[KW_KnowledgeTypes]
ADD [EvaluationsRemovable] [bit] NULL
GO

ALTER TABLE [dbo].[KW_KnowledgeTypes]
ADD [EvaluationsEditable] [bit] NULL
GO

ALTER TABLE [dbo].[KW_KnowledgeTypes]
ADD [UnhideEvaluators] [bit] NULL
GO

ALTER TABLE [dbo].[KW_KnowledgeTypes]
ADD [UnhideEvaluations] [bit] NULL
GO

ALTER TABLE [dbo].[KW_KnowledgeTypes]
ADD [UnhideNodeCreators] [bit] NULL
GO

SET ANSI_PADDING OFF
GO

