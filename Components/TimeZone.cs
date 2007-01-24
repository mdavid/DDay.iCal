using System;
using System.Data;
using System.Configuration;
using DDay.iCal.Objects;
using DDay.iCal.DataTypes;

namespace DDay.iCal.Components
{
    public class TimeZone : ComponentBase
    {
        public TZID TZID;

        public TimeZone(iCalObject parent) : base(parent)
        {
            this.Name = "VTIMEZONE";
        }
    }
}
