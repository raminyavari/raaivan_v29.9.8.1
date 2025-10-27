USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[USR_FN_ActiveUsersReport]') 
    AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[USR_FN_ActiveUsersReport]
GO

CREATE FUNCTION [dbo].[USR_FN_ActiveUsersReport](
	@ApplicationID	uniqueidentifier,
	@BeginDate		datetime,
	@FinishDate		datetime
)
RETURNS @outputTable TABLE (
	UserID	uniqueidentifier,
	[Date]	datetime
)
WITH ENCRYPTION
AS
BEGIN
	INSERT INTO @outputTable (UserID, [Date])
	SELECT LG.UserID, LG.[Date]
	FROM [dbo].[LG_Logs] AS LG
	WHERE LG.ApplicationID = @ApplicationID AND LG.[Action] = 'Login' AND
		(@BeginDate IS NULL OR LG.[Date] >= @BeginDate) AND
		(@FinishDate IS NULL OR LG.[Date] < @FinishDate)

	RETURN
END

GO
