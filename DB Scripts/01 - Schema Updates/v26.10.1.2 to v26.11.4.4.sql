USE [EKM_App]
GO

IF EXISTS(select * FROM sys.views where name = 'CN_View_ExpertiseReferrals')
DROP VIEW [dbo].[CN_View_ExpertiseReferrals]
GO


/****** Object:  Table [dbo].[RV_DeletedStates]    Script Date: 08/02/2015 10:49:52 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[RV_DeletedStates](
	[ID] [bigint] IDENTITY(1, 1) NOT NULL,
	[ObjectID] [uniqueidentifier] NOT NULL,
	[ObjectType] [varchar](50),
	[Deleted] [bit] NOT NULL,
	[Date] [datetime] NULL
 CONSTRAINT [PK_RV_DeletedStates] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


CREATE UNIQUE INDEX UX_RV_DeletedStates_ObjectID ON [dbo].[RV_DeletedStates]
(
	[ObjectID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


/****** Object:  Table [dbo].[CN_NodeMembers]    Script Date: 08/01/2015 20:15:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(select * FROM sys.views where name = 'CN_View_ExpertiseReferrals')
DROP VIEW [dbo].[CN_View_ExpertiseReferrals]
GO

IF EXISTS(select * FROM sys.views where name = 'CN_View_NodeMembers')
DROP VIEW [dbo].[CN_View_NodeMembers]
GO

IF EXISTS(select * FROM sys.views where name = 'CN_View_ListMembers')
DROP VIEW [dbo].[CN_View_ListMembers]
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[CN_TMPNodeMembers](
	[NodeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[MembershipDate] [datetime] NOT NULL,
	[IsAdmin] [bit] NOT NULL,
	[Status] [varchar](20) NOT NULL,
	[AcceptionDate] [datetime] NULL,
	[Position] [nvarchar](255) NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_CN_TMPNodeMembers] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[CN_TMPNodeMembers](
	NodeID,
	UserID,
	MembershipDate,
	IsAdmin,
	[Status],
	[AcceptionDate],
	Position,
	Deleted
)
SELECT NodeID, UserID, MembershipDate, IsAdmin, [Status], AcceptionDate, Position, Deleted
FROM [dbo].[CN_NodeMembers]

GO


DROP TABLE [dbo].[CN_NodeMembers]
GO



CREATE TABLE [dbo].[CN_NodeMembers](
	[NodeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[MembershipDate] [datetime] NOT NULL,
	[IsAdmin] [bit] NOT NULL,
	[Status] [varchar](20) NOT NULL,
	[AcceptionDate] [datetime] NULL,
	[Position] [nvarchar](255) NULL,
	[Deleted] [bit] NOT NULL,
	[UniqueID] [uniqueidentifier] NOT NULL
 CONSTRAINT [PK_CN_NodeMembers] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[CN_NodeMembers](
	NodeID,
	UserID,
	MembershipDate,
	IsAdmin,
	[Status],
	[AcceptionDate],
	Position,
	Deleted,
	UniqueID
)
SELECT NodeID, UserID, MembershipDate, IsAdmin, [Status], AcceptionDate, Position, Deleted, NEWID()
FROM [dbo].[CN_TMPNodeMembers]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[CN_NodeMembers]  WITH CHECK ADD  CONSTRAINT [FK_CN_NodeMembers_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_NodeMembers] CHECK CONSTRAINT [FK_CN_NodeMembers_aspnet_Users]
GO

ALTER TABLE [dbo].[CN_NodeMembers]  WITH CHECK ADD  CONSTRAINT [FK_CN_NodeMembers_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[CN_NodeMembers] CHECK CONSTRAINT [FK_CN_NodeMembers_CN_Nodes]
GO


DROP TABLE [dbo].[CN_TMPNodeMembers]
GO


/****** Object:  Table [dbo].[CN_Experts]    Script Date: 08/01/2015 20:15:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(select * FROM sys.views where name = 'CN_View_ExpertiseReferrals')
DROP VIEW [dbo].[CN_View_ExpertiseReferrals]
GO

IF EXISTS(select * FROM sys.views where name = 'CN_View_Experts')
DROP VIEW [dbo].[CN_View_Experts]
GO

IF EXISTS(select * FROM sys.views where name = 'CN_View_ListExperts')
DROP VIEW [dbo].[CN_View_ListExperts]
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[CN_TMPExperts](
	[NodeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[Approved] [bit] NOT NULL,
	[ReferralsCount] [int] NOT NULL,
	[ConfirmsPercentage] [float] NOT NULL,
	[SocialApproved] [bit] NULL,
 CONSTRAINT [PK_CN_TMPExperts] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[CN_TMPExperts](
	NodeID,
	UserID,
	Approved,
	ReferralsCount,
	ConfirmsPercentage,
	SocialApproved
)
SELECT NodeID, UserID, Approved, ReferralsCount, ConfirmsPercentage, SocialApproved
FROM [dbo].[CN_Experts]

GO


DROP TABLE [dbo].[CN_Experts]
GO


CREATE TABLE [dbo].[CN_Experts](
	[NodeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[Approved] [bit] NOT NULL,
	[ReferralsCount] [int] NOT NULL,
	[ConfirmsPercentage] [float] NOT NULL,
	[SocialApproved] [bit] NULL,
	[UniqueID] [uniqueidentifier] NOT NULL
 CONSTRAINT [PK_CN_Experts] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[CN_Experts](
	NodeID,
	UserID,
	Approved,
	ReferralsCount,
	ConfirmsPercentage,
	SocialApproved,
	UniqueID
)
SELECT NodeID, UserID, Approved, ReferralsCount, ConfirmsPercentage, SocialApproved, NEWID()
FROM [dbo].[CN_TMPExperts]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[CN_Experts]  WITH CHECK ADD  CONSTRAINT [FK_CN_Experts_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_Experts] CHECK CONSTRAINT [FK_CN_Experts_aspnet_Users]
GO

ALTER TABLE [dbo].[CN_Experts]  WITH CHECK ADD  CONSTRAINT [FK_CN_Experts_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[CN_Experts] CHECK CONSTRAINT [FK_CN_Experts_CN_Nodes]
GO


DROP TABLE [dbo].[CN_TMPExperts]
GO


/****** Object:  Table [dbo].[CN_NodeLikes]    Script Date: 08/01/2015 20:15:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[CN_TMPNodeLikes](
	[NodeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[LikeDate] [datetime] NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_CN_TMPNodeLikes] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[CN_TMPNodeLikes](
	NodeID,
	UserID,
	LikeDate,
	Deleted
)
SELECT NodeID, UserID, LikeDate, Deleted
FROM [dbo].[CN_NodeLikes]

GO


DROP TABLE [dbo].[CN_NodeLikes]
GO


CREATE TABLE [dbo].[CN_NodeLikes](
	[NodeID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[LikeDate] [datetime] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[UniqueID] [uniqueidentifier] NOT NULL
 CONSTRAINT [PK_CN_NodeLikes] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[CN_NodeLikes](
	NodeID,
	UserID,
	LikeDate,
	Deleted,
	UniqueID
)
SELECT NodeID, UserID, LikeDate, Deleted, NEWID()
FROM [dbo].[CN_TMPNodeLikes]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[CN_NodeLikes]  WITH CHECK ADD  CONSTRAINT [FK_CN_NodeLikes_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[CN_NodeLikes] CHECK CONSTRAINT [FK_CN_NodeLikes_aspnet_Users]
GO

ALTER TABLE [dbo].[CN_NodeLikes]  WITH CHECK ADD  CONSTRAINT [FK_CN_NodeLikes_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[CN_NodeLikes] CHECK CONSTRAINT [FK_CN_NodeLikes_CN_Nodes]
GO


DROP TABLE [dbo].[CN_TMPNodeLikes]
GO


/****** Object:  Table [dbo].[CN_NodeRelations]    Script Date: 08/01/2015 20:15:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(select * FROM sys.views where name = 'CN_View_NodeRelations')
DROP VIEW [dbo].[CN_View_NodeRelations]
GO

IF EXISTS(select * FROM sys.views where name = 'CN_View_InRelatedNodes')
DROP VIEW [dbo].[CN_View_InRelatedNodes]
GO

IF EXISTS(select * FROM sys.views where name = 'CN_View_OutRelatedNodes')
DROP VIEW [dbo].[CN_View_OutRelatedNodes]
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[CN_TMPNodeRelations](
	[SourceNodeID] [uniqueidentifier] NOT NULL,
	[DestinationNodeID] [uniqueidentifier] NOT NULL,
	[PropertyID] [uniqueidentifier] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[NominalValue] [nvarchar](255) NULL,
	[NumericalValue] [float] NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_CN_TMPNodeRelations] PRIMARY KEY CLUSTERED 
(
	[SourceNodeID] ASC,
	[DestinationNodeID] ASC,
	[PropertyID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[CN_TMPNodeRelations](
	SourceNodeID,
	DestinationNodeID,
	PropertyID,
	CreatorUserID,
	CreationDate,
	LastModifierUserID,
	LastModificationDate,
	NominalValue,
	NumericalValue,
	Deleted
)
SELECT SourceNodeID, DestinationNodeID, PropertyID, CreatorUserID, CreationDate,
	LastModifierUserID, LastModificationDate, NominalValue, NumericalValue, Deleted
FROM [dbo].[CN_NodeRelations]

GO


DROP TABLE [dbo].[CN_NodeRelations]
GO


CREATE TABLE [dbo].[CN_NodeRelations](
	[SourceNodeID] [uniqueidentifier] NOT NULL,
	[DestinationNodeID] [uniqueidentifier] NOT NULL,
	[PropertyID] [uniqueidentifier] NOT NULL,
	[CreatorUserID] [uniqueidentifier] NULL,
	[CreationDate] [datetime] NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NULL,
	[NominalValue] [nvarchar](255) NULL,
	[NumericalValue] [float] NULL,
	[Deleted] [bit] NOT NULL,
	[UniqueID] [uniqueidentifier] NOT NULL
 CONSTRAINT [PK_CN_NodeRelations] PRIMARY KEY CLUSTERED 
(
	[SourceNodeID] ASC,
	[DestinationNodeID] ASC,
	[PropertyID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[CN_NodeRelations](
	SourceNodeID,
	DestinationNodeID,
	PropertyID,
	CreatorUserID,
	CreationDate,
	LastModifierUserID,
	LastModificationDate,
	NominalValue,
	NumericalValue,
	Deleted,
	UniqueID
)
SELECT SourceNodeID, DestinationNodeID, PropertyID, CreatorUserID, CreationDate,
	LastModifierUserID, LastModificationDate, NominalValue, NumericalValue, Deleted, NEWID()
FROM [dbo].[CN_TMPNodeRelations]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[CN_NodeRelations]  WITH CHECK ADD  CONSTRAINT [FK_CN_NodeRelations_CN_Nodes_Destination] FOREIGN KEY([DestinationNodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[CN_NodeRelations] CHECK CONSTRAINT [FK_CN_NodeRelations_CN_Nodes_Destination]
GO

ALTER TABLE [dbo].[CN_NodeRelations]  WITH CHECK ADD  CONSTRAINT [FK_CN_NodeRelations_CN_Nodes_Source] FOREIGN KEY([SourceNodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[CN_NodeRelations] CHECK CONSTRAINT [FK_CN_NodeRelations_CN_Nodes_Source]
GO

ALTER TABLE [dbo].[CN_NodeRelations]  WITH CHECK ADD  CONSTRAINT [FK_CN_NodeRelations_CN_Properties] FOREIGN KEY([PropertyID])
REFERENCES [dbo].[CN_Properties] ([PropertyID])
GO

ALTER TABLE [dbo].[CN_NodeRelations] CHECK CONSTRAINT [FK_CN_NodeRelations_CN_Properties]
GO


DROP TABLE [dbo].[CN_TMPNodeRelations]
GO


/****** Object:  Table [dbo].[USR_ItemVisits]    Script Date: 08/01/2015 20:15:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[USR_TMPItemVisits](
	[ItemID] [uniqueidentifier] NOT NULL,
	[VisitDate] [datetime] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[ItemType] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_USR_TMPItemVisits] PRIMARY KEY CLUSTERED 
(
	[ItemID] ASC,
	[VisitDate] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[USR_TMPItemVisits](
	ItemID,
	VisitDate,
	UserID,
	ItemType
)
SELECT ItemID, VisitDate, UserID, ItemType
FROM [dbo].[USR_ItemVisits]

GO


DROP TABLE [dbo].[USR_ItemVisits]
GO


CREATE TABLE [dbo].[USR_ItemVisits](
	[ItemID] [uniqueidentifier] NOT NULL,
	[VisitDate] [datetime] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[ItemType] [nvarchar](255) NOT NULL,
	[UniqueID] [uniqueidentifier] NOT NULL
 CONSTRAINT [PK_USR_ItemVisits] PRIMARY KEY CLUSTERED 
(
	[ItemID] ASC,
	[VisitDate] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[USR_ItemVisits](
	ItemID,
	VisitDate,
	UserID,
	ItemType,
	UniqueID
)
SELECT ItemID, VisitDate, UserID, ItemType, NEWID()
FROM [dbo].[USR_TMPItemVisits]

GO

SET ANSI_PADDING OFF
GO


DROP TABLE [dbo].[USR_TMPItemVisits]
GO


/****** Object:  Table [dbo].[USR_EmailContacts]    Script Date: 08/01/2015 20:15:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[USR_TMPEmailContacts](
	[UserID] [uniqueidentifier] NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_USR_TMPEmailContacts] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[Email] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[USR_TMPEmailContacts](
	UserID,
	Email,
	CreationDate,
	Deleted
)
SELECT UserID, Email, CreationDate, Deleted
FROM [dbo].[USR_EmailContacts]

GO


DROP TABLE [dbo].[USR_EmailContacts]
GO


CREATE TABLE [dbo].[USR_EmailContacts](
	[UserID] [uniqueidentifier] NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[UniqueID] [uniqueidentifier] NOT NULL
 CONSTRAINT [PK_USR_EmailContacts] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[Email] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[USR_EmailContacts](
	UserID,
	Email,
	CreationDate,
	Deleted,
	UniqueID
)
SELECT UserID, Email, CreationDate, Deleted, NEWID()
FROM [dbo].[USR_TMPEmailContacts]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[USR_EmailContacts]  WITH CHECK ADD  CONSTRAINT [FK_USR_EmailContacts_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[USR_EmailContacts] CHECK CONSTRAINT [FK_USR_EmailContacts_aspnet_Users]
GO


DROP TABLE [dbo].[USR_TMPEmailContacts]
GO



SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(select * FROM sys.views where name = 'USR_View_Friends')
DROP VIEW [dbo].[USR_View_Friends]
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[USR_Friends](
	[SenderUserID] [uniqueidentifier] NOT NULL,
	[ReceiverUserID] [uniqueidentifier] NOT NULL,
	[AreFriends] [bit] NOT NULL,
	[RequestDate] [datetime] NOT NULL,
	[AcceptionDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL,
	[UniqueID] [uniqueidentifier] NOT NULL
 CONSTRAINT [PK_USR_Friends] PRIMARY KEY CLUSTERED 
(
	[SenderUserID] ASC,
	[ReceiverUserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[USR_Friends]  WITH CHECK ADD  CONSTRAINT [FK_USR_Friends_aspnet_Users] FOREIGN KEY([SenderUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[USR_Friends] CHECK CONSTRAINT [FK_USR_Friends_aspnet_Users]
GO

ALTER TABLE [dbo].[USR_Friends]  WITH CHECK ADD  CONSTRAINT [FK_USR_Friends_aspnet_Users_Receiver] FOREIGN KEY([ReceiverUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[USR_Friends] CHECK CONSTRAINT [FK_USR_Friends_aspnet_Users_Receiver]
GO


INSERT INTO [dbo].[USR_Friends](
	SenderUserID,
	ReceiverUserID,
	AreFriends,
	RequestDate,
	AcceptionDate,
	Deleted,
	UniqueID
)
SELECT	SenderUserID, 
		ReceiverUserID,
		(CASE WHEN UC.[Status] = N'Accept' THEN 1 ELSE 0 END),
		RequestDate,
		AcceptionDate,
		0,
		NEWID()
FROM [dbo].[UserConnections] AS UC

GO


DROP TABLE [dbo].[UserConnections]
GO





INSERT INTO [dbo].[RV_DeletedStates](ObjectID, ObjectType, Deleted)
SELECT NodeTypeID, N'NodeType', Deleted
FROM [dbo].[CN_NodeTypes]

GO


INSERT INTO [dbo].[RV_DeletedStates](ObjectID, ObjectType, Deleted)
SELECT NodeID, N'Node', Deleted
FROM [dbo].[CN_Nodes]

GO


INSERT INTO [dbo].[RV_DeletedStates](ObjectID, ObjectType, Deleted)
SELECT UniqueID, N'NodeMember', Deleted
FROM [dbo].[CN_NodeMembers]

GO


INSERT INTO [dbo].[RV_DeletedStates](ObjectID, ObjectType, Deleted)
SELECT UniqueID, N'Expert', (CASE WHEN Approved = 1 OR SocialApproved = 1 THEN 1 ELSE 0 END)
FROM [dbo].[CN_Experts]

GO


INSERT INTO [dbo].[RV_DeletedStates](ObjectID, ObjectType, Deleted)
SELECT UniqueID, N'NodeLike', Deleted
FROM [dbo].[CN_NodeLikes]

GO


INSERT INTO [dbo].[RV_DeletedStates](ObjectID, ObjectType, Deleted)
SELECT UniqueID, N'NodeRelation', Deleted
FROM [dbo].[CN_NodeRelations]

GO


INSERT INTO [dbo].[RV_DeletedStates](ObjectID, ObjectType, Deleted)
SELECT UserId, N'User', (CASE WHEN ISNULL(IsApproved, 0) = 1 THEN 0 ELSE 1 END)
FROM [dbo].[aspnet_Membership]

GO


INSERT INTO [dbo].[RV_DeletedStates](ObjectID, ObjectType, Deleted)
SELECT UniqueID, N'Friend', (CASE WHEN Deleted = 1 OR AreFriends = 0 THEN 1 ELSE 0 END)
FROM [dbo].[USR_Friends]

GO


INSERT INTO [dbo].[RV_DeletedStates](ObjectID, ObjectType, Deleted)
SELECT EmailID, N'EmailAddress', Deleted
FROM [dbo].[USR_EmailAddresses]

GO


INSERT INTO [dbo].[RV_DeletedStates](ObjectID, ObjectType, Deleted)
SELECT UniqueID, N'EmailContact', Deleted
FROM [dbo].[USR_EmailContacts]

GO


INSERT INTO [dbo].[RV_DeletedStates](ObjectID, ObjectType, Deleted)
SELECT UniqueID, N'ItemVisit', 0
FROM [dbo].[USR_ItemVisits]

GO


INSERT INTO [dbo].[RV_DeletedStates](ObjectID, ObjectType, Deleted)
SELECT ID, N'Invitation', 0
FROM [dbo].[USR_Invitations]

GO


/****** Object:  Table [dbo].[CN_NodeLikes]    Script Date: 08/01/2015 20:15:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

DROP TABLE [USR_FriendSuggestions]
GO


CREATE TABLE [dbo].[USR_FriendSuggestions](
	[UserID] [uniqueidentifier] NOT NULL,
	[SuggestedUserID] [uniqueidentifier] NOT NULL,
	[Score] [float] NOT NULL,
 CONSTRAINT [PK_USR_TMPFriendSuggestions] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[SuggestedUserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[USR_FriendSuggestions]  WITH CHECK ADD  CONSTRAINT [FK_USR_FriendSuggestions_aspnet_Users_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[USR_FriendSuggestions] CHECK CONSTRAINT [FK_USR_FriendSuggestions_aspnet_Users_UserID]
GO

ALTER TABLE [dbo].[USR_FriendSuggestions]  WITH CHECK ADD  CONSTRAINT [FK_USR_FriendSuggestions_aspnet_Users_SuggestedUserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[USR_FriendSuggestions] CHECK CONSTRAINT [FK_USR_FriendSuggestions_aspnet_Users_SuggestedUserID]
GO






/****** Object:  Table [dbo].[KKnowledges]    Script Date: 04/04/2012 12:34:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



UPDATE [dbo].[AppSetting]
	SET [Version] = 'v26.11.4.4' -- 13940511
GO
