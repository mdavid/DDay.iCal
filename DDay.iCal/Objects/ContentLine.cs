using System;
using System.Collections;
using System.Text;

namespace DDay.iCal.Objects
{
    public class ContentLine : iCalObject
    {
        public Hashtable Parameters = new Hashtable();
        private string m_value;

        public string Value
        {
            get { return m_value; }
            set { m_value = value; }
        }

        public ContentLine(iCalObject parent) : base(parent) {}
    }
}
