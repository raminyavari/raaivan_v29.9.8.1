USE [EKM_App]
GO


DECLARE @Evs Table(ID int IDENTITY(1,1) primary key clustered, 
	UserID uniqueidentifier, KnowledgeID uniqueidentifier, Score float, DT datetime)

INSERT INTO @Evs(UserID, KnowledgeID, Score, DT)
SELECT	E.UserID,
		E.KnowledgeID,
		CAST(ISNULL(SUM(E.Score), 0) AS float) / 
			CAST(ISNULL(COUNT(E.Score), 1) AS float) AS Score,
		MAX(E.EvaluationDate) AS DT
FROM (	
		SELECT Ev.KnowledgeID, Ev.UserID, Ev.Score, Ev.EvaluationDate
		FROM [dbo].[KWF_Evaluators] AS Ev
		WHERE Ev.Deleted = 0 AND Ev.Score IS NOT NULL AND Ev.Score > 0

		UNION ALL

		SELECT Ex.KnowledgeID, Ex.UserID, Ex.Score, Ex.EvaluationDate
		FROM [dbo].[KWF_Experts] AS Ex
		WHERE Ex.Deleted = 0 AND Ex.Score IS NOT NULL AND Ex.Score > 0
	) AS E
GROUP BY E.UserID, E.KnowledgeID


INSERT INTO [dbo].[TMP_KW_QuestionAnswers](
	KnowledgeID,
	UserID,
	QuestionID,
	Title,
	Score,
	ResponderUserID,
	EvaluationDate,
	Deleted
)
SELECT DISTINCT
		E.KnowledgeID,
		E.UserID,
		TQ.QuestionID,
		Q.Title,
		E.Score,
		E.UserID,
		E.DT,
		0
FROM @Evs AS E
	INNER JOIN [dbo].[CN_Nodes] AS ND
	ON ND.NodeID = E.KnowledgeID
	INNER JOIN [dbo].[TMP_KW_TypeQuestions] AS TQ
	ON TQ.KnowledgeTypeID = ND.NodeTypeID
	INNER JOIN [dbo].[TMP_KW_Questions] AS Q
	ON TQ.QuestionID = Q.QuestionID
	
	
DECLARE @Count int = (SELECT MAX(ID) FROM @Evs)

WHILE @Count > 0 BEGIN
	DECLARE @KID uniqueidentifier = 
		(SELECT TOP(1) E.KnowledgeID FROM @Evs AS E WHERE E.ID = @Count)
	
	DECLARE @Score float = (
		SELECT TOP(1) (SUM(Ref.Score) / ISNULL(COUNT(Ref.UserID), 1)) AS S
		FROM (
				SELECT	QA.UserID, 
						SUM(ISNULL(QA.Score, 0)) / ISNULL(COUNT(QA.QuestionID), 1) AS Score
				FROM [dbo].[TMP_KW_QuestionAnswers] AS QA
				WHERE QA.KnowledgeID = @KID AND QA.Deleted = 0
				GROUP BY QA.UserID
			) AS Ref
		GROUP BY Ref.UserID
	)
	
	UPDATE [dbo].[CN_Nodes]
		SET Score = @Score
	WHERE NodeID = @KID
	
	SET @Count = @Count - 1
END

GO