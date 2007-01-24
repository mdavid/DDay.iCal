using System;
using System.Data;
using System.Configuration;
using DDay.iCal.Objects;

namespace DDay.iCal.Components
{
    public class Journal : ComponentBase
    {
        public Journal(iCalObject parent) : base(parent)
        {
            this.Name = "VJOURNAL";
        }
    }
}
