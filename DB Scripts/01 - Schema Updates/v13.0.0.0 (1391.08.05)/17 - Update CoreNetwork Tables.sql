USE [EKM_App]
GO


ALTER TABLE [dbo].[CN_Nodes]
ADD [Tags] [nvarchar](2000) NULL
GO

DROP VIEW [dbo].[CN_View_Nodes_Normal]
GO

ALTER TABLE [dbo].[CN_Nodes]
ADD [TempAID] [nvarchar](50) NULL
GO

UPDATE [dbo].[CN_Nodes]
	SET TempAID = AdditionalID
GO

ALTER TABLE [dbo].[CN_Nodes]
DROP COLUMN [AdditionalID] 
GO

ALTER TABLE [dbo].[CN_Nodes]
ADD [AdditionalID] [nvarchar](50) NULL
GO

UPDATE [dbo].[CN_Nodes]
	SET AdditionalID = TempAID
GO

ALTER TABLE [dbo].[CN_Nodes]
DROP COLUMN [TempAID] 
GO



-- Node Types

ALTER TABLE [dbo].[CN_NodeTypes]
ADD [TempAID] [nvarchar](50) NULL
GO

UPDATE [dbo].[CN_NodeTypes]
	SET TempAID = AdditionalID
GO

ALTER TABLE [dbo].[CN_NodeTypes]
DROP COLUMN [AdditionalID] 
GO

ALTER TABLE [dbo].[CN_NodeTypes]
ADD [AdditionalID] [nvarchar](50) NULL
GO

UPDATE [dbo].[CN_NodeTypes]
	SET AdditionalID = TempAID
GO

ALTER TABLE [dbo].[CN_NodeTypes]
DROP COLUMN [TempAID] 
GO


-- Lists

IF EXISTS(select * FROM sys.views where name = 'CN_View_Lists')
DROP VIEW [dbo].[CN_View_Lists]
GO

ALTER TABLE [dbo].[CN_Lists]
ADD [TempAID] [nvarchar](50) NULL
GO

UPDATE [dbo].[CN_Lists]
	SET TempAID = AdditionalID
GO

ALTER TABLE [dbo].[CN_Lists]
DROP COLUMN [AdditionalID] 
GO

ALTER TABLE [dbo].[CN_Lists]
ADD [AdditionalID] [nvarchar](50) NULL
GO

UPDATE [dbo].[CN_Lists]
	SET AdditionalID = TempAID
GO

ALTER TABLE [dbo].[CN_Lists]
DROP COLUMN [TempAID] 
GO


-- List Types

ALTER TABLE [dbo].[CN_ListTypes]
ADD [TempAID] [nvarchar](50) NULL
GO

UPDATE [dbo].[CN_ListTypes]
	SET TempAID = AdditionalID
GO

ALTER TABLE [dbo].[CN_ListTypes]
DROP COLUMN [AdditionalID] 
GO

ALTER TABLE [dbo].[CN_ListTypes]
ADD [AdditionalID] [nvarchar](50) NULL
GO

UPDATE [dbo].[CN_ListTypes]
	SET AdditionalID = TempAID
GO

ALTER TABLE [dbo].[CN_ListTypes]
DROP COLUMN [TempAID] 
GO


-- Properties

IF EXISTS(select * FROM sys.views where name = 'CN_View_NodeRelations')
DROP VIEW [dbo].[CN_View_NodeRelations]
GO

ALTER TABLE [dbo].[CN_Properties]
ADD [TempAID] [nvarchar](50) NULL
GO

UPDATE [dbo].[CN_Properties]
	SET TempAID = AdditionalID
GO

ALTER TABLE [dbo].[CN_Properties]
DROP COLUMN [AdditionalID] 
GO

ALTER TABLE [dbo].[CN_Properties]
ADD [AdditionalID] [nvarchar](50) NULL
GO

UPDATE [dbo].[CN_Properties]
	SET AdditionalID = TempAID
GO

ALTER TABLE [dbo].[CN_Properties]
DROP COLUMN [TempAID] 
GO