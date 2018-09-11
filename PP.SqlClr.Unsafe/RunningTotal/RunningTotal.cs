using System;
using Microsoft.SqlServer.Server;
using System.Runtime.Remoting.Messaging;
using System.Data.SqlTypes;
using System.Collections.Generic;

/// <summary>
/// Class contains CLR scalar functions for calculation of running totals
/// </summary>
public class RunningTotals
{
    /// <summary>
    /// Storage Structure for holding actual Total and row number for security check.
    /// </summary>
    /// <typeparam name="T">Totals Data Type</typeparam>
    private struct RtStorage<T> where T : struct        
    {
        public T Total;
        public int RowNo;
    }

    /// <summary>
    /// Calculates a running totals on TinyInt (byte) data type
    /// </summary>
    /// <param name="val">Value of current row</param>
    /// <param name="id">ID of the function in single query</param>
    /// <param name="rowNo">Specifies expecter rowNo. It is for security check to ensure correctness of running totals</param>
    /// <param name="nullValue">Value to be used for NULL values</param>
    /// <returns>SqlByte representing running total</returns>
    [SqlFunction(IsDeterministic = true)]
    public static SqlByte RunningTotalTinyInt(SqlByte val, SqlByte id, int rowNo, SqlByte nullValue)
    {
        string dataName = string.Format("MultiSqlRt_{0}", id.IsNull ? 0 : id.Value);
        
        object lastSum = CallContext.GetData(dataName);

        var storage = lastSum != null ? (RtStorage<SqlByte>)lastSum : new RtStorage<SqlByte>();
        storage.RowNo++;

        if (storage.RowNo != rowNo)
            throw new System.InvalidOperationException(string.Format("Rows were processed out of expected order. Expected RowNo: {0}, received RowNo: {1}", storage.RowNo, rowNo));

        if (!val.IsNull)
            storage.Total = storage.Total.IsNull ? val : storage.Total + val;
        else
            storage.Total = storage.Total.IsNull ? nullValue : (nullValue.IsNull ? storage.Total : storage.Total + nullValue);

        CallContext.SetData(dataName, storage);

        return storage.Total;
    }

    /// <summary>
    /// Calculates a running totals on SmallInt (Int) data type
    /// </summary>
    /// <param name="val">Value of current row</param>
    /// <param name="id">ID of the function in single query</param>
    /// <param name="rowNo">Specifies expecter rowNo. It is for security check to ensure correctness of running totals</param>
    /// <param name="nullValue">Value to be used for NULL values</param>
    /// <returns>SqlInt16 representing running total</returns>
    [SqlFunction(IsDeterministic = true)]
    public static SqlInt16 RunningTotalSmallInt(SqlInt16 val, SqlByte id, int rowNo, SqlInt16 nullValue)
    {
        string dataName = string.Format("MultiSqlRt_{0}", id.IsNull ? 0 : id.Value);

        object lastSum = CallContext.GetData(dataName);

        var storage = lastSum != null ? (RtStorage<SqlInt16>)lastSum : new RtStorage<SqlInt16>();
        storage.RowNo++;

        if (storage.RowNo != rowNo)
            throw new System.InvalidOperationException(string.Format("Rows were processed out of expected order. Expected RowNo: {0}, received RowNo: {1}", storage.RowNo, rowNo));

        if (!val.IsNull)
            storage.Total = storage.Total.IsNull ? val : storage.Total + val;
        else
            storage.Total = storage.Total.IsNull ? nullValue : (nullValue.IsNull ? storage.Total : storage.Total + nullValue);

        CallContext.SetData(dataName, storage);

        return storage.Total;
    }

    /// <summary>
    /// Calculates a running totals on Int (Int32) data type
    /// </summary>
    /// <param name="val">Value of current row</param>
    /// <param name="id">ID of the function in single query</param>
    /// <param name="rowNo">Specifies expecter rowNo. It is for security check to ensure correctness of running totals</param>
    /// <param name="nullValue">Value to be used for NULL values</param>
    /// <returns>SqlInt32 representing running total</returns>
    [SqlFunction(IsDeterministic = true)]
    public static SqlInt32 RunningTotalInt(SqlInt32 val, SqlByte id, int rowNo, SqlInt32 nullValue)
    {
        string dataName = string.Format("MultiSqlRt_{0}", id.IsNull ? 0 : id.Value);

        object lastSum = CallContext.GetData(dataName);

        var storage = lastSum != null ? (RtStorage<SqlInt32>)lastSum : new RtStorage<SqlInt32>();
        storage.RowNo++;

        if (storage.RowNo != rowNo)
            throw new System.InvalidOperationException(string.Format("Rows were processed out of expected order. Expected RowNo: {0}, received RowNo: {1}", storage.RowNo, rowNo));

        if (!val.IsNull)
            storage.Total = storage.Total.IsNull ? val : storage.Total + val;
        else
            storage.Total = storage.Total.IsNull ? nullValue : (nullValue.IsNull ? storage.Total : storage.Total + nullValue);

        CallContext.SetData(dataName, storage);

        return storage.Total;
    }

    /// <summary>
    /// Calculates a running totals on BigInt (Int64) data type
    /// </summary>
    /// <param name="val">Value of current row</param>
    /// <param name="id">ID of the function in single query</param>
    /// <param name="rowNo">Specifies expecter rowNo. It is for security check to ensure correctness of running totals</param>
    /// <param name="nullValue">Value to be used for NULL values</param>
    /// <returns>SqlInt64 representing running total</returns>
    [SqlFunction(IsDeterministic = true)]
    public static SqlInt64 RunningTotalBigInt(SqlInt64 val, SqlByte id, int rowNo, SqlInt64 nullValue)
    {
        string dataName = string.Format("MultiSqlRt_{0}", id.IsNull ? 0 : id.Value);

        object lastSum = CallContext.GetData(dataName);

        var storage = lastSum != null ? (RtStorage<SqlInt64>)lastSum : new RtStorage<SqlInt64>();
        storage.RowNo++;

        if (storage.RowNo != rowNo)
            throw new System.InvalidOperationException(string.Format("Rows were processed out of expected order. Expected RowNo: {0}, received RowNo: {1}", storage.RowNo, rowNo));

        if (!val.IsNull)
            storage.Total = storage.Total.IsNull ? val : storage.Total + val;
        else
            storage.Total = storage.Total.IsNull ? nullValue : (nullValue.IsNull ? storage.Total : storage.Total + nullValue);

        CallContext.SetData(dataName, storage);

        return storage.Total;
    }

    /// <summary>
    /// Calculates a running totals on Float (Double) data type
    /// </summary>
    /// <param name="val">Value of current row</param>
    /// <param name="id">ID of the function in single query</param>
    /// <param name="rowNo">Specifies expecter rowNo. It is for security check to ensure correctness of running totals</param>
    /// <param name="nullValue">Value to be used for NULL values</param>
    /// <returns>SqlDouble representing running total</returns>
    [SqlFunction(IsDeterministic = true)]
    public static SqlDouble RunningTotalFloat(SqlDouble val, SqlByte id, int rowNo, SqlDouble nullValue)
    {
        string dataName = string.Format("MultiSqlRt_{0}", id.IsNull ? 0 : id.Value);

        object lastSum = CallContext.GetData(dataName);

        var storage = lastSum != null ? (RtStorage<SqlDouble>)lastSum : new RtStorage<SqlDouble>();
        storage.RowNo++;

        if (storage.RowNo != rowNo)
            throw new System.InvalidOperationException(string.Format("Rows were processed out of expected order. Expected RowNo: {0}, received RowNo: {1}", storage.RowNo, rowNo));

        if (!val.IsNull)
            storage.Total = storage.Total.IsNull ? val : storage.Total + val;
        else
            storage.Total = storage.Total.IsNull ? nullValue : (nullValue.IsNull ? storage.Total : storage.Total + nullValue);

        CallContext.SetData(dataName, storage);

        return storage.Total;
    }

