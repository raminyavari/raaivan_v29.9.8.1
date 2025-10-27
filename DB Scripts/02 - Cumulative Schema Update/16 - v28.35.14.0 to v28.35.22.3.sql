USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.35.14.0' BEGIN
	ALTER TABLE [dbo].[KW_KnowledgeTypes]
	ADD [EvaluationsRemovable] [bit] NULL	
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.35.14.0' BEGIN
	ALTER TABLE [dbo].[KW_KnowledgeTypes]
	ADD [EvaluationsEditable] [bit] NULL	
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.35.14.0' BEGIN
	ALTER TABLE [dbo].[KW_KnowledgeTypes]
	ADD [UnhideEvaluators] [bit] NULL	
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.35.14.0' BEGIN
	ALTER TABLE [dbo].[KW_KnowledgeTypes]
	ADD [UnhideEvaluations] [bit] NULL	
END
GO

IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.35.14.0' BEGIN
	ALTER TABLE [dbo].[KW_KnowledgeTypes]
	ADD [UnhideNodeCreators] [bit] NULL	
END
GO

SET ANSI_PADDING OFF
GO


IF (SELECT TOP(1) X.[Version] FROM [dbo].[AppSetting] AS X) = N'v28.35.14.0' BEGIN
	UPDATE [dbo].[AppSetting]
		SET [Version] = 'v28.35.22.3'
END
GO