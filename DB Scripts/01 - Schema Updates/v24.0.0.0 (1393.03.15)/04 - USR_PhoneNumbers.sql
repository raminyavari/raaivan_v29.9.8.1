USE [EKM_App]
GO


CREATE TABLE [dbo].[USR_PhoneNumbers](
	[NumberID]				UNIQUEIDENTIFIER NOT NULL,
	[UserID]				UNIQUEIDENTIFIER NOT NULL,
	[PhoneNumber]			VARCHAR (50) NOT NULL,
	[PhoneType]				VARCHAR	(20) NOT NULL,
	[CreatorUserID]			UNIQUEIDENTIFIER NOT NULL,
	[CreationDate]			DATETIME NOT NULL,
	[LastModifierUserID]	UNIQUEIDENTIFIER NULL,
	[LastModificationDate]	DATETIME NULL,
	[Deleted]				BIT NOT NULL
 CONSTRAINT [PK_USR_PhoneNumbers] PRIMARY KEY CLUSTERED 
(
	[NumberID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[USR_PhoneNumbers]  WITH CHECK ADD  CONSTRAINT [FK_USR_PhoneNumbers_USR_Profile] FOREIGN KEY(UserID)
REFERENCES [dbo].[aspnet_Users] (UserId)
GO

ALTER TABLE [dbo].[USR_PhoneNumbers] CHECK CONSTRAINT [FK_USR_PhoneNumbers_USR_Profile]
GO

ALTER TABLE [dbo].[USR_PhoneNumbers]  WITH CHECK ADD  CONSTRAINT [FK_USR_PhoneNumbers_USR_Profile_Creator] FOREIGN KEY(CreatorUserID)
REFERENCES [dbo].[aspnet_Users] (UserId)
GO

ALTER TABLE [dbo].[USR_PhoneNumbers] CHECK CONSTRAINT [FK_USR_PhoneNumbers_USR_Profile_Creator]
GO

ALTER TABLE [dbo].[USR_PhoneNumbers]  WITH CHECK ADD  CONSTRAINT [FK_USR_PhoneNumbers_USR_Profile_Modifier] FOREIGN KEY(LastModifierUserID)
REFERENCES [dbo].[aspnet_Users] (UserId)
GO

ALTER TABLE [dbo].[USR_PhoneNumbers] CHECK CONSTRAINT [FK_USR_PhoneNumbers_USR_Profile_Modifier]
GO