USE [EKM_App]
GO	

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_StateNodesCountReport]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_StateNodesCountReport]
GO

CREATE PROCEDURE [dbo].[WF_StateNodesCountReport]
	@ApplicationID			uniqueidentifier,
	@CurrentUserID			uniqueidentifier,
	@NodeTypeID				uniqueidentifier,
	@WorkFlowID				uniqueidentifier,
	@LowerCreationDateLimit	datetime,
	@UpperCreationDateLimit	datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT WFS.StateID AS StateID_Hide, ST.Title AS StateTitle, 
		WFS.WorkFlowID AS WorkFlowID_Hide, WF.Name AS WorkFlowTitle, 
		NT.NodeTypeID AS NodeTypeID_Hide, NT.Name AS NodeType,
		ISNULL(Ref.[Count], 0) AS [Count], CAST(ST.Deleted AS int) AS RemovedState
	FROM (
			SELECT A.StateID, 
				CAST(MAX(CAST(ND.NodeTypeID AS varchar(36))) AS uniqueidentifier) AS NodeTypeID,
				CAST(MAX(CAST(A.WorkFlowID AS varchar(36))) AS uniqueidentifier) AS WorkFlowID,
				COUNT(ND.NodeID) AS [Count]
			FROM [dbo].[WF_History] AS A
				INNER JOIN (
					SELECT OwnerID, MAX(SendDate) AS SendDate
					FROM [dbo].[WF_History]
					WHERE ApplicationID = @ApplicationID AND Deleted = 0
					GROUP BY OwnerID
				) AS B
				ON B.OwnerID = A.OwnerID AND B.SendDate = A.SendDate
				INNER JOIN [dbo].[CN_Nodes] AS ND
				ON ND.ApplicationID = @ApplicationID AND ND.NodeID = A.OwnerID
			WHERE A.ApplicationID = @ApplicationID AND 
				(@WorkFlowID IS NULL OR A.WorkFlowID = @WorkFlowID) AND
				(@NodeTypeID IS NULL OR ND.NodeTypeID = @NodeTypeID) AND
				(@LowerCreationDateLimit IS NULL OR ND.CreationDate >= @LowerCreationDateLimit) AND
				(@UpperCreationDateLimit IS NULL OR ND.CreationDate <= @UpperCreationDateLimit) AND
				 ND.Deleted = 0
			GROUP BY A.StateID
		) AS Ref
		RIGHT JOIN [dbo].[WF_WorkFlowStates] AS WFS
		ON WFS.ApplicationID = @ApplicationID AND 
			Ref.WorkFlowID = WFS.WorkFlowID AND WFS.StateID = Ref.StateID
		INNER JOIN [dbo].[WF_States] AS ST
		ON ST.ApplicationID = @ApplicationID AND ST.StateID = WFS.StateID
		INNER JOIN [dbo].[WF_WorkFlows] AS WF
		ON WF.ApplicationID = @ApplicationID AND WF.WorkFlowID = Ref.WorkFlowID
		INNER JOIN [dbo].[CN_NodeTypes] AS NT
		ON NT.ApplicationID = @ApplicationID AND NT.NodeTypeID = Ref.NodeTypeID
	WHERE WFS.ApplicationID = @ApplicationID AND (Ref.StateID IS NOT NULL OR WFS.Deleted = 0)
	
	
	SELECT ('{' +
			'"Count": {"Action": "Report",' + 
		   		'"ModuleIdentifier": "WF", "ReportName": "NodesWorkFlowStatesReport",' +
		   		'"Requires": {"StateID": {"Value": "StateID_Hide", "Title": "StateTitle"}}, ' + 
		   		'"Params": {"CurrentState": true }' + 
		   	'}' +
		   '}') AS Actions
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_NodesWorkFlowStatesReport]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_NodesWorkFlowStatesReport]
GO

