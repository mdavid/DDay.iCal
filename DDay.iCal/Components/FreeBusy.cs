using System;
using System.Data;
using System.Configuration;
using DDay.iCal.Objects;

namespace DDay.iCal.Components
{
    /// <summary>
    /// <note>This class has not yet been implemented.</note>
    /// </summary>
    public class FreeBusy : ComponentBase
    {
        public FreeBusy(iCalObject parent) : base(parent)
        {
            this.Name = "VFREEBUSY";
        }
    }
}
