using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.DataTypes
{
    /// <summary>
    /// Represents in iCalendar integer
    /// </summary>
    [DebuggerDisplay("{Value}")]
    public class Integer : iCalDataType
    {
        #region Private Fields

        private int m_Value;

        #endregion

        #region Public Properties

        public int Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        #endregion

        #region Constructors

        public Integer() { }
        public Integer(string value)
            : this()
        {
            CopyFrom(Parse(value));
        }

        #endregion

        #region Overrides

        public override void CopyFrom(object obj)
        {
            if (obj is Integer)
            {
                Integer i = (Integer)obj;
                Value = i.Value;
            }
            base.CopyFrom(obj);
        }

        public override bool TryParse(string value, ref object obj)
        {   
            int i;
            bool retVal = Int32.TryParse(value, out i);
            ((Integer)obj).Value = i;
            return retVal;
        }

        #endregion

        #region Operators

        static public implicit operator int(Integer i)
        {
            return i.Value;
        }

        #endregion
    }
}
