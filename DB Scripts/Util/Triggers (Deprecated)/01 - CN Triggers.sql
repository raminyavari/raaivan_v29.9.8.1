USE [EKM_App]
GO


IF OBJECT_ID ('dbo.CN_TRG_AddOrRemoveNodeType', 'TR') IS NOT NULL
   DROP TRIGGER dbo.CN_TRG_AddOrRemoveNodeType
GO

CREATE TRIGGER [dbo].[CN_TRG_AddOrRemoveNodeType]
ON [dbo].[CN_NodeTypes]
AFTER INSERT, UPDATE
AS

DECLARE @TBL GuidBitTableType

INSERT INTO @TBL (FirstValue, SecondValue)
SELECT inserted.NodeTypeID, ISNULL(inserted.Deleted, 0)
FROM inserted
	LEFT JOIN deleted
	ON deleted.NodeTypeID = inserted.NodeTypeID
WHERE deleted.NodeTypeID IS NULL OR ISNULL(inserted.Deleted, 0) <> ISNULL(deleted.Deleted, 0)

DECLARE @ApplicationID uniqueidentifier = (SELECT TOP(1) inserted.ApplicationID FROM inserted)

DECLARE @_Result int

EXEC [dbo].[RV_P_SetDeletedStates] @ApplicationID, @TBL, N'NodeType', NULL, @_Result

GO


IF OBJECT_ID ('dbo.CN_TRG_AddOrRemoveNode', 'TR') IS NOT NULL
   DROP TRIGGER dbo.CN_TRG_AddOrRemoveNode
GO

CREATE TRIGGER [dbo].[CN_TRG_AddOrRemoveNode]
ON [dbo].[CN_Nodes]
AFTER INSERT, UPDATE
AS

DECLARE @TBL GuidBitTableType

INSERT INTO @TBL (FirstValue, SecondValue)
SELECT inserted.NodeID, ISNULL(inserted.Deleted, 0)
FROM inserted
	LEFT JOIN deleted
	ON deleted.NodeID = inserted.NodeID
WHERE deleted.NodeID IS NULL OR ISNULL(inserted.Deleted, 0) <> ISNULL(deleted.Deleted, 0)

DECLARE @ApplicationID uniqueidentifier = (SELECT TOP(1) inserted.ApplicationID FROM inserted)

DECLARE @_Result int

EXEC [dbo].[RV_P_SetDeletedStates] @ApplicationID, @TBL, N'Node', NULL, @_Result

GO


IF OBJECT_ID ('dbo.CN_TRG_AddOrRemoveNodeMember', 'TR') IS NOT NULL
   DROP TRIGGER dbo.CN_TRG_AddOrRemoveNodeMember
GO

CREATE TRIGGER [dbo].[CN_TRG_AddOrRemoveNodeMember]
ON [dbo].[CN_NodeMembers]
AFTER INSERT, UPDATE
AS

DECLARE @TBL GuidBitTableType

INSERT INTO @TBL (FirstValue, SecondValue)
SELECT inserted.UniqueID, ISNULL(inserted.Deleted, 0)
FROM inserted
	LEFT JOIN deleted
	ON deleted.UniqueID = inserted.UniqueID
WHERE deleted.UniqueID IS NULL OR ISNULL(inserted.Deleted, 0) <> ISNULL(deleted.Deleted, 0)

DECLARE @ApplicationID uniqueidentifier = (SELECT TOP(1) inserted.ApplicationID FROM inserted)

DECLARE @_Result int

EXEC [dbo].[RV_P_SetDeletedStates] @ApplicationID, @TBL, N'NodeMember', NULL, @_Result

GO


IF OBJECT_ID ('dbo.CN_TRG_AddOrRemoveExpert', 'TR') IS NOT NULL
   DROP TRIGGER dbo.CN_TRG_AddOrRemoveExpert
GO

CREATE TRIGGER [dbo].[CN_TRG_AddOrRemoveExpert]
ON [dbo].[CN_Experts]
AFTER INSERT, UPDATE
AS

DECLARE @TBL GuidBitTableType

INSERT INTO @TBL (FirstValue, SecondValue)
SELECT	inserted.UniqueID, 
		(CASE WHEN inserted.Approved = 1 OR inserted.SocialApproved = 1 THEN 0 ELSE 1 END)
FROM inserted
	LEFT JOIN deleted
	ON deleted.UniqueID = inserted.UniqueID
WHERE deleted.UniqueID IS NULL OR 
	(CASE WHEN inserted.Approved = 1 OR inserted.SocialApproved = 1 THEN 0 ELSE 1 END) <>
	(CASE WHEN deleted.Approved = 1 OR deleted.SocialApproved = 1 THEN 0 ELSE 1 END)

DECLARE @ApplicationID uniqueidentifier = (SELECT TOP(1) inserted.ApplicationID FROM inserted)

DECLARE @_Result int

EXEC [dbo].[RV_P_SetDeletedStates] @ApplicationID, @TBL, N'Expert', NULL, @_Result

GO


IF OBJECT_ID ('dbo.CN_TRG_AddOrRemoveNodeLike', 'TR') IS NOT NULL
   DROP TRIGGER dbo.CN_TRG_AddOrRemoveNodeLike
GO

CREATE TRIGGER [dbo].[CN_TRG_AddOrRemoveNodeLike]
ON [dbo].[CN_NodeLikes]
AFTER INSERT, UPDATE
AS

DECLARE @TBL GuidBitTableType

INSERT INTO @TBL (FirstValue, SecondValue)
SELECT inserted.UniqueID, ISNULL(inserted.Deleted, 0)
FROM inserted
	LEFT JOIN deleted
	ON deleted.UniqueID = inserted.UniqueID
WHERE deleted.UniqueID IS NULL OR ISNULL(inserted.Deleted, 0) <> ISNULL(deleted.Deleted, 0)

DECLARE @ApplicationID uniqueidentifier = (SELECT TOP(1) inserted.ApplicationID FROM inserted)

DECLARE @_Result int

EXEC [dbo].[RV_P_SetDeletedStates] @ApplicationID, @TBL, N'NodeLike', NULL, @_Result

GO


IF OBJECT_ID ('dbo.CN_TRG_AddOrRemoveNodeRelation', 'TR') IS NOT NULL
   DROP TRIGGER dbo.CN_TRG_AddOrRemoveNodeRelation
GO

CREATE TRIGGER [dbo].[CN_TRG_AddOrRemoveNodeRelation]
ON [dbo].[CN_NodeRelations]
AFTER INSERT, UPDATE
AS

DECLARE @TBL GuidBitTableType

INSERT INTO @TBL (FirstValue, SecondValue)
SELECT inserted.UniqueID, ISNULL(inserted.Deleted, 0)
FROM inserted
	LEFT JOIN deleted
	ON deleted.UniqueID = inserted.UniqueID
WHERE deleted.UniqueID IS NULL OR ISNULL(inserted.Deleted, 0) <> ISNULL(deleted.Deleted, 0)

DECLARE @ApplicationID uniqueidentifier = (SELECT TOP(1) inserted.ApplicationID FROM inserted)

DECLARE @_Result int

EXEC [dbo].[RV_P_SetDeletedStates] @ApplicationID, @TBL, N'NodeRelation', NULL, @_Result

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

DECLARE @ApplicationID uniqueidentifier = (SELECT TOP(1) inserted.ApplicationID FROM inserted)

DECLARE @_Result int

EXEC [dbo].[RV_P_SetDeletedStates] @ApplicationID, @TBL, N'NodeCreator', NULL, @_Result

GO