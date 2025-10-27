USE [EKM_App]
GO

/****** Object:  Table [dbo].[KKnowledges]    Script Date: 05/31/2012 21:33:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER TABLE [dbo].[KKnowledges]
ADD [Score] [float] NULL
GO

ALTER TABLE [dbo].[KKnowledges]
ADD [ScoresWeight] [float] NULL
GO

