USE [EKM_App]
GO


/****** Object:  Table [dbo].[QA_Comments]    Script Date: 11/29/2016 08:54:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[QA_Comments](
	[CommentID] [uniqueidentifier] NOT NULL,
	[OwnerID] [uniqueidentifier] NOT NULL,
	[ReplyToCommentID] [uniqueidentifier] NULL,
	[BodyText] [nvarchar](max) NOT NULL,
	[SenderUserID] [uniqueidentifier] NOT NULL,
	[SendDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
	[ApplicationID] [uniqueidentifier] NULL
 CONSTRAINT [PK_QA_Comments] PRIMARY KEY CLUSTERED 
(
	[CommentID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[QA_Comments]  WITH CHECK ADD  CONSTRAINT [FK_QA_Comments_QA_Comments] FOREIGN KEY([ReplyToCommentID])
REFERENCES [dbo].[QA_Comments] ([CommentID])
GO

ALTER TABLE [dbo].[QA_Comments] CHECK CONSTRAINT [FK_QA_Comments_QA_Comments]
GO

ALTER TABLE [dbo].[QA_Comments]  WITH CHECK ADD  CONSTRAINT [FK_QA_Comments_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[QA_Comments] CHECK CONSTRAINT [FK_QA_Comments_aspnet_Applications]
GO

ALTER TABLE [dbo].[QA_Comments]  WITH CHECK ADD  CONSTRAINT [FK_QA_Comments_aspnet_Users_Sender] FOREIGN KEY([SenderUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[QA_Comments] CHECK CONSTRAINT [FK_QA_Comments_aspnet_Users_Sender]
GO

ALTER TABLE [dbo].[QA_Comments]  WITH CHECK ADD  CONSTRAINT [FK_QA_Comments_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[QA_Comments] CHECK CONSTRAINT [FK_QA_Comments_aspnet_Users_Modifier]
GO


