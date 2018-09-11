using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using System.Text;
using Microsoft.SqlServer.Server;

public class Accounts
{
    /// <summary>
    /// Stores information abount individual Principal representing an Accnout
    /// </summary>
    private struct AccountInfo
    {
        public string AccountType;
        public string NTAccount;
        public string Context;
        public string SamAccountName;
        public string Name;
        public string DisplayName;
        public string DistinguishedName;
        public string UserPrincipalName;
        public System.Security.Principal.SecurityIdentifier Sid;
        public Guid? Guid;
        public string Description;
        public string ParentNTAccount;
        public System.Security.Principal.SecurityIdentifier ParentSid;

    }

    /// <summary>
    /// Gets information about current Account. In case of group it can list information about group members
    /// </summary>
    /// <param name="NTAccountName">NT Account name to return information about</param>
    /// <param name="getGroupMembers">Specifies whether list Group members in case of Gorup account</param>
    /// <param name="recursive">Specifies whether the group member should be traversed recursivelly</param>
    /// <returns>AccountInfo enumerator</returns>
    [SqlFunction(FillRowMethodName = "FillGroupMembers")]
    public static IEnumerable GetAccountInfo(string NTAccountName, bool getGroupMembers, bool recursive)
    {
        WindowsImpersonationContext ctx = null;
        Principal p = null;
        Exception principalException = null;
        string accountSearchName;
        string contextSearchname;

        //split NTAccountName to Domain/Host and SAMAccountName
        string[] splitName = !string.IsNullOrEmpty(NTAccountName) ? NTAccountName.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries) : null;

