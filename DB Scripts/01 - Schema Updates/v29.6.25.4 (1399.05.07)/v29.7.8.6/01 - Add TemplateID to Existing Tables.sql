USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER TABLE [dbo].[FG_ExtendedForms]
ADD TemplateFormID uniqueidentifier NULL
GO

ALTER TABLE [dbo].[FG_ExtendedForms]  WITH CHECK ADD  CONSTRAINT [FK_FG_ExtendedForms_FG_ExtendedForms_TemplateID] FOREIGN KEY([TemplateFormID])
REFERENCES [dbo].[FG_ExtendedForms] ([FormID])
GO

ALTER TABLE [dbo].[FG_ExtendedForms] CHECK CONSTRAINT [FK_FG_ExtendedForms_FG_ExtendedForms_TemplateID]
GO


ALTER TABLE [dbo].[FG_ExtendedFormElements]
ADD TemplateElementID uniqueidentifier NULL
GO

ALTER TABLE [dbo].[FG_ExtendedFormElements]  WITH CHECK ADD  CONSTRAINT [FK_FG_ExtendedFormElements_FG_ExtendedFormElements_TemplateID] FOREIGN KEY([TemplateElementID])
REFERENCES [dbo].[FG_ExtendedFormElements] ([ElementID])
GO

ALTER TABLE [dbo].[FG_ExtendedFormElements] CHECK CONSTRAINT [FK_FG_ExtendedFormElements_FG_ExtendedFormElements_TemplateID]
GO


ALTER TABLE [dbo].[CN_NodeTypes]
ADD TemplateTypeID uniqueidentifier NULL
GO

ALTER TABLE [dbo].[CN_NodeTypes]  WITH CHECK ADD  CONSTRAINT [FK_CN_NodeTypes_CN_NodeTypes_TemplateID] FOREIGN KEY([TemplateTypeID])
REFERENCES [dbo].[CN_NodeTypes] ([NodeTypeID])
GO

ALTER TABLE [dbo].[CN_NodeTypes] CHECK CONSTRAINT [FK_CN_NodeTypes_CN_NodeTypes_TemplateID]
GO