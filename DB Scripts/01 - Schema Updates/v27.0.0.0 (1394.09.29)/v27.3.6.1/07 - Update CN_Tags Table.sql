USE [EKM_App]
GO

/****** Object:  Table [dbo].[CN_Tags]    Script Date: 05/15/2016 14:19:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER TABLE [dbo].[WF_WorkFlowStates]
DROP CONSTRAINT [FK_WF_WorkFlowStates_CN_Tags]
GO


CREATE TABLE [dbo].[TMP_CN_Tags](
	[TagID] [uniqueidentifier] NOT NULL,
	[Tag] [nvarchar](400) NOT NULL,
	[IsApproved] [bit] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[CallsCount] [int] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[ApplicationID] [uniqueidentifier] NULL,
 CONSTRAINT [TMP_PK_CN_Tags] PRIMARY KEY CLUSTERED 
(
	[TagID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[TMP_CN_Tags]
SELECT *
FROM [dbo].[CN_Tags]
GO

DROP TABLE [dbo].[CN_Tags]
GO

CREATE TABLE [dbo].[CN_Tags](
	[TagID] [uniqueidentifier] NOT NULL,
	[Tag] [nvarchar](400) NOT NULL,
	[IsApproved] [bit] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[CallsCount] [int] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[ApplicationID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_CN_Tags] PRIMARY KEY CLUSTERED 
(
	[TagID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[CN_Tags]
SELECT *
FROM [dbo].[TMP_CN_Tags]
GO

DROP TABLE [dbo].[TMP_CN_Tags]
GO


 ALTER TABLE [dbo].[CN_Tags] ADD  CONSTRAINT [UK_CN_Tags_Tag] UNIQUE NONCLUSTERED 
(
	[ApplicationID] ASC,
	[Tag] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

GO

ALTER TABLE [dbo].[CN_Tags]  WITH CHECK ADD  CONSTRAINT [FK_CN_Tags_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[CN_Tags] CHECK CONSTRAINT [FK_CN_Tags_aspnet_Applications]
GO

ALTER TABLE [dbo].[CN_Tags]  WITH CHECK ADD  CONSTRAINT [FK_CN_Tags_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_Tags] CHECK CONSTRAINT [FK_CN_Tags_aspnet_Users_Creator]
GO


ALTER TABLE [dbo].[WF_WorkFlowStates]  WITH CHECK ADD  CONSTRAINT [FK_WF_WorkFlowStates_CN_Tags] FOREIGN KEY([TagID])
REFERENCES [dbo].[CN_Tags] ([TagID])
GO

ALTER TABLE [dbo].[WF_WorkFlowStates] CHECK CONSTRAINT [FK_WF_WorkFlowStates_CN_Tags]
GO