USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


INSERT INTO [dbo].[CN_Properties]
           ([PropertyID]
			,[AdditionalID]
			,[Name]
			,[Deleted]
			,[ID])
     SELECT NEWID(), dbo.ConnectionType.ID, dbo.ConnectionType.ConnectionType, 0, dbo.ConnectionType.ID
	 FROM   dbo.ConnectionType
GO

