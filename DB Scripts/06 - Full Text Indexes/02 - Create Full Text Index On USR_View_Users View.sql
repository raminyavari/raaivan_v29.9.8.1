USE [EKM_App]
GO

IF NOT EXISTS(SELECT * FROM sys.fulltext_indexes
  where object_id = object_id('USR_View_Users')) BEGIN
	CREATE FULLTEXT INDEX ON [dbo].[USR_View_Users]
	( 
		UserName Language Neutral,
		FirstName Language Neutral,
		Lastname Language Neutral
	)
	KEY INDEX PK_USR_View_Users_UserID
	ON [Default_Catalog]
	WITH CHANGE_TRACKING AUTO
END

GO

