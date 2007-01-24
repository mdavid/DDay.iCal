using System;
using System.Data;
using System.Collections;
using System.Configuration;
using DDay.iCal.Objects;
using DDay.iCal.DataTypes;

namespace DDay.iCal.Components
{
    public partial class TimeZone : ComponentBase
    {
        /// <summary>
        /// A class that contains time zone information, and is usually accessed
        /// from an iCalendar object using the <see cref="DDay.iCal.iCalendar.GetTimeZone"/> method.        
        /// </summary>
        public class TimeZoneInfo : RecurringComponent
        {
            #region Public Fields

            public UTC_Offset TZOffsetFrom;
            public UTC_Offset TZOffsetTo;
            public string[] Comment;
            public string[] TZName;

            #endregion
            
            #region Constructors

            public TimeZoneInfo(iCalObject parent)
                : base(parent)
            {
            }

            #endregion

            #region Overrides

            /// <summary>
            /// Returns the name of the current Time Zone.
            /// <example>
            ///     The following are examples:
            ///     <list type="bullet">
            ///         <item>EST</item>
            ///         <item>EDT</item>
            ///         <item>MST</item>
            ///         <item>MDT</item>
            ///     </list>
            /// </example>
            /// </summary>
            public override string Name
            {
                get
                {
                    if (TZName.Length > 0)
                        return TZName[0];
                    return string.Empty;
                }
                set
                {
                    base.Name = value;
                }
            }

            #endregion
        }
    }
}
