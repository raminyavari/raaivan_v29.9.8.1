USE [EKM_App]
GO


ALTER TABLE [dbo].[TMP_KW_KnowledgeTypes]
ADD [MinAcceptableScore2] int NULL
GO


UPDATE [dbo].[TMP_KW_KnowledgeTypes]
	SET MinAcceptableScore2 = MinAcceptableScore
GO


ALTER TABLE [dbo].[TMP_KW_KnowledgeTypes]
DROP COLUMN [MinAcceptableScore]
GO


ALTER TABLE [dbo].[TMP_KW_KnowledgeTypes]
ADD [MinAcceptableScore] float NULL
GO


UPDATE [dbo].[TMP_KW_KnowledgeTypes]
	SET MinAcceptableScore = MinAcceptableScore2
GO


ALTER TABLE [dbo].[TMP_KW_KnowledgeTypes]
DROP COLUMN [MinAcceptableScore2]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



UPDATE [dbo].[AppSetting]
	SET [Version] = 'v27.5.9.2' -- 13950530
GO

