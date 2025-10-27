USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TABLE [dbo].[RV_EmailQueue](
	[ID] [bigint] IDENTITY(1, 1) NOT NULL,
	[SenderUserID] [uniqueidentifier] NULL,
	[Action] [varchar](50) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[Title] [nvarchar](1000) NULL,
	[EmailBody] [nvarchar](max) NOT NULL
 CONSTRAINT [PK_RV_EmailQueue] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

