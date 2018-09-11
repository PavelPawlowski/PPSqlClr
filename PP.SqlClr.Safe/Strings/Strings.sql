IF (NOT EXISTS(SELECT 1 from sys.schemas WHERE name = 'ClrSafe'))
BEGIN
	PRINT '+Creating schema [ClrSafe]';
	EXEC ('CREATE SCHEMA [ClrSafe] AUTHORIZATION [dbo]');
END
GO

PRINT '============================================'
PRINT '         Common Strings functions           '
PRINT '============================================'

IF (OBJECT_ID('ClrSafe.fn_RemoveAccent') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrSafe].[fn_RemoveAccent]'
	DROP FUNCTION [ClrSafe].[fn_RemoveAccent]
END

PRINT '+Creating [ClrSafe].[fn_RemoveAccent]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2012/01/27
-- Description:	Removes accent (diacritics) from input string
-- =============================================
CREATE FUNCTION [ClrSafe].[fn_RemoveAccent](
	@sourceString nvarchar(max)  --Source string to remove accent
)
RETURNS nvarchar(4000)
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrSafe].[StringsCommon].[RemoveAccent]
GO

IF (OBJECT_ID('ClrSafe.fn_StringToBase64String') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrSafe].[fn_StringToBase64String]'
	DROP FUNCTION [ClrSafe].[fn_StringToBase64String]
END

PRINT '+Creating [ClrSafe].[fn_StringToBase64String]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2016/02/02
-- Description:	Returns BASE64 encoded representation of input string
-- =============================================
CREATE FUNCTION [ClrSafe].[fn_StringToBase64String](
	@sourceString nvarchar(max)  --Source string to be encoded to BASE64
)
RETURNS [nvarchar](max)
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrSafe].[StringsCommon].[StringToBase64String]
GO

IF (OBJECT_ID('ClrSafe.fn_BinaryToBase64String') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrSafe].[fn_BinaryToBase64String]'
	DROP FUNCTION [ClrSafe].[fn_BinaryToBase64String]
END

PRINT '+Creating [ClrSafe].[fn_BinaryToBase64String]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2016/02/02
-- Description:	Returns BASE64 encoded representation of the input data
-- =============================================
CREATE FUNCTION [ClrSafe].[fn_BinaryToBase64String](
	@inputData varbinary(max)  --Input data to be encoded to BASE64
)
RETURNS nvarchar(max)
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrSafe].[StringsCommon].[BinaryToBase64String]
GO


IF (OBJECT_ID('ClrSafe.fn_StringToByteArray') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrSafe].[fn_StringToByteArray]'
	DROP FUNCTION [ClrSafe].[fn_StringToByteArray]
END

PRINT '+Creating [ClrSafe].[fn_StringToByteArray]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2016/02/02
-- Description:	Returns Input string converted to byte array:
--				Supported Encodings: ASCII, BigEndianUnicode, Unicode, UTF32, UTF7, UTF8
-- =============================================
CREATE FUNCTION [ClrSafe].[fn_StringToByteArray](
	@encoding nvarchar(16)			--Encoding to be used for conversion. Supported Encodings: ASCII, BigEndianUnicode, Unicode, UTF32, UTF7, UTF8
	,@inputString nvarchar(max)		--Input data to be converted to byte array
)
RETURNS varbinary(max)
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrSafe].[StringsCommon].[StringToByteArray]
GO

IF (OBJECT_ID('ClrSafe.fn_ConvertStringEncoding') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrSafe].[fn_ConvertStringEncoding]'
	DROP FUNCTION [ClrSafe].[fn_ConvertStringEncoding]
END

PRINT '+Creating [ClrSafe].[fn_ConvertStringEncoding]'
GO
/* =============================================
   Author:		Pavel Pawlowski
   Create date: 2017/08/01
   Description:	Convert string encoded as hexa values from source to destination code page. Default fallback is being applied
================================================ */
CREATE FUNCTION [ClrSafe].[fn_ConvertStringEncoding](
	@sourceHexString        NVARCHAR(4000)  --Hexa encoded scring in source code page
	,@souceCodePage         INT		        --source code page
	,@destinationCodePage	INT				--destination code page
)		
RETURNS NVARCHAR(4000)
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrSafe].[StringsCommon].[ConvertStringEncoding]
GO

IF (OBJECT_ID('ClrSafe.fn_ConvertStringEncodingFallback') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrSafe].[fn_ConvertStringEncodingFallback]'
	DROP FUNCTION [ClrSafe].[fn_ConvertStringEncodingFallback]
END

