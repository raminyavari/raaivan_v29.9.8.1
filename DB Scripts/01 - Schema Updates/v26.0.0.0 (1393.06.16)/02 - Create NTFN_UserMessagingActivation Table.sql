USE [EKM_App]
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