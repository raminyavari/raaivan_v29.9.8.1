USE [EKM_App]
GO


CREATE TABLE [dbo].[USR_PassResetTickets](
	[UserID]		UNIQUEIDENTIFIER NOT NULL,
	[Ticket]		UNIQUEIDENTIFIER NOT NULL
 CONSTRAINT [PK_USR_PassResetTickets] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER TABLE [dbo].[USR_PassResetTickets]  WITH CHECK ADD  CONSTRAINT [FK_USR_PassResetTickets_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[USR_PassResetTickets] CHECK CONSTRAINT [FK_USR_PassResetTickets_aspnet_Users]
GO



/****** Object:  Table [dbo].[KKnowledges]    Script Date: 04/04/2012 12:34:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



UPDATE [dbo].[AppSetting]
	SET [Version] = 'v26.2.0.1' -- 13930719
GO


/****** Object:  Table [dbo].[CN_ListAdmins]    Script Date: 10/12/2014 14:34:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(select * FROM sys.views where name = 'CN_View_ListAdmins')
DROP VIEW [dbo].[CN_View_ListAdmins]
GO


CREATE TABLE [dbo].[CN_TMPListAdmins](
	[ListID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_CN_TMPListAdmins] PRIMARY KEY CLUSTERED 
(
	[ListID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[CN_TMPListAdmins]
SELECT * 
FROM [dbo].[CN_ListAdmins]

GO


DROP TABLE [dbo].[CN_ListAdmins]
GO


CREATE TABLE [dbo].[CN_ListAdmins](
	[ListID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_CN_ListAdmins] PRIMARY KEY CLUSTERED 
(
	[ListID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[CN_ListAdmins]  WITH CHECK ADD  CONSTRAINT [FK_CN_ListAdmins_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_ListAdmins] CHECK CONSTRAINT [FK_CN_ListAdmins_aspnet_Users]
GO

ALTER TABLE [dbo].[CN_ListAdmins]  WITH CHECK ADD  CONSTRAINT [FK_CN_ListAdmins_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_ListAdmins] CHECK CONSTRAINT [FK_CN_ListAdmins_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[CN_ListAdmins]  WITH CHECK ADD  CONSTRAINT [FK_CN_ListAdmins_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_ListAdmins] CHECK CONSTRAINT [FK_CN_ListAdmins_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[CN_ListAdmins]  WITH CHECK ADD  CONSTRAINT [FK_CN_ListAdmins_CN_Lists] FOREIGN KEY([ListID])
REFERENCES [dbo].[CN_Lists] ([ListID])
GO

ALTER TABLE [dbo].[CN_ListAdmins] CHECK CONSTRAINT [FK_CN_ListAdmins_CN_Lists]
GO



INSERT INTO [dbo].[CN_ListAdmins]
SELECT *
FROM [dbo].[CN_TMPListAdmins]
GO


DROP TABLE [dbo].[CN_TMPListAdmins]
GO


/****** Object:  Table [dbo].[KKnowledges]    Script Date: 04/04/2012 12:34:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



UPDATE [dbo].[AppSetting]
	SET [Version] = 'v26.3.1.2' -- 13930720
GO


UPDATE ATT
	SET ObjectType = N'Node'
FROM [dbo].[Attachments] AS ATT
	INNER JOIN [dbo].[CN_Nodes] AS ND
	ON ND.NodeID = ATT.ObjectID
	
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_USR_ItemVisits_aspnet_Users]') AND parent_object_id = OBJECT_ID(N'[dbo].[USR_ItemVisits]'))
ALTER TABLE [dbo].[USR_ItemVisits] DROP CONSTRAINT [FK_USR_ItemVisits_aspnet_Users]
GO



/****** Object:  Table [dbo].[KKnowledges]    Script Date: 04/04/2012 12:34:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



UPDATE [dbo].[AppSetting]
	SET [Version] = 'v26.3.1.7' -- 13930729
GO
