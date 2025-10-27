INSERT INTO [RAAIVAN].[EKM_App].[dbo].[aspnet_Users]
SELECT * FROM [EKM_App].[dbo].[aspnet_Users] AS S
WHERE S.UserId NOT IN(SELECT UserId FROM [RAAIVAN].[EKM_App].[dbo].[aspnet_Users])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[aspnet_Membership]
SELECT * FROM [EKM_App].[dbo].[aspnet_Membership] AS S
WHERE S.UserId NOT IN(SELECT UserId FROM [RAAIVAN].[EKM_App].[dbo].[aspnet_Membership])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[USR_Profile]
SELECT * FROM [EKM_App].[dbo].[USR_Profile] AS S
WHERE S.UserID NOT IN(SELECT UserID FROM [RAAIVAN].[EKM_App].[dbo].[USR_Profile])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[Attachments]
SELECT * FROM [EKM_App].[dbo].[Attachments] AS S
WHERE S.ID NOT IN(SELECT ID FROM [RAAIVAN].[EKM_App].[dbo].[Attachments])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[AttachmentFiles]
SELECT * FROM [EKM_App].[dbo].[AttachmentFiles] AS S
WHERE S.ID NOT IN(SELECT ID FROM [RAAIVAN].[EKM_App].[dbo].[AttachmentFiles]) AND
	S.ID NOT IN(SELECT ID FROM [RAAIVAN].[EKM_App].[dbo].[AttachmentFiles])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[CN_NodeTypes]
SELECT * FROM [EKM_App].[dbo].[CN_NodeTypes] AS S
WHERE S.AdditionalID IS NULL AND 
	S.NodeTypeID NOT IN(SELECT NodeTypeID FROM [RAAIVAN].[EKM_App].[dbo].[CN_NodeTypes]) AND
	S.Name NOT IN(SELECT Name FROM [RAAIVAN].[EKM_App].[dbo].[CN_NodeTypes])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[CN_NodeTypes]
SELECT * FROM [EKM_App].[dbo].[CN_NodeTypes] AS S
WHERE S.AdditionalID IS NOT NULL AND 
	S.NodeTypeID NOT IN(SELECT NodeTypeID FROM [RAAIVAN].[EKM_App].[dbo].[CN_NodeTypes]) AND
	S.AdditionalID NOT IN(SELECT AdditionalID FROM [RAAIVAN].[EKM_App].[dbo].[CN_NodeTypes]) AND
	S.Name NOT IN(SELECT Name FROM [RAAIVAN].[EKM_App].[dbo].[CN_NodeTypes])
GO
	
INSERT INTO [RAAIVAN].[EKM_App].[dbo].[CN_ListTypes]
SELECT * FROM [EKM_App].[dbo].[CN_ListTypes] AS S
WHERE S.ListTypeID NOT IN(SELECT ListTypeID FROM [RAAIVAN].[EKM_App].[dbo].[CN_ListTypes]) AND
	S.AdditionalID NOT IN(SELECT AdditionalID FROM [RAAIVAN].[EKM_App].[dbo].[CN_ListTypes])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[CN_Nodes](
	NodeID, NodeTypeID, Name, [Description], CreatorUserID, CreationDate, LastModifierUserID,
	LastModificationDate, Deleted, TSkill, TExperience, TContent, [Status], DepartmentGroupID,
	ParentNodeID, Privacy, Tags, AdditionalID, OwnerID
)
SELECT S.NodeID, S.NodeTypeID, S.Name, S.[Description], S.CreatorUserID,
	S.CreationDate, S.LastModifierUserID, S.LastModificationDate,
	S.Deleted, S.TSkill, S.TExperience, S.TContent, S.[Status], S.DepartmentGroupID,
	NULL, S.Privacy, S.Tags, S.AdditionalID, S.OwnerID
FROM [EKM_App].[dbo].[CN_Nodes] AS S
WHERE S.NodeID NOT IN(SELECT NodeID FROM [RAAIVAN].[EKM_App].[dbo].[CN_Nodes])
GO
	
