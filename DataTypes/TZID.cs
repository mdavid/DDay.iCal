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

        public override void CopyFrom(object obj)
        {
            if (obj is TZID)
            {
                TZID tzid = (TZID)obj;
                this.GloballyUnique = tzid.GloballyUnique;
                this.ID = tzid.ID;
            }
        }

        public override bool TryParse(string value, ref object obj)
        {
            TZID tzid = (TZID)obj;

            Match match = Regex.Match(value, @"(/)?([^\r]+)");
            if (match.Success)
            {
                if (match.Groups[1].Success)
                    GloballyUnique = true;
                ID = match.Groups[2].Value;
                return true;
            }

            return false;
        }
    }
}
