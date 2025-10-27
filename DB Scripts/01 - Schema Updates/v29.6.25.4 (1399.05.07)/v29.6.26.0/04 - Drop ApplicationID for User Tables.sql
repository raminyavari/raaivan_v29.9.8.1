USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER TABLE [dbo].[aspnet_Users]
DROP COLUMN [ApplicationID]
GO


ALTER TABLE [dbo].[aspnet_Membership]
DROP CONSTRAINT [FK__aspnet_Me__Appli__009508B4]
GO

ALTER TABLE [dbo].[aspnet_Membership]
DROP COLUMN [ApplicationID]
GO


ALTER TABLE [dbo].[USR_Profile]
DROP CONSTRAINT [FK_USR_Profile_aspnet_Applications]
GO

ALTER TABLE [dbo].[USR_Profile]
DROP COLUMN [ApplicationID]
GO



ALTER TABLE [dbo].[USR_EmailContacts]
DROP CONSTRAINT [FK_USR_EmailContacts_aspnet_Applications]
GO

ALTER TABLE [dbo].[USR_EmailContacts]
DROP COLUMN [ApplicationID]
GO


ALTER TABLE [dbo].[USR_PassResetTickets]
DROP CONSTRAINT [FK_USR_PassResetTickets_aspnet_Applications]
GO

ALTER TABLE [dbo].[USR_PassResetTickets]
DROP COLUMN [ApplicationID]
GO


ALTER TABLE [dbo].[USR_PasswordsHistory]
DROP CONSTRAINT [FK_USR_PasswordsHistory_aspnet_Applications]
GO

ALTER TABLE [dbo].[USR_PasswordsHistory]
DROP COLUMN [ApplicationID]
GO


ALTER TABLE [dbo].[USR_TemporaryUsers]
DROP CONSTRAINT [FK_USR_TemporaryUsers_aspnet_Applications]
GO

ALTER TABLE [dbo].[USR_TemporaryUsers]
DROP COLUMN [ApplicationID]
GO
