using System;
using System.Collections;
using System.Text;
using DDay.iCal.Objects;

namespace DDay.iCal.Objects
{
    public class Property : iCalObject
    {        
        private string m_value;

        public string Value
        {
            get { return m_value; }
            set { m_value = value; }
        }

        public Property(iCalObject parent) : base(parent) { }
        public Property(iCalObject parent, string name, string value) : base(parent, name)
        {
            Value = value;
            AddToParent();
        }

        protected void AddToParent()
        {
            if (Parent != null &&
                Name != null &&
                !Parent.Properties.ContainsKey(Name))
                Parent.Properties[Name] = this;
        }
    }
}
