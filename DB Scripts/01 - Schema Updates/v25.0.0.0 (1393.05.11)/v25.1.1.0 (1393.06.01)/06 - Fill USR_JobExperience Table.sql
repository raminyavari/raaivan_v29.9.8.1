USE [EKM_App]
GO


INSERT INTO [dbo].[USR_JobExperiences](
	JobID,
	UserID,
	Title,
	Employer,
	StartDate,
	EndDate,
	CreatorUserID,
	CreationDate,	
	Deleted
)
SELECT
	pj.ID,
	pj.UserID,
	pj.Title,
	pj.Employer,
	pj.StartDate,
	pj.EndDate,
	pj.UserID,
	GETDATE(),
	0
FROM [dbo].[ProfileJobs] AS pj

GO