using System;
using System.Data;
using System.Configuration;
using DDay.iCal.Objects;
using DDay.iCal.DataTypes;

namespace DDay.iCal.Components
{
    /// <summary>
    /// A class that represents an RFC 2445 VJOURNAL component.
    /// </summary>
    public class Journal : RecurringComponent
    {
        #region Public Fields

        public Binary[] Attach;
        public Cal_Address[] Attendee;
        public TextCollection[] Categories;
        public Text Class;
        public Text[] Comment;
        public Text[] Contact;
        [DefaultValueType("DATE-TIME")]
        public Date_Time Created;
        public Text Description;
        [DefaultValueType("DATE-TIME")]
        public Date_Time DTStamp;
        [DefaultValueType("DATE-TIME")]
        public Date_Time LastModified;
        public Cal_Address Organizer;
        public Text[] Related_To;
        public RequestStatus[] RequestStatus;
        public Integer Sequence;
        public JournalStatus Status;
        public Text Summary;        
        public URI URL;

        #endregion

        #region Static Public Methods

        static public Journal Create(iCalendar iCal)
        {
            Journal j = new Journal(iCal);
            j.UID = UniqueComponent.NewUID();

            return j;
        }

        #endregion

        #region Constructors

        public Journal(iCalObject parent) : base(parent)
        {
            this.Name = "VJOURNAL";
        }

        #endregion
    }
}
