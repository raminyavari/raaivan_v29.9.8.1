USE [EKM_App]
GO

SET ANSI_PADDING ON
GO

UPDATE N
	SET N.AdditionalID = '9'
FROM [dbo].[TMP_KW_TempKnowledgeTypeIDs] AS T
	INNER JOIN [dbo].[CN_NodeTypes] AS N
	ON N.ApplicationID = T.ApplicationID AND N.NodeTypeID = T.[Guid]
WHERE T.IntID = 1 AND N.AdditionalID IS NULL

UPDATE N
	SET N.AdditionalID = '8'
FROM [dbo].[TMP_KW_TempKnowledgeTypeIDs] AS T
	INNER JOIN [dbo].[CN_NodeTypes] AS N
	ON N.ApplicationID = T.ApplicationID AND N.NodeTypeID = T.[Guid]
WHERE T.IntID = 2 AND N.AdditionalID IS NULL

UPDATE N
	SET N.AdditionalID = '10'
FROM [dbo].[TMP_KW_TempKnowledgeTypeIDs] AS T
	INNER JOIN [dbo].[CN_NodeTypes] AS N
	ON N.ApplicationID = T.ApplicationID AND N.NodeTypeID = T.[Guid]
WHERE T.IntID = 3 AND N.AdditionalID IS NULL

GO

SET ANSI_PADDING OFF
GO
