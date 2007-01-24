using System;
using System.Collections;
using System.Collections.Generic;
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
    ///         <item>Add support for the Class property</item>
    ///         <item>Add support for the Geo property</item>
    ///         <item>Add support for the Priority property</item>
    ///         <item>Add support for the Related property</item>
    ///         <item>Create a TextCollection DataType for 'text' items separated by commas</item>
    ///     </list>
    /// </note>
    public class Event : RecurringComponent
    {
        #region Public Fields

        public Binary[] Attach;
        public Cal_Address[] Attendee;
        public TextCollection[] Categories;
        public Text Class;
        public Text[] Comment;
        public Text[] Contact;
        [DefaultValueType("DATE-TIME")]
        public Date_Time Created;
        public Text Description;
        [DefaultValueType("DATE-TIME")]
        public Date_Time DTStamp;
        [DefaultValueType("DATE-TIME")]
        public Date_Time DTEnd;
        public Duration Duration;
        public Geo Geo;
        [DefaultValueType("DATE-TIME")]
        public Date_Time LastModified;
        public Text Location;
        public Cal_Address Organizer;
        public Integer Priority;
        public Text[] Related_To;
        public RequestStatus[] RequestStatus;
        public TextCollection[] Resources;
        public Integer Sequence;
        [DefaultValue(EventStatus.TENTATIVE)]
        public EventStatus Status;
        public Text Summary;
        [DefaultValue(Transparency.OPAQUE)]
        public Transparency Transp;        
        public URI URL;

        #endregion
         
        #region Public Properties

        /// <summary>
        /// An alias to the DTEnd field (i.e. end date/time).
        /// </summary>
        public Date_Time End
        {
            get { return DTEnd; }
            set { DTEnd = value; }
        }

        /// <summary>
        /// Returns true of the event is an all-day event.
        /// </summary>
        public bool IsAllDay
        {
            get { return !Start.HasTime; }
        }

        #endregion

        #region Static Public Methods

        static public Event Create(iCalendar iCal)
        {
            Event evt = new Event(iCal);
            evt.UID = UniqueComponent.NewUID();

            return evt;
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

        /// <summary>
        /// Determines whether or not the <see cref="Event"/> is actively displayed
        /// as an upcoming or occurred event.
        /// </summary>
        /// <returns>True if the event has not been cancelled, False otherwise.</returns>
        public bool IsActive()
        {
            return (Status != EventStatus.CANCELLED);
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
        public override List<Period> Evaluate(Date_Time FromDate, Date_Time ToDate)
        {            
            // Add the event itself, before recurrence rules are evaluated
            // NOTE: this fixes a bug where (if evaluated multiple times)
            // a period can be added to the Periods collection multiple times.
            Period period = new Period(DTStart, Duration);
            if (!Periods.Contains(period))
                Periods.Add(period);

            // Evaluate recurrences normally
            base.Evaluate(FromDate, ToDate);
                        
            // Ensure each period has a duration
            foreach(Period p in Periods)
            {
                if (p.EndTime == null)
                {
                    p.Duration = Duration;
                    p.EndTime = p.StartTime + Duration;
                }
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
    }
}
