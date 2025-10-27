USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[RV_VariablesWithOwner](
	[ID] [bigint] IDENTITY(1, 1) NOT NULL,
	[OwnerID] [uniqueidentifier] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
	[CreatorUserID] [uniqueidentifier] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
	[ApplicationID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_RV_VariablesWithOwner] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[RV_VariablesWithOwner]  WITH CHECK ADD  CONSTRAINT [FK_RV_VariablesWithOwner_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[RV_VariablesWithOwner] CHECK CONSTRAINT [FK_RV_VariablesWithOwner_aspnet_Applications]
GO

ALTER TABLE [dbo].[RV_VariablesWithOwner]  WITH CHECK ADD  CONSTRAINT [FK_RV_VariablesWithOwner_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[RV_VariablesWithOwner] CHECK CONSTRAINT [FK_RV_VariablesWithOwner_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[RV_VariablesWithOwner]  WITH CHECK ADD  CONSTRAINT [FK_RV_VariablesWithOwner_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[RV_VariablesWithOwner] CHECK CONSTRAINT [FK_RV_VariablesWithOwner_aspnet_Users_Modifier]
GO


