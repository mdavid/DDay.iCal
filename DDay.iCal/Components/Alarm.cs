using System;
using System.Data;
using System.Configuration;
using DDay.iCal.Objects;

namespace DDay.iCal.Components
{
    public class Alarm : ComponentBase
    {
        public Alarm(iCalObject parent)
            : base(parent)
        {
            this.Name = "VALARM";
        }
    }
}
