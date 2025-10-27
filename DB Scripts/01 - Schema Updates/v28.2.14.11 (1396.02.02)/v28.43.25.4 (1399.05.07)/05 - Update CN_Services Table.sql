USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER TABLE [dbo].[CN_Services]
ADD EnablePreviousVersionSelect bit NULL
GO


UPDATE [dbo].[CN_Services]
	SET EnablePreviousVersionSelect = IsDocument
GO
