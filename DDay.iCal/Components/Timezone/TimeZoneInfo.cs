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
        public class TimeZoneInfo : RecurringComponent
        {
            #region Public Fields

            public UTC_Offset TZOffsetFrom;
            public UTC_Offset TZOffsetTo;
            public string[] Comment;
            public string[] TZName;
            public Date_Time EvalStart;
            public Date_Time EvalEnd;

            #endregion
            
            #region Constructors

            public TimeZoneInfo(iCalObject parent)
                : base(parent)
            {
            }

            #endregion
        }
    }
}
