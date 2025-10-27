USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


INSERT INTO [dbo].[CN_NodeLikes](
	NodeID,
	UserID,
	LikeDate,
	Deleted
)
SELECT DISTINCT Ref.NodeID, Ref.UserID, Ref.Date, 0
FROM [dbo].[NodeUsers] AS Ref 
WHERE UserType = N'Like'


INSERT INTO [dbo].[CN_NodeMembers](
	NodeID,
	UserID,
	MembershipDate,
	IsAdmin,
	Status,
	AcceptionDate,
	Deleted	
)
SELECT DISTINCT Ref.NodeID, Ref.UserID, Ref.Date, 0, N'Accepted', Ref.Date, 0
FROM [dbo].[NodeUsers] AS Ref 
WHERE UserType <> N'Pending' AND UserType <> N'NodeManager' AND 
	NOT EXISTS(SELECT * FROM [dbo].[CN_NodeMembers] AS NM
	WHERE NM.NodeID = Ref.NodeID AND NM.UserID = Ref.UserID)


INSERT INTO [dbo].[CN_NodeMembers](
	NodeID,
	UserID,
	MembershipDate,
	IsAdmin,
	Status,
	AcceptionDate,
	Deleted	
)
SELECT DISTINCT Ref.NodeID, Ref.UserID, Ref.Date, 1, N'Accepted', Ref.Date, 0
FROM [dbo].[NodeUsers] AS Ref 
WHERE UserType = N'NodeManager' AND 
	NOT EXISTS(SELECT * FROM [dbo].[CN_NodeMembers] AS NM
	WHERE NM.NodeID = Ref.NodeID AND NM.UserID = Ref.UserID)


INSERT INTO [dbo].[CN_NodeMembers](
	NodeID,
	UserID,
	MembershipDate,
	IsAdmin,
	Status,
	Deleted	
)
SELECT DISTINCT Ref.NodeID, Ref.UserID, Ref.Date, 0, N'Pending', 0
FROM [dbo].[NodeUsers] AS Ref 
WHERE UserType = N'Pending' AND NOT EXISTS(SELECT * FROM [dbo].[CN_NodeMembers] AS NM
	WHERE NM.NodeID = Ref.NodeID AND NM.UserID = Ref.UserID)
