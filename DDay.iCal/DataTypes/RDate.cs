using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using DDay.iCal.Objects;

namespace DDay.iCal.DataTypes
{
    public class RDate : iCalDataType
    {   
        private ArrayList m_Items = new ArrayList();

        public ArrayList Items
        {
          get { return m_Items; }
          set { m_Items = value; }
        }

        public RDate() { }
        public RDate(string value) : this()
        {
            CopyFrom((RDate)Parse(value));
        }

        public override ContentLine ContentLine
        {
            get { return base.ContentLine; }
            set
            {
                base.ContentLine = value;
                if (ContentLine != null)
                    CopyFrom((RDate)Parse(ContentLine.Value));
            }
        }

        public override void CopyFrom(object obj)
        {
            if (obj is RDate)
            {
                RDate rdt = (RDate)obj;
                foreach (object o in rdt.Items)
                    Items.Add(o);
            }
        }

        public override bool TryParse(string value, ref object obj)
        {
            string[] values = value.Split(',');
            foreach (string v in values)
            {
                object dt = new Date_Time();
                object p = new Period();

                if (((Date_Time)dt).TryParse(v, ref dt))
                    Items.Add(dt);
                else if (((Period)p).TryParse(v, ref p))
                    Items.Add(p);
                else return false;
            }
            return true;
        }

        public ArrayList Evaluate(Date_Time StartDate, Date_Time FromDate, Date_Time EndDate)
        {
            ArrayList Periods = new ArrayList();

            if (StartDate > FromDate)
                FromDate = StartDate;

            if (EndDate < FromDate ||
                FromDate > EndDate)
                return Periods;
            
            foreach (object obj in Items)
            {
                if (obj is Date_Time)
                    Periods.Add(obj);
                else Periods.Add(obj);
            }

            return Periods;
        }
    }
}
