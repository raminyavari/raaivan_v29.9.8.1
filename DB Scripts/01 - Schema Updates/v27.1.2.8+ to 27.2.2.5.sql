USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



UPDATE [dbo].[AppSetting]
	SET [Version] = 'v27.1.6.0' -- 13941226
GO


/****** Object:  Table [dbo].[CN_NodeCreators]    Script Date: 04/17/2016 09:38:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[CN_TMPNodeCreators](
	[NodeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[CollaborationShare] [float] NULL,
	[Status] [varchar](20) NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
	[UniqueID] [uniqueidentifier] NOT NULL
 CONSTRAINT [PK_CN_TMPNodeCreators] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

INSERT INTO [dbo].[CN_TMPNodeCreators](
	NodeID,
	UserID,
	CollaborationShare,
	[Status],
	CreatorUserID,
	CreationDate,
	LastModifierUserID,
	LastModificationDate,
	Deleted,
	UniqueID
)
SELECT NodeID, UserID, CollaborationShare, [Status], CreatorUserID, CreationDate,
	LastModifierUserID, LastModificationDate, Deleted, NEWID()
FROM [dbo].[CN_NodeCreators]

GO

DROP TABLE [dbo].[CN_NodeCreators]
GO


SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[CN_NodeCreators](
	[NodeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[CollaborationShare] [float] NULL,
	[Status] [varchar](20) NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
	[UniqueID] [uniqueidentifier] NOT NULL
 CONSTRAINT [PK_CN_NodeCreators] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


ALTER TABLE [dbo].[CN_NodeCreators]  WITH CHECK ADD  CONSTRAINT [FK_CN_NodeCreators_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_NodeCreators] CHECK CONSTRAINT [FK_CN_NodeCreators_aspnet_Users]
GO

ALTER TABLE [dbo].[CN_NodeCreators]  WITH CHECK ADD  CONSTRAINT [FK_CN_NodeCreators_aspnet_Users_Creator] FOREIGN KEY([CreatorUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_NodeCreators] CHECK CONSTRAINT [FK_CN_NodeCreators_aspnet_Users_Creator]
GO

ALTER TABLE [dbo].[CN_NodeCreators]  WITH CHECK ADD  CONSTRAINT [FK_CN_NodeCreators_aspnet_Users_Modifier] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_NodeCreators] CHECK CONSTRAINT [FK_CN_NodeCreators_aspnet_Users_Modifier]
GO

ALTER TABLE [dbo].[CN_NodeCreators]  WITH CHECK ADD  CONSTRAINT [FK_CN_NodeCreators_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[CN_NodeCreators] CHECK CONSTRAINT [FK_CN_NodeCreators_CN_Nodes]
GO


INSERT INTO [dbo].[CN_NodeCreators]
SELECT * FROM [dbo].[CN_TMPNodeCreators]
GO


DROP TABLE [dbo].[CN_TMPNodeCreators]
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



IF OBJECT_ID ('dbo.CN_TRG_AddOrRemoveNodeCreator', 'TR') IS NOT NULL
   DROP TRIGGER dbo.CN_TRG_AddOrRemoveNodeCreator
GO

CREATE TRIGGER [dbo].[CN_TRG_AddOrRemoveNodeCreator]
ON [dbo].[CN_NodeCreators]
AFTER INSERT, UPDATE
AS

DECLARE @TBL GuidBitTableType

INSERT INTO @TBL (FirstValue, SecondValue)
SELECT inserted.UniqueID, ISNULL(inserted.Deleted, 0)
FROM inserted
	LEFT JOIN deleted
	ON deleted.UniqueID = inserted.UniqueID
WHERE deleted.UniqueID IS NULL OR ISNULL(inserted.Deleted, 0) <> ISNULL(deleted.Deleted, 0)

DECLARE @_Result int

EXEC [dbo].[RV_P_SetDeletedStates] @TBL, N'NodeCreator', NULL, @_Result

GO



IF OBJECT_ID ('dbo.WK_TRG_AddChange', 'TR') IS NOT NULL
   DROP TRIGGER dbo.WK_TRG_AddChange
GO

CREATE TRIGGER [dbo].[WK_TRG_AddChange]
ON [dbo].[WK_Changes]
AFTER INSERT
AS

DECLARE @TBL GuidBitTableType

INSERT INTO @TBL (FirstValue, SecondValue)
SELECT inserted.ChangeID, 0
FROM inserted
	INNER JOIN [dbo].[WK_Paragraphs] AS P
	ON P.ParagraphID = inserted.ParagraphID
	INNER JOIN [dbo].[WK_Titles] AS T
	ON T.TitleID = P.TitleID
	INNER JOIN [dbo].[CN_Nodes] AS ND
	ON ND.NodeID = T.OwnerID

DECLARE @_Result int

EXEC [dbo].[RV_P_SetDeletedStates] @TBL, N'WikiChange', NULL, @_Result

GO



IF OBJECT_ID ('dbo.RV_TRG_AddTaggedItem', 'TR') IS NOT NULL
   DROP TRIGGER dbo.RV_TRG_AddTaggedItem
GO

CREATE TRIGGER [dbo].[RV_TRG_AddTaggedItem]
ON [dbo].[RV_TaggedItems]
AFTER INSERT
AS

DECLARE @TBL GuidBitTableType

INSERT INTO @TBL (FirstValue, SecondValue)
SELECT inserted.UniqueID, 0
FROM inserted

DECLARE @_Result int

EXEC [dbo].[RV_P_SetDeletedStates] @TBL, N'TaggedItem', NULL, @_Result

GO





INSERT INTO [dbo].[RV_DeletedStates](ObjectID, ObjectType, Deleted)
SELECT UniqueID, N'NodeCreator', Deleted
FROM [dbo].[CN_NodeCreators]

GO




INSERT INTO [dbo].[RV_DeletedStates](ObjectID, ObjectType, Deleted)
SELECT C.ChangeID, N'WikiChange', 0
FROM [dbo].[WK_Changes] AS C
	INNER JOIN [dbo].[WK_Paragraphs] AS P
	ON P.ParagraphID = C.ParagraphID
	INNER JOIN [dbo].[WK_Titles] AS T
	ON T.TitleID = P.TitleID
	INNER JOIN [dbo].[CN_Nodes] AS ND
	ON ND.NodeID = T.OwnerID

GO




ALTER TABLE [dbo].[DCT_Trees]
ADD [SequenceNumber] int NULL
GO

ALTER TABLE [dbo].[DCT_TreeNodes]
ADD [SequenceNumber] int NULL
GO


UPDATE DS
	SET [Date] = ISNULL(NR.CreationDate, N'2011-06-20 18:12:18.700')
FROM [dbo].[RV_DeletedStates] AS DS
	INNER JOIN [dbo].[CN_NodeRelations] AS NR
	ON NR.UniqueID = DS.ObjectID	
GO


UPDATE DS
	SET [Date] = ISNULL(ISNULL(NM.AcceptionDate, NM.MembershipDate), N'2011-06-20 18:12:18.700')
FROM [dbo].[RV_DeletedStates] AS DS
	INNER JOIN [dbo].[CN_NodeMembers] AS NM
	ON NM.UniqueID = DS.ObjectID	
GO


UPDATE DS
	SET [Date] = ISNULL(
		CASE 
			WHEN ND.CreationDate > M.CreateDate THEN ND.CreationDate 
			ELSE M.CreateDate 
		END, N'2011-06-20 18:12:18.700')
FROM [dbo].[RV_DeletedStates] AS DS
	INNER JOIN [dbo].[CN_Experts] AS EX
	ON EX.UniqueID = DS.ObjectID
	INNER JOIN [dbo].[CN_Nodes] AS ND
	ON ND.NodeID = EX.NodeID
	INNER JOIN [dbo].[aspnet_Membership] AS M
	ON M.UserID = EX.UserID
GO


UPDATE DS
	SET [Date] = ISNULL(M.CreateDate, N'2011-06-20 18:12:18.700')
FROM [dbo].[RV_DeletedStates] AS DS
	INNER JOIN [dbo].[aspnet_Membership] AS M
	ON M.UserID = DS.ObjectID
GO


UPDATE DS
	SET [Date] = ISNULL(EA.CreationDate, N'2011-06-20 18:12:18.700')
FROM [dbo].[RV_DeletedStates] AS DS
	INNER JOIN [dbo].[USR_EmailAddresses] AS EA
	ON EA.EmailID = DS.ObjectID
GO


UPDATE DS
	SET [Date] = ISNULL(NL.LikeDate, N'2011-06-20 18:12:18.700')
FROM [dbo].[RV_DeletedStates] AS DS
	INNER JOIN [dbo].[CN_NodeLikes] AS NL
	ON NL.UniqueID = DS.ObjectID
GO


UPDATE DS
	SET [Date] = ISNULL(IV.VisitDate, N'2011-06-20 18:12:18.700')
FROM [dbo].[RV_DeletedStates] AS DS
	INNER JOIN [dbo].[USR_ItemVisits] AS IV
	ON IV.UniqueID = DS.ObjectID
GO


UPDATE DS
	SET [Date] = ISNULL(NC.CreationDate, N'2011-06-20 18:12:18.700')
FROM [dbo].[RV_DeletedStates] AS DS
	INNER JOIN [dbo].[CN_NodeCreators] AS NC
	ON NC.UniqueID = DS.ObjectID
GO


UPDATE DS
	SET [Date] = ISNULL(ND.CreationDate, N'2011-06-20 18:12:18.700')
FROM [dbo].[RV_DeletedStates] AS DS
	INNER JOIN [dbo].[CN_Nodes] AS ND
	ON ND.NodeID = DS.ObjectID
GO


UPDATE DS
	SET [Date] = ISNULL(ISNULL(FR.AcceptionDate, FR.RequestDate), N'2011-06-20 18:12:18.700')
FROM [dbo].[RV_DeletedStates] AS DS
	INNER JOIN [dbo].[USR_Friends] AS FR
	ON FR.UniqueID = DS.ObjectID
GO


UPDATE DS
	SET [Date] = N'2015-06-20 18:12:18.700'
FROM [dbo].[RV_DeletedStates] AS DS
	INNER JOIN [dbo].[RV_TaggedItems] AS TI
	ON TI.UniqueID = DS.ObjectID
GO


UPDATE DS
	SET [Date] = ISNULL(NT.CreationDate, N'2011-06-20 18:12:18.700')
FROM [dbo].[RV_DeletedStates] AS DS
	INNER JOIN [dbo].[CN_NodeTypes] AS NT
	ON NT.NodeTypeID = DS.ObjectID
GO


UPDATE DS
	SET [Date] = ISNULL(CH.SendDate, N'2011-06-20 18:12:18.700')
FROM [dbo].[RV_DeletedStates] AS DS
	INNER JOIN [dbo].[WK_Changes] AS CH
	ON CH.ChangeID = DS.ObjectID
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



UPDATE [dbo].[AppSetting]
	SET [Version] = 'v27.2.2.5' -- 13950201
GO