        try
        {
            try
            {
                ctx = null;
                if (SqlContext.WindowsIdentity != null)     //If Current SqlContext WindowsIdentity is not null (Integraget authentication) then impersonate as the current user
                    ctx = SqlContext.WindowsIdentity.Impersonate();
            }
            catch
            {
                ctx = null;
            }

            try
            {
                if (splitName == null || splitName.Length != 2) //If accnout name is not in format of NT Account, thorow an exception
                    throw new System.ArgumentException("NTAccountName has to be in format [DOMAN\\SAMAccountName] or [HOSTNAME\\SAMAccountName]", "NTAccountName");

                string localhostName = System.Net.Dns.GetHostName().ToLower(); //Get Machine Host name to determine whether connecting to local machine or domain.
                if (splitName[0].ToUpper() == "BUILTIN" || splitName[0].ToUpper() == "NT AUTHORITY")
                    contextSearchname = localhostName;
                else
                    contextSearchname = splitName[0];

                accountSearchName = string.Concat(contextSearchname, "\\", splitName[1]);

                //If current host name is equal to the domain part of the NTAccoutn name, then use Machine as context otherwise Domain
                ContextType ct = localhostName == contextSearchname.ToLower() ? ContextType.Machine : ContextType.Domain; 

                PrincipalContext cx = new PrincipalContext(ct, contextSearchname);   //Get PrincipalContext for Domain/Host provided as part of the NTAccount
                if (cx == null)
                    throw new System.Exception(string.Format("Could not connect to Domain/Host [{0}]", splitName[0]));

                p = Principal.FindByIdentity(cx, IdentityType.SamAccountName, splitName[1]);    //Find the Account principal by the Name

                DirectoryEntry directoryEntryWinNT = new DirectoryEntry("WinNT://" + contextSearchname + "/" + splitName[1]);


                if (p == null)
                    throw new System.Exception(string.Format("Account [{0}] was not found", NTAccountName));
            }
            catch (Exception e)
            {
                principalException = e;
            }

            if (principalException != null) //If there was an error during retrieving the Principal information, return an error message
            {
                AccountInfo ai = new AccountInfo() { NTAccount = NTAccountName, Description = principalException.Message };

                yield return ai;
            }
            else if (p != null) //If Account was found, return information
            {
                foreach (var item in GetPrincipalDetails(p, null, getGroupMembers, recursive)) //Get Details about the Principal
                {
                    yield return item;
                }
            }
        }
        finally
        {

            if (ctx != null) //if we were impersonated, then revet the current user context
                ctx.Undo();
        }
    }

    /// <summary>
    /// Returns information about provided Principal
    /// </summary>
    /// <param name="principal">Principal about which the Details should be returned</param>
    /// <param name="parentPrincipal">Parent Principal in caseof recursibelly traversing GroupPPrincipals</param>
    /// <param name="getGroupMembers">Specifies whether return Group membes in case Principal represents Group</param>
    /// <param name="recursive">Specifies whether GroupPrincipals should be traversed recursively</param>
    /// <returns></returns>
    private static IEnumerable GetPrincipalDetails(Principal principal, Principal parentPrincipal, bool getGroupMembers, bool recursive)
    {
        AccountInfo accountInfo;
        NTAccount ntAccount = null;
        NTAccount parentNTAccount = null;
        if (principal != null)
        {
            //If Sid can be translated to NTAccount, translate it to provide NTAccount Name for the Principal
            if (principal.Sid.IsValidTargetType(typeof(NTAccount)))
                ntAccount = (NTAccount)principal.Sid.Translate(typeof(NTAccount));

             if (parentPrincipal != null && parentPrincipal.Sid.IsValidTargetType(typeof(NTAccount)))
                 parentNTAccount = (NTAccount)parentPrincipal.Sid.Translate(typeof(NTAccount));

            accountInfo = new AccountInfo()
            {
                AccountType = principal.StructuralObjectClass,
                NTAccount = ntAccount != null ? ntAccount.Value : null,
                Context = principal.Context.Name,
                SamAccountName = principal.SamAccountName,
                Name = principal.Name,
                DisplayName = principal.DisplayName,
                DistinguishedName = principal.DistinguishedName,
                UserPrincipalName = principal.UserPrincipalName,
                Sid = principal.Sid,
                Guid = principal.Guid,
                Description = principal.Description,
                ParentNTAccount = parentNTAccount != null ? parentNTAccount.Value : null,
                ParentSid = parentPrincipal != null ? parentPrincipal.Sid : null
            };
            
            yield return accountInfo;
        }

        //If GroupMembers should be returned and Principal is GroupPrincipal, traverse the Group
        if (getGroupMembers)
        {
            GroupPrincipal gp = principal as GroupPrincipal;
            Exception principalException;

            if (gp != null && (recursive || parentPrincipal == null))
            {
                var members = gp.GetMembers();
                //Get Enumarator and iterage using the iterator
                //We are not using Foreach to allow bypass Group members which domain information is not available.
                //Wit foreach on first exception the iteration is terminated
                var en = members.GetEnumerator();
                bool traverse = true;

                while (traverse)
                {
                    principalException = null;
                    try
                    {
                        traverse = en.MoveNext();
                    }
                    catch (Exception e)
                    {
                        //An Error occured during retrieval of Principal. Instead of principal we will provide an Error messaage in teh AccountInfo Description
                        if (e is System.DirectoryServices.AccountManagement.PrincipalException || e is SystemException)
                            principalException = e;
                        else
                            throw;
                    }

                    if (principalException != null)
                    {
                        //If an error occured during Principal retrieval return error message instead of real Principal
                        AccountInfo ai = new AccountInfo()
                        {
                            Description = principalException.Message,
                            ParentNTAccount = ntAccount != null ? ntAccount.Value : null,
                            ParentSid = principal != null ? principal.Sid : null
                        };

                        yield return ai;

                    }
                    else if (traverse)
                    {
                        foreach (var item in GetPrincipalDetails(en.Current, principal, true, recursive))
                        {
                            yield return item;
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// FillRow method to populate the output table
    /// </summary>
    /// <param name="obj">Input AccountInfo</param>
    /// <param name="AccountType">Accoun type</param>
    /// <param name="NTAccount">NTAccount name</param>
    /// <param name="Context">Context of the account</param>
    /// <param name="SamAccountName">SAMAccount Name</param>
    /// <param name="Name">Account Name</param>
    /// <param name="DisplayName">Account Display Name</param>
    /// <param name="DistinguishedName">Account Distinguished Name</param>
    /// <param name="UserPrincipalName">User Principal Name</param>
    /// <param name="Sid">SID of the account</param>
    /// <param name="Guid">GUID of the account</param>
    /// <param name="Description">Account description</param>
    /// <param name="ParentNTAccount">Parent NTAccount name in case of group members</param>
    /// <param name="ParentSid">Parent Account SID in case of group members</param>
    public static void FillGroupMembers(object obj, out SqlString AccountType, out SqlString NTAccount, out SqlString Context, out SqlString SamAccountName, out SqlString Name, out SqlString DisplayName, out SqlString DistinguishedName, 
        out SqlString UserPrincipalName, out SqlBinary Sid, out SqlGuid Guid, out SqlString Description, out SqlString ParentNTAccount, out SqlBinary ParentSid)
    {
        AccountInfo ai = (AccountInfo)obj;
        byte[] sidBuffer;

        AccountType = ai.AccountType;
        NTAccount = ai.NTAccount;
        Context = ai.Context;
        SamAccountName = ai.SamAccountName;
        Name = ai.Name;;
        DisplayName = ai.DisplayName;
        DistinguishedName = ai.DistinguishedName;
        UserPrincipalName = ai.UserPrincipalName;
        Description = ai.Description;

        if (ai.Sid != null)
        {
            sidBuffer = new byte[ai.Sid.BinaryLength];
            ai.Sid.GetBinaryForm(sidBuffer, 0);
            Sid = new SqlBinary(sidBuffer);
        }
        else
        {
            Sid = SqlBinary.Null;
        }

        Guid = ai.Guid.HasValue ? ai.Guid.Value : SqlGuid.Null;

        ParentNTAccount = ai.ParentNTAccount == null ? SqlString.Null : ai.ParentNTAccount;

        if (ai.ParentSid != null)
        {
            sidBuffer = new byte[ai.ParentSid.BinaryLength];
            ai.ParentSid.GetBinaryForm(sidBuffer, 0);
            ParentSid = new SqlBinary(sidBuffer);
        }
        else
        {
            ParentSid = SqlBinary.Null;
        }                
    }

}
