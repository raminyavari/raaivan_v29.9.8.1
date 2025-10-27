USE [EKM_App]
GO


IF NOT EXISTS(SELECT * FROM sys.fulltext_indexes
  where object_id = object_id('USR_JobExperiences')) BEGIN
	CREATE FULLTEXT INDEX ON [dbo].[USR_JobExperiences]
	( 
		Title Language Neutral,
		Employer Language Neutral
	)
	KEY INDEX PK_USR_JobExperiences
	ON [Default_Catalog]
	WITH CHANGE_TRACKING AUTO
END

GO

