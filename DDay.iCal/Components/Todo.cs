using System;
using System.Data;
using System.Configuration;
using DDay.iCal.Objects;

namespace DDay.iCal.Components
{
    public class Todo : ComponentBase
    {
        public Todo(iCalObject parent)
            : base(parent)
        {
            this.Name = "VTODO";
        }
    }
}
