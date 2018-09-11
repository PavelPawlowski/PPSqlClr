IF (NOT EXISTS(SELECT 1 from sys.schemas WHERE name = 'ClrUnsafe'))
BEGIN
	PRINT '+Creating schema [ClrUnsafe]';
	EXEC ('CREATE SCHEMA [ClrUnsafe] AUTHORIZATION [dbo]');
END
GO

PRINT '============================================'
PRINT '             Running Totals                 '
PRINT '============================================'


IF (OBJECT_ID('ClrUnsafe.fn_RunningTotalTinyInt') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrUnsafe].[fn_RunningTotalTinyInt]'
	DROP FUNCTION [ClrUnsafe].[fn_RunningTotalTinyInt]
END

PRINT '+Creating [ClrUnsafe].[fn_RunningTotalTinyInt]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2010/12/28
-- Description:	Calculates running total for tinnyint data type
-- =============================================
CREATE FUNCTION [ClrUnsafe].[fn_RunningTotalTinyInt](
	@val [tinyint],					--value to be added to running total (a fiedl in the query)
	@id [tinyint],					--id of the running total within a single query
	@rowNo [int],					--RowNumber of processed records. This is compared to espected rowNo and in case out of synchronization an exceiption is fired
	@nullValue [tinyint] = NULL		--representation of the NULL value when adding to running totals
)
RETURNS [tinyint]
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrUnsafe].[RunningTotals].[RunningTotalTinyInt]
GO

IF (OBJECT_ID('ClrUnsafe.fn_RunningTotalSmallInt') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrUnsafe].[fn_RunningTotalSmallInt]'
	DROP FUNCTION [ClrUnsafe].[fn_RunningTotalSmallInt]
END

PRINT '+Creating [ClrUnsafe].[fn_RunningTotalSmallInt]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2010/12/28
-- Description:	Calculates running total for smallint data type
-- =============================================
CREATE FUNCTION [ClrUnsafe].[fn_RunningTotalSmallInt](
	@val [smallint],					--value to be added to running total (a fiedl in the query)
	@id [tinyint],						--id of the running total within a single query
	@rowNo [int],					--RowNumber of processed records. This is compared to espected rowNo and in case out of synchronization an exceiption is fired
	@nullValue [smallint] = NULL		--representation of the NULL value when adding to running totals
)
RETURNS [smallint]
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrUnsafe].[RunningTotals].[RunningTotalSmallInt]
GO


IF (OBJECT_ID('ClrUnsafe.fn_RunningTotalInt') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrUnsafe].[fn_RunningTotalInt]'
	DROP FUNCTION [ClrUnsafe].[fn_RunningTotalInt]
END

PRINT '+Creating [ClrUnsafe].[fn_RunningTotalInt]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2010/12/28
-- Description:	Calculates running total for int data type
-- =============================================
CREATE FUNCTION [ClrUnsafe].[fn_RunningTotalInt](
	@val [int],					--value to be added to running total (a fiedl in the query)
	@id [tinyint],				--id of the running total within a single query
	@rowNo [int],					--RowNumber of processed records. This is compared to espected rowNo and in case out of synchronization an exceiption is fired
	@nullValue [int] = NULL		--representation of the NULL value when adding to running totals
)
RETURNS [int]
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrUnsafe].[RunningTotals].[RunningTotalInt]
GO


IF (OBJECT_ID('ClrUnsafe.fn_RunningTotalBigInt') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrUnsafe].[fn_RunningTotalBigInt]'
	DROP FUNCTION [ClrUnsafe].[fn_RunningTotalBigInt]
END

PRINT '+Creating [ClrUnsafe].[fn_RunningTotalBigInt]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2010/12/28
-- Description:	Calculates running total for bigint data type
-- =============================================
CREATE FUNCTION [ClrUnsafe].[fn_RunningTotalBigInt](
	@val [bigint],					--value to be added to running total (a fiedl in the query)
	@id [tinyint],					--id of the running total within a single query
	@rowNo [int],					--RowNumber of processed records. This is compared to espected rowNo and in case out of synchronization an exceiption is fired
	@nullValue [bigint] = NULL		--representation of the NULL value when adding to running totals
)
RETURNS [bigint]
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrUnsafe].[RunningTotals].[RunningTotalBigInt]
GO


IF (OBJECT_ID('ClrUnsafe.fn_RunningTotalFloat') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrUnsafe].[fn_RunningTotalFloat]'
	DROP FUNCTION [ClrUnsafe].[fn_RunningTotalFloat]
END

