using System;
using System.Collections;
using System.Text;

namespace DDay.iCal.Objects
{
    public class Parameter : iCalObject
    {
        public ArrayList Values = new ArrayList();
        
        public Parameter(iCalObject parent) : base(parent) { }
        public Parameter(iCalObject parent, string name) : base(parent, name)
        {
            AddToParent();
        }

        protected void AddToParent()
        {
            if (Parent != null &&
                Parent is ContentLine &&
                Name != null &&
                !((ContentLine)Parent).Parameters.ContainsKey(Name))
                ((ContentLine)Parent).Parameters[Name] = this;
        }
    }
}
