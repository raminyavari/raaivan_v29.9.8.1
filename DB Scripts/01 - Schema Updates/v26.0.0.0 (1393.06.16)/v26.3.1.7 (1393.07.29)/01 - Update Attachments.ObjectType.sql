USE [EKM_App]
GO


UPDATE ATT
	SET ObjectType = N'Node'
FROM [dbo].[Attachments] AS ATT
	INNER JOIN [dbo].[CN_Nodes] AS ND
	ON ND.NodeID = ATT.ObjectID
	
GO