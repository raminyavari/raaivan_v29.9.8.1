USE [EKM_App]
GO

/****** Object:  Table [dbo].[KWF_Paraphs]    Script Date: 02/05/2014 14:54:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TABLE [dbo].[TMP_KW_History](
	[ID] [bigint] IDENTITY(1,1),
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[Action] [varchar](50) NOT NULL,
	[Description] [nvarchar](2000) NULL,
	[ActorUserID] [uniqueidentifier] NOT NULL,
	[ActionDate] [datetime] NOT NULL
 CONSTRAINT [PK_TMP_KW_History] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TMP_KW_History]  WITH CHECK ADD  CONSTRAINT [FK_TMP_KW_History_CN_Nodes] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[TMP_KW_History] CHECK CONSTRAINT [FK_TMP_KW_History_CN_Nodes]
GO

ALTER TABLE [dbo].[TMP_KW_History]  WITH CHECK ADD  CONSTRAINT [FK_TMP_KW_History_aspnet_Users] FOREIGN KEY([ActorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[TMP_KW_History] CHECK CONSTRAINT [FK_TMP_KW_History_aspnet_Users]
GO
