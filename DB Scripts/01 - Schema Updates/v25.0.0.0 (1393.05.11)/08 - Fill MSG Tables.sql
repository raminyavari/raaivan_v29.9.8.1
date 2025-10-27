USE [EKM_App]
GO


INSERT [dbo].[MSG_Messages](
	MessageID,
	Title,
	MessageText,
	SenderUserID,
	SendDate,
	ForwardedFrom,
	HasAttachment
)
SELECT
	MSG.ID,
	MSG.[Subject],
	MSG.[FullText],
	MSG.[SenderUserID],
	MSG.DateSent,
	MSG.ParentId,
	CASE WHEN Ref.ID IS NULL THEN 0 ELSE 1 END
FROM [dbo].[Messages] AS MSG
	LEFT JOIN (
		SELECT MSG.ID
		FROM [dbo].[Messages] AS MSG
			INNER JOIN [dbo].[Attachments] AS AT
			ON AT.ObjectID = MSG.ID
		GROUP BY MSG.ID
	) AS Ref
	ON Ref.ID = MSG.ID
WHERE MSG.DateSent IS NOT NULL
GO



INSERT [dbo].[MSG_MessageDetails](
	UserID,
	ThreadID,
	MessageID,
	Seen,
	ViewDate,
	IsSender,
	IsGroup,
	Deleted
)
SELECT	Ref.UserID,
		Ref.ThreadID,
		Ref.MessageID,
		Ref.Seen,
		Ref.ViewDate,
		Ref.IsSender,
		Ref.IsGroup,
		Ref.Deleted
FROM (
		SELECT	U.ReceiverUserId AS UserID,
				M.SenderUserId AS ThreadID,
				M.ID AS MessageID,
				CASE
					WHEN U.DateFirstRead IS NULL THEN 0 ELSE 1
				END AS Seen,
				U.DateFirstRead AS ViewDate,
				0 AS IsSender,
				0 AS IsGroup,
				0 AS Deleted,
				M.DateSent AS SendDate
		FROM [dbo].[Messages] AS M
			INNER JOIN [dbo].[MessageUsers] AS U
			ON U.MessageId = M.ID
			
		UNION ALL
			
		SELECT	M.SenderUserId AS UserID,
				U.ReceiverUserId AS ThreadID,
				M.ID AS MessageID,
				1 AS Seen,
				NULL AS ViewDate,
				1 AS IsSender,
				0 AS IsGroup,
				0 AS Deleted,
				M.DateSent AS SendDate
		FROM [dbo].[Messages] AS M
			INNER JOIN [dbo].[MessageUsers] AS U
			ON U.MessageId = M.ID
	) AS Ref
WHERE Ref.MessageID IN (SELECT M.MessageID FROM [dbo].[MSG_Messages] AS M)
ORDER BY Ref.SendDate ASC, IsSender DESC

GO