USE [EKM_APP]
GO


SET ANSI_PADDING ON
GO

ALTER TABLE [dbo].[PRVC_PrivacyType]
ADD [ConfidentialityID] [uniqueidentifier] NULL
GO

ALTER TABLE [dbo].[PRVC_PrivacyType]
ADD [CalculateHierarchy] [bit] NULL
GO

EXEC ('DELETE [dbo].[PRVC_PrivacyType] WHERE Deleted = 1')
GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[PRVC_PrivacyType]
DROP COLUMN [Deleted]
GO


ALTER TABLE [dbo].[PRVC_PrivacyType]  WITH CHECK ADD  CONSTRAINT [FK_PRVC_PrivacyType_PRVC_ConfidentialityLevels] FOREIGN KEY([ConfidentialityID])
REFERENCES [dbo].[PRVC_ConfidentialityLevels] ([ID])
GO

ALTER TABLE [dbo].[PRVC_PrivacyType] CHECK CONSTRAINT [FK_PRVC_PrivacyType_PRVC_ConfidentialityLevels]
GO


UPDATE T
	SET ConfidentialityID = C.LevelID
FROM [dbo].[PRVC_PrivacyType] AS T
	INNER JOIN [dbo].[PRVC_Confidentialities] AS C
	ON C.ApplicationID = T.ApplicationID AND C.ItemID = T.ObjectID
GO


INSERT INTO [dbo].[PRVC_PrivacyType] (ApplicationID, ObjectID, ConfidentialityID, PrivacyType, 
	CreatorUserID, CreationDate, LastModifierUserID, LastModificationDate)
SELECT C.ApplicationID, C.ItemID, C.LevelID, N'', C.CreatorUserID, C.CreationDate,
	C.LastModifierUserID, C.LastModificationDate
FROM [dbo].[PRVC_PrivacyType] AS P
	RIGHT JOIN (
		SELECT	X.ApplicationID, 
				X.ItemID,
				CAST(MAX(CAST(X.LevelID AS varchar(50))) AS uniqueidentifier) AS LevelID,
				CAST(MAX(CAST(X.CreatorUserID AS varchar(50))) AS uniqueidentifier) AS CreatorUserID,
				MAX(X.CreationDate) AS CreationDate,
				CAST(MAX(CAST(X.LastModifierUserID AS varchar(50))) AS uniqueidentifier) AS LastModifierUserID,
				MAX(X.LastModificationDate) AS LastModificationDate
		FROM [dbo].[PRVC_Confidentialities] AS X
		GROUP BY X.ApplicationID, X.ItemID
	) AS C
	ON C.ApplicationID = P.ApplicationID AND C.ItemID = P.ObjectID
WHERE P.ObjectID IS NULL

GO