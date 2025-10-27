USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


UPDATE A
	SET JobTitle = P.JobTitle
FROM [dbo].[USR_UserApplications] AS A
	INNER JOIN [dbo].[USR_Profile] AS P
	ON P.UserID = A.UserID
GO


IF EXISTS(select * FROM sys.views where name = 'USR_View_Users')
DROP VIEW [dbo].[USR_View_Users]
GO

IF EXISTS(select * FROM sys.views where name = 'Users_Normal')
DROP VIEW [dbo].[Users_Normal]
GO

ALTER TABLE [dbo].[USR_Profile]
DROP COLUMN JobTitle
GO