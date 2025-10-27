USE [EKM_App]
GO


INSERT INTO [dbo].[LG_Logs](
	UserID,
	[Action],
	[Date],
	Info,
	ModuleIdentifier
)
SELECT UserId, N'Search', [Date], SearchText, N'SRCH'
FROM [dbo].[UserSearchLogs]

GO