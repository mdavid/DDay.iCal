﻿using System;
using System.Collections.Generic;

namespace DDay.iCal
{
    public abstract class Evaluator :
        IEvaluator
    {
        #region Private Fields

        private System.Globalization.Calendar m_Calendar;
        private DateTime m_EvaluationStartBounds = DateTime.MaxValue;
        private DateTime m_EvaluationEndBounds = DateTime.MinValue;
        
        private ICalendarObject m_AssociatedObject;
        private ICalendarDataType m_AssociatedDataType;

        #endregion

        #region Protected Fields

        protected HashSet<IPeriod> m_Periods;

        #endregion

        #region Constructors

        public Evaluator()
        {
            Initialize();
        }

        public Evaluator(ICalendarObject associatedObject)
        {
            m_AssociatedObject = associatedObject;

            Initialize();
        }

        public Evaluator(ICalendarDataType dataType)
        {
            m_AssociatedDataType = dataType;

            Initialize();
        }

        void Initialize()
        {
            m_Calendar = System.Globalization.CultureInfo.CurrentCulture.Calendar;
            m_Periods = new HashSet<IPeriod>();
        }

        #endregion

        #region Protected Methods

        protected IDateTime ConvertToIDateTime(DateTime dt, IDateTime referenceDate)
        {
            IDateTime newDt = new iCalDateTime(dt, referenceDate.TzId);
            newDt.AssociateWith(referenceDate);
            return newDt;
        }

        protected void IncrementDate(ref DateTime dt, IRecurrencePattern pattern, int interval)
        {
            // FIXME: use a more specific exception.
            if (interval == 0)
                throw new Exception("Cannot evaluate with an interval of zero.  Please use an interval other than zero.");

            var old = dt;
            switch (pattern.Frequency)
            {
                case FrequencyType.Secondly: dt = old.AddSeconds(interval); break;
                case FrequencyType.Minutely: dt = old.AddMinutes(interval); break;
                case FrequencyType.Hourly: dt = old.AddHours(interval); break;
                case FrequencyType.Daily: dt = old.AddDays(interval); break;
                case FrequencyType.Weekly: dt = DateUtil.AddWeeks(old, interval, pattern.FirstDayOfWeek); break;
                case FrequencyType.Monthly: dt = old.AddDays(-old.Day + 1).AddMonths(interval); break;
                case FrequencyType.Yearly: dt = old.AddDays(-old.DayOfYear + 1).AddYears(interval); break;
                // FIXME: use a more specific exception.
                default: throw new Exception("FrequencyType.NONE cannot be evaluated. Please specify a FrequencyType before evaluating the recurrence.");
            }
        }

        #endregion

        #region IEvaluator Members

        public System.Globalization.Calendar Calendar
        {
            get { return m_Calendar; }
        }

        public virtual DateTime EvaluationStartBounds
        {
            get { return m_EvaluationStartBounds; }
            set { m_EvaluationStartBounds = value; }
        }

        public virtual DateTime EvaluationEndBounds
        {
            get { return m_EvaluationEndBounds; }
            set { m_EvaluationEndBounds = value; }
        }

        public virtual ICalendarObject AssociatedObject
        {
            get
            {
                if (m_AssociatedObject != null)
                    return m_AssociatedObject;
                else if (m_AssociatedDataType != null)
                    return m_AssociatedDataType.AssociatedObject;
                else
                    return null;
            }
            protected set { m_AssociatedObject = value; }
        }

        public virtual HashSet<IPeriod> Periods
        {
            get { return m_Periods; }
        }

        public virtual void Clear()
        {
            m_EvaluationStartBounds = DateTime.MaxValue;
            m_EvaluationEndBounds = DateTime.MinValue;
            m_Periods.Clear();
        }

        public abstract HashSet<IPeriod> Evaluate(
            IDateTime referenceDate,
            DateTime periodStart,
            DateTime periodEnd,
            bool includeReferenceDateInResults);

        #endregion
    }
}
