USE [EKM_App]
GO

/****** Object:  Table [dbo].[WF_History]    Script Date: 08/17/2013 16:07:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LG_Logs](
	[LogID] [bigint] IDENTITY(1,1) NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[HostAddress] [varchar](50) NULL,
	[HostName] [nvarchar](255) NULL,
	[Action] [varchar](100) NOT NULL,
	[SubjectID] [uniqueidentifier] NULL,
	[SecondSubjectID] [uniqueidentifier] NULL,
	[ThirdSubjectID] [uniqueidentifier] NULL,
	[FourthSubjectID] [uniqueidentifier] NULL,
	[Date] [datetime] NOT NULL,
	[Info] [nvarchar](max) NULL,
	[ModuleIdentifier] [varchar](20) NULL
 CONSTRAINT [PK_LG_Logs] PRIMARY KEY CLUSTERED 
(
	[LogID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
