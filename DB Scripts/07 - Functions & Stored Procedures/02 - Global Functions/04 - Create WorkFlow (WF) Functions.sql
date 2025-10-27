USE [EKM_App]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WF_FN_GetDashboardInfo]') 
    AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[WF_FN_GetDashboardInfo]
GO

CREATE FUNCTION [dbo].[WF_FN_GetDashboardInfo](
	@WorkFlowName		nvarchar(1000),
	@StateTitle			nvarchar(1000),
	@DataNeedInstanceID	uniqueidentifier
)
RETURNS nvarchar(max)
WITH ENCRYPTION
AS
BEGIN
	RETURN '{"WorkFlowName":"' + ISNULL([dbo].[GFN_Base64encode](@WorkFlowName), N'') + 
		'","WorkFlowState":"' + ISNULL([dbo].[GFN_Base64encode](@StateTitle), N'') +
		(
			CASE
				WHEN @DataNeedInstanceID IS NULL THEN N''
				ELSE '","DataNeedInstanceID":"' + CAST(@DataNeedInstanceID AS varchar(50))
			END
		) +
		'"}'
END

GO
