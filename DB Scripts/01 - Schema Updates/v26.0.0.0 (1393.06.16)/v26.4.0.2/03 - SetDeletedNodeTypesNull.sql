Use [EKM_App]
GO

Update [dbo].[CN_NodeTypes]
	SET ParentID = NULL
WHERE [Deleted] = 1
GO
