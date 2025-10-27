USE [EKM_App]
GO

/****** Object:  Table [dbo].[QA_Admins]    Script Date: 11/29/2016 08:54:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[QA_Admins](
	[UserID] [uniqueidentifier] NOT NULL,
	[WorkFlowID] [uniqueidentifier] NULL,
	[CreatorUserID] [uniqueidentifier] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
	[ApplicationID] [uniqueidentifier] NULL
 CONSTRAINT [PK_QA_Admins] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[QA_Admins]  WITH CHECK ADD  CONSTRAINT [FK_QA_Admins_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[QA_Admins] CHECK CONSTRAINT [FK_QA_Admins_aspnet_Applications]
GO

ALTER TABLE [dbo].[QA_Admins]  WITH CHECK ADD  CONSTRAINT [FK_QA_Admins_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[QA_Admins] CHECK CONSTRAINT [FK_QA_Admins_aspnet_Users]
GO

ALTER TABLE [dbo].[QA_Admins]  WITH CHECK ADD  CONSTRAINT [FK_QA_Admins_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[QA_Admins] CHECK CONSTRAINT [FK_QA_Admins_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[QA_Admins]  WITH CHECK ADD  CONSTRAINT [FK_QA_Admins_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[QA_Admins] CHECK CONSTRAINT [FK_QA_Admins_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[QA_Admins]  WITH CHECK ADD  CONSTRAINT [FK_QA_Admins_QA_WorkFlows] FOREIGN KEY([WorkFlowID])
REFERENCES [dbo].[QA_WorkFlows] ([WorkFlowID])
GO

ALTER TABLE [dbo].[QA_Admins] CHECK CONSTRAINT [FK_QA_Admins_QA_WorkFlows]
GO
