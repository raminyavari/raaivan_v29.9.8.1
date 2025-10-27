USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



INSERT INTO [dbo].[KW_ConfidentialityLevels](
	LevelID,
	Title
)
SELECT [dbo].[Classifications].[ClassificationID], [dbo].[Classifications].[Title]
FROM [dbo].[Classifications]
GO



INSERT INTO [dbo].[KW_UsersConfidentialityLevels](
	UserID,
	LevelID
)
SELECT [dbo].[UsersClassifications].[UserID], [dbo].[UsersClassifications].[ClassificationID]
FROM [dbo].[UsersClassifications]
GO



INSERT INTO [dbo].[CN_Nodes]
           ([NodeID]
		   ,[NodeTypeID]
           ,[Name]
           ,[Description]
           ,[CreatorUserID]
           ,[CreationDate]
           ,[LastModifierUserID]
           ,[LastModificationDate]
           ,[Deleted])
SELECT  [dbo].[KKnowledges].[ID], [dbo].[CN_NodeTypes].[NodeTypeID],
		[dbo].[KKnowledges].[Title], [dbo].[KKnowledges].[Description],
		[dbo].[KKnowledges].[OwnerUserID], [dbo].[KKnowledges].[CreationDate],
		[dbo].[KKnowledges].[LastModifyerID], [dbo].[KKnowledges].[LastModifyedDate],
		[dbo].[KKnowledges].[Deleted]
FROM    [dbo].[KKnowledges] INNER JOIN [dbo].[CN_NodeTypes] ON
		[dbo].[CN_NodeTypes].[AdditionalID] = '5'
WHERE [dbo].[KKnowledges].[Deleted] IS NOT NULL
GO


INSERT INTO [dbo].[CN_Nodes]
           ([NodeID]
		   ,[NodeTypeID]
           ,[Name]
           ,[Description]
           ,[CreatorUserID]
           ,[CreationDate]
           ,[LastModifierUserID]
           ,[LastModificationDate]
           ,[Deleted])
SELECT  [dbo].[KKnowledges].[ID], [dbo].[CN_NodeTypes].[NodeTypeID],
		[dbo].[KKnowledges].[Title], [dbo].[KKnowledges].[Description],
		[dbo].[KKnowledges].[OwnerUserID], [dbo].[KKnowledges].[CreationDate],
		[dbo].[KKnowledges].[LastModifyerID], [dbo].[KKnowledges].[LastModifyedDate], 0
FROM    [dbo].[KKnowledges] INNER JOIN [dbo].[CN_NodeTypes] ON
		[dbo].[CN_NodeTypes].[AdditionalID] = '5'
WHERE [dbo].[KKnowledges].[Deleted] IS NULL
GO



INSERT INTO [dbo].[KW_KnowledgeTypes]
           ([KnowledgeTypeID]
		   ,[Name])
SELECT  [dbo].[KKnowledgeType].[ID], [dbo].[KKnowledgeType].[KType]
FROM    [dbo].[KKnowledgeType]
GO



INSERT INTO [dbo].[KW_Knowledges]
           ([KnowledgeID]
		   ,[KnowledgeTypeID]
		   ,[IsDefault]
           ,[Usage]
           ,[ConfidentialityLevelID]
           ,[StatusID]
           ,[PublicationDate]
           ,[Score]
           ,[ScoresWeight])
SELECT  [dbo].[KKnowledges].[ID], [dbo].[KKnowledges].[KKnowledgeTypeID],
		0,
		[dbo].[KKnowledges].[Usage], [dbo].[KKnowledges].[ClassificationID], 
		[dbo].[KKnowledges].[StatusID], [dbo].[KKnowledges].[PublicationDate], 
		[dbo].[KKnowledges].[Score], [dbo].[KKnowledges].[ScoresWeight]
FROM    [dbo].[KKnowledges]
WHERE	[dbo].[KKnowledges].[KKnowledgeTypeID] < 3
GO

INSERT INTO [dbo].[KW_Knowledges]
           ([KnowledgeID]
		   ,[KnowledgeTypeID]
		   ,[IsDefault]
           ,[Usage]
           ,[ConfidentialityLevelID]
           ,[StatusID]
           ,[PublicationDate]
           ,[Score]
           ,[ScoresWeight])
