USE [EKM_App]
GO


/****** Object:  Table [dbo].[KAssessmentCheckLists]    Script Date: 02/19/2012 01:21:36 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_KAssessmentCheckLists_KKnowledges]') AND parent_object_id = OBJECT_ID(N'[dbo].[KAssessmentCheckLists]'))
ALTER TABLE [dbo].[KAssessmentCheckLists] DROP CONSTRAINT [FK_KAssessmentCheckLists_KKnowledges]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[KAssessmentCheckLists]') AND type in (N'U'))
DROP TABLE [dbo].[KAssessmentCheckLists]
GO


