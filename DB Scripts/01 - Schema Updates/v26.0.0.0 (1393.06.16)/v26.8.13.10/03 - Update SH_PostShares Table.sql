USE [EKM_App]
GO

SET ANSI_PADDING ON
GO

ALTER TABLE [dbo].[SH_PostShares]
ADD [SendAs] uniqueidentifier null

GO
