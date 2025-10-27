USE [EKM_App]
GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[DE_UpdateMembers]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DE_UpdateMembers]
GO

IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'ExchangeMemberTableType')
DROP TYPE dbo.ExchangeMemberTableType
GO