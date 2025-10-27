USE [EKM_App]
GO


DECLARE @Items TABLE ([Action] varchar(100), [Level] varchar(20))

INSERT INTO @Items ([Action], [Level])
VALUES	(N'ExportReportToExcel', N'Fatal'),
		(N'JobFailed', N'Fatal'),
		(N'ExtractFormsFromXML', N'Fatal'),
		(N'InitSearchIndexDocuments', N'Fatal'),
		(N'IndexUpdateJob', N'Fatal')

UPDATE L
	SET [Level] = I.[Level]
FROM [dbo].[LG_ErrorLogs] AS L
	INNER JOIN @Items AS I
	ON I.[Action] = L.[Subject]
GO


UPDATE L
	SET [Level] = N'Debug'
FROM [dbo].[LG_ErrorLogs] AS L
WHERE L.[Level] IS NULL
GO