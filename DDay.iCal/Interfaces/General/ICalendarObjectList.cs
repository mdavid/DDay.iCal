﻿using DDay.Collections;

namespace DDay.iCal
{
    public interface ICalendarObjectList<TType> : IGroupedCollection<string, TType> where TType : class, ICalendarObject
    {
        TType this[int index] { get; }
    }
}