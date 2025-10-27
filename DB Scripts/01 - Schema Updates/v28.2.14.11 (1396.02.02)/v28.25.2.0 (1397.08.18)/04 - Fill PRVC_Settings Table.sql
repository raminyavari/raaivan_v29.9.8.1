USE [EKM_APP]
GO


INSERT INTO [dbo].[PRVC_Settings](ApplicationID, ObjectID, ConfidentialityID, CalculateHierarchy,
	CreatorUserID, CreationDate, LastModifierUserID, LastModificationDate)
SELECT PT.ApplicationID, PT.ObjectID, PT.ConfidentialityID, PT.CalculateHierarchy,
	PT.CreatorUserID, PT.CreationDate, PT.LastModifierUserID, PT.LastModificationDate
FROM [dbo].[PRVC_PrivacyType] AS PT

GO