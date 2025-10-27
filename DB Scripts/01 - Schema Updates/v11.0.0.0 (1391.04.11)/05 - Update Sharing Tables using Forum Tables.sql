USE [EKM_App]

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


INSERT INTO [dbo].[SH_Posts](
	PostID,
	PostTypeID,
	Description,
	SenderUserID,
	SendDate,
	Deleted
)
SELECT [dbo].[ForumTopics].[ID], 1, 
	   [dbo].[ForumTopics].[Title] + '...' + [dbo].[ForumTopics].[FullText],
	   [dbo].[ForumTopics].[UserId], [dbo].[ForumTopics].[DateAdded], 0
FROM [dbo].[ForumTopics]


INSERT INTO [dbo].[SH_PostShares](
	ShareID,
	PostID,
	OwnerID,
	SenderUserID,
	SendDate,
	Privacy,
	OwnerType,
	Deleted
)
SELECT NEWID(), [dbo].[ForumTopics].[ID], [dbo].[ForumSections].[NodeID] ,
	   [dbo].[ForumTopics].[UserId], [dbo].[ForumTopics].[DateAdded], N'Public', N'Node', 0
FROM [dbo].[ForumTopics] INNER JOIN [dbo].[ForumSections] ON
	 [dbo].[ForumTopics].[SectionId] = [dbo].[ForumSections].[ID]


/*
INSERT INTO [dbo].[SH_Comments](
	CommentID,
	ShareID,
	Description,
	SenderUserID,
	SendDate,
	Deleted
)
SELECT [dbo].[ForumPosts].[ID], [dbo].[SH_PostShares].[ShareID], 
	   [dbo].[ForumPosts].[Title] + '...' + [dbo].[ForumPosts].[FullText],
	   [dbo].[ForumPosts].[UserId], [dbo].[ForumPosts].[DateAdded], 0
FROM [dbo].[ForumPosts] INNER JOIN [dbo].[SH_PostShares] ON
	 [dbo].[ForumPosts].[TopicId] = [dbo].[SH_PostShares].[PostID]
*/