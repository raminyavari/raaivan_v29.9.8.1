USE [EKM_App]
GO

/****** Object:  Table [dbo].[WikiSubjects]    Script Date: 02/17/2012 15:28:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WikiSubjectsTemp](
	[ID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NOT NULL,
	[LastExpert] [uniqueidentifier] NULL,
	[SequenceNo] [bigint] NULL,
	[Title] [nvarchar](max) NOT NULL,
	[Description] [text] NULL,
	[Status] [nvarchar](50) NULL,
 CONSTRAINT [PK_WikiSubjectsTemp] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


INSERT INTO [dbo].[WikiSubjectsTemp]
           ([ID]
			,[NodeID]
			,[LastExpert]
			,[SequenceNo]
			,[Title]
			,[Description]
			,[Status])
		SELECT  dbo.WikiSubjects.ID, dbo.CN_Nodes.NodeID, dbo.WikiSubjects.LastExpert, dbo.WikiSubjects.SequenceNo,
				dbo.WikiSubjects.Title, dbo.WikiSubjects.Description, dbo.WikiSubjects.Status
		FROM    dbo.CN_Nodes INNER JOIN dbo.WikiSubjects ON dbo.CN_Nodes.ID = dbo.WikiSubjects.NodeID
GO


/* Drop old table */
ALTER TABLE [dbo].[WikiChanges]
DROP CONSTRAINT [FK_WikiChanges_WikiSubjects]
GO

ALTER TABLE [dbo].[WikiSubjectExperts]
DROP CONSTRAINT [FK_WikiSubjectExperts_WikiSubjects]
GO

DROP TABLE [dbo].[WikiSubjects]
GO

/* Create old new table */
CREATE TABLE [dbo].[WikiSubjects](
	[ID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NOT NULL,
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

ALTER TABLE [dbo].[WikiSubjects]  WITH CHECK ADD  CONSTRAINT [FK_WikiSubjects_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[WikiSubjects] CHECK CONSTRAINT [FK_WikiSubjects_CN_Nodes]
GO


INSERT INTO [dbo].[WikiSubjects]
	SELECT * FROM dbo.WikiSubjectsTemp
GO

DROP TABLE [dbo].[WikiSubjectsTemp]
GO


ALTER TABLE [dbo].[WikiChanges]  WITH CHECK ADD  CONSTRAINT [FK_WikiChanges_WikiSubjects] FOREIGN KEY([WikiSubjectID])
REFERENCES [dbo].[WikiSubjects] ([ID])
GO

ALTER TABLE [dbo].[WikiChanges] CHECK CONSTRAINT [FK_WikiChanges_WikiSubjects]
GO

ALTER TABLE [dbo].[WikiSubjectExperts]  WITH CHECK ADD  CONSTRAINT [FK_WikiSubjectExperts_WikiSubjects] FOREIGN KEY([WikiSubjectID])
REFERENCES [dbo].[WikiSubjects] ([ID])
GO

ALTER TABLE [dbo].[WikiSubjectExperts] CHECK CONSTRAINT [FK_WikiSubjectExperts_WikiSubjects]
GO