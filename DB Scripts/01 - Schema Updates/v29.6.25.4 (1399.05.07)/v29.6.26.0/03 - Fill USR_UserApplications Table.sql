USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


INSERT INTO [dbo].[USR_UserApplications] (ApplicationID, UserID)
SELECT P.ApplicationID, P.UserID
FROM [dbo].[USR_Profile] AS P

GO