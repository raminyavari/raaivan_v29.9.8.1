USE [EKM_App]
GO

/****** Object:  Table [dbo].[NodePagesTotalVisitors]    Script Date: 02/17/2012 13:39:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[GapColorConstTemp](
	[ID] [uniqueidentifier] NOT NULL,
	[Wbot] [bigint] NOT NULL,
	[Wup] [bigint] NOT NULL,
	[Gbot] [float] NOT NULL,
	[Gup] [float] NOT NULL,
	[Ybot] [float] NOT NULL,
	[Yup] [float] NOT NULL,
	[Obot] [float] NOT NULL,
	[Oup] [float] NOT NULL,
	[Rbot] [float] NOT NULL,
	[Rup] [float] NOT NULL,
 CONSTRAINT [PK_GapColorConstTemp] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[GapColorConstTemp]
           ([ID]
			,[Wbot]
			,[Wup]
			,[Gbot]
			,[Gup]
			,[Ybot]
			,[Yup]
			,[Obot]
			,[Oup]
			,[Rbot]
			,[Rup])
		SELECT  NEWID(), dbo.GapColorConst.Wbot, dbo.GapColorConst.Wup, dbo.GapColorConst.Gbot, dbo.GapColorConst.Gup,
				dbo.GapColorConst.Ybot, dbo.GapColorConst.Yup, dbo.GapColorConst.Obot, dbo.GapColorConst.Oup,
				dbo.GapColorConst.Rbot, dbo.GapColorConst.Rup
		FROM    dbo.GapColorConst
GO


/* Drop old table */
DROP TABLE [dbo].[GapColorConst]
GO

/* Create old new table */
CREATE TABLE [dbo].[GapColorConst](
	[ID] [uniqueidentifier] NOT NULL,
	[Wbot] [bigint] NOT NULL,
	[Wup] [bigint] NOT NULL,
	[Gbot] [float] NOT NULL,
	[Gup] [float] NOT NULL,
	[Ybot] [float] NOT NULL,
	[Yup] [float] NOT NULL,
	[Obot] [float] NOT NULL,
	[Oup] [float] NOT NULL,
	[Rbot] [float] NOT NULL,
	[Rup] [float] NOT NULL,
 CONSTRAINT [PK_GapColorConst] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[GapColorConst]
	SELECT * FROM dbo.GapColorConstTemp
GO

DROP TABLE [dbo].[GapColorConstTemp]
GO