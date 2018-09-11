IF (NOT EXISTS(SELECT 1 from sys.schemas WHERE name = 'ClrTypes'))
BEGIN
	PRINT '+Creating schema [ClrTypes]';
	EXEC ('CREATE SCHEMA [ClrTypes] AUTHORIZATION [dbo]');
END
GO

PRINT '============================================'
PRINT '            Roman CLR Data Type             '
PRINT '============================================'

IF (OBJECT_ID('ClrSafe.fn_RemoveAccents') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrTypes].[Roman]'
	DROP FUNCTION [ClrTypes].[Roman]
END

PRINT '+Creating [ClrTypes].[Roman]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2012/01/27
-- Description:	Removes accent (diacritics) from input string
-- =============================================
CREATE TYPE [ClrTypes].[Roman]
EXTERNAL NAME [PPSqlClrTypes].[Roman]
GO
