USE [EKM_App]
GO

DECLARE @BCName varchar(400) = 'EKM_App_Full_Backup_' + FORMAT(GETDATE(), 'yyyy_MM_dd_HH_mm_ss') + '.bak'
DECLARE @Addr varchar(400) = 'E:\Back\Full\' + @BCName

BACKUP DATABASE [EKM_App]
TO DISK = @Addr
WITH CHECKSUM, COMPRESSION, NOFORMAT, INIT, NAME = @BCName, SKIP, NOREWIND, NOUNLOAD, STATS = 10;

GO