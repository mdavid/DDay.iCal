using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.DataTypes
{
    public enum Status
    {
        // Event-only status codes
        TENTATIVE, 
        CONFIRMED,        

        // Todo-only status codes
        NEEDS_ACTION,
        COMPLETED,
        IN_PROCESS,

        // Event, Todo status codes
        CANCELLED
    };
}
