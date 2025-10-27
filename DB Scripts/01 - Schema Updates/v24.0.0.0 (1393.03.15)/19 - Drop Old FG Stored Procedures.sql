USE [EKM_App]
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[FG_SaveFormInstanceElements]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[FG_SaveFormInstanceElements]
GO

DROP TYPE [dbo].[FormElementTableType]
GO