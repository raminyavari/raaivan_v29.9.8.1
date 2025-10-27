USE [EKM_App]
GO


ALTER TABLE [dbo].[CN_Nodes]
ADD [DocumentTreeNodeID] [uniqueidentifier] NULL,
	[PreviousVersionID] [uniqueidentifier] NULL,
	[PublicationDate] [datetime] NULL,
	[Status] [varchar](20) NULL,
	[Score] [float] NULL
GO


UPDATE ND
	SET DocumentTreeNodeID = KW.TreeNodeID,
		PreviousVersionID = KW.PreviousVersionID,
		PublicationDate = KW.PublicationDate,
		[Status] = S.Name,
		Score = KW.Score,
		Searchable = CASE WHEN KW.StatusID = 6 THEN 1 ELSE 0 END
FROM [dbo].[KW_Knowledges] AS KW
	INNER JOIN [dbo].[CN_Nodes] AS ND
	ON ND.NodeID = KW.KnowledgeID
	LEFT JOIN [dbo].[KWF_Statuses] AS S
	ON KW.StatusID = S.StatusID
GO