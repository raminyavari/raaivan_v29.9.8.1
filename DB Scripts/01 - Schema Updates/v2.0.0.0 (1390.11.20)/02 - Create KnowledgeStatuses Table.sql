USE [EKM_APP]
GO

/****** Object:  Table [dbo].[KnowledgeStatuses]    Script Date: 12/16/2011 23:20:07 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[KWF_Statuses](
	[StatusID] [bigint] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[PersianName] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[Deleted] [bit] NULL,
 CONSTRAINT [PK_KnowledgeStatuses] PRIMARY KEY CLUSTERED 
(
	[StatusID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_PADDING OFF
GO


