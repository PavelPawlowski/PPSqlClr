IF (NOT EXISTS(SELECT 1 from sys.schemas WHERE name = 'SharePoint'))
BEGIN
	PRINT '+Creating schema [SharePoint]';
	EXEC ('CREATE SCHEMA [SharePoint] AUTHORIZATION [dbo]');
END
GO

PRINT '============================================'
PRINT '             SharePointList                 '
PRINT '============================================'


IF (OBJECT_ID('SharePoint.fn_GetSharePointListXml') IS NOT NULL)
BEGIN
	PRINT '-Dropping [SharePoint].[fn_GetSharePointListXml]'
	DROP FUNCTION [SharePoint].[fn_GetSharePointListXml]
END

PRINT '+Creating [SharePoint].[fn_GetSharePointListXml]'
GO
--==========================================================================
-- @siteUrl		- SITE URL from which the list data should be retrieved
-- @@listGuid	- GUID of the list from which the data should beretrieved
-- @userName	- Windows username under which the lsit should be accessed. If Null current credentials are used.
-- @pwd			- Password of the user
--==========================================================================
CREATE FUNCTION [SharePoint].[fn_GetSharePointListXml](
	@siteUrl [nvarchar](256),	 --Site URL
	@listGuid uniqueIdentifier,  --List GUID
	@userName nvarchar(128),	 --Windows username un
	@pwd nvarchar(128)			 --password
)
RETURNS xml
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlSharePoint].[SharePointList].[GetSharePointListXml]
GO


IF (OBJECT_ID('SharePoint.fn_GetSharePointListViewXml') IS NOT NULL)
BEGIN
	PRINT '-Dropping [SharePoint].[fn_GetSharePointListViewXml]'
	DROP FUNCTION [SharePoint].[fn_GetSharePointListViewXml]
END

PRINT '+Creating [SharePoint].[fn_GetSharePointListViewXml]'
GO
--==========================================================================
-- @siteUrl		- SITE URL from which the list data should be retrieved
-- @@listGuid	- GUID of the list from which the data should beretrieved
-- @ViewGuid	- GUID of the view to be used for retrieval
-- @fieldsList	- List of the Fields to retrieve
-- @Filter		- Filter condition in formant "FieldName>=Value&FieldName=Value&FieldName<=Value
-- @userName	- Windows username under which the lsit should be accessed. If Null current credentials are used.
-- @pwd			- Password of the user
--==========================================================================
CREATE FUNCTION [SharePoint].[fn_GetSharePointListViewXml](
	@siteUrl [nvarchar](256),	 --Site URL
	@listGuid uniqueIdentifier,  --List GUID
	@viewGuid uniqueIdentifier,	 --GUID of the view to be used to get data
	@fieldsList nvarchar(256),	 --Fields list to be retrieved
	@filter nvarchar(256),		 --Filter to be used during retrieval
	@userName nvarchar(128),	 --Windows username un
	@pwd nvarchar(128)			 --password
)
RETURNS xml
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlSharePoint].[SharePointList].[GetSharePointListViewXml]
GO
IF (OBJECT_ID('SharePoint.fn_GetSharePointListUrlXml') IS NOT NULL)
BEGIN
	PRINT '-Dropping [SharePoint].[fn_GetSharePointListUrlXml]'
	DROP FUNCTION [SharePoint].[fn_GetSharePointListUrlXml]
END

PRINT '+Creating [SharePoint].[fn_GetSharePointListUrlXml]'
GO
--==========================================================================
-- @listUrl		- Complete List URL for data retrieval
-- @userName	- Windows username under which the lsit should be accessed. If Null current credentials are used.
-- @pwd			- Password of the user
--==========================================================================
CREATE FUNCTION [SharePoint].[fn_GetSharePointListUrlXml](
	@listUrl [nvarchar](256),	 --Site URL
	@userName nvarchar(128),	 --Windows username un
	@pwd nvarchar(128)			 --password
)
RETURNS xml
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlSharePoint].[SharePointList].[GetSharePointListUrlXml]
GO
