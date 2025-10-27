USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


DELETE FROM [dbo].[KWF_Managers]
WHERE EXISTS (SELECT * FROM dbo.KKnowledges INNER JOIN
				  dbo.KWF_Statuses ON dbo.KKnowledges.StatusID = dbo.KWF_Statuses.StatusID
			  WHERE (dbo.KKnowledges.ID = dbo.KWF_Managers.KnowledgeID AND 
				  dbo.KWF_Statuses.Name = 'SentBackForRevision'))
GO


