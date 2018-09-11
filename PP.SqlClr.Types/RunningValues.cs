using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Server;
using System.Data.SqlTypes;

[Serializable]
//[SqlUserDefinedType(Format.UserDefined, MaxByteSize = 9)]
[SqlUserDefinedType(Format.Native, IsByteOrdered = true)]
public struct RunningValues : INullable//, IBinarySerialize
{
    private int _windowSize;

    private SqlInt32 _runningTotal;

    //public RunningValues()
    //{
    //    _isNull = true;
    //    _runningTotal = SqlInt32.Null;
    //}

    public RunningValues(int windowSize)
    {
        _isNull = false;
        _runningTotal = SqlInt32.Null;
        _windowSize = windowSize;
        _total = 0;
    }

    public RunningValues(SqlInt32 windowSize)
    {
        if (windowSize.IsNull)
        {
            _windowSize = 0;
            _isNull = true;
        }
        else
        {
            _isNull = false;
            _windowSize = windowSize.Value;            
        }
        _total = 0;
        _runningTotal = SqlInt32.Null;
    }

    private bool _isNull;
    public bool IsNull
    {
        get { return _isNull; }
        set {_isNull = value;}
    }

    public override string ToString()
    {
        if (this.IsNull)
            return "null";
        else
            return _windowSize.ToString();
    }

    public static RunningValues Null
    {
        get
        {
            RunningValues nullInstance = new RunningValues();
            nullInstance.IsNull = true;
            return nullInstance;
        }
    }

    public static RunningValues Parse(SqlString s)
    {
        int val;

        if (!s.IsNull && int.TryParse(s.Value, out val))
        {
            RunningValues rv = new RunningValues(val);
            return rv;
        }
        else
        {
            return Null;
        }
    }

    private int _total;
    public SqlInt32 RunningTotal(SqlInt32 inputValue, SqlInt32 order)
    {
        //return new SqlInt32(10);

        //if (_runningTotal.IsNull)
        //    _runningTotal = inputValue;
        //else if (!inputValue.IsNull)
        //{
        //    _runningTotal = new SqlInt32(_runningTotal.Value + inputValue.Value);
        //}
        //_total += inputValue.Value;
        _total++;
        return _total;;
    }


    public void Read(System.IO.BinaryReader r)
    {
        byte header = r.ReadByte();
        if ((header & 0x02) == 0x02)
            this._runningTotal = SqlInt32.Null;
        else
            this._runningTotal = 0;

        if ((header & 0x01) == 0x01)
        {
            this.IsNull = true;
            return;
        }

        this.IsNull = false;
        this._windowSize = r.ReadInt32();
        if (!_runningTotal.IsNull)
            this._runningTotal = r.ReadInt32();
    }

    public void Write(System.IO.BinaryWriter w)
    {
        byte header = 0;
        if (this.IsNull)
            header |= 0x01;

        if (_runningTotal.IsNull)
            header |= 0x02;

        w.Write(header);

        if ((header & 0x01) == 1)
        {
            return;
        }

        w.Write(this._windowSize);
        if (!_runningTotal.IsNull)
            w.Write(this._runningTotal.Value);
    }
}





