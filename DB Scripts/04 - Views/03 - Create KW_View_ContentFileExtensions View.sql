USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF EXISTS(select * FROM sys.views where name = 'ContentFileExtensions')
DROP VIEW [dbo].[ContentFileExtensions]
GO

IF EXISTS(select * FROM sys.views where name = 'KW_View_ContentFileExtensions')
DROP VIEW [dbo].[KW_View_ContentFileExtensions]
GO


CREATE VIEW [dbo].[KW_View_ContentFileExtensions] WITH SCHEMABINDING, ENCRYPTION
AS
SELECT	KW.TreeNodeID AS TreeNodeID, 
		KW.ApplicationID,
		KW.KnowledgeID AS KnowledgeID, 
		KW.Title AS KnowledgeTitle, 
		AF.Extension AS Extension,
		KW.Deleted AS Deleted
FROM [dbo].[DCT_Files] AS AF
	INNER JOIN [dbo].[KW_View_Knowledges] AS KW
	ON KW.ApplicationID = AF.ApplicationID AND KW.KnowledgeID = AF.OwnerID

GO
