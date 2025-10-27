USE [EKM_App]
GO


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



/****** Object:  View [dbo].[CN_View_Nodes_Normal]    Script Date: 06/22/2012 13:03:22 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(select * FROM sys.views where name = 'WF_View_CurrentStates')
DROP VIEW [dbo].[WF_View_CurrentStates]
GO

IF EXISTS(select * FROM sys.views where name = 'CN_View_Nodes_Normal')
DROP VIEW [dbo].[CN_View_Nodes_Normal]
GO


CREATE VIEW [dbo].[CN_View_Nodes_Normal] WITH SCHEMABINDING, ENCRYPTION
AS
SELECT  ND.NodeID, 
		ND.Name AS NodeName, 
		ND.[Description],
		ND.AdditionalID AS NodeAdditionalID, 
		ND.NodeTypeID,
		NT.Name AS TypeName, 
		NT.AdditionalID AS TypeAdditionalID, 
        ND.ParentNodeID, 
        ND.Deleted, 
        NT.Deleted AS TypeDeleted,
        ND.Tags AS Tags, 
        ND.CreationDate AS CreationDate,
        ND.CreatorUserID AS CreatorUserID,
        ND.OwnerID AS OwnerID,
        ND.IndexLastUpdateDate
FROM [dbo].[CN_Nodes] AS ND
	INNER JOIN [dbo].[CN_NodeTypes] AS NT
	ON ND.NodeTypeID = NT.NodeTypeID

GO

CREATE UNIQUE CLUSTERED INDEX PX_CN_View_Nodes_Normal ON [dbo].[CN_View_Nodes_Normal]
(
	[NodeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO




IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[TMP_SetWikiDashboards]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[TMP_SetWikiDashboards]
GO

CREATE PROCEDURE [dbo].[TMP_SetWikiDashboards]
	@UserID uniqueidentifier
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @OwnerIDs GuidTableType
	
	INSERT INTO @OwnerIDs
	SELECT EX.[NodeID] AS ID
	FROM [dbo].[CN_Experts] AS EX
		INNER JOIN [dbo].[CN_View_Nodes_Normal] AS ND
		ON EX.[NodeID] = ND.[NodeID] 
	WHERE EX.UserID = @UserID AND (EX.Approved = 1 OR EX.SocialApproved = 1) AND ND.[Deleted] = 0
	
	DECLARE @Now datetime = GETDATE()
	
	INSERT INTO [dbo].[NTFN_Dashboards](
		UserID,
		NodeID,
		RefItemID,
		[Type],
		Removable,
		SendDate,
		Seen,
		Done,
		Deleted
	)
	SELECT	@UserID,
			Ref.Value,
			Ref.Value,
			N'Wiki',
			1,
			MAX(CH.SendDate),
			0,
			0,
			0
	FROM @OwnerIDs AS Ref
		INNER JOIN [dbo].[WK_Titles] AS TT
		ON Ref.Value = TT.OwnerID
		INNER JOIN [dbo].[WK_Paragraphs] AS PG
		ON TT.TitleID = PG.TitleID
		INNER JOIN [dbo].[WK_Changes] AS CH
		ON PG.ParagraphID = CH.ParagraphID
	WHERE TT.Deleted = 0 AND PG.Deleted = 0 AND CH.[Status] = N'Pending' AND CH.Deleted = 0
	GROUP BY Ref.Value
END

GO


DECLARE @UserIDs TABLE (ID int identity(1,1) primary key clustered, UserID uniqueidentifier)
INSERT INTO @UserIDs (UserID)
SELECT UserID 
FROM [dbo].[Users_Normal]

DECLARE @Count int  = (SELECT COUNT(*) FROM @UserIDs)

WHILE @Count > 0 BEGIN
	DECLARE @CurID uniqueidentifier = (SELECT UserID FROM @UserIDs WHERE ID = @Count)
	
	EXEC [dbo].[TMP_SetWikiDashboards] @CurID
	
	SET @Count = @Count - 1
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[TMP_SetWikiDashboards]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[TMP_SetWikiDashboards]
GO

IF EXISTS(select * FROM sys.views where name = 'Users_Normal')
DROP VIEW [dbo].[Users_Normal]
GO