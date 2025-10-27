USE [EKM_App]
GO

DECLARE @BCName varchar(400) = 'EKM_App_Log_Backup_' + FORMAT(GETDATE(), 'yyyy_MM_dd_HH_mm_ss') + '.trn'
DECLARE @Addr varchar(400) = 'E:\Back\Log\' + @BCName

BACKUP LOG [EKM_App]
TO DISK = @Addr
WITH CHECKSUM, COMPRESSION, NOFORMAT, INIT, NAME = @BCName, SKIP, NOREWIND, NOUNLOAD, STATS = 10;

GO