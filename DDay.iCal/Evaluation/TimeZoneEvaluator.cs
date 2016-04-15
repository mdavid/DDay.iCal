﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace DDay.iCal
{
    public class TimeZoneEvaluator :
        Evaluator
    {
        #region Private Fields

        private HashSet<Occurrence> m_Occurrences;        

        #endregion

        #region Protected Properties

        protected ITimeZone TimeZone { get; set; }

        #endregion

        #region Public Properties

        public virtual HashSet<Occurrence> Occurrences
        {
            get { return m_Occurrences; }
            set { m_Occurrences = value; }
        }

        #endregion

        #region Constructors

        public TimeZoneEvaluator(ITimeZone tz)
        {
            TimeZone = tz;
            m_Occurrences = new HashSet<Occurrence>();
        }

        #endregion

        #region Private Methods

        void ProcessOccurrences(IDateTime referenceDate)
        {
            // Sort the occurrences by start time

            var sortedOccurrences = m_Occurrences.OrderBy(o => o.Period.StartTime).ToList();

            for (var i = 0; i < sortedOccurrences.Count; i++)
            {
                var curr = sortedOccurrences[i];
                var next = i < m_Occurrences.Count - 1 ? (Occurrence?)sortedOccurrences[i + 1] : null;

                // Determine end times for our periods, overwriting previously calculated end times.
                // This is important because we don't want to overcalculate our time zone information,
                // but simply calculate enough to be accurate.  When date/time ranges that are out of
                // normal working bounds are encountered, then occurrences are processed again, and
                // new end times are determined.
                if (next != null && next.HasValue)
                {
                    curr.Period.EndTime = next.Value.Period.StartTime.AddTicks(-1);
                }
                else
                {
                    curr.Period.EndTime = ConvertToIDateTime(EvaluationEndBounds, referenceDate);
                }
            }
        }

        #endregion

        #region Overrides

        public override void Clear()
        {
            base.Clear();
            m_Occurrences.Clear();
        }

        public override HashSet<IPeriod> Evaluate(IDateTime referenceDate, DateTime periodStart, DateTime periodEnd, bool includeReferenceDateInResults)
        {
            // Ensure the reference date is associated with the time zone
            if (referenceDate.AssociatedObject == null)
                referenceDate.AssociatedObject = TimeZone;

            var infos = new List<ITimeZoneInfo>(TimeZone.TimeZoneInfos);

            // Evaluate extra time periods, without re-evaluating ones that were already evaluated
            if ((EvaluationStartBounds == DateTime.MaxValue && EvaluationEndBounds == DateTime.MinValue) ||
                (periodEnd.Equals(EvaluationStartBounds)) ||
                (periodStart.Equals(EvaluationEndBounds)))
            {
                foreach (var curr in infos)
                {
                    var evaluator = curr.GetService(typeof(IEvaluator)) as IEvaluator;

                    // Time zones must include an effective start date/time
                    // and must provide an evaluator.
                    if (evaluator != null)
                    {
                        // Set the start bounds
                        if (EvaluationStartBounds > periodStart)
                            EvaluationStartBounds = periodStart;

                        // FIXME: 5 years is an arbitrary number, to eliminate the need
                        // to recalculate time zone information as much as possible.
                        var offsetEnd = periodEnd.AddYears(5); 

                        // Determine the UTC occurrences of the Time Zone observances
                        var periods = evaluator.Evaluate(
                            referenceDate,
                            periodStart,
                            offsetEnd,
                            includeReferenceDateInResults);

                        foreach (var period in periods)
                        {
                            if (!Periods.Contains(period))
                                Periods.Add(period);

                            var o = new Occurrence(curr, period);
                            if (!m_Occurrences.Contains(o))
                                m_Occurrences.Add(o);
                        }

                        if (EvaluationEndBounds == DateTime.MinValue || EvaluationEndBounds < offsetEnd)
                            EvaluationEndBounds = offsetEnd;
                    }
                }
                
                ProcessOccurrences(referenceDate);
            }
            else
            {
                if (EvaluationEndBounds != DateTime.MinValue && periodEnd > EvaluationEndBounds)
                    Evaluate(referenceDate, EvaluationEndBounds, periodEnd, includeReferenceDateInResults);
            }

            return Periods;
        }

        #endregion
    }
}
