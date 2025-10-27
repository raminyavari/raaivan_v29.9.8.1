USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


DECLARE @UserID uniqueidentifier
SET @UserID = (SELECT TOP(1) WE.UserID FROM [dbo].[WikiSubjectExperts] AS WE)
IF @UserID IS NULL SET @UserID = (SELECT TOP(1) AU.UserId FROM [dbo].[aspnet_Users] AS AU)

INSERT INTO [dbo].[WK_Subjects](
	SubjectID,
	OwnerID,
	CreatorUserID,
	CreationDate,
	Title,
	Description,
	Status,
	OwnerType,
	Deleted
)
SELECT Ref.ID, Ref.NodeID, @UserID, GETDATE(), Ref.Title, Ref.Description, 
	Ref.Status, N'Node', 0
FROM [dbo].[WikiSubjects] AS Ref



INSERT INTO [dbo].[WK_Changes](
	SubjectID,
	UserID,
	SendDate,
	Title,
	Description,
	Applied,
	Status,
	Deleted
)
SELECT Ref.WikiSubjectID, Ref.UserID, Ref.CreationDate, Ref.Title, 
	Ref.Description, 0, N'Pending', 0
FROM [dbo].[WikiChanges] AS Ref



INSERT INTO [dbo].[WK_Evaluators](
	UserID,
	SubjectID,
	SenderUserID
)
SELECT Ref.UserID, Ref.WikiSubjectID, Ref.SenderID
FROM [dbo].[WikiSubjectExperts] AS Ref