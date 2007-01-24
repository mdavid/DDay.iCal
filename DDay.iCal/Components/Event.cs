using System;
using System.Collections;
using System.Data;
using System.Configuration;
using DDay.iCal.Objects;
using DDay.iCal.DataTypes;

namespace DDay.iCal.Components
{
    /// <summary>
    /// A class that represents an RFC 2445 VEVENT component.
    /// </summary>    
    /// <note>
    ///     TODO: Add support for the following properties:
    ///     <list type="bullet">
    ///         <item>Add support for the Organizer and Attendee properties</item>    
    ///         <item>Create a TextCollection DataType for 'text' items separated by commas</item>
    ///     </list>
    /// </note>
    public class Event : RecurringComponent
    {
        #region Public Fields

        public URI[] Attach;
        public Cal_Address[] Attendee;
        public Text[] Categories; // FIXME: create TextCollection type instead of Text (will still be an array of TextCollection objects)
        public string Class;
        public Text[] Comment;
        public Text[] Contact;
        public Date_Time Created;
        public Text Description;
        public Date_Time DTStamp;
        public Date_Time DTEnd;
        public Duration Duration;
        public Geo Geo;
        public Date_Time LastMod;
        public Text Location;
        public Cal_Address Organizer;
        public int Priority;
        public Text[] Related;
        public RequestStatus[] RequestStatus;
        public Text[] Resources; // FIXME: create TextCollection type instead of Text
        public int Sequence;
        public Status Status;
        public Text Summary;
        public Transparency Transp;
        public string UID;
        public URI URL;

        #endregion

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

        /// <summary>
        /// An alias to the DTEnd field (i.e. end date/time).
        /// </summary>
        public Date_Time End
        {
            get { return DTEnd; }
            set { DTEnd = value; }
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
            Periods = new ArrayList();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Use this method to determine if an event occurs on a given date.
        /// <note type="caution">
        ///     This event should be called only after the <see cref="Evaluate"/>
        ///     method has calculated the dates for which this event occurs.
        /// </note>
        /// </summary>
        /// <param name="DateTime">The date to test.</param>
        /// <returns>True if the event occurs on the <paramref name="DateTime"/> provided, False otherwise.</returns>
        public bool OccursOn(Date_Time DateTime)
        {            
            foreach (Period p in Periods)
                // NOTE: removed UTC from date checks, since a date is a date.
                if (p.StartTime.Date == DateTime.Date ||    // It's the start date
                    (p.StartTime.Date <= DateTime.Date &&   // It's after the start date AND
                    (p.EndTime.HasTime && p.EndTime.Date >= DateTime.Date || // an end time was specified, and it's before then
                    (!p.EndTime.HasTime && p.EndTime.Date > DateTime.Date)))) // an end time was not specified, and it's before the end date
                    // NOTE: fixed bug as follows:
                    // DTSTART;VALUE=DATE:20060704
                    // DTEND;VALUE=DATE:20060705
                    // Event.OccursOn(new Date_Time(2006, 7, 5)); // Evals to true; should be false
                    return true;
            return false;
        }

        /// <summary>
        /// Use this method to determine if an event begins at a given date and time.
        /// </summary>
        /// <param name="DateTime">The date and time to test.</param>
        /// <returns>True if the event begins at the given date and time</returns>
        public bool OccursAt(Date_Time DateTime)
        {            
            foreach (Period p in Periods)
                if (p.StartTime.Equals(DateTime))
                    return true;
            return false;
        }

        #endregion

        #region Overrides

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
        public override ArrayList Evaluate(Date_Time FromDate, Date_Time ToDate)
        {            
            // Add the event itself, before recurrence rules are evaluated
            Periods.Add(new Period(DTStart, (TimeSpan)Duration));

            // Evaluate recurrences normally
            base.Evaluate(FromDate, ToDate);

            // Convert each calculated Date_Time into a Period.
            foreach(Date_Time dt in DateTimes)
            {
                Period p = new Period(dt, Duration);
                if (!Periods.Contains(p))
                    Periods.Add(p);
            }

            return Periods;
        }
                
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
                    DTEnd = DTStart + Duration;
            }
        }

        #endregion

        #region Protected Methods

        protected override void EvaluateRDate(Date_Time FromDate, Date_Time ToDate)
        {
            // Handle RDATEs
            if (RDate != null)
            {
                foreach (RDate rdate in RDate)
                {
                    ArrayList Items = rdate.Evaluate(DTStart, FromDate, ToDate);
                    foreach (object obj in Items)
                    {
                        Period p = null;
                        if (obj is Period)
                            p = (Period)obj;
                        else if (obj is Date_Time)
                            p = new Period((Date_Time)obj, (TimeSpan)Duration);

                        if (p != null && !Periods.Contains(p))
                            Periods.Add(p);
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
        protected override void EvaluateExDate(Date_Time FromDate, Date_Time ToDate)
        {
            // Handle EXDATEs
            if (ExDate != null)
            {
                foreach (RDate exdate in ExDate)
                {
                    ArrayList Items = exdate.Evaluate(DTStart, FromDate, ToDate);
                    foreach (object obj in Items)
                    {
                        Period p = null;
                        if (obj is Period)
                            p = (Period)obj;
                        else if (obj is Date_Time)
                            p = new Period((Date_Time)obj, (TimeSpan)Duration);

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
