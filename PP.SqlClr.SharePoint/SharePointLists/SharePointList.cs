using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Xml;
using Microsoft.SqlServer.Server;

[PermissionSet(SecurityAction.Demand, Unrestricted = true, Name = "FullTrust")]
public class SharePointList
{

    /// <summary>
    /// Reads the SharePoint List XML Data
    /// </summary>
    /// <param name="siteUrl">Site URL</param>
    /// <param name="listGuid">GUID of the List</param>
    /// <param name="userName">Windows User name under which the SharePoint should be accessed. If not provided a current user credential will be used</param>
    /// <param name="pwd">Password of the windows user which should be used to access the SharePoint. If not provided, current user credential will be used</param>
    /// <returns>SharePoint List XML Data</returns>
    [Microsoft.SqlServer.Server.SqlFunction(IsDeterministic = false, DataAccess = DataAccessKind.Read)]
    public static SqlXml GetSharePointListXml(SqlString siteUrl, SqlGuid listGuid, SqlString userName, SqlString pwd)
    {
        return GetSharePointListViewXml(siteUrl, listGuid, SqlGuid.Null, null, null, userName, pwd);
    }

    /// <summary>
    /// Reads the SharePoint List XML Data
    /// </summary>
    /// <param name="siteUrl">Site URL</param>
    /// <param name="listGuid">GUID of the List</param>
    /// <param name="viewGuid">GUID of the View</param>
    /// <param name="fieldsList">List of the Fields to retrieve</param>
    /// <param name="userName">Windows User name under which the SharePoint should be accessed. If not provided a current user credential will be used</param>
    /// <param name="filter">Filter condition in formant "FieldName>=Value&FieldName=Value&FieldName&lt;=Value</param>
    /// <param name="pwd">Password of the windows user which should be used to access the SharePoint. If not provided, current user credential will be used</param>
    /// <returns>SharePoint List XML Data</returns>
    [Microsoft.SqlServer.Server.SqlFunction(IsDeterministic = false, DataAccess=DataAccessKind.Read)]
    public static SqlXml GetSharePointListViewXml(SqlString siteUrl, SqlGuid listGuid, SqlGuid viewGuid, SqlString fieldsList, SqlString filter, SqlString userName, SqlString pwd)
    {
        StringBuilder sb = new StringBuilder(siteUrl.Value.Length + (fieldsList == null || fieldsList.IsNull ? 0 : fieldsList.Value.Length) + 200);
        
        //Add Site and the Display Command to URL and the List GUID        
        sb.AppendFormat("{0}{1}_vti_bin/owssvr.dll?Cmd=Display&List={2}", siteUrl, siteUrl.Value.EndsWith("/") ? "" : "/", listGuid.Value.ToString("B"));
        
        //Add View guid into the URL
        if (!viewGuid.IsNull)
            sb.AppendFormat("&View={0}", viewGuid.Value.ToString("B"));

        //Process Filter
        if (!filter.IsNull && !String.IsNullOrEmpty(filter.Value))
        {
            string[] filters = filter.Value.Split('&');
            int conditionID = 0;
            string op;
            foreach (string condition in filters)
            {
                string[] conditionParts = condition.Split(new string[] { "=", "<=", ">=" }, StringSplitOptions.RemoveEmptyEntries);
                if (conditionParts.Length == 2)
                {
                    conditionID++;
                    if (condition.Contains("<="))
                        op = "Leq";
                    else if (condition.Contains(">="))
                        op = "Geq";
                    else if (condition.Contains("="))
                        op = "eq";
                    else
                        op = null;

                    if (op != null)
                    {
                        sb.AppendFormat("&FilterField{0}={1}&FilterValue{0}={2}&FilterOp{0}={3}", conditionID, conditionParts[0], conditionParts[1], op);
                    }
                    
                }
            }


        }
            

        sb.AppendFormat("&XMLDATA=TRUE");

        sb.AppendFormat("&Query={0}", fieldsList == null || fieldsList.IsNull || string.IsNullOrEmpty(fieldsList.Value) ? "*" : fieldsList);

        return GetSharePointListUrlXml(sb.ToString(), userName, pwd);
        
    }

    /// <summary>
    /// Reads the SharePoint List XML Data
    /// </summary>
    /// <param name="listUrl">ULR of the List to read data</param>
    /// <param name="userName">Windows User name under which the SharePoint should be accessed. If not provided a current user credential will be used</param>
    /// <param name="pwd">Password of the windows user which should be used to access the SharePoint. If not provided, current user credential will be used</param>
    /// <returns>SharePoint List XML Data</returns>
    [Microsoft.SqlServer.Server.SqlFunction(IsDeterministic = false, DataAccess = DataAccessKind.Read)]
    public static SqlXml GetSharePointListUrlXml(string listUrl, SqlString userName, SqlString pwd)
    {
        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CertificateOverride.RemoteCertificateValidationCallback);
        WebResponse response;
        HttpWebRequest request = (HttpWebRequest)System.Net.HttpWebRequest.Create(listUrl);
        CookieContainer cookieContainer = new CookieContainer();

        WindowsImpersonationContext ctx = null;

        request.CookieContainer = cookieContainer;

        try
        {

            if (userName == null || userName.IsNull || string.IsNullOrEmpty(userName.Value) ||
                pwd == null || pwd.IsNull || string.IsNullOrEmpty(pwd.Value))
            {
                WindowsIdentity id = SqlContext.WindowsIdentity;
                ctx = id.Impersonate();
                
                request.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
            }
            else
            {
                request.Credentials = new NetworkCredential(userName.Value, pwd.Value);
            }

            response = request.GetResponse();
            string html;
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                html = sr.ReadToEnd();
                sr.Close();
            }

            using (StringReader sr = new StringReader(html))
            {
                using (XmlReader xr = new XmlTextReader(sr))
                {
                    SqlXml xml = new SqlXml(xr);
                    return xml;
                }
            }
        }
        finally
        {
            if (ctx != null)
                ctx.Undo();
        }
    }

    internal class CertificateOverride
    {
        public static bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chaing, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }


}


