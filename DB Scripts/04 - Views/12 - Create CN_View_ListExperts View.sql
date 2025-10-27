USE [EKM_App]
GO

SET NUMERIC_ROUNDABORT OFF;
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT,
    QUOTED_IDENTIFIER, ANSI_NULLS ON;


IF EXISTS(select * FROM sys.views where name = 'CN_View_ListExperts')
DROP VIEW [dbo].[CN_View_ListExperts]
GO


CREATE VIEW [dbo].[CN_View_ListExperts] WITH SCHEMABINDING, ENCRYPTION
AS
SELECT  L.ApplicationID,
		L.ListID AS ListID,
		EX.UserID AS UserID,
		ND.NodeID AS NodeID,
		ND.NodeTypeID AS NodeTypeID,
		L.Name AS ListName,
		ND.Name AS NodeName
FROM    [dbo].[CN_Lists] AS L
		INNER JOIN [dbo].[CN_ListNodes] AS LN
		ON LN.ApplicationID = L.ApplicationID AND LN.NodeID = L.ListID
		INNER JOIN [dbo].[CN_Nodes] AS ND
		ON ND.ApplicationID = LN.ApplicationID AND ND.NodeID = LN.NodeID
		INNER JOIN [dbo].[CN_Experts] AS EX
		ON ND.ApplicationID = EX.ApplicationID AND ND.NodeID = EX.NodeID
		INNER JOIN [dbo].[aspnet_Membership] AS M
		ON M.UserId = EX.UserID
WHERE	LN.Deleted = 0 AND ND.Deleted = 0 AND 
		(EX.Approved = 1 OR EX.SocialApproved = 1) AND M.IsApproved = 1

GO


CREATE UNIQUE CLUSTERED INDEX PK_CN_View_ListExperts_ListID ON [dbo].[CN_View_ListExperts]
(
	[ApplicationID] ASC,
	[ListID] ASC,
	[UserID] ASC,
	[NodeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='IX_CN_View_ListExperts_UserID' AND 
	object_id = OBJECT_ID('CN_View_ListExperts'))
CREATE NONCLUSTERED INDEX [IX_CN_View_ListExperts_UserID] ON [dbo].[CN_View_ListExperts] 
(
	[ApplicationID] ASC,
	[UserID] ASC,
	[ListID] ASC,
	[NodeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='IX_CN_View_ListExperts_NodeID' AND 
	object_id = OBJECT_ID('CN_View_ListExperts'))
CREATE NONCLUSTERED INDEX [IX_CN_View_ListExperts_NodeID] ON [dbo].[CN_View_ListExperts] 
(
	[ApplicationID] ASC,
	[NodeID] ASC,
	[ListID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO