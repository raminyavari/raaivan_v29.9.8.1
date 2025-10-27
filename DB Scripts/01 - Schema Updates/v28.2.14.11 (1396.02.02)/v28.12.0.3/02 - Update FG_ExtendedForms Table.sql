USE [EKM_App]
GO


ALTER TABLE [dbo].[FG_ExtendedForms]
ADD Name varchar(100) NULL
GO

ALTER TABLE [dbo].[FG_ExtendedFormElements]
ADD Name varchar(100) NULL
GO