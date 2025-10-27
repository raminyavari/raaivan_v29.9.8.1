USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[USR_ActiveUsersReport]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[USR_ActiveUsersReport]
GO

CREATE PROCEDURE [dbo].[USR_ActiveUsersReport]
	@ApplicationID	uniqueidentifier,
	@CurrentUserID	uniqueidentifier,
	@BeginDate		datetime,
	@FinishDate		datetime
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON

	
	SELECT	UN.UserID AS UserID_Hide,
			UN.UserName,
			LTRIM(RTRIM(ISNULL(UN.FirstName, N'') + N' ' + ISNULL(UN.LastName, N''))) AS FullName
	FROM ( 
			SELECT DISTINCT Ref.UserID
			FROM [dbo].[USR_FN_ActiveUsersReport](@ApplicationID, @BeginDate, @FinishDate) AS Ref
		) AS X
		INNER JOIN [dbo].[Users_Normal] AS UN
		ON UN.ApplicationID = @ApplicationID AND UN.UserID = X.UserID

	SELECT ('{' +
			'"FullName": {"Action": "Link", "Type": "User",' +
				'"Requires": {"UID": "UserID_Hide"}' +
			'}' +
		   '}') AS Actions
END

GO

