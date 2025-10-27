USE [EKM_App]
GO



INSERT INTO [dbo].[QA_RelatedNodes] (
	ApplicationID,
	NodeID,
	QuestionID,
	CreatorUserID,
	CreationDate,
	Deleted
)
SELECT	N.ApplicationID, 
		N.NodeID, 
		N.QuestionID, 
		(
			SELECT TOP(1) UserId
			FROM [dbo].[aspnet_Users]
			WHERE ApplicationID = N.ApplicationID AND LoweredUserName = N'admin'
		), 
		N.SendDate, 
		0
FROM [dbo].[QA_RefNodes] AS N

GO