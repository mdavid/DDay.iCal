using System;
using System.Data;
using System.Configuration;
using DDay.iCal.Objects;

namespace DDay.iCal.Components
{
    public partial class TimeZone : ComponentBase
    {
        public class Standard : TimeZoneInfo
        {
            public Standard(iCalObject parent) : base(parent) {}
        }
    }
}
