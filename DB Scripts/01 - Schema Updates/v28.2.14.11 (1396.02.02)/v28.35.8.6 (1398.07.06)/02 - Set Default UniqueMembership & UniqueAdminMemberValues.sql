USE [EKM_App]
GO


UPDATE S
	SET UniqueAdminMember = 1
FROM [dbo].[CN_NodeTypes] AS NT
	INNER JOIN [dbo].[CN_Services] AS S
	ON S.ApplicationID = NT.ApplicationID AND S.NodeTypeID = NT.NodeTypeID
WHERE NT.AdditionalID = N'2'

GO


UPDATE S
	SET UniqueMembership = 1,
		UniqueAdminMember = 1
FROM [dbo].[CN_NodeTypes] AS NT
	INNER JOIN [dbo].[CN_Services] AS S
	ON S.ApplicationID = NT.ApplicationID AND S.NodeTypeID = NT.NodeTypeID
WHERE NT.AdditionalID = N'6'

GO