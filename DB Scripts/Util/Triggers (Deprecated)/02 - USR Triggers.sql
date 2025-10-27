USE [EKM_App]
GO


IF OBJECT_ID ('dbo.USR_TRG_ActiveOrDeactiveUser', 'TR') IS NOT NULL
   DROP TRIGGER dbo.USR_TRG_ActiveOrDeactiveUser
GO

CREATE TRIGGER [dbo].[USR_TRG_ActiveOrDeactiveUser]
ON [dbo].[aspnet_Membership]
AFTER INSERT, UPDATE
AS

DECLARE @TBL GuidBitTableType

INSERT INTO @TBL (FirstValue, SecondValue)
SELECT inserted.UserId, (CASE WHEN ISNULL(inserted.IsApproved, 0) = 1 THEN 0 ELSE 1 END)
FROM inserted
	LEFT JOIN deleted
	ON deleted.UserId = inserted.UserId
WHERE deleted.UserId IS NULL OR ISNULL(inserted.IsApproved, 0) <> ISNULL(deleted.IsApproved, 0)

DECLARE @ApplicationID uniqueidentifier = (SELECT TOP(1) inserted.ApplicationID FROM inserted)

DECLARE @_Result int

EXEC [dbo].[RV_P_SetDeletedStates] @ApplicationID, @TBL, N'User', NULL, @_Result

GO


IF OBJECT_ID ('dbo.USR_TRG_AddRemoveFriend', 'TR') IS NOT NULL
   DROP TRIGGER dbo.USR_TRG_AddRemoveFriend
GO

CREATE TRIGGER [dbo].[USR_TRG_AddRemoveFriend]
ON [dbo].[USR_Friends]
AFTER INSERT, UPDATE
AS

DECLARE @TBL GuidBitTableType

INSERT INTO @TBL (FirstValue, SecondValue)
SELECT	inserted.UniqueID, 
		(CASE WHEN inserted.Deleted = 1 OR inserted.AreFriends = 0 THEN 1 ELSE 0 END)
FROM inserted
	LEFT JOIN deleted
	ON deleted.UniqueID = inserted.UniqueID
WHERE deleted.UniqueID IS NULL OR
	(CASE WHEN inserted.Deleted = 1 OR inserted.AreFriends = 0 THEN 1 ELSE 0 END) <>
	(CASE WHEN deleted.Deleted = 1 OR deleted.AreFriends = 0 THEN 1 ELSE 0 END)

DECLARE @ApplicationID uniqueidentifier = (SELECT TOP(1) inserted.ApplicationID FROM inserted)

DECLARE @_Result int

EXEC [dbo].[RV_P_SetDeletedStates] @ApplicationID, @TBL, N'Friend', NULL, @_Result

GO


IF OBJECT_ID ('dbo.USR_TRG_AddRemoveEmailAddress', 'TR') IS NOT NULL
   DROP TRIGGER dbo.USR_TRG_AddRemoveEmailAddress
GO

CREATE TRIGGER [dbo].[USR_TRG_AddRemoveEmailAddress]
ON [dbo].[USR_EmailAddresses]
AFTER INSERT, UPDATE
AS

DECLARE @TBL GuidBitTableType

INSERT INTO @TBL (FirstValue, SecondValue)
SELECT inserted.EmailID, ISNULL(inserted.Deleted, 0)
FROM inserted
	LEFT JOIN deleted
	ON deleted.EmailID = inserted.EmailID
WHERE deleted.EmailID IS NULL OR ISNULL(inserted.Deleted, 0) <> ISNULL(deleted.Deleted, 0)

DECLARE @ApplicationID uniqueidentifier = (SELECT TOP(1) inserted.ApplicationID FROM inserted)

DECLARE @_Result int

EXEC [dbo].[RV_P_SetDeletedStates] @ApplicationID, @TBL, N'EmailAddress', NULL, @_Result

GO


IF OBJECT_ID ('dbo.USR_TRG_AddRemoveEmailContact', 'TR') IS NOT NULL
   DROP TRIGGER dbo.USR_TRG_AddRemoveEmailContact
GO

CREATE TRIGGER [dbo].[USR_TRG_AddRemoveEmailContact]
ON [dbo].[USR_EmailContacts]
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

EXEC [dbo].[RV_P_SetDeletedStates] @ApplicationID, @TBL, N'EmailContact', NULL, @_Result

GO


IF OBJECT_ID ('dbo.USR_TRG_AddItemVisit', 'TR') IS NOT NULL
   DROP TRIGGER dbo.USR_TRG_AddItemVisit
GO

CREATE TRIGGER [dbo].[USR_TRG_AddItemVisit]
ON [dbo].[USR_ItemVisits]
AFTER INSERT
AS

DECLARE @TBL GuidBitTableType

INSERT INTO @TBL (FirstValue, SecondValue)
SELECT inserted.UniqueID, 0
FROM inserted

DECLARE @ApplicationID uniqueidentifier = (SELECT TOP(1) inserted.ApplicationID FROM inserted)

DECLARE @_Result int

EXEC [dbo].[RV_P_SetDeletedStates] @ApplicationID, @TBL, N'ItemVisit', NULL, @_Result

GO


IF OBJECT_ID ('dbo.USR_TRG_SendInvitation', 'TR') IS NOT NULL
   DROP TRIGGER dbo.USR_TRG_SendInvitation
GO

CREATE TRIGGER [dbo].[USR_TRG_SendInvitation]
ON [dbo].[USR_Invitations]
AFTER INSERT
AS

DECLARE @TBL GuidBitTableType

INSERT INTO @TBL (FirstValue, SecondValue)
SELECT inserted.ID, 0
FROM inserted

DECLARE @ApplicationID uniqueidentifier = (SELECT TOP(1) inserted.ApplicationID FROM inserted)

DECLARE @_Result int

EXEC [dbo].[RV_P_SetDeletedStates] @ApplicationID, @TBL, N'Invitation', NULL, @_Result

GO