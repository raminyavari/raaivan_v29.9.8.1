USE [EKM_App]
GO


IF EXISTS(select * FROM sys.views where name = 'Users_Normal')
DROP VIEW [dbo].[Users_Normal]
GO


ALTER TABLE [dbo].[USR_Profile]
DROP COLUMN [Phone], [Mobile], [Email]
GO


ALTER TABLE [dbo].[USR_Profile]
ADD [MainPhoneID] uniqueidentifier null,
	[MainEmailID] uniqueidentifier null,
	[EmploymentType] varchar(50) null,
	[Lang] varchar(20) null
GO


ALTER TABLE [dbo].[USR_Profile]  WITH CHECK ADD  CONSTRAINT [FK_USR_Profile_USR_EmailAddresses] FOREIGN KEY(MainEmailID)
REFERENCES [dbo].[USR_EmailAddresses] (EmailID)
GO

ALTER TABLE [dbo].[USR_Profile] CHECK CONSTRAINT [FK_USR_Profile_USR_EmailAddresses]
GO

ALTER TABLE [dbo].[USR_Profile]  WITH CHECK ADD  CONSTRAINT [FK_USR_Profile_USR_PhoneNumbers] FOREIGN KEY(MainPhoneID)
REFERENCES [dbo].[USR_PhoneNumbers] (NumberID)
GO

ALTER TABLE [dbo].[USR_Profile] CHECK CONSTRAINT [FK_USR_Profile_USR_PhoneNumbers]
GO