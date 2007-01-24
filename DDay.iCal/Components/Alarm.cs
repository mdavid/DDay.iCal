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
    /// A class that represents an RFC 2445 VALARM component.
    /// </summary>
    public class Alarm : ComponentBase
    {
        #region Private Fields

        private List<AlarmOccurrence> m_Occurrences;

        #endregion

        #region Public Fields

        public AlarmAction Action;
        public URI Attach;
        public Cal_Address[] Attendee;
        public Text Description;
        public Duration Duration;
        public Integer Repeat;
        public Text Summary;
        public Trigger Trigger;        

        #endregion

        #region Public Properties

        public List<AlarmOccurrence> Occurrences
        {
            get { return m_Occurrences; }
            set { m_Occurrences = value; }
        }

        #endregion

        #region Constructors

        public Alarm(iCalObject parent)
            : base(parent)
        {            
            this.Name = "VALARM";
        }

        #endregion                

        #region Public Methods

        /// <summary>
        /// Evaluates <see cref="Alarm"/>s for the given recurring component, <paramref name="rc"/>.
        /// This evaluation is based on the evaluation period for the <see cref="RecurringComponent"/>.        
        /// </summary>
        /// <param name="rc">The </param>
        /// <returns></returns>
        virtual public List<AlarmOccurrence> Evaluate(RecurringComponent rc)
        {
            Occurrences = new List<AlarmOccurrence>();

            // If the trigger is relative, it can recur right along with
            // the recurring items, otherwise, it happens once and
            // only once (at a precise time).
            if (Trigger.IsRelative)
            {                
                foreach (Date_Time dt in rc.DateTimes)
                    Occurrences.Add(new AlarmOccurrence(this, dt + Trigger.Duration, rc));                
            }
            else Occurrences.Add(new AlarmOccurrence(this, Trigger.DateTime.Copy(), rc));

            // If a REPEAT and DURATION value were specified,
            // then handle those repetitions here.
            AddRepeatedItems();

            return Occurrences;
        }

        /// <summary>
        /// Polls the <see cref="Alarm"/> component for alarms that have been triggered
        /// since the provided <paramref name="Start"/> date/time.  If <paramref name="Start"/>
        /// is null, all triggered alarms will be returned.
        /// </summary>
        /// <param name="Start">The earliest date/time to poll trigerred alarms for.</param>
        /// <returns>A list of <see cref="AlarmOccurrence"/> objects, each containing a triggered alarm.</returns>
        public List<AlarmOccurrence> Poll(Date_Time Start)
        {
            List<AlarmOccurrence> Results = new List<AlarmOccurrence>();
            foreach (AlarmOccurrence ao in Occurrences)
            {
                if ((Start == null || ao.DateTime >= Start) && ao.DateTime <= DateTime.Now)
                    Results.Add(ao.Copy());
            }
            return Results;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Handles the repetitions that occur from the <c>REPEAT</c> and
        /// <c>DURATION</c> properties.  Each recurrence of the alarm will
        /// have its own set of generated repetitions.
        /// </summary>
        virtual protected void AddRepeatedItems()
        {
            if (Repeat != null)
            {
                int len = Occurrences.Count;
                for (int i = 0; i < len; i++)
                {
                    AlarmOccurrence ao = Occurrences[i];
                    Date_Time AlarmTime = ao.DateTime.Copy();

                    for (int j = 0; j < Repeat; j++)
                    {
                        AlarmTime += Duration;
                        Occurrences.Add(new AlarmOccurrence(this, AlarmTime.Copy(), ao.Component));
                    }
                }
            }
        }

        #endregion

        #region Helper Classes

        /// <summary>
        /// A class that represents a specific occurrence of an <see cref="Alarm"/>.        
        /// </summary>
        /// <remarks>
        /// The <see cref="AlarmOccurrence"/> contains the <see cref="Date_Time"/> when
        /// the alarm occurs, the <see cref="Alarm"/> that fired, and the 
        /// component on which the alarm fired.
        /// </remarks>
        public class AlarmOccurrence
        {
            #region Public Fields

            public Alarm Alarm;
            public Date_Time DateTime;
            public RecurringComponent Component;

            #endregion

            #region Constructors

            public AlarmOccurrence(AlarmOccurrence ao)
            {
                this.Alarm = ao.Alarm;
                this.DateTime = ao.DateTime.Copy();
                this.Component = ao.Component;
            }
            public AlarmOccurrence(Alarm a, Date_Time dt, RecurringComponent rc)
            {
                this.Alarm = a;
                this.DateTime = dt;
                this.Component = rc;
            }

            #endregion

            #region Public Methods

            public AlarmOccurrence Copy()
            {
                AlarmOccurrence ao = new AlarmOccurrence(this);
                return ao;
            }

            #endregion
        }
        
        #endregion
    }
}
