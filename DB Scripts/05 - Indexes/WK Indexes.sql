USE [EKM_App]
GO


IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[WK_Paragraphs]') AND name = N'IX_WK_Paragraphs_TitleID')
DROP INDEX [IX_WK_Paragraphs_TitleID] ON [dbo].[WK_Paragraphs] WITH ( ONLINE = OFF )
GO

CREATE NONCLUSTERED INDEX [IX_WK_Paragraphs_TitleID] ON [dbo].[WK_Paragraphs] 
(
	[TitleID] ASC,
	[ApplicationID] ASC,
	[Status] ASC,
	[Deleted] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO