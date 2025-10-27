USE [EKM_App]
GO

--1
ALTER TABLE [dbo].[USR_Invitations]
DROP CONSTRAINT [FK_USR_Invitations_aspnet_Users_Created]

--2
--Changed Farzane
ALTER TABLE [dbo].[USR_Invitations]  WITH CHECK ADD  CONSTRAINT [FK_USR_Invitations_USR_TemporaryUsers_Created] FOREIGN KEY([CreatedUserID])
REFERENCES [dbo].[USR_TemporaryUsers] ([UserId])
GO

ALTER TABLE [dbo].[USR_Invitations] CHECK CONSTRAINT [FK_USR_Invitations_USR_TemporaryUsers_Created]
GO
--End of Changed Farzane
