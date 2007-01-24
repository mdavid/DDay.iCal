using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DDay.iCal.DataTypes
{
    public class TZID : iCalDataType
    {
        public bool GloballyUnique = false;
        public string ID = string.Empty;

        public TZID() { }
        public TZID(string value)
            : this()
        {
            CopyFrom((TZID)Parse(value));
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
                    CopyFrom((TZID)Parse(ContentLine.Value));
            }
        }

        public override void CopyFrom(object obj)
        {
            if (obj is TZID)
            {
                TZID tzid = (TZID)obj;
                this.GloballyUnique = tzid.GloballyUnique;
                this.ID = tzid.ID;
            }
            base.CopyFrom(obj);
        }

        public override bool TryParse(string value, ref object obj)
        {
            TZID tzid = (TZID)obj;

            Match match = Regex.Match(value, @"(/)?([^\r]+)");
            if (match.Success)
            {
                if (match.Groups[1].Success)
                    tzid.GloballyUnique = true;
                tzid.ID = match.Groups[2].Value;
                return true;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is TZID)
            {
                TZID tzid = (TZID)obj;
                return (this.GloballyUnique == tzid.GloballyUnique && this.ID == tzid.ID);
            }
            else if (obj is string)
            {
                object tzid = new TZID();
                if (((TZID)tzid).TryParse(obj.ToString(), ref tzid))
                    return tzid.Equals(this);                
            }
            return false;
        }

        public override string ToString()
        {
            return (GloballyUnique ? "/" : "") + ID;
        }

        static public implicit operator TZID(string input)
        {
            return new TZID(input);
        }
    }
}
