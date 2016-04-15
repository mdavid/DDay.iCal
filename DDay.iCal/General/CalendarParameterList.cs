﻿using System;
using System.Collections.Generic;
using DDay.Collections;

namespace DDay.iCal
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public class CalendarParameterList :
        GroupedValueList<string, ICalendarParameter, CalendarParameter, string>,
        ICalendarParameterCollection
    {
        #region Private Fields

        ICalendarObject m_Parent;
        bool m_CaseInsensitive;

        #endregion

        #region Constructors

        public CalendarParameterList()
        {
        }

        public CalendarParameterList(ICalendarObject parent, bool caseInsensitive)
        {
            m_Parent = parent;
            m_CaseInsensitive = caseInsensitive;


            ItemAdded += new EventHandler<ObjectEventArgs<ICalendarParameter, int>>(OnParameterAdded);
            ItemRemoved += new EventHandler<ObjectEventArgs<ICalendarParameter, int>>(OnParameterRemoved);
        }

        #endregion

        #region Protected Methods

        protected void OnParameterRemoved(object sender, ObjectEventArgs<ICalendarParameter, int> e)
        {
            e.First.Parent = null;
        }

        protected void OnParameterAdded(object sender, ObjectEventArgs<ICalendarParameter, int> e)
        {
            e.First.Parent = m_Parent;
        }

        #endregion

        #region Overrides

        protected override string GroupModifier(string group)
        {
            if (m_CaseInsensitive && group != null)
                return group.ToUpper();
            return group;
        }

        #endregion

        #region ICalendarParameterCollection Members

        public virtual void SetParent(ICalendarObject parent)
        {
            foreach (var parameter in this)
            {
                parameter.Parent = parent;
            }
        }

        public virtual void Add(string name, string value)
        {
            Add(new CalendarParameter(name, value));
        }

        public virtual string Get(string name)
        {
            return Get<string>(name);
        }

        public virtual IList<string> GetMany(string name)
        {
            return GetMany<string>(name);
        }

        #endregion
    }
}
