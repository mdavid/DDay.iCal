using System;
using System.Runtime.Serialization;
using System.Globalization;
using DDay.Collections;

namespace DDay.iCal
{
    /// <summary>
    /// A class that represents an RFC 5545 VTIMEZONE component.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public partial class iCalTimeZone : CalendarComponent, ITimeZone
    {
        #region Static Public Methods

#if !SILVERLIGHT
        public static iCalTimeZone FromLocalTimeZone()
        {
            return FromSystemTimeZone(System.TimeZoneInfo.Local);
        }

        public static iCalTimeZone FromLocalTimeZone(DateTime earlistDateTimeToSupport, bool includeHistoricalData)
        {
            return FromSystemTimeZone(System.TimeZoneInfo.Local, earlistDateTimeToSupport, includeHistoricalData);
        }

        private static void PopulateiCalTimeZoneInfo(ITimeZoneInfo tzi, System.TimeZoneInfo.TransitionTime transition, int year)
        {
            var c = CultureInfo.CurrentCulture.Calendar;

            var recurrence = new RecurrencePattern();
            recurrence.Frequency = FrequencyType.Yearly;
            recurrence.ByMonth.Add(transition.Month);
            recurrence.ByHour.Add(transition.TimeOfDay.Hour);
            recurrence.ByMinute.Add(transition.TimeOfDay.Minute);

            if (transition.IsFixedDateRule)
            {
                recurrence.ByMonthDay.Add(transition.Day);
            }
            else
            {
                if (transition.Week != 5)
                {
                    recurrence.ByDay.Add(new WeekDay(transition.DayOfWeek, transition.Week));
                }
                else
                {
                    recurrence.ByDay.Add(new WeekDay(transition.DayOfWeek, -1));
                }
            }

            tzi.RecurrenceRules.Add(recurrence);
        }

        public static iCalTimeZone FromSystemTimeZone(System.TimeZoneInfo tzinfo)
        {
            // Support date/times for January 1st of the previous year by default.
            return FromSystemTimeZone(tzinfo, new DateTime(DateTime.Now.Year, 1, 1).AddYears(-1), false);
        }

        public static iCalTimeZone FromSystemTimeZone(System.TimeZoneInfo tzinfo, DateTime earlistDateTimeToSupport, bool includeHistoricalData)
        {
            var adjustmentRules = tzinfo.GetAdjustmentRules();
            var utcOffset = tzinfo.BaseUtcOffset;
            var dday_tz = new iCalTimeZone();
            dday_tz.TZID = tzinfo.Id;

            IDateTime earliest = new iCalDateTime(earlistDateTimeToSupport);
            foreach (var adjustmentRule in adjustmentRules)
            {
                // Only include historical data if asked to do so.  Otherwise,
                // use only the most recent adjustment rule available.
                if (!includeHistoricalData && adjustmentRule.DateEnd < earlistDateTimeToSupport)
                {
                    continue;
                }

                var delta = adjustmentRule.DaylightDelta;
                var dday_tzinfo_standard = new DDay.iCal.iCalTimeZoneInfo();
                dday_tzinfo_standard.Name = "STANDARD";
                dday_tzinfo_standard.TimeZoneName = tzinfo.StandardName;
                dday_tzinfo_standard.Start =
                    new iCalDateTime(
                        new DateTime(adjustmentRule.DateStart.Year, adjustmentRule.DaylightTransitionEnd.Month, adjustmentRule.DaylightTransitionEnd.Day,
                            adjustmentRule.DaylightTransitionEnd.TimeOfDay.Hour, adjustmentRule.DaylightTransitionEnd.TimeOfDay.Minute,
                            adjustmentRule.DaylightTransitionEnd.TimeOfDay.Second).AddDays(1));
                if (dday_tzinfo_standard.Start.LessThan(earliest))
                {
                    dday_tzinfo_standard.Start = dday_tzinfo_standard.Start.AddYears(earliest.Year - dday_tzinfo_standard.Start.Year);
                }
                dday_tzinfo_standard.OffsetFrom = new UTCOffset(utcOffset + delta);
                dday_tzinfo_standard.OffsetTo = new UTCOffset(utcOffset);
                PopulateiCalTimeZoneInfo(dday_tzinfo_standard, adjustmentRule.DaylightTransitionEnd, adjustmentRule.DateStart.Year);

                // Add the "standard" time rule to the time zone
                dday_tz.AddChild(dday_tzinfo_standard);

                if (tzinfo.SupportsDaylightSavingTime)
                {
                    var dday_tzinfo_daylight = new DDay.iCal.iCalTimeZoneInfo();
                    dday_tzinfo_daylight.Name = "DAYLIGHT";
                    dday_tzinfo_daylight.TimeZoneName = tzinfo.DaylightName;
                    dday_tzinfo_daylight.Start =
                        new iCalDateTime(new DateTime(adjustmentRule.DateStart.Year, adjustmentRule.DaylightTransitionStart.Month,
                            adjustmentRule.DaylightTransitionStart.Day, adjustmentRule.DaylightTransitionStart.TimeOfDay.Hour,
                            adjustmentRule.DaylightTransitionStart.TimeOfDay.Minute, adjustmentRule.DaylightTransitionStart.TimeOfDay.Second));
                    if (dday_tzinfo_daylight.Start.LessThan(earliest))
                    {
                        dday_tzinfo_daylight.Start = dday_tzinfo_daylight.Start.AddYears(earliest.Year - dday_tzinfo_daylight.Start.Year);
                    }
                    dday_tzinfo_daylight.OffsetFrom = new UTCOffset(utcOffset);
                    dday_tzinfo_daylight.OffsetTo = new UTCOffset(utcOffset + delta);
                    PopulateiCalTimeZoneInfo(dday_tzinfo_daylight, adjustmentRule.DaylightTransitionStart, adjustmentRule.DateStart.Year);

                    // Add the "daylight" time rule to the time zone
                    dday_tz.AddChild(dday_tzinfo_daylight);
                }
            }

            // If no time zone information was recorded, at least
            // add a STANDARD time zone element to indicate the
            // base time zone information.
            if (dday_tz.TimeZoneInfos.Count == 0)
            {
                var dday_tzinfo_standard = new DDay.iCal.iCalTimeZoneInfo();
                dday_tzinfo_standard.Name = "STANDARD";
                dday_tzinfo_standard.TimeZoneName = tzinfo.StandardName;
                dday_tzinfo_standard.Start = earliest;
                dday_tzinfo_standard.OffsetFrom = new UTCOffset(utcOffset);
                dday_tzinfo_standard.OffsetTo = new UTCOffset(utcOffset);

                // Add the "standard" time rule to the time zone
                dday_tz.AddChild(dday_tzinfo_standard);
            }

            return dday_tz;
        }
#endif

        #endregion

        #region Private Fields

        TimeZoneEvaluator m_Evaluator;
        ICalendarObjectList<ITimeZoneInfo> m_TimeZoneInfos;

        #endregion

        #region Constructors

        public iCalTimeZone()
        {
            Initialize();
        }

        private void Initialize()
        {
            this.Name = Components.TIMEZONE;

            m_Evaluator = new TimeZoneEvaluator(this);
            m_TimeZoneInfos = new CalendarObjectListProxy<ITimeZoneInfo>(Children);
            Children.ItemAdded += new EventHandler<ObjectEventArgs<ICalendarObject, int>>(Children_ItemAdded);
            Children.ItemRemoved += new EventHandler<ObjectEventArgs<ICalendarObject, int>>(Children_ItemRemoved);
            SetService(m_Evaluator);
        }

        #endregion

        #region Event Handlers

        void Children_ItemRemoved(object sender, ObjectEventArgs<ICalendarObject, int> e)
        {
            m_Evaluator.Clear();
        }

        void Children_ItemAdded(object sender, ObjectEventArgs<ICalendarObject, int> e)
        {
            m_Evaluator.Clear();
        }

        #endregion

        #region Overrides

        protected override void OnDeserializing(StreamingContext context)
        {
            base.OnDeserializing(context);

            Initialize();
        }

        #endregion

        #region ITimeZone Members

        public virtual string ID
        {
            get { return Properties.Get<string>("TZID"); }
            set { Properties.Set("TZID", value); }
        }

        public virtual string TZID
        {
            get { return ID; }
            set { ID = value; }
        }

        public virtual IDateTime LastModified
        {
            get { return Properties.Get<IDateTime>("LAST-MODIFIED"); }
            set { Properties.Set("LAST-MODIFIED", value); }
        }

        public virtual Uri Url
        {
            get { return Properties.Get<Uri>("TZURL"); }
            set { Properties.Set("TZURL", value); }
        }

        public virtual ICalendarObjectList<ITimeZoneInfo> TimeZoneInfos
        {
            get { return m_TimeZoneInfos; }
            set { m_TimeZoneInfos = value; }
        }

        #endregion
    }
}