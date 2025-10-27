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
