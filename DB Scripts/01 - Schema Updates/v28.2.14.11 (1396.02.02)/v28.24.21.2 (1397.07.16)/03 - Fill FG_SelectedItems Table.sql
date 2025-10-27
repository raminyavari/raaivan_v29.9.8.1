USE [EKM_App]
GO

DECLARE @str varchar(max) = N'INSERT INTO [dbo].[FG_SelectedItems] (ApplicationID, ElementID, SelectedID, ' +
	'LastModifierUserID, LastModificationDate, Deleted) ' +
'SELECT E.ApplicationID, E.ElementID, E.GuidValue, ' +
	'ISNULL(E.LastModifierUserID, E.CreatorUserID), ISNULL(E.LastModificationDate, E.CreationDate), E.Deleted ' +
'FROM [dbo].[FG_InstanceElements] AS E ' +
	'LEFT JOIN [dbo].[FG_SelectedItems] AS G ' +
	'ON G.ApplicationID = E.ApplicationID AND ' +
		'G.ElementID = E.ElementID AND G.SelectedID = E.GuidValue ' +
'WHERE E.GuidValue IS NOT NULL AND E.CreatorUserID IS NOT NULL AND ' +
	'E.CreationDate IS NOT NULL AND G.ElementID IS NULL';

EXEC (@str)

GO

ALTER TABLE [dbo].[FG_InstanceElements]
DROP COLUMN GuidValue
GO

ALTER TABLE [dbo].[FG_Changes]
DROP COLUMN GuidValue
GO