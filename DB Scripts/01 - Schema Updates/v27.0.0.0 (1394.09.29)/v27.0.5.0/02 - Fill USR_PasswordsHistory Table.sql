USE [EKM_App]
GO


INSERT INTO [dbo].[USR_PasswordsHistory](UserID, [Password], SetDate)
SELECT UserId, [Password], CreateDate
FROM [dbo].[aspnet_Membership]

GO


