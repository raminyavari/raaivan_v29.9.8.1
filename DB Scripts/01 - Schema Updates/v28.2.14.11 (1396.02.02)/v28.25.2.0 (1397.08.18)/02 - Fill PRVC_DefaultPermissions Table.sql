USE [EKM_APP]
GO


INSERT INTO [dbo].[PRVC_DefaultPermissions] (ApplicationID, ObjectID, PermissionType, DefaultValue,
	CreatorUserID, CreationDate, LastModifierUserID, LastModificationDate)
SELECT P.ApplicationID, P.ObjectID, N'Create', P.PrivacyType, P.CreatorUserID, P.CreationDate,
	P.LastModifierUserID, P.LastModificationDate
FROM [dbo].[PRVC_PrivacyType] AS P
	INNER JOIN [dbo].[CN_NodeTypes] AS NT
	ON NT.ApplicationID = P.ApplicationID AND NT.NodeTypeID = P.ObjectID
WHERE ISNULL(P.PrivacyType, N'') <> N''

GO


INSERT INTO [dbo].[PRVC_DefaultPermissions] (ApplicationID, ObjectID, PermissionType, DefaultValue,
	CreatorUserID, CreationDate, LastModifierUserID, LastModificationDate)
SELECT P.ApplicationID, P.ObjectID, N'View', P.PrivacyType, P.CreatorUserID, P.CreationDate,
	P.LastModifierUserID, P.LastModificationDate
FROM [dbo].[PRVC_PrivacyType] AS P
	INNER JOIN [dbo].[CN_Nodes] AS NT
	ON NT.ApplicationID = P.ApplicationID AND NT.NodeID = P.ObjectID
WHERE ISNULL(P.PrivacyType, N'') <> N''

GO

INSERT INTO [dbo].[PRVC_DefaultPermissions] (ApplicationID, ObjectID, PermissionType, DefaultValue,
	CreatorUserID, CreationDate, LastModifierUserID, LastModificationDate)
SELECT P.ApplicationID, P.ObjectID, N'View', P.PrivacyType, P.CreatorUserID, P.CreationDate,
	P.LastModifierUserID, P.LastModificationDate
FROM [dbo].[PRVC_PrivacyType] AS P
	LEFT JOIN [dbo].[PRVC_DefaultPermissions] AS NT
	ON NT.ApplicationID = P.ApplicationID AND NT.ObjectID = P.ObjectID
WHERE ISNULL(P.PrivacyType, N'') <> N'' AND NT.ObjectID IS NULL

GO

UPDATE D
	SET DefaultValue = N'Create'
FROM [dbo].[PRVC_DefaultPermissions] AS D
	INNER JOIN [dbo].[CN_Services] AS S
	ON S.ApplicationID = D.ApplicationID AND S.NodeTypeID = D.ObjectID
GO

UPDATE D
	SET DefaultValue = N'Create'
FROM [dbo].[PRVC_DefaultPermissions] AS D
	INNER JOIN [dbo].[QA_WorkFlows] AS W
	ON W.ApplicationID = D.ApplicationID AND W.WorkFlowID = D.ObjectID
GO



UPDATE [dbo].[PRVC_DefaultPermissions]
	SET DefaultValue = N'Public'
WHERE DefaultValue = N'PublicAsDefault'

GO