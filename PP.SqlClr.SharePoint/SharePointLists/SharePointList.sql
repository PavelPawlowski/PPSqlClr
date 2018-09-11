IF (NOT EXISTS(SELECT 1 from sys.schemas WHERE name = 'SharePoint'))
BEGIN
	PRINT '+Creating schema [SharePoint]';
	EXEC ('CREATE SCHEMA [SharePoint] AUTHORIZATION [dbo]');
END
GO

PRINT '============================================'
PRINT '           SharePoint List               '
PRINT '============================================'


IF (OBJECT_ID('SharePoint.fn_GetSharePointListXml') IS NOT NULL)
BEGIN
	PRINT '-Dropping [SharePoint].[fn_GetSharePointListXml]'
	DROP PROCEDURE [SharePoint].[usp_GetSharePointListXml]
END

PRINT '+Creating [SharePoint].[fn_GetSharePointListXml]'
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
CREATE FUNCTION [SharePoint].[fn_GetSharePointListXml](
    @siteUrl nvarchar(255),
    @listGuid uniqueidentifier,
	@username nvarchar(128),
	@pwd nvarchar(128)
)
RETURNS xml
AS
EXTERNAL NAME [PPSqlSharePoint].[SharePointList].[GetSharePointListXml]
GO
