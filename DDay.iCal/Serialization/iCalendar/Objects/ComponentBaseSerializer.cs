using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using DDay.iCal.Objects;
using DDay.iCal.DataTypes;

namespace DDay.iCal.Serialization.iCalendar.Objects
{
    public class ComponentBaseSerializer : iCalObjectSerializer
    {
        #region Private Fields

        private DDay.iCal.Objects.ComponentBase m_component;

        #endregion

        #region Constructors

        public ComponentBaseSerializer(DDay.iCal.Objects.ComponentBase component) : base(component)
        {
            this.m_component = component;
        }

        #endregion

        #region ISerializable Members

        public override void Serialize(Stream stream, Encoding encoding)
        {
            // Open the component
            byte[] open = encoding.GetBytes("BEGIN:" + m_component.Name + "\r\n");
            stream.Write(open, 0, open.Length);
                        
            // Get a list of fields
            List<FieldInfo> fields = new List<FieldInfo>(m_component.GetType().GetFields());
            
            // Alphabetize the list of fields we just obtained
            fields.Sort(new FieldAlphabetizer());

            // Iterate through each field and attempt to serialize it
            foreach (FieldInfo field in fields)
            {
                // Make sure the field isn't marked as "NotSerialized"
                if (field.GetCustomAttributes(typeof(NotSerialized), true).Length == 0)
                {
                    object obj = field.GetValue(m_component);
                    if (obj is iCalObject)
                    {
                        iCalObject ico = (iCalObject)obj;
                        if (ico.Name == null)
                            ico.Name = field.Name.ToUpper().Replace("_", "-");
                    }

                    // Create a serializer for the object
                    ISerializable serializer = SerializerFactory.Create(obj);

                    // Check to see if the property matches its default value.
                    // If so, we don't even need to serialize it, as it is
                    // already at its default value.                    
                    object defaultValue = null;
                    object[] dvAttrs = field.GetCustomAttributes(typeof(DefaultValueAttribute), true);
                    if (dvAttrs.Length > 0)
                        defaultValue = ((DefaultValueAttribute)dvAttrs[0]).Value;
                    
                    // To continue, the default value must either not be set,
                    // or it must not match the actual value of the item.
                    if (defaultValue == null ||
                        (serializer != null && !serializer.SerializeToString().Equals(defaultValue.ToString() + "\r\n")))
                    {
                        // FIXME: enum values cannot name themselves; we need to do it for them.
                        // For this to happen, we probably need to wrap enum values into a 
                        // class that inherits from iCalObject.
                        if (field.FieldType.IsEnum)
                        {       
                            byte[] data = encoding.GetBytes(field.Name.ToUpper().Replace("_", "-") + ":");
                            stream.Write(data, 0, data.Length);
                        }
                        
                        // Actually serialize the object
                        if (serializer != null)
                            serializer.Serialize(stream, encoding);
                    }
                }
            }

            // If any extra serialization is necessary, do it now
            base.Serialize(stream, encoding);

            // Close the component
            byte[] close = encoding.GetBytes("END:" + m_component.Name + "\r\n");
            stream.Write(close, 0, close.Length);
        }

        #endregion

        #region Helper Classes

        private class FieldAlphabetizer : IComparer<FieldInfo>
        {
            #region IComparer<FieldInfo> Members

            public int Compare(FieldInfo x, FieldInfo y)
            {
                return x.Name.CompareTo(y.Name);
            }

            #endregion
        }

        #endregion
    }
}
