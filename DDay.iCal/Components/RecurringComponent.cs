using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DDay.iCal
{
    /// <summary>
    /// An iCalendar component that recurs.
    /// </summary>
    /// <remarks>
    /// This component automatically handles
    /// RRULEs, RDATE, EXRULEs, and EXDATEs, as well as the DTSTART
    /// for the recurring item (all recurring items must have a DTSTART).
    /// </remarks>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class RecurringComponent : 
        UniqueComponent,
        IRecurringComponent
    {
        #region Static Public Methods

        public static IEnumerable<IRecurringComponent> SortByDate(IEnumerable<IRecurringComponent> list)
        {
            return SortByDate<IRecurringComponent>(list);
        }

        public static IEnumerable<T> SortByDate<T>(IEnumerable<T> list)
        {
            var items = new List<IRecurringComponent>();
            foreach (var t in list)
            {
                if (t is IRecurringComponent)
                    items.Add((IRecurringComponent)(object)t);
            }

            // Sort the list by date
            items.Sort(new RecurringComponentDateSorter());
            foreach (var rc in items)
                yield return (T)(object)rc;
        }

        #endregion        

        #region Protected Properties

        protected virtual bool EvaluationIncludesReferenceDate { get { return false; } }

        #endregion

        #region Public Properties

        public virtual IList<IAttachment> Attachments
        {
            get { return Properties.GetMany<IAttachment>("ATTACH"); }
            set { Properties.Set("ATTACH", value); }
        }

        public virtual IList<string> Categories
        {
            get { return Properties.GetMany<string>("CATEGORIES"); }
            set { Properties.Set("CATEGORIES", value); }
        }

        public virtual string Class
        {
            get { return Properties.Get<string>("CLASS"); }
            set { Properties.Set("CLASS", value); }
        }

        public virtual IList<string> Contacts
        {
            get { return Properties.GetMany<string>("CONTACT"); }
            set { Properties.Set("CONTACT", value); }
        }

        public virtual IDateTime Created
        {
            get { return Properties.Get<IDateTime>("CREATED"); }
            set { Properties.Set("CREATED", value); }
        }

        public virtual string Description
        {
            get { return Properties.Get<string>("DESCRIPTION"); }
            set { Properties.Set("DESCRIPTION", value); }
        }

        /// <summary>
        /// The start date/time of the component.
        /// </summary>
        public virtual IDateTime DTStart
        {
            get { return Properties.Get<IDateTime>("DTSTART"); }            
            set { Properties.Set("DTSTART", value); }
        }

        public virtual IList<IPeriodList> ExceptionDates
        {
            get { return Properties.GetMany<IPeriodList>("EXDATE"); }
            set { Properties.Set("EXDATE", value); }
        }

        public virtual IList<IRecurrencePattern> ExceptionRules
        {
            get { return Properties.GetMany<IRecurrencePattern>("EXRULE"); }
            set { Properties.Set("EXRULE", value); }
        }

        public virtual IDateTime LastModified
        {
            get { return Properties.Get<IDateTime>("LAST-MODIFIED"); }
            set { Properties.Set("LAST-MODIFIED", value); }
        }

        public virtual int Priority
        {
            get { return Properties.Get<int>("PRIORITY"); }
            set { Properties.Set("PRIORITY", value); }
        }

        public virtual IList<IPeriodList> RecurrenceDates
        {
            get { return Properties.GetMany<IPeriodList>("RDATE"); }
            set { Properties.Set("RDATE", value); }
        }

        public virtual IList<IRecurrencePattern> RecurrenceRules
        {
            get { return Properties.GetMany<IRecurrencePattern>("RRULE"); }
            set { Properties.Set("RRULE", value); }
        }

        public virtual IDateTime RecurrenceID
        {
            get { return Properties.Get<IDateTime>("RECURRENCE-ID"); }
            set { Properties.Set("RECURRENCE-ID", value); }
        }

        public virtual IList<string> RelatedComponents
        {
            get { return Properties.GetMany<string>("RELATED-TO"); }
            set { Properties.Set("RELATED-TO", value); }
        }

        public virtual int Sequence
        {
            get { return Properties.Get<int>("SEQUENCE"); }
            set { Properties.Set("SEQUENCE", value); }
        }

        /// <summary>
        /// An alias to the DTStart field (i.e. start date/time).
        /// </summary>
        public virtual IDateTime Start
        {
            get { return DTStart; }
            set { DTStart = value; }
        }

        public virtual string Summary
        {
            get { return Properties.Get<string>("SUMMARY"); }
            set { Properties.Set("SUMMARY", value); }
        }

        /// <summary>
        /// A list of <see cref="Alarm"/>s for this recurring component.
        /// </summary>
        public virtual ICalendarObjectList<IAlarm> Alarms
        {
            get { return new CalendarObjectListProxy<IAlarm>(Children); }
        }

        #endregion

        #region Constructors

        public RecurringComponent() : base()
        {
            Initialize();
            EnsureProperties();
        }

        public RecurringComponent(string name) : base(name)
        {
            Initialize();
            EnsureProperties();
        }

        private void Initialize()
        {
            SetService(new RecurringEvaluator(this));
        }

        #endregion   
     
        #region Private Methods

        private void EnsureProperties()
        {
            if (!Properties.ContainsKey("SEQUENCE"))
                Sequence = 0;
        }

        #endregion

        #region Overrides

        protected override void OnDeserializing(StreamingContext context)
        {
            base.OnDeserializing(context);

            Initialize();
        }

        #endregion

        #region IRecurringComponent Members

        public virtual void ClearEvaluation()
        {
            RecurrenceUtil.ClearEvaluation(this);
        }

        public virtual HashSet<Occurrence> GetOccurrences(IDateTime dt)
        {
            return RecurrenceUtil.GetOccurrences(this, dt, EvaluationIncludesReferenceDate);
        }

        public virtual HashSet<Occurrence> GetOccurrences(DateTime dt)
        {
            return RecurrenceUtil.GetOccurrences(this, new iCalDateTime(dt), EvaluationIncludesReferenceDate);
        }

        public virtual HashSet<Occurrence> GetOccurrences(IDateTime startTime, IDateTime endTime)
        {
            return RecurrenceUtil.GetOccurrences(this, startTime, endTime, EvaluationIncludesReferenceDate);
        }

        public virtual HashSet<Occurrence> GetOccurrences(DateTime startTime, DateTime endTime)
        {
            return RecurrenceUtil.GetOccurrences(this, new iCalDateTime(startTime), new iCalDateTime(endTime), EvaluationIncludesReferenceDate);
        }

        public virtual IList<AlarmOccurrence> PollAlarms()
        {
            return PollAlarms(null, null);
        }

        public virtual IList<AlarmOccurrence> PollAlarms(IDateTime startTime, IDateTime endTime)
        {
            var occurrences = new List<AlarmOccurrence>();
            if (Alarms != null)
            {
                foreach (var alarm in Alarms)
                    occurrences.AddRange(alarm.Poll(startTime, endTime));
            }
            return occurrences;
        }

        #endregion
    }

    /// <summary>
    /// Sorts recurring components by their start dates
    /// </summary>
    public class RecurringComponentDateSorter : IComparer<IRecurringComponent>
    {
        #region IComparer<RecurringComponent> Members

        public int Compare(IRecurringComponent x, IRecurringComponent y)
        {
            return x.Start.CompareTo(y.Start);            
        }

        #endregion
    }
}
