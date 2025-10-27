USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TABLE [dbo].[USR_EmailContacts](
	[UserID] [uniqueidentifier] NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_USR_EmailContacts] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[Email] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[USR_EmailContacts]  WITH CHECK ADD  CONSTRAINT [FK_USR_EmailContacts_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[USR_EmailContacts] CHECK CONSTRAINT [FK_USR_EmailContacts_aspnet_Users]
GO



SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TABLE [dbo].[RV_EmailQueue](
	[ID] [bigint] IDENTITY(1, 1) NOT NULL,
	[SenderUserID] [uniqueidentifier] NULL,
	[Action] [varchar](50) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[Title] [nvarchar](1000) NULL,
	[EmailBody] [nvarchar](max) NOT NULL
 CONSTRAINT [PK_RV_EmailQueue] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO




SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TABLE [dbo].[RV_SentEmails](
	[ID] [bigint] IDENTITY(1, 1) NOT NULL,
	[SenderUserID] [uniqueidentifier] NULL,
	[Action] [varchar](50) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[Title] [nvarchar](1000) NULL,
	[EmailBody] [nvarchar](max) NOT NULL,
	[SendDate] datetime NOT NULL
 CONSTRAINT [PK_RV_SentEmails] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO




/****** Object:  Table [dbo].[KKnowledges]    Script Date: 04/04/2012 12:34:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



UPDATE [dbo].[AppSetting]
	SET [Version] = 'v26.10.1.2' -- 13940430
GO

