﻿using System;
using System.IO;

namespace DDay.iCal.Serialization.iCalendar
{
    public class AttachmentSerializer : EncodableDataTypeSerializer
    {
        public override Type TargetType
        {
            get { return typeof (Attachment); }
        }

        public override string SerializeToString(object obj)
        {
            var a = obj as IAttachment;
            if (a != null)
            {
                if (a.Uri != null)
                {
                    if (a.Parameters.ContainsKey("VALUE"))
                    {
                        // Ensure no VALUE type is provided
                        a.Parameters.Remove("VALUE");
                    }

                    return Encode(a, a.Uri.OriginalString);
                }
                else if (a.Data != null)
                {
                    // Ensure the VALUE type is set to BINARY
                    a.SetValueType("BINARY");

                    // BASE64 encoding for BINARY inline attachments.
                    a.Parameters.Set("ENCODING", "BASE64");

                    return Encode(a, a.Data);
                }
            }
            return null;
        }

        public override object Deserialize(TextReader tr)
        {
            var value = tr.ReadToEnd();

            try
            {
                var a = CreateAndAssociate() as IAttachment;
                if (a != null)
                {
                    // Decode the value, if necessary
                    var data = DecodeData(a, value);

                    // Get the currently-used encoding off the encoding stack.
                    var encodingStack = GetService<IEncodingStack>();
                    a.ValueEncoding = encodingStack.Current;

                    // Get the format of the attachment
                    var valueType = a.GetValueType();
                    if (valueType == typeof (byte[]))
                    {
                        // If the VALUE type is specifically set to BINARY,
                        // then set the Data property instead.                    
                        if (value != null)
                        {
                            a.Data = data;
                        }
                        return a;
                    }

                    // The default VALUE type for attachments is URI.  So, let's
                    // grab the URI by default.
                    if (value != null)
                    {
                        var uriValue = Decode(a, value);
                        a.Uri = new Uri(uriValue);
                    }

                    return a;
                }
            }
            catch {}

            return null;
        }
    }
}