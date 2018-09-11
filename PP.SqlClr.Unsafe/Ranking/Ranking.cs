using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Server;
using System.Data.SqlTypes;
using System.Runtime.Remoting.Messaging;

public class Ranking
{
    [SqlFunction]
    public static SqlInt32 RankByValueOccurence(SqlInt32 initialValue, SqlString rankFieldValue, SqlString rankingValue, SqlByte rankFunctionID)
    {
        string dataName = string.Format("SQLRank{0}", rankFunctionID.IsNull ? 0 : rankFunctionID.Value);

        object rank = CallContext.GetData(dataName);

        SqlInt32 currentRank = rank == null ? (initialValue.IsNull ? new SqlInt32(1) : initialValue) : (SqlInt32)rank;
        SqlInt32 nextRank = rankFieldValue == rankingValue ? currentRank + 1 : currentRank;

        CallContext.SetData(dataName, nextRank);
        return currentRank;
    }
}
