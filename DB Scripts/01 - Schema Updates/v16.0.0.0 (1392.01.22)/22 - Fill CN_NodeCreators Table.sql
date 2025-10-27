USE EKM_App
GO


INSERT INTO [dbo].[CN_NodeCreators](
	NodeID,
	UserID,
	CollaborationShare,
	[Status],
	CreatorUserID,
	CreationDate,
	Deleted
)
SELECT ND.NodeID, ND.CreatorUserID, 100, N'Accepted', ND.CreatorUserID, ND.CreationDate, 0
FROM [dbo].[WF_Services] AS SRV
	INNER JOIN [dbo].[CN_Nodes] AS ND
	ON SRV.NodeTypeID = ND.NodeTypeID
WHERE ND.CreatorUserID IS NOT NULL AND NOT EXISTS(SELECT TOP(1) * FROM
	[dbo].[CN_NodeCreators] AS NC WHERE NC.NodeID = ND.NodeID)
	
GO