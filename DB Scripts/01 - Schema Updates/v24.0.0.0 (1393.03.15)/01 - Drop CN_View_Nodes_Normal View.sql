USE [EKM_App]
GO

/****** Object:  View [dbo].[CN_View_Nodes_Normal]    Script Date: 06/22/2012 13:03:22 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF EXISTS(select * FROM sys.views where name = 'CN_View_Nodes_Normal')
DROP VIEW [dbo].[CN_View_Nodes_Normal]
GO