PRINT '+Creating [ClrUnsafe].[fn_RunningTotalFloat]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2010/12/28
-- Description:	Calculates running total for float data type
-- =============================================
CREATE FUNCTION [ClrUnsafe].[fn_RunningTotalFloat](
	@val [float],					--value to be added to running total (a fiedl in the query)
	@id [tinyint],					--id of the running total within a single query
	@rowNo [int],					--RowNumber of processed records. This is compared to espected rowNo and in case out of synchronization an exceiption is fired
	@nullValue [float] = NULL		--representation of the NULL value when adding to running totals
)
RETURNS [float]
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrUnsafe].[RunningTotals].[RunningTotalFloat]
GO


IF (OBJECT_ID('ClrUnsafe.fn_RunningTotalReal') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrUnsafe].[fn_RunningTotalReal]'
	DROP FUNCTION [ClrUnsafe].[fn_RunningTotalReal]
END

PRINT '+Creating [ClrUnsafe].[fn_RunningTotalReal]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2010/12/28
-- Description:	Calculates running total for real data type
-- =============================================
CREATE FUNCTION [ClrUnsafe].[fn_RunningTotalReal](
	@val [real],					--value to be added to running total (a fiedl in the query)
	@id [tinyint],					--id of the running total within a single query
	@rowNo [int],					--RowNumber of processed records. This is compared to espected rowNo and in case out of synchronization an exceiption is fired
	@nullValue [real] = NULL		--representation of the NULL value when adding to running totals
)
RETURNS [real]
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrUnsafe].[RunningTotals].[RunningTotalReal]
GO


IF (OBJECT_ID('ClrUnsafe.fn_RunningTotalMoney') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrUnsafe].[fn_RunningTotalMoney]'
	DROP FUNCTION [ClrUnsafe].[fn_RunningTotalMoney]
END

PRINT '+Creating [ClrUnsafe].[fn_RunningTotalMoney]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2010/12/28
-- Description:	Calculates running total for money data type
-- =============================================
CREATE FUNCTION [ClrUnsafe].[fn_RunningTotalMoney](
	@val [money],					--value to be added to running total (a fiedl in the query)
	@id [tinyint],					--id of the running total within a single query
	@rowNo [int],					--RowNumber of processed records. This is compared to espected rowNo and in case out of synchronization an exceiption is fired
	@nullValue [money] = NULL		--representation of the NULL value when adding to running totals
)
RETURNS [money]
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrUnsafe].[RunningTotals].[RunningTotalMoney]
GO


IF (OBJECT_ID('ClrUnsafe.fn_RunningTotalDecimal_18_0') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrUnsafe].[fn_RunningTotalDecimal_18_0]'
	DROP FUNCTION [ClrUnsafe].[fn_RunningTotalDecimal_18_0]
END

PRINT '+Creating [ClrUnsafe].[fn_RunningTotalDecimal_18_0]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2010/12/28
-- Description:	Calculates running total for decimal(18, 0) data type
-- =============================================
CREATE FUNCTION [ClrUnsafe].[fn_RunningTotalDecimal_18_0](
	@val [decimal](18, 0),					--value to be added to running total (a fiedl in the query)
	@id [tinyint],							--id of the running total within a single query
	@rowNo [int],							--RowNumber of processed records. This is compared to espected rowNo and in case out of synchronization an exceiption is fired
	@nullValue [decimal](18, 0) = NULL		--representation of the NULL value when adding to running totals
)
RETURNS [decimal](18, 0)
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrUnsafe].[RunningTotals].[RunningTotalDecimal]
GO

IF (OBJECT_ID('ClrUnsafe.fn_RunningTotalDecimal_18_4') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrUnsafe].[fn_RunningTotalDecimal_18_4]'
	DROP FUNCTION [ClrUnsafe].[fn_RunningTotalDecimal_18_4]
END

PRINT '+Creating [ClrUnsafe].[fn_RunningTotalDecimal_18_4]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2010/12/28
-- Description:	Calculates running total for decimal(18, 4) data type
-- =============================================
CREATE FUNCTION [ClrUnsafe].[fn_RunningTotalDecimal_18_4](
	@val [decimal](18, 4),					--value to be added to running total (a fiedl in the query)
	@id [tinyint],							--id of the running total within a single query
	@rowNo [int],							--RowNumber of processed records. This is compared to espected rowNo and in case out of synchronization an exceiption is fired
	@nullValue [decimal](18, 4) = NULL		--representation of the NULL value when adding to running totals
)
RETURNS [decimal](18, 4)
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrUnsafe].[RunningTotals].[RunningTotalDecimal]
GO

PRINT '============================================'
PRINT '          Running Totals Queue              '
PRINT '============================================'


IF (OBJECT_ID('ClrUnsafe.fn_RunningTotalTinyIntQueue') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrUnsafe].[fn_RunningTotalTinyIntQueue]'
	DROP FUNCTION [ClrUnsafe].[fn_RunningTotalTinyIntQueue]
END

