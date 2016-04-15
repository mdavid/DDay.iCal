using System;
using System.Collections.Generic;
using System.Linq;

namespace DDay.Collections
{
    /// <summary>
    /// A list of objects that are keyed.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class GroupedList<TGroup, TItem> : IGroupedList<TGroup, TItem> where TItem : class, IGroupedObject<TGroup>
    {
        #region Protected Fields

        List<IMultiLinkedList<TItem>> _Lists = new List<IMultiLinkedList<TItem>>();
        Dictionary<TGroup, IMultiLinkedList<TItem>> _Dictionary = new Dictionary<TGroup, IMultiLinkedList<TItem>>();

        #endregion

        #region Private Methods

        TItem SubscribeToKeyChanges(TItem item)
        {
            if (item != null)
            {
                item.GroupChanged += item_GroupChanged;
            }
            return item;
        }

        TItem UnsubscribeFromKeyChanges(TItem item)
        {
            if (item != null)
            {
                item.GroupChanged -= item_GroupChanged;
            }
            return item;
        }

        #endregion

        #region Protected Methods

        protected virtual TGroup GroupModifier(TGroup group)
        {
            if (group == null)
            {
                throw new ArgumentNullException("The item's group cannot be null.");
            }

            return group;
        }

        #endregion

        #region Private Methods

        IMultiLinkedList<TItem> EnsureList(TGroup group, bool createIfNecessary)
        {
            if (!_Dictionary.ContainsKey(group))
            {
                if (createIfNecessary)
                {
                    var list = new MultiLinkedList<TItem>();
                    _Dictionary[group] = list;

                    if (_Lists.Count > 0)
                    {
                        // Attach the list to our list chain
                        var previous = _Lists[_Lists.Count - 1];
                        previous.SetNext(list);
                        list.SetPrevious(previous);
                    }

                    _Lists.Add(list);
                    return list;
                }
            }
            else
            {
                return _Dictionary[group];
            }
            return null;
        }

        IMultiLinkedList<TItem> ListForIndex(int index, out int relativeIndex)
        {
            foreach (var list in _Lists)
            {
                var startIndex = list.StartIndex;
                if (list.StartIndex <= index && list.ExclusiveEnd > index)
                {
                    relativeIndex = index - list.StartIndex;
                    return list;
                }
            }
            relativeIndex = -1;
            return null;
        }

        #endregion

        #region Event Handlers

        void item_GroupChanged(object sender, ObjectEventArgs<TGroup, TGroup> e)
        {
            var oldValue = e.First;
            var newValue = e.Second;
            var obj = sender as TItem;

            if (obj != null)
            {
                // Remove the object from the hash table
                // based on the old group.
                if (!object.Equals(oldValue, default(TGroup)))
                {
                    // Find the specific item and remove it
                    var group = GroupModifier(oldValue);
                    if (_Dictionary.ContainsKey(group))
                    {
                        var items = _Dictionary[group];

                        // Find the item's index within the list
                        var index = items.IndexOf(obj);
                        if (index >= 0)
                        {
                            // Get a reference to the object
                            var item = items[index];

                            // Remove the object
                            items.RemoveAt(index);

                            // Notify that this item was removed, with the overall
                            // index of the item in the keyed list.
                            OnItemRemoved(UnsubscribeFromKeyChanges(item), items.StartIndex + index);
                        }
                    }
                }

                // If a new group exists, then re-add this item into the hash
                if (!object.Equals(newValue, default(TGroup)))
                {
                    Add(obj);
                }
            }
        }

        #endregion

        #region IGroupedList<TGroup, TObject> Members

        [field: NonSerialized]
        public event EventHandler<ObjectEventArgs<TItem, int>> ItemAdded;

        [field: NonSerialized]
        public event EventHandler<ObjectEventArgs<TItem, int>> ItemRemoved;

        protected void OnItemAdded(TItem obj, int index)
        {
            if (ItemAdded != null)
            {
                ItemAdded(this, new ObjectEventArgs<TItem, int>(obj, index));
            }
        }

        protected void OnItemRemoved(TItem obj, int index)
        {
            if (ItemRemoved != null)
            {
                ItemRemoved(this, new ObjectEventArgs<TItem, int>(obj, index));
            }
        }

        public virtual void Add(TItem item)
        {
            if (item != null)
            {
                // Get the "real" group for this item
                var group = GroupModifier(item.Group);

                // Add a new list if necessary
                var list = EnsureList(group, true);
                var index = list.Count;
                list.Add(SubscribeToKeyChanges(item));
                OnItemAdded(item, list.StartIndex + index);
            }
        }

        public virtual int IndexOf(TItem item)
        {
            // Get the "real" group
            var group = GroupModifier(item.Group);
            if (_Dictionary.ContainsKey(group))
            {
                // Get the list associated with this object's group
                var list = _Dictionary[group];

                // Find the object within the list.
                var index = list.IndexOf(item);

                // Return the index within the overall KeyedList
                if (index >= 0)
                {
                    return list.StartIndex + index;
                }
            }
            return -1;
        }

        public virtual void Clear(TGroup group)
        {
            group = GroupModifier(group);

            if (_Dictionary.ContainsKey(group))
            {
                // Get the list associated with the group
                var list = _Dictionary[group].ToArray();

                // Save the number of items in the list
                var count = list.Length;

                // Save the starting index of the list
                var startIndex = _Dictionary[group].StartIndex;

                // Clear the list (note that this also clears the list
                // in the _Lists object).
                _Dictionary[group].Clear();

                // Notify that each of these items were removed
                for (var i = list.Length - 1; i >= 0; i--)
                {
                    OnItemRemoved(UnsubscribeFromKeyChanges(list[i]), startIndex + i);
                }
            }
        }

        public virtual void Clear()
        {
            // Get a list of items that are being cleared
            var items = _Lists.SelectMany(i => i).ToArray();

            // Clear our lists out
            _Dictionary.Clear();
            _Lists.Clear();

            // Notify that each item was removed
            for (var i = items.Length - 1; i >= 0; i--)
            {
                OnItemRemoved(UnsubscribeFromKeyChanges(items[i]), i);
            }
        }

        public virtual bool ContainsKey(TGroup group)
        {
            group = GroupModifier(group);
            return _Dictionary.ContainsKey(group);
        }

        public virtual int Count
        {
            get { return _Lists.Sum(list => list.Count); }
        }

        public virtual int CountOf(TGroup group)
        {
            group = GroupModifier(group);
            if (_Dictionary.ContainsKey(group))
            {
                return _Dictionary[group].Count;
            }
            return 0;
        }

        public virtual IEnumerable<TItem> Values()
        {
            return _Dictionary.Values.SelectMany(i => i);
        }

        public virtual IEnumerable<TItem> AllOf(TGroup group)
        {
            group = GroupModifier(group);
            if (_Dictionary.ContainsKey(group))
            {
                return _Dictionary[group];
            }
            return new TItem[0];
        }

        public virtual bool Remove(TItem obj)
        {
            var group = GroupModifier(obj.Group);
            if (_Dictionary.ContainsKey(group))
            {
                var items = _Dictionary[group];
                var index = items.IndexOf(obj);

                if (index >= 0)
                {
                    var item = items[index];
                    items.RemoveAt(index);
                    OnItemRemoved(UnsubscribeFromKeyChanges(obj), index);
                    return true;
                }
            }
            return false;
        }

        public virtual bool Remove(TGroup group)
        {
            group = GroupModifier(group);
            if (_Dictionary.ContainsKey(group))
            {
                var list = _Dictionary[group];

                for (var i = list.Count - 1; i >= 0; i--)
                {
                    var obj = list[i];
                    list.RemoveAt(i);
                    OnItemRemoved(UnsubscribeFromKeyChanges(obj), list.StartIndex + i);
                }
                return true;
            }
            return false;
        }

        public virtual void SortKeys(IComparer<TGroup> comparer = null)
        {
            var keys = _Dictionary.Keys.ToArray();

            _Lists.Clear();

            IMultiLinkedList<TItem> previous = null;
            foreach (var group in _Dictionary.Keys.OrderBy(k => k, comparer))
            {
                var list = _Dictionary[group];
                if (previous == null)
                {
                    previous = list;
                    previous.SetPrevious(null);
                }
                else
                {
                    previous.SetNext(list);
                    list.SetPrevious(previous);
                    previous = list;
                }

                _Lists.Add(list);
            }
        }

        #endregion

        #region ICollection<TObject> Members

        public virtual bool Contains(TItem item)
        {
            var group = GroupModifier(item.Group);
            if (_Dictionary.ContainsKey(group))
            {
                return _Dictionary[group].Contains(item);
            }
            return false;
        }

        public virtual void CopyTo(TItem[] array, int arrayIndex)
        {
            _Dictionary.SelectMany(kvp => kvp.Value).ToArray().CopyTo(array, arrayIndex);
        }

        public virtual bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region IList<TObject> Members

        public virtual void Insert(int index, TItem item)
        {
            int relativeIndex;
            var list = ListForIndex(index, out relativeIndex);
            if (list != null)
            {
                list.Insert(relativeIndex, item);
                OnItemAdded(item, index);
            }
        }

        public virtual void RemoveAt(int index)
        {
            int relativeIndex;
            var list = ListForIndex(index, out relativeIndex);
            if (list != null)
            {
                var item = list[relativeIndex];
                list.RemoveAt(relativeIndex);
                OnItemRemoved(item, index);
            }
        }

        public virtual TItem this[int index]
        {
            get
            {
                int relativeIndex;
                var list = ListForIndex(index, out relativeIndex);
                if (list != null)
                {
                    return list[relativeIndex];
                }
                return default(TItem);
            }
            set
            {
                int relativeIndex;
                var list = ListForIndex(index, out relativeIndex);
                if (list != null)
                {
                    // Remove the item at that index and replace it
                    var item = list[relativeIndex];
                    list.RemoveAt(relativeIndex);
                    list.Insert(relativeIndex, value);
                    OnItemRemoved(item, index);
                    OnItemAdded(item, index);
                }
            }
        }

        #endregion

        #region IEnumerable<U> Members

        public IEnumerator<TItem> GetEnumerator()
        {
            return new GroupedListEnumerator<TItem>(_Lists);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new GroupedListEnumerator<TItem>(_Lists);
        }

        #endregion
    }
}