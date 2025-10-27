INSERT INTO [EKM_App].[dbo].[aspnet_Users]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[aspnet_Users] AS S
WHERE S.UserId NOT IN(SELECT UserId FROM [EKM_App].[dbo].[aspnet_Users])
GO

INSERT INTO [EKM_App].[dbo].[aspnet_Membership]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[aspnet_Membership] AS S
WHERE S.UserId NOT IN(SELECT UserId FROM [EKM_App].[dbo].[aspnet_Membership])
GO

INSERT INTO [EKM_App].[dbo].[USR_Profile]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[USR_Profile] AS S
WHERE S.UserID NOT IN(SELECT UserID FROM [EKM_App].[dbo].[USR_Profile])
GO

INSERT INTO [EKM_App].[dbo].[Attachments]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[Attachments] AS S
WHERE S.ID NOT IN(SELECT ID FROM [EKM_App].[dbo].[Attachments])
GO

INSERT INTO [EKM_App].[dbo].[AttachmentFiles]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[AttachmentFiles] AS S
WHERE S.ID NOT IN(SELECT ID FROM [EKM_App].[dbo].[AttachmentFiles]) AND
	S.ID NOT IN(SELECT ID FROM [EKM_App].[dbo].[AttachmentFiles])
GO

INSERT INTO [EKM_App].[dbo].[CN_NodeTypes]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[CN_NodeTypes] AS S
WHERE S.AdditionalID IS NULL AND 
	S.NodeTypeID NOT IN(SELECT NodeTypeID FROM [EKM_App].[dbo].[CN_NodeTypes]) AND
	S.Name NOT IN(SELECT Name FROM [EKM_App].[dbo].[CN_NodeTypes])
GO

INSERT INTO [EKM_App].[dbo].[CN_NodeTypes]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[CN_NodeTypes] AS S
WHERE S.AdditionalID IS NOT NULL AND 
	S.NodeTypeID NOT IN(SELECT NodeTypeID FROM [EKM_App].[dbo].[CN_NodeTypes]) AND
	S.AdditionalID NOT IN(SELECT AdditionalID FROM [EKM_App].[dbo].[CN_NodeTypes]) AND
	S.Name NOT IN(SELECT Name FROM [EKM_App].[dbo].[CN_NodeTypes])
GO
	

INSERT INTO [EKM_App].[dbo].[CN_Nodes](
	NodeID, NodeTypeID, Name, [Description], CreatorUserID, CreationDate, LastModifierUserID,
	LastModificationDate, Deleted, ParentNodeID, Privacy, Tags, AdditionalID, OwnerID
)
SELECT S.NodeID, S.NodeTypeID, S.Name, S.[Description], S.CreatorUserID,
	S.CreationDate, S.LastModifierUserID, S.LastModificationDate,
	S.Deleted, NULL, S.Privacy, S.Tags, S.AdditionalID, S.OwnerID
FROM [RAAIVAN].[EKM_App].[dbo].[CN_Nodes] AS S
WHERE S.NodeID NOT IN(SELECT NodeID FROM [EKM_App].[dbo].[CN_Nodes])
GO

UPDATE [EKM_App].[dbo].[CN_Nodes]
	SET ParentNodeID = S.ParentNodeID
FROM [RAAIVAN].[EKM_App].[dbo].[CN_Nodes] AS S
	INNER JOIN [EKM_App].[dbo].[CN_Nodes]
	ON S.NodeID = [EKM_App].[dbo].[CN_Nodes].[NodeID]
GO
	
INSERT INTO [EKM_App].[dbo].[CN_ListNodes]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[CN_ListNodes] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[CN_ListNodes] AS Ref
	WHERE Ref.ListID = S.ListID AND Ref.NodeID = S.NodeID)
GO
	
INSERT INTO [EKM_App].[dbo].[CN_NodeLikes]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[CN_NodeLikes] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[CN_NodeLikes] AS Ref
	WHERE Ref.NodeID = S.NodeID AND Ref.UserID = S.UserID)
GO
	
INSERT INTO [EKM_App].[dbo].[CN_NodeMembers]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[CN_NodeMembers] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[CN_NodeMembers] AS Ref
	WHERE Ref.NodeID = S.NodeID AND Ref.UserID = S.UserID)
GO
	
