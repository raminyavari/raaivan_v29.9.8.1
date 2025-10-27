USE [EKM_App]
GO


CREATE TABLE [dbo].[NTFN_NotificationMessageTemplates](
	[TemplateID]			UNIQUEIDENTIFIER NOT NULL,
	[Text]					NVARCHAR (max) NULL,
	[Action]				VARCHAR (50) NOT NULL,
	[SubjectType]			VARCHAR (50) NOT NULL,
	[UserStatus]			VARCHAR (50) NOT NULL,
	[Lang]					VARCHAR (50) NOT NULL,
	[Media]					VARCHAR (50) NOT NULL,
	[Enable]				BIT NOT NULL,
	[LastModificationDate]	DATETIME NULL,
	[LastModifierUserID]	UNIQUEIDENTIFIER  NULL
 CONSTRAINT [PK_NTFN_NotificationMessageTemplates] PRIMARY KEY CLUSTERED 
(
	[TemplateID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[NTFN_NotificationMessageTemplates] ADD  CONSTRAINT [UK_NTFN_NotificationMessageTemplates] UNIQUE NONCLUSTERED 
(
	[Action]		ASC,
	[SubjectType]	ASC,
	[UserStatus]	ASC,
	[Media]			ASC,
	[Lang]			ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


ALTER TABLE [dbo].[NTFN_NotificationMessageTemplates]  WITH CHECK ADD  CONSTRAINT [FK_NTFN_NotificationMessageTemplates_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[NTFN_NotificationMessageTemplates] CHECK CONSTRAINT [FK_NTFN_NotificationMessageTemplates_aspnet_Users_Modifier]
GO



CREATE TABLE [dbo].[NTFN_UserMessagingActivation](
	[OptionID]				[uniqueidentifier] NOT NULL,
	[UserID]				[uniqueidentifier] NOT NULL,
	[SubjectType]			[varchar] (20) NOT NULL,
	[UserStatus]			[VARCHAR] (20) NOT NULL,
	[Action]				[varchar] (20) NOT NULL,
	[Media]					[varchar] (20) NOT NULL,
	[Lang]					[varchar] (20) NOT NULL,
	[Enable]				[bit] NOT NULL,
	[LastModificationDate]	[datetime] NULL,
	[LastModifierUserID]	[uniqueidentifier] NULL
CONSTRAINT [PK_NTFN_UserMessagingActivation] PRIMARY KEY CLUSTERED
(
	[OptionID] ASC 
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[NTFN_UserMessagingActivation] ADD  CONSTRAINT [UK_NTFN_UserMessagingActivation] UNIQUE NONCLUSTERED 
(
	[UserID]		ASC,
	[SubjectType]	ASC,
	[UserStatus]	ASC,
	[Action]		ASC,
	[Media]			ASC,
	[Lang]			ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


ALTER TABLE [dbo].[NTFN_UserMessagingActivation]  WITH CHECK ADD  CONSTRAINT [FK_NTFN_UserMessagingActivation_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserID])
GO

ALTER TABLE [dbo].[NTFN_UserMessagingActivation] CHECK CONSTRAINT [FK_NTFN_UserMessagingActivation_aspnet_Users]
GO


ALTER TABLE [dbo].[NTFN_UserMessagingActivation]  WITH CHECK ADD  CONSTRAINT [FK_NTFN_UserMessagingActivation_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserID])
GO

ALTER TABLE [dbo].[NTFN_UserMessagingActivation] CHECK CONSTRAINT [FK_NTFN_UserMessagingActivation_aspnet_Users_Modifier]
GO


ALTER TABLE [dbo].[CN_Services]
ADD [IsTree] [bit] NULL
GO


/****** Object:  Index [UK_NodeTypes_Name]    Script Date: 09/07/2014 14:45:37 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CN_NodeTypes]') AND name = N'UK_NodeTypes_Name')
ALTER TABLE [dbo].[CN_NodeTypes] DROP CONSTRAINT [UK_NodeTypes_Name]
GO


/****** Object:  Table [dbo].[KKnowledges]    Script Date: 04/04/2012 12:34:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



UPDATE [dbo].[AppSetting]
	SET [Version] = 'v26.0.0.0' -- 13930616
GO

