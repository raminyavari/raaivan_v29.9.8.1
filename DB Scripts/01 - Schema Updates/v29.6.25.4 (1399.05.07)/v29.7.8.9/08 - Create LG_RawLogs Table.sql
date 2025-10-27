USE [EKM_App]
GO

/****** Object:  Table [dbo].[LG_Logs]    Script Date: 8/7/2021 4:30:42 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TABLE [dbo].[LG_RawLogs](
	[LogID] [bigint] IDENTITY(1,1) NOT NULL,
	[UserID] [uniqueidentifier] NULL,
	[ApplicationID] [uniqueidentifier] NULL,
	[Date] [datetime] NOT NULL,
	[Info] [nvarchar](max) NULL
 CONSTRAINT [PK_LG_RawLogs] PRIMARY KEY CLUSTERED 
(
	[LogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[LG_RawLogs]  WITH CHECK ADD  CONSTRAINT [FK_LG_RawLogs_aspnet_Applications] FOREIGN KEY([ApplicationID])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO

ALTER TABLE [dbo].[LG_RawLogs] CHECK CONSTRAINT [FK_LG_RawLogs_aspnet_Applications]
GO

ALTER TABLE [dbo].[LG_RawLogs]  WITH CHECK ADD  CONSTRAINT [FK_LG_RawLogs_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserID])
GO

ALTER TABLE [dbo].[LG_RawLogs] CHECK CONSTRAINT [FK_LG_RawLogs_aspnet_Users]
GO


