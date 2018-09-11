IF (NOT EXISTS(SELECT 1 from sys.schemas WHERE name = 'ClrSafe'))
BEGIN
	PRINT '+Creating schema [ClrSafe]';
	EXEC ('CREATE SCHEMA [ClrSafe] AUTHORIZATION [dbo]');
END
GO

PRINT '============================================'
PRINT '         Cryptography functions             '
PRINT '============================================'

IF (OBJECT_ID('ClrSafe.fn_ComputeHashData') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrSafe].[fn_ComputeHashData]'
	DROP FUNCTION [ClrSafe].[fn_ComputeHashData]
END

PRINT '+Creating [ClrSafe].[fn_ComputeHashData]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2016/02/02
-- Description:	Returns Hash of input data using specified algorithm
-- =============================================
CREATE FUNCTION [ClrSafe].[fn_ComputeHashData](
	@algorithm nvarchar(9)			--Hash Algorithm to be used
	,@inputData varbinary(max)		--Input data to be hashed
)
RETURNS varbinary(64)
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrSafe].[Cryptography].[ComputeHashData]
GO

IF (OBJECT_ID('ClrSafe.fn_ComputeHashString') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrSafe].[fn_SComputeHashString]'
	DROP FUNCTION [ClrSafe].[fn_ComputeHashString]
END

PRINT '+Creating [ClrSafe].[fn_ComputeHashString]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2016/02/02
-- Description:	Returns hash of input string using specified Hash algorithm and encoding
-- =============================================
CREATE FUNCTION [ClrSafe].[fn_ComputeHashString](
	@algorithm nvarchar(9)		--Hash Algorithm to be used
	,@encoding	nvarchar(16)	--Encoding to be used to convert string
	,@inputString nvarchar(max) --input string to be hashed
)
RETURNS varbinary(64)
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrSafe].[Cryptography].[ComputeHashString]
GO
