using System;

namespace DDay.iCal.Serialization
{
    public class SerializationSettings :
        ISerializationSettings
    {
        #region Private Fields

        private Type m_iCalendarType = typeof(DDay.iCal.iCalendar);        
        private bool m_EnsureAccurateLineNumbers = false;
        private ParsingModeType m_ParsingMode = ParsingModeType.Strict;
        private bool m_StoreExtraSerializationData = false;        

        #endregion

        #region ISerializationSettings Members

        public virtual Type iCalendarType
        {
            get { return m_iCalendarType; }
            set { m_iCalendarType = value; }
        }

        public virtual bool EnsureAccurateLineNumbers
        {
            get { return m_EnsureAccurateLineNumbers; }
            set { m_EnsureAccurateLineNumbers = value; }
        }

        public virtual ParsingModeType ParsingMode
        {
            get { return m_ParsingMode; }
            set { m_ParsingMode = value; }
        }

        public virtual bool StoreExtraSerializationData
        {
            get { return m_StoreExtraSerializationData; }
            set { m_StoreExtraSerializationData = value; }
        }

        #endregion
    }
}
