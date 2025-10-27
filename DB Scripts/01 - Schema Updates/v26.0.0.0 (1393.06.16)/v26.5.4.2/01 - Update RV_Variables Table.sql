USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[RV_TMPVariables](
	[Name] [varchar](50) NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NOT NULL,
 CONSTRAINT [PK_RV_TMPVariables] PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[RV_TMPVariables]
SELECT * FROM [dbo].[RV_Variables]
GO

DROP TABLE [dbo].[RV_Variables]
GO

CREATE TABLE [dbo].[RV_Variables](
	[Name] [varchar](100) NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
	[LastModifierUserID] [uniqueidentifier] NULL,
	[LastModificationDate] [datetime] NOT NULL,
 CONSTRAINT [PK_RV_Variables] PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[RV_Variables]
SELECT * FROM [dbo].[RV_TMPVariables]
GO

DROP TABLE [dbo].[RV_TMPVariables]
GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[RV_Variables]  WITH CHECK ADD  CONSTRAINT [FK_RV_Variables_aspnet_Users] FOREIGN KEY([LastModifierUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[RV_Variables] CHECK CONSTRAINT [FK_RV_Variables_aspnet_Users]
GO


