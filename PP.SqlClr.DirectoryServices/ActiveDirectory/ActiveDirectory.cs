using System;
using System.Collections.Generic;
using System.Text;
using System.DirectoryServices;
using Microsoft.SqlServer.Server;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;
using System.Reflection;
using System.IO;

public class ActiveDirectory
{
    private struct ADPropertyInfo
    {
        public ADPropertyInfo(string propertyName, int length, Encoding decoder, Encoding encoder, bool binary, bool encodeBinaray)
        {
            PropertyName = propertyName;
            Length = length;
            Decoder = decoder;
            Encoder = encoder;
            IsBinary = binary;
            EncodeBinary = encodeBinaray;
        }
        public string PropertyName;
        public int Length;
        public Encoding Decoder;
        public Encoding Encoder;
        public bool IsBinary;
        public bool EncodeBinary;

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4}", PropertyName, Length, Decoder, Encoder, IsBinary ? "IsBinary" : "IsNotBinary", EncodeBinary ? "EncodeBinary" : "Do not EncodeBinary");
        }
    }

    /// <summary>
    /// Tries to authenticate a user against AD
    /// </summary>
    /// <param name="adRoot">AD root to authenticate</param>
    /// <param name="userName">user name to authenticate</param>
    /// <param name="password">password to authenticate</param>
    /// <param name="throwExceptions">whether to throw authentication exceptions</param>
    /// <returns>true when successfully authenticated otherwise false</returns>
    [Microsoft.SqlServer.Server.SqlFunction(IsDeterministic=false)]
    public static bool AuthenticateDefault(string adRoot, string userName, string password, bool throwExceptions)
    {
        return Authenticate(adRoot, userName, password, null, throwExceptions);
    }

    /// <summary>
    /// Tries to authenticate a user against AD
    /// </summary>
    /// <param name="adRoot">AD root to authenticate</param>
    /// <param name="userName">user name to authenticate</param>
    /// <param name="password">password to authenticate</param>
    /// <param name="throwExceptions">whether to throw authentication exceptions</param>
    /// <param name="authType">authentication type</param>
    /// <returns>true when successfully authenticated otherwise false</returns>
    [Microsoft.SqlServer.Server.SqlFunction(IsDeterministic = false)]
    public static bool Authenticate(string adRoot, string userName, string password, string authType, bool throwExceptions)
    {
        bool authenticated = false;
        DirectoryEntry entry;

        if (authType == null)
            try
            {
                entry = new DirectoryEntry(adRoot, userName, password);
                object obj = entry.NativeObject;
                authenticated = true;
            }
            catch (Exception e)
            {
                if (throwExceptions)
                    throw e;
            }
        else
        {
            AuthenticationTypes at;
            if (!TryParseEnum<AuthenticationTypes>(authType, true, out at))
                throw new System.InvalidCastException(string.Format("authType must be one of '{0}'", GetEnumNames<AuthenticationTypes>()));

            try
            {
                entry = new DirectoryEntry(adRoot, userName, password, at);
                object obj = entry.NativeObject;
                authenticated = true;
            }
            catch (Exception e)
            {
                if (throwExceptions)
                    throw e;
            }
        }

        return authenticated;
    }

    [Microsoft.SqlServer.Server.SqlProcedure]
    public static int SPAuthenticateDefault(string adRoot, string userName, string password, bool throwExceptions)
    {
        return AuthenticateDefault(adRoot, userName, password, throwExceptions) ? 1 : 0;
    }

    [Microsoft.SqlServer.Server.SqlProcedure]
    public static int SPAuthenticate(string adRoot, string userName, string password, string authType, bool throwExceptions)
    {
        return Authenticate(adRoot, userName, password, authType, throwExceptions) ? 1 : 0;
    }

    /// <summary>
    /// Queries Active directory according provided parameters
    /// Current user credentials are used for authentication
    /// </summary>
    /// <param name="adRoot">AD Root for querying AD</param>
    /// <param name="filter">Filter to be used for querying</param>
    /// <param name="searchScope">Scope to be used for queryingg</param>
    /// <param name="propertiesToLoad">List of properties to return</param>
    /// <param name="pageSize">Represents a PageSise for the paged search of AD</param>
    /// <param name="rowsLimit">Rrepresent limit for numbers of rows returned. NULL or value less than 1 represents unlimited</param>
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void QueryAD(string adRoot, string filter, string propertiesToLoad, string searchScope, int pageSize, SqlInt32 rowsLimit)
    {
        SearchAD(null, null, null, adRoot, filter, searchScope, propertiesToLoad, pageSize, rowsLimit);
    }

    /// <summary>
    /// Queries Active directory according provided parameters
    /// </summary>
    /// <param name="userName">UserName to be used to authenticate AD</param>
    /// <param name="password">Password to be used to authenticate to AD</param>
    /// <param name="adRoot">AD Root for querying AD</param>
    /// <param name="filter">Filter to be used for querying</param>
    /// <param name="searchScope">Scope to be used for queryingg</param>
    /// <param name="propertiesToLoad">List of properties to return</param>
    /// <param name="pageSize">Represents a PageSise for the paged search of AD</param>
    /// <param name="rowsLimit">Rrepresent limit for numbers of rows returned. NULL or value less than 1 represents unlimited</param>
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void QueryADUName(string userName, string password, string adRoot, string filter, string propertiesToLoad, string searchScope, int pageSize, SqlInt32 rowsLimit)
    {
        SearchAD(userName, password, null, adRoot, filter, searchScope, propertiesToLoad, pageSize, rowsLimit);
    }

    /// <summary>
    /// Queries Active directory according provided parameters
    /// </summary>
    /// <param name="userName">UserName to be used to authenticate AD</param>
    /// <param name="password">Password to be used to authenticate to AD</param>
    /// <param name="authType">Authentication type to be used to authenticate to AD</param>
    /// <param name="adRoot">AD Root for querying AD</param>
    /// <param name="filter">Filter to be used for querying</param>
    /// <param name="searchScope">Scope to be used for queryingg</param>
    /// <param name="propertiesToLoad">List of properties to return</param>
    /// <param name="pageSize">Represents a PageSise for the paged search of AD</param>
    /// <param name="rowsLimit">Rrepresent limit for numbers of rows returned. NULL or value less than 1 represents unlimited</param>
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void QueryADAuth(string userName, string password, string authType, string adRoot, string filter, string propertiesToLoad, string searchScope, int pageSize, SqlInt32 rowsLimit)
    {
        SearchAD(userName, password, authType, adRoot, filter, searchScope, propertiesToLoad, pageSize, rowsLimit);
    }

    /// <summary>
    /// Creates a Root directory entry according provided adRoot, userName, password and authType
    /// </summary>
    private static DirectoryEntry GetRootEntry(string adRoot, string userName, string password, string authType)
    {
        if (userName == null)
            return new DirectoryEntry(adRoot);
        else if (authType == null)
            return new DirectoryEntry(adRoot, userName, password);
        else
        {
            AuthenticationTypes at;
            if (TryParseEnum<AuthenticationTypes>(authType, true, out at))
                return new DirectoryEntry(adRoot, userName, password, at);
            else
                throw new System.InvalidCastException(string.Format("authType must be one of '{0}'", GetEnumNames<AuthenticationTypes>()));
        }
    }

    private static string GetEnumNames<T>()
    {
        string[] names = Enum.GetNames(typeof(T));
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < names.Length; i++)
        {
            if (i > 0)
                sb.Append(',');
            sb.Append(names[i]);
        }
        return sb.ToString();
    }

    private static bool TryParseEnum<T>(string value, bool ignoreCase, out T outEnum)
    {
        try
        {
            outEnum = (T)Enum.Parse(typeof(T), value, ignoreCase);
            return true;
        }
        catch
        {
            outEnum = (T)Enum.GetValues(typeof(T)).GetValue(0);
            return false;
        }        
    }

    /// <summary>
    /// Searches Active Directory according provided parameters
    /// </summary>
    /// <param name="userName">UserName to be used to authenticate AD</param>
    /// <param name="password">Password to be used to authenticate to AD</param>
    /// <param name="authType">Authentication type to be used to authenticate to AD</param>
    /// <param name="adRoot">AD Root for querying AD</param>
    /// <param name="filter">Filter to be used for querying</param>
    /// <param name="searchScope">Scope to be used for queryingg</param>
    /// <param name="propertiesToLoad">List of properties to return</param>
    /// <param name="pageSize">Represents a PageSise for the paged search of AD</param>
    /// <param name="rowsLimit">Rrepresent limit for numbers of rows returned. NULL or value less than 1 represents unlimited</param>
    private static void SearchAD(string userName, string password, string authType, string adRoot, string filter, string searchScope, string propertiesToLoad, int pageSize, SqlInt32 rowsLimit)
    {
        string[] properties = propertiesToLoad.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        ADPropertyInfo[] adProperties = new ADPropertyInfo[properties.Length];
        SqlMetaData[] recordMetaData = new SqlMetaData[properties.Length];
        SearchScope scope;
        Type et = typeof(Encoding);  //Encoding Type
        
        int limit = rowsLimit.IsNull ? 0 : rowsLimit.Value;
        int rowsCount = 0;

        if (rowsLimit > 0 && pageSize > limit)
            pageSize = limit;

        if (!TryParseEnum<SearchScope>(searchScope, true, out scope))
            throw new System.InvalidCastException(string.Format("searchScope must be one of '{0}'", GetEnumNames<SearchScope>()));


        //Trim properties and prepare result set metadata, also process specified lengths
        for (int i = 0; i < properties.Length; i++)
        {
            string[] propDetails = properties[i].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);  //Properties detals - Split to Name, Length, Decoder and Encoder
            string propName = propDetails[0].Trim();
            int len = 4000;
            int tmpLen;
            bool isBinary = false;
            Encoding decoder = Encoding.Unicode;
            Encoding encoder = Encoding.Unicode;
            bool encodingBiinary = false;

            #region Field Length Retrieval

            if (propDetails.Length > 1)
            {
                //if length is "max" then set len = -1 whihc equals to MAX
                if (propDetails[1].ToLower() == "max")
                    len = -1;
                else if (int.TryParse(propDetails[1], out tmpLen) && tmpLen >= 1 && tmpLen <= 4000)
                    len = tmpLen;
                else
                    throw new System.InvalidCastException(string.Format("[{0}] - Length of field has to be numeric value in range 1 - 4000 or max", properties[i]));
            }

            #endregion

            #region Get Decoder and Encoder
            //find Encoding
            if (propDetails.Length >= 3)
            {
                decoder = null;
                Exception exc = null;

                //If Decoder or Encoder is BINARY then we will use a binary storage
                if (propDetails[2] == "BINARY" || (propDetails.Length >= 4 && propDetails[3] == "BINARY"))
                    isBinary = true;

                if (propDetails.Length >= 4 && propDetails[3] == "BINARY")
                {
                    encodingBiinary = true;
                }

                //Get Decoder
                if (propDetails[2] != "BINARY")
                {
                    int codePage;
                    try
                    {
                        if (int.TryParse(propDetails[2].Trim(), out codePage))
                        {
                            decoder = Encoding.GetEncoding(codePage);
                        }
                        else
                        {
                            //Try to get static property of the Encoding Class
                            System.Reflection.PropertyInfo pi = et.GetProperty(propDetails[2].Trim(), et);

                            //if the static property does not exists, treats the name as Code Page name
                            if (pi == null)
                                decoder = Encoding.GetEncoding(propDetails[2].Trim());
                            else if (pi.CanRead)
                                decoder = pi.GetValue(null, null) as Encoding;
                        }
                    }
                    catch (Exception e)
                    {
                        exc = e;
                    }

                    if (decoder == null)
                    {
                        //Get List of available static properties of the EncodingClass
                        StringBuilder sb = new StringBuilder();
                        sb.Append("BINARY");

                        foreach (PropertyInfo p in et.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Static))
                        {
                            if (p.PropertyType == et)
                            {
                                sb.Append(",");
                                sb.Append(p.Name);
                            }
                        }
                        throw new System.NotSupportedException(string.Format("[{0}] - Decoder has to be one of {1} or a CodePage Numer or CodePage name. For Code Pages see http://msdn.microsoft.com/en-us/library/vstudio/system.text.encoding(v=vs.100).aspx", properties[i], sb.ToString()), exc);
                    }
                }

                //Get Encoder
                if (propDetails.Length >= 4 && propDetails[3] != "BINARY")
                {
                    encoder = null;
                    int codePage;

                    try
                    {
                        //In case of CodePage number, try to get code page
                        if (int.TryParse(propDetails[3].Trim(), out codePage))
                        {
                            encoder = Encoding.GetEncoding(codePage);
                        }
                        else
                        {
                            //Try to get static property of the Encoding Class
                            System.Reflection.PropertyInfo pi = et.GetProperty(propDetails[2].Trim(), et);

                            //if the static property does not exists, treats the name as Code Page name
                            if (pi == null)
                                encoder = Encoding.GetEncoding(propDetails[2].Trim());
                            else if (pi.CanRead)
                                encoder = pi.GetValue(null, null) as Encoding;
                        }
                    }
                    catch (Exception e)
                    {
                        exc = e;
                    }

                    if (encoder == null)
                    {
                        //Get List of available static properties of the EncodingClass
                        StringBuilder sb = new StringBuilder();
                        sb.Append("BINARY");

                        foreach (PropertyInfo p in et.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Static))
                        {
                            if (p.PropertyType == et)
                            {
                                sb.Append(",");
                                sb.Append(p.Name);
                            }
                        }
                        throw new System.NotSupportedException(string.Format("[{0}] - Encoder has to be one of {1} or a CodePage Numer or CodePage name. For Code Pages see http://msdn.microsoft.com/en-us/library/vstudio/system.text.encoding(v=vs.100).aspx", properties[i], sb.ToString()), exc);
                    }
                }
            }
            #endregion

            //IF Binary, use VarBinary data type otherwise NVarChar
            if (isBinary)
                recordMetaData[i] = new SqlMetaData(propName, System.Data.SqlDbType.VarBinary, len);
            else
                recordMetaData[i] = new SqlMetaData(propName, System.Data.SqlDbType.NVarChar, len);

            //Set ADProperties for current property
            adProperties[i] = new ADPropertyInfo(propName, len, decoder, encoder, isBinary, encodingBiinary);
            properties[i] = propName;
        }

        //Get Root Directory Entry
        using (DirectoryEntry rootEntry = GetRootEntry(adRoot, userName, password, authType))
        {
            //Create a directory searcher with aproperiate filter, properties and search scope
            using (DirectorySearcher ds = new DirectorySearcher(rootEntry, filter, properties, scope))
            {
                if (pageSize > 0)
                    ds.PageSize = pageSize; //Set Page Size - without this we will not do a paged search and we will be limiited to 1000 results

                //find all object from the rood, according the filter and search scope
                using (SearchResultCollection results = ds.FindAll())
                {
                    SqlDataRecord record = new SqlDataRecord(recordMetaData);
                    //Start pushing of records to client
                    SqlContext.Pipe.SendResultsStart(record);

                    Regex dnr = null;

                    foreach (SearchResult result in results)
                    {
                        record = new SqlDataRecord(recordMetaData);

                        for (int i = 0; i < properties.Length; i++)
                        {
                            ADPropertyInfo adProperty = adProperties[i];

                            ResultPropertyValueCollection props = result.Properties[adProperty.PropertyName];
                            if (props.Count == 1)           //if property collection contains single vallue, set the record field to that value
                                if (props[0] is byte[])
                                {
                                    byte[] buffer = props[0] as byte[];

                                    if (adProperty.IsBinary) //If Binary output, write binary buffer
                                        record.SetBytes(i, 0, buffer, 0, adProperty.Length != -1 && adProperty.Length < buffer.Length ? adProperty.Length : buffer.Length);
                                    else //Otherwise use decoder to decode the buffer to sctring
                                        record.SetSqlString(i, adProperty.Decoder.GetString(buffer));
                                }
                                else
                                {
                                    if (adProperty.IsBinary) //In case of binary output, Encode binary to bytes array
                                    {
                                        var buffer = adProperty.Encoder.GetBytes(props[0].ToString());
                                        record.SetBytes(i, 0, buffer, 0, adProperty.Length != -1 && adProperty.Length < buffer.Length ? adProperty.Length : buffer.Length);
                                    }
                                    else
                                        record.SetSqlString(i, props[0].ToString());
                                }

                            else if (props.Count == 0)      //if property collection doesn't contain any value, set record field to NULL
                            {
                                //In case Distinguished Name was requested and such attribute is not in the LDAP, parse the result.Path
                                if (adProperty.PropertyName.ToLower() == "dn" || adProperty.PropertyName.ToLower() == "distinguishedname")
                                {
                                    if (dnr == null)
                                        dnr = new Regex(@"(?:.+?)//(?:(?:\w|\.)+?)/(?<dn>.+)", RegexOptions.Compiled); // LDAP://server_name_or_ip/DN

                                    var match = dnr.Match(result.Path);
                                    if (match != null && match.Groups.Count > 1)
                                    {
                                        if (adProperty.IsBinary)
                                        {
                                            var buffer = adProperty.Encoder.GetBytes(match.Groups["dn"].Value);
                                            record.SetBytes(i, 0, buffer, 0, adProperty.Length != -1 && adProperty.Length < buffer.Length ? adProperty.Length : buffer.Length);
                                        }
                                        else
                                            record.SetSqlString(i, match.Groups["dn"].Value);
                                    }
                                }
                                else
                                {
                                    if (adProperty.IsBinary)
                                        record.SetSqlBinary(i, SqlBinary.Null);
                                    else
                                        record.SetSqlString(i, SqlString.Null);
                                }
                            }
                            else   //In case of multiple value, separate the values by commas
                            {
                                if (adProperty.IsBinary) //In case of Binary output, create a MemoryStream to which we write multiple binary values and the store the data in the output field
                                {
                                    using (MemoryStream ms = new MemoryStream()) //MemoryStream to store the binary data
                                    {
                                        using (BinaryWriter bw = new BinaryWriter(ms)) //Binary write for writin the data into the memory stream
                                        {
                                            foreach (object prop in props)
                                            {
                                                if (prop is byte[]) //If property is byte array, write that byte array into the memory stream
                                                    bw.Write((byte[])prop);
                                                else
                                                {
                                                    if (adProperty.EncodeBinary) //If BinaryEncoded, user the BinaryWrite.Write(string)
                                                        bw.Write(prop.ToString());
                                                    else //Otherwise use Encoder to encode the string into byte array and write it iinto the memory stream
                                                        bw.Write(adProperty.Encoder.GetBytes(prop.ToString()));
                                                }
                                            }
                                        }
                                        //Get the MemoryStream buffer and write it into the output field
                                        var buffer = ms.GetBuffer();
                                        record.SetBytes(i, 0, buffer, 0, adProperty.Length != -1 && adProperty.Length < buffer.Length ? adProperty.Length : buffer.Length);
                                    }
                                }
                                else  //character output
                                {
                                    StringBuilder sb = new StringBuilder();
                                    bool firstItem = true;
                                    foreach (object prop in props)
                                    {
                                        if (!firstItem)
                                            sb.Append(',');
                                        else
                                            firstItem = false;
                                        if (prop is byte[]) //In case of binary data, decode them to a string using decoder
                                            sb.Append(adProperty.Decoder.GetString((byte[])prop));
                                        else
                                            sb.Append(prop.ToString());
                                    }

                                    var c = sb.ToString();
                                    record.SetSqlString(i, sb.ToString());
                                }
                            }
                        }

                        //send record to client
                        SqlContext.Pipe.SendResultsRow(record);
                        
                        //if rowsLimit was reached, break the loop
                        if (++rowsCount == rowsLimit)
                            break;
                    }

                    //stop sending records to client
                    SqlContext.Pipe.SendResultsEnd();
                }
            }
        }
    }

}
