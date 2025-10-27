USE [EKM_App]
GO

IF EXISTS(select * FROM sys.views where name = 'CN_View_ExpertiseReferrals')
DROP VIEW [dbo].[CN_View_ExpertiseReferrals]
GO