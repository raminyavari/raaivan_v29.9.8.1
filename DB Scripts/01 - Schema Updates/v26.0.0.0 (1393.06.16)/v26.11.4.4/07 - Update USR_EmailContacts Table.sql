USE [EKM_App]
GO

/****** Object:  Table [dbo].[USR_EmailContacts]    Script Date: 08/01/2015 20:15:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[USR_TMPEmailContacts](
	[UserID] [uniqueidentifier] NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_USR_TMPEmailContacts] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[Email] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[USR_TMPEmailContacts](
	UserID,
	Email,
	CreationDate,
	Deleted
)
SELECT UserID, Email, CreationDate, Deleted
FROM [dbo].[USR_EmailContacts]

GO


DROP TABLE [dbo].[USR_EmailContacts]
GO


CREATE TABLE [dbo].[USR_EmailContacts](
	[UserID] [uniqueidentifier] NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[UniqueID] [uniqueidentifier] NOT NULL
 CONSTRAINT [PK_USR_EmailContacts] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[Email] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[USR_EmailContacts](
	UserID,
	Email,
	CreationDate,
	Deleted,
	UniqueID
)
SELECT UserID, Email, CreationDate, Deleted, NEWID()
FROM [dbo].[USR_TMPEmailContacts]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[USR_EmailContacts]  WITH CHECK ADD  CONSTRAINT [FK_USR_EmailContacts_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[USR_EmailContacts] CHECK CONSTRAINT [FK_USR_EmailContacts_aspnet_Users]
GO


DROP TABLE [dbo].[USR_TMPEmailContacts]
GO