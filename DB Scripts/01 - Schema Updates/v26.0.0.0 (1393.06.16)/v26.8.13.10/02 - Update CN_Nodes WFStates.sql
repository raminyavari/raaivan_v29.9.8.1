USE [EKM_App]
GO

UPDATE ND
	SET WFState = S.Title
FROM [dbo].[CN_Nodes] AS ND
	INNER JOIN [dbo].[WF_History] AS A
	INNER JOIN (
		SELECT OwnerID, MAX(SendDate) AS SendDate
		FROM [dbo].[WF_History]
		GROUP BY OwnerID
	) AS B
	ON A.OwnerID = B.OwnerID AND A.SendDate = B.SendDate
	INNER JOIN [dbo].[WF_States] AS S
	ON S.StateID = A.StateID
	ON A.OwnerID = ND.NodeID
WHERE A.Deleted = 0


GO