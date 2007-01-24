using System;
using System.Collections;
using System.Text;
using DDay.iCal.Components;
using DDay.iCal.DataTypes;

namespace DDay.iCal.Objects
{
    /// <summary>
    /// An iCalendar component that recurs. This component automatically handles
    /// <see cref="RRULE"/>s, <see cref="RDATE"/>s, <see cref="EXRULE"/>s, and
    /// <see cref="EXDATE"/>s, as well as the <see cref="DTSTART"/>
    /// for the recurring item (all recurring items must have a DTSTART).
    /// </summary>
    public class RecurringComponent : ComponentBase
    {
        #region Public Fields

        public Date_Time DTStart;
        public Date_Time EvalStart;
        public Date_Time EvalEnd;
        public RDate[] ExDate;
        public Recur[] ExRule;
        public RDate[] RDate;
        public Recur[] RRule;
        public Date_Time RecurID;        

        #endregion

        #region Private Fields

        private ArrayList m_DateTimes;

        #endregion

        #region Public Properties

        /// <summary>
        /// A collection of <see cref="Date_Time"/> objects that contain the dates and times
        /// when each item occurs/recurs.
        /// </summary>
        public ArrayList DateTimes
        {
            get { return m_DateTimes; }
            set { m_DateTimes = value; }
        }

        /// <summary>
        /// An alias to the DTStart field (i.e. start date/time).
        /// </summary>
        public Date_Time Start
        {
            get { return DTStart; }
            set { DTStart = value; }
        }

        #endregion

        #region Constructors

        public RecurringComponent(iCalObject parent) : base(parent) { DateTimes = new ArrayList();  }
        public RecurringComponent(iCalObject parent, string name) : base(parent, name) { DateTimes = new ArrayList(); }

        #endregion

        #region Public Overridables

        /// <summary>
        /// Evaluates this item to determine the dates and times for which it occurs/recurs.
        /// This method only evaluates items which occur/recur between <paramref name="FromDate"/>
        /// and <paramref name="ToDate"/>; therefore, if you require a list of items which
        /// occur outside of this range, you must specify a <paramref name="FromDate"/> and
        /// <paramref name="ToDate"/> which encapsulate the date(s) of interest.
        /// <note type="caution">
        ///     For events with very complex recurrence rules, this method may be a bottleneck
        ///     during processing time, especially when this method is called for a large number
        ///     of items, in sequence, or for a very large time span.
        /// </note>
        /// </summary>
        /// <param name="FromDate">The beginning date of the range to evaluate.</param>
        /// <param name="ToDate">The end date of the range to evaluate.</param>
        /// <returns>
        ///     An <see cref="ArrayList"/> containing a <see cref="Date_Time"/> object for
        ///     each date/time this item occurs/recurs.
        /// </returns>
        virtual public ArrayList Evaluate(Date_Time FromDate, Date_Time ToDate)
        {
            // Evaluate extra time periods, without re-evaluating ones that were already evaluated
            if ((EvalStart == null && EvalEnd == null) ||
                (ToDate == EvalStart) ||
                (FromDate == EvalEnd))
            {
                EvaluateRRule(FromDate, ToDate);
                EvaluateRDate(FromDate, ToDate);
                EvaluateExRule(FromDate, ToDate);
                EvaluateExDate(FromDate, ToDate);
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

        #region Protected Overridables

        /// <summary>
        /// Evaulates the RRule component, and adds each specified DateTime
        /// to the <see cref="DateTimes"/> collection.
        /// </summary>
        /// <param name="FromDate">The beginning date of the range to evaluate.</param>
        /// <param name="ToDate">The end date of the range to evaluate.</param>
        virtual protected void EvaluateRRule(Date_Time FromDate, Date_Time ToDate)
        {
            // Handle RRULEs
            if (RRule != null)
            {
                foreach (Recur rrule in RRule)
                {
                    ArrayList DateTimes = rrule.Evaluate(DTStart, FromDate, ToDate);
                    foreach (Date_Time dt in DateTimes)
                    {
                        if (!this.DateTimes.Contains(dt))
                            this.DateTimes.Add(dt);
                    }
                }
            }
        }

        /// <summary>
        /// Evalates the RDate component, and adds each specified DateTime or
        /// Period to the <see cref="DateTimes"/> collection.
        /// </summary>
        /// <param name="FromDate">The beginning date of the range to evaluate.</param>
        /// <param name="ToDate">The end date of the range to evaluate.</param>
        virtual protected void EvaluateRDate(Date_Time FromDate, Date_Time ToDate)
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

                        if (dt != null && !DateTimes.Contains(dt))
                            DateTimes.Add(dt);
                    }
                }
            }
        }

        /// <summary>
        /// Evaulates the ExRule component, and excludes each specified DateTime
        /// from the <see cref="DateTimes"/> collection.
        /// </summary>
        /// <param name="FromDate">The beginning date of the range to evaluate.</param>
        /// <param name="ToDate">The end date of the range to evaluate.</param>
        virtual protected void EvaluateExRule(Date_Time FromDate, Date_Time ToDate)
        {
            // Handle EXRULEs
            if (ExRule != null)
            {
                foreach (Recur exrule in ExRule)
                {
                    ArrayList DateTimes = exrule.Evaluate(DTStart, FromDate, ToDate);
                    foreach (Date_Time dt in DateTimes)
                    {
                        if (this.DateTimes.Contains(dt))
                            this.DateTimes.Remove(dt);
                    }
                }
            }
        }

        /// <summary>
        /// Evalates the ExDate component, and excludes each specified DateTime or
        /// Period from the <see cref="DateTimes"/> collection.
        /// </summary>
        /// <param name="FromDate">The beginning date of the range to evaluate.</param>
        /// <param name="ToDate">The end date of the range to evaluate.</param>
        virtual protected void EvaluateExDate(Date_Time FromDate, Date_Time ToDate)
        {
            // Handle EXDATEs
            if (ExDate != null)
            {
                foreach (RDate exdate in ExDate)
                {
                    ArrayList Items = exdate.Evaluate(DTStart, FromDate, ToDate);
                    foreach (object obj in Items)
                    {
                        Date_Time dt = null;
                        if (obj is Period)
                            dt = ((Period)obj).StartTime;
                        else if (obj is Date_Time)
                            dt = (Date_Time)obj;

                        if (dt != null)
                        {
                            while (DateTimes.Contains(dt))
                                DateTimes.Remove(dt);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
