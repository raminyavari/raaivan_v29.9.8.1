USE [EKM_App]
GO


ALTER TABLE [dbo].[DCT_Trees]
ADD [SequenceNumber] int NULL
GO

ALTER TABLE [dbo].[DCT_TreeNodes]
ADD [SequenceNumber] int NULL
GO