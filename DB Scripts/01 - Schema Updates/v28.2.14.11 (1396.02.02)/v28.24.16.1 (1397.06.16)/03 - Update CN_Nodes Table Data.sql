USE [EKM_App]
GO

SET ANSI_PADDING ON
GO

UPDATE ND
	SET Searchable = 1
FROM [dbo].[CN_Nodes] AS ND
	INNER JOIN (
		SELECT DISTINCT H.OwnerID
		FROM [WF_History] AS H
	) AS H
	ON H.OwnerID = ND.NodeID
	
GO


UPDATE ND
	SET HideCreators = 1
FROM [dbo].[CN_Nodes] AS ND
	INNER JOIN[dbo].[WF_History] AS X
	ON X.OwnerID = ND.NodeID 
	INNER JOIN (
		SELECT H.OwnerID, MAX(H.ID) AS ID
		FROM [dbo].[WF_History] AS H
		WHERE H.Deleted = 0
		GROUP BY H.OwnerID
	) AS A
	ON A.ID = X.ID
	INNER JOIN [dbo].[WF_WorkFlowStates] AS WS
	ON WS.WorkFlowID = X.WorkFlowID AND WS.StateID = X.StateID
WHERE WS.HideOwnerName = 1

GO

SET ANSI_PADDING OFF
GO