INSERT INTO [EKM_App].[dbo].[CN_Properties]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[CN_Properties] AS S
WHERE S.PropertyID NOT IN(SELECT PropertyID FROM [EKM_App].[dbo].[CN_Properties])
GO
	
INSERT INTO [EKM_App].[dbo].[CN_NodeRelations]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[CN_NodeRelations] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[CN_NodeRelations] AS Ref
	WHERE Ref.SourceNodeID = S.SourceNodeID AND 
		Ref.DestinationNodeID = S.DestinationNodeID AND
		Ref.PropertyID = S.PropertyID)
GO


INSERT INTO [EKM_App].[dbo].[DCT_TreeTypes]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[DCT_TreeTypes] AS S
WHERE S.TreeTypeID NOT IN(SELECT TreeTypeID FROM [EKM_App].[dbo].[DCT_TreeTypes])
GO


INSERT INTO [EKM_App].[dbo].[DCT_Trees]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[DCT_Trees] AS S
WHERE S.TreeID NOT IN(SELECT TreeID FROM [EKM_App].[dbo].[DCT_Trees])
GO

INSERT INTO [EKM_App].[dbo].[DCT_TreeNodes]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[DCT_TreeNodes] AS S
WHERE S.TreeNodeID NOT IN(SELECT TreeNodeID FROM [EKM_App].[dbo].[DCT_TreeNodes])
GO

INSERT INTO [EKM_App].[dbo].[EVT_Events]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[EVT_Events] AS S
WHERE S.EventID NOT IN(SELECT EventID FROM [EKM_App].[dbo].[EVT_Events])
GO

INSERT INTO [EKM_App].[dbo].[EVT_RelatedNodes]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[EVT_RelatedNodes] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[EVT_RelatedNodes] AS Ref
	WHERE Ref.EventID = S.EventID AND Ref.NodeID = S.NodeID)
GO

INSERT INTO [EKM_App].[dbo].[EVT_RelatedUsers]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[EVT_RelatedUsers] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[EVT_RelatedUsers] AS Ref
	WHERE Ref.EventID = S.EventID AND Ref.UserID = S.UserID)
GO

INSERT INTO [EKM_App].[dbo].[KW_ConfidentialityLevels]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[KW_ConfidentialityLevels] AS S
WHERE S.LevelID NOT IN(SELECT LevelID FROM [EKM_App].[dbo].[KW_ConfidentialityLevels])
GO

INSERT INTO [EKM_App].[dbo].[KW_UsersConfidentialityLevels]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[KW_UsersConfidentialityLevels] AS S
WHERE S.UserID NOT IN(SELECT UserID FROM [EKM_App].[dbo].[KW_UsersConfidentialityLevels])
GO

INSERT INTO [EKM_App].[dbo].[KW_KnowledgeTypes]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[KW_KnowledgeTypes] AS S
WHERE S.KnowledgeTypeID NOT IN(SELECT KnowledgeTypeID FROM [EKM_App].[dbo].[KW_KnowledgeTypes])
GO

INSERT INTO [EKM_App].[dbo].[KWF_Statuses]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[KWF_Statuses] AS S
WHERE S.StatusID NOT IN(SELECT StatusID FROM [EKM_App].[dbo].[KWF_Statuses])
GO

INSERT INTO [EKM_App].[dbo].[KW_Knowledges]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[KW_Knowledges] AS S
WHERE S.KnowledgeID NOT IN(SELECT KnowledgeID FROM [EKM_App].[dbo].[KW_Knowledges])
GO

INSERT INTO [EKM_App].[dbo].[KW_CreatorUsers]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[KW_CreatorUsers] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[KW_CreatorUsers] AS Ref
	WHERE Ref.KnowledgeID = S.KnowledgeID AND Ref.UserID = S.UserID)
GO

INSERT INTO [EKM_App].[dbo].[KW_ExperienceHolders]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[KW_ExperienceHolders] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[KW_ExperienceHolders] AS Ref
	WHERE Ref.KnowledgeID = S.KnowledgeID AND Ref.UserID = S.UserID)
GO

INSERT INTO [EKM_App].[dbo].[CN_Experts]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[CN_Experts] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[CN_Experts] AS Ref
	WHERE Ref.NodeID = S.NodeID AND Ref.UserID = S.UserID)
