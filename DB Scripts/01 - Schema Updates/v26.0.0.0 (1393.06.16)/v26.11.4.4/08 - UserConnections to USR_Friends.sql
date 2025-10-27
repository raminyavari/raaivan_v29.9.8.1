USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(select * FROM sys.views where name = 'USR_View_Friends')
DROP VIEW [dbo].[USR_View_Friends]
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[USR_Friends](
	[SenderUserID] [uniqueidentifier] NOT NULL,
	[ReceiverUserID] [uniqueidentifier] NOT NULL,
	[AreFriends] [bit] NOT NULL,
	[RequestDate] [datetime] NOT NULL,
	[AcceptionDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
	[UniqueID] [uniqueidentifier] NOT NULL
 CONSTRAINT [PK_USR_Friends] PRIMARY KEY CLUSTERED 
(
	[SenderUserID] ASC,
	[ReceiverUserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[USR_Friends]  WITH CHECK ADD  CONSTRAINT [FK_USR_Friends_aspnet_Users] FOREIGN KEY([SenderUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[USR_Friends] CHECK CONSTRAINT [FK_USR_Friends_aspnet_Users]
GO

ALTER TABLE [dbo].[USR_Friends]  WITH CHECK ADD  CONSTRAINT [FK_USR_Friends_aspnet_Users_Receiver] FOREIGN KEY([ReceiverUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[USR_Friends] CHECK CONSTRAINT [FK_USR_Friends_aspnet_Users_Receiver]
GO


INSERT INTO [dbo].[USR_Friends](
	SenderUserID,
	ReceiverUserID,
	AreFriends,
	RequestDate,
	AcceptionDate,
	Deleted,
	UniqueID
)
SELECT	SenderUserID, 
		ReceiverUserID,
		(CASE WHEN UC.[Status] = N'Accept' THEN 1 ELSE 0 END),
		RequestDate,
		AcceptionDate,
		0,
		NEWID()
FROM [dbo].[UserConnections] AS UC

GO


DROP TABLE [dbo].[UserConnections]
GO

