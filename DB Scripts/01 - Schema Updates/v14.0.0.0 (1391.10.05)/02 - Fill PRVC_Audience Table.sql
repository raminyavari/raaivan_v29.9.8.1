USE [EKM_App]
GO

INSERT INTO [dbo].[PRVC_Audience](
	RoleID,
	ObjectID,
	Allow,
	ModificationRight,
	Deleted
)
SELECT ND.NodeID, ND.NodeID, 1, 0, 0
FROM [dbo].[CN_Nodes] AS ND
WHERE NOT EXISTS(SELECT TOP(1) * FROM [dbo].[PRVC_Audience] AS AU
	WHERE AU.RoleID = ND.NodeID AND AU.ObjectID = ND.NodeID)

GO