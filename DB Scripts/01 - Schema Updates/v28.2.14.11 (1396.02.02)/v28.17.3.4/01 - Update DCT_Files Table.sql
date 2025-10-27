USE [EKM_App]
GO


UPDATE [dbo].[DCT_Files]
	SET Size = Size * 1024
WHERE OwnerType = N'Message' OR OwnerType = N'Wiki' OR OwnerType = N'WorkFlow'

GO