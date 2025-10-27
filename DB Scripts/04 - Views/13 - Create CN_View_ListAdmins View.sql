USE [EKM_App]
GO

SET NUMERIC_ROUNDABORT OFF;
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT,
    QUOTED_IDENTIFIER, ANSI_NULLS ON;


IF EXISTS(select * FROM sys.views where name = 'CN_View_ListAdmins')
DROP VIEW [dbo].[CN_View_ListAdmins]
GO


CREATE VIEW [dbo].[CN_View_ListAdmins] WITH SCHEMABINDING, ENCRYPTION
AS
SELECT  L.ApplicationID,
		L.ListID AS ListID,
		LA.UserID AS UserID,
		ND.NodeID AS NodeID,
		L.Name AS ListName
FROM    [dbo].[CN_Lists] AS L
		INNER JOIN [dbo].[CN_ListNodes] AS LN
		ON LN.ApplicationID = L.ApplicationID AND LN.ListID = L.ListID
		INNER JOIN [dbo].[CN_Nodes] AS ND
		ON ND.ApplicationID = LN.ApplicationID AND ND.NodeID = LN.NodeID
		INNER JOIN [dbo].[CN_ListAdmins] AS LA
		ON LA.ApplicationID = L.ApplicationID AND LA.ListID = L.ListID
		INNER JOIN [dbo].[aspnet_Membership] AS M
		ON M.UserId = LA.UserID
WHERE	LN.Deleted = 0 AND ND.Deleted = 0 AND LA.Deleted = 0 AND M.IsApproved = 1

GO


CREATE UNIQUE CLUSTERED INDEX PK_CN_View_ListAdmins_ListID ON [dbo].[CN_View_ListAdmins]
(
	[ApplicationID] ASC,
	[ListID] ASC,
	[UserID] ASC,
	[NodeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='IX_CN_View_ListAdmins_UserID' AND 
	object_id = OBJECT_ID('CN_View_ListAdmins'))
CREATE NONCLUSTERED INDEX [IX_CN_View_ListAdmins_UserID] ON [dbo].[CN_View_ListAdmins] 
(
	[ApplicationID] ASC,
	[UserID] ASC,
	[ListID] ASC,
	[NodeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='IX_CN_View_ListAdmins_NodeID' AND 
	object_id = OBJECT_ID('CN_View_ListAdmins'))
CREATE NONCLUSTERED INDEX [IX_CN_View_ListAdmins_NodeID] ON [dbo].[CN_View_ListAdmins] 
(
	[ApplicationID] ASC,
	[NodeID] ASC,
	[ListID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO