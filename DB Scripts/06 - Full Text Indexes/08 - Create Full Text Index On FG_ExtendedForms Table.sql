USE [EKM_App]
GO


IF NOT EXISTS(SELECT * FROM sys.fulltext_indexes
  where object_id = object_id('FG_ExtendedForms')) BEGIN
	CREATE FULLTEXT INDEX ON [dbo].[FG_ExtendedForms]
	( 
		Title Language Neutral
	)
	KEY INDEX PK_FG_ExtendedForms
	ON [Default_Catalog]
	WITH CHANGE_TRACKING AUTO
END

GO

