IF (NOT EXISTS(SELECT 1 from sys.schemas WHERE name = 'ClrExternal'))
BEGIN
	PRINT '+Creating schema [ClrExternal]';
	EXEC ('CREATE SCHEMA [ClrExternal] AUTHORIZATION [dbo]');
END
GO

IF (OBJECT_ID('[ClrExternal].[usp_ExportBinaryData]') IS NOT NULL)
BEGIN
	RAISERROR(N'-Dropping [ClrExternal].[usp_ExportBinaryData]', 0, 0) WITH NOWAIT;
	DROP PROCEDURE [ClrExternal].[usp_ExportBinaryData]
END

PRINT '+Creating [ClrExternal].[usp_ExportProjectClr]'
GO
/* ==========================================================================
   Author:		Pavel Pawlowski
   Create date: 2020/02/01
   Description:	Exports Binary data specified by the source query

   Parameters:
     @sourceQuery			nvarchar(max)		-- Source query providing binary data and corresponding files for data export
	,@filePathFieldName		nvarchar(128)		-- Name of the field providing full path to export file
	,@binaryDataFieldName	nvarchar(128)		-- Name of the field containing binary data to be exported
	,@createPath			bit					-- Specifies whether directories in path of target filename should be automatically created if not exists
========================================================================== */
CREATE PROCEDURE [ClrExternal].[usp_ExportBinaryData]
    @sourceQuery			nvarchar(max)		-- Source query providing binary data and corresponding files for data export
	,@filePathFieldName		nvarchar(128)		-- Name of the field providing full path to export file
	,@binaryDataFieldName	nvarchar(128)		-- Name of the field containing binary data to be exported
	,@createPath			bit				= 1	-- Specifies whether directories in path of target filename should be automatically created if not exists
	,@reportProgress		bit				= 1 -- Specifies whether progress should be reported.
WITH EXECUTE AS CALLER
AS
EXTERNAL NAME [PPSqlClrExternal].[BinaryDataExporter].[ExportBinaryData]
GO

IF (OBJECT_ID('[ClrExternal].[fn_ExportBinaryColumn]') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrExternal].[fn_ExportBinaryColumn]'
	DROP FUNCTION [ClrExternal].[fn_ExportBinaryColumn]
END

PRINT '+Creating [ClrUnsafe].[fn_ExportBinaryColumn]'
GO
/* =============================================
   Author:		Pavel Pawlowski
   Create date: 2020/02/01
   Description:	Exports Binary data to file

   Parameters:
	@binaryData varbinary(max)		--Binary data to be exported
	,@targetFile nvarchar(1024)		--Full Path to target file
	,@createPath bit			= 1	--Specifies whether directories in path of target filename should be automatically created if not exists
   ============================================= */
CREATE FUNCTION [ClrExternal].[fn_ExportBinaryColumn](
	@binaryData varbinary(max)		--Binary data to be exported
	,@targetFile nvarchar(1024)		--Full Path to target file
	,@createPath bit			= 1	--Specifies whether directories in path of target filename should be automatically created if not exists
)
RETURNS [int]
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrExternal].[BinaryDataExporter].[ExportBinaryColumn]
GO