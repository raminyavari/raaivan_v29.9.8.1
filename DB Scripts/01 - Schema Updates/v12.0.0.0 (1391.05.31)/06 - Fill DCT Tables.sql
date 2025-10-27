USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


INSERT INTO [dbo].[DCT_Trees]
           ([TreeID]
           ,[TreeTypeID]
           ,[IsPrivate]
           ,[Name]
           ,[Description]
           ,[CreatorUserID]
           ,[CreationDate]
           ,[Privacy]
           ,[Deleted])
SELECT  [dbo].[ContentTrees].[ID], 1, 0, [dbo].[ContentTrees].[Title],
		[dbo].[ContentTrees].[Description],
		(SELECT TOP(1) [dbo].[aspnet_Users].[UserId] FROM [dbo].[aspnet_Users]),
		GETDATE(), N'Public', 0
FROM	[dbo].[ContentTrees]
GO



INSERT INTO [dbo].[DCT_TreeNodes]
           ([TreeNodeID]
           ,[TreeID]
           ,[ParentNodeID]
           ,[Name]
           ,[Description]
           ,[CreatorUserID]
           ,[CreationDate]
           ,[Privacy]
           ,[Deleted])
SELECT  [dbo].[ContentTreeNodes].[ID], [dbo].[ContentTreeNodes].[ContentTreeID], 
		[dbo].[ContentTreeNodes].[ParentID], [dbo].[ContentTreeNodes].[Title],
		[dbo].[ContentTreeNodes].[Description], 
		(SELECT TOP(1) [dbo].[aspnet_Users].[UserId] FROM [dbo].[aspnet_Users]),
		GETDATE(), N'Public', 0
FROM	[dbo].[ContentTreeNodes]
WHERE	[dbo].[ContentTreeNodes].[ContentTreeID] IS NOT NULL
GO