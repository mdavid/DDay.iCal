﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DDay.iCal.Serialization.iCalendar
{
    public class iCalendarSerializer :
        ComponentSerializer
    {
        #region Private Fields

        IICalendar m_ICalendar;

        #endregion

        #region Constructors

        public iCalendarSerializer() : base()
        {
        }

        public iCalendarSerializer(IICalendar iCal)
        {
            m_ICalendar = iCal;
        }

        public iCalendarSerializer(ISerializationContext ctx) : base(ctx)
        {
        }

        #endregion

        #region Public Methods

        [Obsolete("Use the Serialize(IICalendar iCal, string filename) method instead.")]
        public virtual void Serialize(string filename)
        {
            if (m_ICalendar != null)
                Serialize(m_ICalendar, filename);
        }

        [Obsolete("Use the SerializeToString(ICalendarObject obj) method instead.")]
        public virtual string SerializeToString()
        {
            return SerializeToString(m_ICalendar);
        }

        public virtual void Serialize(IICalendar iCal, string filename)
        {
            using (var fs = new FileStream(filename, FileMode.Create))
            {
                Serialize(iCal, fs, new UTF8Encoding());
            }
        }        

        #endregion

        #region Overrides

        protected override IComparer<ICalendarProperty> PropertySorter
        {
            get
            {
                return new CalendarPropertySorter();
            }
        }

        public override string SerializeToString(object obj)
        {
            var iCal = obj as IICalendar;
            if (iCal != null)
            {
                // Ensure VERSION and PRODUCTID are both set,
                // as they are required by RFC5545.
                var copy = iCal.Copy<IICalendar>();
                if (string.IsNullOrEmpty(copy.Version))
                    copy.Version = CalendarVersions.v2_0;                    
                if (string.IsNullOrEmpty(copy.ProductID))
                    copy.ProductID = CalendarProductIDs.Default;

                return base.SerializeToString(copy);
            }

            return base.SerializeToString(obj);
        }

        public override object Deserialize(TextReader tr)
        {
            if (tr != null)
            {
                // Normalize the text before parsing it
                tr = TextUtil.Normalize(tr, SerializationContext);

                // Create a lexer for our text stream
                var lexer = new iCalLexer(tr);
                var parser = new iCalParser(lexer);

                // Parse the iCalendar(s)!
                var iCalendars = parser.icalendar(SerializationContext);

                // Close our text stream
                tr.Close();

                // Return the parsed iCalendar(s)
                return iCalendars;
            }
            return null;
        }

        #endregion

        private class CalendarPropertySorter :
            IComparer<ICalendarProperty>
        {
            #region IComparer<ICalendarProperty> Members

            public int Compare(ICalendarProperty x, ICalendarProperty y)
            {
                if (x == y || (x == null && y == null))
                    return 0;
                else if (x == null)
                    return -1;
                else if (y == null)
                    return 1;
                else
                {
                    // Alphabetize all properties except VERSION, which should appear first. 
                    if (string.Equals("VERSION", x.Name, StringComparison.InvariantCultureIgnoreCase))
                        return -1;
                    else if (string.Equals("VERSION", y.Name, StringComparison.InvariantCultureIgnoreCase))
                        return 1;
                    return string.Compare(x.Name, y.Name);
                }
            }

            #endregion
        }
    }
}
