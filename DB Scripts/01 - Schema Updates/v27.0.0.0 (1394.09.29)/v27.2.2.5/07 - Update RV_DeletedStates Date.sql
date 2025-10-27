USE [EKM_App]
GO


UPDATE DS
	SET [Date] = ISNULL(NR.CreationDate, N'2011-06-20 18:12:18.700')
FROM [dbo].[RV_DeletedStates] AS DS
	INNER JOIN [dbo].[CN_NodeRelations] AS NR
	ON NR.UniqueID = DS.ObjectID	
GO


UPDATE DS
	SET [Date] = ISNULL(ISNULL(NM.AcceptionDate, NM.MembershipDate), N'2011-06-20 18:12:18.700')
FROM [dbo].[RV_DeletedStates] AS DS
	INNER JOIN [dbo].[CN_NodeMembers] AS NM
	ON NM.UniqueID = DS.ObjectID	
GO


UPDATE DS
	SET [Date] = ISNULL(
		CASE 
			WHEN ND.CreationDate > M.CreateDate THEN ND.CreationDate 
			ELSE M.CreateDate 
		END, N'2011-06-20 18:12:18.700')
FROM [dbo].[RV_DeletedStates] AS DS
	INNER JOIN [dbo].[CN_Experts] AS EX
	ON EX.UniqueID = DS.ObjectID
	INNER JOIN [dbo].[CN_Nodes] AS ND
	ON ND.NodeID = EX.NodeID
	INNER JOIN [dbo].[aspnet_Membership] AS M
	ON M.UserID = EX.UserID
GO


UPDATE DS
	SET [Date] = ISNULL(M.CreateDate, N'2011-06-20 18:12:18.700')
FROM [dbo].[RV_DeletedStates] AS DS
	INNER JOIN [dbo].[aspnet_Membership] AS M
	ON M.UserID = DS.ObjectID
GO


UPDATE DS
	SET [Date] = ISNULL(EA.CreationDate, N'2011-06-20 18:12:18.700')
FROM [dbo].[RV_DeletedStates] AS DS
	INNER JOIN [dbo].[USR_EmailAddresses] AS EA
	ON EA.EmailID = DS.ObjectID
GO


UPDATE DS
	SET [Date] = ISNULL(NL.LikeDate, N'2011-06-20 18:12:18.700')
FROM [dbo].[RV_DeletedStates] AS DS
	INNER JOIN [dbo].[CN_NodeLikes] AS NL
	ON NL.UniqueID = DS.ObjectID
GO


UPDATE DS
	SET [Date] = ISNULL(IV.VisitDate, N'2011-06-20 18:12:18.700')
FROM [dbo].[RV_DeletedStates] AS DS
	INNER JOIN [dbo].[USR_ItemVisits] AS IV
	ON IV.UniqueID = DS.ObjectID
GO


UPDATE DS
	SET [Date] = ISNULL(NC.CreationDate, N'2011-06-20 18:12:18.700')
FROM [dbo].[RV_DeletedStates] AS DS
	INNER JOIN [dbo].[CN_NodeCreators] AS NC
	ON NC.UniqueID = DS.ObjectID
GO


UPDATE DS
	SET [Date] = ISNULL(ND.CreationDate, N'2011-06-20 18:12:18.700')
FROM [dbo].[RV_DeletedStates] AS DS
	INNER JOIN [dbo].[CN_Nodes] AS ND
	ON ND.NodeID = DS.ObjectID
GO


UPDATE DS
	SET [Date] = ISNULL(ISNULL(FR.AcceptionDate, FR.RequestDate), N'2011-06-20 18:12:18.700')
FROM [dbo].[RV_DeletedStates] AS DS
	INNER JOIN [dbo].[USR_Friends] AS FR
	ON FR.UniqueID = DS.ObjectID
GO


UPDATE DS
	SET [Date] = N'2015-06-20 18:12:18.700'
FROM [dbo].[RV_DeletedStates] AS DS
	INNER JOIN [dbo].[RV_TaggedItems] AS TI
	ON TI.UniqueID = DS.ObjectID
GO


UPDATE DS
	SET [Date] = ISNULL(NT.CreationDate, N'2011-06-20 18:12:18.700')
FROM [dbo].[RV_DeletedStates] AS DS
	INNER JOIN [dbo].[CN_NodeTypes] AS NT
	ON NT.NodeTypeID = DS.ObjectID
GO


UPDATE DS
	SET [Date] = ISNULL(CH.SendDate, N'2011-06-20 18:12:18.700')
FROM [dbo].[RV_DeletedStates] AS DS
	INNER JOIN [dbo].[WK_Changes] AS CH
	ON CH.ChangeID = DS.ObjectID
GO