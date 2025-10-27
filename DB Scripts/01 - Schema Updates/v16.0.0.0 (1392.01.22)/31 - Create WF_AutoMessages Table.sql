USE [EKM_App]
GO

/****** Object:  Table [dbo].[WF_StateConnectionAudience]    Script Date: 06/10/2013 09:30:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO


CREATE TABLE [dbo].[WF_AutoMessages](
	[AutoMessageID] [uniqueidentifier] NOT NULL,
	[OwnerID] [uniqueidentifier] NOT NULL,
	[BodyText] [nvarchar](4000) NOT NULL,
	[AudienceType] [varchar](20) NULL,
	[RefStateID] [uniqueidentifier] NULL,
	[NodeID] [uniqueidentifier] NULL,
	[Admin] [bit] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_WF_AutoMessages] PRIMARY KEY CLUSTERED 
(
	[AutoMessageID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


ALTER TABLE [dbo].[WF_AutoMessages]  WITH CHECK ADD  CONSTRAINT [FK_WF_AutoMessages_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[WF_AutoMessages] CHECK CONSTRAINT [FK_WF_AutoMessages_CN_Nodes]
GO

ALTER TABLE [dbo].[WF_AutoMessages]  WITH CHECK ADD  CONSTRAINT [FK_WF_AutoMessages_WF_States_Ref] FOREIGN KEY([RefStateID])
REFERENCES [dbo].[WF_States] ([StateID])
GO

ALTER TABLE [dbo].[WF_AutoMessages] CHECK CONSTRAINT [FK_WF_AutoMessages_WF_States_Ref]
GO

ALTER TABLE [dbo].[WF_AutoMessages]  WITH CHECK ADD  CONSTRAINT [FK_WF_AutoMessages_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WF_AutoMessages] CHECK CONSTRAINT [FK_WF_AutoMessages_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[WF_AutoMessages]  WITH CHECK ADD  CONSTRAINT [FK_WF_AutoMessages_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[WF_AutoMessages] CHECK CONSTRAINT [FK_WF_AutoMessages_aspnet_Users_Modifier]
GO


INSERT INTO [dbo].[WF_AutoMessages](
	AutoMessageID,
	OwnerID,
	BodyText,
	AudienceType,
	RefStateID,
	NodeID,
	[Admin],
	CreatorUserID,
	CreationDate,
	LastModifierUserID,
	LastModificationDate,
	Deleted
)
SELECT CA.AudienceID, 
	   SC.PattAttachmentID, 
	   CA.BodyText,
	   CA.AudienceType,
	   CA.RefStateID,
	   CA.NodeID,
	   CA.[Admin],
	   CA.CreatorUserID,
	   CA.CreationDate,
	   CA.LastModifierUserID,
	   CA.LastModificationDate,
	   CA.Deleted
FROM [dbo].[WF_StateConnectionAudience] AS CA
	INNER JOIN [dbo].[WF_StateConnections] AS SC
	ON CA.WorkFlowID = SC.WorkFlowID AND CA.InStateID = SC.InStateID AND
		CA.OutStateID = SC.OutStateID
GO



DROP TABLE [dbo].[WF_StateConnectionAudience]
GO