USE [EKM_App]
GO

/****** Object:  Table [dbo].[Phrases]    Script Date: 04/26/2013 20:38:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TABLE [dbo].[NTFN_Dashboards](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[NodeID] [uniqueidentifier] NOT NULL,
	[RefItemID] [uniqueidentifier] NOT NULL,
	[Type] [varchar](20) NOT NULL,
	[Info] [nvarchar](max) NULL,
	[Removable] [bit] NOT NULL,
	[SenderUserID] [uniqueidentifier] NULL,
	[SendDate] [datetime] NOT NULL,
	[ExpirationDate] [datetime] NULL,
	[Seen] [bit] NOT NULL,
	[ViewDate] [datetime] NULL,
	[Done] [bit] NOT NULL,
	[ActionDate] [datetime] NULL,
	[Deleted] [bit] NOT NULL
 CONSTRAINT [PK_NTFN_Dashboards] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[NTFN_Dashboards]  WITH CHECK ADD  CONSTRAINT [FK_NTFN_Dashboards_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[NTFN_Dashboards] CHECK CONSTRAINT [FK_NTFN_Dashboards_aspnet_Users]
GO