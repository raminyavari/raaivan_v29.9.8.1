USE [EKM_App]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER TABLE [dbo].[aspnet_Applications]
ADD [InvitationID] uniqueidentifier NULL
GO

ALTER TABLE [dbo].[aspnet_Applications]
ADD [EnableInvitationLink] bit NULL
GO


ALTER TABLE [dbo].[aspnet_Applications]
ADD [Language] varchar(50) NULL
GO


ALTER TABLE [dbo].[aspnet_Applications]
ADD [Calendar] varchar(50) NULL
GO
