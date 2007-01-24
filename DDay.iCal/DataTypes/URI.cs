using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.DataTypes
{
    /// <summary>
    /// An iCalendar URI (Universal Resource Identifier) value.
    /// </summary>
    [DebuggerDisplay("{Value}")]
    public class URI : iCalDataType
    {
        #region Private Fields

        private Uri m_Value;

        #endregion

        #region Public Properties

        public Uri Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        #endregion

        #region Constructors

        public URI() { }
        public URI(string value)
            : this()
        {
            CopyFrom(Parse(value));
        }

        #endregion

        #region Overrides

        public override bool TryParse(string value, ref object obj)
        {
            URI uri = (URI)obj;
            Uri uriValue;
            bool retVal = Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out uriValue);
            uri.Value = uriValue;
            return retVal;
        }

        public override void CopyFrom(object obj)
        {
            if (obj is URI)
            {
                URI uri = (URI)obj;
                Value = uri.Value;
            }
        }

        #endregion
    }
}
