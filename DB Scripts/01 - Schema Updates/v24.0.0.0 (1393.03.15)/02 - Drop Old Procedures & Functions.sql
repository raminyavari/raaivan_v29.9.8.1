USE [EKM_App]
GO


-- Functions

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CN_FN_GetListTypeID]') 
    AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[CN_FN_GetListTypeID]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CN_FN_GetDepartmentGroupListTypeID]') 
            AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[CN_FN_GetDepartmentGroupListTypeID]
GO

-- end of Functions

-- Views

IF EXISTS(select * FROM sys.views where name = 'CN_View_Lists')
DROP VIEW [dbo].[CN_View_Lists]
GO

-- end of Views

-- CN

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_P_GetNodeHirarchyAdminIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_P_GetNodeHirarchyAdminIDs]
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_GetNodeHirarchyAdminIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_GetNodeHirarchyAdminIDs]
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_P_GetNodeHirarchy]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_P_GetNodeHirarchy]
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_GetNodeHirarchy]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_GetNodeHirarchy]
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[CN_P_AddNodeCreators]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CN_P_AddNodeCreators]
GO

-- end of CN

-- DataExchange

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[DE_UpdateUsers]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DE_UpdateUsers]
GO

-- end of DataExchange

-- USR

IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[DoAspnet_userAfterCascadeDelete]'))
DROP TRIGGER [dbo].[DoAspnet_userAfterCascadeDelete]
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[USR_CreateProfile]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[USR_CreateProfile]
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[USR_P_SetGeneralInfo]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[USR_P_SetGeneralInfo]
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[USR_SetGeneralInfo]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[USR_SetGeneralInfo]
GO

-- end of USR

-- WF

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_SetStateShowOwnerName]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_SetStateShowOwnerName]
GO

-- end of WF

-- WF Reports

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_NodeCreatorsReport]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_NodeCreatorsReport]
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_UserCreatedNodesReport]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_UserCreatedNodesReport]
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_NodesCreatedNodesReport]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_NodesCreatedNodesReport]
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_NodeCreatedNodesReport]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_NodeCreatedNodesReport]
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_AddService]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_AddService]
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_ModifyService]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_ModifyService]
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_RemoveService]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_RemoveService]
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_P_GetServicesByIDs]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_P_GetServicesByIDs]
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetServices]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetServices]
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[WF_GetService]') and 
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WF_GetService]
GO

-- end of WF Reports


-- User Defined Types

IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'NodesHirarchyTableType')
DROP TYPE dbo.NodesHirarchyTableType
GO

IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'ExchangeUserTableType')
DROP TYPE dbo.ExchangeUserTableType
GO

-- User Defined Types