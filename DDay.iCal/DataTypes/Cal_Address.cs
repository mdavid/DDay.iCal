using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.DataTypes
{
    /// <summary>
    /// A class that represents the address of an iCalendar user.
    /// In internet terms, this is usually represented in
    /// the following form:
    /// <c>MAILTO:email.address@host.com</c>
    /// </summary>
    [DebuggerDisplay("{Value}")]
    public class Cal_Address : URI
    {
        #region Constructors
        
        public Cal_Address() : base() { }
        public Cal_Address(string value) : base(value) { }

        #endregion
    }
}
