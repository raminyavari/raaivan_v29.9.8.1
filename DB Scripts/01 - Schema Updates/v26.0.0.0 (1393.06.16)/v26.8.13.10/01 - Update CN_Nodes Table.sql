USE [EKM_App]
GO

SET ANSI_PADDING ON
GO

ALTER TABLE [dbo].[CN_Nodes]
ADD [WFState] nvarchar(1000) null

GO
