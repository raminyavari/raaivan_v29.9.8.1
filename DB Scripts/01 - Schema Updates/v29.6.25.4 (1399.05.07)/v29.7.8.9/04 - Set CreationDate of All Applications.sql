USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


UPDATE A
	SET CreationDate = X.MinDate
FROM [dbo].[aspnet_Applications] AS A 
	INNER JOIN (
		SELECT lg.ApplicationID, MIN(lg.[Date]) AS MinDate
		FROM [dbo].[LG_Logs] AS lg
		GROUP BY lg.[ApplicationID]
	) AS X
	ON A.ApplicationID = A.ApplicationID
WHERE A.CreationDate IS NULL

GO