using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace DDay.Collections
{
    /// <summary>
    /// A proxy for a keyed list.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class GroupedCollectionProxy<TGroup, TOriginal, TNew> :
        IGroupedCollectionProxy<TGroup, TOriginal, TNew>
        where TOriginal : class, IGroupedObject<TGroup>
        where TNew : class, TOriginal
    {
        #region Private Fields

        IGroupedCollection<TGroup, TOriginal> _RealObject;
        Func<TNew, bool> _Predicate;

        #endregion

        #region Constructors

        public GroupedCollectionProxy(IGroupedCollection<TGroup, TOriginal> realObject, Func<TNew, bool> predicate = null)
        {
            _Predicate = predicate ?? new Func<TNew, bool>(o => true);
            SetProxiedObject(realObject);

            _RealObject.ItemAdded += new EventHandler<ObjectEventArgs<TOriginal, int>>(_RealObject_ItemAdded);
            _RealObject.ItemRemoved += new EventHandler<ObjectEventArgs<TOriginal, int>>(_RealObject_ItemRemoved);
        }

        #endregion

        #region Event Handlers

        void _RealObject_ItemRemoved(object sender, ObjectEventArgs<TOriginal, int> e)
        {
            if (e.First is TNew)
                OnItemRemoved((TNew)e.First, e.Second);
        }

        void _RealObject_ItemAdded(object sender, ObjectEventArgs<TOriginal, int> e)
        {
            if (e.First is TNew)
                OnItemAdded((TNew)e.First, e.Second);
        }

        #endregion

        #region IGroupedCollection Members

        public virtual event EventHandler<ObjectEventArgs<TNew, int>> ItemAdded;
        public virtual event EventHandler<ObjectEventArgs<TNew, int>> ItemRemoved;

        protected void OnItemAdded(TNew item, int index)
        {
            if (ItemAdded != null)
                ItemAdded(this, new ObjectEventArgs<TNew, int>(item, index));
        }

        protected void OnItemRemoved(TNew item, int index)
        {
            if (ItemRemoved != null)
                ItemRemoved(this, new ObjectEventArgs<TNew, int>(item, index));
        }

        public virtual bool Remove(TGroup group)
        {
            return _RealObject.Remove(group);
        }

        public virtual void Clear(TGroup group)
        {
            _RealObject.Clear(group);
        }

        public virtual bool ContainsKey(TGroup group)
        {
            return _RealObject.ContainsKey(group);            
        }

        public virtual int CountOf(TGroup group)
        {
            return _RealObject.Count(g => g.Group.GetType() == typeof (TGroup));
        }

        public virtual IEnumerable<TNew> AllOf(TGroup group)
        {
            return _RealObject
                .AllOf(group)
                .OfType<TNew>()
                .Where(_Predicate);
        }
        
        public virtual void SortKeys(IComparer<TGroup> comparer = null)
        {
            _RealObject.SortKeys(comparer);
        }

        public virtual void Add(TNew item)
        {
            _RealObject.Add(item);
        }

        public virtual void Clear()
        {
            // Only clear items of this type
            // that match the predicate.

            var items = _RealObject
                .OfType<TNew>()
                .ToArray();

            foreach (var item in items)
            {
                _RealObject.Remove(item);
            }
        }

        public virtual bool Contains(TNew item)
        {
            return _RealObject.Contains(item);
        }

        public virtual void CopyTo(TNew[] array, int arrayIndex)
        {
            var i = 0;
            foreach (var item in this)
            {
                array[arrayIndex + (i++)] = item;
            }
        }

        public virtual int Count
        {
            get 
            { 
                return _RealObject
                    .OfType<TNew>()
                    .Count(); 
            }
        }

        public virtual bool IsReadOnly
        {
            get { return false; }
        }

        public virtual bool Remove(TNew item)
        {
            return _RealObject.Remove(item);
        }

        public virtual IEnumerator<TNew> GetEnumerator()
        {
            return _RealObject
                .OfType<TNew>()
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _RealObject
                .OfType<TNew>()
                .GetEnumerator();
        }

        #endregion

        #region IGroupedCollectionProxy Members

        public IGroupedCollection<TGroup, TOriginal> RealObject
        {
            get { return _RealObject; }
        }

        public virtual void SetProxiedObject(IGroupedCollection<TGroup, TOriginal> realObject)
        {
            _RealObject = realObject;
        }

        #endregion
    }
}
