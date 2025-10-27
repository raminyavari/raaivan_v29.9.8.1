USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


UPDATE A
	SET CreationDate = ISNULL(X.MinDate, DATEADD(DAY, -10, GETDATE()))
FROM [dbo].[USR_UserApplications] AS A 
	LEFT JOIN (
		SELECT lg.ApplicationID, lg.UserID, MIN(lg.[Date]) AS MinDate
		FROM [dbo].[LG_Logs] AS lg
		GROUP BY lg.[ApplicationID], lg.UserID
	) AS X
	ON A.ApplicationID = A.ApplicationID AND X.UserID = A.UserID
WHERE A.CreationDate IS NULL

GO