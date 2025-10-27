USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RV_FN_KnowledgeSupplyIndicatorsReport]') 
    AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[RV_FN_KnowledgeSupplyIndicatorsReport]
GO

CREATE FUNCTION [dbo].[RV_FN_KnowledgeSupplyIndicatorsReport](
	@ApplicationID			uniqueidentifier,
	@CurrentUserID			uniqueidentifier,
	@ContentTypeIDsTemp		GuidTableType readonly,
	@UserIDsTemp			GuidTableType readonly,
	@AllUsers				bit,
	@LowerCreationDateLimit datetime,
	@UpperCreationDateLimit datetime
)
RETURNS @outputTable TABLE (
	UserID						uniqueidentifier,
	ContentID					uniqueidentifier,
	ContentCollaborationShare	float,
	ContentStatus				varchar(100),
	ContentScore				float,
	ContentPublished			bit,
	AnswerID					uniqueidentifier,
	WikiParagraphID				uniqueidentifier,
	[Date]						datetime
)
WITH ENCRYPTION
AS
BEGIN
	DECLARE @ContentTypeIDs GuidTableType
	INSERT INTO @ContentTypeIDs (Value) SELECT Ref.Value FROM @ContentTypeIDsTemp AS Ref

	DECLARE @ContentTypesCount int = (SELECT COUNT(*) FROM @ContentTypeIDs)

	DECLARE @UserIDs GuidTableType

	INSERT INTO @UserIDs ([Value])
	SELECT Ref.[Value]
	FROM @UserIDsTemp AS Ref

	INSERT INTO @outputTable (UserID, ContentID, ContentCollaborationShare,
		ContentStatus, ContentScore, ContentPublished, AnswerID, WikiParagraphID, [Date])
	(
		SELECT	NC.UserID,
				NC.NodeID AS ContentID,
				NC.CollaborationShare AS ContentCollaborationShare,
				Contents.[Status] AS ContentStatus,
				Contents.Score AS ContentScore,
				Contents.Searchable AS ContentPublished,
				NULL AS AnswerID,
				NULL AS WikiParagraphID,
				Contents.CreationDate AS [Date]
		FROM [dbo].[CN_NodeCreators] AS NC
			INNER JOIN [dbo].[CN_Nodes] AS Contents
			ON Contents.ApplicationID = @ApplicationID AND Contents.NodeID = NC.NodeID
			INNER JOIN [dbo].[Users_Normal] AS UN
			ON UN.ApplicationID = @ApplicationID AND UN.UserID = NC.UserID AND UN.IsApproved = 1
		WHERE NC.ApplicationID = @ApplicationID AND 
			(@AllUsers = 1 OR NC.UserID IN (SELECT U.[Value] FROM @UserIDs AS U)) AND NC.Deleted = 0 AND 
			(@ContentTypesCount = 0 OR Contents.NodeTypeID IN (SELECT X.Value FROM @ContentTypeIDs AS X)) AND 
			(@LowerCreationDateLimit IS NULL OR Contents.CreationDate >= @LowerCreationDateLimit) AND
			(@UpperCreationDateLimit IS NULL OR Contents.CreationDate < @UpperCreationDateLimit) AND
			Contents.Deleted = 0

		UNION ALL

		SELECT	A.SenderUserID AS UserID,
				NULL AS ContentID,
				NULL AS ContentCollaborationShare,
				NULL AS ContentStatus,
				NULL AS ContentScore,
				NULL AS ContentPublished,
				A.AnswerID,
				NULL AS WikiParagraphID,
				A.SendDate AS [Date]
		FROM [dbo].[QA_Answers] AS A
			INNER JOIN [dbo].[QA_Questions] AS Q
			ON Q.ApplicationID = @ApplicationID AND Q.QuestionID = A.QuestionID AND Q.Deleted = 0
			INNER JOIN [dbo].[Users_Normal] AS UN
			ON UN.ApplicationID = @ApplicationID AND UN.UserID = A.SenderUserID AND UN.IsApproved = 1
		WHERE A.ApplicationID = @ApplicationID AND 
			(@AllUsers = 1 OR A.SenderUserID IN (SELECT U.[Value] FROM @UserIDs AS U)) AND A.Deleted = 0 AND
			(@LowerCreationDateLimit IS NULL OR A.SendDate >= @LowerCreationDateLimit) AND
			(@UpperCreationDateLimit IS NULL OR A.SendDate < @UpperCreationDateLimit)

		UNION ALL

		SELECT	C.UserID,
				NULL AS ContentID,
				NULL AS ContentCollaborationShare,
				NULL AS ContentStatus,
				NULL AS ContentScore,
				NULL AS ContentPublished,
				NULL AS AnswerID,
				C.ParagraphID AS WikiParagraphID,
				C.SendDate AS [Date]
		FROM [dbo].[WK_Changes] AS C
			INNER JOIN [dbo].[WK_Paragraphs] AS P
			ON P.ApplicationID = @ApplicationID AND P.ParagraphID = C.ParagraphID AND 
				P.Deleted = 0 AND (P.[Status] = N'Accepted' OR P.[Status] = N'CitationNeeded')
			INNER JOIN [dbo].[WK_Titles] AS T
			ON T.ApplicationID = @ApplicationID AND T.TitleID = P.TitleID AND T.Deleted = 0
			INNER JOIN [dbo].[CN_Nodes] AS ND
			ON ND.ApplicationID = @ApplicationID AND ND.NodeID = T.OwnerID AND 
				ND.Deleted = 0 AND (ND.Searchable = 1 OR ND.[Status] = N'Accepted') AND
				(@ContentTypesCount = 0 OR ND.NodeTypeID IN (SELECT X.Value FROM @ContentTypeIDs AS X))
			INNER JOIN [dbo].[Users_Normal] AS UN
			ON UN.ApplicationID = @ApplicationID AND UN.UserID = C.UserID AND UN.IsApproved = 1
		WHERE C.ApplicationID = @ApplicationID AND 
			(@AllUsers = 1 OR C.UserID IN (SELECT U.[Value] FROM @UserIDs AS U)) AND 
			(@LowerCreationDateLimit IS NULL OR C.SendDate >= @LowerCreationDateLimit) AND
			(@UpperCreationDateLimit IS NULL OR C.SendDate < @UpperCreationDateLimit) AND
			(C.[Status] = N'Accepted' OR C.Applied = 1) AND C.Deleted = 0
	)

	RETURN
END

GO