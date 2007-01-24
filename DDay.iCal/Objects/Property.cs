using System;
using System.Collections;
using System.Text;
using DDay.iCal.Objects;

namespace DDay.iCal.Objects
{
    /// <summary>
    /// A class that represents a property of the <see cref="iCalendar"/>
    /// itself.
    /// </summary>
    /// <remarks>
    /// Currently, the "known" properties for an iCalendar are as
    /// follows:
    /// <list type="bullet">
    ///     <item>ProdID</item>
    ///     <item>Version</item>
    ///     <item>CalScale</item>
    ///     <item>Method</item>
    /// </list>
    /// There may be other, custom properties applied to the calendar.
    /// </remarks>
    public class Property : iCalObject
    {
        #region Private Fields

        private string m_value;

        #endregion

        #region Public Properties

        public string Value
        {
            get { return m_value; }
            set { m_value = value; }
        }

        #endregion

        #region Constructors

        public Property(iCalObject parent) : base(parent) { }
        public Property(iCalObject parent, string name, string value) : base(parent, name)
        {
            Value = value;
            AddToParent();
        }

        #endregion

        #region Protected Methods

        protected void AddToParent()
        {
            if (Parent != null &&
                Name != null &&
                !Parent.Properties.ContainsKey(Name))
                Parent.Properties[Name] = this;
        }

        #endregion
    }
}
