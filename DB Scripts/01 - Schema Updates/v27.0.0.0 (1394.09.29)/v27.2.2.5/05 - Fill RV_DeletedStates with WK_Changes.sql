USE [EKM_App]
GO


INSERT INTO [dbo].[RV_DeletedStates](ObjectID, ObjectType, Deleted)
SELECT C.ChangeID, N'WikiChange', 0
FROM [dbo].[WK_Changes] AS C
	INNER JOIN [dbo].[WK_Paragraphs] AS P
	ON P.ParagraphID = C.ParagraphID
	INNER JOIN [dbo].[WK_Titles] AS T
	ON T.TitleID = P.TitleID
	INNER JOIN [dbo].[CN_Nodes] AS ND
	ON ND.NodeID = T.OwnerID

GO

