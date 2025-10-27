USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/****** Object:  Table [dbo].[WikiSubjects]    Script Date: 11/06/2011 01:01:57 ******/
CREATE TABLE [dbo].[WikiSubjects](
	[ID] [uniqueidentifier] NOT NULL,
	[NodeID] [bigint] NOT NULL,
	[LastExpert] [uniqueidentifier] NULL,
	[SequenceNo] [bigint] NULL,
	[Title] [nvarchar](max) NOT NULL,
	[Description] [text] NULL,
	[Status] [nvarchar](50) NULL,
 CONSTRAINT [PK_WikiSubjects] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[WikiSubjects]  WITH CHECK ADD  CONSTRAINT [FK_WikiSubjects_aspnet_Users_LastExpert] FOREIGN KEY([LastExpert])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WikiSubjects] CHECK CONSTRAINT [FK_WikiSubjects_aspnet_Users_LastExpert]
GO

ALTER TABLE [dbo].[WikiSubjects]  WITH CHECK ADD  CONSTRAINT [FK_WikiSubjects_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[Nodes] ([ID])
GO

ALTER TABLE [dbo].[WikiSubjects] CHECK CONSTRAINT [FK_WikiSubjects_Nodes]
GO


/****** Object:  Table [dbo].[WikiChanges]    Script Date: 11/06/2011 01:02:19 ******/
CREATE TABLE [dbo].[WikiChanges](
	[WikiSubjectID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](max) NULL,
	[Description] [text] NOT NULL,
	[CreationDate] [datetime] NULL,
 CONSTRAINT [PK_WikiChanges] PRIMARY KEY CLUSTERED 
(
	[WikiSubjectID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[WikiChanges]  WITH CHECK ADD  CONSTRAINT [FK_WikiChanges_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WikiChanges] CHECK CONSTRAINT [FK_WikiChanges_aspnet_Users]
GO

ALTER TABLE [dbo].[WikiChanges]  WITH CHECK ADD  CONSTRAINT [FK_WikiChanges_WikiSubjects] FOREIGN KEY([WikiSubjectID])
REFERENCES [dbo].[WikiSubjects] ([ID])
GO

ALTER TABLE [dbo].[WikiChanges] CHECK CONSTRAINT [FK_WikiChanges_WikiSubjects]
GO


/****** Object:  Table [dbo].[WikiSubjectExperts]    Script Date: 11/06/2011 01:03:19 ******/
CREATE TABLE [dbo].[WikiSubjectExperts](
	[WikiSubjectID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[SenderID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_WikiSubjectExperts] PRIMARY KEY CLUSTERED 
(
	[WikiSubjectID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[WikiSubjectExperts]  WITH CHECK ADD  CONSTRAINT [FK_WikiSubjectExperts_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WikiSubjectExperts] CHECK CONSTRAINT [FK_WikiSubjectExperts_aspnet_Users]
GO

ALTER TABLE [dbo].[WikiSubjectExperts]  WITH CHECK ADD  CONSTRAINT [FK_WikiSubjectExperts_aspnet_Users_Expert] FOREIGN KEY([SenderID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WikiSubjectExperts] CHECK CONSTRAINT [FK_WikiSubjectExperts_aspnet_Users_Expert]
GO

ALTER TABLE [dbo].[WikiSubjectExperts]  WITH CHECK ADD  CONSTRAINT [FK_WikiSubjectExperts_WikiSubjects] FOREIGN KEY([WikiSubjectID])
REFERENCES [dbo].[WikiSubjects] ([ID])
GO

ALTER TABLE [dbo].[WikiSubjectExperts] CHECK CONSTRAINT [FK_WikiSubjectExperts_WikiSubjects]
GO


/* Fill WikiSubjects table using old WikiTitlesTable */
INSERT INTO [dbo].[WikiSubjects](ID, NodeID, Title, Description, Status)
SELECT [WikiTitles].ID, [WikiTitles].NodeID, [WikiTitles].Title, 
	[WikiTitles].Description, [WikiTitles].Status
FROM [dbo].[WikiTitles]
WHERE [WikiTitles].Status = 'Accepted'
GO

DROP TABLE [dbo].[WikiTitleExperts]
GO

DROP TABLE [dbo].[WikiTitles]
GO