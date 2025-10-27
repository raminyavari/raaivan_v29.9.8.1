USE [EKM_App]
GO

/****** Object:  Table [dbo].[OrganStructs]    Script Date: 04/18/2012 19:15:34 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER TABLE [dbo].[OrganStructs]
ADD [DepartmentID] [uniqueidentifier] NULL
GO

UPDATE [dbo].[OrganStructs]
   SET [DepartmentID] = NEWID()
GO

UPDATE [dbo].[OrganStructs]
   SET [ParentID] = NULL
   WHERE NOT EXISTS 
     (SELECT * FROM [dbo].[OrganStructs] AS OS1 WHERE OS1.ID = [dbo].[OrganStructs].[ParentID])
GO


CREATE TABLE [dbo].[Departments](
	[DepartmentID] [uniqueidentifier] NOT NULL,
	[AdditionalID] [nvarchar](50) NULL,
	[Title] [nvarchar](255) NOT NULL,
	[ParentID] [uniqueidentifier] NULL,
	[ID] [bigint] NULL, /* To be removed */
	[OldParentID] [bigint] NULL, /* To be removed */
 CONSTRAINT [PK_Departments] PRIMARY KEY CLUSTERED 
(
	[DepartmentID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Departments]  WITH NOCHECK ADD  CONSTRAINT [FK_Departments_Departments] FOREIGN KEY([ParentID])
REFERENCES [dbo].[Departments] ([DepartmentID])
GO

ALTER TABLE [dbo].[Departments] CHECK CONSTRAINT [FK_Departments_Departments]
GO


INSERT INTO [dbo].[Departments]
           ([DepartmentID]
           ,[AdditionalID]
           ,[Title]
           ,[ID]
           ,[OldParentID])
SELECT dbo.OrganStructs.DepartmentID, dbo.OrganStructs.AdditionalID, dbo.OrganStructs.Title,
	   dbo.OrganStructs.ID, dbo.OrganStructs.ParentID
FROM dbo.OrganStructs
GO

DELETE FROM [dbo].[Departments]
      WHERE dbo.Departments.OldParentID > 0
GO

INSERT INTO [dbo].[Departments]
           ([DepartmentID]
           ,[AdditionalID]
           ,[Title]
           ,[ParentID]
           ,[ID])
SELECT dbo.OrganStructs.DepartmentID, dbo.OrganStructs.AdditionalID, dbo.OrganStructs.Title,
	   Departments_1.DepartmentID, dbo.OrganStructs.ID
FROM dbo.OrganStructs INNER JOIN
     dbo.OrganStructs AS Departments_1 ON dbo.OrganStructs.ParentID = Departments_1.ID
GO


ALTER TABLE [dbo].[OrganStructs]
DROP COLUMN [DepartmentID]
GO