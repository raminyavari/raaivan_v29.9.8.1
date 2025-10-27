USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QA_FN_GetParentCategoryHierarchy]') 
            AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[QA_FN_GetParentCategoryHierarchy]
GO

CREATE FUNCTION [dbo].[QA_FN_GetParentCategoryHierarchy](
	@ApplicationID	uniqueidentifier,
	@CategoryID		uniqueidentifier
)	
RETURNS @OutputTable TABLE (
	CategoryID uniqueidentifier primary key clustered,
	ParentID uniqueidentifier,
	[Level] int,
	Name nvarchar(2000)
)
WITH ENCRYPTION
AS
BEGIN
	;WITH hierarchy (ID, ParentID, [Level], Name)
	AS
	(
		SELECT F.CategoryID AS ID, F.ParentID AS ParentID, 0 AS [Level], F.Name
		FROM [dbo].[QA_FAQCategories] AS F
		WHERE F.ApplicationID = @ApplicationID AND F.CategoryID = @CategoryID
		
		UNION ALL
		
		SELECT C.CategoryID AS ID, C.ParentID AS ParentID, [Level] + 1, C.Name
		FROM [dbo].[QA_FAQCategories] AS C
			INNER JOIN hierarchy AS HR
			ON C.CategoryID = HR.ParentID
		WHERE C.ApplicationID = @ApplicationID AND 
			C.CategoryID <> HR.ID AND C.Deleted = 0
	)
	INSERT INTO @OutputTable(CategoryID, ParentID, [Level], Name)
	SELECT * 
	FROM hierarchy AS H
	ORDER BY H.[Level] ASC
	
	RETURN
END

GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QA_FN_GetChildCategoriesHierarchy]') 
            AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[QA_FN_GetChildCategoriesHierarchy]
GO

CREATE FUNCTION [dbo].[QA_FN_GetChildCategoriesHierarchy](
	@ApplicationID	uniqueidentifier,
	@CategoryIDs	GuidTableType readonly
)	
RETURNS @OutputTable TABLE (
	CategoryID uniqueidentifier primary key clustered,
	ParentID uniqueidentifier,
	[Level] int,
	Name nvarchar(2000)
)
WITH ENCRYPTION
AS
BEGIN
	INSERT INTO @OutputTable(CategoryID, ParentID, [Level], Name)
	SELECT C.CategoryID AS ID, C.ParentID AS ParentID, 0 AS [Level], C.Name AS Name
	FROM @CategoryIDs AS N
		INNER JOIN [dbo].[QA_FAQCategories] AS C
		ON C.ApplicationID = @ApplicationID AND C.CategoryID = N.Value
	
	INSERT INTO @OutputTable(CategoryID, ParentID, [Level], Name)
	SELECT C.CategoryID AS ID, C.ParentID, 
		[Level] + 1, C.Name AS Name
	FROM [dbo].[QA_FAQCategories] AS C
		INNER JOIN @OutputTable AS HR
		ON C.ParentID = HR.CategoryID
	WHERE C.ApplicationID = @ApplicationID AND C.CategoryID <> HR.CategoryID AND C.Deleted = 0
	
	RETURN
END

GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QA_FN_GetQuestionContent]') 
    AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[QA_FN_GetQuestionContent]
GO

CREATE FUNCTION [dbo].[QA_FN_GetQuestionContent](
	@ApplicationID	uniqueidentifier,
	@QuestionID		uniqueidentifier
)
RETURNS nvarchar(max)
WITH ENCRYPTION
AS
BEGIN
	DECLARE @Ret nvarchar(max) = NULL

	;WITH Partitioned AS
	(
		SELECT	AN.AnswerID,
				CAST(SUBSTRING(ISNULL(AN.AnswerBody, N''), 1, 4000) AS nvarchar(4000)) AS Content,
				ROW_NUMBER() OVER (PARTITION BY AN.QuestionID ORDER BY AN.AnswerID) AS Number,
				COUNT(*) OVER (PARTITION BY AN.QuestionID) AS [Count]
		FROM [dbo].[QA_Answers] AS AN
		WHERE AN.ApplicationID = @ApplicationID AND
			AN.QuestionID = @QuestionID AND AN.Deleted = 0
	),
	Fetched AS
	(
		SELECT P.AnswerID, P.Content AS FullContent, P.Content, P.Number, P.[Count] 
		FROM Partitioned AS P
		WHERE P.Number = 1

		UNION ALL

		SELECT	P.AnswerID, C.FullContent + ' ' + P.Content, P.Content, P.Number, P.[Count]
		FROM Partitioned AS P
			INNER JOIN Fetched AS C 
			ON P.AnswerID = C.AnswerID AND P.Number = C.Number + 1
		WHERE P.Number <= 95
	)
	SELECT TOP(1) @Ret = F.FullContent
	FROM Fetched AS F
	WHERE F.Number = (CASE WHEN F.[Count] > 90 THEN 90 ELSE F.[Count] END)
	
	RETURN @Ret
END

GO
