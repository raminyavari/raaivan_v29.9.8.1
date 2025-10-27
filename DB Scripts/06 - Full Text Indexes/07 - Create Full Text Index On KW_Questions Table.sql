USE [EKM_App]
GO


IF NOT EXISTS(SELECT * FROM sys.fulltext_indexes
  where object_id = object_id('KW_Questions')) BEGIN
	CREATE FULLTEXT INDEX ON [dbo].[KW_Questions]
	( 
		Title Language Neutral
	)
	KEY INDEX PK_TMP_KW_Questions
	ON [Default_Catalog]
	WITH CHANGE_TRACKING AUTO
END

GO

