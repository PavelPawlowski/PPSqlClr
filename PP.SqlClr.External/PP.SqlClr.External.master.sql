USE [master]
GO
/*
Create Asymmetric Key from the CLR to enable Unsafe assmebly execution
Further we create login based on that asymmetric key to enable creation of unsafe assembly
*/
IF NOT EXISTS(SELECT 1 FROM sys.asymmetric_keys WHERE name = 'PP.SqlClr.External.dll')
BEGIN
	RAISERROR( N'+ Creating [PP.SqlClr.Unsafe.dll] asymmetric key form PP.SqlClr.External.dll', 0, 0) WITH NOWAIT;
	CREATE ASYMMETRIC KEY [PP.SqlClr.External.dll] FROM EXECUTABLE FILE = N'C:\SQLCLR\PP.SqlClr.External.dll';
END
GO

/*
Create login from the Assembly asymmetric key. Futher we grant unsafe assembly to that login to enable unsafe assembly creation
*/
IF NOT EXISTS(SELECT 1 FROM sys.server_principals WHERE name = 'PP.SqlClr.External.dll' AND type = 'K')
BEGIN
	RAISERROR( N'+ Creating Login [PP.SqlClr.External.dll] from asymmetric key [PP.SqlClr.External.dll]', 0, 0) WITH NOWAIT;
	CREATE LOGIN [PP.SqlClr.External.dll] FROM ASYMMETRIC KEY [PP.SqlClr.External.dll];
END
GO

/*
Grant unsafe aseembly to the [PP.SqlClr.External.dll] login based on the assembly asymmetric key.
This ensures, that we can create an ussafe asembly in the SSISDB database even without setting
database as trustworthy.
*/
RAISERROR( N'+ Granting UNSAFE ASSEMBLY TO [PP.SqlClr.External.dll]', 0, 0) WITH NOWAIT;
GO
GRANT UNSAFE ASSEMBLY TO [PP.SqlClr.External.dll];
GO
