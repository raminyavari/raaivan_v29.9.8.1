USE [EKM_App]
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