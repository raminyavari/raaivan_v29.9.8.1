USE [EKM_App]
GO

-- create new table
CREATE TABLE [dbo].[USR_HonorsAndAwards](
	ID				UNIQUEIDENTIFIER NOT NULL,
	AdditionalID	NVARCHAR(50) NULL,
	UserID			UNIQUEIDENTIFIER NOT NULL,
	Title			NVARCHAR(512) NOT NULL,
	Issuer			NVARCHAR(512) NOT NULL,
	Occupation		NVARCHAR(512) NOT NULL,
	IssueDate		DATETIME NULL,
	[Description]	NVARCHAR(MAX) NULL,
	CreatorUserID	UNIQUEIDENTIFIER NOT NULL,
	CreationDate	DATETIME NOT NULL,
	Deleted			BIT NOT NULL
	
	CONSTRAINT [PK_USR_HonorsAndAwards] PRIMARY KEY CLUSTERED
	(
		ID ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER TABLE [dbo].[USR_HonorsAndAwards] WITH CHECK ADD  CONSTRAINT [FK_USR_HonorsAndAwards_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserID])
GO

ALTER TABLE [dbo].[USR_HonorsAndAwards] CHECK CONSTRAINT [FK_USR_HonorsAndAwards_aspnet_Users]
GO


ALTER TABLE [dbo].[USR_HonorsAndAwards] WITH CHECK ADD  CONSTRAINT [FK_USR_HonorsAndAwards_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserID])
GO

ALTER TABLE [dbo].[USR_HonorsAndAwards] CHECK CONSTRAINT [FK_USR_HonorsAndAwards_aspnet_Users_Creator]
GO