    /// <summary>
    /// Calculates a running totals on Real (Single) data type
    /// </summary>
    /// <param name="val">Value of current row</param>
    /// <param name="id">ID of the function in single query</param>
    /// <param name="rowNo">Specifies expecter rowNo. It is for security check to ensure correctness of running totals</param>
    /// <param name="nullValue">Value to be used for NULL values</param>
    /// <returns>SqlSingle representing running total</returns>
    [SqlFunction(IsDeterministic = true)]
    public static SqlSingle RunningTotalReal(SqlSingle val, SqlByte id, int rowNo, SqlSingle nullValue)
    {
        string dataName = string.Format("MultiSqlRt_{0}", id.IsNull ? 0 : id.Value);

        object lastSum = CallContext.GetData(dataName);

        var storage = lastSum != null ? (RtStorage<SqlSingle>)lastSum : new RtStorage<SqlSingle>();
        storage.RowNo++;

        if (storage.RowNo != rowNo)
            throw new System.InvalidOperationException(string.Format("Rows were processed out of expected order. Expected RowNo: {0}, received RowNo: {1}", storage.RowNo, rowNo));

        if (!val.IsNull)
            storage.Total = storage.Total.IsNull ? val : storage.Total + val;
        else
            storage.Total = storage.Total.IsNull ? nullValue : (nullValue.IsNull ? storage.Total : storage.Total + nullValue);

        CallContext.SetData(dataName, storage);

        return storage.Total;
    }

    /// <summary>
    /// Calculates a running totals on Money data type
    /// </summary>
    /// <param name="val">Value of current row</param>
    /// <param name="id">ID of the function in single query</param>
    /// <param name="rowNo">Specifies expecter rowNo. It is for security check to ensure correctness of running totals</param>
    /// <param name="nullValue">Value to be used for NULL values</param>
    /// <returns>SqlMoney representing running total</returns>
    [SqlFunction(IsDeterministic = true)]
    public static SqlMoney RunningTotalMoney(SqlMoney val, SqlByte id, int rowNo, SqlMoney nullValue)
    {
        string dataName = string.Format("MultiSqlRt_{0}", id.IsNull ? 0 : id.Value);

        object lastSum = CallContext.GetData(dataName);

        var storage = lastSum != null ? (RtStorage<SqlMoney>)lastSum : new RtStorage<SqlMoney>();
        storage.RowNo++;

        if (storage.RowNo != rowNo)
            throw new System.InvalidOperationException(string.Format("Rows were processed out of expected order. Expected RowNo: {0}, received RowNo: {1}", storage.RowNo, rowNo));

        if (!val.IsNull)
            storage.Total = storage.Total.IsNull ? val : storage.Total + val;
        else
            storage.Total = storage.Total.IsNull ? nullValue : (nullValue.IsNull ? storage.Total : storage.Total + nullValue);

        CallContext.SetData(dataName, storage);

        return storage.Total;
    }

    /// <summary>
    /// Calculates a running totals on Decimal data type
    /// </summary>
    /// <param name="val">Value of current row</param>
    /// <param name="id">ID of the function in single query</param>
    /// <param name="rowNo">Specifies expecter rowNo. It is for security check to ensure correctness of running totals</param>
    /// <param name="nullValue">Value to be used for NULL values</param>
    /// <returns>SqlDecimal representing running total</returns>
    [SqlFunction(IsDeterministic = true)]
    public static SqlDecimal RunningTotalDecimal(SqlDecimal val, SqlByte id, int rowNo, SqlDecimal nullValue)
    {
        string dataName = string.Format("MultiSqlRt_{0}", id.IsNull ? 0 : id.Value);

        object lastSum = CallContext.GetData(dataName);

        var storage = lastSum != null ? (RtStorage<SqlDecimal>)lastSum : new RtStorage<SqlDecimal>();
        storage.RowNo++;

        if (storage.RowNo != rowNo)
            throw new System.InvalidOperationException(string.Format("Rows were processed out of expected order. Expected RowNo: {0}, received RowNo: {1}", storage.RowNo, rowNo));

        if (!val.IsNull)
            storage.Total = storage.Total.IsNull ? val : storage.Total + val;
        else
            storage.Total = storage.Total.IsNull ? nullValue : (nullValue.IsNull ? storage.Total : storage.Total + nullValue);

        CallContext.SetData(dataName, storage);

        return storage.Total;
    }
}
