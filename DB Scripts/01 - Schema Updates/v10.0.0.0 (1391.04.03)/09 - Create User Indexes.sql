USE [EKM_App]
GO


IF NOT EXISTS(SELECT * 
FROM sys.indexes WHERE name='IX_USR_UserConnections_ReceiverUserID' AND object_id = OBJECT_ID('UserConnections'))
CREATE NONCLUSTERED INDEX [IX_USR_UserConnections_ReceiverUserID] ON [dbo].[UserConnections] 
(
	[ReceiverUserID] ASC,
	[SenderUserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


