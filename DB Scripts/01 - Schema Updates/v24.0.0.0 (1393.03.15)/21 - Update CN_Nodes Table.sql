USE [EKM_App]
GO

/****** Object:  Table [dbo].[Phrases]    Script Date: 04/26/2013 20:38:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER TABLE [dbo].[CN_Nodes]
ADD [AreaID] [uniqueidentifier] NULL
GO


ALTER TABLE [dbo].[CN_Nodes]  WITH CHECK ADD  CONSTRAINT [FK_CN_Nodes_CN_Nodes_Area] FOREIGN KEY([AreaID])
REFERENCES [dbo].[CN_Nodes] ([NodeID])
GO

ALTER TABLE [dbo].[CN_Nodes] CHECK CONSTRAINT [FK_CN_Nodes_CN_Nodes_Area]
GO