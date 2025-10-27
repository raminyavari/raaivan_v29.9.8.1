USE [EKM_App]
GO

/****** Object:  Table [dbo].[QQuestion_Users]    Script Date: 03/02/2012 18:00:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[QuestionUsersTemp](
	[QuestionID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[Seen] [bit] NOT NULL,
 CONSTRAINT [PK_QuestionUsersTemp] PRIMARY KEY CLUSTERED 
(
	[QuestionID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[QuestionUsersTemp]
           ([QuestionID]
			,[UserID]
			,[CreationDate]
			,[Seen])
		SELECT  dbo.QQuestion_Users.QuestionId, dbo.QQuestion_Users.UserId, dbo.QQuestion_Users.Date, 0
		FROM    dbo.QQuestion_Users
GO


/* Drop old table */
DROP TABLE [dbo].[QQuestion_Users]
GO


/* Create old new table */
CREATE TABLE [dbo].[QuestionUsers](
	[QuestionID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[Seen] [bit] NOT NULL,
 CONSTRAINT [PK_QuestionUsers] PRIMARY KEY CLUSTERED 
(
	[QuestionID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[QuestionUsers]  WITH CHECK ADD  CONSTRAINT [FK_QuestionUsers_Questions] FOREIGN KEY([QuestionID])
REFERENCES [dbo].[Questions] ([QuestionID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[QuestionUsers] CHECK CONSTRAINT [FK_QuestionUsers_Questions]
GO

ALTER TABLE [dbo].[QuestionUsers]  WITH CHECK ADD  CONSTRAINT [FK_QuestionUsers_ProfileCommon] FOREIGN KEY([UserID])
REFERENCES [dbo].[ProfileCommon] ([UserId])
GO

ALTER TABLE [dbo].[QuestionUsers] CHECK CONSTRAINT [FK_QuestionUsers_ProfileCommon]
GO


INSERT INTO [dbo].[QuestionUsers]
SELECT * FROM dbo.QuestionUsersTemp
GO

DROP TABLE [dbo].[QuestionUsersTemp]
GO