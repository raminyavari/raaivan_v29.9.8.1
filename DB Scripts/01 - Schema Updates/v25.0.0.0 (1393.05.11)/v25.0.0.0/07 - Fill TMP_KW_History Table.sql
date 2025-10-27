USE [EKM_App]
GO


INSERT INTO [dbo].[TMP_KW_History](
	KnowledgeID,
	[Action],
	ActorUserID,
	ActionDate
)
SELECT *
FROM (
		SELECT	MNG.KnowledgeID,
				N'SendToAdmin' AS [Action],
				ND.CreatorUserID AS ActorUserID,
				MNG.EntranceDate AS ActionDate
		FROM [dbo].[KWF_Managers] AS MNG
			INNER JOIN [dbo].[CN_Nodes] AS ND
			ON ND.NodeID = MNG.KnowledgeID
			
		UNION ALL
		
		SELECT	MNG.KnowledgeID,
				N'SendToEvaluators' AS [Action],
				MNG.UserID AS ActorUserID,
				MIN(MNG.EvaluationDate) AS ActionDate
		FROM [dbo].[KWF_Managers] AS MNG
		WHERE MNG.[Sent] = 1 AND MNG.EvaluationDate IS NOT NULL
		GROUP BY MNG.KnowledgeID, MNG.UserID
		
		UNION ALL
		
		SELECT	E.KnowledgeID,
				N'Evaluation' AS [Action],
				E.UserID AS ActorUserID,
				MIN(E.EvaluationDate) AS ActionDate
		FROM (
				SELECT Ex.KnowledgeID, Ex.UserID, Ex.EvaluationDate
				FROM [dbo].[KWF_Experts] AS Ex
				WHERE Ex.Evaluated = 1 AND ISNULL(Ex.Score, 0) > 0
				
				UNION ALL
				
				SELECT Ev.KnowledgeID, Ev.UserID, Ev.EvaluationDate
				FROM [dbo].[KWF_Evaluators] AS Ev
				WHERE Ev.Evaluated = 1 AND ISNULL(Ev.Score, 0) > 0
			) AS E
		GROUP BY E.KnowledgeID, E.UserID
	) AS Ref
ORDER BY Ref.ActionDate ASC

GO