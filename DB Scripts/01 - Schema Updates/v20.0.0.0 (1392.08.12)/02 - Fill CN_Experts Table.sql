USE [EKM_App]
GO

/****** Object:  Table [dbo].[Phrases]    Script Date: 04/26/2013 20:38:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

INSERT INTO [dbo].[CN_Experts](
	NodeID,
	UserID,
	Approved,
	ReferralsCount,
	ConfirmsPercentage,
	SocialApproved
)
SELECT NodeID, UserID, 1, 0, 0, 0
FROM [dbo].[KW_Experts]

GO

