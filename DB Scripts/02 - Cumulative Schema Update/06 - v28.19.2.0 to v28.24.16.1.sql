USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.19.2.0' BEGIN
	ALTER TABLE [dbo].[CN_Services]
	ADD [NoContent] bit NULL
END

GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.19.2.0' BEGIN
	ALTER TABLE [dbo].[CN_Nodes]
	ADD [HideCreators] bit NULL
END

GO

SET ANSI_PADDING ON
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.19.2.0' BEGIN
	UPDATE ND
		SET Searchable = 1
	FROM [dbo].[CN_Nodes] AS ND
		INNER JOIN (
			SELECT DISTINCT H.OwnerID
			FROM [WF_History] AS H
		) AS H
		ON H.OwnerID = ND.NodeID
END
	
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.19.2.0' BEGIN
	UPDATE ND
		SET HideCreators = 1
	FROM [dbo].[CN_Nodes] AS ND
		INNER JOIN[dbo].[WF_History] AS X
		ON X.OwnerID = ND.NodeID 
		INNER JOIN (
			SELECT H.OwnerID, MAX(H.ID) AS ID
			FROM [dbo].[WF_History] AS H
			WHERE H.Deleted = 0
			GROUP BY H.OwnerID
		) AS A
		ON A.ID = X.ID
		INNER JOIN [dbo].[WF_WorkFlowStates] AS WS
		ON WS.WorkFlowID = X.WorkFlowID AND WS.StateID = X.StateID
	WHERE WS.HideOwnerName = 1
END

GO

SET ANSI_PADDING OFF
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.19.2.0' BEGIN
	ALTER TABLE [dbo].[FG_FormInstances]
	ADD [IsTemporary] bit NULL
END

GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.19.2.0' BEGIN
	UPDATE [dbo].[AppSetting]
		SET [Version] = 'v28.24.16.1'
END

GO