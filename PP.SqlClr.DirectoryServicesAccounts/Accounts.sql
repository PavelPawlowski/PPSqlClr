IF (NOT EXISTS(SELECT 1 from sys.schemas WHERE name = 'DirectoryServices'))
BEGIN
	PRINT '+Creating schema [DirectoryServices]';
	EXEC ('CREATE SCHEMA [DirectoryServices] AUTHORIZATION [dbo]');
END
GO

PRINT '============================================'
PRINT '           Directory Services               '
PRINT '============================================'


IF (OBJECT_ID('DirectoryServices.fn_GetAccountInfo') IS NOT NULL)
BEGIN
	PRINT '-Dropping [DirectoryServices].[fn_GetAccountInfo]'
	DROP FUNCTION [DirectoryServices].[fn_GetAccountInfo]
END

PRINT '+Creating [DirectoryServices].[fn_GetAccountInfo]'
GO
--==========================================================================
-- @adRoot - root of searching eg. 'LDAP://OU=Sales,DC=Fabricam,DC=com'
-- @filter -  filter to be used for searching eg. '(&(objectCategory=group)'
-- @propertiesToLoad - list of properties to be retrieved eg. 'cn,50;ADsPath'
--                   - properties are separated by a semicolon and
--                   - and comma can be used to specify return length of the property
-- @searchScope - scope to be used for searching: {Base,OneLevel,Subtree}
-- @pageSize - specifies the PageSize for paged search - default is 1000
--			   it is possible to lower the value if there is a problem 
--             retrieving such amount of records at once.
-- @rowsLimit - represents maximum number of rows returned.
--              NULL or value less than 1 represents unlimited
--==========================================================================
CREATE FUNCTION [DirectoryServices].[fn_GetAccountInfo](
	@ntAccountName [nvarchar](256),  --Source string to apply RegEx
	@getGroupMembers bit,
	@recursive bit			--RegEx pattern to apply on the source string
)
RETURNS  TABLE (
	AccountType nvarchar(10),
	NTAccount nvarchar(256),
	Context nvarchar(64),
	SamAccountName nvarchar(256),
	Name nvarchar(256),
	DisplayName nvarchar(256),
	DistinguishedName nvarchar(256),
	UserPrincipalName nvarchar(128),
	[Sid] varbinary(85),
	[Guid] uniqueidentifier,
	[Description] nvarchar(4000),
	ParentNTAccount nvarchar(256),
	ParentSid varbinary(85)
) WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlDirectoryServicesAccounts].[Accounts].[GetAccountInfo]
GO

