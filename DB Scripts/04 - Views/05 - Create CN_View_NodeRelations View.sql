USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF EXISTS(select * FROM sys.views where name = 'CN_View_NodeRelations')
DROP VIEW [dbo].[CN_View_NodeRelations]
GO

CREATE VIEW [dbo].[CN_View_NodeRelations] WITH SCHEMABINDING, ENCRYPTION
AS
SELECT NR.ApplicationID,
	   NR.SourceNodeID AS SourceNodeID, 
	   NR.DestinationNodeID AS DestinationNodeID, 
	   NR.PropertyID AS RelationTypeID, 
	   NR.NominalValue AS NominalValue, 
       NR.NumericalValue AS NumericalValue, 
       NR.Deleted AS RelationDeleted, 
       RT.AdditionalID AS RelationTypeAdditionalID, 
       RT.Name AS RelationType, 
       RT.Deleted AS RelationTypeDeleted
FROM   dbo.CN_NodeRelations  AS NR 
	INNER JOIN dbo.CN_Properties AS RT
	ON RT.ApplicationID = NR.ApplicationID AND RT.PropertyID = NR.PropertyID
	
GO

CREATE UNIQUE CLUSTERED INDEX PX_CN_View_NodeRelations ON [dbo].[CN_View_NodeRelations]
(
	[ApplicationID] ASC,
	[SourceNodeID] ASC,
	[DestinationNodeID] ASC,
	[RelationTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_CN_View_NodeRelations_DestinationNodeID] ON [dbo].[CN_View_NodeRelations] 
(
	[ApplicationID] ASC,
	[DestinationNodeID] ASC,
	[SourceNodeID] ASC,
	[RelationTypeID] ASC,
	[RelationDeleted] ASC,
	[RelationTypeAdditionalID] ASC,
	[RelationTypeDeleted] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
