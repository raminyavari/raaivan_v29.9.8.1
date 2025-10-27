USE [EKM_App]
GO



IF  EXISTS (SELECT * FROM sys.objects 
            WHERE object_id = OBJECT_ID(N'[dbo].[GFN_Base64Encode]') 
            AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[GFN_Base64Encode]
GO

CREATE FUNCTION [dbo].[GFN_Base64Encode]
(
	@input	nvarchar(max)
)
RETURNS nvarchar(max)
WITH ENCRYPTION
AS
BEGIN
	IF @input IS NULL OR @input = N'' RETURN @input

	DECLARE @binaryText varbinary(max) = CONVERT(varbinary(max), @input)
	
	RETURN CAST('' AS xml).value('xs:base64Binary(sql:variable("@binaryText"))', 'nvarchar(max)')
END

GO


IF  EXISTS (SELECT * FROM sys.objects 
            WHERE object_id = OBJECT_ID(N'[dbo].[GFN_Base64Decode]') 
            AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[GFN_Base64Decode]
GO

CREATE FUNCTION [dbo].[GFN_Base64Decode]
(
	@input	nvarchar(max)
)
RETURNS nvarchar(max)
WITH ENCRYPTION
AS
BEGIN
	IF @input IS NULL OR @input = N'' RETURN @input

	DECLARE @binaryText varbinary(max) = 
		CAST('' AS xml).value('xs:base64Binary(sql:variable("@input"))', 'varbinary(max)')
	
	RETURN CONVERT(nvarchar(max), @binaryText)
END

GO



/****** Object:  View [dbo].[Users_Normal]    Script Date: 05/31/2012 21:31:04 ******/
SET NUMERIC_ROUNDABORT OFF;
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT,
    QUOTED_IDENTIFIER, ANSI_NULLS ON;


IF EXISTS(select * FROM sys.views where name = 'Users_Normal')
DROP VIEW [dbo].[Users_Normal]
GO


CREATE VIEW [dbo].[Users_Normal] WITH SCHEMABINDING, ENCRYPTION
AS
SELECT  U.UserId AS UserID, 
		U.UserName AS UserName, 
		P.FirstName AS FirstName, 
		P.LastName AS LastName, 
		P.BirthDay AS BirthDay,
		P.JobTitle AS JobTitle,
		P.EmploymentType AS EmploymentType,
		P.MainPhoneID AS MainPhoneID,
		P.MainEmailID AS MainEmailID,
		M.IsApproved AS IsApproved,
		M.IsLockedOut AS IsLockedOut,
		M.CreateDate AS CreationDate,
		P.IndexLastUpdateDate AS IndexLastUpdateDate
FROM    [dbo].[aspnet_Users] AS U
		INNER JOIN [dbo].[USR_Profile] AS P
		ON U.UserId = P.UserID 
		INNER JOIN [dbo].[aspnet_Membership] AS M
		ON U.UserId = M.UserId

GO

CREATE UNIQUE CLUSTERED INDEX PK_View_Users_Normal_UserID ON [dbo].[Users_Normal]
(
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[TMP_SetWFDashboards]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[TMP_SetWFDashboards]
GO

CREATE PROCEDURE [dbo].[TMP_SetWFDashboards]
	@UserID uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @WFDash Table(NodeID uniqueidentifier, HistoryID uniqueidentifier,
		DataNeedInstanceID uniqueidentifier, CreationDate datetime, 
		Removable bit, Info nvarchar(max), StateID uniqueidentifier,
		[StateTitle] nvarchar(1000), WorkFlowID uniqueidentifier, 
		WorkFlowName nvarchar(max))
	
	DECLARE @CreatedNodeIDs GuidTableType
	INSERT INTO @CreatedNodeIDs
	SELECT Ref.ID
	FROM (
			SELECT NodeID AS ID
			FROM [dbo].[CN_Nodes]
			WHERE CreatorUserID = @UserID AND Deleted = 0
			
			UNION ALL
			
			SELECT NC.NodeID
			FROM [dbo].[CN_NodeCreators] AS NC
				INNER JOIN [dbo].[CN_Nodes] AS ND
				ON NC.NodeID = ND.NodeID
			WHERE NC.UserID = @UserID AND NC.Deleted = 0 AND 
				ND.CreatorUserID <> @UserID AND ND.Deleted = 0
		) AS Ref
	
	
	INSERT INTO @WFDash(NodeID, HistoryID, DataNeedInstanceID, CreationDate, 
		Removable, StateID, StateTitle, WorkFlowID, WorkFlowName
	)
	SELECT Ref.NodeID, Ref.HistoryID, Ref.InstanceID, Ref.CreationDate, Ref.Removable,
		Ref.StateID, Ref.StateTitle, Ref.WorkFlowID, Ref.WorkFlowName
	FROM (
		SELECT A.OwnerID AS NodeID,
			   A.HistoryID,
			   NULL AS InstanceID,
			   A.SendDate AS CreationDate,
			   CASE
					WHEN W.[Admin] = 1 AND N.IsAdmin = 0 THEN CAST(1 AS bit)
					ELSE CAST(0 AS bit)
			   END AS Removable,
			   ST.StateID,
			   ST.Title AS StateTitle,
			   WF.WorkFlowID,
			   WF.Name AS WorkFlowName
		FROM [dbo].[WF_History] AS A
			INNER JOIN
			(SELECT OwnerID, MAX(SendDate) AS SendDate
			 FROM [dbo].[WF_History]
			 GROUP BY OwnerID) AS B
			ON A.OwnerID = B.OwnerID AND A.SendDate = B.SendDate
			INNER JOIN [dbo].[WF_WorkFlowStates] AS W
			ON W.StateID = A.StateID
			INNER JOIN [dbo].[CN_NodeMembers] AS N
			ON N.UserID = @UserID AND N.NodeID = A.DirectorNodeID
			INNER JOIN [dbo].[CN_View_Nodes_Normal] AS ND
			ON A.OwnerID = ND.NodeID
			LEFT JOIN [dbo].[WF_States] AS ST
			ON A.StateID = ST.StateID
			INNER JOIN [dbo].[WF_WorkFlows] AS WF
			ON A.WorkFlowID = WF.WorkFlowID
		WHERE A.DirectorUserID IS NULL AND N.[Status] = 'Accepted' AND 
			--(W.IsAdmin = 0 OR N.IsAdmin = w.IsAdmin) AND 
			N.Deleted = 0 AND A.Terminated = 0 AND A.Deleted = 0 AND ND.Deleted = 0
			
		UNION ALL
		
		SELECT A.OwnerID AS NodeID,
			   A.HistoryID,
			   NULL AS InstanceID,
			   A.SendDate AS CreationDate,
			   CAST(0 AS bit) AS Removable,
			   ST.StateID,
			   ST.Title AS StateTitle,
			   WF.WorkFlowID,
			   WF.Name
		FROM [dbo].[WF_History] AS A
			INNER JOIN
			(SELECT OwnerID, MAX(SendDate) AS SendDate
			 FROM [dbo].[WF_History]
			 GROUP BY OwnerID) AS B
			ON A.OwnerID = B.OwnerID AND A.SendDate = B.SendDate
			INNER JOIN [dbo].[CN_View_Nodes_Normal] AS ND
			ON A.OwnerID = ND.NodeID
			LEFT JOIN [dbo].[WF_States] AS ST
			ON A.StateID = ST.StateID
			INNER JOIN [dbo].[WF_WorkFlows] AS WF
			ON A.WorkFlowID = WF.WorkFlowID
		--WHERE A.DirectorUserID = @UserID AND 
		WHERE A.DirectorUserID IS NOT NULL AND 
			A.OwnerID IN(SELECT * FROM @CreatedNodeIDs) AND 
			A.Terminated = 0 AND A.Deleted = 0 AND ND.Deleted = 0
			
		UNION ALL
		
		SELECT HS.OwnerID AS NodeID,
			   NULL AS HistoryID,
			   SDNI.InstanceID AS InstanceID,
			   SDNI.CreationDate AS CreationDate,
			   CAST(1 AS bit)  AS Removable,
			   ST.StateID,
			   ST.Title AS StateTitle,
			   WF.WorkFlowID,
			   WF.Name
		FROM [dbo].[WF_StateDataNeedInstances] AS SDNI
			INNER JOIN [dbo].[CN_NodeMembers] AS NM
			ON NM.UserID = @UserID AND NM.NodeID = SDNI.NodeID
			INNER JOIN [dbo].[WF_History] AS HS
			ON SDNI.HistoryID = HS.HistoryID
			INNER JOIN [dbo].[CN_View_Nodes_Normal] AS ND
			ON HS.OwnerID = ND.NodeID
			LEFT JOIN [dbo].[WF_States] AS ST
			ON HS.StateID = ST.StateID
			INNER JOIN [dbo].[WF_WorkFlows] AS WF
			ON HS.WorkFlowID = WF.WorkFlowID
		WHERE NM.[Status] = 'Accepted' AND 
			(SDNI.[Admin] = 0 OR NM.IsAdmin = SDNI.[Admin]) AND
			SDNI.Filled = 0 AND NM.Deleted = 0 AND SDNI.Deleted = 0 AND ND.Deleted = 0
	) AS Ref
	
	
	UPDATE Ref
		SET Info = '{"WorkFlowName":"' + ISNULL([dbo].[GFN_Base64encode](Ref.WorkFlowName), N'') + 
			'","WorkFlowState":"' + ISNULL([dbo].[GFN_Base64encode](Ref.StateTitle), N'') +
			(
				CASE
					WHEN Ref.DataNeedInstanceID IS NULL THEN N''
					ELSE '","DataNeedInstanceID":"' + CAST(Ref.DataNeedInstanceID AS varchar(50))
				END
			) +
			'"}'
	FROM @WFDash AS Ref
	
	
	INSERT INTO [dbo].[NTFN_Dashboards](
		UserID,
		NodeID,
		RefItemID,
		[Type],
		Info,
		Removable,
		SendDate,
		Seen,
		Done,
		Deleted
	)
	SELECT @UserID, Ref.NodeID, ISNULL(Ref.DataNeedInstanceID, Ref.HistoryID), N'WorkFlow', 
		Ref.Info, Ref.Removable, Ref.CreationDate, 0, 0, 0
	FROM @WFDash AS Ref
	ORDER BY Ref.CreationDate
END

GO


DECLARE @UserIDs TABLE (ID int identity(1,1) primary key clustered, UserID uniqueidentifier)
INSERT INTO @UserIDs (UserID)
SELECT UserID 
FROM [dbo].[Users_Normal]

DECLARE @Count int  = (SELECT COUNT(*) FROM @UserIDs)

WHILE @Count > 0 BEGIN
	DECLARE @CurID uniqueidentifier = (SELECT UserID FROM @UserIDs WHERE ID = @Count)
	
	EXEC [dbo].[TMP_SetWFDashboards] @CurID
	
	SET @Count = @Count - 1
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[TMP_SetWFDashboards]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[TMP_SetWFDashboards]
GO

IF EXISTS(select * FROM sys.views where name = 'Users_Normal')
DROP VIEW [dbo].[Users_Normal]
GO


IF  EXISTS (SELECT * FROM sys.objects 
            WHERE object_id = OBJECT_ID(N'[dbo].[GFN_Base64Encode]') 
            AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[GFN_Base64Encode]
GO

IF  EXISTS (SELECT * FROM sys.objects 
            WHERE object_id = OBJECT_ID(N'[dbo].[GFN_Base64Decode]') 
            AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[GFN_Base64Decode]
GO