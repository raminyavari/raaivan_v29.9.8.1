USE [EKM_App]
GO

/****** Object:  Table [dbo].[Phrases]    Script Date: 04/26/2013 20:38:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[USR_Profile](
	[UserID] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](256) NULL,
	[LastName] [nvarchar](256) NULL,
	[BirthDay] [datetime] NULL,
	[JobTitle] [nvarchar](500) NULL,
	[Phone] [varchar](20) NULL,
	[Mobile] [varchar](20) NULL,
	[EMail] [varchar](100) NULL,
	[WorkLocation] [nvarchar](500) NULL,
	[Theme] [varchar](50) NULL
 CONSTRAINT [PK_USR_Profile] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[USR_Profile]  WITH CHECK ADD  CONSTRAINT [FK_USR_Profile_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[USR_Profile] CHECK CONSTRAINT [FK_USR_Profile_aspnet_Users]
GO

INSERT INTO [dbo].[USR_Profile](
	UserID,
	FirstName,
	LastName,
	BirthDay,
	JobTitle
)
SELECT UserId, FirstName, Lastname, BirthDay, JobTitle
FROM [dbo].[ProfileCommon]

GO


ALTER TABLE [dbo].[SH_CommentLikes]
DROP CONSTRAINT [FK_SH_CommentLikes_ProfileCommon]
GO

ALTER TABLE [dbo].[SH_CommentLikes]  WITH CHECK ADD  CONSTRAINT [FK_SH_CommentLikes_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[SH_CommentLikes] CHECK CONSTRAINT [FK_SH_CommentLikes_aspnet_Users]
GO


ALTER TABLE [dbo].[SH_Comments]
DROP CONSTRAINT [FK_SH_Comments_ProfileCommon_Sender]
GO

ALTER TABLE [dbo].[SH_Comments]  WITH CHECK ADD  CONSTRAINT [FK_SH_Comments_aspnet_Users_Sender] FOREIGN KEY([SenderUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[SH_Comments] CHECK CONSTRAINT [FK_SH_Comments_aspnet_Users_Sender]
GO


ALTER TABLE [dbo].[SH_Comments]
DROP CONSTRAINT [FK_SH_Comments_ProfileCommon_Modifier]
GO

ALTER TABLE [dbo].[SH_Comments]  WITH CHECK ADD  CONSTRAINT [FK_SH_Comments_aspnet_Users_Modifier] FOREIGN KEY([SenderUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[SH_Comments] CHECK CONSTRAINT [FK_SH_Comments_aspnet_Users_Modifier]
GO


ALTER TABLE [dbo].[SH_Posts]
DROP CONSTRAINT [FK_SH_Posts_ProfileCommon_Sender]
GO

ALTER TABLE [dbo].[SH_Posts]  WITH CHECK ADD  CONSTRAINT [FK_SH_Posts_aspnet_Users_Sender] FOREIGN KEY([SenderUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[SH_Posts] CHECK CONSTRAINT [FK_SH_Posts_aspnet_Users_Sender]
GO


ALTER TABLE [dbo].[SH_Posts]
DROP CONSTRAINT [FK_SH_Posts_ProfileCommon_Modifier]
GO

ALTER TABLE [dbo].[SH_Posts]  WITH CHECK ADD  CONSTRAINT [FK_SH_Posts_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[SH_Posts] CHECK CONSTRAINT [FK_SH_Posts_aspnet_Users_Modifier]
GO


ALTER TABLE [dbo].[SH_PostShares]
DROP CONSTRAINT [FK_SH_PostShares_ProfileCommon_Sender]
GO

ALTER TABLE [dbo].[SH_PostShares]  WITH CHECK ADD  CONSTRAINT [FK_SH_PostShares_aspnet_Users_Sender] FOREIGN KEY([SenderUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[SH_PostShares] CHECK CONSTRAINT [FK_SH_PostShares_aspnet_Users_Sender]
GO


ALTER TABLE [dbo].[SH_PostShares]
DROP CONSTRAINT [FK_SH_PostShares_ProfileCommon_Modifier]
GO

ALTER TABLE [dbo].[SH_PostShares]  WITH CHECK ADD  CONSTRAINT [FK_SH_PostShares_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[SH_PostShares] CHECK CONSTRAINT [FK_SH_PostShares_aspnet_Users_Modifier]
GO


ALTER TABLE [dbo].[SH_ShareLikes]
DROP CONSTRAINT [FK_SH_ShareLikes_ProfileCommon]
GO

ALTER TABLE [dbo].[SH_ShareLikes]  WITH CHECK ADD  CONSTRAINT [FK_SH_ShareLikes_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[SH_ShareLikes] CHECK CONSTRAINT [FK_SH_ShareLikes_aspnet_Users]
GO


ALTER TABLE [dbo].[UserConnections]
DROP CONSTRAINT [FK_UserConnections_ProfileCommon]
GO

ALTER TABLE [dbo].[UserConnections]  WITH CHECK ADD  CONSTRAINT [FK_UserConnections_aspnet_Users] FOREIGN KEY([SenderUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[UserConnections] CHECK CONSTRAINT [FK_UserConnections_aspnet_Users]
GO


ALTER TABLE [dbo].[UserConnections]
DROP CONSTRAINT [FK_UserConnections_ProfileCommon_Receiver]
GO

ALTER TABLE [dbo].[UserConnections]  WITH CHECK ADD  CONSTRAINT [FK_UserConnections_aspnet_Users_Receiver] FOREIGN KEY([ReceiverUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[UserConnections] CHECK CONSTRAINT [FK_UserConnections_aspnet_Users_Receiver]
GO


ALTER TABLE [dbo].[QA_Answers]
DROP CONSTRAINT [FK_QA_Answers_ProfileCommon_Sender]
GO

ALTER TABLE [dbo].[QA_Answers]  WITH CHECK ADD  CONSTRAINT [FK_QA_Answers_aspnet_Users_Sender] FOREIGN KEY([SenderUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[QA_Answers] CHECK CONSTRAINT [FK_QA_Answers_aspnet_Users_Sender]
GO


ALTER TABLE [dbo].[QA_Answers]
DROP CONSTRAINT [FK_QA_Answers_ProfileCommon_Modifier]
GO

ALTER TABLE [dbo].[QA_Answers]  WITH CHECK ADD  CONSTRAINT [FK_QA_Answers_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[QA_Answers] CHECK CONSTRAINT [FK_QA_Answers_aspnet_Users_Modifier]
GO


ALTER TABLE [dbo].[QA_QuestionLikes]
DROP CONSTRAINT [FK_QA_QuestionLikes_ProfileCommon]
GO

ALTER TABLE [dbo].[QA_QuestionLikes]  WITH CHECK ADD  CONSTRAINT [FK_QA_QuestionLikes_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[QA_QuestionLikes] CHECK CONSTRAINT [FK_QA_QuestionLikes_aspnet_Users]
GO


ALTER TABLE [dbo].[QA_Questions]
DROP CONSTRAINT [FK_QA_Questions_ProfileCommon_Sender]
GO

ALTER TABLE [dbo].[QA_Questions]  WITH CHECK ADD  CONSTRAINT [FK_QA_Questions_aspnet_Users_Sender] FOREIGN KEY([SenderUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[QA_Questions] CHECK CONSTRAINT [FK_QA_Questions_aspnet_Users_Sender]
GO


ALTER TABLE [dbo].[QA_Questions]
DROP CONSTRAINT [FK_QA_Questions_ProfileCommon_Modifier]
GO

ALTER TABLE [dbo].[QA_Questions]  WITH CHECK ADD  CONSTRAINT [FK_QA_Questions_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[QA_Questions] CHECK CONSTRAINT [FK_QA_Questions_aspnet_Users_Modifier]
GO


ALTER TABLE [dbo].[QA_RefUsers]
DROP CONSTRAINT [FK_QA_RefUsers_ProfileCommon]
GO

ALTER TABLE [dbo].[QA_RefUsers]  WITH CHECK ADD  CONSTRAINT [FK_QA_RefUsers_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[QA_RefUsers] CHECK CONSTRAINT [FK_QA_RefUsers_aspnet_Users]
GO


ALTER TABLE [dbo].[KW_CreatorUsers]
DROP CONSTRAINT [FK_KW_CreatorUsers_ProfileCommon]
GO

ALTER TABLE [dbo].[KW_CreatorUsers]  WITH CHECK ADD  CONSTRAINT [FK_KW_CreatorUsers_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KW_CreatorUsers] CHECK CONSTRAINT [FK_KW_CreatorUsers_aspnet_Users]
GO


ALTER TABLE [dbo].[KW_Experts]
DROP CONSTRAINT [FK_KW_Experts_ProfileCommon]
GO

ALTER TABLE [dbo].[KW_Experts]  WITH CHECK ADD  CONSTRAINT [FK_KW_Experts_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KW_Experts] CHECK CONSTRAINT [FK_KW_Experts_aspnet_Users]
GO


ALTER TABLE [dbo].[KW_KnowledgeAssets]
DROP CONSTRAINT [FK_KW_KnowledgeAssets_ProfileCommon]
GO

ALTER TABLE [dbo].[KW_KnowledgeAssets]  WITH CHECK ADD  CONSTRAINT [FK_KW_KnowledgeAssets_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KW_KnowledgeAssets] CHECK CONSTRAINT [FK_KW_KnowledgeAssets_aspnet_Users]
GO


ALTER TABLE [dbo].[KW_KnowledgeCards]
DROP CONSTRAINT [FK_KW_KnowledgeCards_ProfileCommon_Sender]
GO

ALTER TABLE [dbo].[KW_KnowledgeCards]  WITH CHECK ADD  CONSTRAINT [FK_KW_KnowledgeCards_aspnet_Users_Sender] FOREIGN KEY([SenderUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KW_KnowledgeCards] CHECK CONSTRAINT [FK_KW_KnowledgeCards_aspnet_Users_Sender]
GO


ALTER TABLE [dbo].[KW_KnowledgeCards]
DROP CONSTRAINT [FK_KW_KnowledgeCards_ProfileCommon_Receiver]
GO

ALTER TABLE [dbo].[KW_KnowledgeCards]  WITH CHECK ADD  CONSTRAINT [FK_KW_KnowledgeCards_aspnet_Users_Receiver] FOREIGN KEY([ReceiverUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KW_KnowledgeCards] CHECK CONSTRAINT [FK_KW_KnowledgeCards_aspnet_Users_Receiver]
GO


ALTER TABLE [dbo].[KW_RefrenceUsers]
DROP CONSTRAINT [FK_KW_RefrenceUsers_ProfileCommon]
GO

ALTER TABLE [dbo].[KW_RefrenceUsers]  WITH CHECK ADD  CONSTRAINT [FK_KW_RefrenceUsers_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KW_RefrenceUsers] CHECK CONSTRAINT [FK_KW_RefrenceUsers_aspnet_Users]
GO


DROP VIEW [dbo].[Users_Normal]
GO


DROP TABLE [dbo].[ProfileCommon]
GO