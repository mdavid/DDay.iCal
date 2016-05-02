﻿using System;
using System.IO;

namespace DDay.iCal.Serialization.iCalendar
{
    public class AttendeeSerializer : StringSerializer
    {
        public override Type TargetType
        {
            get { return typeof (Attendee); }
        }

        public override string SerializeToString(object obj)
        {
            var a = obj as IAttendee;
            if (a != null && a.Value != null)
            {
                return Encode(a, a.Value.OriginalString);
            }
            return null;
        }

        public override object Deserialize(TextReader tr)
        {
            var value = tr.ReadToEnd();

            IAttendee a = null;
            try
            {
                a = CreateAndAssociate() as IAttendee;
                if (a != null)
                {
                    var uriString = Unescape(Decode(a, value));

                    // Prepend "mailto:" if necessary
                    if (!uriString.StartsWith("mailto:", StringComparison.InvariantCultureIgnoreCase))
                    {
                        uriString = "mailto:" + uriString;
                    }

                    a.Value = new Uri(uriString);
                }
            }
            catch {}

            return a;
        }
    }
}