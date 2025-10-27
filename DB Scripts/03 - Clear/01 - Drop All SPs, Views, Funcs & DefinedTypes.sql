USE [EKM_App]
GO

WHILE EXISTS(SELECT TOP(1) * FROM sys.objects WHERE [type] = 'p' AND is_ms_shipped = 0) BEGIN
	DECLARE @ProcName varchar(500)
	DECLARE Cur cursor 

	FOR SELECT [Name] 
	FROM sys.objects 
	WHERE [type] = 'p' AND is_ms_shipped = 0
	OPEN Cur
	FETCH NEXT FROM Cur INTO @ProcName
	WHILE @@FETCH_STATUS = 0
	BEGIN
		BEGIN TRY
			EXEC('DROP PROCEDURE ' + @ProcName)
		END TRY
		BEGIN CATCH
		END CATCH
			
		FETCH NEXT FROM Cur INTO @ProcName
	END
	CLOSE Cur
	DEALLOCATE Cur
END

GO

WHILE EXISTS(
	SELECT TOP(1) * 
	FROM sys.objects 
	WHERE [type] IN (N'FN', N'IF', N'TF', N'FS', N'FT') AND is_ms_shipped = 0
) BEGIN
	DECLARE @FuncName varchar(500)
	DECLARE Cur cursor 

	FOR SELECT [Name] 
	FROM sys.objects 
	WHERE [type] IN (N'FN', N'IF', N'TF', N'FS', N'FT') AND is_ms_shipped = 0
	OPEN Cur
	FETCH NEXT FROM Cur INTO @FuncName
	WHILE @@FETCH_STATUS = 0
	BEGIN
		BEGIN TRY
			EXEC('DROP FUNCTION ' + @FuncName)
		END TRY
		BEGIN CATCH
		END CATCH
			
		FETCH NEXT FROM Cur INTO @FuncName
	END
	CLOSE Cur
	DEALLOCATE Cur
END

GO

WHILE EXISTS(SELECT TOP(1) * FROM sys.types WHERE is_user_defined = 1) BEGIN
	DECLARE @TypeName varchar(500)
	DECLARE Cur cursor 

	FOR SELECT [Name] 
	FROM sys.types
	WHERE is_user_defined = 1
	OPEN Cur
	FETCH NEXT FROM Cur INTO @TypeName
	WHILE @@FETCH_STATUS = 0
	BEGIN
		BEGIN TRY
			EXEC('DROP TYPE ' + @TypeName)
		END TRY
		BEGIN CATCH
		END CATCH
			
		FETCH NEXT FROM Cur INTO @TypeName
	END
	CLOSE Cur
	DEALLOCATE Cur
END

GO



WHILE EXISTS(SELECT TOP(1) * FROM sys.views) BEGIN
	DECLARE @ViewName varchar(500)
	DECLARE Cur cursor 

	FOR SELECT [Name] 
	FROM sys.views
	OPEN Cur
	FETCH NEXT FROM Cur INTO @ViewName
	WHILE @@FETCH_STATUS = 0
	BEGIN
		BEGIN TRY
			EXEC('DROP VIEW ' + @ViewName)
		END TRY
		BEGIN CATCH
		END CATCH
		
		FETCH NEXT FROM Cur INTO @ViewName
	END
	CLOSE Cur
	DEALLOCATE Cur
END

GO

WHILE EXISTS(SELECT TOP(1) * FROM sys.objects WHERE [type] IN (N'TR') AND is_ms_shipped = 0) BEGIN
	DECLARE @TRName varchar(500)
	DECLARE Cur cursor 

	FOR SELECT [Name] 
	FROM sys.objects 
	WHERE [type] IN (N'TR') AND is_ms_shipped = 0
	OPEN Cur
	FETCH NEXT FROM Cur INTO @TRName
	WHILE @@FETCH_STATUS = 0
	BEGIN
		BEGIN TRY
			EXEC('DROP TRIGGER ' + @TRName)
		END TRY
		BEGIN CATCH
		END CATCH
			
		FETCH NEXT FROM Cur INTO @TRName
	END
	CLOSE Cur
	DEALLOCATE Cur
END

GO