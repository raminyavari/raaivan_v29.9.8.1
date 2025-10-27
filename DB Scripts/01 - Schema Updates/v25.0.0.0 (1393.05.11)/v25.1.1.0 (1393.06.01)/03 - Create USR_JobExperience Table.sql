USE [EKM_App]
GO

-- create new table
CREATE TABLE [dbo].[USR_JobExperiences](
	JobID			UNIQUEIDENTIFIER NOT NULL,
	AdditionalID	NVARCHAR(50) NULL,
	UserID			UNIQUEIDENTIFIER NOT NULL,
	Title			NVARCHAR(256) NOT NULL,
	Employer		NVARCHAR(256) NOT NULL,
	StartDate		DATETIME NULL,
	EndDate			DATETIME NULL,
	CreatorUserID	UNIQUEIDENTIFIER NOT NULL,
	CreationDate	DATETIME NOT NULL,
	Deleted			BIT NOT NULL
	
	CONSTRAINT [PK_USR_JobExperiences] PRIMARY KEY CLUSTERED
	(
		[JobID] ASC 
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER TABLE [dbo].[USR_JobExperiences]  WITH CHECK ADD  CONSTRAINT [FK_USR_JobExperiences_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserID])
GO

ALTER TABLE [dbo].[USR_JobExperiences] CHECK CONSTRAINT [FK_USR_JobExperiences_aspnet_Users]
GO


ALTER TABLE [dbo].[USR_JobExperiences]  WITH CHECK ADD  CONSTRAINT [FK_USR_JobExperiences_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserID])
GO

ALTER TABLE [dbo].[USR_JobExperiences] CHECK CONSTRAINT [FK_USR_JobExperiences_aspnet_Users_Creator]
GO

