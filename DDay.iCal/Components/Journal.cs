using System;
using System.Diagnostics;
using System.Data;
using System.Configuration;
using DDay.iCal.Objects;
using DDay.iCal.DataTypes;
using DDay.iCal.Serialization;

namespace DDay.iCal.Components
{
    /// <summary>
    /// A class that represents an RFC 2445 VJOURNAL component.
    /// </summary>
    [DebuggerDisplay("{Summary}: {Description.ToString().Substring(0, 32)}")]
    public class Journal : RecurringComponent
    {
        #region Public Fields

        [Serialized]
        public Binary[] Attach;
        [Serialized]
        public Cal_Address[] Attendee;
        [Serialized]
        public TextCollection[] Categories;
        [Serialized]
        public Text Class;
        [Serialized]
        public Text[] Comment;
        [Serialized]
        public Text[] Contact;
        [Serialized, DefaultValueType("DATE-TIME")]
        public Date_Time Created;
        [Serialized]
        public Text Description;
        [Serialized]
        public Cal_Address Organizer;
        [Serialized]
        public Text[] Related_To;
        [Serialized]
        public RequestStatus[] RequestStatus;
        [Serialized]
        public JournalStatus Status;
        [Serialized]
        public Text Summary;
        [Serialized]
        public URI URL;

        #endregion

        #region Static Public Methods

        static public Journal Create(iCalendar iCal)
        {
            Journal j = (Journal)iCal.Create(iCal, "VJOURNAL");
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
