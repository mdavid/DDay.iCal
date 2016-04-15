using System.Collections.Generic;

namespace DDay.Collections
{
    public class GroupedListEnumerator<TType> :
        IEnumerator<TType>
    {
        IList<IMultiLinkedList<TType>> _Lists;
        IEnumerator<IMultiLinkedList<TType>> _ListsEnumerator;
        IEnumerator<TType> _ListEnumerator;

        public GroupedListEnumerator(IList<IMultiLinkedList<TType>> lists)
        {
            _Lists = lists;
        }

        public virtual TType Current
        {
            get
            {
                if (_ListEnumerator != null)
                    return _ListEnumerator.Current;
                return default(TType);
            }
        }

        public virtual void Dispose()
        {
            Reset();
        }

        void DisposeListEnumerator()
        {
            if (_ListEnumerator != null)
            {
                _ListEnumerator.Dispose();
                _ListEnumerator = null;
            }
        }

        object System.Collections.IEnumerator.Current
        {
            get
            {
                if (_ListEnumerator != null)
                    return _ListEnumerator.Current;
                return default(TType);
            }
        }

        private bool MoveNextList()
        {
            if (_ListsEnumerator == null)
            {
                _ListsEnumerator = _Lists.GetEnumerator();
            }

            if (_ListsEnumerator != null)
            {
                if (_ListsEnumerator.MoveNext())
                {
                    DisposeListEnumerator();
                    if (_ListsEnumerator.Current != null)
                    {
                        _ListEnumerator = _ListsEnumerator.Current.GetEnumerator();
                        return true;
                    }
                }
            }

            return false;
        }

        public virtual bool MoveNext()
        {
            if (_ListEnumerator != null)
            {
                if (_ListEnumerator.MoveNext())
                {
                    return true;
                }
                else
                {
                    DisposeListEnumerator();
                    if (MoveNextList())
                        return MoveNext();
                }
            }
            else
            {
                if (MoveNextList())
                    return MoveNext();
            }
            return false;
        }

        public virtual void Reset()
        {

            if (_ListsEnumerator != null)
            {
                _ListsEnumerator.Dispose();
                _ListsEnumerator = null;
            }
        }
    }
}
