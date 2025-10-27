USE [EKM_App]
GO



INSERT INTO [dbo].[RV_DeletedStates](ObjectID, ObjectType, Deleted)
SELECT UniqueID, N'NodeCreator', Deleted
FROM [dbo].[CN_NodeCreators]

GO

