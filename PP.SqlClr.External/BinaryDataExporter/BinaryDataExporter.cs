using Microsoft.SqlServer.Server;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;

public class BinaryDataExporter
{
    /// <summary>
    /// Context connection string
    /// </summary>
    public const string ContextConnectionString = "context connection=true";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceQuery">Source query providing binary data and corresponding files for data export</param>
    /// <param name="filePathFieldName">Name of the field providing full path to export file</param>
    /// <param name="binaryDataFieldName">Name of the field containing binary data to be exported</param>
    /// <param name="connectionString">Connection string to be used for the source query</param>
    /// <param name="createPath">Specifies whether directories in path of target filename should be automatically created if not exists</param>
    private static void ExportBinaryDataInternal(string sourceQuery, string filePathFieldName, string binaryDataFieldName, string connectionString, bool createPath, bool reportProgress)
    {
        byte[] buffer = new byte[65536];

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            //Command to execute the [internal].[get_project_internal] to get the decrypted data stream with project content
            SqlCommand cmd = new SqlCommand(sourceQuery, con);

            con.Open();

            //get the decrypted project data stream 
            using (var reader = cmd.ExecuteReader(System.Data.CommandBehavior.SingleResult | System.Data.CommandBehavior.SequentialAccess))
            {
                string lastFilePath = string.Empty;
                int filePathField = reader.GetOrdinal(filePathFieldName);
                int binaryField = reader.GetOrdinal(binaryDataFieldName);

                while (reader.Read())
                {
                    //file path is not null
                    if (!reader.IsDBNull(filePathField))
                    {
                        string fullFileName = reader.GetString(filePathField);
                        string path = Path.GetDirectoryName(fullFileName);
                        //if createPath is true and path has changed, check path existence and craete if if not exists
                        if (createPath && !path.Equals(lastFilePath))
                        {
                            Directory.CreateDirectory(path);
                            lastFilePath = path;
                        }

                        long bytesRead = 0;
                        long dataIndex = 0;

                        //write binary data to output file
                        using (FileStream fs = File.Open(fullFileName, FileMode.Create))
                        {
                            //Read the data in 64kB chunks and write to output file
                            do
                            {
                                bytesRead = reader.GetBytes(binaryField, dataIndex, buffer, 0, buffer.Length);
                                dataIndex += bytesRead;

                                fs.Write(buffer, 0, (int)bytesRead);

                            } while (bytesRead == buffer.LongLength);

                            fs.Close();
                        }

                        if (reportProgress)
                            SqlContext.Pipe.Send(string.Format("Binary data written to: {0}", fullFileName));
                    }
                }
            }

            con.Close();
        }
    }

    /// <summary>
    /// Exports binary data provided by the source query to files on disk
    /// </summary>
    /// <param name="sourceQuery">source query providing binary data and corresponding files for data export</param>
    /// <param name="filePathFieldName">Name of the field providing full path to export file</param>
    /// <param name="binaryDataFieldName">Name of the field containing binary data to be exported</param>
    /// <param name="createPath">Specifies whether directories in path of target filename should be automatically created if not exists</param>
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void ExportBinaryData(SqlString sourceQuery, SqlString filePathFieldName, SqlString binaryDataFieldName, SqlBoolean createPath, SqlBoolean reportProgress)
    {
        ExportBinaryDataInternal(sourceQuery.Value, filePathFieldName.Value, binaryDataFieldName.Value, ContextConnectionString, createPath.Value, reportProgress.Value);
    }

    /// <summary>
    /// Exports binary data from a binary column or variable to a file on disk
    /// </summary>
    /// <param name="binaryData">Binary data to be exported</param>
    /// <param name="targetFile">Full Path to target file</param>
    /// <param name="createPath">Specifies whether directories in path of target filename should be automatically created if not exists</param>
    /// <returns></returns>
    [SqlFunction]
    public static SqlInt32 ExportBinaryColumn(SqlBytes binaryData, SqlString targetFile, SqlBoolean createPath)
    {
        byte[] buffer = new byte[65536];

        if (createPath.IsTrue)
        {
            string filePath = Path.GetDirectoryName(targetFile.Value);
            Directory.CreateDirectory(filePath);
        }

        long bytesRead;
        long dataIndex = 0;

        using (FileStream fs = File.Open(targetFile.Value, FileMode.Create))
        {
            bytesRead = binaryData.Read(dataIndex, buffer, 0, buffer.Length);
            dataIndex += bytesRead;
            fs.Write(buffer, 0, (int)bytesRead);
        }

        return new SqlInt32((int)dataIndex);
    }
}