PRINT '+Creating [ClrUnsafe].[fn_RunningTotalTinyIntQueue]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2010/12/28
-- Description:	Calculates running total for tinnyint data type
-- =============================================
CREATE FUNCTION [ClrUnsafe].[fn_RunningTotalTinyIntQueue](
	@val [tinyint],					--value to be added to running total (a fiedl in the query)
	@id [tinyint],					--id of the running total within a single query
	@queueSize [int],				--Specifies queueSize - how much rows should be aggregated
	@nullValue [tinyint] = NULL,	--representation of the NULL value when adding to running totals
	@nullForLessRows [bit] = 1		--Specifies whether NULL should be returned if less than @queueSize items are aggregated
)
RETURNS [tinyint]
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrUnsafe].[RunningTotalsQueue].[RunningTotalTinyIntQueue]
GO

IF (OBJECT_ID('ClrUnsafe.fn_RunningTotalSmallIntQueue') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrUnsafe].[fn_RunningTotalSmallIntQueue]'
	DROP FUNCTION [ClrUnsafe].[fn_RunningTotalSmallIntQueue]
END

PRINT '+Creating [ClrUnsafe].[fn_RunningTotalSmallIntQueue]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2010/12/28
-- Description:	Calculates running total for [smallint] data type
-- =============================================
CREATE FUNCTION [ClrUnsafe].[fn_RunningTotalSmallIntQueue](
	@val [smallint],				--value to be added to running total (a fiedl in the query)
	@id [tinyint],					--id of the running total within a single query
	@queueSize [int],				--Specifies queueSize - how much rows should be aggregated
	@nullValue [smallint] = NULL,	--representation of the NULL value when adding to running totals
	@nullForLessRows [bit] = 1		--Specifies whether NULL should be returned if less than @queueSize items are aggregated
)
RETURNS [smallint]
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrUnsafe].[RunningTotalsQueue].[RunningTotalSmallIntQueue]
GO


IF (OBJECT_ID('ClrUnsafe.fn_RunningTotalIntQueue') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrUnsafe].[fn_RunningTotalIntQueue]'
	DROP FUNCTION [ClrUnsafe].[fn_RunningTotalIntQueue]
END

PRINT '+Creating [ClrUnsafe].[fn_RunningTotalIntQueue]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2010/12/28
-- Description:	Calculates running total for [int] data type
-- =============================================
CREATE FUNCTION [ClrUnsafe].[fn_RunningTotalIntQueue](
	@val [int],					--value to be added to running total (a fiedl in the query)
	@id [tinyint],				--id of the running total within a single query
	@queueSize [int],			--Specifies queueSize - how much rows should be aggregated
	@nullValue [int] = NULL,	--representation of the NULL value when adding to running totals
	@nullForLessRows [bit] = 1	--Specifies whether NULL should be returned if less than @queueSize items are aggregated
)
RETURNS [int]
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrUnsafe].[RunningTotalsQueue].[RunningTotalIntQueue]
GO


IF (OBJECT_ID('ClrUnsafe.fn_RunningTotalBigIntQueue') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrUnsafe].[fn_RunningTotalBigIntQueue]'
	DROP FUNCTION [ClrUnsafe].[fn_RunningTotalBigIntQueue]
END

PRINT '+Creating [ClrUnsafe].[fn_RunningTotalBigIntQueue]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2010/12/28
-- Description:	Calculates running total for [bigint] data type
-- =============================================
CREATE FUNCTION [ClrUnsafe].[fn_RunningTotalBigIntQueue](
	@val [bigint],					--value to be added to running total (a fiedl in the query)
	@id [tinyint],					--id of the running total within a single query
	@queueSize [int],				--Specifies queueSize - how much rows should be aggregated
	@nullValue [bigint] = NULL,		--representation of the NULL value when adding to running totals
	@nullForLessRows [bit] = 1		--Specifies whether NULL should be returned if less than @queueSize items are aggregated
)
RETURNS [bigint]
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrUnsafe].[RunningTotalsQueue].[RunningTotalBigIntQueue]
GO


IF (OBJECT_ID('ClrUnsafe.fn_RunningTotalFloatQueue') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrUnsafe].[fn_RunningTotalFloatQueue]'
	DROP FUNCTION [ClrUnsafe].[fn_RunningTotalFloatQueue]
END

PRINT '+Creating [ClrUnsafe].[fn_RunningTotalFloatQueue]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2010/12/28
-- Description:	Calculates running total for [float] data type
-- =============================================
CREATE FUNCTION [ClrUnsafe].[fn_RunningTotalFloatQueue](
	@val [float],					--value to be added to running total (a fiedl in the query)
	@id [tinyint],					--id of the running total within a single query
	@queueSize [int],				--Specifies queueSize - how much rows should be aggregated
	@nullValue [float] = NULL,		--representation of the NULL value when adding to running totals
	@nullForLessRows [bit] = 1		--Specifies whether NULL should be returned if less than @queueSize items are aggregated
)
RETURNS [float]
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrUnsafe].[RunningTotalsQueue].[RunningTotalFloatQueue]
GO


