IF (NOT EXISTS(SELECT 1 from sys.schemas WHERE name = 'ClrUnsafe'))
BEGIN
	PRINT '+Creating schema [ClrUnsafe]';
	EXEC ('CREATE SCHEMA [ClrUnsafe] AUTHORIZATION [dbo]');
END
GO

PRINT '============================================'
PRINT '             Ranking functions              '
PRINT '============================================'

IF (OBJECT_ID('ClrUnsafe.fn_RankByValueChange') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrUnsafe].[fn_RankByValueOccurence]'
	DROP FUNCTION [ClrUnsafe].[fn_RankByValueOccurence]
END

PRINT '+Creating [ClrUnsafe].[fn_RankByValueOccurence]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2011/08/23
-- Description:	Ranks records when particular value occurs
-- =============================================
CREATE FUNCTION [ClrUnsafe].[fn_RankByValueOccurence](
	@initialValue int,				--Initial rank value
	@rankFieldValue nvarchar(10),	--value of the field for ranking
	@rankingValue nvarchar(10),		--value on which occurence a rank should increase
	@rankFunctionID [tinyint] = 0	--identifies multiple calls to the function within single query
)
RETURNS [int]
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrUnsafe].[Ranking].[RankByValueOccurence]
GO
