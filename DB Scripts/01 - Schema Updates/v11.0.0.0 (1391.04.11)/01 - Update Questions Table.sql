/****** Object:  StoredProcedure [dbo].[AddFolder]    Script Date: 03/14/2012 11:38:59 ******/
USE [EKM_App]

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER TABLE [dbo].[Questions]
ADD [Deleted] [bit] NULL
GO


ALTER TABLE [dbo].[QAnswers]
ADD [Deleted] [bit] NULL
GO