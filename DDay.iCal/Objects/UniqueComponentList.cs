using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DDay.iCal.Components;
using DDay.iCal.DataTypes;

namespace DDay.iCal.Objects
{
    /// <summary>
    /// A collection of iCalendar components.  This class is used by the 
    /// <see cref="iCalendar"/> class to maintain a collection of events,
    /// to-do items, journal entries, and free/busy times.
    /// </summary>
    public class UniqueComponentList<T> : iCalObject, IList<T>
    {
        #region Private Fields

        private List<T> m_Components = new List<T>();
        private Dictionary<string, T> m_Dictionary = new Dictionary<string, T>();

        #endregion

        #region Constructors

        public UniqueComponentList(iCalObject parent) : base(parent) { }
        public UniqueComponentList(iCalObject parent, string name) : base(parent, name) { }

        #endregion

        #region Public Members

        /// <summary>
        /// Re-links the UID dictionary to the actual components in our list.
        /// Also, if any items do not have a UID assigned to them, they will
        /// automatically have a UID assigned.
        /// </summary> 
        public void ResolveUIDs()
        {
            m_Dictionary.Clear();
            foreach (T item in m_Components)
            {
                if ((item as UniqueComponent).UID == null)
                    (item as UniqueComponent).UID = UniqueComponent.NewUID();                
                m_Dictionary[(item as UniqueComponent).UID.Value] = item;
            }
        }
                
        #endregion

        #region IList<T> Members

        public int IndexOf(T item)
        {
            return m_Components.IndexOf(item);
        }

        public void Insert(int index, T item)
        {   
            m_Components.Insert(index, item);
            if ((item as UniqueComponent).UID != null)
                m_Dictionary[(item as UniqueComponent).UID.Value] = item;
        }

        public void RemoveAt(int index)
        {
            T item = m_Components[index];
            if ((item as UniqueComponent).UID != null)
                m_Dictionary.Remove((item as UniqueComponent).UID.Value);
            m_Components.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {                
                return m_Components[index];                
            }
            set
            {
                T item = m_Components[index];
                if ((item as UniqueComponent).UID != null)
                {
                    m_Dictionary.Remove((item as UniqueComponent).UID.Value);
                    m_Dictionary[(value as UniqueComponent).UID.Value] = value;
                }
                m_Components[index] = value;
            }
        }

        public T this[string uid]
        {
            get
            {
                if (m_Dictionary.ContainsKey(uid))
                    return m_Dictionary[uid];
                return default(T);
            }
            set
            {                
                if (m_Dictionary.ContainsKey(uid))
                {
                    T item = m_Dictionary[uid];
                    m_Components.Remove(item);
                    m_Components.Add(value);
                    m_Dictionary[uid] = value;
                }
                else
                {
                    m_Components.Add(value);
                    m_Dictionary[uid] = value;
                }
            }
        }

        #endregion

        #region ICollection<T> Members

        public void Add(T item)
        {            
            m_Components.Add(item);
            if ((item as UniqueComponent).UID != null)
                m_Dictionary[(item as UniqueComponent).UID.Value] = item;
        }

        public bool Contains(T item)
        {
            return m_Components.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            m_Components.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return m_Components.Remove(item) && ((item as UniqueComponent).UID == null || m_Dictionary.Remove((item as UniqueComponent).UID.Value));
        }

        public void Clear()
        {
            m_Components.Clear();
            m_Dictionary.Clear();
        }

        public int Count
        {
            get { return m_Components.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return m_Components.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return m_Components.GetEnumerator();
        }

        #endregion
    }
}
