﻿using System;
using System.IO;
using System.Text.RegularExpressions;

namespace DDay.iCal.Serialization.iCalendar
{
    public class RequestStatusSerializer : StringSerializer
    {
        public override Type TargetType
        {
            get { return typeof (RequestStatus); }
        }

        public override string SerializeToString(object obj)
        {
            try
            {
                var rs = obj as IRequestStatus;
                if (rs != null)
                {
                    // Push the object onto the serialization stack
                    SerializationContext.Push(rs);

                    try
                    {
                        var factory = GetService<ISerializerFactory>();
                        if (factory != null)
                        {
                            var serializer = factory.Build(typeof (IStatusCode), SerializationContext) as IStringSerializer;
                            if (serializer != null)
                            {
                                var value = Escape(serializer.SerializeToString(rs.StatusCode));
                                value += ";" + Escape(rs.Description);
                                if (!string.IsNullOrEmpty(rs.ExtraData))
                                {
                                    value += ";" + Escape(rs.ExtraData);
                                }

                                return Encode(rs, value);
                            }
                        }
                    }
                    finally
                    {
                        // Pop the object off the serialization stack
                        SerializationContext.Pop();
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        internal static readonly Regex _narrowRequestMatch = new Regex(@"(.*?[^\\]);(.*?[^\\]);(.+)", RegexOptions.Compiled);
        internal static readonly Regex _broadRequestMatch = new Regex(@"(.*?[^\\]);(.+)", RegexOptions.Compiled);

        public override object Deserialize(TextReader tr)
        {
            var value = tr.ReadToEnd();

            var rs = CreateAndAssociate() as IRequestStatus;
            if (rs != null)
            {
                // Decode the value as needed
                value = Decode(rs, value);

                // Push the object onto the serialization stack
                SerializationContext.Push(rs);

                try
                {
                    var factory = GetService<ISerializerFactory>();
                    if (factory != null)
                    {
                        var match = _narrowRequestMatch.Match(value);
                        if (!match.Success)
                        {
                            match = _broadRequestMatch.Match(value);
                        }

                        if (match.Success)
                        {
                            var serializer = factory.Build(typeof (IStatusCode), SerializationContext) as IStringSerializer;
                            if (serializer != null)
                            {
                                rs.StatusCode = serializer.Deserialize(new StringReader(Unescape(match.Groups[1].Value))) as IStatusCode;
                                rs.Description = Unescape(match.Groups[2].Value);
                                if (match.Groups.Count == 4)
                                {
                                    rs.ExtraData = Unescape(match.Groups[3].Value);
                                }

                                return rs;
                            }
                        }
                    }
                }
                finally
                {
                    // Pop the object off the serialization stack
                    SerializationContext.Pop();
                }
            }
            return null;
        }
    }
}