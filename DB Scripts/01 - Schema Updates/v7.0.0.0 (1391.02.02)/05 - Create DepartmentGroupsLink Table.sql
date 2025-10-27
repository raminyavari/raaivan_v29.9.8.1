USE [EKM_App]
GO

/****** Object:  Table [dbo].[OrganStructUsers]    Script Date: 04/18/2012 19:53:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DepartmentGroupsLink](
	[GroupID] [uniqueidentifier] NOT NULL,
	[DepartmentID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_DepartmentGroupsLink] PRIMARY KEY CLUSTERED 
(
	[GroupID] ASC,
	[DepartmentID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[DepartmentGroupsLink]  WITH CHECK ADD  CONSTRAINT [FK_DepartmentGroupsLink_DepartmentGroups] FOREIGN KEY([GroupID])
REFERENCES [dbo].[DepartmentGroups] ([ID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[DepartmentGroupsLink] CHECK CONSTRAINT [FK_DepartmentGroupsLink_DepartmentGroups]
GO

ALTER TABLE [dbo].[DepartmentGroupsLink]  WITH CHECK ADD  CONSTRAINT [FK_DepartmentGroupsLink_Departments] FOREIGN KEY([DepartmentID])
REFERENCES [dbo].[Departments] ([DepartmentID])
GO

ALTER TABLE [dbo].[DepartmentGroupsLink] CHECK CONSTRAINT [FK_DepartmentGroupsLink_Departments]
GO


INSERT INTO [dbo].[DepartmentGroupsLink]
           ([GroupID]
            ,[DepartmentID])
SELECT  dbo.DepartmentsGroupsLink.GroupID, dbo.Departments.DepartmentID
FROM    dbo.Departments INNER JOIN
        dbo.DepartmentsGroupsLink ON dbo.Departments.ID = dbo.DepartmentsGroupsLink.DepartmentID
GO


