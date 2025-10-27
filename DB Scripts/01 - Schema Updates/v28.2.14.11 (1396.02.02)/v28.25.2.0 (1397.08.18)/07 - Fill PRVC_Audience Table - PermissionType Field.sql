USE [EKM_App]
GO


UPDATE [dbo].[PRVC_Audience]
	SET PermissionType = N'View'
GO


UPDATE A
	SET PermissionType = N'Create'
FROM [dbo].[PRVC_Audience] AS A
	INNER JOIN [dbo].[CN_NodeTypes] AS NT
	ON NT.ApplicationID = A.ApplicationID AND NT.NodeTypeID = A.ObjectID
GO


UPDATE A
	SET PermissionType = N'Create'
FROM [dbo].[PRVC_Audience] AS A
	INNER JOIN [dbo].[QA_WorkFlows] AS NT
	ON NT.ApplicationID = A.ApplicationID AND NT.WorkFlowID = A.ObjectID
GO