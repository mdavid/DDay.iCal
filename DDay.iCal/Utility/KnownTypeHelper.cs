using System;
using System.Collections.Generic;

namespace DDay.iCal
{
    public static class KnownTypeHelper
    {
        public static IList<Type> GetKnownTypes()
        {
            var types = new List<Type>();

            types.Add(typeof(CalendarPropertyList));
            types.Add(typeof(CalendarParameterList));

            return types;
        }
    }
}
