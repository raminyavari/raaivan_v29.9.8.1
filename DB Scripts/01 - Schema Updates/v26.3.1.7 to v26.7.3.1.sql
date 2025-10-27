USE [EKM_App]
GO


CREATE TABLE [dbo].[USR_FriendSuggestions](
	SuggestionID			UNIQUEIDENTIFIER NOT NULL,
	UserID1					UNIQUEIDENTIFIER NOT NULL,
	UserID2					UNIQUEIDENTIFIER NOT NULL,
	Score					INT NOT NULL
CONSTRAINT [PK_USR_FriendSuggestions] PRIMARY KEY CLUSTERED
(
	SuggestionID ASC 
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[USR_FriendSuggestions] ADD  CONSTRAINT [UK_USR_FriendSuggestions] UNIQUE NONCLUSTERED 
(
	[UserID1]	ASC,
	[UserID2]	ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


ALTER TABLE [dbo].[USR_FriendSuggestions]  WITH CHECK ADD  CONSTRAINT [FK_USR_FriendSuggestions_aspnet_Users_UserID1] FOREIGN KEY([UserID1])
REFERENCES [dbo].[aspnet_Users] ([UserID])
GO

ALTER TABLE [dbo].[USR_FriendSuggestions] CHECK CONSTRAINT [FK_USR_FriendSuggestions_aspnet_Users_UserID1]
GO


ALTER TABLE [dbo].[USR_FriendSuggestions]  WITH CHECK ADD  CONSTRAINT [FK_USR_FriendSuggestions_aspnet_Users_UserID2] FOREIGN KEY([UserID2])
REFERENCES [dbo].[aspnet_Users] ([UserID])
GO

ALTER TABLE [dbo].[USR_FriendSuggestions] CHECK CONSTRAINT [FK_USR_FriendSuggestions_aspnet_Users_UserID2]
GO



--1
ALTER TABLE [dbo].[USR_Invitations]
DROP CONSTRAINT [FK_USR_Invitations_aspnet_Users_Created]

--2
--Changed Farzane
ALTER TABLE [dbo].[USR_Invitations]  WITH CHECK ADD  CONSTRAINT [FK_USR_Invitations_USR_TemporaryUsers_Created] FOREIGN KEY([CreatedUserID])
REFERENCES [dbo].[USR_TemporaryUsers] ([UserId])
GO

ALTER TABLE [dbo].[USR_Invitations] CHECK CONSTRAINT [FK_USR_Invitations_USR_TemporaryUsers_Created]
GO
--End of Changed Farzane



Update [dbo].[CN_NodeTypes]
	SET ParentID = NULL
WHERE [Deleted] = 1
GO



/****** Object:  Table [dbo].[KKnowledges]    Script Date: 04/04/2012 12:34:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



UPDATE [dbo].[AppSetting]
	SET [Version] = 'v26.4.0.2' -- 13930929
GO


CREATE TABLE [dbo].[RV_Variables](
	[Name]					VARCHAR(50) NOT NULL,
	[Value]					NVARCHAR (max) NOT NULL,
	[LastModifierUserID]	UNIQUEIDENTIFIER,
	[LastModificationDate]	DATETIME NOT NULL
 CONSTRAINT [PK_RV_Variables] PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER TABLE [dbo].[RV_Variables] WITH CHECK ADD CONSTRAINT [FK_RV_Variables_aspnet_Users] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserID])
GO

ALTER TABLE [dbo].[RV_Variables] CHECK CONSTRAINT [FK_RV_Variables_aspnet_Users]
GO




ALTER TABLE [dbo].[MSG_Messages] CHECK CONSTRAINT [FK_MSG_Messages_aspnet_Users]
GO


/****** Object:  Table [dbo].[KKnowledges]    Script Date: 04/04/2012 12:34:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



UPDATE [dbo].[AppSetting]
	SET [Version] = 'v26.5.1.0' -- 13931006
GO


ALTER TABLE [dbo].[NTFN_NotificationMessageTemplates]
ADD [Subject] nvarchar(512) NULL
GO


/****** Object:  Table [dbo].[KKnowledges]    Script Date: 04/04/2012 12:34:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



UPDATE [dbo].[AppSetting]
	SET [Version] = 'v26.5.2.2' -- 13931014
GO



ALTER TABLE [dbo].[NTFN_NotificationMessageTemplates]
ADD [IsDefault] bit NULL
GO


/****** Object:  Table [dbo].[KKnowledges]    Script Date: 04/04/2012 12:34:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



UPDATE [dbo].[AppSetting]
	SET [Version] = 'v26.5.3.0' -- 13931017
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[RV_TMPVariables](
	[Name] [varchar](50) NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NOT NULL,
 CONSTRAINT [PK_RV_TMPVariables] PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[RV_TMPVariables]
SELECT * FROM [dbo].[RV_Variables]
GO

DROP TABLE [dbo].[RV_Variables]
GO

CREATE TABLE [dbo].[RV_Variables](
	[Name] [varchar](100) NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NOT NULL,
 CONSTRAINT [PK_RV_Variables] PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[RV_Variables]
SELECT * FROM [dbo].[RV_TMPVariables]
GO

DROP TABLE [dbo].[RV_TMPVariables]
GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[RV_Variables]  WITH CHECK ADD  CONSTRAINT [FK_RV_Variables_aspnet_Users] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[RV_Variables] CHECK CONSTRAINT [FK_RV_Variables_aspnet_Users]
GO




SET ANSI_PADDING ON
GO

UPDATE [dbo].[CN_Nodes]
	SET [Status] = N'SentToAdmin'
WHERE [Status] = N'ManagerEvaluation'

GO


UPDATE [dbo].[CN_Nodes]
	SET [Status] = N'SentToEvaluators'
WHERE [Status] = N'ExpertEvaluation'

GO


/****** Object:  Table [dbo].[KKnowledges]    Script Date: 04/04/2012 12:34:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



UPDATE [dbo].[AppSetting]
	SET [Version] = 'v26.5.4.2' -- 13931021
GO


/****** Object:  Table [dbo].[KKnowledges]    Script Date: 04/04/2012 12:34:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



UPDATE [dbo].[AppSetting]
	SET [Version] = 'v26.5.10.11' -- 13931121
GO


/****** Object:  Table [dbo].[KKnowledges]    Script Date: 04/04/2012 12:34:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



UPDATE [dbo].[AppSetting]
	SET [Version] = 'v26.6.2.5' -- 13940131
GO


/****** Object:  Table [dbo].[KKnowledges]    Script Date: 04/04/2012 12:34:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



UPDATE [dbo].[AppSetting]
	SET [Version] = 'v26.7.3.1' -- 13940229
GO

