﻿--Drop objects in the Assembly
DECLARE c CURSOR FOR
	SELECT
		S.name AS SchemaName
		,O.name AS ObjectName
		,O.type AS ObjectType
	FROM SYS.module_assembly_usages AU
	INNER JOIN SYS.assemblies A ON AU.assembly_id = A.assembly_id
	INNER JOIN sys.objects O ON AU.object_id = O.object_id
	LEFT JOIN sys.schemas S ON O.schema_id = S.schema_id
	WHERE A.name ='PPSqlClrTypes';

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
	IF(@type IN ('PC'))
		SET @sql = N'DROP PROCEDURE ' + QUOTENAME(@schemaName) + '.' + QUOTENAME(@objName);

	IF (@sql IS NOT NULL)
		EXEC (@sql)

	FETCH NEXT FROM c INTO @schemaName, @objName, @type;
END

CLOSE c;
DEALLOCATE c;

DECLARE tc CURSOR FOR
    SELECT
		    S.name AS SchemaName
		    ,T.name AS ObjectName
    FROM sys.type_assembly_usages TU
    INNER JOIN sys.assemblies A ON TU.assembly_id = A.assembly_id
    INNER JOIN sys.types T ON TU.user_type_id = T.user_type_id
    INNER JOIN sys.schemas S ON T.schema_id = S.schema_id
    WHERE A.name ='PPSqlClrTypes';

DECLARE
    @typeName nvarchar(128);
    
OPEN tc;

FETCH NEXT FROM tc INTO @schemaName, @typeName;

WHILE (@@FETCH_STATUS = 0)
BEGIN
    PRINT N'-Dropping CLR Type ' + QUOTENAME(@schemaName) + N'.' + QUOTENAME(@typeName);
    SET @sql = N'DROP TYPE ' + QUOTENAME(@schemaName) + N'.' + QUOTENAME(@typeName);
    
    IF (@sql IS NOT NULL)
        EXEC (@sql)

    FETCH NEXT FROM tc INTO @schemaName, @typeName;
END

CLOSE tc;
DEALLOCATE tc;

--Drop Assembly
IF (EXISTS(SELECT 1 FROM sys.assemblies WHERE name = 'PPSqlClrTypes'))
BEGIN
	PRINT '-Dropping [PPSqlClrTypes] Assembly'
	DROP ASSEMBLY [PPSqlClrTypes]
END
GO

--Create the asssembly
PRINT '+Creating [PPSqlClrTypes] Assembly'
CREATE ASSEMBLY [PPSqlClrTypes]
AUTHORIZATION [dbo]
FROM 'C:\SQLCLR\PPSqlClrTypes.dll'
WITH PERMISSION_SET = SAFE
GO

