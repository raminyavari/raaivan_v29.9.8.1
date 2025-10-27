USE [EKM_App]
GO


CREATE TABLE [dbo].[USR_TemporaryUsers](
	[UserID]			UNIQUEIDENTIFIER NOT NULL,
	[UserName]			NVARCHAR (255) NOT NULL,
	[FirstName]			NVARCHAR (255) NULL,
	[LastName]			NVARCHAR (255) NULL,
	[Password]			NVARCHAR (255) NULL,
	[PasswordSalt]		NVARCHAR (255) NULL,
	[EMail]				NVARCHAR (255) NULL,
	[CreationDate]		DATETIME NOT NULL,
	[ExpirationDate]	DATETIME NULL,
	[ActivationCode]	VARCHAR (255) NULL
 CONSTRAINT [PK_USR_TemporaryUsers] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



CREATE TABLE [dbo].[USR_Invitations](
	[ID]			UNIQUEIDENTIFIER NOT NULL,
	[Email]			NVARCHAR (255) NOT NULL,
	[SenderUserID]	UNIQUEIDENTIFIER NOT NULL,
	[SendDate]		DATETIME NOT NULL,
	[CreatedUserID] UNIQUEIDENTIFIER NULL
 CONSTRAINT [PK_USR_Invitations] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER TABLE [dbo].[USR_Invitations]  WITH CHECK ADD  CONSTRAINT [FK_USR_Invitations_aspnet_Users_Sender] FOREIGN KEY([SenderUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[USR_Invitations] CHECK CONSTRAINT [FK_USR_Invitations_aspnet_Users_Sender]
GO


ALTER TABLE [dbo].[USR_Invitations]  WITH CHECK ADD  CONSTRAINT [FK_USR_Invitations_aspnet_Users_Created] FOREIGN KEY([CreatedUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[USR_Invitations] CHECK CONSTRAINT [FK_USR_Invitations_aspnet_Users_Created]
GO


/****** Object:  Table [dbo].[KKnowledges]    Script Date: 04/04/2012 12:34:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



UPDATE [dbo].[AppSetting]
	SET [Version] = 'v26.1.0.2' -- 13930616
GO

