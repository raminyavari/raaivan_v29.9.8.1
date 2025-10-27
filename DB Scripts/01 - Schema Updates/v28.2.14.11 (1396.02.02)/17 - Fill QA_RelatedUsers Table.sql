USE [EKM_App]
GO



INSERT INTO [dbo].[QA_RelatedUsers] (
	ApplicationID,
	UserID,
	QuestionID,
	SenderUserID,
	SendDate,
	Seen,
	Deleted
)
SELECT	U.ApplicationID, 
		U.UserID, 
		U.QuestionID, 
		(
			SELECT TOP(1) UserId
			FROM [dbo].[aspnet_Users]
			WHERE ApplicationID = U.ApplicationID AND LoweredUserName = N'admin'
		), 
		U.SendDate, 
		U.Seen,
		0
FROM [dbo].[QA_RefUsers] AS U

GO