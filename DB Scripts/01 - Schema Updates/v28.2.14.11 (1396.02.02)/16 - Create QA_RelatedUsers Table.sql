USE [EKM_App]
GO

/****** Object:  Table [dbo].[QA_RelatedUsers]    Script Date: 11/29/2016 09:33:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[QA_RelatedUsers](
	[UserID] [uniqueidentifier] NOT NULL,
	[QuestionID] [uniqueidentifier] NOT NULL,
	[SenderUserID] [uniqueidentifier] NOT NULL,
	[SendDate] [datetime] NOT NULL,
	[Seen] [bit] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
	[ApplicationID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_QA_RelatedUsers] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[QuestionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[QA_RelatedUsers]  WITH CHECK ADD  CONSTRAINT [FK_QA_RelatedUsers_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[QA_RelatedUsers] CHECK CONSTRAINT [FK_QA_RelatedUsers_aspnet_Applications]
GO

ALTER TABLE [dbo].[QA_RelatedUsers]  WITH CHECK ADD  CONSTRAINT [FK_QA_RelatedUsers_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[QA_RelatedUsers] CHECK CONSTRAINT [FK_QA_RelatedUsers_aspnet_Users]
GO

ALTER TABLE [dbo].[QA_RelatedUsers]  WITH CHECK ADD  CONSTRAINT [FK_QA_RelatedUsers_aspnet_Users_Sender] FOREIGN KEY([SenderUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[QA_RelatedUsers] CHECK CONSTRAINT [FK_QA_RelatedUsers_aspnet_Users_Sender]
GO

ALTER TABLE [dbo].[QA_RelatedUsers]  WITH CHECK ADD  CONSTRAINT [FK_QA_RelatedUsers_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[QA_RelatedUsers]  WITH CHECK ADD  CONSTRAINT [FK_QA_RelatedUsers_QA_Questions] FOREIGN KEY([QuestionID])
REFERENCES [dbo].[QA_Questions] ([QuestionID])
GO

ALTER TABLE [dbo].[QA_RelatedUsers] CHECK CONSTRAINT [FK_QA_RelatedUsers_QA_Questions]
GO