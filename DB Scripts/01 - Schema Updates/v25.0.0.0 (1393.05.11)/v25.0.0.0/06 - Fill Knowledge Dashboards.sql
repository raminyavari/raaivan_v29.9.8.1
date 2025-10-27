USE [EKM_App]
GO


IF EXISTS(select * FROM sys.views where name = 'KW_View_ContentFileExtensions')
DROP VIEW [dbo].[KW_View_ContentFileExtensions]
GO

IF EXISTS(select * FROM sys.views where name = 'KW_View_Knowledges')
DROP VIEW [dbo].[KW_View_Knowledges]
GO

CREATE VIEW [dbo].[KW_View_Knowledges] WITH SCHEMABINDING, ENCRYPTION
AS
SELECT     dbo.KW_Knowledges.KnowledgeID, dbo.KW_Knowledges.KnowledgeTypeID,
		   dbo.KW_KnowledgeTypes.Name AS KnowledgeType, 
		   dbo.CN_Nodes.AdditionalID AS AdditionalID,
		   dbo.KW_Knowledges.PreviousVersionID,
		   dbo.KW_Knowledges.ContentType, dbo.KW_Knowledges.IsDefault, 
           dbo.KW_Knowledges.ExtendedFormID, dbo.KW_Knowledges.TreeNodeID, 
           dbo.KW_Knowledges.ConfidentialityLevelID, dbo.KW_Knowledges.StatusID, 
           dbo.KW_Knowledges.PublicationDate, dbo.CN_Nodes.Name AS Title, 
           dbo.CN_Nodes.CreatorUserID, dbo.CN_Nodes.LastModifierUserID, 
           dbo.CN_Nodes.CreationDate, dbo.CN_Nodes.LastModificationDate, 
           dbo.KW_Knowledges.Score, dbo.KW_Knowledges.ScoresWeight,
           dbo.CN_Nodes.Privacy, dbo.CN_Nodes.Deleted
FROM       dbo.CN_Nodes INNER JOIN dbo.KW_Knowledges ON 
		   dbo.CN_Nodes.NodeID = dbo.KW_Knowledges.KnowledgeID INNER JOIN
		   dbo.KW_KnowledgeTypes ON 
		   dbo.KW_Knowledges.KnowledgeTypeID = dbo.KW_KnowledgeTypes.KnowledgeTypeID

GO



CREATE TYPE [dbo].[TMPDashboardTableType] AS TABLE(
	UserID			uniqueidentifier NOT NULL,
	NodeID			uniqueidentifier NOT NULL,
	RefItemID		uniqueidentifier NULL,
	[Type]			varchar(20) NOT NULL,
	SubType			varchar(20) NULL,
	Info			nvarchar(max) NULL,
	Removable		bit	NULL,
	SenderUserID	uniqueidentifier NULL,
	SendDate		datetime NULL,
	ExpirationDate	datetime NULL,
	Seen			bit NULL,
	ViewDate		datetime NULL,
	Done			bit NULL,
	ActionDate		datetime NULL
)
GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[NTFN_P_SendDashboards]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[NTFN_P_SendDashboards]
GO

CREATE PROCEDURE [dbo].[NTFN_P_SendDashboards]
    @Dashboards		TMPDashboardTableType readonly,
    @_Result		int output
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	INSERT INTO [dbo].[NTFN_Dashboards](
		UserID,
		NodeID,
		RefItemID,
		[Type],
		SubType,
		Info,
		Removable,
		SenderUserID,
		SendDate,
		ExpirationDate,
		Seen,
		ViewDate,
		Done,
		ActionDate,
		Deleted
	)
	SELECT DISTINCT
		Ref.UserID,
		Ref.NodeID,
		ISNULL(Ref.RefItemID, Ref.NodeID),
		Ref.[Type],
		Ref.SubType,
		ref.Info,
		ISNULL(Ref.Removable, 0),
		Ref.SenderUserID,
		Ref.SendDate,
		Ref.ExpirationDate,
		ISNULL(Ref.Seen, 0),
		Ref.ViewDate,
		ISNULL(Ref.Done, 0),
		Ref.ActionDate,
		0
	FROM @Dashboards AS Ref
	/*
		LEFT JOIN [dbo].[NTFN_Dashboards] AS D
		ON D.UserID = Ref.UserID AND D.NodeID = Ref.NodeID AND 
			D.RefItemID = Ref.RefItemID AND D.[Type] = Ref.[Type] AND
			((D.SubType IS NULL AND Ref.SubType IS NULL) OR D.SubType = Ref.SubType) AND
			D.Done = 0 AND D.Deleted = 0
	WHERE D.ID IS NULL
	*/
	
	SET @_Result = @@ROWCOUNT
END

GO


DECLARE @Dashboards TMPDashboardTableType

INSERT INTO [dbo].[NTFN_Dashboards](
	UserID, 
	NodeID, 
	RefItemID, 
	[Type], 
	SubType, 
	SendDate,
	ExpirationDate,
	ViewDate,
	Done,
	ActionDate,
	Seen,
	Removable, 
	Deleted
)
SELECT Ref.*
FROM (
		SELECT	MNG.UserID AS UserID,
				MNG.KnowledgeID AS NodeID,
				MNG.KnowledgeID AS RefItemID,
				N'Knowledge' AS [Type],
				N'Admin' AS SubType,
				MNG.EntranceDate AS SendDate,
				NULL AS ExpirationDate,
				NULL AS ViewDate,
				MNG.[Sent] AS Done,
				MNG.EvaluationDate AS ActionDate,
				1 AS Seen,
				0 AS Removable,
				0 AS Deleted
		FROM [dbo].[KWF_Managers] AS MNG
			INNER JOIN [dbo].[KW_View_Knowledges] AS KW
			ON MNG.[KnowledgeID] = KW.[KnowledgeID]
		WHERE MNG.[Deleted] = 0 AND 
			KW.[StatusID] < 5 AND KW.[Deleted] = 0
			
		UNION ALL
		
		SELECT	Ev.UserID AS UserID,
				Ev.KnowledgeID AS NodeID,
				Ev.KnowledgeID AS RefItemID,
				N'Knowledge' AS [Type],
				N'Evaluator' AS SubType,
				MAX(Ev.EntranceDate) AS SendDate,
				NULL AS ExpirationDate,
				NULL AS ViewDate,
				MAX(CAST(Ev.[Evaluated] AS int)) AS Done,
				MAX(Ev.EvaluationDate) AS ActionDate,
				1 AS Seen,
				0 AS Removable,
				0 AS Deleted
		FROM (
				SELECT	Ex.UserID, Ex.KnowledgeID, Ex.EntranceDate, Ex.ExpirationDate, 
						Ex.Evaluated, Ex.EvaluationDate, 1 AS IsEx
				FROM [dbo].[KWF_Experts] AS Ex
					INNER JOIN [dbo].[KW_View_Knowledges] AS KW
					ON Ex.[KnowledgeID] = KW.[KnowledgeID]
				WHERE Ex.[Deleted] = 0 AND
					(Ex.[Rejected] IS NULL OR Ex.[Rejected] = 0) AND KW.[Deleted] = 0
					
				UNION ALL
				
				SELECT	Ev.UserID, Ev.KnowledgeID, Ev.EntranceDate, Ev.ExpirationDate, 
						Ev.Evaluated, Ev.EvaluationDate, 0 AS IsEx
				FROM [dbo].[KWF_Evaluators] AS Ev
					INNER JOIN [dbo].[KW_View_Knowledges] AS KW
					ON Ev.[KnowledgeID] = KW.[KnowledgeID]
				WHERE Ev.[Deleted] = 0 AND
					(Ev.[Rejected] IS NULL OR Ev.[Rejected] = 0) AND KW.[Deleted] = 0
			) AS Ev
		GROUP BY Ev.UserID, Ev.KnowledgeID
	) AS Ref

DECLARE @_Result int

EXEC [dbo].[NTFN_P_SendDashboards] @Dashboards, @_Result output

GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[NTFN_P_SendDashboards]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[NTFN_P_SendDashboards]
GO

DROP TYPE dbo.TMPDashboardTableType
GO

IF EXISTS(select * FROM sys.views where name = 'KW_View_Knowledges')
DROP VIEW [dbo].[KW_View_Knowledges]
GO