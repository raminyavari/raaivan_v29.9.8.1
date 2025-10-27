USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



ALTER TABLE [dbo].[CN_NodeTypes]
ADD [SequenceNumber] int NULL
GO

ALTER TABLE [dbo].[CN_Nodes]
ADD [SequenceNumber] int NULL
GO