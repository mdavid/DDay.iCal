﻿using System;
using System.IO;

namespace DDay.iCal.Serialization.iCalendar
{
    public class EnumSerializer : EncodableDataTypeSerializer
    {
        #region Private Fields

        Type m_EnumType;

        #endregion

        #region Constructors

        public EnumSerializer(Type enumType)
        {
            m_EnumType = enumType;
        }

        #endregion

        public override Type TargetType
        {
            get { return m_EnumType; }
        }

        public override string SerializeToString(object enumValue)
        {
            try
            {
                var obj = SerializationContext.Peek() as ICalendarObject;
                if (obj != null)
                {
                    // Encode the value as needed.
                    var dt = new EncodableDataType();
                    dt.AssociatedObject = obj;
                    return Encode(dt, enumValue.ToString());
                }
                return enumValue.ToString();
            }
            catch
            {
                return null;
            }
        }

        public override object Deserialize(TextReader tr)
        {
            var value = tr.ReadToEnd();

            try
            {
                var obj = SerializationContext.Peek() as ICalendarObject;
                if (obj != null)
                {
                    // Decode the value, if necessary!
                    var dt = new EncodableDataType();
                    dt.AssociatedObject = obj;
                    value = Decode(dt, value);
                }

                // Remove "-" characters while parsing Enum values.
                return Enum.Parse(m_EnumType, value.Replace("-", ""), true);
            }
            catch {}

            return value;
        }
    }
}