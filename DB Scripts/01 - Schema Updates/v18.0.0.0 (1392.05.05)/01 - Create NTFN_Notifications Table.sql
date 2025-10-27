USE [EKM_App]
GO

/****** Object:  Table [dbo].[Phrases]    Script Date: 04/26/2013 20:38:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[NTFN_Notifications](
	[ID] [bigint] IDENTITY(1,1),
	[UserID] [uniqueidentifier] NOT NULL,
	[SubjectID] [uniqueidentifier] NOT NULL,
	[RefItemID] [uniqueidentifier] NOT NULL,
	[SubjectType] [varchar](20) NOT NULL,
	[SubjectName] [nvarchar](2000) NULL,
	[Action] [varchar](20) NOT NULL,
	[SenderUserID] [uniqueidentifier] NULL,
	[SendDate] [datetime] NOT NULL,
	[Description] [nvarchar](2000),
	[Seen] [bit] NOT NULL,
	[ViewDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_NTFN_Notifications] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[NTFN_Notifications]  WITH CHECK ADD  CONSTRAINT [FK_NTFN_Notifications_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[NTFN_Notifications] CHECK CONSTRAINT [FK_NTFN_Notifications_aspnet_Users]
GO


ALTER TABLE [dbo].[NTFN_Notifications]  WITH CHECK ADD  CONSTRAINT [FK_NTFN_Notifications_aspnet_Users_Sender] FOREIGN KEY([SenderUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[NTFN_Notifications] CHECK CONSTRAINT [FK_NTFN_Notifications_aspnet_Users_Sender]
GO