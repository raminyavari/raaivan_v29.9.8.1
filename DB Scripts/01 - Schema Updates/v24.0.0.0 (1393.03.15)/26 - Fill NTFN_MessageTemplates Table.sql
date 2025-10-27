USE [EKM_App]
GO



INSERT INTO [dbo].[NTFN_MessageTemplates](
	TemplateID,
	OwnerID,
	BodyText,
	AudienceType,
	AudienceRefOwnerID,
	AudienceNodeID,
	AudienceNodeAdmin,
	CreatorUserID,
	CreationDate,
	LastModifierUserID,
	LastModificationDate,
	Deleted
)
SELECT	AutoMessageID,
		OwnerID,
		BodyText,
		AudienceType,
		RefStateID,
		NodeID,
		[Admin],
		CreatorUserID,
		CreationDate,
		LastModifierUserID,
		LastModificationDate,
		Deleted
FROM [dbo].[WF_AutoMessages]

GO


UPDATE [dbo].[NTFN_MessageTemplates]
	SET AudienceType = N'Creator'
WHERE AudienceType = N'SendToOwner'

GO

UPDATE [dbo].[NTFN_MessageTemplates]
	SET AudienceType = N'RefOwner'
WHERE AudienceType = N'RefState'

GO