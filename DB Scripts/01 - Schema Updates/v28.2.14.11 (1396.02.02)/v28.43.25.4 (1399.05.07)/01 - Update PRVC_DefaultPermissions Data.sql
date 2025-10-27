USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


DELETE [dbo].[PRVC_DefaultPermissions]
WHERE DefaultValue NOT IN (N'Public', N'Restricted')

GO