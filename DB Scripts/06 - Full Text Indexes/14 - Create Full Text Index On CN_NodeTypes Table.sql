USE [EKM_App]
GO

IF NOT EXISTS(SELECT * FROM sys.fulltext_indexes
  where object_id = object_id('CN_NodeTypes')) BEGIN
	CREATE FULLTEXT INDEX ON [dbo].[CN_NodeTypes]
	( 
		Name Language Neutral
	)
	KEY INDEX PK_NodeTypes_NodeTypeID
	ON [Default_Catalog]
	WITH CHANGE_TRACKING AUTO
END

GO

