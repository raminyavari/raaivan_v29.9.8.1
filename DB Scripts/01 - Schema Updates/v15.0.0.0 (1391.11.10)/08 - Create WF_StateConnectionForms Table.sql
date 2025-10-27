USE [EKM_App]
GO



CREATE TABLE [dbo].[WF_StateConnectionForms](
	[WorkFlowID] [uniqueidentifier] NOT NULL,
	[InStateID] [uniqueidentifier] NOT NULL,
	[OutStateID] [uniqueidentifier] NOT NULL,
	[FormID] [uniqueidentifier] NOT NULL,
	[Description] [nvarchar](4000) NULL,
	[Necessary] [bit] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
CONSTRAINT [PK_WF_StateConnectionForms] PRIMARY KEY CLUSTERED 
(
	[WorkFlowID] ASC,
	[InStateID] ASC,
	[OutStateID] ASC,
	[FormID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[WF_StateConnectionForms]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateConnectionForms_WF_WorkFlows] FOREIGN KEY([WorkFlowID])
REFERENCES [dbo].[WF_WorkFlows] ([WorkFlowID])
GO

ALTER TABLE [dbo].[WF_StateConnectionForms] CHECK CONSTRAINT [FK_WF_StateConnectionForms_WF_WorkFlows]
GO

ALTER TABLE [dbo].[WF_StateConnectionForms]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateConnectionForms_WF_States_In] FOREIGN KEY([InStateID])
REFERENCES [dbo].[WF_States] ([StateID])
GO

ALTER TABLE [dbo].[WF_StateConnectionForms] CHECK CONSTRAINT [FK_WF_StateConnectionForms_WF_States_In]
GO

ALTER TABLE [dbo].[WF_StateConnectionForms]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateConnectionForms_WF_States_Out] FOREIGN KEY([OutStateID])
REFERENCES [dbo].[WF_States] ([StateID])
GO

ALTER TABLE [dbo].[WF_StateConnectionForms] CHECK CONSTRAINT [FK_WF_StateConnectionForms_WF_States_Out]
GO

ALTER TABLE [dbo].[WF_StateConnectionForms]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateConnectionForms_FG_ExtendedForms] FOREIGN KEY([FormID])
REFERENCES [dbo].[FG_ExtendedForms] ([FormID])
GO

ALTER TABLE [dbo].[WF_StateConnectionForms] CHECK CONSTRAINT [FK_WF_StateConnectionForms_FG_ExtendedForms]
GO

ALTER TABLE [dbo].[WF_StateConnectionForms]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateConnectionForms_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WF_StateConnectionForms] CHECK CONSTRAINT [FK_WF_StateConnectionForms_aspnet_Users_Creator]
GO


ALTER TABLE [dbo].[WF_StateConnectionForms]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateConnectionForms_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WF_StateConnectionForms] CHECK CONSTRAINT [FK_WF_StateConnectionForms_aspnet_Users_Modifier]
GO