UPDATE Ref
	SET ParentNodeID = S.ParentNodeID
FROM [EKM_App].[dbo].[CN_Nodes] AS S
	INNER JOIN [RAAIVAN].[EKM_App].[dbo].[CN_Nodes] AS Ref
	ON S.NodeID = Ref.[NodeID]
GO
	
INSERT INTO [RAAIVAN].[EKM_App].[dbo].[CN_Lists]
SELECT * FROM [EKM_App].[dbo].[CN_Lists] AS S
WHERE S.ListTypeID NOT IN(SELECT ListTypeID FROM [RAAIVAN].[EKM_App].[dbo].[CN_Lists]) AND
	S.AdditionalID NOT IN(SELECT AdditionalID FROM [RAAIVAN].[EKM_App].[dbo].[CN_Lists])
GO
	
INSERT INTO [RAAIVAN].[EKM_App].[dbo].[CN_ListNodes]
SELECT * FROM [EKM_App].[dbo].[CN_ListNodes] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [RAAIVAN].[EKM_App].[dbo].[CN_ListNodes] AS Ref
	WHERE Ref.ListID = S.ListID AND Ref.NodeID = S.NodeID)
GO
	
INSERT INTO [RAAIVAN].[EKM_App].[dbo].[CN_NodeLikes]
SELECT * FROM [EKM_App].[dbo].[CN_NodeLikes] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [RAAIVAN].[EKM_App].[dbo].[CN_NodeLikes] AS Ref
	WHERE Ref.NodeID = S.NodeID AND Ref.UserID = S.UserID)
GO
	
INSERT INTO [RAAIVAN].[EKM_App].[dbo].[CN_NodeMembers]
SELECT * FROM [EKM_App].[dbo].[CN_NodeMembers] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [RAAIVAN].[EKM_App].[dbo].[CN_NodeMembers] AS Ref
	WHERE Ref.NodeID = S.NodeID AND Ref.UserID = S.UserID)
GO
	
INSERT INTO [RAAIVAN].[EKM_App].[dbo].[CN_Properties]
SELECT * FROM [EKM_App].[dbo].[CN_Properties] AS S
WHERE S.PropertyID NOT IN(SELECT PropertyID FROM [RAAIVAN].[EKM_App].[dbo].[CN_Properties])
GO
	
INSERT INTO [RAAIVAN].[EKM_App].[dbo].[CN_NodeRelations]
SELECT * FROM [EKM_App].[dbo].[CN_NodeRelations] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [RAAIVAN].[EKM_App].[dbo].[CN_NodeRelations] AS Ref
	WHERE Ref.SourceNodeID = S.SourceNodeID AND 
		Ref.DestinationNodeID = S.DestinationNodeID AND
		Ref.PropertyID = S.PropertyID)
GO


INSERT INTO [RAAIVAN].[EKM_App].[dbo].[DCT_TreeTypes]
SELECT * FROM [EKM_App].[dbo].[DCT_TreeTypes] AS S
WHERE S.TreeTypeID NOT IN(SELECT TreeTypeID FROM [RAAIVAN].[EKM_App].[dbo].[DCT_TreeTypes])
GO


INSERT INTO [RAAIVAN].[EKM_App].[dbo].[DCT_Trees]
SELECT * FROM [EKM_App].[dbo].[DCT_Trees] AS S
WHERE S.TreeID NOT IN(SELECT TreeID FROM [RAAIVAN].[EKM_App].[dbo].[DCT_Trees])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[DCT_TreeNodes]
SELECT * FROM [EKM_App].[dbo].[DCT_TreeNodes] AS S
WHERE S.TreeNodeID NOT IN(SELECT TreeNodeID FROM [RAAIVAN].[EKM_App].[dbo].[DCT_TreeNodes])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[EVT_Events]
SELECT * FROM [EKM_App].[dbo].[EVT_Events] AS S
WHERE S.EventID NOT IN(SELECT EventID FROM [RAAIVAN].[EKM_App].[dbo].[EVT_Events])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[EVT_RelatedNodes]
SELECT * FROM [EKM_App].[dbo].[EVT_RelatedNodes] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [RAAIVAN].[EKM_App].[dbo].[EVT_RelatedNodes] AS Ref
	WHERE Ref.EventID = S.EventID AND Ref.NodeID = S.NodeID)
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[EVT_RelatedUsers]
SELECT * FROM [EKM_App].[dbo].[EVT_RelatedUsers] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [RAAIVAN].[EKM_App].[dbo].[EVT_RelatedUsers] AS Ref
	WHERE Ref.EventID = S.EventID AND Ref.UserID = S.UserID)
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[KW_ConfidentialityLevels]
SELECT * FROM [EKM_App].[dbo].[KW_ConfidentialityLevels] AS S
WHERE S.LevelID NOT IN(SELECT LevelID FROM [RAAIVAN].[EKM_App].[dbo].[KW_ConfidentialityLevels])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[KW_UsersConfidentialityLevels]
SELECT * FROM [EKM_App].[dbo].[KW_UsersConfidentialityLevels] AS S
WHERE S.UserID NOT IN(SELECT UserID FROM [RAAIVAN].[EKM_App].[dbo].[KW_UsersConfidentialityLevels])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[KW_KnowledgeTypes]
SELECT * FROM [EKM_App].[dbo].[KW_KnowledgeTypes] AS S
WHERE S.KnowledgeTypeID NOT IN(SELECT KnowledgeTypeID FROM [RAAIVAN].[EKM_App].[dbo].[KW_KnowledgeTypes])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[KWF_Statuses]
SELECT * FROM [EKM_App].[dbo].[KWF_Statuses] AS S
WHERE S.StatusID NOT IN(SELECT StatusID FROM [RAAIVAN].[EKM_App].[dbo].[KWF_Statuses])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[KW_Knowledges]
SELECT * FROM [EKM_App].[dbo].[KW_Knowledges] AS S
WHERE S.KnowledgeID NOT IN(SELECT KnowledgeID FROM [RAAIVAN].[EKM_App].[dbo].[KW_Knowledges])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[KW_CreatorUsers]
SELECT * FROM [EKM_App].[dbo].[KW_CreatorUsers] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [RAAIVAN].[EKM_App].[dbo].[KW_CreatorUsers] AS Ref
	WHERE Ref.KnowledgeID = S.KnowledgeID AND Ref.UserID = S.UserID)
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[KW_ExperienceHolders]
SELECT * FROM [EKM_App].[dbo].[KW_ExperienceHolders] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [RAAIVAN].[EKM_App].[dbo].[KW_ExperienceHolders] AS Ref
	WHERE Ref.KnowledgeID = S.KnowledgeID AND Ref.UserID = S.UserID)
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[KW_ExpertiseLevels]
SELECT * FROM [EKM_App].[dbo].[KW_ExpertiseLevels] AS S
WHERE S.LevelID NOT IN(SELECT LevelID FROM [RAAIVAN].[EKM_App].[dbo].[KW_ExpertiseLevels])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[KW_Experts]
SELECT * FROM [EKM_App].[dbo].[KW_Experts] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [RAAIVAN].[EKM_App].[dbo].[KW_Experts] AS Ref
	WHERE Ref.NodeID = S.NodeID AND Ref.UserID = S.UserID)
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[KW_SkillLevels]
SELECT * FROM [EKM_App].[dbo].[KW_SkillLevels] AS S
WHERE S.LevelID NOT IN(SELECT LevelID FROM [RAAIVAN].[EKM_App].[dbo].[KW_SkillLevels])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[KW_KnowledgeAssets]
SELECT * FROM [EKM_App].[dbo].[KW_KnowledgeAssets] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [RAAIVAN].[EKM_App].[dbo].[KW_KnowledgeAssets] AS Ref
	WHERE Ref.UserID = S.UserID AND Ref.KnowledgeID = S.KnowledgeID)
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[KW_KnowledgeCards]
SELECT * FROM [EKM_App].[dbo].[KW_KnowledgeCards] AS S
WHERE S.CardID NOT IN(SELECT CardID FROM [RAAIVAN].[EKM_App].[dbo].[KW_KnowledgeCards])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[KW_KnowledgeManagers]
SELECT * FROM [EKM_App].[dbo].[KW_KnowledgeManagers] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [RAAIVAN].[EKM_App].[dbo].[KW_KnowledgeManagers] AS Ref
	WHERE Ref.ListID = S.ListID AND Ref.UserID = S.UserID)
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[KW_RefrenceUsers]
SELECT * FROM [EKM_App].[dbo].[KW_RefrenceUsers] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [RAAIVAN].[EKM_App].[dbo].[KW_RefrenceUsers] AS Ref
	WHERE Ref.KnowledgeID = S.KnowledgeID AND Ref.UserID = S.UserID)
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[KW_RelatedNodes]
SELECT * FROM [EKM_App].[dbo].[KW_RelatedNodes] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [RAAIVAN].[EKM_App].[dbo].[KW_RelatedNodes] AS Ref
	WHERE Ref.KnowledgeID = S.KnowledgeID AND Ref.NodeID = S.NodeID)
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[KW_TripForms]
SELECT * FROM [EKM_App].[dbo].[KW_TripForms] AS S
WHERE S.KnowledgeID NOT IN(SELECT KnowledgeID FROM [RAAIVAN].[EKM_App].[dbo].[KW_TripForms])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[KWF_Paraphs]
SELECT * FROM [EKM_App].[dbo].[KWF_Paraphs] AS S
WHERE S.ParaphID NOT IN(SELECT ParaphID FROM [RAAIVAN].[EKM_App].[dbo].[KWF_Paraphs])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[KWF_Evaluators](
	KnowledgeID, UserID, SenderUserID, Evaluated, Score, EntranceDate, ExpirationDate,
	EvaluationDate, Rejected, Deleted
)
SELECT S.KnowledgeID, S.UserID, S.SenderUserID, S.Evaluated, S.Score, S.EntranceDate, 
	S.ExpirationDate, S.EvaluationDate, S.Rejected, S.Deleted
