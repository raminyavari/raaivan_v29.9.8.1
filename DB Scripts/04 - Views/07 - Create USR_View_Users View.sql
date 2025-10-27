USE [EKM_App]
GO

SET NUMERIC_ROUNDABORT OFF;
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT,
    QUOTED_IDENTIFIER, ANSI_NULLS ON;


IF EXISTS(select * FROM sys.views where name = 'USR_View_Users')
DROP VIEW [dbo].[USR_View_Users]
GO


CREATE VIEW [dbo].[USR_View_Users] WITH SCHEMABINDING, ENCRYPTION
AS
SELECT  U.UserId AS UserID, 
		U.UserName, 
		U.LoweredUserName,
		P.FirstName, 
		P.LastName, 
		P.BirthDay,
		P.MainPhoneID,
		P.MainEmailID,
		P.Settings,
		P.TwoStepAuthentication,
		M.IsApproved,
		M.IsLockedOut,
		M.CreateDate AS CreationDate
FROM    [dbo].[aspnet_Users] AS U
		INNER JOIN [dbo].[USR_Profile] AS P
		ON P.UserID = U.UserId
		INNER JOIN [dbo].[aspnet_Membership] AS M
		ON M.UserId = U.UserId

GO

CREATE UNIQUE CLUSTERED INDEX PK_USR_View_Users_UserID ON [dbo].[USR_View_Users]
(
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
