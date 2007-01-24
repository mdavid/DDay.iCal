using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using DDay.iCal.Objects;

namespace DDay.iCal.DataTypes
{
    [DebuggerDisplay("{HasTime ? Value.ToString() : Value.ToShortDateString()}")]
    public class Date_Time : iCalDataType
    {        
        private DateTime m_Value;
        private bool m_HasDate = false;
        private bool m_HasTime = false;

        public DateTime Local
        {
            get { return m_Value.ToLocalTime(); }
        }
        
        public DateTime UTC
        {
            get { return m_Value.ToUniversalTime(); }
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

        public Date_Time() { }
        public Date_Time(string value) : this()
        {
            CopyFrom((Date_Time)Parse(value));
        }
        public Date_Time(Date_Time value)
            : this(value.Value) {}
        public Date_Time(DateTime value)
            : this()
        {
            this.Value = value;
            this.HasDate = true;
            this.HasTime = (value.Second == 0 && value.Minute == 0 && value.Hour == 0) ? false : true;
        }

        public override ContentLine ContentLine
        {
            get { return base.ContentLine; }
            set
            {
                base.ContentLine = value;
                if (ContentLine != null)
                    CopyFrom((Date_Time)Parse(ContentLine.Value));                
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
            }
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

                // FIXME: Handle TimeZone differences here...
                // check current TimeZone objects for TimeZone settings
                
                DateTimeKind dtk = match.Groups[9].Success ? DateTimeKind.Utc : DateTimeKind.Local;

                dt.Value = new DateTime(year, month, date, hour, minute, second, dtk);
            }
            return true;
        }

        public override string ToString()
        {
            if (HasDate)
                return Value.ToString();
            else return Value.ToShortDateString();            
        }

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
    }
}
