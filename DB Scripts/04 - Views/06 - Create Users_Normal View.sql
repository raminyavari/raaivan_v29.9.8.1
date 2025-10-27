USE [EKM_App]
GO

SET NUMERIC_ROUNDABORT OFF;
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT,
    QUOTED_IDENTIFIER, ANSI_NULLS ON;


IF EXISTS(select * FROM sys.views where name = 'Users_Normal')
DROP VIEW [dbo].[Users_Normal]
GO


CREATE VIEW [dbo].[Users_Normal] WITH SCHEMABINDING, ENCRYPTION
AS
SELECT  APP.ApplicationID,
		U.UserId AS UserID, 
		U.UserName, 
		U.LoweredUserName,
		P.FirstName, 
		P.LastName, 
		P.BirthDay,
		P.AboutMe,
		P.City,
		APP.Organization,
		APP.Department,
		APP.JobTitle,
		APP.EmploymentType,
		P.Settings,
		P.MainPhoneID,
		P.MainEmailID,
		P.TwoStepAuthentication,
		M.IsApproved,
		M.IsLockedOut,
		M.CreateDate AS CreationDate,
		P.IndexLastUpdateDate,
		U.LastActivityDate
FROM    [dbo].[aspnet_Users] AS U
		INNER JOIN [dbo].[USR_Profile] AS P
		ON P.UserID = U.UserId
		INNER JOIN [dbo].[aspnet_Membership] AS M
		ON M.UserId = U.UserId
		INNER JOIN [dbo].[USR_UserApplications] AS APP
		ON APP.UserID = U.UserId

GO

CREATE UNIQUE CLUSTERED INDEX PK_View_Users_Normal_UserID ON [dbo].[Users_Normal]
(
	[ApplicationID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO