using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Security.Permissions;
using System.Security.Principal;
using System.Collections.Generic;
using System.Collections;

public class AssemblyExporter
{
    private struct AssemblyData
    {
        public string AssemblyFileName;
        public string OutputFileName;
        public byte[] Data;

    }
    /// <summary>
    /// Exports and saves all assembly files from current SQL Server database to disk
    /// </summary>
    /// <param name="assemblyName">Assembly which files should be exported</param>
    /// <param name="destinationPath">Destination path to which the files should be stored</param>
    /// <param name="impersonateCurrentUser">Specifies whether impersonate current user</param>
    [SqlProcedure]
    public static void ExportAssembly(string assemblyName, string destinationPath, bool impersonateCurrentUser)
    {
        string sql = @"SELECT af.name, af.content FROM sys.assemblies a INNER JOIN sys.assembly_files af ON a.assembly_id = af.assembly_id WHERE a.name = @assemblyname";
        Queue<AssemblyData> assembliesQueue = new Queue<AssemblyData>(); //Queue to store assembly data as querying data under impersonation is not possible

        using (SqlConnection conn = new SqlConnection("context connection=true"))   //Create current context connection
        {
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                SqlParameter param = new SqlParameter("@assemblyname", SqlDbType.VarChar);
                param.Value = assemblyName;
                cmd.Parameters.Add(param);

                cmd.Connection.Open();  //Open the context connetion
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string assemblyFileName = reader.GetString(0);  //get assembly file name from the name (first) column
                        string outputFile = Path.Combine(destinationPath, assemblyFileName);
                        SqlContext.Pipe.Send(string.Format("Reading assembly file [{0}]", assemblyFileName)); //Send information about exported file back to the calling session
                        SqlBytes bytes = reader.GetSqlBytes(1);         //get assembly binary data from the content (second) column
                        assembliesQueue.Enqueue(new AssemblyData() { AssemblyFileName = assemblyFileName, OutputFileName = outputFile, Data = bytes.Value });
                    }
                }
            }
            conn.Close();

            AssemblyData asmData;
            WindowsImpersonationContext ctx;
            while (assembliesQueue.Count > 0)
            {
                asmData = assembliesQueue.Dequeue();
                SqlContext.Pipe.Send(string.Format("Exporting assembly file [{0}] to [{1}]", asmData.AssemblyFileName, asmData.OutputFileName)); //Send information about exported file back to the calling session
                ctx = null;
                try
                {
                    if (impersonateCurrentUser && SqlContext.WindowsIdentity != null) //Impersonate current account in case impersonateCurrentUser is selected and and WindowsAccount is used to login tinto SQL Server
                        ctx = SqlContext.WindowsIdentity.Impersonate();

                    using (FileStream byteStream = new FileStream(asmData.OutputFileName, FileMode.CreateNew))
                    {
                        byteStream.Write(asmData.Data, 0, (int)asmData.Data.Length);
                        byteStream.Close();
                    }
                }
                finally
                {
                    if (ctx != null) //If Impersonated, rollback to original Context
                        ctx.Undo();
                }
            }

        }
    }
}
