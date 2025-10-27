USE [EKM_App]
GO


IF NOT EXISTS(SELECT * 
FROM sys.indexes WHERE name='IX_SH_PostShares_SendDate' AND object_id = OBJECT_ID('SH_PostShares'))
CREATE NONCLUSTERED INDEX [IX_SH_PostShares_SendDate] ON [dbo].[SH_PostShares] 
(
	[SendDate] DESC,
	[OwnerID] ASC,
	[SenderUserID] ASC,
	[Privacy] ASC,
	[ShareID] ASC,
	[OwnerType] ASC,
	[Deleted] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


IF NOT EXISTS(SELECT * 
FROM sys.indexes WHERE name='IX_SH_PostShares_OwnerID' AND object_id = OBJECT_ID('SH_PostShares'))
CREATE NONCLUSTERED INDEX [IX_SH_PostShares_OwnerID] ON [dbo].[SH_PostShares] 
(
	[OwnerID] ASC,
	[SendDate] DESC,
	[ShareID] ASC,
	[OwnerType] ASC,
	[Privacy] ASC,
	[Deleted] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


IF NOT EXISTS(SELECT * 
FROM sys.indexes WHERE name='IX_SH_PostShares_SenderUserID' AND object_id = OBJECT_ID('SH_PostShares'))
CREATE NONCLUSTERED INDEX [IX_SH_PostShares_SenderUserID] ON [dbo].[SH_PostShares] 
(
	[SenderUserID] ASC,
	[SendDate] DESC,
	[ShareID] ASC,
	[OwnerType] ASC,
	[Privacy] ASC,
	[Deleted] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


