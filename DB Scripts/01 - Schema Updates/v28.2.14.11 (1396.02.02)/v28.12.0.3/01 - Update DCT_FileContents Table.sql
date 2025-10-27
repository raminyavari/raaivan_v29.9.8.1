USE [EKM_App]
GO


DELETE [dbo].[DCT_FileContents]
WHERE FileNotFound = 1

GO