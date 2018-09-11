using System;
using Microsoft.SqlServer.Server;
using System.Runtime.Remoting.Messaging;
using System.Data.SqlTypes;
using System.Collections.Generic;

/// <summary>
/// Class contains CLR scalar functions for calculation of running totals of last X rows
/// </summary>
public class RunningTotalsQueue
{
    /// <summary>
    /// Calculates a running totals on TinyInt (Byte) data type for last X rows in queue
    /// </summary>
    /// <param name="val">Value of current row</param>
    /// <param name="id">ID of the function in single query</param>
    /// <param name="queueSize">Size of the queue (how much element to accumuleta in the running totals</param>
    /// <param name="nullValue">Value to be used for NULL values</param>
    /// <param name="nullForLessRows">Specifies whether function will return null if less then queueSize values are in the queue</param>
    /// <returns>SqlInt64 representing running total</returns>
    [SqlFunction(IsDeterministic = true)]
    public static SqlByte RunningTotalTinyIntQueue(SqlByte val, SqlByte id, SqlInt32 queueSize, SqlByte nullValue, SqlBoolean nullForLessRows)
    {
        string dataName = string.Format("MulstiSqlRt_{0}", id.IsNull ? 0 : id.Value);
        string queueName = dataName + "_q";
        object lastSum = CallContext.GetData(dataName);
        Queue<SqlByte> queue = (Queue<SqlByte>)CallContext.GetData(queueName);
        if (queue == null)
        {
            queue = new Queue<SqlByte>(queueSize.Value);
            CallContext.SetData(queueName, queue);
        }

        SqlByte total = lastSum != null ? (SqlByte)lastSum : SqlByte.Null;

        if (!val.IsNull)
            total = total.IsNull ? val : total + val;
        else
            total = total.IsNull ? nullValue : (nullValue.IsNull ? total : total + nullValue);

        SqlByte lastQueue = queue.Count == queueSize ? queue.Dequeue() : SqlByte.Null;
        queue.Enqueue(total);
        CallContext.SetData(dataName, total);

        if (!lastQueue.IsNull)
            total -= lastQueue;
        else if (nullForLessRows.IsTrue)
            total = SqlByte.Null;

        return total;
    }

    /// <summary>
    /// Calculates a running totals on SmallInt (Int16) data type for last X rows in queue
    /// </summary>
    /// <param name="val">Value of current row</param>
    /// <param name="id">ID of the function in single query</param>
    /// <param name="queueSize">Size of the queue (how much element to accumuleta in the running totals</param>
    /// <param name="nullValue">Value to be used for NULL values</param>
    /// <param name="nullForLessRows">Specifies whether function will return null if less then queueSize values are in the queue</param>
    /// <returns>SqlInt64 representing running total</returns>
    [SqlFunction(IsDeterministic = true)]
    public static SqlInt16 RunningTotalSmallIntQueue(SqlInt16 val, SqlByte id, SqlInt32 queueSize, SqlInt16 nullValue, SqlBoolean nullForLessRows)
    {
        string dataName = string.Format("MulstiSqlRt_{0}", id.IsNull ? 0 : id.Value);
        string queueName = dataName + "_q";
        object lastSum = CallContext.GetData(dataName);
        Queue<SqlInt16> queue = (Queue<SqlInt16>)CallContext.GetData(queueName);
        if (queue == null)
        {
            queue = new Queue<SqlInt16>(queueSize.Value);
            CallContext.SetData(queueName, queue);
        }

        SqlInt16 total = lastSum != null ? (SqlInt16)lastSum : SqlInt16.Null;

        if (!val.IsNull)
            total = total.IsNull ? val : total + val;
        else
            total = total.IsNull ? nullValue : (nullValue.IsNull ? total : total + nullValue);

        SqlInt16 lastQueue = queue.Count == queueSize ? queue.Dequeue() : SqlInt16.Null;
        queue.Enqueue(total);
        CallContext.SetData(dataName, total);

        if (!lastQueue.IsNull)
            total -= lastQueue;
        else if (nullForLessRows.IsTrue)
            total = SqlInt16.Null;

        return total;
    }

