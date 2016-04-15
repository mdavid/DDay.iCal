﻿using System.Collections.Generic;

namespace DDay.Collections
{
    public interface IMultiLinkedList<TType> : IList<TType>
    {
        void SetPrevious(IMultiLinkedList<TType> previous);
        void SetNext(IMultiLinkedList<TType> next);
        int StartIndex { get; }
        int ExclusiveEnd { get; }
    }
}