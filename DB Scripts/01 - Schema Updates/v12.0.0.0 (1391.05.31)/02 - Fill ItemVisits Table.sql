USE [EKM_App]

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



INSERT INTO [dbo].[USR_ItemVisits](
	ItemID,
	UserID,
	VisitDate,
	ItemType
)
SELECT Ref.VisitedUserId, Ref.VisitorUserId, Ref.Date, N'User'
FROM [dbo].[UserPersonalPageVisitors] AS Ref


INSERT INTO [dbo].[USR_ItemVisits](
	ItemID,
	UserID,
	VisitDate,
	ItemType
)
SELECT Ref.KKnowledgeId, Ref.VisitorUserId, Ref.Date, N'Knowledge'
FROM [dbo].[KnowledgePageVisitors] AS Ref


INSERT INTO [dbo].[USR_ItemVisits](
	ItemID,
	UserID,
	VisitDate,
	ItemType
)
SELECT Ref.NodeID, Ref.UserID, Ref.VisitDate, N'Node'
FROM [dbo].[NodePageVisitors] AS Ref