GO

INSERT INTO [EKM_App].[dbo].[CN_Services]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[CN_Services] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[CN_Services] AS Ref
	WHERE Ref.NodeTypeID = S.NodeTypeID)
GO

INSERT INTO [EKM_App].[dbo].[CN_ServiceAdmins]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[CN_ServiceAdmins] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[CN_ServiceAdmins] AS Ref
	WHERE Ref.NodeTypeID = S.NodeTypeID AND Ref.UserID = S.UserID)
GO

INSERT INTO [EKM_App].[dbo].[CN_FreeUsers]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[CN_FreeUsers] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[CN_FreeUsers] AS Ref
	WHERE Ref.NodeTypeID = S.NodeTypeID AND Ref.UserID = S.UserID)
GO

INSERT INTO [EKM_App].[dbo].[CN_NodeCreators]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[CN_NodeCreators] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[CN_NodeCreators] AS Ref
	WHERE Ref.NodeID = S.NodeID AND Ref.UserID = S.UserID)
GO

INSERT INTO [EKM_App].[dbo].[CN_Extensions]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[CN_Extensions] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[CN_Extensions] AS Ref
	WHERE Ref.OwnerID = S.OwnerID AND Ref.Extension = S.Extension)
GO

INSERT INTO [EKM_App].[dbo].[FG_ExtendedForms]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[FG_ExtendedForms] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[FG_ExtendedForms] AS Ref
	WHERE Ref.FormID = S.FormID)
GO

INSERT INTO [EKM_App].[dbo].[FG_ExtendedFormElements]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[FG_ExtendedFormElements] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[FG_ExtendedFormElements] AS Ref
	WHERE Ref.ElementID = S.ElementID)
GO

INSERT INTO [EKM_App].[dbo].[FG_FormOwners]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[FG_FormOwners] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[FG_FormOwners] AS Ref
	WHERE Ref.OwnerID = S.OwnerID)
GO

INSERT INTO [EKM_App].[dbo].[FG_ElementLimits]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[FG_ElementLimits] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[FG_ElementLimits] AS Ref
	WHERE Ref.OwnerID = S.OwnerID AND Ref.ElementID = S.ElementID)
GO

INSERT INTO [EKM_App].[dbo].[FG_FormInstances]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[FG_FormInstances] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[FG_FormInstances] AS Ref
	WHERE Ref.InstanceID = S.InstanceID)
GO

INSERT INTO [EKM_App].[dbo].[FG_InstanceElements]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[FG_InstanceElements] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[FG_InstanceElements] AS Ref
	WHERE Ref.ElementID = S.ElementID)
GO

INSERT INTO [EKM_App].[dbo].[KW_SkillLevels]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[KW_SkillLevels] AS S
WHERE S.LevelID NOT IN(SELECT LevelID FROM [EKM_App].[dbo].[KW_SkillLevels])
GO

INSERT INTO [EKM_App].[dbo].[KW_KnowledgeAssets]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[KW_KnowledgeAssets] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[KW_KnowledgeAssets] AS Ref
	WHERE Ref.UserID = S.UserID AND Ref.KnowledgeID = S.KnowledgeID)
GO

INSERT INTO [EKM_App].[dbo].[KW_KnowledgeCards]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[KW_KnowledgeCards] AS S
WHERE S.CardID NOT IN(SELECT CardID FROM [EKM_App].[dbo].[KW_KnowledgeCards])
GO

INSERT INTO [EKM_App].[dbo].[KW_KnowledgeManagers]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[KW_KnowledgeManagers] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[KW_KnowledgeManagers] AS Ref
	WHERE Ref.ListID = S.ListID AND Ref.UserID = S.UserID)
GO

INSERT INTO [EKM_App].[dbo].[KW_RefrenceUsers]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[KW_RefrenceUsers] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[KW_RefrenceUsers] AS Ref
	WHERE Ref.KnowledgeID = S.KnowledgeID AND Ref.UserID = S.UserID)
GO

INSERT INTO [EKM_App].[dbo].[KW_RelatedNodes]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[KW_RelatedNodes] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[KW_RelatedNodes] AS Ref
	WHERE Ref.KnowledgeID = S.KnowledgeID AND Ref.NodeID = S.NodeID)
GO