SELECT  [dbo].[KKnowledges].[ID], [dbo].[KKnowledges].[KKnowledgeTypeID],
		[dbo].[KContents].[IsDefault],
		[dbo].[KKnowledges].[Usage], [dbo].[KKnowledges].[ClassificationID], 
		[dbo].[KKnowledges].[StatusID], [dbo].[KKnowledges].[PublicationDate], 
		[dbo].[KKnowledges].[Score], [dbo].[KKnowledges].[ScoresWeight]
FROM    [dbo].[KKnowledges] INNER JOIN [dbo].[KContents] ON
		[dbo].[KKnowledges].[ID] = [dbo].[KContents].[KKnowledgeID]
WHERE	[dbo].[KKnowledges].[KKnowledgeTypeID] = 3 AND [dbo].[KContents].[IsDefault] IS NOT NULL
GO


INSERT INTO [dbo].[KW_Knowledges]
           ([KnowledgeID]
		   ,[KnowledgeTypeID]
		   ,[IsDefault]
           ,[Usage]
           ,[ConfidentialityLevelID]
           ,[StatusID]
           ,[PublicationDate]
           ,[Score]
           ,[ScoresWeight])
SELECT  [dbo].[KKnowledges].[ID], [dbo].[KKnowledges].[KKnowledgeTypeID],
		0,
		[dbo].[KKnowledges].[Usage], [dbo].[KKnowledges].[ClassificationID], 
		[dbo].[KKnowledges].[StatusID], [dbo].[KKnowledges].[PublicationDate], 
		[dbo].[KKnowledges].[Score], [dbo].[KKnowledges].[ScoresWeight]
FROM    [dbo].[KKnowledges] INNER JOIN [dbo].[KContents] ON
		[dbo].[KKnowledges].[ID] = [dbo].[KContents].[KKnowledgeID]
WHERE	[dbo].[KKnowledges].[KKnowledgeTypeID] = 3 AND [dbo].[KContents].[IsDefault] IS NULL
GO



UPDATE [dbo].[KW_Knowledges]
   SET [ContentType] = (SELECT [dbo].[KContents].[Type] FROM [dbo].[KContents]
		WHERE  [dbo].[KW_Knowledges].[KnowledgeID] = [dbo].[KContents].[KKnowledgeID])
       ,[TreeNodeID] = (SELECT [dbo].[KContentTreeNodeContents].[ContentTreeNodeID] 
		FROM [dbo].[KContentTreeNodeContents]
		WHERE  [dbo].[KW_Knowledges].[KnowledgeID] = [dbo].[KContentTreeNodeContents].[KContentID])
       ,[PreviousVersionID] = (SELECT TOP(1) [dbo].[KContentVersions].[OriginalContentID] 
		FROM [dbo].[KContentVersions]
		WHERE  [dbo].[KW_Knowledges].[KnowledgeID] = [dbo].[KContentVersions].[VersionedContentID])
WHERE [dbo].[KW_Knowledges].[KnowledgeTypeID] = 3
GO


INSERT INTO [dbo].[KW_CreatorUsers]
           ([KnowledgeID]
		   ,[UserID]
           ,[CollaborationShare])
SELECT DISTINCT [dbo].[KKnowledges].[ID], [dbo].[KContentUsers].[UserID], 
		[dbo].[KContentUsers].[PartnershipPercent]
FROM    [dbo].[KContentUsers] INNER JOIN [dbo].[KKnowledges] ON 
		[dbo].[KContentUsers].[KContentID] = [dbo].[KKnowledges].[ID]
GO



INSERT INTO [dbo].[KW_KnowledgeCards]
           ([CardID]
		   ,[SenderUserID]
		   ,[ReceiverUserID]
		   ,[KnowledgeTypeID]
		   ,[Title]
		   ,[Description]
		   ,[SendDate]
           ,[Deleted])
SELECT [dbo].[KnowledgeCards].[ID], [dbo].[KnowledgeCards].[SenderUserID], 
		[dbo].[KnowledgeCards].[ReceiverUserID], [dbo].[KnowledgeCards].[KTypeID],
		[dbo].[KnowledgeCards].[Title], [dbo].[KnowledgeCards].[Description],
		[dbo].[KnowledgeCards].[CreationDate], [dbo].[KnowledgeCards].[Deleted]
FROM    [dbo].[KnowledgeCards]
GO



INSERT INTO [dbo].[KW_Experts]
           ([NodeID]
		   ,[UserID]
		   ,[ExpertiseLevelID]
           ,[Deleted])
