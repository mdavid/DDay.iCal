using System;
using System.Collections;
using System.Data;
using System.Configuration;
using DDay.iCal.Objects;
using DDay.iCal.DataTypes;

namespace DDay.iCal.Components
{
    /// <summary>
    /// A class that represents an RFC 2445 VTODO component.
    /// </summary>    
    public class Todo : RecurringComponent
    {
        #region Public Fields

        public URI[] Attach;
        public Cal_Address[] Attendee;
        public TextCollection[] Categories;
        public string Class;
        public Text[] Comment;
        public Date_Time Completed;
        public Text[] Contact;
        public Date_Time Created;
        public Text Description;
        public Date_Time DTStamp;
        public Date_Time Due;
        public Duration Duration;
        public Geo Geo;
        public Date_Time LastMod;
        public Text Location;
        public Cal_Address Organizer;
        public Integer PercentComplete;
        public Integer Priority;
        public Text[] RelatedTo; 
        public RequestStatus[] RequestStatus;
        public TextCollection[] Resources;
        public Integer Sequence;
        public TodoStatus Status;
        public Text Summary;
        public string UID;
        public Uri Url;

        #endregion

        #region Constructors

        public Todo(iCalObject parent)
            : base(parent)
        {
            this.Name = "VTODO";
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Use this method to determine if a todo item has been completed.
        /// This takes into account recurrence items and the previous date
        /// of completion, if any.
        /// </summary>
        /// <param name="DateTime">The date and time to test.</param>
        /// <returns>True if the todo item has been completed</returns>
        public bool IsCompleted(Date_Time currDt)
        {
            if (Status == TodoStatus.COMPLETED)
            {
                if (Completed == null)
                    return true;

                foreach (Date_Time dt in DateTimes)
                {                    
                    if (dt > Completed && // The item has recurred after it was completed
                        currDt >= dt)     // and the current date is after or on the recurrence date.
                        return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns 'True' if the todo item is Active as of <paramref name="currDt"/>.
        /// An item is Active if it requires action of some sort.
        /// </summary>
        /// <param name="currDt">The date and time to test.</param>
        /// <returns>True if the item is Active as of <paramref name="currDt"/>, False otherwise.</returns>
        public bool IsActive(Date_Time currDt)
        {
            if (DTStart == null)
                return !IsCompleted(currDt) && !IsCancelled();
            else if (currDt >= DTStart)
                return !IsCompleted(currDt) && !IsCancelled();
            else return false;
        }

        /// <summary>
        /// Returns True if the todo item was cancelled.
        /// </summary>
        /// <returns>True if the todo was cancelled, False otherwise.</returns>
        public bool IsCancelled()
        {
            return Status == TodoStatus.CANCELLED;
        }

        #endregion

        #region Overrides

        public override ArrayList Evaluate(Date_Time FromDate, Date_Time ToDate)
        {
            // Add the event itself, before recurrence rules are evaluated
            if (DTStart != null)
                DateTimes.Add(DTStart);

            return base.Evaluate(FromDate, ToDate);
        }

        /// <summary>
        /// Automatically derives property values based on others it
        /// contains, to provide a more "complete" object.
        /// </summary>
        /// <param name="e"></param>        
        public override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Automatically determine Duration from Due, or Due from Duration
            if (DTStart != null)
            {
                if (Due != null && Duration == null)
                    Duration = new Duration(Due - DTStart);
                else if (Due == null && Duration != null)
                    Due = DTStart + Duration;                
            }
        }

        #endregion
    }
}