INSERT INTO [EKM_App].[dbo].[KW_TripForms]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[KW_TripForms] AS S
WHERE S.KnowledgeID NOT IN(SELECT KnowledgeID FROM [EKM_App].[dbo].[KW_TripForms])
GO

INSERT INTO [EKM_App].[dbo].[KWF_Paraphs]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[KWF_Paraphs] AS S
WHERE S.ParaphID NOT IN(SELECT ParaphID FROM [EKM_App].[dbo].[KWF_Paraphs])
GO

INSERT INTO [EKM_App].[dbo].[KWF_Evaluators](
	KnowledgeID, UserID, SenderUserID, Evaluated, Score, EntranceDate, ExpirationDate,
	EvaluationDate, Rejected, Deleted
)
SELECT S.KnowledgeID, S.UserID, S.SenderUserID, S.Evaluated, S.Score, S.EntranceDate, 
	S.ExpirationDate, S.EvaluationDate, S.Rejected, S.Deleted
FROM [RAAIVAN].[EKM_App].[dbo].[KWF_Evaluators] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[KWF_Evaluators] AS Ref
	WHERE Ref.KnowledgeID = S.KnowledgeID AND Ref.UserID = S.UserID AND
		Ref.EntranceDate = S.EntranceDate)
GO

INSERT INTO [EKM_App].[dbo].[KWF_Experts](
	KnowledgeID, UserID, NodeID, SenderUserID, Evaluated, Score, EntranceDate, ExpirationDate,
	EvaluationDate, Rejected, Deleted
)
SELECT S.KnowledgeID, S.UserID, S.NodeID, S.SenderUserID, S.Evaluated, S.Score, S.EntranceDate, 
	S.ExpirationDate, S.EvaluationDate, S.Rejected, S.Deleted
FROM [RAAIVAN].[EKM_App].[dbo].[KWF_Experts] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[KWF_Experts] AS Ref
	WHERE Ref.KnowledgeID = S.KnowledgeID AND Ref.UserID = S.UserID AND
		Ref.NodeID = S.NodeID AND Ref.EntranceDate = S.EntranceDate)
GO

INSERT INTO [EKM_App].[dbo].[KWF_Managers](
	KnowledgeID, UserID, EntranceDate, EvaluationDate, [Sent], HaveRejectedEvaluators, Deleted
)
SELECT S.KnowledgeID, S.UserID, S.EntranceDate, S.EvaluationDate, S.[Sent],
	S.HaveRejectedEvaluators, S.Deleted
FROM [RAAIVAN].[EKM_App].[dbo].[KWF_Managers] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[KWF_Managers] AS Ref
	WHERE Ref.KnowledgeID = S.KnowledgeID AND Ref.UserID = S.UserID AND
		Ref.EntranceDate = S.EntranceDate)
GO

INSERT INTO [EKM_App].[dbo].[NGeneralQuestions]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[NGeneralQuestions] AS S
WHERE S.ID NOT IN(SELECT ID FROM [EKM_App].[dbo].[NGeneralQuestions])
GO

INSERT INTO [EKM_App].[dbo].[ProfileEducation]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[ProfileEducation] AS S
WHERE S.ID NOT IN(SELECT ID FROM [EKM_App].[dbo].[ProfileEducation])
GO

INSERT INTO [EKM_App].[dbo].[ProfileInstitute]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[ProfileInstitute] AS S
WHERE S.ID NOT IN(SELECT ID FROM [EKM_App].[dbo].[ProfileInstitute])
GO

INSERT INTO [EKM_App].[dbo].[ProfileJobs]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[ProfileJobs] AS S
WHERE S.ID NOT IN(SELECT ID FROM [EKM_App].[dbo].[ProfileJobs])
GO

INSERT INTO [EKM_App].[dbo].[ProfileScientific]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[ProfileScientific] AS S
WHERE S.ID NOT IN(SELECT ID FROM [EKM_App].[dbo].[ProfileScientific])
GO

INSERT INTO [EKM_App].[dbo].[PRVC_Audience]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[PRVC_Audience] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[PRVC_Audience] AS Ref
	WHERE Ref.RoleID = S.RoleID AND Ref.ObjectID = S.ObjectID)
GO

INSERT INTO [EKM_App].[dbo].[QA_Questions]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[QA_Questions] AS S
WHERE S.QuestionID NOT IN(SELECT QuestionID FROM [EKM_App].[dbo].[QA_Questions])
GO