CREATE PROCEDURE [dbo].[WF_NodesWorkFlowStatesReport]
	@ApplicationID			uniqueidentifier,
	@CurrentUserID			uniqueidentifier,
	@NodeTypeID				uniqueidentifier,
	@WorkFlowID				uniqueidentifier,
	@StateID				uniqueidentifier,
	@TagID					uniqueidentifier,
	@CurrentState			bit,
	@LowerCreationDateLimit datetime,
	@UpperCreationDateLimit datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @Results Table (
		NodeID_Hide uniqueidentifier primary key clustered, 
		Name nvarchar(1000), 
		AdditionalID varchar(1000), 
		Classification nvarchar(250),
		UserID_Hide uniqueidentifier, 
		[User] nvarchar(1000), 
		UserName nvarchar(1000), 
		RefStateID_Hide uniqueidentifier, 
		RefStateTitle nvarchar(1000), 
		TagID_Hide uniqueidentifier, 
		Tag nvarchar(1000), 
		RefDirectorNodeID_Hide uniqueidentifier, 
		RefDirectorNode nvarchar(1000), 
		RefDirectorUserID_Hide uniqueidentifier, 
		RefDirectorName nvarchar(1000), 
		RefDirectorUserName nvarchar(1000), 
		EntranceDate datetime, 
		RefSenderUserID_Hide uniqueidentifier, 
		RefSenderName nvarchar(1000),
		RefSenderUserName nvarchar(1000),
		StateID_Hide uniqueidentifier, 
		StateTitle nvarchar(1000), 
		DirectorNodeID_Hide uniqueidentifier, 
		DirectorNodeName nvarchar(1000),
		DirectorUserID_Hide uniqueidentifier, 
		DirectorName nvarchar(1000),
		DirectorUserName nvarchar(1000),
		SendDate datetime, 
		SenderUserID_Hide uniqueidentifier, 
		SenderName nvarchar(1000),
		SenderUserName nvarchar(1000)
	)
	
	INSERT INTO @Results
	SELECT	RPT.NodeID AS NodeID_Hide, 
			ND.Name, 
			ND.AdditionalID, 
			Conf.[Level] AS Classification,
			UN.UserID AS UserID_Hide, 
			(UN.FirstName + N' ' + UN.LastName) AS [User], 
			UN.UserName, 
			RPT.RefStateID AS RefStateID_Hide, 
			RS.Title AS RefStateTitle, 
			RPT.TagID AS TagID_Hide, 
			TG.Tag, 
			RPT.RefDirectorNodeID AS RefDirectorNodeID_Hide, 
			RDN.Name AS RefDirectorNode, 
			RPT.RefDirectorUserID AS RefDirectorUserID_Hide, 
			(RDU.FirstName + N' ' + RDU.LastName) AS RefDirectorName, 
			RDU.UserName AS RefDirectorUserName, 
			RPT.EntranceDate, 
			RPT.RefSenderUserID AS RefSenderUserID_Hide, 
			(RSU.FirstName + N' ' + RSU.LastName) AS RefSenderName,
			RSU.UserName AS RefSenderUserName,
			RPT.StateID AS StateID_Hide, 
			ST.Title AS StateTitle, 
			RPT.DirectorNodeID AS DirectorNodeID_Hide, 
			DN.Name AS DirectorNodeName,
			RPT.DirectorUserID AS DirectorUserID_Hide, 
			(DU.FirstName + N' ' + DU.LastName) AS DirectorName,
			DU.UserName AS DirectorUserName,
			RPT.SendDate, 
			RPT.SenderUserID AS SenderUserID_Hide, 
			(SU.FirstName + N' ' + SU.LastName) AS SenderName,
			SU.UserName AS SenderUserName
	FROM (
			SELECT Ref.NodeID AS NodeID, 
				CAST(MAX(CAST(Ref.StateID AS varchar(36))) AS uniqueidentifier) AS RefStateID,
				CAST(MAX(CAST(Ref.TagID AS varchar(36))) AS uniqueidentifier) AS TagID,
				CAST(MAX(CAST(Ref.DirectorNodeID AS varchar(36))) AS uniqueidentifier) AS RefDirectorNodeID,
				CAST(MAX(CAST(Ref.DirectorUserID AS varchar(36))) AS uniqueidentifier) AS RefDirectorUserID,
				MAX(Ref.SendDate) AS EntranceDate, 
				CAST(MAX(CAST(Ref.SenderUserID AS varchar(36))) AS uniqueidentifier) AS RefSenderUserID,
				CAST(MAX(CAST(H.StateID AS varchar(36))) AS uniqueidentifier) AS StateID,
				CAST(MAX(CAST(H.DirectorNodeID AS varchar(36))) AS uniqueidentifier) AS DirectorNodeID,
				CAST(MAX(CAST(H.DirectorUserID AS varchar(36))) AS uniqueidentifier) AS DirectorUserID,
				MAX(H.SendDate) AS SendDate,
				CAST(MAX(CAST(H.SenderUserID AS varchar(36))) AS uniqueidentifier) AS SenderUserID
			FROM (
					SELECT A.OwnerID AS NodeID, A.WorkFlowID, A.StateID AS StateID, B.TagID AS TagID, 
						A.DirectorNodeID, A.DirectorUserID, B.SendDate, A.SenderUserID
					FROM [dbo].[WF_History] AS A
						INNER JOIN (
							SELECT H.OwnerID, MAX(H.SendDate) AS SendDate, 
								CAST(MAX(CAST(WFS.TagID AS varchar(36))) AS uniqueidentifier) AS TagID
							FROM [dbo].[WF_History] AS H
								INNER JOIN [dbo].[WF_WorkFlowStates] AS WFS
								ON WFS.ApplicationID = @ApplicationID AND 
									H.WorkFlowID = WFS.WorkFlowID AND WFS.StateID = H.StateID
							WHERE H.ApplicationID = @ApplicationID AND 
								(@WorkFlowID IS NULL OR H.WorkFlowID = @WorkFlowID) AND
								(@StateID IS NULL OR H.StateID = @StateID) AND
								(@TagID IS NULL OR WFS.TagID = @TagID) AND H.Deleted = 0
							GROUP BY H.OwnerID
						) AS B
						ON B.OwnerID = A.OwnerID AND A.SendDate = B.SendDate
					WHERE A.ApplicationID = @ApplicationID
				) AS Ref
				LEFT JOIN [dbo].[WF_History] AS H
				ON H.ApplicationID = @ApplicationID AND H.OwnerID = Ref.NodeID AND 
					H.WorkFlowID = Ref.WorkFlowID AND H.SendDate > Ref.SendDate
			WHERE (@CurrentState IS NULL OR (@CurrentState = 1 AND H.SendDate IS NULL) OR 
				@CurrentState = 0 AND H.SendDate IS NOT NULL)
			GROUP BY Ref.NodeID
		) AS RPT
		INNER JOIN [dbo].[CN_Nodes] AS ND
		ON ND.ApplicationID = @ApplicationID AND ND.NodeID = RPT.NodeID
		LEFT JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND UN.UserID = ND.CreatorUserID
		LEFT JOIN [dbo].[WF_States] AS RS
		ON RS.ApplicationID = @ApplicationID AND RS.StateID = RPT.RefStateID
		LEFT JOIN [dbo].[CN_Tags] AS TG
		ON TG.ApplicationID = @ApplicationID AND TG.TagID = RPT.TagID
		LEFT JOIN [dbo].[CN_Nodes] AS RDN
		ON RDN.ApplicationID = @ApplicationID AND RDN.NodeID = RPT.RefDirectorNodeID
		LEFT JOIN [dbo].[Users_Normal] AS RDU
		ON RDU.ApplicationID = @ApplicationID AND RDU.UserID = RPT.RefDirectorUserID
		LEFT JOIN [dbo].[Users_Normal] AS RSU
		ON RSU.ApplicationID = @ApplicationID AND RSU.UserID = RPT.RefSenderUserID
		LEFT JOIN [dbo].[WF_States] AS ST
		ON ST.ApplicationID = @ApplicationID AND ST.StateID = RPT.StateID
		LEFT JOIN [dbo].[CN_Nodes] AS DN
		ON DN.ApplicationID = @ApplicationID AND DN.NodeID = RPT.DirectorNodeID
		LEFT JOIN [dbo].[Users_Normal] AS DU
		ON DU.ApplicationID = @ApplicationID AND DU.UserID = RPT.DirectorUserID
		LEFT JOIN [dbo].[Users_Normal] AS SU
		ON SU.ApplicationID = @ApplicationID AND SU.UserID = RPT.SenderUserID
		LEFT JOIN [dbo].[PRVC_View_Confidentialities] AS Conf
		ON Conf.ApplicationID = @ApplicationID AND Conf.ObjectID = ND.NodeID
	WHERE (@NodeTypeID IS NULL OR ND.NodeTypeID = @NodeTypeID) AND
		(@LowerCreationDateLimit IS NULL OR ND.CreationDate >= @LowerCreationDateLimit) AND
		(@UpperCreationDateLimit IS NULL OR ND.CreationDate <= @UpperCreationDateLimit) AND
		ND.Deleted = 0
		
	SELECT *
	FROM @Results
		
	SELECT ('{' +
			'"Name": {"Action": "Link", "Type": "Node",' +
				'"Requires": {"ID": "NodeID_Hide"}' +
			'},' +
			'"User": {"Action": "Link", "Type": "User",' +
				'"Requires": {"UID": "UserID_Hide"}' +
			'},' +
			'"RefDirectorNode": {"Action": "Link", "Type": "Node",' +
				'"Requires": {"ID": "RefDirectorNodeID_Hide"}' +
			'},' +
			'"RefDirectorName": {"Action": "Link", "Type": "User",' +
				'"Requires": {"UID": "RefDirectorUserID_Hide"}' +
			'},' +
			'"RefSenderName": {"Action": "Link", "Type": "User",' +
				'"Requires": {"UID": "RefSenderUserID_Hide"}' +
			'},' +
			'"DirectorNodeName": {"Action": "Link", "Type": "Node",' +
				'"Requires": {"ID": "DirectorNodeID_Hide"}' +
			'},' +
			'"DirectorName": {"Action": "Link", "Type": "User",' +
				'"Requires": {"UID": "DirectorUserID_Hide"}' +
			'},' +
			'"SenderName": {"Action": "Link", "Type": "User",' +
				'"Requires": {"UID": "SenderUserID_Hide"}' +
			'}' +
		   '}') AS Actions
		   
	IF @NodeTypeID IS NOT NULL BEGIN
		DECLARE @FormID uniqueidentifier = (
			SELECT TOP(1) FormID
			FROM [dbo].[FG_FormOwners]
			WHERE ApplicationID = @ApplicationID AND OwnerID = @NodeTypeID AND Deleted = 0
		)
		
		IF @FormID IS NOT NULL AND EXISTS(
			SELECT TOP(1) *
			FROM [dbo].[CN_Extensions] AS Ex
			WHERE Ex.ApplicationID = @ApplicationID AND 
				Ex.OwnerID = @NodeTypeID AND Ex.Extension = N'Form' AND Ex.Deleted = 0
		) BEGIN
			-- Second Part: Describes the Third Part
			SELECT CAST(EFE.ElementID AS varchar(50)) AS ColumnName, EFE.Title AS Translation,
				CASE
					WHEN EFE.[Type] = N'Binary' THEN N'bool'
					WHEN EFE.[Type] = N'Number' THEN N'double'
					WHEN EFE.[Type] = N'Date' THEN N'datetime'
					WHEN EFE.[Type] = N'User' THEN N'user'
					WHEN EFE.[Type] = N'Node' THEN N'node'
					ELSE N'string'
				END AS [Type]
			FROM [dbo].[FG_ExtendedFormElements] AS EFE
			WHERE EFE.ApplicationID = @ApplicationID AND 
				EFE.FormID = @FormID AND EFE.Deleted = 0
			ORDER BY EFE.SequenceNumber ASC
			
			SELECT ('{"IsDescription": "true"}') AS Info
			-- end of Second Part
			
			-- Third Part: The Form Info
			DECLARE @NodeIDs GuidTableType
			
			INSERT INTO @NodeIDs (Value)
			SELECT R.NodeID_Hide
			FROM @Results AS R
			
			DECLARE @ElementIDs GuidTableType
			
			DECLARE @FakeInstances GuidTableType
			DECLARE @FakeFilters FormFilterTableType
			
			EXEC [dbo].[FG_P_GetFormRecords] @ApplicationID, @FormID, 
				@ElementIDs, @FakeInstances, @NodeIDs, 
				@FakeFilters, NULL, 1000000, NULL, NULL
			
			SELECT ('{' +
				'"ColumnsMap": "NodeID_Hide:OwnerID",' +
				'"ColumnsToTransfer": "' + STUFF((
					SELECT ',' + CAST(EFE.ElementID AS varchar(50))
					FROM [dbo].[FG_ExtendedFormElements] AS EFE
					WHERE EFE.ApplicationID = @ApplicationID AND 
						EFE.FormID = @FormID AND EFE.Deleted = 0
					ORDER BY EFE.SequenceNumber ASC
					FOR xml path('a'), type
				).value('.','nvarchar(max)'), 1, 1, '') + '"' +
			   '}') AS Info
			-- End of Third Part
		END
	END
END

GO
