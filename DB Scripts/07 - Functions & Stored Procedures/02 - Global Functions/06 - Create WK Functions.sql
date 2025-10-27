USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WK_FN_HasWikiContent]') 
    AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[WK_FN_HasWikiContent]
GO

CREATE FUNCTION [dbo].[WK_FN_HasWikiContent](
	@ApplicationID	uniqueidentifier,
	@OwnerID		uniqueidentifier
)
RETURNS bit
WITH ENCRYPTION
AS
BEGIN
	RETURN CAST((
		SELECT TOP(1) 1
		FROM [dbo].[WK_Titles] AS TT
			INNER JOIN [dbo].[WK_Paragraphs] AS P
			ON P.ApplicationID = @ApplicationID AND P.TitleID = TT.TitleID
		WHERE TT.ApplicationID = @ApplicationID AND TT.OwnerID = @OwnerID AND 
			TT.Deleted = 0 AND P.Deleted = 0 AND ISNULL(P.BodyText, N'') <> N'' AND
			(P.[Status] = N'Accepted' OR P.[Status] = N'CitationNeeded')
	) AS bit)
END

GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WK_FN_GetWikiContent]') 
    AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[WK_FN_GetWikiContent]
GO

CREATE FUNCTION [dbo].[WK_FN_GetWikiContent](
	@ApplicationID	uniqueidentifier,
	@OwnerID		uniqueidentifier
)
RETURNS nvarchar(max)
WITH ENCRYPTION
AS
BEGIN
	/*
	RETURN STUFF(
		(
			SELECT	N' <h1>' + ISNULL(TT.Title, N'') + N'</h1> ' + 
					N' <h2>' + ISNULL(P.Title, N'') + N'</h2> ' + ISNULL(P.BodyText, N'')
			FROM [dbo].[WK_Titles] AS TT
				INNER JOIN [dbo].[WK_Paragraphs] AS P
				ON P.ApplicationID = @ApplicationID AND P.TitleID = TT.TitleID
			WHERE TT.ApplicationID = @ApplicationID AND TT.OwnerID = @OwnerID AND 
				TT.Deleted = 0 AND P.Deleted = 0 AND
				(P.[Status] = N'Accepted' OR P.[Status] = N'CitationNeeded')
			FOR xml path('a'), type
		).value('.','nvarchar(max)'),
		1,
		1,
		''
	)
	*/

	DECLARE @Ret nvarchar(max) = NULL

	;WITH Partitioned AS
	(
		SELECT	TT.OwnerID,
				N' <h1>' + ISNULL(TT.Title, N'') + N'</h1> ' + 
				N' <h2>' + ISNULL(P.Title, N'') + N'</h2> ' + 
				CAST(SUBSTRING(ISNULL(P.BodyText, N''), 1, 4000) AS nvarchar(4000)) AS Content,
				ROW_NUMBER() OVER (PARTITION BY TT.OwnerID ORDER BY P.ParagraphID) AS Number,
				COUNT(*) OVER (PARTITION BY TT.OwnerID) AS [Count]
		FROM [dbo].[WK_Titles] AS TT
			INNER JOIN [dbo].[WK_Paragraphs] AS P
			ON P.ApplicationID = @ApplicationID AND P.TitleID = TT.TitleID
		WHERE TT.ApplicationID = @ApplicationID AND TT.OwnerID = @OwnerID AND 
			TT.Deleted = 0 AND P.Deleted = 0 AND ISNULL(P.BodyText, N'') <> N'' AND
			(P.[Status] = N'Accepted' OR P.[Status] = N'CitationNeeded')
	),
	Fetched AS
	(
		SELECT	P.OwnerID, P.Content AS FullContent, P.Content, P.Number, P.[Count] 
		FROM Partitioned AS P
		WHERE P.Number = 1

		UNION ALL

		SELECT	P.OwnerID, C.FullContent + ' ' + P.Content, P.Content, P.Number, P.[Count]
		FROM Partitioned AS P
			INNER JOIN Fetched AS C 
			ON P.OwnerID = C.OwnerID AND P.Number = C.Number + 1
		WHERE P.Number <= 95
	)
	SELECT TOP(1) @Ret = F.FullContent
	FROM Fetched AS F
	WHERE F.Number = (CASE WHEN F.[Count] > 90 THEN 90 ELSE F.[Count] END)
	
	RETURN @Ret
END

GO
