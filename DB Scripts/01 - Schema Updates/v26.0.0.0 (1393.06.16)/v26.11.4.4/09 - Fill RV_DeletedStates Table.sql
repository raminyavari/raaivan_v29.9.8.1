USE [EKM_App]
GO


INSERT INTO [dbo].[RV_DeletedStates](ObjectID, ObjectType, Deleted)
SELECT NodeTypeID, N'NodeType', Deleted
FROM [dbo].[CN_NodeTypes]

GO


INSERT INTO [dbo].[RV_DeletedStates](ObjectID, ObjectType, Deleted)
SELECT NodeID, N'Node', Deleted
FROM [dbo].[CN_Nodes]

GO


INSERT INTO [dbo].[RV_DeletedStates](ObjectID, ObjectType, Deleted)
SELECT UniqueID, N'NodeMember', Deleted
FROM [dbo].[CN_NodeMembers]

GO


INSERT INTO [dbo].[RV_DeletedStates](ObjectID, ObjectType, Deleted)
SELECT UniqueID, N'Expert', (CASE WHEN Approved = 1 OR SocialApproved = 1 THEN 1 ELSE 0 END)
FROM [dbo].[CN_Experts]

GO


INSERT INTO [dbo].[RV_DeletedStates](ObjectID, ObjectType, Deleted)
SELECT UniqueID, N'NodeLike', Deleted
FROM [dbo].[CN_NodeLikes]

GO


INSERT INTO [dbo].[RV_DeletedStates](ObjectID, ObjectType, Deleted)
SELECT UniqueID, N'NodeRelation', Deleted
FROM [dbo].[CN_NodeRelations]

GO


INSERT INTO [dbo].[RV_DeletedStates](ObjectID, ObjectType, Deleted)
SELECT UserId, N'User', (CASE WHEN ISNULL(IsApproved, 0) = 1 THEN 0 ELSE 1 END)
FROM [dbo].[aspnet_Membership]

GO


INSERT INTO [dbo].[RV_DeletedStates](ObjectID, ObjectType, Deleted)
SELECT UniqueID, N'Friend', (CASE WHEN Deleted = 1 OR AreFriends = 0 THEN 1 ELSE 0 END)
FROM [dbo].[USR_Friends]

GO


INSERT INTO [dbo].[RV_DeletedStates](ObjectID, ObjectType, Deleted)
SELECT EmailID, N'EmailAddress', Deleted
FROM [dbo].[USR_EmailAddresses]

GO


INSERT INTO [dbo].[RV_DeletedStates](ObjectID, ObjectType, Deleted)
SELECT UniqueID, N'EmailContact', Deleted
FROM [dbo].[USR_EmailContacts]

GO


INSERT INTO [dbo].[RV_DeletedStates](ObjectID, ObjectType, Deleted)
SELECT UniqueID, N'ItemVisit', 0
FROM [dbo].[USR_ItemVisits]

GO


INSERT INTO [dbo].[RV_DeletedStates](ObjectID, ObjectType, Deleted)
SELECT ID, N'Invitation', 0
FROM [dbo].[USR_Invitations]

GO