USE [EKM_App]
GO

/****** Object:  Table [dbo].[OrganStructUsers]    Script Date: 04/18/2012 19:53:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CNDP_NodeDepartments](
	[NodeID] [uniqueidentifier] NOT NULL,
	[DepartmentID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_CNDP_NodeDepartments] PRIMARY KEY CLUSTERED 
(
	[NodeID] ASC,
	[DepartmentID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[CNDP_NodeDepartments]  WITH CHECK ADD  CONSTRAINT [FK_CNDP_NodeDepartments_CN_Nodes] FOREIGN KEY([NodeID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[CNDP_NodeDepartments] CHECK CONSTRAINT [FK_CNDP_NodeDepartments_CN_Nodes]
GO

ALTER TABLE [dbo].[CNDP_NodeDepartments]  WITH CHECK ADD  CONSTRAINT [FK_CNDP_NodeDepartments_Departments] FOREIGN KEY([DepartmentID])
REFERENCES [dbo].[Departments] ([DepartmentID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[CNDP_NodeDepartments] CHECK CONSTRAINT [FK_CNDP_NodeDepartments_Departments]
GO


INSERT INTO [dbo].[CNDP_NodeDepartments]
           ([NodeID]
			,[DepartmentID])
SELECT  dbo.NodeOrganStructs.NodeID, dbo.Departments.DepartmentID
FROM    dbo.Departments INNER JOIN
        dbo.NodeOrganStructs ON dbo.Departments.ID = dbo.NodeOrganStructs.DepartmentID
GO
