USE [EKM_App]
GO

IF NOT EXISTS(SELECT * FROM sys.fulltext_indexes
  where object_id = object_id('CN_Lists')) BEGIN
	CREATE FULLTEXT INDEX ON [dbo].[CN_Lists]
	( 
		Name Language Neutral,
		[Description] Language Neutral
	)
	KEY INDEX PK_CN_Lists
	ON [Default_Catalog]
	WITH CHANGE_TRACKING AUTO
END

GO

