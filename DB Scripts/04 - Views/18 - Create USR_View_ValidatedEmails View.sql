USE [EKM_App]
GO

SET NUMERIC_ROUNDABORT OFF;
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT,
    QUOTED_IDENTIFIER, ANSI_NULLS ON;


IF EXISTS(select * FROM sys.views where name = 'USR_View_ValidatedEmails')
DROP VIEW [dbo].[USR_View_ValidatedEmails]
GO

CREATE VIEW [dbo].[USR_View_ValidatedEmails] WITH SCHEMABINDING, ENCRYPTION
AS
SELECT	E.EmailID,
		E.UserID,
		LOWER(U.LoweredUserName) AS UserName,
		LOWER(E.EmailAddress) AS Email
FROM [dbo].[USR_EmailAddresses] AS E
	INNER JOIN [dbo].[aspnet_Users] AS U
	ON U.UserId = E.UserID
WHERE E.Deleted = 0 AND E.Validated = 1

GO

CREATE UNIQUE CLUSTERED INDEX PK_View_ValidatedEmails_Email ON [dbo].[USR_View_ValidatedEmails]
(
	[Email] ASC,
	[UserID] ASC,
	[EmailID] ASC,
	[UserName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO