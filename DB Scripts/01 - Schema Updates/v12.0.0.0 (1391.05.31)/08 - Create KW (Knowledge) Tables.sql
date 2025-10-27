USE [EKM_App]

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



ALTER TABLE [dbo].[CN_Nodes]
ADD [Privacy] [varchar](20) NULL
GO


ALTER TABLE [dbo].[KWF_Paraphs]
ADD [Deleted] [bit] NULL
GO



/****** Object:  Table [dbo].[KW_KnowledgeTypes]    Script Date: 06/23/2012 00:20:28 ******/
CREATE TABLE [dbo].[KW_ConfidentialityLevels](
	[LevelID] [int] NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_KW_ConfidentialityLevels] PRIMARY KEY CLUSTERED 
(
	[LevelID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO



CREATE TABLE [dbo].[KW_UsersConfidentialityLevels](
	[UserID] [uniqueidentifier] NOT NULL,
	[LevelID] [int] NOT NULL,
 CONSTRAINT [PK_KW_UsersConfidentialityLevels] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[KW_UsersConfidentialityLevels]  WITH CHECK ADD  CONSTRAINT [FK_KW_UsersConfidentialityLevels_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[KW_UsersConfidentialityLevels] CHECK CONSTRAINT [FK_KW_UsersConfidentialityLevels_aspnet_Users]
GO

ALTER TABLE [dbo].[KW_UsersConfidentialityLevels]  WITH CHECK ADD  CONSTRAINT [FK_KW_UsersConfidentialityLevels_KW_ConfidentialityLevels] FOREIGN KEY([LevelID])
REFERENCES [dbo].[KW_ConfidentialityLevels] ([LevelID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[KW_UsersConfidentialityLevels] CHECK CONSTRAINT [FK_KW_UsersConfidentialityLevels_KW_ConfidentialityLevels]
GO



CREATE TABLE [dbo].[KW_KnowledgeTypes](
	[KnowledgeTypeID] [int] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[Description] [nvarchar](1000) NULL
CONSTRAINT [PK_KW_KnowledgeTypes] PRIMARY KEY CLUSTERED 
(
	[KnowledgeTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO



/****** Object:  Table [dbo].[KW_ExtendedForms]    Script Date: 06/23/2012 00:20:28 ******/
CREATE TABLE [dbo].[KW_ExtendedForms](
	[KnowledgeTypeID] [int] NOT NULL,
	[FormTypeID] [int] NOT NULL,
CONSTRAINT [PK_KW_ExtendedForms] PRIMARY KEY CLUSTERED 
(
	[KnowledgeTypeID] ASC,
	[FormTypeID]	  ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[KW_ExtendedForms]  WITH CHECK ADD  CONSTRAINT [FK_KW_ExtendedForms_KW_KnowledgeTypes] FOREIGN KEY([KnowledgeTypeID])
REFERENCES [dbo].[KW_KnowledgeTypes] ([KnowledgeTypeID])
GO

ALTER TABLE [dbo].[KW_ExtendedForms] CHECK CONSTRAINT [FK_KW_ExtendedForms_KW_KnowledgeTypes]
GO



/****** Object:  Table [dbo].[KW_Knowledges]    Script Date: 06/23/2012 00:20:00 ******/
CREATE TABLE [dbo].[KW_Knowledges](
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[KnowledgeTypeID] [int] NOT NULL,
	[ContentType] [varchar](30) NULL,
	[IsDefault] [bit] NOT NULL,
	[ExtendedFormID] [uniqueidentifier] NULL,
	[TreeNodeID] [uniqueidentifier] NULL,
	[PreviousVersionID] [uniqueidentifier] NULL,
	[Usage] [nvarchar](max) NULL,
	[ConfidentialityLevelID] [int] NOT NULL,
	[StatusID] [bigint] NOT NULL,
	[PublicationDate] [datetime] NULL,
	[Score] [float] NULL,
	[ScoresWeight] [float] NULL
 CONSTRAINT [PK_KW_Knowledges] PRIMARY KEY CLUSTERED 
(
	[KnowledgeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[KW_Knowledges]  WITH CHECK ADD  CONSTRAINT [FK_KW_Knowledges_CN_Nodes] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[KW_Knowledges] CHECK CONSTRAINT [FK_KW_Knowledges_CN_Nodes]
GO


ALTER TABLE [dbo].[KW_Knowledges]  WITH CHECK ADD  CONSTRAINT [FK_KW_Knowledges_KW_KnowledgeTypes] FOREIGN KEY([KnowledgeTypeID])
REFERENCES [dbo].[KW_KnowledgeTypes] ([KnowledgeTypeID])
GO

ALTER TABLE [dbo].[KW_Knowledges] CHECK CONSTRAINT [FK_KW_Knowledges_KW_KnowledgeTypes]
GO


ALTER TABLE [dbo].[KW_Knowledges]  WITH CHECK ADD  CONSTRAINT [FK_KW_Knowledges_DCT_TreeNodes] FOREIGN KEY([TreeNodeID])
REFERENCES [dbo].[DCT_TreeNodes] ([TreeNodeID])
GO

ALTER TABLE [dbo].[KW_Knowledges] CHECK CONSTRAINT [FK_KW_Knowledges_DCT_TreeNodes]
GO


ALTER TABLE [dbo].[KW_Knowledges]  WITH CHECK ADD  CONSTRAINT [FK_KW_Knowledges_KW_Knowledges] FOREIGN KEY([PreviousVersionID])
REFERENCES [dbo].[KW_Knowledges] ([KnowledgeID])
GO

ALTER TABLE [dbo].[KW_Knowledges] CHECK CONSTRAINT [FK_KW_Knowledges_KW_Knowledges]
GO


ALTER TABLE [dbo].[KW_Knowledges]  WITH CHECK ADD  CONSTRAINT [FK_KW_Knowledges_KW_ConfidentialityLevels] FOREIGN KEY([ConfidentialityLevelID])
REFERENCES [dbo].[KW_ConfidentialityLevels] ([LevelID])
GO

ALTER TABLE [dbo].[KW_Knowledges] CHECK CONSTRAINT [FK_KW_Knowledges_KW_ConfidentialityLevels]
GO


ALTER TABLE [dbo].[KW_Knowledges]  WITH CHECK ADD  CONSTRAINT [FK_KW_Knowledges_KWF_Statuses] FOREIGN KEY([StatusID])
REFERENCES [dbo].[KWF_Statuses] ([StatusID])
GO

ALTER TABLE [dbo].[KW_Knowledges] CHECK CONSTRAINT [FK_KW_Knowledges_KWF_Statuses]
GO



/****** Object:  Table [dbo].[KW_CreatorUsers]    Script Date: 06/23/2012 00:18:27 ******/
CREATE TABLE [dbo].[KW_CreatorUsers](
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[CollaborationShare] [int] NOT NULL,
 CONSTRAINT [PK_KW_CreatorUsers] PRIMARY KEY CLUSTERED 
(
	[KnowledgeID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[KW_CreatorUsers]  WITH CHECK ADD  CONSTRAINT [FK_KW_CreatorUsers_KW_Knowledges] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[KW_Knowledges] ([KnowledgeID])
GO

ALTER TABLE [dbo].[KW_CreatorUsers] CHECK CONSTRAINT [FK_KW_CreatorUsers_KW_Knowledges]
GO


ALTER TABLE [dbo].[KW_CreatorUsers]  WITH CHECK ADD  CONSTRAINT [FK_KW_CreatorUsers_ProfileCommon] FOREIGN KEY([UserID])
REFERENCES [dbo].[ProfileCommon] ([UserId])
GO

ALTER TABLE [dbo].[KW_CreatorUsers] CHECK CONSTRAINT [FK_KW_CreatorUsers_ProfileCommon]
GO



/****** Object:  Table [dbo].[KW_KnowledgeCards]    Script Date: 06/23/2012 00:19:39 ******/
CREATE TABLE [dbo].[KW_KnowledgeCards](
	[CardID] [uniqueidentifier] NOT NULL,
	[SenderUserID] [uniqueidentifier] NULL,
	[ReceiverUserID] [uniqueidentifier] NOT NULL,
	[KnowledgeTypeID] [int] NOT NULL,
	[Title] [nvarchar](500) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[SendDate] [datetime] NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_KW_KnowledgeCards] PRIMARY KEY CLUSTERED 
(
	[CardID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[KW_KnowledgeCards]  WITH CHECK ADD  CONSTRAINT [FK_KW_KnowledgeCards_ProfileCommon_Sender] FOREIGN KEY([SenderUserID])
REFERENCES [dbo].[ProfileCommon] ([UserId])
GO

ALTER TABLE [dbo].[KW_KnowledgeCards] CHECK CONSTRAINT [FK_KW_KnowledgeCards_ProfileCommon_Sender]
GO


ALTER TABLE [dbo].[KW_KnowledgeCards]  WITH CHECK ADD  CONSTRAINT [FK_KW_KnowledgeCards_ProfileCommon_Receiver] FOREIGN KEY([ReceiverUserID])
REFERENCES [dbo].[ProfileCommon] ([UserId])
GO

ALTER TABLE [dbo].[KW_KnowledgeCards] CHECK CONSTRAINT [FK_KW_KnowledgeCards_ProfileCommon_Receiver]
GO


ALTER TABLE [dbo].[KW_KnowledgeCards]  WITH CHECK ADD  CONSTRAINT [FK_KW_KnowledgeCards_KW_KnowledgeTypes] FOREIGN KEY([KnowledgeTypeID])
REFERENCES [dbo].[KW_KnowledgeTypes] ([KnowledgeTypeID])
GO

ALTER TABLE [dbo].[KW_KnowledgeCards] CHECK CONSTRAINT [FK_KW_KnowledgeCards_KW_KnowledgeTypes]
GO



/****** Object:  Table [dbo].[KW_ExpertiseLevels]    Script Date: 06/23/2012 00:18:50 ******/
CREATE TABLE [dbo].[KW_ExpertiseLevels](
	[LevelID] [int] NOT NULL,
	[Title] [nvarchar](256) NOT NULL,
	[Description] [nvarchar](1000) NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_KW_ExpertiseLevels] PRIMARY KEY CLUSTERED 
(
	[LevelID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[KW_ExpertiseLevels]  WITH CHECK ADD  CONSTRAINT [FK_KW_ExpertiseLevels_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KW_ExpertiseLevels] CHECK CONSTRAINT [FK_KW_ExpertiseLevels_aspnet_Users_Creator]
GO


ALTER TABLE [dbo].[KW_ExpertiseLevels]  WITH CHECK ADD  CONSTRAINT [FK_KW_ExpertiseLevels_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KW_ExpertiseLevels] CHECK CONSTRAINT [FK_KW_ExpertiseLevels_aspnet_Users_Modifier]
GO



/****** Object:  Table [dbo].[KW_Experts]    Script Date: 06/23/2012 00:19:14 ******/
CREATE TABLE [dbo].[KW_Experts](
	[NodeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[ExpertiseLevelID] [int] NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_KW_Experts] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[KW_Experts]  WITH CHECK ADD  CONSTRAINT [FK_KW_Experts_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[KW_Experts] CHECK CONSTRAINT [FK_KW_Experts_CN_Nodes]
GO


ALTER TABLE [dbo].[KW_Experts]  WITH CHECK ADD  CONSTRAINT [FK_KW_Experts_ProfileCommon] FOREIGN KEY([UserID])
REFERENCES [dbo].[ProfileCommon] ([UserId])
GO

ALTER TABLE [dbo].[KW_Experts] CHECK CONSTRAINT [FK_KW_Experts_ProfileCommon]
GO


ALTER TABLE [dbo].[KW_Experts]  WITH CHECK ADD  CONSTRAINT [FK_KW_Experts_KW_ExpertiseLevels] FOREIGN KEY([ExpertiseLevelID])
REFERENCES [dbo].[KW_ExpertiseLevels] ([LevelID])
GO

ALTER TABLE [dbo].[KW_Experts] CHECK CONSTRAINT [FK_KW_Experts_KW_ExpertiseLevels]
GO



/****** Object:  Table [dbo].[KW_SkillLevels]    Script Date: 06/23/2012 00:20:28 ******/
CREATE TABLE [dbo].[KW_SkillLevels](
	[LevelID] [int] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[Description] [nvarchar](1000) NULL,
	[IsPractical] [bit] NOT NULL,
	[Deleted] [bit] NOT NULL
CONSTRAINT [PK_KW_SkillLevels] PRIMARY KEY CLUSTERED 
(
	[LevelID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO



/****** Object:  Table [dbo].[KW_KnowledgeAssets]    Script Date: 06/23/2012 00:20:28 ******/
CREATE TABLE [dbo].[KW_KnowledgeAssets](
	[UserID] [uniqueidentifier] NOT NULL,
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[TheoricalLevelID] [int] NULL,
	[PracticalLevelID] [int] NULL,
	[AcceptionDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
CONSTRAINT [PK_KW_KnowledgeAssets] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[KnowledgeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[KW_KnowledgeAssets]  WITH CHECK ADD  CONSTRAINT [FK_KW_KnowledgeAssets_ProfileCommon] FOREIGN KEY([UserID])
REFERENCES [dbo].[ProfileCommon] ([UserId])
GO

ALTER TABLE [dbo].[KW_KnowledgeAssets] CHECK CONSTRAINT [FK_KW_KnowledgeAssets_ProfileCommon]
GO


ALTER TABLE [dbo].[KW_KnowledgeAssets]  WITH CHECK ADD  CONSTRAINT [FK_KW_KnowledgeAssets_KW_Knowledges] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[KW_Knowledges] ([KnowledgeID])
GO

ALTER TABLE [dbo].[KW_KnowledgeAssets] CHECK CONSTRAINT [FK_KW_KnowledgeAssets_KW_Knowledges]
GO


ALTER TABLE [dbo].[KW_KnowledgeAssets]  WITH CHECK ADD  CONSTRAINT [FK_KW_KnowledgeAssets_KW_SkillLevels_Theorical] FOREIGN KEY([TheoricalLevelID])
REFERENCES [dbo].[KW_SkillLevels] ([LevelID])
GO

ALTER TABLE [dbo].[KW_KnowledgeAssets] CHECK CONSTRAINT [FK_KW_KnowledgeAssets_KW_SkillLevels_Theorical]
GO


ALTER TABLE [dbo].[KW_KnowledgeAssets]  WITH CHECK ADD  CONSTRAINT [FK_KW_KnowledgeAssets_KW_SkillLevels_Practical] FOREIGN KEY([PracticalLevelID])
REFERENCES [dbo].[KW_SkillLevels] ([LevelID])
GO

ALTER TABLE [dbo].[KW_KnowledgeAssets] CHECK CONSTRAINT [FK_KW_KnowledgeAssets_KW_SkillLevels_Practical]
GO



/****** Object:  Table [dbo].[KW_RefrenceUsers]    Script Date: 06/23/2012 00:20:28 ******/
CREATE TABLE [dbo].[KW_RefrenceUsers](
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
CONSTRAINT [PK_KW_RefrenceUsers] PRIMARY KEY CLUSTERED 
(
	[KnowledgeID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[KW_RefrenceUsers]  WITH CHECK ADD  CONSTRAINT [FK_KW_RefrenceUsers_KW_Knowledges] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[KW_Knowledges] ([KnowledgeID])
GO

ALTER TABLE [dbo].[KW_RefrenceUsers] CHECK CONSTRAINT [FK_KW_RefrenceUsers_KW_Knowledges]
GO

ALTER TABLE [dbo].[KW_RefrenceUsers]  WITH CHECK ADD  CONSTRAINT [FK_KW_RefrenceUsers_ProfileCommon] FOREIGN KEY([UserID])
REFERENCES [dbo].[ProfileCommon] ([UserId])
GO

ALTER TABLE [dbo].[KW_RefrenceUsers] CHECK CONSTRAINT [FK_KW_RefrenceUsers_ProfileCommon]
GO



/****** Object:  Table [dbo].[KW_RelatedDepartments]    Script Date: 06/23/2012 00:20:28 ******/
CREATE TABLE [dbo].[KW_RelatedDepartments](
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[DepartmentID] [uniqueidentifier] NOT NULL,
	[Score] [float] NOT NULL,
	[ScoresWeight] [float] NOT NULL,
	[Deleted] [bit] NOT NULL
CONSTRAINT [PK_KW_RelatedDepartments] PRIMARY KEY CLUSTERED 
(
	[KnowledgeID] ASC,
	[DepartmentID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[KW_RelatedDepartments]  WITH CHECK ADD  CONSTRAINT [FK_KW_RelatedDepartments_KW_Knowledges] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[KW_Knowledges] ([KnowledgeID])
GO

ALTER TABLE [dbo].[KW_RelatedDepartments] CHECK CONSTRAINT [FK_KW_RelatedDepartments_KW_Knowledges]
GO


ALTER TABLE [dbo].[KW_RelatedDepartments]  WITH CHECK ADD  CONSTRAINT [FK_KW_RelatedDepartments_Departments] FOREIGN KEY([DepartmentID])
REFERENCES [dbo].[Departments] ([DepartmentID])
GO

ALTER TABLE [dbo].[KW_RelatedDepartments] CHECK CONSTRAINT [FK_KW_RelatedDepartments_Departments]
GO



/****** Object:  Table [dbo].[KW_LearningMethods]    Script Date: 06/23/2012 00:20:28 ******/
CREATE TABLE [dbo].[KW_LearningMethods](
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[MethodID] [int] NOT NULL IDENTITY(1,1),
	[Title] [nvarchar](256) NOT NULL,
	[Description] [nvarchar](4000) NOT NULL,
CONSTRAINT [PK_KW_LearningMethods] PRIMARY KEY CLUSTERED 
(
	[KnowledgeID] ASC,
	[MethodID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[KW_LearningMethods]  WITH CHECK ADD  CONSTRAINT [FK_KW_LearningMethods_KW_Knowledges] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[KW_Knowledges] ([KnowledgeID])
GO

ALTER TABLE [dbo].[KW_LearningMethods] CHECK CONSTRAINT [FK_KW_LearningMethods_KW_Knowledges]
GO



/****** Object:  Table [dbo].[KW_TripForms]    Script Date: 06/23/2012 00:20:28 ******/
CREATE TABLE [dbo].[KW_TripForms](
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[BeginDate] [datetime] NULL,
	[FinishDate] [datetime] NULL,
	[Country] [nvarchar](256) NULL,
	[City] [nvarchar](256) NOT NULL,
	[Results] [nvarchar](4000) NULL,
	[Chalenges] [nvarchar](4000) NULL
CONSTRAINT [PK_KW_TripForms] PRIMARY KEY CLUSTERED 
(
	[KnowledgeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[KW_TripForms]  WITH CHECK ADD  CONSTRAINT [FK_KW_TripForms_KW_Knowledges] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[KW_Knowledges] ([KnowledgeID])
GO

ALTER TABLE [dbo].[KW_TripForms] CHECK CONSTRAINT [FK_KW_TripForms_KW_Knowledges]
GO



/****** Object:  Table [dbo].[KW_Companies]    Script Date: 06/23/2012 00:20:28 ******/
CREATE TABLE [dbo].[KW_Companies](
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[CompanyID] [int] NOT NULL IDENTITY(1,1),
	[Title] [nvarchar](256) NULL,
	[Products] [nvarchar](4000) NULL
CONSTRAINT [PK_KW_Companies] PRIMARY KEY CLUSTERED 
(
	[KnowledgeID] ASC,
	[CompanyID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[KW_Companies]  WITH CHECK ADD  CONSTRAINT [FK_KW_Companies_KW_Knowledges] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[KW_Knowledges] ([KnowledgeID])
GO

ALTER TABLE [dbo].[KW_Companies] CHECK CONSTRAINT [FK_KW_Companies_KW_Knowledges]
GO



/****** Object:  Table [dbo].[KW_RelatedNodes]    Script Date: 06/23/2012 00:20:28 ******/
CREATE TABLE [dbo].[KW_RelatedNodes](
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NOT NULL,
	[Score] [float] NOT NULL,
	[ScoresWeight] [float] NOT NULL,
	[StatusID] [bigint] NULL,
	[Deleted] [bit] NOT NULL
CONSTRAINT [PK_KW_RelatedNodes] PRIMARY KEY CLUSTERED 
(
	[KnowledgeID] ASC,
	[NodeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[KW_RelatedNodes]  WITH CHECK ADD  CONSTRAINT [FK_KW_RelatedNodes_KW_Knowledges] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[KW_Knowledges] ([KnowledgeID])
GO

ALTER TABLE [dbo].[KW_RelatedNodes] CHECK CONSTRAINT [FK_KW_RelatedNodes_KW_Knowledges]
GO


ALTER TABLE [dbo].[KW_RelatedNodes]  WITH CHECK ADD  CONSTRAINT [FK_KW_RelatedNodes_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[KW_RelatedNodes] CHECK CONSTRAINT [FK_KW_RelatedNodes_CN_Nodes]
GO



/****** Object:  Table [dbo].[KW_RelatedKnowledges]    Script Date: 06/23/2012 00:20:28 ******/
CREATE TABLE [dbo].[KW_RelatedKnowledges](
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[RelatedKnowledgeID] [uniqueidentifier] NOT NULL,
	[Deleted] [bit] NOT NULL
CONSTRAINT [PK_KW_RelatedKnowledges] PRIMARY KEY CLUSTERED 
(
	[KnowledgeID] ASC,
	[RelatedKnowledgeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[KW_RelatedKnowledges]  WITH CHECK ADD  CONSTRAINT [FK_KW_RelatedKnowledges_KW_Knowledges] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[KW_Knowledges] ([KnowledgeID])
GO

ALTER TABLE [dbo].[KW_RelatedKnowledges] CHECK CONSTRAINT [FK_KW_RelatedKnowledges_KW_Knowledges]
GO


ALTER TABLE [dbo].[KW_RelatedKnowledges]  WITH CHECK ADD  CONSTRAINT [FK_KW_RelatedKnowledges_KW_RelatedKnowledges_Related] FOREIGN KEY([RelatedKnowledgeID])
REFERENCES [dbo].[KW_Knowledges] ([KnowledgeID])
GO

ALTER TABLE [dbo].[KW_RelatedKnowledges] CHECK CONSTRAINT [FK_KW_RelatedKnowledges_KW_RelatedKnowledges_Related]
GO


/****** Object:  Table [dbo].[KW_ExperienceHolders]    Script Date: 06/23/2012 00:20:28 ******/
CREATE TABLE [dbo].[KW_ExperienceHolders](
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[SendDate] [datetime] NOT NULL,
	[Deleted] [bit] NOT NULL
CONSTRAINT [PK_KW_ExperienceHolders] PRIMARY KEY CLUSTERED 
(
	[KnowledgeID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[KW_ExperienceHolders]  WITH CHECK ADD  CONSTRAINT [FK_KW_ExperienceHolders_KW_Knowledges] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[KW_Knowledges] ([KnowledgeID])
GO

ALTER TABLE [dbo].[KW_ExperienceHolders] CHECK CONSTRAINT [FK_KW_ExperienceHolders_KW_Knowledges]
GO


ALTER TABLE [dbo].[KW_ExperienceHolders]  WITH CHECK ADD  CONSTRAINT [FK_KW_ExperienceHolders_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KW_ExperienceHolders] CHECK CONSTRAINT [FK_KW_ExperienceHolders_aspnet_Users]
GO


/****** Object:  Table [dbo].[KW_FeedBacks]    Script Date: 06/23/2012 00:20:28 ******/
CREATE TABLE [dbo].[KW_FeedBacks](
	[FeedBackID] [bigint] NOT NULL IDENTITY(1,1),
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[FeedBackTypeID] [int] NOT NULL,
	[SendDate] [datetime] NOT NULL,
	[Value] [float] NOT NULL,
	[Description] [nvarchar](2000) NULL,
	[Deleted] [bit] NOT NULL
CONSTRAINT [PK_KW_FeedBacks] PRIMARY KEY CLUSTERED 
(
	[FeedBackID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[KW_FeedBacks]  WITH CHECK ADD  CONSTRAINT [FK_KW_FeedBacks_KW_Knowledges] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[KW_Knowledges] ([KnowledgeID])
GO

ALTER TABLE [dbo].[KW_FeedBacks] CHECK CONSTRAINT [FK_KW_FeedBacks_KW_Knowledges]
GO


ALTER TABLE [dbo].[KW_FeedBacks]  WITH CHECK ADD  CONSTRAINT [FK_KW_FeedBacks_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KW_FeedBacks] CHECK CONSTRAINT [FK_KW_FeedBacks_aspnet_Users]
GO