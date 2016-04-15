﻿using System;
using System.IO;

namespace DDay.iCal.Serialization.iCalendar
{
    public class OrganizerSerializer : StringSerializer
    {
        public override Type TargetType
        {
            get { return typeof (Organizer); }
        }

        public override string SerializeToString(object obj)
        {
            try
            {
                var o = obj as IOrganizer;
                if (o != null && o.Value != null)
                {
                    return Encode(o, Escape(o.Value.OriginalString));
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public override object Deserialize(TextReader tr)
        {
            var value = tr.ReadToEnd();

            IOrganizer o = null;
            try
            {
                o = CreateAndAssociate() as IOrganizer;
                if (o != null)
                {
                    var uriString = Unescape(Decode(o, value));

                    // Prepend "mailto:" if necessary
                    if (!uriString.StartsWith("mailto:", StringComparison.InvariantCultureIgnoreCase))
                    {
                        uriString = "mailto:" + uriString;
                    }

                    o.Value = new Uri(uriString);
                }
            }
            catch {}

            return o;
        }
    }
}