USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[RV_WorkSpaceApplications](
	[WorkSpaceID] uniqueidentifier NOT NULL,
	[ApplicationID] uniqueidentifier NOT NULL
 CONSTRAINT [PK_RV_WorkSpaceApplications] PRIMARY KEY CLUSTERED 
(
	[WorkSpaceID] ASC,
	[ApplicationID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[RV_WorkSpaceApplications]  WITH CHECK ADD  CONSTRAINT [FK_RV_WorkSpaceApplications_RV_WorkSpaces] FOREIGN KEY([WorkSpaceID])
REFERENCES [dbo].[RV_WorkSpaces] ([WorkSpaceID])
GO

ALTER TABLE [dbo].[RV_WorkSpaceApplications] CHECK CONSTRAINT [FK_RV_WorkSpaceApplications_RV_WorkSpaces]
GO

ALTER TABLE [dbo].[RV_WorkSpaceApplications]  WITH CHECK ADD  CONSTRAINT [FK_RV_WorkSpaceApplications_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[RV_WorkSpaceApplications] CHECK CONSTRAINT [FK_RV_WorkSpaceApplications_aspnet_Applications]
GO




