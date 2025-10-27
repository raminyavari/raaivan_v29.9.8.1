USE [EKM_App]
GO


ALTER TABLE [dbo].[MSG_Messages] CHECK CONSTRAINT [FK_MSG_Messages_aspnet_Users]
GO