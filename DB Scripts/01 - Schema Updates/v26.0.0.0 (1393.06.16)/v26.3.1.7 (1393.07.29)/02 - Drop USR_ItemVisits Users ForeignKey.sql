USE [EKM_App]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_USR_ItemVisits_aspnet_Users]') AND parent_object_id = OBJECT_ID(N'[dbo].[USR_ItemVisits]'))
ALTER TABLE [dbo].[USR_ItemVisits] DROP CONSTRAINT [FK_USR_ItemVisits_aspnet_Users]
GO


