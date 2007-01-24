using System;
using System.Collections;
using System.Text;
using DDay.iCal.Components;

namespace DDay.iCal.Objects
{
    public class ComponentBase : iCalObject
    {
        public ComponentBase(iCalObject parent) : base(parent) {}
        public ComponentBase(iCalObject parent, string name) : base(parent, name) {}

        static public ComponentBase Create(iCalObject parent, string name)
        {
            switch(name.ToUpper())
            {
                case "VALARM": return new Alarm(parent); break;
                case "VEVENT": return new Event(parent); break;
                case "VFREEBUSY": return new FreeBusy(parent); break;
                case "VJOURNAL": return new Journal(parent); break;
                case "VTIMEZONE": return new DDay.iCal.Components.TimeZone(parent); break;
                case "VTODO": return new Todo(parent); break;
                default: return new ComponentBase(parent, name); break;
            }
        }
    }
}
