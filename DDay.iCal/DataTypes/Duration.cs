using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace DDay.iCal.DataTypes
{
    public class Duration : iCalDataType
    {
        private TimeSpan m_Value;

        public TimeSpan Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

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

        public override void CopyFrom(object obj)
        {
            if (obj is Duration)
            {
                Duration d = (Duration)obj;
                Value = d.Value;
            }
        }

        public override bool TryParse(string value, ref object obj)
        {
            Match match = Regex.Match(value, @"^(\+|-)?P(((\d+)W)|(((\d+)D)?T((\d+)H)?((\d+)M)?((\d+)S)?))$");
            int days = 0;
            int hours = 0;
            int minutes = 0;
            int seconds = 0;

            if (match.Success)
            {
                if (match.Groups[3].Success)
                    days = Convert.ToInt32(match.Groups[4].Value) * 7;
                else if (match.Groups[5].Success)
                {
                    if (match.Groups[7].Success) days = Convert.ToInt32(match.Groups[7].Value);
                    if (match.Groups[9].Success) hours = Convert.ToInt32(match.Groups[9].Value);
                    if (match.Groups[11].Success) minutes = Convert.ToInt32(match.Groups[11].Value);
                    if (match.Groups[13].Success) seconds = Convert.ToInt32(match.Groups[13].Value);
                }

                ((Duration)obj).Value = new TimeSpan(days, hours, minutes, seconds);
                return true;
            }
            return false;
        }

        static public implicit operator TimeSpan(Duration value)
        {
            return value.Value;
        }
    }
}
