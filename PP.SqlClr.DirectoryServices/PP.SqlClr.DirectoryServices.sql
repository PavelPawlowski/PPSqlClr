--Drop objects in the Assembly
DECLARE c CURSOR FOR
	SELECT
		S.name AS SchemaName
		,O.name AS ObjectName
		,O.type AS ObjectType
	FROM SYS.module_assembly_usages AU
	INNER JOIN SYS.assemblies A ON AU.assembly_id = A.assembly_id
	INNER JOIN sys.objects O ON AU.object_id = O.object_id
	LEFT JOIN sys.schemas S ON O.schema_id = S.schema_id
	WHERE A.name ='PPSqlDirectoryServices';

DECLARE 
	@schemaName nvarchar(128),
	@objName nvarchar(128),
	@type char(2),
	@sql nvarchar(max);

OPEN c;

FETCH NEXT FROM c INTO @schemaName, @objName, @type;

WHILE (@@FETCH_STATUS = 0)
BEGIN
	PRINT N'-Dropping ' + QUOTENAME(@schemaName) + '.' + QUOTENAME(@objName);
	IF (@type IN ('FT', 'FS'))
		SET @sql = N'DROP FUNCTION ' + QUOTENAME(@schemaName) + '.' + QUOTENAME(@objName);
    IF (@type IN ('PC'))
		SET @sql = N'DROP PROCEDURE ' + QUOTENAME(@schemaName) + '.' + QUOTENAME(@objName);

	IF (@sql IS NOT NULL)
		EXEC (@sql)

	FETCH NEXT FROM c INTO @schemaName, @objName, @type;
END

CLOSE c;
DEALLOCATE c;



--Drop Assembly
IF (EXISTS(SELECT 1 FROM sys.assemblies WHERE name = 'PPSqlDirectoryServices'))
BEGIN
	PRINT '-Dropping [PPSqlDirectoryServices] Assembly'
	DROP ASSEMBLY [PPSqlDirectoryServices]
END
GO

--Create System.DirectoryServices
IF (NOT EXISTS(SELECT 1 FROM sys.assemblies WHERE name = 'System.DirectoryServices'))
BEGIN
	PRINT '+Creating [System.DirectoryServices] Assembly'
    CREATE ASSEMBLY [System.DirectoryServices]
    AUTHORIZATION [dbo]
    --FROM 'C:\Windows\Microsoft.NET\Framework\v2.0.50727\System.DirectoryServices.dll'
	FROM 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.DirectoryServices.dll'
    WITH PERMISSION_SET = UNSAFE
END
GO


--Create the asssembly
PRINT '+Creating [PPSqlDirectoryServices] Assembly'
CREATE ASSEMBLY [PPSqlDirectoryServices]
AUTHORIZATION [dbo]
FROM 'C:\SQLCLR\PPSqlDirectoryServices.dll'
WITH PERMISSION_SET = UNSAFE
GO