SELECT DISTINCT [dbo].[NodeUsers].[NodeID], [dbo].[NodeUsers].[UserID], 1, 0
FROM    [dbo].[NodeUsers] INNER JOIN [dbo].[CN_Nodes] ON
		[dbo].[NodeUsers].[NodeID] = [dbo].[CN_Nodes].[NodeID] INNER JOIN 
		[dbo].[CN_NodeTypes] ON 
		[dbo].[CN_Nodes].[NodeTypeID] = [dbo].[CN_NodeTypes].[NodeTypeID]
WHERE	[dbo].[CN_NodeTypes].[AdditionalID] = '1' AND
		[dbo].[NodeUsers].[UserType] = 'Membership'
GO

DELETE [dbo].[NodeUsers]
FROM [dbo].[NodeUsers] INNER JOIN [dbo].[CN_Nodes] ON
	 [dbo].[NodeUsers].[NodeID] = [dbo].[CN_Nodes].[NodeID] INNER JOIN 
	 [dbo].[CN_NodeTypes] ON 
	 [dbo].[CN_Nodes].[NodeTypeID] = [dbo].[CN_NodeTypes].[NodeTypeID]
WHERE	[dbo].[CN_NodeTypes].[AdditionalID] = '1' AND
		[dbo].[NodeUsers].[UserType] = 'Membership'
	 


/* Fill Knowledge Assets */
ALTER TABLE [dbo].[KW_KnowledgeAssets]
ADD [Theorical] [nvarchar](256) NULL
GO

ALTER TABLE [dbo].[KW_KnowledgeAssets]
ADD [Practical] [nvarchar](256) NULL
GO

INSERT INTO [dbo].[KW_KnowledgeAssets]
           ([UserID]
           ,[KnowledgeID]
		   ,[TheoricalLevelID]
		   ,[PracticalLevelID]
		   ,[AcceptionDate]
           ,[Deleted])
SELECT DISTINCT [dbo].[KKnowledges].[OwnerUserID], [dbo].[KKnowledges].[ID], 
	1, 1, [dbo].[KKnowledges].[PublicationDate], 0
FROM  [dbo].[KKnowledges]
WHERE [dbo].[KKnowledges].[OwnerUserID] IS NOT NULL
GO


UPDATE [dbo].[KW_KnowledgeAssets]
   SET  [Theorical] = (SELECT [dbo].[KSkills].[SkillLevelTeoric] 
		FROM [dbo].[KSkills] 
		WHERE [dbo].[KW_KnowledgeAssets].[KnowledgeID] = [dbo].[KSkills].[KKnowledgeID])
       ,[Practical] = (SELECT [dbo].[KSkills].[SkillLevelOperative] 
		FROM [dbo].[KSkills] 
		WHERE [dbo].[KW_KnowledgeAssets].[KnowledgeID] = [dbo].[KSkills].[KKnowledgeID])
GO


UPDATE [dbo].[KW_KnowledgeAssets]
   SET  [TheoricalLevelID] = 1
WHERE [dbo].[KW_KnowledgeAssets].[Theorical] = N'Acquaintance'
GO

UPDATE [dbo].[KW_KnowledgeAssets]
   SET  [TheoricalLevelID] = 2
WHERE [dbo].[KW_KnowledgeAssets].[Theorical] = N'GeneralRecognition'
GO

UPDATE [dbo].[KW_KnowledgeAssets]
   SET  [TheoricalLevelID] = 3
WHERE [dbo].[KW_KnowledgeAssets].[Theorical] = N'Inking'
GO

UPDATE [dbo].[KW_KnowledgeAssets]
   SET  [TheoricalLevelID] = 4
WHERE [dbo].[KW_KnowledgeAssets].[Theorical] = N'FullGripe'
GO

UPDATE [dbo].[KW_KnowledgeAssets]
   SET  [PracticalLevelID] = 5
WHERE [dbo].[KW_KnowledgeAssets].[Practical] = N'Beginner'
GO

UPDATE [dbo].[KW_KnowledgeAssets]
   SET  [PracticalLevelID] = 6
WHERE [dbo].[KW_KnowledgeAssets].[Practical] = N'Ordinary'
GO

UPDATE [dbo].[KW_KnowledgeAssets]
   SET  [PracticalLevelID] = 7
WHERE [dbo].[KW_KnowledgeAssets].[Practical] = N'HalfPorfessional'
GO

UPDATE [dbo].[KW_KnowledgeAssets]
   SET  [PracticalLevelID] = 8
WHERE [dbo].[KW_KnowledgeAssets].[Practical] = N'Porfessional'
GO


