USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


INSERT INTO [dbo].[EVT_Events](
	EventID,
	EventType,
	Title,
	Description,
	BeginDate,
	FinishDate,
	CreatorUserID,
	CreationDate,
	Deleted
)
SELECT Ref.ID, Ref.Type, Ref.Title, Ref.Description, Ref.StartDate, Ref.EndDate,
	Ref.CreatorUserID, Ref.CreateDate, 0
FROM [dbo].[Calendars] AS Ref
WHERE Ref.Title IS NOT NULL AND Ref.CreateDate IS NOT NULL

GO


INSERT INTO [dbo].[EVT_RelatedUsers](
	EventID,
	UserID,
	Status,
	Done,
	Deleted
)
SELECT Ref.CalenderID, Ref.UserID, Ref.Status, Ref.Done, 0
FROM [dbo].[Calender_Users] AS Ref
WHERE Ref.Done IS NOT NULL

GO

INSERT INTO [dbo].[EVT_RelatedUsers](
	EventID,
	UserID,
	Status,
	Done,
	Deleted
)
SELECT Ref.CalenderID, Ref.UserID, Ref.Status, 0, 0
FROM [dbo].[Calender_Users] AS Ref
WHERE Ref.Done IS NULL

GO


INSERT INTO [dbo].[EVT_RelatedNodes](
	EventID,
	NodeID,
	Deleted
)
SELECT DISTINCT Ref.CalenderID, Ref.NodeID, 0
FROM [dbo].[CalenderNodes] AS Ref

GO


INSERT INTO [dbo].[EVT_RelatedNodes](
	EventID,
	NodeID,
	Deleted
)
SELECT DISTINCT Ref.CalendarID, Ref.KnowledgeID, 0
FROM [dbo].[CalendarKnowledges] AS Ref

GO