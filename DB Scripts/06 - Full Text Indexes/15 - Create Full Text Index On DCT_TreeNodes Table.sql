USE [EKM_App]
GO

IF NOT EXISTS(SELECT * FROM sys.fulltext_indexes
  where object_id = object_id('DCT_TreeNodes')) BEGIN
	CREATE FULLTEXT INDEX ON [dbo].[DCT_TreeNodes]
	( 
		Name Language Neutral
	)
	KEY INDEX PK_DCT_TreeNodes
	ON [Default_Catalog]
	WITH CHANGE_TRACKING AUTO
END

GO

