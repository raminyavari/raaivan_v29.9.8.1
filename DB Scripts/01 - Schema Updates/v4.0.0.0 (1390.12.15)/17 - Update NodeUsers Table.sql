USE [EKM_App]
GO

/****** Object:  Table [dbo].[NodeUsers]    Script Date: 02/17/2012 14:59:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[NodeUsersTemp](
	[NodeUsersID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NOT NULL,
	[UserType] [nvarchar](max) NULL,
	[Date] [datetime] NULL,
	[Position] [nvarchar](max) NULL,
 CONSTRAINT [PK_NodeUsersTemp] PRIMARY KEY CLUSTERED 
(
	[NodeUsersID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[NodeUsersTemp]
           ([NodeUsersID]
			,[UserID]
			,[NodeID]
			,[UserType]
			,[Date]
			,[Position])
		SELECT  dbo.NodeUsers.NodeUsersID, dbo.NodeUsers.UserID, dbo.CN_Nodes.NodeID, dbo.NodeUsers.UserType, 
				dbo.NodeUsers.Date, dbo.NodeUsers.Position
		FROM    dbo.CN_Nodes INNER JOIN dbo.NodeUsers ON dbo.CN_Nodes.ID = dbo.NodeUsers.NodeID
GO


/* Drop old table */
DROP TABLE [dbo].[NodeUsers]
GO

/* Create old new table */
CREATE TABLE [dbo].[NodeUsers](
	[NodeUsersID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NOT NULL,
	[UserType] [nvarchar](max) NULL,
	[Date] [datetime] NULL,
	[Position] [nvarchar](max) NULL,
 CONSTRAINT [PK_NodeUsers] PRIMARY KEY CLUSTERED 
(
	[NodeUsersID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[NodeUsers]  WITH CHECK ADD  CONSTRAINT [FK_NodeUsers_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[NodeUsers] CHECK CONSTRAINT [FK_NodeUsers_aspnet_Users]
GO

ALTER TABLE [dbo].[NodeUsers]  WITH CHECK ADD  CONSTRAINT [FK_NodeUsers_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[NodeUsers] CHECK CONSTRAINT [FK_NodeUsers_CN_Nodes]
GO


INSERT INTO [dbo].[NodeUsers]
	SELECT * FROM dbo.NodeUsersTemp
GO

DROP TABLE [dbo].[NodeUsersTemp]
GO