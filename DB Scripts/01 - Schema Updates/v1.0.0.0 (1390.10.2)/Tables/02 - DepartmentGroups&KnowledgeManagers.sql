USE [EKM_App]
GO

/****** Object:  Table [dbo].[DepartmentGroups]    Script Date: 10/14/2011 17:10:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DepartmentGroups](
	[ID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [text] NULL,
 CONSTRAINT [PK_DepartmentGroups] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


/****** Object:  Table [dbo].[KnowledgeManagers]    Script Date: 10/14/2011 17:10:16 ******/
CREATE TABLE [dbo].[KnowledgeManagers](
	[DepartmentGroupID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_KnowledgeManagers] PRIMARY KEY CLUSTERED 
(
	[DepartmentGroupID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[KnowledgeManagers]  WITH CHECK ADD  CONSTRAINT [FK_KnowledgeManagers_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KnowledgeManagers] CHECK CONSTRAINT [FK_KnowledgeManagers_aspnet_Users]
GO

ALTER TABLE [dbo].[KnowledgeManagers]  WITH CHECK ADD  CONSTRAINT [FK_KnowledgeManagers_DepartmentGroups] FOREIGN KEY([DepartmentGroupID])
REFERENCES [dbo].[DepartmentGroups] ([ID])
GO

ALTER TABLE [dbo].[KnowledgeManagers] CHECK CONSTRAINT [FK_KnowledgeManagers_DepartmentGroups]
GO


/****** Object:  Table [dbo].[DepartmentsGroupsLink]    Script Date: 10/14/2011 17:10:22 ******/
CREATE TABLE [dbo].[DepartmentsGroupsLink](
	[GroupID] [uniqueidentifier] NOT NULL,
	[DepartmentID] [bigint] NOT NULL,
 CONSTRAINT [PK_DepartmentsGroupsLink] PRIMARY KEY CLUSTERED 
(
	[GroupID] ASC,
	[DepartmentID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[DepartmentsGroupsLink]  WITH CHECK ADD  CONSTRAINT [FK_DepartmentsGroupsLink_DepartmentGroups] FOREIGN KEY([GroupID])
REFERENCES [dbo].[DepartmentGroups] ([ID])
GO

ALTER TABLE [dbo].[DepartmentsGroupsLink] CHECK CONSTRAINT [FK_DepartmentsGroupsLink_DepartmentGroups]
GO

ALTER TABLE [dbo].[DepartmentsGroupsLink]  WITH CHECK ADD  CONSTRAINT [FK_DepartmentsGroupsLink_OrganStructs] FOREIGN KEY([DepartmentID])
REFERENCES [dbo].[OrganStructs] ([ID])
GO

ALTER TABLE [dbo].[DepartmentsGroupsLink] CHECK CONSTRAINT [FK_DepartmentsGroupsLink_OrganStructs]
GO