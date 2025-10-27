USE [EKM_App]
GO

/****** Object:  Table [dbo].[KKnowledges]    Script Date: 04/04/2012 12:34:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER TABLE [dbo].[KKnowledges]
ADD [Deleted][bit] NULL
GO

UPDATE [dbo].[KKnowledges]
   SET [Deleted] = 0
GO


