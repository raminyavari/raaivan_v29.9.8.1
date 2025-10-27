USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER TABLE [dbo].[FG_ExtendedFormElements]
ADD InitialValue nvarchar(max) NULL
GO