    /// <summary>
    /// Calculates a running totals on Int (Int32) data type for last X rows in queue
    /// </summary>
    /// <param name="val">Value of current row</param>
    /// <param name="id">ID of the function in single query</param>
    /// <param name="queueSize">Size of the queue (how much element to accumuleta in the running totals</param>
    /// <param name="nullValue">Value to be used for NULL values</param>
    /// <param name="nullForLessRows">Specifies whether function will return null if less then queueSize values are in the queue</param>
    /// <returns>SqlInt64 representing running total</returns>
    [SqlFunction(IsDeterministic = true)]
    public static SqlInt32 RunningTotalIntQueue(SqlInt32 val, SqlByte id, SqlInt32 queueSize, SqlInt32 nullValue, SqlBoolean nullForLessRows)
    {
        string dataName = string.Format("MulstiSqlRt_{0}", id.IsNull ? 0 : id.Value);
        string queueName = dataName + "_q";
        object lastSum = CallContext.GetData(dataName);
        Queue<SqlInt32> queue = (Queue<SqlInt32>)CallContext.GetData(queueName);
        if (queue == null)
        {
            queue = new Queue<SqlInt32>(queueSize.Value);
            CallContext.SetData(queueName, queue);
        }

        SqlInt32 total = lastSum != null ? (SqlInt32)lastSum : SqlInt32.Null;

        if (!val.IsNull)
            total = total.IsNull ? val : total + val;
        else
            total = total.IsNull ? nullValue : (nullValue.IsNull ? total : total + nullValue);

        SqlInt32 lastQueue = queue.Count == queueSize ? queue.Dequeue() : SqlInt32.Null;
        queue.Enqueue(total);
        CallContext.SetData(dataName, total);

        if (!lastQueue.IsNull)
            total -= lastQueue;
        else if (nullForLessRows.IsTrue)
            total = SqlInt32.Null;

        return total;
    }

    /// <summary>
    /// Calculates a running totals on BigInt (Int64) data type for last X rows in queue
    /// </summary>
    /// <param name="val">Value of current row</param>
    /// <param name="id">ID of the function in single query</param>
    /// <param name="queueSize">Size of the queue (how much element to accumuleta in the running totals</param>
    /// <param name="nullValue">Value to be used for NULL values</param>
    /// <param name="nullForLessRows">Specifies whether function will return null if less then queueSize values are in the queue</param>
    /// <returns>SqlInt64 representing running total</returns>
    [SqlFunction(IsDeterministic = true)]
    public static SqlInt64 RunningTotalBigIntQueue(SqlInt64 val, SqlByte id, SqlInt32 queueSize, SqlInt64 nullValue, SqlBoolean nullForLessRows)
    {
        string dataName = string.Format("MulstiSqlRt_{0}", id.IsNull ? 0 : id.Value);
        string queueName = dataName + "_q";
        object lastSum = CallContext.GetData(dataName);
        Queue<SqlInt64> queue = (Queue<SqlInt64>)CallContext.GetData(queueName);
        if (queue == null)
        {
            queue = new Queue<SqlInt64>(queueSize.Value);
            CallContext.SetData(queueName, queue);
        }

        SqlInt64 total = lastSum != null ? (SqlInt64)lastSum : SqlInt64.Null;

        if (!val.IsNull)
            total = total.IsNull ? val : total + val;
        else
            total = total.IsNull ? nullValue : (nullValue.IsNull ? total : total + nullValue);

        SqlInt64 lastQueue = queue.Count == queueSize ? queue.Dequeue() : SqlInt64.Null;
        queue.Enqueue(total);
        CallContext.SetData(dataName, total);

        if (!lastQueue.IsNull)
            total -= lastQueue;
        else if (nullForLessRows.IsTrue)
            total = SqlInt64.Null;

        return total;
    }

    /// <summary>
    /// Calculates a running totals on Flat (Double) data type for last X rows in queue
    /// </summary>
    /// <param name="val">Value of current row</param>
    /// <param name="id">ID of the function in single query</param>
    /// <param name="queueSize">Size of the queue (how much element to accumuleta in the running totals</param>
    /// <param name="nullValue">Value to be used for NULL values</param>
    /// <param name="nullForLessRows">Specifies whether function will return null if less then queueSize values are in the queue</param>
    /// <returns>SqlInt64 representing running total</returns>
    [SqlFunction(IsDeterministic = true)]
    public static SqlDouble RunningTotalFloatQueue(SqlDouble val, SqlByte id, SqlInt32 queueSize, SqlDouble nullValue, SqlBoolean nullForLessRows)
    {
        string dataName = string.Format("MulstiSqlRt_{0}", id.IsNull ? 0 : id.Value);
        string queueName = dataName + "_q";
        object lastSum = CallContext.GetData(dataName);
        Queue<SqlDouble> queue = (Queue<SqlDouble>)CallContext.GetData(queueName);
        if (queue == null)
        {
            queue = new Queue<SqlDouble>(queueSize.Value);
            CallContext.SetData(queueName, queue);
        }

        SqlDouble total = lastSum != null ? (SqlDouble)lastSum : SqlDouble.Null;

        if (!val.IsNull)
            total = total.IsNull ? val : total + val;
        else
            total = total.IsNull ? nullValue : (nullValue.IsNull ? total : total + nullValue);

        SqlDouble lastQueue = queue.Count == queueSize ? queue.Dequeue() : SqlDouble.Null;
        queue.Enqueue(total);
        CallContext.SetData(dataName, total);

        if (!lastQueue.IsNull)
            total -= lastQueue;
        else if (nullForLessRows.IsTrue)
            total = SqlDouble.Null;

        return total;
    }

    /// <summary>
    /// Calculates a running totals on Real (single) data type for last X rows in queue
    /// </summary>
    /// <param name="val">Value of current row</param>
    /// <param name="id">ID of the function in single query</param>
    /// <param name="queueSize">Size of the queue (how much element to accumuleta in the running totals</param>
    /// <param name="nullValue">Value to be used for NULL values</param>
    /// <param name="nullForLessRows">Specifies whether function will return null if less then queueSize values are in the queue</param>
    /// <returns>SqlInt64 representing running total</returns>
    [SqlFunction(IsDeterministic = true)]
    public static SqlSingle RunningTotalRealQueue(SqlSingle val, SqlByte id, SqlInt32 queueSize, SqlSingle nullValue, SqlBoolean nullForLessRows)
    {
        string dataName = string.Format("MulstiSqlRt_{0}", id.IsNull ? 0 : id.Value);
        string queueName = dataName + "_q";
        object lastSum = CallContext.GetData(dataName);
        Queue<SqlSingle> queue = (Queue<SqlSingle>)CallContext.GetData(queueName);
        if (queue == null)
        {
            queue = new Queue<SqlSingle>(queueSize.Value);
            CallContext.SetData(queueName, queue);
        }

        SqlSingle total = lastSum != null ? (SqlSingle)lastSum : SqlSingle.Null;

        if (!val.IsNull)
            total = total.IsNull ? val : total + val;
        else
            total = total.IsNull ? nullValue : (nullValue.IsNull ? total : total + nullValue);

        SqlSingle lastQueue = queue.Count == queueSize ? queue.Dequeue() : SqlSingle.Null;
        queue.Enqueue(total);
        CallContext.SetData(dataName, total);

        if (!lastQueue.IsNull)
            total -= lastQueue;
        else if (nullForLessRows.IsTrue)
            total = SqlSingle.Null;

        return total;
    }

    /// <summary>
    /// Calculates a running totals on Money data type for last X rows in queue
    /// </summary>
    /// <param name="val">Value of current row</param>
    /// <param name="id">ID of the function in single query</param>
    /// <param name="queueSize">Size of the queue (how much element to accumuleta in the running totals</param>
    /// <param name="nullValue">Value to be used for NULL values</param>
    /// <param name="nullForLessRows">Specifies whether function will return null if less then queueSize values are in the queue</param>
    /// <returns>SqlInt64 representing running total</returns>
    [SqlFunction(IsDeterministic = true)]
    public static SqlMoney RunningTotalMoneyQueue(SqlMoney val, SqlByte id, SqlInt32 queueSize, SqlMoney nullValue, SqlBoolean nullForLessRows)
    {
        string dataName = string.Format("MulstiSqlRt_{0}", id.IsNull ? 0 : id.Value);
        string queueName = dataName + "_q";
        object lastSum = CallContext.GetData(dataName);
        Queue<SqlMoney> queue = (Queue<SqlMoney>)CallContext.GetData(queueName);
        if (queue == null)
        {
            queue = new Queue<SqlMoney>(queueSize.Value);
            CallContext.SetData(queueName, queue);
        }

        SqlMoney total = lastSum != null ? (SqlMoney)lastSum : SqlMoney.Null;

        if (!val.IsNull)
            total = total.IsNull ? val : total + val;
        else
            total = total.IsNull ? nullValue : (nullValue.IsNull ? total : total + nullValue);

        SqlMoney lastQueue = queue.Count == queueSize ? queue.Dequeue() : SqlMoney.Null;
        queue.Enqueue(total);
        CallContext.SetData(dataName, total);

        if (!lastQueue.IsNull)
            total -= lastQueue;
        else if (nullForLessRows.IsTrue)
            total = SqlMoney.Null;

        return total;
    }

    /// <summary>
    /// Calculates a running totals on Decimal data type for last X rows in queue
    /// </summary>
    /// <param name="val">Value of current row</param>
    /// <param name="id">ID of the function in single query</param>
    /// <param name="queueSize">Size of the queue (how much element to accumuleta in the running totals</param>
    /// <param name="nullValue">Value to be used for NULL values</param>
    /// <param name="nullForLessRows">Specifies whether function will return null if less then queueSize values are in the queue</param>
    /// <returns>SqlInt64 representing running total</returns>
    [SqlFunction(IsDeterministic = true)]
    public static SqlDecimal RunningTotalDecimalQueue(SqlDecimal val, SqlByte id, SqlInt32 queueSize, SqlDecimal nullValue, SqlBoolean nullForLessRows)
    {
        string dataName = string.Format("MulstiSqlRt_{0}", id.IsNull ? 0 : id.Value);
        string queueName = dataName + "_q";
        object lastSum = CallContext.GetData(dataName);
        Queue<SqlDecimal> queue = (Queue<SqlDecimal>)CallContext.GetData(queueName);
        if (queue == null)
        {
            queue = new Queue<SqlDecimal>(queueSize.Value);
            CallContext.SetData(queueName, queue);
        }

        SqlDecimal total = lastSum != null ? (SqlDecimal)lastSum : SqlDecimal.Null;

        if (!val.IsNull)
            total = total.IsNull ? val : total + val;
        else
            total = total.IsNull ? nullValue : (nullValue.IsNull ? total : total + nullValue);

        SqlDecimal lastQueue = queue.Count == queueSize ? queue.Dequeue() : SqlDecimal.Null;
        queue.Enqueue(total);
        CallContext.SetData(dataName, total);

        if (!lastQueue.IsNull)
            total -= lastQueue;
        else if (nullForLessRows.IsTrue)
            total = SqlDecimal.Null;

        return total;
    }
}
