USE [EKM_App]
GO


ALTER TABLE [dbo].[CN_Services]
ADD [IsDocument] [bit] NULL,
	[IsKnowledge] [bit] NULL,
	[EditSuggestion] [bit] NULL
GO


UPDATE [dbo].[CN_Services]
	SET EditSuggestion = 1
GO