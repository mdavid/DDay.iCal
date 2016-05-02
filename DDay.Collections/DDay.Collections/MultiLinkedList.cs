using System.Collections.Generic;

namespace DDay.Collections
{
    public class MultiLinkedList<TType> : List<TType>, IMultiLinkedList<TType>
    {
        #region Private Fields

        IMultiLinkedList<TType> _Previous;
        IMultiLinkedList<TType> _Next;

        #endregion

        #region IMultiLinkedList<TType> Members

        public virtual void SetPrevious(IMultiLinkedList<TType> previous)
        {
            _Previous = previous;
        }

        public virtual void SetNext(IMultiLinkedList<TType> next)
        {
            _Next = next;
        }

        public virtual int StartIndex
        {
            get
            {
                return _Previous != null
                    ? _Previous.ExclusiveEnd
                    : 0;
            }
        }

        public virtual int ExclusiveEnd
        {
            get
            {
                return Count > 0
                    ? StartIndex + Count
                    : StartIndex;
            }
        }

        #endregion
    }
}