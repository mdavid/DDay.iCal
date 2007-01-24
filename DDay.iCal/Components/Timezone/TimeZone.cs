using System;
using System.Data;
using System.Collections;
using System.Configuration;
using DDay.iCal.Objects;
using DDay.iCal.DataTypes;

namespace DDay.iCal.Components
{
    /// <summary>
    /// A class that represents an RFC 2445 VTIMEZONE component.
    /// </summary>
    public partial class TimeZone : ComponentBase
    {
        #region Public Fields

        public TZID TZID;
        public Date_Time LastModified;
        public URI TZUrl;
        public ArrayList TimeZoneInfos = new ArrayList();

        #endregion
        
        #region Constructors

        public TimeZone(iCalObject parent) : base(parent)
        {
            this.Name = "VTIMEZONE";
        }

        #endregion

        #region Overrides

        public override void AddChild(iCalObject child)
        {
            if (child is TimeZoneInfo)
                TimeZoneInfos.Add(child);

            base.AddChild(child);
        }

        #endregion

        #region Public Methods

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
                    curr.Start = curr.Start.AddHours(curr.TZOffsetTo.Hours * mult);
                    curr.Start = curr.Start.AddMinutes(curr.TZOffsetTo.Minutes * mult);
                    curr.Start = curr.Start.AddSeconds(curr.TZOffsetTo.Seconds * mult);
                }
                                
                // Determine the UTC occurrences of the Time Zone changes                
                if (curr.EvalStart == null ||
                    curr.EvalEnd == null ||
                    dtUTC < curr.EvalStart.Value ||
                    dtUTC > curr.EvalEnd.Value)
                    curr.Evaluate(Start, End);

                foreach (Period p in curr.Periods)
                {                    
                    TimeSpan currentSpan = dtUTC - p.StartTime;
                    if (currentSpan.Ticks >= 0 &&
                        currentSpan.Ticks < mostRecent.Ticks)
                    {
                        mostRecent = currentSpan;
                        tzi = curr;
                    }
                }
            }

            return tzi;
        }

        #endregion
    }
}
