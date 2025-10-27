/* This Scripts Converts old 'KKnowledgeExistanceCards' table to the new 'KnowledgeCards' table */

USE [EKM_App]
GO

/****** Object:  Table [dbo].[KnowledgeCards]    Script Date: 10/12/2011 19:42:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[KnowledgeCards](
	[ID] [uniqueidentifier] NOT NULL,
	[SenderUserID] [uniqueidentifier] NOT NULL,
	[ReceiverUserID] [uniqueidentifier] NOT NULL,
	[KTypeID] [bigint] NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
	[Description] [text] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[Deleted] [bit] NULL,
 CONSTRAINT [PK_KnowledgeCards] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[KnowledgeCards]  WITH CHECK ADD  CONSTRAINT [FK_KnowledgeCards_aspnet_Users] FOREIGN KEY([SenderUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KnowledgeCards] CHECK CONSTRAINT [FK_KnowledgeCards_aspnet_Users]
GO

ALTER TABLE [dbo].[KnowledgeCards]  WITH CHECK ADD  CONSTRAINT [FK_KnowledgeCards_aspnet_Users1] FOREIGN KEY([ReceiverUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KnowledgeCards] CHECK CONSTRAINT [FK_KnowledgeCards_aspnet_Users1]
GO

ALTER TABLE [dbo].[KnowledgeCards]  WITH CHECK ADD  CONSTRAINT [FK_KnowledgeCards_KKnowledgeType] FOREIGN KEY([KTypeID])
REFERENCES [dbo].[KKnowledgeType] ([ID])
GO

ALTER TABLE [dbo].[KnowledgeCards] CHECK CONSTRAINT [FK_KnowledgeCards_KKnowledgeType]
GO

insert into KnowledgeCards(ID, SenderUserID, ReceiverUserID, KTypeID, Title, Description, 
	CreationDate, Deleted)
select KKnowledgeExistanceCards.ID, KKnowledgeExistanceCards.NominatorUserID,
	KKnowledgeExistanceCards.NominatedUserID, 1, KKnowledgeExistanceCards.Title,
	KKnowledgeExistanceCards.Description, KKnowledgeExistanceCards.NominationDateTime, 0 
	from KKnowledgeExistanceCards
	where KKnowledgeExistanceCards.Type = 'Experience'
go

insert into KnowledgeCards(ID, SenderUserID, ReceiverUserID, KTypeID, Title, Description, 
	CreationDate, Deleted)
select KKnowledgeExistanceCards.ID, KKnowledgeExistanceCards.NominatorUserID,
	KKnowledgeExistanceCards.NominatedUserID, 3, KKnowledgeExistanceCards.Title,
	KKnowledgeExistanceCards.Description, KKnowledgeExistanceCards.NominationDateTime, 0 
	from KKnowledgeExistanceCards
	where KKnowledgeExistanceCards.Type = 'Skill'
go

insert into KnowledgeCards(ID, SenderUserID, ReceiverUserID, KTypeID, Title, Description, 
	CreationDate, Deleted)
select KKnowledgeExistanceCards.ID, KKnowledgeExistanceCards.NominatorUserID,
	KKnowledgeExistanceCards.NominatedUserID, 3, KKnowledgeExistanceCards.Title,
	KKnowledgeExistanceCards.Description, KKnowledgeExistanceCards.NominationDateTime, 0 
	from KKnowledgeExistanceCards
	where KKnowledgeExistanceCards.Type = 'Content'
go

drop table [dbo].[KKnowledgeExistanceCards]
go