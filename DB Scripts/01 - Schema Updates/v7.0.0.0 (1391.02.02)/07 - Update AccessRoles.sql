USE [EKM_App]

DELETE FROM [dbo].[UserGroupAccessRoles]
WHERE [dbo].[UserGroupAccessRoles].[AccessRoleId] =
	  (SELECT dbo.AccessRoles.ID FROM dbo.AccessRoles INNER JOIN 
       dbo.UserGroupAccessRoles ON dbo.AccessRoles.ID = dbo.UserGroupAccessRoles.AccessRoleId
       WHERE dbo.AccessRoles.Role = N'NodeScan')
GO


DELETE FROM [dbo].[AccessRoles]
WHERE [dbo].[AccessRoles].[Role] = N'NodeScan'
GO
