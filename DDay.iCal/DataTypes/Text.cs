using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace DDay.iCal.DataTypes
{
    public class Text : iCalDataType
    {
        #region Private Fields

        private string m_value;

        #endregion

        #region Public Properties

        public string Value
        {
            get { return m_value; }
            set { m_value = value; }
        }

        #endregion

        #region Constructors

        public Text() { }
        public Text(string value) : this()
        {
            CopyFrom(Parse(value));
        }

        #endregion

        #region Overrides

        public override void CopyFrom(object obj)
        {
            if (obj is Text)
            {
                Text t = (Text)obj;
                this.Value = t.Value;          
            }
        }

        public override bool TryParse(string value, ref object obj)
        {
            Text t = (Text)obj;
            value = value.Replace(@"\n", "\n");
            value = value.Replace(@"\N", "\n");
            value = value.Replace(@"\;", ";");
            value = value.Replace(@"\,", ",");
            
            // Everything but backslashes has been unescaped. Validate this...
            if (Regex.IsMatch(value, @"[^\\]\\[^\\]"))
                return false;

            value = value.Replace(@"\\", @"\");
            t.Value = value;

            return true;
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
                    CopyFrom(Parse(ContentLine.Value));
            }
        }
        
        public override string ToString()
        {
            return Value;
        }

        public override string Serialize()
        {
            // Escape semicolons, commas, newlines, and backslashes
            string value = Value;
            value = value.Replace("\n", @"\n");
            value = value.Replace(";", @"\;");
            value = value.Replace(",", @"\,");
            value = value.Replace(@"\", @"\\");

            return value;
        }

        #endregion        
    }
}
