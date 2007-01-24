using System;
using System.Diagnostics;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace DDay.iCal.DataTypes
{
    /// <summary>
    /// The iCalendar equivalent of the .NET <see cref="TimeSpan"/> class.
    /// <remarks>
    /// This class handles parsing an RFC 2445 Duration.
    /// </remarks>
    /// </summary>
    [DebuggerDisplay("{Value}")]
    public class Duration : iCalDataType
    {
        #region Private Fields

        private TimeSpan m_Value;

        #endregion

        #region Public Properties

        public TimeSpan Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        #endregion

        #region Constructors

        public Duration() { }
        public Duration(TimeSpan ts)
            : this()
        {
            this.Value = ts;
        }
        public Duration(string value)
            : this()
        {
            CopyFrom((Duration)Parse(value));
        }

        #endregion

        #region Overrides

        public override void CopyFrom(object obj)
        {
            if (obj is Duration)
            {
                Duration d = (Duration)obj;
                Value = d.Value;
            }
            base.CopyFrom(obj);
        }

        public override bool TryParse(string value, ref object obj)
        {
            Match match = Regex.Match(value, @"^(?<sign>\+|-)?P(((?<week>\d+)W)|(?<main>((?<day>\d+)D)?(?<time>T((?<hour>\d+)H)?((?<minute>\d+)M)?((?<second>\d+)S)?)?))$");
            int days = 0;
            int hours = 0;
            int minutes = 0;
            int seconds = 0;

            if (match.Success)
            {
                int mult = 1;
                if (match.Groups["sign"].Success && match.Groups["sign"].Value == "-")
                    mult = -1;

                if (match.Groups["week"].Success)
                    days = Convert.ToInt32(match.Groups["week"].Value) * 7;
                else if (match.Groups["main"].Success)
                {
                    if (match.Groups["day"].Success) days = Convert.ToInt32(match.Groups["day"].Value);
                    if (match.Groups["time"].Success)
                    {
                        if (match.Groups["hour"].Success) hours = Convert.ToInt32(match.Groups["hour"].Value);
                        if (match.Groups["minute"].Success) minutes = Convert.ToInt32(match.Groups["minute"].Value);
                        if (match.Groups["second"].Success) seconds = Convert.ToInt32(match.Groups["second"].Value);
                    }
                }

                ((Duration)obj).Value = new TimeSpan(days * mult, hours * mult, minutes * mult, seconds * mult);
                return true;
            }
            return false;
        }

        public override string ToString()
        {            
            TimeSpan ts = new TimeSpan(0);
            string value = string.Empty;
            if (ts > Value)
                value = "-";

            value += "P";

            if (Value.Days > 7 &&
                Value.Days % 7 == 0 &&
                Value.Hours == 0 &&
                Value.Minutes == 0 &&
                Value.Seconds == 0)                
                value += Math.Round((double)Value.Days / 7) + "W";
            else
            {
                if (Value.Days != 0)
                    value += Value.Days + "D";
                if (Value.Hours != 0 ||
                    Value.Minutes != 0 ||
                    Value.Seconds != 0)
                {
                    value += "T";
                    if (Value.Hours != 0)
                        value += Value.Hours + "H";
                    if (Value.Minutes != 0)
                        value += Value.Minutes + "M";
                    if (Value.Seconds != 0)
                        value += Value.Seconds + "S";
                }
            }

            return value;
        }

        #endregion

        #region Operators

        static public implicit operator TimeSpan(Duration value)
        {
            return value.Value;
        }

        #endregion
    }
}
