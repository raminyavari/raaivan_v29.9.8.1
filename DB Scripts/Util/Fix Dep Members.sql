USE EKM_App
GO


DECLARE @DepTypeID uniqueidentifier = [dbo].[CN_FN_GetDepartmentNodeTypeID]()
DECLARE @Mems Table(UserID uniqueidentifier, D datetime)

INSERT INTO @Mems(UserID, D)
SELECT * 
FROM
(
	SELECT NM.UserID, MAX(NM.MembershipDate) AS D
	FROM [dbo].[CN_NodeMembers] AS NM
		INNER JOIN [dbo].[CN_Nodes] AS ND
		ON NM.NodeID = ND.NodeID
	WHERE ND.NodeTypeID = @DepTypeID AND NM.Deleted = 0 AND ND.Deleted = 0
	GROUP BY NM.UserID
) AS A
ORDER BY UserID, D


UPDATE [dbo].[CN_NodeMembers]
	SET Deleted = 1
FROM @Mems AS Ref
	INNER JOIN [dbo].[CN_NodeMembers]
	ON Ref.UserID = [dbo].[CN_NodeMembers].UserID
	INNER JOIN [dbo].[CN_Nodes] AS ND
	ON [dbo].[CN_NodeMembers].NodeID = ND.NodeID
WHERE ND.NodeTypeID = @DepTypeID AND [dbo].[CN_NodeMembers].[MembershipDate] < Ref.D
	
GO