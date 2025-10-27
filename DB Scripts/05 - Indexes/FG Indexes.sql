USE [EKM_App]
GO


IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[FG_FormInstances]') AND name = N'IX_FG_FormInstances_OwnerID')
DROP INDEX [IX_FG_FormInstances_OwnerID] ON [dbo].[FG_FormInstances] WITH ( ONLINE = OFF )
GO

CREATE NONCLUSTERED INDEX [IX_FG_FormInstances_OwnerID] ON [dbo].[FG_FormInstances] 
(
	[OwnerID] ASC,
	[ApplicationID] ASC,
	[FormID] ASC,
	[InstanceID] ASC,
	[OwnerType] ASC,
	[Deleted] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[FG_InstanceElements]') AND name = N'IX_FG_InstanceElements_InstanceID')
DROP INDEX [IX_FG_InstanceElements_InstanceID] ON [dbo].[FG_InstanceElements] WITH ( ONLINE = OFF )
GO

CREATE NONCLUSTERED INDEX [IX_FG_InstanceElements_InstanceID] ON [dbo].[FG_InstanceElements] 
(
	[InstanceID] ASC,
	[ApplicationID] ASC,
	[ElementID] ASC,
	[RefElementID] ASC,
	[Type] ASC,
	[Deleted] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO



