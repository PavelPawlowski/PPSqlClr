IF (NOT EXISTS(SELECT 1 from sys.schemas WHERE name = 'DirectoryServices'))
BEGIN
	PRINT '+Creating schema [DirectoryServices]';
	EXEC ('CREATE SCHEMA [DirectoryServices] AUTHORIZATION [dbo]');
END
GO

PRINT '============================================'
PRINT '            Active Directory                '
PRINT '============================================'


IF (OBJECT_ID('DirectoryServices.usp_QueryAD') IS NOT NULL)
BEGIN
	PRINT '-Dropping [DirectoryServices].[usp_QueryAD]'
	DROP PROCEDURE [DirectoryServices].[usp_QueryAD]
END

PRINT '+Creating [DirectoryServices].[usp_QueryAD]'
GO
--==========================================================================
-- @adRoot - root of searching eg. 'LDAP://OU=Sales,DC=Fabricam,DC=com'
-- @filter -  filter to be used for searching eg. '(&(objectCategory=group)'
-- @propertiesToLoad - list of properties to be retrieved eg. 'cn,50;ADsPath' Format is 'PropertyName,Len,Decoder,Encoder;Propertyname1,Len,Decoder,Encoder'
--                   - properties are separated by a semicolon and
--                   - and comma can be used to specify return length of the property
--					 - third and fouth part of the comma separated sequence defines Decoder and Encoder
--					 - Encoder and Decoder can be one of BINARY,ASCII,Default,Unicode,BigEndianUnicode,UTF7,UTF8,UTF32
--					 - Decoder is used when property is in binary Format. Encoder is used to encode non binary properties to binary format
-- @searchScope - scope to be used for searching: {Base,OneLevel,Subtree}
-- @pageSize - specifies the PageSize for paged search - default is 1000
--			   it is possible to lower the value if there is a problem 
--             retrieving such amount of records at once.
-- @rowsLimit - represents maximum number of rows returned.
--              NULL or value less than 1 represents unlimited
--==========================================================================
CREATE PROCEDURE [DirectoryServices].[usp_QueryAD]
    @adRoot nvarchar(255),
    @filter nvarchar(255),
    @propertiesToLoad nvarchar(255),
    @searchScope nvarchar(8),
	@pageSize int = 1000,
	@rowsLimit int = 0
AS
EXTERNAL NAME [PPSqlDirectoryServices].[ActiveDirectory].[QueryAD]
GO


IF (OBJECT_ID('DirectoryServices.usp_QueryADUname') IS NOT NULL)
BEGIN
	PRINT '-Dropping [DirectoryServices].[usp_QueryADUname]'
	DROP PROCEDURE [DirectoryServices].[usp_QueryADUname]
END

PRINT '+Creating [DirectoryServices].[usp_QueryADUname]'
GO
--==========================================================================
-- @userName - username to be used for authentificaiton to AD
-- @password - password to be used for authentification to AD
-- @adRoot - root of searching eg. 'LDAP://OU=Sales,DC=Fabricam,DC=com'
-- @filter -  filter to be used for searching eg. '(&(objectCategory=group)'
-- @propertiesToLoad - list of properties to be retrieved eg. 'cn,50;ADsPath' Format is 'PropertyName,Len,Decoder,Encoder;Propertyname1,Len,Decoder,Encoder'
--                   - properties are separated by a semicolon and
--                   - and comma can be used to specify return length of the property
--					 - third and fouth part of the comma separated sequence defines Decoder and Encoder
--					 - Encoder and Decoder can be one of BINARY,ASCII,Default,Unicode,BigEndianUnicode,UTF7,UTF8,UTF32
--					 - Decoder is used when property is in binary Format. Encoder is used to encode non binary properties to binary format
-- @searchScope - scope to be used for searching: {Base,OneLevel,Subtree}
-- @pageSize - specifies the PageSize for paged search - default is 1000
--			   it is possible to lower the value if there is a problem 
--             retrieving such amount of records at once.
-- @rowsLimit - represents maximum number of rows returned.
--              NULL or value less than 1 represents unlimited
--==========================================================================
CREATE PROCEDURE [DirectoryServices].[usp_QueryADUname]
    @userName nvarchar(255),
    @password nvarchar(255),
    @adRoot nvarchar(255),
    @filter nvarchar(255),
    @propertiesToLoad nvarchar(255),
    @searchScope nvarchar(8),
	@pageSize int = 1000,
	@rowsLimit int = 0
AS
EXTERNAL NAME [PPSqlDirectoryServices].[ActiveDirectory].[QueryADUName]
GO


IF (OBJECT_ID('DirectoryServices.usp_QueryADAuth') IS NOT NULL)
BEGIN
	PRINT '-Dropping [DirectoryServices].[usp_QueryADAuth]'
	DROP PROCEDURE [DirectoryServices].[usp_QueryADAuth]
END

PRINT '+Creating [DirectoryServices].[usp_QueryADAuth]'
GO
--==========================================================================
-- @userName - username to be used for authentificaiton to AD
-- @password - password to be used for authentification to AD
-- @authType - Authentification Type to be used for AD Authentification
--             {None,Secure,Encryption,SecureSocketsLayer,ReadonlyServer,Anonymous,
--             FastBind,Signing,Sealing,Delegation,ServerBind}
-- @adRoot - root of searching eg. 'LDAP://OU=Sales,DC=Fabricam,DC=com'
-- @filter -  filter to be used for searching eg. '(&(objectCategory=group)'
-- @propertiesToLoad - list of properties to be retrieved eg. 'cn,50;ADsPath' Format is 'PropertyName,Len,Decoder,Encoder;Propertyname1,Len,Decoder,Encoder'
--                   - properties are separated by a semicolon and
--                   - and comma can be used to specify return length of the property
--					 - third and fouth part of the comma separated sequence defines Decoder and Encoder
--					 - Encoder and Decoder can be one of BINARY,ASCII,Default,Unicode,BigEndianUnicode,UTF7,UTF8,UTF32
--					 - Decoder is used when property is in binary Format. Encoder is used to encode non binary properties to binary format
-- @searchScope - scope to be used for searching: {Base,OneLevel,Subtree}
-- @pageSize - specifies the PageSize for paged search - default is 1000
--			   it is possible to lower the value if there is a problem 
--             retrieving such amount of records at once.
-- @rowsLimit - represents maximum number of rows returned.
--              NULL or value less than 1 represents unlimited
--==========================================================================
CREATE PROCEDURE [DirectoryServices].[usp_QueryADAuth]
    @userName nvarchar(255),
    @password nvarchar(255),
    @authType nvarchar(20),
    @adRoot nvarchar(255),
    @filter nvarchar(255),
    @propertiesToLoad nvarchar(255),
    @searchScope nvarchar(8),
	@pageSize int = 1000,
	@rowsLimit int = 0
AS
EXTERNAL NAME [PPSqlDirectoryServices].[ActiveDirectory].[QueryADAuth]
GO



IF (OBJECT_ID('DirectoryServices.fn_ADAuthenticate') IS NOT NULL)
BEGIN
	PRINT '-Dropping [DirectoryServices].[fn_ADAuthenticate]'
	DROP FUNCTION [DirectoryServices].[fn_ADAuthenticate]
END

PRINT '+Creating [DirectoryServices].[fn_ADAuthenticate]'
GO
--==========================================================================
-- @adRoot - root of searching eg. 'LDAP://OU=Sales,DC=Fabricam,DC=com'
-- @userName - username to be used for authentificaiton to AD
-- @password - password to be used for authentification to AD
-- @authType - Authentification Type to be used for AD Authentification
--             {None,Secure,Encryption,SecureSocketsLayer,ReadonlyServer,Anonymous,
--             FastBind,Signing,Sealing,Delegation,ServerBind}
-- @throwExceptions - throws authentication exceptions
-- If successfully authenticated returns 1 otherwise 0
--==========================================================================
CREATE FUNCTION [DirectoryServices].[fn_ADAuthenticate] (
    @adRoot nvarchar(255),
    @userName nvarchar(255),
    @password nvarchar(255),
    @authType nvarchar(20),
    @throwExceptions bit
)
RETURNS bit
AS
EXTERNAL NAME [PPSqlDirectoryServices].[ActiveDirectory].[Authenticate]
GO

IF (OBJECT_ID('DirectoryServices.fn_ADAuthenticateDefault') IS NOT NULL)
BEGIN
	PRINT '-Dropping [DirectoryServices].[fn_ADAuthenticateDefault]'
	DROP FUNCTION [DirectoryServices].[fn_ADAuthenticateDefault]
END

PRINT '+Creating [DirectoryServices].[fn_ADAuthenticateDefault]'
GO
--==========================================================================
-- @adRoot - root of searching eg. 'LDAP://OU=Sales,DC=Fabricam,DC=com'
-- @userName - username to be used for authentificaiton to AD
-- @password - password to be used for authentification to AD
-- @throwExceptions - throws authentication exceptions
-- If successfully authenticated returns 1 otherwise 0
--==========================================================================
CREATE FUNCTION [DirectoryServices].[fn_ADAuthenticateDefault] (
    @adRoot nvarchar(255),
    @userName nvarchar(255),
    @password nvarchar(255),
    @throwExceptions bit
)
RETURNS bit
AS
EXTERNAL NAME [PPSqlDirectoryServices].[ActiveDirectory].[AuthenticateDefault]
GO

IF (OBJECT_ID('DirectoryServices.usp_ADAuthenticate') IS NOT NULL)
BEGIN
	PRINT '-Dropping [DirectoryServices].[usp_ADAuthenticate]'
	DROP PROCEDURE [DirectoryServices].[usp_ADAuthenticate]
END

PRINT '+Creating [DirectoryServices].[usp_ADAuthenticate]'
GO
--==========================================================================
-- @adRoot - root of searching eg. 'LDAP://OU=Sales,DC=Fabricam,DC=com'
-- @userName - username to be used for authentificaiton to AD
-- @password - password to be used for authentification to AD
-- @authType - Authentification Type to be used for AD Authentification
--             {None,Secure,Encryption,SecureSocketsLayer,ReadonlyServer,Anonymous,
--             FastBind,Signing,Sealing,Delegation,ServerBind}
-- @throwExceptions - throws authentication exceptions
-- If successfully authenticated returns 1 otherwise 0
--==========================================================================
CREATE PROCEDURE [DirectoryServices].[usp_ADAuthenticate]
    @adRoot nvarchar(255),
    @userName nvarchar(255),
    @password nvarchar(255),
    @authType nvarchar(20),
    @throwExceptions bit
AS
EXTERNAL NAME [PPSqlDirectoryServices].[ActiveDirectory].[SPAuthenticate]
GO

IF (OBJECT_ID('DirectoryServices.usp_ADAuthenticateDefault') IS NOT NULL)
BEGIN
	PRINT '-Dropping [DirectoryServices].[usp_ADAuthenticateDefault]'
	DROP PROCEDURE [DirectoryServices].[usp_ADAuthenticateDefault]
END

PRINT '+Creating [DirectoryServices].[usp_ADAuthenticateDefault]'
GO
--==========================================================================
-- @adRoot - root of searching eg. 'LDAP://OU=Sales,DC=Fabricam,DC=com'
-- @userName - username to be used for authentificaiton to AD
-- @password - password to be used for authentification to AD
-- @throwExceptions - throws authentication exceptions
-- If successfully authenticated returns 1 otherwise 0
--==========================================================================
CREATE PROCEDURE [DirectoryServices].[usp_ADAuthenticateDefault]
    @adRoot nvarchar(255),
    @userName nvarchar(255),
    @password nvarchar(255),
    @throwExceptions bit
AS
EXTERNAL NAME [PPSqlDirectoryServices].[ActiveDirectory].[SPAuthenticateDefault]
GO