INSERT INTO [EKM_App].[dbo].[QA_Answers]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[QA_Answers] AS S
WHERE S.AnswerID NOT IN(SELECT AnswerID FROM [EKM_App].[dbo].[QA_Answers])
GO

INSERT INTO [EKM_App].[dbo].[QA_QuestionLikes]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[QA_QuestionLikes] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[QA_QuestionLikes] AS Ref
	WHERE Ref.QuestionID = S.QuestionID AND Ref.UserID = S.UserID)
GO

INSERT INTO [EKM_App].[dbo].[QA_RefUsers]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[QA_RefUsers] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[QA_RefUsers] AS Ref
	WHERE Ref.UserID = S.UserID AND Ref.QuestionID = S.QuestionID)
GO

INSERT INTO [EKM_App].[dbo].[QA_RefNodes]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[QA_RefNodes] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[QA_RefNodes] AS Ref
	WHERE Ref.NodeID = S.NodeID AND Ref.QuestionID = S.QuestionID)
GO

INSERT INTO [EKM_App].[dbo].[SH_PostTypes]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[SH_PostTypes] AS S
WHERE S.PostTypeID NOT IN(SELECT PostTypeID FROM [EKM_App].[dbo].[SH_PostTypes])
GO

INSERT INTO [EKM_App].[dbo].[SH_Posts]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[SH_Posts] AS S
WHERE S.PostID NOT IN(SELECT PostID FROM [EKM_App].[dbo].[SH_Posts])
GO

INSERT INTO [EKM_App].[dbo].[SH_PostShares]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[SH_PostShares] AS S
WHERE S.ShareID NOT IN(SELECT ShareID FROM [EKM_App].[dbo].[SH_PostShares])
GO

INSERT INTO [EKM_App].[dbo].[SH_Comments]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[SH_Comments] AS S
WHERE S.CommentID NOT IN(SELECT CommentID FROM [EKM_App].[dbo].[SH_Comments])
GO

INSERT INTO [EKM_App].[dbo].[SH_CommentLikes]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[SH_CommentLikes] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[SH_CommentLikes] AS Ref
	WHERE Ref.CommentID = S.CommentID AND Ref.UserID = S.UserID)
GO

INSERT INTO [EKM_App].[dbo].[SH_ShareLikes]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[SH_ShareLikes] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[SH_ShareLikes] AS Ref
	WHERE Ref.ShareID = S.ShareID AND Ref.UserID = S.UserID)
GO

INSERT INTO [EKM_App].[dbo].[UserConnections]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[UserConnections] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[UserConnections] AS Ref
	WHERE Ref.SenderUserID = S.SenderUserID AND Ref.ReceiverUserID = S.ReceiverUserID)
GO

INSERT INTO [EKM_App].[dbo].[UserGroups]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[UserGroups] AS S
WHERE S.ID NOT IN(SELECT ID FROM [EKM_App].[dbo].[UserGroups])
GO

INSERT INTO [EKM_App].[dbo].[UserGroupUsers]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[UserGroupUsers] AS S
WHERE S.ID NOT IN(SELECT ID FROM [EKM_App].[dbo].[UserGroupUsers])
GO

INSERT INTO [EKM_App].[dbo].[AccessRoles]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[AccessRoles] AS S
WHERE S.ID NOT IN(SELECT ID FROM [EKM_App].[dbo].[AccessRoles])
GO

INSERT INTO [EKM_App].[dbo].[UserGroupAccessRoles]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[UserGroupAccessRoles] AS S
WHERE S.ID NOT IN(SELECT ID FROM [EKM_App].[dbo].[UserGroupAccessRoles])
GO

INSERT INTO [EKM_App].[dbo].[USR_ItemVisits]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[USR_ItemVisits] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[USR_ItemVisits] AS Ref
	WHERE Ref.ItemID = S.ItemID AND Ref.VisitDate = S.VisitDate AND
		Ref.UserID = S.UserID)
GO

INSERT INTO [EKM_App].[dbo].[WK_Titles]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[WK_Titles] AS S
WHERE S.TitleID NOT IN(SELECT TitleID FROM [EKM_App].[dbo].[WK_Titles])
GO

