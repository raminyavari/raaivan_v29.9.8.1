USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER TABLE [dbo].[USR_EmailAddresses]
ADD Validated bit NULL
GO

ALTER TABLE [dbo].[USR_EmailAddresses]
DROP CONSTRAINT [FK_USR_EmailAddresses_aspnet_Applications]
GO

ALTER TABLE [dbo].[USR_EmailAddresses]
DROP COLUMN ApplicationID
GO


ALTER TABLE [dbo].[USR_PhoneNumbers]
ADD Validated bit NULL
GO

ALTER TABLE [dbo].[USR_PhoneNumbers]
DROP CONSTRAINT [FK_USR_PhoneNumbers_aspnet_Applications]
GO

ALTER TABLE [dbo].[USR_PhoneNumbers]
DROP COLUMN ApplicationID
GO