USE [EKM_App]
GO

/****** Object:  Table [dbo].[WF_Services]    Script Date: 06/10/2013 11:16:07 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TABLE [dbo].[WF_TMPServices](
	[ServiceID] [uniqueidentifier] NOT NULL,
	[WorkFlowID] [uniqueidentifier] NOT NULL,
	[NodeTypeID] [uniqueidentifier] NOT NULL,
	[FormID] [uniqueidentifier] NULL,
	[Title] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[InitialMessage] [nvarchar](max) NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_WF_TMPServices] PRIMARY KEY CLUSTERED 
(
	[ServiceID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[WF_TMPServices](
	ServiceID,
	WorkFlowID,
	NodeTypeID,
	FormID,
	Title,
	[Description],
	InitialMessage,
	CreatorUserID,
	CreationDate,
	LastModifierUserID,
	LastModificationDate,
	Deleted
)
SELECT NEWID(),
	   WorkFlowID,
	   NodeTypeID,
	   FormID,
	   Title,
	   [Description],
	   InitialMessage,
	   CreatorUserID,
	   CreationDate,
	   LastModifierUserID,
	   LastModificationDate,
	   Deleted
FROM [dbo].[WF_Services]
GO


INSERT INTO [dbo].[WF_AutoMessages](
	AutoMessageID,
	OwnerID,
	BodyText,
	AudienceType,
	[Admin],
	CreatorUserID,
	CreationDate,
	LastModifierUserID,
	LastModificationDate,
	Deleted
)
SELECT NEWID(),
	   ServiceID,
	   InitialMessage,
	   N'SendToOwner',
	   0,
	   CreatorUserID,
	   CreationDate,
	   LastModifierUserID,
	   LastModificationDate,
	   0
FROM [dbo].[WF_TMPServices]
WHERE InitialMessage IS NOT NULL AND InitialMessage <> N''
GO


DROP TABLE [dbo].[WF_Services]
GO


CREATE TABLE [dbo].[WF_Services](
	[ServiceID] [uniqueidentifier] NOT NULL,
	[WorkFlowID] [uniqueidentifier] NOT NULL,
	[NodeTypeID] [uniqueidentifier] NOT NULL,
	[FormID] [uniqueidentifier] NULL,
	[Title] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_WF_Services] PRIMARY KEY CLUSTERED 
(
	[ServiceID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[WF_Services]  WITH CHECK ADD  CONSTRAINT [FK_WF_Services_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WF_Services] CHECK CONSTRAINT [FK_WF_Services_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[WF_Services]  WITH CHECK ADD  CONSTRAINT [FK_WF_Services_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WF_Services] CHECK CONSTRAINT [FK_WF_Services_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[WF_Services]  WITH CHECK ADD  CONSTRAINT [FK_WF_Services_CN_NodeTypes_NodeTypeID] FOREIGN KEY([NodeTypeID])
REFERENCES [dbo].[CN_NodeTypes] ([NodeTypeID])
GO

ALTER TABLE [dbo].[WF_Services] CHECK CONSTRAINT [FK_WF_Services_CN_NodeTypes_NodeTypeID]
GO

ALTER TABLE [dbo].[WF_Services]  WITH CHECK ADD  CONSTRAINT [FK_WF_Services_FG_ExtendedForms_FormID] FOREIGN KEY([FormID])
REFERENCES [dbo].[FG_ExtendedForms] ([FormID])
GO

ALTER TABLE [dbo].[WF_Services] CHECK CONSTRAINT [FK_WF_Services_FG_ExtendedForms_FormID]
GO

ALTER TABLE [dbo].[WF_Services]  WITH CHECK ADD  CONSTRAINT [FK_WF_Services_WF_WorkFlows_WorkFlowID] FOREIGN KEY([WorkFlowID])
REFERENCES [dbo].[WF_WorkFlows] ([WorkFlowID])
GO

ALTER TABLE [dbo].[WF_Services] CHECK CONSTRAINT [FK_WF_Services_WF_WorkFlows_WorkFlowID]
GO


INSERT INTO [dbo].[WF_Services](
	ServiceID,
	WorkFlowID,
	NodeTypeID,
	FormID,
	Title,
	[Description],
	CreatorUserID,
	CreationDate,
	LastModifierUserID,
	LastModificationDate,
	Deleted
)
SELECT ServiceID,
	   WorkFlowID,
	   NodeTypeID,
	   FormID,
	   Title,
	   [Description],
	   CreatorUserID,
	   CreationDate,
	   LastModifierUserID,
	   LastModificationDate,
	   Deleted
FROM [dbo].[WF_TMPServices]
GO


DROP TABLE [dbo].[WF_TMPServices]
GO