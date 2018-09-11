IF (NOT EXISTS(SELECT 1 from sys.schemas WHERE name = 'ClrExternal'))
BEGIN
	PRINT '+Creating schema [ClrExternal]';
	EXEC ('CREATE SCHEMA [ClrExternal] AUTHORIZATION [dbo]');
END
GO

IF (OBJECT_ID('[ClrExternal].[usp_SaveAssembly]') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrExternal].[usp_ExportAssembly]'
	DROP FUNCTION [ClrExternal].[usp_ExportAssembly]
END

PRINT '+Creating [ClrExternal].[usp_ExportAssembly]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2013/11/28
-- Description:	Export Assembly from SQL Server back to .dll files
-- Parameters:
--	@assemblyName				- assembly to be exported
--	@destinationPath			- destination path to which the exported files will be stored
--  @impersonateCurrentUser		- Specifeis whether current user should be impersonated when saving
--  							  assembly to the destination folder. Otherwise the SQL Server service
--								  account will be used.
-- =============================================
CREATE PROCEDURE [ClrExternal].[usp_ExportAssembly] (
	@assemblyName [nvarchar](128),		--Assembly Name
	@destinationPath [nvarchar](256),	--Destination path for export
	@impersonateCurrentUser bit = 1
)		
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrExternal].[AssemblyExporter].[ExportAssembly]
GO