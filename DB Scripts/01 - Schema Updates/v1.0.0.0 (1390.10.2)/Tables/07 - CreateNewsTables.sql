USE [EKM_App]
GO

/****** Object:  Table [dbo].[NewsObjectTypes]    Script Date: 10/28/2011 00:05:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[NewsObjectTypes](
	[TypeID] [uniqueidentifier] NOT NULL,
	[TypeName] [nvarchar](50) NULL,
	[PersianType] [nvarchar](50) NULL,
 CONSTRAINT [PK_NewsObjectTypes] PRIMARY KEY CLUSTERED 
(
	[TypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[NewsObjectTypes]
           ([TypeID]
           ,[TypeName]
           ,[PersianType])
     VALUES
           (NEWID() 
           ,'Knowledge'
           ,'œ«‰‘')
GO

INSERT INTO [dbo].[NewsObjectTypes]
           ([TypeID]
           ,[TypeName]
           ,[PersianType])
     VALUES
           (NEWID()
           ,'QAnswer'
           ,'Å«”Œ ”Ê«·« ')
GO


/****** Object:  Table [dbo].[PersonalNews]    Script Date: 10/28/2011 00:05:12 ******/
CREATE TABLE [dbo].[PersonalNews](
	[UserID] [uniqueidentifier] NOT NULL,
	[ObjectID] [uniqueidentifier] NOT NULL,
	[NewsTypeID] [uniqueidentifier] NULL,
	[Seen] [bit] NULL,
 CONSTRAINT [PK_Table_1] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[ObjectID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[PersonalNews]  WITH CHECK ADD  CONSTRAINT [FK_PersonalNews_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[PersonalNews] CHECK CONSTRAINT [FK_PersonalNews_aspnet_Users]
GO

ALTER TABLE [dbo].[PersonalNews]  WITH CHECK ADD  CONSTRAINT [FK_PersonalNews_NewsObjectTypes] FOREIGN KEY([NewsTypeID])
REFERENCES [dbo].[NewsObjectTypes] ([TypeID])
GO

ALTER TABLE [dbo].[PersonalNews] CHECK CONSTRAINT [FK_PersonalNews_NewsObjectTypes]
GO