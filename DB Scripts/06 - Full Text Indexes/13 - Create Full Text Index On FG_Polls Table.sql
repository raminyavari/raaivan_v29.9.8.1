USE [EKM_App]
GO


IF NOT EXISTS(SELECT * FROM sys.fulltext_indexes
  where object_id = object_id('FG_Polls')) BEGIN
	CREATE FULLTEXT INDEX ON [dbo].[FG_Polls]
	( 
		Name Language Neutral
	)
	KEY INDEX PK_FG_Polls
	ON [Default_Catalog]
	WITH CHANGE_TRACKING AUTO
END

GO

