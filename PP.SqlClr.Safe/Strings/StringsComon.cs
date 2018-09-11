using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Server;
using System.Data.SqlTypes;
using System.Globalization;

public class StringsCommon
{
    /// <summary>
    /// Removes Accent (Diacritics) from string
    /// </summary>
    /// <param name="sourceString">Source string fro wchich accent should be removed</param>
    /// <returns>string without accent</returns>
    [SqlFunction(IsDeterministic = true)]
    public static SqlChars RemoveAccent(SqlString sourceString)
    {
        if (sourceString.IsNull)
            return SqlChars.Null;

        string normalized = sourceString.Value.Normalize(NormalizationForm.FormD);
        
        StringBuilder output = new StringBuilder(sourceString.Value.Length);
        
        foreach (char ch in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                output.Append(ch);
        }
        return new SqlChars(output.ToString());
    }

    /// <summary>
    /// Returns BASE64 encoded representation of input string
    /// </summary>
    /// <param name="sourceString">Input string to be BASE64 encoded</param>
    /// <returns>BASE64 representation of input string</returns>
    [SqlFunction(IsDeterministic = true, IsPrecise = true, Name = "fn_StringToBase64")]
    public static SqlChars StringToBase64String(SqlString sourceString)
    {
        if (sourceString.IsNull)
            return SqlChars.Null;

        string base64String = Convert.ToBase64String(Encoding.Unicode.GetBytes(sourceString.Value));

        return new SqlChars(base64String);
    }

    /// <summary>
    /// Returns BASE64 encoded representation of input data
    /// </summary>
    /// <param name="inputData">Input Data to be BASE64 encoded</param>
    /// <returns>BASE64 representation of input data</returns>
    [SqlFunction(IsDeterministic = true, IsPrecise = true, Name = "fn_BinaryToBase64String")]
    public static SqlChars BinaryToBase64String(SqlBytes inputData)
    {
        if (inputData.IsNull)
            return SqlChars.Null;

        string base64String = Convert.ToBase64String(inputData.Buffer);

        return new SqlChars(base64String);

    }

    /// <summary>
    /// Return input string as byte array using specified encoding
    /// </summary>
    /// <param name="encoding">Encoding to be used for conversion to byte aray.
    /// Supported Encodings: ASCII, BigEndianUnicode, Unicode, UTF32, UTF7, UTF8</param>
    /// <param name="inputString">Source string to be converted to byte array</param>
    /// <returns>Byte Arraty representaion of input string</returns>
    [SqlFunction(IsDeterministic = true, IsPrecise = true, Name = "fn_StringToByteArray")]
    public static SqlBytes StringToByteArray(SqlString encoding, SqlString inputString)
    {
        if (inputString.IsNull)
            return SqlBytes.Null;

        string enc = encoding.IsNull ? string.Empty : encoding.Value;
        string stringEncoding = enc.ToUpper();

        Encoding e;
        switch(stringEncoding)
        {
            case "ASCII":
                e = Encoding.ASCII;
                break;
            case "BIGENDIANUNICODE":
                e = Encoding.BigEndianUnicode;
                break;
            case "UNICODE":
            case "UTF16":
                e = Encoding.Unicode;
                break;
            case "UTF32":
                e = Encoding.UTF32;
                break;
            case "UTF7":
                e = Encoding.UTF7;
                break;
            case "UTF8":
                e = Encoding.UTF8;
                break;
            default:
                throw new System.ArgumentException(string.Format("Unsupported encoding '{0}'. Only following ecodings are supported: ASCII, BigEndianUnicode, Unicode, UTF32, UTF7, UTF8", enc), "encoding");
        }

        var buffer = e.GetBytes(inputString.Value);

        return new SqlBytes(buffer);
    }


    /// <summary>
    /// Converts source HEX String from source CodePage to destination CodePage
    /// Users Default fallback
    /// </summary>
    /// <param name="sourceHexString">Source String in HEX format</param>
    /// <param name="sourceCodePage">Source Code Page</param>
    /// <param name="destinationCodePage">Destination Code Page</param>
    /// <returns>Source string encoded in destination code page</returns>
    [SqlFunction(IsDeterministic = true, IsPrecise = true, Name = "fn_ConvertStringEncoding")]
    public static SqlChars ConvertStringEncoding(SqlString sourceHexString, int sourceCodePage, int destinationCodePage)
    {
        return ConvertStringEncodingFallBack(sourceHexString, sourceCodePage, destinationCodePage, null, null);
    }

    /// <summary>
    /// Converts source HEX String from source CodePage to destination CodePage
    /// </summary>
    /// <param name="sourceHexString">Source String in HEX format</param>
    /// <param name="sourceCodePage">Source Code Page</param>
    /// <param name="destinationCodePage">Destination Code Page</param>
    /// <param name="encoderFallBack">Encoder Fallback String to be used in case encoding cannot be handled to destiantion code page. When NULL default fallback is being used</param>
    /// <param name="staticReplacement">Static replacement done on the source string prior decoding. 
    /// Format is: XX:YY;AA:BBCC;DD:;EEFF:YY Double dash separates lookup and replacement value. If no replacement is specified after the double dash, the lookup value is removed
    /// Semicolong separates individual replacements.
    /// Replacements are applied Left to Right</param>
    /// <returns>Source string encoded in destination code page</returns>
    [SqlFunction(IsDeterministic = true, IsPrecise = true, Name = "fn_ConvertStringEncodingFallBack")]
    public static SqlChars ConvertStringEncodingFallBack(SqlString sourceHexString, int sourceCodePage, int destinationCodePage, String encoderFallBack, SqlString staticReplacement)
    {
        if (sourceHexString.IsNull)
                return SqlChars.Null;

        StringBuilder srcStr = new StringBuilder(sourceHexString.ToString());

        string sourceStr = srcStr.ToString();

        byte[] buffer = new byte[sourceStr.Length / 2];

        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = byte.Parse(sourceStr.Substring(i * 2, 2), NumberStyles.HexNumber);
        }

        return ConvertBinaryEncodingFallBack(new SqlBytes(buffer), sourceCodePage, destinationCodePage, encoderFallBack, staticReplacement);
    }

    /// <summary>
    /// Converts source string in binary arracy from source CodePage to destination CodePage
    /// Users Default fallback
    /// </summary>
    /// <param name="sourceHexString">Source String in HEX format</param>
    /// <param name="sourceCodePage">Source Code Page</param>
    /// <param name="destinationCodePage">Destination Code Page</param>
    /// <returns>Source string encoded in destination code page</returns>
    [SqlFunction(IsDeterministic = true, IsPrecise = true, Name = "fn_ConvertBinaryEncoding")]
    public static SqlChars ConvertBinaryEncoding(SqlBytes sourceBinaryString, int sourceCodePage, int destinationCodePage)
    {
        return ConvertBinaryEncodingFallBack(sourceBinaryString, sourceCodePage, destinationCodePage, null, null);
    }

    /// <summary>
    /// Converts source string in binary arracy from source CodePage to destination CodePage
    /// </summary>
    /// <param name="sourceHexString">Source String in binary array</param>
    /// <param name="sourceCodePage">Source Code Page</param>
    /// <param name="destinationCodePage">Destination Code Page</param>
    /// <param name="encoderFallBack">Encoder Fallback String to be used in case encoding cannot be handled to destiantion code page. When NULL default fallback is being used</param>
    /// <param name="staticReplacement">Static replacement done on the source string prior decoding. 
    /// Format is: XX:YY;AA:BBCC;DD:;EEFF:YY Double dash separates lookup and replacement value. If no replacement is specified after the double dash, the lookup value is removed
    /// Semicolong separates individual replacements.
    /// Replacements are applied Left to Right</param>
    /// <returns>Source string encoded in destination code page</returns>
    [SqlFunction(IsDeterministic = true, IsPrecise = true, Name = "fn_ConvertBinaryEncodingFallBack")]
    public static SqlChars ConvertBinaryEncodingFallBack(SqlBytes sourceBinaryString, int sourceCodePage, int destinationCodePage, SqlString encoderFallBack, SqlString staticReplacement)
    {
        if (sourceBinaryString.IsNull)
            return SqlChars.Null;

    
        byte[] sourceData = sourceBinaryString.Buffer;

        //If there are static replacement, handle those prior any transofmation
        if (!staticReplacement.IsNull && !string.IsNullOrEmpty(staticReplacement.Value))
        {
            var replacementStrings = staticReplacement.Value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string replacement in replacementStrings)
            {
                string[] srcDest = replacement.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                if (srcDest.Length > 0)
                {
                    byte[] srcBuf = new byte[srcDest[0].Length / 2];

                    for (int i = 0; i < srcBuf.Length; i++)
                    {
                        srcBuf[i] = byte.Parse(srcDest[0].Substring(i * 2, 2), NumberStyles.HexNumber);
                    }

                    byte[] destBuff = srcDest.Length > 1 ? new byte[srcDest[1].Length / 2] : null;
                    if (destBuff != null)
                    {
                        for (int i = 0; i < destBuff.Length; i++)
                        {
                            destBuff[i] = byte.Parse(srcDest[1].Substring(i * 2, 2), NumberStyles.HexNumber);
                        }
                    }
                    //Apply the replacement
                    sourceData = Replace(sourceData, srcBuf, destBuff);
                }
            }

        }


        //Get Decoders and Encoders with appropriate Fallbacks
        EncoderFallback fb = encoderFallBack.IsNull ? new EncoderReplacementFallback() : new EncoderReplacementFallback(encoderFallBack.Value);
        Encoding destinationEncoding = Encoding.GetEncoding(destinationCodePage, fb, new DecoderExceptionFallback());
        Encoding sourceEncoding = Encoding.GetEncoding(sourceCodePage);

        var buffer = Encoding.Convert(sourceEncoding, destinationEncoding, sourceData);
        var str = destinationEncoding.GetString(buffer);

        return new SqlChars(str);
    }

    private static byte[] Replace(byte[] input, byte[] pattern, byte[] replacement)
    {
        if (pattern.Length == 0)
        {
            return input;
        }

        List<byte> result = new List<byte>(input.Length);

        int i;

        for (i = 0; i <= input.Length - pattern.Length; i++)
        {
            bool foundMatch = true;
            for (int j = 0; j < pattern.Length; j++)
            {
                if (input[i + j] != pattern[j])
                {
                    foundMatch = false;
                    break;
                }
            }

            if (foundMatch)
            {
                if (replacement != null && replacement.Length > 0)
                    result.AddRange(replacement);
                i += pattern.Length - 1;
            }
            else
            {
                result.Add(input[i]);
            }
        }

        for (; i < input.Length; i++)
        {
            result.Add(input[i]);
        }

        return result.ToArray();
    }

}
