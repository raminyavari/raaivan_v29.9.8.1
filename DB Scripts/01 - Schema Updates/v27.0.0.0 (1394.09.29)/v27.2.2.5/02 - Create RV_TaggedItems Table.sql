USE [EKM_App]
GO


CREATE TABLE [dbo].[RV_TaggedItems](
	[ContextID] [uniqueidentifier] NOT NULL,
	[TaggedID] [uniqueidentifier] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NOT NULL,
	[ContextType] [varchar](50) NOT NULL,
	[TaggedType] [varchar](50) NOT NULL,
	[UniqueID] [uniqueidentifier] NOT NULL
CONSTRAINT [PK_RV_TaggedItems] PRIMARY KEY CLUSTERED
(
	[ContextID] ASC,
	[TaggedID] ASC,
	[CreatorUserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[RV_TaggedItems] ADD  CONSTRAINT [UK_RV_TaggedItems] UNIQUE NONCLUSTERED 
(
	[UniqueID]		ASC,
	[ContextID]		ASC,
	[TaggedID]		ASC,
	[CreatorUserID]	ASC,
	[ContextType]	ASC,
	[TaggedType]	ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


ALTER TABLE [dbo].[RV_TaggedItems]  WITH CHECK ADD  CONSTRAINT [FK_RV_TaggedItems_aspnet_Users] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[RV_TaggedItems] CHECK CONSTRAINT [FK_RV_TaggedItems_aspnet_Users]
GO