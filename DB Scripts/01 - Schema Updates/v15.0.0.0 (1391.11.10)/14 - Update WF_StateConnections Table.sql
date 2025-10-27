USE EKM_App
GO


UPDATE [dbo].[WF_StateConnections]
	SET NodeTypeID = NULL
WHERE NodeTypeID = N'00000000-0000-0000-0000-000000000000'
GO


ALTER TABLE [dbo].[WF_StateConnections]  WITH CHECK ADD  CONSTRAINT [FK_WF_StateConnections_CN_NodeTypes] FOREIGN KEY([NodeTypeID])
REFERENCES [dbo].[CN_NodeTypes] ([NodeTypeID])
GO

ALTER TABLE [dbo].[WF_StateConnections] CHECK CONSTRAINT [FK_WF_StateConnections_CN_NodeTypes]
GO