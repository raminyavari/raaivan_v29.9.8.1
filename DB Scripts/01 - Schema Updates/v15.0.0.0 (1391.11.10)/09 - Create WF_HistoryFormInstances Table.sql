USE [EKM_App]
GO



CREATE TABLE [dbo].[WF_HistoryFormInstances](
	[HistoryID] [uniqueidentifier] NOT NULL,
	[OutStateID] [uniqueidentifier] NOT NULL,
	[FormsID] [uniqueidentifier] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
CONSTRAINT [PK_WF_HistoryFormInstances] PRIMARY KEY CLUSTERED 
(
	[HistoryID] ASC,
	[OutStateID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[WF_HistoryFormInstances]  WITH CHECK ADD  CONSTRAINT [FK_WF_HistoryFormInstances_WF_History] FOREIGN KEY([HistoryID])
REFERENCES [dbo].[WF_History] ([HistoryID])
GO

ALTER TABLE [dbo].[WF_HistoryFormInstances] CHECK CONSTRAINT [FK_WF_HistoryFormInstances_WF_History]
GO

ALTER TABLE [dbo].[WF_HistoryFormInstances]  WITH CHECK ADD  CONSTRAINT [FK_WF_HistoryFormInstances_WF_States_Out] FOREIGN KEY([OutStateID])
REFERENCES [dbo].[WF_States] ([StateID])
GO

ALTER TABLE [dbo].[WF_HistoryFormInstances] CHECK CONSTRAINT [FK_WF_HistoryFormInstances_WF_States_Out]
GO

ALTER TABLE [dbo].[WF_HistoryFormInstances]  WITH CHECK ADD  CONSTRAINT [FK_WF_HistoryFormInstances_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WF_HistoryFormInstances] CHECK CONSTRAINT [FK_WF_HistoryFormInstances_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[WF_HistoryFormInstances]  WITH CHECK ADD  CONSTRAINT [FK_WF_HistoryFormInstances_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WF_HistoryFormInstances] CHECK CONSTRAINT [FK_WF_HistoryFormInstances_aspnet_Users_Modifier]
GO

