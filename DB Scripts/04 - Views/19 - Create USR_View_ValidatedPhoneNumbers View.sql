USE [EKM_App]
GO

SET NUMERIC_ROUNDABORT OFF;
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT,
    QUOTED_IDENTIFIER, ANSI_NULLS ON;


IF EXISTS(select * FROM sys.views where name = 'USR_View_ValidatedPhoneNumbers')
DROP VIEW [dbo].[USR_View_ValidatedPhoneNumbers]
GO

CREATE VIEW [dbo].[USR_View_ValidatedPhoneNumbers] WITH SCHEMABINDING, ENCRYPTION
AS
SELECT	E.NumberID,
		E.UserID,
		LOWER(U.LoweredUserName) AS UserName,
		E.PhoneNumber
FROM [dbo].[USR_PhoneNumbers] AS E
	INNER JOIN [dbo].[aspnet_Users] AS U
	ON U.UserId = E.UserID
WHERE E.Deleted = 0 AND E.Validated = 1

GO

CREATE UNIQUE CLUSTERED INDEX PK_View_ValidatedPhoneNumbers_Number ON [dbo].[USR_View_ValidatedPhoneNumbers]
(
	[PhoneNumber] ASC,
	[UserID] ASC,
	[NumberID] ASC,
	[UserName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO