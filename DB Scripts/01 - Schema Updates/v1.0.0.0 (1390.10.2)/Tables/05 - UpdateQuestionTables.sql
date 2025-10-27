USE [EKM_App]
GO

/****** Object:  Table [dbo].[QuestionLikes]    Script Date: 10/23/2011 11:37:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[QuestionLikes](
	[UserID] [uniqueidentifier] NOT NULL,
	[QuestionID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_QuestionLikes] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[QuestionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[QuestionLikes]  WITH CHECK ADD  CONSTRAINT [FK_QuestionLikes_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[QuestionLikes] CHECK CONSTRAINT [FK_QuestionLikes_aspnet_Users]
GO

ALTER TABLE [dbo].[QuestionLikes]  WITH CHECK ADD  CONSTRAINT [FK_QuestionLikes_QQuestions] FOREIGN KEY([QuestionID])
REFERENCES [dbo].[QQuestions] ([ID])
GO

ALTER TABLE [dbo].[QuestionLikes] CHECK CONSTRAINT [FK_QuestionLikes_QQuestions]
GO


/****** Object:  Table [dbo].[QQuestions]    Script Date: 10/23/2011 11:33:04 ******/
ALTER TABLE [dbo].[QQuestions]
ADD [VisitsCount] [bigint] NULL;
GO