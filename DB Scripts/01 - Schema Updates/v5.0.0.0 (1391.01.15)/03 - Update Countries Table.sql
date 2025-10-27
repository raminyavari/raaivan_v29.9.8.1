USE [EKM_App]
GO

/****** Object:  Table [dbo].[Countries]    Script Date: 03/05/2012 17:21:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CountriesTemp](
	[CountryID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[CountryCode] [nvarchar](20) NULL,
	[Code] [nvarchar](20) NULL,
	[int] [bigint] NULL,
 CONSTRAINT [PK_CountriesTemp] PRIMARY KEY CLUSTERED 
(
	[CountryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[CountriesTemp]
           ([CountryID]
			,[Name]
			,[CountryCode]
			,[Code]
			,[int])
		SELECT  NEWID(), dbo.Countries.Country_Name, dbo.Countries.Country_Code, dbo.Countries.Code, dbo.Countries.int
		FROM    dbo.Countries
GO



/* Drop old table */
DROP TABLE [dbo].[Countries]
GO

/* Create old new table */
CREATE TABLE [dbo].[Countries](
	[CountryID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[CountryCode] [nvarchar](20) NULL,
	[Code] [nvarchar](20) NULL,
	[int] [bigint] NULL,
 CONSTRAINT [PK_Countries] PRIMARY KEY CLUSTERED 
(
	[CountryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[Countries]
	SELECT * FROM dbo.CountriesTemp
GO

DROP TABLE [dbo].[CountriesTemp]
GO


