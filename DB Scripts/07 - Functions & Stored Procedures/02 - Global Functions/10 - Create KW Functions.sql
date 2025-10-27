USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[KW_FN_GetWFVersionID]') 
            AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[KW_FN_GetWFVersionID]
GO

CREATE FUNCTION [dbo].[KW_FN_GetWFVersionID](
	@ApplicationID	uniqueidentifier,
	@KnowledgeID	uniqueidentifier
)	
RETURNS int
WITH ENCRYPTION
AS
BEGIN
	RETURN ISNULL((
		SELECT TOP(1) MAX(H.WFVersionID)
		FROM [dbo].[KW_History] AS H
		WHERE H.ApplicationID = @ApplicationID AND H.KnowledgeID = @KnowledgeID
	), 1)
END

GO


