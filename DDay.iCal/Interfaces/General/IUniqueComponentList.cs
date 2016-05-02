﻿namespace DDay.iCal
{
    public interface IUniqueComponentList<TComponentType> : ICalendarObjectList<TComponentType> where TComponentType : class, IUniqueComponent
    {
        TComponentType this[string uid] { get; set; }
    }
}