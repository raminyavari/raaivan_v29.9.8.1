USE [EKM_App]
GO


IF NOT EXISTS(SELECT * FROM sys.fulltext_indexes
  where object_id = object_id('USR_HonorsAndAwards')) BEGIN
	CREATE FULLTEXT INDEX ON [dbo].[USR_HonorsAndAwards]
	( 
		Title Language Neutral,
		Issuer Language Neutral,
		Occupation Language Neutral,
		[Description] Language Neutral
	)
	KEY INDEX PK_USR_HonorsAndAwards
	ON [Default_Catalog]
	WITH CHANGE_TRACKING AUTO
END

GO

