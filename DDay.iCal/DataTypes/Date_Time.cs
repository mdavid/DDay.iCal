using System;
using System.Diagnostics;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using DDay.iCal.Objects;
using DDay.iCal.Components;

namespace DDay.iCal.DataTypes
{
    [DebuggerDisplay("{HasTime ? Value.ToString() : Value.ToShortDateString()}")]
    public class Date_Time : iCalDataType
    {
        #region Private Fields

        private DateTime m_Value;
        private bool m_HasDate = false;
        private bool m_HasTime = false;
        private TZID m_TZID = null;
        private DDay.iCal.Components.TimeZone.TimeZoneInfo m_TimeZoneInfo = null;
        private iCalendar m_iCalendar = null;

        #endregion

        #region Public Properties

        public DateTime Local
        {
            get
            {
                if (Value.Kind == DateTimeKind.Local &&
                    TimeZoneInfo != null)
                    return UTC.ToLocalTime();
                else return Value.ToLocalTime();
            }
        }

        public DateTime UTC
        {
            get
            {
                if (Value.Kind == DateTimeKind.Local)
                {
                    DateTime value = Value;
                    if (TimeZoneInfo != null)
                    {
                        int mult = TimeZoneInfo.TZOffsetTo.Positive ? -1 : 1;
                        value = value.AddHours(TimeZoneInfo.TZOffsetTo.Hours * mult);
                        value = value.AddMinutes(TimeZoneInfo.TZOffsetTo.Minutes * mult);
                        value = value.AddSeconds(TimeZoneInfo.TZOffsetTo.Seconds * mult);
                        value = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                    }
                    else value = value.ToUniversalTime();
                    return value;
                }
                else return Value.ToUniversalTime();
            }
        }

        public DDay.iCal.Components.TimeZone.TimeZoneInfo TimeZoneInfo
        {
            get
            {
                if (m_TimeZoneInfo == null && TZID != null)
                {
                    if (iCalendar != null)
                    {
                        DDay.iCal.Components.TimeZone tz = iCalendar.GetTimeZone(TZID);
                        if (tz != null)
                            m_TimeZoneInfo = tz.GetTimeZoneInfo(this);
                    }
                }
                return m_TimeZoneInfo;
            }
        }

        public iCalendar iCalendar
        {
            get { return m_iCalendar; }
            set { m_iCalendar = value; }
        }

        public DateTime Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        public bool HasDate
        {
            get { return m_HasDate; }
            set { m_HasDate = value; }
        }        

        public bool HasTime
        {
            get { return m_HasTime; }
            set { m_HasTime = value; }
        }

        public TZID TZID
        {
            get { return m_TZID; }
            set { m_TZID = value; }
        }

        public int Year
        {
            get { return Value.Year; }
        }

        public int Month
        {
            get { return Value.Month; }
        }

        public int Day
        {
            get { return Value.Day; }
        }

        public int Hour
        {
            get { return Value.Hour; }
        }

        public int Minute
        {
            get { return Value.Minute; }
        }

        public int Second
        {
            get { return Value.Second; }
        }

        public int Millisecond
        {
            get { return Value.Millisecond; }
        }

        public long Ticks
        {
            get { return Value.Ticks; }
        }

        public DayOfWeek DayOfWeek
        {
            get { return Value.DayOfWeek; }
        }

        public int DayOfYear
        {
            get { return Value.DayOfYear; }
        }

        public DateTime Date
        {
            get { return Value.Date; }
        }

        public TimeSpan TimeOfDay
        {
            get { return Value.TimeOfDay; }
        }

        #endregion

        #region Constructors

        public Date_Time() { }
        public Date_Time(Date_Time value)
        {
            CopyFrom(value);
        }
        public Date_Time(string value) : this()
        {
            CopyFrom(Parse(value));
        }
        public Date_Time(DateTime value) : this(value, null, null) {}
        public Date_Time(DateTime value, TZID tzid, iCalendar iCal)
        {
            this.Value = value;
            this.HasDate = true;
            this.HasTime = (value.Second == 0 && value.Minute == 0 && value.Hour == 0) ? false : true;
            this.TZID = tzid;
            this.iCalendar = iCal;
        }
        public Date_Time(int year, int month, int day, int hour, int minute, int second)
            : this(new DateTime(year, month, day, hour, minute, second, DateTimeKind.Local)) { }
        public Date_Time(int year, int month, int day, int hour, int minute, int second, TZID tzid, iCalendar iCal)
            : this(new DateTime(year, month, day, hour, minute, second, DateTimeKind.Local), tzid, iCal) {}
        public Date_Time(int year, int month, int day)
            : this(year, month, day, 0, 0, 0) { }
        public Date_Time(int year, int month, int day, TZID tzid, iCalendar iCal)
            : this(year, month, day, 0, 0, 0, tzid, iCal) { }
        
        #endregion

        #region Overrides

        public override ContentLine ContentLine
        {
            get { return base.ContentLine; }
            set
            {                
                if (value != null)
                {
                    CopyFrom((Date_Time)Parse(value.Value));

                    foreach (DictionaryEntry de in value.Parameters)
                    {
                        Parameter p = (Parameter)de.Value;
                        if (de.Key.Equals("TZID"))
                        {
                            TZID = new TZID();
                            TZID = (TZID)TZID.Parse(p.Values[0].ToString());
                        }
                    }

                    iCalendar = value.iCalendar;
                }

                base.ContentLine = value;
            }
        }

        public override void CopyFrom(object obj)
        {
            if (obj is Date_Time)
            {
                Date_Time dt = (Date_Time)obj;
                this.Value = dt.Value;
                this.HasDate = dt.HasDate;
                this.HasTime = dt.HasTime;
                this.TZID = dt.TZID;
                this.iCalendar = dt.iCalendar;                
            }
            base.CopyFrom(obj);
        }

        virtual public void MergeWith(Date_Time dt)
        {
            if (iCalendar == null)
                iCalendar = dt.iCalendar;
            if (TZID == null)
                TZID = dt.TZID;
        }

        public override bool TryParse(string value, ref object obj)
        {
            string[] values = value.Split('T');

            if (obj == null)
                obj = new Date_Time();
            Date_Time dt = (Date_Time)obj;

            Match match = Regex.Match(value, @"^((\d{4})(\d{2})(\d{2}))?T?((\d{2})(\d{2})(\d{2})(Z)?)?$", RegexOptions.IgnoreCase);
            if (!match.Success)
                return false;
            else
            {
                DateTime now = DateTime.Now;

                int year = now.Year;
                int month = now.Month;
                int date = now.Day;
                int hour = 0;
                int minute = 0;
                int second = 0;

                if (match.Groups[1].Success)
                {
                    dt.HasDate = true;
                    year = Convert.ToInt32(match.Groups[2].Value);
                    month = Convert.ToInt32(match.Groups[3].Value);
                    date = Convert.ToInt32(match.Groups[4].Value);
                }
                if (match.Groups[5].Success)
                {
                    dt.HasTime = true;
                    hour = Convert.ToInt32(match.Groups[6].Value);
                    minute = Convert.ToInt32(match.Groups[7].Value);
                    second = Convert.ToInt32(match.Groups[8].Value);
                }
                
                DateTimeKind dtk = match.Groups[9].Success ? DateTimeKind.Utc : DateTimeKind.Local;
                if (dtk == DateTimeKind.Utc && TZID != null)
                    throw new ApplicationException("TZID cannot be speicified on a UTC time");
                DateTime setDateTime = new DateTime(year, month, date, hour, minute, second, dtk);                               

                dt.Value = setDateTime;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj is Date_Time)
                return ((Date_Time)obj).UTC == UTC;
            else if (obj is DateTime)
                return ((DateTime)obj).ToUniversalTime() == UTC;
            return base.Equals(obj);
        }

        public override string ToString()
        {
            if (HasDate)
                return Value.ToString();
            else return Value.ToShortDateString();
        }

        #endregion

        #region Operators

        public static bool operator <(Date_Time left, Date_Time right)
        {
            if (left.HasTime || right.HasTime)
                return left.UTC < right.UTC;
            else return left.UTC.Date < right.UTC.Date;
        }

        public static bool operator >(Date_Time left, Date_Time right)
        {
            if (left.HasTime || right.HasTime)
                return left.UTC > right.UTC;
            else return left.UTC.Date > right.UTC.Date;
        }

        public static bool operator <=(Date_Time left, Date_Time right)
        {
            if (left.HasTime || right.HasTime)
                return left.UTC <= right.UTC;
            else return left.UTC.Date <= right.UTC.Date;
        }

        public static bool operator >=(Date_Time left, Date_Time right)
        {
            if (left.HasTime || right.HasTime)
                return left.UTC >= right.UTC;
            else return left.UTC.Date >= right.UTC.Date;
        }

        public static TimeSpan operator -(Date_Time left, Date_Time right)
        {
            return left.UTC - right.UTC;
        }

        public static Date_Time operator -(Date_Time left, TimeSpan right)
        {
            Date_Time copy = left.Copy();
            copy.Value -= right;
            return copy;
        }

        public static Date_Time operator +(Date_Time left, TimeSpan right)
        {
            Date_Time copy = left.Copy();
            copy.Value += right;
            return copy;
        }

        /*public static implicit operator DateTime(Date_Time left)
        {
            return left.UTC;
        }*/

        public static implicit operator Date_Time(DateTime left)
        {
            return new Date_Time(left.ToUniversalTime());
        }

        #endregion

        #region Public Methods

        public Date_Time Copy()
        {
            Date_Time dt = new Date_Time();
            dt.CopyFrom(this);
            return dt;
        }

        public Date_Time AddYears(int years)
        {
            Date_Time dt = Copy();
            dt.Value = Value.AddYears(years);
            return dt;
        }

        public Date_Time AddMonths(int months)
        {
            Date_Time dt = Copy();
            dt.Value = Value.AddMonths(months);
            return dt;
        }

        public Date_Time AddDays(int days)
        {
            Date_Time dt = Copy();
            dt.Value = Value.AddDays(days);
            return dt;
        }

        public Date_Time AddHours(int hours)
        {
            Date_Time dt = Copy();
            dt.Value = Value.AddHours(hours);
            return dt;
        }

        public Date_Time AddMinutes(int minutes)
        {
            Date_Time dt = Copy();
            dt.Value = Value.AddMinutes(minutes);
            return dt;
        }

        public Date_Time AddSeconds(int seconds)
        {
            Date_Time dt = Copy();
            dt.Value = Value.AddSeconds(seconds);
            return dt;
        }

        public Date_Time AddMilliseconds(int milliseconds)
        {
            Date_Time dt = Copy();
            dt.Value = Value.AddMilliseconds(milliseconds);
            return dt;
        }

        #endregion
    }
}
