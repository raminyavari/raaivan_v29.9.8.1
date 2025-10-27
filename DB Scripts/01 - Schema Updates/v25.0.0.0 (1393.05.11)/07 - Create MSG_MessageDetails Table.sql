USE [EKM_App]
GO


CREATE TABLE [dbo].[MSG_MessageDetails](
	[ID]					BIGINT IDENTITY(1,1) NOT NULL,
	[UserID]				UNIQUEIDENTIFIER  NOT NULL,
	[ThreadID]				UNIQUEIDENTIFIER  NOT NULL,
	[MessageID]				UNIQUEIDENTIFIER  NOT NULL,
	[Seen]					BIT NOT NULL,
	[ViewDate]				DATETIME NULL,
	[IsSender]				BIT NOT NULL,
	[IsGroup]				BIT NOT NULL,
	[Deleted]				BIT NOT NULL
	
	CONSTRAINT [PK_MSG_MessageDetails] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[MSG_MessageDetails] WITH CHECK ADD CONSTRAINT [FK_MSG_MessageDetails_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[MSG_MessageDetails] CHECK CONSTRAINT [FK_MSG_MessageDetails_aspnet_Users]
GO


ALTER TABLE [dbo].[MSG_MessageDetails] WITH CHECK ADD CONSTRAINT [FK_MSG_MessageDetails_MSG_Messages] FOREIGN KEY([MessageID])
REFERENCES [dbo].[MSG_Messages] ([MessageID])
GO

ALTER TABLE [dbo].[MSG_MessageDetails] CHECK CONSTRAINT [FK_MSG_MessageDetails_MSG_Messages]
GO