UPDATE Ref
	SET Title = S.Title,
		SequenceNo = S.SequenceNo,
		[Status] = S.[Status],
		Deleted = S.Deleted
FROM [RAAIVAN].[EKM_App].[dbo].[WK_Titles] AS S
	INNER JOIN [EKM_App].[dbo].[WK_Titles] AS Ref
	ON S.TitleID = Ref.TitleID
GO

INSERT INTO [EKM_App].[dbo].[WK_Paragraphs]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[WK_Paragraphs] AS S
WHERE S.ParagraphID NOT IN(SELECT ParagraphID FROM [EKM_App].[dbo].[WK_Paragraphs])
GO

UPDATE Ref
	SET Title = S.Title,
		BodyText = S.BodyText,
		SequenceNo = S.SequenceNo,
		[Status] = S.[Status],
		Deleted = S.Deleted
FROM [RAAIVAN].[EKM_App].[dbo].[WK_Paragraphs] AS S
	INNER JOIN [EKM_App].[dbo].[WK_Paragraphs] AS Ref
	ON S.ParagraphID = Ref.ParagraphID
GO

INSERT INTO [EKM_App].[dbo].[WK_Changes]
SELECT * FROM [RAAIVAN].[EKM_App].[dbo].[WK_Changes] AS S
WHERE S.ChangeID NOT IN(SELECT ChangeID FROM [EKM_App].[dbo].[WK_Changes])
GO

UPDATE Ref
	SET Title = S.Title,
		BodyText = S.BodyText,
		Applied = S.Applied,
		[Status] = S.[Status],
		Deleted = S.Deleted
FROM [RAAIVAN].[EKM_App].[dbo].[WK_Changes] AS S
	INNER JOIN [EKM_App].[dbo].[WK_Changes] AS Ref
	ON S.ParagraphID = Ref.ParagraphID
GO

INSERT INTO [EKM_App].[dbo].[NTFN_Notifications](
	UserID, SubjectID, RefItemID, SubjectType, SubjectName, [Action], SenderUserID, 
	SendDate, [Description], Info, UserStatus, Seen, ViewDate, Deleted
)
SELECT UserID, SubjectID, RefItemID, SubjectType, SubjectName, [Action], SenderUserID, 
	SendDate, [Description], Info, UserStatus, Seen, ViewDate, Deleted
FROM [RAAIVAN].[EKM_App].[dbo].[NTFN_Notifications] AS S
WHERE S.ID NOT IN(SELECT ID FROM [EKM_App].[dbo].[NTFN_Notifications])
GO


INSERT INTO [EKM_App].[dbo].[WF_States]
SELECT *
FROM [RAAIVAN].[EKM_App].[dbo].[WF_States] AS S
WHERE S.StateID NOT IN(SELECT StateID FROM [EKM_App].[dbo].[WF_States])
GO

INSERT INTO [EKM_App].[dbo].[WF_WorkFlows]
SELECT *
FROM [RAAIVAN].[EKM_App].[dbo].[WF_WorkFlows] AS S
WHERE S.WorkFlowID NOT IN(SELECT WorkFlowID FROM [EKM_App].[dbo].[WF_WorkFlows])
GO

INSERT INTO [EKM_App].[dbo].[WF_WorkFlowStates]
SELECT *
FROM [RAAIVAN].[EKM_App].[dbo].[WF_WorkFlowStates] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[WF_WorkFlowStates]
	WHERE WorkFlowID = S.WorkFlowID AND StateID = S.StateID)
GO

INSERT INTO [EKM_App].[dbo].[WF_StateConnections]
SELECT *
FROM [RAAIVAN].[EKM_App].[dbo].[WF_StateConnections] AS S
WHERE NOT EXISTS(SELECT TOP(1) * FROM [EKM_App].[dbo].[WF_StateConnections]
	WHERE WorkFlowID = S.WorkFlowID AND InStateID = S.InStateID AND OutStateID = S.OutStateID)
GO

INSERT INTO [EKM_App].[dbo].[WF_AutoMessages]
SELECT *
FROM [RAAIVAN].[EKM_App].[dbo].[WF_AutoMessages] AS S
WHERE S.AutoMessageID NOT IN(SELECT AutoMessageID FROM [EKM_App].[dbo].[WF_AutoMessages])
GO
