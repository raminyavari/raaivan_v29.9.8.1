USE [EKM_App]
GO

/****** Object:  Table [dbo].[Nodes]    Script Date: 10/16/2011 13:35:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER TABLE [dbo].[OrganStructs]
ADD [AdditionalID] [nvarchar](20) NULL;
GO

