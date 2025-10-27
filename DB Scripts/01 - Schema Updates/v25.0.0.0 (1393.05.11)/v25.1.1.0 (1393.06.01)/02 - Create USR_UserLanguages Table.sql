USE [EKM_App]
GO

CREATE TABLE [dbo].[USR_UserLanguages](
	ID				UNIQUEIDENTIFIER NOT NULL,
	AdditionalID	NVARCHAR(50) NULL,
	LanguageID		UNIQUEIDENTIFIER NOT NULL,
	UserID			UNIQUEIDENTIFIER NOT NULL,
	[Level]			VARCHAR(50) NOT NULL,
	CreatorUserID	UNIQUEIDENTIFIER NOT NULL,
	CreationDate	DATETIME NOT NULL,
	Deleted			BIT NOT NULL
	
	CONSTRAINT [PK_USR_UserLanguages] PRIMARY KEY CLUSTERED
	(
		[ID] ASC 
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER TABLE [dbo].[USR_UserLanguages]  WITH CHECK ADD  CONSTRAINT [FK_USR_UserLanguages_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserID])
GO

ALTER TABLE [dbo].[USR_UserLanguages] CHECK CONSTRAINT [FK_USR_UserLanguages_aspnet_Users]
GO


ALTER TABLE [dbo].[USR_UserLanguages]  WITH CHECK ADD  CONSTRAINT [FK_USR_UserLanguages_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserID])
GO

ALTER TABLE [dbo].[USR_UserLanguages] CHECK CONSTRAINT [FK_USR_UserLanguages_aspnet_Users_Creator]
GO


ALTER TABLE [dbo].[USR_UserLanguages]  WITH CHECK ADD  CONSTRAINT [FK_USR_UserLanguages_USR_LanguageNames] FOREIGN KEY([LanguageID])
REFERENCES [dbo].[USR_LanguageNames] ([LanguageID])
GO

ALTER TABLE [dbo].[USR_UserLanguages] CHECK CONSTRAINT [FK_USR_UserLanguages_USR_LanguageNames]
GO