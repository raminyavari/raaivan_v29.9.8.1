USE [EKM_App]
GO

DECLARE @UserID uniqueidentifier = (SELECT TOP(1) UserId FROM [dbo].[aspnet_Users] WHERE LoweredUserName = N'admin')

INSERT INTO [dbo].[PRVC_PrivacyType](
	ObjectID,
	PrivacyType,
	CreatorUserID,
	CreationDate,
	Deleted
)
SELECT	ND.NodeID,
		CASE
			WHEN ND.Privacy = N'Custom' THEN N'Restricted'
			ELSE N'Public'
		END,
		ISNULL(ND.LastModifierUserID, ISNULL(ND.CreatorUserID, @UserID)),
		ISNULL(ND.LastModificationDate, ISNULL(ND.CreationDate, '04/04/2012 12:34:15')),
		0
FROM [dbo].[CN_Nodes] AS ND
	LEFT JOIN [dbo].[PRVC_PrivacyType] AS PT
	ON ND.NodeID = PT.ObjectID
WHERE ND.Privacy IS NOT NULL AND PT.ObjectID IS NULL

GO


/****** Object:  Table [dbo].[KKnowledges]    Script Date: 04/04/2012 12:34:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



UPDATE [dbo].[AppSetting]
	SET [Version] = 'v24.0.2.3' -- 13930331
GO