FROM [EKM_App].[dbo].[KWF_Evaluators] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [RAAIVAN].[EKM_App].[dbo].[KWF_Evaluators] AS Ref
	WHERE Ref.KnowledgeID = S.KnowledgeID AND Ref.UserID = S.UserID AND
		Ref.EntranceDate = S.EntranceDate)
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[KWF_Experts](
	KnowledgeID, UserID, NodeID, SenderUserID, Evaluated, Score, EntranceDate, ExpirationDate,
	EvaluationDate, Rejected, Deleted
)
SELECT S.KnowledgeID, S.UserID, S.NodeID, S.SenderUserID, S.Evaluated, S.Score, S.EntranceDate, 
	S.ExpirationDate, S.EvaluationDate, S.Rejected, S.Deleted
FROM [EKM_App].[dbo].[KWF_Experts] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [RAAIVAN].[EKM_App].[dbo].[KWF_Experts] AS Ref
	WHERE Ref.KnowledgeID = S.KnowledgeID AND Ref.UserID = S.UserID AND
		Ref.NodeID = S.NodeID AND Ref.EntranceDate = S.EntranceDate)
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[KWF_Managers](
	KnowledgeID, UserID, EntranceDate, EvaluationDate, [Sent], HaveRejectedEvaluators, Deleted
)
SELECT S.KnowledgeID, S.UserID, S.EntranceDate, S.EvaluationDate, S.[Sent],
	S.HaveRejectedEvaluators, S.Deleted
