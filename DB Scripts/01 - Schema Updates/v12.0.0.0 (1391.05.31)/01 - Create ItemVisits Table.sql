USE [EKM_App]

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE TABLE [dbo].[USR_ItemVisits](
	[ItemID] [uniqueidentifier] NOT NULL,
	[VisitDate] [datetime] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[ItemType] [nvarchar](255) NOT NULL
 CONSTRAINT [PK_USR_ItemVisits] PRIMARY KEY CLUSTERED 
(
	[ItemID] ASC,
	[VisitDate] ASC,
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [dbo].[USR_ItemVisits]  WITH CHECK ADD  CONSTRAINT [FK_USR_ItemVisits_aspnet_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[USR_ItemVisits] CHECK CONSTRAINT [FK_USR_ItemVisits_aspnet_Users]
GO