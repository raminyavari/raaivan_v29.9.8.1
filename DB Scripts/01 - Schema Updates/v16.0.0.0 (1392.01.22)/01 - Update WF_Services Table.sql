USE [EKM_App]
GO

ALTER TABLE [dbo].[WF_Services]
ADD [Description] [nvarchar](max)
GO


ALTER TABLE [dbo].[WF_Services]
ADD [InitialMessage] [nvarchar](max)
GO