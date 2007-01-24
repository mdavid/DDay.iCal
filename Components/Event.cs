using System;
using System.Collections;
using System.Data;
using System.Configuration;
using DDay.iCal.Objects;
using DDay.iCal.DataTypes;

namespace DDay.iCal.Components
{
    /// <summary>
    /// <todo></todo>
    /// </summary>
    public class Event : ComponentBase
    {
        public URI Attach;
        public Cal_Address Attendee;
        public string Categories;
        public string Class;
        public string Comment;
        public string Contact;
        public Date_Time Created;
        public string Description;
        public Date_Time DTStamp;
        public Date_Time DTStart;
        public Date_Time DTEnd;
        public Duration Duration;
        public RDate[] ExDate;
        public Recur[] ExRule;
        public Geo Geo;
        public Date_Time LastMod;
        public string Location;
        public Cal_Address Organizer;
        public int Priority;
        public RDate[] RDate;
        public Date_Time RecurID;
        public string Related;
        public string Resources;
        public Recur[] RRule;
        public string RStatus;
        public int Seq;
        public Status Status;
        public string Summary;
        public Transparency Transp;
        public string UID;
        public URI URL;

        #region Private Fields

        private ArrayList m_Periods;   
     
        #endregion

        #region Public Properties

        /// <summary>
        /// A collection of <see cref="Period"/> objects that represent
        /// each start and end time for which this event can occur.
        /// This collection is usually populated from the <see cref="Evaluate"/>
        /// method.
        /// </summary>
        public ArrayList Periods
        {
            get { return m_Periods; }
            set { m_Periods = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs an Event object, with an <see cref="iCalObject"/>
        /// (usually an iCalendar object) as its parent.
        /// </summary>
        /// <param name="parent">An <see cref="iCalObject"/>, usually an iCalendar object.</param>
        public Event(iCalObject parent) : base(parent)
        {
            this.Name = "VEVENT";
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Evaluates this event to determine the dates and times for which the event occurs.
        /// This method only evaluates events which occur between <paramref name="FromDate"/>
        /// and <paramref name="ToDate"/>; therefore, if you require a list of events which
        /// occur outside of this range, you must specify a <paramref name="FromDate"/> and
        /// <paramref name="ToDate"/> which encapsulate the date(s) of interest.
        /// <note type="caution">
        ///     For events with very complex recurrence rules, this method may be a bottleneck
        ///     during processing time, especially when this method in called for a large number
        ///     of events, in sequence, or for a very large time span.
        /// </summary>
        /// <param name="FromDate">The beginning date of the range to evaluate.</param>
        /// <param name="ToDate">The end date of the range to evaluate.</param>
        /// <returns></returns>
        public ArrayList Evaluate(DateTime FromDate, DateTime ToDate)
        {            
            Periods = new ArrayList();

            // Add the event itself, before recurrence rules are evaluated
            Periods.Add(new Period(DTStart.Value, Duration.Value));

            EvaluateRRule(FromDate, ToDate);
            EvaluateRDate(FromDate, ToDate);
            EvaluateExRule(FromDate, ToDate);
            EvaluateExDate(FromDate, ToDate);

            return Periods;
        }

        /// <summary>
        /// Use this method to determine if an event occurs on a given date.
        /// <note type="caution">
        ///     This event should be called only after the <see cref="Evaluate"/>
        ///     method has calculated the dates for which this event occurs.
        /// </note>
        /// </summary>
        /// <param name="DateTime">The date to test.</param>
        /// <returns>True if the event occurs on the <paramref name="DateTime"/> provided, False otherwise.</returns>
        public bool OccursOn(DateTime DateTime)
        {
            foreach (Period p in Periods)
                if (p.StartTime.Value.Date <= DateTime.Date &&
                    p.EndTime.Value.Date > DateTime.Date) // Changed ">=" to ">"
                    // NOTE: fixed bug as follows:
                    // DTSTART;VALUE=DATE:20060704
                    // DTEND;VALUE=DATE:20060705
                    // Event.OccursOn(new DateTime(2006, 7, 5)); // Evals to true; should be false
                    return true;
            return false;
        }

        /// <summary>
        /// Use this method to determine if an event begins at a given date and time.
        /// </summary>
        /// <param name="DateTime">The date and time to test.</param>
        /// <returns>True if the event begins at the given date and time</returns>
        public bool OccursAt(DateTime DateTime)
        {
            foreach (Period p in Periods)
                if (p.StartTime.Value == DateTime)
                    return true;
            return false;
        }

        #endregion

        #region Overrides
        
        /// <summary>
        /// Automatically derives property values based on others it
        /// contains, to provide a more "complete" object.
        /// </summary>
        /// <param name="e"></param>
        public override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Automatically determine Duration from DTEnd, or DTEnd from Duration
            if (DTStart != null)
            {
                if (DTEnd != null && Duration == null)
                    Duration = new Duration(DTEnd.Value - DTStart.Value);
                else if (DTEnd == null && Duration != null)
                    DTEnd = new Date_Time(DTStart.Value + Duration.Value);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Evaulates the RRule component, and adds each specified DateTime
        /// to the <see cref="Periods"/> collection.
        /// </summary>
        /// <param name="FromDate">The beginning date of the range to evaluate.</param>
        /// <param name="ToDate">The end date of the range to evaluate.</param>
        protected void EvaluateRRule(DateTime FromDate, DateTime ToDate)
        {
            // Handle RRULEs
            if (RRule != null)
            {
                foreach (Recur rrule in RRule)
                {
                    ArrayList DateTimes = rrule.Evaluate(DTStart, new Date_Time(FromDate), new Date_Time(ToDate));
                    foreach (DateTime dt in DateTimes)
                    {
                        Period p = new Period(dt, Duration.Value);
                        if (!Periods.Contains(p))
                            Periods.Add(p);
                    }
                }
            }
        }

        /// <summary>
        /// Evalates the RDate component, and adds each specified DateTime or
        /// Period to the <see cref="Periods"/> collection.
        /// </summary>
        /// <param name="FromDate">The beginning date of the range to evaluate.</param>
        /// <param name="ToDate">The end date of the range to evaluate.</param>
        protected void EvaluateRDate(DateTime FromDate, DateTime ToDate)
        {
            // Handle RDATEs
            if (RDate != null)
            {
                foreach (RDate rdate in RDate)
                {
                    ArrayList Items = rdate.Evaluate(DTStart, new Date_Time(FromDate), new Date_Time(ToDate));
                    foreach (object obj in Items)
                    {
                        Period p = null;
                        if (obj is Period)
                            p = (Period)obj;
                        else if (obj is Date_Time)
                            p = new Period(((Date_Time)obj).Value, Duration.Value);

                        if (p != null && !Periods.Contains(p))
                            Periods.Add(p);
                    }
                }
            }
        }

        /// <summary>
        /// Evaulates the ExRule component, and excludes each specified DateTime
        /// from the <see cref="Periods"/> collection.
        /// </summary>
        /// <param name="FromDate">The beginning date of the range to evaluate.</param>
        /// <param name="ToDate">The end date of the range to evaluate.</param>
        protected void EvaluateExRule(DateTime FromDate, DateTime ToDate)
        {
            // Handle EXRULEs
            if (ExRule != null)
            {
                foreach (Recur exrule in ExRule)
                {
                    ArrayList DateTimes = exrule.Evaluate(DTStart, new Date_Time(FromDate), new Date_Time(ToDate));
                    foreach (DateTime dt in DateTimes)
                    {
                        Period p = new Period(dt, Duration.Value);
                        if (Periods.Contains(p))
                            Periods.Remove(p);
                    }
                }
            }
        }

        /// <summary>
        /// Evalates the ExDate component, and excludes each specified DateTime or
        /// Period from the <see cref="Periods"/> collection.
        /// </summary>
        /// <param name="FromDate">The beginning date of the range to evaluate.</param>
        /// <param name="ToDate">The end date of the range to evaluate.</param>
        protected void EvaluateExDate(DateTime FromDate, DateTime ToDate)
        {
            // Handle EXDATEs
            if (ExDate != null)
            {
                foreach (RDate exdate in ExDate)
                {
                    ArrayList Items = exdate.Evaluate(DTStart, new Date_Time(FromDate), new Date_Time(ToDate));
                    foreach (object obj in Items)
                    {
                        Period p = null;
                        if (obj is Period)
                            p = (Period)obj;
                        else if (obj is Date_Time)
                            p = new Period(((Date_Time)obj).Value, Duration.Value);

                        // If no time was provided for the ExDate, then it excludes the entire day
                        if (!p.StartTime.HasTime || !p.EndTime.HasTime)
                            p.MatchesDateOnly = true;

                        if (p != null)
                        {
                            // If p.MatchesDateOnly, remove all occurrences of this event
                            // on that specific date
                            while (Periods.Contains(p))
                                Periods.Remove(p);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
