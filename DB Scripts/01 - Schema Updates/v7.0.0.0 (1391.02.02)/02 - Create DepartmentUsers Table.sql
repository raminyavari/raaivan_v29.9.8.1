USE [EKM_App]
GO

/****** Object:  Table [dbo].[OrganStructUsers]    Script Date: 04/18/2012 19:53:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DepartmentUsers](
	[UserID] [uniqueidentifier] NOT NULL,
	[DepartmentID] [uniqueidentifier] NOT NULL,
	[UserType] [nvarchar](255) NULL,
 CONSTRAINT [PK_DepartmentUsers] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[DepartmentUsers]  WITH CHECK ADD  CONSTRAINT [FK_DepartmentUsers_ProfileCommon] FOREIGN KEY([UserID])
REFERENCES [dbo].[ProfileCommon] ([UserId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[DepartmentUsers] CHECK CONSTRAINT [FK_DepartmentUsers_ProfileCommon]
GO

ALTER TABLE [dbo].[DepartmentUsers]  WITH NOCHECK ADD  CONSTRAINT [FK_DepartmentUsers_Departments] FOREIGN KEY([DepartmentID])
REFERENCES [dbo].[Departments] ([DepartmentID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[DepartmentUsers] CHECK CONSTRAINT [FK_DepartmentUsers_Departments]
GO


INSERT INTO [dbo].[DepartmentUsers]
           ([DepartmentID]
			,[UserID]
			,[UserType])
SELECT  dbo.Departments.DepartmentID, dbo.OrganStructUsers.UserID, 
		dbo.OrganStructUsers.UserType
FROM    dbo.Departments INNER JOIN
        dbo.OrganStructUsers ON dbo.Departments.ID = dbo.OrganStructUsers.OrganStructID
WHERE   dbo.OrganStructUsers.UserID > '00000000-0000-0000-0000-000000000000' AND dbo.OrganStructUsers.OrganStructID > 0
GO
