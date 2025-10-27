USE [EKM_App]
GO

/****** Object:  Table [dbo].[UserConnections]    Script Date: 03/11/2012 11:57:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserConnectionsTemp](
	[SenderUserID] [uniqueidentifier] NOT NULL,
	[ReceiverUserID] [uniqueidentifier] NOT NULL,
	[RequestDate] [datetime] NOT NULL,
	[AcceptionDate] [datetime] NULL,
	[Status] [nvarchar](255) NULL,
 CONSTRAINT [PK_UserConnectionsTemp] PRIMARY KEY CLUSTERED 
(
	[SenderUserID] ASC,
	[ReceiverUserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[UserConnectionsTemp]
           ([SenderUserID]
			,[ReceiverUserID]
			,[RequestDate]
			,[Status])
		SELECT  dbo.UserConnections.UserIdFrom, dbo.UserConnections.UserIdTo,
				dbo.UserConnections.Date, dbo.UserConnections.Status
		FROM    dbo.UserConnections
GO


UPDATE [dbo].[UserConnectionsTemp]
   SET [AcceptionDate] = [RequestDate]
 WHERE [Status] = 'Accept'
GO


/* Drop old table */
DROP TABLE [dbo].[UserConnections]
GO


/* Create old new table */
CREATE TABLE [dbo].[UserConnections](
	[SenderUserID] [uniqueidentifier] NOT NULL,
	[ReceiverUserID] [uniqueidentifier] NOT NULL,
	[RequestDate] [datetime] NOT NULL,
	[AcceptionDate] [datetime] NULL,
	[Status] [nvarchar](255) NULL,
 CONSTRAINT [PK_UserConnections] PRIMARY KEY CLUSTERED 
(
	[SenderUserID] ASC,
	[ReceiverUserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[UserConnections]  WITH CHECK ADD  CONSTRAINT [FK_UserConnections_ProfileCommon] FOREIGN KEY([SenderUserID])
REFERENCES [dbo].[ProfileCommon] ([UserId])
GO

ALTER TABLE [dbo].[UserConnections] CHECK CONSTRAINT [FK_UserConnections_ProfileCommon]
GO

ALTER TABLE [dbo].[UserConnections]  WITH CHECK ADD  CONSTRAINT [FK_UserConnections_ProfileCommon_Receiver] FOREIGN KEY([ReceiverUserID])
REFERENCES [dbo].[ProfileCommon] ([UserId])
GO

ALTER TABLE [dbo].[UserConnections] CHECK CONSTRAINT [FK_UserConnections_ProfileCommon_Receiver]
GO


INSERT INTO [dbo].[UserConnections]
SELECT * FROM dbo.UserConnectionsTemp
GO

DROP TABLE [dbo].[UserConnectionsTemp]
GO

