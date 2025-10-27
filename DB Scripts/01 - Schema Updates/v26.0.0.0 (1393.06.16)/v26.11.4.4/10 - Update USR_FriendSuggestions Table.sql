USE [EKM_App]
GO

/****** Object:  Table [dbo].[CN_NodeLikes]    Script Date: 08/01/2015 20:15:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

DROP TABLE [USR_FriendSuggestions]
GO


CREATE TABLE [dbo].[USR_FriendSuggestions](
	[UserID] [uniqueidentifier] NOT NULL,
	[SuggestedUserID] [uniqueidentifier] NOT NULL,
	[Score] [float] NOT NULL,
 CONSTRAINT [PK_USR_TMPFriendSuggestions] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[SuggestedUserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[USR_FriendSuggestions]  WITH CHECK ADD  CONSTRAINT [FK_USR_FriendSuggestions_aspnet_Users_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[USR_FriendSuggestions] CHECK CONSTRAINT [FK_USR_FriendSuggestions_aspnet_Users_UserID]
GO

ALTER TABLE [dbo].[USR_FriendSuggestions]  WITH CHECK ADD  CONSTRAINT [FK_USR_FriendSuggestions_aspnet_Users_SuggestedUserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[USR_FriendSuggestions] CHECK CONSTRAINT [FK_USR_FriendSuggestions_aspnet_Users_SuggestedUserID]
GO



