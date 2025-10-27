USE [EKM_App]
GO

IF NOT EXISTS(SELECT * FROM sys.fulltext_indexes
  where object_id = object_id('CN_Nodes')) BEGIN
	CREATE FULLTEXT INDEX ON [dbo].[CN_Nodes]
	( 
		Name Language Neutral,
		AdditionalID Language Neutral,
		[Description] Language Neutral,
		Tags Language Neutral
	)
	KEY INDEX PK_Nodes_NodeID
	ON [Default_Catalog]
	WITH CHANGE_TRACKING AUTO
END

GO

