USE [EKM_App]
GO

UPDATE M
	SET [Password] = 'hzpljiwy35CCLSmxIuTk49mhDI4=',
		[PasswordSalt] = '6rG7hIBmkJ6cfestS4Ycow==',
		PasswordFormat = 1
FROM [dbo].[aspnet_Membership] AS M
	INNER JOIN [dbo].[Users_Normal] AS U
	ON U.UserID = M.UserId
WHERE LOWER(U.UserName) = N'admin'

GO