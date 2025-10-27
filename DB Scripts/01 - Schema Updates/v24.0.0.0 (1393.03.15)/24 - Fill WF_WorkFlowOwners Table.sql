USE [EKM_App]
GO


INSERT INTO [dbo].[WF_WorkFlowOwners](
	ID,
	NodeTypeID,
	WorkFlowID,
	CreatorUserID,
	CreationDate,
	LastModifierUserID,
	LastModificationDate,
	Deleted
)
SELECT	ServiceID,
		NodeTypeID,
		WorkFlowID,
		CreatorUserID,
		CreationDate,
		LastModifierUserID,
		LastModificationDate,
		Deleted
FROM [dbo].[WF_Services] 

GO