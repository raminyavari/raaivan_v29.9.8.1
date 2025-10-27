USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[USR_ActiveUsersReport_Chart]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[USR_ActiveUsersReport_Chart]
GO

CREATE PROCEDURE [dbo].[USR_ActiveUsersReport_Chart]
	@ApplicationID	uniqueidentifier,
	@CurrentUserID	uniqueidentifier,
	@Period			varchar(50),
	@CalendarType	varchar(50),
	@PeriodList		BigIntTableType readonly,
	@BeginDate		datetime,
	@FinishDate		datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

	SELECT P.[Value] AS [Period], ISNULL(COUNT(DISTINCT X.UserID), 0) AS TotalCount
	FROM @PeriodList AS P
		LEFT JOIN (
			SELECT Ref.UserID, [dbo].[GFN_GetTimePeriod](Ref.[Date], @Period, @CalendarType) AS [Period]
			FROM [dbo].[USR_FN_ActiveUsersReport](@ApplicationID, @BeginDate, @FinishDate) AS Ref
		) AS X
		ON X.[Period] = P.[Value]
	GROUP BY P.[Value]
	ORDER BY P.[Value]
END

GO

