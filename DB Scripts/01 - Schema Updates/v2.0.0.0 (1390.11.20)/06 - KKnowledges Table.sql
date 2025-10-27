USE [EKM_App]
GO

/****** Object:  Table [dbo].[KKnowledges]    Script Date: 01/31/2012 16:04:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER TABLE [dbo].[KKnowledges]
ADD [StatusID] [bigint] NULL;
GO

ALTER TABLE [dbo].[KKnowledges]
ADD [PublicationDate] [datetime] NULL;
GO

ALTER TABLE [dbo].[KKnowledges]  WITH CHECK ADD  CONSTRAINT [FK_KKnowledges_aspnet_Users] FOREIGN KEY([OwnerUserID])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO

ALTER TABLE [dbo].[KKnowledges] CHECK CONSTRAINT [FK_KKnowledges_aspnet_Users]
GO

ALTER TABLE [dbo].[KKnowledges]  WITH CHECK ADD  CONSTRAINT [FK_KKnowledges_KWF_Statuses] FOREIGN KEY([StatusID])
REFERENCES [dbo].[KWF_Statuses] ([StatusID])
GO

ALTER TABLE [dbo].[KKnowledges] CHECK CONSTRAINT [FK_KKnowledges_KWF_Statuses]
GO

ALTER TABLE [dbo].[KKnowledges]
DROP COLUMN [OwnerName]
GO

ALTER TABLE [dbo].[KKnowledges]
DROP COLUMN [OwnerFamily]
GO

ALTER TABLE [dbo].[KKnowledges]
DROP COLUMN [OwnerJobTitle]
GO

ALTER TABLE [dbo].[KKnowledges]
DROP COLUMN [DepUserID]
GO

/* Update 'StatusID' Column */
UPDATE [dbo].[KKnowledges]
   SET [StatusID] = 1
 WHERE Status = 'Personal'
GO

UPDATE [dbo].[KKnowledges]
   SET [StatusID] = 2
 WHERE Status = 'DepartmentEvaluation'
GO

UPDATE [dbo].[KKnowledges]
   SET [StatusID] = 3
 WHERE Status = 'SendBackForRevision'
GO

UPDATE [dbo].[KKnowledges]
   SET [StatusID] = 4
 WHERE Status = 'ExpertEvaluation'
GO

UPDATE [dbo].[KKnowledges]
   SET [StatusID] = 5
 WHERE Status = 'Reject'
GO

UPDATE [dbo].[KKnowledges]
   SET [StatusID] = 6
 WHERE Status = 'Accept' OR
	   Status = 'SpecifyEventualState' OR
	   Status = 'ConnectionConfirmationToTreeContents' OR
	   Status = 'ConnectionConfirmationToTreeContentsAndExpertEvaluation' OR
	   Status = 'ConnectionConfirmationToTreeContentsAndAcceptedByExperts' OR
	   Status = 'ConnectionConfirmationToTreeContentsAndRejectedByExperts'
GO

/* Drop 'Status' Column */
ALTER TABLE [dbo].[KKnowledges]
DROP COLUMN [Status]
GO

