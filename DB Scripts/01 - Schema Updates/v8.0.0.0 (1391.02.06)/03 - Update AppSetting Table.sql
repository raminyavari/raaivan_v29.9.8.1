USE [EKM_App]
GO

/****** Object:  Table [dbo].[KKnowledges]    Script Date: 04/04/2012 12:34:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER TABLE [dbo].[AppSetting]
ADD [Version] [nvarchar](100)
GO


UPDATE [dbo].[AppSetting]
	SET Version = '13910206'
GO