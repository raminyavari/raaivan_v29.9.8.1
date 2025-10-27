USE [EKM_App]
GO

IF NOT EXISTS(SELECT * FROM sys.fulltext_indexes
  where object_id = object_id('CN_Tags')) BEGIN
	CREATE FULLTEXT INDEX ON [dbo].[CN_Tags]
	( 
		Tag Language Neutral
	)
	KEY INDEX PK_CN_Tags
	ON [Default_Catalog]
	WITH CHANGE_TRACKING AUTO
END

GO

