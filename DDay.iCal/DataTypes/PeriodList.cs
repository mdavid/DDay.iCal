using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.IO;
using DDay.iCal.Serialization.iCalendar;

namespace DDay.iCal
{
    /// <summary>
    /// An iCalendar list of recurring dates (or date exclusions)
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class PeriodList : 
        EncodableDataType,
        IPeriodList
    {
        #region Private Fields

        private IList<IPeriod> m_Periods = new List<IPeriod>();
        private string m_TZID;

        #endregion

        #region Public Properties

        public string TZID
        {
            get { return m_TZID; }
            set { m_TZID = value; }
        }

        protected IList<IPeriod> Periods
        {
            get { return m_Periods; }
            set { m_Periods = value; }
        }

        #endregion

        #region Constructors

        public PeriodList()
        {
            Initialize();
        }
        public PeriodList(string value) : this()
        {
            var serializer = new PeriodListSerializer();
            CopyFrom(serializer.Deserialize(new StringReader(value)) as ICopyable);
        }

        void Initialize()
        {
            SetService(new PeriodListEvaluator(this));
        }

        #endregion

        #region Overrides

        protected override void OnDeserializing(StreamingContext context)
        {
            base.OnDeserializing(context);

            Initialize();
        }

        public override bool Equals(object obj)
        {
            if (obj is IPeriodList)
            {
                var r = (IPeriodList)obj;

                var p1Enum = GetEnumerator();
                var p2Enum = r.GetEnumerator();

                while (p1Enum.MoveNext())
                {
                    if (!p2Enum.MoveNext())
                        return false;

                    if (!object.Equals(p1Enum.Current, p2Enum.Current))
                        return false;
                }

                if (p2Enum.MoveNext())
                    return false;

                return true;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            foreach (var p in this)
                hashCode ^= p.GetHashCode();
            return hashCode;
        }
 
        public override void CopyFrom(ICopyable obj)
        {
            base.CopyFrom(obj);
            if (obj is IPeriodList)
            {
                var rdt = (IPeriodList)obj;
                foreach (var p in rdt)
                    Add(p.Copy<IPeriod>());
            }
        }

        public override string ToString()
        {
            var serializer = new PeriodListSerializer();
            return serializer.SerializeToString(this);
        }

        #endregion

        #region Public Methods

        public List<Period> Evaluate(iCalDateTime StartDate, iCalDateTime FromDate, iCalDateTime EndDate)
        {
            var periods = new List<Period>();

            if (StartDate > FromDate)
                FromDate = StartDate;

            if (EndDate < FromDate ||
                FromDate > EndDate)
                return periods;

            foreach (Period p in Periods)
                if (!periods.Contains(p))
                    periods.Add(p);

            return periods;
        }

        #endregion

        #region IPeriodList Members

        public virtual void Add(IDateTime dt)
        {
            Periods.Add(new Period(dt));
        }

        public virtual void Remove(IDateTime dt)
        {
            Periods.Remove(new Period(dt));
        }

        public IPeriod this[int index]
        {
            get
            {
                return m_Periods[index];
            }
            set
            {
                m_Periods[index] = value;
            }
        }

        #endregion

        #region IList<IPeriod> Members

        public virtual void Add(IPeriod item)
        {
            m_Periods.Add(item);
        }

        public virtual void Clear()
        {
            m_Periods.Clear();
        }

        public bool Contains(IPeriod item)
        {
            return m_Periods.Contains(item);
        }

        public void CopyTo(IPeriod[] array, int arrayIndex)
        {
            m_Periods.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return m_Periods.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(IPeriod item)
        {
            return m_Periods.Remove(item);
        }

        public int IndexOf(IPeriod item)
        {
            return m_Periods.IndexOf(item);
        }

        public void Insert(int index, IPeriod item)
        {
            m_Periods.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            m_Periods.RemoveAt(index);
        }

        #endregion

        #region IEnumerable<IPeriod> Members

        public IEnumerator<IPeriod> GetEnumerator()
        {
            return m_Periods.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Periods.GetEnumerator();
        }

        #endregion
    }
}
