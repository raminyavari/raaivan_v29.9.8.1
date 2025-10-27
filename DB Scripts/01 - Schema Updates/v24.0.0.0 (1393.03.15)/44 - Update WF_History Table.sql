USE [EKM_App]
GO


DECLARE @HIDs Table (ID int IDENTITY(1,1) primary key clustered, HistoryID uniqueidentifier,
	WorkFlowID uniqueidentifier, OwnerID uniqueidentifier, SenderUserID uniqueidentifier)

INSERT INTO @HIDs(HistoryID, WorkFlowID, OwnerID, SenderUserID)
SELECT H.HistoryID, H.WorkFlowID, H.OwnerID, H.SenderUserID
FROM [dbo].[WF_History] AS H
ORDER BY H.SendDate ASC

DECLARE @Count int = (SELECT COUNT(*) FROM @HIDs)

WHILE @Count > 0 BEGIN
	DECLARE @HistoryID uniqueidentifier
	DECLARE @WorkFlowID uniqueidentifier
	DECLARE @OwnerID uniqueidentifier
	DECLARE @SenderUserID uniqueidentifier
	
	DECLARE @PrevHistoryID uniqueidentifier
	DECLARE @PrevWorkFlowID uniqueidentifier
	DECLARE @PrevOwnerID uniqueidentifier
	DECLARE @PrevSenderUserID uniqueidentifier
	
	SET @HistoryID = NULL
	SET @WorkFlowID = NULL
	SET @OwnerID = NULL
	SET @SenderUserID = NULL
	
	SET @PrevHistoryID = NULL
	SET @PrevWorkFlowID = NULL
	SET @PrevOwnerID = NULL
	SET @PrevSenderUserID = NULL
	
	SELECT TOP(1)
			@HistoryID = Ref.HistoryID,
			@WorkFlowID = Ref.WorkFlowID,
			@OwnerID = Ref.OwnerID,
			@SenderUserID = Ref.SenderUserID
	FROM @HIDs AS Ref
	WHERE ID = @Count
	
	SELECT TOP(1)
			@PrevHistoryID = Ref.HistoryID,
			@PrevWorkFlowID = Ref.WorkFlowID,
			@PrevOwnerID = Ref.OwnerID,
			@PrevSenderUserID = Ref.SenderUserID
	FROM @HIDs AS Ref
	WHERE ID < @Count AND Ref.WorkFlowID = @WorkFlowID AND Ref.OwnerID = @OwnerID
	ORDER BY Ref.ID DESC
	
	IF @PrevHistoryID IS NOT NULL AND @PrevSenderUserID IS NOT NULL BEGIN
		UPDATE [dbo].[WF_History]
			SET ActorUserID = @SenderUserID
		WHERE HistoryID = @PrevHistoryID
		
		UPDATE [dbo].[WF_History]
			SET PreviousHistoryID = @PrevHistoryID
		WHERE HistoryID = @HistoryID
	END

	SET @Count = @Count - 1
END

GO