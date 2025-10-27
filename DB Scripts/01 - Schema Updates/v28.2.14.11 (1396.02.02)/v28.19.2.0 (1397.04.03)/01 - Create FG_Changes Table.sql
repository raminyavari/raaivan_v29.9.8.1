USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FG_Changes](
	[ID] [bigint] IDENTITY(1, 1) NOT NULL,
	[ElementID] [uniqueidentifier] NOT NULL,
	[TextValue] [nvarchar](max) NULL,
	[FloatValue] [float] NULL,
	[BitValue] [bit] NULL,
	[DateValue] [datetime] NULL,
	[GuidValue] [uniqueidentifier] NULL,
	[CreatorUserID] [uniqueidentifier] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[ApplicationID] [uniqueidentifier] NOT NULL
 CONSTRAINT [PK_FG_Changes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


CREATE UNIQUE INDEX UK_FG_Changes_ElementID ON [dbo].[FG_Changes]
(
	[ApplicationID] ASC,
	[ElementID] ASC,
	[Deleted] ASC,
	[CreationDate] ASC,
	[CreatorUserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


ALTER TABLE [dbo].[FG_Changes]  WITH CHECK ADD  CONSTRAINT [FK_FG_Changes_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[FG_Changes] CHECK CONSTRAINT [FK_FG_Changes_aspnet_Applications]
GO

ALTER TABLE [dbo].[FG_Changes]  WITH CHECK ADD  CONSTRAINT [FK_FG_Changes_aspnet_Users] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[FG_Changes] CHECK CONSTRAINT [FK_FG_Changes_aspnet_Users]
GO