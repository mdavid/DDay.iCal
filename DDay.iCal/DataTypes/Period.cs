using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.DataTypes
{
    [DebuggerDisplay("Period ( {StartTime} - {EndTime} )")]
    public class Period : iCalDataType
    {
        #region Public Fields

        public Date_Time StartTime = new Date_Time();
        public Date_Time EndTime = new Date_Time();
        public Duration Duration = new Duration();
        
        /// <summary>
        /// When true, comparisons between this and other <see cref="Period"/>
        /// objects are matched against the date only, and
        /// not the date-time combination.
        /// </summary>
        public bool MatchesDateOnly = false;

        #endregion

        #region Constructors

        public Period() { }
        public Period(Date_Time start, Date_Time end)
            : this()
        {
            StartTime = start;
            EndTime = end;
            Duration = new Duration(end.Value - start.Value);
        }
        public Period(Date_Time start, TimeSpan duration)
            : this()
        {
            StartTime = start;
            Duration = new Duration(duration);
            EndTime = new Date_Time(start.Value + duration);
        }
        public Period(string value)
            : this()
        {
            CopyFrom((Period)Parse(value));
        }

        #endregion

        #region Overrides

        public override bool Equals(object obj)
        {
            if (obj is Period)
            {
                Period p = (Period)obj;
                if (MatchesDateOnly || p.MatchesDateOnly)
                {
                    return
                        StartTime.Value.Date == p.StartTime.Value.Date &&
                        EndTime.Value.Date == p.EndTime.Value.Date;
                }
                else
                {
                    return
                        StartTime.Value == p.StartTime.Value &&
                        EndTime.Value == p.EndTime.Value;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {            
            return (StartTime.GetHashCode() & 0xFF00) + (EndTime.GetHashCode() & 0xFF);
        }

        public override void CopyFrom(object obj)
        {
            if (obj is Period)
            {
                Period p = (Period)obj;
                StartTime = p.StartTime;
                EndTime = p.EndTime;
                Duration = p.Duration;
            }
        }

        public override object Parse(string value)
        {
            object p = new Period();
            if (!TryParse(value, ref p))
                throw new ArgumentException("Period.Parse cannot parse the value '" + value + "' because it is not formatted correctly.");
            return p;
        }

        public override bool TryParse(string value, ref object obj)
        {
            Period p = (Period)obj;

            string[] values = value.Split('/');
            if (values.Length != 2)
                return false;

            object st = p.StartTime;
            object et = p.EndTime;
            object d = p.Duration;

            bool retVal = p.StartTime.TryParse(values[0], ref st) &&
                (
                    p.EndTime.TryParse(values[1], ref et) ||
                    p.Duration.TryParse(values[1], ref d)
                );

            // Fill in missing values
            if (!p.EndTime.HasDate)
                p.EndTime = new Date_Time(p.StartTime.Value + p.Duration.Value);
            else if (p.Duration.Value.Ticks == 0)
                p.Duration = new Duration(p.EndTime.Value - p.StartTime.Value);

            return retVal;
        }        

        #endregion
    }
}
