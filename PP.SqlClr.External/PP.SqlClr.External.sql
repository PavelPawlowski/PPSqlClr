DECLARE @dropExistingObjects bit = 0

IF @dropExistingObjects = 1
BEGIN
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
		WHERE A.name ='PPSqlClrExternal';

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
		WHERE A.name ='PPSqlClrExternal';

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
	IF (EXISTS(SELECT 1 FROM sys.assemblies WHERE name = 'PPSqlClrExternal'))
	BEGIN
		PRINT '-Dropping [PPSqlClrExternal] Assembly'
		DROP ASSEMBLY [PPSqlClrExternal]
	END
END


IF EXISTS(SELECT 1 FROM sys.assemblies WHERE name = 'PPSqlClrExternal')
BEGIN
    RAISERROR(N'+Updating [PPSqlClrExternal] Assembly... 

If the assmebly does not differs form existing one in the databse, an exception about MVID will follow and you can ignore it.

', 0, 0) WITH NOWAIT;

    ALTER ASSEMBLY [PPSqlClrExternal]
        DROP FILE ALL

    ALTER ASSEMBLY [PPSqlClrExternal]
        FROM 'C:\SQLCLR\PP.SqlClr.External.dll'

    ALTER ASSEMBLY [PPSqlClrExternal]
        ADD FILE FROM 'C:\SQLCLR\PP.SqlClr.External.dll'
END
ELSE
BEGIN
	--Create the asssembly
	PRINT '+Creating [PPSqlClrExternal] Assembly'
	CREATE ASSEMBLY [PPSqlClrExternal]
	AUTHORIZATION [dbo]
	FROM 'C:\SQLCLR\PP.SqlClr.External.dll'
	WITH PERMISSION_SET = EXTERNAL_ACCESS
END
GO
