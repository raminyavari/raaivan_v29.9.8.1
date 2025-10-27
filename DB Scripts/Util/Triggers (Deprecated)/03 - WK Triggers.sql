USE [EKM_App]
GO


IF OBJECT_ID ('dbo.WK_TRG_AddChange', 'TR') IS NOT NULL
   DROP TRIGGER dbo.WK_TRG_AddChange
GO

CREATE TRIGGER [dbo].[WK_TRG_AddChange]
ON [dbo].[WK_Changes]
AFTER INSERT
AS

DECLARE @TBL GuidBitTableType

INSERT INTO @TBL (FirstValue, SecondValue)
SELECT inserted.ChangeID, 0
FROM inserted
	INNER JOIN [dbo].[WK_Paragraphs] AS P
	ON P.ParagraphID = inserted.ParagraphID
	INNER JOIN [dbo].[WK_Titles] AS T
	ON T.TitleID = P.TitleID
	INNER JOIN [dbo].[CN_Nodes] AS ND
	ON ND.NodeID = T.OwnerID
	
DECLARE @ApplicationID uniqueidentifier = (SELECT TOP(1) inserted.ApplicationID FROM inserted)

DECLARE @_Result int

EXEC [dbo].[RV_P_SetDeletedStates] @ApplicationID, @TBL, N'WikiChange', NULL, @_Result

GO