USE [EKM_App]
GO


UPDATE [dbo].[Attachments]
	SET ObjectType = N'Node'
WHERE ObjectType = N'Knowledge'

GO