IF (OBJECT_ID('ClrUnsafe.fn_RunningTotalRealQueue') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrUnsafe].[fn_RunningTotalRealQueue]'
	DROP FUNCTION [ClrUnsafe].[fn_RunningTotalRealQueue]
END

PRINT '+Creating [ClrUnsafe].[fn_RunningTotalRealQueue]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2010/12/28
-- Description:	Calculates running total for [real] data type
-- =============================================
CREATE FUNCTION [ClrUnsafe].[fn_RunningTotalRealQueue](
	@val [real],					--value to be added to running total (a fiedl in the query)
	@id [tinyint],					--id of the running total within a single query
	@queueSize [int],				--Specifies queueSize - how much rows should be aggregated
	@nullValue [real] = NULL,		--representation of the NULL value when adding to running totals
	@nullForLessRows [bit] = 1		--Specifies whether NULL should be returned if less than @queueSize items are aggregated
)
RETURNS [real]
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrUnsafe].[RunningTotalsQueue].[RunningTotalRealQueue]
GO


IF (OBJECT_ID('ClrUnsafe.fn_RunningTotalMoneyQueue') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrUnsafe].[fn_RunningTotalMoneyQueue]'
	DROP FUNCTION [ClrUnsafe].[fn_RunningTotalMoneyQueue]
END

PRINT '+Creating [ClrUnsafe].[fn_RunningTotalMoneyQueue]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2010/12/28
-- Description:	Calculates running total for [money] data type
-- =============================================
CREATE FUNCTION [ClrUnsafe].[fn_RunningTotalMoneyQueue](
	@val [money],					--value to be added to running total (a fiedl in the query)
	@id [tinyint],					--id of the running total within a single query
	@queueSize [int],				--Specifies queueSize - how much rows should be aggregated
	@nullValue [money] = NULL,		--representation of the NULL value when adding to running totals
	@nullForLessRows [bit] = 1		--Specifies whether NULL should be returned if less than @queueSize items are aggregated
)
RETURNS [money]
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrUnsafe].[RunningTotalsQueue].[RunningTotalMoneyQueue]
GO


IF (OBJECT_ID('ClrUnsafe.fn_RunningTotalDecimalQueue_18_0') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrUnsafe].[fn_RunningTotalDecimalQueue_18_0]'
	DROP FUNCTION [ClrUnsafe].[fn_RunningTotalDecimalQueue_18_0]
END

PRINT '+Creating [ClrUnsafe].[fn_RunningTotalDecimalQueue_18_0]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2010/12/28
-- Description:	Calculates running total for [decimal](18, 0) data type
-- =============================================
CREATE FUNCTION [ClrUnsafe].[fn_RunningTotalDecimalQueue_18_0](
	@val [decimal](18, 0),				--value to be added to running total (a fiedl in the query)
	@id [tinyint],						--id of the running total within a single query
	@queueSize [int],					--Specifies queueSize - how much rows should be aggregated
	@nullValue [decimal](18, 0) = NULL,	--representation of the NULL value when adding to running totals
	@nullForLessRows [bit] = 1			--Specifies whether NULL should be returned if less than @queueSize items are aggregated
)
RETURNS [decimal](18, 4)
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrUnsafe].[RunningTotalsQueue].[RunningTotalDecimalQueue]
GO


IF (OBJECT_ID('ClrUnsafe.fn_RunningTotalDecimalQueue_18_4') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrUnsafe].[fn_RunningTotalDecimalQueue_18_4]'
	DROP FUNCTION [ClrUnsafe].[fn_RunningTotalDecimalQueue_18_4]
END

PRINT '+Creating [ClrUnsafe].[fn_RunningTotalDecimalQueue_18_4]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2010/12/28
-- Description:	Calculates running total for [decimal](18, 4) data type
-- =============================================
CREATE FUNCTION [ClrUnsafe].[fn_RunningTotalDecimalQueue_18_4](
	@val [decimal](18, 4),				--value to be added to running total (a fiedl in the query)
	@id [tinyint],						--id of the running total within a single query
	@queueSize [int],					--Specifies queueSize - how much rows should be aggregated
	@nullValue [decimal](18, 4) = NULL,	--representation of the NULL value when adding to running totals
	@nullForLessRows [bit] = 1			--Specifies whether NULL should be returned if less than @queueSize items are aggregated
)
RETURNS [decimal](18, 4)
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrUnsafe].[RunningTotalsQueue].[RunningTotalDecimalQueue]
GO
