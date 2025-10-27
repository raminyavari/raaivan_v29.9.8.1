USE [EKM_App]
GO

/****** Object:  Table [dbo].[City]    Script Date: 03/05/2012 17:16:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Cities](
	[CityID] [uniqueidentifier] NOT NULL,
	[CountryID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Population] [bigint] NULL,
	[Longtitude] [float] NULL,
	[Latitude] [float] NULL,
	[Code] [nvarchar](200) NULL,
 CONSTRAINT [PK_Cities] PRIMARY KEY CLUSTERED 
(
	[CityID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[Cities]  WITH CHECK ADD  CONSTRAINT [FK_Cities_Countries] FOREIGN KEY([CountryID])
REFERENCES [dbo].[Countries] ([CountryID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Cities] CHECK CONSTRAINT [FK_Cities_Countries]
GO


INSERT INTO [dbo].[Cities]
           ([CityID]
			,[CountryID]
			,[Name]
			,[Population]
			,[Longtitude]
			,[Latitude]
			,[Code])
		SELECT  NEWID(), dbo.Countries.CountryID, dbo.City.cityname, dbo.City.population, 
				dbo.City.lat, dbo.City.lat, dbo.City.code
		FROM    dbo.City INNER JOIN dbo.Countries ON dbo.City.countryname = dbo.Countries.Name
GO



/* Drop old table */
DROP TABLE [dbo].[City]
GO

