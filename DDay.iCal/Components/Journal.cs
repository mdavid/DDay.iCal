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

        public URI[] Attach;
        public Cal_Address[] Attendee;
        public TextCollection[] Categories;
        public string Class;
        public Text[] Comment;
        public Text[] Contact;
        public Date_Time Created;
        public Text Description;
        public Date_Time DTStamp;
        public Date_Time LastMod;
        public Cal_Address Organizer;
        public Text[] Related_To;
        public RequestStatus[] RequestStatus;
        public Integer Sequence;
        public JournalStatus Status;
        public Text Summary;
        public string UID;
        public URI URL;

        #endregion

        #region Constructors

        public Journal(iCalObject parent) : base(parent)
        {
            this.Name = "VJOURNAL";
        }

        #endregion
    }
}
