USE [EKM_App]
GO

ALTER TABLE [dbo].[CN_Nodes]
ADD [ExpirationDate] DATETIME NULL
GO

ALTER TABLE [dbo].[CN_Nodes]
ADD [PublicDescription] nvarchar(max) NULL
GO

