using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Server;
using System.Data.SqlTypes;
using System.Collections;

/// <summary>
/// Class constains functions fro string splitting
/// </summary>
public class StringSplitting
{
    /// <summary>
    /// represents returned row
    /// </summary>
    private struct StrRow
    {
        public StrRow(int rowId, SqlChars value)
        {
            RowId = rowId;
            Value = value;
        }

        public int RowId;
        public SqlChars Value;

    }

    private struct StrRowString
    {
        public StrRowString(int rowId, SqlString value)
        {
            RowId = rowId;
            Value = value;
        }

        public int RowId;
        public SqlString Value;
    }

    /// <summary>
    /// Splits CSV string using specified delimiter and bufferSize
    /// </summary>
    /// <param name="sourceString">Source string to be split</param>
    /// <param name="delimiter">Delimiter to be used to split source string</param>
    /// <param name="bufferSize">Buffer size (maximum length of item)</param>
    /// <returns>IEnumerable</returns>
    [SqlFunction(FillRowMethodName = "FillSplitString")]
    public static IEnumerable SplitStringBuffer(SqlString sourceString, SqlString delimiter, int bufferSize)
    {
        char[] buffer = new char[bufferSize];
        char delim = delimiter.Value[0];
        int rowNumber = 0;
        int chars = 0;
        char[] finalString;

        foreach (char chr in sourceString.Value)
        {
            if (chr == delim)
            {
                finalString = new char[chars];
                Array.Copy(buffer, finalString, chars);
                yield return new StrRow(++rowNumber, new SqlChars(finalString));
                chars = 0;
            }
            else
            {
                buffer[chars++] = chr;
            }
        }
        if (chars > 0)
        {
            finalString = new char[chars];
            Array.Copy(buffer, finalString, chars);
            yield return new StrRow(++rowNumber, new SqlChars(finalString));
        }
    }

    [SqlFunction(FillRowMethodName = "FillSplitStrings")]
    public static IEnumerable SplitStrings([SqlFacet(MaxSize = -1)]SqlChars sourceString, [SqlFacet(MaxSize = 128)]SqlChars delimiters)
    {
        char[] delims = delimiters.Value;
        int bufferSize = 10;
        int rowNumber = 0;
        int chars = 0;
        char[] buffer = new char[bufferSize];
        bool isDelim = false;

        if (delims.Length == 1)
        {
            char delim = delims[0];

            foreach (char chr in sourceString.Value)
            {
                if (chr == delim)
                {
                    if (chars > 0)
                    {
                        yield return new StrRowString(++rowNumber, new SqlString(new string(buffer, 0, chars)));
                        buffer = new char[bufferSize];
                        chars = 0;
                    }
                }
                else
                {
                    if (chars == bufferSize)
                    {
                        bufferSize = chars + chars / 2;
                        char[] newBuffer = new char[bufferSize];
                        buffer.CopyTo(newBuffer, 0);
                        buffer = newBuffer;
                    }
                    buffer[chars++] = chr;
                }
            }

            if (chars > 0)
                yield return new StrRow(++rowNumber, new SqlChars(buffer));
        }
        else
        {

            foreach (char chr in sourceString.Value)
            {
                foreach (char delim in delims)
                {
                    if (chr == delim && chars > 0)
                    {
                        yield return new StrRowString(++rowNumber, new SqlString(new string(buffer, 0, chars)));
                        buffer = new char[bufferSize];
                        chars = 0;
                    }

                    isDelim = true;
                    break;
                }

                if (isDelim)
                    continue;

                if (chars == bufferSize)
                {
                    bufferSize = chars + chars / 2;
                    char[] newBuffer = new char[bufferSize];
                    buffer.CopyTo(newBuffer, 0);
                    buffer = newBuffer;
                }

                buffer[chars++] = chr;
            }

            if (chars > 0)
                yield return new StrRow(++rowNumber, new SqlChars(buffer));

        }
    }

    /// <summary>
    /// Splits CSV string using specified delimiter
    /// This function auto allocates necessary buffer
    /// </summary>
    /// <param name="sourceString">Source string to be split</param>
    /// <param name="delimiter">Delimiter to be used to split source string</param>
    /// <returns>IEnumerable</returns>
    [SqlFunction(FillRowMethodName = "FillSplitString")]
    public static IEnumerable SplitString(SqlString sourceString, SqlString delimiter)
    {
        char[] buffer = new char[10];
        char delim = delimiter.Value[0];
        int rowNumber = 0;
        int chars = 0;
        char[] finalString;

        foreach (char chr in sourceString.Value)
        {
            if (chr == delim)
            {
                finalString = new char[chars];
                Array.Copy(buffer, finalString, chars);
                yield return new StrRow(++rowNumber, new SqlChars(finalString));
                chars = 0;
            }
            else
            {
                //increase buffer by half of it's current size
                if (chars == buffer.Length)
                {
                    char[] newBuffer = new char[chars + chars / 2];
                    buffer.CopyTo(newBuffer, 0);
                    buffer = newBuffer;
                }

                buffer[chars++] = chr;
            }
        }
        if (chars > 0)
        {
            finalString = new char[chars];
            Array.Copy(buffer, finalString, chars);
            yield return new StrRow(++rowNumber, new SqlChars(finalString));
        }
    }


    /// <summary>
    /// FillRow method to populate the output table
    /// </summary>
    /// <param name="obj">StrRow passed as object</param>
    /// <param name="rowId">rowID of returned string part (item number in the parsed string)</param>
    /// <param name="value">item of the splitted string</param>
    public static void FillSplitString(object obj, out int rowId, out SqlChars value)
    {
        StrRow r = (StrRow)obj;
        rowId = r.RowId;
        value = r.Value;
    }

    //public static void FillSplitString(object obj, out int rowId, out SqlString value)
    //{
    //    StrRowString r = (StrRowString)obj;
    //    rowId = r.RowId;
    //    value = r.Value;
    //}
}
