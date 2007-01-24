using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace DDay.iCal.DataTypes
{
    public class UTC_Offset : iCalDataType
    {
        public bool Positive = false;
        public int Hours;
        public int Minutes;
        public int Seconds = 0;

        public UTC_Offset() { }
        public UTC_Offset(string value)
            : this()
        {
            CopyFrom((UTC_Offset)Parse(value));            
        }

        public override void CopyFrom(object obj)
        {
            if (obj is UTC_Offset)
            {
                UTC_Offset utco = (UTC_Offset)obj;
                this.Positive = utco.Positive;
                this.Hours = utco.Hours;
                this.Minutes = utco.Minutes;
                this.Seconds = utco.Seconds;
            }
            base.CopyFrom(obj);
        }

        public override bool TryParse(string value, ref object obj)
        {
            UTC_Offset utco = (UTC_Offset)obj;
            Match match = Regex.Match(value, @"(\+|-)(\d{2})(\d{2})(\d{2})?");
            if (match.Success)
            {
                try
                {
                    if (match.Groups[0].Value == "+")
                        utco.Positive = true;
                    utco.Hours = Int32.Parse(match.Groups[2].Value);
                    utco.Minutes = Int32.Parse(match.Groups[3].Value);
                    if (match.Groups[4].Success)
                        utco.Seconds = Int32.Parse(match.Groups[4].Value);
                }
                catch
                {
                    return false;
                }
                return true;
            }

            return false;
        }

        public override DDay.iCal.Objects.ContentLine ContentLine
        {
            get
            {
                return base.ContentLine;
            }
            set
            {
                base.ContentLine = value;
                if (ContentLine != null)
                    CopyFrom((UTC_Offset)Parse(ContentLine.Value));
            }
        }
    }
}
