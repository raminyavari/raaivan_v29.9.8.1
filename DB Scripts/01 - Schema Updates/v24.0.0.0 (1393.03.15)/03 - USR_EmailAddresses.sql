USE [EKM_App]
GO


CREATE TABLE [dbo].[USR_EmailAddresses](
	[EmailID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[EmailAddress] [varchar] (100) NOT NULL,
	[CreatorUserID] [uniqueidentifier] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_USR_EmailAddresses] PRIMARY KEY CLUSTERED 
(
	[EmailID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[USR_EmailAddresses]  WITH CHECK ADD  CONSTRAINT [FK_USR_EmailAddresses_USR_Profile] FOREIGN KEY(UserID)
REFERENCES [dbo].[aspnet_Users] (UserId)
GO

ALTER TABLE [dbo].[USR_EmailAddresses] CHECK CONSTRAINT [FK_USR_EmailAddresses_USR_Profile]
GO

ALTER TABLE [dbo].[USR_EmailAddresses]  WITH CHECK ADD  CONSTRAINT [FK_USR_EmailAddresses_USR_Profile_Creator] FOREIGN KEY(CreatorUserID)
REFERENCES [dbo].[aspnet_Users] (UserId)
GO

ALTER TABLE [dbo].[USR_EmailAddresses] CHECK CONSTRAINT [FK_USR_EmailAddresses_USR_Profile_Creator]
GO

ALTER TABLE [dbo].[USR_EmailAddresses]  WITH CHECK ADD  CONSTRAINT [FK_USR_EmailAddresses_USR_Profile_Modifier] FOREIGN KEY(LastModifierUserID)
REFERENCES [dbo].[aspnet_Users] (UserId)
GO

ALTER TABLE [dbo].[USR_EmailAddresses] CHECK CONSTRAINT [FK_USR_EmailAddresses_USR_Profile_Modifier]
GO