FROM [EKM_App].[dbo].[KWF_Managers] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [RAAIVAN].[EKM_App].[dbo].[KWF_Managers] AS Ref
	WHERE Ref.KnowledgeID = S.KnowledgeID AND Ref.UserID = S.UserID AND
		Ref.EntranceDate = S.EntranceDate)
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[NGeneralQuestions]
SELECT * FROM [EKM_App].[dbo].[NGeneralQuestions] AS S
WHERE S.ID NOT IN(SELECT ID FROM [RAAIVAN].[EKM_App].[dbo].[NGeneralQuestions])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[ProfileEducation]
SELECT * FROM [EKM_App].[dbo].[ProfileEducation] AS S
WHERE S.ID NOT IN(SELECT ID FROM [RAAIVAN].[EKM_App].[dbo].[ProfileEducation])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[ProfileInstitute]
SELECT * FROM [EKM_App].[dbo].[ProfileInstitute] AS S
WHERE S.ID NOT IN(SELECT ID FROM [RAAIVAN].[EKM_App].[dbo].[ProfileInstitute])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[ProfileJobs]
SELECT * FROM [EKM_App].[dbo].[ProfileJobs] AS S
WHERE S.ID NOT IN(SELECT ID FROM [RAAIVAN].[EKM_App].[dbo].[ProfileJobs])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[ProfileScientific]
SELECT * FROM [EKM_App].[dbo].[ProfileScientific] AS S
WHERE S.ID NOT IN(SELECT ID FROM [RAAIVAN].[EKM_App].[dbo].[ProfileScientific])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[PRVC_Audience]
SELECT * FROM [EKM_App].[dbo].[PRVC_Audience] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [RAAIVAN].[EKM_App].[dbo].[PRVC_Audience] AS Ref
	WHERE Ref.RoleID = S.RoleID AND Ref.ObjectID = S.ObjectID)
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[QA_Questions]
SELECT * FROM [EKM_App].[dbo].[QA_Questions] AS S
WHERE S.QuestionID NOT IN(SELECT QuestionID FROM [RAAIVAN].[EKM_App].[dbo].[QA_Questions])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[QA_Answers]
SELECT * FROM [EKM_App].[dbo].[QA_Answers] AS S
WHERE S.AnswerID NOT IN(SELECT AnswerID FROM [RAAIVAN].[EKM_App].[dbo].[QA_Answers])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[QA_QuestionLikes]
SELECT * FROM [EKM_App].[dbo].[QA_QuestionLikes] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [RAAIVAN].[EKM_App].[dbo].[QA_QuestionLikes] AS Ref
	WHERE Ref.QuestionID = S.QuestionID AND Ref.UserID = S.UserID)
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[QA_RefUsers]
SELECT * FROM [EKM_App].[dbo].[QA_RefUsers] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [RAAIVAN].[EKM_App].[dbo].[QA_RefUsers] AS Ref
	WHERE Ref.UserID = S.UserID AND Ref.QuestionID = S.QuestionID)
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[QA_RefNodes]
SELECT * FROM [EKM_App].[dbo].[QA_RefNodes] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [RAAIVAN].[EKM_App].[dbo].[QA_RefNodes] AS Ref
	WHERE Ref.NodeID = S.NodeID AND Ref.QuestionID = S.QuestionID)
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[SH_PostTypes]
SELECT * FROM [EKM_App].[dbo].[SH_PostTypes] AS S
WHERE S.PostTypeID NOT IN(SELECT PostTypeID FROM [RAAIVAN].[EKM_App].[dbo].[SH_PostTypes])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[SH_Posts]
SELECT * FROM [EKM_App].[dbo].[SH_Posts] AS S
WHERE S.PostID NOT IN(SELECT PostID FROM [RAAIVAN].[EKM_App].[dbo].[SH_Posts])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[SH_PostShares]
SELECT * FROM [EKM_App].[dbo].[SH_PostShares] AS S
WHERE S.ShareID NOT IN(SELECT ShareID FROM [RAAIVAN].[EKM_App].[dbo].[SH_PostShares])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[SH_Comments]
SELECT * FROM [EKM_App].[dbo].[SH_Comments] AS S
WHERE S.CommentID NOT IN(SELECT CommentID FROM [RAAIVAN].[EKM_App].[dbo].[SH_Comments])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[SH_CommentLikes]
SELECT * FROM [EKM_App].[dbo].[SH_CommentLikes] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [RAAIVAN].[EKM_App].[dbo].[SH_CommentLikes] AS Ref
	WHERE Ref.CommentID = S.CommentID AND Ref.UserID = S.UserID)
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[SH_ShareLikes]
SELECT * FROM [EKM_App].[dbo].[SH_ShareLikes] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [RAAIVAN].[EKM_App].[dbo].[SH_ShareLikes] AS Ref
	WHERE Ref.ShareID = S.ShareID AND Ref.UserID = S.UserID)
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[UserConnections]
SELECT * FROM [EKM_App].[dbo].[UserConnections] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [RAAIVAN].[EKM_App].[dbo].[UserConnections] AS Ref
	WHERE Ref.SenderUserID = S.SenderUserID AND Ref.ReceiverUserID = S.ReceiverUserID)
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[UserGroups]
SELECT * FROM [EKM_App].[dbo].[UserGroups] AS S
WHERE S.ID NOT IN(SELECT ID FROM [RAAIVAN].[EKM_App].[dbo].[UserGroups])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[UserGroupUsers]
SELECT * FROM [EKM_App].[dbo].[UserGroupUsers] AS S
WHERE S.ID NOT IN(SELECT ID FROM [RAAIVAN].[EKM_App].[dbo].[UserGroupUsers])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[AccessRoles]
SELECT * FROM [EKM_App].[dbo].[AccessRoles] AS S
WHERE S.ID NOT IN(SELECT ID FROM [RAAIVAN].[EKM_App].[dbo].[AccessRoles])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[UserGroupAccessRoles]
SELECT * FROM [EKM_App].[dbo].[UserGroupAccessRoles] AS S
WHERE S.ID NOT IN(SELECT ID FROM [RAAIVAN].[EKM_App].[dbo].[UserGroupAccessRoles])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[USR_ItemVisits]
SELECT * FROM [EKM_App].[dbo].[USR_ItemVisits] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [RAAIVAN].[EKM_App].[dbo].[USR_ItemVisits] AS Ref
	WHERE Ref.ItemID = S.ItemID AND Ref.VisitDate = S.VisitDate AND
		Ref.UserID = S.UserID)
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[WK_Titles]
SELECT * FROM [EKM_App].[dbo].[WK_Titles] AS S
WHERE S.TitleID NOT IN(SELECT TitleID FROM [RAAIVAN].[EKM_App].[dbo].[WK_Titles])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[WK_Paragraphs]
SELECT * FROM [EKM_App].[dbo].[WK_Paragraphs] AS S
WHERE S.ParagraphID NOT IN(SELECT ParagraphID FROM [RAAIVAN].[EKM_App].[dbo].[WK_Paragraphs])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[WK_Changes]
SELECT * FROM [EKM_App].[dbo].[WK_Changes] AS S
WHERE S.ChangeID NOT IN(SELECT ChangeID FROM [RAAIVAN].[EKM_App].[dbo].[WK_Changes])
GO

INSERT INTO [RAAIVAN].[EKM_App].[dbo].[NTFN_Notifications](
	UserID, SubjectID, RefItemID, SubjectType, SubjectName, [Action], SenderUserID, 
	SendDate, [Description], Info, UserStatus, Seen, ViewDate, Deleted
)
SELECT UserID, SubjectID, RefItemID, SubjectType, SubjectName, [Action], SenderUserID, 
	SendDate, [Description], Info, UserStatus, Seen, ViewDate, Deleted
FROM [EKM_App].[dbo].[NTFN_Notifications] AS S
WHERE S.ID NOT IN(SELECT ID FROM [RAAIVAN].[EKM_App].[dbo].[NTFN_Notifications])
GO