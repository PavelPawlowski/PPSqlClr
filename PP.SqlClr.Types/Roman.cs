using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

/// <summary>
/// Represents a Roman Number
/// </summary>
[Microsoft.SqlServer.Server.SqlUserDefinedType(Format.UserDefined,
     IsByteOrdered = true, MaxByteSize = 2)]
public struct Roman : IComparable<Roman>, IComparable<int>, IComparable, IEquatable<Roman>, IEquatable<int>, INullable, Microsoft.SqlServer.Server.IBinarySerialize
{
    private bool _isNull;
    private static readonly int[] values = new int[] { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
    private static readonly string[] numerals = new string[] { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
    /// <summary>
    /// Represets possible roman characters "IVXLCDM"
    /// </summary>
    private const string romanCharacters = "IVXLCDM";
    /// <summary>
    /// Represents possible subtractive characters "IXC"
    /// </summary>
    private const string subtractiveCharacters = "IXC";
    /// <summary>
    /// Represetns values of roman characters.
    /// </summary>
    private static readonly int[] romanValues = new int[] { 1, 5, 10, 50, 100, 500, 1000 };

    /// <summary>
    /// Returns NULL Roman Value
    /// </summary>
    public static Roman Null { get; } = new Roman() { _isNull = true, _integerValue = -1, _romanString = string.Empty };

    /// <summary>
    /// Creates a new instance of Roman number from a string representing Roman number
    /// </summary>
    /// <param name="romanString"></param>
    public Roman(string romanString)
    {
        _integerValue = RomanStringToInt(romanString);
        _romanString = romanString.ToUpper();
        _isNull = false;
    }

    /// <summary>
    /// Creates a new instance of Roman number from an integer
    /// </summary>
    /// <param name="integerValue"></param>
    public Roman(int integerValue)
    {
        _romanString = IntToRomanString(integerValue);
        _integerValue = integerValue;
        _isNull = false;
    }


    [SqlMethod(OnNullCall = false)]
    public static Roman Parse(SqlString s)
    {
        if (s.IsNull)
            return Null;

        return new Roman(s.Value);
    }

    private string _romanString;
    /// <summary>
    /// Gets string representation of the Roman number
    /// </summary>
    public string RomanString
    {
        set
        {
            Roman r = new Roman(value);
            _romanString = r.RomanString;
            _integerValue = r.IntegerValue;
        }
        get
        {
            if (IsNull)
                return "NULL";
            else if (_integerValue == 0)
                return "N";
            else
                return _romanString;
        }
    }

    private int _integerValue;
    /// <summary>
    /// Gets integer representation of the Roman numer
    /// </summary>
    public int IntegerValue
    {
        set
        {
            Roman r = new Roman(value);
            _integerValue = r.IntegerValue;
            _romanString = r.RomanString;
        }
        get { return IsNull ? -1 : _integerValue; }
    }

    public override string ToString()
    {
        return RomanString;
    }

    #region Public Static Methods
    /// <summary>
    /// Converts a string representing a Roman value into an integer number
    /// </summary>
    /// <param name="romanString">String representing Roman number to be converted</param>
    /// <returns>integer representation of the Roman string</returns>

    [SqlMethod(OnNullCall = false)]
    public static int RomanStringToInt(string romanString)
    {
        string roman = romanString.Trim().ToUpper();
        if (roman == "N") return 0;

        if (roman.IndexOf('V') != roman.LastIndexOf('V') ||
            roman.IndexOf('L') != roman.LastIndexOf('L') ||
            roman.IndexOf('D') != roman.LastIndexOf('D'))
        {
            throw new System.ArgumentException("'V', 'L', 'D', can occure only once in the entire roman string", "romanString");
        }

        int cnt = 1;
        char lastNumeral = 'Z';
        foreach (char numeral in roman)
        {
            if (!romanCharacters.Contains(numeral))
                throw new System.ArgumentException(string.Format("'{0}' is not a valid Roman numeral", numeral), "romanString");

            if (numeral == lastNumeral)
            {
                if (++cnt == 4)
                    throw new System.ArgumentException(string.Format("Numeral '{0}' occures more than three times in a line in the roman string. There could be maximum 3 occurenses of roman numeral in a line", numeral), "romanString");
            }
            else
            {
                cnt = 1;
                lastNumeral = numeral;
            }
        }

        int ptr = 0;
        List<int> values = new List<int>();
        int maxDigit = 1000;
        while (ptr < roman.Length)
        {
            char numeral = roman[ptr];
            int digit = romanValues[romanCharacters.IndexOf(numeral)];

            if (digit > maxDigit)
                throw new System.ArgumentException("Numerals must be in Subtractive Combination", "romanString");

            if (ptr < roman.Length - 1)
            {
                char nextNumeral = roman[ptr + 1];
                int nextDigit = romanValues[romanCharacters.IndexOf(nextNumeral)];

                if (nextDigit > digit)
                {
                    if (!subtractiveCharacters.Contains(numeral) ||
                             nextDigit > (digit * 10) ||
                             roman.Split(numeral).Length > 3)
                    {
                        throw new System.ArgumentException("Numerals must be in Subtractive Combination", "romanString");
                    }
                    maxDigit = digit - (int)1;
                    digit = nextDigit - digit;
                    ptr++;
                }
            }
            values.Add(digit);
            ptr++;
        }

        for (int i = 0; i < values.Count - 1; i++)
        {
            if (values[i] < values[i + 1])
                throw new ArgumentException("Numeral values must be in reducing order", "romanString");
        }

        int total = 0;
        foreach (int digit in values)
        {
            total += digit;
        }
        return total;
    }

    /// <summary>
    /// Converts an Array of RomanDigits into equivalent RomanString
    /// </summary>
    /// <param name="romanArray">RomanDigits array to be converted</param>
    /// <returns>string representing Roman number represented by the array of RomanDigits</returns>
    [SqlMethod(OnNullCall = false)]
    public static string RomanDigitArrayToRomanString(RomanDigit[] romanArray)
    {
        if (romanArray == null)
            throw new System.ArgumentNullException("romanArray", "romanArray cannot be null");
        if (romanArray.Length == 0)
            throw new System.ArgumentException("Roman array must be non empty array of RomanDigits", "romanArray");

        StringBuilder sb = new StringBuilder(romanArray.Length);
        foreach (RomanDigit dr in romanArray)
        {
            sb.Append(dr.ToString());
        }

        return sb.ToString();
    }

    /// <summary>
    /// Converts an array of RomanDigits into equivalent integer value
    /// </summary>
    /// <param name="romanArray">Array of RomanDigits to be converted</param>
    /// <returns>integer representation of the Roman number represented by array of RomanDigits</returns>
    [SqlMethod(OnNullCall = false)]
    public static int RomanDigitArrayToInt(RomanDigit[] romanArray)
    {
        return RomanStringToInt(RomanDigitArrayToRomanString(romanArray));
    }

    /// <summary>
    /// Converts an Integer value to a string representation of Roman number
    /// </summary>
    /// <param name="integerValue">integer value to be converted into the Roman string</param>
    /// <returns>string representing Roman value</returns>
    [SqlMethod(OnNullCall = false)]
    public static string IntToRomanString(int integerValue)
    {
        if (integerValue < 0 || integerValue > 3999)
        {
            throw new ArgumentOutOfRangeException("integerValue", "Value must be in the range 0 - 3999.");
        }

        if (integerValue == 0)
            return "N";
        else
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < values.Length; i++)
            {
                while (integerValue >= values[i])
                {
                    integerValue -= values[i];
                    sb.Append(numerals[i]);
                }
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// Converts and Integer value to an array of RomanDigits representing the Roman number
    /// </summary>
    /// <param name="integerValue">Integer value to be converted into the array fo Roman digits</param>
    /// <returns>Array of RomanDigit</returns>
    [SqlMethod(OnNullCall = false)]
    public static RomanDigit[] IntToRomanDigitArray(int integerValue)
    {
        return RomanStringToRomanDigitArray(IntToRomanString(integerValue));
    }

    /// <summary>
    /// Converts RomanString to the array of RomanDigits
    /// </summary>
    /// <param name="romanString">Roman String to be converted to the array of RomanDigits</param>
    /// <returns>RomanDigits array representing the RomanString</returns>
    [SqlMethod(OnNullCall = false)]
    public static RomanDigit[] RomanStringToRomanDigitArray(string romanString)
    {
        try
        {
            RomanDigit[] rda = new RomanDigit[romanString.Length];
            for (int i = 0; i < romanString.Length; i++)
            {
                rda[i] = (RomanDigit)Enum.Parse(typeof(RomanDigit), new String(romanString[i], 1));
            }
            return rda;
        }
        catch (Exception e)
        {
            throw new System.InvalidOperationException("The roman string is not representing a Roman number", e);
        }
    }

    /// <summary>
    /// Minimum value of the Roman Number
    /// </summary>
    public static readonly Roman MinValue = new Roman(0);

    /// <summary>
    /// Maximum value of the Roman Number
    /// </summary>
    public static readonly Roman MaxValue = new Roman(3999);

    #endregion

    #region IComparable Members

    [SqlMethod(OnNullCall = false)]
    public int CompareTo(object obj)
    {
        if (obj is int)
            return CompareTo((int)obj);
        else if (obj is Roman)
            return CompareTo((Roman)obj);
        else
            throw new System.ArgumentException("obj is not Roman or integer value", "obj");
    }

    #endregion

    #region IComparable<int> Members

    [SqlMethod(OnNullCall = false)]
    public int CompareTo(int other)
    {
        return IntegerValue.CompareTo(other);
    }

    #endregion

    #region IComparable<Roman> Members

    [SqlMethod(OnNullCall = false)]
    public int CompareTo(Roman other)
    {
        return IntegerValue.CompareTo(other.IntegerValue);
    }

    #endregion

    #region IEquatable<Roman> Members

    [SqlMethod(OnNullCall = false)]
    public bool Equals(Roman other)
    {
        return IntegerValue.Equals(other.IntegerValue);
    }

    #endregion

    #region IEquatable<int> Members

    [SqlMethod(OnNullCall = false)]
    public bool Equals(int other)
    {
        return IntegerValue.Equals(other);
    }

    [SqlMethod(OnNullCall = false)]
    public override bool Equals(object obj)
    {
        if (obj is Roman)
            return this.Equals((Roman)obj);
        else if (obj is int)
            return this.Equals((int)obj);
        else return false;
    }

    #endregion

    public override int GetHashCode()
    {
        return IntegerValue.GetHashCode();
    }

    #region Operators overloading
    public static bool operator ==(Roman a, Roman b)
    {
        return a.IntegerValue == b.IntegerValue;
    }

    public static bool operator !=(Roman a, Roman b)
    {
        return !(a == b);
    }

    public static bool operator >(Roman a, Roman b)
    {
        return a.IntegerValue > b.IntegerValue;
    }

    public static bool operator >=(Roman a, Roman b)
    {
        return a.IntegerValue >= b.IntegerValue;
    }

    public static bool operator <(Roman a, Roman b)
    {
        return a.IntegerValue < b.IntegerValue;
    }

    public static bool operator <=(Roman a, Roman b)
    {
        return a.IntegerValue <= b.IntegerValue;
    }

    public static Roman operator +(Roman a, Roman b)
    {
        return new Roman(a.IntegerValue + b.IntegerValue);
    }

    public static Roman operator -(Roman a, Roman b)
    {
        return new Roman(a.IntegerValue - b.IntegerValue);
    }

    public static Roman operator *(Roman a, Roman b)
    {
        return new Roman(a.IntegerValue * b.IntegerValue);
    }

    public static Roman operator /(Roman a, Roman b)
    {
        return new Roman(a.IntegerValue / b.IntegerValue);
    }

    public static Roman operator %(Roman a, Roman b)
    {
        return new Roman(a.IntegerValue % b.IntegerValue);
    }

    public static Roman operator &(Roman a, Roman b)
    {
        return new Roman(a.IntegerValue & b.IntegerValue);
    }

    public static Roman operator |(Roman a, Roman b)
    {
        return new Roman(a.IntegerValue | b.IntegerValue);
    }

    public static Roman operator ^(Roman a, Roman b)
    {
        return new Roman(a.IntegerValue ^ b.IntegerValue);
    }

    public static Roman operator ++(Roman a)
    {
        return new Roman(a.IntegerValue + 1);
    }

    public static Roman operator --(Roman a)
    {
        return new Roman(a.IntegerValue - 1);
    }

    #endregion

    #region Cast operators overloading

    public static implicit operator int(Roman a)
    {
        return a.IntegerValue;
    }

    public static implicit operator Roman(int a)
    {
        return new Roman(a);
    }

    public static implicit operator Roman(Int16 a)
    {
        return new Roman(a);
    }

    public static implicit operator Roman(UInt16 a)
    {
        return new Roman((int)a);
    }

    public static implicit operator Roman(byte a)
    {
        return new Roman(a);
    }

    public static explicit operator Roman(uint a)
    {
        return new Roman((int)a);
    }

    public static explicit operator string(Roman a)
    {
        return a.RomanString;
    }

    public static explicit operator Roman(string a)
    {
        return new Roman(a);
    }

    #endregion

    public bool IsNull
    {
        set
        {
            if (value == true)
            {
                _isNull = true;
                _integerValue = -1;
                _romanString = string.Empty;
            }
        }
        get { return _isNull; }
    }

    void IBinarySerialize.Read(System.IO.BinaryReader r)
    {
        short s = r.ReadInt16();
        if (s == -1)
        {
            _isNull = true;
            _integerValue = -1;
            _romanString = string.Empty;
        }
        else
        {
            Roman rn = new Roman(s);
            _isNull = rn.IsNull;
            _integerValue = rn.IntegerValue;
            _romanString = rn.RomanString;
        }
    }

    void IBinarySerialize.Write(System.IO.BinaryWriter w)
    {
        w.Write((short)_integerValue);
    }
}

/// <summary>
/// Represents a RomanDigit
/// </summary>
public enum RomanDigit
{
    /// <summary>
    /// 1
    /// </summary>
    I = 1,
    /// <summary>
    /// 5
    /// </summary>
    V = 5,
    /// <summary>
    /// 10
    /// </summary>
    X = 10,
    /// <summary>
    /// 50
    /// </summary>
    L = 50,
    /// <summary>
    /// 100
    /// </summary>
    C = 100,
    /// <summary>
    /// 500
    /// </summary>
    D = 500,
    /// <summary>
    /// 1000
    /// </summary>
    M = 1000
}

