using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DDay.iCal
{
    /// <summary>
    /// A class that represents an RFC 5545 VTODO component.
    /// </summary> 
    [DebuggerDisplay("{Summary} - {Status}")]
#if !SILVERLIGHT
    [Serializable]
#endif
    public class Todo : RecurringComponent, ITodo
    {
        #region Private Fields

        TodoEvaluator m_Evaluator;

        #endregion

        #region Public Properties

        /// <summary>
        /// The date/time the todo was completed.
        /// </summary>
        public virtual IDateTime Completed
        {
            get { return Properties.Get<IDateTime>("COMPLETED"); }
            set { Properties.Set("COMPLETED", value); }
        }

        /// <summary>
        /// The start date/time of the todo item.
        /// </summary>
        public override IDateTime DTStart
        {
            get { return base.DTStart; }
            set
            {
                base.DTStart = value;
                ExtrapolateTimes();
            }
        }

        /// <summary>
        /// The due date of the todo item.
        /// </summary>
        public virtual IDateTime Due
        {
            get { return Properties.Get<IDateTime>("DUE"); }
            set
            {
                Properties.Set("DUE", value);
                ExtrapolateTimes();
            }
        }

        /// <summary>
        /// The duration of the todo item.
        /// </summary>
        // NOTE: Duration is not supported by all systems,
        // (i.e. iPhone) and cannot co-exist with Due.
        // RFC 5545 states:
        //
        //      ; either 'due' or 'duration' may appear in
        //      ; a 'todoprop', but 'due' and 'duration'
        //      ; MUST NOT occur in the same 'todoprop'
        //
        // Therefore, Duration is not serialized, as Due
        // should always be extrapolated from the duration.
        public virtual TimeSpan Duration
        {
            get { return Properties.Get<TimeSpan>("DURATION"); }
            set
            {
                Properties.Set("DURATION", value);
                ExtrapolateTimes();
            }
        }

        public virtual IGeographicLocation GeographicLocation
        {
            get { return Properties.Get<IGeographicLocation>("GEO"); }
            set { Properties.Set("GEO", value); }
        }

        public virtual string Location
        {
            get { return Properties.Get<string>("LOCATION"); }
            set { Properties.Set("LOCATION", value); }
        }

        public virtual int PercentComplete
        {
            get { return Properties.Get<int>("PERCENT-COMPLETE"); }
            set { Properties.Set("PERCENT-COMPLETE", value); }
        }

        public virtual IList<string> Resources
        {
            get { return Properties.GetMany<string>("RESOURCES"); }
            set { Properties.Set("RESOURCES", value); }
        }

        /// <summary>
        /// The status of the todo item.
        /// </summary>
        public virtual TodoStatus Status
        {
            get { return Properties.Get<TodoStatus>("STATUS"); }
            set
            {
                if (Status != value)
                {
                    // Automatically set/unset the Completed time, once the
                    // component is fully loaded (When deserializing, it shouldn't
                    // automatically set the completed time just because the
                    // status was changed).
                    if (IsLoaded)
                    {
                        if (value == TodoStatus.Completed)
                        {
                            Completed = iCalDateTime.Now;
                        }
                        else
                        {
                            Completed = null;
                        }
                    }

                    Properties.Set("STATUS", value);
                }
            }
        }

        #endregion

        #region Constructors

        public Todo()
        {
            Initialize();
        }

        private void Initialize()
        {
            this.Name = Components.TODO;

            m_Evaluator = new TodoEvaluator(this);
            SetService(m_Evaluator);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Use this method to determine if a todo item has been completed.
        /// This takes into account recurrence items and the previous date
        /// of completion, if any.        
        /// <note>
        /// This method evaluates the recurrence pattern for this TODO
        /// as necessary to ensure all relevant information is taken
        /// into account to give the most accurate result possible.
        /// </note>
        /// </summary>
        /// <returns>True if the todo item has been completed</returns>
        public virtual bool IsCompleted(IDateTime currDt)
        {
            if (Status == TodoStatus.Completed)
            {
                if (Completed == null || Completed.GreaterThan(currDt))
                {
                    return true;
                }

                // Evaluate to the previous occurrence.
                m_Evaluator.EvaluateToPreviousOccurrence(Completed, currDt);

                foreach (Period p in m_Evaluator.Periods)
                {
                    if (p.StartTime.GreaterThan(Completed) && // The item has recurred after it was completed
                        currDt.GreaterThanOrEqual(p.StartTime)) // and the current date is after or on the recurrence date.
                    {
                        return false;
                    }
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
        public virtual bool IsActive(IDateTime currDt)
        {
            if (DTStart == null)
            {
                return !IsCompleted(currDt) && !IsCancelled();
            }
            else if (currDt.GreaterThanOrEqual(DTStart))
            {
                return !IsCompleted(currDt) && !IsCancelled();
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns True if the todo item was cancelled.
        /// </summary>
        /// <returns>True if the todo was cancelled, False otherwise.</returns>
        public virtual bool IsCancelled()
        {
            return Status == TodoStatus.Cancelled;
        }

        #endregion

        #region Overrides

        protected override bool EvaluationIncludesReferenceDate
        {
            get { return true; }
        }

        protected override void OnDeserializing(StreamingContext context)
        {
            base.OnDeserializing(context);

            Initialize();
        }

        #endregion

        #region Private Methods

        private void ExtrapolateTimes()
        {
            if (Due == null && DTStart != null && Duration != default(TimeSpan))
            {
                Due = DTStart.Add(Duration);
            }
            else if (Duration == default(TimeSpan) && DTStart != null && Due != null)
            {
                Duration = Due.Subtract(DTStart);
            }
            else if (DTStart == null && Duration != default(TimeSpan) && Due != null)
            {
                DTStart = Due.Subtract(Duration);
            }
        }

        #endregion
    }
}