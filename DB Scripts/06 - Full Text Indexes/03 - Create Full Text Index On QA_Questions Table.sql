USE [EKM_App]
GO

IF NOT EXISTS(SELECT * FROM sys.fulltext_indexes
  where object_id = object_id('QA_Questions')) BEGIN
	CREATE FULLTEXT INDEX ON [dbo].[QA_Questions]
	( 
		Title Language Neutral,
		[Description] Language Neutral
	)
	KEY INDEX PK_QA_Questions
	ON [Default_Catalog]
	WITH CHANGE_TRACKING AUTO
END

GO

