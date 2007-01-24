using System;
using System.Collections;
using System.Text;
using DDay.iCal.Components;
using DDay.iCal.DataTypes;

namespace DDay.iCal.Objects
{
    /// <summary>
    /// This class is used by the parsing framework as a factory class
    /// for <see cref="iCalendar"/> components.  Generally, you should
    /// not need to use this class directly.
    /// </summary>
    public class ComponentBase : iCalObject
    {
        #region Constructors

        public ComponentBase(iCalObject parent) : base(parent) {}
        public ComponentBase(iCalObject parent, string name) : base(parent, name) { }

        #endregion

        #region Static Public Methods

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
                case "DAYLIGHT":
                case "STANDARD":
                    return new DDay.iCal.Components.TimeZone.TimeZoneInfo(name.ToUpper(), parent); break;
                default: return new ComponentBase(parent, name); break;
            }
        }

        #endregion
    }
}
