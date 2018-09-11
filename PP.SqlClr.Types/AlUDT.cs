using System;
using System.Data;
using System.Data.Sql;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Globalization;

[Serializable]
[SqlUserDefinedType(Format.Native, IsByteOrdered = true)]
public struct UmAlQuraDateTime : INullable
{

    /*
     * Private state.
     */

    private long dtTicks;
    private bool isNull;

    // Calendar object used for all calendar-specific operations
    private static readonly UmAlQuraCalendar s_calendar =
        new UmAlQuraCalendar();

    // For correct formatting we need to provie a culture code for 
    // a country that uses the Um Al Qura calendar: Saudi Arabia.
    private static readonly CultureInfo ci =
        new CultureInfo("ar-SA", false);


    /*
     * Null-Handling
     */

    // get a null instance
    public static UmAlQuraDateTime Null
    {
        get
        {
            UmAlQuraDateTime dt = new UmAlQuraDateTime();
            dt.isNull = true;
            return dt;
        }
    }

    public bool IsNull
    {
        get
        {
            return this.isNull;
        }
    }

    /*
     * Constructors
     */

    public UmAlQuraDateTime(long ticks)
    {
        isNull = false;
        dtTicks = ticks;
    }

    public UmAlQuraDateTime(DateTime time)
        : this(time.Ticks)
    {
    }

    /*
     * Factory routines.
     */

    public static UmAlQuraDateTime Parse(SqlString s)
    {
        if (s.IsNull) return Null;
        DateTime t = DateTime.Parse(s.Value);
        return new UmAlQuraDateTime(t);
    }

    public static UmAlQuraDateTime ParseArabic(SqlString s)
    {
        if (s.IsNull) return Null;
        DateTime t = DateTime.Parse(s.Value, ci);
        return new UmAlQuraDateTime(t);
    }

    public static UmAlQuraDateTime FromSqlDateTime(SqlDateTime d)
    {
        if (d.IsNull) return Null;
        return new UmAlQuraDateTime(d.Value);
    }

    public static UmAlQuraDateTime Now
    {
        get
        {
            return new UmAlQuraDateTime(DateTime.Now);
        }
    }

    /*
     * Conversion Routines
     */

    public DateTime DateTime
    {
        get { return new DateTime(this.dtTicks); }
    }

    public SqlDateTime ToSqlDateTime()
    {
        return new SqlDateTime(this.DateTime);
    }

    public override String ToString()
    {
        return this.DateTime.ToString(ci);
    }

    public String ToStringUsingFormat(String format)
    {
        return this.DateTime.ToString(format, ci);
    }

    /*
     * Methods for getting date parts.
     */

    public int Year
    {
        get
        {
            return s_calendar.GetYear(this.DateTime);
        }
    }

    public int Month
    {
        get
        {
            return s_calendar.GetMonth(this.DateTime);
        }
    }

    public int Day
    {
        get
        {
            return s_calendar.GetDayOfMonth(this.DateTime);
        }
    }

    /*
     * Date arithmetic methods.
     */

    public UmAlQuraDateTime AddYears(int years)
    {
        return new
            UmAlQuraDateTime(s_calendar.AddYears(this.DateTime, years));
    }

    public UmAlQuraDateTime AddDays(int days)
    {
        return new
            UmAlQuraDateTime(s_calendar.AddDays(this.DateTime, days));
    }

    public double DiffDays(UmAlQuraDateTime other)
    {
        TimeSpan diff = DateTime.Subtract(other.DateTime);
        return diff.Days;
    }
}

