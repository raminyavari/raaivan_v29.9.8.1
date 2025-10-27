USE [EKM_App]
GO

SET NUMERIC_ROUNDABORT OFF;
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT,
    QUOTED_IDENTIFIER, ANSI_NULLS ON;


IF EXISTS(select * FROM sys.views where name = 'USR_View_Friends')
DROP VIEW [dbo].[USR_View_Friends]
GO


CREATE VIEW [dbo].[USR_View_Friends] WITH SCHEMABINDING, ENCRYPTION
AS
SELECT	F.ApplicationID,
		UN.UserID,
		CASE 
			WHEN UN.UserID = F.SenderUserID THEN F.ReceiverUserID 
			ELSE F.SenderUserID 
		END AS FriendID,
		CAST((CASE WHEN UN.UserID = F.SenderUserID THEN 1 ELSE 0 END) AS bit) AS IsSender,
		F.RequestDate,
		F.AcceptionDate,
		F.AreFriends
FROM [dbo].[aspnet_Users] AS UN
	INNER JOIN [dbo].[USR_Friends] AS F
	ON (F.SenderUserID = UN.UserID OR F.ReceiverUserID = UN.UserID)
WHERE F.Deleted = 0

GO


CREATE UNIQUE CLUSTERED INDEX PK_USR_View_Friends_UserID ON [dbo].[USR_View_Friends]
(
	[ApplicationID] ASC,
	[UserID] ASC,
	[FriendID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

GO
