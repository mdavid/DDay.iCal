using System;
using System.Data;
using System.Configuration;
using DDay.iCal.Objects;

namespace DDay.iCal.Components
{
    public partial class TimeZone : ComponentBase
    {
        public class Daylight : TimeZoneInfo
        {
            public Daylight(iCalObject parent)
                : base(parent)
            {
                this.Name = "DAYLIGHT";
            }
        }
    }
}
