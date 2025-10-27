USE [EKM_App]
GO

/****** Object:  Table [dbo].[TMP_CN_Lists]    Script Date: 02/10/2014 11:02:07 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO


IF EXISTS(select * FROM sys.views where name = 'CN_View_Lists')
DROP VIEW [dbo].[CN_View_Lists]
GO

IF EXISTS(select * FROM sys.views where name = 'CN_View_NodeMembers')
DROP VIEW [dbo].[CN_View_NodeMembers]
GO

IF EXISTS(select * FROM sys.views where name = 'CN_View_ListMembers')
DROP VIEW [dbo].[CN_View_ListMembers]
GO

IF EXISTS(select * FROM sys.views where name = 'CN_View_ListExperts')
DROP VIEW [dbo].[CN_View_ListExperts]
GO


CREATE TABLE [dbo].[TMP_CN_Lists](
	[ListID] [uniqueidentifier] NOT NULL,
	[NodeTypeID] [uniqueidentifier] NOT NULL,
	[AdditionalID] [nvarchar](50) NULL,
	[ParentListID] [uniqueidentifier] NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2000) NULL,
	[OwnerID] [uniqueidentifier] NULL,
	[OwnerType] [varchar](20) NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_TMP_CN_Lists] PRIMARY KEY CLUSTERED 
(
	[ListID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[TMP_CN_Lists]  WITH CHECK ADD  CONSTRAINT [FK_TMP_CN_Lists_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[TMP_CN_Lists] CHECK CONSTRAINT [FK_TMP_CN_Lists_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[TMP_CN_Lists]  WITH CHECK ADD  CONSTRAINT [FK_TMP_CN_Lists_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[TMP_CN_Lists] CHECK CONSTRAINT [FK_TMP_CN_Lists_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[TMP_CN_Lists]  WITH CHECK ADD  CONSTRAINT [FK_TMP_CN_Lists_TMP_CN_Lists] FOREIGN KEY([ParentListID])
REFERENCES [dbo].[TMP_CN_Lists] ([ListID])
GO

ALTER TABLE [dbo].[TMP_CN_Lists] CHECK CONSTRAINT [FK_TMP_CN_Lists_TMP_CN_Lists]
GO

ALTER TABLE [dbo].[TMP_CN_Lists]  WITH CHECK ADD  CONSTRAINT [FK_TMP_CN_Lists_CN_NodeTypes] FOREIGN KEY([NodeTypeID])
REFERENCES [dbo].[CN_NodeTypes] ([NodeTypeID])
GO

ALTER TABLE [dbo].[TMP_CN_Lists] CHECK CONSTRAINT [FK_TMP_CN_Lists_CN_NodeTypes]
GO


DECLARE @DepTypeID uniqueidentifier = [dbo].[CN_FN_GetDepartmentNodeTypeID]()

INSERT INTO [dbo].[TMP_CN_Lists](
	[ListID],
	[NodeTypeID],
	[AdditionalID],
	[ParentListID],
	[Name],
	[Description],
	[OwnerID],
	[OwnerType],
	[CreatorUserID],
	[CreationDate],
	[LastModifierUserID],
	[LastModificationDate],
	[Deleted]
)
SELECT L.ListID, @DepTypeID, LT.AdditionalID, L.ParentListID,
	L.Name, LT.Name, L.OwnerID, L.OwnerType, L.CreatorUserID, L.CreationDate,
	L.LastModifierUserID, L.LastModificationDate, L.Deleted
FROM [dbo].[CN_Lists] AS L
	INNER JOIN [dbo].[CN_ListTypes] AS LT
	ON L.ListTypeID = LT.ListTypeID
	
GO


ALTER TABLE [dbo].[CN_ListNodes]
DROP CONSTRAINT [FK_CN_ListNodes_CN_Lists]
GO

ALTER TABLE [dbo].[CN_Lists]
DROP CONSTRAINT [FK_CN_Lists_CN_Lists]
GO

ALTER TABLE [dbo].[KW_KnowledgeManagers]
DROP CONSTRAINT [FK_KW_KnowledgeManagers_CN_Lists]
GO


DROP TABLE [dbo].[CN_Lists]
GO

DROP TABLE [dbo].[CN_ListTypes]
GO


CREATE TABLE [dbo].[CN_Lists](
	[ListID] [uniqueidentifier] NOT NULL,
	[NodeTypeID] [uniqueidentifier] NOT NULL,
	[AdditionalID] [nvarchar](50) NULL,
	[ParentListID] [uniqueidentifier] NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2000) NULL,
	[OwnerID] [uniqueidentifier] NULL,
	[OwnerType] [varchar](20) NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_CN_Lists] PRIMARY KEY CLUSTERED 
(
	[ListID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[CN_Lists]  WITH CHECK ADD  CONSTRAINT [FK_CN_Lists_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_Lists] CHECK CONSTRAINT [FK_CN_Lists_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[CN_Lists]  WITH CHECK ADD  CONSTRAINT [FK_CN_Lists_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_Lists] CHECK CONSTRAINT [FK_CN_Lists_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[CN_Lists]  WITH CHECK ADD  CONSTRAINT [FK_CN_Lists_CN_Lists] FOREIGN KEY([ParentListID])
REFERENCES [dbo].[CN_Lists] ([ListID])
GO

ALTER TABLE [dbo].[CN_Lists] CHECK CONSTRAINT [FK_CN_Lists_CN_Lists]
GO

ALTER TABLE [dbo].[CN_Lists]  WITH CHECK ADD  CONSTRAINT [FK_CN_Lists_CN_NodeTypes] FOREIGN KEY([NodeTypeID])
REFERENCES [dbo].[CN_NodeTypes] ([NodeTypeID])
GO

ALTER TABLE [dbo].[CN_Lists] CHECK CONSTRAINT [FK_CN_Lists_CN_NodeTypes]
GO


INSERT INTO [dbo].[CN_Lists]
SELECT * 
FROM [dbo].[TMP_CN_Lists]

GO

DROP TABLE [dbo].[TMP_CN_Lists]
GO

ALTER TABLE [dbo].[CN_ListNodes]  WITH CHECK ADD  CONSTRAINT [FK_CN_ListNodes_CN_Lists] FOREIGN KEY([ListID])
REFERENCES [dbo].[CN_Lists] ([ListID])
GO

ALTER TABLE [dbo].[CN_ListNodes] CHECK CONSTRAINT [FK_CN_ListNodes_CN_Lists]
GO