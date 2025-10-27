USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER TABLE [dbo].[KW_KnowledgeTypes]
ADD TextOptions nvarchar(max) NULL
GO


ALTER TABLE [dbo].[KW_KnowledgeTypes]
ADD EvaluationsEditableForAdmin bit NULL
GO