PRINT '+Creating [ClrSafe].[fn_ConvertStringEncodingFallback]'
GO
/* =============================================
   Author:		Pavel Pawlowski
   Create date: 2017/08/01
   Description:	Convert string encoded as hexa values from source to destination code page with fallback and static replacements
                Static replacement is applied on source hexa string prior translation. The replacement has to be specified in the source code page
                Multiple replacements are applied left to right

                Replacements are semicollong separated and lookup value from replacement value is separated by double dash:

   Samples of Static Replacement:
                15:0D25
                0D25:15
                15:
                15:0D25;0B:FF;0C:;6A:5E
=============================================== */
CREATE FUNCTION [ClrSafe].[fn_ConvertStringEncodingFallback](
	@sourceHexString        NVARCHAR(4000)  --Hexa encoded scring in source code page
	,@souceCodePage         INT		        --source code page
	,@destinationCodePage	INT				--destination code page
    ,@encoderFallback       NVARCHAR(10)    --fallback replacement
    ,@staticReplacement     NVARCHAR(4000)  --Static replacement
)		
RETURNS NVARCHAR(4000)
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrSafe].[StringsCommon].[ConvertStringEncodingFallBack]
GO


IF (OBJECT_ID('ClrSafe.fn_ConvertBinaryEncoding') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrSafe].[fn_ConvertBinaryEncoding]'
	DROP FUNCTION [ClrSafe].[fn_ConvertBinaryEncoding]
END

PRINT '+Creating [ClrSafe].[fn_ConvertStringEncoding]'
GO
/* =============================================
   Author:		Pavel Pawlowski
   Create date: 2017/08/01
   Description:	Convert string encoded as binary array from source to destination code page. Default fallback is being applied
================================================ */
CREATE FUNCTION [ClrSafe].[fn_ConvertBinaryEncoding](
	@sourceBinaryString     VARBINARY(8000) --binary encoded scring in source code page
	,@souceCodePage         INT		        --source code page
	,@destinationCodePage	INT				--destination code page
)		
RETURNS NVARCHAR(4000)
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrSafe].[StringsCommon].[ConvertBinaryEncoding]
GO

IF (OBJECT_ID('ClrSafe.fn_ConvertBinaryEncodingFallBack') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrSafe].[fn_ConvertBinaryEncodingFallBack]'
	DROP FUNCTION [ClrSafe].[fn_ConvertBinaryEncodingFallBack]
END

PRINT '+Creating [ClrSafe].[fn_ConvertBinaryEncodingFallBack]'
GO
/* =============================================
   Author:		Pavel Pawlowski
   Create date: 2017/08/01
   Description:	Convert string encoded as binary array from source to destination code page with fallback and static replacements
                Static replacement is applied on source binary array prior translation. The replacement has to be specified in the source code page
                Multiple replacements are applied left to right

                Replacements are semicollong separated and lookup value from replacement value is separated by double dash:

   Samples of Static Replacement:
                15:0D25
                0D25:15
                15:
                15:0D25;0B:FF;0C:;6A:5E
================================================ */
CREATE FUNCTION [ClrSafe].[fn_ConvertBinaryEncodingFallBack](
	 @sourceBinaryString    VARBINARY(8000) --binary encoded scring in source code page
	,@souceCodePage         INT		        --source code page
	,@destinationCodePage	INT				--destination code page
    ,@encoderFallback       NVARCHAR(10)    --fallback replacement
    ,@staticReplacement     NVARCHAR(4000)  --Static replacement
)		
RETURNS NVARCHAR(4000)
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrSafe].[StringsCommon].[ConvertBinaryEncodingFallBack]
GO


PRINT '============================================'
PRINT '           Regular Expressions              '
PRINT '============================================'

IF (OBJECT_ID('ClrSafe.fn_RegExMatches') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrSafe].[fn_RegExMatches]'
	DROP FUNCTION [ClrSafe].[fn_RegExMatches]
END

PRINT '+Creating [ClrSafe].[fn_RegExMatches]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2010/12/28
-- Description:	Applies RegEx on the source string and returns all matches
-- =============================================
CREATE FUNCTION [ClrSafe].[fn_RegExMatches](
	@sourceString nvarchar(max),			--Source string to apply RegEx
	@pattern nvarchar(4000),				--RegEx pattern to apply on the source string
	@options [nvarchar](4000) = 'Compile'	--RegExOptions to be used
)
RETURNS  TABLE (
	[rowId] int NULL,				--ID of returned row
	[matchId] int NULL,				--ID of returned match
	[groupId] int NULL,				--ID of Group within a match (GroupID = 0 is whole match
	[groupName] nvarchar(128),		--Nam of the capture group
	[matchSuccess] bit,				--Identifies whether the match of the group was successfull
	[captureId] int,				--id of the group capture
	[value] nvarchar(4000) NULL		--value of particular group
) WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrSafe].[SQLRegEx].[RegExMatches]
GO


IF (OBJECT_ID('ClrSafe.fn_RegExMatchesReplace') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrSafe].[fn_RegExMatchesReplace]'
	DROP FUNCTION [ClrSafe].[fn_RegExMatchesReplace]
END

PRINT '+Creating [ClrSafe].[fn_RegExMatchesReplace]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2010/12/28
-- Description:	Applies RegEx on the source and perform replacement on the matches
-- =============================================
CREATE FUNCTION [ClrSafe].[fn_RegExMatchesReplace](
	@sourceString nvarchar(max),			--Source string to apply RegEx
	@pattern nvarchar(4000),				--RegEx to apply on the source string
	@replacement nvarchar(4000),			--Replacement pattern to apply on matches
	@options [nvarchar](4000) = 'Compile'	--RegExOptions to be used
)		
RETURNS  TABLE (
	[matchId] int NULL,					--ID of returnet match
	[match] nvarchar(4000) NULL,		--value of match
	[result] nvarchar(4000) NULL		--Replacement result
) WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrSafe].[SQLRegEx].[RegExMatchesReplace]
GO

IF (OBJECT_ID('ClrSafe.fn_RegExMatchesReplaceOnly') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrSafe].[fn_RegExMatchesReplaceOnly]'
	DROP FUNCTION [ClrSafe].[fn_RegExMatchesReplaceOnly]
END

PRINT '+Creating [ClrSafe].[fn_RegExMatchesReplaceOnly]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2010/12/28
-- Description:	Applies RegEx on the source and perform replacement on the matches - returns only replacements
-- =============================================
CREATE FUNCTION [ClrSafe].[fn_RegExMatchesReplaceOnly](
	@sourceString [nvarchar](max),			--Source string to apply RegEx
	@pattern [nvarchar](4000),				--RegEx pattern to apply on the source string
	@replacement [nvarchar](4000),			--Replacement pattern to apply on matches
	@options [nvarchar](4000) = 'Compile'	--RegExOptions to be used
)		
RETURNS  TABLE (
	[matchId] [int] NULL,				--ID of returnet match
	[result] [nvarchar](4000) NULL		--Replacement result
) WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrSafe].[SQLRegEx].[RegExMatchesReplaceOnly]
GO

IF (OBJECT_ID('ClrSafe.fn_RegExMatch') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrSafe].[fn_RegExMatch]'
	DROP FUNCTION [ClrSafe].[fn_RegExMatch]
END

PRINT '+Creating [ClrSafe].[fn_RegExMatch]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2010/12/28
-- Description:	Applies RegEx on the source and returns value of particular group of particular match
-- =============================================
CREATE FUNCTION [ClrSafe].[fn_RegExMatch](
	@sourceString [nvarchar](max),			--Source string to apply RegEx
	@pattern [nvarchar](4000),				--RegEx pattern to apply on the source string
	@matchId [int] = 1,						--ID of the match to return. MatchID = 1 is the first Match
	@groupId [int] = 0,						--ID of the Group to return it's value. @groupID = 0 is whole Match
	@options [nvarchar](4000) = 'Compile'	--RegExOptions to be used
)		
RETURNS [nvarchar](4000)
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrSafe].[SQLRegEx].[RegExMatch]
GO

IF (OBJECT_ID('ClrSafe.fn_RegExIsMatch') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrSafe].[fn_RegExIsMatch]'
	DROP FUNCTION [ClrSafe].[fn_RegExIsMatch]
END

PRINT '+Creating [ClrSafe].[fn_RegExIsMatch]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2018/05/05
-- Description:	Returns whether source scring matches the passed pattern
-- =============================================
CREATE FUNCTION [ClrSafe].[fn_RegExIsMatch](
	@sourceString [nvarchar](max),			--Source string to apply RegEx
	@pattern [nvarchar](4000),				--RegEx pattern to apply on the source string
	@options [nvarchar](4000) = 'Compile'	--RegExOptions to be used
)		
RETURNS bit
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrSafe].[SQLRegEx].[RegExIsMatch]
GO

IF (OBJECT_ID('ClrSafe.fn_RegExMatchName') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrSafe].[fn_RegExMatchName]'
	DROP FUNCTION [ClrSafe].[fn_RegExMatchName]
END

PRINT '+Creating [ClrSafe].[fn_RegExMatchName]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2010/12/28
-- Description:	Applies RegEx on the source and returns value of particular group of particular match
-- =============================================
CREATE FUNCTION [ClrSafe].[fn_RegExMatchName](
	@sourceString [nvarchar](max),			--Source string to apply RegEx
	@pattern [nvarchar](4000),				--RegEx pattern to apply on the source string
	@matchId [int] = 1,						--ID of the match to return. MatchID = 1 is the first Match
	@groupName [nvarchar](128) = NULL,		--Name of the group to return. @groupName IS NULL represents whole match
	@options [nvarchar](4000) = 'Compile'	--RegExOptions to be used
)		
RETURNS [nvarchar](4000)
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrSafe].[SQLRegEx].[RegExMatchName]
GO


IF (OBJECT_ID('ClrSafe.fn_RegExReplace') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrSafe].[fn_RegExReplace]'
	DROP FUNCTION [ClrSafe].[fn_RegExReplace]
END

PRINT '+Creating [ClrSafe].[fn_RegExReplace]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2010/12/28
-- Description:	Applies RegEx on the source string and return replacement on particular match
-- =============================================
CREATE FUNCTION [ClrSafe].[fn_RegExReplace](
	@sourceString [nvarchar](max),			--Source string to apply RegEx
	@pattern [nvarchar](4000),				--RegEx pattern to apply on the source string
	@replacement [nvarchar](400),			--Replacement pattern to apply on match
	@matchID [int] = 1,						--ID of the match to return. @matchID = 1 is the first match. IF @matchID=0 then all the patterns in the @sourceString are replaced
	@options [nvarchar](4000) = 'Compile'	--RegExOptions to be used
)		
RETURNS [nvarchar](4000)
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrSafe].[SQLRegEx].[RegExReplace]
GO


PRINT '============================================'
PRINT '         String Splitting functions         '
PRINT '============================================'


IF (OBJECT_ID('ClrSafe.fn_SplitString') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrSafe].[fn_SplitString]'
	DROP FUNCTION [ClrSafe].[fn_SplitString]
END

PRINT '+Creating [ClrSafe].[fn_SplitString]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2010/12/28
-- Description:	Splits string using particular delimiter (uses automatic buffer allocation)
-- =============================================
CREATE FUNCTION [ClrSafe].[fn_SplitString](
	@sourceString [nvarchar](max),		--Source string to be delimited
	@delimiter [nchar](1)				--Delimiter to be used for splitting string
)		
RETURNS TABLE (
	rowId [int],				--Id of returned Row
	value [nvarchar](4000)		--delimited value
)
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrSafe].[StringSplitting].[SplitString]
GO


IF (OBJECT_ID('ClrSafe.fn_SplitStringBuffer') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrSafe].[fn_SplitStringBuffer]'
	DROP FUNCTION [ClrSafe].[fn_SplitStringBuffer]
END

PRINT '+Creating [ClrSafe].[fn_SplitStringBuffer]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2010/12/28
-- Description:	Splits string using particular delimiter (uses automatic buffer allocation)
-- =============================================
CREATE FUNCTION [ClrSafe].[fn_SplitStringBuffer](
	@sourceString [nvarchar](max),		--Source string to be delimited
	@delimiter [nchar](1),				--Delimiter to be used for splitting string
	@bufferSize [int]					--Specifies the buffer size to be used for string splitting (maximum length of an item within a delimited string
)		
RETURNS TABLE (
	rowId [int],				--Id of returned Row
	value [nvarchar](4000)		--delimited value
)
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrSafe].[StringSplitting].[SplitStringBuffer]
GO


PRINT '============================================'
PRINT '          Fuzzy strings processing          '
PRINT '============================================'

IF (OBJECT_ID('ClrSafe.fn_LevenshteinDistance') IS NOT NULL)
BEGIN
	PRINT '-Dropping [ClrSafe].[fn_LevenshteinDistance]'
	DROP FUNCTION [ClrSafe].[fn_LevenshteinDistance]
END

PRINT '+Creating [ClrSafe].[fn_LevenshteinDistance]'
GO
-- =============================================
-- Author:		Pavel Pawlowski
-- Create date: 2010/12/28
-- Description:	Calculates the Levenshtein Distance between two strings
-- =============================================
CREATE FUNCTION [ClrSafe].[fn_LevenshteinDistance](
	@firstString [nvarchar](4000),		--first string for the distance calculation
	@secondString [nvarchar](4000),		--second string for the distance calculation
	@ingoreCase [bit] = 1				--Specifies whether ignore case in comparison
)		
RETURNS [int]
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [PPSqlClrSafe].[FuzzyStrings].[LevenshteinDistance]
GO