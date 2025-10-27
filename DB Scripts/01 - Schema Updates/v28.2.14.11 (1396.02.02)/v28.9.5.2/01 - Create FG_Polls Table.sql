USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FG_Polls](
	[PollID] [uniqueidentifier] NOT NULL,
	[IsCopyOfPollID] [uniqueidentifier] NULL,
	[OwnerID] [uniqueidentifier] NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](2000) NULL,
	[BeginDate] [datetime] NULL,
	[FinishDate] [datetime] NULL,
	[ShowSummary] [bit] NOT NULL,
	[HideContributors] [bit] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
	[ApplicationID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_FG_Polls] PRIMARY KEY CLUSTERED 
(
	[PollID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[FG_Polls]  WITH CHECK ADD  CONSTRAINT [FK_FG_Polls_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[FG_Polls] CHECK CONSTRAINT [FK_FG_Polls_aspnet_Applications]
GO

ALTER TABLE [dbo].[FG_Polls]  WITH CHECK ADD  CONSTRAINT [FK_FG_Polls_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[FG_Polls] CHECK CONSTRAINT [FK_FG_Polls_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[FG_Polls]  WITH CHECK ADD  CONSTRAINT [FK_FG_Polls_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[FG_Polls] CHECK CONSTRAINT [FK_FG_Polls_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[FG_Polls]  WITH CHECK ADD  CONSTRAINT [FK_FG_Polls_FG_Polls] FOREIGN KEY([IsCopyOfPollID])
REFERENCES [dbo].[FG_Polls] ([PollID])
GO

ALTER TABLE [dbo].[FG_Polls] CHECK CONSTRAINT [FK_FG_Polls_FG_Polls]
GO