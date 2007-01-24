using System;
using System.Data;
using System.Collections;
using System.Configuration;
using DDay.iCal.Objects;
using DDay.iCal.DataTypes;

namespace DDay.iCal.Components
{
    public partial class TimeZone : ComponentBase
    {
        public class TimeZoneInfo : ComponentBase
        {
            #region Public Fields

            public Date_Time DTStart;
            public UTC_Offset TZOffsetFrom;
            public UTC_Offset TZOffsetTo;
            public string[] Comment;
            public RDate[] RDate;
            public Recur[] RRule;
            public string[] TZName;
            public Date_Time EvalStart;
            public Date_Time EvalEnd;

            #endregion

            #region Private Fields

            private ArrayList m_DateTimes = new ArrayList();

            #endregion

            #region Public Properties

            /// <summary>
            /// A collection of <see cref="Period"/> objects that represent
            /// each start and end time for which this time zone changes.
            /// This collection is usually populated from the <see cref="Evaluate"/>
            /// method.
            /// </summary>
            public ArrayList DateTimes
            {
                get { return m_DateTimes; }
                set { m_DateTimes = value; }
            }

            #endregion

            #region Constructors

            public TimeZoneInfo(iCalObject parent)
                : base(parent)
            {
            }

            #endregion

            #region Protected Methods

            /// <summary>
            /// Evaulates the RRule component, and adds each specified DateTime
            /// to the <see cref="Periods"/> collection.
            /// </summary>
            /// <param name="FromDate">The beginning date of the range to evaluate.</param>
            /// <param name="ToDate">The end date of the range to evaluate.</param>
            protected void EvaluateRRule(Date_Time FromDate, Date_Time ToDate)
            {
                // Handle RRULEs
                if (RRule != null)
                {
                    foreach (Recur rrule in RRule)
                    {
                        ArrayList DateTimes = rrule.Evaluate(DTStart, FromDate, ToDate);
                        foreach (Date_Time dt in DateTimes)
                        {                            
                            if (!this.DateTimes.Contains(dt.Value))
                                this.DateTimes.Add(dt.Value);
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
            protected void EvaluateRDate(Date_Time FromDate, Date_Time ToDate)
            {
                // Handle RDATEs
                if (RDate != null)
                {
                    foreach (RDate rdate in RDate)
                    {
                        ArrayList Items = rdate.Evaluate(DTStart, FromDate, ToDate);
                        foreach (object obj in Items)
                        {
                            Date_Time dt = null;
                            if (obj is Period)
                                dt = ((Period)obj).StartTime;
                            else if (obj is Date_Time)
                                dt = (Date_Time)obj;

                            if (dt != null && !DateTimes.Contains(dt.Value))
                                DateTimes.Add(dt.Value);
                        }
                    }
                }
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
            public ArrayList Evaluate(Date_Time FromDate, Date_Time ToDate)
            {
                // Evaluate extra time periods, without re-evaluating ones that were already evaluated
                if ((EvalStart == null && EvalEnd == null) ||
                    (ToDate == EvalStart) ||
                    (FromDate == EvalEnd))
                {
                    EvaluateRRule(FromDate, ToDate);
                    EvaluateRDate(FromDate, ToDate);
                    if (EvalStart == null || EvalStart > FromDate)
                        EvalStart = FromDate.Copy();
                    if (EvalEnd == null || EvalEnd < ToDate)
                        EvalEnd = ToDate.Copy();
                }
                
                if (EvalStart != null && FromDate < EvalStart)
                    Evaluate(FromDate, EvalStart);
                if (EvalEnd != null && ToDate > EvalEnd)
                    Evaluate(EvalEnd, ToDate);

                return DateTimes;
            }

            #endregion
        }
    }
}
