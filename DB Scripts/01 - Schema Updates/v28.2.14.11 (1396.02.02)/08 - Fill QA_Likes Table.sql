USE [EKM_App]
GO


INSERT INTO [dbo].[RV_Likes] (
	UserID,
	LikedID,
	[Like],
	ActionDate,
	ApplicationID
)
SELECT UserID, QuestionID, [Like], [Date], [ApplicationID]
FROM [dbo].[QA_QuestionLikes]

GO