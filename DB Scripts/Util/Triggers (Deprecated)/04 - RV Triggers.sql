USE [EKM_App]
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

DECLARE @ApplicationID uniqueidentifier = (SELECT TOP(1) inserted.ApplicationID FROM inserted)

DECLARE @_Result int

EXEC [dbo].[RV_P_SetDeletedStates] @ApplicationID, @TBL, N'TaggedItem', NULL, @_Result

GO