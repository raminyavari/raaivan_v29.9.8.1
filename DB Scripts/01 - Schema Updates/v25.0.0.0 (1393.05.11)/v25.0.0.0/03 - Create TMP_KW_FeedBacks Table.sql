USE [EKM_App]
GO

/****** Object:  Table [dbo].[Phrases]    Script Date: 04/26/2013 20:38:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TABLE [dbo].[TMP_KW_FeedBacks](
	[FeedBackID] [bigint] IDENTITY(1,1) NOT NULL,
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[FeedBackTypeID] [int] NOT NULL,
	[SendDate] [datetime] NOT NULL,
	[Value] [float] NOT NULL,
	[Description] [nvarchar](2000) NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_TMP_KW_FeedBacks] PRIMARY KEY CLUSTERED 
(
	[FeedBackID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TMP_KW_FeedBacks]  WITH CHECK ADD  CONSTRAINT [FK_TMP_KW_FeedBacks_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[TMP_KW_FeedBacks] CHECK CONSTRAINT [FK_TMP_KW_FeedBacks_aspnet_Users]
GO

ALTER TABLE [dbo].[TMP_KW_FeedBacks]  WITH CHECK ADD  CONSTRAINT [FK_TMP_KW_FeedBacks_CN_Nodes] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[TMP_KW_FeedBacks] CHECK CONSTRAINT [FK_TMP_KW_FeedBacks_CN_Nodes]
GO
