/****** Object:  LinkedServer [RAAIVAN]    Script Date: 08/24/2013 15:20:13 ******/
IF  EXISTS (SELECT srv.name FROM sys.servers srv WHERE srv.server_id != 0 AND srv.name = N'RAAIVAN')EXEC master.dbo.sp_dropserver @server=N'RAAIVAN', @droplogins='droplogins'
GO

/****** Object:  LinkedServer [RAAIVAN]    Script Date: 08/24/2013 15:20:13 ******/
EXEC master.dbo.sp_addlinkedserver @server = N'RAAIVAN', @srvproduct=N'SQL Server'
 /* For security reasons the linked server remote logins password is changed with ######## */
EXEC master.dbo.sp_addlinkedsrvlogin @rmtsrvname=N'RAAIVAN',@useself=N'False',@locallogin=NULL,@rmtuser=N'sa',@rmtpassword='hrdovjshjpvjo'

GO

EXEC master.dbo.sp_serveroption @server=N'RAAIVAN', @optname=N'collation compatible', @optvalue=N'false'
GO

EXEC master.dbo.sp_serveroption @server=N'RAAIVAN', @optname=N'data access', @optvalue=N'true'
GO

EXEC master.dbo.sp_serveroption @server=N'RAAIVAN', @optname=N'dist', @optvalue=N'false'
GO

EXEC master.dbo.sp_serveroption @server=N'RAAIVAN', @optname=N'pub', @optvalue=N'false'
GO

EXEC master.dbo.sp_serveroption @server=N'RAAIVAN', @optname=N'rpc', @optvalue=N'false'
GO

EXEC master.dbo.sp_serveroption @server=N'RAAIVAN', @optname=N'rpc out', @optvalue=N'false'
GO

EXEC master.dbo.sp_serveroption @server=N'RAAIVAN', @optname=N'sub', @optvalue=N'false'
GO

EXEC master.dbo.sp_serveroption @server=N'RAAIVAN', @optname=N'connect timeout', @optvalue=N'0'
GO

EXEC master.dbo.sp_serveroption @server=N'RAAIVAN', @optname=N'collation name', @optvalue=null
GO

EXEC master.dbo.sp_serveroption @server=N'RAAIVAN', @optname=N'lazy schema validation', @optvalue=N'false'
GO

EXEC master.dbo.sp_serveroption @server=N'RAAIVAN', @optname=N'query timeout', @optvalue=N'0'
GO

EXEC master.dbo.sp_serveroption @server=N'RAAIVAN', @optname=N'use remote collation', @optvalue=N'true'
GO

EXEC master.dbo.sp_serveroption @server=N'RAAIVAN', @optname=N'remote proc transaction promotion', @optvalue=N'true'
GO


