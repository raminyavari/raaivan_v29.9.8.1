USE [EKM_App]
GO

SET NUMERIC_ROUNDABORT OFF;
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT,
    QUOTED_IDENTIFIER, ANSI_NULLS ON;


IF EXISTS(select * FROM sys.views where name = 'CN_View_TagRelations_WikiContext')
DROP VIEW [dbo].[CN_View_TagRelations_WikiContext]
GO

CREATE VIEW [dbo].[CN_View_TagRelations_WikiContext] WITH SCHEMABINDING, ENCRYPTION
AS
SELECT DISTINCT	
		TI.ApplicationID,
		Context.NodeID AS ContextID,
		TI.TaggedID,
		TI.TaggedType
FROM [dbo].[RV_TaggedItems] AS TI
	INNER JOIN [dbo].[WK_Paragraphs] AS P
	ON P.ApplicationID = TI.ApplicationID AND P.ParagraphID = TI.ContextID
	INNER JOIN [dbo].[WK_Titles] AS T
	ON T.ApplicationID = TI.ApplicationID AND T.TitleID = P.TitleID
	INNER JOIN [dbo].[CN_Nodes] AS Context
	ON Context.ApplicationID = TI.ApplicationID AND Context.NodeID = T.OwnerID
WHERE TI.TaggedType IN (N'Node', N'User') AND  P.Deleted = 0 AND 
	(P.[Status] = N'Accepted' OR P.[Status] = N'CitationNeeded') AND T.Deleted = 0

GO

