USE [EKM_App]
GO


IF OBJECT_ID ('dbo.CN_TRG_AddOrRemoveNodeCreator', 'TR') IS NOT NULL
   DROP TRIGGER dbo.CN_TRG_AddOrRemoveNodeCreator
GO

CREATE TRIGGER [dbo].[CN_TRG_AddOrRemoveNodeCreator]
ON [dbo].[CN_NodeCreators]
AFTER INSERT, UPDATE
AS

DECLARE @TBL GuidBitTableType

INSERT INTO @TBL (FirstValue, SecondValue)
SELECT inserted.UniqueID, ISNULL(inserted.Deleted, 0)
FROM inserted
	LEFT JOIN deleted
	ON deleted.UniqueID = inserted.UniqueID
WHERE deleted.UniqueID IS NULL OR ISNULL(inserted.Deleted, 0) <> ISNULL(deleted.Deleted, 0)

DECLARE @_Result int

EXEC [dbo].[RV_P_SetDeletedStates] @TBL, N'NodeCreator', NULL, @_Result

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

DECLARE @_Result int

EXEC [dbo].[RV_P_SetDeletedStates] @TBL, N'WikiChange', NULL, @_Result

GO



IF OBJECT_ID ('dbo.RV_TRG_AddTaggedItem', 'TR') IS NOT NULL
   DROP TRIGGER dbo.RV_TRG_AddTaggedItem
GO

CREATE TRIGGER [dbo].[RV_TRG_AddTaggedItem]
ON [dbo].[RV_TaggedItems]
AFTER INSERT
AS

DECLARE @TBL GuidBitTableType

INSERT INTO @TBL (FirstValue, SecondValue)
SELECT inserted.UniqueID, 0
FROM inserted

DECLARE @_Result int

EXEC [dbo].[RV_P_SetDeletedStates] @TBL, N'TaggedItem', NULL, @_Result

GO

