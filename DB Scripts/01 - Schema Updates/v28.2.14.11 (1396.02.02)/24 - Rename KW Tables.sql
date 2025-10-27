USE [EKM_App]
GO

EXEC sp_rename 'TMP_KW_AnswerOptions', 'KW_AnswerOptions'
GO

EXEC sp_rename 'TMP_KW_CandidateRelations', 'KW_CandidateRelations'
GO

EXEC sp_rename 'TMP_KW_FeedBacks', 'KW_FeedBacks'
GO

EXEC sp_rename 'TMP_KW_History', 'KW_History'
GO

EXEC sp_rename 'TMP_KW_KnowledgeTypes', 'KW_KnowledgeTypes'
GO

EXEC sp_rename 'TMP_KW_NecessaryItems', 'KW_NecessaryItems'
GO

EXEC sp_rename 'TMP_KW_QuestionAnswers', 'KW_QuestionAnswers'
GO

EXEC sp_rename 'TMP_KW_Questions', 'KW_Questions'
GO

EXEC sp_rename 'TMP_KW_TempKnowledgeTypeIDs', 'KW_TempKnowledgeTypeIDs'
GO

EXEC sp_rename 'TMP_KW_TypeQuestions', 'KW_TypeQuestions'
GO
