USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- Update RelatedKnowledges
UPDATE [dbo].[KW_RelatedKnowledges]
	SET Deleted = 0
WHERE Deleted IS NULL

DECLARE @_KRK Table(firstV uniqueidentifier, secondV uniqueidentifier, del bit)
INSERT INTO @_KRK(firstV, secondV, del)
SELECT DISTINCT Ref.KnowledgeID, Ref.RelatedKnowledgeID, Ref.Deleted
FROM [dbo].[KW_RelatedKnowledges] AS Ref


DECLARE @_KRKRTID uniqueidentifier
SET @_KRKRTID = (SELECT PropertyID FROM [dbo].[CN_Properties]
	WHERE AdditionalID = '3')
		
INSERT INTO [dbo].[CN_NodeRelations](SourceNodeID, DestinationNodeID, PropertyID, Deleted)
SELECT Ref.firstV, Ref.secondV, @_KRKRTID, Ref.del
FROM @_KRK AS Ref
-- end of Update RelatedKnowledges

DROP TABLE [dbo].[KW_RelatedKnowledges]
GO

