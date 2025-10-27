/****** Object:  StoredProcedure [dbo].[AddFolder]    Script Date: 03/14/2012 11:38:59 ******/
USE [EKM_App]
GO

IF NOT EXISTS(SELECT * 
FROM sys.indexes WHERE name='IX_WF_History_ID' AND object_id = OBJECT_ID('WF_History'))
CREATE NONCLUSTERED INDEX [IX_WF_History_ID] ON [dbo].[WF_History] 
(
	[ID] ASC,
	[OwnerID] ASC,
	[Terminated] ASC,
	[Deleted] ASC,
	[Rejected] ASC,
	[HistoryID] ASC,
	[WorkFlowID] ASC,
	[StateID] ASC,
	[ActorUserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO