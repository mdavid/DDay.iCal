using System;
using System.Data;
using System.Collections;
using System.Configuration;
using DDay.iCal.Objects;
using DDay.iCal.DataTypes;

namespace DDay.iCal.Components
{
    public partial class TimeZone : ComponentBase
    {
        public TZID TZID;
        public Date_Time LastModified;
        public URI TZUrl;
        public ArrayList TimeZoneInfos = new ArrayList();
        public ArrayList StandardTimes = new ArrayList();
        public ArrayList DaylightTimes = new ArrayList();        
        
        #region Constructors

        public TimeZone(iCalObject parent) : base(parent)
        {
            this.Name = "VTIMEZONE";
        }

        #endregion

        /// <summary>
        /// Adds an <see cref="iCalObject"/>-based component to the
        /// appropriate collection.  Currently, the TimeZone component
        /// supports the following components:
        ///     <list>
        ///         <item>Standard</item>
        ///         <item>Daylight</item>
        ///     </list>
        /// </summary>
        /// <param name="child"></param>
        public override void AddChild(iCalObject child)
        {
            Type type = child.GetType();
            switch (type.Name)
            {
                case "Standard": TimeZoneInfos.Add(child); StandardTimes.Add(child); break;
                case "Daylight": TimeZoneInfos.Add(child); DaylightTimes.Add(child); break;
                default: break;
            }
        }

        /// <summary>
        /// Retrieves the TimeZoneInfo object that contains information
        /// about the TimeZone, with the name of the current timezone,
        /// offset from UTC, etc.
        /// </summary>
        /// <param name="dt">The Date_Time object for which to retrieve the TimeZoneInfo</param>
        /// <returns>A TimeZoneInfo object for the specified Date_Time</returns>
        public TimeZoneInfo GetTimeZoneInfo(Date_Time dt)
        {
            TimeZoneInfo tzi = null;

            TimeSpan mostRecent = TimeSpan.MaxValue;
            foreach (TimeZoneInfo curr in TimeZoneInfos)
            {
                DateTime Start = new DateTime(dt.Year - 1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime End = new DateTime(dt.Year + 1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                
                DateTime dtUTC = dt.Value;
                dtUTC = DateTime.SpecifyKind(dtUTC, DateTimeKind.Utc);

                if (curr.TZOffsetTo != null)
                {
                    int mult = curr.TZOffsetTo.Positive ? -1 : 1;
                    dtUTC = dtUTC.AddHours(curr.TZOffsetTo.Hours * mult);
                    dtUTC = dtUTC.AddMinutes(curr.TZOffsetTo.Minutes * mult);
                    dtUTC = dtUTC.AddSeconds(curr.TZOffsetTo.Seconds * mult);
                    curr.DTStart = curr.DTStart.AddHours(curr.TZOffsetTo.Hours * mult);
                    curr.DTStart = curr.DTStart.AddMinutes(curr.TZOffsetTo.Minutes * mult);
                    curr.DTStart = curr.DTStart.AddSeconds(curr.TZOffsetTo.Seconds * mult);
                }
                                
                // Determine the UTC occurrences of the Time Zone changes
                // FIXME: are these start/end dates correct when TimeZone or UTC time is taken into account?
                if (curr.EvalStart == null ||
                    curr.EvalEnd == null ||
                    dtUTC < curr.EvalStart.Value ||
                    dtUTC > curr.EvalEnd.Value)
                    curr.Evaluate(new Date_Time(Start), new Date_Time(End));

                foreach (DateTime currDt in curr.DateTimes)
                {                    
                    TimeSpan currentSpan = dtUTC - currDt;
                    if (currentSpan.Ticks > 0 &&
                        currentSpan.Ticks < mostRecent.Ticks)
                    {
                        mostRecent = currentSpan;
                        tzi = curr;
                    }
                }
            }

            return tzi;
        }
    }
}
