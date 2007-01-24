using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using DDay.iCal.Objects;

namespace DDay.iCal.DataTypes
{
    /// <summary>
    /// A class that represents the address of an iCalendar user.
    /// In iCalendar terms, this is usually represented by an
    /// e-mail address, in the following form:
    /// <c>MAILTO:email.address@host.com</c>
    /// </summary>
    [DebuggerDisplay("{Value}")]
    public class Cal_Address : URI
    {
        #region Public Properties

        public Cal_Address SentBy
        {
            get
            {
                if (Parameters.ContainsKey("SENT-BY"))
                {
                    Parameter p = (Parameter)Parameters["SENT-BY"];
                    Cal_Address ca = new Cal_Address(p.Values[0].ToString());
                    return ca;
                }
                return null;
            }
        }

        public Text CommonName
        {
            get
            {
                if (Parameters.ContainsKey("CN"))
                {
                    Parameter p = (Parameter)Parameters["CN"];
                    Text cn = new Text(p.Values[0].ToString());
                    return cn;
                }
                return null;
            }
        }

        public URI DirectoryEntry
        {
            get
            {
                if (Parameters.ContainsKey("DIR"))
                {
                    Parameter p = (Parameter)Parameters["DIR"];
                    URI uri = new URI(p.Values[0].ToString());
                    return uri;
                }
                return null;
            }
        }

        #endregion

        #region Constructors

        public Cal_Address() : base() { }
        public Cal_Address(string value) : base(value) { }

        #endregion
    }
}