ALTER TABLE [dbo].[KW_KnowledgeAssets]
DROP COLUMN [Theorical]
GO

ALTER TABLE [dbo].[KW_KnowledgeAssets]
DROP COLUMN [Practical]
GO
/* end of Fill Knowledge Assets */



INSERT INTO [dbo].[KW_RefrenceUsers]
           ([KnowledgeID]
		   ,[UserID])
SELECT DISTINCT [dbo].[KKnowledges].[ID], [dbo].[KLearningMethodaspnet_Users].[CooperatorUserID]
FROM    [dbo].[KLearningMethodaspnet_Users] INNER JOIN [dbo].[KLearningMethods] ON
		[dbo].[KLearningMethodaspnet_Users].[KLearningMethodID] = [dbo].[KLearningMethods].[ID]
		INNER JOIN [dbo].[KKnowledges] ON 
		[dbo].[KLearningMethods].[KKnowledgeID] = [dbo].[KKnowledges].[ID]
		INNER JOIN [dbo].[ProfileCommon] ON 
		[dbo].[KLearningMethodaspnet_Users].[CooperatorUserID] = [dbo].[ProfileCommon].[UserId]
GO



INSERT INTO [dbo].[KW_RelatedDepartments]
           ([KnowledgeID]
		   ,[DepartmentID]
		   ,[Score]
		   ,[ScoresWeight]
		   ,[Deleted])
SELECT  [dbo].[KnowledgeDepartments].[KnowledgeID], 
		[dbo].[KnowledgeDepartments].[DepartmentID], 0, 0, 0
FROM    [dbo].[KnowledgeDepartments]
GO



INSERT INTO [dbo].[KW_LearningMethods]
           ([KnowledgeID]
		   ,[Title] 
		   ,[Description])
SELECT  [dbo].[KLearningMethods].[KKnowledgeID], [dbo].[KLearningMethods].[Title],
		[dbo].[KLearningMethods].[Description]
FROM    [dbo].[KLearningMethods]
WHERE   [dbo].[KLearningMethods].[KKnowledgeID] IS NOT NULL
GO



INSERT INTO [dbo].[KW_TripForms]
           ([KnowledgeID]
		   ,[BeginDate] 
		   ,[FinishDate]
		   ,[Country]
		   ,[City]
		   ,[Results]
		   ,[Chalenges])
SELECT  [dbo].[KContentTripForm].[KKnowledgeID], [dbo].[KContentTripForm].[Date],
		[dbo].[KContentTripForm].[FinishDate], [dbo].[KContentTripForm].[Country],
		[dbo].[KContentTripForm].[City], [dbo].[KContentTripForm].[Result],
		[dbo].[KContentTripForm].[Chalange]
FROM    [dbo].[KContentTripForm]
GO



INSERT INTO [dbo].[KW_Companies]
           ([KnowledgeID]
		   ,[Title] 
		   ,[Products])
SELECT  [dbo].[KTripFormCompanies].[KKnowledgeID], [dbo].[KTripFormCompanies].[Name],
		[dbo].[KTripFormCompanies].[Products]
FROM    [dbo].[KTripFormCompanies]
WHERE	[dbo].[KTripFormCompanies].[KKnowledgeID] IS NOT NULL
GO




UPDATE [dbo].[KnowledgeNodes]
	SET Score = 0
WHERE Score IS NULL

UPDATE [dbo].[KnowledgeNodes]
	SET ScoresWeight = 0
WHERE ScoresWeight IS NULL

INSERT INTO [dbo].[KW_RelatedNodes]
           ([KnowledgeID]
           ,[NodeID]
		   ,[Score] 
		   ,[ScoresWeight]
		   ,[StatusID]
		   ,[Deleted])
SELECT  [dbo].[KnowledgeNodes].[KnowledgeID], [dbo].[KnowledgeNodes].[NodeID],
		[dbo].[KnowledgeNodes].[Score], [dbo].[KnowledgeNodes].[ScoresWeight],
		[dbo].[KnowledgeNodes].[StatusID], 0
FROM    [dbo].[KnowledgeNodes]
GO


INSERT INTO [dbo].[KW_ExperienceHolders](
	KnowledgeID,
	UserID,
	SendDate,
	Deleted
)
SELECT Ref.KnowledgeID, Ref.CreatorUserID, Ref.CreationDate, Ref.Deleted
FROM [dbo].[KW_View_Knowledges] AS Ref
WHERE Ref.KnowledgeTypeID = 1 -- 1: Experience
GO