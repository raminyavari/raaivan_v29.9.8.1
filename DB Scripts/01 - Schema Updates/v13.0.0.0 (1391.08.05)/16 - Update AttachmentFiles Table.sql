USE [EKM_App]
GO

ALTER TABLE [dbo].[AttachmentFiles]
ADD [Deleted] [bit] NULL
GO

ALTER TABLE [dbo].[Attachments]
ADD [Deleted] [bit] NULL
GO