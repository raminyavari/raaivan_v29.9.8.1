USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[USR_RemoteServers](
	[ServerID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[URL] [nvarchar](100) NOT NULL,
	[UserName] [nvarchar](100) NOT NULL,
	[Password] [varbinary](100) NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastModificationDate] [datetime] NULL,
	[ApplicationID] [uniqueidentifier] NULL
 CONSTRAINT [PK_USR_RemoteServers] PRIMARY KEY CLUSTERED 
(
	[ServerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[USR_RemoteServers]  WITH CHECK ADD  CONSTRAINT [FK_USR_RemoteServers_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[USR_RemoteServers] CHECK CONSTRAINT [FK_USR_RemoteServers_aspnet_Applications]
GO

ALTER TABLE [dbo].[USR_RemoteServers]  WITH CHECK ADD  CONSTRAINT [FK_USR_RemoteServers_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[USR_RemoteServers] CHECK CONSTRAINT [FK_USR_RemoteServers_aspnet_Users]
GO
