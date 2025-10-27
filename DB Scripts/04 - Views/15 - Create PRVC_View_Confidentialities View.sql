USE [EKM_App]
GO

SET NUMERIC_ROUNDABORT OFF;
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT,
    QUOTED_IDENTIFIER, ANSI_NULLS ON;


IF EXISTS(select * FROM sys.views where name = 'PRVC_View_Confidentialities')
DROP VIEW [dbo].[PRVC_View_Confidentialities]
GO


CREATE VIEW [dbo].[PRVC_View_Confidentialities] WITH SCHEMABINDING, ENCRYPTION
AS
SELECT  S.ApplicationID,
		S.ObjectID,
		CL.ID AS ConfidentialityID,
		CL.LevelID AS LevelID,
		CL.Title AS [Level]
FROM    [dbo].[PRVC_ConfidentialityLevels] AS CL
		INNER JOIN [dbo].[PRVC_Settings] AS S
		ON S.ApplicationID = CL.ApplicationID AND S.ConfidentialityID = CL.ID
WHERE CL.Deleted = 0 AND S.ConfidentialityID IS NOT NULL

GO


CREATE UNIQUE CLUSTERED INDEX PK_PRVC_View_Confidentialities_ObjectID ON [dbo].[PRVC_View_Confidentialities]
(
	[ObjectID] ASC,
	[ApplicationID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO