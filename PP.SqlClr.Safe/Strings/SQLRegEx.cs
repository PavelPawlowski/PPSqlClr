using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Text.RegularExpressions;
using System.Collections;

/// <summary>
/// Regular expressions for SQL
/// </summary>
public class SQLRegEx
{
    #region Internal structures
    /// <summary>
    /// Private struct for passsing matches of the RegExMatches to the FillRow method
    /// </summary>
    private struct RegExRow
    {
        /// <summary>
        /// Private struct for passing matches of the RegExMatches to the FillRow method
        /// </summary>
        /// <param name="rowId">ID of the Row</param>
        /// <param name="matchId">ID of the Match</param>
        /// <param name="groupId">ID of the Group within the Match</param>
        /// <param name="groupName">Name of the capture group</param>
        /// <param name="matchSuccess">Identifies whetehr macht was successfull</param>
        /// <param name="captureId">ID of the capture</param>
        /// <param name="value">Value of the particular group</param>
        public RegExRow(int rowId, int matchId, int groupId, SqlString groupName, SqlBoolean matchSuccess, int captureId, SqlChars value)
        {
            RowId = rowId;
            MatchId = matchId;
            GroupId = groupId;
            GroupName = groupName;
            MatchSuccess = matchSuccess;
            CaptureId = captureId;
            Value = value;
        }

        public int RowId;
        public int MatchId;
        public int GroupId;
        public SqlString GroupName;
        public SqlBoolean MatchSuccess;
        public int CaptureId;
        public SqlChars Value;
    }

    private struct RegExRowReplace
    {
        public RegExRowReplace(int matchID, SqlChars match, SqlChars result)
        {
            MatchID = matchID;
            Match = match;
            Result = result;
        }

        public int MatchID;
        public SqlChars Match;
        public SqlChars Result;
    }

    #endregion

    #region RegEx functions
    /// <summary>
    /// Applies Regular Expression on the Source string and returns value of particular group from withing a specified match
    /// </summary>
    /// <param name="sourceString">Source string on which the regular expression should be applied</param>
    /// <param name="pattern">Regular Expression pattern</param>
    /// <param name="options">RegexOptions to be used during regular expression application</param>
    /// <returns>Returns whether source string matches pattern</returns>
    [SqlFunction(IsDeterministic = true)]
    public static SqlBoolean RegExIsMatch(SqlString sourceString, SqlString pattern, SqlString options)
    {
        RegexOptions regOptions = RegexOptions.None;

        RegexOptionsEnum.TyPrase(options.Value, out regOptions);

        regOptions |= RegexOptions.Compiled;

        Regex r = new Regex(pattern.Value, regOptions);

        return new SqlBoolean(r.IsMatch(sourceString.Value));
    }


    /// <summary>
    /// Applies Regular Expression on the Source string and returns value of particular group from withing a specified match
    /// </summary>
    /// <param name="sourceString">Source string on which the regular expression should be applied</param>
    /// <param name="pattern">Regular Expression pattern</param>
    /// <param name="matchId">ID of the Match to be returned 1 inex-based</param>
    /// <param name="groupId">ID of the group from within a match to return. GroupID 0 returns complete match</param>
    /// <param name="options">RegexOptions to be used during regular expression application</param>
    /// <returns>Value of the Group from within a Match</returns>
    [SqlFunction(IsDeterministic = true)]
    public static SqlChars RegExMatch(SqlString sourceString, SqlString pattern, int matchId, int groupId, SqlString options)
    {
        RegexOptions regOptions = RegexOptions.None;

        RegexOptionsEnum.TyPrase(options.Value, out regOptions);

        regOptions |= RegexOptions.Compiled;

        Match m = null;
        Regex r = new Regex(pattern.Value, regOptions);

        if (matchId == 1)
        {
            m = r.Match(sourceString.Value);
        }
        else if (matchId > 1)
        {
            MatchCollection mc = r.Matches(sourceString.Value);
            m = mc != null && mc.Count > matchId - 1 ? mc[matchId - 1] : null;
        }

        return m != null && m.Groups.Count > groupId ? new SqlChars(m.Groups[groupId].Value) : SqlChars.Null;
    }

    /// <summary>
    /// Applies Regular Expression on the Source string and returns value of particular group from withing a specified match
    /// </summary>
    /// <param name="sourceString">Source string on which the regular expression should be applied</param>
    /// <param name="pattern">Regular Expression pattern</param>
    /// <param name="matchId">ID of the Match to be returned 1 inex-based</param>
    /// <param name="groupname">Group name from within a match to return. NULL or empty string teruns complete match</param>
    /// <param name="options">RegexOptions to be used during regular expression application</param>
    /// <returns>Value of the Group from within a Match</returns>
    [SqlFunction(IsDeterministic = true)]
    public static SqlChars RegExMatchName(SqlString sourceString, SqlString pattern, int matchId, SqlString groupname, SqlString options)
    {
        RegexOptions regOptions = RegexOptions.None;

        RegexOptionsEnum.TyPrase(options.Value, out regOptions);

        regOptions |= RegexOptions.Compiled;

        Match m = null;
        Regex r = new Regex(pattern.Value, regOptions);

        if (matchId == 1)
        {
            m = r.Match(sourceString.Value);
        }
        else if (matchId > 1)
        {
            MatchCollection mc = r.Matches(sourceString.Value);
            m = mc != null && mc.Count > matchId - 1 ? mc[matchId - 1] : null;
        }

        if (groupname.IsNull || string.IsNullOrEmpty(groupname.Value))
            return new SqlChars(m.Value);
        else
            return m != null ? new SqlChars(m.Groups[groupname.Value].Value) : SqlChars.Null;
    }

    /// <summary>
    /// Applies Regular Expression on the Source string, takes apropriate match and aplies e replace on it.
    /// </summary>
    /// <param name="sourceString">Source string on which the regular expression should be applied</param>
    /// <param name="pattern">Regular Expression pattern</param>
    /// <param name="replacement">Replacement to be used on match</param>
    /// <param name="matchId">ID of the Match to be returned 1 inex-based</param>
    /// <param name="options">RegexOptions to be used during regular expression application</param>
    /// <returns>Value of the Group from within a Match</returns>
    [SqlFunction(IsDeterministic = true)]
    public static SqlChars RegExReplace(SqlString sourceString, SqlString pattern, SqlString replacement, int matchId, SqlString options)
    {
        RegexOptions regOptions = RegexOptions.None;

        RegexOptionsEnum.TyPrase(options.Value, out regOptions);

        regOptions |= RegexOptions.Compiled;

        Match m = null;
        Regex r = new Regex(pattern.Value, regOptions);

        if (matchId == 0)
        {
            return new SqlChars(r.Replace(sourceString.Value, replacement.Value));
        }
        if (matchId == 1)
        {
            m = r.Match(sourceString.Value);
        }
        else if (matchId > 1)
        {
            MatchCollection mc = r.Matches(sourceString.Value);
            m = mc != null && mc.Count > matchId - 1 ? mc[matchId - 1] : null;
        }
        
        return m != null ? new SqlChars(m.Result(replacement.Value)) : SqlChars.Null;
    }


    /// <summary>
    /// Applies Regular Expression o the Source strings and return all matches and groups
    /// </summary>
    /// <param name="sourceString">Source string on which the regular expression should be applied</param>
    /// <param name="pattern">Regular Expression pattern</param>
    /// <param name="options">RegexOptions to be used during regular expression application</param>
    /// <returns>Returns list of RegExRows representing the group value</returns>
    [SqlFunction(FillRowMethodName = "FillRegExRow", TableDefinition = "rowID int, matchId int, groupName nvarchar(128), matchSuccess bit, captureId int, value nvarchar(4000)")]
    public static IEnumerable RegExMatches(SqlString sourceString, SqlString pattern, SqlString options)
    {
        RegexOptions regOptions = RegexOptions.None;

        RegexOptionsEnum.TyPrase(options.Value, out regOptions);

        regOptions |= RegexOptions.Compiled;

        Regex r = new Regex(pattern.Value, regOptions);
        int rowId = 0;
        int matchId = 0;
        string[] grpNames = r.GetGroupNames();
        foreach (Match m in r.Matches(sourceString.Value))
        {
            matchId++;            
            for (int i = 0; i < m.Groups.Count; i++)
            {
                string grpName = grpNames[i];
                var grp = m.Groups[i];

                for (int j = 0; j < grp.Captures.Count; j++)
                {
                    var cap = grp.Captures[j];
                    yield return new RegExRow(++rowId, matchId, i, grpName, grp.Success, j, new SqlChars(cap.Value));
                }
            }
        }
    }

    /// <summary>
    /// Applies Regular Expression o the Source strings and return all matches and final results after replacement
    /// </summary>
    /// <param name="sourceString">Source string on which the regular expression should be applied</param>
    /// <param name="pattern">Regular Expression pattern</param>
    /// <param name="replacement">Replacement to be used on matches</param>
    /// <param name="options">RegexOptions to be used during regular expression application</param>
    /// <returns>IEnumerable</returns>
    [SqlFunction(FillRowMethodName = "FillRegExRowReplace")]
    public static IEnumerable RegExMatchesReplace(SqlString sourceString, SqlString pattern, SqlString replacement, SqlString options)
    {
        RegexOptions regOptions = RegexOptions.None;

        RegexOptionsEnum.TyPrase(options.Value, out regOptions);

        regOptions |= RegexOptions.Compiled;

        Regex r = new Regex(pattern.Value, regOptions);
        int matchId = 0;

        foreach (Match m in r.Matches(sourceString.Value))
        {
            yield return new RegExRowReplace(++matchId, new SqlChars(m.Value), new SqlChars(m.Result(replacement.Value)));
        }
    }

    /// <summary>
    /// Applies Regular Expression o the Source strings and return all replacemen for all matches
    /// </summary>
    /// <param name="sourceString">Source string on which the regular expression should be applied</param>
    /// <param name="pattern">Regular Expression pattern</param>
    /// <param name="replacement">Replacement to be used on matches</param>
    /// <param name="options">RegexOptions to be used during regular expression application</param>
    /// <returns>IEnumerable</returns>
    [SqlFunction(FillRowMethodName = "FillRegExRowReplaceOnly")]
    public static IEnumerable RegExMatchesReplaceOnly(SqlString sourceString, SqlString pattern, SqlString replacement, SqlString options)
    {
        RegexOptions regOptions = RegexOptions.None;

        RegexOptionsEnum.TyPrase(options.Value, out regOptions);

        regOptions |= RegexOptions.Compiled;


        Regex r = new Regex(pattern.Value, regOptions);
        int matchId = 0;
        foreach (Match m in r.Matches(sourceString.Value))
        {
            yield return new RegExRowReplace(++matchId, null, new SqlChars(m.Result(replacement.Value)));
        }
    }

    #endregion

    #region FillRow methods
    /// <summary>
    /// FillRow method to populate the output table
    /// </summary>
    /// <param name="obj">RegExRow passed as object</param>
    /// <param name="rowId">ID or the returned row</param>
    /// <param name="matchId">ID of returned Match</param>
    /// <param name="groupID">ID of group in the Match</param>
    /// <param name="value">Value of the Group</param>
    public static void FillRegExRow(object obj, out int rowId, out int matchId, out int groupID, out SqlString groupName, out SqlBoolean matchSuccess, out int captureId, out SqlChars value)
    {
        RegExRow r = (RegExRow)obj;
        rowId = r.RowId;
        matchId = r.MatchId;
        groupID = r.GroupId;
        groupName = r.GroupName;
        captureId = r.CaptureId;
        matchSuccess = r.MatchSuccess;
        value = r.Value;
    }

    public static void FillRegExRowReplace(object obj, out int matchID, out SqlChars match, out SqlChars result)
    {
        RegExRowReplace row = (RegExRowReplace)obj;
        matchID = row.MatchID;
        match = row.Match;
        result = row.Result;
    }

    public static void FillRegExRowReplaceOnly(object obj, out int matchID, out SqlChars result)
    {
        RegExRowReplace row = (RegExRowReplace)obj;
        matchID = row.MatchID;
        result = row.Result;
    }
    #endregion

    static class RegexOptionsEnum
    {

        private static Dictionary<string, RegexOptions> GetDictionaryValues()
        {
            var values = Enum.GetValues(typeof(RegexOptions));
            Dictionary<string, RegexOptions> dic = new Dictionary<string, RegexOptions>(values.Length);

            foreach (RegexOptions o in values)
            {
                dic.Add(Enum.GetName(enumType, o).ToLowerInvariant(), o);
            }
            return dic;
        }


        static readonly Type enumType = typeof(RegexOptions);
        static readonly char[] FlagDelimiter = new[] { ',' };
        static readonly Dictionary<string, RegexOptions> insensitiveNames = GetDictionaryValues();

        public static bool TyPrase(string value, out RegexOptions options)
        {
            RegexOptions alloptions;
            RegexOptions option;

            options = alloptions = default(RegexOptions);

            if (Enum.IsDefined(enumType, value))
            {
                options = (RegexOptions)Enum.Parse(enumType, value);
                return true;
            }

            foreach (string val in value.ToLowerInvariant().Split(FlagDelimiter))
            {
                if (insensitiveNames.TryGetValue(val.Trim(), out option))
                {
                    alloptions |= option;
                }
                else
                    return false;
            }

            options = alloptions;
            return true;
        }
    }
}
