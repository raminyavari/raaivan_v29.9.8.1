USE [EKM_App]
GO

/* -- Get File & FileGroup Names
SELECT name AS AvailableFilegroups FROM sys.filegroups WHERE type = 'FG'
SELECT name as [FileName], physical_name AS [FilePath] FROM sys.database_files WHERE type_desc = 'ROWS'
*/

DECLARE @DBName varchar(100) = (SELECT DB_NAME());

DECLARE @DBFolder varchar(1000) = (
	SELECT LEFT(N.physical_name, LEN(N.physical_name) - 
				CHARINDEX('\', REVERSE(N.physical_name) + '\')) AS Folder
	FROM sys.database_files AS N
	WHERE LOWER(N.physical_name) LIKE N'%.mdf'
);

DECLARE @Q varchar(max)

DECLARE @GroupNames TABLE (Name varchar(200), ID int IDENTITY(1, 1))
DECLARE @PartitionsCount int = 10
DECLARE @Counter int = 1

WHILE @Counter <= @PartitionsCount BEGIN
	DECLARE @GroupName varchar(200) = LOWER('Group_' + CAST(@Counter AS varchar(20)))
	DECLARE @LogicalName varchar(200) = 'Part_' + CAST(@Counter AS varchar(20))
	DECLARE @NewFileName varchar(1000) = @DBFolder + '\' + @DBName + '_p' + CAST(@Counter AS varchar(20)) + '.ndf'
	
	INSERT INTO @GroupNames (Name)
	VALUES (@GroupName)
	
	IF NOT EXISTS (SELECT TOP(1) * FROM sys.filegroups WHERE name = @GroupName) BEGIN
		EXEC ('ALTER DATABASE [' + @DBName + '] ADD FILEGROUP [' + @GroupName + ']')
	END	
	
	IF NOT EXISTS (SELECT TOP(1) * FROM sys.database_files WHERE LOWER(name) = LOWER(@LogicalName)) BEGIN
		SET @Q =
			'ALTER DATABASE [' + @DBName + '] ' +
			'ADD FILE ( ' +
				'NAME = [' + @LogicalName + '], ' +
				'FILENAME = ''' + @NewFileName + ''', ' +
				'SIZE = 5 MB, ' +
				'MAXSIZE = UNLIMITED, ' +
				'FILEGROWTH = 5 MB ' +
			') TO FILEGROUP [' + @GroupName + ']'
		
		EXEC (@Q)
	END
	
	SET @Counter = @Counter + 1
END


DECLARE @Ranges varchar(max) = (
	SELECT TOP(1) STUFF(
		(
			SELECT ', ''00000000-0000-0000-0000-' + 
				SUBSTRING(CONVERT(VARCHAR(8), CONVERT(VARBINARY(4), 
					CAST((CAST(X.ID AS float) / CAST(@PartitionsCount AS float)) * 65535.0 AS int)), 2), 5, 4) + '00000000'''
			FROM @GroupNames AS X
			WHERE X.ID < @PartitionsCount
			FOR XML PATH(''), TYPE
		).value('(./text())[1]', 'VARCHAR(MAX)'), 1, 1, '')
)


DECLARE @SchemaGroups varchar(max) = (
	SELECT TOP(1) STUFF(
		(
			SELECT ', ''' + X.Name + ''''
			FROM @GroupNames AS X
			FOR XML PATH(''), TYPE
		).value('(./text())[1]', 'VARCHAR(MAX)'), 1, 1, '')
)


IF EXISTS(SELECT 1 FROM sys.partition_schemes WHERE name = N'RV_PartitionScheme')
	DROP PARTITION SCHEME RV_PartitionScheme

IF EXISTS(SELECT 1 FROM sys.partition_functions WHERE name = N'RV_PartitionFunction')
	DROP PARTITION FUNCTION RV_PartitionFunction
	
	
SET @Q = 'CREATE PARTITION FUNCTION RV_PartitionFunction (uniqueidentifier) AS RANGE LEFT ' +
	'FOR VALUES (' + @Ranges + ')'

EXEC (@Q)


SET @Q = 'CREATE PARTITION SCHEME RV_PartitionScheme AS PARTITION RV_PartitionFunction ' +
	'TO (' + @SchemaGroups + ')'

EXEC (@Q)

/*
DECLARE @TableNames TABLE (ID int IDENTITY(1, 1), Name varchar(200))

INSERT INTO @TableNames (Name)
SELECT T.name
FROM sys.columns AS C
	JOIN sys.tables AS T
	ON C.object_id = T.object_id
WHERE LOWER(C.name) = 'applicationid'
ORDER BY T.name ASC


DECLARE @TimeStamp bigint = DATEDIFF(SECOND,'1970-01-01', GETUTCDATE())

DECLARE @TablesCount int = (SELECT MAX(T.ID) FROM @TableNames AS T)

WHILE @TablesCount > 0 BEGIN
	DECLARE @TableName varchar(200) = (SELECT TOP(1) T.Name FROM @TableNames AS T WHERE T.ID = @TablesCount)
	
	SET @Q = 
		'BEGIN TRANSACTION; ' +
		
		'CREATE CLUSTERED INDEX [ClusteredIndex_on_RV_PartitionScheme_' + CAST(@TimeStamp AS varchar(100)) + '] ' + 
		'ON [dbo].[' + @TableName + '] ([ApplicationID]) ' +
		'WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ' + 
		'ON [RV_PartitionScheme]([ApplicationID]); ' +

		'DROP INDEX [ClusteredIndex_on_RV_PartitionScheme_' + CAST(@TimeStamp AS varchar(100)) + '] ' + 
		'ON [dbo].[' + @TableName + '] WITH ( ONLINE = OFF ); ' +

		'COMMIT TRANSACTION;'
		
	EXEC (@Q)

	SET @TimeStamp = @TimeStamp + 1
	SET @TablesCount = @TablesCount - 1
END
GO
*/