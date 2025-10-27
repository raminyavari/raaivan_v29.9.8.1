USE [EKM_App]
GO

/****** Object:  Table [dbo].[OrganStructUsers]    Script Date: 04/18/2012 19:53:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[KnowledgeDepartments](
	[KnowledgeID] [uniqueidentifier] NOT NULL,
	[DepartmentID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_KnowledgeDepartments] PRIMARY KEY CLUSTERED 
(
	[KnowledgeID] ASC,
	[DepartmentID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[KnowledgeDepartments]  WITH CHECK ADD  CONSTRAINT [FK_KnowledgeDepartments_Knowledges] FOREIGN KEY([KnowledgeID])
REFERENCES [dbo].[KKnowledges] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[KnowledgeDepartments] CHECK CONSTRAINT [FK_KnowledgeDepartments_Knowledges]
GO

ALTER TABLE [dbo].[KnowledgeDepartments]  WITH CHECK ADD  CONSTRAINT [FK_KnowledgeDepartments_Departments] FOREIGN KEY([DepartmentID])
REFERENCES [dbo].[Departments] ([DepartmentID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[KnowledgeDepartments] CHECK CONSTRAINT [FK_KnowledgeDepartments_Departments]
GO


INSERT INTO [dbo].[KnowledgeDepartments]
           ([KnowledgeID]
            ,[DepartmentID])
SELECT  dbo.KKnowledgeOrganStructs.KKnowledgeID, dbo.Departments.DepartmentID
FROM    dbo.Departments INNER JOIN
        dbo.KKnowledgeOrganStructs ON dbo.Departments.ID = dbo.KKnowledgeOrganStructs.OrganStructID
WHERE   dbo.KKnowledgeOrganStructs.KKnowledgeID > '00000000-0000-0000-0000-000000000000' AND dbo.KKnowledgeOrganStructs.OrganStructID > 0
GO
