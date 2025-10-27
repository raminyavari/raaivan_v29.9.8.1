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
