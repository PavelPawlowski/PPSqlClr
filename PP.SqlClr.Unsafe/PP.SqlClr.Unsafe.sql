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
		WHERE A.name ='PPSqlClrUnsafe';

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

	--Drop Assembly
	IF (EXISTS(SELECT 1 FROM sys.assemblies WHERE name = 'PPSqlClrUnsafe'))
	BEGIN
		PRINT '-Dropping [PPSqlClrUnsafe] Assembly'
		DROP ASSEMBLY [PPSqlClrUnsafe]
	END
END

IF EXISTS(SELECT 1 FROM sys.assemblies WHERE name = 'PPSqlClrUnsafe')
BEGIN
    RAISERROR(N'+Updating [PPSqlClrUnsafe] Assembly... 

If the assmebly does not differs form existing one in the databse, an exception about MVID will follow and you can ignore it.

', 0, 0) WITH NOWAIT;

    ALTER ASSEMBLY [PPSqlClrUnsafe]
        DROP FILE ALL

    ALTER ASSEMBLY [PPSqlClrUnsafe]
        FROM 'C:\SQLCLR\PP.SqlClr.Unsafe.dll'

    ALTER ASSEMBLY [PPSqlClrUnsafe]
        ADD FILE FROM 'C:\SQLCLR\PP.SqlClr.Unsafe.dll'
END
ELSE
BEGIN
	--Create the asssembly
	PRINT '+Creating [PPSqlClrUnsafe] Assembly'
	CREATE ASSEMBLY [PPSqlClrUnsafe]
	AUTHORIZATION [dbo]
	FROM 'C:\SQLCLR\PP.SqlClr.Unsafe.dll'
	WITH PERMISSION_SET = UNSAFE
END