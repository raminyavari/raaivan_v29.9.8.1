USE [EKM_App]
GO

SET NUMERIC_ROUNDABORT OFF;
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT,
    QUOTED_IDENTIFIER, ANSI_NULLS ON;


IF EXISTS(select * FROM sys.views where name = 'CN_View_NodeMembers')
DROP VIEW [dbo].[CN_View_NodeMembers]
GO


CREATE VIEW [dbo].[CN_View_NodeMembers] WITH SCHEMABINDING, ENCRYPTION
AS
SELECT  NM.ApplicationID,
		NM.NodeID AS NodeID,
		NM.UserID AS UserID,
		ND.NodeTypeID AS NodeTypeID,
		ND.Name AS NodeName,
		NM.IsAdmin AS IsAdmin,
		CAST((CASE WHEN NM.[Status] = 'Pending' THEN 1 ELSE 0 END) AS bit) AS IsPending
FROM    [dbo].[CN_NodeMembers] AS NM
		INNER JOIN [dbo].[CN_Nodes] AS ND
		ON ND.ApplicationID = NM.ApplicationID AND ND.NodeID = NM.NodeID
		INNER JOIN [dbo].[aspnet_Membership] AS M
		ON M.UserId = NM.UserID
WHERE	NM.Deleted = 0 AND ND.Deleted = 0 AND M.IsApproved = 1

GO

CREATE UNIQUE CLUSTERED INDEX PK_CN_View_NodeMembers_NodeID ON [dbo].[CN_View_NodeMembers]
(
	[ApplicationID] ASC,
	[NodeID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='IX_CN_View_NodeMembers_UserID' AND 
	object_id = OBJECT_ID('CN_View_NodeMembers'))
CREATE NONCLUSTERED INDEX [IX_CN_View_NodeMembers_UserID] ON [dbo].[CN_View_NodeMembers] 
(
	[ApplicationID] ASC,
	[UserID] ASC,
	[NodeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO